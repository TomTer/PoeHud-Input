namespace PoeHUD.Poe
{
	public class IngameData : RemoteMemoryObject
	{
		public AreaTemplate CurrentArea
		{
			get
			{
				return base.ReadObject<AreaTemplate>(this.address + 8);
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

		public Entity LocalPlayer
		{
			get
			{
				return base.ReadObject<Entity>(this.address + 1440);
			}
		}
		public EntityList EntityList
		{
			get
			{
				return base.GetObject<EntityList>(this.address + 1472);
			}
		}
	}
}
