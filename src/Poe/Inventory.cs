using System.Collections.Generic;

namespace PoeHUD.Poe
{
	public class Inventory : RemoteMemoryObject
	{
		public int Width
		{
			get
			{
				return this.M.ReadInt(this.Address + 28);
			}
		}
		public int Height
		{
			get
			{
				return this.M.ReadInt(this.Address + 32);
			}
		}
		private int ListStart
		{
			get
			{
				return this.M.ReadInt(this.Address + 48);
			}
		}
		private int ListEnd
		{
			get
			{
				return this.M.ReadInt(this.Address + 52);
			}
		}
		public List<Entity> Items
		{
			get
			{
				List<Entity> list = new List<Entity>();
				int num = (this.ListEnd - this.ListStart) / 4;
				if (num > 1000 || num <= 0)
				{
					return list;
				}
				HashSet<int> hashSet = new HashSet<int>();
				for (int i = 0; i < num; i++)
				{
					int num2 = this.M.ReadInt(this.ListStart + i * 4);
					if (num2 != 0 && !hashSet.Contains(num2))
					{
						list.Add(base.ReadObject<Entity>(num2));
						hashSet.Add(num2);
					}
				}
				return list;
			}
		}
	}
}
