using CheatSheet.Menus;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CheatSheet
{
	internal class CheatSheetWorld : ModWorld
	{
		public override void Initialize()
		{
			if (!Main.dedServ && Main.LocalPlayer.name != "")
			{
				(mod as CheatSheet).hotbar.bCycleExtraAccessorySlots.Tooltip = "Extra Accessory Slots: " + Main.LocalPlayer.GetModPlayer<CheatSheetPlayer>(mod).numberExtraAccessoriesEnabled;
			}

			//    ((CheatSheet)mod).hotbar.ChangedBossDowner();
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