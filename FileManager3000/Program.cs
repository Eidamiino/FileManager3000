using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			string path = null;
			do
			{
				path = ReadValue($"Enter directory path: ", $"c:\\Windows\\System32");
			} while (!IsExisting(path));

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

		public static bool IsExisting(string dirPath)
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
