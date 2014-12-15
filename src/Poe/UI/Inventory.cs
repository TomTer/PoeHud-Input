namespace PoeHUD.Poe.UI
{
	public class Inventory : Element
	{
		public Poe.Inventory InventoryModel
		{
			get
			{
				return base.ReadObject<Poe.Inventory>(this.Address + 0x17C + OffsetBuffers);
			}
		}
	}
}
