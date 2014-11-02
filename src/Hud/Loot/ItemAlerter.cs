using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Hud.Icons;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;
using SlimDX.Direct3D9;
using Entity = PoeHUD.Poe.Entity;

namespace PoeHUD.Hud.Loot
{
	public class ItemAlerter : HUDPluginBase, EntityListObserver, HUDPluginWithMapIcons
	{
		private HashSet<long> playedSoundsCache;
		private Dictionary<EntityWrapper, AlertDrawStyle> currentAlerts;
		private Dictionary<EntityWrapper, MapIcon> currentIcons;

		private Dictionary<string, CraftingBase> craftingBases;
		private HashSet<string> currencyNames;

		public override void OnEnable()
		{
			playedSoundsCache = new HashSet<long>();
			currentAlerts = new Dictionary<EntityWrapper, AlertDrawStyle>();
			currentIcons = new Dictionary<EntityWrapper, MapIcon>();
			currencyNames = LoadCurrency();
			craftingBases = LoadCraftingBases();

			model.Area.OnAreaChange += CurrentArea_OnAreaChange;
		}
		public override void OnDisable()
		{
			model.Area.OnAreaChange -= CurrentArea_OnAreaChange;
		}

		public void EntityRemoved(EntityWrapper entity)
		{
			currentAlerts.Remove(entity);
			currentIcons.Remove(entity);
		}

		public void EntityAdded(EntityWrapper entity)
		{
			if (!Settings.GetBool("ItemAlert") || currentAlerts.ContainsKey(entity))
			{
				return;
			}
			if (entity.HasComponent<WorldItem>())
			{
				EntityWrapper item = new EntityWrapper(model, entity.GetComponent<WorldItem>().ItemEntity);
				ItemUsefulProperties props = EvaluateItem(item);

				if (props.IsWorthAlertingPlayer(currencyNames))
				{
					AlertDrawStyle drawStyle = props.GetDrawStyle();
					currentAlerts.Add(entity, drawStyle);
					currentIcons[entity] = new MapIcon(entity, new HudTexture("minimap_default_icon.png", drawStyle.color), 8);

					if (Settings.GetBool("ItemAlert.PlaySound") && !playedSoundsCache.Contains(entity.LongId))
					{
						playedSoundsCache.Add(entity.LongId);
						Sounds.AlertSound.Play();
					}
				}
			}
		}


		private ItemUsefulProperties EvaluateItem(EntityWrapper item)
		{
			ItemUsefulProperties ip = new ItemUsefulProperties();

			Mods mods = item.GetComponent<Mods>();
			Sockets socks = item.GetComponent<Sockets>();
			Map map = item.HasComponent<Map>() ? item.GetComponent<Map>() : null;
			SkillGem sk = item.HasComponent<SkillGem>() ? item.GetComponent<SkillGem>() : null;
			Quality q = item.HasComponent<Quality>() ? item.GetComponent<Quality>() : null;

			ip.Name = model.Files.BaseItemTypes.Translate(item.Path);
			ip.ItemLevel = mods.ItemLevel;
			ip.NumLinks = socks.LargestLinkSize;
			ip.NumSockets = socks.NumberOfSockets;
			ip.Rarity = mods.ItemRarity;
			ip.MapLevel = map == null ? 0 : 1;
			ip.IsCurrency = item.Path.Contains("Currency");
			ip.IsSkillGem = sk != null;
			ip.Quality = q == null ? 0 : q.ItemQuality;
			ip.WorthChrome = socks != null && socks.IsRGB;

			ip.IsVaalFragment = item.Path.Contains("VaalFragment");

			CraftingBase craftingBase;
			if (craftingBases.TryGetValue(ip.Name, out craftingBase) && Settings.GetBool("ItemAlert.Crafting"))
				ip.IsCraftingBase = ip.ItemLevel >= craftingBase.MinItemLevel 
					&& ip.Quality >= craftingBase.MinQuality
					&& (craftingBase.Rarities == null || craftingBase.Rarities.Contains(ip.Rarity));

			return ip;
		}

		private void CurrentArea_OnAreaChange(AreaController area)
		{
			playedSoundsCache.Clear();
			currentIcons.Clear();
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.GetBool("ItemAlert") || !Settings.GetBool("ItemAlert.ShowText"))
			{
				return;
			}


			var playerPos = model.Player.GetComponent<Positioned>().GridPos;


			Vec2 rightTopAnchor = mountPoints[UiMountPoint.UnderMinimap];
			int y = rightTopAnchor.Y;
			int fontSize = Settings.GetInt("ItemAlert.ShowText.FontSize");
			
			const int vMargin = 2;
			foreach (KeyValuePair<EntityWrapper, AlertDrawStyle> kv in currentAlerts)
			{
				if (!kv.Key.IsValid) continue;

				string text = GetItemName(kv);
				if( null == text ) continue;

				Vec2 itemPos = kv.Key.GetComponent<Positioned>().GridPos;
				var delta = itemPos - playerPos;

				Vec2 vPadding = new Vec2(5, 2);
				Vec2 itemDrawnSize = drawItem(rc, kv.Value, delta, rightTopAnchor.X, y, vPadding, text, fontSize);
				y += itemDrawnSize.Y + vMargin;
			}
			
		}

		public IEnumerable<MapIcon> GetIcons()
		{
			List<EntityWrapper> toRemove = new List<EntityWrapper>();
			foreach (KeyValuePair<EntityWrapper, MapIcon> kv in currentIcons)
			{
				if (kv.Value.IsEntityStillValid())
					yield return kv.Value;
				else
					toRemove.Add(kv.Key);
			}
			foreach (EntityWrapper wrapper in toRemove)
			{
				currentIcons.Remove(wrapper);
			}
		}

		private static Vec2 drawItem(RenderingContext rc, AlertDrawStyle drawStyle, Vec2 delta, int x, int y, Vec2 vPadding, string text,
			int fontSize)
		{
			// collapse padding when there's a frame
			vPadding.X -= drawStyle.FrameWidth;
			vPadding.Y -= drawStyle.FrameWidth;
			// item will appear to have equal size

			double phi;
			var distance = delta.GetPolarCoordinates(out phi);


			//text = text + " @ " + (int)distance + " : " + (int)(phi / Math.PI * 180)  + " : " + xSprite;

			int compassOffset = fontSize + 8;
			Vec2 textPos = new Vec2(x - vPadding.X - compassOffset, y + vPadding.Y);
			Vec2 vTextSize = rc.AddTextWithHeight(textPos, text, drawStyle.color, fontSize, DrawTextFormat.Right);

			int iconSize =  drawStyle.IconIndex >= 0 ? vTextSize.Y : 0;

			int fullHeight = vTextSize.Y + 2 * vPadding.Y + 2 * drawStyle.FrameWidth;
			int fullWidth = vTextSize.X + 2 * vPadding.X + iconSize + 2 * drawStyle.FrameWidth + compassOffset;
			rc.AddBox(new Rect(x - fullWidth, y, fullWidth - compassOffset, fullHeight), Color.FromArgb(180, 0, 0, 0));

			var rectUV = GetDirectionsUv(phi, distance);
			rc.AddSprite("directions.png", new Rect(x - vPadding.X - compassOffset + 6, y + vPadding.Y, vTextSize.Y, vTextSize.Y), rectUV);

			if (iconSize > 0)
			{
				const float iconsInSprite = 4;

				Rect iconPos = new Rect(textPos.X - iconSize - vTextSize.X, textPos.Y, iconSize, iconSize);
				RectUV uv = new RectUV(drawStyle.IconIndex/iconsInSprite, 0, (drawStyle.IconIndex + 1)/iconsInSprite, 1);
				rc.AddSprite("item_icons.png", iconPos, uv);
			}
			if (drawStyle.FrameWidth > 0)
			{
				Rect frame = new Rect(x - fullWidth, y, fullWidth - compassOffset , fullHeight);
				rc.AddFrame(frame, drawStyle.color, drawStyle.FrameWidth);
			}
			return new Vec2(fullWidth, fullHeight);
		}

		private string GetItemName(KeyValuePair<EntityWrapper, AlertDrawStyle> kv)
		{
			string text;
			EntityLabel labelFromEntity = model.GetLabelForEntity(kv.Key);

			if (labelFromEntity == null)
			{
				Entity itemEntity = kv.Key.GetComponent<WorldItem>().ItemEntity;
				if (!itemEntity.IsValid)
					return null;
				text = kv.Value.Text;
			}
			else
			{
				text = labelFromEntity.Text;
			}
			return text;
		}

		private Dictionary<string, CraftingBase> LoadCraftingBases()
		{
			if (!File.Exists("config/crafting_bases.txt"))
			{
				return new Dictionary<string, CraftingBase>();
			}
			Dictionary<string, CraftingBase> dictionary = new Dictionary<string, CraftingBase>(StringComparer.OrdinalIgnoreCase);
			List<string> parseErrors = new List<string>();
			string[] array = File.ReadAllLines("config/crafting_bases.txt");
			foreach (
				string text2 in
					array.Select(text => text.Trim()).Where(text2 => !string.IsNullOrWhiteSpace(text2) && !text2.StartsWith("#")))
			{
				string[] parts = text2.Split(new[]{','});
				string itemName = parts[0].Trim();

				CraftingBase item = new CraftingBase() {Name = itemName};

				int tmpVal = 0;
				if (parts.Length > 1 && int.TryParse(parts[1], out tmpVal))
					item.MinItemLevel = tmpVal;

				if (parts.Length > 2 && int.TryParse(parts[2], out tmpVal))
					item.MinQuality = tmpVal;

				const int RarityPosition = 3;
				if (parts.Length > RarityPosition)
				{
					item.Rarities = new ItemRarity[parts.Length - 3];
					for (int i = RarityPosition; i < parts.Length; i++)
					{
						if (!Enum.TryParse(parts[i], true, out item.Rarities[i - RarityPosition]))
						{
							parseErrors.Add("Incorrect rarity definition at line: " + text2);
							item.Rarities = null;
						}
					}
				}

				if( !dictionary.ContainsKey(itemName))
					dictionary.Add(itemName, item);
				else
					parseErrors.Add("Duplicate definition for item was ignored: " + text2);
			}

			if(parseErrors.Any())
				throw new Exception("Error parsing config/crafting_bases.txt \r\n" + string.Join(Environment.NewLine, parseErrors) + Environment.NewLine + Environment.NewLine);

			return dictionary;
		}
		private HashSet<string> LoadCurrency()
		{
			if (!File.Exists("config/currency.txt"))
				return null;
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
