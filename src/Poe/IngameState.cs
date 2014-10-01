using PoeHUD.ExileBot;
using PoeHUD.Poe.UI;

namespace PoeHUD.Poe
{
	public class IngameState : RemoteMemoryObject
	{
		public Camera Camera
		{
			get
			{
				return base.GetObject<Camera>(this.address + 5556 + Offsets.IgsOffset - Offsets.IgsDelta);
			}
		}
		public float CurrentZoomLevel
		{
			get
			{
				return this.m.ReadFloat(this.address + 5784 + Offsets.IgsOffset - Offsets.IgsDelta);
			}
			set
			{
				this.m.WriteFloat(this.address + 5784 + Offsets.IgsOffset - Offsets.IgsDelta, value);
			}
		}
		public IngameData Data
		{
			get
			{
				return base.ReadObject<IngameData>(this.address + 316 + Offsets.IgsOffset);
			}
		}
		public bool InGame
		{
			get
			{
				return this.m.ReadInt(this.address + 316 + Offsets.IgsOffset) != 0 && this.ServerData.IsInGame;
			}
		}
		public ServerData ServerData
		{
			get
			{
				return base.ReadObject<ServerData>(this.address + 320 + Offsets.IgsOffset);
			}
		}
		public IngameUIElements IngameUi
		{
			get
			{
				return base.ReadObject<IngameUIElements>(this.address + 1516 + Offsets.IgsOffset);
			}
		}
		public Element UIRoot
		{
			get
			{
				return base.ReadObject<Element>(this.address + 3088 + Offsets.IgsOffset);
			}
		}
		public Element UIHover
		{
			get
			{
				return base.ReadObject<Element>(this.address + 3108 + Offsets.IgsOffset);
			}
		}
		public int EntityLabelMap
		{
			get
			{
				return this.m.ReadInt(this.address + 68, new int[]
				{
					2528
				});
			}
		}
	}
}
