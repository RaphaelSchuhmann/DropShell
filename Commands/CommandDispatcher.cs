using DropShell.Commands.Models;
using DropShell.Config;
using DropShell.Services.Display;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DropShell.Commands
{
    /// <summary>
    /// Takse user input -> finds matching command -> executes it
    /// Keeps track of current working directory
    /// </summary>
    public sealed class CommandDispatcher
    {
        private static readonly Lazy<CommandDispatcher> _instance = new(() => new CommandDispatcher());

        public static CommandDispatcher Instance => _instance.Value;

        private CommandDispatcher()
        {
            RegisterBuiltinCommands();
            ResetWorkingDir();
        }

        private static string _currentWorkingDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        private readonly Dictionary<string, ICommand> _commands = new();
        private readonly Dictionary<string, string> _defaultPaths = new()
        {
            ["user"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/'),
    };

        private void RegisterBuiltinCommands()
        {
            var builtinCmds = Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      .Where(t =>
                                             !t.IsAbstract &&
                                             typeof(ICommand).IsAssignableFrom(t) &&
                                             t.Namespace == "DropShell.Commands.BuiltIn")
                                      .ToList();

            foreach (var builtinCmd in builtinCmds)
            {
                var cmd = (ICommand)Activator.CreateInstance(builtinCmd)!;
                _commands[cmd.Name] = cmd;
            }
        }

        public void Register(ICommand command)
        {
            _commands[command.Name] = command;
        }

        public List<ICommand> GetRegisteredCommands()
        {
            return _commands.Values.ToList();
        }

        public async Task Dispatch(string input, MainWindow window)
        {
            if (string.IsNullOrEmpty(input)) return;

            var tokens = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                  .Cast<Match>()
                  .Select(m => m.Value.Trim('"'))
                  .ToList();

            if (tokens.Count == 0)
                return;

            var commandName = tokens[0];
            var args = tokens.Skip(1).ToList();

            if (!_commands.TryGetValue(commandName, out var command))
            {
                OutputService.Instance.LogCommandError($"{OutputService.Instance.errorMessages["command.unknown"]}{commandName}");
                return;
            }

            CommandContext ctx = new CommandContext { RawInput = input, Args = args, Window = window };

            await command.ExecuteAsync(ctx);
        }

        public void ChangeWorkingDir(string newDir)
        {
            if (!Directory.Exists(newDir))
            {
                OutputService.Instance.LogCommandError(OutputService.Instance.errorMessages["command.cd.dirNotExist"]);
                return;
            }

            _currentWorkingDir = newDir;
        }

        public string TranslateRelativeDir(string relativePath)
        {
            if (relativePath == null) return CurrentWorkingDir();

            if (relativePath.StartsWith("~"))
            {
                string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                relativePath = Path.Combine(userProfilePath, relativePath.Substring(1).TrimStart('/', '\\'));
            }

            string fullPath;

            try
            {
                string combinedPath = Path.Combine(CurrentWorkingDir(), relativePath);
                fullPath = Path.GetFullPath(combinedPath);
            }
            catch (Exception ex)
            {
                OutputService.Instance.LogCommandError($"{OutputService.Instance.errorMessages["command.cd.badPath"]}{ex.Message}");
                return CurrentWorkingDir();
            }

            return fullPath.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
        }

        public void ResetWorkingDir()
        {
            //_currentWorkingDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
            string configDefaultDir = ConfigService.Instance.Config.DefaultDir;

            if (_defaultPaths.ContainsKey(configDefaultDir))
            {
                _currentWorkingDir = _defaultPaths[configDefaultDir];
                return;
            }

            if (string.IsNullOrEmpty(configDefaultDir) || !Directory.Exists(configDefaultDir))
            {
                _currentWorkingDir = _defaultPaths["user"];
                return;
            }

            _currentWorkingDir = configDefaultDir;
        }

        public string CurrentWorkingDir() { return _currentWorkingDir; }
    }
}
