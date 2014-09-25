namespace ExileHUD.EntityComponents
{
	public class Player : Component
	{
		public string PlayerName
		{
			get
			{
				if (this.address == 0)
				{
					return "";
				}
				int num = this.m.ReadInt(this.address + 32);
				if (num > 512)
				{
					return "";
				}
				if (num < 8)
				{
					return this.m.ReadStringU(this.address + 16, num * 2, true);
				}
				return this.m.ReadStringU(this.m.ReadInt(this.address + 16), num * 2, true);
			}
		}
		public long XP
		{
			get
			{
				if (this.address != 0)
				{
					return (long)((ulong)this.m.ReadUInt(this.address + 52));
				}
				return 0L;
			}
		}
		public int Level
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 68);
				}
				return 1;
			}
		}
	}
}
