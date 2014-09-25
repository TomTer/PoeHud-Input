namespace ExileHUD.EntityComponents
{
	public class Chest : Component
	{
		public bool IsOpened
		{
			get
			{
				return this.address != 0 && this.m.ReadByte(this.address + 36) == 1;
			}
		}
		public bool IsStrongbox
		{
			get
			{
				return this.address != 0 && this.m.ReadInt(this.address + 52) != 0;
			}
		}
	}
}
