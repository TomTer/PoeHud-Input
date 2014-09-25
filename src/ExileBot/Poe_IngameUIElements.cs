namespace ExileHUD.ExileBot
{
	public class Poe_IngameUIElements : RemoteMemoryObject
	{
		public Poe_UI_BigMinimap Minimap
		{
			get
			{
				return base.ReadObject<Poe_UI_BigMinimap>(this.address + 284);
			}
		}
	}
}
