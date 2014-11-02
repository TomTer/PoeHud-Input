using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Framework;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
	public class Menu : HUDPluginBase
	{
		private const int ButtonWidth = 210;
		private const int ButtonHeight = 40;
		private MouseHook hook;
		private List<BooleanButton> buttons;
		private BooleanButton currentHover;
		private Rect bounds;
		private bool menuVisible;
		public override void OnEnable()
		{
			this.bounds = new Rect(Settings.GetInt("Menu.PositionWidth"), Settings.GetInt("Menu.PositionHeight"), Settings.GetInt("Menu.Length"), Settings.GetInt("Menu.Size"));
			this.CreateButtons();
			this.hook = new MouseHook(new MouseHook.MouseEvent(this.OnMouseEvent));
		}
		public override void OnDisable()
		{
			this.hook.Dispose();
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			int alpha = this.menuVisible ? 255 : 100;
			rc.AddBox(this.bounds, Color.FromArgb(alpha, Color.Gray));
			rc.AddTextWithHeight(new Vec2(Settings.GetInt("Menu.PositionWidth") + 25, Settings.GetInt("Menu.PositionHeight") + 12), "Menu", Color.Gray, 10, DrawTextFormat.VerticalCenter | DrawTextFormat.Center); foreach (BooleanButton current in this.buttons)
			{
				current.Render(rc);
			}
		}


		private bool OnMouseEvent(MouseEventID id, int x, int y)
		{
			if (Settings.GetBool("Window.RequireForeground") && !this.model.Window.IsForeground())
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
				using (List<BooleanButton>.Enumerator enumerator = this.buttons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BooleanButton current = enumerator.Current;
						if (current.TestHit(vec))
						{
							if (this.currentHover != null)
							{
								this.currentHover.SetHovered(false);
							}
							this.currentHover = current;
							current.SetHovered(true);
							return false;
						}
					}
					return false;
				}
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
		private void CreateButtons()
		{
			int r = 0;
			this.buttons = new List<BooleanButton>();
			BooleanButton parent = this.CreateRootMenu("Health bars", r++, "Healthbars");
			BooleanButton booleanButton = this.AddButton(parent, "Players", "Healthbars.Players");
			BooleanButton parent2 = this.AddButton(parent, "Enemies", "Healthbars.Enemies");
			BooleanButton booleanButton2 = this.AddButton(parent, "Minions", "Healthbars.Minions");
			this.AddButton(parent, "Show ES", "Healthbars.ShowES");
			this.AddButton(parent, "Show in town", "Healthbars.ShowInTown");
			booleanButton.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Players.Width"));
			booleanButton.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Players.Height"));
			booleanButton2.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Minions.Width"));
			booleanButton2.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Minions.Height"));
			BooleanButton booleanButton3 = this.AddButton(parent2, "White", "Healthbars.Enemies.Normal");
            booleanButton3.AddChild(new BooleanButton("Print percents", "Healthbars.Enemies.Normal.PrintPercents"));
            booleanButton3.AddChild(new BooleanButton("Print health text", "Healthbars.Enemies.Normal.PrintHealthText"));
			booleanButton3.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Enemies.Normal.Width"));
			booleanButton3.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Normal.Height"));
			BooleanButton booleanButton4 = this.AddButton(parent2, "Magic", "Healthbars.Enemies.Magic");
            booleanButton4.AddChild(new BooleanButton("Print percents", "Healthbars.Enemies.Magic.PrintPercents"));
            booleanButton4.AddChild(new BooleanButton("Print health text", "Healthbars.Enemies.Magic.PrintHealthText"));
			booleanButton4.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Enemies.Magic.Width"));
			booleanButton4.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Magic.Height"));
			BooleanButton booleanButton5 = this.AddButton(parent2, "Rare", "Healthbars.Enemies.Rare");
            booleanButton5.AddChild(new BooleanButton("Print percents", "Healthbars.Enemies.Rare.PrintPercents"));
            booleanButton5.AddChild(new BooleanButton("Print health text", "Healthbars.Enemies.Rare.PrintHealthText"));
			booleanButton5.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Enemies.Rare.Width"));
			booleanButton5.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Rare.Height"));
			BooleanButton booleanButton6 = this.AddButton(parent2, "Uniques", "Healthbars.Enemies.Unique");
            booleanButton6.AddChild(new BooleanButton("Print percents", "Healthbars.Enemies.Unique.PrintPercents"));
            booleanButton6.AddChild(new BooleanButton("Print health text", "Healthbars.Enemies.Unique.PrintHealthText"));
			booleanButton6.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Enemies.Unique.Width"));
			booleanButton6.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Unique.Height"));
			BooleanButton parent3 = this.CreateRootMenu("Minimap icons", r++, "MinimapIcons");
			this.AddButton(parent3, "Monsters", "MinimapIcons.Monsters");
			this.AddButton(parent3, "Minions", "MinimapIcons.Minions");
			this.AddButton(parent3, "Strongboxes", "MinimapIcons.Strongboxes");
			this.AddButton(parent3, "Chests", "MinimapIcons.Chests");
			this.AddButton(parent3, "Alert items", "MinimapIcons.AlertedItems");
            this.AddButton(parent3, "Masters", "MinimapIcons.Masters");
			BooleanButton parent4 = this.CreateRootMenu("Item alert", r++, "ItemAlert");
			this.AddButton(parent4, "Rares", "ItemAlert.Rares");
			this.AddButton(parent4, "Uniques", "ItemAlert.Uniques");
			this.AddButton(parent4, "Currency", "ItemAlert.Currency");
			this.AddButton(parent4, "Maps", "ItemAlert.Maps");
			this.AddButton(parent4, "RGB", "ItemAlert.RGB");
			this.AddButton(parent4, "Crafting bases", "ItemAlert.Crafting");
			this.AddButton(parent4, "Skill gems", "ItemAlert.SkillGems");
			this.AddButton(parent4, "Only quality gems", "ItemAlert.QualitySkillGems");
			this.AddButton(parent4, "Play sound", "ItemAlert.PlaySound");
			BooleanButton booleanButton7 = this.AddButton(parent4, "Show text", "ItemAlert.ShowText");
			booleanButton7.AddChild(new IntPicker("Font size", 6, 30, "ItemAlert.ShowText.FontSize"));
			BooleanButton tooltip = this.CreateRootMenu("Advanced tooltips", r++, "Tooltip");
			this.AddButton(tooltip, "Item level on hover", "Tooltip.ShowItemLevel");
			this.AddButton(tooltip, "Item mods on hover", "Tooltip.ShowItemMods");
			BooleanButton parent5 = this.CreateRootMenu("Boss warnings", r++, "MonsterTracker");
			this.AddButton(parent5, "Sound warning", "MonsterTracker.PlaySound");
			BooleanButton booleanButton8 = this.AddButton(parent5, "Text warning", "MonsterTracker.ShowText");
			booleanButton8.AddChild(new IntPicker("Font size", 6, 30, "MonsterTracker.ShowText.FontSize"));
			booleanButton8.AddChild(new IntPicker("Background alpha", 0, 200, "MonsterTracker.ShowText.BgAlpha"));
			BooleanButton booleanButton9 = this.CreateRootMenu("Xph Display", r++, "XphDisplay");
			booleanButton9.AddChild(new IntPicker("Font size", 6, 30, "XphDisplay.FontSize"));
			booleanButton9.AddChild(new IntPicker("Background alpha", 0, 200, "XphDisplay.BgAlpha"));
			BooleanButton parent6 = this.CreateRootMenu("Client hacks", r++, "ClientHacks");
			this.AddButton(parent6, "Maphack", "ClientHacks.Maphack");
			this.AddButton(parent6, "Zoomhack", "ClientHacks.Zoomhack");
			this.AddButton(parent6, "Fullbright", "ClientHacks.Fullbright");
			this.AddButton(parent6, "Disable Particles", "ClientHacks.Particles");
			BooleanButton booleanButton10 = this.CreateRootMenu("Preload Alert", r++, "PreloadAlert");
			booleanButton10.AddChild(new IntPicker("Font size", 6, 30, "PreloadAlert.FontSize"));
			booleanButton10.AddChild(new IntPicker("Background alpha", 0, 200, "PreloadAlert.BgAlpha"));
			BooleanButton dps = this.CreateRootMenu("Show DPS", r++, "DpsDisplay");
			// BooleanButton closeWithGame = this.CreateRootMenu("Exit when game is closed", 8, "ExitWithGame");
		}
		private BooleanButton AddButton(BooleanButton parent, string text, string setting)
		{
			BooleanButton booleanButton = new BooleanButton(text, setting);
			parent.AddChild(booleanButton);
			return booleanButton;
		}
		private BooleanButton CreateRootMenu(string text, int yIndex, string setting)
		{
			BooleanButton booleanButton = new BooleanButton(text, setting);
			booleanButton.Bounds = new Rect(Settings.GetInt("Menu.PositionWidth"), Settings.GetInt("Menu.PositionHeight") + Settings.GetInt("Menu.Size") + yIndex * booleanButton.DesiredHeight, booleanButton.DesiredWidth, booleanButton.DesiredHeight);
			this.buttons.Add(booleanButton);
			return booleanButton;
		}
	}
}


