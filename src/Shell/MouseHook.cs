using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PoeHUD.Framework;

namespace PoeHUD.Shell
{
	public class MouseHook : IDisposable
	{
		private delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
		public delegate bool MouseEvent(MouseEventID eventId, int x, int y);
		private struct POINT
#pragma warning disable 649
		{
			public int x;
			public int y;
		}
		private struct MSLLHOOKSTRUCT
		{
			public MouseHook.POINT pt;
			public uint mouseData;
			public uint flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}
#pragma warning restore 649
		private int mousehookId;
		private MouseHook.MouseEvent mouseEvent;
		private MouseHook.HookProc LLHookProc;
		public MouseHook(MouseHook.MouseEvent callback)
		{
			this.mouseEvent = callback;
			this.LLHookProc = new MouseHook.HookProc(this.LLMouseProc);
			this.mousehookId = MouseHook.SetWindowsHookEx(14, this.LLHookProc, IntPtr.Zero, 0);
		}
		public void Dispose()
		{
			MouseHook.UnhookWindowsHookEx(this.mousehookId);
		}
		private int LLMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (Imports.IsKeyDown(Keys.F12))
			{
				return MouseHook.CallNextHookEx(this.mousehookId, nCode, wParam, lParam);
			}
			MouseHook.MSLLHOOKSTRUCT mSLLHOOKSTRUCT = (MouseHook.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MouseHook.MSLLHOOKSTRUCT));
			try
			{
				if (this.mouseEvent((MouseEventID)((int)wParam), mSLLHOOKSTRUCT.pt.x, mSLLHOOKSTRUCT.pt.y))
				{
					return 1;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in mousehook " + ex.Message);
			}
			return MouseHook.CallNextHookEx(this.mousehookId, nCode, wParam, lParam);
		}
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		private static extern int SetWindowsHookEx(int idHook, MouseHook.HookProc lpfn, IntPtr hInstance, int threadId);
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		private static extern bool UnhookWindowsHookEx(int idHook);
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr LoadLibrary(string fileName);
		[DllImport("user32.dll")]
		private static extern int GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
	}
}
