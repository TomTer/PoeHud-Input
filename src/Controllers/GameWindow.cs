using System;
using System.Diagnostics;
using PoeHUD.Framework;

namespace PoeHUD.Controllers
{
	public class GameWindow
	{
		public readonly IntPtr HWnd;
		public Process Process
		{
			get;
			private set;
		}
		public GameWindow(Process process)
		{
			this.Process = process;
			this.HWnd = process.MainWindowHandle;
		}
		public Rect ClientRect()
		{
			Rect result;
			Imports.GetClientRect(this.HWnd, out result);
			Vec2 vec = this.ClientToScreen(Vec2.Empty);
			result.X = vec.X;
			result.Y = vec.Y;
			return result;
		}
		public Vec2 ClientToScreen(Vec2 v)
		{
			Imports.ClientToScreen(this.HWnd, ref v);
			return v;
		}
		public Vec2 ScreenToClient(Vec2 v)
		{
			Imports.ScreenToClient(this.HWnd, ref v);
			return v;
		}
		public bool IsForeground()
		{
			return Imports.GetForegroundWindow() == this.HWnd;
		}
	}
}
