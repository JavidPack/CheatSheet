using CheatSheet.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CheatSheet
{
	internal class AllItemsMenu : GlobalItem
	{
		private static Item[] singleSlotArray;

		public AllItemsMenu()
		{
			singleSlotArray = new Item[1];
		}

		//internal void UpdateInput()
		//{
		//	try
		//	{
		//		UIView.UpdateUpdateInput();
		//		((CheatSheet)mod).npcBrowser.Update();
		//		((CheatSheet)mod).itemBrowser.Update();
		//		((CheatSheet)mod).recipeBrowser.Update();
		//		((CheatSheet)mod).extendedCheatMenu.Update();

		//		((CheatSheet)mod).hotbar.Update();
		//		((CheatSheet)mod).paintToolsHotbar.Update();
		//		((CheatSheet)mod).quickTeleportHotbar.Update();
		//		((CheatSheet)mod).quickClearHotbar.Update();
		//		((CheatSheet)mod).npcButchererHotbar.Update();
		//		ConfigurationTool.configurationWindow.Update();
		//	}
		//	catch (Exception e)
		//	{
		//		ErrorLogger.Log(e.Message + " " + e.StackTrace);
		//	}
		//}

		public void DrawUpdateAll(SpriteBatch spriteBatch)
		{
			((CheatSheet)mod).itemBrowser.Draw(spriteBatch);
			((CheatSheet)mod).npcBrowser.Draw(spriteBatch);
			((CheatSheet)mod).recipeBrowser.Draw(spriteBatch);
			((CheatSheet)mod).extendedCheatMenu.Draw(spriteBatch);
			((CheatSheet)mod).paintToolsUI.Draw(spriteBatch);

			//			((CheatSheet)mod).itemBrowser.Update();
			//	spriteBatch.End();
			//	spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

			((CheatSheet)mod).npcBrowser.Update();
			((CheatSheet)mod).itemBrowser.Update();
			((CheatSheet)mod).recipeBrowser.Update();
			((CheatSheet)mod).extendedCheatMenu.Update();

			((CheatSheet)mod).hotbar.Update();
			((CheatSheet)mod).paintToolsHotbar.Update();
			((CheatSheet)mod).paintToolsUI.Update();
			((CheatSheet)mod).quickTeleportHotbar.Update();
			((CheatSheet)mod).quickClearHotbar.Update();
			((CheatSheet)mod).npcButchererHotbar.Update();
			ConfigurationTool.configurationWindow.Update();
			//BossDowner.bossDownerWindow.Update();
			//((CheatSheet)mod).eventManagerHotbar.Update();

			((CheatSheet)mod).hotbar.Draw(spriteBatch);
			((CheatSheet)mod).paintToolsHotbar.Draw(spriteBatch);
			((CheatSheet)mod).quickTeleportHotbar.Draw(spriteBatch);
			((CheatSheet)mod).quickClearHotbar.Draw(spriteBatch);
			((CheatSheet)mod).npcButchererHotbar.Draw(spriteBatch);
			ConfigurationTool.configurationWindow.Draw(spriteBatch);
			//BossDowner.bossDownerWindow.Draw(spriteBatch);
			//((CheatSheet)mod).eventManagerHotbar.Draw(spriteBatch);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

			//	DrawUpdateExtraAccessories(spriteBatch);
		}

		internal void DrawUpdateExtraAccessories(SpriteBatch spriteBatch)
		{
			if (Main.playerInventory && Main.EquipPage == 0)
			{
				Point value = new Point(Main.mouseX, Main.mouseY);
				Rectangle r = new Rectangle(0, 0, (int)((float)Main.inventoryBackTexture.Width * Main.inventoryScale), (int)((float)Main.inventoryBackTexture.Height * Main.inventoryScale));

				CheatSheetPlayer csp = Main.LocalPlayer.GetModPlayer<CheatSheetPlayer>(mod);
				for (int i = 0; i < csp.numberExtraAccessoriesEnabled; i++)
				{
					Main.inventoryScale = 0.85f;
					Item accItem = csp.ExtraAccessories[i];
					//if (accItem.type > 0)
					//{
					//	ErrorLogger.Log("aaa " + i + " " + accItem.type);
					//}

					int mH = 0;
					if (Main.mapEnabled)
					{
						if (!Main.mapFullscreen && Main.mapStyle == 1)
						{
							mH = 256;
						}
						if (mH + 600 > Main.screenHeight)
						{
							mH = Main.screenHeight - 600;
						}
					}

					int num17 = Main.screenWidth - 92 - (47 * 3);
					int num18 = /*Main.mH +*/mH + 174;
					if (Main.netMode == 1) num17 -= 47;
					r.X = num17/* + l * -47*/;
					r.Y = num18 + (0 + i) * 47;

					if (r.Contains(value)/* && !flag2*/)
					{
						Main.LocalPlayer.mouseInterface = true;
						Main.armorHide = true;
						singleSlotArray[0] = accItem;
						ItemSlot.Handle(singleSlotArray, ItemSlot.Context.EquipAccessory, 0);
						accItem = singleSlotArray[0];
						//ItemSlot.Handle(ref accItem, ItemSlot.Context.EquipAccessory);
					}
					singleSlotArray[0] = accItem;
					ItemSlot.Draw(spriteBatch, singleSlotArray, 10, 0, new Vector2(r.X, r.Y));
					accItem = singleSlotArray[0];

					//ItemSlot.Draw(spriteBatch, ref accItem, 10, new Vector2(r.X, r.Y));

					csp.ExtraAccessories[i] = accItem;
					//	ErrorLogger.Log("pd");
					//player.VanillaUpdateAccessory(csp.ExtraAccessories[i], false, ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
				}
			}
		}
	}
}