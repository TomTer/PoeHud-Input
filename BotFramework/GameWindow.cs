using System;
using System.Diagnostics;
namespace BotFramework
{
	public class GameWindow
	{
		private IntPtr handle;
		public Process Process
		{
			get;
			private set;
		}
		public GameWindow(Process process)
		{
			this.Process = process;
			this.handle = process.MainWindowHandle;
		}
		public Rect ClientRect()
		{
			Rect result = default(Rect);
			Imports.GetClientRect(this.handle, out result);
			Vec2 vec = this.ClientToScreen(Vec2.Empty);
			result.X = vec.X;
			result.Y = vec.Y;
			return result;
		}
		public Vec2 ClientToScreen(Vec2 v)
		{
			Imports.ClientToScreen(this.handle, ref v);
			return v;
		}
		public Vec2 ScreenToClient(Vec2 v)
		{
			Imports.ScreenToClient(this.handle, ref v);
			return v;
		}
		public bool IsForeground()
		{
			return Imports.GetForegroundWindow() == this.handle;
		}
	}
}
