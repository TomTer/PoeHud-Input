using System;
namespace BotFramework
{
	public struct Vec3
	{
		public float x;
		public float y;
		public float z;
		public static Vec3 Zero = new Vec3(0f, 0f, 0f);
		public static Vec3 Invalid = new Vec3(-1f, -1f, -1f);
		public Vec3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public float Dist(Vec3 other)
		{
			float num = this.x - other.x;
			float num2 = this.y - other.y;
			float num3 = this.z - other.z;
			return (float)Math.Sqrt((double)(num * num + num2 * num2 + num3 * num3));
		}
		public float SquareDist(Vec3 other)
		{
			float num = this.x - other.x;
			float num2 = this.y - other.y;
			float num3 = this.z - other.z;
			return num * num + num2 * num2 + num3 * num3;
		}
		public Vec3 Normalize()
		{
			float num = (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y + this.z * this.z));
			return new Vec3(this.x / num, this.y / num, this.z / num);
		}
		public float Dot(Vec3 other)
		{
			return this.x * other.x + this.y * other.y + this.z * other.z;
		}
		public Vec3 Cross(Vec3 other)
		{
			return new Vec3(this.y * other.z - other.y * this.z, this.z * other.x - this.x * other.z, this.x * other.y - this.y * other.x);
		}
		public Vec3 Translate(float x, float y, float z)
		{
			return new Vec3(this.x + x, this.y + y, this.z + z);
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.x,
				" ",
				this.y,
				" ",
				this.z
			});
		}
		public override bool Equals(object obj)
		{
			if (obj is Vec3)
			{
				Vec3 vec = (Vec3)obj;
				return vec.x == this.x && vec.y == this.y && vec.z == this.z;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(this.x + this.y + this.z), 0);
		}
		public static Vec3 operator +(Vec3 left, Vec3 right)
		{
			return new Vec3(left.x + right.x, left.y + right.y, left.z + right.z);
		}
		public static Vec3 operator -(Vec3 left, Vec3 right)
		{
			return new Vec3(left.x - right.x, left.y - right.y, left.z - right.z);
		}
		public static Vec3 operator *(Vec3 left, float right)
		{
			return new Vec3(left.x * right, left.y * right, left.z * right);
		}
		public static bool operator ==(Vec3 left, Vec3 right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Vec3 left, Vec3 right)
		{
			return !left.Equals(right);
		}
	}
}
