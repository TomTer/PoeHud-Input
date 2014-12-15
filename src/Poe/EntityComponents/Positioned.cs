using PoeHUD.Framework;

namespace PoeHUD.Poe.EntityComponents
{
	public class Positioned : Component
	{
		public float X
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadFloat(this.Address + 44);
				}
				return 0f;
			}
		}
		public float Y
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadFloat(this.Address + 48);
				}
				return 0f;
			}
		}
		public int GridX
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 52);
				}
				return 0;
			}
		}
		public int GridY
		{
			get
			{
				if (this.Address != 0)
				{
					return this.M.ReadInt(this.Address + 56);
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
