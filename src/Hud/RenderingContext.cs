using System;
using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud
{
	public class RenderingContext
	{
		public Device Dx;
		private readonly Sprite textSprite;
		private readonly GameWindow window;
		private readonly TextureRenderer texrenderer;
		private readonly Dictionary<int, SlimDX.Direct3D9.Font> fonts = new Dictionary<int, SlimDX.Direct3D9.Font>();
		
		public Action<RenderingContext> RenderModules;
		public RenderingContext(Device dx, GameWindow window)
		{
			this.Dx = dx;
			this.window = window;
			this.textSprite = new Sprite(dx);
			this.texrenderer = new TextureRenderer(dx);
		}
		public void RenderFrame()
		{
			this.textSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);
			this.texrenderer.Begin();
			if (this.RenderModules != null) {
				this.RenderModules(this);
			}
			this.texrenderer.End();
			this.textSprite.End();
		}
		public void AddTexture(string file, Rect rect)
		{
			this.texrenderer.DrawTexture(file, rect);
		}
		public void AddTexture(string file, Rect rect, Color color)
		{
			this.texrenderer.DrawTexture(file, rect, color);
		}

		public void AddSprite(string file, Rect rect, RectUV uv)
		{
			this.texrenderer.DrawSprite(file, rect, uv);
		}
		public void AddSprite(string file, Rect rect, RectUV uv, Color color)
		{
			this.texrenderer.DrawSprite(file, rect, uv, color);
		}
		public void AddBox(Rect rect, Color color)
		{
			this.texrenderer.DrawBox(rect, color);
		}
		public void AddFrame(Rect rect, Color color, int width = 1)
		{
			this.texrenderer.DrawHollowBox(rect, width, color);
		}
		public Vec2 AddTextWithHeight(Vec2 pos, string text, Color color, int height, DrawTextFormat format)
		{
			SlimDX.Direct3D9.Font font = this.GetFont(height);
			Rectangle rectangle = font.MeasureString(this.textSprite, text, format);
			rectangle.X += pos.X;
			rectangle.Y += pos.Y;
			font.DrawString(this.textSprite, text, rectangle, format, color);
			return new Vec2(rectangle.Width, rectangle.Height);
		}

		public Vec2 MeasureString(string text, int height, DrawTextFormat format)
		{
			SlimDX.Direct3D9.Font font = this.GetFont(height);
			Rectangle rectangle = font.MeasureString(this.textSprite, text, format);
			return new Vec2(rectangle.Width, rectangle.Height);
		}

		public Vec2 AddTextWithHeightAndOutline(Vec2 pos, string text, Color color, Color outLine, int height, DrawTextFormat format, int outlineOsset = 1)
		{
			SlimDX.Direct3D9.Font font = this.GetFont(height);
			Rectangle rectangle = font.MeasureString(this.textSprite, text, format);
			rectangle.X += pos.X;
			rectangle.Y += pos.Y;

			rectangle.X -= 1 * outlineOsset;
			rectangle.Y -= 1 * outlineOsset;
			font.DrawString(this.textSprite, text, rectangle, format, outLine);
			rectangle.Y += 2 * outlineOsset;
			font.DrawString(this.textSprite, text, rectangle, format, outLine);
			rectangle.X += 2 * outlineOsset;
			font.DrawString(this.textSprite, text, rectangle, format, outLine);
			rectangle.Y -= 2 * outlineOsset;
			font.DrawString(this.textSprite, text, rectangle, format, outLine);

			rectangle.X -= 1 * outlineOsset;
			rectangle.Y += 1 * outlineOsset;
			font.DrawString(this.textSprite, text, rectangle, format, color);
			return new Vec2(rectangle.Width, rectangle.Height);
		}

		private bool Validate(params Vec2[] vertices)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				Vec2 v = vertices[i];
				if (!this.window.ClientRect().HasPoint(v))
				{
					return false;
				}
			}
			return true;
		}
		private SlimDX.Direct3D9.Font GetFont(int size)
		{
			if (this.fonts.ContainsKey(size))
			{
				return this.fonts[size];
			}
			System.Drawing.Font font = new System.Drawing.Font("Verdana", (float)size);
			SlimDX.Direct3D9.Font font2 = new SlimDX.Direct3D9.Font(this.Dx, font);
			this.fonts.Add(size, font2);
			return font2;
		}
		public void OnLostDevice()
		{
			foreach (SlimDX.Direct3D9.Font current in this.fonts.Values)
			{
				current.OnLostDevice();
			}
			this.texrenderer.OnLostDevice();
			this.textSprite.OnLostDevice();
		}
		public void OnResetDevice()
		{
			foreach (SlimDX.Direct3D9.Font current in this.fonts.Values)
			{
				current.OnResetDevice();
			}
			this.texrenderer.OnResetDevice();
			this.textSprite.OnResetDevice();
		}
	}
}
