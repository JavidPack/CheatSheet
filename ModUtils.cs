using System;
using CheatSheet.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet
{
	internal static class ModUtils
	{
		internal static Asset<Texture2D> GetItemTexture(int id)
		{
			Main.instance.LoadItem(id);

			return TextureAssets.Item[id];
		}

		// TODO: Temp until tmod is fixed
		internal static bool AcceptedByItemGroups(this Recipe recipe, int invType, int reqType)
		{
			for (int i = 0; i < RecipeBrowserWindow.maxRequirementsOld; i++)
			{
				int num = recipe.acceptedGroups[i];
				if (num == -1)
					break;

				if (RecipeGroup.recipeGroups[num].ContainsItem(invType) && RecipeGroup.recipeGroups[num].ContainsItem(reqType))
					return true;
			}

			return false;
		}

		internal static bool ProcessGroupsForText(this Recipe recipe, int type, out string theText)
		{
			for (int i = 0; i < recipe.acceptedGroups.Count; i++)
			{
				int num = recipe.acceptedGroups[i];
				if (num == -1)
					break;

				if (RecipeGroup.recipeGroups[num].ContainsItem(type))
				{
					theText = RecipeGroup.recipeGroups[num].GetText();
					return true;
				}
			}

			theText = "";
			return false;
		}

		internal const int TextureMaxTile = 128;

		/*
		internal static Texture2D Resize(this Texture2D texture, int size)
		{
			Texture2D result = texture;

			float max = Math.Max(texture.Width, texture.Height);
			float scale = size / max;
			int width = (int)(texture.Width * scale);
			int height = (int)(texture.Height * scale);

			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				texture.SaveAsPng(ms, width, height);
				// Crashes Mac (maybe Linux?)
				result = Texture2D.FromStream(texture.GraphicsDevice, ms);
			}
			return result;
		}
		*/

		internal static Texture2D Resize(this Texture2D[,] textures, int size)
		{
			Texture2D result = new Texture2D(Main.graphics.GraphicsDevice, size, size);
			Color[] data1 = new Color[size * size];

			int maxX = textures.GetLength(0);
			int maxY = textures.GetLength(1);

			int maxWidth = 0;
			int maxHeight = 0;

			for (int x = 0; x < maxX; x++)
			{
				maxWidth += textures[x, 0].Width;
			}
			for (int y = 0; y < maxY; y++)
			{
				maxHeight += textures[0, y].Height;
			}

			float max = Math.Max(maxWidth, maxHeight);
			float scale = size / max;

			int[] widths = new int[maxX + 1];
			int[] heights = new int[maxY + 1];

			// TODO: Wait, shouldn't I just do this all in 1 draw call?
			for (int x = 0; x < maxX; x++)
			{
				for (int y = 0; y < maxY; y++)
				{
					Texture2D texture = textures[x, y];
					int width = (int)(texture.Width * scale);
					int height = (int)(texture.Height * scale);

					widths[x + 1] = widths[x] + width;
					heights[y + 1] = heights[y] + height;

					Texture2D texture2;
					if(Platform.IsWindows)
					//if (Terraria.ModLoader.ModLoader.windows)
					{
						using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
						{
							// This doesn't work on Mac/Linux but doesn't cause a screen flash.
							// TODO: Do this in an Update method so screen flash doesn't matter.
							texture.SaveAsPng(ms, width, height);
							texture2 = Texture2D.FromStream(texture.GraphicsDevice, ms);
						}
					}
					else
					{
						// Alt method for Mac/Linux, causes screen flash
						texture2 = ResizeTexture(texture, scale);
					}

					Color[] data2 = new Color[width * height];
					texture2.GetData<Color>(data2);
					for (int i = 0; i < height; i++)
					{
						for (int j = 0; j < width; j++)
						{
							data1[heights[y] * size + widths[x] + i * size + j] = data2[i * width + j];
						}
					}
				}
			}

			result.SetData(data1);

			return result;
		}

		private static Texture2D ResizeTexture(Texture2D texture, float scale)
		{
			int width = (int)(texture.Width * scale);
			int height = (int)(texture.Height * scale);

			Main.spriteBatch.End();
			RenderTarget2D renderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
			Main.instance.GraphicsDevice.SetRenderTarget(renderTarget);
			Main.instance.GraphicsDevice.Clear(Color.Transparent);
			Main.spriteBatch.Begin();

			Rectangle destinationRectangle = new Rectangle(
			0, 0, width, height);
			Main.spriteBatch.Draw(texture, destinationRectangle, Color.White);

			Main.spriteBatch.End();
			Main.instance.GraphicsDevice.SetRenderTarget(null);
			Main.spriteBatch.Begin();
			return renderTarget;
		}

		public static Texture2D Offset(this Texture2D texture, int x, int y, int width, int height)
		{
			Texture2D result = new Texture2D(Main.graphics.GraphicsDevice, width, height);
			Color[] data = new Color[height * width];
			texture.GetData(0, new Rectangle?(new Rectangle(x, y, width, height)), data, 0, data.Length);
			result.SetData(data);
			return result;
		}

		internal static Vector2 Offset(this Vector2 position, float x, float y)
		{
			position.X += x;
			position.Y += y;
			return position;
		}
	}
}
