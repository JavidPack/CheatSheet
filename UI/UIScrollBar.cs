using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using ReLogic.Content;

namespace CheatSheet.UI
{
	internal class UIScrollBar : UIView
	{
		internal static Asset<Texture2D> ScrollbarTexture;

		private static Texture2D scrollbarFill;

		private float height = 100f;

		private static Texture2D ScrollbarFill
		{
			get
			{
				if (UIScrollBar.scrollbarFill == null && UIScrollBar.ScrollbarTexture.Value != null)
				{
					int width = UIScrollBar.ScrollbarTexture.Width();
					int height = UIScrollBar.ScrollbarTexture.Height();
					Color[] array = new Color[width * height];
					UIScrollBar.ScrollbarTexture.Value.GetData<Color>(array);
					Color[] array2 = new Color[width];
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = array[i + (height - 1) * width];
					}
					UIScrollBar.scrollbarFill = new Texture2D(UIView.graphics, array2.Length, 1);
					UIScrollBar.scrollbarFill.SetData<Color>(array2);
				}
				return UIScrollBar.scrollbarFill;
			}
		}

		public UIScrollBar()
		{
			ScrollbarTexture = CheatSheet.instance.Assets.Request<Texture2D>("UI/Images.UIKit.scrollbarEdge", ReLogic.Content.AssetRequestMode.ImmediateLoad);
		}

		protected override float GetHeight()
		{
			return this.height;
		}

		protected override void SetHeight(float height)
		{
			this.height = height;
		}

		protected override float GetWidth()
		{
			return (float)UIScrollBar.ScrollbarTexture.Width();
		}

		private void DrawScrollBar(SpriteBatch spriteBatch)
		{
			float num = base.Height - (float)(UIScrollBar.ScrollbarTexture.Height() * 2);
			Vector2 drawPosition = base.DrawPosition;
			spriteBatch.Draw(UIScrollBar.ScrollbarTexture.Value, drawPosition, null, Color.White, 0f, base.Origin, 1f, SpriteEffects.None, 0f);
			drawPosition.Y += (float)UIScrollBar.ScrollbarTexture.Height();
			if (UIScrollBar.ScrollbarFill != null)
            {
				spriteBatch.Draw(UIScrollBar.ScrollbarFill, drawPosition - base.Origin, null, Color.White, 0f, Vector2.Zero, new Vector2(1f, num), SpriteEffects.None, 0f);
			}
			drawPosition.Y += num;
			spriteBatch.Draw(UIScrollBar.ScrollbarTexture.Value, drawPosition, null, Color.White, 0f, base.Origin, 1f, SpriteEffects.FlipVertically, 0f);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			this.DrawScrollBar(spriteBatch);
			base.Draw(spriteBatch);
		}
	}
}