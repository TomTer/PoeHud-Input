namespace PoeHUD.Poe.UI
{
	public class InventoryItemIcon : Element
	{
		public Tooltip Tooltip
		{
			get
			{
				return base.ReadObject<Tooltip>(this.address + 2796);
			}
		}
		public Entity Item
		{
			get
			{
				return base.ReadObject<Entity>(this.address + 2832);
			}
		}
	}
}
