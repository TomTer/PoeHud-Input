using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Settings;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.DPS
{
	public class DpsMeter : HUDPluginBase
	{
		private bool hasStarted;
		private DateTime lastCalcTime;
		private Dictionary<int, int> lastEntities;
		private DateTime startTime;

		
		private const float dps_period = 0.2f;
		private readonly float[] damageMemory = new float[10];
		private int ixDamageMemory;
		private int maxDps;

		public class DpsDisplaySettings : SettingsForModule
		{
			public SettingIntRange PeakFontSize = new SettingIntRange("Peak font size", 7, 18, 8);
			public SettingIntRange DpsFontSize = new SettingIntRange("DPS font size", 10, 30, 16);
			public DpsDisplaySettings() : base("DPS-meter") { }
		}

		public DpsDisplaySettings Settings = new DpsDisplaySettings(); 

		public override void OnEnable()
		{
			lastEntities = new Dictionary<int, int>();
		}

		public override void OnAreaChange(AreaController area)
		{
			lastEntities = new Dictionary<int, int>();
			hasStarted = false;
			maxDps = 0;
		}

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!hasStarted)
			{
				lastCalcTime = DateTime.Now;
				hasStarted = true;
				return;
			}

			DateTime dtNow = DateTime.Now;
			TimeSpan delta = dtNow - lastCalcTime;

			if (delta.TotalSeconds > dps_period)
			{
				ixDamageMemory++;
				if (ixDamageMemory >= damageMemory.Length)
					ixDamageMemory = 0;
				damageMemory[ixDamageMemory] = CalculateDps(delta);
				lastCalcTime = dtNow;
			}

			Vec2 mapWithOffset = mountPoints[UiMountPoint.LeftOfMinimap];
			int dps = ((int)damageMemory.Average());
			if (maxDps < dps)
				maxDps = dps;
			
			var textSize = rc.AddTextWithHeight(mapWithOffset,  dps + " DPS", Color.White, Settings.DpsFontSize, DrawTextFormat.Right);
			var tx2 = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, mapWithOffset.Y + textSize.Y), maxDps + " peak DPS", Color.White, Settings.PeakFontSize, DrawTextFormat.Right);

			int width = Math.Max(tx2.X, textSize.X);
			Rect rect = new Rect(mapWithOffset.X - 5 - width, mapWithOffset.Y - 5, width + 10, textSize.Y + tx2.Y + 10);
			
			rc.AddBox(rect, Color.FromArgb(160, Color.Black));

			mountPoints[UiMountPoint.LeftOfMinimap] = new Vec2(mapWithOffset.X, mapWithOffset.Y + 5 + rect.H);
		}

		private float CalculateDps(TimeSpan dt)
		{
			Dictionary<int, int> currentEntities = new Dictionary<int, int>();

			int damageDoneThisCycle = 0;

			foreach (var entity in model.Entities.Where(x => x.HasComponent<Poe.EntityComponents.Monster>() && x.IsHostile))
			{
				int entityEHP = entity.IsAlive ? entity.GetComponent<Life>().CurHP + entity.GetComponent<Life>().CurES : 0;
				if (entityEHP > 10000000 || entityEHP < -1000000) //discard those - read form invalid addresses
					continue;

				if (lastEntities.ContainsKey(entity.Id))
				{
					if (lastEntities[entity.Id] > entityEHP) damageDoneThisCycle += lastEntities[entity.Id] - entityEHP;
				}

				currentEntities.Add(entity.Id, entityEHP);
			}
			// cache current life/es values for next check
			lastEntities = currentEntities;

			return (float)(damageDoneThisCycle / dt.TotalSeconds);
		}

		public override void OnDisable()
		{
		}

		public override SettingsForModule SettingsNode
		{
			get { return Settings; }
		}
	}
}
