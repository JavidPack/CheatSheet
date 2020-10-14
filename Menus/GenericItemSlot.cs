﻿using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class GenericItemSlot : UIView
	{
		public static Asset<Texture2D> backgroundTexture = TextureAssets.InventoryBack9;

		public Item item = null;

		public GenericItemSlot()
		{
			base.onHover += new EventHandler(this.Slot_OnHover);
		}

		protected override float GetWidth()
		{
			return (float)GenericItemSlot.backgroundTexture.Width() * base.Scale;
		}

		protected override float GetHeight()
		{
			return (float)GenericItemSlot.backgroundTexture.Height() * base.Scale;
		}

		private void Slot_OnHover(object sender, EventArgs e)
		{
			if (item != null)
			{
				//	UIView.HoverText = this.item.name;
				//	UIView.HoverItem = this.item.Clone();

				Main.hoverItemName = this.item.Name;
				Main.HoverItem = item.Clone();
				Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.Mod.Name + "]" : ""));
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (item != null)
			{
				spriteBatch.Draw(Slot.backgroundTexture.Value, base.DrawPosition, null, Color.White, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
				Texture2D texture2D = Terraria.GameContent.TextureAssets.Item[this.item.type].Value;
				Rectangle rectangle2;
				if (Main.itemAnimations[item.type] != null)
				{
					rectangle2 = Main.itemAnimations[item.type].GetFrame(texture2D);
				}
				else
				{
					rectangle2 = texture2D.Frame(1, 1, 0, 0);
				}
				float num = 1f;
				float num2 = (float)Slot.backgroundTexture.Width() * base.Scale * 0.6f;
				if ((float)rectangle2.Width > num2 || (float)rectangle2.Height > num2)
				{
					if (rectangle2.Width > rectangle2.Height)
					{
						num = num2 / (float)rectangle2.Width;
					}
					else
					{
						num = num2 / (float)rectangle2.Height;
					}
				}
				Vector2 drawPosition = base.DrawPosition;
				drawPosition.X += (float)Slot.backgroundTexture.Width() * base.Scale / 2f - (float)rectangle2.Width * num / 2f;
				drawPosition.Y += (float)Slot.backgroundTexture.Height() * base.Scale / 2f - (float)rectangle2.Height * num / 2f;
				this.item.GetColor(Color.White);
				spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(rectangle2), this.item.GetAlpha(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
				if (this.item.color != default(Color))
				{
					spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(rectangle2), this.item.GetColor(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
				}
				if (this.item.stack > 1)
				{
					spriteBatch.DrawString(FontAssets.ItemStack.Value, this.item.stack.ToString(), new Vector2(base.DrawPosition.X + 10f * base.Scale, base.DrawPosition.Y + 26f * base.Scale), Color.White, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
				}
			}
			base.Draw(spriteBatch);
		}
	}
}