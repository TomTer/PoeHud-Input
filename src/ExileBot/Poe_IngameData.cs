namespace ExileHUD.ExileBot
{
	public class Poe_IngameData : RemoteMemoryObject
	{
		public Poe_Area CurrentArea
		{
			get
			{
				return base.ReadObject<Poe_Area>(this.address + 8);
			}
		}

		public int CurrentAreaLevel
		{
			get
			{
				return this.m.ReadInt(this.address + 12);
			}
		}
		public int CurrentAreaHash
		{
			get
			{
				return this.m.ReadInt(this.address + 16);
			}
		}

		public Poe_Entity LocalPlayer
		{
			get
			{
				return base.ReadObject<Poe_Entity>(this.address + 1440);
			}
		}
		public Poe_EntityList EntityList
		{
			get
			{
				return base.GetObject<Poe_EntityList>(this.address + 1472);
			}
		}
	}
}
