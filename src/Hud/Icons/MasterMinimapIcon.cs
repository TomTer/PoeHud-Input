using PoeHUD.ExileBot;

namespace PoeHUD.Hud.Icons
{
	public class MasterMinimapIcon : MinimapIcon
	{
		public MasterMinimapIcon(Entity entity, string texture, int size, MinimapRenderPriority prio) : base(entity, texture, size, prio)
		{
		}
		public override bool Validate()
		{
			return this.Entity.IsValid;
		}
		public override bool WantsToRender()
		{
			return Settings.GetBool("MinimapIcons.Masters") && this.Entity.IsAlive;
		}
	}
}
