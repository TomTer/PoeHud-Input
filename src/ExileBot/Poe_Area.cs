namespace ExileHUD.ExileBot
{
	public class Poe_Area : RemoteMemoryObject
	{
		public string RawName
		{
			get
			{
				return this.m.ReadStringU(this.m.ReadInt(this.address), 256, true);
			}
		}
		public string Name
		{
			get
			{
				return this.m.ReadStringU(this.m.ReadInt(this.address + 4), 256, true);
			}
		}
		public int Act
		{
			get
			{
				return this.m.ReadInt(this.address + 8);
			}
		}
		public bool IsTown
		{
			get
			{
				return (this.m.ReadInt(this.address + 12) & 1) == 1;
			}
		}
		public bool HasWaypoint
		{
			get
			{
				return (this.m.ReadInt(this.address + 13) & 1) == 1;
			}
		}
	}
}
