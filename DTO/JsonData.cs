using System.Collections.Generic;

namespace IPTV_Checker_2.DTO
{
    public class JsonData
    {
        public string Country
        {
            get;
            set;
        }

        public string UserAgent
        {
            get;
            set;
        }

        public List<JsonChannel> Channels
        {
            get;
            set;
        }

        public JsonData()
        {
            Channels = new List<JsonChannel>();
        }
    }
}
