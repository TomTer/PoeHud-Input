namespace PoeHUD.Poe.UI
{
	public class BigMinimap : Element
	{
		public Element SmallMinimap { get { return base.ReadObjectAt<Element>(0x164 + OffsetBuffers); } }
		
		// when this is visible, draw on large map
		public Element MapProperties { get { return base.ReadObjectAt<Element>(0x16C + OffsetBuffers); } }

		public Element OrangeWords { get { return base.ReadObjectAt<Element>(0x170 + OffsetBuffers); } }

		public Element BlueWords { get { return base.ReadObjectAt<Element>(0x188 + OffsetBuffers); } }


		/*
		 *         public float ShiftX
			{
				get
				{
					return M.ReadFloat(M.ReadInt(Address+0x160 + OffsetBuffers) + 0x96C);
				}
			}

			public float ShiftY
			{
				get
				{
					return M.ReadFloat(M.ReadInt(Address+0x160 + OffsetBuffers) + 0x970);
				}
			}
		 */
	}
}
