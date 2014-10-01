namespace PoeHUD.Poe.UI
{
	public class EntityLabel : Element
	{
		public string Text
		{
			get
			{
				return base.AsObject<Label>().Text;
			}
		}
	}
}
