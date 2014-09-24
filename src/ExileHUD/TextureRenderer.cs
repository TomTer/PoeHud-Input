using BotFramework;
using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace ExileHUD
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
				this.X = x;
				this.Y = y;
				this.Z = z;
				this.Color = color.ToArgb();
			}
			public ColoredVertex(float x, float y, Color color)
			{
				this = new TextureRenderer.ColoredVertex(x, y, 0f, color);
			}

			public static readonly VertexElement[] VertexElements = new VertexElement[] {
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};
		}
		private struct ColoredTexturedVertex
		{
			public float X;
			public float Y;
			public float Z;
			public float U;
			public float V;
			public int Color;
			public static VertexFormat Format
			{
				get
				{
					return VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1;
				}
			}
			public ColoredTexturedVertex(float x, float y, float z, float u, float v, Color color)
			{
				this.X = x;
				this.Y = y;
				this.Z = z;
				this.U = u;
				this.V = v;
				this.Color = color.ToArgb();
			}
			public ColoredTexturedVertex(float x, float y, float u, float v, Color color)
			{
				this = new TextureRenderer.ColoredTexturedVertex(x, y, 0f, u, v, color);
			}

			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};
		}
		private Sprite sprite;
		private Device dx;
		private Dictionary<string, Texture> textures;
		public TextureRenderer(Device dx)
		{
			this.dx = dx;
			this.sprite = new Sprite(dx);
			this.textures = new Dictionary<string, Texture>();
		}
		public void Begin()
		{
			this.sprite.Begin(SpriteFlags.None);
		}
		public void DrawTexture(string fileName, Rect position, Color color)
		{
			fileName = "textures/" + fileName;
			if (!this.textures.ContainsKey(fileName))
			{
				try
				{
					this.textures.Add(fileName, Texture.FromFile(this.dx, fileName));
				}
				catch (Exception ex)
				{
					MessageBox.Show("Failed to load texture " + fileName + ": " + ex.Message);
					Environment.Exit(0);
				}
			}
			this.DrawTexture(this.textures[fileName], position, color);
		}
		public void DrawTexture(string fileName, Rect position)
		{
			this.DrawTexture(fileName, position, Color.White);
		}
		public void DrawTexture(Texture texture, Rect position)
		{
			this.DrawTexture(texture, position);
		}
		public void DrawTexture(Texture texture, Rect rect, Color color)
		{
			TextureRenderer.ColoredTexturedVertex[] data = new TextureRenderer.ColoredTexturedVertex[]
			{
				new TextureRenderer.ColoredTexturedVertex((float)rect.X, (float)rect.Y, 0f, 0f, color),
				new TextureRenderer.ColoredTexturedVertex((float)(rect.X + rect.W), (float)rect.Y, 1f, 0f, color),
				new TextureRenderer.ColoredTexturedVertex((float)(rect.X + rect.W), (float)(rect.Y + rect.H), 1f, 1f, color),
				new TextureRenderer.ColoredTexturedVertex((float)rect.X, (float)(rect.Y + rect.H), 0f, 1f, color)
			};
			this.dx.SetTexture(0, texture);
			SendVerticesToDevice(data, 2, PrimitiveType.TriangleFan, TextureRenderer.ColoredTexturedVertex.VertexElements);
		}
		public void DrawBox(Rect rect, Color color)
		{
			TextureRenderer.ColoredVertex[] data = new TextureRenderer.ColoredVertex[]
			{
				new TextureRenderer.ColoredVertex((float)rect.X, (float)rect.Y, color),
				new TextureRenderer.ColoredVertex((float)(rect.X + rect.W), (float)rect.Y, color),
				new TextureRenderer.ColoredVertex((float)(rect.X + rect.W), (float)(rect.Y + rect.H), color),
				new TextureRenderer.ColoredVertex((float)rect.X, (float)(rect.Y + rect.H), color)
			};
			this.dx.SetTexture(0, null);
			SendVerticesToDevice(data, 2, PrimitiveType.TriangleFan, TextureRenderer.ColoredVertex.VertexElements);
		}

		private void SendVerticesToDevice<T>(T[] data, int cntPrimitives, PrimitiveType primitiveType, VertexElement[] structFields) where T : struct
		{
			using (VertexDeclaration declaration = new VertexDeclaration(dx, structFields))
			{
				VertexDeclaration vertexDeclaration = this.dx.VertexDeclaration;
				this.dx.VertexDeclaration = declaration;
				this.dx.DrawUserPrimitives<T>(primitiveType, cntPrimitives, data);
				this.dx.VertexDeclaration = vertexDeclaration;
			}
		}

		public void DrawHollowBox(Rect rect, int frameWidth, Color color)
		{
			var p1 = new TextureRenderer.ColoredVertex((float)rect.X, (float)rect.Y, color);
			var p2 = new TextureRenderer.ColoredVertex((float)(rect.X + rect.W), (float)rect.Y, color);
			var p3 = new TextureRenderer.ColoredVertex((float)(rect.X + rect.W), (float)(rect.Y + rect.H), color);
			var p4 = new TextureRenderer.ColoredVertex((float)rect.X, (float)(rect.Y + rect.H), color);

			var p5 = new TextureRenderer.ColoredVertex((float)rect.X + frameWidth, (float)rect.Y + frameWidth, color);
			var p6 = new TextureRenderer.ColoredVertex((float)(rect.X + rect.W) - frameWidth, (float)rect.Y + frameWidth, color);
			var p7 = new TextureRenderer.ColoredVertex((float)(rect.X + rect.W) - frameWidth, (float)(rect.Y + rect.H) - frameWidth, color);
			var p8 = new TextureRenderer.ColoredVertex((float)rect.X + frameWidth, (float)(rect.Y + rect.H) - frameWidth, color);

			TextureRenderer.ColoredVertex[] data = new TextureRenderer.ColoredVertex[] { p1, p5, p2, p6, p3, p7, p4, p8, p1, p5 };
			this.dx.SetTexture(0, null);
			SendVerticesToDevice(data, 8, PrimitiveType.TriangleStrip, ColoredVertex.VertexElements);
		}

		public void End()
		{
			this.sprite.End();
		}
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool AdjustWindowRect(ref Rectangle lpRect, uint dwStyle, bool bMenu);
		public void OnLostDevice()
		{
			foreach (Texture current in this.textures.Values)
			{
				current.Dispose();
			}
			this.textures.Clear();
			this.sprite.OnLostDevice();
		}
		public void OnResetDevice()
		{
			this.sprite.OnResetDevice();
		}
	}
}
