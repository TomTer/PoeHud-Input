using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Game;

namespace PoeHUD.Poe.EntityComponents
{
	public class Mods : Component
	{
		public Rarity ItemRarity
		{
			get
			{
				if (this.Address != 0)
				{
					return (Rarity)this.M.ReadInt(this.Address + 0x4C);
				}
				return Rarity.White;
			}
		}
		public int ItemLevel
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 0xF0);
				}
				return 1;
			}
		}
		public string UniqueName
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadStringU(this.M.ReadInt(this.Address + 12, new int[]
					{
						4,
						4
					}), 256, true);
				}
				return "";
			}
		}

		public List<ItemMod> ItemMods
		{
			get
			{
				List<ItemMod> list = new List<ItemMod>();
				if (this.Address == 0)
				{
					return list;
				}
				int i = this.M.ReadInt(this.Address + 68);
				int num = this.M.ReadInt(this.Address + 72);
				int num2 = (num - i) / 24;
				if (num2 > 12)
				{
					return list;
				}
				while (i < num)
				{
					list.Add(base.GetObject<ItemMod>(i));
					i += 24;
				}
				return list;
			}
		}
		public List<ItemMod> ImplicitMods
		{
			get
			{
				List<ItemMod> list = new List<ItemMod>();
				if (this.Address == 0)
				{
					return list;
				}
				int i = this.M.ReadInt(this.Address + 52);
				int num = this.M.ReadInt(this.Address + 56);
				int num2 = (num - i) / 24;
				if (num2 > 100 || num2 <= 0)
				{
					return list;
				}
				while (i < num)
				{
					list.Add(base.GetObject<ItemMod>(i));
					i += 24;
				}
				return list;
			}
		}

		/*
		 * 
		 * {
            get
            {
                var implicitMods = GetMods(0x50, 0x54);
                var explicitMods = GetMods(0x60, 0x64);
                return implicitMods.Concat(explicitMods).ToList();
            }
        }

        private List<ItemMod> GetMods(int startOffset, int endOffset)
        {
            var list = new List<ItemMod>();
            if (Address == 0)
                return list;

            int begin = M.ReadInt(Address + startOffset);
            int end = M.ReadInt(Address + endOffset);
            int count = (end - begin) / 24;
            if (count > 12)
                return list;

            for (int i = begin; i < end; i += 24)
                list.Add(base.GetObject<ItemMod>(i));

            return list;
        }
		 * */
	}
}
