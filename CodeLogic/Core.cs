using IPTV_Checker_2.DTO;
using IPTV_Checker_2.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IPTV_Checker_2
{
    public class Core : INotifyPropertyChanged
    {
        private static Core instance = null;

        private static readonly object padlock = new object();

        private CancellationTokenSource cancellationToken;

        private bool cancel_request;

        private int Timeout = 5;

        private int _online_count;

        private int _offline_count;

        private string _checked;

        private int _channelsCount;

        private int _checked_Percentage;

        private bool caseSensitiveSearch;

        private CheckStatus _checkstatus;

        private string statusBarText;

        private bool isBusy;

        private int unknown_count;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public bool CaseSensitiveSearch
        {
            get => caseSensitiveSearch;
            set
            {
                caseSensitiveSearch = value;
                OnPropertyChanged(nameof(CaseSensitiveSearch));
            }
        }

        public string StatusBarText
        {
            get => statusBarText;
            set
            {
                statusBarText = value;
                OnPropertyChanged(nameof(StatusBarText));
            }
        }

        public int CheckedPercentage
        {
            get => _checked_Percentage;
            set
            {
                _checked_Percentage = value;
                OnPropertyChanged(nameof(CheckedPercentage));
            }
        }

        public CheckStatus CheckStatus
        {
            get => _checkstatus;
            set
            {
                _checkstatus = value;                
                OnPropertyChanged(nameof(CheckStatus));
            }
        }

        public ObservableCollection<Channel> Channels
        {
            get;
            set;
        }

        public List<Channel> Channel_Full
        {
            get;
            set;
        }

        public int ChannelsCount
        {
            get => _channelsCount;
            set
            {
                _channelsCount = value;
                OnPropertyChanged(nameof(ChannelsCount));
            }
        }

        public int Online_count
        {
            get => _online_count;
            set
            {
                _online_count = value;
                OnPropertyChanged(nameof(Online_count));
            }
        }

        public int Offline_count
        {
            get => _offline_count;
            set
            {
                _offline_count = value;
                OnPropertyChanged(nameof(Offline_count));
            }
        }

        public string Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        public string UserAgent
        {
            get;
            set;
        }

        public string Vlc_Location
        {
            get;
            set;
        }

        public int TimeOut
        {
            get;
            set;
        }

        public int NumThreads
        {
            get;
            set;
        }

        public int NumTries
        {
            get;
            set;
        }

        public string LastDir
        {
            get;
            set;
        }

        public string Country
        {
            get;
            set;
        }

        public static Core Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Core
                        {
                            Channel_Full = new List<Channel>(),
                            UserAgent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64;) Gecko/20100101 Firefox/20.0",
                            Channels = new ObservableCollection<Channel>(),
                            Checked = "∞",
                            StatusBarText = "准备..",
                            Country = ""
                        };
                    }
                    return instance;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private Core()
        {
        }

        // uses the IP-API service to get a IP's country location.
        public async Task<string> GetCountryAsync(string url)
        {
            try
            {
                Uri myUri = new Uri(url);
                string host = myUri.Host;
                HttpClient client = new HttpClient();
                var data = await client.GetStringAsync("http://ip-api.com/json/" + host);
                JsonData somedata = JsonConvert.DeserializeObject<JsonData>(data);
                isBusy = false;
                if (somedata.Country != "")
                {
                    return somedata.Country;
                } else
                {
                    return "未知";
                }
                
            }
            catch (WebException e)
            {
                isBusy = false;
                throw e;
            }
        }

        // parses a m3u8 file to add channels.
        public List<Channel> ParseM3u8(string str, SpecificLinkTypes linktype)
        {
            statusBarText = "导入频道数..";
            List<Channel> list;
            list = new List<Channel>();
            if (linktype == SpecificLinkTypes.NO && str.ToUpper().Contains("#EXTINF"))
            {
                foreach (Match item in new Regex("#([^#]*)", RegexOptions.Singleline).Matches(str))
                {
                    string text, text2, text3;
                    text = item.Value.Trim();
                    text2 = Regex.Match(text, "#.*").Value.Trim();
                    text3 = text.Replace(text2, "").Trim();
                    text2 = text2.Substring(text2.LastIndexOf(",") + 1);
                    string tagValue, tagValue2;
                    tagValue = GetTagValue("group-title", text);
                    tagValue2 = GetTagValue("tvg-logo", text);
                    
                    if (text3.Trim() != string.Empty)
                    {
                        list.Add(new Channel
                        {
                            URL = text3,
                            Name = text2?.Trim(),
                            GroupTag = tagValue,
                            TvgLogo = tagValue2,
                        });
                    }
                }
                return list;
            }
            else if (linktype == SpecificLinkTypes.IPTV_ORG && str.ToUpper().Contains("#EXTINF"))
            {
                foreach (Match item in new Regex("#([^#]*)", RegexOptions.Singleline).Matches(str))
                {
                    if (!item.Value.Trim().Contains("[Not 24/7]") && !item.Value.Trim().Contains("[Geo-blocked]"))
                    {
                        string text, text2, text3;
                        text = item.Value.Trim();
                        text2 = Regex.Match(text, "#.*").Value.Trim();
                        text3 = text.Replace(text2, "").Trim();
                        text2 = text2.Substring(text2.LastIndexOf(",") + 1);
                        string tagValue, tagValue2;
                        tagValue = GetTagValue("group-title", text);
                        tagValue2 = GetTagValue("tvg-logo", text);

                        if (text3.Trim() != string.Empty)
                        {
                            list.Add(new Channel
                            {
                                URL = text3,
                                Name = text2?.Trim(),
                                GroupTag = tagValue,
                                TvgLogo = tagValue2,
                            });
                        }
                    }
                   
                }
                return list;
            }
            foreach (Match item2 in new Regex("(rtp|rtsp|rtmp|http|https):\\/\\/(\\w+:{0,1}\\w*@)?(\\S+)(:[0-9]+)?(\\/|\\/([\\w#!:.?+=&%@!\\-\\/]))?").Matches(str))
            {
                list.Add(new Channel
                {
                    URL = item2.Value.Trim(),
                    Name = $"未知数量{++unknown_count}"
                });
            }
            return list;
        }

        public void Add_channels(List<Channel> channels)
        {
            if (channels == null)
            {
                StatusBarText = "没有频道被导入";
                return;
            }
            int numberofchannels = channels.Count;
            channels = channels.Where((Channel w) => w != null).ToList();
            List<Channel> list = channels.Except(Channel_Full).ToList();
            int num = channels.Count - list.Count;
            Channel_Full.AddRange(list);
            StatusBarText = (num != 0) ? $"发现 {channels.Count}, 已导入 {list.Count} 个频道, 并且跳过 {num} 重复." : $"已导入 {list.Count()} 个频道.";
        }

        private string GetTagValue(string tag, string str)
        {
            Regex regex;
            regex = new Regex(tag + ".*?\".*?\"", RegexOptions.IgnoreCase);
            string result = "";
            if (regex.IsMatch(str))
            {
                result = Regex.Match(regex.Match(str).Value.Trim(), "\".*?\"").Value.Replace("\"", "").Trim();
            }
            return result;
        }

        public void Reset()
        {
            Channels.Clear();
            Channel_Full.Clear();
            unknown_count = 0;
            Online_count = 0;
            ChannelsCount = 0;
            Offline_count = 0;
            Checked = "∞";
            CheckedPercentage = 0;
        }

        public async Task StarChecking()
        {
            cancellationToken = new CancellationTokenSource();
            ParallelOptions parallelOptions;
            parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = NumThreads,
                CancellationToken = cancellationToken.Token
            };
            List<Channel> sourceTobeChecked;
            sourceTobeChecked = Channels.Where((Channel w) => w.Status == Status.Unchecked).ToList();
            cancel_request = false;
            try
            {
                await Task.Run(delegate
                {
                    try
                    {
                        Parallel.ForEach(sourceTobeChecked, parallelOptions, delegate (Channel channel)
                        {
                            try
                            {
                                channel.Status = CheckURL(channel.URL);
                                Online_count = sourceTobeChecked.Count((Channel w) => w.Status == Status.Online);
                                Offline_count = sourceTobeChecked.Count((Channel w) => w.Status == Status.Offline);
                                Checked = sourceTobeChecked.Count((Channel w) => w.Status == Status.Online || w.Status == Status.Offline) + "/" + sourceTobeChecked.Count;
                                CheckedPercentage = sourceTobeChecked.Count((Channel w) => w.Status == Status.Online || w.Status == Status.Offline) * 100 / sourceTobeChecked.Count;
                                cancellationToken.Token.ThrowIfCancellationRequested();
                            }
                            catch
                            {
                            }
                        });
                    }
                    catch
                    {
                    }
                }, cancellationToken.Token);
            }
            catch
            {
            }
        }

        private Status CheckURL(string URL)
        {
            for (int i = 1; i <= NumTries; i++)
            {
                if (cancel_request)
                {
                    continue;
                }
                try
                {
                    HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(URL);
                    ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                    obj.Timeout = 1000 * Timeout;
                    obj.Method = "GET";
                    obj.ContentType = "application/x-www-form-urlencoded";
                    obj.KeepAlive = true;
                    obj.UserAgent = UserAgent;
                    obj.AllowAutoRedirect = true;
                    using WebResponse webResponse = obj.GetResponse();
                    return webResponse.ContentType.Contains("application/x-mpegURL") ||
                        webResponse.ContentType.Contains("application/x-mpegurl") ||
                        webResponse.ContentType.Contains("video/mp2t") ||
                        webResponse.ContentType.Contains("application/vnd.apple.mpegurl") ||
                        webResponse.ContentType.Contains("application/octet-stream")
                        ? Status.Online
                        : Status.Offline;
                }
                catch (HttpRequestException)
                {
                }
            }
            Online_count = Channels.Count((Channel w) => w.Status == Status.Online);
            Offline_count = Channels.Count((Channel w) => w.Status == Status.Offline);
            Checked = Channels.Count((Channel w) => w.Status == Status.Online || w.Status == Status.Offline) + "/" + Channels.Count;
            return Status.Offline;
        }

        public void Stop()
        {
            cancel_request = true;
            cancellationToken.Cancel();
        }

        public void Save()
        {
            SaveFileDialog saveFileDialog;
            saveFileDialog = new SaveFileDialog
            {
                Filter = "Playlist 文件 (*.m3u, *.m3u8) | *.m3u, *.m3u8;",
                FileName = "",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != string.Empty)
            {
                string fileName;
                fileName = saveFileDialog.FileName;
                StringBuilder stringBuilder;
                stringBuilder = new StringBuilder();
                //stringBuilder.AppendLine("#EXTM3U");
                foreach (Channel channel in Channels)
                {
//                     string text = "";
//                     text = (channel.GroupTag != "") ? (text + "group-title=\"" + channel.GroupTag + "\"") : text;
//                     text = (channel.TvgLogo != "") ? (text + " tvg-logo=\"" + channel.TvgLogo + "\"") : text;
//                     text = (channel.TvgName != "") ? (text + " tvg-name=\"" + channel.TvgName + "\"") : text;
//                     text = (channel.TvgShift != "") ? (text + " tvg-shift=\"" + channel.TvgShift + "\"") : text;
//                     text = (channel.AspectRatio != "") ? (text + " aspect-ratio=\"" + channel.AspectRatio + "\"") : text;
//                     stringBuilder.AppendLine("#EXTINF:-1 " + ((channel.Url_EPG != "") ? (text + " url-epg=\"" + channel.Url_EPG + "\"") : text) + "," + channel.Name);
                    stringBuilder.AppendLine(channel.Name + "," + channel.URL);
                }
                File.WriteAllText(fileName, stringBuilder.ToString().Trim());
                MessageBox.Show("文件成功保存", "文件保存", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("文件保存被取消", "文件保存", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public void Save2()
        {
            SaveFileDialog saveFileDialog;
            saveFileDialog = new SaveFileDialog
            {
                Filter = "Playlist 文件 (*.m3u, *.m3u8) | *.m3u, *.m3u8;",
                FileName = "",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != string.Empty)
            {
                string fileName;
                fileName = saveFileDialog.FileName;
                StringBuilder stringBuilder;
                stringBuilder = new StringBuilder();
                //stringBuilder.AppendLine("#EXTM3U");
                foreach (Channel channel in Channels)
                {
                    string text = "";
                    text = (channel.GroupTag != "") ? (text + "group-title=\"" + channel.GroupTag + "\"") : text;
                    text = (channel.TvgLogo != "") ? (text + " tvg-logo=\"" + channel.TvgLogo + "\"") : text;
                    text = (channel.TvgName != "") ? (text + " tvg-name=\"" + channel.TvgName + "\"") : text;
                    text = (channel.TvgShift != "") ? (text + " tvg-shift=\"" + channel.TvgShift + "\"") : text;
                    text = (channel.AspectRatio != "") ? (text + " aspect-ratio=\"" + channel.AspectRatio + "\"") : text;
                    stringBuilder.AppendLine("#EXTINF:-1 " + ((channel.Url_EPG != "") ? (text + " url-epg=\"" + channel.Url_EPG + "\"") : text) + "," + channel.Name);
                    stringBuilder.AppendLine(channel.URL);
                }
                File.WriteAllText(fileName, stringBuilder.ToString().Trim());
                MessageBox.Show("文件保存成功", "文件保存", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("文件保存被取消", "文件保存", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
