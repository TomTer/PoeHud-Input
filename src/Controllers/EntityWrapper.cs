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
		public int Address { get { return this.InternalEntity.address; } }
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
			this.Components = this.InternalEntity.GetComponents();
			this.Path = this.InternalEntity.Path;
			this.cachedId = this.InternalEntity.ID;
			this.LongId = this.InternalEntity.LongId;
		}
		public EntityWrapper(GameController Poe, int address) : this(Poe, Poe.Internal.GetObject<Poe.Entity>(address))
		{
		}
		public T GetComponent<T>() where T : Component, new()
		{
			string name = typeof(T).Name;
			return this.Root.Internal.GetObject<T>(this.Components.ContainsKey(name) ? this.Components[name] : 0);
		}
		public bool HasComponent<T>() where T : Component, new()
		{
			return this.Components.ContainsKey(typeof(T).Name);
		}
		public void PrintComponents()
		{
			Console.WriteLine(this.InternalEntity.Path + " " + this.InternalEntity.address.ToString("X"));
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
