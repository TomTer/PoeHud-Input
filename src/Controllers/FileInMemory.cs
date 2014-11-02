using System.Collections.Generic;
using PoeHUD.Framework;

namespace PoeHUD.Controllers
{
	public class FileInMemory
	{
		protected readonly Memory m;
		protected readonly int address;

		public FileInMemory(Memory m, int address)
		{
			this.m = m;
			this.address = address;
		}

		public int NumberOfRecords { get { return this.m.ReadInt(this.address + 0x44); } }

		public IEnumerable<int> RecordAdresses()
		{
			int firstRec = this.m.ReadInt(this.address + 0x30);
			int lastRec = this.m.ReadInt(this.address + 0x34);
			int cnt = NumberOfRecords;

			int recLen = ( lastRec - firstRec ) / cnt;
			for (int i = 0; i < cnt; i++)
				yield return firstRec + i * recLen;
		}
	}
}