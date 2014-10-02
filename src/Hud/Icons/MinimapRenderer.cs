using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.ExileBot;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;

namespace PoeHUD.Hud.Icons
{
	public class MinimapRenderer : HUDPlugin
	{
		private const double InvSq2 = 0.7071067811;
		private List<MinimapIcon>[] icons;
		private Vec2 playerPos;
		public override void OnEnable()
		{
			this.icons = new List<MinimapIcon>[Enum.GetValues(typeof(MinimapRenderPriority)).Length];
			for (int i = 0; i < this.icons.Length; i++)
			{
				this.icons[i] = new List<MinimapIcon>();
			}
			this.poe.EntityList.OnEntityAdded += this.EntityList_OnEntityAdded;
			foreach (Entity current in this.poe.Entities)
			{
				this.EntityList_OnEntityAdded(current);
			}
		}
		public override void OnDisable()
		{
		}
		private void EntityList_OnEntityAdded(Entity entity)
		{
			MinimapIcon icon = this.GetIcon(entity);
			if (icon != null)
			{
				this.AddIcon(icon);
			}
		}
		public override void Render(RenderingContext rc)
		{
			if (!this.poe.InGame || !Settings.GetBool("MinimapIcons"))
			{
				return;
			}
			this.playerPos = this.poe.Player.GetComponent<Positioned>().GridPos;
			// Element smallMinimap = this.poe.Internal.IngameState.IngameUi.Minimap.SmallMinimap;
			float scale = 240f;
			Rect clientRect = poe.__getApproxMinimapRect(); // this.poe.Internal.IngameState.IngameUi.Minimap.SmallMinimap.GetClientRect());
			// Rect clientRect = smallMinimap.GetClientRect();
			Vec2 minimapCenter = new Vec2(clientRect.X + clientRect.W / 2, clientRect.Y + clientRect.H / 2);
			double diag = Math.Sqrt((double)(clientRect.W * clientRect.W + clientRect.H * clientRect.H)) / 2.0;
			List<MinimapIcon>[] array = this.icons;
			for (int i = 0; i < array.Length; i++)
			{
				List<MinimapIcon> list = array[i];
				list.RemoveAll((MinimapIcon x) => !x.Validate());
				foreach (MinimapIcon current in 
					from x in list
					where x.WantsToRender()
					select x)
				{
					Vec2 point = this.WorldToMinimap(current.WorldPosition, minimapCenter, diag, scale);
					current.RenderAt(rc, point);
				}
			}
		}
		private Vec2 WorldToMinimap(Vec2 world, Vec2 minimapCenter, double diag, float scale)
		{
			float num = (float)((double)((float)(world.X - this.playerPos.X) / scale) * diag * 0.7071067811);
			float num2 = (float)((double)((float)(world.Y - this.playerPos.Y) / scale) * diag * 0.7071067811);
			int x = (int)((float)minimapCenter.X - num2 + num);
			int y = (int)((float)minimapCenter.Y - num2 - num);
			return new Vec2(x, y);
		}
		private MinimapIcon GetIcon(Entity e)
		{
			if (e.HasComponent<Poe.EntityComponents.Monster>())
			{
				if (!e.IsHostile)
				{
					return new MinionMinimapIcon(e, "monster_ally.png", 6, MinimapRenderPriority.BlueMonster);
				}
				switch (e.GetComponent<ObjectMagicProperties>().Rarity)
				{
				case MonsterRarity.White:
					return new MonsterMinimapIcon(e, "monster_enemy.png", 6, MinimapRenderPriority.Monster);
				case MonsterRarity.Magic:
					return new MonsterMinimapIcon(e, "monster_enemy_blue.png", 8, MinimapRenderPriority.BlueMonster);
				case MonsterRarity.Rare:
					return new MonsterMinimapIcon(e, "monster_enemy_yellow.png", 10, MinimapRenderPriority.RareMonster);
				case MonsterRarity.Unique:
					return new MonsterMinimapIcon(e, "monster_enemy_orange.png", 10, MinimapRenderPriority.RareMonster);
				}
			}
			if (e.HasComponent<Chest>() && !e.GetComponent<Chest>().IsOpened)
			{
				if (!e.GetComponent<Chest>().IsStrongbox)
				{
					return new ChestMinimapIcon(e, "minimap_default_icon.png", 6);
				}
				switch (e.GetComponent<ObjectMagicProperties>().Rarity)
				{
				case MonsterRarity.White:
					return new StrongboxMinimapIcon(e, "strongbox.png", 16, Color.White);
				case MonsterRarity.Magic:
					return new StrongboxMinimapIcon(e, "strongbox.png", 16, Color.LightBlue);
				case MonsterRarity.Rare:
					return new StrongboxMinimapIcon(e, "strongbox.png", 16, Color.Yellow);
				case MonsterRarity.Unique:
					return new StrongboxMinimapIcon(e, "strongbox.png", 16, Color.Orange);
				}
			}
			return null;
		}
		public void AddIcon(MinimapIcon icon)
		{
			this.icons[(int)icon.RenderPriority].Add(icon);
		}
	}
}
