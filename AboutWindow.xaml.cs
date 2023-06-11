using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace IPTV_Checker_2
{
    public partial class AboutWindow : Window
    {
        //private readonly string url = "https://raw.githubusercontent.com/LaneSh4d0w/iptv_checker/main/VERSION.txt";
        public static string currentVersion = "2.6";

        public AboutWindow()
        {
            InitializeComponent();
            //_ = CheckForUpdatesAsync();
            CheckForUpdatesAsync();
        }


        public void CheckForUpdatesAsync()
        {
            // public async Task CheckForUpdatesAsync()
            TB_UpdateVerif.Text = "modify by edata@2023-06-11";
            TB_Version.Text = "群晖专用 IPTV Checker v2.6 ";
            TB_Version2.Text = "修改自 https://github.com/BellezaEmporium/iptv_checker";
            //using HttpClient client = new HttpClient();
            //            string actualversion = await client.GetStringAsync(url);
            //actualversion = actualversion.Replace("\n", "").Replace("\r", "");

            //             if (true) //actualversion == currentVersion)
            //             {
            //                 TB_UpdateVerif.Text = "You're up to date.";
            //                 TB_Version.Text = "IPTV Checker version " + currentVersion;
            //             }
            //             else
            //             {
            //                 TB_UpdateVerif.Text = "A new update has been released !\nVersion " + actualversion + " has been released.";
            //                 TB_Version.Text = "IPTV Checker version " + currentVersion;
            //             }
        }
    }
}