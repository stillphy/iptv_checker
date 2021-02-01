using System;

namespace IPTV_Checker_2.DTO
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
}
