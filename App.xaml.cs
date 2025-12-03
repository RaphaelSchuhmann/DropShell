using DropShell.Config;
using DropShell.Services.Hotkey;
using Hardcodet.Wpf.TaskbarNotification;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Interop;

namespace DropShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _trayIcon;
        private HotkeyService _trigger;
        private HotkeyService _hotkeyService;
        private MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create main window but hide it
            _mainWindow = new MainWindow();

            var helper = new WindowInteropHelper(_mainWindow);
            _ = helper.Handle;

            _mainWindow.Hide();

            // Register Hotkey
            _hotkeyService = new HotkeyService();
            _hotkeyService.hotkeyTriggered += OpenOnHotkey!;
            _hotkeyService.Register(ConfigService.Instance.Config.HotKey, _mainWindow);

            // Initialize tray icon
            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.Activate();
        }

        private void OpenOnHotkey(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.Activate();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _trayIcon.Dispose();
            _hotkeyService.Unregister(_mainWindow);
            Shutdown();
        }
    }
}
