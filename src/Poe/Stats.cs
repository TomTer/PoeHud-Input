namespace PoeHUD.Poe
{
	public class Stats : RemoteMemoryObject
	{
		public int this[int stat]
		{
			get
			{
				if (this.address == 0)
				{
					return -1;
				}
				int result = 0;
				if (this.GetStat(stat, out result))
				{
					return result;
				}
				return -1;
			}
		}
		private bool GetStat(int stat, out int result)
		{
			int num = this.m.ReadInt(this.address + 16);
			int num2 = this.m.ReadInt(num + 16);
			int i = this.m.ReadInt(num + 20) - num2 >> 3;
			while (i > 0)
			{
				int num3 = i / 2;
				if (this.m.ReadInt(num2 + 8 * num3) >= stat)
				{
					i /= 2;
				}
				else
				{
					num2 += 8 * num3 + 8;
					i += -1 - num3;
				}
			}
			if (this.m.ReadInt(num + 20) != num2 && this.m.ReadInt(num2) == stat)
			{
				result = this.m.ReadInt(num2 + 4);
				return true;
			}
			if (this.m.ReadInt(num + 8) != 0 && this.GetStat2(stat, out result))
			{
				return true;
			}
			result = 0;
			return false;
		}
		private bool GetStat2(int stat, out int res)
		{
			int num = this.m.ReadInt(this.address + 16, new int[]
			{
				8
			});
			int num2;
			while (true)
			{
				num2 = this.m.ReadInt(num + 36);
				int i = (this.m.ReadInt(num + 40) - num2) / 28;
				while (i > 0)
				{
					int num3 = i / 2;
					if (this.m.ReadInt(num2 + 28 * num3) >= stat)
					{
						i /= 2;
					}
					else
					{
						num2 += 28 * num3 + 28;
						i += -1 - num3;
					}
				}
				if (this.m.ReadInt(num2) == stat)
				{
					break;
				}
				num = this.m.ReadInt(num + 12);
				if (num == 0)
				{
					goto Block_4;
				}
			}
			res = this.m.ReadInt(num2 + 4);
			return true;
			Block_4:
			res = 0;
			return false;
		}
	}
}
