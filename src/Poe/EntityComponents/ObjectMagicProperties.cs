using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Game;

namespace PoeHUD.Poe.EntityComponents
{
	public class ObjectMagicProperties : Component
	{
		public Rarity Rarity
		{
			get
			{
				if (Address != 0)
				{
					return (Rarity)M.ReadInt(Address + 40 + 0x18);
				}
				return Rarity.White;
			}
		}

		public List<string> Mods
		{
			get
			{
				if (this.Address == 0)
				{
					return new List<string>();
				}
				int num = this.M.ReadInt(this.Address + 54);
				int num2 = this.M.ReadInt(this.Address + 58);
				List<string> list = new List<string>();
				if (num == 0 || num2 == 0)
				{
					return list;
				}
				for (int i = num; i < num2; i += 24)
				{
					string item = this.M.ReadStringU(this.M.ReadInt(i + 20, 0), 256, true);
					list.Add(item);
				}
				return list;
			}
		}
	}
}
