using System;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Hud
{
	public class MapIconCreature : MapIcon
	{
		public MapIconCreature(EntityWrapper entity) : base(entity) { }
		public MapIconCreature(EntityWrapper entity, HudTexture hudTexture, int iconSize) : base(entity, hudTexture, iconSize) { }

		public override bool ShouldSkip() { return !Entity.IsAlive; }
	}

	public class MapIconChest : MapIcon
	{
		public MapIconChest(EntityWrapper entity) : base(entity) { }
		public MapIconChest(EntityWrapper entity, HudTexture hudTexture, int iconSize) : base(entity, hudTexture, iconSize) { }

		public override bool IsEntityStillValid()
		{
			return Entity.IsValid && !Entity.GetComponent<Chest>().IsOpened;
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
		public HudTexture MinimapIcon;
		public HudTexture LargeMapIcon;
		public int Size;
		public int? SizeOfLargeIcon;

		public Vec2 WorldPosition { get { return Entity.GetComponent<Positioned>().GridPos; } }

		public MapIcon(EntityWrapper entity) {
			Entity = entity;
		}

		public MapIcon(EntityWrapper entity, HudTexture hudTexture, int iconSize = 10) : this(entity)
		{
			MinimapIcon = hudTexture;
			Size = iconSize;
		}

		public static Vec2 deltaInWorldToMinimapDelta(Vec2 delta, double diag, float scale, int deltaZ = 0)
		{
			const float CameraAngle = 38;

			// Values according to 40 degree rotation of cartesian coordiantes, still doesn't seem right but closer
			float cosX = (float)(delta.X / scale * diag * Math.Cos(Math.PI / 180 * CameraAngle));
			float cosY = (float)(delta.Y / scale * diag * Math.Cos(Math.PI / 180 * CameraAngle));
			float sinX = (float)(delta.X / scale * diag * Math.Sin(Math.PI / 180 * CameraAngle));
			float sinY = (float)(delta.Y / scale * diag * Math.Sin(Math.PI / 180 * CameraAngle));
			// 2D rotation formulas not correct, but it's what appears to work?
			return new Vec2((int)(cosX - cosY), -(int)((sinX + sinY)) + deltaZ);
		}

		public virtual bool IsEntityStillValid() { return Entity.IsValid; }
		public virtual bool ShouldSkip() { return false; }
	}
}
