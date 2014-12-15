namespace PoeHUD.Framework
{
	public struct Rect
	{
		public int X;
		public int Y;
		public int W;
		public int H;
		public Rect(int x, int y, int w, int h)
		{
			X = x;
			Y = y;
			W = w;
			H = h;
		}
		public Rect(int w, int h)
		{
			this = new Rect(0, 0, w, h);
		}

		public Rect(Vec2 position, Vec2 size = default(Vec2))
		{
			this = new Rect(position.X, position.Y, size.X, size.Y);
		}

		public bool HasPoint(Vec2 v)
		{
			return HasPoint(v.X, v.Y);
		}
		public bool HasPoint(int x, int y)
		{
			return x >= X && y >= Y && x <= X + W && y <= Y + H;
		}
		public bool HasRect(Rect r)
		{
			return r.X >= X && r.Y >= Y && r.W <= W && r.H <= H;
		}
		public Rect Expand(int xd, int yd)
		{
			return new Rect(X - xd, Y - yd, W + xd, H + yd);
		}
		public Rect Adjust(int width, int height)
		{
			float num = (float)width / 2560f;
			float num2 = (float)height / 1600f;
			return new Rect((int)((float)X * num), (int)((float)Y * num2), (int)((float)W * num), (int)((float)H * num2));
		}
		public override bool Equals(object obj)
		{
			if (!(obj is Rect))
			{
				return false;
			}
			Rect rect = (Rect)obj;
			return rect.X == X && rect.Y == Y && rect.W == W && rect.H == H;
		}
		public override string ToString()
		{
			return string.Concat(new object[] { "[", X, ", ", Y, ", ", W, ", ", H, "]" });
		}
		public override int GetHashCode()
		{
			return X + Y + W + H;
		}
	}
}
