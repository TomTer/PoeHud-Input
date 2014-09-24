using System;
namespace BotFramework
{
	public struct Rect
	{
		public int X;
		public int Y;
		public int W;
		public int H;
		public Rect(int x, int y, int w, int h)
		{
			this.X = x;
			this.Y = y;
			this.W = w;
			this.H = h;
		}
		public Rect(int w, int h)
		{
			this = new Rect(0, 0, w, h);
		}
		public bool HasPoint(Vec2 v)
		{
			return this.HasPoint(v.X, v.Y);
		}
		public bool HasPoint(int x, int y)
		{
			return x >= this.X && y >= this.Y && x <= this.X + this.W && y <= this.Y + this.H;
		}
		public bool HasRect(Rect r)
		{
			return r.X >= this.X && r.Y >= this.Y && r.W <= this.W && r.H <= this.H;
		}
		public Rect Expand(int xd, int yd)
		{
			return new Rect(this.X - xd, this.Y - yd, this.W + xd, this.H + yd);
		}
		public Rect Adjust(int width, int height)
		{
			float num = (float)width / 2560f;
			float num2 = (float)height / 1600f;
			return new Rect((int)((float)this.X * num), (int)((float)this.Y * num2), (int)((float)this.W * num), (int)((float)this.H * num2));
		}
		public override bool Equals(object obj)
		{
			if (!(obj is Rect))
			{
				return false;
			}
			Rect rect = (Rect)obj;
			return rect.X == this.X && rect.Y == this.Y && rect.W == this.W && rect.H == this.H;
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[",
				this.X,
				", ",
				this.Y,
				", ",
				this.W,
				", ",
				this.H,
				"]"
			});
		}
		public override int GetHashCode()
		{
			return this.X + this.Y + this.W + this.H;
		}
	}
}
