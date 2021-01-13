using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace IPTV_Checker_2
{
    public class Updater
    {
        private string url = "https://github.com/LaneSh4d0w/iptv_checker/releases";

        public static string currentVersion = "2.6";

        public Updater()
        {
            Task.Run(delegate
            {
                check_updates();
            });
        }

        private void check_updates()
        {
            try
            {
                JObject obj;
                obj = JObject.Parse(new WebClient().DownloadString(url));
                string a;
                a = obj.SelectToken("last_version").ToString();
                string messageBoxText;
                messageBoxText = obj.SelectToken("message").ToString();
                string fileName;
                fileName = obj.SelectToken("url").ToString();
                bool flag;
                flag = Convert.ToBoolean(obj.SelectToken("is_must"));
                if (a != currentVersion)
                {
                    MessageBox.Show(messageBoxText, "IPTV Checker " + currentVersion, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Process.Start(fileName);
                    if (flag)
                    {
                        Environment.Exit(0);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
