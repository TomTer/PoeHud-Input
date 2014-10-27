using System;
using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;

namespace PoeHUD.Hud.Icons
{
	public class LargeMapRenderer : HUDPluginBase
	{
		private readonly Func<IEnumerable<MapIcon>> getIcons;
		private Vec2 playerPos;

		public LargeMapRenderer(Func<IEnumerable<MapIcon>> gatherMapIcons)
		{
			getIcons = gatherMapIcons;
		}

		public override void OnEnable()
		{
		}
		public override void OnDisable()
		{
		}

		public override void Render(RenderingContext rc)
		{
			if (!model.InGame || !Settings.GetBool("MinimapIcons"))
			{
				return;
			}
			bool largeMapVisible = model.Internal.IngameState.IngameUi.Minimap.OrangeWords.IsVisible;
			if (!largeMapVisible)
				return;

			var camera = model.Internal.game.IngameState.Camera;
			BigMinimap mapWindow = model.Internal.game.IngameState.IngameUi.Minimap;
			Rect rcMap = mapWindow.GetClientRect();

			playerPos = model.Player.GetComponent<Positioned>().GridPos;
			Vec2 screenCenter = new Vec2(rcMap.W / 2, rcMap.H / 2) + new Vec2(rcMap.X, rcMap.Y);
			float diag = (float)Math.Sqrt(camera.Width * camera.Width + camera.Height * camera.Height);

			const float scale = 1280f;

			foreach(MapIcon icon in getIcons())
			{
				if (icon.ShouldSkip())
					continue;

				Vec2 point = screenCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale);

				var texture = icon.LargeMapIcon ?? icon.MinimapIcon;
				int size = icon.SizeOfLargeIcon.GetValueOrDefault(icon.Size * 2);
				Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				texture.DrawAt(rc, point, rect);
			}
		}
	}
}
