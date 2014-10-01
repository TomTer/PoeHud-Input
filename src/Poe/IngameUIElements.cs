using PoeHUD.ExileBot;
using PoeHUD.Poe.UI;

namespace PoeHUD.Poe
{
	public class IngameUIElements : RemoteMemoryObject
	{
		public BigMinimap Minimap
		{
			get
			{
				return base.ReadObject<BigMinimap>(this.address + 284);
			}
		}
	}
}
