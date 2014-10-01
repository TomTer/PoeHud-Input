namespace PoeHUD.Poe.EntityComponents
{
	public class Weapon : Component
	{
		public int DamageMin
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 16, new int[]
					{
						4
					});
				}
				return 0;
			}
		}
		public int DamageMax
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 16, new int[]
					{
						8
					});
				}
				return 0;
			}
		}
		public int AttackTime
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 16, new int[]
					{
						12
					});
				}
				return 1;
			}
		}
		public int CritChance
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 16, new int[]
					{
						16
					});
				}
				return 0;
			}
		}
	}
}
