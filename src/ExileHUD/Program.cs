using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ExileHUD.ExileBot;
using ExileHUD.Framework;

namespace ExileHUD.ExileHUD
{
	public class Program
	{

		private static int FindPoeProcess(out Offsets offs)
		{
			offs = Offsets.Regular;
			Process process = Process.GetProcessesByName(offs.ExeName).FirstOrDefault<Process>();
			if (process != null)
			{
				return process.Id;
			}
	
			offs = Offsets.Steam;
			process = Process.GetProcessesByName(offs.ExeName).FirstOrDefault<Process>();
			return process == null ? 0 : process.Id;
		}

		[STAThread]
		public static void Main(string[] args)
		{
			Offsets offs;
			int pid = FindPoeProcess(out offs);

			if (pid == 0)
			{
				MessageBox.Show("Path of Exile is not running!");
				return;
			}

			Sounds.LoadSounds();
			if (!Settings.LoadSettings())
			{
				return;
			}
			using (Memory memory = new Memory(offs, pid))
			{
				offs.DoPatternScans(memory);
				PathOfExile pathOfExile = new PathOfExile(memory);
				pathOfExile.Update();
				OverlayRenderer overlay = null;
				AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs exceptionArgs)
				{
					if (overlay != null)
					{
						overlay.Detach();
					}
					MessageBox.Show("Program exited with message:\n " + exceptionArgs.ExceptionObject.ToString());
					Environment.Exit(1);
				};
				try
				{
					Console.WriteLine("Starting overlay");
					TransparentDXOverlay transparentDXOverlay = new TransparentDXOverlay(pathOfExile.Window, () => memory.IsInvalid());
					transparentDXOverlay.InitD3D();
					overlay = new OverlayRenderer(pathOfExile, transparentDXOverlay.RC);
					Application.Run(transparentDXOverlay);
				}
				finally
				{
					if (overlay != null)
					{
						overlay.Detach();
					}
				}
			}
		}
	}
}
