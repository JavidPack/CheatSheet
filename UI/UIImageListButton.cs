using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace CheatSheet.UI
{
	internal class ImageList
	{
		public List<Texture2D> listTexture = new List<Texture2D>();
		public ImageList(Texture2D texture, int width, int height)
		{
			for (int y = 0; y < texture.Height / height; y++)
			{
				for (int x = 0; x < texture.Width / width; x++)
				{
					listTexture.Add(texture.Offset(x * width, y * height, width, height));
				}
			}
		}
	}

	internal class UIImageListButton : UIView
	{
		private List<Asset<Texture2D>> _textures;
		private List<string> _hoverTexts;
		private List<object> _values;
		internal float visibilityActive = 1f;
		internal float visibilityInactive = 0.4f;
		internal int Index { get; set; }

		public T GetValue<T>()
		{
			T result = (T)_values[Index];
			return result;

		}

		public UIImageListButton(List<Asset<Texture2D>> textures, List<object> values, List<string> hoverTexts, int defaultIndex = 0)
		{
			this._textures = textures;
			this._values = values;
			this._hoverTexts = hoverTexts;
			this.Width = (float)this._textures[0].Width();
			this.Height = (float)this._textures[0].Height();
			Index = defaultIndex;

			onHover += (a, b) => UIView.HoverText = $"Current: {_hoverTexts[Index]}{Environment.NewLine}Next: {GetNextTooltip()}";
		}

		public void AddImage(Asset<Texture2D> texture)
		{
			this._textures.Add(texture);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
            Texture2D texture = this._textures[Index].Value;
			if (texture != null)
            {
				spriteBatch.Draw(texture, base.DrawPosition, Color.White);
			}
		}

		public void SetVisibility(float whenActive, float whenInactive)
		{
			this.visibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
			this.visibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
		}

		public int NextIamge()
		{
			Index = GetNextImageIndex();
			return Index;
		}

		public int PrevIamge()
		{
			Index = GetPrevImageIndex();
			return Index;
		}

		private int GetNextImageIndex()
		{
			int result = Index + 1;
			if (_textures.Count <= result)
			{
				result = 0;
			}
			return result;
		}

		private int GetPrevImageIndex()
		{
			int result = Index - 1;
			if (result < 0)
			{
				result = _textures.Count - 1;
			}
			return result;
		}

		private string GetNextTooltip()
		{
			string result = _hoverTexts[GetNextImageIndex()];
			return result;
		}
	}
}
