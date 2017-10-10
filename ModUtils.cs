using System;
using Microsoft.Xna.Framework.Graphics;

namespace CheatSheet
{
	internal static class ModUtils
	{
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
				result = Texture2D.FromStream(texture.GraphicsDevice, ms);
			}
			return result;
		}
	}
}
