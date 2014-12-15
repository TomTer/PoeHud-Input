using System;
using PoeHUD.Framework;

namespace PoeHUD.Poe
{
	public abstract class RemoteMemoryObject
	{
		public Memory M;
		public int Address;
		public TheGame Game;

		protected RemoteMemoryObject()
		{
		}

		protected Offsets Offsets { get { return this.M.offsets; } }

		public T GetObject<T>(int address) where T : RemoteMemoryObject, new()
		{
			T t = Activator.CreateInstance<T>();
			t.M = this.M;
			t.Address = address;
			t.Game = this.Game;
			return t;
		}

		public virtual T ReadObjectAt<T>(int offset) where T : RemoteMemoryObject, new()
		{
			return ReadObject<T>(Address + offset);
		}
		public T ReadObject<T>(int address) where T : RemoteMemoryObject, new()
		{
			T t = Activator.CreateInstance<T>();
			t.M = this.M;
			t.Address = this.M.ReadInt(address);
			t.Game = this.Game;
			return t;
		}

		public T AsObject<T>() where T : RemoteMemoryObject, new()
		{
			T t = Activator.CreateInstance<T>();
			t.M = this.M;
			t.Address = this.Address;
			t.Game = this.Game;
			return t;
		}
		public override bool Equals(object obj)
		{
			RemoteMemoryObject remoteMemoryObject = obj as RemoteMemoryObject;
			return remoteMemoryObject != null && remoteMemoryObject.Address == this.Address;
		}
		public override int GetHashCode()
		{
			return this.Address + base.GetType().Name.GetHashCode();
		}
	}
}
