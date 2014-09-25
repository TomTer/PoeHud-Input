using ExileHUD.Framework;

namespace ExileHUD.EntityComponents
{
	public class Render : Component
	{
		public float X
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
		public float Y
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadFloat(this.address + 52);
				}
				return 0f;
			}
		}
		public float Z
		{
			get
			{
				if (this.address != 0)
				{
					return this.m.ReadFloat(this.address + 136);
				}
				return 0f;
			}
		}
		public Vec3 Pos
		{
			get
			{
				return new Vec3(this.X, this.Y, this.Z);
			}
		}
		public string DisplayName
		{
			get
			{
				if (this.address == 0)
				{
					return "";
				}
				int num = this.m.ReadInt(this.address + 88);
				if (num < 8)
				{
					return this.m.ReadStringU(this.address + 72, 16, true);
				}
				return this.m.ReadStringU(this.m.ReadInt(this.address + 72), 256, true);
			}
		}
	}
}
