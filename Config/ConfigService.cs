using DropShell.Config.Models;
using System.IO;
using System.Text.Json;
using System.Windows;

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

        public void ClearConfig()
        {
            // Reset config to default values
            Config.HotKey = string.Empty;
            Config.DefaultDir = string.Empty;
            Config.Window = new Models.Window();
            Config.ShowOnStartup = false;
			Config.AutoClear = false;
			Config.Groups = new List<Group>();
            Config.StartupCommands = new List<string>();
            Config.LaunchAliases = new Dictionary<string, string>();
		}

		public void LoadConfig()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configPath = Path.Combine(appData, "DropShell", "config.json");

            bool isUsingDefaultConfig = false;

            if (!File.Exists(configPath))
            {
                // Display error and switch to default config
                if (!File.Exists("./dropshell.json") && !File.Exists("./Config/dropshell.json"))
                {
                    // No default config available
                    MessageBox.Show("No default config was found. Please reinstall.");
                    Environment.Exit(1);
                }

                configPath = !File.Exists("./dropshell.json") ? "./Config/dropshell.json" : "./dropshell.json";

                MessageBox.Show("No custom config found in '%APPDATA%/DropShell/', switching to default config");
                isUsingDefaultConfig = true;
            }

            try
            {
                using FileStream fs = new FileStream(configPath, FileMode.Open, FileAccess.Read);
                using JsonDocument doc = JsonDocument.Parse(fs);
                JsonElement root = doc.RootElement;

                Config.HotKey = root.GetProperty("hotkey").GetString() ?? string.Empty;
                Config.DefaultDir = root.GetProperty("defaultDir").GetString() ?? string.Empty;

                JsonElement window = root.GetProperty("window");
                Config.Window.Height = window.GetProperty("height").GetInt32();
                Config.Window.BackgroundColor = window.GetProperty("background").GetString() ?? "#1e1e1e";
                Config.Window.TextColor = window.GetProperty("textColor").GetString() ?? "#ffffff";
                Config.Window.FontSize = window.GetProperty("fontSize").GetInt32();

                Config.ShowOnStartup = root.GetProperty("behavior").GetProperty("showOnStartup").GetBoolean();
                Config.AutoClear = root.GetProperty("behavior").GetProperty("autoClear").GetBoolean();

                JsonElement aliases = root.GetProperty("launchAliases");
                foreach (JsonProperty prop in aliases.EnumerateObject())
                {
                    string name = prop.Name;
                    string path = prop.Value.GetString() ?? string.Empty;

                    Config.LaunchAliases.Add(name, path);
                }

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

                if (isUsingDefaultConfig)
                {
                    MessageBox.Show($"Default hotkey: {Config.HotKey}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
