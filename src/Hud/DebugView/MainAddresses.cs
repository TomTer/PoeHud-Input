using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PoeHUD.Framework;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.DebugView
{
	public class MainAddresses : HUDPluginBase
	{
		public override void OnEnable()
		{
			
		}

		public override void OnDisable()
		{
			
		}

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			Vec2 pos = mountPoints[UiMountPoint.LeftOfMinimap];

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Player = {0:X8}\r\n", model.Player.Address);
			sb.AppendFormat("TheGame = {0:X8}\r\n", model.Internal.address);
			sb.AppendFormat("IngameState = {0:X8}\r\n", model.Internal.IngameState.address);
			sb.AppendFormat("IngameData = {0:X8}\r\n", model.Internal.IngameState.Data.address);
			sb.AppendFormat("InventoryFrame = {0:X8}\r\n", model.Internal.IngameState.IngameUi.InventoryPanel.address);

			sb.AppendFormat("Chest = {0}\r\n", model.Internal.IngameState.IngameUi.InventoryPanel.ChestSlot.GetItem().Path );

			var szText = rc.AddTextWithHeight(pos, sb.ToString(), Color.White, 11, DrawTextFormat.Right);
			Rect box = new Rect(pos.X - szText.X - 5, pos.Y - 5, szText.X + 10, szText.Y + 10);
			rc.AddBox(box, Color.FromArgb(160, 0, 0, 0));
			 
			mountPoints[UiMountPoint.LeftOfMinimap]  = new Vec2(pos.X, pos.Y + szText.Y + 10 + 5);

		}
	}
}
