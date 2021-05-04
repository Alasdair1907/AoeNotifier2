using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace AoeNotifier
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            StreamResourceInfo info = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Help.html"));
            WebBrowserHelp.NavigateToStream(info.Stream);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
