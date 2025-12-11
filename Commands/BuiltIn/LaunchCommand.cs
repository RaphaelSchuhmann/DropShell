using DropShell.Commands.Models;
using DropShell.Config;
using DropShell.Services;
using DropShell.Services.Display;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace DropShell.Commands.BuiltIn
{
	public class LaunchCommand : ICommand
	{
		[DllImport("user32.dll")]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		const int SW_RESTORE = 9;

		public string Name => "launch";
		public string Description => "Launches a given application name or group";

		public Task ExecuteAsync(CommandContext ctx)
		{
			if (ctx.Args.Count <= 0 || ctx.Args[0].Length == 0)
			{
				OutputService.Instance.LogCommandError(OutputService.Instance.errorMessages["command.noArgs"]);
				return Task.CompletedTask;
			}

			Process? processFound = null;

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
						var match = Regex.Match(command, "\"([^\"]+)\"");
						if (match.Success)
						{
							string path = match.Groups[1].Value;

							if (ConfigService.Instance.Config.LaunchAliases.ContainsKey(path))
							{
								path = ConfigService.Instance.Config.LaunchAliases[path];

								processFound = FindProcessByPath(path);

								if (processFound != null)
								{
									FocusProcessWindow(processFound);
								}
								else
								{
									LaunchProgram(path);
								}
							}
							else
							{
								processFound = FindProcessByPath(path);

								if (processFound != null)
								{
									FocusProcessWindow(processFound);
								}
								else
								{
									LaunchProgram(path);
								}
							}
						}
					}

					UIHelpers.HideMain();
					return Task.CompletedTask; // Return after execution
				}
			}

			if (!File.Exists(ctx.Args[0]))
			{
				// First check if it is an alias
				if (ConfigService.Instance.Config.LaunchAliases.ContainsKey(ctx.Args[0]))
				{
					string path = ConfigService.Instance.Config.LaunchAliases[ctx.Args[0]];

					processFound = FindProcessByPath(path);

					if (processFound != null)
					{
						FocusProcessWindow(processFound);
					}
					else
					{
						LaunchProgram(path);
					}

					UIHelpers.HideMain();
					return Task.CompletedTask;
				}
				else
				{
					OutputService.Instance.LogCommandError($"{OutputService.Instance.errorMessages["command.launch.badPath"]}{ctx.Args[0]}");
					return Task.CompletedTask;
				}
			}

			processFound = FindProcessByPath(ctx.Args[0]);

			if (processFound != null)
			{
				FocusProcessWindow(processFound);
			}
			else
			{
				LaunchProgram(ctx.Args[0]);
			}

			UIHelpers.HideMain();
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

			try
			{
				Process.Start(psi);
			}
			catch (Exception ex)
			{
				OutputService.Instance.LogCommandError($"{OutputService.Instance.errorMessages["command.launch.errorStarting"]}");
			}
		}

		private void FocusProcessWindow(Process proc)
		{
			IntPtr hWnd = proc.MainWindowHandle;

			if (hWnd == IntPtr.Zero)
				return; // no window found

			ShowWindow(hWnd, SW_RESTORE);   // restore if minimized
			SetForegroundWindow(hWnd);      // bring to front
		}

		private Process? FindProcessByPath(string path)
		{
			if (string.IsNullOrEmpty(path)) return null;

			Process[] processes = Process.GetProcesses();

			path = path.ToLower().Replace(" vs ", " visual studio "); // Normalize path

			int colonIndex = path.IndexOf(':');
			int dotIndex = path.LastIndexOf('.');
			string cleanedPath = colonIndex >= 0 && dotIndex  > colonIndex + 1
				? path.Substring(colonIndex + 1, dotIndex - colonIndex - 1)
				: path;

			// Split path into keywords
			List<string> keywords = new List<string>();

			char[] delimiters = new char[] { ' ', '/' };

			string[] tokensArray = cleanedPath.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

			keywords.AddRange(tokensArray);

			// if keywords contains the windows apps directory, it should rather use just the alias
			if (keywords.Contains("windowsapps"))
			{
				string? alias = GetKeyByValue(path);
				if (alias != null)
				{
					keywords.Clear();
					keywords.Add(alias);
				}
			}

			double highestFirstSimilarity = 0;
			double highestScore = 0.0;
			Process? closestMatch = null;

			// Jaro-Winkler Similarity check
			foreach (Process process in processes)
			{
				if (string.IsNullOrEmpty(process.MainWindowTitle.ToLower())) continue;

				string title = process.MainWindowTitle.ToLower();

				double similarity = CalculateSimilarity(path, title);

				if (similarity >= highestFirstSimilarity)
				{
					int maxRelevance = keywords.Count;
					double relevance = 0;

					foreach (string keyword in keywords)
					{
						// If keyword is a direct match add 1 to relevance
						if (title.Contains(keyword))
						{
							relevance += 1.0;
							continue;
						}

						double relevanceSimilarity = CalculateSimilarity(keyword, title);
						if (relevanceSimilarity >= 0.85)
						{
							relevance += 0.5;
						}
					}

					double normalizedRelevance = relevance / maxRelevance;
					double finalScore = (similarity * 0.7) + (normalizedRelevance * 0.3);

					if (finalScore > 0.6 && finalScore > highestScore)
					{
						highestScore = finalScore;
						closestMatch = process;
					}
				}
			}

			return closestMatch;
		}

		private double CalculateSimilarity(string a, string b)
		{
			int aLength = a.Length;
			int bLength = b.Length;

			if (a == b) return 1.0;
			if (aLength == 0 || bLength == 0) return 0.0;

			int matchDistance = (Math.Max(aLength, bLength) / 2) - 1;

			bool[] bUsed = new bool[bLength];
			List<char> MatchedA = new List<char>();

			int matches = 0;

			for (int i = 0; i < aLength; i++)
			{
				int j_start = Math.Max(0, i - matchDistance);
				int j_end = Math.Min(bLength - 1, i + matchDistance);

				for (int j = j_start; j <= j_end; j++)
				{
					if (a[i] == b[j] && bUsed[j] == false)
					{
						MatchedA.Add(a[i]); ;
						bUsed[j] = true;

						matches++;
						break;
					}
				}
			}

			if (matches == 0) return 0.0;

			List<char> orderedMatchedB = new List<char>();
			for (int j = 0; j < bLength; j++)
			{
				if (bUsed[j]) orderedMatchedB.Add(b[j]);
			}

			int minCount = Math.Min(MatchedA.Count, orderedMatchedB.Count);
			int transpositions = 0;
			for (int k = 0; k < minCount; k++)
			{
				if (MatchedA[k] != orderedMatchedB[k]) transpositions++;
			}

			transpositions /= 2;

			double jaro = (1.0 / 3.0) * (
				(double)matches / aLength +
				(double)matches / bLength +
				(double)(matches - transpositions) / matches
			);

			int prefixLength = 0;
			int maxPrefixCheck = Math.Min(4, Math.Min(aLength, bLength));

			for (int i = 0; i < maxPrefixCheck; i++)
			{
				if (a[i] == b[i])
				{
					prefixLength++;
				}
				else
				{
					break;
				}
			}

			double jaroWinkler = jaro + ((double)prefixLength * 0.1 * (1.0 - jaro));

			return jaroWinkler;
		}

		private string? GetKeyByValue(string value)
		{
			if (string.IsNullOrEmpty(value)) return null;

			var reverseDict = new Dictionary<string, string>();

			foreach (var keyValuePair in ConfigService.Instance.Config.LaunchAliases)
			{
				reverseDict.TryAdd(keyValuePair.Value.ToLower(), keyValuePair.Key.ToLower());
			}

			reverseDict.TryGetValue(value, out var key);
			return key;
		}
	}
}
