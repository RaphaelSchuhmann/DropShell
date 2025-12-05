using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DropShell.Commands;
using DropShell.Commands.Models;
using DropShell.Services.Display;

namespace DropShell.Commands.BuiltIn
{
    public class PwdCommand : ICommand
    {
        public string Name => "pwd";
        public string Description => "Prints the current working directory";

        public Task ExecuteAsync(CommandContext ctx)
        {
            OutputService.Instance.LogCommand($"Path: {CommandDispatcher.Instance.CurrentWorkingDir()}");

            return Task.CompletedTask;
        }
    }
}
