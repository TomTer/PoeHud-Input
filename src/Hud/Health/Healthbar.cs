using PoeHUD.ExileBot;

namespace PoeHUD.Hud.Health
{
	class Healthbar
	{
		public Entity entity;
		public string settings;
		public RenderPrio prio;
		public Healthbar(Entity entity, string settings, RenderPrio prio)
		{
			this.entity = entity;
			this.settings = settings;
			this.prio = prio;
		}
	}
}