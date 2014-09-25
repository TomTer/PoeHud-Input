namespace ExileHUD.ExileBot
{
	public class Poe_UI_Inventory : Poe_UIElement
	{
		public Poe_Inventory Inventory
		{
			get
			{
				return base.ReadObject<Poe_Inventory>(this.address + 2436);
			}
		}
	}
}
