using System;
using PoeHUD.ExileBot;
using PoeHUD.Framework;

namespace PoeHUD.Hud
{
	public abstract class HUDPlugin
	{
		protected PathOfExile poe;
		protected OverlayRenderer overlay;
		public void Init(PathOfExile poe, OverlayRenderer overlay)
		{
			this.poe = poe;
			this.overlay = overlay;
			this.OnEnable();
		}
		public abstract void OnEnable();
		public abstract void Render(RenderingContext rc);
		public abstract void OnDisable();


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
