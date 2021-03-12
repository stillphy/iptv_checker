using System;
using System.ComponentModel;

namespace IPTV_Checker_2.Models
{
    public class Channel : INotifyPropertyChanged
    {
        private string _name;

        private string _server;

        private Status _status;

        private string _url;

        public string Country { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get;
            set;
        }

        public bool Sent
        {
            get;
            set;
        }

        public string GroupTag
        {
            get;
            set;
        } = "";


        public string TvgLogo
        {
            get;
            set;
        } = "";

        public string TvgName
        {
            get;
            set;
        } = "";

        public string TvgShift
        {
            get;
            set;
        } = "";

        public string AspectRatio
        {
            get;
            set;
        } = "";

        public string Url_EPG
        {
            get;
            set;
        } = "";

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string URL
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged("URL");
                _server = GetServer(value);
                OnPropertyChanged("Name");
            }
        }

        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                OnPropertyChanged("Server");
            }
        }

        public Status Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public DateTime DateTime
        {
            get;
            set;
        }

        public Channel()
        {
            Server = string.Empty;
            URL = string.Empty;
            Name = string.Empty;
            Status = Status.Unchecked;
            DateTime = DateTime.Now;
        }

        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private string GetServer(string url)
        {
            if (url != string.Empty && url != null)
            {
                Uri uri = new Uri(url);

                return uri.Authority;
            }
            else
            {
                return string.Empty;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Channel))
            {
                return false;
            }
            Channel channel;
            channel = obj as Channel;
            if (channel == null)
            {
                return false;
            }
            if (URL.ToLower() == channel.URL.ToLower())
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return URL.GetHashCode();
        }
    }
}
