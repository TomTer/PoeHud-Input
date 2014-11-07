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

		public SingleItemContainerElement HeadSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x218); } }
		public SingleItemContainerElement AmuletSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x21C); } }
		public SingleItemContainerElement ChestSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x220); } }

		public SingleItemContainerElement MainWeaponSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x224); } }
		public SingleItemContainerElement OffWeaponSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x228); } }

		public Element SwapMainWeaponSlot { get { return base.ReadObjectAfterBuffers<Element>(0x22C); } }
		public Element SwapOffWeaponSlot { get { return base.ReadObjectAfterBuffers<Element>(0x230); } }
		public SingleItemContainerElement LeftRingSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x234); } }
		public SingleItemContainerElement RightRingSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x238); } }

		public SingleItemContainerElement GlovesSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x23C); } }
		public SingleItemContainerElement BeltSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x240); } }
		public SingleItemContainerElement BootsSlot { get { return base.ReadObjectAfterBuffers<SingleItemContainerElement>(0x244); } }

		public Element BackpackInnerFrame { get { return base.ReadObjectAfterBuffers<Element>(0x248); } }

		public Element FlasksFrame { get { return base.ReadObjectAfterBuffers<Element>(0x24C); } }

		public Element GemLevelUpsFrame { get { return base.ReadObjectAfterBuffers<Element>(0x250); } }

		public Element ButtonSwapLeft { get { return base.ReadObjectAfterBuffers<Element>(0x254); } }
		public Element ButtonSwapRight { get { return base.ReadObjectAfterBuffers<Element>(0x258); } }

	}
}
