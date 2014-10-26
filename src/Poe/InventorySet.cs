using System.Collections.Generic;
using PoeHUD.Controllers;

namespace PoeHUD.Poe
{
	public class InventorySet : RemoteMemoryObject
	{
		private int ListStart
		{
			get
			{
				return this.m.ReadInt(this.address + 112);
			}
		}
		private int ListEnd
		{
			get
			{
				return this.m.ReadInt(this.address + 116);
			}
		}
		public int InventoryCount
		{
			get
			{
				return (this.ListEnd - this.ListStart) / 16;
			}
		}
		public List<Inventory> Inventories
		{
			get
			{
				List<Inventory> list = new List<Inventory>();
				int inventoryCount = this.InventoryCount;
				if (inventoryCount > 50 || inventoryCount <= 0)
				{
					return list;
				}
				for (int i = 0; i < inventoryCount; i++)
				{
					list.Add(base.ReadObject<Inventory>(this.ListStart + 8 + i * 16));
				}
				return list;
			}
		}
        public Inventory this[InventoryIndex inv]
        {
            get
            {
                int num = (int)inv;
                return base.ReadObject<Inventory>((this.ListStart + 8) + (num * 0x10));
            }
        }
	}
}
