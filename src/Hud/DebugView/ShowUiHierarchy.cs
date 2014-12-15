using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Framework;
using PoeHUD.Poe.UI;
using PoeHUD.Settings;
using SlimDX.Direct3D9;
using PoeHUD.Poe;

namespace PoeHUD.Hud.DebugView
{
	class ShowUiHierarchy : HUDPluginBase
	{


		public override void OnEnable()
		{
		}
		public override void OnDisable()
		{
		}

		public SettingsForModule Settings = new SettingsForModule("UiHierarchy");

		public override SettingsForModule SettingsNode
		{
			get { return Settings; }
		}

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			Element root = this.model.Internal.IngameState.UIRoot;


			int yPos = 80;
			int x = 320;
			int[] path = new int[12];

			//var tt = this.model.Internal.IngameState.IngameUi.;
			//for (path[0] = 0x0; path[0] <= 0x0; path[0] += 4)
			//{

			//	var starting_it = tt;
			//	//	.ReadObjectAt<Element>(path[0]);
			//	var v2 = starting_it.GetParentPos();
			//	drawElt(rc, starting_it, new Vec2((int)(v2.X * .75), (int)(v2.Y * .75)), ref x, ref yPos, path, 1);
			//}

			//var start0 = this.model.Internal.IngameState.AddrAsObject<Element>(0x6AEE0FC0);
			//drawElt(rc, start0, start0.GetParentPos() * 0.75f, ref x, ref yPos, path, 1);

			//foreach (int addr in GetUiElementsByOffset(0, 12, 16))
			//{
			//	path[0] = addr;
			var starting = this.model.Internal.IngameState.ElementUnderCursor;
			if (starting != null)
			{
				drawElt(rc, starting, starting.GetParentPos() * starting.Scale, ref x, ref yPos, path, 1);
				//starting = starting.HintToChatLink;
				//drawElt(rc, starting, starting.GetParentPos()*0.75f, ref x, ref yPos, path, 1);
			}
			// }

		//foreach (int addr in GetUiElementsByOffset(0x120, 50))
			//{
			//	path[0] = addr;
			//	Element starting_it = this.model.Internal.IngameState.IngameUi.ReadObjectAt<Element>(path[0]);

			//	drawElt(rc, starting_it, starting_it.GetParentPos() * .75f, ref x, ref yPos, path, 1);
			//}
		}

		private IEnumerable<int> GetUiElementsByOffset(int offset, int cnt, int step = 4)
		{
			for (int i = 0; i < cnt; i++)
			{
				yield return offset + i * step;
			}
		}

		private const int MAX_DEPTH = 3;
		private const int MAX_CHILDREN = 8000;


		private static void drawElt(RenderingContext rc, Element root, Vec2 parent, ref int x, ref int yPos, int[] path, int depth = 0)
		{
			if ( /* !root.IsVisibleLocal || */ depth > MAX_DEPTH)
			{
				return;
			}
			var scale = root.Scale;
			Vec2f position = new Vec2f(parent.X + root.X * scale, parent.Y + root.Y * scale);
			Vec2 size = new Vec2((int)(root.Width * scale), (int)(root.Height * scale));

//			if (rC.W < 20) return;

			string sPath = path[0].ToString("X3") + "-" + String.Join("-", path.Skip(1).Take(depth-1));
			int ix = depth > 0 ? path[depth - 1] : 0;
			var c = Color.FromArgb(255, 255 - 25 * (ix % 10), 255 - 25 * ((ix % 100) / 10), 255);

			string msg = string.Format("[{2}] {1:X8} : {0} {3}", position, root.Address, sPath, size);

			var v = rc.AddTextWithHeight(new Vec2(x, yPos), msg, c, 9, DrawTextFormat.Left);
			rc.AddTextWithHeight(new Vec2f(position.X, position.Y + depth * 10 - 10), sPath, c, 8, DrawTextFormat.Left);
			// rc.AddTextWithHeightAndOutline(new Vec2(rC.X, rC.Y + depth * 10 - 10), sPath, c, Color.Black, 8, DrawTextFormat.Left);

			rc.AddFrame(new Rect(position, size), c);
			yPos += v.Y;

			if (yPos > 1100)
			{
				yPos = 80;
				x += 300;
			}
			position.Y += root.ScrollY * root.Scale;

			List<Element> children = root.Children;
			for (int i = 0; i < children.Count && i < MAX_CHILDREN; i++)
			{
				var elt = children[i];
				path[depth] = i;
				drawElt(rc, elt, position, ref x, ref yPos, path, depth + 1);
			}
		}
	}
}