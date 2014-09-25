using System.Collections.Generic;
using ExileHUD.Framework;

namespace ExileHUD.ExileBot
{
	public class Poe_UIElement : RemoteMemoryObject
	{
		public float Width
		{
			get
			{
				return this.m.ReadFloat(this.address + 2296);
			}
		}
		public float Height
		{
			get
			{
				return this.m.ReadFloat(this.address + 2300);
			}
		}
		public float X
		{
			get
			{
				return this.m.ReadFloat(this.address + 2156);
			}
		}
		public float Y
		{
			get
			{
				return this.m.ReadFloat(this.address + 2160);
			}
		}
		public int ChildCount
		{
			get
			{
				return (this.m.ReadInt(this.address + 2076) - this.m.ReadInt(this.address + 2072)) / 4;
			}
		}
		public Poe_UIElement Root
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 2148);
			}
		}
		public Poe_UIElement Parent
		{
			get
			{
				return base.ReadObject<Poe_UIElement>(this.address + 2152);
			}
		}
		public bool IsVisible
		{
			get
			{
				if ((this.m.ReadInt(this.address + 2144) & 1) != 1)
				{
					return false;
				}
				foreach (Poe_UIElement current in this.GetParentChain())
				{
					if ((this.m.ReadInt(current.address + 2144) & 1) != 1)
					{
						return false;
					}
				}
				return true;
			}
		}
		public List<Poe_UIElement> Children
		{
			get
			{
				List<Poe_UIElement> list = new List<Poe_UIElement>();
				if (this.m.ReadInt(this.address + 2076) == 0 || this.m.ReadInt(this.address + 2072) == 0 || this.ChildCount > 1000)
				{
					return list;
				}
				for (int i = 0; i < this.ChildCount; i++)
				{
					int address = this.m.ReadInt(this.address + 2072, new int[]
					{
						i * 4
					});
					list.Add(base.GetObject<Poe_UIElement>(address));
				}
				return list;
			}
		}
		private List<Poe_UIElement> GetParentChain()
		{
			List<Poe_UIElement> list = new List<Poe_UIElement>();
			HashSet<Poe_UIElement> hashSet = new HashSet<Poe_UIElement>();
			Poe_UIElement root = this.Root;
			Poe_UIElement parent = this.Parent;
			while (!hashSet.Contains(parent) && root.address != parent.address && parent.address != 0)
			{
				list.Add(parent);
				hashSet.Add(parent);
				parent = parent.Parent;
			}
			return list;
		}
		public Rect GetClientRect()
		{
			float num = this.X;
			float num2 = this.Y;
			foreach (Poe_UIElement current in this.GetParentChain())
			{
				num += current.X;
				num2 += current.Y;
			}
			float width = this.game.IngameState.Camera.Width;
			float height = this.game.IngameState.Camera.Height;
			float num3 = width / 2560f;
			float num4 = height / 1600f;
			float num5 = width / height / 1.6f;
			num = num * num3 / num5;
			num2 *= num4;
			float num6 = num3 * this.Width / num5;
			float num7 = num4 * this.Height;
			return new Rect((int)num, (int)num2, (int)num6, (int)num7);
		}
		public Poe_UIElement GetChildFromIndices(params int[] indices)
		{
			Poe_UIElement poe_UIElement = this;
			for (int i = 0; i < indices.Length; i++)
			{
				int index = indices[i];
				poe_UIElement = poe_UIElement.GetChildAtIndex(index);
				if (poe_UIElement == null)
				{
					return poe_UIElement;
				}
			}
			return poe_UIElement;
		}
		public Poe_UIElement GetChildAtIndex(int index)
		{
			if (index >= this.ChildCount)
			{
				return null;
			}
			return base.GetObject<Poe_UIElement>(this.m.ReadInt(this.address + 2072, new int[]
			{
				index * 4
			}));
		}
	}
}
