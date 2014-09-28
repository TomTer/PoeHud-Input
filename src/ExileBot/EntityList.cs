using System.Collections.Generic;
using System.Diagnostics;

namespace ExileHUD.ExileBot
{
	public class EntityList
	{
		private PathOfExile Poe;
		private Dictionary<int, Entity> entities;
		private HashSet<string> blackList;
		private Stopwatch stopwatch = new Stopwatch();
		public event EntityEvent OnEntityAdded;
		public event EntityEvent OnEntityRemoved;
		public ICollection<Entity> Entities
		{
			get
			{
				return this.entities.Values;
			}
		}
		public Entity Player
		{
			get;
			private set;
		}
		public Dictionary<int, Entity> EntitiesHashmap
		{
			get
			{
				return this.entities;
			}
		}
		public EntityList(PathOfExile poe)
		{
			this.Poe = poe;
			this.entities = new Dictionary<int, Entity>();
			this.blackList = new HashSet<string>();
			poe.Area.OnAreaChange += this.AreaChanged;
			poe.OnUpdate += new UpdateEvent(this.Update);
		}
		private void AreaChanged(AreaController area)
		{
			this.blackList.Clear();
			foreach (Entity current in this.entities.Values)
			{
				current.IsInList = false;
				if (this.OnEntityRemoved != null)
				{
					this.OnEntityRemoved(current);
				}
			}
			this.entities.Clear();
			int address = this.Poe.Internal.IngameState.Data.LocalPlayer.address;
			if (this.Player == null || this.Player.Address != address)
			{
				this.Player = new Entity(this.Poe, address);
			}
		}
        public void Update()
        {
            int address = this.Poe.Internal.IngameState.Data.LocalPlayer.address;
            if ((this.Player == null) || (this.Player.Address != address))
            {
                this.Player = new Entity(this.Poe, address);
            }
            Dictionary<int, Poe_Entity> entitiesAsDictionary = this.Poe.Internal.IngameState.Data.EntityList.EntitiesAsDictionary;
            Dictionary<int, Entity> dictionary2 = new Dictionary<int, Entity>();
            foreach (KeyValuePair<int, Poe_Entity> pair in entitiesAsDictionary)
            {
                int key = pair.Key;
                string item = pair.Value.Path + key.ToString();
                if (!this.blackList.Contains(item))
                {
                    if (this.entities.ContainsKey(key) && this.entities[key].IsValid)
                    {
                        dictionary2.Add(key, this.entities[key]);
                        this.entities[key].IsInList = true;
                        this.entities.Remove(key);
                    }
                    else
                    {
                        if (this.entities.ContainsKey(key))
                        {
                            this.entities.Remove(key);
                        }
                        Entity entity = new Entity(this.Poe, pair.Value);
                        if ((!entity.Path.StartsWith("Metadata/Effects") && ((((long)pair.Key) & 0x80000000L) == 0L)) && !entity.Path.StartsWith("Metadata/Monsters/Daemon"))
                        {
                            if (entity.IsValid)
                            {
                                if (this.OnEntityAdded != null)
                                {
                                    this.OnEntityAdded(entity);
                                }
                                dictionary2.Add(key, entity);
                            }
                        }
                        else
                        {
                            this.blackList.Add(item);
                        }
                    }
                }
            }
            foreach (Entity entity2 in this.entities.Values)
            {
                if (this.OnEntityRemoved != null)
                {
                    this.OnEntityRemoved(entity2);
                }
                entity2.IsInList = false;
            }
            this.entities = dictionary2;
        }

		public Entity GetByID(int id)
		{
			if (!this.entities.ContainsKey(id))
			{
				return null;
			}
			return this.entities[id];
		}
	}
}
