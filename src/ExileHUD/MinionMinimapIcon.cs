using ExileBot;
using System;
namespace ExileHUD
{
	public class MinionMinimapIcon : MonsterMinimapIcon
	{
		public MinionMinimapIcon(Entity entity, string texture, int size, MinimapRenderPriority prio) : base(entity, texture, size, prio)
		{
		}
		public override bool WantsToRender()
		{
			return Settings.GetBool("MinimapIcons.Minions") && this.Entity.IsAlive;
		}
	}
}
