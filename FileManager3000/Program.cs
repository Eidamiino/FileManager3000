using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

			string extensions = ReadValue($"Enter extensions to search for (separated with ;): ", null).Trim();
			var allFiles = Directory.GetFiles(path).Select(x=>new FileInfo(x)).ToList();

			if (extensions != string.Empty||string.IsNullOrEmpty(extensions))
			{
				var trimmedExtensions = extensions.Split(';').Select(x => x.Trim()).ToList();
				allFiles = allFiles.Where(x => trimmedExtensions.Contains(x.Extension)).ToList();
			}
			//var onlyType = files.Where(x => x.Extension == extensions).Select(x => x).ToList();

			allFiles.ForEach(x => Console.WriteLine($"File: {x.Name}\tSize: {x.Length / 1024 / 1024}MB"));

			var fileCount = allFiles.Count();
			var sumSize = allFiles.Sum(x => x.Length);
			var avgSize = allFiles.Average(x => x.Length);
			Console.WriteLine($"Amount of files: {fileCount}\tSum: {sumSize / 1024 / 1024}MB\tAverage:{avgSize / 1024 / 1024}MB");
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
	}
}
