using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Hud.Icons;
using PoeHUD.Poe.EntityComponents;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Monster
{
	public class MonsterTracker : HUDPluginBase, EntityListObserver, HUDPluginWithMapIcons
	{
		private HashSet<int> alreadyAlertedOf;
		private Dictionary<EntityWrapper, string> alertTexts;

		private Dictionary<string, string> modAlerts;
		private Dictionary<string, string> typeAlerts;
		private readonly Dictionary<EntityWrapper, MapIcon> currentIcons = new Dictionary<EntityWrapper, MapIcon>();

		public override void OnEnable()
		{
			this.alreadyAlertedOf = new HashSet<int>();
			this.alertTexts = new Dictionary<EntityWrapper, string>();
			this.InitAlertStrings();
			this.model.Area.OnAreaChange += this.CurrentArea_OnAreaChange;

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
			alertTexts.Remove(entity);
			currentIcons.Remove(entity);
		}

		public void EntityAdded(EntityWrapper entity)
		{
			if (!Settings.GetBool("MonsterTracker") || this.alertTexts.ContainsKey(entity))
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
				if (this.typeAlerts.ContainsKey(text))
				{
					this.alertTexts.Add(entity, this.typeAlerts[text]);
					this.PlaySound(entity);
					return;
				}
				foreach (string current in entity.GetComponent<ObjectMagicProperties>().Mods)
				{
					if (this.modAlerts.ContainsKey(current))
					{
						this.alertTexts.Add(entity, this.modAlerts[current]);
						this.PlaySound(entity);
						break;
					}
				}
			}
		}
		private void PlaySound(EntityWrapper entity)
		{
			if (!Settings.GetBool("MonsterTracker.PlaySound"))
			{
				return;
			}
			if (!this.alreadyAlertedOf.Contains(entity.Id))
			{
				Sounds.DangerSound.Play();
				this.alreadyAlertedOf.Add(entity.Id);
			}
		}
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			this.alreadyAlertedOf.Clear();
			this.alertTexts.Clear();
			currentIcons.Clear();
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.GetBool("MonsterTracker.ShowText"))
			{
				return;
			}
			Rect rect = this.model.Window.ClientRect();
			int xScreenCenter = rect.W / 2 + rect.X;
			int yPos = rect.H / 5 + rect.Y;

			var playerPos = this.model.Player.GetComponent<Positioned>().GridPos;
			int fontSize = Settings.GetInt("MonsterTracker.ShowText.FontSize");
			bool first = true;
			Rect rectBackground = new Rect();
			foreach (var alert in this.alertTexts)
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
				rc.AddBox(rectBackground, Color.FromArgb(Settings.GetInt("MonsterTracker.ShowText.BgAlpha"), 1, 1, 1));
				rc.AddSprite("directions.png", rectDirection, uv, Color.Red);
				yPos += textSize.Y;
			}
			if (!first)  // vertical padding below
			{
				rectBackground.Y = rectBackground.Y + rectBackground.H;
				rectBackground.H = 5;
				rc.AddBox(rectBackground, Color.FromArgb(Settings.GetInt("MonsterTracker.ShowText.BgAlpha"), 1, 1, 1));
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
			this.modAlerts = LoadMonsterModAlerts();
			this.typeAlerts = LoadMonsterNameAlerts();
		}

		private MapIcon GetMapIconForMonster(EntityWrapper e)
		{
			if (!e.IsHostile)
				return new MapIconCreature(e, new HudTexture("monster_ally.png"), 6);

			switch (e.GetComponent<ObjectMagicProperties>().Rarity)
			{
				case MonsterRarity.White: return new MapIconCreature(e, new HudTexture("monster_enemy.png"), 6);
				case MonsterRarity.Magic: return new MapIconCreature(e, new HudTexture("monster_enemy_blue.png"), 8);
				case MonsterRarity.Rare: return new MapIconCreature(e, new HudTexture("monster_enemy_yellow.png"), 10);
				case MonsterRarity.Unique: return new MapIconCreature(e, new HudTexture("monster_enemy_orange.png"), 10);
			}
			return null;
		}

		private static Dictionary<string, string> LoadMonsterModAlerts()
		{
			var result = new Dictionary<string, string>();

			string[] lines = File.ReadAllLines("config/monster_mod_alerts.txt");
			foreach (string line in lines.Select(a => a.Trim()))
			{
				if (string.IsNullOrWhiteSpace(line) || line.IndexOf(',') < 0)
					continue;

				var parts = line.Split(new[] {','}, 2);
				result[parts[0].Trim()] = parts[1].Trim();
			}

			return result;
		}

		private static Dictionary<string, string> LoadMonsterNameAlerts()
		{
			var result = new Dictionary<string, string>();

			string[] lines = File.ReadAllLines("config/monster_name_alerts.txt");
			foreach (string line in lines.Select(a => a.Trim()))
			{
				if (string.IsNullOrWhiteSpace(line) || line.IndexOf(',') < 0)
					continue;

				var parts = line.Split(new[] { ',' }, 2);
				result[parts[0].Trim()] = parts[1].Trim();
			}

			return result;
		}
	}
}
