namespace PoeHUD.Framework
{
	public struct Vec2f
	{
		public static readonly Vec2f Empty = new Vec2f(0f, 0f);
		public float X;
		public float Y;
		public Vec2f(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}
		public override int GetHashCode()
		{
			return this.X.GetHashCode() + this.Y.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (obj is Vec2f)
			{
				Vec2f vec2f = (Vec2f)obj;
				return vec2f.X == this.X && vec2f.Y == this.Y;
			}
			return false;
		}
		public override string ToString()
		{
			return string.Concat(new object[] { "[", this.X, ", ", this.Y, "]" });
		}
		public static bool operator ==(Vec2f left, Vec2f right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Vec2f left, Vec2f right)
		{
			return !left.Equals(right);
		}
		public static Vec2f operator +(Vec2f left, Vec2f right)
		{
			return new Vec2f(left.X + right.X, left.Y + right.Y);
		}
		public static Vec2f operator -(Vec2f left, Vec2f right)
		{
			return new Vec2f(left.X - right.X, left.Y - right.Y);
		}
	}
}
