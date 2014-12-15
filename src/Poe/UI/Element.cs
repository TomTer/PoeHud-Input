using System.Collections.Generic;
using System.Linq;
using PoeHUD.Framework;

namespace PoeHUD.Poe.UI
{
	public class Element : RemoteMemoryObject
	{
		public const int OffsetBuffers = 0x808;
		public const int UiElementSize = 0x15C;
		// dd id
		// dd (something zero)
		// 16 dup <128-bytes structure>
		// then the rest is

		public int vTable { get { return M.ReadInt(this.Address + 0); } }

		public Element Root { get { return base.ReadObject<Element>(this.Address + 0x5c + OffsetBuffers); } }
		public Element Parent { get { return base.ReadObject<Element>(this.Address + 0x60 + OffsetBuffers); } }

		public Element HintToChatLink { get { return base.ReadObject<Element>(this.Address + 0x80 + OffsetBuffers); } }

		public float ScrollY { get { return this.M.ReadFloat(this.Address + 0x2C + OffsetBuffers); } }
		public float Scale { get { return this.M.ReadFloat(this.Address + 0xD8 + OffsetBuffers); } }
		
		public float X { get { return this.M.ReadFloat(this.Address + 0x64 + OffsetBuffers); } }
		public float Y { get { return this.M.ReadFloat(this.Address + 0x68 + OffsetBuffers); } }
		public float Width { get { return this.M.ReadFloat(this.Address + 0xF0 + OffsetBuffers); } }
		public float Height { get { return this.M.ReadFloat(this.Address + 0xF4 + OffsetBuffers); } }
		
		// +110, 114 contain reference to texture
		// +11C points to array of textures

		// +160 contains reference to an object located next. Must be some kind of model (since it contains refs to children)

		public int ChildCount
		{
			get
			{
				return (this.M.ReadInt(this.Address + 0x14 + OffsetBuffers) - this.M.ReadInt(this.Address + 0x10 + OffsetBuffers)) / 4;
			}
		}
		public bool IsVisibleLocal
		{
			get {
				return (this.M.ReadInt(this.Address + 0x58 +  OffsetBuffers) & 1) == 1;
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
				if (this.M.ReadInt(this.Address + listOffset + 4) == 0 || this.M.ReadInt(this.Address + listOffset) == 0 || this.ChildCount > 1000)
				{
					return list;
				}
				for (int i = 0; i < this.ChildCount; i++)
				{
					list.Add(base.GetObject<Element>(this.M.ReadInt(this.Address + listOffset, i * 4)));
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
			while (!hashSet.Contains(parent) && root.Address != parent.Address && parent.Address != 0)
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
				num2 += current.Y + current.ScrollY;
			}
			return new Vec2f(num, num2);
		}

		public Rect GetClientRect()
		{
			Vec2f vPos = GetParentPos();
			float width = this.Game.IngameState.Camera.Width;
			float height = this.Game.IngameState.Camera.Height;
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
			return index >= this.ChildCount ? null : base.GetObject<Element>(this.M.ReadInt(this.Address + OffsetBuffers + 0x10, index * 4));
		}

		public virtual T ReadObjectAfterBuffers<T>(int offet) where T : RemoteMemoryObject, new()
		{
			return base.ReadObjectAt<T>(offet + OffsetBuffers);
		}
	}
}
