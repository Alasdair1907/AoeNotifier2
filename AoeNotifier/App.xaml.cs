using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AoeNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unknown error occured:\r\n" + e.Exception.Message + "\r\n" + e.Exception.StackTrace +
                "\r\n-------------\r\n" + "If the problems persist, you can " +
                "try resetting the application by opening %APPDATA% in your file browser and deleting AoeNotifier_Data folder. This will delete all your filters.");
            e.Handled = true;
        }
    }
}
