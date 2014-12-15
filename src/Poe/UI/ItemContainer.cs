
using System;

namespace PoeHUD.Poe.UI
{
	public class ItemContainer : Element
	{
		private readonly Lazy<int> slotWidth;
		private readonly Lazy<int> slotHeight;

		public ItemContainer()
		{
			slotWidth = new Lazy<int>(() => this.M.ReadInt(this.Address + 0xC));
			slotHeight = new Lazy<int>(() => this.M.ReadInt(this.Address + 0x10));
		}

		public int SlotWidth { get { return slotWidth.Value;  } }
		public int SlotHeight { get { return slotHeight.Value; } }


		public Entity GetItemAt(int x = 0, int y = 0)
		{
			var addrContainer = this.M.ReadInt(this.Address + OffsetBuffers + UiElementSize + 0x20);
			if (addrContainer == 0)
				return null;
			var addrList = M.ReadInt(addrContainer + 0x20);
			var addrListEnd = M.ReadInt(addrContainer + 0x24);
			int ea = addrList +  4 * ( x + y * SlotWidth );
			if (ea >= addrListEnd) 
				throw new ArgumentOutOfRangeException("x");

			var itemInstance = M.ReadInt(ea); // skinned item or something
			return this.ReadObject<Entity>(itemInstance);
		}
	}
}
