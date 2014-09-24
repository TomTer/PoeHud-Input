using BotFramework;
using ExileBot;
using SlimDX.Direct3D9;
using System;
using System.Drawing;
namespace ExileHUD
{
	public class ItemLevelRenderer : HUDPlugin
	{
		public override void OnEnable()
		{
		}
		public override void OnDisable()
		{
		}
		public override void Render(RenderingContext rc)
		{
			if (!Settings.GetBool("Tooltip") || !Settings.GetBool("Tooltip.ShowItemLevel"))
			{
				return;
			}
			Poe_UIElement uIHover = this.poe.Internal.IngameState.UIHover;
			Poe_Entity item = uIHover.AsObject<Poe_UI_InventoryItemIcon>().Item;
			if (item.address != 0 && item.IsValid)
			{
				Poe_UI_Tooltip tooltip = uIHover.AsObject<Poe_UI_InventoryItemIcon>().Tooltip;
				if (tooltip == null)
				{
					return;
				}
				Poe_UIElement childAtIndex = tooltip.GetChildAtIndex(0);
				if (childAtIndex == null)
				{
					return;
				}
				Poe_UIElement childAtIndex2 = childAtIndex.GetChildAtIndex(1);
				if (childAtIndex2 == null)
				{
					return;
				}
				Rect clientRect = childAtIndex2.GetClientRect();
				rc.AddTextWithHeight(new Vec2(clientRect.X + 2, clientRect.Y + 2), item.GetComponent<Mods>().ItemLevel.ToString(), Color.White, 16, DrawTextFormat.Left);
			}
		}
	}
}
