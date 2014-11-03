using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using NHotkey;
using NHotkey.WindowsForms;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe;

namespace PoeHUD
{
    public class PoeHotkeys
    {


        public static void SendKey1()
        {
            new InputSimulator().Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_1);
        }

        public static void SendKey2()
        {
            new InputSimulator().Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_2);
        }
        public static void SendKey3()
        {
            new InputSimulator().Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_3);
        }

        public static void SendKey4()
        {
            new InputSimulator().Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_4);
        }

        public static void SendKey5()
        {
            new InputSimulator().Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_5);
        }

        public static void SendRemaining(object sender, HotkeyEventArgs e)
        {
            new InputSimulator().Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
            Thread.Sleep(3);
            new InputSimulator().Keyboard.TextEntry("/remaining");
            Thread.Sleep(3);
            new InputSimulator().Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
            e.Handled = true;
        }

    }
}
