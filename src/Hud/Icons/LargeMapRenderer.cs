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

				Vec2 point = WorldToMinimap(icon.WorldPosition, screenCenter, diag, scale);

				var style = icon.LargeMapStyle;
				int size = style.Size * 2;
				Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				style.Texture.DrawAt(rc, point, rect);
			}
		}

		private Vec2 WorldToMinimap(Vec2 world, Vec2 minimapCenter, double diag, float scale)
		{
			const float cameraAngle = 38;

			// Values according to 40 degree rotation of cartesian coordiantes, still doesn't seem right but closer
			float cosX = (float)((world.X - playerPos.X) / scale * diag * Math.Cos((Math.PI / 180) * cameraAngle));
			float cosY = (float)((world.Y - playerPos.Y) / scale * diag * Math.Cos((Math.PI / 180) * cameraAngle));
			float sinX = (float)((world.X - playerPos.X) / scale * diag * Math.Sin((Math.PI / 180) * cameraAngle));
			float sinY = (float)((world.Y - playerPos.Y) / scale * diag * Math.Sin((Math.PI / 180) * cameraAngle));
			// 2D rotation formulas not correct, but it's what appears to work?
			int x = (int)(minimapCenter.X + cosX - cosY);
			int y = (int)(minimapCenter.Y - (sinX + sinY));
			return new Vec2(x, y);
		}
	}
}
