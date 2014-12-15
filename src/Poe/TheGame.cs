using PoeHUD.Framework;

namespace PoeHUD.Poe
{
	public class TheGame : RemoteMemoryObject
	{
		public IngameState IngameState
		{
			get
			{
				return base.ReadObject<IngameState>(this.Address + 0x9C);
			}
		}
		public int AreaChangeCount
		{
			get
			{
				return this.M.ReadInt(this.M.BaseAddress + Offsets.AreaChangeCount);
			}
		}
		public TheGame(Memory m)
		{
			this.M = m;
			this.Address = m.ReadInt(m.BaseAddress + Offsets.Base, new[]{ 4, 124 });
			this.Game = this;
		}
	}
}
