using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoeHUD.Poe.UI
{
	public class SingleItemContainerElement : Element
	{
		public Entity GetItem()
		{
			var addrContainer = this.m.ReadInt(this.address + OffsetBuffers + UiElementSize + 0x20);
			var addrList = m.ReadInt(addrContainer + 0x20);
			var addrListEnd = m.ReadInt(addrContainer + 0x24);

			var firstItem = m.ReadInt(addrList); // skinned item or something

			return this.ReadObject<Entity>(firstItem);

		}
	}
}
