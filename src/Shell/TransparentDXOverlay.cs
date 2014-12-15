using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud;
using PoeHUD.Settings;
using SlimDX;
using SlimDX.Direct3D9;

namespace PoeHUD.Shell
{
	public class TransparentDxOverlay : Form
	{
		public bool ExitRequested;

		internal struct Margins
		{
			public int Left;
			public int Right;
			public int Top;
			public int Bottom;
		}
		public const int GWL_EXSTYLE = -20;
		public const int WS_EX_LAYERED = 524288;
		public const int WS_EX_TRANSPARENT = 32;
		public const int LWA_ALPHA = 2;
		public const int LWA_COLORKEY = 1;
		private Device dx;
		private Direct3D d3d;
		
		private readonly GameWindow window;
		private Thread dxThread;
		private readonly SettingsRoot Settings;

		private Margins marg;

		public RenderingContext RC
		{
			get;
			private set;
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.Control;
			base.ClientSize = new Size(594, 448);
			base.Name = "TransparentDxOverlay";
			base.TransparencyKey = Color.Transparent;
			base.ResumeLayout(false);
		}

		public TransparentDxOverlay(GameWindow window, SettingsRoot settings, EventScheduler es)
		{
			Settings = settings;
			InitializeComponent();
			BackColor = Color.Black;
			this.window = window;
			base.ShowIcon = false;
			base.FormClosing += new FormClosingEventHandler(TransparentDXOverlay_FormClosing);
			string textForTitle = Settings.Global.WindowName;
			Text = String.IsNullOrWhiteSpace(textForTitle) ? "ExileHUD" : textForTitle;
			base.FormBorderStyle = FormBorderStyle.None;
			base.Load += TransparentDXOverlay_Load;
			overseer = es;
		}

		private readonly EventScheduler overseer;




		private void TransparentDXOverlay_Load(object sender, EventArgs e)
		{
			uint curExStyle = GetWindowLong(base.Handle, GWL_EXSTYLE);
			int swlRes = SetWindowLong(base.Handle, GWL_EXSTYLE, (IntPtr)(curExStyle| WS_EX_LAYERED | WS_EX_TRANSPARENT));
			Console.WriteLine("SetWindowLong returned: " + swlRes + "; error code = " + Marshal.GetLastWin32Error());
			SetLayeredWindowAttributes(base.Handle, 0u, 255, LWA_ALPHA);

			Rect rect = window.ClientRect();
			marg.Left = rect.X;
			marg.Top = rect.Y;
			marg.Right = rect.X + rect.W;
			marg.Bottom = rect.Y + rect.H;
			IntPtr intPtr = DwmExtendFrameIntoClientArea(base.Handle, ref marg);
			Console.WriteLine("DwmExtendFrameIntoClientArea: " + intPtr);
			
			base.Bounds = new Rectangle(rect.X, rect.Y, rect.W, rect.H);

			dxThread = new Thread(DxLoop) {IsBackground = true};
			dxThread.Start();
			overseer.StartWatching(this);
		}

		private readonly PresentParameters presentParameters = new PresentParameters
		{
			Windowed = true,
			SwapEffect = SwapEffect.Discard,
			BackBufferFormat = Format.A8R8G8B8,
			PresentationInterval = PresentInterval.One
		};

		public void InitD3D()
		{
			Rect rect = window.ClientRect();

			presentParameters.BackBufferWidth = rect.W;
			presentParameters.BackBufferHeight = rect.H;

			dx = new Device(d3d = new Direct3D(), 0, DeviceType.Hardware, base.Handle, CreateFlags.Multithreaded | CreateFlags.HardwareVertexProcessing, new[] { presentParameters });
			RC = new RenderingContext(dx, window);
			Configuration.AddResultWatch(ResultCode.DeviceLost, ResultWatchFlags.AlwaysIgnore);
		}
		private void TransparentDXOverlay_FormClosing(object sender, FormClosingEventArgs e)
		{
			ExitRequested = true;
			dxThread.Join();
			d3d.Dispose();
			dx.Dispose();
		}
		public void OnDeactivate()
		{
			base.TopMost = true;
			base.BringToFront();
		}
		public void ResetDirectx()
		{
			if (dx.Disposed) return;

			RC.OnLostDevice();
			while (dx.TestCooperativeLevel() != ResultCode.DeviceNotReset)
				Thread.Sleep(50);

			Rect rect = window.ClientRect();

			presentParameters.BackBufferWidth = rect.W;
			presentParameters.BackBufferHeight = rect.H;

			dx.Reset(new[] { presentParameters });
			RC.OnResetDevice();
		}

		public void DxLoop()
		{
			do {
				dx.Clear(ClearFlags.Target, Color.FromArgb(0, 0, 0, 0), 1f, 0);
				dx.SetRenderState(RenderState.ZEnable, false);
				dx.SetRenderState(RenderState.Lighting, false);
				dx.SetRenderState(RenderState.AlphaBlendEnable, true);
				dx.SetRenderState(RenderState.CullMode, Cull.None);
				dx.BeginScene();
				RC.RenderFrame();
				dx.EndScene();

				if (!ExitRequested && dx.Present() == ResultCode.DeviceLost ) // recover dx
					base.Invoke(new EventScheduler.HatedCsharpDelegate(ResetDirectx));
			} while (!ExitRequested);
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
		[DllImport("dwmapi.dll")]
		private static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
	}
}
