using System;
using System.Collections.Generic;
using PoeHUD.Framework;

namespace PoeHUD.Hud
{
	public class ClientHacks : HUDPluginBase
	{
		private Memory m;
		private bool maphackEnabled;
		private bool zoomhackEnabled;
		private bool fullbrightEnabled;
		private bool particlesEnabled;
		private bool hasSetWriteAccess;
		public override void OnEnable()
		{
			this.m = this.model.Memory;
			if (Settings.GetBool("ClientHacks"))
			{
				this.maphackEnabled = Settings.GetBool("ClientHacks.Maphack");
				if (this.maphackEnabled)
				{
					this.EnableMaphack();
				}
				this.zoomhackEnabled = Settings.GetBool("ClientHacks.Zoomhack");
				if (this.zoomhackEnabled)
				{
					this.EnableZoomhack();
				}
				this.fullbrightEnabled = Settings.GetBool("ClientHacks.Fullbright");
				if (this.fullbrightEnabled)
				{
					this.EnableFullbright();
				}
			}
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			bool flag = Settings.GetBool("ClientHacks") && Settings.GetBool("ClientHacks.Maphack");
			if (flag != this.maphackEnabled)
			{
				this.maphackEnabled = !this.maphackEnabled;
				if (this.maphackEnabled)
					this.EnableMaphack();
				else
					this.DisableMaphack();
			}
			if (this.zoomhackEnabled && this.model.InGame)
			{
				float zFar = this.model.Internal.IngameState.Camera.ZFar;
				if (zFar != 10000f)
				{
					this.model.Internal.IngameState.Camera.ZFar = 10000f;
				}
			}
			bool flag2 = Settings.GetBool("ClientHacks") && Settings.GetBool("ClientHacks.Zoomhack");
			if (flag2 != this.zoomhackEnabled)
			{
				this.zoomhackEnabled = !this.zoomhackEnabled;
				if (this.zoomhackEnabled)
					this.EnableZoomhack();
				else
					this.DisableZoomhack();
			}
			bool flag3 = Settings.GetBool("ClientHacks") && Settings.GetBool("ClientHacks.Fullbright");
			if (flag3 != this.fullbrightEnabled)
			{
				this.fullbrightEnabled = !this.fullbrightEnabled;
				if (this.fullbrightEnabled)
					this.EnableFullbright();
				else
					this.DisableFullbright();
			}
			bool flag4 = Settings.GetBool("ClientHacks") && Settings.GetBool("ClientHacks.Particles");
			if (flag4 != this.particlesEnabled)
			{
				this.particlesEnabled = !this.particlesEnabled;
				if (this.particlesEnabled)
					this.EnableParticles();
				else
					this.DisableParticles();
			}
		}

		public override void OnDisable()
		{
			if (!this.m.IsInvalid())
			{
				this.DisableMaphack();
				this.DisableZoomhack();
				this.DisableFullbright();
				this.DisableParticles();
			}
		}
		private void EnableFullbright()
		{
			if (!this.hasSetWriteAccess)
			{
				this.hasSetWriteAccess = true;
				this.m.MakeMemoryWriteable(this.m.BaseAddress + m.offsets.Fullbright1, 4);
				this.m.MakeMemoryWriteable(this.m.BaseAddress + m.offsets.Fullbright2, 4);
			}
			this.m.WriteFloat(this.m.BaseAddress + m.offsets.Fullbright1, 15000f);
			this.m.WriteFloat(this.m.BaseAddress + m.offsets.Fullbright2, 5000f);
		}
		private void DisableFullbright()
		{
			this.m.WriteFloat(this.m.BaseAddress + m.offsets.Fullbright1, 1300f);
			this.m.WriteFloat(this.m.BaseAddress + m.offsets.Fullbright2, 350f);
		}

		private void EnableParticles()
		{
			this.m.WriteBytes(m.offsets.ParticlesCode, new byte[] { 0x90, 0xE9 });
		}
		private void DisableParticles()
		{
			this.m.WriteBytes(m.offsets.ParticlesCode, new byte[] { 0x0F, 0x85 });
		}


		private void EnableZoomhack()
		{
			this.m.WriteBytes(this.m.BaseAddress + m.offsets.ZoomHackFunc, new byte[]
			{
				16
			});
		}
		private void DisableZoomhack()
		{
			this.m.WriteBytes(this.m.BaseAddress + m.offsets.ZoomHackFunc, new byte[]
			{
				20
			});
		}
		private void EnableMaphack()
		{
			int num = this.m.BaseAddress + m.offsets.MaphackFunc;
			if (this.m.ReadByte(num) != 81)
			{
				Console.WriteLine("Something is wrong with maphackfunc");
				return;
			}
			while (this.m.ReadByte(num) != 195)
			{
				byte b = this.m.ReadByte(num);
				byte b2 = this.m.ReadByte(num + 1);
				if (b == 217 && b2 == 0)
				{
					this.m.WriteBytes(num + 1, new byte[]
					{
						232
					});
				}
				num++;
			}
			Console.WriteLine("Maphack applied");
		}
		private void DisableMaphack()
		{
			int num = this.m.BaseAddress + m.offsets.MaphackFunc;
			if (this.m.ReadByte(num) != 81)
			{
				Console.WriteLine("Something is wrong with maphackfunc");
				return;
			}
			while (this.m.ReadByte(num) != 195)
			{
				byte b = this.m.ReadByte(num);
				byte b2 = this.m.ReadByte(num + 1);
				if (b == 217 && b2 == 232)
				{
					Memory arg_6A_0 = this.m;
					int arg_6A_1 = num + 1;
					byte[] bytes = new byte[1];
					arg_6A_0.WriteBytes(arg_6A_1, bytes);
				}
				num++;
			}
			Console.WriteLine("Maphack removed");
		}
	}
}
