//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using CheatSheet.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Reflection;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using System.IO;

//namespace CheatSheet.Menus
//{
//	static class ModUtils
//	{
//		internal static FieldInfo lightingStates;
//		private static void InitReflection()
//		{
//			try
//			{
//				//typeof(Lighting).getPr
//				lightingStates = typeof(NPC).GetField("states", BindingFlags.NonPublic | BindingFlags.Static);
//				Lighting.LightingState[][] arr = (Lighting.LightingState[][]) lightingStates.GetValue(Lighting, null);
//				arr[i] = value;
//			}
//			catch (Exception e)
//			{
//				ErrorLogger.Log("ModUtils Refletion: " + e.Message + " " + e.StackTrace);
//			}

//		}
//	}
//	class FullBrightBlobalTile : GlobalTile
//	{
//		public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
//		{
//			if (FullBright.fullBrightEnabled)
//			{
//				ModUtils.lightingStates.SetValue
//			}
//			return base.PreDraw(i, j, type, spriteBatch);
//		}
//		//public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
//		//{
//		//	//Main.NewText("?");
//		//	if (FullBright.fullBrightEnabled)
//		//	{
//		//		r = 1f;
//		//		g = 1f;
//		//		b = 10f;
//		//	}
//		//}
//	}
//	class FullBright
//	{
//		internal static bool fullBrightEnabled = false;
//		static UIImage button;

//		public static UIImage GetButton(Mod mod)
//		{
//			button = new UIImage(Main.buffTexture[BuffID.MagicLantern]);
//			button.Tooltip = "Fullbright: Off";
//			button.onLeftClick += (s, e) =>
//			{
//				buttonLogic();
//			};
//			button.ForegroundColor = Color.White;
//			return button;
//		}

//		public static void buttonLogic()
//		{
//			fullBrightEnabled = !fullBrightEnabled;
//			if (fullBrightEnabled)
//			{
//				button.Tooltip = "Fullbright: On";
//				Main.NewText("Fullbright On");
//			}
//			else
//			{
//				button.Tooltip = "Fullbright: Off";
//				Main.NewText("Fullbright Off");
//			}
//		}
//	}
//}