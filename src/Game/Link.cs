using System;

namespace PoeHUD.Game
{
	public class Link
	{
		public static readonly Link EmptyLink = new Link(new Socket[0]);
		private Socket[] link;
		public int Length
		{
			get
			{
				return this.link.Length;
			}
		}
		public int NumberOfRed
		{
			get;
			private set;
		}
		public int NumberOfGreen
		{
			get;
			private set;
		}
		public int NumberOfBlue
		{
			get;
			private set;
		}
		public Link(Socket[] sockets)
		{
			this.link = sockets;
			this.CountColors();
		}
		public Link(string sockets)
		{
			this.link = new Socket[sockets.Length];
			for (int i = 0; i < sockets.Length; i++)
			{
				this.link[i] = this.CharToSocket(sockets.ToCharArray()[i]);
			}
			this.CountColors();
		}
		private void CountColors()
		{
			Socket[] array = this.link;
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case Socket.Red:
					this.NumberOfRed++;
					break;
				case Socket.Green:
					this.NumberOfGreen++;
					break;
				case Socket.Blue:
					this.NumberOfBlue++;
					break;
				}
			}
		}
		private Socket CharToSocket(char s)
		{
			char c = char.ToUpper(s);
			if (c == 'B')
			{
				return Socket.Blue;
			}
			if (c == 'G')
			{
				return Socket.Green;
			}
			if (c == 'R')
			{
				return Socket.Red;
			}
			throw new Exception("Invalid socket char: " + s);
		}
		public bool Contains(Link other)
		{
			return other.NumberOfRed <= this.NumberOfRed && other.NumberOfGreen <= this.NumberOfGreen && other.NumberOfBlue <= this.NumberOfBlue;
		}
	}
}
