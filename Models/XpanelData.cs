using IPTV_Checker_2.DTO;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IPTV_Checker_2.Models
{
    public class XpanelData
    {

        private Core core = Core.Instance;

        public string Url
        {
            get;
            set;
        }

        public string Userdata
        {
            get;
            set;
        }

        public string Host
        {
            get;
            set;
        }

        public string AllChannelsUrl
        {
            get;
            set;
        }

        public string ServerInfoUrl
        {
            get;
            set;
        }

        public XpanelData()
        {}

        public async Task<string> GetAllChannelsInM3u8(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                return await client.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {
                core.StatusBarText = "在服务器中找不到任何内容。";
                return null;
            }
        }

        public async void GetServerStatus(string url)
        {
            if (url.Contains("get.php?")) {
                url = url.Replace("get.php", "player_api.php");
            }
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(url);

                ServerStatus serverStatus = JsonConvert.DeserializeObject<ServerStatus>(response);

                serverStatus.user_info.created_at = GetTimeStampToDateTime(Convert.ToInt32(serverStatus.user_info.created_at));
                serverStatus.user_info.exp_date = GetTimeStampToDateTime(Convert.ToInt32(serverStatus.user_info.exp_date));

                ServerStatusWindow sswindow = new ServerStatusWindow(serverStatus);
                core.StatusBarText = "已完成获取服务器信息";
                core.IsBusy = false;
                sswindow.ShowDialog();
            }
            catch (HttpRequestException e)
            {
                core.StatusBarText = "获取服务器信息时出错 : " + e.Message;
                core.IsBusy = false;
            }
        }

        public string GetTimeStampToDateTime(long timestamp)
        {
            // Some conversions
            // First make a System.DateTime equivalent to the UNIX Epoch.
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the number of seconds in UNIX timestamp to be converted.
            dateTime = dateTime.AddSeconds(timestamp);

            // The dateTime now contains the right date/time so to format the string,
            // use the standard formatting methods of the DateTime object.
            string thetime = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();

            return thetime;
        }
    }
}
