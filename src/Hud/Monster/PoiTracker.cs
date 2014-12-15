using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Settings;

namespace PoeHUD.Hud.Monster
{
	public class PoiTracker : HUDPluginBase, EntityListObserver, HUDPluginWithMapIcons
	{
		private readonly Dictionary<EntityWrapper, MapIcon> currentIcons = new Dictionary<EntityWrapper, MapIcon>();

		public override SettingsForModule SettingsNode { get { return null; } }
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints) {
			// nothing to render, we only supply icons for maps
		}

		public override void OnEnable()
		{
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
			var icon = GetMapIcon(entity);
			if ( null != icon )
				currentIcons[entity] = icon;

		}
		public override void OnAreaChange(AreaController area)
		{
			currentIcons.Clear();
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
			"Metadata/NPC/Missions/Wild/StrInt",
			"Metadata/NPC/Missions/Wild/Fish"
		};

		private MapIcon GetMapIcon(EntityWrapper e)
		{
			if (e.HasComponent<NPC>() && masters.Contains(e.Path))
			{
				return new MapIconCreature(e, new HudTexture("monster_ally.png"), 10) { Type = MapIcon.IconType.Master };
			}
			if (e.HasComponent<Chest>() && !e.GetComponent<Chest>().IsOpened)
			{
				return e.GetComponent<Chest>().IsStrongbox 
					? new MapIconChest(e, new HudTexture("strongbox.png", e.GetComponent<ObjectMagicProperties>().Rarity), 16) { Type = MapIcon.IconType.Strongbox } 
					: new MapIconChest(e, new HudTexture("minimap_default_icon.png"), 6) { Type = MapIcon.IconType.Chest };
			}
			return null;
		}
	}
}
