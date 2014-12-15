using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Settings;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Preload
{
	public class PreloadAlert : HUDPluginBase
	{
		private HashSet<string> disp;
		private Dictionary<string, string> alertStrings;
		private int lastCount;

		public class SettingsPreload : SettingsForModule
		{
			public readonly SettingIntRange FontSize = new SettingIntRange("FontSize", 7, 30, 12);
			public readonly SettingIntRange BgAlpha = new SettingIntRange("BgAlpha", 0, 255, 160);
			public SettingsPreload() : base("PreloadAlert")
			{
			}
		}
		public SettingsPreload Settings = new SettingsPreload();

		public override void OnEnable()
		{
			this.disp = new HashSet<string>();
			alertStrings = FsUtils.LoadKeyValueCommaSeparatedFromFile("config/preloads.txt");
			this.OnAreaChange(this.model.Area);
		}
		public override void OnDisable()
		{
		}

		public override SettingsForModule SettingsNode
		{
			get { return Settings; }
		}

		public override void OnAreaChange(AreaController area)
		{
			if (Settings.Enabled) {
				this.Parse();
			}
		}
		private void Parse()
		{
			this.disp.Clear();
			int pFileRoot = this.model.Memory.ReadInt(this.model.Memory.BaseAddress + model.Memory.offsets.FileRoot);
			int num2 = this.model.Memory.ReadInt(pFileRoot + 12);
			int listIterator = this.model.Memory.ReadInt(pFileRoot + 20);
			int areaChangeCount = this.model.Internal.AreaChangeCount;
			for (int i = 0; i < num2; i++)
			{
				listIterator = this.model.Memory.ReadInt(listIterator);
				if (this.model.Memory.ReadInt(listIterator + 8) != 0 && this.model.Memory.ReadInt(listIterator + 12, 36) == areaChangeCount)
				{
					string text = this.model.Memory.ReadStringU(this.model.Memory.ReadInt(listIterator + 8), 256, true);
					if (text.Contains("vaal_sidearea"))
					{
						this.disp.Add("Area contains Corrupted Area");
					}
					if (text.Contains('@'))
					{
						text = text.Split(new char[] { '@' })[0];
					}
					if (text.StartsWith("Metadata/Monsters/Missions/MasterStrDex"))
					{
						Console.WriteLine("bad alert " + text);
						this.disp.Add("Area contains Vagan, Weaponmaster");
					}
					if (this.alertStrings.ContainsKey(text))
					{
						Console.WriteLine("Alert because of " + text);
						this.disp.Add(this.alertStrings[text]);
					}
					else
					{
						if (text.EndsWith("BossInvasion"))
						{
							this.disp.Add("Area contains Invasion Boss");
						}
					}
				}
			}
		}
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.Enabled)
			{
				return;
			}
			int num = this.model.Memory.ReadInt(this.model.Memory.BaseAddress + model.Memory.offsets.FileRoot, new int[]
			{
				12
			});
			if (num != this.lastCount)
			{
				this.lastCount = num;
				this.Parse();
			}
			if (this.disp.Count > 0)
			{

				Vec2 vec = mountPoints[UiMountPoint.LeftOfMinimap];
				int num2 = vec.Y;
				int maxWidth = 0;
				foreach (string current in this.disp)
				{
					Vec2 vec2 = rc.AddTextWithHeight(new Vec2(vec.X, num2), current, Color.White, Settings.FontSize, DrawTextFormat.Right);
					if (vec2.X + 10 > maxWidth)
					{
						maxWidth = vec2.X + 10;
					}
					num2 += vec2.Y;
				}
				if (maxWidth > 0 && Settings.BgAlpha > 0)
				{
					Rect bounds = new Rect(vec.X - maxWidth + 5, vec.Y - 5, maxWidth, num2 - vec.Y + 10);
					rc.AddBox(bounds, Color.FromArgb(Settings.BgAlpha, 1, 1, 1));
					mountPoints[UiMountPoint.LeftOfMinimap] = new Vec2(vec.X, vec.Y + 5 + bounds.H);
				}

				
			}
		}
	}
}
