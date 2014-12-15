using System;
using System.Collections.Generic;
using System.Linq;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Poe
{
	public class Entity : RemoteMemoryObject
	{
		private int ComponentLookup { get { return M.ReadInt(Address, 0x58, 0); } }
		private int ComponentList { get { return M.ReadInt(Address + 4); } }
		private int ComponentListEnd { get { return M.ReadInt(Address + 8); } }
		public string Path { get { return M.ReadStringU(M.ReadInt(Address, 8), 256, true); } }
		public int ID { get { return M.ReadInt(Address + 0x18); } }
		public long LongId { get { return (long)(ID | Path.GetHashCode()); } }
		
		public bool IsValid { get { return M.ReadInt(Address, 8, 0) == 6619213; } }
		public bool IsHostile { get { return (M.ReadByte(Address + 0x1D) & 1) == 0; } }

		public bool HasComponent<T>() where T : Component, new()
		{
			string name = ComponentNames.Map[typeof(T)];
			return EnumComponents().Any(keyValuePair => keyValuePair.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
		}
		public T GetComponent<T>() where T : Component, new()
		{
			string name = ComponentNames.Map[typeof(T)];
			KeyValuePair<string, int> pairNeeded = EnumComponents().FirstOrDefault(keyValuePair => keyValuePair.Key.Equals(name, StringComparison.OrdinalIgnoreCase));

			return base.GetObject<T>(pairNeeded.Value);
		}

		public IEnumerable<KeyValuePair<string, int>> EnumComponents(bool uniqueOnly = false)
		{
			HashSet<string> shown = uniqueOnly ? new HashSet<string>() : null;
			int componentLookup = ComponentLookup;
			int cntCopmonents = (ComponentListEnd - ComponentList) / 4;

			for(int addr = -2; addr != componentLookup && addr != 0 && addr != -1; addr = M.ReadInt(addr))
			{
				if (addr == -2) // to pass the for cycle condition on 1st iteration
					addr = componentLookup;
				string text = M.ReadString(M.ReadInt(addr + 8), 256, true);
				int index = M.ReadInt(addr + 0xC);
				if( index >= cntCopmonents || string.IsNullOrWhiteSpace(text))
					continue;

				int componentAddress = M.ReadInt(ComponentList + index * 4);
				
				if( 0 == componentAddress)
					continue;
				if (uniqueOnly && shown.Contains(text))
					continue;
				if (uniqueOnly)
					shown.Add(text);
				yield return new KeyValuePair<string, int>(text, componentAddress);
			}

		}
		public override string ToString()
		{
			return Path;
		}


	}
}
