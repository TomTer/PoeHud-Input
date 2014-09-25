using System.Drawing;
using ExileHUD.ExileBot;

namespace ExileHUD.ExileHUD
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
