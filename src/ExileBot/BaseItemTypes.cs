using System;
using System.Collections.Generic;
using ExileHUD.Framework;

namespace ExileHUD.ExileBot
{
	public class BaseItemTypes
	{
		private Memory m;
		private int address;
		private Dictionary<string, string> contents;
		public BaseItemTypes(Memory m, int address)
		{
			this.m = m;
			this.address = address;
			this.contents = new Dictionary<string, string>();
		}
		public string Translate(string metadata)
		{
			if (!this.contents.ContainsKey(metadata))
			{
				int i = this.m.ReadInt(this.address + 48);
				int num = this.m.ReadInt(this.address + 52);
				while (i < num)
				{
					string key = this.m.ReadStringU(this.m.ReadInt(i), 256, true);
					string value = this.m.ReadStringU(this.m.ReadInt(i + 16), 256, true);
					if (!this.contents.ContainsKey(key))
					{
						this.contents.Add(key, value);
					}
					i += 176;
				}
			}
			if (!this.contents.ContainsKey(metadata))
			{
				Console.WriteLine("Key not found in BaseItemTypes: " + metadata);
				return metadata;
			}
			return this.contents[metadata];
		}
	}
}
