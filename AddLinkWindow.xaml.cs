using System.Windows;
using System.Windows.Controls;

namespace IPTV_Checker_2
{
    /// <summary>
    /// Logique d'interaction pour AddLinkWindow.xaml
    /// </summary>
    public partial class AddLinkWindow : Window
    {
        public string str = string.Empty;

        public AddLinkWindow()
        {
            InitializeComponent();
        }

        private void Btn_clear_txt_Click(object sender, RoutedEventArgs e)
        {
            txt_input.Text = string.Empty;
        }

        private void Btn_add_text_Click(object sender, RoutedEventArgs e)
        {
            str = txt_input.Text.Trim();
            Close();
        }
    }
}
