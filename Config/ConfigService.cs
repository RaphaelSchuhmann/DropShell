using DropShell.Commands;
using DropShell.Config.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Text.Json;
using Microsoft.Windows;
using System.Windows;
using System.Configuration;

namespace DropShell.Config
{
    /// <summary>
    /// Singleton service that loads and provides access to DropShell configuration.
    /// Only one instance exists during the application lifetime.
    /// </summary>
    public sealed class ConfigService
    {
        private static readonly Lazy<ConfigService> _instance = new(() => new ConfigService());
        public static ConfigService Instance => _instance.Value;
        
        public DropShellConfig Config { get; set; } = new DropShellConfig();

        private ConfigService()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configPath = Path.Combine(appData, "DropShell", "config.json");

            if (!File.Exists(configPath))
            {
                // Display error and switch to default config
                MessageBox.Show("No custom config found in '%APPDATA%/DropShell/', switching to default config");
                configPath = "./dropshell.json";
            }

            try
            {
                using FileStream fs = new FileStream(configPath, FileMode.Open, FileAccess.Read);
                using JsonDocument doc = JsonDocument.Parse(fs);
                JsonElement root = doc.RootElement;

                Config.HotKey = root.GetProperty("hotkey").GetString() ?? string.Empty;

                JsonElement window = root.GetProperty("window");
                Config.Window.Height = window.GetProperty("height").GetInt32();
                Config.Window.BackgroundColor = window.GetProperty("background").GetString() ?? "#1e1e1e";
                Config.Window.TextColor = window.GetProperty("textColor").GetString() ?? "#ffffff";
                Config.Window.FontSize = window.GetProperty("fontSize").GetInt32();

                Config.ShowOnStartup = root.GetProperty("behavior").GetProperty("showOnStartup").GetBoolean();
                Config.AutoClear = root.GetProperty("behavior").GetProperty("autoClear").GetBoolean();

                JsonElement groups = root.GetProperty("groups");
                Config.Groups = new List<Group>();

                foreach (JsonProperty prop in groups.EnumerateObject())
                {
                    Group g = new Group();
                    g.Name = prop.Name;

                    g.RawCommands = prop.Value
                                        .EnumerateArray()
                                        .Select(v => v.GetString() ?? string.Empty)
                                        .ToList();

                    Config.Groups.Add(g);
                }

                Config.StartupCommands = new List<string>();
                JsonElement startupElement = root.GetProperty("startup");

                foreach (JsonElement item in startupElement.EnumerateArray())
                {
                    Config.StartupCommands.Add(item.GetString() ?? "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
