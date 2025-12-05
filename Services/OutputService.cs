using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DropShell.Commands;
using DropShell.Config;

namespace DropShell.Services.Display
{
    public sealed class OutputService
    {
        private static OutputService? _instance;

        public static OutputService Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("OutputService must be initialized by calling Initialize() first.");
                }
                return _instance;
            }
        }

        private readonly ScrollViewer _outputPanel;

        private OutputService(ScrollViewer outputPanel) 
        { 
            _outputPanel = outputPanel;
        }

        public static void Initialize(ScrollViewer outputStackPanel)
        {
            if (_instance == null)
            {
                _instance = new OutputService(outputStackPanel);
            }
        }

        public readonly Dictionary<string, string> errorMessages = new Dictionary<string, string>
        {
            ["command.cd.dirNotExist"] = "Invalid Input: New directory does not exist!",
            ["command.cd.badPath"] = "Invalid path format: ",
            ["command.noArgs"] = "Invalid Input: No arguments were supplied",
            ["command.unknown"] = "Unknown command: ",
        };

        private void Display(TextBox message)
        {
            if (message == null) return;

            StackPanel display = _outputPanel.Content as StackPanel;

            _outputPanel.Dispatcher.Invoke(() =>
            {
                display!.Children.Add(message);

                // Add auto scrolling to bottom here
                _outputPanel.ScrollToBottom();
            });
        }

        public void ClearScreen()
        {
            if (_outputPanel == null) return;

            StackPanel display = _outputPanel.Content as StackPanel;

            _outputPanel.Dispatcher.Invoke(() =>
            {
                display!.Children.Clear();
            });
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            // add message prefix
            string currentDirectory = CommandDispatcher.Instance.CurrentWorkingDir();
            string currentTime = DateTime.Now.ToString("dd.mm.yyyy hh:mm:ss");

            message = $"[{currentTime}] {currentDirectory}> {message}";


            TextBox messageBox = new TextBox
            {
                Text = message,
                Margin = new System.Windows.Thickness(0, 2, 0, 2),
                Foreground = (Brush)new BrushConverter().ConvertFromString(ConfigService.Instance.Config.Window.TextColor!)!,
                Background = Brushes.Transparent,
                BorderThickness = new System.Windows.Thickness(0),
                IsReadOnly = true,
                FontSize = ConfigService.Instance.Config.Window.FontSize,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            Display(messageBox);
        }

        public void LogError(string error)
        {
            if (string.IsNullOrEmpty(error)) return;

            // add message prefix
            string currentDirectory = CommandDispatcher.Instance.CurrentWorkingDir();
            string currentTime = DateTime.Now.ToString("dd.mm.yyyy hh:mm");

            error = $"<ERROR> [{currentTime}] {currentDirectory}> {error}";

            TextBox messageBox = new TextBox
            {
                Text = error,
                Margin = new System.Windows.Thickness(0, 2, 0, 2),
                Foreground = Brushes.Red,
                Background = Brushes.Transparent,
                BorderThickness = new System.Windows.Thickness(0),
                IsReadOnly = true,
                FontSize = ConfigService.Instance.Config.Window.FontSize,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            Display(messageBox);
        }

        public void LogCommand(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            TextBox messageBox = new TextBox
            {
                Text = message,
                Margin = new System.Windows.Thickness(0, 2, 0, 2),
                Foreground = (Brush)new BrushConverter().ConvertFromString(ConfigService.Instance.Config.Window.TextColor!)!,
                Background = Brushes.Transparent,
                BorderThickness = new System.Windows.Thickness(0),
                IsReadOnly = true,
                FontSize = ConfigService.Instance.Config.Window.FontSize,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            Display(messageBox);
        }

        public void LogCommandError(string error)
        {
            if (string.IsNullOrEmpty(error)) return;

            TextBox messageBox = new TextBox
            {
                Text = error,
                Margin = new System.Windows.Thickness(0, 2, 0, 2),
                Foreground = Brushes.Red,
                Background = Brushes.Transparent,
                BorderThickness = new System.Windows.Thickness(0),
                IsReadOnly = true,
                FontSize = ConfigService.Instance.Config.Window.FontSize,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            Display(messageBox);
        }
    }
}
