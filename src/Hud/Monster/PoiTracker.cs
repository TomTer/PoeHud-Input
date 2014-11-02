using System.Collections.Generic;
using System.IO;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Hud.Monster
{
	public class PoiTracker : HUDPluginBase, EntityListObserver, HUDPluginWithMapIcons
	{
		private readonly Dictionary<EntityWrapper, MapIcon> currentIcons = new Dictionary<EntityWrapper, MapIcon>();

		public override void OnEnable()
		{
			this.model.Area.OnAreaChange += this.CurrentArea_OnAreaChange;

			currentIcons.Clear();
			foreach (EntityWrapper current in this.model.Entities)
			{
				this.EntityAdded(current);
			}
		}
		public override void OnDisable()
		{
		}
		public void EntityRemoved(EntityWrapper entity)
		{
			currentIcons.Remove(entity);
		}

		public void EntityAdded(EntityWrapper entity)
		{
			if (!Settings.GetBool("MonsterTracker"))
			{
				return;
			}
			var icon = GetMapIcon(entity);
			if ( null != icon )
				currentIcons[entity] = icon;

		}
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			currentIcons.Clear();
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.GetBool("MonsterTracker.ShowText"))
			{
				return;
			}
		}

		public IEnumerable<MapIcon> GetIcons()
		{
			List<EntityWrapper> toRemove = new List<EntityWrapper>();
			foreach (KeyValuePair<EntityWrapper, MapIcon> kv in currentIcons)
			{
				if (kv.Value.IsEntityStillValid())
					yield return kv.Value;
				else
					toRemove.Add(kv.Key);
			}
			foreach (EntityWrapper wrapper in toRemove)
			{
				currentIcons.Remove(wrapper);
			}
		}


		private static readonly List<string> masters = new List<string> {
			"Metadata/NPC/Missions/Wild/Dex",
			"Metadata/NPC/Missions/Wild/DexInt",
			"Metadata/NPC/Missions/Wild/Int",
			"Metadata/NPC/Missions/Wild/Str",
			"Metadata/NPC/Missions/Wild/StrDex",
			"Metadata/NPC/Missions/Wild/StrDexInt",
			"Metadata/NPC/Missions/Wild/StrInt"
		};

		private MapIcon GetMapIcon(EntityWrapper e)
		{
			if (e.HasComponent<NPC>() && masters.Contains(e.Path))
			{
				return new MapIconCreature(e, new HudTexture("monster_ally.png"), 10);
			}
			if (e.HasComponent<Chest>() && !e.GetComponent<Chest>().IsOpened)
			{
				return e.GetComponent<Chest>().IsStrongbox
					? new MapIconChest(e, new HudTexture("strongbox.png", e.GetComponent<ObjectMagicProperties>().Rarity), 16)
					: new MapIconChest(e, new HudTexture("minimap_default_icon.png"), 6);
			}
			return null;

		}

		private static Dictionary<string, string> LoadMonsterModAlerts()
		{
			var result = new Dictionary<string, string>();

			string[] lines = File.ReadAllLines("config/monster_mod_alerts.txt");
			foreach (string line in lines.Select(a => a.Trim()))
			{
				if (string.IsNullOrWhiteSpace(line) || line.IndexOf(',') < 0)
					continue;

				var parts = line.Split(new[] {','}, 2);
				result[parts[0].Trim()] = parts[1].Trim();
			}

			return result;
		}

		private static Dictionary<string, string> LoadMonsterNameAlerts()
		{
			var result = new Dictionary<string, string>();

			string[] lines = File.ReadAllLines("config/monster_name_alerts.txt");
			foreach (string line in lines.Select(a => a.Trim()))
			{
				if (string.IsNullOrWhiteSpace(line) || line.IndexOf(',') < 0)
					continue;

				var parts = line.Split(new[] { ',' }, 2);
				result[parts[0].Trim()] = parts[1].Trim();
			}

			return result;
		}
	}
}
