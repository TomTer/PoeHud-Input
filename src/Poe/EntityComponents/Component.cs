namespace PoeHUD.Poe.EntityComponents
{
	public abstract class Component : RemoteMemoryObject
	{
		public Entity Owner
		{
			get
			{
				return base.ReadObject<Entity>(this.address + 4);
			}
		}
	}
}
