namespace PoeHUD.Poe.EntityComponents
{
	public class WorldItem : Component
	{
		public Entity ItemEntity
		{
			get
			{
				if (this.Address != 0)
				{
					return base.ReadObject<Entity>(this.Address + 20);
				}
				return base.GetObject<Entity>(0);
			}
		}
	}
}
