using DropShell.Config;
using DropShell.Services.Hotkey;
using DropShell.UI.CommandOutput;
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

        private void OutputData(string data)
        {
            string currentDirectory = Directory.GetCurrentDirectory(); // Needs to be changed to internal current directory
            string currentTime = DateTime.Now.ToString("dd.mm.yyyy hh:mm tt");

            string output = $"[{currentTime}] {currentDirectory}> ${data}";
            CommandDisplay.Instance.AddItem(output);

            UpdateOutputDisplay();
        }

        private void UpdateOutputDisplay()
        {
            List<string> data = CommandDisplay.Instance.GetList();
            if (data.Count > 0)
            {
                CommandOutput.Children.Clear();
                foreach (string item in data)
                {
                    CommandOutput.Children.Add(new TextBox { Text = item, IsReadOnly = true, FontSize = 30, Foreground = (Brush)new BrushConverter().ConvertFromString(ConfigService.Instance.Config.Window.TextColor!)! });
                }
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _hotkeyService.Unregister(this); // unregister the hotkey
            base.OnClosing(e);
        }

        private void CommandInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!string.IsNullOrEmpty(CommandInput.Text)) 
                { 
                    OutputData(CommandInput.Text); 
                    CommandInput.Text = string.Empty; 
                }
            }
        }
    }
}