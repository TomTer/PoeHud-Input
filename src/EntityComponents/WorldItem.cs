using ExileHUD.ExileBot;

namespace ExileHUD.EntityComponents
{
	public class WorldItem : Component
	{
		public Poe_Entity ItemEntity
		{
			get
			{
				if (this.address != 0)
				{
					return base.ReadObject<Poe_Entity>(this.address + 20);
				}
				return base.GetObject<Poe_Entity>(0);
			}
		}
	}
}
