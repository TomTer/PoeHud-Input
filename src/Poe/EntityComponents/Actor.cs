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
                if (this.Address != 0)
                {
                    return this.M.ReadInt(this.Address + 0x9C);
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
				if (this.Address == 0)
				{
					return list;
				}
				int num = this.M.ReadInt(this.Address + 656);
				int num2 = this.M.ReadInt(this.Address + 660);
				for (int i = num; i < num2; i += 8)
				{
					int item = this.M.ReadInt(i);
					list.Add(item);
				}
				return list;
			}
		}
		public bool HasMinion(Entity entity)
		{
			if (this.Address == 0)
			{
				return false;
			}
			int num = this.M.ReadInt(this.Address + 656);
			int num2 = this.M.ReadInt(this.Address + 660);
			for (int i = num; i < num2; i += 8)
			{
				int num3 = this.M.ReadInt(i);
				if (num3 == entity.ID)
				{
					return true;
				}
			}
			return false;
		}
	}
}
