using System;
using System.Drawing;
using ExileHUD.Framework;
using SlimDX.Direct3D9;

namespace ExileHUD.ExileHUD
{
	public class IntPicker : MenuItem
	{
		private int value;
		private int min;
		private int max;
		private string text;
		private bool isHolding;
		private string settingName;
		public override int DesiredWidth
		{
			get
			{
				return 210;
			}
		}
		public override int DesiredHeight
		{
			get
			{
				return 30;
			}
		}
		public IntPicker(string text, int min, int max, string settingName)
		{
			this.text = text;
			this.min = min;
			this.max = max;
			this.value = Settings.GetInt(settingName);
			this.settingName = settingName;
		}
		public override void Render(RenderingContext rc)
		{
			if (!this.isVisible)
			{
				return;
			}
			Color gray = Color.Gray;
			rc.AddTextWithHeight(new Vec2(base.Bounds.X + base.Bounds.W / 2, base.Bounds.Y + base.Bounds.H / 4), this.text + ": " + this.value, Color.White, 11, DrawTextFormat.VerticalCenter | DrawTextFormat.Center);
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
			int num2 = num + base.Bounds.W - 10;
			float num3;
			if (x <= num)
			{
				num3 = 0f;
			}
			else
			{
				if (x >= num2)
				{
					num3 = 1f;
				}
				else
				{
					num3 = (float)(x - num) / (float)(num2 - num);
				}
			}
			this.value = (int)Math.Round((double)((float)this.min + num3 * (float)(this.max - this.min)));
			Settings.SetInt(this.settingName, this.value);
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
