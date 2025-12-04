using DropShell.Commands;
using DropShell.Config;
using DropShell.Services.Hotkey;
using DropShell.Services.Display;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

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

            OutputService.Initialize(OutputScroller);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this);
            var screen = System.Windows.SystemParameters.PrimaryScreenWidth;
            if (mainWindow != null)
            {
                mainWindow.Height = ConfigService.Instance.Config.Window.Height;
                mainWindow.Width = screen;
                mainWindow.Top = 0;
                mainWindow.Left = 0;

                Brush brush = (Brush)new BrushConverter().ConvertFromString(ConfigService.Instance.Config.Window.BackgroundColor!)!;
                mainWindow.Background = brush;

                Brush foregroundColor = (Brush)new BrushConverter().ConvertFromString(ConfigService.Instance.Config.Window.TextColor!)!;
                ClosePath.Fill = foregroundColor;
                CommandInput.Foreground = foregroundColor;
                CommandInput.Focus();
            }
        }


        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void CommandInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!string.IsNullOrEmpty(CommandInput.Text)) 
                { 
                    OutputService.Instance.Log(CommandInput.Text); 
                    CommandInput.Text = string.Empty; 
                }
            }
        }
    }
}