using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.DPS
{
	public class DpsMeter : HUDPluginBase
	{
		private bool hasStarted;
		private DateTime lastCalcTime;
		private Dictionary<int, int> lastEntities;
		private DateTime startTime;

		
		private const float dps_period = 0.4f;
		private readonly float[] damageMemory = new float[10];
		private int ixDamageMemory;

		public override void OnEnable()
		{
			lastEntities = new Dictionary<int, int>();
			model.Area.OnAreaChange += CurrentArea_OnAreaChange;
		}

		private void CurrentArea_OnAreaChange(AreaController area)
		{
			startTime = DateTime.Now;
			lastEntities = new Dictionary<int, int>();
		}

		public override void Render(RenderingContext rc)
		{
			if (!hasStarted)
			{
				startTime = DateTime.Now;
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
				model.Area.CurrentArea.AddTimeSpent(delta);
				damageMemory[ixDamageMemory] = CalculateDps(delta);
				lastCalcTime = dtNow;
			}

			int fontSize = Settings.GetInt("XphDisplay.FontSize");

			Rect clientRect = model.Internal.IngameState.IngameUi.Minimap.SmallMinimap.GetClientRect();
			Vec2 mapWithOffset = new Vec2(clientRect.X - 10, clientRect.Y + 5);
			int yCursor = 0;

			int width = clientRect.W;
			Rect rect = new Rect(mapWithOffset.X - width + 5, mapWithOffset.Y - 5, width, yCursor + 20);

			rc.AddTextWithHeight(new Vec2(rect.X + 5, mapWithOffset.Y + 55), ((int)damageMemory.Average()) + " DPS", Color.White,
				fontSize, DrawTextFormat.Center);
		}

		private float CalculateDps(TimeSpan dt)
		{
			Dictionary<int, int> currentEntities = new Dictionary<int, int>();

			int damageDoneThisCycle = 0;

			foreach (var entity in model.Entities.Where(x => x.HasComponent<Poe.EntityComponents.Monster>() && x.IsHostile))
			{
				int entityEHP = entity.IsAlive ? entity.GetComponent<Life>().CurHP + entity.GetComponent<Life>().CurES : 0;

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
	}
}
