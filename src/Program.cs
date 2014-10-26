using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe;

namespace PoeHUD.Hud
{
	public class Program
	{

		private static int FindPoeProcess(out Offsets offs)
		{
			var clients = Process.GetProcessesByName(Offsets.Regular.ExeName).Select(p => Tuple.Create(p, Offsets.Regular)).ToList();
			clients.AddRange(Process.GetProcessesByName(Offsets.Steam.ExeName).Select(p => Tuple.Create(p, Offsets.Steam)));
			int ixChosen = clients.Count > 1 ? chooseSingleProcess(clients) : 0;
			if (clients.Count > 0 && ixChosen >= 0)
			{
				offs = clients[ixChosen].Item2;
				return clients[ixChosen].Item1.Id;
			} else {
				offs = null;
				return 0;
			}
		}

		private static int chooseSingleProcess(List<Tuple<Process, Offsets>> clients)
		{
			String o1 = String.Format("Yes - process #{0}, started at {1}", clients[0].Item1.Id, clients[0].Item1.StartTime.ToLongTimeString());
			String o2 = String.Format("No - process #{0}, started at {1}", clients[1].Item1.Id, clients[1].Item1.StartTime.ToLongTimeString());
			const string o3 = "Cancel - quit this application";
			var answer = MessageBox.Show(null, String.Join(Environment.NewLine, o1, o2, o3), "Choose a PoE instance to attach to", MessageBoxButtons.YesNoCancel);
			return answer == DialogResult.Cancel ? -1 : answer == DialogResult.Yes ? 0 : 1;
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


			using (Memory memory = new Memory(offs, pid))
			{
				offs.DoPatternScans(memory);
				GameController gameController = new GameController(memory);
				gameController.RefreshState();
				try
				{
					Console.WriteLine("Starting overlay");
					TransparentDXOverlay transparentDXOverlay = new TransparentDXOverlay(gameController.Window, () => memory.IsInvalid());
					transparentDXOverlay.InitD3D();
					overlay = new OverlayRenderer(gameController, transparentDXOverlay.RC);
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
