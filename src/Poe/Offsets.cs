using PoeHUD.Framework;

namespace PoeHUD.Poe
{
	public class Offsets
	{
		public string ExeName = "PathOfExile";

		public int IgsOffset;
		public int IgsDelta;

		public int Base;
		public int FileRoot;
		public int MaphackFunc;
		public int ZoomHackFunc;
		public int AreaChangeCount;
		public int Fullbright1;
		public int Fullbright2;


		public static Offsets Regular = new Offsets { IgsOffset = 0, IgsDelta = 0, ExeName = "PathOfExile" };
		public static Offsets Steam = new Offsets { IgsOffset = 24, IgsDelta = 0, ExeName = "PathOfExileSteam" };
		/* offsets from some older steam version: 
		 	Base = 8841968;
			FileRoot = 8820476;
			MaphackFunc = 4939552;
			ZoomHackFunc = 2225383;
			AreaChangeCount = 8730996;
			Fullbright1 = 7639804;
			Fullbright2 = 8217084;
		*/


		private static Pattern maphackPattern = new Pattern(new byte[]
		{
			81, 139, 70, 104, 139, 8, 104, 0, 32, 0, 0, 141, 84, 36, 4, 82, 
			106, 0, 106, 0, 80, 139, 65, 44, 255, 208, 139, 70, 72, 59, 70, 76 
		}, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
		
		private static Pattern zoomhackPattern = new Pattern(new byte[]
		{
			85, 139, 236, 131, 228, 248, 139, 69, 12, 131, 236, 44, 128, 56, 0, 83, 
			86, 87, 139, 217, 15, 133, 233, 0, 0, 0, 131, 187
		}, "xxxxxxxxxxxxxxxxxxxxxxxxxxxx");
		private static Pattern fullbrightPattern = new Pattern(new byte[]
		{
			85, 139, 236, 131, 228, 248, 106, 255, 104, 0, 0, 0, 0, 100, 161, 0,
			0, 0, 0, 80, 100, 137, 37, 0, 0, 0, 0, 129, 236, 160, 0, 0,
			0, 83, 139, 93, 16, 199, 68, 36, 68, 0, 0, 0, 0, 139
		}, "xxxxxxxxx????xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
		private static Pattern basePtrPattern = new Pattern(new byte[]
		{
			100, 161, 0, 0, 0, 0, 106, 255, 104, 0, 0, 0, 0, 80, 100, 137,
			37, 0, 0, 0, 0, 161, 0, 0, 0, 0, 129, 236, 144, 0, 0, 0,
			83, 85, 86, 87, 51, 255, 59, 199
		}, "xxxxxxxxx????xxxxxxxxx????xxxxxxxxxxxxxx");
		private static Pattern fileRootPattern = new Pattern(new byte[]
		{
			106, 255, 104, 0, 0, 0, 0, 100, 161, 0, 0, 0, 0, 80, 100, 137,
			37, 0, 0, 0, 0, 131, 236, 48, 255, 5, 0, 0, 0, 0, 83, 85,
			139, 45, 0, 0, 0, 0, 86, 184
		}, "xxx????xxxxxxxxxxxxxxxxxxx????xxxx????xx");
		private static Pattern areaChangePattern = new Pattern(new byte[]
		{
			139, 9, 137, 8, 133, 201, 116, 12, 255, 65, 40, 139, 21, 0, 0, 0,
			0, 137, 81, 36, 195, 204
		}, "xxxxxxxxxxxxx????xxxxx");
		public void DoPatternScans(Memory m)
		{
			int[] array = m.FindPatterns(new Pattern[]
			{
				Offsets.maphackPattern,
				Offsets.zoomhackPattern,
				Offsets.fullbrightPattern,
				Offsets.basePtrPattern,
				Offsets.fileRootPattern,
				Offsets.areaChangePattern
			});
			MaphackFunc = array[0];
			ZoomHackFunc = array[1] + 247;
			Fullbright1 = m.ReadInt(m.BaseAddress + array[2] + 1487) - m.BaseAddress;
			Fullbright2 = m.ReadInt(m.BaseAddress + array[2] + 1573) - m.BaseAddress;
			Base = m.ReadInt(m.BaseAddress + array[3] + 22) - m.BaseAddress;
			FileRoot = m.ReadInt(m.BaseAddress + array[4] + 40) - m.BaseAddress;
			AreaChangeCount = m.ReadInt(m.BaseAddress + array[5] + 13) - m.BaseAddress;
		}
	}
}
