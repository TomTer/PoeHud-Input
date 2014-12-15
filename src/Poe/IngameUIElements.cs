using PoeHUD.Framework;
using PoeHUD.Poe.UI;
using System.ComponentModel.DataAnnotations;

namespace PoeHUD.Poe
{
	public class IngameUIElements : RemoteMemoryObject
	{

		public Element HudRoot { get { return ReadObjectAt<Element>(0x3C); } }

		public Element HpGlobe { get { return ReadObjectAt<Element>(0x40); } }

		public Element ManaGlobe { get { return ReadObjectAt<Element>(0x44); } }

		// x48 is a dot next to chat... what could it be?

		public Element Flasks { get { return ReadObjectAt<Element>(0x4C); } }

		public Element XpBar { get { return ReadObjectAt<Element>(0x50); } }

		public Element MenuButton { get { return ReadObjectAt<Element>(0x54); } }

		public Element MenuPanel { get { return ReadObjectAt<Element>(0x58); } }

		public Element OptionsMenuItem { get { return ReadObjectAt<Element>(0x5C); } }

		public Element SocialMenuItem { get { return ReadObjectAt<Element>(0x60); } }

		public Element CharacterMenuItem { get { return ReadObjectAt<Element>(0x64); } }
		public Element SkillTreeMenuItem { get { return ReadObjectAt<Element>(0x68); } }
		public Element InventoryMenuItem { get { return ReadObjectAt<Element>(0x6C); } }
		public Element AchievementsMenuItem { get { return ReadObjectAt<Element>(0x70); } }

		public Element OptionsIconMenuItem { get { return ReadObjectAt<Element>(0x74); } }
		public Element WorldMapMenuItem { get { return ReadObjectAt<Element>(0x78); } }
		public Element OverlayMapMenuItem { get { return ReadObjectAt<Element>(0x7C); } }

		public Element ShopButton { get { return ReadObjectAt<Element>(0x80); } }

		public Element HideoutEditButton { get { return ReadObjectAt<Element>(0x84); } }

		public Element HideoutStashButton { get { return ReadObjectAt<Element>(0x88); } }

		public Element LevelUpButton { get { return ReadObjectAt<Element>(0x8C); } }

		public Element QuestButton { get { return ReadObjectAt<Element>(0x90); } }

		// 94, 98 - uknown
		
		public Element OpenChatButton { get { return ReadObjectAt<Element>(0x9C); } }
		
		public Element Mouseposition { get { return ReadObjectAt<Element>(0xA0); } }

		public Element ActionButtons { get { return ReadObjectAt<Element>(0xA4); } }

		public Element SkillChoicePanel { get { return ReadObjectAt<Element>(0xA8); } }

		// 0XAC = 12 what could it be?

		public Element PartyPanel { get { return ReadObjectAt<Element>(0xB0); } }

		public Element TopScreenPanel1 { get { return ReadObjectAt<Element>(0xB4); } }
		public Element TopScreenPanel2 { get { return ReadObjectAt<Element>(0xB8); } }

		public Element TopScreenPanel3 { get { return ReadObjectAt<Element>(0xBC); } }

		// 0xC0 is some parent overlay
		public Element TimerTopPanel { get { return ReadObjectAt<Element>(0xC4); } }

		public Element TopScreenPanel4 { get { return ReadObjectAt<Element>(0xC8); } }

		public Element LeftBottomPanel1 { get { return ReadObjectAt<Element>(0xCC); } }
		public Element LeftBottomPanel2 { get { return ReadObjectAt<Element>(0xD0); } }

		public Element CenterBottomPanel1 { get { return ReadObjectAt<Element>(0xD4); } }
		public Element CenterBottomPanel2 { get { return ReadObjectAt<Element>(0xD8); } }

		public ChatElement Chat { get { return ReadObjectAt<ChatElement>(0xDC); } }

		public Element QuestTracker { get { return ReadObjectAt<Element>(0xF0); } }

		public Element MtxInventory { get { return ReadObjectAt<Element>(4+0xEC); } }

		public Element MtxShop { get { return ReadObjectAt<Element>(4+0xF0); } }

		public InventoryPanel InventoryPanel { get { return ReadObjectAt<InventoryPanel>(0xF8); } }

		public Element StashPanel { get { return ReadObjectAt<Element>(4+0xF8); } }

		public Element SocialPanel { get { return ReadObjectAt<Element>(4+0x104); } }

		public Element TreePanel { get { return ReadObjectAt<Element>(4+0x108); } }

		public Element CharacterPanel { get { return ReadObjectAt<Element>(4+0x10C); } }

		public Element OptionsPanel { get { return ReadObjectAt<Element>(4+0x110); } }

		public Element AchievementsPanel { get { return ReadObjectAt<Element>(4+0x114); } }

		public Element WorldPanel { get { return ReadObjectAt<Element>(4+0x118); } }

		public BigMinimap Minimap { get { return ReadObjectAt<BigMinimap>(0x120 + 8); } }

		public Element ItemsOnGroundLabels { get { return ReadObjectAt<Element>(4+0x120); } }

		public Element MonsterHpLabels { get { return ReadObjectAt<Element>(4+0x124); } }

		public Element Buffs { get { return ReadObjectAt<Element>(4+0x130); } }
		public Element Buffs2 { get { return ReadObjectAt<Element>(4+0x18c); } }

		[Display(GroupName="Hidden")]
		public Element OpenLeftPanel { get { return ReadObjectAt<Element>(4+0x154); } }
		
		[Display(GroupName = "Hidden")]
		public Element OpenRightPanel { get { return ReadObjectAt<Element>(4+0x158); } }

		public Element OpenNpcDialogPanel { get { return ReadObjectAt<Element>(4+0x160); } }

		public Element CreatureInfoPanel { get { return ReadObjectAt<Element>(4+0x184); } } // above, it shows name and hp

		public Element InstanceManagerPanel { get { return ReadObjectAt<Element>(4+0x198); } }
		public Element InstanceManagerPanel2 { get { return ReadObjectAt<Element>(4+0x19C); } }

		// dunno what it is
		public Element SwitchingZoneInfo { get { return ReadObjectAt<Element>(0x1C8); } }

		public Element GemLvlUpPanel { get { return ReadObjectAt<Element>(0x208); } }

		public Element ItemOnGroundTooltip { get { return ReadObjectAt<Element>(4+0x208); } }


		public Vec2 GetRightTopLeftOfMinimap()
		{
			Rect clientRect = Minimap.SmallMinimap.GetClientRect();
			return new Vec2(clientRect.X - 10, clientRect.Y + 5);
		}

		public Vec2 GetRightTopUnderMinimap()
		{
			var mm = Minimap.SmallMinimap;
			var gl = GemLvlUpPanel;
			Rect mmRect = mm.GetClientRect();
			Rect glRect = gl.GetClientRect();

			Rect clientRect;
			if (gl.IsVisible && glRect.X + gl.Width < mmRect.X + mmRect.X + 50) // also this +50 value doesn't seem to have any impact
				clientRect = glRect;
			else
				clientRect = mmRect;
			return new Vec2(mmRect.X + mmRect.W, clientRect.Y + clientRect.H + 10);
		}
	}
}
