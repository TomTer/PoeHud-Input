using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Settings;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Monster
{
	public class MonsterTracker : HUDPluginBase, EntityListObserver, HUDPluginWithMapIcons
	{
		public class MonstersSettings : SettingsForModule
		{
			public MonstersSettings() : base("Monster Alerts") { }

			public readonly Setting<bool> PlaySound = new Setting<bool>("Play Sound", true);
			public readonly Setting<bool> ShowText = new Setting<bool>("Show Text", true);
			public readonly SettingIntRange TextFontSize = new SettingIntRange("Font Size", 7, 30, 16);
			public readonly SettingIntRange TextBgAlpha = new SettingIntRange("Bg Alpha",0, 255, 128);
		}


		public readonly MonstersSettings Settings = new MonstersSettings();
		private HashSet<int> alreadyAlertedOf;
		private Dictionary<EntityWrapper, string> alertTexts;
		private readonly Dictionary<EntityWrapper, MapIcon> currentIcons = new Dictionary<EntityWrapper, MapIcon>();


		private Dictionary<string, string> ModsToAlertOf;
		private Dictionary<string, string> NamesToAlertOf;

		public override void OnEnable()
		{
			alreadyAlertedOf = new HashSet<int>();
			alertTexts = new Dictionary<EntityWrapper, string>();
			InitAlertStrings();

			currentIcons.Clear();
			foreach (EntityWrapper current in model.Entities)
			{
				EntityAdded(current);
			}
		}

		public override SettingsForModule SettingsNode { get { return Settings; } }

		public void EntityRemoved(EntityWrapper entity)
		{
			alertTexts.Remove(entity);
			currentIcons.Remove(entity);
		}

		public void EntityAdded(EntityWrapper entity)
		{
			if ( !Settings.Enabled || alertTexts.ContainsKey(entity))
			{
				return;
			}
			if (entity.IsAlive && entity.HasComponent<Poe.EntityComponents.Monster>())
			{
				currentIcons[entity] = GetMapIconForMonster(entity);
				string text = entity.Path;
				if (text.Contains('@'))
				{
					text = text.Split('@')[0];
				}
				if (NamesToAlertOf.ContainsKey(text))
				{
					alertTexts.Add(entity, NamesToAlertOf[text]);
					PlaySound(entity);
					return;
				}
				foreach (string current in entity.GetComponent<ObjectMagicProperties>().Mods)
				{
					if (ModsToAlertOf.ContainsKey(current))
					{
						alertTexts.Add(entity, ModsToAlertOf[current]);
						PlaySound(entity);
						break;
					}
				}
			}
		}
		private void PlaySound(EntityWrapper entity)
		{
			if (!Settings.PlaySound)
			{
				return;
			}
			if (!alreadyAlertedOf.Contains(entity.Id))
			{
				Sounds.DangerSound.Play();
				alreadyAlertedOf.Add(entity.Id);
			}
		}
		public override void OnAreaChange(AreaController area)
		{
			alreadyAlertedOf.Clear();
			alertTexts.Clear();
			currentIcons.Clear();
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.ShowText)
			{
				return;
			}
			Rect rect = model.Window.ClientRect();
			int xScreenCenter = rect.W / 2 + rect.X;
			int yPos = rect.H / 5 + rect.Y;

			var playerPos = model.Player.GetComponent<Positioned>().GridPos;
			int fontSize = Settings.TextFontSize;
			bool first = true;
			Rect rectBackground = new Rect();
			foreach (var alert in alertTexts)
			{
				if( !alert.Key.IsAlive )
					continue;

				Vec2 delta = alert.Key.GetComponent<Positioned>().GridPos - playerPos;
				double phi;
				var distance = delta.GetPolarCoordinates(out phi);
				RectUV uv = GetDirectionsUv(phi, distance);

				Vec2 textSize = rc.AddTextWithHeight(new Vec2(xScreenCenter, yPos), alert.Value, Color.Red, fontSize, DrawTextFormat.Center);

				rectBackground = new Rect(xScreenCenter - textSize.X / 2 - 6, yPos, textSize.X + 12, textSize.Y);
				rectBackground.X -= textSize.Y + 3;
				rectBackground.W += textSize.Y;

				Rect rectDirection = new Rect(rectBackground.X + 3, rectBackground.Y, rectBackground.H, rectBackground.H);

				if (first) // vertical padding above
				{
					rectBackground.Y -= 5;
					rectBackground.H += 5;
					first = false;
				}
				rc.AddBox(rectBackground, Color.FromArgb(Settings.TextBgAlpha, 1, 1, 1));
				rc.AddSprite("directions.png", rectDirection, uv, Color.Red);
				yPos += textSize.Y;
			}
			if (!first)  // vertical padding below
			{
				rectBackground.Y = rectBackground.Y + rectBackground.H;
				rectBackground.H = 5;
				rc.AddBox(rectBackground, Color.FromArgb(Settings.TextBgAlpha, 1, 1, 1));
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


		private void InitAlertStrings()
		{
			ModsToAlertOf = FsUtils.LoadKeyValueCommaSeparatedFromFile("config/monster_mod_alerts.txt");
			NamesToAlertOf = FsUtils.LoadKeyValueCommaSeparatedFromFile("config/monster_name_alerts.txt");
		}

		private MapIcon GetMapIconForMonster(EntityWrapper e)
		{
			Rarity rarity = e.GetComponent<ObjectMagicProperties>().Rarity;
			if (!e.IsHostile)
				return new MapIconCreature(e, new HudTexture("monster_ally.png"), 6) { Rarity = rarity, Type = MapIcon.IconType.Minion };

			switch (rarity)
			{
				case Rarity.White: return new MapIconCreature(e, new HudTexture("monster_enemy.png"), 6) { Type = MapIcon.IconType.Monster, Rarity = rarity };
				case Rarity.Magic: return new MapIconCreature(e, new HudTexture("monster_enemy_blue.png"), 8) { Type = MapIcon.IconType.Monster, Rarity = rarity };
				case Rarity.Rare: return new MapIconCreature(e, new HudTexture("monster_enemy_yellow.png"), 10) { Type = MapIcon.IconType.Monster, Rarity = rarity };
				case Rarity.Unique: return new MapIconCreature(e, new HudTexture("monster_enemy_orange.png"), 10) { Type = MapIcon.IconType.Monster, Rarity = rarity };
			}
			return null;
		}
	}
}
