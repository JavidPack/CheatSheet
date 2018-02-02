using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal class ConfigurationTool
	{
		internal static string CSText(string key, string category = "Hotbar") => CheatSheet.CSText(category, key);
		public static UIImage button;
		public static ConfigurationWindow configurationWindow;
		public static CheatSheet cheatSheet;

		public static UIImage GetButton(Mod mod)
		{
			cheatSheet = mod as CheatSheet;

			configurationWindow = new ConfigurationWindow(mod);
			configurationWindow.SetDefaultPosition(new Vector2(200, 200));
			configurationWindow.Visible = false;

			button = new UIImage(Main.itemTexture[ItemID.Cog]);
			button.Tooltip = CSText("Configure Available Tools");
			button.onLeftClick += new EventHandler(bConfigurationToggle_onLeftClick);
			//+= (s, e) =>
			//{
			//	configurationWindow.selected = true;
			//};
			button.ForegroundColor = Color.White;

			return button;
		}

		private static void bConfigurationToggle_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;

			if (ConfigurationTool.configurationWindow.selected)
			{
				ConfigurationTool.configurationWindow.selected = false;
				uIImage.ForegroundColor = Hotbar.buttonUnselectedColor;
			}
			else
			{
				cheatSheet.hotbar.DisableAllWindows();
				configurationWindow.selected = true;
				uIImage.ForegroundColor = Hotbar.buttonSelectedColor;
			}
		}
	}

	internal class ConfigurationWindow : UISlideWindow
	{
		internal static string CSText(string key, string category = "ConfigurationTool") => CheatSheet.CSText(category, key);
		public Mod mod;
		private float spacing = 16f;

		public ConfigurationWindow(Mod mod)
		{
			this.mod = mod;
			this.CanMove = true;
			base.Width = 280;
			base.Height = 348;

			Texture2D texture = mod.GetTexture("UI/closeButton");
			UIImage uIImage = new UIImage(texture);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(base.Width - this.spacing, this.spacing);
			uIImage.onLeftClick += new EventHandler(this.bClose_onLeftClick);
			this.AddChild(uIImage);

			//ConfigurationLoader.Initialized();

			string[] labels = new string[] { CSText("Item Browser"), CSText("NPC Browser"), CSText("Recipe Browser"), CSText("Minion Booster"), CSText("Butcher"), CSText("Clear Menu"), 
			CSText("Extra Accessory Slots"), CSText("Mod Extensions"), CSText("Paint Tools"), CSText("Spawn Rate"), CSText("Vacuum"), CSText("Waypoints"), CSText("Light Hack")
			/* "Boss Downer", "Event Manager"*/
			};
			bool[] selecteds = new bool[] {
				ConfigurationLoader.personalConfiguration.ItemBrowser,
				ConfigurationLoader.personalConfiguration.NPCBrowser,
				ConfigurationLoader.personalConfiguration.RecipeBrowser,
				ConfigurationLoader.personalConfiguration.MinionBooster,
				ConfigurationLoader.personalConfiguration.Butcher,
				ConfigurationLoader.personalConfiguration.ClearMenu,
				ConfigurationLoader.personalConfiguration.ExtraAccessorySlots,
				ConfigurationLoader.personalConfiguration.ModExtensions,
				ConfigurationLoader.personalConfiguration.PaintTools,
				ConfigurationLoader.personalConfiguration.SpawnRate,
				ConfigurationLoader.personalConfiguration.Vacuum,
				ConfigurationLoader.personalConfiguration.Waypoints,
				ConfigurationLoader.personalConfiguration.LightHack,
              //  ConfigurationLoader.configuration.BossDowner,
              //  ConfigurationLoader.configuration.EventManager,
            };

			for (int i = 0; i < labels.Length; i++)
			{
				UICheckbox cb = new UICheckbox(labels[i]);
				cb.Selected = selecteds[i];
				cb.X = spacing;
				cb.Y = i * 24 + spacing;
				cb.SelectedChanged += new EventHandler(bCheckBoxTicked);

				//cb.label.ForegroundColor = Color.Red;
				AddChild(cb);
			}

			//UICheckbox cb = new UICheckbox("Item Browser");
			//cb.Selected = ConfigurationLoader.configuration.ItemBrowser;
			//cb.X = 0;
			//cb.Y = 1 * 20;
			//cb.SelectedChanged += new EventHandler(bCheckBoxTicked);
			//AddChild(cb);

			//cb = new UICheckbox("NPC Browser");
			//cb.Selected = ConfigurationLoader.configuration.NPCBrowser;
			//cb.X = 0;
			//cb.Y = 2 * 20;
			//cb.SelectedChanged += new EventHandler(bCheckBoxTicked);
			//AddChild(cb);

			//cb = new UICheckbox("Recipe Browser");
			//cb.Selected = ConfigurationLoader.configuration.RecipeBrowser;
			//cb.X = 0;
			//cb.Y = 3 * 20;
			//cb.SelectedChanged += new EventHandler(bCheckBoxTicked);
			//AddChild(cb);

			//cb = new UICheckbox("Minion Booster");
			//cb.Selected = ConfigurationLoader.configuration.MinionBooster;
			//cb.X = 0;
			//cb.Y = 4 * 20;
			//cb.SelectedChanged += new EventHandler(bCheckBoxTicked);
			//AddChild(cb);

			//for (int j = 0; j < 7; j++)
			//{
			//	GenericItemSlot genericItemSlot = new GenericItemSlot();
			//	Vector2 position = new Vector2(this.spacing, this.spacing);

			//	position.X += j * 60;
			//	position.Y += 250;

			//	genericItemSlot.Position = position;
			//	genericItemSlot.Tag = j;
			//	RecipeBrowser.ingredients[j] = genericItemSlot;
			//	this.AddChild(genericItemSlot, false);
			//}
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			Hide();
		}

		private void bCheckBoxTicked(object sender, EventArgs e)
		{
			UICheckbox checkbox = (UICheckbox)sender;
			switch (checkbox.Text)
			{
				case "Item Browser":
					ConfigurationLoader.personalConfiguration.ItemBrowser = checkbox.Selected;
					break;

				case "NPC Browser":
					ConfigurationLoader.personalConfiguration.NPCBrowser = checkbox.Selected;
					break;

				case "Recipe Browser":
					ConfigurationLoader.personalConfiguration.RecipeBrowser = checkbox.Selected;
					break;

				case "Minion Booster":
					ConfigurationLoader.personalConfiguration.MinionBooster = checkbox.Selected;
					break;

				case "Butcher":
					ConfigurationLoader.personalConfiguration.Butcher = checkbox.Selected;
					break;

				case "Clear Menu":
					ConfigurationLoader.personalConfiguration.ClearMenu = checkbox.Selected;
					break;

				case "Extra Accessory Slots":
					ConfigurationLoader.personalConfiguration.ExtraAccessorySlots = checkbox.Selected;
					break;

				case "Mod Extensions":
					ConfigurationLoader.personalConfiguration.ModExtensions = checkbox.Selected;
					break;

				case "Paint Tools":
					ConfigurationLoader.personalConfiguration.PaintTools = checkbox.Selected;
					break;

				case "Spawn Rate":
					ConfigurationLoader.personalConfiguration.SpawnRate = checkbox.Selected;
					break;

				case "Vacuum":
					ConfigurationLoader.personalConfiguration.Vacuum = checkbox.Selected;
					break;

				case "Waypoints":
					ConfigurationLoader.personalConfiguration.Waypoints = checkbox.Selected;
					break;

				case "Light Hack":
					ConfigurationLoader.personalConfiguration.LightHack = checkbox.Selected;
					break;
				//case "Boss Downer":
				//    ConfigurationLoader.configuration.BossDowner = checkbox.Selected;
				//    break;
				//case "Event Manager":
				//    ConfigurationLoader.configuration.EventManager = checkbox.Selected;
				//    break;
				default:
					break;
			}
			ConfigurationLoader.SaveSetting();
			((CheatSheet)mod).hotbar.ChangedConfiguration();
			ConfigurationTool.configurationWindow.selected = true;
		}
	}
}