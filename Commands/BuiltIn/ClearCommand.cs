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
    public class ClearCommand : ICommand
    {
        public string Name => "clear";
        public string Description => "Clears the screen.";

        public Task ExecuteAsync(CommandContext ctx)
        {
            OutputService.Instance.ClearScreen();

            return Task.CompletedTask;
        }
    }
}
