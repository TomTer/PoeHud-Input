using System;
namespace ExileBot
{
	public class Poe_ServerData : RemoteMemoryObject
	{
		public bool IsInGame
		{
			get
			{
				return this.m.ReadInt(this.address + 10872) == 3;
			}
		}
		public Poe_InventorySet PlayerInventories
		{
			get
			{
				return base.GetObject<Poe_InventorySet>(this.address + 10496);
			}
		}
	}
}
