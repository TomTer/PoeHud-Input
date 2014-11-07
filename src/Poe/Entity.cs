using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Poe
{
	public class Entity : RemoteMemoryObject
	{
		private int ComponentLookup { get { return this.m.ReadInt(this.address, 0x58, 0); } }
		private int ComponentList { get { return this.m.ReadInt(this.address + 4); } }
		public string Path { get { return this.m.ReadStringU(this.m.ReadInt(this.address, 8), 256, true); } }
		public int ID { get { return this.m.ReadInt(this.address + 0x18); } }
		public long LongId { get { return (long)(this.ID | this.Path.GetHashCode()); } }
		
		public bool IsValid
		{
			get
			{
				return this.m.ReadInt(this.address, 8, 0) == 6619213;
			}
		}
		public bool IsHostile
		{
			get
			{
				return (this.m.ReadByte(this.address + 0x1D) & 1) == 0;
			}
		}

		public IEnumerable<int> EnumComponentAdresses(){
			int start = this.m.ReadInt(this.address + 4);
			int end = this.m.ReadInt(this.address + 8);
			while(start < end) {
				yield return this.m.ReadInt(start);
				start += 4;
			}
		}


		public bool HasComponent<T>() where T : Component, new()
		{
			string name = typeof(T).Name;
			int componentLookup = this.ComponentLookup;
			int num = componentLookup;
			int num2 = 0;
			while (true)
			{
				string text = this.m.ReadString(this.m.ReadInt(num + 8), 256, true);
				if (text.Equals(name))
				{
					break;
				}
				num = this.m.ReadInt(num);
				num2++;
				if (num == componentLookup || num == 0 || num == -1 || num2 >= 200)
				{
					return false;
				}
			}
			return true;
		}
		public T GetComponent<T>() where T : Component, new()
		{
			string name = typeof(T).Name;
			int componentLookup = this.ComponentLookup;
			int num = componentLookup;
			int num2 = 0;
			while (true)
			{
				string text = this.m.ReadString(this.m.ReadInt(num + 8), 256, true);
				if (text.Equals(name))
				{
					break;
				}
				num = this.m.ReadInt(num);
				num2++;
				if (num == componentLookup || num == 0 || num == -1 || num2 >= 200)
				{
					goto IL_8D;
				}
			}
			int num3 = this.m.ReadInt(num + 12);
			return base.ReadObject<T>(this.ComponentList + num3 * 4);
			IL_8D:
			return base.GetObject<T>(0);
		}
		public Dictionary<string, int> GetComponents()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int componentLookup = this.ComponentLookup;
			int num = componentLookup;
			do
			{
				string text = this.m.ReadString(this.m.ReadInt(num + 8), 256, true);
				int value = this.m.ReadInt(this.ComponentList + this.m.ReadInt(num + 12) * 4);
				if (!dictionary.ContainsKey(text) && !string.IsNullOrWhiteSpace(text))
				{
					dictionary.Add(text, value);
				}
				num = this.m.ReadInt(num);
			}
			while (num != componentLookup && num != 0 && num != -1);
			return dictionary;
		}
		public override string ToString()
		{
			return this.Path;
		}


	}
}
