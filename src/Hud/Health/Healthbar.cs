using PoeHUD.Controllers;
using PoeHUD.Settings;

namespace PoeHUD.Hud.Health
{
	class Healthbar
	{
		public EntityWrapper entity;
		public HealthBarRenderer.PerGroupSetting settings;
		public RenderPrio prio;
		public Healthbar(EntityWrapper entity, HealthBarRenderer.PerGroupSetting settings, RenderPrio prio)
		{
			this.entity = entity;
			this.settings = settings;
			this.prio = prio;
		}
	}
}