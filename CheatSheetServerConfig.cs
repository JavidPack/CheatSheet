using System.ComponentModel;
using Terraria;
using Terraria.ModLoader.Config;

namespace CheatSheet
{
	class CheatSheetServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[DefaultValue(false)]
		public bool DisableCheatsForNonHostUsers { get; set; }

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) {
			if (!CheatSheet.IsPlayerLocalServerOwner(Main.player[whoAmI])) {
				message = "You are not the server owner so you can not change this config";
				return false;
			}
			return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
		}
	}
}
