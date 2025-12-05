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
    public class EchoCommand : ICommand
    {
        public string Name => "echo";
        public string Description => "Prints out a given message";

        public Task ExecuteAsync(CommandContext ctx)
        {
            if (ctx.Args.Count <= 0)
            {
                OutputService.Instance.LogCommandError(OutputService.Instance.errorMessages["command.noArgs"]);
                return Task.CompletedTask;
            }

            OutputService.Instance.LogCommand(ctx.Args[0]);

            return Task.CompletedTask;
        }
    }
}
