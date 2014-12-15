using System;
using PoeHUD.Poe.UI;

namespace PoeHUD.Poe
{
	public class IngameState : RemoteMemoryObject
	{

		public const int IngameStateSize = 0x15C;

		public Camera Camera
		{
			get
			{
				return base.GetObject<Camera>(this.Address + 0x15B4 + Offsets.IgsOffset - Offsets.IgsDelta);
			}
		}
		public float CurrentZoomLevel
		{
			get
			{
				return this.M.ReadFloat(this.Address + 0x1694 + Offsets.IgsOffset - Offsets.IgsDelta);
			}
			set
			{
				this.M.WriteFloat(this.Address + 0x1694 + Offsets.IgsOffset - Offsets.IgsDelta, value);
			}
		}
		public IngameData Data
		{
			get
			{
				return base.ReadObject<IngameData>(this.Address + 0x138 + Offsets.IgsOffset);
			}
		}
		public bool InGame
		{
			get
			{
				return this.M.ReadInt(this.Address + 0x138 + Offsets.IgsOffset) != 0 && this.ServerData.IsInGame;
			}
		}
		public ServerData ServerData
		{
			get
			{
				return base.ReadObjectAt<ServerData>(0x13C + Offsets.IgsOffset);
			}
		}



		public IngameUIElements IngameUi
		{
			get
			{
				return base.ReadObjectAt<IngameUIElements>(0x5E8 + Offsets.IgsOffset);
			}
		}
		public Element UIRoot
		{
			get
			{
				return base.ReadObjectAt<Element>(0xC0C + Offsets.IgsOffset);
			}
		}
		public Element ElementUnderCursor
		{
			get
			{
				return base.ReadObjectAt<Element>(0xC20 + Offsets.IgsOffset);
			}
		}

		public float ParentEltXPos{ get { return this.M.ReadFloat(Address + 0xc90); } }
		public float ParentEltYPos { get { return this.M.ReadFloat(Address + 0xc94); } }

		public float[] LatencyHistory { get { return M.ReadFloatArray(Address + Offsets.IgsOffset + 0xC8C, 80); } }

		public int EntityLabelMap { get { return this.M.ReadInt(this.Address + 68, 2528); } }
		public float CurrentLatency { get { return this.M.ReadFloat(Address + 0xc8c); } }
		public float CurrentFrameTime { get { return M.ReadFloat(Address + 0x10f4); } }
		public float CurFps { get { return M.ReadFloat(Address + 0x1370); } }
		public TimeSpan TimeInGame { get { return TimeSpan.FromMilliseconds(M.ReadInt(Address + 0xc7c)); } }
	}
}
