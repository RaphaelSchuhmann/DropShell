using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShell.Commands.Models
{
    /// <summary>
    /// Holds things the command may need (raw input, args, config, services, etc.)
    /// </summary>
    public class CommandContext
    {
        public List<string> Args { get; set; } = new List<string>();
        public string RawInput { get; set; } = string.Empty;
    }
}
