using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropShell.Services
{
	public sealed class CommandHistoryService
	{
		private static readonly Lazy<CommandHistoryService> _instance = new(() => new CommandHistoryService());
		public static CommandHistoryService Instance => _instance.Value;
		private CommandHistoryService() { }

		private readonly List<string> _history = new List<string>();
		private int _cursor = -1; // "-1" = not browsing history

		public void Add(string command)
		{
			if (string.IsNullOrEmpty(command)) return;
			
			_history.Add(command);
			_cursor = -1;
		}

		public bool TryGetPrevious(out string? command)
		{
			if (_history.Count == 0)
			{
				command = null;
				return false;
			}

			if (_cursor == -1)
				_cursor = _history.Count - 1;
			else if (_cursor > 0)
				_cursor--;

			command = _history[_cursor];
			return true;
		}

		public bool TryGetNext(out string? command)
		{
			if (_history.Count == 0 || _cursor == -1)
			{
				command = null;
				return false;
			}

			if (_cursor < _history.Count - 1)
			{
				_cursor++;
				command = _history[_cursor];
				return true;
			}

			// At end -> return empty input
			_cursor = -1;
			command = "";
			return true;
		}
	}
}
