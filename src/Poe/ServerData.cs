namespace PoeHUD.Poe
{
	public class ServerData : RemoteMemoryObject
	{
		public bool IsInGame
		{
			get
			{
				return this.m.ReadInt(this.address + 10872) == 3;
			}
		}
		public InventorySet PlayerInventories
		{
			get
			{
				return base.GetObject<InventorySet>(this.address + 10496);
			}
		}
	}
}
