namespace ExileHUD.ExileBot
{
	public class Poe_IngameState : RemoteMemoryObject
	{
		public Poe_Camera Camera
		{
			get
			{
				return base.GetObject<Poe_Camera>(this.address + 5556 + Offsets.IgsOffset - Offsets.IgsDelta);
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
		public Poe_IngameData Data
		{
			get
			{
				return base.ReadObject<Poe_IngameData>(this.address + 316 + Offsets.IgsOffset);
			}
		}
		public bool InGame
		{
			get
			{
				return this.m.ReadInt(this.address + 316 + Offsets.IgsOffset) != 0 && this.ServerData.IsInGame;
			}
		}
		public Poe_ServerData ServerData
		{
			get
			{
				return base.ReadObject<Poe_ServerData>(this.address + 320 + Offsets.IgsOffset);
			}
		}
		public Poe_IngameUIElements IngameUi
		{
			get
			{
				return base.ReadObject<Poe_IngameUIElements>(this.address + 1516 + Offsets.IgsOffset);
			}
		}
		public Poe_UIElement UIRoot
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 3088 + Offsets.IgsOffset);
			}
		}
		public Poe_UIElement UIHover
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 3108 + Offsets.IgsOffset);
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
