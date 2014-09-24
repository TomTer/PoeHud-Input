using BotFramework;
using ExileBot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
namespace ExileHUD
{
	public class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Process process = Process.GetProcessesByName("PathOfExile").FirstOrDefault<Process>();
			if (process == null)
			{
				MessageBox.Show("Path of Exile is not running!");
				return;
			}
			Sounds.LoadSounds();
			if (!Settings.LoadSettings())
			{
				return;
			}
			using (Memory memory = new Memory(process.Id))
			{
				Offsets.DoPatternScans(memory);
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
					TransparentDXOverlay transparentDXOverlay = new TransparentDXOverlay(pathOfExile.Window);
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
