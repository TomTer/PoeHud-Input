using System;
using System.Collections.Generic;
namespace ExileBot
{
	public class Life : Component
	{
		public int MaxHP
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 80);
				}
				return 1;
			}
		}
		public int CurHP
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 84);
				}
				return 1;
			}
		}
		public int ReservedHP
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 92);
				}
				return 0;
			}
		}
		public int MaxMana
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 116);
				}
				return 1;
			}
		}
		public int CurMana
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 120);
				}
				return 1;
			}
		}
		public int ReservedMana
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 128);
				}
				return 0;
			}
		}
		public int MaxES
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 152);
				}
				return 0;
			}
		}
		public int CurES
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 156);
				}
				return 0;
			}
		}
		public float HPPercentage
		{
			get
			{
				return (float)this.CurHP / (float)(this.MaxHP - this.ReservedHP);
			}
		}
		public float MPPercentage
		{
			get
			{
				return (float)this.CurMana / (float)(this.MaxMana - this.ReservedMana);
			}
		}
		public float ESPercentage
		{
			get
			{
				if (this.MaxES != 0)
				{
					return (float)this.CurES / (float)this.MaxES;
				}
				return 0f;
			}
		}
		public bool CorpseUsable
		{
			get
			{
				return this.m.ReadBytes(this.address + 212, 1)[0] == 1;
			}
		}
		public List<Poe_Buff> Buffs
		{
			get
			{
				List<Poe_Buff> list = new List<Poe_Buff>();
				int num = this.m.ReadInt(this.address + 184);
				int num2 = this.m.ReadInt(this.address + 188);
				int num3 = (num2 - num) / 4;
				if (num3 <= 0 || num3 > 32)
				{
					return list;
				}
				for (int i = 0; i < num3; i++)
				{
					list.Add(base.ReadObject<Poe_Buff>(this.m.ReadInt(num + i * 4) + 4));
				}
				return list;
			}
		}
		public bool HasBuff(string buff)
		{
			return this.Buffs.Exists((Poe_Buff x) => x.Name == buff);
		}
	}
}
