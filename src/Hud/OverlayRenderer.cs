using System.Collections.Generic;
//using PoeHUD.Hud.Debug;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.AdvTooltips;
using PoeHUD.Hud.DebugView;
using PoeHUD.Hud.DPS;
using PoeHUD.Hud.Health;
using PoeHUD.Hud.Icons;
using PoeHUD.Hud.Loot;
using PoeHUD.Hud.MaxRolls;
using PoeHUD.Hud.Monster;
using PoeHUD.Hud.Preload;
using PoeHUD.Hud.XpRate;
using PoeHUD.Settings;

namespace PoeHUD.Hud
{
	public class OverlayRenderer
	{
		private readonly List<HUDPlugin> plugins;
		private readonly GameController gameController;
		private readonly SettingsRoot Settings;
		private int _modelUpdatePeriod;
		public OverlayRenderer(GameController gameController, SettingsRoot settings, RenderingContext rc)
		{
			this.Settings = settings;
			this.gameController = gameController;

			this.plugins = new List<HUDPlugin>{
				new HealthBarRenderer(),
				new ItemAlerter(),
				new MapIconsRenderer(gatherMapIcons),
				new AdvTooltopRenderer(),
				new MonsterTracker(),
				new PoiTracker(),
				new XPHRenderer(),
				new ClientHacks(),
	#if DEBUG
				//new ShowUiHierarchy(),
				//new MainAddresses(),
	#endif
				new PreloadAlert(),
				new DpsMeter(),
			};

			gameController.Area.OnAreaChange += (area) => {
				_modelUpdatePeriod = 6;
				foreach (var hudPlugin in plugins)
					hudPlugin.OnAreaChange(area);
			};

			foreach (var plugin in plugins)
			{
				if( null != plugin.SettingsNode )
					Settings.AddModule(plugin.SettingsNode);
			}
			if (Settings.Global.ShowIngameMenu)
			{
	#if !DEBUG
				this.plugins.Add(new Menu.Menu(settings));
	#endif
			}
			UpdateObserverLists();

			rc.RenderModules = this.rc_OnRender;

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
			if (Settings.Global.RequireForeground && !this.gameController.Window.IsForeground()) return;

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
			mountPoints[UiMountPoint.UnderMinimap] = gameController.Internal.IngameState.IngameUi.GetRightTopUnderMinimap();
			mountPoints[UiMountPoint.LeftOfMinimap] = gameController.Internal.IngameState.IngameUi.GetRightTopLeftOfMinimap();

			foreach (HUDPlugin current in this.plugins)
			{
				if (current.SettingsNode == null || current.SettingsNode.Enabled)
					current.Render(rc, mountPoints);
			}
		}

		public void KeyPressOnForm(object sender, KeyPressEventArgs args)
		{
			if (args.KeyChar == 'b')
			{
				Clipboard.SetText(new IdcScriptMaker(gameController).GetBaseAddressScript());
			}
			if (args.KeyChar == 'f')
			{
				Clipboard.SetText(new IdcScriptMaker(gameController).GetFlaskAddressScript());
			}
			
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
