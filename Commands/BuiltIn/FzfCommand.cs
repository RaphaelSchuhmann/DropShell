using DropShell.Commands.Models;
using DropShell.Services.Display;
using System.Diagnostics;
using System.IO;

namespace DropShell.Commands.BuiltIn
{
	public class FzFCommand : ICommand
	{
		public string Name => "fzf";
		public string Description => "Fuzzy search current working directory for a given file name";

		public Task ExecuteAsync(CommandContext ctx)
		{
			if (ctx.Args.Count <= 0 || ctx.Args[0].Length == 0)
			{
				OutputService.Instance.LogCommandError(OutputService.Instance.errorMessages["command.noArgs"]);
				return Task.CompletedTask;
			}

			try
			{
				string[] items = Directory.GetFileSystemEntries(CommandDispatcher.Instance.CurrentWorkingDir());

				List<(string Name, double Score)> results = new();

				foreach (string item in items)
				{
					if (string.IsNullOrEmpty(item)) continue;

					string path = item;

					// Remove trailing slash or backslash
					if (path.EndsWith("/") || path.EndsWith("\\"))
					{
						path = path.Substring(0, path.Length - 1);
					}

					string name = Path.GetFileName(path);
					if (!string.IsNullOrEmpty(name))
					{
						double score = FuzzySearch(ctx.Args[0], name);
						if (score > 0.0)
						{
							results.Add((name, score));
						}
					}
				}

				// Sort results by score descending
				results.Sort((a, b) => b.Score.CompareTo(a.Score));

				// Output top matches (you can adjust how many to show)
				foreach (var result in results)
				{
					OutputService.Instance.LogCommand(result.Name);
				}

				return Task.CompletedTask;
			}
			catch (Exception _)
			{
				OutputService.Instance.LogCommandError("An unexpected error occured, resetting current working directory...");
				CommandDispatcher.Instance.ResetWorkingDir();
				return Task.CompletedTask;
			}
		}

		// Returns the fuzzy match score (0 = no match)
		private double FuzzySearch(string pattern, string text)
		{
			int patternLength = pattern.Length;
			int textLength = text.Length;

			if (patternLength == 0 || textLength == 0) return 0.0;

			List<int> matches = new List<int>();
			int patternIndex = 0;

			for (int i = 0; i < textLength; i++)
			{
				if (char.ToLower(text[i]) == char.ToLower(pattern[patternIndex]))
				{
					patternIndex++;
					matches.Add(i);
					if (patternIndex == patternLength) break; // all pattern chars matched
				}
			}

			if (patternIndex == patternLength)
			{
				double score = CalculateFinalScore(matches, text, pattern);
				return score;
			}
			else
			{
				return 0.0;
			}
		}

		private double CalculateFinalScore(List<int> matches, string text, string pattern)
		{
			double consecutiveBonus = CalculateConsecutiveBonus(matches, 5.0);
			double wordBonus = CalculateWordBoundaryBonus(text, matches, 15.0, 12.0);
			double earlyBonus = CalculateStartingEarlyBonus(matches, text.Length);
			double tightBonus = CalculateTightMatchBonus(matches, text.Length);
			double caseBonus = CalculateCaseSensitivityBonus(pattern, text, matches, 1.5);

			double totalScore = consecutiveBonus + wordBonus + tightBonus + earlyBonus + caseBonus;

			// Optional slight penalty for extra characters in text
			totalScore -= (text.Length - matches.Count) * 0.1;

			return totalScore;
		}

		private double CalculateConsecutiveBonus(List<int> matches, double baseBonus)
		{
			double bonus = 0.0;
			for (int k = 1; k < matches.Count; k++)
			{
				if (matches[k] == matches[k - 1] + 1)
				{
					bonus += baseBonus;
				}
			}
			return bonus;
		}

		private double CalculateWordBoundaryBonus(string text, List<int> matches, double startBonus, double wordBonus)
		{
			double bonus = 0.0;

			if (matches.Count > 0 && matches[0] == 0)
			{
				bonus += startBonus;
			}

			foreach (int index in matches)
			{
				if (index > 0)
				{
					char precedingChar = text[index - 1];
					if (char.IsWhiteSpace(precedingChar) || precedingChar == '_' || precedingChar == '-' || precedingChar == '/')
					{
						bonus += wordBonus;
					}

					// CamelCase boundary
					if (char.IsUpper(text[index]) && char.IsLower(precedingChar))
					{
						bonus += wordBonus;
					}
				}
			}

			return bonus;
		}

		private double CalculateStartingEarlyBonus(List<int> matches, int length)
		{
			if (matches.Count == 0) return 0.0;

			int firstMatchIndex = matches[0];
			double maxBonus = 10.0;
			double scaledBonus = maxBonus * (1.0 - (double)firstMatchIndex / length);
			return scaledBonus;
		}

		private double CalculateTightMatchBonus(List<int> matches, int length)
		{
			if (matches.Count < 2) return 0.0;

			int firstIndex = matches[0];
			int lastIndex = matches[^1];

			int spanLength = lastIndex - firstIndex + 1;
			double maxBonus = 20.0;
			double scaledBonus = maxBonus * (1.0 - (double)spanLength / length);
			return scaledBonus;
		}

		private double CalculateCaseSensitivityBonus(string pattern, string text, List<int> matchIndices, double caseBonus)
		{
			double bonus = 0.0;
			for (int k = 0; k < pattern.Length; k++)
			{
				if (pattern[k] == text[matchIndices[k]])
				{
					bonus += caseBonus;
				}
			}
			return bonus;
		}
	}
}
