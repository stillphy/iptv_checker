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
        {

        }

        public async Task<string> GetAllChannelsInM3u8(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                return await client.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {
                core.StatusBarText = "Nothing found in server..";
                return null;
            }
        }

        public async void GetServerStatus(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(url);

                ServerStatus serverStatus = JsonConvert.DeserializeObject<ServerStatus>(response);

                string creationTime = getTimeStampToDateTime(serverStatus.user_info.created_at);
                string expireTime = getTimeStampToDateTime(serverStatus.user_info.exp_date);

                ServerStatusWindow sswindow = new ServerStatusWindow(serverStatus, creationTime, expireTime);
                core.StatusBarText = "Finished getting server information";
                core.IsBusy = false;
                sswindow.ShowDialog();
            }
            catch (HttpRequestException e)
            {
                core.StatusBarText = "An error has occured while getting server information : " + e.Message;
            }
        }

        public string getTimeStampToDateTime(long timestamp)
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
