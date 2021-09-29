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
		internal static float[] LightStrengthValues;
		private static string[] LightStrengthStrings;

		public static UIImage button;

		public static void LoadStatic()
		{
			LightStrengthValues = new float[]
			{
				0,
				.25f,
				.5f,
				1f
			};

			LightStrengthStrings = new string[]
			{
				CSText("LightHackDisabled"),
				CSText("LightHack25%"),
				CSText("LightHack50%"),
				CSText("LightHack100%")
			};
		}

		public static void UnloadStatic()
		{
			LightStrengthValues = null;
			LightStrengthStrings = null;

			button = null;
		}

		public static UIImage GetButton(Mod mod)
		{
			button = new UIImage(ModUtils.GetItemTexture(ItemID.UltrabrightTorch));

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
				float strength = LightHack.LightStrengthValues[LightHack.LightStrength];
				r = MathHelper.Clamp(r + strength, 0, 1);
				g = MathHelper.Clamp(g + strength, 0, 1);
				b = MathHelper.Clamp(b + strength, 0, 1);
			}
		}
	}
}
