using System.Collections.Generic;
using System.Linq;
using PoeHUD.Framework;

namespace PoeHUD.Hud.Menu
{
	public abstract class MenuItem
	{
		protected readonly Menu.MenuSettings settings;
		protected readonly List<MenuItem> children = new List<MenuItem>();
		protected MenuItem currentHover;
		protected bool isVisible;
		public Rect Bounds { get; set; }
		public virtual int Width { get { return settings.ItemWidth; } }
		public virtual int Height { get { return settings.ItemHeight; } }

		protected MenuItem(Menu.MenuSettings settings)
		{
			this.settings = settings;
		}
		public abstract void Render(RenderingContext rc);
		protected abstract void HandleEvent(MouseEventID id, Vec2 pos);
		protected virtual bool TestBounds(Vec2 pos)
		{
			return this.Bounds.HasPoint(pos);
		}
		public bool TestHit(Vec2 pos)
		{
			return this.isVisible && (this.TestBounds(pos) || this.children.Any(current => current.TestHit(pos)));
		}

		public void SetVisible(bool visible)
		{
			this.isVisible = visible;
			if (visible) return;
			foreach (MenuItem current in this.children)
			{
				current.SetVisible(false);
			}
		}
		public void SetHovered(bool hover)
		{
			foreach (MenuItem current in this.children)
			{
				current.SetVisible(hover);
			}
		}
		public void OnEvent(MouseEventID id, Vec2 pos)
		{
			if (id == MouseEventID.MouseMove)
			{
				if (this.TestBounds(pos))
				{
					this.HandleEvent(id, pos);
					if (this.currentHover != null)
					{
						this.currentHover.SetHovered(false);
						this.currentHover = null;
					}
					return;
				}
				if (this.currentHover != null)
				{
					if (this.currentHover.TestHit(pos))
					{
						this.currentHover.OnEvent(id, pos);
						return;
					}
					this.currentHover.SetHovered(false);
				}
				MenuItem childAt = this.GetChildAt(pos);
				if (childAt != null)
				{
					childAt.SetHovered(true);
					this.currentHover = childAt;
					return;
				}
				this.currentHover = null;
				return;
			}

			if (this.TestBounds(pos))
			{
				this.HandleEvent(id, pos);
				return;
			}
			if (this.currentHover != null)
				this.currentHover.OnEvent(id, pos);
		}
		private MenuItem GetChildAt(Vec2 pos)
		{
			return this.children.FirstOrDefault(current => current.TestHit(pos));
		}

		public void AddChild(MenuItem item)
		{
			int x = this.Bounds.X + this.Bounds.W;
			int num = this.Bounds.Y + this.children.Sum(current => current.Bounds.H);
			item.Bounds = new Rect(x, num, item.Width, item.Height);
			this.children.Add(item);
		}
	}
}
