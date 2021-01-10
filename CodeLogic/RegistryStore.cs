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
                return Registry.GetValue(RegistryStore.AppNameRegistry, name, string.Empty).ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void SetRegistry(string name, string value)
        {
            try
            {
                Registry.SetValue(RegistryStore.AppNameRegistry, name, value);
            }
            catch
            {
            }
        }

        public RegistryStore()
        {
            Vlc_Location = Find_VLC();
            string fromRegistry;
            fromRegistry = GetFromRegistry("User_Agent");
            if (fromRegistry != string.Empty)
            {
                UserAgent = fromRegistry;
            }
            string fromRegistry2;
            fromRegistry2 = GetFromRegistry("VLC_Location");
            if (fromRegistry2 != string.Empty)
            {
                Vlc_Location = fromRegistry2;
            }
            string fromRegistry3;
            fromRegistry3 = GetFromRegistry("Timeout");
            if (fromRegistry3 != string.Empty)
            {
                Timeout = int.Parse(fromRegistry3);
            }
            string fromRegistry4;
            fromRegistry4 = GetFromRegistry("NumTries");
            if (fromRegistry4 != string.Empty)
            {
                NumTries = int.Parse(fromRegistry4);
            }
            string fromRegistry5;
            fromRegistry5 = GetFromRegistry("NumThreads");
            if (fromRegistry5 != string.Empty)
            {
                NumThreads = int.Parse(fromRegistry5);
            }
            string fromRegistry6;
            fromRegistry6 = GetFromRegistry("LastDir");
            if (fromRegistry6 != string.Empty)
            {
                LastDir = fromRegistry6;
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
