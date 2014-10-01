using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Game;

namespace PoeHUD.Poe.EntityComponents
{
	public class ObjectMagicProperties : Component
	{
		public MonsterRarity Rarity
		{
			get
			{
				if (this.address != 0)
				{
					return (MonsterRarity)this.m.ReadInt(this.address + 36);
				}
				return MonsterRarity.White;
			}
		}
		public List<string> Mods
		{
			get
			{
				if (this.address == 0)
				{
					return new List<string>();
				}
				int num = this.m.ReadInt(this.address + 56);
				int num2 = this.m.ReadInt(this.address + 60);
				List<string> list = new List<string>();
				if (num == 0 || num2 == 0)
				{
					return list;
				}
				for (int i = num; i < num2; i += 24)
				{
					Memory arg_6F_0 = this.m;
					Memory arg_64_0 = this.m;
					int arg_64_1 = i + 20;
					int[] offsets = new int[1];
					string item = arg_6F_0.ReadStringU(arg_64_0.ReadInt(arg_64_1, offsets), 256, true);
					list.Add(item);
				}
				return list;
			}
		}
	}
}
