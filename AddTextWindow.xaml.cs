using System.Windows;

namespace IPTV_Checker_2
{
    public partial class AddTextWindow : Window
    {
        public string str = string.Empty;

        public AddTextWindow()
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
            base.Close();
        }
    }
}
