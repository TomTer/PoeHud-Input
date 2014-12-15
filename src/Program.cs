using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe;
using PoeHUD.Settings;
using PoeHUD.Shell;

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

			Sounds.PreLoadCommonSounds();

			OverlayRenderer overlay = null;
			AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs exceptionArgs)
			{
				if (overlay != null)
					overlay.Detach();
				MessageBox.Show("Program exited with message:\n " + exceptionArgs.ExceptionObject.ToString());
				Environment.Exit(1);
			};

			SettingsRoot settings = new SettingsRoot("config\\new_settings.txt");
			settings.ReadFromFile(); // 1st read for globals and menu

			using (Memory memory = new Memory(offs, pid))
			{
				offs.DoPatternScans(memory);
				GameController gameController = new GameController(memory);
				gameController.RefreshState();
				try
				{
					Console.WriteLine("Starting overlay");
					EventScheduler es = new EventScheduler(gameController);
					settings.SetObserver((o,s) => es.RequestSave(settings));

					TransparentDxOverlay transparentDXOverlay = new TransparentDxOverlay(gameController.Window, settings, es);
					transparentDXOverlay.InitD3D();
					
					overlay = new OverlayRenderer(gameController, settings, transparentDXOverlay.RC);
					transparentDXOverlay.KeyPress += overlay.KeyPressOnForm;
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

	public class EventScheduler
	{
		private TransparentDxOverlay form;

		public readonly GameController gameController;
		private const int TICK_PERIOD = 500; //ms

		public EventScheduler(GameController gameController) {
			this.gameController = gameController;
		}


		private const int CntTasks = 1;
		private readonly int[] TasksDelays = new int[CntTasks];
		private readonly Action[] TaskExecutors = new Action[] { null };


		public delegate void HatedCsharpDelegate();
		private void CheckGameStillRunningLoop()
		{
			HatedCsharpDelegate deactivate = new HatedCsharpDelegate(form.OnDeactivate);
			while (!form.ExitRequested && !gameController.Memory.IsInvalid())
			{
				Thread.Sleep(TICK_PERIOD);
				for (int i = 0; i < CntTasks; i++) {
					if (TasksDelays[i] == 0 && TaskExecutors[i] != null) TaskExecutors[i]();
					if (TasksDelays[i] >= 0) TasksDelays[i]--;
				}

				bool gameIsFg = gameController.Window.IsForeground();
				if (!gameIsFg)
					continue;

				if (!form.TopMost)
					form.Invoke(deactivate);

			}
			if (!form.ExitRequested)
				form.Invoke(new HatedCsharpDelegate(form.Close));
		}

		internal void StartWatching(TransparentDxOverlay transparentDXOverlay)
		{
			form = transparentDXOverlay;
			var th = new Thread(CheckGameStillRunningLoop) { IsBackground = true };
			th.Start();
		}


		public void RequestSave(SettingsRoot settings) {
			TaskExecutors[0] = settings.SaveSettings;
			TasksDelays[0] = 2;
		}
	}
}
