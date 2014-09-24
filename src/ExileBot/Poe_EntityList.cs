using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace ExileBot
{
	public class Poe_EntityList : RemoteMemoryObject
	{
		private Stopwatch stopwatch = new Stopwatch();
		public List<Poe_Entity> Entities
		{
			get
			{
				return this.EntitiesAsDictionary.Values.ToList<Poe_Entity>();
			}
		}
		public Dictionary<int, Poe_Entity> EntitiesAsDictionary
		{
			get
			{
				Dictionary<int, Poe_Entity> dictionary = new Dictionary<int, Poe_Entity>();
				this.CollectEntities(this.m.ReadInt(this.address + 12), dictionary);
				return dictionary;
			}
		}
		private void CollectEntities(int addr, Dictionary<int, Poe_Entity> list)
		{
			int num = addr;
			addr = this.m.ReadInt(addr + 4);
			HashSet<int> hashSet = new HashSet<int>();
			Queue<int> queue = new Queue<int>();
			queue.Enqueue(addr);
			while (queue.Count > 0)
			{
				int num2 = queue.Dequeue();
				if (!hashSet.Contains(num2))
				{
					hashSet.Add(num2);
					if (this.m.ReadByte(num2 + 21) == 0 && num2 != num && num2 != 0)
					{
						int key = this.m.ReadInt(num2 + 12);
						if (!list.ContainsKey(key))
						{
							int address = this.m.ReadInt(num2 + 16);
							Poe_Entity @object = base.GetObject<Poe_Entity>(address);
							list.Add(key, @object);
						}
						queue.Enqueue(this.m.ReadInt(num2));
						queue.Enqueue(this.m.ReadInt(num2 + 8));
					}
				}
			}
		}
	}
}
