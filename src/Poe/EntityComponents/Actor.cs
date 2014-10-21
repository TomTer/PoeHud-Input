using System.Collections.Generic;

namespace PoeHUD.Poe.EntityComponents
{
	public class Actor : Component
	{
        /// <summary>
        /// Standing still = 2048 =bit 11 set
        /// running = 2178 = bit 11 & 7
        /// Maybe Bit-field : Bit 7 set = running 
        /// </summary>
        public int ActionId
        {
            get
            {
                if (this.address != 0)
                {
                    return this.m.ReadInt(this.address + 0x9C);
                }
                return 1;
            }
        }

        public bool isMoving
        {
            get
            {
                return (ActionId & 128) > 0;
            }
        }

		public List<int> Minions
		{
			get
			{
				List<int> list = new List<int>();
				if (this.address == 0)
				{
					return list;
				}
				int num = this.m.ReadInt(this.address + 656);
				int num2 = this.m.ReadInt(this.address + 660);
				for (int i = num; i < num2; i += 8)
				{
					int item = this.m.ReadInt(i);
					list.Add(item);
				}
				return list;
			}
		}
		public bool HasMinion(Entity entity)
		{
			if (this.address == 0)
			{
				return false;
			}
			int num = this.m.ReadInt(this.address + 656);
			int num2 = this.m.ReadInt(this.address + 660);
			for (int i = num; i < num2; i += 8)
			{
				int num3 = this.m.ReadInt(i);
				if (num3 == entity.ID)
				{
					return true;
				}
			}
			return false;
		}
	}
}
