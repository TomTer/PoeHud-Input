using System;
using System.Collections.Generic;
using System.Linq;
using PoeHUD.Framework;
using PoeHUD.Poe;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.Files;
using PoeHUD.Poe.UI;

namespace PoeHUD.Controllers
{
	public class GameController
	{
		public GameWindow Window;
		public TheGame Internal;
		private readonly EntityListWrapper _entityListWrapper;
		public AreaController Area;

		public Memory Memory { get; private set; }

		public ICollection<EntityWrapper> Entities { get { return this._entityListWrapper.Entities; } }
		public EntityWrapper Player { get { return this._entityListWrapper.Player; } }

		public bool InGame { get { return this.Internal.IngameState.InGame; } }

		public FsController Files { get; private set; }

		public EntityListObserver EntityListObserver { get { return _entityListWrapper.Observer; } set { _entityListWrapper.Observer = value; } }

		public GameController(Memory memory)
		{
			this.Memory = memory;
			this.Area = new AreaController(this);
			this._entityListWrapper = new EntityListWrapper(this);
			this.Window = new GameWindow(memory.Process);
			this.Internal = new TheGame(memory);
			this.Files = new FsController(memory);
		}
		public void RefreshState()
		{
			if (this.InGame)
			{
				_entityListWrapper.RefreshState();
				Area.RefreshState();
			}
		}
		public List<EntityWrapper> GetAllPlayerMinions()
		{
			return this.Entities.Where(x => x.HasComponent<Player>()).SelectMany(c => c.Minions).ToList();
		}

		public EntityLabel GetLabelForEntity(EntityWrapper entity)
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
			return this.Internal.ReadObject<EntityLabel>(num + 12);
		}

		internal EntityWrapper GetEntityByID(int id)
		{
			return _entityListWrapper.GetByID(id);
		}
	}
}
