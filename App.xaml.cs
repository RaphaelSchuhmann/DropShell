using DropShell.Config;
using DropShell.Services.Hotkey;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace DropShell
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static HotkeyService _hotkeyService { get; private set; } = new HotkeyService();
		public static MainWindow _mainWindow { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Create main window but hide it
			_mainWindow = new MainWindow();

			var helper = new WindowInteropHelper(_mainWindow);
			_ = helper.Handle;

			_mainWindow.Show();
			if (!ConfigService.Instance.Config.ShowOnStartup) _mainWindow.Hide();

			// Register Hotkey
			_hotkeyService.hotkeyTriggered += OpenOnHotkey!;
			_hotkeyService.Register(ConfigService.Instance.Config.HotKey, _mainWindow);
		}

		public static void Open_Click()
		{
			App._mainWindow.Dispatcher.Invoke(() =>
			{
				App._mainWindow.Show();
				App._mainWindow.Activate();
			});
		}

		private void OpenOnHotkey(object sender, EventArgs e)
		{
			App._mainWindow.Dispatcher.Invoke(() =>
			{
				App._mainWindow.Show();
				App._mainWindow.Activate();
			});
		}

		public static void Exit_Click()
		{
			App._hotkeyService.Unregister(_mainWindow);
			Environment.Exit(0);
		}
	}
}
