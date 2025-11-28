using DropShell.Config;
using DropShell.Services.Hotkey;
using System.Windows;

namespace DropShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HotkeyService _hotkeyService;

        public MainWindow()
        {
            InitializeComponent();
            _hotkeyService = new HotkeyService();

            this.Loaded += (s, e) =>
            {
                _hotkeyService.Register(ConfigService.Instance.Config.HotKey, this);
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button clicked");
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _hotkeyService.Unregister(this); // unregister the hotkey
            base.OnClosing(e);
        }
    }
}