using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Framework;
using PoeHUD.Settings;
using PoeHUD.Shell;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
	public class Menu : HUDPluginBase
	{
		public class MenuSettings : SettingsForModule
		{
			public MenuSettings() : base("Menu") { }

			public Setting<int> PositionHeight = new Setting<int>("PositionHeight", 100);
			public Setting<int> PositionWidth = new Setting<int>("PositionWidth", 0);
			public SettingIntRange AnchorWidth = new SettingIntRange("Anchor Width", 40, 100, 50);
			public SettingIntRange AnchorHeight = new SettingIntRange("Anchor Height", 16, 40, 25);
			public SettingIntRange ItemWidth = new SettingIntRange("Item Width", 40, 300, 200);
			public SettingIntRange ItemHeight = new SettingIntRange("Item Height", 16, 40, 25);
		}

		public MenuSettings Settings = new MenuSettings();

		private MouseHook hook;
		private List<BooleanButton> buttons;
		private BooleanButton currentHover;
		private Rect bounds;
		private bool menuVisible;
		private SettingsRoot settingsRoot;

		public Menu(SettingsRoot settings)
		{
			this.settingsRoot = settings;
			settingsRoot.AddModule(Settings);
			settingsRoot.ReadFromFile();
		}
		public override void OnEnable()
		{
			this.bounds = new Rect(Settings.PositionWidth, Settings.PositionHeight, Settings.AnchorWidth, Settings.AnchorHeight);
			this.CreateButtons();
			this.hook = new MouseHook(this.OnMouseEvent);
		}
		public override void OnDisable()
		{
			this.hook.Dispose();
		}

		public override SettingsForModule SettingsNode
		{
			get { return Settings; }
		}

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			int alpha = this.menuVisible ? 255 : 100;
			rc.AddBox(this.bounds, Color.FromArgb(alpha, Color.Gray));
			rc.AddTextWithHeight(new Vec2(Settings.PositionWidth + Settings.AnchorWidth / 2, Settings.PositionHeight + Settings.AnchorHeight / 2), "Menu", Color.Gray, 10, DrawTextFormat.Center | DrawTextFormat.VerticalCenter); 
			foreach (BooleanButton current in this.buttons)
				current.Render(rc);
		}


		private bool OnMouseEvent(MouseEventID id, int x, int y)
		{
			if (Settings.Global.RequireForeground && !this.model.Window.IsForeground())
			{
				return false;
			}
			Vec2 vec = this.model.Window.ScreenToClient(new Vec2(x, y));
			if (id == MouseEventID.MouseMove)
			{
				if (this.currentHover != null && this.currentHover.TestHit(vec))
				{
					this.currentHover.OnEvent(id, vec);
					return false;
				}
				foreach (var current in buttons.Where(current => current.TestHit(vec)))
				{
					if (this.currentHover != null)
						this.currentHover.SetHovered(false);
					this.currentHover = current;
					current.SetHovered(true);
					return false;
				}
				return false;
			}
			if (this.bounds.HasPoint(vec) && id == MouseEventID.LeftButtonDown)
			{
				this.menuVisible = !this.menuVisible;
				foreach (BooleanButton current2 in this.buttons)
				{
					current2.SetVisible(this.menuVisible);
				}
				return true;
			}
			if (this.currentHover != null && this.currentHover.TestHit(vec))
			{
				this.currentHover.OnEvent(id, vec);
				return true;
			}
			return false;
		}

		private void createChildMenus(BooleanButton parent, SettingsBlock module)
		{
			foreach (ISetting setting in module.Members.Where(setting => !String.IsNullOrWhiteSpace(setting.Key)))
			{
				if( setting is Setting<bool>)
					parent.AddChild(new BooleanButton(Settings, setting.Key, setting as Setting<bool>));
				else if (setting is SettingsBlock)
				{
					var sm = setting as SettingsForModule;
					var c = new BooleanButton(Settings,setting.Key, sm == null ? null : sm.Enabled);
					parent.AddChild(c);
					createChildMenus(c, setting as SettingsBlock);
				}
				if (setting is SettingIntRange)
				{
					var sir = setting as SettingIntRange;
					parent.AddChild(new IntPicker(Settings, setting.Key, sir));
				}
			}
		}

		private void CreateButtons()
		{
			this.buttons = new List<BooleanButton>();

			foreach (var module in settingsRoot.Members.OfType<SettingsForModule>())
			{
				bool isMenuSettings = module == Settings;

				BooleanButton parent = this.CreateRootMenu(module.BlockName, isMenuSettings ? null : module.Enabled);
				createChildMenus(parent, module);
			}
		}

		private BooleanButton CreateRootMenu(string text, Setting<bool> setting)
		{
			BooleanButton booleanButton = new BooleanButton(Settings, text, setting);
			int dy = Settings.AnchorHeight + this.buttons.Sum(c => c.Height);
			booleanButton.Bounds = new Rect(Settings.PositionWidth, Settings.PositionHeight + dy, booleanButton.Width, booleanButton.Height);
			this.buttons.Add(booleanButton);
			return booleanButton;
		}
	}
}


