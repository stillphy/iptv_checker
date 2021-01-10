using System.Windows;

namespace IPTV_Checker_2
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create the startup window
            MainWindow wnd = new MainWindow
            {
                // Do stuff here, e.g. to the window
                Title = "IPTV Checker 2.6"
            };
            // Show the window
            wnd.Show();
        }
    }
}