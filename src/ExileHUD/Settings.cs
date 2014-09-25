using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ExileHUD.ExileHUD
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
			return Settings.bools.ContainsKey(setting) && Settings.bools[setting];
		}
		public static int GetInt(string setting)
		{
			if (Settings.ints.ContainsKey(setting))
			{
				return Settings.ints[setting];
			}
			return 0;
		}
		public static float GetFloat(string setting)
		{
			if (Settings.floats.ContainsKey(setting))
			{
				return Settings.floats[setting];
			}
			return 0f;
		}
		public static string GetString(string setting)
		{
			if (Settings.strings.ContainsKey(setting))
			{
				return Settings.strings[setting];
			}
			return "";
		}
		public static Color GetColor(string setting)
		{
			if (Settings.colors.ContainsKey(setting))
			{
				return Settings.colors[setting];
			}
			return Color.White;
		}
		public static void SetBool(string setting, bool value)
		{
			if (Settings.bools.ContainsKey(setting))
			{
				Settings.bools[setting] = value;
			}
			else
			{
				Settings.bools.Add(setting, value);
			}
			Settings.SaveSettings();
		}
		public static void SetInt(string setting, int value)
		{
			if (Settings.ints.ContainsKey(setting))
			{
				Settings.ints[setting] = value;
			}
			else
			{
				Settings.ints.Add(setting, value);
			}
			Settings.SaveSettings();
		}
		public static void SetColor(string setting, Color color)
		{
			if (Settings.colors.ContainsKey(setting))
			{
				Settings.colors[setting] = color;
			}
			else
			{
				Settings.colors.Add(setting, color);
			}
			Settings.SaveSettings();
		}
		public static void SetString(string setting, string value)
		{
			if (Settings.strings.ContainsKey(setting))
			{
				Settings.strings[setting] = value;
			}
			else
			{
				Settings.strings.Add(setting, value);
			}
			Settings.SaveSettings();
		}
		private static void SaveSettings()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, bool> current in Settings.bools)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"bool ",
					current.Key,
					"=",
					current.Value
				}));
			}
			foreach (KeyValuePair<string, int> current2 in Settings.ints)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"int ",
					current2.Key,
					"=",
					current2.Value
				}));
			}
			foreach (KeyValuePair<string, float> current3 in Settings.floats)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"float ",
					current3.Key,
					"=",
					current3.Value
				}));
			}
			foreach (KeyValuePair<string, string> current4 in Settings.strings)
			{
				stringBuilder.AppendLine("string " + current4.Key + "=" + current4.Value);
			}
			foreach (KeyValuePair<string, Color> current5 in Settings.colors)
			{
				stringBuilder.AppendLine("color " + current5.Key + "=" + current5.Value.ToArgb().ToString("X"));
			}
			File.WriteAllText("config/settings.txt", stringBuilder.ToString());
		}
		public static bool LoadSettings()
		{
			Settings.bools = new Dictionary<string, bool>();
			Settings.ints = new Dictionary<string, int>();
			Settings.floats = new Dictionary<string, float>();
			Settings.strings = new Dictionary<string, string>();
			Settings.colors = new Dictionary<string, Color>();
			Settings.SetDefault();
			if (!File.Exists("config/settings.txt"))
			{
				MessageBox.Show("config/settings.txt does not exist!");
				return false;
			}
			string[] array = File.ReadAllLines("config/settings.txt");
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				try
				{
					string text2 = text.Trim().Replace("\n", "").Replace("\r", "");
					if (!string.IsNullOrWhiteSpace(text2))
					{
						string[] array3 = text2.Split(new char[]
						{
							' '
						});
						if (array3.Length != 2)
						{
							Console.WriteLine("Invalid settings line: " + text2);
						}
						else
						{
							string text3 = array3[0];
							string[] array4 = array3[1].Split(new char[]
							{
								'='
							});
							if (array4.Length != 2)
							{
								Console.WriteLine("Invalid settings line: " + text2);
							}
							else
							{
								string key = array4[0];
								string text4 = array4[1];
								string a;
								if ((a = text3) != null)
								{
									if (!(a == "bool"))
									{
										if (!(a == "int"))
										{
											if (!(a == "float"))
											{
												if (!(a == "string"))
												{
													if (a == "color")
													{
														Settings.colors[key] = Color.FromArgb(int.Parse(text4, NumberStyles.HexNumber));
													}
												}
												else
												{
													Settings.strings[key] = text4;
												}
											}
											else
											{
												Settings.floats[key] = float.Parse(text4);
											}
										}
										else
										{
											Settings.ints[key] = int.Parse(text4);
										}
									}
									else
									{
										Settings.bools[key] = bool.Parse(text4);
									}
								}
							}
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
			Settings.bools.Add("Window.RequireForeground", false);
			Settings.bools.Add("Window.ShowIngameMenu", true);
			Settings.bools.Add("ClientHacks", true);
			Settings.bools.Add("ClientHacks.Maphack", true);
			Settings.bools.Add("ClientHacks.Zoomhack", false);
			Settings.bools.Add("ClientHacks.Fullbright", false);
			Settings.bools.Add("PreloadAlert", true);
			Settings.bools.Add("Healthbars", true);
			Settings.bools.Add("Healthbars.ShowES", false);
			Settings.bools.Add("Healthbars.ShowIncrements", true);
			Settings.bools.Add("Healthbars.ShowInTown", true);
			Settings.bools.Add("MinimapIcons", true);
			Settings.bools.Add("MinimapIcons.Monsters", true);
			Settings.bools.Add("MinimapIcons.Strongboxes", true);
			Settings.bools.Add("MinimapIcons.Chests", true);
			Settings.bools.Add("MinimapIcons.Minions", true);
			Settings.bools.Add("MinimapIcons.AlertedItems", true);
			Settings.bools.Add("ItemAlert", true);
			Settings.bools.Add("ItemAlert.Rares", true);
			Settings.bools.Add("ItemAlert.Uniques", true);
			Settings.bools.Add("ItemAlert.Maps", true);
			Settings.bools.Add("ItemAlert.Currency", true);
			Settings.bools.Add("ItemAlert.SkillGems", true);
			Settings.bools.Add("ItemAlert.RGB", false);
			Settings.bools.Add("ItemAlert.PlaySound", true);
			Settings.bools.Add("ItemAlert.ShowText", true);
			Settings.bools.Add("ItemAlert.Crafting", true);
			Settings.bools.Add("Tooltip", true);
			Settings.bools.Add("Tooltip.ShowItemLevel", true);
			Settings.bools.Add("DangerAlert", true);
			Settings.bools.Add("DangerAlert.ShowText", true);
			Settings.bools.Add("DangerAlert.PlaySound", true);
			Settings.bools.Add("Healthbars.Players", true);
			Settings.bools.Add("Healthbars.Minions", true);
			Settings.bools.Add("Healthbars.Enemies", true);
			Settings.bools.Add("Healthbars.Enemies.Normal", true);
			Settings.bools.Add("Healthbars.Enemies.Magic", true);
			Settings.bools.Add("Healthbars.Enemies.Rare", true);
			Settings.bools.Add("Healthbars.Enemies.Unique", true);
			Settings.bools.Add("XphDisplay", true);
			Settings.ints.Add("PreloadAlert.FontSize", 12);
			Settings.ints.Add("PreloadAlert.BgAlpha", 180);
			Settings.ints.Add("XphDisplay.FontSize", 12);
			Settings.ints.Add("XphDisplay.BgAlpha", 180);
			Settings.ints.Add("ItemAlert.MinLinks", 5);
			Settings.ints.Add("ItemAlert.MinSockets", 6);
			Settings.ints.Add("ItemAlert.ShowText.FontSize", 16);
			Settings.ints.Add("DangerAlert.ShowText.FontSize", 16);
			Settings.ints.Add("DangerAlert.ShowText.BgAlpha", 128);
			Settings.ints.Add("Healthbars.Players.Width", 105);
			Settings.ints.Add("Healthbars.Players.Height", 25);
			Settings.ints.Add("Healthbars.Minions.Width", 105);
			Settings.ints.Add("Healthbars.Minions.Height", 25);
			Settings.ints.Add("Healthbars.Enemies.Normal.Width", 105);
			Settings.ints.Add("Healthbars.Enemies.Normal.Height", 25);
			Settings.ints.Add("Healthbars.Enemies.Magic.Width", 105);
			Settings.ints.Add("Healthbars.Enemies.Magic.Height", 25);
			Settings.ints.Add("Healthbars.Enemies.Rare.Width", 105);
			Settings.ints.Add("Healthbars.Enemies.Rare.Height", 25);
			Settings.ints.Add("Healthbars.Enemies.Unique.Width", 105);
			Settings.ints.Add("Healthbars.Enemies.Unique.Height", 25);
			Settings.strings.Add("Window.Name", "ExileHUD");
			Settings.colors.Add("Healthbars.Players.Color", Color.FromArgb(255, 0, 128, 0));
			Settings.colors.Add("Healthbars.Minions.Color", Color.FromArgb(255, 144, 238, 144));
			Settings.colors.Add("Healthbars.Players.Outline", Color.FromArgb(0));
			Settings.colors.Add("Healthbars.Minions.Outline", Color.FromArgb(0));
			Settings.colors.Add("Healthbars.Enemies.Normal.Outline", Color.FromArgb(0));
			Settings.colors.Add("Healthbars.Enemies.Magic.Outline", Color.FromArgb(255, 136, 136, 255));
			Settings.colors.Add("Healthbars.Enemies.Rare.Outline", Color.FromArgb(255, 255, 255, 119));
			Settings.colors.Add("Healthbars.Enemies.Unique.Outline", Color.FromArgb(255, 175, 96, 37));
			Settings.colors.Add("Healthbars.Enemies.Normal.Color", Color.FromArgb(255, 255, 0, 0));
			Settings.colors.Add("Healthbars.Enemies.Magic.Color", Color.FromArgb(255, 255, 0, 0));
			Settings.colors.Add("Healthbars.Enemies.Rare.Color", Color.FromArgb(255, 255, 0, 0));
			Settings.colors.Add("Healthbars.Enemies.Unique.Color", Color.FromArgb(255, 255, 0, 0));
		}
	}
}
