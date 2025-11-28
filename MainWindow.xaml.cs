using DropShell.Config;
using System.Windows;

namespace DropShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var hotkey = ConfigService.Instance.Config.HotKey;
            MessageBox.Show(hotkey);
        }
    }
}