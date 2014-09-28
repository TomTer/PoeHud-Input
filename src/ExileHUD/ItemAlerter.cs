using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ExileHUD.EntityComponents;
using ExileHUD.ExileBot;
using ExileHUD.Framework;
using ExileHUD.Game;
using SlimDX.Direct3D9;

namespace ExileHUD.ExileHUD
{

	public class ItemUsefulProperties {

		public string Name;
		public string DisplayName { get { return (Quality > 0 ? "Superior " : String.Empty) + Name; } }
		public bool IsCurrency;
		public bool IsSkillGem;
		public ItemRarity Rarity;
		public bool WorthChrome;
		
		public int NumSockets;
		public int NumLinks;

		public int ItemLevel;
		public int Quality;
		public int MapLevel;

		public bool IsWorthAlertingPlayer(HashSet<string> currencyNames, Dictionary<string, ItemAlerter.CraftingBase> craftingBases)
		{			
			if( Rarity == ItemRarity.Rare && Settings.GetBool("ItemAlert.Rares"))
				return true;
			if( Rarity == ItemRarity.Unique && Settings.GetBool("ItemAlert.Uniques"))
				return true;
			if( MapLevel > 0 && Settings.GetBool("ItemAlert.Maps"))
				return true;
			if( NumLinks >= Settings.GetInt("ItemAlert.MinLinks"))
				return true;
			if( IsCurrency && Settings.GetBool("ItemAlert.Currency")) {
				if (currencyNames == null) {
					if( !Name.Contains("Portal") && Name.Contains("Wisdom") )
						return true;
				}
				else if (currencyNames.Contains(Name))
					return true;
			}

			if (IsSkillGem && Settings.GetBool("ItemAlert.SkillGems"))
				return true;
			if (WorthChrome && Settings.GetBool("ItemAlert.RGB"))
				return true;
			if (NumSockets >= Settings.GetInt("ItemAlert.MinSockets"))
				return true;

			if (craftingBases.ContainsKey(Name) && Settings.GetBool("ItemAlert.Crafting"))
			{
				ItemAlerter.CraftingBase craftingBase = craftingBases[Name];
				if (ItemLevel >= craftingBase.MinItemLevel && Quality >= craftingBase.MinQuality)
				{
					return true;
				}
				
			}
			return false;
		}

		internal AlertDrawStyle GetDrawStyle()
		{
			System.Drawing.Color color = Color.White;
			switch(this.Rarity) {
				case ItemRarity.White : color = Color.White; break;
				case ItemRarity.Magic: color = AlertDrawStyle.MagicColor; break;
				case ItemRarity.Rare : color = AlertDrawStyle.RareColor; break;
				case ItemRarity.Unique : color = AlertDrawStyle.UniqueColor; break;
			}
			if( IsSkillGem )
				color = AlertDrawStyle.SkillGemColor;
			if (IsCurrency)
				color = AlertDrawStyle.CurrencyColor;

			int iconIndex = -1;
			if (WorthChrome)
				iconIndex = 1;
			if (NumSockets == 6)
				iconIndex = 0;

			return new AlertDrawStyle()
			{
				color = color,
				FrameWidth = MapLevel > 0 ? 1 : 0,
				Text = DisplayName,
				IconIndex = iconIndex
			};
		}
	}


	public class AlertDrawStyle
	{
		public static readonly Color MagicColor = Color.FromArgb(136, 136, 255);
		public static readonly Color RareColor = Color.FromArgb(255, 255, 119);
		public static readonly Color CurrencyColor = Color.FromArgb(170, 158, 130);
		public static readonly Color UniqueColor = Color.FromArgb(175, 96, 37);
		public static readonly Color SkillGemColor = Color.FromArgb(26, 162, 155);


		public Color color;
		public int FrameWidth;
		public string Text;
		public int IconIndex;
	}

	public class ItemAlerter : HUDPlugin
	{
		public struct CraftingBase
		{
			public string Name;
			public int MinItemLevel;
			public int MinQuality;
		}
		private HashSet<long> playedSoundsCache;
		private Dictionary<Entity, AlertDrawStyle> currentAlerts;
		private Dictionary<string, ItemAlerter.CraftingBase> craftingBases;
		private HashSet<string> currencyNames;
		public override void OnEnable()
		{
			this.playedSoundsCache = new HashSet<long>();
			this.currentAlerts = new Dictionary<Entity, AlertDrawStyle>();
			this.currencyNames = this.LoadCurrency();
			this.craftingBases = this.LoadCraftingBases();
			this.poe.Area.OnAreaChange += this.CurrentArea_OnAreaChange;
			this.poe.EntityList.OnEntityAdded += new EntityEvent(this.EntityList_OnEntityAdded);
			this.poe.EntityList.OnEntityRemoved += new EntityEvent(this.EntityList_OnEntityRemoved);
		}
		public override void OnDisable()
		{
		}
		private void EntityList_OnEntityRemoved(Entity entity)
		{
			if (this.currentAlerts.ContainsKey(entity))
			{
				this.currentAlerts.Remove(entity);
			}
		}
		private void EntityList_OnEntityAdded(Entity entity)
		{
			if (!Settings.GetBool("ItemAlert") || this.currentAlerts.ContainsKey(entity))
			{
				return;
			}
			if (entity.HasComponent<WorldItem>())
			{
				Entity item = new Entity(this.poe, entity.GetComponent<WorldItem>().ItemEntity);
				ItemUsefulProperties props = this.EvaluateItem(item);

				if (props.IsWorthAlertingPlayer(currencyNames, craftingBases))
				{
					this.DoAlert(entity, props);
				}
			}
		}


		private ItemUsefulProperties EvaluateItem(Entity item)
		{
			ItemUsefulProperties ip = new ItemUsefulProperties();

			Mods mods = item.GetComponent<Mods>();
			Sockets socks = item.GetComponent<Sockets>();
			Map map = item.HasComponent<Map>() ? item.GetComponent<Map>() : null;
			SkillGem sk = item.HasComponent<SkillGem>() ? item.GetComponent<SkillGem>() : null;
			Quality q = item.HasComponent<Quality>() ? item.GetComponent<Quality>() : null;

			ip.Name = this.poe.Files.BaseItemTypes.Translate(item.Path);
			ip.ItemLevel = mods.ItemLevel;
			ip.NumLinks = socks.LargestLinkSize;
			ip.NumSockets = socks.NumberOfSockets;
			ip.Rarity = mods.ItemRarity;
			ip.MapLevel = map == null ? 0 : 1;
			ip.IsCurrency = item.Path.Contains("Currency");
			ip.IsSkillGem = sk != null;
			ip.Quality = q == null ? 0 : q.ItemQuality;
			ip.WorthChrome = socks != null && socks.IsRGB;
			return ip;
		}

		private void DoAlert(Entity entity, ItemUsefulProperties ip)
		{
			AlertDrawStyle drawStyle = ip.GetDrawStyle();
			this.currentAlerts.Add(entity, drawStyle);
			this.overlay.MinimapRenderer.AddIcon(new ItemMinimapIcon(entity, "minimap_default_icon.png", drawStyle.color, 8));
			if (Settings.GetBool("ItemAlert.PlaySound") && !this.playedSoundsCache.Contains(entity.LongId))
			{
				this.playedSoundsCache.Add(entity.LongId);
				Sounds.AlertSound.Play();
			}
		}
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			this.playedSoundsCache.Clear();
		}
		public override void Render(RenderingContext rc)
		{
			if (!Settings.GetBool("ItemAlert") || !Settings.GetBool("ItemAlert.ShowText"))
			{
				return;
			}
			Rect clientRect = this.poe.Internal.game.IngameState.IngameUi.Minimap.SmallMinimap.GetClientRect();
			Vec2 vec = new Vec2(clientRect.X + clientRect.W, clientRect.Y + clientRect.H);
			
			int y = vec.Y;
			int fontSize = Settings.GetInt("ItemAlert.ShowText.FontSize");
			foreach (KeyValuePair<Entity, AlertDrawStyle> kv in this.currentAlerts)
			{
				if (kv.Key.IsValid)
				{
					Poe_UI_EntityLabel labelFromEntity = this.poe.GetLabelFromEntity(kv.Key);
					string text;
					if (labelFromEntity == null)
					{
						Poe_Entity itemEntity = kv.Key.GetComponent<WorldItem>().ItemEntity;
						if (!itemEntity.IsValid)
						{
							continue;
						}
						text = kv.Value.Text;
					}
					else
					{
						text = labelFromEntity.Text;
					}

					var drawStyle = kv.Value;
					int frameWidth = drawStyle.FrameWidth;
					Vec2 vPadding = new Vec2(frameWidth + 5, frameWidth);
					int frameMargin = frameWidth + 2;

					Vec2 textPos = new Vec2(vec.X - vPadding.X, y + vPadding.Y);

					var vTextFrame = rc.AddTextWithHeightAndOutline(textPos, text, drawStyle.color, Color.Black, fontSize, DrawTextFormat.Right);
					if( frameWidth > 0)
					{
						rc.AddFrame(new Rect(vec.X - vTextFrame.X - 2 * vPadding.X, y, vTextFrame.X + 2 * vPadding.X, vTextFrame.Y + 2 * vPadding.Y), kv.Value.color, frameWidth);
					}

					if (drawStyle.IconIndex >= 0)
					{
						const float iconsInSprite = 2;
						int iconSize = vTextFrame.Y;
						Rect iconPos = new Rect(textPos.X - iconSize - vTextFrame.X, textPos.Y, iconSize, iconSize);
						RectUV uv = new RectUV(drawStyle.IconIndex / iconsInSprite, 0, (drawStyle.IconIndex + 1)/ iconsInSprite, 1);
						rc.AddSprite("item_icons.png", iconPos, uv);
					}
					y += vTextFrame.Y + 2 * vPadding.Y + frameMargin;
				}
			}
		}
		private Dictionary<string, ItemAlerter.CraftingBase> LoadCraftingBases()
		{
			if (!File.Exists("config/crafting_bases.txt"))
			{
				return new Dictionary<string, ItemAlerter.CraftingBase>();
			}
			Dictionary<string, ItemAlerter.CraftingBase> dictionary = new Dictionary<string, ItemAlerter.CraftingBase>(StringComparer.OrdinalIgnoreCase);
			string[] array = File.ReadAllLines("config/crafting_bases.txt");
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				try
				{
					string text2 = text.Trim();
					if (!string.IsNullOrWhiteSpace(text2) && !text2.StartsWith("#"))
					{
						string[] array2 = text2.Split(new char[]
						{
							','
						});
						if (array2.Length != 0 && array2.Length <= 3)
						{
							string text3 = array2[0].Trim().ToLowerInvariant();
							int minItemLevel = 0;
							if (array2.Length >= 2)
							{
								minItemLevel = int.Parse(array2[1].Trim());
							}
							int minQuality = 0;
							if (array2.Length >= 3)
							{
								minQuality = int.Parse(array2[2].Trim());
							}
							dictionary.Add(text3, new ItemAlerter.CraftingBase
							{
								Name = text3,
								MinItemLevel = minItemLevel,
								MinQuality = minQuality
							});
						}
					}
				}
				catch (Exception)
				{
					throw new Exception("Error parsing config/whites.txt at line " + text);
				}
			}
			return dictionary;
		}
		private HashSet<string> LoadCurrency()
		{
			if (!File.Exists("config/currency.txt"))
			{
				return null;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string[] array = File.ReadAllLines("config/currency.txt");
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = text.Trim();
				if (!string.IsNullOrWhiteSpace(text2))
				{
					hashSet.Add(text2.ToLowerInvariant());
				}
			}
			return hashSet;
		}
	}
}
