using DropShell.Commands.Models;
using DropShell.Config;
using DropShell.Services.Display;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace DropShell.Commands.BuiltIn
{
	public class LaunchCommand : ICommand
	{
		public string Name => "launch";
		public string Description => "Launches a given application name or group";

		public Task ExecuteAsync(CommandContext ctx)
		{
			if (ctx.Args.Count <= 0 || ctx.Args[0].Length == 0)
			{
				OutputService.Instance.LogCommandError(OutputService.Instance.errorMessages["command.noArgs"]);
				return Task.CompletedTask;
			}

			// Args should either be a file path or group name
			// Check if arg is a group
			foreach (DropShell.Config.Models.Group group in ConfigService.Instance.Config.Groups)
			{
				if (group.Name == ctx.Args[0])
				{
					OutputService.Instance.LogCommand($"Executing group {group.Name}");

					List<string> commands = group.RawCommands;

					foreach (string command in commands)
					{
						Debug.WriteLine($"{command}");
						var match = Regex.Match(command, "\"([^\"]+)\"");
						if (match.Success)
						{
							string path = match.Groups[1].Value;

							if (ConfigService.Instance.Config.LaunchAliases.ContainsKey(path))
							{
								path = ConfigService.Instance.Config.LaunchAliases[path];
								LaunchProgram(path);
							}
							else
							{
								LaunchProgram(path);
							}
						}
					}

					ctx.Window!.Hide();
					return Task.CompletedTask; // Return after execution
				}
			}

			if (!File.Exists(ctx.Args[0]))
			{
				// First check if it is an alias
				if (ConfigService.Instance.Config.LaunchAliases.ContainsKey(ctx.Args[0]))
				{
					string path = ConfigService.Instance.Config.LaunchAliases[ctx.Args[0]];

					LaunchProgram(path);

					ctx.Window!.Hide();
					return Task.CompletedTask;
				}
				else
				{
					OutputService.Instance.LogCommandError($"{OutputService.Instance.errorMessages["command.launch.badPath"]}{ctx.Args[0]}");
					return Task.CompletedTask;
				}
			}

			LaunchProgram(ctx.Args[0]);

			ctx.Window!.Hide();
			return Task.CompletedTask;
		}

		private void LaunchProgram(string path)
		{
			if (!File.Exists(path))
			{
				OutputService.Instance.LogCommandError($"{OutputService.Instance.errorMessages["command.launch.badPath"]}{path}");
				return;
			}

			path = CommandDispatcher.Instance.TranslateRelativeDir(path); // Ensure path is not a relative path

			string ext = Path.GetExtension(path).ToLower();

			bool isExe = ext == ".exe" || ext == ".bat" || ext == ".cmd";

			if (!isExe)
			{
				OutputService.Instance.LogCommandError($"{OutputService.Instance.errorMessages["command.launch.notExe"]}{path}");
				return;
			}

			var psi = new ProcessStartInfo
			{
				FileName = path,
				UseShellExecute = true,
			};

			Process.Start(psi);
		}
	}
}
