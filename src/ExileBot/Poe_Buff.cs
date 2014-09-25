using ExileHUD.Framework;

namespace ExileHUD.ExileBot
{
	public class Poe_Buff : RemoteMemoryObject
	{
		public string Name
		{
			get
			{
				Memory arg_27_0 = this.m;
				Memory arg_1C_0 = this.m;
				int arg_1C_1 = this.address + 4;
				int[] offsets = new int[1];
				return arg_27_0.ReadStringU(arg_1C_0.ReadInt(arg_1C_1, offsets), 256, true);
			}
		}
		public int Charges
		{
			get
			{
				return this.m.ReadInt(this.address + 24);
			}
		}
		public int SkillID
		{
			get
			{
				return this.m.ReadInt(this.address + 36);
			}
		}
	}
}
