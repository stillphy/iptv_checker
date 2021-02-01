namespace IPTV_Checker_2.Entities
{
    public class IPAPI
        {
            public string Status { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public string Region { get; set; }
            public string RegionName { get; set; }
            public string City { get; set; }
            public string Zip { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
            public string ISP { get; set; }
            public string Org { get; set; }
            public string AS { get; set; }
            public string Query { get; set; }
        }
}
