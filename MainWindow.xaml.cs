using DropShell.Commands;
using DropShell.Config;
using DropShell.Services.Display;
using System.Windows;
using System.Windows.Media;
using DropShell.Services;

namespace DropShell
{
    public partial class MainWindow : Window
    {
        private H.NotifyIcon.TaskbarIcon? _trayIcon;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
			OutputService.Initialize(OutputScroller, CommandOutput);

            _trayIcon = TrayIcon as H.NotifyIcon.TaskbarIcon;
            if (_trayIcon is null)
            {
                MessageBox.Show("Tray icon resource not found.");
                Environment.Exit(1);
            }

            var mainWindow = Window.GetWindow(this);
            var screen = System.Windows.SystemParameters.PrimaryScreenWidth;
            if (mainWindow != null)
            {
                mainWindow.Width = screen;
                SetPreferences();

                // Run startup commands
                List<string> startUpCommands = ConfigService.Instance.Config.StartupCommands;
                foreach (string command in startUpCommands)
                {
                    await CommandDispatcher.Instance.Dispatch(command, this);
                }
            }
        }

        public void SetPreferences()
        {
            var mainWindow = Window.GetWindow(this);
            mainWindow.Height = ConfigService.Instance.Config.Window.Height;
            mainWindow.Top = 0;
            mainWindow.Left = 0;

            Brush brush = (Brush)new BrushConverter().ConvertFromString(ConfigService.Instance.Config.Window.BackgroundColor!)!;
            mainWindow.Background = brush;

            Brush foregroundColor = (Brush)new BrushConverter().ConvertFromString(ConfigService.Instance.Config.Window.TextColor!)!;
            ClosePath.Fill = foregroundColor;
            CommandInput.Foreground = foregroundColor;
            CommandInput.Focus();
            CurrentPathDisplay.Text = $"{CommandDispatcher.Instance.CurrentWorkingDir()}>";
            CurrentPathDisplay.Foreground = foregroundColor;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void TrayOpen_Click(object sender, RoutedEventArgs e)
        {
            App.Open_Click();
        }

        private void TrayExit_Click(object sender, RoutedEventArgs e)
        {
            _trayIcon?.Dispose();
            App.Exit_Click();
        }

		private async void CommandInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!string.IsNullOrEmpty(CommandInput.Text)) 
                { 
                    if (CommandInput.Text != "clear") OutputService.Instance.Log(CommandInput.Text);
                    
                    await CommandDispatcher.Instance.Dispatch(CommandInput.Text, this);
                    CurrentPathDisplay.Text = $"{CommandDispatcher.Instance.CurrentWorkingDir()}>";

                    CommandHistoryService.Instance.Add(CommandInput.Text);

                    CommandInput.Text = string.Empty; 
                }
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                if (CommandHistoryService.Instance.TryGetNext(out var cmd))
                {
                    CommandInput.Text = cmd;
                    CommandInput.CaretIndex = CommandInput.Text!.Length;
                }
            }
            else if (e.Key == System.Windows.Input.Key.Up)
            {
				if (CommandHistoryService.Instance.TryGetPrevious(out var cmd))
                {
                    CommandInput.Text = cmd;
					CommandInput.CaretIndex = CommandInput.Text!.Length;
				}
            }
        }
    }
}