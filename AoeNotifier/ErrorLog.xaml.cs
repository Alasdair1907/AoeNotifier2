using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AoeNotifier
{
    /// <summary>
    /// Interaction logic for ErrorLog.xaml
    /// </summary>
    public partial class ErrorLog : Window
    {


        private string _ErrorLogText;
        public string ErrorLogText
        {
            get { return _ErrorLogText; }
            set
            {
                _ErrorLogText = value;
            }
        }
        public ErrorLog(string errorLogText)
        {
            InitializeComponent();
            ErrorLogText = errorLogText;
            this.DataContext = this;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
