using System.Collections.Generic;
//using PoeHUD.Hud.Debug;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.DPS;
using PoeHUD.Hud.Health;
using PoeHUD.Hud.Icons;
using PoeHUD.Hud.Loot;
using PoeHUD.Hud.MaxRolls;
using PoeHUD.Hud.Monster;
using PoeHUD.Hud.Preload;
using PoeHUD.Hud.XpRate;

namespace PoeHUD.Hud
{

	public class OverlayRenderer
	{
		private readonly List<HUDPlugin> plugins;
		private readonly GameController gameController;
		private int _modelUpdatePeriod;
		public OverlayRenderer(GameController gameController, RenderingContext rc)
		{
			this.gameController = gameController;
			gameController.Area.OnAreaChange += area => _modelUpdatePeriod = 6;

			this.plugins = new List<HUDPlugin>{
				new HealthBarRenderer(),
				new ItemAlerter(),
				new MinimapRenderer(gatherMapIcons),
				new LargeMapRenderer(gatherMapIcons),
				new ItemLevelRenderer(),
				new ItemRollsRenderer(),
				new MonsterTracker(),
				new PoiTracker(),
				new XPHRenderer(),
				new ClientHacks(),
	#if DEBUG
			//	new ShowUiHierarchy(),
	#endif
				new PreloadAlert(),
				new DpsMeter(),
			};
			if (Settings.GetBool("Window.ShowIngameMenu"))
			{
	#if !DEBUG
				this.plugins.Add(new Menu.Menu());
	#endif
			}
			UpdateObserverLists();
			rc.OnRender += this.rc_OnRender;

			this.plugins.ForEach(x => x.Init(gameController));
		}

		private void UpdateObserverLists()
		{
			EntityListObserverComposite observer = new EntityListObserverComposite();
			observer.Observers.AddRange(plugins.OfType<EntityListObserver>());
			gameController.EntityListObserver = observer;
		}

		private IEnumerable<MapIcon> gatherMapIcons()
		{
			foreach (HUDPlugin plugin in plugins)
			{
				HUDPluginWithMapIcons iconSource = plugin as HUDPluginWithMapIcons;
				if (iconSource != null)
				{
					// kvPair.Value.RemoveAll(x => !x.IsEntityStillValid());
					foreach (MapIcon icon in iconSource.GetIcons())
						yield return icon;
				}
			}
		}

		private void rc_OnRender(RenderingContext rc)
		{
			if (Settings.GetBool("Window.RequireForeground") && !this.gameController.Window.IsForeground()) return;

			this._modelUpdatePeriod++;
			if (this._modelUpdatePeriod > 6)
			{
				this.gameController.RefreshState();
				this._modelUpdatePeriod = 0;
			}
			bool ingame = this.gameController.InGame;
			if ( !ingame || this.gameController.Player == null)
			{
				return;
			}

			Dictionary<UiMountPoint, Vec2> mountPoints = new Dictionary<UiMountPoint, Vec2>();
			mountPoints[UiMountPoint.UnderMinimap] = GetRightTopUnderMinimap();
			mountPoints[UiMountPoint.LeftOfMinimap] = GetRightTopLeftOfMinimap();

			foreach (HUDPlugin current in this.plugins)
			{
				current.Render(rc, mountPoints);
			}
		}

		private Vec2 GetRightTopLeftOfMinimap()
		{
			Rect clientRect = gameController.Internal.IngameState.IngameUi.Minimap.SmallMinimap.GetClientRect();
			return new Vec2(clientRect.X - 10, clientRect.Y + 5);
		}

		private Vec2 GetRightTopUnderMinimap()
		{
			var mm = gameController.Internal.game.IngameState.IngameUi.Minimap.SmallMinimap;
			var gl = gameController.Internal.game.IngameState.IngameUi.GemLvlUpPanel;
			Rect mmRect = mm.GetClientRect();
			Rect glRect = gl.GetClientRect();

			Rect clientRect;
			if (gl.IsVisible && glRect.X + gl.Width < mmRect.X + mmRect.X + 50) // also this +50 value doesn't seems to have any impact
				clientRect = glRect;
			else
				clientRect = mmRect;
			return new Vec2(mmRect.X + mmRect.W, clientRect.Y + clientRect.H + 10);
		}

		public bool Detach() {
			foreach (HUDPlugin current in this.plugins)
				current.OnDisable();
			return false;
		}
	}

	public enum UiMountPoint
	{
		UnderMinimap,
		LeftOfMinimap
	}
}
