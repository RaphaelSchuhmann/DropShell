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
    public class HelpCommand : ICommand
    {
        public string Name => "help";
        public string Description => "Displays additional information to all commands";

        public Task ExecuteAsync(CommandContext ctx)
        {
            List<ICommand> commands = CommandDispatcher.Instance.GetRegisteredCommands();

            OutputService.Instance.LogCommand("These are all registered commands:");

            foreach (var command in commands)
            {
                OutputService.Instance.LogCommand($"{command.Name} - {command.Description}");
            }

            return Task.CompletedTask;
        }
    }
}
