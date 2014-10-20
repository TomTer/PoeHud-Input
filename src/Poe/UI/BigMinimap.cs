namespace PoeHUD.Poe.UI
{
	public class BigMinimap : Element
	{
		public Element SmallMinimap
		{
			get
			{
				return base.ReadObject<Element>(this.address + 0x164 + OffsetBuffers);
			}
		}
	}
}
