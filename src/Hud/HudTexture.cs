using System.Drawing;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Hud.Loot;

namespace PoeHUD.Hud
{
	public class HudTexture
	{
		public string TextureFile;
		public Color TintColor;

		public HudTexture(string fileName) : this(fileName, Color.White) { }

		public HudTexture(string fileName, MonsterRarity rarity) : this(fileName, Color.White)
		{
			switch (rarity)
			{
				case MonsterRarity.Magic: TintColor = HudSkin.MagicColor; break;
				case MonsterRarity.Rare: TintColor = HudSkin.RareColor; break;
				case MonsterRarity.Unique: TintColor = HudSkin.UniqueColor; break;
			}
		}

		public HudTexture(string fileName, Color tintColor) 
		{
			this.TextureFile = fileName;
			TintColor = tintColor;
		}


		public void DrawAt(RenderingContext rc, Vec2 point, Rect rect)
		{
			rc.AddTexture(this.TextureFile, rect, TintColor);
		}
	}

	public class HudSprite : HudTexture
	{
		public Vec2 SpriteSize;
		public Vec2 ChosenImage;

		public HudSprite(string fileName, Vec2 chosenImage, Vec2 spriteSize) : this(fileName, Color.White, chosenImage, spriteSize) {}

		public HudSprite(string fileName, Color tintColor, Vec2 chosenImage, Vec2 spriteSize) : base(fileName, tintColor)
		{
			ChosenImage = chosenImage;
			SpriteSize = spriteSize;
		}

		public void DrawAt(RenderingContext rc, Vec2 point, Rect rect)
		{
			float xD = ((float)(SpriteSize.X)) / SpriteSize.X;
			float yD = ((float)(SpriteSize.Y)) / SpriteSize.Y;
			RectUV uvRect = new RectUV(xD * ChosenImage.X, yD * ChosenImage.Y, xD * (ChosenImage.X + 1), yD * (ChosenImage.Y + 1));
			rc.AddSprite(this.TextureFile, rect, uvRect, TintColor);
		}
	}
}
