using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PoeHUD.Framework;
using PoeHUD.Settings;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.DebugView
{
	public class MainAddresses : HUDPluginBase
	{
		public SettingsForModule Settings = new SettingsForModule("MainAddresses");


		public override void OnEnable()
		{
			
		}

		public override void OnDisable()
		{
			
		}

		public override SettingsForModule SettingsNode
		{
			get { return Settings; }
		}

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			Vec2 pos = mountPoints[UiMountPoint.LeftOfMinimap];

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Player = {0:X8}\r\n", model.Player.Address);
			sb.AppendFormat("TheGame = {0:X8}\r\n", model.Internal.Address);
			sb.AppendFormat("IngameState = {0:X8}\r\n", model.Internal.IngameState.Address);
			sb.AppendFormat("IngameData = {0:X8}\r\n", model.Internal.IngameState.Data.Address);
			sb.AppendFormat("InventoryFrame = {0:X8}\r\n", model.Internal.IngameState.IngameUi.InventoryPanel.Address);

			var w1 = model.Internal.IngameState.IngameUi.InventoryPanel.MainWeaponSlot.GetItemAt();

			//var flasks = model.Internal.IngameState.IngameUi.InventoryPanel.FlasksFrame;
			//for (int i = 0; i < 5; i++)
			//{
			//	var f1 = flasks.GetItemAt(i);
			//	if (f1 != null)
			//		sb.AppendFormat("F{1} = {0}\r\n",String.Join("; ", f1.EnumComponents().Select(kv => String.Format("{0}: {1:X8}", kv.Key, kv.Value))), i + 1);
			//}

			var szText = rc.AddTextWithHeight(pos, sb.ToString(), Color.White, 11, DrawTextFormat.Right);
			Rect box = new Rect(pos.X - szText.X - 5, pos.Y - 5, szText.X + 10, szText.Y + 10);
			rc.AddBox(box, Color.FromArgb(160, 0, 0, 0));
			 
			mountPoints[UiMountPoint.LeftOfMinimap]  = new Vec2(pos.X, pos.Y + szText.Y + 10 + 5);

		}
	}
}
