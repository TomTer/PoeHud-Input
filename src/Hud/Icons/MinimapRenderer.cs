using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;

namespace PoeHUD.Hud.Icons
{
	public class MinimapRenderer : HUDPluginBase
	{
		private readonly Func<IEnumerable<MapIcon>> getIcons;
		private Vec2 playerPos;

		public MinimapRenderer(Func<IEnumerable<MapIcon>> gatherMapIcons)
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

			playerPos = model.Player.GetComponent<Positioned>().GridPos;
			Element smallMinimap = model.Internal.IngameState.IngameUi.Minimap.SmallMinimap;
			const float scale = 240f;
			Rect clientRect = smallMinimap.GetClientRect();
			Vec2 minimapCenter = new Vec2(clientRect.X + clientRect.W / 2, clientRect.Y + clientRect.H / 2);
			double diag = Math.Sqrt(clientRect.W * clientRect.W + clientRect.H * clientRect.H) / 2.0;
			foreach(MapIcon icon in getIcons())
			{
				if (icon.ShouldSkip())
					continue;

				Vec2 point = WorldToMinimap(icon.WorldPosition, minimapCenter, diag, scale);

				var style = icon.MinimapStyle;
				int size = style.Size;
				Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				style.Texture.DrawAt(rc, point, rect);
			}
		}

		private Vec2 WorldToMinimap(Vec2 world, Vec2 minimapCenter, double diag, float scale)
		{
			// Values according to 40 degree rotation of cartesian coordiantes, still doesn't seem right but closer
			float cosX = (float)((world.X - playerPos.X) / scale * diag * Math.Cos((Math.PI / 180) * 40));
			float cosY = (float)((world.Y - playerPos.Y) / scale * diag * Math.Cos((Math.PI / 180) * 40));
			float sinX = (float)((world.X - playerPos.X) / scale * diag * Math.Sin((Math.PI / 180) * 40));
			float sinY = (float)((world.Y - playerPos.Y) / scale * diag * Math.Sin((Math.PI / 180) * 40));
			// 2D rotation formulas not correct, but it's what appears to work?
			int x = (int)(minimapCenter.X + cosX - cosY);
			int y = (int)(minimapCenter.Y - (sinX + sinY));
			return new Vec2(x, y);
		}
	}
}
