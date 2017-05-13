//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Microsoft.Xna.Framework;

//namespace CheatSheet.UI
//{
//	internal class UIImageButtonWithBackground : UIImage
//	{
//		public static Texture2D backgroundTexture = Main.inventoryBack9Texture;
//		public static Texture2D selectedBackgroundTexture = Main.inventoryBack15Texture;

//		internal bool selected = false;

//		public UIImageButtonWithBackground(Texture2D texture) : base(texture)
//		{
//		}

//		public override void Draw(SpriteBatch spriteBatch)
//		{
//			if (selected)
//			{
//				spriteBatch.Draw(RecipeSlot.selectedBackgroundTexture, base.DrawPosition, null, Color.White, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
//			}
//			else
//			{
//				spriteBatch.Draw(RecipeSlot.backgroundTexture, base.DrawPosition, null, Color.White, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
//			}
//			base.Draw(spriteBatch);
//		}
//	}
//}
