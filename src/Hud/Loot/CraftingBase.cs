using PoeHUD.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PoeHUD.Hud.Loot
{
	public struct CraftingBase
	{
		public string Name;
		public int MinItemLevel;
		public int MinQuality;
		public Rarity[] Rarities;


		public static Dictionary<string, CraftingBase> LoadFromFile(string fileName)
		{
			if (!File.Exists(fileName))
			{
				return new Dictionary<string, CraftingBase>();
			}
			Dictionary<string, CraftingBase> dictionary = new Dictionary<string, CraftingBase>(StringComparer.OrdinalIgnoreCase);
			List<string> parseErrors = new List<string>();
			string[] array = File.ReadAllLines(fileName);
			foreach (string text2 in array.Select(text => text.Trim()).Where(text2 => !string.IsNullOrWhiteSpace(text2) && !text2.StartsWith("#")))
			{
				string[] parts = text2.Split(new[] { ',' });
				string itemName = parts[0].Trim();

				CraftingBase item = new CraftingBase{ Name = itemName };

				int tmpVal;
				if (parts.Length > 1 && int.TryParse(parts[1], out tmpVal))
					item.MinItemLevel = tmpVal;

				if (parts.Length > 2 && int.TryParse(parts[2], out tmpVal))
					item.MinQuality = tmpVal;

				const int RarityPosition = 3;
				if (parts.Length > RarityPosition)
				{
					item.Rarities = new Rarity[parts.Length - 3];
					for (int i = RarityPosition; i < parts.Length; i++)
					{
						if (!Enum.TryParse(parts[i], true, out item.Rarities[i - RarityPosition]))
						{
							parseErrors.Add("Incorrect rarity definition at line: " + text2);
							item.Rarities = null;
						}
					}
				}

				if (!dictionary.ContainsKey(itemName))
					dictionary.Add(itemName, item);
				else
					parseErrors.Add("Duplicate definition for item was ignored: " + text2);
			}

			if (parseErrors.Any())
				throw new Exception("Error parsing config/crafting_bases.txt \r\n" + string.Join(Environment.NewLine, parseErrors) + Environment.NewLine + Environment.NewLine);

			return dictionary;
		}
	}


}