namespace PoeHUD.Poe.UI
{
	public class Inventory : Element
	{
		public Poe.Inventory InventoryModel
		{
			get
			{
				return base.ReadObject<Poe.Inventory>(this.address + 2436);
			}
		}
	}
}
