using System.Drawing;
using PoeHUD.ExileBot;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.ExileHUD.MapIcon
{
	public abstract class MinimapIcon
	{
		private string Texture;
		protected Entity Entity;
		private int Size;
		private Color color;
		public MinimapRenderPriority RenderPriority
		{
			get;
			private set;
		}
		public Vec2 WorldPosition
		{
			get
			{
				return this.Entity.GetComponent<Positioned>().GridPos;
			}
		}
		public MinimapIcon(Entity entity, string texture, int size, MinimapRenderPriority prio) : this(entity, texture, size, Color.White, prio)
		{
		}
		public MinimapIcon(Entity entity, string texture, int size, Color color, MinimapRenderPriority prio)
		{
			this.Entity = entity;
			this.Texture = texture;
			this.Size = size;
			this.color = color;
			this.RenderPriority = prio;
		}
		public void RenderAt(RenderingContext rc, Vec2 point)
		{
			Rect rect = new Rect(point.X - this.Size / 2, point.Y - this.Size / 2, this.Size, this.Size);
			rc.AddTexture(this.Texture, rect, this.color);
		}
		public abstract bool Validate();
		public abstract bool WantsToRender();
	}
}
