using BotFramework;
using System;
namespace ExileBot
{
	public abstract class RemoteMemoryObject
	{
		public Memory m;
		public int address;
		public Poe_Game game;
		public RemoteMemoryObject()
		{
		}
		public T GetObject<T>(int address) where T : RemoteMemoryObject, new()
		{
			T t = Activator.CreateInstance<T>();
			t.m = this.m;
			t.address = address;
			t.game = this.game;
			return t;
		}
		public T ReadObject<T>(int address) where T : RemoteMemoryObject, new()
		{
			T t = Activator.CreateInstance<T>();
			t.m = this.m;
			t.address = this.m.ReadInt(address);
			t.game = this.game;
			return t;
		}
		public T AsObject<T>() where T : RemoteMemoryObject, new()
		{
			T t = Activator.CreateInstance<T>();
			t.m = this.m;
			t.address = this.address;
			t.game = this.game;
			return t;
		}
		public override bool Equals(object obj)
		{
			RemoteMemoryObject remoteMemoryObject = obj as RemoteMemoryObject;
			return remoteMemoryObject != null && remoteMemoryObject.address == this.address;
		}
		public override int GetHashCode()
		{
			return this.address + base.GetType().Name.GetHashCode();
		}
	}
}
