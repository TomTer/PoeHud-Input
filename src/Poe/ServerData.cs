namespace PoeHUD.Poe
{
	public class ServerData : RemoteMemoryObject
	{
		// 5x addresses (ref to method)
		// dd 0
		// UI_buffers : 26 times

		public const int SubscribersLength = 0x2A00 + 0x14; 

		public bool IsInGame
		{
			get
			{
				return this.M.ReadInt(this.Address + 0x2D80) == 3; // 2A78
			}
		}

		public int PtrObjectRegister { get { return this.M.ReadInt(0x10 + SubscribersLength); } }
		public int PtrPassiveSkillGraph { get { return this.M.ReadInt(0x30 + SubscribersLength); } }
	}
}
