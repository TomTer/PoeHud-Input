using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ExileHUD.EntityComponents;
using ExileHUD.ExileBot;
using ExileHUD.Framework;
using ExileHUD.Game;

namespace ExileHUD.ExileHUD
{
	public class HealthBarRenderer : HUDPlugin
	{
		private class Healthbar
		{
			public Entity entity;
			public string settings;
			public HealthBarRenderer.RenderPrio prio;
			public Healthbar(Entity entity, string settings, HealthBarRenderer.RenderPrio prio)
			{
				this.entity = entity;
				this.settings = settings;
				this.prio = prio;
			}
		}
		private enum RenderPrio
		{
			Normal,
			Minion,
			Magic,
			Rare,
			Unique,
			Player
		}
		private List<HealthBarRenderer.Healthbar>[] healthBars;
		public override void OnEnable()
		{
			this.healthBars = new List<HealthBarRenderer.Healthbar>[Enum.GetValues(typeof(HealthBarRenderer.RenderPrio)).Length];
			for (int i = 0; i < this.healthBars.Length; i++)
			{
				this.healthBars[i] = new List<HealthBarRenderer.Healthbar>();
			}
			this.poe.EntityList.OnEntityAdded += new EntityEvent(this.EntityList_OnEntityAdded);
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
			HealthBarRenderer.Healthbar healthbarSettings = this.GetHealthbarSettings(entity);
			if (healthbarSettings != null)
			{
				this.healthBars[(int)healthbarSettings.prio].Add(healthbarSettings);
			}
		}
		public override void Render(RenderingContext rc)
		{
			if (!this.poe.InGame || !Settings.GetBool("Healthbars"))
			{
				return;
			}
			if (!Settings.GetBool("Healthbars.ShowInTown") && this.poe.CurrentArea.IsTown)
			{
				return;
			}
			float clientWidth = (float)this.poe.Window.ClientRect().W / 2560f;
			float clientHeight = (float)this.poe.Window.ClientRect().H / 1600f;
			List<HealthBarRenderer.Healthbar>[] array = this.healthBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].RemoveAll((HealthBarRenderer.Healthbar x) => !x.entity.IsValid);
				foreach (HealthBarRenderer.Healthbar current in array[i].Where(x => x.entity.IsAlive && Settings.GetBool(x.settings)))
				{
					Vec3 worldCoords = current.entity.Pos;
					Vec2 mobScreenCoords = this.poe.Internal.IngameState.Camera.WorldToScreen(worldCoords.Translate(0f, 0f, -170f));
					System.Diagnostics.Debug.WriteLine("{0} is at {1} => {2} on screen", current.entity.Path, worldCoords, mobScreenCoords);
					if (mobScreenCoords != Vec2.Empty)
					{
						int scaledWidth = (int)(Settings.GetInt(current.settings + ".Width") * clientWidth);
						int scaledHeight = (int)(Settings.GetInt(current.settings + ".Height") * clientHeight);
						Color color = Settings.GetColor(current.settings + ".Color");
						Color color2 = Settings.GetColor(current.settings + ".Outline");
						float hpPercent = current.entity.GetComponent<Life>().HPPercentage;
						float esPercent = current.entity.GetComponent<Life>().ESPercentage;
						float hpWidth = hpPercent * scaledWidth;
						float esWidth = esPercent * scaledWidth;
						Rect bg = new Rect(mobScreenCoords.X - scaledWidth / 2, mobScreenCoords.Y - scaledHeight / 2, scaledWidth, scaledHeight);
						this.DrawEntityHealthbar(color, color2, bg, hpWidth, esWidth, rc);
					}
				}
			}
		}
		private void DrawEntityHealthbar(Color color, Color outline, Rect bg, float hpWidth, float esWidth, RenderingContext rc)
		{
			if (outline.ToArgb() != 0)
			{
				Rect rect = new Rect(bg.X - 2, bg.Y - 2, bg.W + 4, bg.H + 4);
				rc.AddBox(rect, outline);
			}
			if (Settings.GetBool("Healthbars.ShowIncrements"))
			{
				rc.AddTexture("healthbar_increment.png", bg, color);
			}
			else
			{
				rc.AddTexture("healthbar.png", bg, color);
			}
			if ((int)hpWidth < bg.W)
			{
				Rect rect2 = new Rect(bg.X + (int)hpWidth, bg.Y, bg.W - (int)hpWidth, bg.H);
				rc.AddTexture("healthbar_bg.png", rect2, color);
			}
			if (Settings.GetBool("Healthbars.ShowES"))
			{
				bg.W = (int)esWidth;
				rc.AddTexture("esbar.png", bg, Color.White);
			}
		}
		private HealthBarRenderer.Healthbar GetHealthbarSettings(Entity e)
		{
			if (e.HasComponent<Player>())
			{
				return new HealthBarRenderer.Healthbar(e, "Healthbars.Players", HealthBarRenderer.RenderPrio.Player);
			}
			if (e.HasComponent<Monster>())
			{
				if (e.IsHostile)
				{
					switch (e.GetComponent<ObjectMagicProperties>().Rarity)
					{
					case MonsterRarity.White:
						return new HealthBarRenderer.Healthbar(e, "Healthbars.Enemies.Normal", HealthBarRenderer.RenderPrio.Normal);
					case MonsterRarity.Magic:
						return new HealthBarRenderer.Healthbar(e, "Healthbars.Enemies.Magic", HealthBarRenderer.RenderPrio.Magic);
					case MonsterRarity.Rare:
						return new HealthBarRenderer.Healthbar(e, "Healthbars.Enemies.Rare", HealthBarRenderer.RenderPrio.Rare);
					case MonsterRarity.Unique:
						return new HealthBarRenderer.Healthbar(e, "Healthbars.Enemies.Unique", HealthBarRenderer.RenderPrio.Unique);
					}
				}
				else
				{
					if (!e.IsHostile)
					{
						return new HealthBarRenderer.Healthbar(e, "Healthbars.Minions", HealthBarRenderer.RenderPrio.Minion);
					}
				}
			}
			return null;
		}
	}
}
