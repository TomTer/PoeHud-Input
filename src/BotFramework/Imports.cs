using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace BotFramework
{
	public class Imports
	{
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(uint access, bool inheritHandle, int processId);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int CloseHandle(IntPtr hObject);
		[DllImport("kernel32.dll")]
		public static extern void ReadProcessMemory(IntPtr hProcess, IntPtr baseAddress, byte[] buffer, int size, int bytesRead);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);
		[DllImport("user32.dll")]
		public static extern uint MapVirtualKey(uint uCode, uint uMapType);
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern short VkKeyScan(char ch);
		public static Keys CharToKey(char ch)
		{
			return (Keys)Imports.VkKeyScan(ch);
		}
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref Vec2 lpPoint);
		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);
		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);
		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref Vec2 lpPoint);
		[DllImport("user32.dll")]
		public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
		[DllImport("kernel32.dll")]
		public static extern uint GetTickCount();
		[DllImport("user32.dll")]
		public static extern ushort GetAsyncKeyState(int vKey);
		public static bool IsKeyDown(Keys vKey)
		{
			return 0 != (Imports.GetAsyncKeyState((int)vKey) & 32768);
		}
	}
}
