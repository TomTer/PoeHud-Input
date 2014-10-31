using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Preload
{
	public class PreloadAlert : HUDPluginBase
	{
		private HashSet<string> disp;
		private Dictionary<string, string> alertStrings;
		private int lastCount;

		public override void OnEnable()
		{
			this.disp = new HashSet<string>();
			this.InitAlertStrings();
			this.model.Area.OnAreaChange += this.CurrentArea_OnAreaChange;
			this.CurrentArea_OnAreaChange(this.model.Area);
		}
		public override void OnDisable()
		{
		}
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			if (Settings.GetBool("PreloadAlert"))
			{
				this.Parse();
			}
		}
		private void Parse()
		{
			this.disp.Clear();
			int pFileRoot = this.model.Memory.ReadInt(this.model.Memory.BaseAddress + model.Memory.offsets.FileRoot);
			int num2 = this.model.Memory.ReadInt(pFileRoot + 12);
			int listIterator = this.model.Memory.ReadInt(pFileRoot + 20);
			int areaChangeCount = this.model.Internal.AreaChangeCount;
			for (int i = 0; i < num2; i++)
			{
				listIterator = this.model.Memory.ReadInt(listIterator);
				if (this.model.Memory.ReadInt(listIterator + 8) != 0 && this.model.Memory.ReadInt(listIterator + 12, 36) == areaChangeCount)
				{
					string text = this.model.Memory.ReadStringU(this.model.Memory.ReadInt(listIterator + 8), 256, true);
					if (text.Contains("vaal_sidearea"))
					{
						this.disp.Add("Area contains Corrupted Area");
					}
					if (text.Contains('@'))
					{
						text = text.Split(new char[] { '@' })[0];
					}
					if (text.StartsWith("Metadata/Monsters/Missions/MasterStrDex"))
					{
						Console.WriteLine("bad alert " + text);
						this.disp.Add("Area contains Vagan, Weaponmaster");
					}
					if (this.alertStrings.ContainsKey(text))
					{
						Console.WriteLine("Alert because of " + text);
						this.disp.Add(this.alertStrings[text]);
					}
					else
					{
						if (text.EndsWith("BossInvasion"))
						{
							this.disp.Add("Area contains Invasion Boss");
						}
					}
				}
			}
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.GetBool("PreloadAlert"))
			{
				return;
			}
			int num = this.model.Memory.ReadInt(this.model.Memory.BaseAddress + model.Memory.offsets.FileRoot, new int[]
			{
				12
			});
			if (num != this.lastCount)
			{
				this.lastCount = num;
				this.Parse();
			}
			if (this.disp.Count > 0)
			{

				Vec2 vec = mountPoints[UiMountPoint.LeftOfMinimap];
				int num2 = vec.Y;
				int maxWidth = 0;
				int @int = Settings.GetInt("PreloadAlert.FontSize");
				int int2 = Settings.GetInt("PreloadAlert.BgAlpha");
				foreach (string current in this.disp)
				{
					Vec2 vec2 = rc.AddTextWithHeight(new Vec2(vec.X, num2), current, Color.White, @int, DrawTextFormat.Right);
					if (vec2.X + 10 > maxWidth)
					{
						maxWidth = vec2.X + 10;
					}
					num2 += vec2.Y;
				}
				if (maxWidth > 0 && int2 > 0)
				{
					Rect bounds = new Rect(vec.X - maxWidth + 5, vec.Y - 5, maxWidth, num2 - vec.Y + 10);
					rc.AddBox(bounds, Color.FromArgb(int2, 1, 1, 1));
					mountPoints[UiMountPoint.LeftOfMinimap] = new Vec2(vec.X, vec.Y + 5 + bounds.H);
				}

				
			}
		}
		private void InitAlertStrings()
		{
			this.alertStrings = new Dictionary<string, string>();
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Strongbox", "Area contains Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Strongbox.ao", "Area contains Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Ornate", "Area contains Ornate Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/OrnateStrongbox.ao", "Area contains Ornate Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Large", "Area contains Large Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/LargeStrongbox.ao", "Area contains Large Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Jeweller", "Area contains Jeweler's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/JewellerStrongBox.ao", "Area contains Jeweler's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Gemcutter", "Area contains Gemcutter's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/GemcutterStrongBox.ao", "Area contains Gemcutter's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Chemist", "Area contains Chemist's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/ChemistStrongBox.ao", "Area contains Chemist's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Cartographer", "Area contains Cartographer's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/CartographerStrongBox.ao", "Area contains Cartographer's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Artisan", "Area contains Artisan's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/ArtisanStrongBox.ao", "Area contains Artisan's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Arsenal", "Area contains Blacksmith's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/ArsenalStrongBox.ao", "Area contains Blacksmith's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Armory", "Area contains Armourer's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/ArmoryStrongBox.ao", "Area contains Armourer's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/Arcanist", "Area contains Arcanist's Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/ArcanistStrongBox.ao", "Area contains Arcanist's Strongbox");
			this.alertStrings.Add("Metadata/Chests/CopperChestEpic3", "Area contains Large Chest");
			this.alertStrings.Add("Metadata/Chests/CopperChests/CopperChestEpic3.ao", "Area contains Large Chest");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/PerandusBox.ao", "Area contains Perandus Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/KaomBox.ao", "Area contains Kaom Strongbox");
			this.alertStrings.Add("Metadata/Chests/StrongBoxes/MalachaisBox.ao", "Area contains Malachai Strongbox");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileRanger1", "Area contains Orra Greengate");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileRanger2", "Area contains Thena Moga");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileRanger3", "Area contains Antalie Napora");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileDuelist1", "Area contains Torr Olgosso");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileDuelist2", "Area contains Armios Bell");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileDuelist4", "Area contains Zacharie Desmarais");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileWitch1", "Area contains Minara Anenima");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileWitch2", "Area contains Igna Phoenix");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileMarauder1", "Area contains Jonah Unchained");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileMarauder2", "Area contains Damoi Tui");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileMarauder3", "Area contains Xandro Blooddrinker");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileMarauder5", "Area contains Vickas Giantbone");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileTemplar1", "Area contains Eoin Greyfur");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileTemplar2", "Area contains Tinevin Highdove");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileTemplar4", "Area contains Magnus Stonethorn");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileShadow1", "Area contains Ion Darkshroud");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileShadow2", "Area contains Ash Lessard");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileShadow4", "Area contains Wilorin Demontamer");
			this.alertStrings.Add("Metadata/Monsters/Exiles/ExileScion2", "Area contains Augustina Solaria");
			this.alertStrings.Add("Metadata/Monsters/Squid/SquidBossSideArea", "Area contains The All-seeing Eye");
			this.alertStrings.Add("Metadata/Monsters/Goatman/GoatmanLeapBossSideArea", "Area contains Konu, Maker of Wind");
			this.alertStrings.Add("Metadata/Monsters/GhostPirates/GhostPirateBossSideArea", "Area contains Coniraya, Shadow of Malice");
			this.alertStrings.Add("Metadata/Monsters/DemonModular/DemonModularElementsBossSideArea", "Area contains Atziri's Pride");
			this.alertStrings.Add("Metadata/Monsters/Goatman/GoatmanShamanBossSideArea", "Area contains Sheaq, Maker of Floods");
			this.alertStrings.Add("Metadata/Monsters/Skeleton/SkeletonMeleeLargeBossSideArea", "Area contains Ossecati, Boneshaper");
			this.alertStrings.Add("Metadata/Monsters/RootSpiders/RootSpiderBossSideArea", "Area contains Kamaq, Soilmaker");
			this.alertStrings.Add("Metadata/Monsters/Kiweth/KiwethBossSideArea", "Area contains Inti of the Blood Moon");
			this.alertStrings.Add("Metadata/Monsters/incaminion/FragmentBossSideArea", "Area contains Shrapnelbearer");
			this.alertStrings.Add("Metadata/Monsters/Snake/SnakeRoboBossSideArea", "Area contains Wiraqucha, Ancient Guardian");
			this.alertStrings.Add("Metadata/Monsters/DemonFemale/WhipDemonBossSideArea", "Area contains Cava, Artist of Pain");
			this.alertStrings.Add("Metadata/Monsters/Pyromaniac/PyromaniacBossSideArea", "Area contains Curator Miem");
			this.alertStrings.Add("Metadata/Monsters/BloodChieftain/MonkeyChiefBossSideArea", "Area contains Simi, the Nature Touched");
			this.alertStrings.Add("Metadata/Monsters/InsectSpawner/CarrionQueenBossSideArea", "Area contains The Sunburst Queen");
			this.alertStrings.Add("Metadata/Monsters/Totems/TotemBossSideArea", "Area contains M'gaska, the Living Pyre");
			this.alertStrings.Add("Metadata/Monsters/Spiders/SpiderBossSideArea", "Area contains Cintiq, the Inescapable");
			this.alertStrings.Add("Metadata/Monsters/Snake/SnakeScorpionBossSideArea", "Area contains Thornrunner");
			this.alertStrings.Add("Metadata/Monsters/Cannibal/CannibalBossSideArea", "Area contains Perquil the Lucky");
			this.alertStrings.Add("Metadata/Monsters/Skeletons/ConstructMeleeBossSideArea", "Area contains Haviri, Vaal Metalsmith");
			this.alertStrings.Add("Metadata/Monsters/Skeletons/ConstructRangedBossSideArea", "Area contains Kutec, Vaal Fleshsmith");
			this.alertStrings.Add("Metadata/Monsters/AnimatedItem/AnimatedArmourBossSideArea", "Area contains Shadow of Vengeance");
			this.alertStrings.Add("Metadata/Monsters/DemonBosses/DemonBoss1/Demon1BossSideArea", "Area contains Beheader Ataguchu");
			this.alertStrings.Add("Metadata/Monsters/DemonBosses/DemonBoss2/Demon2BossSideArea", "Area contains Wiraq, the Impaler");
			this.alertStrings.Add("Metadata/Monsters/DemonBosses/DemonBoss3/Demon3BossSideArea", "Area contains Ch'aska, Maker of Rain");
			this.alertStrings.Add("Metadata/Monsters/SandSpitterEmerge/SandSpitterBossSideArea", "Area contains Mother of the Hive");
			this.alertStrings.Add("Metadata/Monsters/Seawitch/SeaWitchBossSideArea", "Area contains Rima, Deep Temptress");
			this.alertStrings.Add("Metadata/Monsters/Axis/AxisCasterBossInvasion", "Area contains Evocata Apocalyptica");
			this.alertStrings.Add("Metadata/Monsters/Axis/AxisExperimenterBossInvasion", "Area contains Docere Incarnatis");
			this.alertStrings.Add("Metadata/Monsters/Axis/AxisSoldierBossInvasion", "Area contains Corrector Draconides");
			this.alertStrings.Add("Metadata/Monsters/Bandits/BanditMeleeBossInvasion", "Area contains Balus Stoneskull");
			this.alertStrings.Add("Metadata/Monsters/Bandits/BanditBowBossInvasion", "Area contains Kall Foxfly");
			this.alertStrings.Add("Metadata/Monsters/Beasts/BeastBossInvasion", "Area contains Marrowcrush");
			this.alertStrings.Add("Metadata/Monsters/Rhoas/RhoaBossInvasion", "Area contains The Cadaver Bull");
			this.alertStrings.Add("Metadata/Monsters/BloodChieftain/BloodChieftainBossInvasion", "Area contains Junglemare");
			this.alertStrings.Add("Metadata/Monsters/BloodElemental/BloodElementalBossInvasion", "Area contains The Sanguine Wave");
			this.alertStrings.Add("Metadata/Monsters/Cannibal/CannibalMaleBossInvasion", "Area contains Graveblood");
			this.alertStrings.Add("Metadata/Monsters/Cannibal/CannibalFemaleBossInvasion", "Area contains Nighteater");
			this.alertStrings.Add("Metadata/Monsters/Undying/CityStalkerMaleBossInvasion", "Area contains The Book Burner");
			this.alertStrings.Add("Metadata/Monsters/Undying/CityStalkerFemaleBossInvasion", "Area contains The Bolt Juggler");
			this.alertStrings.Add("Metadata/Monsters/DemonFemale/DemonFemaleBossInvasion", "Area contains Avatar of Pain");
			this.alertStrings.Add("Metadata/Monsters/DemonModular/DemonModularBossInvasion", "Area contains Rancor");
			this.alertStrings.Add("Metadata/Monsters/DemonModular/DemonFemaleRangedBossInvasion", "Area contains Hatespitter");
			this.alertStrings.Add("Metadata/Monsters/MossMonster/FireMonsterBossInvasion", "Area contains Bluntslag");
			this.alertStrings.Add("Metadata/Monsters/Monkeys/FlameBearerBossInvasion", "Area contains The Revenant");
			this.alertStrings.Add("Metadata/Monsters/incaminion/FragmentBossInvasion", "Area contains Judgement Apparatus");
			this.alertStrings.Add("Metadata/Monsters/Frog/FrogBossInvasion", "Area contains Spinesnap");
			this.alertStrings.Add("Metadata/Monsters/GemMonster/GemFrogBossInvasion", "Area contains Genesis Paradisae");
			this.alertStrings.Add("Metadata/Monsters/Goatman/GoatmanBossInvasion", "Area contains Death from Above");
			this.alertStrings.Add("Metadata/Monsters/Goatman/GoatmanShamanBossInvasion", "Area contains Guardian of the Mound");
			this.alertStrings.Add("Metadata/Monsters/Grappler/GrapplerBossInvasion", "Area contains Wonderwalker");
			this.alertStrings.Add("Metadata/Monsters/Guardians/GuardianFireBossInvasion", "Area contains The Raging Mask");
			this.alertStrings.Add("Metadata/Monsters/Guardians/GuardianLightningBossInvasion", "Area contains The Teetering Mask");
			this.alertStrings.Add("Metadata/Monsters/Guardians/GuardianHeadFireBossInvasion", "Area contains The Furious Mask");
			this.alertStrings.Add("Metadata/Monsters/Guardians/GuardianHeadColdBossInvasion", "Area contains The Callous Mask");
			this.alertStrings.Add("Metadata/Monsters/Guardians/GuardianHeadLightningBossInvasion", "Area contains The Capricious Mask");
			this.alertStrings.Add("Metadata/Monsters/GemMonster/IguanaBossInvasion", "Area contains Alpha Paradisae");
			this.alertStrings.Add("Metadata/Monsters/InsectSpawner/CarrionQueenBossInvasion", "Area contains Mother of the Swarm");
			this.alertStrings.Add("Metadata/Monsters/Kiweth/KiwethBossInvasion", "Area contains Deathflutter");
			this.alertStrings.Add("Metadata/Monsters/Lion/LionBossInvasion", "Area contains Bladetooth");
			this.alertStrings.Add("Metadata/Monsters/MossMonster/MossMonsterBossInvasion", "Area contains Granitecrush");
			this.alertStrings.Add("Metadata/Monsters/Necromancer/NecromancerBossInvasion", "Area contains Corpsestitch");
			this.alertStrings.Add("Metadata/Monsters/Pyromaniac/PyromaniacBossInvasion", "Area contains The Firestarter");
			this.alertStrings.Add("Metadata/Monsters/RootSpiders/RootSpiderBossInvasion", "Area contains Wrigglechaw");
			this.alertStrings.Add("Metadata/Monsters/SandSpitterEmerge/SandSpitterBossInvasion", "Area contains Blinkflame");
			this.alertStrings.Add("Metadata/Monsters/Seawitch/SeaWitchBossInvasion", "Area contains The Duchess");
			this.alertStrings.Add("Metadata/Monsters/ShieldCrabs/ShieldCrabBossInvasion", "Area contains Shivershell");
			this.alertStrings.Add("Metadata/Monsters/SandSpitters/SandSpitterFromCrabBossInvasion", "Area contains Shivershell");
			this.alertStrings.Add("Metadata/Monsters/Beasts/BeastSkeletonBossInvasion", "Area contains Mammothcage");
			this.alertStrings.Add("Metadata/Monsters/Skeletons/SkeletonElementalBossInvasion", "Area contains Harbinger of Elements");
			this.alertStrings.Add("Metadata/Monsters/Skeletons/SkeletonBowBossInvasion", "Area contains Nightsight");
			this.alertStrings.Add("Metadata/Monsters/Snake/SnakeMeleeBossInvasion", "Area contains Tailsinger");
			this.alertStrings.Add("Metadata/Monsters/Snake/SnakeRangedBossInvasion", "Area contains Razorleaf");
			this.alertStrings.Add("Metadata/Monsters/Spawn/SpawnBossInvasion", "Area contains Stranglecreep");
			this.alertStrings.Add("Metadata/Monsters/Spiders/SpiderBossInvasion", "Area contains Pewterfang");
			this.alertStrings.Add("Metadata/Monsters/Spikers/SpikerBossInvasion", "Area contains Bladeback Guardian");
			this.alertStrings.Add("Metadata/Monsters/Squid/SquidBossInvasion", "Area contains Strangledrift");
			this.alertStrings.Add("Metadata/Monsters/Totems/TotemBossInvasion", "Area contains Jikeji");
			this.alertStrings.Add("Metadata/Monsters/Rhoas/RhoaUndeadBossInvasion", "Area contains Ghostram");
			this.alertStrings.Add("Metadata/Monsters/Undying/UndyingBossInvasion", "Area contains Stranglecrawl");
			this.alertStrings.Add("Metadata/Monsters/WaterElemental/WaterElementalBossInvasion", "Area contains Mirageblast");
			this.alertStrings.Add("Metadata/Monsters/Zombies/ZombieBossInvasion", "Area contains The Walking Waste");
			this.alertStrings.Add("Metadata/Monsters/Skeletons/SkeletonMeleeBossInvasion", "Area contains Glassmaul");
			this.alertStrings.Add("Metadata/Monsters/Skeletons/SkeletonLargeBossInvasion", "Area contains Grath");
			this.alertStrings.Add("Metadata/Monsters/Skeletons/ConstructBossInvasion", "Area contains The Spiritless");
			this.alertStrings.Add("Metadata/Monsters/GhostPirates/GhostPirateBossInvasion", "Area contains Droolscar");
			this.alertStrings.Add("Metadata/Monsters/Rhoas/RhoaAlbino", "Area contains Albino Rhoa");
			this.alertStrings.Add("Metadata/NPC/Missions/Wild/Dex", "Area contains Tora, Master of the Hunt");
			this.alertStrings.Add("Metadata/NPC/Missions/Wild/DexInt", "Area contains Vorici, Master Assassin");
			this.alertStrings.Add("Metadata/NPC/Missions/Wild/Int", "Area contains Catarina, Master of the Dead");
			this.alertStrings.Add("Metadata/NPC/Missions/Wild/Str", "Area contains Haku, Armourmaster");
			this.alertStrings.Add("Metadata/NPC/Missions/Wild/StrDex", "Area contains Vagan, Weaponmaster");
			this.alertStrings.Add("Metadata/NPC/Missions/Wild/StrDexInt", "Area contains Zana, Master Cartographer");
			this.alertStrings.Add("Metadata/NPC/Missions/Wild/StrInt", "Area contains Elreon, Loremaster");
		}
	}
}
