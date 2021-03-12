using IPTV_Checker_2.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace IPTV_Checker_2
{
    public partial class MainWindow : Window
    {
        private string bindingpath = "";

        private Core core = Core.Instance;

        private ListSortDirection sortDirection;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = core;
            Loaded += MainWindow_Loaded;
            datagrid.Sorting += Datagrid_Sorting;
            Title += AboutWindow.currentVersion;
            LoadFromRegistry();
            btn_reset.IsEnabled = false;
        }

        private void Btn_about_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void Btn_add_m3u8_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog;
            openFileDialog = new OpenFileDialog
            {
                FileName = "",
                Filter = "M3u files (*.m3u8; *.m3u) | *.m3u8; *.m3u",
                InitialDirectory = core.LastDir
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != string.Empty)
            {
                string str = File.ReadAllText(openFileDialog.FileName);
                List<Channel> channels;
                channels = core.ParseM3u8(str);
                core.Add_channels(channels);
                string directoryName = Path.GetDirectoryName(openFileDialog.FileName);
                core.LastDir = directoryName;
                RegistryStore.SetRegistry("LastDir", directoryName);
            }
            txt_search.Text = string.Empty;
            radio_all.IsChecked = true;
            FilterResults();
            if (core.Channel_Full.Count > 0)
            {
                btn_reset.IsEnabled = true;
            }
            else
            {
                btn_reset.IsEnabled = false;
            }
        }

        private void Btn_add_text_Click(object sender, RoutedEventArgs e)
        {
            AddTextWindow addTextWindow;
            addTextWindow = new AddTextWindow();
            addTextWindow.ShowDialog();
            if (addTextWindow.str.Length != 0)
            {
                int count = core.Channel_Full.Count;
                List<Channel> channels;
                channels = core.ParseM3u8(addTextWindow.str);
                core.Add_channels(channels);
                txt_search.Text = string.Empty;
                radio_all.IsChecked = true;
                FilterResults();
                MessageBox.Show($"Loaded {core.Channel_Full.Count - count} channels into the list.", "IPTV Checker", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            if (core.Channel_Full.Count > 0)
            {
                btn_reset.IsEnabled = true;
            }
            else
            {
                btn_reset.IsEnabled = false;
            }
        }

        private void Btn_caseSensitive_MouseDown(object sender, MouseButtonEventArgs e)
        {
            core.CaseSensitiveSearch = !core.CaseSensitiveSearch;
            if (!string.IsNullOrWhiteSpace(txt_search.Text))
            {
                FilterResults();
            }
        }

        private async void Btn_check_Click(object sender, RoutedEventArgs e)
        {
            btn_reset.IsEnabled = false;
            core.CheckStatus = CheckStatus.Checking;
            await core.StarChecking();
            if (core.CheckStatus == CheckStatus.Stopping)
            {
                core.CheckStatus = CheckStatus.Stopped;
            }
            else
            {
                core.CheckStatus = CheckStatus.Finished;
            }
            btn_reset.IsEnabled = true;
            MessageBox.Show("Finished checking links...", "Links Checking", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void Btn_down_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedIndexes;
            selectedIndexes = GetSelectedIndexes();
            int num;
            num = selectedIndexes[selectedIndexes.Count - 1];
            selectedIndexes.Reverse();
            if (num == core.Channels.Count - 1)
            {
                return;
            }
            foreach (int item in selectedIndexes)
            {
                core.Channels.Move(item, item + 1);
            }
        }

        private void Btn_reset_Click(object sender, RoutedEventArgs e)
        {
            sortDirection = ListSortDirection.Ascending;
            core.Reset();
            btn_reset.IsEnabled = false;
            core.StatusBarText = "Ready..";
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            core.Save();
        }

        private void Btn_settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
            LoadFromRegistry();
        }

        private void Btn_stop_Click(object sender, RoutedEventArgs e)
        {
            core.CheckStatus = CheckStatus.Stopping;
            core.Stop();
        }

        private void Btn_up_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedIndexes;
            selectedIndexes = GetSelectedIndexes();
            if (selectedIndexes[0] == 0)
            {
                return;
            }
            foreach (int item in selectedIndexes)
            {
                core.Channels.Move(item, item - 1);
            }
        }

        private void Check_group_Checked(object sender, RoutedEventArgs e)
        {
            if (groupColumn != null)
            {
                groupColumn.Visibility = Visibility.Visible;
            }
        }

        private void Check_group_Unchecked(object sender, RoutedEventArgs e)
        {
            if (groupColumn != null)
            {
                groupColumn.Visibility = Visibility.Collapsed;
            }
        }

        private void Check_logo_Checked(object sender, RoutedEventArgs e)
        {
            if (logoColumn != null)
            {
                logoColumn.Visibility = Visibility.Visible;
            }
        }

        private void Check_logo_Unchecked(object sender, RoutedEventArgs e)
        {
            if (logoColumn != null)
            {
                logoColumn.Visibility = Visibility.Collapsed;
            }
        }

        private void Check_server_Checked(object sender, RoutedEventArgs e)
        {
            if (serverColumn != null)
            {
                serverColumn.Visibility = Visibility.Visible;
            }
        }

        private void Check_server_Unchecked(object sender, RoutedEventArgs e)
        {
            if (serverColumn != null)
            {
                serverColumn.Visibility = Visibility.Collapsed;
            }
        }

        private void Datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Play_in_VLC();
        }

        private void Datagrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            bindingpath = ((Binding)e.Column.ClipboardContentBinding).Path.Path;
            new Channel();
            if (e.Column.SortDirection.HasValue)
            {
                if (e.Column.SortDirection.Value == ListSortDirection.Ascending)
                {
                    sortDirection = ListSortDirection.Descending;
                    Sort();
                }
                else
                {
                    sortDirection = ListSortDirection.Ascending;
                    Sort();
                }
            }
            else
            {
                sortDirection = ListSortDirection.Ascending;
                Sort();
            }
        }

        private void FilterResults()
        {
            Status status;
            status = Status.All;
            if (radio_all.IsChecked == true)
            {
                status = Status.All;
            }
            if (radio_offline.IsChecked == true)
            {
                status = Status.Offline;
            }
            if (radio_online.IsChecked == true)
            {
                status = Status.Online;
            }
            if (radio_unchecked.IsChecked == true)
            {
                status = Status.Unchecked;
            }
            List<Channel> list;
            list = ((status == Status.All) ? core.Channel_Full : core.Channel_Full.Where((Channel w) => w.Status == status).ToList());
            if (bindingpath != "")
            {
                if (sortDirection == ListSortDirection.Ascending)
                {
                    list = list.OrderBy((Channel w) => OrderSource(w, bindingpath)).ToList();
                }
                if (sortDirection == ListSortDirection.Descending)
                {
                    list = list.OrderByDescending((Channel w) => OrderSource(w, bindingpath)).ToList();
                }
            }
            if (txt_search.Text.Trim() == string.Empty)
            {
                core.Channels.Clear();
                foreach (Channel item in list)
                {
                    core.Channels.Add(item);
                }
            }
            else
            {
                core.Channels.Clear();
                foreach (Channel item2 in (!core.CaseSensitiveSearch) ? list.Where((Channel w) => w.Name.ToLower().Contains(txt_search.Text.ToLower())) : list.Where((Channel w) => w.Name.Contains(txt_search.Text)))
                {
                    core.Channels.Add(item2);
                }
            }
            core.ChannelsCount = core.Channels.Count;
        }

        private List<int> GetSelectedIndexes()
        {
            List<int> list;
            list = new List<int>();
            for (int i = 0; i < datagrid.Items.Count; i++)
            {
                if (datagrid.SelectedItems.Contains(datagrid.Items[i]))
                {
                    list.Add(i);
                }
            }
            return list;
        }

        private void LoadFromRegistry()
        {
            RegistryStore registryStore;
            registryStore = new RegistryStore();
            core.Vlc_Location = registryStore.Vlc_Location;
            core.UserAgent = registryStore.UserAgent;
            core.NumTries = registryStore.NumTries;
            core.TimeOut = registryStore.Timeout;
            core.NumThreads = registryStore.NumThreads;
            core.LastDir = registryStore.LastDir;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 500;
        }

        private void Menu_copy_list_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedIndexes = GetSelectedIndexes();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (int item in selectedIndexes)
            {
                stringBuilder.AppendLine("#EXTINF:-1," + core.Channels[item].Name);
                stringBuilder.AppendLine(core.Channels[item].URL);
            }
            Clipboard.SetText(stringBuilder.ToString().Trim());
            MessageBox.Show("Channels have been copied to clipboard.", "Copy Channels as", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void Menu_copy_url_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<int> selectedIndexes = GetSelectedIndexes();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (int item in selectedIndexes)
                {
                    stringBuilder.AppendLine(core.Channels[item].URL);
                }
                Clipboard.SetText(stringBuilder.ToString().Trim());
                MessageBox.Show("Links have been copied to clipboard.", "Copy Channels", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
            }
        }

        private void Menu_delete_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            foreach (Channel selectedItem in datagrid.SelectedItems)
            {
                list.Add(selectedItem.URL);
            }
            foreach (string x in list)
            {
                Channel item = core.Channels.First((Channel w) => w.URL == x);
                core.Channel_Full.Remove(item);
            }
            FilterResults();
            core.StatusBarText = $"{list.Count} channels have been deleted";
        }

        private void Menu_get_server_channels_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(async delegate
                {
                    core.IsBusy = true;
                    core.StatusBarText = "XTREAM : Verifying channel data...";
                    string text = await new XpanelData().GetAllChannelsInM3u8(((Channel)datagrid.SelectedItem).URL);
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        core.IsBusy = false;
                        core.StatusBarText = "Failed to get server channels..";
                        MessageBox.Show("Failed to get server channels..", "Server Channels", MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                    else
                    {
                        List<Channel> channels;
                        channels = core.ParseM3u8(text);
                        core.Add_channels(channels);
                        FilterResults();
                        core.IsBusy = false;
                        MessageBox.Show("Successfully extracted server channels", "Server Channels", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                });
        }

        private void Menu_get_server_Status_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                core.IsBusy = true;
                Channel channel = (Channel)datagrid.SelectedItem;
                if (channel != null)
                {
                    XpanelData xp = new XpanelData();
                    Uri uri = new Uri(channel.URL);
                    core.StatusBarText = "Getting server information: " + channel.Server + ".";
                    string player_url = "http://" + uri.Authority + "/player_api.php" + uri.Query;
                    xp.GetServerStatus(player_url);
                }
            });
        }

        private void Menu_play_Click(object sender, RoutedEventArgs e)
        {
            Play_in_VLC();
        }

        private void Menu_rename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Channel channel;
                channel = (Channel)datagrid.SelectedItem;
                RenameWindow renameWindow;
                renameWindow = new RenameWindow(channel.Name);
                renameWindow.ShowDialog();
                if (renameWindow.ChannelName.Trim() != string.Empty)
                {
                    channel.Name = renameWindow.ChannelName.Trim();
                    MessageBox.Show("Channel Has been Renamed successfully..", "Channel Rename", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch
            {
            }
        }

        private object OrderSource(Channel b, string name)
        {
            PropertyInfo runtimeProperty;
            runtimeProperty = b.GetType().GetRuntimeProperty(name);
            if (runtimeProperty != null)
            {
                return runtimeProperty.GetValue(b);
            }
            return null;
        }

        private void Play_in_VLC()
        {
            try
            {
                if (core.Vlc_Location != string.Empty)
                {
                    Process.Start(arguments: ((Channel)datagrid.SelectedItem).URL, fileName: core.Vlc_Location);
                    return;
                }
                MessageBox.Show("Please install VLC Player or use settings to locate VLC Location", "VLC Player", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch
            {
            }
        }

        private void Radio_all_Click(object sender, RoutedEventArgs e)
        {
            FilterResults();
        }

        private void Radio_offline_Click(object sender, RoutedEventArgs e)
        {
            FilterResults();
        }

        private void Radio_online_Click(object sender, RoutedEventArgs e)
        {
            FilterResults();
        }

        private void Radio_unchecked_Click(object sender, RoutedEventArgs e)
        {
            FilterResults();
        }

        private void Sort()
        {
            List<Channel> list;
            list = new List<Channel>();
            list = ((sortDirection != 0) ? core.Channels.OrderByDescending((Channel w) => OrderSource(w, bindingpath)).ToList() : core.Channels.OrderBy((Channel w) => OrderSource(w, bindingpath)).ToList());
            core.Channels.Clear();
            foreach (Channel item in list)
            {
                core.Channels.Add(item);
            }
        }

        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterResults();
        }
    }
}
