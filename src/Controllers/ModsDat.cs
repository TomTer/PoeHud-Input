using System;
using System.Collections.Generic;
using PoeHUD.Framework;

namespace PoeHUD.Controllers
{
	public class ModsDat : FileInMemory
	{
		public class ModRecord
		{
			public readonly string Key;
			public int Unknown_4;
			public int MinLevel;
			public string StatName1; // game refers to Stats.dat line
			public string StatName2;
			public string StatName3;
			public string StatName4;
			public int Domain;
			public string UserFriendlyName;
			public int Generaion;
			public string Group;
			public Tuple<int, int> Stat1Range;
			public Tuple<int, int> Stat2Range;
			public Tuple<int, int> Stat3Range;
			public Tuple<int, int> Stat4Range;
			public string[] Tags; // game refers to Tags.dat line
			public int[] TagChances;
			// more fields can be added (see in visualGGPK)

			public ModRecord(Memory m, int addr)
			{
				Key = m.ReadStringU(m.ReadInt(addr + 0), 255);
				Unknown_4 = m.ReadInt(addr + 4);
				MinLevel = m.ReadInt(addr + 8);

				int addrStat1 = m.ReadInt(addr + 0x10);
				StatName1 = addrStat1 == 0 ? null : m.ReadStringU(m.ReadInt(addrStat1), 255);
				int addrStat2 = m.ReadInt(addr + 0x18);
				StatName2 = addrStat2 == 0 ? null : m.ReadStringU(m.ReadInt(addrStat2), 255);
				int addrStat3 = m.ReadInt(addr + 0x20);
				StatName3 = addrStat3 == 0 ? null : m.ReadStringU(m.ReadInt(addrStat3), 255);
				int addrStat4 = m.ReadInt(addr + 0x28);
				StatName4 = addrStat4 == 0 ? null : m.ReadStringU(m.ReadInt(addrStat4), 255);

				Domain = m.ReadInt(addr + 0x2C);
				UserFriendlyName = m.ReadStringU(addr + 0x30, 255);
				Generaion = m.ReadInt(addr + 0x34);
				Group = m.ReadStringU(addr + 0x38, 255);

				Stat1Range = Tuple.Create(m.ReadInt(addr + 0x3C), m.ReadInt(addr + 0x40));
				Stat2Range = Tuple.Create(m.ReadInt(addr + 0x44), m.ReadInt(addr + 0x48));
				Stat3Range = Tuple.Create(m.ReadInt(addr + 0x4C), m.ReadInt(addr + 0x50));
				Stat4Range = Tuple.Create(m.ReadInt(addr + 0x54), m.ReadInt(addr + 0x58));

				Tags = new string[m.ReadInt(addr + 0x5C)];
				int ta = m.ReadInt(addr + 0x60);
				for(int i = 0; i < Tags.Length; i++ ){
					var ii = ta + 4 + 8 * i;
					Tags[i] = m.ReadStringU(m.ReadInt(ii, 0), 255);
				}

				TagChances = new int[m.ReadInt(addr + 0x64)];
				int tc = m.ReadInt(addr + 0x68);
				for (int i = 0; i < Tags.Length; i++)
					TagChances[i] = m.ReadInt(tc + 4 * i);

			}
		}

		public Dictionary<string, ModRecord> records = new Dictionary<string, ModRecord>(StringComparer.OrdinalIgnoreCase);

		public ModsDat(Memory m, int address) : base(m, address) {
			loadItems();
		}

		private void loadItems()
		{
			foreach(var addr in RecordAdresses())
			{
				ModRecord r = new ModRecord(m, addr);
				records.Add(r.Key, r);
			}
		}

	}
}