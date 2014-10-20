using System;
using System.Drawing;
using System.Linq;
using PoeHUD.Framework;
using PoeHUD.Poe.UI;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.DebugView
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
			Element root = this.poe.Internal.IngameState.UIRoot;


			int yPos = 80;
			int x = 320;
			int[] path = new int[12];
			for (path[0] = 0x80; path[0] <= 0x210 ; path[0] += 4 ) {

				if (path[0] == 0x120 || path[0] == 0xd8 || path[0] == 0xa0 || path[0] == 0x154 || path[0] == 0x158 )
					continue;
				
				Element starting_it = this.poe.Internal.IngameState.IngameUi.ReadObjectAt<Element>(path[0]);
				var v2 = starting_it.GetParentPos();
				drawElt(rc, starting_it, new Vec2((int)(v2.X*.75), (int)(v2.Y*.75)), ref x, ref yPos, path, 1);
			}
		}

		private static void drawElt(RenderingContext rc, Element root, Vec2 parent, ref int x, ref int yPos, int[] path, int depth = 0)
		{
			if (!root.IsVisibleLocal || depth > 3)
			{
				return;
			}
			Rect rC = new Rect(parent.X + (int)(root.X * 0.75), parent.Y + (int)(root.Y * 0.75), (int)(root.Width * 0.75), (int)(root.Height * 0.75));

			if (rC.W < 20)
				return;
			string sPath = path[0].ToString("X3") + "-" + String.Join("-", path.Skip(1).Take(depth-1));
			int ix = depth > 0 ? path[depth - 1] : 0;
			var c = Color.FromArgb(255, 255 - 25 * (ix % 10), 255 - 25 * ((ix % 100) / 10), 255);

			string msg = string.Format("[{2}] {1:X8} : {0}", rC, root.address, sPath);

			var v = rc.AddTextWithHeight(new Vec2(x, yPos), msg, c, 9, DrawTextFormat.Left);
			rc.AddTextWithHeight(new Vec2(rC.X, rC.Y + depth * 10 - 10), sPath, c, 8, DrawTextFormat.Left);

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
				path[depth] = i;
				if (depth < 8)
					drawElt(rc, elt, pp, ref x, ref yPos, path, depth + 1);
			}
		}
	}
}