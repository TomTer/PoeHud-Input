using System.Collections.Generic;
using System.Linq;
using PoeHUD.Framework;

namespace PoeHUD.Poe.UI
{
	public class Element : RemoteMemoryObject
	{
		public const int OffsetBuffers = 0x808;
		// dd id
		// dd (something zero)
		// 16 dup <128-bytes structure>
		// then the rest is

		public int vTable { get { return m.ReadInt(this.address + 0); } }

		public Element Root { get { return base.ReadObject<Element>(this.address + 0x5c + OffsetBuffers); } }
		public Element Parent { get { return base.ReadObject<Element>(this.address + 0x60 + OffsetBuffers); } }
		public float X { get { return this.m.ReadFloat(this.address + 0x64 + OffsetBuffers); } }
		public float Y { get { return this.m.ReadFloat(this.address + 0x68 + OffsetBuffers); } }
		public float Width { get { return this.m.ReadFloat(this.address + 0xF0 + OffsetBuffers); } }
		public float Height { get { return this.m.ReadFloat(this.address + 0xF4 + OffsetBuffers); } }

		public int ChildCount
		{
			get
			{
				return (this.m.ReadInt(this.address + 20 + OffsetBuffers) - this.m.ReadInt(this.address + 16 + OffsetBuffers)) / 4;
			}
		}
		public bool IsVisibleLocal
		{
			get {
				return (this.m.ReadInt(this.address + 2144) & 1) == 1;
			}
		}

		public bool IsVisible
		{
			get
			{
				return IsVisibleLocal && this.GetParentChain().All(current => current.IsVisibleLocal);
			}
		}
		public List<Element> Children
		{
			get
			{
				const int listOffset = 0x10 + OffsetBuffers;
				List<Element> list = new List<Element>();
				if (this.m.ReadInt(this.address + listOffset + 4) == 0 || this.m.ReadInt(this.address + listOffset) == 0 || this.ChildCount > 1000)
				{
					return list;
				}
				for (int i = 0; i < this.ChildCount; i++)
				{
					list.Add(base.GetObject<Element>(this.m.ReadInt(this.address + listOffset, i * 4)));
				}
				return list;
			}
		}
		private IEnumerable<Element> GetParentChain()
		{
			List<Element> list = new List<Element>();
			HashSet<Element> hashSet = new HashSet<Element>();
			Element root = this.Root;
			Element parent = this.Parent;
			while (!hashSet.Contains(parent) && root.address != parent.address && parent.address != 0)
			{
				list.Add(parent);
				hashSet.Add(parent);
				parent = parent.Parent;
			}
			return list;
		}

		public Vec2f GetParentPos()
		{
			float num = 0;
			float num2 = 0;
			foreach (Element current in this.GetParentChain())
			{
				num += current.X;
				num2 += current.Y;
			}
			return new Vec2f(num, num2);
		}

		public Rect GetClientRect()
		{
			Vec2f vPos = GetParentPos();
			float width = this.game.IngameState.Camera.Width;
			float height = this.game.IngameState.Camera.Height;
			float ratioFixMult = width / height / 1.6f;
			float xScale = width / 2560f / ratioFixMult;
			float yScale = height / 1600f;
			
			float num = (vPos.X + this.X) * xScale;
			float num2 = (vPos.Y + this.Y) * yScale;
			return new Rect((int)num, (int)num2, (int)(xScale * this.Width), (int)(yScale * this.Height));
		}
		public Element GetChildFromIndices(params int[] indices)
		{
			Element poe_UIElement = this;
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
		public Element GetChildAtIndex(int index)
		{
			if (index >= this.ChildCount)
			{
				return null;
			}
			return base.GetObject<Element>(this.m.ReadInt(this.address + 2072, new int[]
			{
				index * 4
			}));
		}
	}
}
