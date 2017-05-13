using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CheatSheet.UI;
using CheatSheet.CustomUI;

namespace CheatSheet.Menus
{

	// TODO, test if server needs to know this as well.
	class MinionSlotBoosterModPlayer : ModPlayer
	{
		public override void PostUpdateEquips()
		{
			player.maxMinions += MinionSlotBooster.currentBoost;
		}
	}
		class MinionSlotBoosterModPlayer2 : GlobalTile
	{
		public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			base.ModifyLight(i, j, type, ref r, ref g, ref b);
		}
	}
	class MinionSlotBooster
	{
		static int[] boosts = new int[] { 0,1,2,3,5,10,15,20};
		static string[] boostStrings = new string[] { "+0", "+1", "+2", "+3", "+5", "+10", "+15", "+20" };
		public static int currentBoost = 0;
		static int currentBoostIndex = 0;
		public static UIImage button;

		public static UIImage GetButton(Mod mod)
		{
			button = new UIImage(Main.buffTexture[BuffID.Summoning]);
			button.Tooltip = "Minion Slot Boost: +0";
			button.onRightClick += (s, e) =>
			{
				buttonLogic(false);
			};
			button.onLeftClick += (s, e) =>
			{
				buttonLogic(true);
			};
			button.ForegroundColor = Color.White;
			return button;
		}

		public static void buttonLogic(bool leftMouse)
		{
			int newIndex = leftMouse ? (currentBoostIndex + 1) % boostStrings.Length : (boostStrings.Length + currentBoostIndex - 1) % boostStrings.Length;

			//if (Main.netMode == 1)
			//{
			//	RequestSetSpawnRate(newIndex);
			//}
			//else
			//{
				ChangeSettingLogic(newIndex);
			//}
		}

		public static void ChangeSettingLogic(int newSetting)
		{
			currentBoostIndex = newSetting;
			currentBoost = boosts[currentBoostIndex];
			if (!Main.dedServ)
			{
				button.Tooltip = "Minion Slot Boost: " + boostStrings[currentBoostIndex];
				Main.NewText("Minion boost now at " + boostStrings[currentBoostIndex]);
			}
		}

		//// Action Taken by Client Button
		//internal static void RequestSetSpawnRate(int index)
		//{
		//	var netMessage = CheatSheet.instance.GetPacket();
		//	netMessage.Write((byte)CheatSheetMessageType.SetSpawnRate);
		//	netMessage.Write(index);
		//	netMessage.Send();
		//}

		//// Action taken by sever receiving button
		//internal static void HandleSetSpawnRate(BinaryReader reader, int whoAmI)
		//{
		//	int newSetting = reader.ReadInt32();
		//	ChangeSettingLogic(newSetting);

		//	var netMessage = CheatSheet.instance.GetPacket();
		//	netMessage.Write((byte)CheatSheetMessageType.SpawnRateSet);
		//	netMessage.Write(newSetting);
		//	netMessage.Send();
		//}

		
		//// Action taken by client receiving button
		//internal static void HandleSpawnRateSet(BinaryReader reader, int whoAmI)
		//{
		//	int newSetting = reader.ReadInt32();
		//	ChangeSettingLogic(newSetting);
		//}
	}
}
