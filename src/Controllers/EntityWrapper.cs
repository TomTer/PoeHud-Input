using System;
using System.Collections.Generic;
using System.Linq;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Controllers
{
	public class EntityWrapper
	{
		private readonly Poe.Entity InternalEntity;
		private readonly GameController Root;
		private readonly Dictionary<string, int> Components;
		private readonly int cachedId;
		public bool IsInList = true;

		public string Path { get; private set; }
		public bool IsValid { get { return this.InternalEntity.IsValid && this.IsInList && this.cachedId == this.InternalEntity.ID; } }
		public int Address { get { return this.InternalEntity.Address; } }
		public int Id { get { return this.cachedId; } }
		public bool IsHostile { get { return this.InternalEntity.IsHostile; } }
		public long LongId { get; private set; }
		public bool IsAlive { get { return this.GetComponent<Life>().CurHP > 0; } }
		public Vec3 Pos
		{
			get
			{
				var p = this.GetComponent<Positioned>();
				return new Vec3(p.X, p.Y, this.GetComponent<Render>().Z);
			}
		}
		public List<EntityWrapper> Minions
		{
			get
			{
				return this.GetComponent<Actor>().Minions.Select(current => this.Root.GetEntityByID(current)).Where(byId => byId != null).ToList();
			}
		}
		public EntityWrapper(GameController Poe, Poe.Entity entity)
		{
			this.Root = Poe;

			this.InternalEntity = entity;
			this.Components = new Dictionary<string, int>();
			foreach (KeyValuePair<string, int> kv in this.InternalEntity.EnumComponents(true)) {
				Components.Add(kv.Key, kv.Value);
			}
			this.Path = this.InternalEntity.Path;
			this.cachedId = this.InternalEntity.ID;
			this.LongId = this.InternalEntity.LongId;
		}
		public EntityWrapper(GameController Poe, int address) : this(Poe, Poe.Internal.GetObject<Poe.Entity>(address))
		{
		}
		public T GetComponent<T>() where T : Component, new()
		{
			int addr = this.Components.TryGetValue(ComponentNames.Map[typeof(T)], out addr) ? addr : 0;
			return this.Root.Internal.GetObject<T>(addr);
		}
		public bool HasComponent<T>() where T : Component, new()
		{
			return this.Components.ContainsKey(ComponentNames.Map[typeof(T)]);
		}
		public void PrintComponents()
		{
			Console.WriteLine(this.InternalEntity.Path + " " + this.InternalEntity.Address.ToString("X"));
			foreach (KeyValuePair<string, int> current in this.Components)
			{
				Console.WriteLine(current.Key + " " + current.Value.ToString("X"));
			}
		}
		public override bool Equals(object obj)
		{
			EntityWrapper entity = obj as EntityWrapper;
			return entity != null && entity.LongId == this.LongId;
		}
		public override int GetHashCode()
		{
			return this.LongId.GetHashCode();
		}

		public override string ToString()
		{
			return "EntityWrapper: " + Path;
		}
	}
}
