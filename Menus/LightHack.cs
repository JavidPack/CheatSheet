using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	// https://gfycat.com/LoathsomeSelfassuredBoilweevil
	internal class LightHack
	{
		internal static string CSText(string key, string category = "LightHack") => CheatSheet.CSText(category, key);
		internal static int LightStrength;
		internal static float[] LightStrengthValues = new float[] { 0, .25f, .5f, 1f };
		private static string[] LightStrengthStrings = new string[] { CSText("LightHackDisabled"), CSText("LightHack25%"), CSText("LightHack50%"), CSText("LightHack100%")};

		public static UIImage button;

		public static UIImage GetButton(Mod mod)
		{
			button = new UIImage(Main.itemTexture[ItemID.UltrabrightTorch]);
			button.Tooltip = LightStrengthStrings[LightStrength];
			button.onRightClick += (s, e) =>
			{
				buttonLogic(false);
			};
			button.onLeftClick += (s, e) =>
			{
				buttonLogic(true);
			};
			button.ForegroundColor = Color.LightSkyBlue;
			return button;
		}

		public static void buttonLogic(bool leftMouse)
		{
			LightStrength = leftMouse ? (LightStrength + 1) % LightStrengthStrings.Length : (LightStrength + LightStrengthStrings.Length - 1) % LightStrengthStrings.Length;
			button.Tooltip = LightStrengthStrings[LightStrength];
			button.ForegroundColor = LightStrength == 0 ? Color.LightSkyBlue : Color.White;
		}
	}

	public class LightHackGlobalWall : GlobalWall
	{
		public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			if (LightHack.LightStrength > 0)
			{
				r = MathHelper.Clamp(r + LightHack.LightStrengthValues[LightHack.LightStrength], 0, 1);
				g = MathHelper.Clamp(g + LightHack.LightStrengthValues[LightHack.LightStrength], 0, 1);
				b = MathHelper.Clamp(b + LightHack.LightStrengthValues[LightHack.LightStrength], 0, 1);
			}
		}
	}
}
