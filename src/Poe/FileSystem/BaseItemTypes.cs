using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;

namespace PoeHUD.Poe.Files
{
	public class BaseItemTypes : FileInMemory
	{
		private Dictionary<string, string> contents = new Dictionary<string, string>();

		public BaseItemTypes(Memory m, int address) : base(m, address) {}
		public string Translate(string metadata)
		{
			if (!this.contents.ContainsKey(metadata))
			{
				loadItemTypes();
			}
			if (!this.contents.ContainsKey(metadata))
			{
				Console.WriteLine("Key not found in BaseItemTypes: " + metadata);
				return metadata;
			}
			return this.contents[metadata];
		}

		private void loadItemTypes()
		{
			foreach (int i in RecordAdresses())
			{
				string key = this.m.ReadStringU(this.m.ReadInt(i), 256, true);
				string value = this.m.ReadStringU(this.m.ReadInt(i + 16), 256, true);
				if (!this.contents.ContainsKey(key))
				{
					this.contents.Add(key, value);
				}
			}
		}
	}
}
