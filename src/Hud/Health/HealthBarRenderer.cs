using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Settings;
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

		public class MobHealthSettings : SettingsForModule
		{

			public MobHealthSettings() : base("Health Bars")
			{
				Players.Color.Default = Color.FromArgb(255, 0, 128, 0);
				Minions.Color.Default = Color.FromArgb(255, 144, 238, 144);
				Players.Outline.Default = Color.FromArgb(0);
				Minions.Outline.Default = Color.FromArgb(0);
			}

			public EnemiesSettigns Enemies = new EnemiesSettigns("Enemies");
			public PerGroupSetting Minions = new PerGroupSetting("Minions");
			public PerGroupSetting Players = new PerGroupSetting("Players");
			public Setting<bool> ShowInTown = new Setting<bool>("Show In Town", true);
			public Setting<bool> ShowIncrements = new Setting<bool>("Show Increments", true);
			public Setting<bool> ShowES = new Setting<bool>("Show ES", true);

		}

		public class EnemiesSettigns : SettingsBlock
		{
			public PerGroupSetting Normal = new PerGroupSetting("Normal");
			public PerGroupSetting Magic = new PerGroupSetting("Magic");
			public PerGroupSetting Rare = new PerGroupSetting("Rare");
			public PerGroupSetting Unique = new PerGroupSetting("Unique");

			public EnemiesSettigns(string name) : base(name)
			{
				Normal.Outline.Default = Color.FromArgb(0);
				Magic.Outline.Default = HudSkin.MagicColor;
				Rare.Outline.Default = HudSkin.RareColor;
				Unique.Outline.Default = HudSkin.UniqueColor;

				Normal.Color.Default = Color.FromArgb(255, 255, 0, 0);
				Magic.Color.Default = Color.FromArgb(255, 255, 0, 0);
				Rare.Color.Default = Color.FromArgb(255, 255, 0, 0);
				Unique.Color.Default = Color.FromArgb(255, 255, 0, 0);
			}
		}

		public class PerGroupSetting : SettingsForModule
		{
			public Setting<Color> Color = new Setting<Color>("Color", System.Drawing.Color.Fuchsia);
			public Setting<Color> Outline = new Setting<Color>("Outline", System.Drawing.Color.Cyan);
			public Setting<Color> PercentTextColor = new Setting<Color>("Color for Percents Text", System.Drawing.Color.White);
			public SettingIntRange Height = new SettingIntRange("Height", 10, 50, 25);
			public SettingIntRange Width = new SettingIntRange("Width", 50, 180, 105);
			public Setting<Color> Under10Percent = new Setting<Color>("Color when under 10%", System.Drawing.Color.White);
			public Setting<bool> PrintPercents = new Setting<bool>("Show Percents", true);
			public Setting<bool> PrintHealthText = new Setting<bool>("Show Health Value", true);
			public Setting<Color> HealthTextColorUnder10Percent = new Setting<Color>("Text Color when under 10%", System.Drawing.Color.Red);
			public Setting<Color> HealthTextColor = new Setting<Color>("Health Text Color", System.Drawing.Color.White);
			public PerGroupSetting(string name) : base(name) { }
		}

		public MobHealthSettings Settings = new MobHealthSettings();

		public override SettingsForModule SettingsNode
		{
			get { return Settings; }
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
			if (!this.model.InGame) {
				return;
			}
			if (!Settings.ShowInTown && this.model.Area.CurrentArea.IsTown)
			{
				return;
			}
			float clientWidth = (float)this.model.Window.ClientRect().W / 2560f;
			float clientHeight = (float)this.model.Window.ClientRect().H / 1600f;
			foreach (List<Healthbar> bars in this.healthBars)
			{
				bars.RemoveAll(x => !x.entity.IsValid);
				foreach (Healthbar current in bars.Where(x => x.entity.IsAlive && x.settings.Enabled))
				{
					RenderHealthBar(rc, current, clientWidth, clientHeight);
				}
			}
		}

		private void RenderHealthBar(RenderingContext rc, Healthbar current, float clientWidth, float clientHeight)
		{
			Vec3 worldCoords = current.entity.Pos;
			Vec2 mobScreenCoords = this.model.Internal.IngameState.Camera.WorldToScreen(worldCoords.Translate(0f, 0f, -170f));
			// System.Diagnostics.Debug.WriteLine("{0} is at {1} => {2} on screen", current.entity.Path, worldCoords, mobScreenCoords);
			if (mobScreenCoords == Vec2.Empty) return;

			int scaledWidth = (int) (current.settings.Width*clientWidth);
			int scaledHeight = (int) (current.settings.Height*clientHeight);
			Color color = current.settings.Color;
			Color color2 = current.settings.Outline;
			Color percentsTextColor = current.settings.PercentTextColor;
			float hpPercent = current.entity.GetComponent<Life>().HPPercentage;
			float esPercent = current.entity.GetComponent<Life>().ESPercentage;
			float hpWidth = hpPercent*scaledWidth;
			float esWidth = esPercent*scaledWidth;
			Rect bg = new Rect(mobScreenCoords.X - scaledWidth/2, mobScreenCoords.Y - scaledHeight/2, scaledWidth,
				scaledHeight);
			if (current.entity.IsHostile && hpPercent <= 0.1)
				// Set healthbar color to configured in settings.txt for hostiles when hp is <=10%
			{
				color = current.settings.Under10Percent;
			}
			// Draw healthbar
			this.DrawEntityHealthbar(color, color2, bg, hpWidth, esWidth, rc);

			// Draw percents or health text for hostiles. Configurable in settings.txt
			if (current.entity.IsHostile)
			{
				int curHp = current.entity.GetComponent<Life>().CurHP;
				int maxHp = current.entity.GetComponent<Life>().MaxHP;
				string monsterHpCorrectString = this.GetCorrectMonsterHealthString(curHp, maxHp);
				string hppercentAsString = ((int) (hpPercent*100)).ToString();
				Color monsterHpTextColor = (hpPercent <= 0.1)
					? current.settings.HealthTextColorUnder10Percent
					: current.settings.HealthTextColor;

				if (current.settings.PrintPercents)
					this.DrawEntityHealthPercents(percentsTextColor, hppercentAsString, bg, rc);
				if (current.settings.PrintHealthText)
					this.DrawEntityHealthbarText(monsterHpTextColor, monsterHpCorrectString, bg, rc);
			}
		}

		private void DrawEntityHealthbar(Color color, Color outline, Rect bg, float hpWidth, float esWidth, RenderingContext rc)
		{
			if (outline.ToArgb() != 0)
			{
				Rect rect = new Rect(bg.X - 1, bg.Y - 1, bg.W + 2, bg.H + 2);
				rc.AddBox(rect, outline);
			}
			rc.AddTexture(Settings.ShowIncrements ? "healthbar_increment.png" : "healthbar.png", bg, color);
			
			if ((int)hpWidth < bg.W)
			{
				Rect rect2 = new Rect(bg.X + (int)hpWidth, bg.Y, bg.W - (int)hpWidth, bg.H);
				if( rect2.W > 0 )
					rc.AddTexture("healthbar_bg.png", rect2, color);
			}
			if (Settings.ShowES)
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
				return new Healthbar(e, Settings.Players, RenderPrio.Player);
			}
			if (e.HasComponent<Poe.EntityComponents.Monster>())
			{
				if (e.IsHostile)
				{
					switch (e.GetComponent<ObjectMagicProperties>().Rarity)
					{
					case Rarity.White:
						return new Healthbar(e, Settings.Enemies.Normal, RenderPrio.Normal);
					case Rarity.Magic:
						return new Healthbar(e, Settings.Enemies.Magic, RenderPrio.Magic);
					case Rarity.Rare:
						return new Healthbar(e, Settings.Enemies.Rare, RenderPrio.Rare);
					case Rarity.Unique:
						return new Healthbar(e, Settings.Enemies.Unique, RenderPrio.Unique);
					}
				}
				else
				{
					if (!e.IsHostile)
					{
						return new Healthbar(e, Settings.Minions, RenderPrio.Minion);
					}
				}
			}
			return null;
		}
	}
}
