using System;
namespace ExileBot
{
	public class Poe_IngameState : RemoteMemoryObject
	{
		public Poe_Camera Camera
		{
			get
			{
				return base.GetObject<Poe_Camera>(this.address + 5556);
			}
		}
		public float CurrentZoomLevel
		{
			get
			{
				return this.m.ReadFloat(this.address + 5784);
			}
			set
			{
				this.m.WriteFloat(this.address + 5784, value);
			}
		}
		public Poe_IngameData Data
		{
			get
			{
				return base.ReadObject<Poe_IngameData>(this.address + 316);
			}
		}
		public bool InGame
		{
			get
			{
				return this.m.ReadInt(this.address + 316) != 0 && this.ServerData.IsInGame;
			}
		}
		public Poe_ServerData ServerData
		{
			get
			{
				return base.ReadObject<Poe_ServerData>(this.address + 320);
			}
		}
		public Poe_IngameUIElements IngameUi
		{
			get
			{
				return base.ReadObject<Poe_IngameUIElements>(this.address + 1516);
			}
		}
		public Poe_UIElement UIRoot
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 3088);
			}
		}
		public Poe_UIElement UIHover
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 3108);
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
