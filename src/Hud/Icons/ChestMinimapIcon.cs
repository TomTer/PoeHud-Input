using System.Drawing;
using PoeHUD.ExileBot;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Hud.Icons
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
