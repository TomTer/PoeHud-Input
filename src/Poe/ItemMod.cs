using PoeHUD.Framework;

namespace PoeHUD.Poe
{
	public class ItemMod : RemoteMemoryObject
	{
		private string rawName;
		private string name;
		private int level;
		public int Value1 { get { return this.M.ReadInt(this.Address, 0); } }
		public int Value2 { get { return this.M.ReadInt(this.Address, 4); } }
		public int Value3 { get { return this.M.ReadInt(this.Address, 8); } }
		public int Value4 { get { return this.M.ReadInt(this.Address, 12); } }

		public string RawName
		{
			get
			{
				if (this.rawName == null)
					this.ParseName();
				return this.rawName;
			}
		}
		public string Name
		{
			get
			{
				if (this.rawName == null)
					this.ParseName();
				return this.name;
			}
		}
		public int Level
		{
			get
			{
				if (this.rawName == null)
					this.ParseName();
				return this.level;
			}
		}
		private void ParseName()
		{
			this.rawName = this.M.ReadStringU(this.M.ReadInt(this.Address + 20, 0), 256, true);
			this.name = this.rawName.Replace("_", ""); // Master Crafted mod can have underscore on the end, need to ignore
			int ixDigits = this.name.IndexOfAny("0123456789".ToCharArray());
			if (ixDigits < 0 || !int.TryParse(this.name.Substring(ixDigits), out this.level))
			{
				this.level = 1;
			}
			else
			{
				this.name = this.name.Substring(0, ixDigits);
			}
		}
	}
}
