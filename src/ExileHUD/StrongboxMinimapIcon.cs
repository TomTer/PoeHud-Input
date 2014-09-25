using System.Drawing;
using ExileHUD.EntityComponents;
using ExileHUD.ExileBot;

namespace ExileHUD.ExileHUD
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
