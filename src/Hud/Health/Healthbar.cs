using PoeHUD.Controllers;

namespace PoeHUD.Hud.Health
{
	class Healthbar
	{
		public EntityWrapper entity;
		public string settings;
		public RenderPrio prio;
		public Healthbar(EntityWrapper entity, string settings, RenderPrio prio)
		{
			this.entity = entity;
			this.settings = settings;
			this.prio = prio;
		}
	}
}