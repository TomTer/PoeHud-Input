namespace ExileHUD.ExileBot
{
	public class Poe_UI_Tooltip : Poe_UIElement
	{
		public Poe_UIElement Contents
		{
			get
			{
				return base.GetChildFromIndices(new int[]
				{
					0,
					1
				});
			}
		}
	}
}
