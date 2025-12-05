using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DropShell.Commands;
using DropShell.Commands.Models;
using DropShell.Services.Display;

namespace DropShell.Commands.BuiltIn
{
    public class ExitCommand : ICommand
    {
        public string Name => "exit";
        public string Description => "Hides the shell window";

        public Task ExecuteAsync(CommandContext ctx)
        {
            ctx.Window!.Hide();

            return Task.CompletedTask;
        }
    }
}
