using System;
using System.Drawing;
using ExileHUD.EntityComponents;
using ExileHUD.ExileBot;
using ExileHUD.Framework;
using SlimDX.Direct3D9;

namespace ExileHUD.ExileHUD
{
	public class XPHRenderer : HUDPlugin
	{
		private long startXp;
		private DateTime startTime;
		private DateTime lastCalcTime;
		private bool hasStarted;
		private string curDisplayString = "0.00 XP/h";
		private string curTimeLeftString = "--h --m --s until level up";
		private Rect bounds = new Rect(0, 0, 0, 0);
		public Rect Bounds
		{
			get
			{
				if (!Settings.GetBool("XphDisplay"))
				{
					return new Rect(0, 0, 0, 0);
				}
				return this.bounds;
			}
			private set
			{
				this.bounds = value;
			}
		}
		public override void OnEnable()
		{
			this.poe.CurrentArea.OnAreaChange += new AreaChangeEvent(this.CurrentArea_OnAreaChange);
		}
		public override void OnDisable()
		{
		}
		private void CurrentArea_OnAreaChange(Area area)
		{
			this.startXp = this.poe.Player.GetComponent<Player>().XP;
			this.startTime = DateTime.Now;
			this.curTimeLeftString = "--h --m --s until level up";
		}
		public override void Render(RenderingContext rc)
		{
			if (!Settings.GetBool("XphDisplay") || (this.poe.Player != null && this.poe.Player.GetComponent<Player>().Level >= 100))
			{
				return;
			}
			if (!this.hasStarted)
			{
				this.startXp = this.poe.Player.GetComponent<Player>().XP;
				this.startTime = DateTime.Now;
				this.lastCalcTime = DateTime.Now;
				this.hasStarted = true;
				return;
			}
			DateTime dtNow = DateTime.Now;
			if ((dtNow - this.lastCalcTime).TotalSeconds > 1.0)
			{
				long currentExp = this.poe.Player.GetComponent<Player>().XP - this.startXp;
				float expRate = (float)((double)currentExp / (dtNow - this.startTime).TotalHours);
				if ((double)expRate > 1000000.0)
				{
					this.curDisplayString = ((double)expRate / 1000000.0).ToString("0.00") + "M XP/h";
				}
				else
				{
					if ((double)expRate > 1000.0)
					{
						this.curDisplayString = ((double)expRate / 1000.0).ToString("0.00") + "K XP/h";
					}
					else
					{
						this.curDisplayString = expRate.ToString("0.00") + " XP/h";
					}
				}
				int level = this.poe.Player.GetComponent<Player>().Level;
				if (level + 1 >= Constants.PlayerXpLevels.Length)
				{
					return;
				}
				ulong expRemaining = (ulong)Constants.PlayerXpLevels[level + 1] - (ulong)this.poe.Player.GetComponent<Player>().XP;
				if (expRate > 1f)
				{
					int num4 = (int)((float)expRemaining / expRate * 3600f);
					int num5 = num4 / 60;
					int num6 = num5 / 60;
					this.curTimeLeftString = string.Concat(num6, "h ", num5 % 60, "m ", num4 % 60, "s until level up" );
				}
				this.lastCalcTime = dtNow;
			}

			int fontSize = Settings.GetInt("XphDisplay.FontSize");
			int bgAlpha = Settings.GetInt("XphDisplay.BgAlpha");

			Rect clientRect = this.poe.Internal.IngameState.IngameUi.Minimap.SmallMinimap.GetClientRect();
			Vec2 mapWithOffset = new Vec2(clientRect.X - 10, clientRect.Y + 5);
			int num7 = 0;
			Vec2 vec2 = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, mapWithOffset.Y), this.curDisplayString, Color.White, fontSize, DrawTextFormat.Right);
			num7 += vec2.Y;
			Vec2 vec3 = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, mapWithOffset.Y + num7), this.curTimeLeftString, Color.White, fontSize, DrawTextFormat.Right);
			num7 += vec3.Y;
			int val = Math.Max(vec2.X, vec3.X) + 10;
			int num8 = Math.Max(val, Math.Max(clientRect.W, this.overlay.PreloadAlert.Bounds.W));
			Rect rect = new Rect(mapWithOffset.X - num8 + 5, mapWithOffset.Y - 5, num8, num7 + 10);
			this.Bounds = rect;

			rc.AddTextWithHeight(new Vec2(rect.X + 5, mapWithOffset.Y), dtNow.ToShortTimeString(), Color.White, fontSize, DrawTextFormat.Left);
			rc.AddBox(rect, Color.FromArgb(bgAlpha, 1, 1, 1));
		}
	}
}
