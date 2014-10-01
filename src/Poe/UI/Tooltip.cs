namespace PoeHUD.Poe.UI
{
	public class Tooltip : Element
	{
		public Element Contents
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
