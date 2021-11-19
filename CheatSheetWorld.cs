using CheatSheet.Menus;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace CheatSheet
{
	internal class CheatSheetWorld : ModSystem
	{
		internal static string CSText(string key, string category = "ExtraAccessorySlots") => CheatSheet.CSText(category, key);
		public override void OnWorldLoad()
		{
			if (!Main.dedServ && Main.LocalPlayer.name != "")
			{
				try
				{
					CheatSheet.instance.hotbar.bCycleExtraAccessorySlots.Tooltip = CSText("ExtraAccessorySlots") + " " + Main.LocalPlayer.GetModPlayer<CheatSheetPlayer>().numberExtraAccessoriesEnabled;
					CheatSheet.instance.paintToolsHotbar.UndoHistory.Clear();
					CheatSheet.instance.paintToolsHotbar.UpdateUndoTooltip();
				}
				catch (Exception e)
				{
					CheatSheetUtilities.ReportException(e);
				}
			}

			//    CheatSheet.instance.hotbar.ChangedBossDowner();
		}

        public override void OnWorldUnload()
        {
			if (!Main.dedServ && Main.LocalPlayer.name != "")
			{
				try
				{
					CheatSheet.instance.hotbar.bCycleExtraAccessorySlots.Tooltip = CSText("ExtraAccessorySlots") + " " + Main.LocalPlayer.GetModPlayer<CheatSheetPlayer>().numberExtraAccessoriesEnabled;
					CheatSheet.instance.paintToolsHotbar.UndoHistory.Clear();
					CheatSheet.instance.paintToolsHotbar.UpdateUndoTooltip();
				}
				catch (Exception e)
				{
					CheatSheetUtilities.ReportException(e);
				}
			}

			//    CheatSheet.instance.hotbar.ChangedBossDowner();
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.WriteVarInt((int)NPCBrowser.filteredNPCSlots.Count);
			foreach (var item in NPCBrowser.filteredNPCSlots)
			{
				writer.WriteVarInt((int)item);
			}
		}

		public override void NetReceive(BinaryReader reader)
		{
			NPCBrowser.filteredNPCSlots.Clear();
			int numFiltered = reader.ReadVarInt();
			for (int i = 0; i < numFiltered; i++)
			{
				NPCBrowser.filteredNPCSlots.Add(reader.ReadVarInt());
			}
			NPCBrowser.needsUpdate = true;
		}

		public override void UpdateUI(GameTime gameTime)
		{
			base.UpdateUI(gameTime);

			if (Main.netMode == 1 && ModContent.GetInstance<CheatSheetServerConfig>().DisableCheatsForNonHostUsers && !CheatSheet.IsPlayerLocalServerOwner(Main.LocalPlayer))
				return;

			if (PaintToolsEx.schematicsToLoad != null && CheatSheet.instance.numberOnlineToLoad > 0 && CheatSheet.instance.paintToolsUI.view.childrenToRemove.Count == 0)
			{
				PaintToolsEx.LoadSingleSchematic();
				//CheatSheet.instance.paintToolsUI.view.ReorderSlots();
			}

			if (PaintToolsSlot.updateNeeded)
			{
				bool oneUpdated = false;
				foreach (var item in CheatSheet.instance.paintToolsUI.view.slotList)
				{
					if (item.texture == TextureAssets.MagicPixel.Value)
					{
						item.texture = item.MakeThumbnail(item.stampInfo);
						oneUpdated = true;
						break;
					}
				}
				if (!oneUpdated)
					PaintToolsSlot.updateNeeded = false;
			}
		}

		//public override void PostDrawFullscreenMap(ref string mouseText)
		//{
		//	Main.spriteBatch.DrawString(FontAssets.MouseText.Value, "Testing Testing", new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), Color.Pink, 0.0f, new Vector2(), 0.8f, SpriteEffects.None, 0.0f);
		//}

		private int lastmode = -1;

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.netMode == 1 && ModContent.GetInstance<CheatSheetServerConfig>().DisableCheatsForNonHostUsers && !CheatSheet.IsPlayerLocalServerOwner(Main.LocalPlayer))
				return;

			if (Main.netMode != lastmode)
			{
				lastmode = Main.netMode;
				if (Main.netMode == 0)
				{
					SpawnRateMultiplier.HasPermission = true;
					foreach (var key in CheatSheet.instance.herosPermissions.Keys.ToList())
					{
						CheatSheet.instance.herosPermissions[key] = true;
					}
				}
				CheatSheet.instance.hotbar.ChangedConfiguration();
			}
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"CheatSheet: All Cheat Sheet",
					delegate
					{
						ModContent.GetInstance<AllItemsMenu>().DrawUpdateAll(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"CheatSheet: Paint Tools",
					delegate
					{
						ModContent.GetInstance<AllItemsMenu>().DrawUpdatePaintTools(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.Game)
				);
			}

			MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"CheatSheet: Extra Accessories",
					delegate
					{
						ModContent.GetInstance<AllItemsMenu>().DrawUpdateExtraAccessories(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}