﻿using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace CheatSheet.Menus
{
	class GodMode
	{
		internal static string CSText(string key, string category = "GodMode") => CheatSheet.CSText(category, key);
		private static string[] GodModeStateStrings;
		public static UIImage button;


		private static bool _enabled = false;
		public static bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				button.Tooltip = GodModeStateStrings[_enabled ? 1 : 0];
				button.ForegroundColor = _enabled ? Color.White : Color.LightSkyBlue;
			}
		}

		public static void LoadStatic()
		{
			GodModeStateStrings = new string[]
			{
				CSText("GodModeDisabled"),
				CSText("GodModeEnabled")
			};
		}

		public static void UnloadStatic()
		{
			GodModeStateStrings = null;

			button = null;
		}

		public static UIImage GetButton(Mod mod)
		{
			button = new UIImage(ModUtils.GetItemTexture(ItemID.JimsWings));

			button.onLeftClick += (s, e) =>
			{
				Enabled = !Enabled;
			};
			Enabled = false;
			return button;
		}
	}

	class GodModeModPlayer : ModPlayer
	{
		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
		{
			if (GodMode.Enabled)
				return false;
			return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource, ref cooldownCounter);
		}

		public override void PreUpdate()
		{
			if (GodMode.Enabled)
			{
				Player.statLife = Player.statLifeMax2;
				Player.statMana = Player.statManaMax2;
				Player.wingTime = Player.wingTimeMax;
			}
		}
	}
}
