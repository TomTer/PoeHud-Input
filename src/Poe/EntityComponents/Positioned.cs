using PoeHUD.Framework;

namespace PoeHUD.Poe.EntityComponents
{
	public class Positioned : Component
	{
		public float X
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadFloat(this.address + 44);
				}
				return 0f;
			}
		}
		public float Y
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadFloat(this.address + 48);
				}
				return 0f;
			}
		}
		public int GridX
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 52);
				}
				return 0;
			}
		}
		public int GridY
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadInt(this.address + 56);
				}
				return 0;
			}
		}
		public Vec2 GridPos
		{
			get
			{
				return new Vec2(this.GridX, this.GridY);
			}
		}
	}
}
