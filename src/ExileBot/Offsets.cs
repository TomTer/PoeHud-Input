//#define STEAM
using ExileHUD.Framework;

namespace ExileHUD.ExileBot
{
	public class Offsets
	{
#if STEAM
		public const string ProcessName = "PathOfExileSteam";
		public const int ISOffset = 28;
		public const int ISDelta = 4;
#else
		public const string ProcessName = "PathOfExile";
		public const int ISOffset = 0;
		public const int ISDelta = 0;
#endif
		public static int Base = 8825704;
		public static int FileRoot = 8804204;
		public static int MaphackFunc = 4927600;
		public static int ZoomHackFunc = 2215847;
		public static int AreaChangeCount = 8714612;
		public static int Fullbright1 = 7627500;
		public static int Fullbright2 = 8206296;


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
			85,
			139,
			236,
			131,
			228,
			248,
			106,
			255,
			104,
			0,
			0,
			0,
			0,
			100,
			161,
			0,
			0,
			0,
			0,
			80,
			100,
			137,
			37,
			0,
			0,
			0,
			0,
			129,
			236,
			160,
			0,
			0,
			0,
			83,
			139,
			93,
			16,
			199,
			68,
			36,
			68,
			0,
			0,
			0,
			0,
			139
		}, "xxxxxxxxx????xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
		private static Pattern basePtrPattern = new Pattern(new byte[]
		{
			100,
			161,
			0,
			0,
			0,
			0,
			106,
			255,
			104,
			0,
			0,
			0,
			0,
			80,
			100,
			137,
			37,
			0,
			0,
			0,
			0,
			161,
			0,
			0,
			0,
			0,
			129,
			236,
			144,
			0,
			0,
			0,
			83,
			85,
			86,
			87,
			51,
			255,
			59,
			199
		}, "xxxxxxxxx????xxxxxxxxx????xxxxxxxxxxxxxx");
		private static Pattern fileRootPattern = new Pattern(new byte[]
		{
			106,
			255,
			104,
			0,
			0,
			0,
			0,
			100,
			161,
			0,
			0,
			0,
			0,
			80,
			100,
			137,
			37,
			0,
			0,
			0,
			0,
			131,
			236,
			48,
			255,
			5,
			0,
			0,
			0,
			0,
			83,
			85,
			139,
			45,
			0,
			0,
			0,
			0,
			86,
			184
		}, "xxx????xxxxxxxxxxxxxxxxxxx????xxxx????xx");
		private static Pattern areaChangePattern = new Pattern(new byte[]
		{
			139,
			9,
			137,
			8,
			133,
			201,
			116,
			12,
			255,
			65,
			40,
			139,
			21,
			0,
			0,
			0,
			0,
			137,
			81,
			36,
			195,
			204
		}, "xxxxxxxxxxxxx????xxxxx");
		public static void DoPatternScans(Memory m)
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
			Offsets.MaphackFunc = array[0];
			Offsets.ZoomHackFunc = array[1] + 247;
			Offsets.Fullbright1 = m.ReadInt(m.BaseAddress + array[2] + 1487) - m.BaseAddress;
			Offsets.Fullbright2 = m.ReadInt(m.BaseAddress + array[2] + 1573) - m.BaseAddress;
			Offsets.Base = m.ReadInt(m.BaseAddress + array[3] + 22) - m.BaseAddress;
			Offsets.FileRoot = m.ReadInt(m.BaseAddress + array[4] + 40) - m.BaseAddress;
			Offsets.AreaChangeCount = m.ReadInt(m.BaseAddress + array[5] + 13) - m.BaseAddress;
		}
	}
}
