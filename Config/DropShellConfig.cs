using DropShell.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShell.Config.Models
{
    /// <summary>
    /// Model that mirrors the JSON file.
    /// </summary>
    public class DropShellConfig
    {
        public string HotKey { get; set; } = string.Empty;
        public Window Window { get; set; } = new Window();
        public bool ShowOnStartup { get; set; } = false;
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<string> StartupCommands { get; set; } = new List<string>();
    }

    public class Window
    {
        public int Height { get; set; } = 300;
        public double Opacity { get; set; } = 0.9;
        public string BackgroundColor { get; set; } = string.Empty;
        public string TextColor { get; set; } = string.Empty;
        public int FontSize { get; set; } = 14;
    }

    public class Group
    {
        public string Name { get; set; } = string.Empty;
        public List<string> RawCommands { get; set; } = new List<string>();
    }
}
