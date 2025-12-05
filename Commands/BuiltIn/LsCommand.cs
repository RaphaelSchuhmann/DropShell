using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DropShell.Commands;
using DropShell.Commands.Models;
using DropShell.Services.Display;

namespace DropShell.Commands.BuiltIn
{
    public class LsCommand : ICommand
    {
        public string Name => "ls";
        public string Description => "Lists all items in the current working directory";

        public Task ExecuteAsync(CommandContext ctx)
        {
            string[] items = Directory.GetFileSystemEntries(CommandDispatcher.Instance.CurrentWorkingDir());

            foreach (string item in items)
            {
                if (string.IsNullOrEmpty(item)) continue;

                OutputService.Instance.LogCommand(item.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/'));
            }

            return Task.CompletedTask;
        }
    }
}
