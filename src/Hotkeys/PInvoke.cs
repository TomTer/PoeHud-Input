using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PoeHUD.Hotkeys
{
    class PInvoke
    {
        #region P/Invoke

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        // You can also call FindWindow(default(string), lpWindowName) or FindWindow((string)null, lpWindowName)


        public static bool BringWindowToTop(string windowName, bool wait)
        {
            int hWnd = FindWindow(windowName, wait);
            if (hWnd != 0)
            {
                return SetForegroundWindow((IntPtr)hWnd);
            }
            return false;
        }

        // THE FOLLOWING METHOD REFERENCES THE FindWindowAPI
        public static int FindWindow(string windowName, bool wait)
        {
            int hWnd = (int)FindWindow(null, windowName);
            while (wait && hWnd == 0)
            {
                System.Threading.Thread.Sleep(500);
                hWnd = (int)FindWindow(null, windowName);
            }

            return hWnd;
        }

        #endregion
    }
}
