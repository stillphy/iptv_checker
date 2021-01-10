using System.Windows;

namespace IPTV_Checker_2
{
    public partial class RenameWindow : Window
    {
        public string ChannelName
        {
            get;
            set;
        }

        public RenameWindow(string channelName)
        {
            InitializeComponent();
            ChannelName = channelName;
            txt_channel_name.Text = channelName;
        }

        private void Btn_ok_Click(object sender, RoutedEventArgs e)
        {
            ChannelName = txt_channel_name.Text.Trim();
            Close();
        }
    }
}
