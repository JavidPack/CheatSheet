﻿using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class RecipeQuerySlot : UIView
	{
		public static Texture2D backgroundTexture = TextureAssets.InventoryBack9.Value;
		public static Texture2D backgroundTextureFake = TextureAssets.InventoryBack8.Value;

		public Item item = new Item();
		internal bool real = true;

		public RecipeQuerySlot()
		{
			item.SetDefaults(0);
			base.onHover += new EventHandler(this.Slot_OnHover);
			base.onLeftClick += new EventHandler(this.Slot2_onLeftClick);
		}

		protected override float GetWidth()
		{
			return (float)GenericItemSlot.backgroundTexture.Width * base.Scale;
		}

		protected override float GetHeight()
		{
			return (float)GenericItemSlot.backgroundTexture.Height * base.Scale;
		}

		private void Slot_OnHover(object sender, EventArgs e)
		{
			Main.hoverItemName = this.item.Name;
			Main.HoverItem = item.Clone();
			Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.Mod.Name + "]" : ""));
		}

		private void Slot2_onLeftClick(object sender, EventArgs e)
		{
			Player player = Main.LocalPlayer;
			if (real)
			{
				if (player.itemAnimation == 0 && player.itemTime == 0)
				{
					Item item = Main.mouseItem.Clone();
					Main.mouseItem = this.item.Clone();
					if (Main.mouseItem.type > 0)
					{
						Main.playerInventory = true;
					}
					this.item = item.Clone();
				}
			}
			else
			{
				if (player.itemAnimation == 0 && player.itemTime == 0)
				{
					//Item item = Main.mouseItem.Clone();
					this.item = Main.mouseItem.Clone();
					Main.mouseItem.SetDefaults(0);
					real = true;
				}
			}

			//call update.
			RecipeBrowserWindow.recipeView.ReorderSlots();
			//Main.NewText(item.type + "");

			return;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//if (item != null)
			{
				spriteBatch.Draw(real ? backgroundTexture : backgroundTextureFake, base.DrawPosition, null, Color.White, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
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
				float num2 = (float)Slot.backgroundTexture.Width * base.Scale * 0.6f;
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
				drawPosition.X += (float)Slot.backgroundTexture.Width * base.Scale / 2f - (float)rectangle2.Width * num / 2f;
				drawPosition.Y += (float)Slot.backgroundTexture.Height * base.Scale / 2f - (float)rectangle2.Height * num / 2f;
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

		internal void ReplaceWithFake(int type)
		{
			if (real && RecipeBrowserWindow.lookupItemSlot.item.stack > 0)
			{
				//Main.LocalPlayer.QuickSpawnItem(RecipeBrowserWindow.lookupItemSlot.item.type, RecipeBrowserWindow.lookupItemSlot.item.stack);

				Player player = Main.LocalPlayer;
				RecipeBrowserWindow.lookupItemSlot.item.position = player.Center;
				Item item2 = player.GetItem(player.whoAmI, RecipeBrowserWindow.lookupItemSlot.item, GetItemSettings.GetItemInDropItemCheck);
				if (item2.stack > 0)
				{
					int num = Item.NewItem((int)player.position.X, (int)player.position.Y, player.width, player.height, item2.type, item2.stack, false, (int)RecipeBrowserWindow.lookupItemSlot.item.prefix, true, false);
					Main.item[num].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, null, num, 1f, 0f, 0f, 0, 0, 0);
					}
				}
				RecipeBrowserWindow.lookupItemSlot.item = new Item();
			}

			item.SetDefaults(type);
			real = false;
			RecipeBrowserWindow.recipeView.ReorderSlots();
		}
	}
}