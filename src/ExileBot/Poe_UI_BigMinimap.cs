namespace ExileHUD.ExileBot
{
	public class Poe_UI_BigMinimap : Poe_UIElement
	{
		public Poe_UIElement SmallMinimap
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 2412);
			}
		}
	}
}
