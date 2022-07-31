using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class PaintToolsSlot : UIView
	{
		internal static int slotSize = 100;
		internal static PaintToolsSlot CurrentSelect;
		public StampInfo stampInfo;
		public bool isSelect;
		internal Texture2D texture;

		// For items from the Schematics Browser.
		internal string browserName;
		internal int browserID;
		internal int rating;
		internal int vote;

		public static bool updateNeeded;

		public PaintToolsSlot(StampInfo stampInfo)
		{
			this.stampInfo = stampInfo;
			texture = TextureAssets.MagicPixel.Value;
			updateNeeded = true;
			base.onLeftClick += (a, b) => Select();
		}

		internal Texture2D MakeThumbnail(StampInfo stampInfo)
		{
			int desiredWidth = 100;
			int desiredHeight = 100;

			int actualWidth = stampInfo.Width;
			int actualHeight = stampInfo.Height;

			float scale = 1;
			Vector2 offset = new Vector2();
			if (actualWidth > desiredWidth || actualHeight > desiredHeight)
			{
				if (actualHeight > actualWidth)
				{
					scale = (float)desiredWidth / actualHeight;
					offset.X = (desiredWidth - actualWidth * scale) / 2;
				}
				else
				{
					scale = (float)desiredWidth / actualWidth;
					offset.Y = (desiredHeight - actualHeight * scale) / 2;
				}
			}
			offset = offset / scale;

			RenderTarget2D renderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, desiredWidth, desiredHeight);
			Main.instance.GraphicsDevice.SetRenderTarget(renderTarget);
			Main.instance.GraphicsDevice.Clear(Color.Transparent);
			Main.spriteBatch.Begin();

			PaintToolsHotbar.DrawPreview(Main.spriteBatch, stampInfo.Tiles, offset, scale);

			Main.spriteBatch.End();
			Main.instance.GraphicsDevice.SetRenderTarget(null);

			Texture2D mergedTexture = new Texture2D(Main.instance.GraphicsDevice, desiredWidth, desiredHeight);
			Color[] content = new Color[desiredWidth * desiredHeight];
			renderTarget.GetData<Color>(content);
			mergedTexture.SetData<Color>(content);
			return mergedTexture;
		}

		public void Select()
		{
			CheatSheet.instance.paintToolsUI.infoPanel.Visible = false;
			CheatSheet.instance.paintToolsUI.submitPanel.Visible = false;
			CheatSheet.instance.paintToolsUI.submitInput.Text = "";
			if (CurrentSelect != null)
			{
				CurrentSelect.isSelect = false;
			}
			if (CurrentSelect == this)
			{
				CurrentSelect = null;
				CheatSheet.instance.paintToolsHotbar.StampTiles = new TileData[0, 0];
				CheatSheet.instance.paintToolsHotbar.stampInfo = null;
			}
			else
			{
				isSelect = true;
				CurrentSelect = this;
				CheatSheet.instance.paintToolsHotbar.StampTiles = stampInfo.Tiles;
				CheatSheet.instance.paintToolsHotbar.stampInfo = stampInfo;

				// Update UI;
				if (CurrentSelect.browserID > 0)
				{
					CheatSheet.instance.paintToolsUI.infoPanel.Visible = true;
					CheatSheet.instance.paintToolsUI.infoMessage.Text = browserName + ": " + rating;
					CheatSheet.instance.paintToolsUI.upVoteButton.ForegroundColor = Color.White;
					CheatSheet.instance.paintToolsUI.downVoteButton.ForegroundColor = Color.White;

					if (CurrentSelect.vote == 1)
						CheatSheet.instance.paintToolsUI.upVoteButton.ForegroundColor = Color.Gray;
					if (CurrentSelect.vote == -1)
						CheatSheet.instance.paintToolsUI.downVoteButton.ForegroundColor = Color.Gray;
				}
				else if (CurrentSelect.browserID == 0)
				{
					// TODO: restore this when online fixed
					//CheatSheet.instance.paintToolsUI.submitPanel.Visible = true;
				}
			}
		}

		protected override float GetWidth()
		{
			return 100;
			//return (float)texture.Width * base.Scale;
		}

		protected override float GetHeight()
		{
			return 100;
			//return (float)texture.Height * base.Scale;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Point pos = base.DrawPosition.ToPoint();
			Rectangle rectangle = new Rectangle(pos.X, pos.Y, (int)GetWidth(), (int)GetHeight());

			Rectangle value = new Rectangle(0, 0, 1, 1);
			float r = 1f;
			if (isSelect)
				r = .25f;
			float g = 0.9f;
			float b = 0.1f;
			float a = 1f;
			float scale2 = 0.6f;
			Color color = PaintToolsHotbar.buffColor(Color.White, r, g, b, a);
			if (isSelect)
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, color * scale2);

			SpriteEffects effects = SpriteEffects.None;
			if (stampInfo.bFlipHorizontal)
				effects |= SpriteEffects.FlipHorizontally;
			if (stampInfo.bFlipVertical)
				effects |= SpriteEffects.FlipVertically;

			spriteBatch.Draw(texture, base.DrawPosition, null, Color.White, 0f, Vector2.Zero, base.Scale, effects, 0f);

			if (isSelect)
			{
				b = 0.3f;
				g = 0.95f;
				scale2 = (a = 1f);
				color = PaintToolsHotbar.buffColor(Color.White, r, g, b, a);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(pos.X, pos.Y, rectangle.Width, 2), color * scale2);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(pos.X, pos.Y, 2, rectangle.Height), color * scale2);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(pos.X + rectangle.Width - 2, pos.Y, 2, rectangle.Height), color * scale2);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(pos.X, pos.Y + rectangle.Height - 2, rectangle.Width, 2), color * scale2);
			}

			base.Draw(spriteBatch);
		}
	}
}
