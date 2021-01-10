using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPTV_Checker_2.Models
{
    public class XpanelData
    {
        public class ServerStatus
        {
            public int CurrentConnections
            {
                get;
                set;
            }

            public int MaxConnections
            {
                get;
                set;
            }

            public DateTime ExpirationDate
            {
                get;
                set;
            }

            public DateTime CreationDate
            {
                get;
                set;
            }
        }

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

        public XpanelData(string url)
        {
            GenerateData(url);
        }

        private void GenerateData(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                MatchCollection urlmatches = Regex.Matches(url, "^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(?([^#]*))?(#(.*))?", RegexOptions.IgnoreCase);
                Host = urlmatches[4].Value;
                Userdata = urlmatches[6].Value;
                AllChannelsUrl = Host + "get.php" + Userdata + "&type=m3u_plus&output=mpegts";
                ServerInfoUrl = Host + "panel_api.php" + Userdata;
            }
        }

        public async Task<string> GetAllChannelsInM3u8()
        {
            core.StatusBarText = "Contacting server..";
            try
            {
                using HttpClient client = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
                });
                using HttpResponseMessage httpMessage = await client.GetAsync(AllChannelsUrl);
                return await httpMessage.Content.ReadAsStringAsync();
            }
            catch
            {
                core.StatusBarText = "Nothing found in server..";
                return null;
            }
        }

        public List<ServerStatus> GetServerStatus()
        {
            List<ServerStatus> serverStatus = new List<ServerStatus>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpWebRequest request = WebRequest.CreateHttp(Url);
                    request.Method = "GET";
                    if (request.HaveResponse)
                    {
                        byte[] g;
                        g = new byte[20000];
                        using Stream stream = request.GetRequestStream();
                        stream.Read(g, 0, g.Length);
                        string @string;
                        @string = Encoding.UTF8.GetString(g);
                        ExtractFromPartialJson(@string, "exp_date");
                        serverStatus.Add(new ServerStatus
                        {
                            CurrentConnections = Convert.ToInt32(ExtractFromPartialJson(@string, "active_cons")),
                            MaxConnections = Convert.ToInt32(ExtractFromPartialJson(@string, "max_connections")),
                            CreationDate = UnixTimeStampToDateTime(Convert.ToDouble(ExtractFromPartialJson(@string, "created_at"))),
                            ExpirationDate = UnixTimeStampToDateTime(Convert.ToDouble(ExtractFromPartialJson(@string, "exp_date")))
                        });
                        core.StatusBarText = "Finished getting server information";
                    }
                }
                return serverStatus;
            }
            catch
            {
                core.StatusBarText = "Failed to get server information..";
                return serverStatus;
            }
        }

        private string ExtractFromPartialJson(string json, string tag)
        {
            Regex regex = new Regex("\"" + tag.ToLower() + "\":\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!regex.IsMatch(json.ToLower()))
            {
                return string.Empty;
            }
            return regex.Match(json.ToLower()).Groups[1].Value;
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }
}
