namespace ExileHUD.ExileBot
{
	public class Poe_IngameState : RemoteMemoryObject
	{
		public Poe_Camera Camera
		{
			get
			{
				return base.GetObject<Poe_Camera>(this.address + 5556 + Offsets.ISOffset - Offsets.ISDelta);
			}
		}
		public float CurrentZoomLevel
		{
			get
			{
				return this.m.ReadFloat(this.address + 5784 + Offsets.ISOffset - Offsets.ISDelta);
			}
			set
			{
				this.m.WriteFloat(this.address + 5784 + Offsets.ISOffset - Offsets.ISDelta, value);
			}
		}
		public Poe_IngameData Data
		{
			get
			{
				return base.ReadObject<Poe_IngameData>(this.address + 316 + Offsets.ISOffset);
			}
		}
		public bool InGame
		{
			get
			{
				return this.m.ReadInt(this.address + 316 + Offsets.ISOffset) != 0 && this.ServerData.IsInGame;
			}
		}
		public Poe_ServerData ServerData
		{
			get
			{
				return base.ReadObject<Poe_ServerData>(this.address + 320 + Offsets.ISOffset);
			}
		}
		public Poe_IngameUIElements IngameUi
		{
			get
			{
				return base.ReadObject<Poe_IngameUIElements>(this.address + 1516 + Offsets.ISOffset);
			}
		}
		public Poe_UIElement UIRoot
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 3088 + Offsets.ISOffset);
			}
		}
		public Poe_UIElement UIHover
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 3108 + Offsets.ISOffset);
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
