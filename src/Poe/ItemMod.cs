using PoeHUD.Framework;

namespace PoeHUD.Poe
{
	public class ItemMod : RemoteMemoryObject
	{
		private string name;
		private int level;
		public int Value1
		{
			get
			{
				Memory arg_14_0 = this.m;
				int arg_14_1 = this.address;
				int[] offsets = new int[1];
				return arg_14_0.ReadInt(arg_14_1, offsets);
			}
		}
		public int Value2
		{
			get
			{
				return this.m.ReadInt(this.address, new int[]
				{
					4
				});
			}
		}
		public string Name
		{
			get
			{
				if (this.name == null)
				{
					this.ParseName();
				}
				return this.name;
			}
		}
		public int Level
		{
			get
			{
				if (this.name == null)
				{
					this.ParseName();
				}
				return this.level;
			}
		}
		private void ParseName()
		{
			Memory arg_29_0 = this.m;
			Memory arg_1E_0 = this.m;
			int arg_1E_1 = this.address + 20;
			int[] offsets = new int[1];
			this.name = arg_29_0.ReadStringU(arg_1E_0.ReadInt(arg_1E_1, offsets), 256, true);
			this.name = this.name.Replace("_", ""); // Master Crafted mod can have underscore on the end, need to ignore
			if (this.name.IndexOfAny("0123456789".ToCharArray()) < 0 || !int.TryParse(this.name.Substring(this.name.IndexOfAny("0123456789".ToCharArray())), out this.level))
			{
				this.level = 1;
			}
			else
			{
				this.name = this.name.Substring(0, this.name.IndexOfAny("0123456789".ToCharArray()));
			}
		}
	}
}
