using PoeHUD.Framework;

namespace PoeHUD.ExileBot
{
	public class Buff
	{
		private int address;
		private Memory Memory;
		public string Name
		{
			get
			{
				Memory arg_27_0 = this.Memory;
				Memory arg_1C_0 = this.Memory;
				int arg_1C_1 = this.address + 4;
				int[] offsets = new int[1];
				return arg_27_0.ReadStringU(arg_1C_0.ReadInt(arg_1C_1, offsets), 256, true);
			}
		}
		public int Charges
		{
			get
			{
				return this.Memory.ReadInt(this.address + 24);
			}
		}
		public int SkillID
		{
			get
			{
				return this.Memory.ReadInt(this.address + 36);
			}
		}
		public Buff(int address, Memory mem)
		{
			this.address = address;
			this.Memory = mem;
		}
	}
}
