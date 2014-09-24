using System;
using System.Collections.Generic;
namespace ExileBot
{
	public class Poe_InventorySet : RemoteMemoryObject
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
		public List<Poe_Inventory> Inventories
		{
			get
			{
				List<Poe_Inventory> list = new List<Poe_Inventory>();
				int inventoryCount = this.InventoryCount;
				if (inventoryCount > 50 || inventoryCount <= 0)
				{
					return list;
				}
				for (int i = 0; i < inventoryCount; i++)
				{
					list.Add(base.ReadObject<Poe_Inventory>(this.ListStart + 8 + i * 16));
				}
				return list;
			}
		}
        public Poe_Inventory this[InventoryIndex inv]
        {
            get
            {
                int num = (int)inv;
                return base.ReadObject<Poe_Inventory>((this.ListStart + 8) + (num * 0x10));
            }
        }
	}
}
