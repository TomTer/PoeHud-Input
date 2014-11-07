using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PoeHUD.Framework;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud
{
	public class TextureRenderer
	{
		private struct ColoredVertex
		{
			public float X;
			public float Y;
			public float Z;
			public int Color;
			public static VertexFormat Format
			{
				get
				{
					return VertexFormat.Position | VertexFormat.Diffuse;
				}
			}
			public ColoredVertex(float x, float y, float z, Color color)
			{
				X = x;
				Y = y;
				Z = z;
				Color = color.ToArgb();
			}
			public ColoredVertex(float x, float y, Color color)
			{
				this = new ColoredVertex(x, y, 0f, color);
			}

			public static readonly VertexElement[] VertexElements = new VertexElement[] {
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};
		}
		private struct ColoredTexturedVertex
		{
			public readonly float X;
			public readonly float Y;
			public readonly float U;
			public readonly float V;
			public readonly int Color;
			public static VertexFormat Format
			{
				get
				{
					return VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1;
				}
			}
			public ColoredTexturedVertex(float x, float y, float u, float v, Color color)
			{
				X = x;
				Y = y;
				U = u;
				V = v;
				Color = color.ToArgb();
			}

			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 8, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};
		}
		private Sprite sprite;
		private Device dx;
		private Dictionary<string, Texture> textures;
		public TextureRenderer(Device dx)
		{
			this.dx = dx;
			sprite = new Sprite(dx);
			textures = new Dictionary<string, Texture>();
		}
		public void Begin()
		{
			sprite.Begin(SpriteFlags.None);
		}

		private Texture GetTextureFrom(string fileName)
		{
			fileName = "textures/" + fileName;
			if (!textures.ContainsKey(fileName))
			{
				try
				{
					textures.Add(fileName, Texture.FromFile(dx, fileName));
				}
				catch (Exception ex)
				{
					MessageBox.Show("Failed to load texture " + fileName + ": " + ex.Message);
					Environment.Exit(0);
				}
			}
			var tex = textures[fileName];
			return tex;
		}

		public void DrawTexture(string fileName, Rect position)
		{
			DrawTexture(GetTextureFrom(fileName), position, Color.White);
		}

		public void DrawTexture(string fileName, Rect position, Color color)
		{
			DrawTexture(GetTextureFrom(fileName), position, color);
		}

		public void DrawTexture(Texture texture, Rect position)
		{
			DrawTexture(texture, position, Color.White);
		}

		public void DrawTexture(Texture texture, Rect rect, Color color)
		{
			ColoredTexturedVertex[] data = {
				new ColoredTexturedVertex(rect.X, rect.Y, 0f, 0f, color),
				new ColoredTexturedVertex(rect.X + rect.W, rect.Y, 1f, 0f, color),
				new ColoredTexturedVertex(rect.X + rect.W, rect.Y + rect.H, 1f, 1f, color),
				new ColoredTexturedVertex(rect.X, rect.Y + rect.H, 0f, 1f, color)
			};
			dx.SetTexture(0, texture);
			SendVerticesToDevice(data, 2, PrimitiveType.TriangleFan, ColoredTexturedVertex.VertexElements);
		}

		public void DrawSprite(string fileName, Rect rect, RectUV uvCoords)
		{
			DrawSprite(GetTextureFrom(fileName), rect, uvCoords, Color.White);
		}

		public void DrawSprite(string fileName, Rect rect, RectUV uvCoords, Color color)
		{
			DrawSprite(GetTextureFrom(fileName), rect, uvCoords, color);
		}

		public void DrawSprite(Texture texture, Rect rect, RectUV uvCoords)
		{
			DrawSprite(texture, rect, uvCoords, Color.White);
		}

		public void DrawSprite(Texture texture, Rect rect, RectUV uvCoords, Color color)
		{
			ColoredTexturedVertex[] data = {
				new ColoredTexturedVertex(rect.X, rect.Y, uvCoords.U1, uvCoords.V1, color),
				new ColoredTexturedVertex(rect.X + rect.W, rect.Y, uvCoords.U2, uvCoords.V1, color),
				new ColoredTexturedVertex(rect.X + rect.W, rect.Y + rect.H, uvCoords.U2, uvCoords.V2, color),
				new ColoredTexturedVertex(rect.X, rect.Y + rect.H, uvCoords.U1, uvCoords.V2, color)
			};
			dx.SetTexture(0, texture);
			SendVerticesToDevice(data, 2, PrimitiveType.TriangleFan, ColoredTexturedVertex.VertexElements);
		}


		public void DrawBox(Rect rect, Color color)
		{
			ColoredVertex[] data = {
				new ColoredVertex(rect.X, rect.Y, color),
				new ColoredVertex(rect.X + rect.W, rect.Y, color),
				new ColoredVertex(rect.X + rect.W, rect.Y + rect.H, color),
				new ColoredVertex(rect.X, rect.Y + rect.H, color)
			};
			dx.SetTexture(0, null);
			SendVerticesToDevice(data, 2, PrimitiveType.TriangleFan, ColoredVertex.VertexElements);
		}

		private void SendVerticesToDevice<T>(T[] data, int cntPrimitives, PrimitiveType primitiveType, VertexElement[] structFields) where T : struct
		{
			using (VertexDeclaration declaration = new VertexDeclaration(dx, structFields))
			{
				VertexDeclaration vertexDeclaration = dx.VertexDeclaration;
				dx.VertexDeclaration = declaration;
				dx.DrawUserPrimitives<T>(primitiveType, cntPrimitives, data);
				dx.VertexDeclaration = vertexDeclaration;
			}
		}

		public void DrawHollowBox(Rect rect, int frameWidth, Color color)
		{
			var p1 = new ColoredVertex(rect.X, rect.Y, color);
			var p2 = new ColoredVertex(rect.X + rect.W, rect.Y, color);
			var p3 = new ColoredVertex(rect.X + rect.W, rect.Y + rect.H, color);
			var p4 = new ColoredVertex(rect.X, rect.Y + rect.H, color);

			var p5 = new ColoredVertex((float)rect.X + frameWidth, (float)rect.Y + frameWidth, color);
			var p6 = new ColoredVertex((float)(rect.X + rect.W) - frameWidth, (float)rect.Y + frameWidth, color);
			var p7 = new ColoredVertex((float)(rect.X + rect.W) - frameWidth, (float)(rect.Y + rect.H) - frameWidth, color);
			var p8 = new ColoredVertex((float)rect.X + frameWidth, (float)(rect.Y + rect.H) - frameWidth, color);

			ColoredVertex[] data = new ColoredVertex[] { p1, p5, p2, p6, p3, p7, p4, p8, p1, p5 };
			dx.SetTexture(0, null);
			SendVerticesToDevice(data, 8, PrimitiveType.TriangleStrip, ColoredVertex.VertexElements);
		}

		public void End()
		{
			sprite.End();
		}
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool AdjustWindowRect(ref Rectangle lpRect, uint dwStyle, bool bMenu);
		public void OnLostDevice()
		{
			foreach (Texture current in textures.Values)
			{
				current.Dispose();
			}
			textures.Clear();
			sprite.OnLostDevice();
		}
		public void OnResetDevice()
		{
			sprite.OnResetDevice();
		}
	}
}
