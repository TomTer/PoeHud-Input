using System;
namespace ExileBot
{
	public class Poe_UI_InventoryItemIcon : Poe_UIElement
	{
		public Poe_UI_Tooltip Tooltip
		{
			get
			{
				return base.ReadObject<Poe_UI_Tooltip>(this.address + 2796);
			}
		}
		public Poe_Entity Item
		{
			get
			{
				return base.ReadObject<Poe_Entity>(this.address + 2832);
			}
		}
	}
}
