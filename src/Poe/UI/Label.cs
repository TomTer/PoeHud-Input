namespace PoeHUD.Poe.UI
{
	public class Label : Element
	{
		public string Text
		{
			get
			{
				int num = this.M.ReadInt(this.Address + 2468);
				if (num <= 0 || num > 256)
				{
					return "";
				}
				if (num >= 8)
				{
					return this.M.ReadStringU(this.M.ReadInt(this.Address + 2452), num * 2, true);
				}
				return this.M.ReadStringU(this.Address + 2452, num * 2, true);
			}
		}
	}
}
