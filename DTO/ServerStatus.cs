using System;
using System.Collections.Generic;

namespace IPTV_Checker_2.DTO
{
    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public string message { get; set; }
        public int auth { get; set; }
        public string status { get; set; }
        public string exp_date { get; set; }
        public string is_trial { get; set; }
        public int active_cons { get; set; }
        public string created_at { get; set; }
        public int max_connections { get; set; }
        public List<string> allowed_output_formats { get; set; }
    }

    public class ServerInfo
    {
        public string url { get; set; }
        public int port { get; set; }
        public int https_port { get; set; }
        public string server_protocol { get; set; }
        public int rtmp_port { get; set; }
        public string timezone { get; set; }
        public int timestamp_now { get; set; }
        public string time_now { get; set; }
    }

    public class ServerStatus
    {
        public UserInfo user_info { get; set; }
        public ServerInfo server_info { get; set; }
    }
}
