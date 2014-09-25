using System.Collections.Generic;
using ExileHUD.Framework;

namespace ExileHUD.ExileHUD
{
	public abstract class MenuItem
	{
		protected List<MenuItem> children;
		protected MenuItem currentHover;
		protected bool isVisible;
		public Rect Bounds
		{
			get;
			set;
		}
		public abstract int DesiredWidth
		{
			get;
		}
		public abstract int DesiredHeight
		{
			get;
		}
		public MenuItem()
		{
			this.children = new List<MenuItem>();
		}
		public abstract void Render(RenderingContext rc);
		protected abstract void HandleEvent(MouseEventID id, Vec2 pos);
		protected virtual bool TestBounds(Vec2 pos)
		{
			return this.Bounds.HasPoint(pos);
		}
		public bool TestHit(Vec2 pos)
		{
			if (!this.isVisible)
			{
				return false;
			}
			if (this.TestBounds(pos))
			{
				return true;
			}
			foreach (MenuItem current in this.children)
			{
				if (current.TestHit(pos))
				{
					return true;
				}
			}
			return false;
		}
		public void SetVisible(bool visible)
		{
			this.isVisible = visible;
			if (!visible)
			{
				foreach (MenuItem current in this.children)
				{
					current.SetVisible(false);
				}
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
			else
			{
				if (this.TestBounds(pos))
				{
					this.HandleEvent(id, pos);
					return;
				}
				if (this.currentHover != null)
				{
					this.currentHover.OnEvent(id, pos);
				}
				return;
			}
		}
		private MenuItem GetChildAt(Vec2 pos)
		{
			foreach (MenuItem current in this.children)
			{
				if (current.TestHit(pos))
				{
					return current;
				}
			}
			return null;
		}
		public void AddChild(MenuItem item)
		{
			int num = this.Bounds.Y;
			int x = this.Bounds.X + this.Bounds.W;
			foreach (MenuItem current in this.children)
			{
				num += current.Bounds.H;
			}
			item.Bounds = new Rect(x, num, item.DesiredWidth, item.DesiredHeight);
			this.children.Add(item);
		}
	}
}
