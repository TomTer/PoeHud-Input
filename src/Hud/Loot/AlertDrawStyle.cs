using System.Drawing;
using System.Media;

namespace PoeHUD.Hud.Loot
{
	public class AlertDrawStyle
	{

		public Color color;
		public int FrameWidth;
		public string Text;
		public int IconIndex;

		public MapIcon IconForMap;
		public string SoundFileName;
		public SoundPlayer soundToPlay;
	}
}