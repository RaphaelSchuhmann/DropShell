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
    public class StopCommand : ICommand
    {
        public string Name => "stop";
        public string Description => "Fully exits DropShell";

        public Task ExecuteAsync(CommandContext ctx)
        {
            App.Exit_Click();

            return Task.CompletedTask;
        }
    }
}
