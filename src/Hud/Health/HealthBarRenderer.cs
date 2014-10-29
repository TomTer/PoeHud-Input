using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using SlimDX.Direct3D9;


namespace PoeHUD.Hud.Health
{
	public class HealthBarRenderer : HUDPluginBase, EntityListObserver
	{
		private List<Healthbar>[] healthBars;
		public override void OnEnable()
		{
			this.healthBars = new List<Healthbar>[Enum.GetValues(typeof(RenderPrio)).Length];
			for (int i = 0; i < this.healthBars.Length; i++)
			{
				this.healthBars[i] = new List<Healthbar>();
			}

			foreach (EntityWrapper current in this.model.Entities)
			{
				this.EntityAdded(current);
			}
		}
		public override void OnDisable()
		{
		}

		public void EntityAdded(EntityWrapper entity)
		{
			Healthbar healthbarSettings = this.GetHealthbarSettings(entity);
			if (healthbarSettings != null)
			{
				this.healthBars[(int)healthbarSettings.prio].Add(healthbarSettings);
			}
		}

		public void EntityRemoved(EntityWrapper entity)
		{
			
		}


		public override void Render(RenderingContext rc)
		{
			if (!this.model.InGame || !Settings.GetBool("Healthbars"))
			{
				return;
			}
			if (!Settings.GetBool("Healthbars.ShowInTown") && this.model.Area.CurrentArea.IsTown)
			{
				return;
			}
			float clientWidth = (float)this.model.Window.ClientRect().W / 2560f;
			float clientHeight = (float)this.model.Window.ClientRect().H / 1600f;
			List<Healthbar>[] array = this.healthBars;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].RemoveAll((Healthbar x) => !x.entity.IsValid);
				foreach (Healthbar current in array[i].Where(x => x.entity.IsAlive && Settings.GetBool(x.settings)))
				{
					Vec3 worldCoords = current.entity.Pos;
					Vec2 mobScreenCoords = this.model.Internal.IngameState.Camera.WorldToScreen(worldCoords.Translate(0f, 0f, -170f));
					// System.Diagnostics.Debug.WriteLine("{0} is at {1} => {2} on screen", current.entity.Path, worldCoords, mobScreenCoords);
					if (mobScreenCoords != Vec2.Empty)
					{
						int scaledWidth = (int) (Settings.GetInt(current.settings + ".Width")*clientWidth);
						int scaledHeight = (int) (Settings.GetInt(current.settings + ".Height")*clientHeight);
						Color color = Settings.GetColor(current.settings + ".Color");
						Color color2 = Settings.GetColor(current.settings + ".Outline");
						Color percentsTextColor = Settings.GetColor(current.settings + ".PercentTextColor");
						float hpPercent = current.entity.GetComponent<Life>().HPPercentage;
						float esPercent = current.entity.GetComponent<Life>().ESPercentage;
						float hpWidth = hpPercent*scaledWidth;
						float esWidth = esPercent*scaledWidth;
						String hppercentAsString = ((int) (hpPercent*100)).ToString();
						Rect bg = new Rect(mobScreenCoords.X - scaledWidth/2, mobScreenCoords.Y - scaledHeight/2, scaledWidth,
							scaledHeight);
						if (current.entity.IsHostile && hpPercent <= 0.1)
						{
							color = Settings.GetColor(current.settings + ".Under10Percent");
						}
						this.DrawEntityHealthbar(color, color2, bg, hpWidth, esWidth, rc);
						if (current.entity.IsHostile && 
								(current.prio == RenderPrio.Magic ||
								current.prio == RenderPrio.Rare ||
								current.prio == RenderPrio.Unique))
						{
							this.DrawEntityHealthPercents(percentsTextColor, hppercentAsString, bg, rc);
						}
					}
				}
			}
		}
        private void DrawEntityHealthbar(Color color, Color outline, Rect bg, float hpWidth, float esWidth, RenderingContext rc)
		{
			if (outline.ToArgb() != 0)
			{
				Rect rect = new Rect(bg.X - 1, bg.Y - 1, bg.W + 2, bg.H + 2);
				rc.AddBox(rect, outline);
			}
			rc.AddTexture(Settings.GetBool("Healthbars.ShowIncrements") ? "healthbar_increment.png" : "healthbar.png", bg, color);
			
			if ((int)hpWidth < bg.W)
			{
				Rect rect2 = new Rect(bg.X + (int)hpWidth, bg.Y, bg.W - (int)hpWidth, bg.H);
				if( rect2.W > 0 )
					rc.AddTexture("healthbar_bg.png", rect2, color);
			}
			if (Settings.GetBool("Healthbars.ShowES"))
			{
				bg.W = (int)esWidth;
				rc.AddTexture("esbar.png", bg, Color.White);
			}
		}

		private void DrawEntityHealthPercents(Color hppercentsTextColor, String hppercentsText, Rect bg, RenderingContext rc)
		{
			// Draw percents 
			rc.AddTextWithHeight(new Vec2(bg.X + bg.W + 2, bg.Y), hppercentsText, hppercentsTextColor, 9, DrawTextFormat.Left);
		}

		private Healthbar GetHealthbarSettings(EntityWrapper e)
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
