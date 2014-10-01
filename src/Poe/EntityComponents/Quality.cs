namespace PoeHUD.Poe.EntityComponents
{
	public class Quality : Component
	{
		public int ItemQuality
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 12);
				}
				return 0;
			}
		}
	}
}
