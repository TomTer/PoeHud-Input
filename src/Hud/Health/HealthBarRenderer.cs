using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.ExileBot;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Hud.Health
{
	public class HealthBarRenderer : HUDPlugin
	{

		private List<Healthbar>[] healthBars;
		public override void OnEnable()
		{
			this.healthBars = new List<Healthbar>[Enum.GetValues(typeof(RenderPrio)).Length];
			for (int i = 0; i < this.healthBars.Length; i++)
			{
				this.healthBars[i] = new List<Healthbar>();
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
			Healthbar healthbarSettings = this.GetHealthbarSettings(entity);
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
			if (!Settings.GetBool("Healthbars.ShowInTown") && this.poe.Area.CurrentArea.IsTown)
			{
				return;
			}
			float clientWidth = (float)this.poe.Window.ClientRect().W / 2560f;
			float clientHeight = (float)this.poe.Window.ClientRect().H / 1600f;
			List<Healthbar>[] array = this.healthBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].RemoveAll((Healthbar x) => !x.entity.IsValid);
				foreach (Healthbar current in array[i].Where(x => x.entity.IsAlive && Settings.GetBool(x.settings)))
				{
					Vec3 worldCoords = current.entity.Pos;
					Vec2 mobScreenCoords = this.poe.Internal.IngameState.Camera.WorldToScreen(worldCoords.Translate(0f, 0f, -170f));
					// System.Diagnostics.Debug.WriteLine("{0} is at {1} => {2} on screen", current.entity.Path, worldCoords, mobScreenCoords);
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
			rc.AddTexture(Settings.GetBool("Healthbars.ShowIncrements") ? "healthbar_increment.png" : "healthbar.png", bg, color);
			
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
		private Healthbar GetHealthbarSettings(Entity e)
		{
			if (e.HasComponent<Player>())
			{
				return new Healthbar(e, "Healthbars.Players", RenderPrio.Player);
			}
			if (e.HasComponent<Poe.EntityComponents.Monster>())
			{
				if (e.IsHostile)
				{
					switch (e.GetComponent<ObjectMagicProperties>().Rarity)
					{
					case MonsterRarity.White:
						return new Healthbar(e, "Healthbars.Enemies.Normal", RenderPrio.Normal);
					case MonsterRarity.Magic:
						return new Healthbar(e, "Healthbars.Enemies.Magic", RenderPrio.Magic);
					case MonsterRarity.Rare:
						return new Healthbar(e, "Healthbars.Enemies.Rare", RenderPrio.Rare);
					case MonsterRarity.Unique:
						return new Healthbar(e, "Healthbars.Enemies.Unique", RenderPrio.Unique);
					}
				}
				else
				{
					if (!e.IsHostile)
					{
						return new Healthbar(e, "Healthbars.Minions", RenderPrio.Minion);
					}
				}
			}
			return null;
		}
	}
}
