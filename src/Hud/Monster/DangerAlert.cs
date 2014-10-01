using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.ExileBot;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Monster
{
	public class DangerAlert : HUDPlugin
	{
		private HashSet<int> alertBlacklist;
		private Dictionary<Entity, string> currentAlerts;
		private Dictionary<string, string> modAlerts;
		private Dictionary<string, string> typeAlerts;
		public override void OnEnable()
		{
			this.alertBlacklist = new HashSet<int>();
			this.currentAlerts = new Dictionary<Entity, string>();
			this.InitAlertStrings();
			this.poe.Area.OnAreaChange += this.CurrentArea_OnAreaChange;
			this.poe.EntityList.OnEntityAdded += this.EntityList_OnEntityAdded;
			this.poe.EntityList.OnEntityRemoved += this.EntityList_OnEntityRemoved;
			foreach (Entity current in this.poe.Entities)
			{
				this.EntityList_OnEntityAdded(current);
			}
		}
		public override void OnDisable()
		{
		}
		private void EntityList_OnEntityRemoved(Entity entity)
		{
			if (this.currentAlerts.ContainsKey(entity))
			{
				this.currentAlerts.Remove(entity);
			}
		}
		private void EntityList_OnEntityAdded(Entity entity)
		{
			if (!Settings.GetBool("DangerAlert") || this.currentAlerts.ContainsKey(entity))
			{
				return;
			}
			if (entity.IsAlive && entity.HasComponent<Poe.EntityComponents.Monster>() && entity.HasComponent<ObjectMagicProperties>() && entity.GetComponent<ObjectMagicProperties>().Rarity >= MonsterRarity.Magic)
			{
				string text = entity.Path;
				if (text.Contains('@'))
				{
					text = text.Split('@')[0];
				}
				if (this.typeAlerts.ContainsKey(text))
				{
					this.currentAlerts.Add(entity, this.typeAlerts[text]);
					this.PlaySound(entity);
					return;
				}
				foreach (string current in entity.GetComponent<ObjectMagicProperties>().Mods)
				{
					if (this.modAlerts.ContainsKey(current))
					{
						this.currentAlerts.Add(entity, this.modAlerts[current]);
						this.PlaySound(entity);
						break;
					}
				}
			}
		}
		private void PlaySound(Entity entity)
		{
			if (!Settings.GetBool("DangerAlert.PlaySound"))
			{
				return;
			}
			if (!this.alertBlacklist.Contains(entity.Id))
			{
				Sounds.DangerSound.Play();
				this.alertBlacklist.Add(entity.Id);
			}
		}
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			this.alertBlacklist.Clear();
			this.currentAlerts.Clear();
		}
		public override void Render(RenderingContext rc)
		{
			if (!Settings.GetBool("DangerAlert.ShowText"))
			{
				return;
			}
			Rect rect = this.poe.Window.ClientRect();
			int num = rect.W / 2 + rect.X;
			int num2 = (int)((float)rect.H * 0.2f) + rect.Y;
			int num3 = 0;
			int num4 = 0;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<Entity, string> current in this.currentAlerts)
			{
				if (current.Key.IsAlive && !hashSet.Contains(current.Value))
				{
					hashSet.Add(current.Value);
				}
			}
			int @int = Settings.GetInt("DangerAlert.ShowText.FontSize");
			foreach (string current2 in hashSet)
			{
				Vec2 vec = rc.AddTextWithHeight(new Vec2(num, num2), current2, Color.Red, @int, DrawTextFormat.Center);
				if (vec.X > num3)
				{
					num3 = vec.X;
				}
				num2 += vec.Y;
				num4 += vec.Y;
			}
			if (num3 > 0)
			{
				Rect rect2 = new Rect(num - num3 / 2 - 5, (int)((float)rect.H * 0.2f) + rect.Y - 5, num3 + 10, num4 + 10);
				rc.AddBox(rect2, Color.FromArgb(Settings.GetInt("DangerAlert.ShowText.BgAlpha"), 1, 1, 1));
			}
		}
		private void InitAlertStrings()
		{
			this.modAlerts = new Dictionary<string, string>();
			this.modAlerts.Add("MonsterNemesisUniqueDrop", "Inner Treasure nearby");
			this.modAlerts.Add("MonsterImplicitNemesisUniqueDrop", "Inner Treasure nearby");
			this.modAlerts.Add("MonsterAuraPhysicalThorns1", "Physical Reflect nearby");
			this.modAlerts.Add("MonsterPhysicalThorns", "Physical Reflect nearby");
			this.modAlerts.Add("MonsterAuraElementalThorns1", "Elemental Reflect nearby");
			this.modAlerts.Add("MonsterElementalThorns", "Elemental Reflect nearby");
			this.modAlerts.Add("MonsterNemesisCorruptedBlood", "Corrupting Blood nearby");
			this.modAlerts.Add("MonsterItems1", "Wealth monsters nearby");
			this.typeAlerts = new Dictionary<string, string>();
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileRanger1", "Orra Greengate nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileRanger2", "Thena Moga nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileRanger3", "Antalie Napora nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileDuelist1", "Torr Olgosso nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileDuelist2", "Armios Bell nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileDuelist4", "Zacharie Desmarais nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileWitch1", "Minara Anenima nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileWitch2", "Igna Phoenix nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileMarauder1", "Jonah Unchained nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileMarauder2", "Damoi Tui nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileMarauder3", "Xandro Blooddrinker nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileMarauder5", "Vickas Giantbone nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileTemplar1", "Eoin Greyfur nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileTemplar2", "Tinevin Highdove nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileTemplar4", "Magnus Stonethorn nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileShadow1", "Ion Darkshroud nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileShadow2", "Ash Lessard nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileShadow4", "Wilorin Demontamer nearby");
			this.typeAlerts.Add("Metadata/Monsters/Exiles/ExileScion2", "Augustina Solaria nearby");
			this.typeAlerts.Add("Metadata/Monsters/Squid/SquidBossSideArea", "The All-seeing Eye nearby");
			this.typeAlerts.Add("Metadata/Monsters/Goatman/GoatmanLeapBossSideArea", "Konu, Maker of Wind nearby");
			this.typeAlerts.Add("Metadata/Monsters/GhostPirates/GhostPirateBossSideArea", "Coniraya, Shadow of Malice nearby");
			this.typeAlerts.Add("Metadata/Monsters/DemonModular/DemonModularElementsBossSideArea", "Atziri's Pride nearby");
			this.typeAlerts.Add("Metadata/Monsters/Goatman/GoatmanShamanBossSideArea", "Sheaq, Maker of Floods nearby");
			this.typeAlerts.Add("Metadata/Monsters/Skeleton/SkeletonMeleeLargeBossSideArea", "Ossecati, Boneshaper nearby");
			this.typeAlerts.Add("Metadata/Monsters/RootSpiders/RootSpiderBossSideArea", "Kamaq, Soilmaker nearby");
			this.typeAlerts.Add("Metadata/Monsters/Kiweth/KiwethBossSideArea", "Inti of the Blood Moon nearby");
			this.typeAlerts.Add("Metadata/Monsters/incaminion/FragmentBossSideArea", "Shrapnelbearer nearby");
			this.typeAlerts.Add("Metadata/Monsters/Snake/SnakeRoboBossSideArea", "Wiraqucha, Ancient Guardian nearby");
			this.typeAlerts.Add("Metadata/Monsters/DemonFemale/WhipDemonBossSideArea", "Cava, Artist of Pain nearby");
			this.typeAlerts.Add("Metadata/Monsters/Pyromaniac/PyromaniacBossSideArea", "Curator Miem nearby");
			this.typeAlerts.Add("Metadata/Monsters/BloodChieftain/MonkeyChiefBossSideArea", "Simi, the Nature Touched nearby");
			this.typeAlerts.Add("Metadata/Monsters/InsectSpawner/CarrionQueenBossSideArea", "The Sunburst Queen nearby");
			this.typeAlerts.Add("Metadata/Monsters/Totems/TotemBossSideArea", "M'gaska, the Living Pyre nearby");
			this.typeAlerts.Add("Metadata/Monsters/Spiders/SpiderBossSideArea", "Cintiq, the Inescapable nearby");
			this.typeAlerts.Add("Metadata/Monsters/Snake/SnakeScorpionBossSideArea", "Thornrunner nearby");
			this.typeAlerts.Add("Metadata/Monsters/Cannibal/CannibalBossSideArea", "Perquil the Lucky nearby");
			this.typeAlerts.Add("Metadata/Monsters/Skeletons/ConstructMeleeBossSideArea", "Haviri, Vaal Metalsmith nearby");
			this.typeAlerts.Add("Metadata/Monsters/Skeletons/ConstructRangedBossSideArea", "Kutec, Vaal Fleshsmith nearby");
			this.typeAlerts.Add("Metadata/Monsters/AnimatedItem/AnimatedArmourBossSideArea", "Shadow of Vengeance nearby");
			this.typeAlerts.Add("Metadata/Monsters/DemonBosses/DemonBoss1/Demon1BossSideArea", "Beheader Ataguchu nearby");
			this.typeAlerts.Add("Metadata/Monsters/DemonBosses/DemonBoss2/Demon2BossSideArea", "Wiraq, the Impaler nearby");
			this.typeAlerts.Add("Metadata/Monsters/DemonBosses/DemonBoss3/Demon3BossSideArea", "Ch'aska, Maker of Rain nearby");
			this.typeAlerts.Add("Metadata/Monsters/SandSpitterEmerge/SandSpitterBossSideArea", "Mother of the Hive nearby");
			this.typeAlerts.Add("Metadata/Monsters/Seawitch/SeaWitchBossSideArea", "Rima, Deep Temptress nearby");
			this.typeAlerts.Add("Metadata/Monsters/Axis/AxisCasterBossInvasion", "Evocata Apocalyptica nearby");
			this.typeAlerts.Add("Metadata/Monsters/Axis/AxisExperimenterBossInvasion", "Docere Incarnatis nearby");
			this.typeAlerts.Add("Metadata/Monsters/Axis/AxisSoldierBossInvasion", "Corrector Draconides nearby");
			this.typeAlerts.Add("Metadata/Monsters/Bandits/BanditMeleeBossInvasion", "Balus Stoneskull nearby");
			this.typeAlerts.Add("Metadata/Monsters/Bandits/BanditBowBossInvasion", "Kall Foxfly nearby");
			this.typeAlerts.Add("Metadata/Monsters/Beasts/BeastBossInvasion", "Marrowcrush nearby");
			this.typeAlerts.Add("Metadata/Monsters/Rhoas/RhoaBossInvasion", "The Cadaver Bull nearby");
			this.typeAlerts.Add("Metadata/Monsters/BloodChieftain/BloodChieftainBossInvasion", "Junglemare nearby");
			this.typeAlerts.Add("Metadata/Monsters/BloodElemental/BloodElementalBossInvasion", "The Sanguine Wave nearby");
			this.typeAlerts.Add("Metadata/Monsters/Cannibal/CannibalMaleBossInvasion", "Graveblood nearby");
			this.typeAlerts.Add("Metadata/Monsters/Cannibal/CannibalFemaleBossInvasion", "Nighteater nearby");
			this.typeAlerts.Add("Metadata/Monsters/Undying/CityStalkerMaleBossInvasion", "The Book Burner nearby");
			this.typeAlerts.Add("Metadata/Monsters/Undying/CityStalkerFemaleBossInvasion", "The Bolt Juggler nearby");
			this.typeAlerts.Add("Metadata/Monsters/DemonFemale/DemonFemaleBossInvasion", "Avatar of Pain nearby");
			this.typeAlerts.Add("Metadata/Monsters/DemonModular/DemonModularBossInvasion", "Rancor nearby");
			this.typeAlerts.Add("Metadata/Monsters/DemonModular/DemonFemaleRangedBossInvasion", "Hatespitter nearby");
			this.typeAlerts.Add("Metadata/Monsters/MossMonster/FireMonsterBossInvasion", "Bluntslag nearby");
			this.typeAlerts.Add("Metadata/Monsters/Monkeys/FlameBearerBossInvasion", "The Revenant nearby");
			this.typeAlerts.Add("Metadata/Monsters/incaminion/FragmentBossInvasion", "Judgement Apparatus nearby");
			this.typeAlerts.Add("Metadata/Monsters/Frog/FrogBossInvasion", "Spinesnap nearby");
			this.typeAlerts.Add("Metadata/Monsters/GemMonster/GemFrogBossInvasion", "Genesis Paradisae nearby");
			this.typeAlerts.Add("Metadata/Monsters/Goatman/GoatmanBossInvasion", "Death from Above nearby");
			this.typeAlerts.Add("Metadata/Monsters/Goatman/GoatmanShamanBossInvasion", "Guardian of the Mound nearby");
			this.typeAlerts.Add("Metadata/Monsters/Grappler/GrapplerBossInvasion", "Wonderwalker nearby");
			this.typeAlerts.Add("Metadata/Monsters/Guardians/GuardianFireBossInvasion", "The Raging Mask nearby");
			this.typeAlerts.Add("Metadata/Monsters/Guardians/GuardianLightningBossInvasion", "The Teetering Mask nearby");
			this.typeAlerts.Add("Metadata/Monsters/Guardians/GuardianHeadFireBossInvasion", "The Furious Mask nearby");
			this.typeAlerts.Add("Metadata/Monsters/Guardians/GuardianHeadColdBossInvasion", "The Callous Mask nearby");
			this.typeAlerts.Add("Metadata/Monsters/Guardians/GuardianHeadLightningBossInvasion", "The Capricious Mask nearby");
			this.typeAlerts.Add("Metadata/Monsters/GemMonster/IguanaBossInvasion", "Alpha Paradisae nearby");
			this.typeAlerts.Add("Metadata/Monsters/InsectSpawner/CarrionQueenBossInvasion", "Mother of the Swarm nearby");
			this.typeAlerts.Add("Metadata/Monsters/Kiweth/KiwethBossInvasion", "Deathflutter nearby");
			this.typeAlerts.Add("Metadata/Monsters/Lion/LionBossInvasion", "Bladetooth nearby");
			this.typeAlerts.Add("Metadata/Monsters/MossMonster/MossMonsterBossInvasion", "Granitecrush nearby");
			this.typeAlerts.Add("Metadata/Monsters/Necromancer/NecromancerBossInvasion", "Corpsestitch nearby");
			this.typeAlerts.Add("Metadata/Monsters/Pyromaniac/PyromaniacBossInvasion", "The Firestarter nearby");
			this.typeAlerts.Add("Metadata/Monsters/RootSpiders/RootSpiderBossInvasion", "Wrigglechaw nearby");
			this.typeAlerts.Add("Metadata/Monsters/SandSpitterEmerge/SandSpitterBossInvasion", "Blinkflame nearby");
			this.typeAlerts.Add("Metadata/Monsters/Seawitch/SeaWitchBossInvasion", "The Duchess nearby");
			this.typeAlerts.Add("Metadata/Monsters/ShieldCrabs/ShieldCrabBossInvasion", "Shivershell nearby");
			this.typeAlerts.Add("Metadata/Monsters/SandSpitters/SandSpitterFromCrabBossInvasion", "Shivershell nearby");
			this.typeAlerts.Add("Metadata/Monsters/Beasts/BeastSkeletonBossInvasion", "Mammothcage nearby");
			this.typeAlerts.Add("Metadata/Monsters/Skeletons/SkeletonElementalBossInvasion", "Harbinger of Elements nearby");
			this.typeAlerts.Add("Metadata/Monsters/Skeletons/SkeletonBowBossInvasion", "Nightsight nearby");
			this.typeAlerts.Add("Metadata/Monsters/Snake/SnakeMeleeBossInvasion", "Tailsinger nearby");
			this.typeAlerts.Add("Metadata/Monsters/Snake/SnakeRangedBossInvasion", "Razorleaf nearby");
			this.typeAlerts.Add("Metadata/Monsters/Spawn/SpawnBossInvasion", "Stranglecreep nearby");
			this.typeAlerts.Add("Metadata/Monsters/Spiders/SpiderBossInvasion", "Pewterfang nearby");
			this.typeAlerts.Add("Metadata/Monsters/Spikers/SpikerBossInvasion", "Bladeback Guardian nearby");
			this.typeAlerts.Add("Metadata/Monsters/Squid/SquidBossInvasion", "Strangledrift nearby");
			this.typeAlerts.Add("Metadata/Monsters/Totems/TotemBossInvasion", "Jikeji nearby");
			this.typeAlerts.Add("Metadata/Monsters/Rhoas/RhoaUndeadBossInvasion", "Ghostram nearby");
			this.typeAlerts.Add("Metadata/Monsters/Undying/UndyingBossInvasion", "Stranglecrawl nearby");
			this.typeAlerts.Add("Metadata/Monsters/WaterElemental/WaterElementalBossInvasion", "Mirageblast nearby");
			this.typeAlerts.Add("Metadata/Monsters/Zombies/ZombieBossInvasion", "The Walking Waste nearby");
			this.typeAlerts.Add("Metadata/Monsters/Skeletons/SkeletonMeleeBossInvasion", "Glassmaul nearby");
			this.typeAlerts.Add("Metadata/Monsters/Skeletons/SkeletonLargeBossInvasion", "Grath nearby");
			this.typeAlerts.Add("Metadata/Monsters/Skeletons/ConstructBossInvasion", "The Spiritless nearby");
			this.typeAlerts.Add("Metadata/Monsters/GhostPirates/GhostPirateBossInvasion", "Droolscar nearby");
		}
	}
}
