using Microsoft.Win32;
using System.Windows;

namespace IPTV_Checker_2
{
    public partial class SettingsWindow : Window
    {
        private RegistryStore RegistryStore = new RegistryStore();

        public SettingsWindow()
        {
            InitializeComponent();
            txt_userAgent.Text = RegistryStore.UserAgent;
            txt_VLC_Location.Text = RegistryStore.Vlc_Location.Trim();
            combo_timeout.SelectedValue = RegistryStore.Timeout;
            combo_tries.SelectedValue = RegistryStore.NumTries;
            combo_threads.SelectedValue = RegistryStore.NumThreads;
        }

        private void Btn_default_Click(object sender, RoutedEventArgs e)
        {
            txt_userAgent.Text = "Mozilla/5.0 (Windows NT 6.2; Win64; x64;) Gecko/20100101 Firefox/20.0";
        }

        private void Btn_VLC_Location_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog;
            openFileDialog = new OpenFileDialog
            {
                Filter = "VLC.EXE | *.exe"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != string.Empty)
            {
                txt_VLC_Location.Text = openFileDialog.FileName;
            }
        }

        private void Btn_save_settings_Click(object sender, RoutedEventArgs e)
        {
            RegistryStore.UserAgent = txt_userAgent.Text.Trim();
            RegistryStore.Vlc_Location = txt_VLC_Location.Text.Trim();
            RegistryStore.NumTries = int.Parse(combo_tries.SelectedValue.ToString());
            RegistryStore.Timeout = int.Parse(combo_timeout.SelectedValue.ToString());
            RegistryStore.NumThreads = int.Parse(combo_threads.SelectedValue.ToString());
            RegistryStore.SaveToRegistry();
            base.Close();
        }
    }
}
