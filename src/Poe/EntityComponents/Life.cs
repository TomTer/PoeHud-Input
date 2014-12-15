using System.Collections.Generic;

namespace PoeHUD.Poe.EntityComponents
{
	public class Life : Component
	{
		public int MaxHP
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 80);
				}
				return 1;
			}
		}
		public int CurHP
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 84);
				}
				return 1;
			}
		}
		public int ReservedHP
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 92);
				}
				return 0;
			}
		}
		public int MaxMana
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 116);
				}
				return 1;
			}
		}
		public int CurMana
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 120);
				}
				return 1;
			}
		}
		public int ReservedMana
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 128);
				}
				return 0;
			}
		}
		public int MaxES
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 152);
				}
				return 0;
			}
		}
		public int CurES
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 156);
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
				return this.M.ReadBytes(this.Address + 212, 1)[0] == 1;
			}
		}
		public List<Buff> Buffs
		{
			get
			{
				List<Buff> list = new List<Buff>();
				int num = this.M.ReadInt(this.Address + 184);
				int num2 = this.M.ReadInt(this.Address + 188);
				int num3 = (num2 - num) / 4;
				if (num3 <= 0 || num3 > 32)
				{
					return list;
				}
				for (int i = 0; i < num3; i++)
				{
					list.Add(base.ReadObject<Buff>(this.M.ReadInt(num + i * 4) + 4));
				}
				return list;
			}
		}
		public bool HasBuff(string buff)
		{
			return this.Buffs.Exists((Buff x) => x.Name == buff);
		}
	}
}
