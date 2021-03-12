using IPTV_Checker_2.DTO;
using System;
using System.Windows;

namespace IPTV_Checker_2
{
    public partial class ServerStatusWindow : Window
    {
        public ServerStatusWindow(ServerStatus serverStatus)
        {
            InitializeComponent();
            DataContext = serverStatus.user_info;
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
