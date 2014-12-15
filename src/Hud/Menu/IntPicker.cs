using System;
using System.Drawing;
using PoeHUD.Framework;
using PoeHUD.Settings;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
	public class IntPicker : MenuItem
	{
		private int value;
		private readonly int min;
		private readonly int max;
		private readonly string text;
		private bool isHolding;
		private readonly Setting<int> setting;

		public override int Height { get { return base.Height + 5; } }

		public IntPicker(Menu.MenuSettings menuSettings, string text, SettingIntRange setting) : base(menuSettings)
		{
			this.text = text;
			this.min = setting.Min;
			this.max = setting.Max;
			this.value = setting.Value;
			this.setting = setting;
		}
		public override void Render(RenderingContext rc)
		{
			if (!this.isVisible)
			{
				return;
			}
			Color gray = Color.Gray;
			rc.AddTextWithHeight(new Vec2(base.Bounds.X + base.Bounds.W / 2, base.Bounds.Y + base.Bounds.H / 3), this.text + ": " + this.value, Color.White, 11, DrawTextFormat.VerticalCenter | DrawTextFormat.Center);
			rc.AddBox(base.Bounds, Color.Black);
			rc.AddBox(new Rect(base.Bounds.X + 1, base.Bounds.Y + 1, base.Bounds.W - 2, base.Bounds.H - 2), gray);
			rc.AddBox(new Rect(base.Bounds.X + 5, base.Bounds.Y + 3 * base.Bounds.H / 4, base.Bounds.W - 10, 5), Color.Black);
			float num = (float)(this.value - this.min) / (float)(this.max - this.min);
			int num2 = (int)((float)(base.Bounds.W - 10) * num);
			rc.AddBox(new Rect(base.Bounds.X + 5 + num2 - 2, base.Bounds.Y + 3 * base.Bounds.H / 4 - 4, 4, 8), Color.White);
		}
		private void CalcValue(int x)
		{
			int num = base.Bounds.X + 5;
			int num2 = base.Bounds.X + base.Bounds.W - 5;
			float num3 = x <= num ? 0f : (x >= num2 ? 1f : (float) (x - num)/(num2 - num));
			this.value = (int)Math.Round(this.min + num3 * (this.max - this.min));
			setting.Value = value;

		}
		protected override bool TestBounds(Vec2 pos)
		{
			return this.isHolding || base.TestBounds(pos);
		}
		protected override void HandleEvent(MouseEventID id, Vec2 pos)
		{
			if (id == MouseEventID.LeftButtonDown)
			{
				this.isHolding = true;
				return;
			}
			if (id == MouseEventID.LeftButtonUp)
			{
				this.CalcValue(pos.X);
				this.isHolding = false;
				return;
			}
			if (this.isHolding && id == MouseEventID.MouseMove)
			{
				this.CalcValue(pos.X);
			}
		}
	}
}
