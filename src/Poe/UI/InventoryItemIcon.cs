namespace PoeHUD.Poe.UI
{
	public class InventoryItemIcon : Element
	{
		public Tooltip Tooltip
		{
			get
			{
				return base.ReadObjectAt<Tooltip>(UiElementSize + OffsetBuffers + 0x188);
				// return base.ReadObject<Tooltip>(this.address + 2796);
			}
		}
		public Entity Item
		{
			get
			{
				return base.ReadObjectAt<Entity>(UiElementSize + OffsetBuffers + 0x1ac);
				//return base.ReadObject<Entity>(this.address + 2832);
			}
		}
	}
}
