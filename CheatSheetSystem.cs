using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CheatSheet
{
	internal class CheatSheetSystem : ModSystem
	{
		public override void PostSetupRecipes()
		{
			CheatSheet.instance.SetupUI();
		}
	}
}
