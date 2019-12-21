using CheatSheet.Menus;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CheatSheet
{
	internal class CheatSheetWorld : ModWorld
	{
		internal static string CSText(string key, string category = "ExtraAccessorySlots") => CheatSheet.CSText(category, key);
		public override void Initialize()
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
			writer.Write(NPCBrowser.filteredNPCSlots.Count);
			foreach (var item in NPCBrowser.filteredNPCSlots)
			{
				writer.Write((int)item);
			}
		}

		public override void NetReceive(BinaryReader reader)
		{
			NPCBrowser.filteredNPCSlots.Clear();
			int numFiltered = reader.ReadInt32();
			for (int i = 0; i < numFiltered; i++)
			{
				NPCBrowser.filteredNPCSlots.Add(reader.ReadInt32());
			}
			NPCBrowser.needsUpdate = true;
		}
	}
}