using PoeHUD.ExileBot;
using PoeHUD.Framework;

namespace PoeHUD.Poe
{
	public class TheGame : RemoteMemoryObject
	{
		public IngameState IngameState
		{
			get
			{
				return base.ReadObject<IngameState>(this.address + 156);
			}
		}
		public int AreaChangeCount
		{
			get
			{
				return this.m.ReadInt(this.m.BaseAddress + Offsets.AreaChangeCount);
			}
		}
		public TheGame(Memory m)
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
