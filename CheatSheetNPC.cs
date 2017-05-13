using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CheatSheet.Menus;

namespace CheatSheet
{
    class CheatSheetNPC : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            //Main.NewText("using " + npc.ToString());

            if (NPCBrowser.filteredNPCSlots.Contains(npc.netID)/* || NPCBrowser.filteredNPCSlots.Contains(npc.type)*/)
            {
                //NPCSlot.HandleFilterNPC(npc.whoAmI);
                npc.GetModInfo<CheatSheetNPCInfo>(mod).isFiltered = true;
				int life = npc.life;
                npc.StrikeNPCNoInteraction(life, 0f, -npc.direction, true);
                if (Main.netMode == 1) // syncData does not do visuals
                {
                    NetMessage.SendData(28, -1, -1, "", npc.whoAmI, life, 0f, -Main.npc[npc.whoAmI].direction, 1);
                    // type, -1, -1, msg, index, damage, knockback, direction, crit
                }
                //NetMessage.SendData(23, -1, -1, "", npc.whoAmI);
            }

            return base.PreAI(npc);
        }


        public override bool PreNPCLoot(NPC npc)
        {
            return !npc.GetModInfo<CheatSheetNPCInfo>(mod).isFiltered;
        }

        /*public override void SpawnNPC(int npc, int tileX, int tileY)
        {
            Main.NewText("spawning " + npc.ToString());

            if (NPCBrowser.filteredNPCSlots.Contains(Main.npc[npc].netID))
            {
                Main.NewText("deleting " + npc.ToString());
                Main.npc[npc].netDefaults(0);
                Main.npc[npc].active = false;
            }
        }*/
    }

    class CheatSheetNPCInfo : NPCInfo
    {
        public bool isFiltered = false;
    }
}
