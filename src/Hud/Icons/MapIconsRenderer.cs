using System;
using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;
using PoeHUD.Settings;

namespace PoeHUD.Hud.Icons
{
	public class MapIconsRenderer : HUDPluginBase
	{
		private readonly Func<IEnumerable<MapIcon>> _getIcons;

		public class MapIconSettings : SettingsForModule
		{
			public MapIconSettings() : base("Map Icons")
			{
			}

			public MapSizeSetting ShowOn = new MapSizeSetting();
			public MonsterSetting Monsters = new MonsterSetting();
			public Setting<bool> Chests = new Setting<bool>("Chests", true);
			public Setting<bool> Masters = new Setting<bool>("Masters", true);
			public Setting<bool> Strongboxes = new Setting<bool>("Strongboxes", true);
			public Setting<bool> Items = new Setting<bool>("Valuable items", true);
		}

		public class MapSizeSetting : SettingsBlock
		{
			public Setting<bool> LargeMap = new Setting<bool>("Large map", true);
			public Setting<bool> MiniMap = new Setting<bool>("Minimap", true);
			public MapSizeSetting() : base(" Show on") { }
		}

		public class MonsterSetting : SettingsForModule
		{
			public Setting<bool> Normal = new Setting<bool>("Normal", true);
			public Setting<bool> Magic = new Setting<bool>("Magic", true);
			public Setting<bool> Rare = new Setting<bool>("Rare", true);
			public Setting<bool> Unique = new Setting<bool>("Unique", true);
			public MonsterSetting() : base("Monsters") { }
		}

		private readonly MapIconSettings Settings = new MapIconSettings();
		public override SettingsForModule SettingsNode { get { return Settings; } }
		public MapIconsRenderer(Func<IEnumerable<MapIcon>> gatherMapIcons)
		{
			_getIcons = gatherMapIcons;
			Settings.Group = "Map Icons";
		}

		public override void OnEnable()
		{
		}
		public override void OnDisable()
		{
		}

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!model.InGame)
				return;

			var mapRoot = model.Internal.IngameState.IngameUi.Minimap;

			if( Settings.ShowOn.MiniMap && mapRoot.SmallMinimap.IsVisible )
				RenderIconsOnMiniMap(rc, mapRoot.SmallMinimap);

			if ( Settings.ShowOn.LargeMap && mapRoot.OrangeWords.IsVisible)
				RednerIconsOnLargeMap(rc);
		}

		bool ShouldSkipIcon(MapIcon icon)
		{
			if (icon.Type == MapIcon.IconType.Chest && !Settings.Chests)
				return true;
			if (icon.Type == MapIcon.IconType.Strongbox && !Settings.Strongboxes)
				return true;
			if (icon.Type == MapIcon.IconType.Master && !Settings.Masters)
				return true;
			if (icon.Type == MapIcon.IconType.Monster) {
				if( !Settings.Monsters )
					return true;
				switch(icon.Rarity) {
					case Game.Rarity.White: if( !Settings.Monsters.Normal ) return true; break;
					case Game.Rarity.Magic: if (!Settings.Monsters.Magic) return true; break;
					case Game.Rarity.Rare: if (!Settings.Monsters.Rare) return true; break;
					case Game.Rarity.Unique: if (!Settings.Monsters.Unique) return true; break;
				}
			}
			if (icon.Type == MapIcon.IconType.Item && !Settings.Items)
				return true;
			
			return icon.ShouldSkip();
		}

		private void RenderIconsOnMiniMap(RenderingContext rc, Element smallMinimap)
		{
			Vec2 playerPos = model.Player.GetComponent<Positioned>().GridPos;
			float pPosZ = model.Player.GetComponent<Render>().Z;

			const float scale = 240f;
			Rect clientRect = smallMinimap.GetClientRect();
			Vec2 minimapCenter = new Vec2(clientRect.X + clientRect.W/2, clientRect.Y + clientRect.H/2);
			double diag = Math.Sqrt(clientRect.W*clientRect.W + clientRect.H*clientRect.H)/2.0;
			foreach (MapIcon icon in _getIcons())
			{
				if (ShouldSkipIcon(icon))
					continue;

				float iZ = icon.Entity.GetComponent<Render>().Z;
				Vec2 point = minimapCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale, (int) ((iZ - pPosZ)/20));

				var texture = icon.MinimapIcon;
				int size = icon.Size;
				Rect rect = new Rect(point.X - size/2, point.Y - size/2, size, size);
				texture.DrawAt(rc, point, rect);
			}
		}

		private void RednerIconsOnLargeMap(RenderingContext rc)
		{
			var camera = model.Internal.Game.IngameState.Camera;
			var cw = camera.Width;
			var ch = camera.Height;
			BigMinimap mapWindow = model.Internal.Game.IngameState.IngameUi.Minimap;
			Rect rcMap = mapWindow.GetClientRect();

			Vec2 playerPos = model.Player.GetComponent<Positioned>().GridPos;
			float pPosZ = model.Player.GetComponent<Render>().Z;
			Vec2 screenCenter = new Vec2(rcMap.W / 2, rcMap.H / 2) + new Vec2(rcMap.X, rcMap.Y);
			float diag = (float)Math.Sqrt(cw * cw + ch * ch);

			// const float scale = 1280f;
			var k = cw < 1024 ? 1120 : 1024;
			float scale = (float)k / ch * cw * 3 / 4;

			foreach (MapIcon icon in _getIcons())
			{
				if (ShouldSkipIcon(icon))
					continue;

				float iZ = icon.Entity.GetComponent<Render>().Z;
				Vec2 point = screenCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale, (int)((iZ - pPosZ) / 10));

				var texture = icon.LargeMapIcon ?? icon.MinimapIcon;
				int size = icon.SizeOfLargeIcon.GetValueOrDefault(icon.Size * 2);
				Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				texture.DrawAt(rc, point, rect);
			}
		}

	}
}
