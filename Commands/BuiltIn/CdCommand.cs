using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropShell.Commands;
using DropShell.Commands.Models;
using DropShell.Services.Display;

namespace DropShell.Commands.BuiltIn
{
    public class CdCommand : ICommand
    {
        public string Name => "cd";
        public string Description => "Changes the current working directory";

        public Task ExecuteAsync(CommandContext ctx)
        {
            if (ctx.Args.Count <= 0)
            {
                OutputService.Instance.LogCommandError(OutputService.Instance.errorMessages["command.noArgs"]);
                return Task.CompletedTask;
            }

            string newDir = CommandDispatcher.Instance.TranslateRelativeDir(ctx.Args[0]);
            CommandDispatcher.Instance.ChangeWorkingDir(newDir);

            return Task.CompletedTask;
        }
    }
}
