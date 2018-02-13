using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	// TODO, test if server needs to know this as well.
	internal class MinionSlotBoosterModPlayer : ModPlayer
	{
		public override void PostUpdateEquips()
		{
			player.maxMinions += MinionSlotBooster.currentBoost;
		}
	}

	internal class MinionSlotBoosterModPlayer2 : GlobalTile
	{
		public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			base.ModifyLight(i, j, type, ref r, ref g, ref b);
		}
	}

	internal class MinionSlotBooster
	{
		internal static string CSText(string key, string category = "MinionSlot") => CheatSheet.CSText(category, key);
		private static int[] boosts = new int[] { 0, 1, 2, 3, 5, 10, 15, 20 };
		private static string[] boostStrings = new string[] { "+0", "+1", "+2", "+3", "+5", "+10", "+15", "+20" };
		public static int currentBoost = 0;
		private static int currentBoostIndex = 0;
		public static UIImage button;

		public static UIImage GetButton(Mod mod)
		{
			button = new UIImage(Main.buffTexture[BuffID.Summoning]);
			button.Tooltip = CSText("MinionSlotBooster");
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
				button.Tooltip = CSText("MinionSlotBoosterNew") + boostStrings[currentBoostIndex];
				Main.NewText(CSText("MinionSlotBoosterText") + boostStrings[currentBoostIndex]);
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