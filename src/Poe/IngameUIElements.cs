using PoeHUD.ExileBot;
using PoeHUD.Poe.UI;

namespace PoeHUD.Poe
{
	public class IngameUIElements : RemoteMemoryObject
	{
		public Element Chat
		{
			get
			{
				return base.ReadObject<BigMinimap>(this.address + 0xD8);
			}
		}

		public Element QuestTracker
		{
			get
			{
				return base.ReadObject<BigMinimap>(this.address + 0xE8);
			}
		}

		public BigMinimap Minimap
		{
			get
			{
				return base.ReadObject<BigMinimap>(this.address + 284);
			}
		}
	}
}
