using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PoeHUD.Poe
{
	public class EntityList : RemoteMemoryObject
	{
		private Stopwatch stopwatch = new Stopwatch();
		public List<Entity> Entities
		{
			get
			{
				return this.EntitiesAsDictionary.Values.ToList<Entity>();
			}
		}
		public Dictionary<int, Entity> EntitiesAsDictionary
		{
			get
			{
				Dictionary<int, Entity> dictionary = new Dictionary<int, Entity>();
				this.CollectEntities(this.M.ReadInt(this.Address + 12), dictionary);
				return dictionary;
			}
		}
		private void CollectEntities(int addr, Dictionary<int, Entity> list)
		{
			int num = addr;
			addr = this.M.ReadInt(addr + 4);
			HashSet<int> hashSet = new HashSet<int>();
			Queue<int> queue = new Queue<int>();
			queue.Enqueue(addr);
			while (queue.Count > 0)
			{
				int nextAddr = queue.Dequeue();
				if (hashSet.Contains(nextAddr)) 
					continue;

				hashSet.Add(nextAddr);
				if (this.M.ReadByte(nextAddr + 21) == 0 && nextAddr != num && nextAddr != 0)
				{
					int key = this.M.ReadInt(nextAddr + 12);
					if (!list.ContainsKey(key))
					{
						int address = this.M.ReadInt(nextAddr + 16);
						Entity @object = base.GetObject<Entity>(address);
						list.Add(key, @object);
					}
					queue.Enqueue(this.M.ReadInt(nextAddr));
					queue.Enqueue(this.M.ReadInt(nextAddr + 8));
				}
			}
		}
	}
}
