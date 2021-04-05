using System.Net.Http;
using System.Threading.Tasks;
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
            _ = CheckForUpdatesAsync();
        }


        public async Task CheckForUpdatesAsync()
        {
            using HttpClient client = new HttpClient();
            var actualversion = await client.GetStringAsync(url);

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