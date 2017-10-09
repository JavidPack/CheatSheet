using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CheatSheet.Menus
{
	internal class PaintToolsSlot : UIView
	{
        internal static int slotSize = 100;
        internal static PaintToolsSlot CurrentSelect;
        public StampInfo stampInfo;
        public bool isSelect;
        private Texture2D texture;

		public PaintToolsSlot(StampInfo stampInfo)
		{
            this.stampInfo = stampInfo;
            texture = stampInfo.Textures.Resize(slotSize);
            base.onLeftClick += (a, b) => Select();
        }

        public void Select()
        {
            if (CurrentSelect != null)
            {
                CurrentSelect.isSelect = false;
            }
            if (CurrentSelect == this)
            {
                CurrentSelect = null;
                CheatSheet.instance.paintToolsHotbar.StampTiles = new Tile[0, 0];
                CheatSheet.instance.paintToolsHotbar.stampInfo = null;
            }
            else
            {
                isSelect = true;
                CurrentSelect = this;
                CheatSheet.instance.paintToolsHotbar.StampTiles = stampInfo.Tiles;
                CheatSheet.instance.paintToolsHotbar.stampInfo = stampInfo;
            }
        }

        protected override float GetWidth()
		{
			return (float)texture.Width * base.Scale;
		}

		protected override float GetHeight()
		{
			return (float)texture.Height * base.Scale;
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
                spriteBatch.Draw(Main.magicPixel, rectangle, color * scale2);

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
                spriteBatch.Draw(Main.magicPixel, new Rectangle(pos.X, pos.Y, rectangle.Width, 2), color * scale2);
                spriteBatch.Draw(Main.magicPixel, new Rectangle(pos.X, pos.Y, 2, rectangle.Height), color * scale2);
                spriteBatch.Draw(Main.magicPixel, new Rectangle(pos.X + rectangle.Width - 2, pos.Y, 2, rectangle.Height), color * scale2);
                spriteBatch.Draw(Main.magicPixel, new Rectangle(pos.X, pos.Y + rectangle.Height - 2, rectangle.Width, 2), color * scale2);
            }

            base.Draw(spriteBatch);
		}
	}
}
