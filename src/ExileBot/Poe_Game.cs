using ExileHUD.Framework;

namespace ExileHUD.ExileBot
{
	public class Poe_Game : RemoteMemoryObject
	{
		public Poe_IngameState IngameState
		{
			get
			{
				return base.ReadObject<Poe_IngameState>(this.address + 156);
			}
		}
		public int AreaChangeCount
		{
			get
			{
				return this.m.ReadInt(this.m.BaseAddress + Offsets.AreaChangeCount);
			}
		}
		public Poe_Game(Memory m)
		{
			this.m = m;
			this.address = m.ReadInt(m.BaseAddress + Offsets.Base, new int[]
			{
				4,
				124
			});
			this.game = this;
		}
	}
}
