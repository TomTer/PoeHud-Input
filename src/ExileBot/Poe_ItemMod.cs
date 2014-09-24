using BotFramework;
using System;
namespace ExileBot
{
	public class Poe_ItemMod : RemoteMemoryObject
	{
		private string name;
		private int level;
		public int Value1
		{
			get
			{
				Memory arg_14_0 = this.m;
				int arg_14_1 = this.address;
				int[] offsets = new int[1];
				return arg_14_0.ReadInt(arg_14_1, offsets);
			}
		}
		public int Value2
		{
			get
			{
				return this.m.ReadInt(this.address, new int[]
				{
					4
				});
			}
		}
		public string Name
		{
			get
			{
				if (this.name == null)
				{
					this.ParseName();
				}
				return this.name;
			}
		}
		public int Level
		{
			get
			{
				if (this.name == null)
				{
					this.ParseName();
				}
				return this.level;
			}
		}
		private void ParseName()
		{
			Memory arg_29_0 = this.m;
			Memory arg_1E_0 = this.m;
			int arg_1E_1 = this.address + 20;
			int[] offsets = new int[1];
			this.name = arg_29_0.ReadStringU(arg_1E_0.ReadInt(arg_1E_1, offsets), 256, true);
			if (!int.TryParse(this.name.Substring(this.name.Length - 1), out this.level))
			{
				this.level = 1;
			}
			else
			{
				this.name = this.name.Substring(0, this.name.Length - 1);
			}
			if (this.name.EndsWith("AndStunRecovery"))
			{
				this.name = this.name.Replace("AndStunRecovery", "");
				return;
			}
			if (this.name.EndsWith("AndAccuracyRating"))
			{
				this.name = this.name.Replace("AndAccuracyRating", "");
				return;
			}
			if (this.name.EndsWith("OnWeapon"))
			{
				this.name = this.name.Replace("OnWeapon", "");
				return;
			}
			if (this.name.EndsWith("Prefix"))
			{
				this.name = this.name.Replace("Prefix", "");
			}
		}
	}
}
