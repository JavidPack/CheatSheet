﻿using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class RecipeSlot : UIView
	{
		public static Texture2D backgroundTexture = TextureAssets.InventoryBack9.Value;
		public static Texture2D selectedBackgroundTexture = TextureAssets.InventoryBack15.Value;

		public int recipeIndex = -1;
		public Recipe recipe;

		public RecipeSlot(int recipeIndex)
		{
			this.Init(recipeIndex);
		}

		private void Init(int recipeIndex)
		{
			base.Scale = 0.85f;
			this.recipeIndex = recipeIndex;
			this.recipe = Main.recipe[recipeIndex];

			base.onLeftClick += new EventHandler(this.Slot_onLeftClick);
			base.onHover += new EventHandler(this.Slot_onHover);
		}

		protected override float GetWidth()
		{
			return (float)Slot.backgroundTexture.Width * base.Scale;
		}

		protected override float GetHeight()
		{
			return (float)Slot.backgroundTexture.Height * base.Scale;
		}

		private void Slot_onHover(object sender, EventArgs e)
		{
			//UIView.HoverText = recipe.createItem.name;
			Main.hoverItemName = recipe.createItem.Name;
			Main.HoverItem = recipe.createItem.Clone();
			Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.Mod.Name + "]" : ""));
			//UIView.HoverItem = this.item.Clone();
			//	hovering = true;
		}

		private double doubleClickTimer;

		private void Slot_onLeftClick(object sender, EventArgs e)
		{
			((Parent as RecipeView).Parent as RecipeBrowserWindow).selectedRecipe = recipe;
			((Parent as RecipeView).Parent as RecipeBrowserWindow).selectedRecipeChanged = true;
			// TODO if double click, go use item as lookup.
			if (Math.Abs(Main.time - doubleClickTimer) < 20)
			{
				RecipeBrowserWindow.lookupItemSlot.ReplaceWithFake(recipe.createItem.type);
			}
			doubleClickTimer = Main.time;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (((Parent as RecipeView).Parent as RecipeBrowserWindow).selectedRecipe == recipe)
			{
				spriteBatch.Draw(RecipeSlot.selectedBackgroundTexture, base.DrawPosition, null, Color.White, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
			}
			else
			{
				spriteBatch.Draw(RecipeSlot.backgroundTexture, base.DrawPosition, null, Color.White, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
			}
			Texture2D texture2D = Terraria.GameContent.TextureAssets.Item[this.recipe.createItem.type].Value;
			Rectangle rectangle2;
			if (Main.itemAnimations[recipe.createItem.type] != null)
			{
				rectangle2 = Main.itemAnimations[recipe.createItem.type].GetFrame(texture2D);
			}
			else
			{
				rectangle2 = texture2D.Frame(1, 1, 0, 0);
			}
			float num = 1f;
			float num2 = (float)RecipeSlot.backgroundTexture.Width * base.Scale * 0.6f;
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
			drawPosition.X += (float)RecipeSlot.backgroundTexture.Width * base.Scale / 2f - (float)rectangle2.Width * num / 2f;
			drawPosition.Y += (float)RecipeSlot.backgroundTexture.Height * base.Scale / 2f - (float)rectangle2.Height * num / 2f;
			this.recipe.createItem.GetColor(Color.White);
			spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(rectangle2), this.recipe.createItem.GetAlpha(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			if (this.recipe.createItem.color != default(Color))
			{
				spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(rectangle2), this.recipe.createItem.GetColor(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			}
			if (this.recipe.createItem.stack > 1)
			{
				spriteBatch.DrawString(FontAssets.ItemStack.Value, this.recipe.createItem.stack.ToString(), new Vector2(base.DrawPosition.X + 10f * base.Scale, base.DrawPosition.Y + 26f * base.Scale), Color.White, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
			}
			base.Draw(spriteBatch);
		}
	}
}