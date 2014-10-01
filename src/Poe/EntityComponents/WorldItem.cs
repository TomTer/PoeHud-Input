namespace PoeHUD.Poe.EntityComponents
{
	public class WorldItem : Component
	{
		public Entity ItemEntity
		{
			get
			{
				if (this.address != 0)
				{
					return base.ReadObject<Entity>(this.address + 20);
				}
				return base.GetObject<Entity>(0);
			}
		}
	}
}
