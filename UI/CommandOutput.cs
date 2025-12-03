using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShell.UI.CommandOutput
{
    public sealed class CommandDisplay
    {
        private static readonly Lazy<CommandDisplay> _instance = new(() => new CommandDisplay());

        public static CommandDisplay Instance => _instance.Value;

        private CommandDisplay() { }

        // List
        private static List<string> outputList = new();

        // Add to list
        public void AddItem(string item)
        {
            if (outputList.Count > 100)
            {
                outputList.RemoveAt(0);
            }
            outputList.Add(item);
        }

        // Get list
        public List<string> GetList()
        {
            return outputList;
        }

        // Clear List
        public void Clear()
        {
            outputList.Clear();
        }
    }
}
