using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class OpenCommand : ICommand
    {
        public string Name => "open";
        public string Description => "Opens a file or folder with the default application";

        public Task ExecuteAsync(CommandContext ctx)
        {
            string path = CommandDispatcher.Instance.TranslateRelativeDir(ctx.Args[0]);

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                OutputService.Instance.LogCommandError($"{OutputService.Instance.errorMessages["command.cd.badPath"]}{path}");
                return Task.CompletedTask;
            }

            var psi = new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
            };

            Process.Start(psi);

            ctx.Window!.Hide();

            return Task.CompletedTask;
        }
    }
}
