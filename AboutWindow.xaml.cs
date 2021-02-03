using System.Net;
using System.Windows;

namespace IPTV_Checker_2
{
    public partial class AboutWindow : Window
    {
        private readonly string url = "https://raw.githubusercontent.com/LaneSh4d0w/iptv_checker/main/VERSION.txt";
        public static string currentVersion = "2.6";

        public AboutWindow()
        {
            InitializeComponent();
            CheckForUpdates();
        }


        public void CheckForUpdates()
        {
            string actualversion = new WebClient().DownloadString(url);

            if (actualversion == currentVersion)
            {
                TB_UpdateVerif.Text = "You're up to date.";
                TB_Version.Text = "IPTV Checker " + currentVersion;
            }
            else
            {
                TB_UpdateVerif.Text = "A new update has been released !\nVersion " + actualversion + " has been released.";
                TB_Version.Text = "IPTV Checker " + currentVersion;
            }
        }
    }
}