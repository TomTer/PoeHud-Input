using System;
using PoeHUD.Framework;

namespace PoeHUD.Poe
{
	public class Camera : RemoteMemoryObject
	{
		public int Width { get { return M.ReadInt(Address + 4); } }
		public int Height { get { return M.ReadInt(Address + 8); } }
		public float ZFar
		{
			get
			{
				return M.ReadFloat(Address + 392);
			}
			set
			{
				M.WriteFloat(Address + 392, value);
			}
		}
		public Vec3 Position
		{
			get
			{
				return new Vec3(M.ReadFloat(Address + 256), M.ReadFloat(Address + 260), M.ReadFloat(Address + 264));
			}
		}
        public unsafe Vec2 WorldToScreen(Vec3 vec3, bool allowOffscreen = false)
        {
            double num2;
            double num3;
            double num4;
            int addr = base.Address + 0xbc;
            fixed (byte* numRef = base.M.ReadBytes(addr, 0x40))
            {
                float* numPtr = (float*)numRef;
                double num5 = (((numPtr[3] * vec3.x) + (numPtr[7] * vec3.y)) + (numPtr[11] * vec3.z)) + numPtr[15];
                num2 = ((((numPtr[0] * vec3.x) + (numPtr[4] * vec3.y)) + (numPtr[8] * vec3.z)) + numPtr[12]) / num5;
                num3 = ((((numPtr[1] * vec3.x) + (numPtr[5] * vec3.y)) + (numPtr[9] * vec3.z)) + numPtr[13]) / num5;
                num4 = ((((numPtr[2] * vec3.x) + (numPtr[6] * vec3.y)) + (numPtr[10] * vec3.z)) + numPtr[14]) / num5;
            }
            if (!allowOffscreen && (num4 < 0.0 || Math.Abs(num2) > 1.0 || Math.Abs(num3) > 1.0))
            {
                return Vec2.Empty;
            }
            num2 = ((num2 + 1.0) * 0.5) * Width;
            num3 = ((1.0 - num3) * 0.5) * Height;
            return new Vec2((int)Math.Round(num2), (int)Math.Round(num3));
        }

	}
}
