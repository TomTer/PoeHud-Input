using System.Drawing;
using PoeHUD.ExileBot;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Hud.Icons
{
	public class StrongboxMinimapIcon : MinimapIcon
	{
		public StrongboxMinimapIcon(Entity entity, string texture, int size, Color color) : base(entity, texture, size, color, MinimapRenderPriority.Strongbox)
		{
		}
		public override bool Validate()
		{
			return this.Entity.IsValid && !this.Entity.GetComponent<Chest>().IsOpened;
		}
		public override bool WantsToRender()
		{
			return Settings.GetBool("MinimapIcons.Strongboxes");
		}
	}
}
