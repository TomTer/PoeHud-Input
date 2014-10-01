using System.Drawing;

namespace PoeHUD.Hud.Loot
{
	public class AlertDrawStyle
	{
		public static readonly Color MagicColor = Color.FromArgb(136, 136, 255);
		public static readonly Color RareColor = Color.FromArgb(255, 255, 119);
		public static readonly Color CurrencyColor = Color.FromArgb(170, 158, 130);
		public static readonly Color UniqueColor = Color.FromArgb(175, 96, 37);
		public static readonly Color SkillGemColor = Color.FromArgb(26, 162, 155);


		public Color color;
		public int FrameWidth;
		public string Text;
		public int IconIndex;
	}
}