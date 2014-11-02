using System;
using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.XpRate
{
	public class XPHRenderer : HUDPluginBase
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
			this.model.Area.OnAreaChange += this.CurrentArea_OnAreaChange;
		}
		public override void OnDisable()
		{
		}
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			this.startXp = this.model.Player.GetComponent<Player>().XP;
			this.startTime = DateTime.Now;
			this.curTimeLeftString = "--h --m --s until level up";
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.GetBool("XphDisplay") || (this.model.Player != null && this.model.Player.GetComponent<Player>().Level >= 100))
			{
				return;
			}
			if (!this.hasStarted)
			{
				this.startXp = this.model.Player.GetComponent<Player>().XP;
				this.startTime = DateTime.Now;
				this.lastCalcTime = DateTime.Now;
				this.hasStarted = true;
				return;
			}
			DateTime dtNow = DateTime.Now;
			TimeSpan delta = dtNow - this.lastCalcTime;
			
			if (delta.TotalSeconds > 1)
			{
				calculateRemainingExp(dtNow);
				this.lastCalcTime = dtNow;
			}

			int fontSize = Settings.GetInt("XphDisplay.FontSize");
			int bgAlpha = Settings.GetInt("XphDisplay.BgAlpha");

			Vec2 mapWithOffset = mountPoints[UiMountPoint.LeftOfMinimap];

			int yCursor = 0;
			Vec2 rateTextSize = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, mapWithOffset.Y), this.curDisplayString, Color.White, fontSize, DrawTextFormat.Right);
			yCursor += rateTextSize.Y;
			Vec2 remainingTextSize = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, mapWithOffset.Y + yCursor), this.curTimeLeftString, Color.White, fontSize, DrawTextFormat.Right);
			yCursor += remainingTextSize.Y;
			int thirdLine = mapWithOffset.Y + yCursor;
			Vec2 areaLevelNote = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, thirdLine), this.model.Area.CurrentArea.DisplayName, Color.White, fontSize, DrawTextFormat.Right);

			string strTimer = AreaInstance.GetTimeString(dtNow - this.model.Area.CurrentArea.TimeEntered);
			Vec2 timerSize = rc.MeasureString(strTimer, fontSize, DrawTextFormat.Left);
			yCursor += areaLevelNote.Y;

			Rect clientRect = model.Internal.IngameState.IngameUi.Minimap.SmallMinimap.GetClientRect();
			int textWidth = Math.Max( Math.Max(rateTextSize.X, remainingTextSize.X), areaLevelNote.X + timerSize.X + 20 ) + 10;
			int width = Math.Max(textWidth, Math.Max(clientRect.W, 0/*this.overlay.PreloadAlert.Bounds.W*/));
			Rect rect = new Rect(mapWithOffset.X - width + 5, mapWithOffset.Y - 5, width, yCursor + 10);
			this.Bounds = rect;
			
			rc.AddTextWithHeight(new Vec2(rect.X + 5, mapWithOffset.Y), dtNow.ToShortTimeString(), Color.White, fontSize, DrawTextFormat.Left);
			rc.AddTextWithHeight(new Vec2(rect.X + 5, thirdLine), strTimer, Color.White, fontSize, DrawTextFormat.Left);
			
			rc.AddBox(rect, Color.FromArgb(bgAlpha, 1, 1, 1));

			mountPoints[UiMountPoint.LeftOfMinimap] = new Vec2(mapWithOffset.X, mapWithOffset.Y + 5 + rect.H);
		}

		private void calculateRemainingExp(DateTime dtNow)
		{
			long currentExp = model.Player.GetComponent<Player>().XP - this.startXp;
			float expRate = (float) (currentExp/(dtNow - this.startTime).TotalHours);
			this.curDisplayString = (double) expRate > 1000000.0
				? (expRate/1000000.0).ToString("0.00") + "M XP/h"
				: ((double) expRate > 1000.0 ? (expRate/1000.0).ToString("0.00") + "K XP/h"
					: expRate.ToString("0.00") + " XP/h");
			int level = this.model.Player.GetComponent<Player>().Level;
			if (level < 0 || level + 1 >= Constants.PlayerXpLevels.Length)
			{
				return;
			}
			ulong expRemaining = Constants.PlayerXpLevels[level + 1] - (ulong) this.model.Player.GetComponent<Player>().XP;
			if (expRate > 1f)
			{
				int num4 = (int) (expRemaining/expRate*3600f);
				int num5 = num4/60;
				int num6 = num5/60;
				this.curTimeLeftString = string.Concat(num6, "h ", num5%60, "m ", num4%60, "s until level up");
			}
		}
	}
}
