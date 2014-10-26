using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Hud
{
	public struct MapIconDrawStyle
	{
		public readonly HudTexture Texture;
		public readonly int Size;

		public MapIconDrawStyle(HudTexture hudTexture, int iconSize)
		{
			Texture = hudTexture;
			Size = iconSize;
		}

		// might add gradient style here
	}

	public class MapIconBehaviour
	{
		public virtual bool IsEntityStillValid(EntityWrapper entity) { return entity.IsValid; }
		public virtual bool ShouldSkip(EntityWrapper entity) { return false; }

		public static readonly MapIconBehaviour Default = new MapIconBehaviour();
		public static readonly MapIconBehaviour Chest = new MapIconBehaviourChest();
		public static readonly MapIconBehaviour Creature = new MapIconBehaviourMonster();
	}

	public class MapIconBehaviourMonster : MapIconBehaviour
	{
		public override bool ShouldSkip(EntityWrapper entity)
		{
			return !entity.IsAlive;
		}
	}

	public class MapIconBehaviourChest : MapIconBehaviour
	{
		public override bool IsEntityStillValid(EntityWrapper entity)
		{
			return entity.IsValid && !entity.GetComponent<Chest>().IsOpened;
		}
	}

	// Settings.GetBool("MinimapIcons.Masters");
	// Settings.GetBool("MinimapIcons.AlertedItems");
	// Settings.GetBool("MinimapIcons.Monsters")
	// Settings.GetBool("MinimapIcons.Minions")
	// Settings.GetBool("MinimapIcons.Chests");
	// Settings.GetBool("MinimapIcons.Strongboxes");


	public class MapIcon
	{
		public readonly EntityWrapper Entity;
		public readonly MapIconDrawStyle MinimapStyle;
		public readonly MapIconDrawStyle LargeMapStyle;
		public MapIconBehaviour Behaviour = MapIconBehaviour.Default;

		public Vec2 WorldPosition { get { return this.Entity.GetComponent<Positioned>().GridPos; } }

		public MapIcon(EntityWrapper entity, HudTexture hudTexture, int iconSize) : this(entity, new MapIconDrawStyle(hudTexture, iconSize)) { }

		public MapIcon(EntityWrapper entity, MapIconDrawStyle miniStyle) : this(entity, miniStyle, miniStyle) { }
		public MapIcon(EntityWrapper entity, MapIconDrawStyle miniStyle, MapIconDrawStyle largeStyle)
		{
			Entity = entity;
			MinimapStyle = miniStyle;
			LargeMapStyle = largeStyle;
		}

		public bool IsEntityStillValid() { return Behaviour.IsEntityStillValid(Entity); }
		public bool ShouldSkip() { return Behaviour.ShouldSkip(Entity); }
	}
}
