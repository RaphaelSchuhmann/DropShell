using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DropShell.Commands;
using DropShell.Commands.Models;
using DropShell.Config;
using DropShell.Services.Display;

namespace DropShell.Commands.BuiltIn
{
    public class ReloadCommand : ICommand
    {
        public string Name => "reload";
        public string Description => "Reloads the config";

        public Task ExecuteAsync(CommandContext ctx)
        {
            ConfigService.Instance.LoadConfig();

            // Reopen shell
            OutputService.Instance.ClearScreen();
            ctx.Window!.Hide();
            
            ctx.Window!.SetPreferences();

            ctx.Window!.Show();

            return Task.CompletedTask;
        }
    }
}
