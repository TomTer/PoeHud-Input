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


		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
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
						Rect bg = new Rect(mobScreenCoords.X - scaledWidth/2, mobScreenCoords.Y - scaledHeight/2, scaledWidth,
							scaledHeight);
						if (current.entity.IsHostile && hpPercent <= 0.1) // Set healthbar color to configured in settings.txt for hostiles when hp is <=10%
						{
							color = Settings.GetColor(current.settings + ".Under10Percent");
						}
						// Draw healthbar
						this.DrawEntityHealthbar(color, color2, bg, hpWidth, esWidth, rc);

						// Draw percents or health text for hostiles. Configurable in settings.txt
						if (current.entity.IsHostile)
						{
							int curHp = current.entity.GetComponent<Life>().CurHP;
							int maxHp = current.entity.GetComponent<Life>().MaxHP;
							string monsterHpCorrectString = this.GetCorrectMonsterHealthString(curHp, maxHp);
							string hppercentAsString = ((int)(hpPercent * 100)).ToString();
							Color monsterHpTextColor = (hpPercent <= 0.1) ?
								Settings.GetColor(current.settings + ".HealthTextColorUnder10Percent") :
								Settings.GetColor(current.settings + ".HealthTextColor");

							if (Settings.GetBool(current.settings + ".PrintPercents")) 
								this.DrawEntityHealthPercents(percentsTextColor, hppercentAsString, bg, rc);
							if (Settings.GetBool(current.settings + ".PrintHealthText")) 
								this.DrawEntityHealthbarText(monsterHpTextColor, monsterHpCorrectString, bg, rc);
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

		private void DrawEntityHealthbarText(Color textColor, String healthString, Rect bg, RenderingContext rc)
		{
			// Draw monster health ex. "163 / 12k" 
			rc.AddTextWithHeight(new Vec2(bg.X + bg.W / 2, bg.Y), healthString, textColor, 9, DrawTextFormat.Center);
		}

		private void DrawEntityHealthPercents(Color hppercentsTextColor, String hppercentsText, Rect bg, RenderingContext rc)
		{
			// Draw percents 
			rc.AddTextWithHeight(new Vec2(bg.X + bg.W + 4, bg.Y), hppercentsText, hppercentsTextColor, 9, DrawTextFormat.Left);
		}

		private string GetCorrectMonsterHealthString(int currentHp, int maxHp)
		{
			string currentHpString = null;
			string maxHpString = null;

			if (currentHp > 1000)
			{
				if (currentHp < 1000000) currentHpString = (currentHp/1000).ToString() + "k";
				else currentHpString = (currentHp/1000000).ToString() + "kk";
			}
			else currentHpString = currentHp.ToString();

			if (maxHp > 1000)
			{
				if (maxHp < 1000000) maxHpString = (maxHp/1000).ToString() + "k";
				else maxHpString = (maxHp/1000000).ToString() + "kk";
			}
			else maxHpString = maxHp.ToString();

			return String.Format("{0} / {1}", currentHpString, maxHpString);
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
