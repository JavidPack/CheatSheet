using CheatSheet.Menus;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;

namespace CheatSheet
{
    internal class CheatSheetSystem : ModSystem
    {
		public override void UpdateUI(GameTime gameTime)
		{
			base.UpdateUI(gameTime);

			if (Main.netMode == NetmodeID.MultiplayerClient && ModContent.GetInstance<CheatSheetServerConfig>().DisableCheatsForNonHostUsers && !CheatSheet.IsPlayerLocalServerOwner(Main.LocalPlayer))
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

		private int lastmode = -1;

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient && ModContent.GetInstance<CheatSheetServerConfig>().DisableCheatsForNonHostUsers && !CheatSheet.IsPlayerLocalServerOwner(Main.LocalPlayer))
				return;

			if (Main.netMode != lastmode)
			{
				lastmode = Main.netMode;
				if (Main.netMode == NetmodeID.SinglePlayer)
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
