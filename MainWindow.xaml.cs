using DropShell.Commands;
using DropShell.Config;
using DropShell.Services.Hotkey;
using DropShell.Services.Display;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace DropShell
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            OutputService.Initialize(OutputScroller);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

        private async void CommandInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!string.IsNullOrEmpty(CommandInput.Text)) 
                { 
                    OutputService.Instance.Log(CommandInput.Text);
                    await CommandDispatcher.Instance.Dispatch(CommandInput.Text, this);
                    CurrentPathDisplay.Text = $"{CommandDispatcher.Instance.CurrentWorkingDir()}>";
                    CommandInput.Text = string.Empty; 
                }
            }
        }
    }
}