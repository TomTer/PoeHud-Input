using System;

namespace PoeHUD.Framework
{
	public struct Vec2
	{
		public static readonly Vec2 Empty = new Vec2(0, 0);
		public int X;
		public int Y;
		public Vec2(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
		public float Dist(Vec2 other)
		{
			float num = (float)(other.X - this.X);
			float num2 = (float)(other.Y - this.Y);
			return (float)Math.Sqrt((double)(num * num + num2 * num2));
		}
		public override int GetHashCode()
		{
			return this.X + this.Y;
		}
		public override bool Equals(object obj)
		{
			if (obj is Vec2)
			{
				Vec2 vec = (Vec2)obj;
				return vec.X == this.X && vec.Y == this.Y;
			}
			return false;
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[",
				this.X,
				", ",
				this.Y,
				"]"
			});
		}
		public static bool operator ==(Vec2 left, Vec2 right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Vec2 left, Vec2 right)
		{
			return !left.Equals(right);
		}
		public static Vec2 operator +(Vec2 left, Vec2 right)
		{
			return new Vec2(left.X + right.X, left.Y + right.Y);
		}
		public static Vec2 operator -(Vec2 left, Vec2 right)
		{
			return new Vec2(left.X - right.X, left.Y - right.Y);
		}

		public static Vec2 operator * (Vec2 left, float right)
		{
			return new Vec2((int)(left.X * right), (int)(left.Y * right));
		}
		public static Vec2 operator / (Vec2 left, float right)
		{
			return new Vec2((int)(left.X / right), (int)(left.Y / right));
		}


		public double GetPolarCoordinates(out double phi)
		{
			double distance = Math.Sqrt(this.X*this.X + this.Y*this.Y);
			phi = Math.Acos(this.X/distance);
			if (this.Y < 0)
				phi = 2*Math.PI - phi;
			return distance;
		}
	}
}
