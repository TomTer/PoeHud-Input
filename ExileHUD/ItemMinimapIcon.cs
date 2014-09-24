using ExileBot;
using System;
using System.Drawing;
namespace ExileHUD
{
	public class ItemMinimapIcon : MinimapIcon
	{
		public ItemMinimapIcon(Entity entity, string texture, int size) : base(entity, texture, size, Color.FromArgb(255, 0, 255, 0), MinimapRenderPriority.RareMonster)
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
