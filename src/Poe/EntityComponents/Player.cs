namespace PoeHUD.Poe.EntityComponents
{
	public class Player : Component
	{
		public string PlayerName
		{
			get
			{
				if (this.Address == 0)
				{
					return "";
				}
				int num = this.M.ReadInt(this.Address + 32);
				if (num > 512)
				{
					return "";
				}
				if (num < 8)
				{
					return this.M.ReadStringU(this.Address + 16, num * 2, true);
				}
				return this.M.ReadStringU(this.M.ReadInt(this.Address + 16), num * 2, true);
			}
		}
		public long XP
		{
			get
			{
				if (this.Address != 0)
				{
					return (long)((ulong)this.M.ReadUInt(this.Address + 52));
				}
				return 0L;
			}
		}
		public int Level
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 68);
				}
				return 1;
			}
		}
	}
}
