namespace ExileHUD.ExileBot
{
	public class Poe_UI_EntityLabel : Poe_UIElement
	{
		public string Text
		{
			get
			{
				return base.AsObject<Poe_UI_Label>().Text;
			}
		}
	}
}
