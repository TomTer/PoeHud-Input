using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PoeHUD.Hud
{
	public class Settings
	{
		private static Dictionary<string, bool> bools;
		private static Dictionary<string, int> ints;
		private static Dictionary<string, float> floats;
		private static Dictionary<string, string> strings;
		private static Dictionary<string, Color> colors;
		public static bool GetBool(string setting)
		{
			return bools.ContainsKey(setting) && bools[setting];
		}
		public static int GetInt(string setting)
		{
			if (ints.ContainsKey(setting))
			{
				return ints[setting];
			}
			return 0;
		}
		public static float GetFloat(string setting)
		{
			if (floats.ContainsKey(setting))
			{
				return floats[setting];
			}
			return 0f;
		}
		public static string GetString(string setting)
		{
			if (strings.ContainsKey(setting))
			{
				return strings[setting];
			}
			return "";
		}
		public static Color GetColor(string setting)
		{
			if (colors.ContainsKey(setting))
			{
				return colors[setting];
			}
			return Color.White;
		}
		public static void SetBool(string setting, bool value)
		{
			if (bools.ContainsKey(setting))
			{
				bools[setting] = value;
			}
			else
			{
				bools.Add(setting, value);
			}
			SaveSettings();
		}
		public static void SetInt(string setting, int value)
		{
			if (ints.ContainsKey(setting))
			{
				ints[setting] = value;
			}
			else
			{
				ints.Add(setting, value);
			}
			SaveSettings();
		}
		public static void SetColor(string setting, Color color)
		{
			if (colors.ContainsKey(setting))
			{
				colors[setting] = color;
			}
			else
			{
				colors.Add(setting, color);
			}
			SaveSettings();
		}
		public static void SetString(string setting, string value)
		{
			if (strings.ContainsKey(setting))
			{
				strings[setting] = value;
			}
			else
			{
				strings.Add(setting, value);
			}
			SaveSettings();
		}
		private static void SaveSettings()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, bool> current in bools)
			{
				stringBuilder.AppendLine(string.Concat(new object[] { "bool ", current.Key, "=", current.Value }));
			}
			foreach (KeyValuePair<string, int> current2 in ints)
			{
				stringBuilder.AppendLine(string.Concat(new object[] { "int ", current2.Key, "=", current2.Value }));
			}
			foreach (KeyValuePair<string, float> current3 in floats)
			{
				stringBuilder.AppendLine(string.Concat(new object[] { "float ", current3.Key, "=", current3.Value }));
			}
			foreach (KeyValuePair<string, string> current4 in strings)
			{
				stringBuilder.AppendLine("string " + current4.Key + "=" + current4.Value);
			}
			foreach (KeyValuePair<string, Color> current5 in colors)
			{
				stringBuilder.AppendLine("color " + current5.Key + "=" + current5.Value.ToArgb().ToString("X"));
			}
			File.WriteAllText("config/settings.txt", stringBuilder.ToString());
		}
		public static bool LoadSettings()
		{
			bools = new Dictionary<string, bool>();
			ints = new Dictionary<string, int>();
			floats = new Dictionary<string, float>();
			strings = new Dictionary<string, string>();
			colors = new Dictionary<string, Color>();
			SetDefault();
			if (!File.Exists("config/settings.txt"))
			{
				MessageBox.Show("config/settings.txt does not exist!");
				return false;
			}
			string[] allLines = File.ReadAllLines("config/settings.txt");
			foreach (string text in allLines)
			{
				try
				{
					string text2 = text.Trim().Replace("\n", "").Replace("\r", "");
					if (string.IsNullOrWhiteSpace(text2)) continue;


					string[] allWords = text2.Split(new []{' '}, 2, StringSplitOptions.RemoveEmptyEntries);
					if (allWords.Length != 2)
					{
						Console.WriteLine("Invalid settings line: " + text2);
						continue;
					}

					string firstWord = allWords[0];
					string[] kv = allWords[1].Split(new []{'='}, 2, StringSplitOptions.RemoveEmptyEntries);
					if (kv.Length < 2)
					{
						Console.WriteLine("Invalid settings line: " + text2);
					}
					else
					{
						string key = kv[0].Trim();
						string value = kv[1].Trim();
						switch (firstWord.ToLowerInvariant())
						{
							case "bool":
								bools[key] = bool.Parse(value);
								break;
							case "int":
								ints[key] = int.Parse(value);
								break;
							case "float":
								floats[key] = float.Parse(value);
								break;
							case "string":
								strings[key] = value;
								break;
							case "color":
								colors[key] = Color.FromArgb(int.Parse(value, NumberStyles.HexNumber));
								break;
						}
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Error in settings line: " + text);
					return false;
				}
			}
			return true;
		}
		private static void SetDefault()
		{
			ints.Add("Menu.Size", 25);
			ints.Add("Menu.Length", 50);
			ints.Add("Menu.PositionWidth", 0);
			ints.Add("Menu.PositionHeight", 100);
			bools.Add("Window.RequireForeground", false);
			bools.Add("Window.ShowIngameMenu", true);
			bools.Add("ClientHacks", false);
			bools.Add("ClientHacks.Maphack", false);
			bools.Add("ClientHacks.Zoomhack", false);
			bools.Add("ClientHacks.Fullbright", false);
			bools.Add("ClientHacks.Particles", false);
			bools.Add("PreloadAlert", true);
			bools.Add("Healthbars", true);
			bools.Add("Healthbars.ShowES", false);
			bools.Add("Healthbars.ShowIncrements", true);
			bools.Add("Healthbars.ShowInTown", true);
			bools.Add("MinimapIcons", true);
			bools.Add("MinimapIcons.Monsters", true);
			bools.Add("MinimapIcons.Strongboxes", true);
			bools.Add("MinimapIcons.Chests", true);
			bools.Add("MinimapIcons.Minions", true);
			bools.Add("MinimapIcons.AlertedItems", true);
            bools.Add("MinimapIcons.Masters", true);
			bools.Add("ItemAlert", true);
			bools.Add("ItemAlert.Rares", true);
			bools.Add("ItemAlert.Uniques", true);
			bools.Add("ItemAlert.Maps", true);
			bools.Add("ItemAlert.Currency", true);
			bools.Add("ItemAlert.SkillGems", true);
			bools.Add("ItemAlert.QualitySkillGems", true);
			bools.Add("ItemAlert.RGB", false);
			bools.Add("ItemAlert.PlaySound", true);
			bools.Add("ItemAlert.ShowText", true);
			bools.Add("ItemAlert.Crafting", true);
			bools.Add("Tooltip", true);
			bools.Add("Tooltip.ShowItemLevel", true);
			bools.Add("MonsterTracker", true);
			bools.Add("MonsterTracker.ShowText", true);
			bools.Add("MonsterTracker.PlaySound", true);
			bools.Add("Healthbars.Players", true);
			bools.Add("Healthbars.Minions", true);
			bools.Add("Healthbars.Enemies", true);
			bools.Add("Healthbars.Enemies.Normal", true);
			bools.Add("Healthbars.Enemies.Magic", true);
			bools.Add("Healthbars.Enemies.Rare", true);
			bools.Add("Healthbars.Enemies.Unique", true);
			bools.Add("XphDisplay", true);
			bools.Add("ExitWithGame", true);
			bools.Add("DpsDisplay", true);
			ints.Add("PreloadAlert.FontSize", 12);
			ints.Add("PreloadAlert.BgAlpha", 180);
			ints.Add("XphDisplay.FontSize", 12);
			ints.Add("XphDisplay.BgAlpha", 180);
			ints.Add("ItemAlert.MinLinks", 5);
			ints.Add("ItemAlert.MinSockets", 6);
			ints.Add("ItemAlert.ShowText.FontSize", 16);
			ints.Add("ItemAlert.PositionHeight", 200);
			ints.Add("ItemAlert.PositionWidth", 0);
			ints.Add("MonsterTracker.ShowText.FontSize", 16);
			ints.Add("MonsterTracker.ShowText.BgAlpha", 128);
			ints.Add("Healthbars.Players.Width", 105);
			ints.Add("Healthbars.Players.Height", 25);
			ints.Add("Healthbars.Minions.Width", 105);
			ints.Add("Healthbars.Minions.Height", 25);
			ints.Add("Healthbars.Enemies.Normal.Width", 105);
			ints.Add("Healthbars.Enemies.Normal.Height", 25);
			ints.Add("Healthbars.Enemies.Magic.Width", 105);
			ints.Add("Healthbars.Enemies.Magic.Height", 25);
			ints.Add("Healthbars.Enemies.Rare.Width", 105);
			ints.Add("Healthbars.Enemies.Rare.Height", 25);
			ints.Add("Healthbars.Enemies.Unique.Width", 105);
			ints.Add("Healthbars.Enemies.Unique.Height", 25);
			strings.Add("Window.Name", "PoeHUD");
			colors.Add("Healthbars.Players.Color", Color.FromArgb(255, 0, 128, 0));
			colors.Add("Healthbars.Minions.Color", Color.FromArgb(255, 144, 238, 144));
			colors.Add("Healthbars.Players.Outline", Color.FromArgb(0));
			colors.Add("Healthbars.Minions.Outline", Color.FromArgb(0));
			colors.Add("Healthbars.Enemies.Normal.Outline", Color.FromArgb(0));
			colors.Add("Healthbars.Enemies.Magic.Outline", Color.FromArgb(255, 136, 136, 255));
			colors.Add("Healthbars.Enemies.Rare.Outline", Color.FromArgb(255, 255, 255, 119));
			colors.Add("Healthbars.Enemies.Unique.Outline", Color.FromArgb(255, 175, 96, 37));
			colors.Add("Healthbars.Enemies.Normal.Color", Color.FromArgb(255, 255, 0, 0));
			colors.Add("Healthbars.Enemies.Magic.Color", Color.FromArgb(255, 255, 0, 0));
			colors.Add("Healthbars.Enemies.Rare.Color", Color.FromArgb(255, 255, 0, 0));
			colors.Add("Healthbars.Enemies.Unique.Color", Color.FromArgb(255, 255, 0, 0));
		}
	}
}
