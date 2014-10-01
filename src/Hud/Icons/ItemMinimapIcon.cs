using System.Drawing;
using PoeHUD.ExileBot;

namespace PoeHUD.Hud.Icons
{
	public class ItemMinimapIcon : MinimapIcon
	{
		public ItemMinimapIcon(Entity entity, string texture, Color color, int size) : 
			base(entity, texture, size, color, MinimapRenderPriority.RareMonster)
		{
		}
		public override bool Validate()
		{
			return this.Entity.IsValid;
		}
		public override bool WantsToRender()
		{
			return Settings.GetBool("MinimapIcons.AlertedItems");
		}
	}
}
