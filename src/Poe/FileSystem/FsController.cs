using System.Collections.Generic;
using PoeHUD.Framework;

namespace PoeHUD.Poe.Files
{
	public class FsController
	{
		private readonly Dictionary<string, int> files;
		private readonly Memory mem;
		public readonly BaseItemTypes BaseItemTypes;
		public readonly ModsDat Mods;
		public readonly StatsDat Stats;
		public readonly TagsDat Tags;

		public FsController(Memory mem)
		{
			this.files = new Dictionary<string, int>();
			this.mem = mem;
			this.BaseItemTypes = new BaseItemTypes(mem, this.FindFile("Data/BaseItemTypes.dat"));
			this.Tags = new TagsDat(mem, this.FindFile("Data/Tags.dat"));
			this.Stats = new StatsDat(mem, this.FindFile("Data/Stats.dat"));
			this.Mods = new ModsDat(mem, this.FindFile("Data/Mods.dat"), Stats, Tags);
		}

		public int FindFile(string name)
		{
			if (!this.files.ContainsKey(name))
			{
				int num = this.mem.ReadInt(this.mem.BaseAddress + mem.offsets.FileRoot, 8 );
				for (int num2 = this.mem.ReadInt(num); num2 != num; num2 = this.mem.ReadInt(num2))
				{
					string text = this.mem.ReadStringU(this.mem.ReadInt(num2 + 8), 512);
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
