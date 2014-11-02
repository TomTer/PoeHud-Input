using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Icons;

namespace PoeHUD.Hud
{
	public interface HUDPlugin
	{
		void Init(GameController poe);
		void OnEnable();
		void OnDisable();

		void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints);
	}

	public interface HUDPluginWithMapIcons : HUDPlugin
	{
		IEnumerable<MapIcon> GetIcons();
	}

	public abstract class HUDPluginBase : HUDPlugin
	{
		protected GameController model;
		public void Init(GameController poe)
		{
			this.model = poe;
			this.OnEnable();
		}
		public abstract void OnEnable();
		public abstract void OnDisable();

		public abstract void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints);

		// could not fing a better place yet
		protected static RectUV GetDirectionsUv(double phi, double distance)
		{
			phi += Math.PI * 0.25; // fix roration due to projection
			if (phi > 2 * Math.PI)
				phi -= 2 * Math.PI;
			float xSprite = (float)Math.Round(phi / Math.PI * 4);
			if (xSprite >= 8) xSprite = 0;
			float ySprite = distance > 60 ? distance > 120 ? 2 : 1 : 0;
			var rectUV = new RectUV(xSprite / 8, ySprite / 3, (xSprite + 1) / 8, (ySprite + 1) / 3);
			return rectUV;
		}
	}
}
