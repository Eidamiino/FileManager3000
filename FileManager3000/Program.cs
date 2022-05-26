using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleTables;

namespace FileManager3000
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string path = null;
			do
			{
				path = ReadValue($"Enter directory path: ", $"c:\\Windows\\System32");
			} while (!IsExist(path));

			string extensions = ReadValue($"Enter extensions to search for (separated with ;): ", string.Empty);
			var allFiles = Directory.GetFiles(path).Select(x=>new FileInfo(x)).ToList();

			if (extensions != string.Empty||!string.IsNullOrEmpty(extensions))
			{
				var trimmedExtensions = extensions.Split(';').Select(x => x.Trim()).ToList();
				allFiles = allFiles.Where(x => trimmedExtensions.Contains(x.Extension)).ToList();
			}

			//var distinctExtensions = allFiles.Select(x => x.Extension.ToLower()).Distinct().ToList();
			//distinctExtensions.ForEach(x =>
			//{
			//	var filesPerExtension = allFiles.Where(y => y.Extension.ToLower() == x).ToList();
			//});

			var table = new ConsoleTable($"Extension", $"Name", $"Size");
			allFiles.ForEach(x=>table.AddRow($"{x.Extension}", $"{x.Name}", $"{GetSize(x.Length)}MB"));
			table.Write();

			var sumSize = allFiles.Sum(x => x.Length);
			Console.WriteLine($"Sum: {GetSize(sumSize)}MB");
		}

		private static string ReadValue(string label, string defaultValue)
		{
			Console.Write($"{label} (default: {defaultValue}): ");
			string value = Console.ReadLine();
			if (value == "")
				return defaultValue;
			return value;
		}

		public static bool IsExist(string dirPath)
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
			return Math.Round(length / 1024/1024,2);
		}
	}
}
