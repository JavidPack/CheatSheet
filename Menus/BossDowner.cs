using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal class BossDowner
	{
		public static UIImage button;
		public static BossDownerWindow bossDownerWindow;
		public static CheatSheet cheatSheet;

		public static UIImage GetButton(Mod mod)
		{
			cheatSheet = mod as CheatSheet;

			bossDownerWindow = new BossDownerWindow(mod);
			bossDownerWindow.SetDefaultPosition(new Vector2(200, 200));
			bossDownerWindow.Visible = false;

			button = new UIImage(ModUtils.GetItemTexture(ItemID.MechanicalSkull));

			button.Tooltip = "Open Boss Downer";
			button.onLeftClick += new EventHandler(bBossDownerToggle_onLeftClick);
			//+= (s, e) =>
			//{
			//	configurationWindow.selected = true;
			//};
			button.ForegroundColor = Color.White;

			return button;
		}

		private static void bBossDownerToggle_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;

			if (BossDowner.bossDownerWindow.selected)
			{
				BossDowner.bossDownerWindow.selected = false;
				uIImage.ForegroundColor = Hotbar.buttonUnselectedColor;
			}
			else
			{
				cheatSheet.hotbar.DisableAllWindows();
				bossDownerWindow.selected = true;
				uIImage.ForegroundColor = Hotbar.buttonSelectedColor;
			}
		}
	}

	internal class BossDownerWindow : UISlideWindow
	{
		public Mod mod;
		private float spacing = 16f;

		public BossDownerWindow(Mod mod)
		{
			this.mod = mod;
			this.CanMove = true;
			base.Width = 450;
			base.Height = 357;

			Asset<Texture2D> texture = mod.Assets.Request<Texture2D>("UI/closeButton", ReLogic.Content.AssetRequestMode.ImmediateLoad);
			UIImage uIImage = new UIImage(texture);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(base.Width - this.spacing, this.spacing);
			uIImage.onLeftClick += new EventHandler(this.bClose_onLeftClick);
			this.AddChild(uIImage);

			string[] labels = new string[] { "Eye of Cthulhu", "Eater of Worlds / Brain of Cthulhu", "Skeletron", "Plantera", "Golem", "The Destroyer", "The Twins",
				"Skeletron Prime", "Ancient Cultist", "Nebula Pillar", "Vortex Pillar","Solar Pillar","Stardust Pillar","Moonlord",
			};
			UICheckbox[] cb = new UICheckbox[labels.Length];

			bool[] selecteds = new bool[] {
				NPC.downedBoss1, // eye of cthulhu
                NPC.downedBoss2, // eater of worlds // brain of cthulhu
                NPC.downedBoss3, // skeletron
                NPC.downedPlantBoss, // plantera
                NPC.downedGolemBoss, // golem
                NPC.downedMechBoss1, // the destroyer
                NPC.downedMechBoss2, // the twins
                NPC.downedMechBoss3, // skeletron prime
                NPC.downedAncientCultist, // cultist
                NPC.downedTowerNebula, // nebula pillar
                NPC.downedTowerVortex, // vortex pillar
                NPC.downedTowerSolar, // solar pillar
                NPC.downedTowerStardust, // stardust pillar
                NPC.downedMoonlord, // moonlord
            };

			for (int i = 0; i < cb.Length; i++)
			{
				cb[i] = new UICheckbox(labels[i]);
				cb[i].Selected = selecteds[i];
				cb[i].X = spacing;
				cb[i].Y = i * 24 + spacing;
				cb[i].SelectedChanged += new EventHandler(bCheckBoxTicked);
				AddChild(cb[i]);
			}
		}

		private void bCheckBoxTicked(object sender, EventArgs e)
		{
			UICheckbox checkbox = (UICheckbox)sender;
			switch (checkbox.Text)
			{
				case "Eye of Cthulhu":
					NPC.downedBoss1 = checkbox.Selected;
					break;

				case "Eater of Worlds / Brain of Cthulhu":
					NPC.downedBoss2 = checkbox.Selected;
					break;

				case "Skeletron":
					NPC.downedBoss3 = checkbox.Selected;
					break;

				case "Plantera":
					NPC.downedPlantBoss = checkbox.Selected;
					break;

				case "Golem":
					NPC.downedGolemBoss = checkbox.Selected;
					break;

				case "The Destroyer":
					NPC.downedMechBoss1 = checkbox.Selected;
					break;

				case "The Twins":
					NPC.downedMechBoss2 = checkbox.Selected;
					break;

				case "Skeletron Prime":
					NPC.downedMechBoss3 = checkbox.Selected;
					break;

				case "Ancient Cultist":
					NPC.downedAncientCultist = checkbox.Selected;
					break;

				case "Nebula Pillar":
					NPC.downedTowerNebula = checkbox.Selected;
					break;

				case "Vortex Pillar":
					NPC.downedTowerVortex = checkbox.Selected;
					break;

				case "Solar Pillar":
					NPC.downedTowerSolar = checkbox.Selected;
					break;

				case "Stardust Pillar":
					NPC.downedTowerStardust = checkbox.Selected;
					break;

				case "Moonlord":
					NPC.downedMoonlord = checkbox.Selected;
					break;

				default:
					break;
			}
			////((CheatSheet)mod).hotbar.ChangedBossDowner();
			////BossDowner.bossDownerWindow.selected = true;
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			Hide();
		}
	}
}