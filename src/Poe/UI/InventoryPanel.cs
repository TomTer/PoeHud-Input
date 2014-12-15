using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoeHUD.Poe.UI
{
	public class InventoryPanel : Element
	{
		// offsets 1E0 -> 204 are texture refreneces (used to draw the ui probably)

		public Element BackpackOuterFrame { get { return base.ReadObjectAfterBuffers<Element>(0x208); } }
		public Element WindowCaption { get { return base.ReadObjectAfterBuffers<Element>(0x20C); } }

		public Element CloseButton { get { return base.ReadObjectAfterBuffers<Element>(0x210); } }

		// 214 is something about weapon swap (just a ptr to vtable, with strings nearb)

		public ItemContainer HeadSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x218); } }
		public ItemContainer AmuletSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x21C); } }
		public ItemContainer ChestSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x220); } }

		public ItemContainer MainWeaponSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x224); } }
		public ItemContainer OffWeaponSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x228); } }

		public Element SwapMainWeaponSlot { get { return base.ReadObjectAfterBuffers<Element>(0x22C); } }
		public Element SwapOffWeaponSlot { get { return base.ReadObjectAfterBuffers<Element>(0x230); } }
		public ItemContainer LeftRingSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x234); } }
		public ItemContainer RightRingSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x238); } }

		public ItemContainer GlovesSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x23C); } }
		public ItemContainer BeltSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x240); } }
		public ItemContainer BootsSlot { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x244); } }

		public ItemContainer BackpackInnerFrame { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x248); } }

		public ItemContainer FlasksFrame { get { return base.ReadObjectAfterBuffers<ItemContainer>(0x24C); } }

		public Element GemLevelUpsFrame { get { return base.ReadObjectAfterBuffers<Element>(0x250); } }

		public Element ButtonSwapLeft { get { return base.ReadObjectAfterBuffers<Element>(0x254); } }
		public Element ButtonSwapRight { get { return base.ReadObjectAfterBuffers<Element>(0x258); } }

	}
}
