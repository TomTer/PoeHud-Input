using System.Collections.Generic;
using PoeHUD.Framework;

namespace PoeHUD.Controllers
{
	public class FileIndex
	{
		private Dictionary<string, int> files;
		private Memory mem;
		public BaseItemTypes BaseItemTypes;
		private ModsDat Mods;

		public FileIndex(Memory mem)
		{
			this.files = new Dictionary<string, int>();
			this.mem = mem;
			this.BaseItemTypes = new BaseItemTypes(mem, this.FindFile("Data/BaseItemTypes.dat"));
			this.Mods = new ModsDat(mem, this.FindFile("Data/Mods.dat"));
		}
		public int FindFile(string name)
		{
			if (!this.files.ContainsKey(name))
			{
				int num = this.mem.ReadInt(this.mem.BaseAddress + mem.offsets.FileRoot, 8 );
				for (int num2 = this.mem.ReadInt(num); num2 != num; num2 = this.mem.ReadInt(num2))
				{
					string text = this.mem.ReadStringU(this.mem.ReadInt(num2 + 8), 512, true);
					if (text.Contains("."))
					{
						this.files.Add(text, this.mem.ReadInt(num2 + 12));
					}
				}
			}
			return this.files[name];
		}
	}
}
