using System.Collections.Generic;
using System.Diagnostics;

namespace PoeHUD.Controllers
{
	public interface EntityListObserver {
		void EntityAdded(EntityWrapper entity);
		void EntityRemoved(EntityWrapper entity);
	}


	public class EntityListObserverComposite : EntityListObserver
	{
		public readonly List<EntityListObserver> Observers = new List<EntityListObserver>();
		public void EntityRemoved(EntityWrapper entity)
		{
			foreach (var observer in Observers)
				observer.EntityRemoved(entity);
		}

		public void EntityAdded(EntityWrapper entity)
		{
			foreach (var observer in Observers)
				observer.EntityAdded(entity);
		}
	}


	public class EntityListWrapper
	{
		private class EntityListBlankObserver : EntityListObserver
		{
			public void EntityAdded(EntityWrapper entity) { }
			public void EntityRemoved(EntityWrapper entity) { }
		}

		private readonly GameController Root;
		private Dictionary<int, EntityWrapper> entityCache;
		private readonly HashSet<string> ignoredEntities;
		private Stopwatch stopwatch = new Stopwatch();

		public EntityListObserver Observer = new EntityListBlankObserver();

		public ICollection<EntityWrapper> Entities { get { return this.entityCache.Values; } }

		public EntityWrapper Player { get; private set; }

		public IDictionary<int, EntityWrapper> EntitiesCache { get { return this.entityCache; } }

		public EntityListWrapper(GameController root)
		{
			this.Root = root;
			this.entityCache = new Dictionary<int, EntityWrapper>();
			this.ignoredEntities = new HashSet<string>();
			Root.Area.OnAreaChange += this.AreaChanged;
		}
		private void AreaChanged(AreaController area)
		{
			this.ignoredEntities.Clear();
			foreach (EntityWrapper current in this.entityCache.Values)
			{
				current.IsInList = false;
				this.Observer.EntityRemoved(current);
			}
			this.entityCache.Clear();
			int address = this.Root.Internal.IngameState.Data.LocalPlayer.address;
			if (this.Player == null || this.Player.Address != address) {
				this.Player = new EntityWrapper(this.Root, address);
			}
		}
		public void RefreshState()
		{
			int address = this.Root.Internal.IngameState.Data.LocalPlayer.address;
			if ((this.Player == null) || (this.Player.Address != address))
			{
				this.Player = new EntityWrapper(this.Root, address);
			}

			Dictionary<int, Poe.Entity> currentEntities = this.Root.Internal.IngameState.Data.EntityList.EntitiesAsDictionary;
			Dictionary<int, EntityWrapper> newCache = new Dictionary<int, EntityWrapper>();
			foreach (KeyValuePair<int, Poe.Entity> kv in currentEntities)
			{
				int key = kv.Key;
				string item = kv.Value.Path + key;
				if (this.ignoredEntities.Contains(item)) 
					continue;

				if (this.entityCache.ContainsKey(key) && this.entityCache[key].IsValid)
				{
					newCache.Add(key, this.entityCache[key]);
					this.entityCache[key].IsInList = true;
					this.entityCache.Remove(key);
					continue;
				}

				if (this.entityCache.ContainsKey(key))
					this.entityCache.Remove(key);

				EntityWrapper entity = new EntityWrapper(this.Root, kv.Value);
				if ((entity.Path.StartsWith("Metadata/Effects") || ((((long) kv.Key) & 0x80000000L) != 0L)) || entity.Path.StartsWith("Metadata/Monsters/Daemon")) {
					this.ignoredEntities.Add(item);
					continue;
				}

				if (!entity.IsValid)
					continue;

				Observer.EntityAdded(entity);
				newCache.Add(key, entity);
			}

			foreach (EntityWrapper entity2 in this.entityCache.Values)
			{
				Observer.EntityRemoved(entity2);
				entity2.IsInList = false;
			}
			this.entityCache = newCache;
		}

		public EntityWrapper GetByID(int id)
		{
			EntityWrapper result;
			return entityCache.TryGetValue(id, out result) ? result : null;
		}
	}
}
