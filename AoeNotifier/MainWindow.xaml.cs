using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using AoeNotifier.Engine;
using AoeNotifier.Model;
using AoeNotifier.Model.AoeNet;
using AoeNotifier.Util;
using Microsoft.Toolkit.Uwp.Notifications;
using Colors = AoeNotifier.Model.Colors;
using Matching = AoeNotifier.Engine.Matching;

namespace AoeNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private static readonly string startMonitoringButtonText = "Start Monitoring";
        private static readonly string stopMonitoringButtonText = "Stop Monitoring";

        private static readonly string statusInactive = "INACTIVE";
        private static readonly string statusActive = "ACTIVE";

        private static readonly int lobbyNamesInNotification = 4;
        private static readonly int refreshInterval = 25;

        private static readonly string aoeLinkPrefix = "aoe2de://0/";

        Filters filters;

        #region GuiGettersSetters
        List<DisplayLobby> _LobbySource;
        public List<DisplayLobby> LobbySource
        {
            get { return _LobbySource;  }
            set 
            {
                _LobbySource = value;
                OnPropertyChanged();
            }
        }

        private bool _ButtonsEnabled = true;
        public bool ButtonsEnabled
        {
            get { return _ButtonsEnabled; }
            set
            {
                _ButtonsEnabled = value;
                OnPropertyChanged();

            }
        }

        private bool _RefreshEnabled = true;
        public bool RefreshEnabled
        {
            get { return _RefreshEnabled; }
            set
            {
                _RefreshEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _StartStopEnabled = true;
        public bool StartStopEnabled
        {
            get { return _StartStopEnabled; }
            set
            {
                _StartStopEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _ResetNotificationsEnabled = true;
        public bool ResetNotificationsEnabled
        {
            get { return _ResetNotificationsEnabled; }
            set
            {
                _ResetNotificationsEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _StartStopText = startMonitoringButtonText;
        public string StartStopText
        {
            get { return _StartStopText;  }
            set
            {
                _StartStopText = value;
                OnPropertyChanged();
            }
        }

        private string _CurrentStatus = "INACTIVE";

        public string CurrentStatus
        {
            get { return _CurrentStatus;  }
            set
            {
                _CurrentStatus = value;
                OnPropertyChanged();
            }
        }

        private string _LastRefresh = "--";
        public string LastRefresh
        {
            get { return _LastRefresh; }
            set
            {
                _LastRefresh = value;
                OnPropertyChanged();
            }
        }

        private string _TotalLobbies = "--";
        public string TotalLobbies
        {
            get { return _TotalLobbies; }
            set
            {
                _TotalLobbies = value;
                OnPropertyChanged();
            }
        }

        private string _TotalPlayers = "--";
        public string TotalPlayers
        {
            get { return _TotalPlayers;  }
            set
            {
                _TotalPlayers = value;
                OnPropertyChanged();
            }
        }

        private string _MatchingLobbies = "--";
        public string MatchingLobbies
        {
            get { return _MatchingLobbies;  }
            set
            {
                _MatchingLobbies = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly BackgroundWorker refreshNowWorker;
        private readonly BackgroundWorker monitoringWorker;

        private object refreshLock = new object();
        //private volatile bool isRefreshInProgress = false;

        private DateTime? lastRefresh = null;
        private HashSet<string> notifiedLobbies;
        private List<string> errors = new List<string>();

        private ErrorLog errorLogWindow;
        private Help helpWindow;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            filters = Storage.LoadFilters();
            ButtonsEnabled = true;

            InitializeComponent();
            lvFilters.ItemsSource = filters.FilterList;
            this.DataContext = this;

            refreshNowWorker = new BackgroundWorker();
            refreshNowWorker.DoWork += delegate
            {
                bool buttonsEnabledPrevious = ButtonsEnabled;
                
                ButtonsEnabled = false;
                StartStopEnabled = false;
                RefreshEnabled = false;
                ResetNotificationsEnabled = false;

                lock (refreshLock)
                {
                    LoadResult loadResult = Main.LoadLobbies(filters.FilterList);

                    if (loadResult.ErrorMessage != null)
                    {
                        MessageBox.Show(loadResult.ErrorMessage);
                    }

                    if (loadResult.LobbyStats != null)
                    {
                        TotalLobbies = loadResult.LobbyStats.TotalLobbies.ToString();
                        TotalPlayers = loadResult.LobbyStats.TotalPlayers.ToString();
                        MatchingLobbies = loadResult.LobbyStats.MatchingLobbies.ToString();
                    }

                    LobbySource = loadResult.DisplayLobbies;
                }

                if (!ButtonsEnabled)
                {
                    ButtonsEnabled = buttonsEnabledPrevious;
                }

                StartStopEnabled = true;
                RefreshEnabled = true;
                ResetNotificationsEnabled = true;
            };



            monitoringWorker = new BackgroundWorker();
            monitoringWorker.WorkerSupportsCancellation = true;
            monitoringWorker.DoWork += delegate
            {
                ButtonsEnabled = false;

                StartStopText = stopMonitoringButtonText;
                CurrentStatus = statusActive;

                while (!monitoringWorker.CancellationPending)
                {
                    while (lastRefresh != null)
                    {
                        if (monitoringWorker.CancellationPending)
                        {
                            return;
                        }

                        int diffSeconds = (int)((TimeSpan)(DateTime.Now - lastRefresh)).TotalSeconds;

                        if (!string.Equals(LastRefresh, diffSeconds.ToString())){
                            LastRefresh = diffSeconds.ToString();
                        }

                        if (diffSeconds >= refreshInterval)
                        {
                            break;
                        }

                        Thread.Sleep(250);
                    }

                    RefreshEnabled = false;
                    StartStopEnabled = false;
                    ResetNotificationsEnabled = false;


                    lock (refreshLock)
                    {
                        LoadResult loadResult = Main.LoadLobbies(filters.FilterList);

                        if (!string.IsNullOrEmpty(loadResult.ErrorMessage))
                        {
                            errors.Add("[" + DateTime.Now.ToString() + "] " + loadResult.ErrorMessage + "\r\n-------------\r\n");
                            MatchingLobbies = "ERROR";
                        }
                        else if (loadResult.LobbyStats != null)
                        {
                            TotalLobbies = loadResult.LobbyStats.TotalLobbies.ToString();
                            TotalPlayers = loadResult.LobbyStats.TotalPlayers.ToString();
                            MatchingLobbies = loadResult.LobbyStats.MatchingLobbies.ToString();
                        }

                        LobbySource = loadResult.DisplayLobbies;

                        handleNotifications(loadResult.DisplayLobbies);
                    }


                    lastRefresh = DateTime.Now;
                    LastRefresh = "0";

                    RefreshEnabled = true;
                    StartStopEnabled = true;
                    ResetNotificationsEnabled = true;

                }
            };

            monitoringWorker.RunWorkerCompleted += delegate
            {
                ButtonsEnabled = true;
                StartStopText = startMonitoringButtonText;
                CurrentStatus = statusInactive;
                LastRefresh = "--";
                lastRefresh = null;
            };
        }

        private void handleNotifications(List<DisplayLobby> displayLobbies)
        {
            if (displayLobbies == null)
            {
                return;
            }

            if (notifiedLobbies == null)
            {
                notifiedLobbies = new HashSet<string>();
            }

            List<DisplayLobby> newLobbies = displayLobbies.Where(dL => !string.IsNullOrEmpty(dL.LobbyID) && !string.IsNullOrEmpty(dL.LobbyName) && !notifiedLobbies.Contains(dL.LobbyID)).ToList();

            if (newLobbies.Count == 0)
            {
                return;
            }

            notifiedLobbies.UnionWith((from dL in newLobbies select dL.LobbyID).ToHashSet());

            List<string> newNames = (from dL in newLobbies select dL.LobbyName).ToList();
            string title = newNames.Count + " New Lobbies";
            string lobbies = string.Join("; ", newNames.Take(lobbyNamesInNotification).ToList());

            ToastContentBuilder tcb = new ToastContentBuilder();
            tcb.AddAppLogoOverride(new Uri(Storage.GetLogoPath())).
                AddText(title).AddText(lobbies).Show();
        }

        private void DeleteSelectedFilters_Click(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                List<int> idsToDelete = getSelectedFilterIds();

                if (!idsToDelete.Any())
                {
                    return;
                }

                filters.DeleteByIds(idsToDelete);
                this.lvFilters.Items.Refresh();

                Storage.SaveFilters(filters);
            }
        }

        private void SaveFilter_Click(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                Filter newFilter = GetNewFilter();

                if (string.IsNullOrWhiteSpace(newFilter.Text))
                {
                    MessageBox.Show("Enter search text to create a filter");
                    return;
                }

                filters.AddFilter(newFilter);
                this.lvFilters.Items.Refresh();

                Storage.SaveFilters(filters);
            }
            
        }

        private void JoinSelectedFilters_Click(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                List<int> idsToJoin = getSelectedFilterIds();

                if (idsToJoin.Count < 2)
                {
                    MessageBox.Show("Select at least two notification filters to join");
                    return;
                }

                filters.JoinByIds(idsToJoin);
                this.lvFilters.SelectedItem = null;
                this.lvFilters.Items.Refresh();

                Storage.SaveFilters(filters);
            }
        }

        private void StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(StartStopText, startMonitoringButtonText)){

                if (!ensureFiltersNotEmpty())
                {
                    return;
                }

                monitoringWorker.RunWorkerAsync();
            } else
            {
                monitoringWorker.CancelAsync();
            }
        }

        private void RefreshNow_Click(object sender, RoutedEventArgs e)
        {
            if (!ensureFiltersNotEmpty())
            {
                return;
            }

            refreshNowWorker.RunWorkerAsync();
        }

        private bool ensureFiltersNotEmpty()
        {
            if (filters.FilterList == null || filters.FilterList.Count == 0)
            {
                MessageBox.Show("Create at least 1 filter to search for lobbies");
                return false;
            }

            return true;
        }
        private void Join_Click(object sender, RoutedEventArgs e)
        {
            string lobbyId = (string) ((Button)sender).Tag;
            joinLobby(lobbyId);
        }

        private void CopyID_Click(object sender, RoutedEventArgs e)
        {
            string lobbyId = (string)((Button)sender).Tag;
            Clipboard.SetText(aoeLinkPrefix + lobbyId);
        }

        private void joinLobby(string lobbyId)
        {
            var psi = new ProcessStartInfo
            {
                FileName = aoeLinkPrefix + lobbyId,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private List<int> getSelectedFilterIds()
        {
            List<int> selectedIds = new List<int>();
            for (int x = 0; x < lvFilters.SelectedItems.Count; x++)
            {
                selectedIds.Add(((Filter)lvFilters.SelectedItems[x]).id);
            }

            return selectedIds;
        }

        private Filter GetNewFilter()
        {
            Filter filter = new Filter()
            {
                filterType = GetSelectedFilterType(),
                predicateType = GetSelectedPredicateType(),
                textMode = GetSelectedTextMode(),
                filterMode = GetSelectedFilterMode(),
                Text = searchText.Text
            };

            return filter;
        }

        private FilterType GetSelectedFilterType()
        {

            if ((bool)this.radioPlayerName.IsChecked)
            {
                return FilterType.PlayerName;
            }

            return FilterType.LobbyTitle;
        }

        private PredicateType GetSelectedPredicateType()
        {
            if ((bool)this.radioFullMatch.IsChecked)
            {
                return PredicateType.FullMatch;
            } else if ((bool)this.radioAllWords.IsChecked)
            {
                return PredicateType.AllWords;
            }

            return PredicateType.Contains;
        }

        private TextMode GetSelectedTextMode()
        {
            if ((bool)this.radioCaseSensitive.IsChecked)
            {
                return TextMode.CaseSensitive;
            }

            return TextMode.CaseInsensitive;
        }

        private FilterMode GetSelectedFilterMode()
        {
            if ((bool)this.ignoreMatchingLobbies.IsChecked)
            {
                return FilterMode.Ignore;
            }

            return FilterMode.Notify;
        }

        private void ErrorLogButton_Click(object sender, RoutedEventArgs e)
        {
            string errorStr = string.Join("\r\n", errors);
            if (errorLogWindow != null)
            {
                errorLogWindow.Close();
            }

            errorLogWindow = new ErrorLog(errorStr);
            errorLogWindow.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (errorLogWindow != null)
            {
                errorLogWindow.Close();
            }

            if (helpWindow != null)
            {
                helpWindow.Close();
            }
        }

        private void ResetNotificationsButton_Click(object sender, RoutedEventArgs e)
        {
            lock (refreshLock)
            {
                notifiedLobbies.Clear();
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            if (helpWindow != null)
            {
                helpWindow.Close();
            }

            helpWindow = new Help();
            helpWindow.Show();
        }
    }
}
