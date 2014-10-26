using System.Collections.Generic;

namespace PoeHUD.Controllers
{
	public class Inventory
	{
		private Poe.Inventory InternalInventory;
		private GameController Poe;
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
		public List<EntityWrapper> Items
		{
			get
			{
				List<EntityWrapper> list = new List<EntityWrapper>();
				foreach (Poe.Entity current in this.InternalInventory.Items)
				{
					list.Add(new EntityWrapper(this.Poe, current));
				}
				return list;
			}
		}
		public Inventory(GameController Poe, Poe.Inventory InternalInventory)
		{
			this.Poe = Poe;
			this.InternalInventory = InternalInventory;
		}
		public Inventory(GameController Poe, int address) : this(Poe, Poe.Internal.GetObject<Poe.Inventory>(address))
		{
		}
	}
}
