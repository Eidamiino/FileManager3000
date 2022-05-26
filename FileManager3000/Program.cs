using System;
using System.IO;
using System.Linq;

namespace FileManager3000
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string path = ReadValue($"Enter directory path: ", $"c:\\Windows\\System32");
			string extensions = ReadValue($"Enter extensions to search for (separated with ;): ", null);

			extensions.Trim(' ');

			var files = Directory.GetFiles(path);
			var fileInfos = files.Select(x => new FileInfo(x)).ToList();
			var onlyType = fileInfos.Where(x => x.Extension == extensions).Select(x=>x).ToList();

			onlyType.ForEach(x=>Console.WriteLine($"File: {x.Name}\tSize: {x.Length/1024/1024}MB"));

			var fileCount = onlyType.Count();
			var sumSize = onlyType.Sum(x => x.Length);
			var avgSize = onlyType.Average(x => x.Length);
			Console.WriteLine($"Amount of files: {fileCount}\tSum: {sumSize / 1024 / 1024}MB\tAverage:{avgSize / 1024 / 1024}MB");
		}

		private static string ReadValue(string label,string defaultValue)
		{
			Console.Write($"{label} (default: {defaultValue}): ");
			string value = Console.ReadLine();
			if (value=="")
				return defaultValue;
			return value;
		}
	}
}
