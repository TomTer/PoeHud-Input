using System;
using System.Collections.Generic;
namespace ExileBot
{
	public class Actor : Component
	{
		public List<int> Minions
		{
			get
			{
				List<int> list = new List<int>();
				if (this.address == 0)
				{
					return list;
				}
				int num = this.m.ReadInt(this.address + 656);
				int num2 = this.m.ReadInt(this.address + 660);
				for (int i = num; i < num2; i += 8)
				{
					int item = this.m.ReadInt(i);
					list.Add(item);
				}
				return list;
			}
		}
		public bool HasMinion(Poe_Entity entity)
		{
			if (this.address == 0)
			{
				return false;
			}
			int num = this.m.ReadInt(this.address + 656);
			int num2 = this.m.ReadInt(this.address + 660);
			for (int i = num; i < num2; i += 8)
			{
				int num3 = this.m.ReadInt(i);
				if (num3 == entity.ID)
				{
					return true;
				}
			}
			return false;
		}
	}
}
