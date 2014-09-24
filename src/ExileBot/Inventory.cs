using System;
using System.Collections.Generic;
namespace ExileBot
{
	public class Inventory
	{
		private Poe_Inventory InternalInventory;
		private PathOfExile Poe;
		public int Width
		{
			get
			{
				return this.InternalInventory.Width;
			}
		}
		public int Height
		{
			get
			{
				return this.InternalInventory.Height;
			}
		}
		public List<Entity> Items
		{
			get
			{
				List<Entity> list = new List<Entity>();
				foreach (Poe_Entity current in this.InternalInventory.Items)
				{
					list.Add(new Entity(this.Poe, current));
				}
				return list;
			}
		}
		public Inventory(PathOfExile Poe, Poe_Inventory InternalInventory)
		{
			this.Poe = Poe;
			this.InternalInventory = InternalInventory;
		}
		public Inventory(PathOfExile Poe, int address) : this(Poe, Poe.Internal.GetObject<Poe_Inventory>(address))
		{
		}
	}
}
