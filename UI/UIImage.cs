using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace CheatSheet.UI
{
	internal class UIImage : UIView
	{
		private Asset<Texture2D> textureAsset;

		private SpriteEffects _spriteEfftct;

		private Rectangle? sourceRectangle = null;

		public Asset<Texture2D> TextureAsset
		{
			get
			{
				return this.textureAsset;
			}
			set
			{
				this.textureAsset = value;
			}
		}

		private float width
		{
			get
			{
				return (float)this.textureAsset.Width();
			}
		}

		private float height
		{
			get
			{
				return (float)this.textureAsset.Height();
			}
		}

		public SpriteEffects SpriteEffect
		{
			get
			{
				return this._spriteEfftct;
			}
			set
			{
				this._spriteEfftct = value;
			}
		}

		public Rectangle SourceRectangle
		{
			get
			{
				if (!this.sourceRectangle.HasValue)
				{
					this.sourceRectangle = new Rectangle?(default(Rectangle));
				}
				return this.sourceRectangle.Value;
			}
			set
			{
				this.sourceRectangle = new Rectangle?(value);
			}
		}

		public int SR_X
		{
			get
			{
				return this.SourceRectangle.X;
			}
			set
			{
				this.SourceRectangle = new Rectangle(value, this.SourceRectangle.Y, this.SourceRectangle.Width, this.SourceRectangle.Height);
			}
		}

		public int SR_Y
		{
			get
			{
				return this.SourceRectangle.X;
			}
			set
			{
				this.SourceRectangle = new Rectangle(this.SourceRectangle.X, value, this.SourceRectangle.Width, this.SourceRectangle.Height);
			}
		}

		public int SR_Width
		{
			get
			{
				return this.SourceRectangle.X;
			}
			set
			{
				this.SourceRectangle = new Rectangle(this.SourceRectangle.X, this.SourceRectangle.Y, value, this.SourceRectangle.Height);
			}
		}

		public int SR_Height
		{
			get
			{
				return this.SourceRectangle.X;
			}
			set
			{
				this.SourceRectangle = new Rectangle(this.SourceRectangle.X, this.SourceRectangle.Y, this.SourceRectangle.Width, value);
			}
		}

		public UIImage(Asset<Texture2D> textureAsset)
		{
			this.TextureAsset = textureAsset;
		}

		public UIImage()
		{
		}

		protected override float GetWidth()
		{
			if (this.sourceRectangle.HasValue)
			{
				return (float)this.sourceRectangle.Value.Width * base.Scale;
			}
			return this.width * base.Scale;
		}

		protected override float GetHeight()
		{
			if (this.sourceRectangle.HasValue)
			{
				return (float)this.sourceRectangle.Value.Height * base.Scale;
			}
			return this.height * base.Scale;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (base.Visible)
			{
				Texture2D texture = this.textureAsset.Value;
				if (texture != null)
                {
					spriteBatch.Draw(texture, base.DrawPosition, this.sourceRectangle, base.ForegroundColor * base.Opacity, 0f, base.Origin / base.Scale, base.Scale, this.SpriteEffect, 0f);
				}
			}
			base.Draw(spriteBatch);
		}
	}
}