using PoeHUD.Poe.UI;

namespace PoeHUD.Poe
{
	public class IngameUIElements : RemoteMemoryObject
	{

		public Element HpGlobe { get { return ReadObjectAt<Element>(0x40); } }

		public Element ManaGlobe { get { return ReadObjectAt<Element>(0x44); } }

		public Element Flasks { get { return ReadObjectAt<Element>(0x4C); } }

		public Element XpBar { get { return ReadObjectAt<Element>(0x50); } }

		public Element MenuButton { get { return ReadObjectAt<Element>(0x54); } }

		public Element ShopButton { get { return ReadObjectAt<Element>(0x7C); } }

		public Element ActionButtons { get { return ReadObjectAt<Element>(0xA0); } }

		public Element Chat { get { return ReadObjectAt<Element>(0xD8); } }

		public Element QuestTracker { get { return ReadObjectAt<Element>(0xE8); } }

		public Element MtxInventory { get { return ReadObjectAt<Element>(0xEC); } }

		public Element MtxShop { get { return ReadObjectAt<Element>(0xF0); } }

		public Element InventoryPanel { get { return ReadObjectAt<Element>(0xF4); } }

		public Element StashPanel { get { return ReadObjectAt<Element>(0xF8); } }

		public Element SocialPanel { get { return ReadObjectAt<Element>(0x104); } }

		public Element TreePanel { get { return ReadObjectAt<Element>(0x108); } }

		public Element CharacterPanel { get { return ReadObjectAt<Element>(0x10C); } }

		public Element OptionsPanel { get { return ReadObjectAt<Element>(0x110); } }

		public Element AchievementsPanel { get { return ReadObjectAt<Element>(0x114); } }

		public Element WorldPanel { get { return ReadObjectAt<Element>(0x118); } }

		public BigMinimap Minimap { get { return ReadObjectAt<BigMinimap>(0x11C); } }

		public Element OnGroundLabels { get { return ReadObjectAt<Element>(0x120); } }

		public Element Buffs { get { return ReadObjectAt<Element>(0x130); } }
		public Element Buffs2 { get { return ReadObjectAt<Element>(0x18c); } }

		public Element OpenLeftPanel { get { return ReadObjectAt<Element>(0x154); } }
		public Element OpenRightPanel { get { return ReadObjectAt<Element>(0x158); } }

		public Element OpenNpcDialogPanel { get { return ReadObjectAt<Element>(0x160); } }

		public Element InstanceManagerPanel { get { return ReadObjectAt<Element>(0x198); } }
		public Element InstanceManagerPanel2 { get { return ReadObjectAt<Element>(0x19C); } }

		public Element GemLvlUpPanel { get { return ReadObjectAt<Element>(0x1F8); } }

		public Element OnGroundTooltipPanel { get { return ReadObjectAt<Element>(0x208); } }
	}
}
