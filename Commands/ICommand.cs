using DropShell.Commands.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShell.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        Task ExecuteAsync(CommandContext ctx);
    }
}
