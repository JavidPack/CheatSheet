using CheatSheet.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	abstract class CheatSheetTool
	{
		public CheatSheetTool(Mod mod)
		{

		}

		public static UIImage hotbarButton;

		public abstract UIImage GetButton(Mod mod);
	}
}
