using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PoeHUD.Framework;
using PoeHUD.Poe.UI;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Debug
{
	class ShowUiHierarchy : HUDPlugin
	{
		public override void OnEnable()
		{
		}
		public override void OnDisable()
		{
		}
		public override void Render(RenderingContext rc)
		{
			var root = this.poe.Internal.IngameState.UIRoot;

			int yPos = 80;
			int x = 20;

			drawElt(rc, root, Vec2.Empty, ref x, ref yPos);
		}

		private static void drawElt(RenderingContext rc, Element root, Vec2 parent, ref int x, ref int yPos, int ix = 0, int depth = 0)
		{
			if (!root.IsVisibleLocal || depth > 3)
			{
				return;
			}
			var c = Color.FromArgb(255, 255 - 25 * (ix % 10), 255 - 25 * ((ix % 100) / 10), 255);
			Rect rC = new Rect(parent.X + (int)(root.X * 0.75), parent.Y + (int)(root.Y * 0.75), (int)(root.Width * 0.75), (int)(root.Height * 0.75));

			if (rC.W < 200)
				return;

			string msg = string.Format("{2}{1:X8} [{3}] {4:X8} : {0}", rC, root.address, new String('-', depth), ix, root.Id);

			var v = rc.AddTextWithHeight(new Vec2(x, yPos), msg, c, 9, DrawTextFormat.Left);
			rc.AddTextWithHeight(new Vec2(rC.X, rC.Y + depth * 10 - 10), ix.ToString(), c, 8, DrawTextFormat.Left);

			rc.AddFrame(rC, c);
			yPos += v.Y;

			if (yPos > 1100)
			{
				yPos = 80;
				x += 300;
			}
			var pp = new Vec2(rC.X, rC.Y);
			for (int i = 0; i < root.Children.Count; i++)
			{
				var elt = root.Children[i];
				if (depth < 4)
					drawElt(rc, elt, pp, ref x, ref yPos, i, depth + 1);
			}
		}
	}
}