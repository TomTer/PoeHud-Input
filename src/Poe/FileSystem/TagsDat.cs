using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;

namespace PoeHUD.Poe.Files
{
	public class TagsDat : FileInMemory
	{

		public class TagRecord
		{
			public readonly string Key;
			public int Hash;
			// more fields can be added (see in visualGGPK)

			public TagRecord(Memory m, int addr)
			{
				Key = m.ReadStringU(m.ReadInt(addr + 0), 255);
				Hash = m.ReadInt(addr + 4);
			}
		}

		public Dictionary<string, TagRecord> records = new Dictionary<string, TagRecord>(StringComparer.OrdinalIgnoreCase);


		public TagsDat(Memory m, int address)
			: base(m, address)
		{
			loadItems();
		}

		private void loadItems()
		{
			foreach (var addr in RecordAdresses())
			{
				TagRecord r = new TagRecord(m, addr);
				records.Add(r.Key, r);
			}
		}
	}
}