namespace PoeHUD.Poe.UI
{
	public class Label : Element
	{
		public string Text
		{
			get
			{
				int num = this.m.ReadInt(this.address + 2468);
				if (num <= 0 || num > 256)
				{
					return "";
				}
				if (num >= 8)
				{
					return this.m.ReadStringU(this.m.ReadInt(this.address + 2452), num * 2, true);
				}
				return this.m.ReadStringU(this.address + 2452, num * 2, true);
			}
		}
	}
}
