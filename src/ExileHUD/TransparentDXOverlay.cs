using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ExileHUD.Framework;
using SlimDX;
using SlimDX.Direct3D9;

namespace ExileHUD.ExileHUD
{
	public class TransparentDXOverlay : Form
	{
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
		private IContainer components;
		private Device dx;
		private GameWindow window;
		private Thread dxThread;
		private bool wantsExit;
		public static IntPtr CurrentHandle;
		private TransparentDXOverlay.Margins marg;
		public RenderingContext RC
		{
			get;
			private set;
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Control;
			base.ClientSize = new Size(594, 448);
			base.Name = "TransparentDXOverlay";
			this.Text = "ExileHUD";
			base.TopMost = true;
			base.TransparencyKey = Color.Transparent;
			base.Deactivate += new EventHandler(this.TransparentDXOverlay_Deactivate);
			base.ResumeLayout(false);
		}
		public TransparentDXOverlay(GameWindow window)
		{
			this.InitializeComponent();
			this.BackColor = Color.Black;
			this.window = window;
			base.ShowIcon = false;
			base.TopMost = true;
			base.FormClosing += new FormClosingEventHandler(this.TransparentDXOverlay_FormClosing);
			this.Text = Settings.GetString("Window.Name");
			base.FormBorderStyle = FormBorderStyle.None;
			base.Load += new EventHandler(this.TransparentDXOverlay_Load);
		}
		private void TransparentDXOverlay_Load(object sender, EventArgs e)
		{
			Console.WriteLine("SetWindowLong: " + TransparentDXOverlay.SetWindowLong(base.Handle, -20, (IntPtr)((long)((ulong)(TransparentDXOverlay.GetWindowLong(base.Handle, -20) | 524288u | 32u)))));
			Console.WriteLine("SetWindowLong error: " + Marshal.GetLastWin32Error());
			TransparentDXOverlay.SetLayeredWindowAttributes(base.Handle, 0u, 255, 2u);
			Rect rect = this.window.ClientRect();
			this.marg.Left = rect.X;
			this.marg.Top = rect.Y;
			this.marg.Right = rect.X + rect.W;
			this.marg.Bottom = rect.Y + rect.H;
			IntPtr intPtr = TransparentDXOverlay.DwmExtendFrameIntoClientArea(base.Handle, ref this.marg);
			Console.WriteLine("DwmExtendFrameIntoClientArea: " + intPtr);
			base.Bounds = new Rectangle(rect.X, rect.Y, rect.W, rect.H);
			TransparentDXOverlay.CurrentHandle = base.Handle;
			this.dxThread = new Thread(new ThreadStart(this.DxLoop));
			this.dxThread.IsBackground = true;
			this.dxThread.Start();
		}
		public void InitD3D()
		{
			Rect rect = this.window.ClientRect();
			PresentParameters presentParameters = new PresentParameters();
			presentParameters.Windowed = true;
			presentParameters.SwapEffect = SwapEffect.Discard;
			presentParameters.BackBufferFormat = Format.A8R8G8B8;
			presentParameters.BackBufferWidth = rect.W;
			presentParameters.BackBufferHeight = rect.H;
			presentParameters.PresentationInterval = PresentInterval.One;
			this.dx = new Device(new Direct3D(), 0, DeviceType.Hardware, base.Handle, CreateFlags.Multithreaded | CreateFlags.HardwareVertexProcessing, new PresentParameters[]
			{
				presentParameters
			});
			this.RC = new RenderingContext(this.dx, this.window);
			Configuration.AddResultWatch(ResultCode.DeviceLost, ResultWatchFlags.AlwaysIgnore);
		}
		private void TransparentDXOverlay_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.wantsExit = true;
			this.dxThread.Join();
			this.dx.Dispose();
		}
		private void TransparentDXOverlay_Deactivate(object sender, EventArgs e)
		{
			base.TopMost = true;
			base.BringToFront();
		}
		public void ResetDirectx()
		{
			if (!this.dx.Disposed)
			{
				this.RC.OnLostDevice();
				while (this.dx.TestCooperativeLevel() != ResultCode.DeviceNotReset)
				{
					Thread.Sleep(10);
				}
				Rect rect = this.window.ClientRect();
				PresentParameters presentParameters = new PresentParameters();
				presentParameters.Windowed = true;
				presentParameters.SwapEffect = SwapEffect.Discard;
				presentParameters.BackBufferFormat = Format.A8R8G8B8;
				presentParameters.BackBufferWidth = rect.W;
				presentParameters.BackBufferHeight = rect.H;
				presentParameters.PresentationInterval = PresentInterval.One;
				this.dx.Reset(new PresentParameters[]
				{
					presentParameters
				});
				this.RC.OnResetDevice();
				this.dxThread = new Thread(new ThreadStart(this.DxLoop));
				this.dxThread.IsBackground = true;
				this.dxThread.Start();
			}
		}
		public void DxLoop()
		{
			while (!this.wantsExit)
			{
				this.dx.Clear(ClearFlags.Target, Color.FromArgb(0, 0, 0, 0), 1f, 0);
				this.dx.SetRenderState(RenderState.ZEnable, false);
				this.dx.SetRenderState(RenderState.Lighting, false);
				this.dx.SetRenderState(RenderState.AlphaBlendEnable, true);
				this.dx.SetRenderState<Cull>(RenderState.CullMode, Cull.None);
				this.dx.BeginScene();
				this.RC.RenderFrame();
				this.dx.EndScene();
				if (this.dx.Present() == ResultCode.DeviceLost)
				{
					base.Invoke(new Action(this.ResetDirectx));
					return;
				}
			}
		}
		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
		[DllImport("dwmapi.dll")]
		private static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hWnd, ref TransparentDXOverlay.Margins pMargins);
	}
}
