using Microsoft.Win32;
using System;
using System.IO;

namespace IPTV_Checker_2
{
    public class RegistryStore
    {
        private static string AppNameRegistry = "HKEY_CURRENT_USER\\Software\\IPTV_Checker";

        public string UserAgent
        {
            get;
            set;
        } = "Mozilla/5.0 (Windows NT 6.2; Win64; x64;) Gecko/20100101 Firefox/20.0";


        public string Vlc_Location
        {
            get;
            set;
        } = string.Empty;


        public int Timeout
        {
            get;
            set;
        } = 5;


        public int NumThreads
        {
            get;
            set;
        } = 5;


        public int NumTries
        {
            get;
            set;
        } = 5;


        public string LastDir
        {
            get;
            set;
        } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


        public string GetFromRegistry(string name)
        {
            try
            {
                return Registry.GetValue(AppNameRegistry, name, string.Empty).ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void SetRegistry(string name, string value)
        {
            Registry.SetValue(AppNameRegistry, name, value);
        }

        public RegistryStore()
        {
            Vlc_Location = Find_VLC();
            if (GetFromRegistry("User_Agent") != string.Empty)
            {
                UserAgent = GetFromRegistry("User_Agent");
            }
            if (GetFromRegistry("VLC_Location") != string.Empty)
            {
                Vlc_Location = GetFromRegistry("VLC_Location");
            }
            if (GetFromRegistry("Timeout") != string.Empty)
            {
                Timeout = Convert.ToInt32(GetFromRegistry("Timeout"));
            }
            if (GetFromRegistry("NumTries") != string.Empty)
            {
                NumTries = Convert.ToInt32(GetFromRegistry("NumTries"));
            }
            if (GetFromRegistry("NumThreads") != string.Empty)
            {
                NumThreads = Convert.ToInt32(GetFromRegistry("NumThreads"));
            }
            if (GetFromRegistry("LastDir") != string.Empty)
            {
                LastDir = GetFromRegistry("LastDir");
            }
        }

        private string Find_VLC()
        {
            if (File.Exists("C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe"))
            {
                return "C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe";
            }
            return string.Empty;
        }

        public void SaveToRegistry()
        {
            SetRegistry("User_Agent", UserAgent.Trim());
            SetRegistry("VLC_Location", Vlc_Location.Trim());
            SetRegistry("Timeout", Timeout.ToString());
            SetRegistry("NumTries", NumTries.ToString());
            SetRegistry("NumThreads", NumThreads.ToString());
            SetRegistry("LastDir", LastDir.ToLower().Trim());
        }
    }
}
