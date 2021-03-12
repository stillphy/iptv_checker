using IPTV_Checker_2.DTO;
using System;
using System.Windows;

namespace IPTV_Checker_2
{
    public partial class ServerStatusWindow : Window
    {
        public ServerStatusWindow(ServerStatus serverStatus, string creationTime, string expireTime)
        {
            InitializeComponent();
            creationdate.Text = creationTime.ToString();
            expirationdate.Text = expireTime.ToString();
            DataContext = serverStatus.user_info;
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
