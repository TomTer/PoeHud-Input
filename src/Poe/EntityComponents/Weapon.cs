namespace PoeHUD.Poe.EntityComponents
{
	public class Weapon : Component
	{
		public int DamageMin
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 16, new int[]
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
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 16, new int[]
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
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 16, new int[]
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
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 16, new int[]
					{
						16
					});
				}
				return 0;
			}
		}
	}
}
