using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using ConsoleTables;

namespace FileManager3000
{
	public class ExtensionStatistic
	{
		public string Name { get; set; }
		public long Count { get; set; }
		public long Sum { get; set; }

		public ExtensionStatistic(string name, long count, long sum)
		{
			Name = name;
			Count = count;
			Sum = sum;
		}
	}
	internal class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				var choice = int.Parse(ReadValue($"Choose your option:\n1) File extension statistics\t2) Create backup file\t3) List backup files\n", "0"));
				switch (choice)
				{
					case 1:
						{
							string path = null;
							do
							{
								path = ReadValue($"Enter directory path: ", $"c:\\Windows\\System32");
							} while (!DirExists(path));

							string extensions = ReadValue($"Enter extensions to search for (separated with ;): ", string.Empty).Trim();
							var allFiles = Directory.GetFiles(path).Select(x => new FileInfo(x)).ToList();

							if (extensions != string.Empty || !string.IsNullOrEmpty(extensions))
							{
								var trimmedExtensions = extensions.Split(';').Select(x => x.Trim()).ToList();
								allFiles = allFiles.Where(x => trimmedExtensions.Contains(x.Extension)).ToList();
							}

							var allExtensions = allFiles.Select(x => x.Extension.ToLower()).Distinct().ToList();

							var extensionStatistics = GetExtensionsStats(allExtensions, allFiles);
							extensionStatistics = extensionStatistics.OrderByDescending(x => x.Count).ToList();

							var table = new ConsoleTable($"Extension", $"Count", $"Sum");
							extensionStatistics.ForEach(x => table.AddRow($"{x.Name}", $"{x.Count}", $"{GetSize(x.Sum)}MB"));
							table.Write();
						}
						break;
					case 2:
						{
							var rootPath = ReadValue($"Enter directory path: ", $"c:\\Temp");
							if (!DirExists(rootPath))
								Directory.CreateDirectory(rootPath);
							var allFiles = Directory.GetFiles(rootPath).Select(x => new FileInfo(x)).ToList();
							var folderPath = $"{rootPath}\\{DateTime.Now:yyyy-mm-dd_hh-mm-ss}";
							Directory.CreateDirectory(folderPath);
							var zipPath = $"{folderPath}\\backup.zip";

							using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
							allFiles.ForEach(x => archive.CreateEntryFromFile($"{x}", Path.GetFileName($"{x}")));
						}
						break;
					case 3:
						{
							string rootPath;
							do
							{
								rootPath = ReadValue($"Enter directory path: ", $"c:\\Temp");
							} while (!DirExists(rootPath));

							var foundFiles = Directory.EnumerateFiles(rootPath, "backup.zip", SearchOption.AllDirectories).ToList();
							List<string> foundFolders = new List<string>();
							foundFiles.ForEach(x => foundFolders.Add(Path.GetDirectoryName(x)));
							foundFolders.ForEach(x => Console.WriteLine($"{foundFolders.IndexOf(x) + 1}: {x}"));
							do
							{
								var openedFileIndex = ReadValue($"Which file you'd like to open ('b' to go to main menu): ", "b");
								if (openedFileIndex == "b")
									break;

								if (foundFolders.Count > int.Parse(openedFileIndex) && int.Parse(openedFileIndex) > 0)
									Process.Start("explorer.exe", foundFiles[int.Parse(openedFileIndex) - 1]);
								else
									InvalidOption();
							} while (true);
						}
						break;
					default:
						{
							InvalidOption();
						}
						break;
				}
			}
		}

		private static void InvalidOption()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Invalid menu option selected");
			Console.ResetColor();
			Thread.Sleep(750);
		}

		private static List<ExtensionStatistic> GetExtensionsStats(List<string> allExtensions, List<FileInfo> allFiles)
		{
			List<ExtensionStatistic> extensionStatistics = new List<ExtensionStatistic>();
			allExtensions.ForEach(x =>
			{
				var fileCountPerExtensions = allFiles.Where(y => y.Extension.ToLower() == x).ToList();
				var sumSize = fileCountPerExtensions.Sum(x => x.Length);
				var count = fileCountPerExtensions.Count;
				extensionStatistics.Add(new ExtensionStatistic(x, count, sumSize));
			});
			return extensionStatistics;
		}

		private static string ReadValue(string label, string defaultValue)
		{
			Console.Write($"{label} (default: {defaultValue}): ");
			string value = Console.ReadLine();
			if (value == "")
				return defaultValue;
			return value;
		}

		public static bool DirExists(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				Console.WriteLine($"Directory with path {dirPath} does not exist");
				return false;
			}
			return true;
		}

		public static double GetSize(double length)
		{
			return Math.Round(length / 1024 / 1024, 2);
		}
	}
}
