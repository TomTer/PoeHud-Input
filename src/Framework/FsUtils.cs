using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PoeHUD.Framework
{
	public static class FsUtils
	{
		public static Dictionary<string, string> LoadKeyValueCommaSeparatedFromFile(string fileName)
		{
			var result = new Dictionary<string, string>();

			string[] lines = File.ReadAllLines(fileName);
			foreach (string line in lines.Select(a => a.Trim()))
			{
				if (string.IsNullOrWhiteSpace(line) || line.IndexOf(',') < 0)
					continue;

				var parts = line.Split(new[] { ',' }, 2);
				result[parts[0].Trim()] = parts[1].Trim();
			}

			return result;
		}

	}
}
