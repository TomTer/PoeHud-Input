using System.Drawing;
using ExileHUD.Framework;
using SlimDX.Direct3D9;

namespace ExileHUD.ExileHUD
{
	public class BooleanButton : MenuItem
	{
		private string text;
		private string settingName;
		public bool isEnabled;
		public override int DesiredHeight
		{
			get
			{
				return 25;
			}
		}
		public override int DesiredWidth
		{
			get
			{
				return 210;
			}
		}
		public BooleanButton(string text, string settingName)
		{
			this.text = text;
			this.settingName = settingName;
			this.isEnabled = Settings.GetBool(settingName);
		}
		public override void Render(RenderingContext rc)
		{
			if (!this.isVisible)
			{
				return;
			}
			Color color = this.isEnabled ? Color.Green : Color.Red;
			rc.AddTextWithHeight(new Vec2(base.Bounds.X + base.Bounds.W / 2, base.Bounds.Y + base.Bounds.H / 2), this.text, Color.White, 12, DrawTextFormat.VerticalCenter | DrawTextFormat.Center);
			rc.AddBox(base.Bounds, Color.Black);
			rc.AddBox(new Rect(base.Bounds.X + 1, base.Bounds.Y + 1, base.Bounds.W - 2, base.Bounds.H - 2), color);
			if (this.children.Count > 0)
			{
				int num = (int)((float)(base.Bounds.W - 2) * 0.05f);
				int num2 = (base.Bounds.H - 2) / 2;
				rc.AddTexture("menu_submenu.png", new Rect(base.Bounds.X + base.Bounds.W - 1 - num, base.Bounds.Y + 1 + num2 - num2 / 2, num, num2));
			}
			foreach (MenuItem current in this.children)
			{
				current.Render(rc);
			}
		}
		protected override void HandleEvent(MouseEventID id, Vec2 pos)
		{
			if (id == MouseEventID.LeftButtonDown)
			{
				this.isEnabled = !this.isEnabled;
			}
			Settings.SetBool(this.settingName, this.isEnabled);
		}
	}
}
