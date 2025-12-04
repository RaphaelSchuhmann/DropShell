using DropShell.Services.Display;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private CommandDispatcher() { }

        private static string _currentWorkingDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public void ChangeWorkingDir(string newDir)
        {
            if (newDir == _currentWorkingDir)
            {
                OutputService.Instance.LogError(OutputService.Instance.errorMessages["command.cd.sameDir"]);
            }

            if (!Directory.Exists(newDir))
            {
                OutputService.Instance.LogError(OutputService.Instance.errorMessages["command.cd.dirNotExist"]);
            }

            _currentWorkingDir = newDir;
        }

        public void ResetWorkingDir()
        {
            _currentWorkingDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public string CurrentWorkingDir() { return _currentWorkingDir; }
    }
}
