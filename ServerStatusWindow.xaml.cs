using IPTV_Checker_2.Models;
using System.Windows;

namespace IPTV_Checker_2
{
    public partial class ServerStatusWindow : Window
    {
        public ServerStatusWindow(DTO.ServerStatus serverStatus)
        {
            InitializeComponent();
            DataContext = serverStatus;
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
