using System.Collections.Generic;
using System.Drawing;
using ExileHUD.Framework;
using SlimDX.Direct3D9;

namespace ExileHUD.ExileHUD
{
	public class Menu : HUDPlugin
	{
		private const int ButtonWidth = 256;
		private const int ButtonHeight = 32;
		private MouseHook hook;
		private List<BooleanButton> buttons;
		private BooleanButton currentHover;
		private Rect bounds;
		private bool menuVisible;
		public override void OnEnable()
		{
			this.bounds = new Rect(0, 75, 256, 32);
			this.CreateButtons();
			this.hook = new MouseHook(new MouseHook.MouseEvent(this.OnMouseEvent));
		}
		public override void OnDisable()
		{
			this.hook.Dispose();
		}
		public override void Render(RenderingContext rc)
		{
			int alpha = this.menuVisible ? 255 : 50;
			rc.AddBox(this.bounds, Color.FromArgb(alpha, Color.Gray));
			rc.AddTextWithHeight(new Vec2(128, 16), "Menu", Color.White, 16, DrawTextFormat.VerticalCenter | DrawTextFormat.Center);
			foreach (BooleanButton current in this.buttons)
			{
				current.Render(rc);
			}
		}
		private bool OnMouseEvent(MouseEventID id, int x, int y)
		{
			if (Settings.GetBool("Window.RequireForeground") && !this.poe.Window.IsForeground())
			{
				return false;
			}
			Vec2 vec = this.poe.Window.ScreenToClient(new Vec2(x, y));
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
			this.buttons = new List<BooleanButton>();
			BooleanButton parent = this.CreateRootMenu("Health bars", 0, "Healthbars");
			BooleanButton booleanButton = this.AddButton(parent, "Players", "Healthbars.Players");
			BooleanButton parent2 = this.AddButton(parent, "Enemies", "Healthbars.Enemies");
			BooleanButton booleanButton2 = this.AddButton(parent, "Minions", "Healthbars.Minions");
			this.AddButton(parent, "Show ES", "Healthbars.ShowES");
			this.AddButton(parent, "Show in town", "Healthbars.ShowInTown");
			booleanButton.AddChild(new IntPicker("Width", 50, 250, "Healthbars.Players.Width"));
			booleanButton.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Players.Height"));
			booleanButton2.AddChild(new IntPicker("Width", 50, 250, "Healthbars.Minions.Width"));
			booleanButton2.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Minions.Height"));
			BooleanButton booleanButton3 = this.AddButton(parent2, "White", "Healthbars.Enemies.Normal");
			booleanButton3.AddChild(new IntPicker("Width", 50, 250, "Healthbars.Enemies.Normal.Width"));
			booleanButton3.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Normal.Height"));
			BooleanButton booleanButton4 = this.AddButton(parent2, "Magic", "Healthbars.Enemies.Magic");
			booleanButton4.AddChild(new IntPicker("Width", 50, 250, "Healthbars.Enemies.Magic.Width"));
			booleanButton4.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Magic.Height"));
			BooleanButton booleanButton5 = this.AddButton(parent2, "Rare", "Healthbars.Enemies.Rare");
			booleanButton5.AddChild(new IntPicker("Width", 50, 250, "Healthbars.Enemies.Rare.Width"));
			booleanButton5.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Rare.Height"));
			BooleanButton booleanButton6 = this.AddButton(parent2, "Uniques", "Healthbars.Enemies.Unique");
			booleanButton6.AddChild(new IntPicker("Width", 50, 250, "Healthbars.Enemies.Unique.Width"));
			booleanButton6.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Unique.Height"));
			BooleanButton parent3 = this.CreateRootMenu("Minimap icons", 1, "MinimapIcons");
			this.AddButton(parent3, "Monsters", "MinimapIcons.Monsters");
			this.AddButton(parent3, "Minions", "MinimapIcons.Minions");
			this.AddButton(parent3, "Strongboxes", "MinimapIcons.Strongboxes");
			this.AddButton(parent3, "Chests", "MinimapIcons.Chests");
			this.AddButton(parent3, "Alert items", "MinimapIcons.AlertedItems");
			BooleanButton parent4 = this.CreateRootMenu("Item alert", 2, "ItemAlert");
			this.AddButton(parent4, "Rares", "ItemAlert.Rares");
			this.AddButton(parent4, "Uniques", "ItemAlert.Uniques");
			this.AddButton(parent4, "Currency", "ItemAlert.Currency");
			this.AddButton(parent4, "Maps", "ItemAlert.Maps");
			this.AddButton(parent4, "RGB", "ItemAlert.RGB");
			this.AddButton(parent4, "Crafting bases", "ItemAlert.Crafting");
			this.AddButton(parent4, "Skill gems", "ItemAlert.SkillGems");
			this.AddButton(parent4, "Play sound", "ItemAlert.PlaySound");
			BooleanButton booleanButton7 = this.AddButton(parent4, "Show text", "ItemAlert.ShowText");
			booleanButton7.AddChild(new IntPicker("Font size", 6, 32, "ItemAlert.ShowText.FontSize"));
			BooleanButton tooltip = this.CreateRootMenu("Advanced tooltips", 3, "Tooltip");
			this.AddButton(tooltip, "Item level on hover", "Tooltip.ShowItemLevel");
			this.AddButton(tooltip, "Item mods on hover", "Tooltip.ShowItemMods");
			BooleanButton parent5 = this.CreateRootMenu("Boss warnings", 4, "DangerAlert");
			this.AddButton(parent5, "Sound warning", "DangerAlert.PlaySound");
			BooleanButton booleanButton8 = this.AddButton(parent5, "Text warning", "DangerAlert.ShowText");
			booleanButton8.AddChild(new IntPicker("Font size", 6, 32, "DangerAlert.ShowText.FontSize"));
			booleanButton8.AddChild(new IntPicker("Background alpha", 0, 255, "DangerAlert.ShowText.BgAlpha"));
			BooleanButton booleanButton9 = this.CreateRootMenu("Xph Display", 5, "XphDisplay");
			booleanButton9.AddChild(new IntPicker("Font size", 6, 32, "XphDisplay.FontSize"));
			booleanButton9.AddChild(new IntPicker("Background alpha", 0, 255, "XphDisplay.BgAlpha"));
			BooleanButton parent6 = this.CreateRootMenu("Client hacks", 6, "ClientHacks");
			this.AddButton(parent6, "Maphack", "ClientHacks.Maphack");
			this.AddButton(parent6, "Zoomhack", "ClientHacks.Zoomhack");
			this.AddButton(parent6, "Fullbright", "ClientHacks.Fullbright");
			BooleanButton booleanButton10 = this.CreateRootMenu("Preload Alert", 7, "PreloadAlert");
			booleanButton10.AddChild(new IntPicker("Font size", 6, 32, "PreloadAlert.FontSize"));
			booleanButton10.AddChild(new IntPicker("Background alpha", 0, 255, "PreloadAlert.BgAlpha"));
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
			booleanButton.Bounds = new Rect(0, 105 + yIndex * booleanButton.DesiredHeight, booleanButton.DesiredWidth, booleanButton.DesiredHeight);
			this.buttons.Add(booleanButton);
			return booleanButton;
		}
	}
}
