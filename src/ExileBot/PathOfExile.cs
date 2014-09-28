using System.Collections.Generic;
using System.Linq;
using ExileHUD.EntityComponents;
using ExileHUD.Framework;

namespace ExileHUD.ExileBot
{
	public class PathOfExile
	{
		public GameWindow Window;
		public Poe_Game Internal;
		public EntityList EntityList;
		public AreaController Area;
		public event UpdateEvent OnUpdate;
		public Memory Memory
		{
			get;
			private set;
		}
		public ICollection<Entity> Entities
		{
			get
			{
				return this.EntityList.Entities;
			}
		}
		public Entity Player
		{
			get
			{
				return this.EntityList.Player;
			}
		}
		public bool InGame
		{
			get
			{
				return this.Internal.IngameState.InGame;
			}
		}
		public FileIndex Files
		{
			get;
			private set;
		}
		public PathOfExile(Memory memory)
		{
			this.Memory = memory;
			this.Area = new AreaController(this);
			this.EntityList = new EntityList(this);
			this.Window = new GameWindow(memory.Process);
			this.Internal = new Poe_Game(memory);
			this.Files = new FileIndex(memory);
		}
		public void Update()
		{
			if (this.InGame && this.OnUpdate != null)
			{
				this.OnUpdate();
			}
		}
		public List<Entity> GetAllPlayerMinions()
		{
			List<Entity> list = new List<Entity>();
			foreach (Entity current in 
				from x in this.Entities
				where x.HasComponent<Player>()
				select x)
			{
				list.AddRange(current.Minions);
			}
			return list;
		}
		public Poe_UI_EntityLabel GetLabelFromEntity(Entity entity)
		{
			HashSet<int> hashSet = new HashSet<int>();
			int entityLabelMap = this.Internal.game.IngameState.EntityLabelMap;
			int num = entityLabelMap;
			while (true)
			{
				hashSet.Add(num);
				if (this.Memory.ReadInt(num + 8) == entity.Address)
				{
					break;
				}
				num = this.Memory.ReadInt(num);
				if (hashSet.Contains(num) || num == 0 || num == -1)
				{
					return null;
				}
			}
			return this.Internal.ReadObject<Poe_UI_EntityLabel>(num + 12);
		}
	}
}
