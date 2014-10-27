using System;
using System.Collections.Generic;
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
			Element smallMinimap = model.Internal.IngameState.IngameUi.Minimap.SmallMinimap;
			if( !smallMinimap.IsVisible )
				return;


			playerPos = model.Player.GetComponent<Positioned>().GridPos;
			
			const float scale = 240f;
			Rect clientRect = smallMinimap.GetClientRect();
			Vec2 minimapCenter = new Vec2(clientRect.X + clientRect.W / 2, clientRect.Y + clientRect.H / 2);
			double diag = Math.Sqrt(clientRect.W * clientRect.W + clientRect.H * clientRect.H) / 2.0;
			foreach(MapIcon icon in getIcons())
			{
				if (icon.ShouldSkip())
					continue;

				Vec2 point = minimapCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale);

				var texture = icon.MinimapIcon;
				int size = icon.Size;
				Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				texture.DrawAt(rc, point, rect);
			}
		}
	}
}
