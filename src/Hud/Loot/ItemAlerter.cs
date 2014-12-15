using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;
using PoeHUD.Settings;
using SlimDX.Direct3D9;
using Entity = PoeHUD.Poe.Entity;

namespace PoeHUD.Hud.Loot
{
	public class ItemAlerter : HUDPluginBase, EntityListObserver, HUDPluginWithMapIcons
	{
		private HashSet<long> playedSoundsCache;
		private Dictionary<EntityWrapper, AlertDrawStyle> currentAlerts;

		private Dictionary<string, CraftingBase> craftingBases;
		private HashSet<string> currencyNames;

		public class ItemAlertSettings : SettingsForModule
		{
			public Setting<bool> PlaySound = new Setting<bool>("Play Sound", true);
			public Setting<bool> AlertOfCraftingBases = new Setting<bool>("Crafting Bases", true);
			public Setting<bool> ShowText = new Setting<bool>("Show Text", true);
			public SettingIntRange TextFontSize = new SettingIntRange("Font Size", 7, 30, 14);
			public Setting<bool> AlertOfRares = new Setting<bool>("Rares", true);
			public Setting<bool> AlertOfUniques = new Setting<bool>("Uniques", true);
			public Setting<bool> AlertOfMaps = new Setting<bool>("Maps", true);
			public ItemAlertSocketSettings Sockets = new ItemAlertSocketSettings();
			public GemSettings AlertOfGems = new GemSettings();
			public Setting<bool> AlertOfCurrency = new Setting<bool>("Currency", true);
			
			public ItemAlertSettings() : base("Item Alert") {}
		}

		public class GemSettings : SettingsForModule
		{
			public GemSettings() : base("Skill Gems")
			{
			}

			public SettingIntRange MinQuality = new SettingIntRange("Minimum Quality", 0, 20);
		}

		public class ItemAlertSocketSettings : SettingsBlock
		{
			public ItemAlertSocketSettings() : base("Sockets") { }

			public SettingIntRange MinLinksToAlert = new SettingIntRange("Minimum Links", 2, 6, 5);
			public Setting<bool> AlertOfRgb = new Setting<bool>("Chrome Link", true);
			public SettingIntRange MinSocketsToAlert = new SettingIntRange("Minimum Sockets", 1, 6, 6);
		}

		public ItemAlertSettings Settings = new ItemAlertSettings();
		public override void OnEnable()
		{
			playedSoundsCache = new HashSet<long>();
			currentAlerts = new Dictionary<EntityWrapper, AlertDrawStyle>();
			currencyNames = LoadCurrency("config/currency.txt");
			craftingBases = CraftingBase.LoadFromFile("config/crafting_bases.txt");
		}

		public override SettingsForModule SettingsNode
		{
			get { return Settings; }
		}

		public void EntityRemoved(EntityWrapper entity)
		{
			currentAlerts.Remove(entity);
		}

		public void EntityAdded(EntityWrapper entity)
		{
			if (!Settings.Enabled || currentAlerts.ContainsKey(entity))
			{
				return;
			}
			if (!entity.HasComponent<WorldItem>()) return;

			EntityWrapper item = new EntityWrapper(model, entity.GetComponent<WorldItem>().ItemEntity);
			ItemUsefulProperties props = EvaluateItem(item);

			if (!props.IsWorthAlertingPlayer(Settings, currencyNames)) return;

			AlertDrawStyle drawStyle = props.GetDrawStyle();
			currentAlerts.Add(entity, drawStyle);
			drawStyle.IconForMap = new MapIcon(entity, new HudTexture("minimap_default_icon.png", drawStyle.color), 8) { Type = MapIcon.IconType.Item };

			if (Settings.PlaySound && drawStyle.soundToPlay != null && !playedSoundsCache.Contains(entity.LongId))
			{
				playedSoundsCache.Add(entity.LongId);
				drawStyle.soundToPlay.Play();
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
			if (craftingBases.TryGetValue(ip.Name, out craftingBase) && Settings.AlertOfCraftingBases)
				ip.IsCraftingBase = ip.ItemLevel >= craftingBase.MinItemLevel 
					&& ip.Quality >= craftingBase.MinQuality
					&& (craftingBase.Rarities == null || craftingBase.Rarities.Contains(ip.Rarity));

			return ip;
		}

		public override void OnAreaChange(AreaController area)
		{
			playedSoundsCache.Clear();
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.ShowText) return;

			var playerPos = model.Player.GetComponent<Positioned>().GridPos;

			Vec2 rightTopAnchor = mountPoints[UiMountPoint.UnderMinimap];
			int y = rightTopAnchor.Y;
			int fontSize = Settings.TextFontSize;
			
			const int VMargin = 2;
			foreach (KeyValuePair<EntityWrapper, AlertDrawStyle> kv in currentAlerts.Where(a => a.Key.IsValid))
			{
				string text = GetItemName(kv);
				if( null == text ) continue;

				Vec2 itemPos = kv.Key.GetComponent<Positioned>().GridPos;
				var delta = itemPos - playerPos;

				Vec2 vPadding = new Vec2(5, 2);
				Vec2 itemDrawnSize = drawItem(rc, kv.Value, delta, rightTopAnchor.X, y, vPadding, text, fontSize);
				y += itemDrawnSize.Y + VMargin;
			}
			
		}

		public IEnumerable<MapIcon> GetIcons()
		{
			List<EntityWrapper> toRemove = new List<EntityWrapper>();
			foreach (KeyValuePair<EntityWrapper, AlertDrawStyle> kv in currentAlerts)
			{
				if (kv.Value.IconForMap.IsEntityStillValid())
					yield return kv.Value.IconForMap;
				else
					toRemove.Add(kv.Key);
			}
			foreach (EntityWrapper wrapper in toRemove)
			{
				currentAlerts.Remove(wrapper);
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

		private HashSet<string> LoadCurrency(string fileName)
		{
			if (!File.Exists(fileName))
				return null;
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string[] array = File.ReadAllLines(fileName);
			foreach (string text2 in array.Where(text2 => !string.IsNullOrWhiteSpace(text2)))
			{
				hashSet.Add(text2.Trim().ToLowerInvariant());
			}
			return hashSet;
		}
	}
}
