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

		public Element ShopButton { get { return ReadObjectAt<Element>(4+0x7C); } }

		public Element HideoutEditButton { get { return ReadObjectAt<Element>(0x84); } }

		public Element HideoutStashButton { get { return ReadObjectAt<Element>(0x88); } }

		public Element Mouseposition { get { return ReadObjectAt<Element>(0xA0); } }

		public Element ActionButtons { get { return ReadObjectAt<Element>(0xA4); } }

		public Element Chat { get { return ReadObjectAt<Element>(4+0xD8); } }

		public Element QuestTracker { get { return ReadObjectAt<Element>(4+0xE8); } }

		public Element MtxInventory { get { return ReadObjectAt<Element>(4+0xEC); } }

		public Element MtxShop { get { return ReadObjectAt<Element>(4+0xF0); } }

		public Element InventoryPanel { get { return ReadObjectAt<Element>(4+0xF4); } }

		public Element StashPanel { get { return ReadObjectAt<Element>(4+0xF8); } }

		public Element SocialPanel { get { return ReadObjectAt<Element>(4+0x104); } }

		public Element TreePanel { get { return ReadObjectAt<Element>(4+0x108); } }

		public Element CharacterPanel { get { return ReadObjectAt<Element>(4+0x10C); } }

		public Element OptionsPanel { get { return ReadObjectAt<Element>(4+0x110); } }

		public Element AchievementsPanel { get { return ReadObjectAt<Element>(4+0x114); } }

		public Element WorldPanel { get { return ReadObjectAt<Element>(4+0x118); } }

		public BigMinimap Minimap { get { return ReadObjectAt<BigMinimap>(4+0x11C); } }

		public Element ItemsOnGroundLabels { get { return ReadObjectAt<Element>(4+0x120); } }

		public Element MonsterHpLabels { get { return ReadObjectAt<Element>(4+0x124); } }

		public Element Buffs { get { return ReadObjectAt<Element>(4+0x130); } }
		public Element Buffs2 { get { return ReadObjectAt<Element>(4+0x18c); } }

		public Element OpenLeftPanel { get { return ReadObjectAt<Element>(4+0x154); } }
		public Element OpenRightPanel { get { return ReadObjectAt<Element>(4+0x158); } }

		public Element OpenNpcDialogPanel { get { return ReadObjectAt<Element>(4+0x160); } }

		public Element CreatureInfoPanel { get { return ReadObjectAt<Element>(4+0x184); } } // above, it shows name and hp

		public Element InstanceManagerPanel { get { return ReadObjectAt<Element>(4+0x198); } }
		public Element InstanceManagerPanel2 { get { return ReadObjectAt<Element>(4+0x19C); } }

		// dunno what it is
		public Element SwitchingZoneInfo { get { return ReadObjectAt<Element>(0x1C8); } }

		public Element GemLvlUpPanel { get { return ReadObjectAt<Element>(4+0x1F8); } }

		public Element ItemOnGroundTooltip { get { return ReadObjectAt<Element>(4+0x208); } }
	}
}
