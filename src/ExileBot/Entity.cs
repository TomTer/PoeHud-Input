using System;
using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.ExileBot
{
	public class Entity
	{
		private Poe.Entity InternalEntity;
		private PathOfExile Poe;
		private Dictionary<string, int> Components;
		private int cachedId;
		public bool IsInList = true;
		public string Path
		{
			get;
			private set;
		}
		public bool IsValid
		{
			get
			{
				return this.InternalEntity.IsValid && this.IsInList && this.cachedId == this.InternalEntity.ID;
			}
		}
		public int Address
		{
			get
			{
				return this.InternalEntity.address;
			}
		}
		public int Id
		{
			get
			{
				return this.cachedId;
			}
		}
		public bool IsHostile
		{
			get
			{
				return this.InternalEntity.IsHostile;
			}
		}
		public long LongId
		{
			get;
			private set;
		}
		public bool IsAlive
		{
			get
			{
				return this.GetComponent<Life>().CurHP > 0;
			}
		}
		public Vec3 Pos
		{
			get
			{
				return new Vec3(this.GetComponent<Positioned>().X, this.GetComponent<Positioned>().Y, this.GetComponent<Render>().Z);
			}
		}
		public List<Entity> Minions
		{
			get
			{
				List<int> minions = this.GetComponent<Actor>().Minions;
				List<Entity> list = new List<Entity>();
				foreach (int current in minions)
				{
					Entity byID = this.Poe.EntityList.GetByID(current);
					if (byID != null)
					{
						list.Add(byID);
					}
				}
				return list;
			}
		}
		public Entity(PathOfExile Poe, Poe.Entity entity)
		{
			this.Poe = Poe;
			this.InternalEntity = entity;
			this.Components = this.InternalEntity.GetComponents();
			this.Path = this.InternalEntity.Path;
			this.cachedId = this.InternalEntity.ID;
			this.LongId = this.InternalEntity.LongId;
		}
		public Entity(PathOfExile Poe, int address) : this(Poe, Poe.Internal.GetObject<Poe.Entity>(address))
		{
		}
		public T GetComponent<T>() where T : Component, new()
		{
			string name = typeof(T).Name;
			if (!this.Components.ContainsKey(name))
			{
				return this.Poe.Internal.GetObject<T>(0);
			}
			return this.Poe.Internal.GetObject<T>(this.Components[name]);
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
			Entity entity = obj as Entity;
			return entity != null && entity.LongId == this.LongId;
		}
		public override int GetHashCode()
		{
			return this.LongId.GetHashCode();
		}
	}
}
