using System.Collections.Generic;
using ExileHUD.Framework;

namespace ExileHUD.ExileBot
{
	public class FileIndex
	{
		private Dictionary<string, int> files;
		private Memory mem;
		public BaseItemTypes BaseItemTypes;
		public FileIndex(Memory mem)
		{
			this.files = new Dictionary<string, int>();
			this.mem = mem;
			this.BaseItemTypes = new BaseItemTypes(mem, this.FindFile("Data/BaseItemTypes.dat"));
		}
		public int FindFile(string name)
		{
			if (!this.files.ContainsKey(name))
			{
				int num = this.mem.ReadInt(this.mem.BaseAddress + mem.offsets.FileRoot, new int[]
				{
					8
				});
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
