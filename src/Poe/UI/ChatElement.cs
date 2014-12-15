using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoeHUD.Poe.UI
{
	public class ChatElement : Element
	{

		public override T ReadObjectAfterBuffers<T>(int offset)
		{
			return base.ReadObjectAfterBuffers<T>(offset + UiElementSize + 0x20C);
		}

		public Element CaptionPanel { get { return ReadObjectAfterBuffers<Element>(0x0); } }

		public Element CheckBoxGlobal { get { return ReadObjectAfterBuffers<Element>(0x4); } }
		public Element CheckBoxTrade { get { return ReadObjectAfterBuffers<Element>(0x8); } }
		public Element CheckBoxGuild { get { return ReadObjectAfterBuffers<Element>(0xC); } }

		public Element LedGlobal { get { return ReadObjectAfterBuffers<Element>(0x10); } }
		public Element LedTrade { get { return ReadObjectAfterBuffers<Element>(0x14); } }

		public Element ExtraTrade { get { return ReadObjectAfterBuffers<Element>(0x18); } }
		public Element ExtraGlobal { get { return ReadObjectAfterBuffers<Element>(0x1C); } }

		public Element ExtraTrade1 { get { return ReadObjectAfterBuffers<Element>(0x20); } }
		public Element CaptionGlobal { get { return ReadObjectAfterBuffers<Element>(0x24); } }
		public Element CaptionTrade { get { return ReadObjectAfterBuffers<Element>(0x28); } }

		public ChatMessageList MessageList { get { return ReadObjectAfterBuffers<ChatMessageList>(0x2C); } }

		public Element ChannelDropDown { get { return ReadObjectAfterBuffers<Element>(0x30); } }

		public Element TextInput { get { return ReadObjectAfterBuffers<Element>(0x3C); } }


		public Element Unknown54 { get { return ReadObjectAfterBuffers<Element>(0x54); } }
		
	}

	public class ChatMessageList : Element
	{

		public override T ReadObjectAfterBuffers<T>(int offset)
		{
			return base.ReadObjectAfterBuffers<T>(offset + 0x104 + UiElementSize);
		}

		public Element LeftTopUnknown { get { return ReadObjectAfterBuffers<Element>(0x30); } }
		public Element ScrollButtons { get { return ReadObjectAfterBuffers<Element>(0x34); } }
	}

}
