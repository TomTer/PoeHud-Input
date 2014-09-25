using System.Drawing;
using ExileHUD.EntityComponents;
using ExileHUD.ExileBot;

namespace ExileHUD.ExileHUD
{
	public class ChestMinimapIcon : MinimapIcon
	{
		public ChestMinimapIcon(Entity entity, string texture, int size) : base(entity, texture, size, Color.White, MinimapRenderPriority.Monster)
		{
		}
		public override bool Validate()
		{
			return this.Entity.IsValid && !this.Entity.GetComponent<Chest>().IsOpened;
		}
		public override bool WantsToRender()
		{
			return Settings.GetBool("MinimapIcons.Chests");
		}
	}
}
