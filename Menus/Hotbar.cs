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
	internal class Hotbar : UIWindow
	{
		internal static string CSText(string key, string category = "Hotbar") => CheatSheet.CSText(category, key);

		private static float moveSpeed = 8f;

		public static float xPosition = 78f;

		public bool hidden;
		public static bool pVac = false;
		public static int pVacID = 0;

		private float lerpAmount;

		private float spacing = 8f;

		internal UIHotbar currentHotbar;

		//public static Texture2D mapTexture;
		public static Texture2D loginTexture;
		public static Texture2D logoutTexture;

		public UIView buttonView;

		private UIImage arrow;
		internal UIImage button;
		public UIImage bToggleItemBrowser;

		//		public UIImage bToggleEnemies;
		//		public UIImage bToggleBlockReach;
		//		public UIImage bFlyCamera;
		public UIImage bToggleClearMenu;

		//		public UIImage bRevealMap;
		//		public UIImage bWaypoints;
		//		public UIImage bGroupManager;
		//		public UIImage bOnlinePlayers;
		//		public UIImage bTime;
		public UIImage bToggleNPCBrowser;

		public UIImage bTogglePaintTools;
		//		public UIImage bWeatherWindow;
		//		public UIImage bBackupWorld;
		//		public UIImage bCTFSettings;
		//		public UIImage bLogin;

		public UIImage bToggleRecipeBrowser;
		public UIImage bToggleExtendedCheat;
		public UIImage bCycleExtraAccessorySlots;
		public UIImage bVacuum;
		public UIImage bToggleNPCButcherer;
		public UIImage bToggleQuickTeleport;
		// public UIImage bSpawnRateMultiplier;
		//public UIImage bToggleEventManager;

		//private static Color buttonUnselectedColor = Color.White;
		internal static Color buttonUnselectedColor = Color.LightSkyBlue;

		internal static Color buttonSelectedColor = Color.White;

		//private static Color buttonSelectedColor = Color.LightSkyBlue;
		internal static Color buttonSelectedHiddenColor = Color.Blue;

		public static float disabledOpacity = 0.5f;
		private CheatSheet mod;

		public Vector2 chatOffset
		{
			get
			{
				if (base.Visible)
				{
					return new Vector2(0f, base.Position.Y - (float)Main.screenHeight - this.arrow.Height);
				}
				return Vector2.Zero;
			}
		}

		public float shownPosition
		{
			get
			{
				return (float)Main.screenHeight - base.Height - 12f;
			}
		}

		public float hiddenPosition
		{
			get
			{
				return (float)Main.screenHeight;
			}
		}

		public Hotbar(CheatSheet mod)
		{
			this.mod = mod;
			this.buttonView = new UIView();
			//	this.timeWindow = new TimeControlWindow();
			//	this.npcSpawnWindow = new NPCSpawnerWindow();
			//	this.weatherWindow = new WeatherControlWindow();
			//	this.timeWindow.Visible = false;
			//	this.npcSpawnWindow.Visible = false;
			//	this.weatherWindow.Visible = false;
			//	this.AddChild(this.timeWindow);
			//	this.AddChild(this.npcSpawnWindow);
			//	this.AddChild(this.weatherWindow);
			Hotbar.loginTexture = mod.GetTexture("UI/Images.login");// UIView.GetEmbeddedTexture("Images.login.png");
			Hotbar.logoutTexture = mod.GetTexture("UI/Images.logout"); //UIView.GetEmbeddedTexture("Images.logout.png");
																	   //	this.bLogin = new UIImage(Hotbar.loginTexture);
																	   //		bLogin = new UIImage(mod.GetTexture("UI/Images.login"));
			base.Visible = false;
			base.UpdateWhenOutOfBounds = true;
			//	Hotbar.groupWindow = new GroupManagementWindow();
			this.button = new UIImage(mod.GetTexture("UI/Images.CollapseBar.CollapseButtonHorizontal"));//new UIImage(UIView.GetEmbeddedTexture("Images.CollapseBar.CollapseButtonHorizontal.png"));
			this.button.UpdateWhenOutOfBounds = true;
			this.arrow = new UIImage(mod.GetTexture("UI/Images.CollapseBar.CollapseArrowHorizontal"));  //new UIImage(UIView.GetEmbeddedTexture("Images.CollapseBar.CollapseArrowHorizontal.png"));

			//		bToggleEnemies = new UIImage(mod.GetTexture("UI/Images.npcIcon"));
			//		bToggleBlockReach = new UIImage(Main.itemTexture[407]);
			//		bFlyCamera = new UIImage(Main.itemTexture[493]);
			//		bRevealMap = new UIImage(mod.GetTexture("UI/Images.canIcon"));// Hotbar.mapTexture);
			//		bWaypoints = new UIImage(mod.GetTexture("UI/Images.waypointIcon"));
			//		bGroupManager = new UIImage(mod.GetTexture("UI/Images.manageGroups"));
			//		bOnlinePlayers = new UIImage(mod.GetTexture("UI/Images.connectedPlayers"));
			//		bTime = new UIImage(mod.GetTexture("UI/Images.sunIcon"));
			//		bWeatherWindow = new UIImage(Main.npcHeadTexture[2]);// WeatherControlWindow.rainTexture);
			//		bBackupWorld = new UIImage(mod.GetTexture("UI/Images.UIKit.saveIcon"));
			//		bCTFSettings = new UIImage(mod.GetTexture("UI/Images.CTF.redFlag"));

			//	Main.instance.LoadNPC(NPCID.KingSlime);

			bToggleItemBrowser = new UIImage(Main.itemTexture[ItemID.WorkBench]);
			bToggleNPCBrowser = new UIImage(mod.GetTexture("UI/Images.npcIcon"));
			bToggleClearMenu = new UIImage(Main.itemTexture[ItemID.TrashCan]);
			bToggleRecipeBrowser = new UIImage(Main.itemTexture[ItemID.CookingPot]);
			bToggleExtendedCheat = new UIImage(Main.itemTexture[ItemID.CellPhone]);
			bTogglePaintTools = new UIImage(Main.itemTexture[ItemID.Paintbrush]);
			bCycleExtraAccessorySlots = new UIImage(Main.itemTexture[ItemID.DemonHeart]);
			bVacuum = new UIImage(mod.GetTexture("UI/Images.bVacuum"));
			bToggleNPCButcherer = new UIImage(Main.itemTexture[ItemID.Skull]);
			bToggleQuickTeleport = new UIImage(Main.itemTexture[ItemID.WoodenDoor]);
			//bToggleEventManager = new UIImage(Main.itemTexture[ItemID.PirateMap]);

			this.arrow.UpdateWhenOutOfBounds = true;
			this.button.Anchor = AnchorPosition.Top;
			this.arrow.Anchor = AnchorPosition.Top;
			this.arrow.SpriteEffect = SpriteEffects.FlipVertically;
			this.AddChild(this.button);
			this.AddChild(this.arrow);
			this.button.Position = new Vector2(0f, -this.button.Height);
			this.button.CenterXAxisToParentCenter();
			//Do i need this?		this.button.X -= 40;
			this.arrow.Position = this.button.Position;
			this.arrow.onLeftClick += new EventHandler(this.button_onLeftClick);
			//		this.bBackupWorld.onLeftClick += new EventHandler(this.bBackupWorld_onLeftClick);
			//		this.bToggleBlockReach.Tooltip = "Toggle Block Reach";
			//		this.bToggleEnemies.Tooltip = "Toggle Enemy Spawns";
			//		this.bFlyCamera.Tooltip = "Toggle Fly Cam";
			//		this.bRevealMap.Tooltip = "Reveal Map";
			//		this.bWaypoints.Tooltip = "Open Waypoints Window";
			//		this.bGroupManager.Tooltip = "Open Group Management";
			//		this.bOnlinePlayers.Tooltip = "View Connected Players";
			//		this.bTime.Tooltip = "Set Time";
			//		this.bWeatherWindow.Tooltip = "Control Rain";
			//		this.bLogin.Tooltip = "Login";
			//		this.bCTFSettings.Tooltip = "Capture the Flag Settings";
			//		this.bBackupWorld.Tooltip = "Backup World";
			this.bToggleItemBrowser.Tooltip = CSText("ShowItemBrowser");
			this.bToggleClearMenu.Tooltip = CSText("ShowClearMenu");
			this.bToggleNPCBrowser.Tooltip = CSText("ShowNPCBrowser");
			bToggleRecipeBrowser.Tooltip = CSText("ShowRecipeBrowser");
			bToggleExtendedCheat.Tooltip = CSText("ShowModExtensionCheats");
			bTogglePaintTools.Tooltip = CSText("ShowPaintTools");
			bCycleExtraAccessorySlots.Tooltip = CSText("ExtraAccessorySlots") + ": ?";
			bVacuum.Tooltip = CSText("VacuumItems");
			bToggleNPCButcherer.Tooltip = CSText("ShowNPCButcherer");
			bToggleQuickTeleport.Tooltip = CSText("ShowQuickWaypoints");
			//		bToggleEventManager.Tooltip = "Show Event Manager";

			//		this.bToggleBlockReach.Opacity = Hotbar.disabledOpacity;
			//		this.bFlyCamera.Opacity = Hotbar.disabledOpacity;
			//		this.bToggleEnemies.Opacity = Hotbar.disabledOpacity;
			//		this.bToggleBlockReach.onLeftClick += new EventHandler(this.bToggleBlockReach_onLeftClick);
			//		this.bFlyCamera.onLeftClick += new EventHandler(this.bFlyCamera_onLeftClick);
			//		this.bToggleEnemies.onLeftClick += new EventHandler(this.bToggleEnemies_onLeftClick);
			//			this.bRevealMap.onLeftClick += new EventHandler(this.bRevealMap_onLeftClick);
			//			this.bWaypoints.onLeftClick += new EventHandler(this.bWaypoints_onLeftClick);
			//		this.bGroupManager.onLeftClick += new EventHandler(this.bGroupManager_onLeftClick);
			//		this.bOnlinePlayers.onLeftClick += new EventHandler(this.bOnlinePlayers_onLeftClick);
			//		this.bCTFSettings.onLeftClick += new EventHandler(this.bCTFSettings_onLeftClick);
			//		this.bLogin.onLeftClick += new EventHandler(this.bLogin_onLeftClick);
			//		this.bTime.onLeftClick += new EventHandler(this.bTime_onLeftClick);
			//		this.bWeatherWindow.onLeftClick += new EventHandler(this.bWeatherWindow_onLeftClick);
			this.bToggleItemBrowser.onLeftClick += new EventHandler(this.bToggleItemBrowser_onLeftClick);
			this.bToggleClearMenu.onLeftClick += new EventHandler(this.bClearItems_onLeftClick);
			this.bToggleClearMenu.onRightClick += (s, e) =>
			{
				QuickClearHotbar.HandleQuickClear();
			};
			this.bToggleNPCBrowser.onLeftClick += new EventHandler(this.bToggleNPCBrowser_onLeftClick);
			this.bToggleRecipeBrowser.onLeftClick += new EventHandler(this.bToggleRecipeBrowser_onLeftClick);
			this.bToggleExtendedCheat.onLeftClick += new EventHandler(this.bToggleExtendedCheat_onLeftClick);
			this.bTogglePaintTools.onLeftClick += new EventHandler(this.bTogglePaintTools_onLeftClick);
			this.bCycleExtraAccessorySlots.onLeftClick += (s, e) =>
			{
				CheatSheetPlayer cheatSheetPlayer = Main.LocalPlayer.GetModPlayer<CheatSheetPlayer>();
				cheatSheetPlayer.numberExtraAccessoriesEnabled = (cheatSheetPlayer.numberExtraAccessoriesEnabled + 1) % (CheatSheetPlayer.MaxExtraAccessories + 1);
				bCycleExtraAccessorySlots.Tooltip = CSText("ExtraAccessorySlots") + ": " + cheatSheetPlayer.numberExtraAccessoriesEnabled;
			};
			this.bCycleExtraAccessorySlots.onRightClick += (s, e) =>
			{
				CheatSheetPlayer cheatSheetPlayer = Main.LocalPlayer.GetModPlayer<CheatSheetPlayer>();
				cheatSheetPlayer.numberExtraAccessoriesEnabled = cheatSheetPlayer.numberExtraAccessoriesEnabled == 0 ? 0 : (cheatSheetPlayer.numberExtraAccessoriesEnabled - 1) % (CheatSheetPlayer.MaxExtraAccessories + 1);
				bCycleExtraAccessorySlots.Tooltip = CSText("ExtraAccessorySlots") + ": " + cheatSheetPlayer.numberExtraAccessoriesEnabled;
			};
			this.bVacuum.onLeftClick += new EventHandler(this.bVacuum_onLeftClick);
			this.bToggleNPCButcherer.onLeftClick += new EventHandler(this.bButcher_onLeftClick);
			this.bToggleNPCButcherer.onRightClick += (s, e) =>
			{
				NPCButchererHotbar.HandleButcher();
			};
			this.bToggleQuickTeleport.onLeftClick += new EventHandler(this.bToggleQuickTeleport_onLeftClick);
			this.bToggleQuickTeleport.onRightClick += (s, e) =>
			{
				//QuickTeleportHotbar.TeleportPlayer(Main.LocalPlayer, new Vector2(Main.spawnTileX, Main.spawnTileY), true);
				QuickTeleportHotbar.HandleTeleport();
			};
			//		this.bToggleEventManager.onLeftClick += new EventHandler(this.bToggleEventManager_onLeftClick);

			//		this.buttonView.AddChild(this.bToggleBlockReach);
			//		this.buttonView.AddChild(this.bFlyCamera);
			//		this.buttonView.AddChild(this.bToggleEnemies);
			//		this.buttonView.AddChild(this.bRevealMap);
			//		this.buttonView.AddChild(this.bWaypoints);
			//		this.buttonView.AddChild(this.bTime);
			//			this.buttonView.AddChild(this.bWeatherWindow);
			//			this.buttonView.AddChild(this.bGroupManager);
			//			this.buttonView.AddChild(this.bOnlinePlayers);
			//			this.buttonView.AddChild(this.bCTFSettings);
			//			this.buttonView.AddChild(this.bLogin);
			//			this.buttonView.AddChild(this.bBackupWorld);

			buttonView.AddChild(bToggleItemBrowser);
			buttonView.AddChild(bToggleNPCBrowser);
			buttonView.AddChild(bToggleRecipeBrowser);
			buttonView.AddChild(bToggleExtendedCheat);
			buttonView.AddChild(bToggleClearMenu);
			buttonView.AddChild(bTogglePaintTools);
			buttonView.AddChild(bCycleExtraAccessorySlots);
			buttonView.AddChild(bVacuum);
			buttonView.AddChild(bToggleNPCButcherer);
			buttonView.AddChild(bToggleQuickTeleport);
			//			buttonView.AddChild(bToggleEventManager);
			buttonView.AddChild(SpawnRateMultiplier.GetButton(mod));
			buttonView.AddChild(MinionSlotBooster.GetButton(mod));
			buttonView.AddChild(LightHack.GetButton(mod));
			buttonView.AddChild(GodMode.GetButton(mod));
			//	buttonView.AddChild(FullBright.GetButton(mod));
			//			buttonView.AddChild(BossDowner.GetButton(mod));
			buttonView.AddChild(ConfigurationTool.GetButton(mod));

			base.Width = 200f;
			base.Height = 55f;
			this.buttonView.Height = base.Height;
			base.Anchor = AnchorPosition.Top;
			this.AddChild(this.buttonView);
			base.Position = new Vector2(Hotbar.xPosition, this.hiddenPosition);
			base.CenterXAxisToParentCenter();
			float num = this.spacing;
			for (int i = 0; i < this.buttonView.children.Count; i++)
			{
				this.buttonView.children[i].Anchor = AnchorPosition.Left;
				this.buttonView.children[i].Position = new Vector2(num, 0f);
				this.buttonView.children[i].CenterYAxisToParentCenter();
				this.buttonView.children[i].Visible = true;
				this.buttonView.children[i].ForegroundColor = buttonUnselectedColor;
				num += this.buttonView.children[i].Width + this.spacing;
			}
			//	Hotbar.groupWindow.Visible = false;
			//	MasterView.gameScreen.AddChild(Hotbar.groupWindow);
			ChangedConfiguration();
			//this.Resize();
			return;
		}

		private void bTogglePaintTools_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			if (mod.paintToolsHotbar.selected)
			{
				mod.paintToolsHotbar.selected = false;
				mod.paintToolsHotbar.Hide();
				uIImage.ForegroundColor = buttonUnselectedColor;
				mod.paintToolsUI.selected = false;
				mod.paintToolsUI.Hide();
			}
			else
			{
				DisableAllWindows();
				mod.paintToolsHotbar.selected = true;
				mod.paintToolsHotbar.Show();
				uIImage.ForegroundColor = buttonSelectedColor;
				mod.paintToolsUI.selected = true;
				mod.paintToolsUI.Show();
			}
		}

		private void bToggleExtendedCheat_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			if (mod.extendedCheatMenu.selected)
			{
				mod.extendedCheatMenu.selected = false;
				//	uIImage.selected = false;
				uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				DisableAllWindows();
				mod.extendedCheatMenu.selected = true;
				//	uIImage.selected = true;
				uIImage.ForegroundColor = buttonSelectedColor;
			}
			//DisableAllWindows();
			//mod.extendedCheatMenu.Visible = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			if (Visible && (IsMouseInside() || button.MouseInside))
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.showItemIcon = false;
			}

			float x = Main.fontMouseText.MeasureString(UIView.HoverText).X;
			Vector2 vector = new Vector2((float)Main.mouseX, (float)Main.mouseY) + new Vector2(16f);
			if (vector.Y > (float)(Main.screenHeight - 30))
			{
				vector.Y = (float)(Main.screenHeight - 30);
			}
			if (vector.X > (float)Main.screenWidth - x)
			{
				vector.X = (float)(Main.screenWidth - 460);
			}
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, UIView.HoverText, vector.X, vector.Y, new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), Color.Black, Vector2.Zero, 1f);
		}

		private void bToggleRecipeBrowser_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			if (mod.recipeBrowser.selected)
			{
				mod.recipeBrowser.selected = false;
				//	uIImage.selected = false;
				uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				DisableAllWindows();
				mod.recipeBrowser.selected = true;
				//	uIImage.selected = true;
				uIImage.ForegroundColor = buttonSelectedColor;
			}
			//	DisableAllWindows();
			//	mod.recipeBrowser.Visible = true;
		}

		private void bWeatherWindow_onLeftClick(object sender, EventArgs e)
		{
			//this.weatherWindow.Visible = !this.weatherWindow.Visible;
			//if (this.bWeatherWindow.Visible)
			//{
			//	this.weatherWindow.X = this.bWeatherWindow.X + this.bWeatherWindow.Width / 2f - this.weatherWindow.Width / 2f;
			//	this.weatherWindow.Y = -this.weatherWindow.Height;
			//}
		}

		private void bToggleNPCBrowser_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;

			if (mod.npcBrowser.selected)
			{
				mod.npcBrowser.selected = false;
				//		uIImage.selected = false;
				uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				DisableAllWindows();
				mod.npcBrowser.selected = true;
				//		uIImage.selected = true;
				uIImage.ForegroundColor = buttonSelectedColor;
			}
			//this.npcSpawnWindow.Visible = !this.npcSpawnWindow.Visible;
			//if (this.npcSpawnWindow.Visible)
			//{
			//	this.npcSpawnWindow.X = base.Width / 2f - this.npcSpawnWindow.Width / 2f;
			//	this.npcSpawnWindow.Y = -this.npcSpawnWindow.Height;
			//}
		}

		private void bToggleEventManager_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;

			if (mod.eventManagerHotbar.selected)
			{
				mod.eventManagerHotbar.selected = false;
				mod.eventManagerHotbar.Hide();
				uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				DisableAllWindows();
				mod.eventManagerHotbar.selected = true;
				mod.eventManagerHotbar.Show();
				uIImage.ForegroundColor = buttonSelectedColor;
			}
		}

		internal void DisableAllWindows()
		{
			mod.itemBrowser.selected = false;
			mod.npcBrowser.selected = false;
			mod.extendedCheatMenu.selected = false;
			mod.recipeBrowser.selected = false;
			mod.paintToolsHotbar.selected = false;
			mod.paintToolsUI.selected = false;
			mod.quickTeleportHotbar.selected = false;
			mod.quickClearHotbar.selected = false;
			mod.npcButchererHotbar.selected = false;
			ConfigurationTool.configurationWindow.selected = false;
			//BossDowner.bossDownerWindow.selected = false;
			//mod.eventManagerHotbar.selected = false;

			//bToggleNPCBrowser.selected = false;
			//bToggleRecipeBrowser.selected = false;
			//bToggleExtendedCheat.selected = false;
			//bToggleItemBrowser.selected = false;

			bToggleNPCBrowser.ForegroundColor = buttonUnselectedColor;
			bToggleRecipeBrowser.ForegroundColor = buttonUnselectedColor;
			bToggleExtendedCheat.ForegroundColor = buttonUnselectedColor;
			bToggleItemBrowser.ForegroundColor = buttonUnselectedColor;
			bTogglePaintTools.ForegroundColor = buttonUnselectedColor;
			bToggleQuickTeleport.ForegroundColor = buttonUnselectedColor;
			bToggleClearMenu.ForegroundColor = buttonUnselectedColor;
			ConfigurationTool.button.ForegroundColor = buttonUnselectedColor;
			//BossDowner.button.ForegroundColor = buttonUnselectedColor;
			//bToggleEventManager.ForegroundColor = buttonUnselectedColor;
		}

		private void bCTFSettings_onLeftClick(object sender, EventArgs e)
		{
			//if (Mod.ctf.GameInProgress || Mod.ctf.inLobby)
			//{
			//	Mod.ctf.ToggleTeamListVisible();
			//	return;
			//}
			//Mod.ctf.ToggleSettingsWindow();
		}

		private void bBackupWorld_onLeftClick(object sender, EventArgs e)
		{
			//ServerTools.SendTextToServer("¶606", default(Color));
		}

		private void bTime_onLeftClick(object sender, EventArgs e)
		{
			//this.timeWindow.Visible = !this.timeWindow.Visible;
			//if (this.timeWindow.Visible)
			//{
			//	this.timeWindow.X = this.bTime.X + this.bTime.Width / 2f - this.timeWindow.Width / 2f;
			//	this.timeWindow.Y = -this.timeWindow.Height;
			//}
		}

		private void bOnlinePlayers_onLeftClick(object sender, EventArgs e)
		{
			//ServerTools.playersWindow.Visible = !ServerTools.playersWindow.Visible;
		}

		private void bGroupManager_onLeftClick(object sender, EventArgs e)
		{
			//Hotbar.groupWindow.Visible = !Hotbar.groupWindow.Visible;
		}

		private void bLogin_onLeftClick(object sender, EventArgs e)
		{
			//if (this.bLogin.Tooltip == "Login")
			//{
			//	MasterView.gameScreen.AddChild(new LoginWindow());
			//	return;
			//}
			//ServerTools.SendTextToServer("¶urz", default(Color));
		}

		private void bWaypoints_onLeftClick(object sender, EventArgs e)
		{
			//Waypoints.ToggleWindow();
		}

		private void bRevealMap_onLeftClick(object sender, EventArgs e)
		{
			//Creative.RevealMap();
		}

		private void button_onLeftClick(object sender, EventArgs e)
		{
			if (this.hidden)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
			//this.hidden = !this.hidden;
			//if (this.hidden)
			//{
			//	//this.timeWindow.Visible = false;
			//	this.arrow.SpriteEffect = SpriteEffects.None;
			//	return;
			//}
			//this.arrow.SpriteEffect = SpriteEffects.FlipVertically;
		}

		private void bClearItems_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			if (mod.quickClearHotbar.selected)
			{
				mod.quickClearHotbar.selected = false;
				mod.quickClearHotbar.Hide();
				uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				DisableAllWindows();
				mod.quickClearHotbar.selected = true;
				mod.quickClearHotbar.Show();
				uIImage.ForegroundColor = buttonSelectedColor;
			}
		}

		private void bToggleEnemies_onLeftClick(object sender, EventArgs e)
		{
			//Creative.ToggleNPCs(0);
		}

		private void bFlyCamera_onLeftClick(object sender, EventArgs e)
		{
			//Creative.ToggleFlyCam();
		}

		private void bToggleBlockReach_onLeftClick(object sender, EventArgs e)
		{
			//Creative.ToggleBlockReach();
		}

		private void bToggleItemBrowser_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			if (mod.itemBrowser.selected)
			{
				mod.itemBrowser.selected = false;
				uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				DisableAllWindows();
				mod.itemBrowser.selected = true;
				uIImage.ForegroundColor = buttonSelectedColor;
			}
			//	DisableAllWindows();
			//	mod.itemBrowser.Visible = true;
			//Creative.ToggleItemBrowser();
		}

		private void bVacuum_onLeftClick(object sender, EventArgs e)
		{
			//case 90 or 21
			HandleVacuum();
		}

		public static void HandleVacuum(bool forceHandle = false, int whoAmI = 0)
		{
			bool syncData = forceHandle || Main.netMode == 0;
			if (syncData)
			{
				VacuumItems(forceHandle, whoAmI);
			}
			else
			{
				SyncVacuum();
			}
		}

		private static void SyncVacuum()
		{
			var netMessage = CheatSheet.instance.GetPacket();
			netMessage.Write((byte)CheatSheetMessageType.VacuumItems);
			netMessage.Send();
		}

		private static void VacuumItems(bool syncData = false, int whoAmI = 0)
		{
			/*
            	Item item2 = Main.item[number];
				writer.Write((short)number);
				writer.WriteVector2(item2.position);
				writer.WriteVector2(item2.velocity);
				writer.Write((short)item2.stack);
				writer.Write(item2.prefix);
				writer.Write((byte)number2);
				writer.Write(value); //netID

                int num56 = (int)this.reader.ReadInt16();
				Vector2 vector = this.reader.ReadVector2();
				Vector2 velocity = this.reader.ReadVector2();
				int stack3 = (int)this.reader.ReadInt16();
				int pre = (int)this.reader.ReadByte();
				int num57 = (int)this.reader.ReadByte();
				int num58 = (int)this.reader.ReadInt16();
            */
			Player player;
			if (!syncData)
			{
				player = Main.LocalPlayer;
			}
			else
			{
				player = Main.player[whoAmI];
			}
			Vector2 changePos = new Vector2((int)player.position.X, (int)player.position.Y);
			for (int i = 0; i < Main.item.Length; i++)
			{
				if (Main.item[i].active)
				{
					Main.item[i].position = changePos;
					if (syncData)
					{
						NetMessage.SendData(21, -1, -1, null, i, Main.item[i].netID, 0f, 0f, 0);
					}
				}
			}
		}

		private void bButcher_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			if (mod.npcButchererHotbar.selected)
			{
				mod.npcButchererHotbar.selected = false;
				mod.npcButchererHotbar.Hide();
				uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				DisableAllWindows();
				mod.npcButchererHotbar.selected = true;
				mod.npcButchererHotbar.Show();
				uIImage.ForegroundColor = buttonSelectedColor;
			}
		}

		private void bToggleQuickTeleport_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			if (mod.quickTeleportHotbar.selected)
			{
				mod.quickTeleportHotbar.selected = false;
				mod.quickTeleportHotbar.Hide();
				uIImage.ForegroundColor = buttonUnselectedColor;
			}
			else
			{
				DisableAllWindows();
				mod.quickTeleportHotbar.selected = true;
				mod.quickTeleportHotbar.Show();
				uIImage.ForegroundColor = buttonSelectedColor;
			}
		}

		public override void Update()
		{
			try
			{
				if (this.hidden)
				{
					this.lerpAmount -= /*Mod.deltaTime*/ .01f * Hotbar.moveSpeed;
					if (this.lerpAmount < 0f)
					{
						this.lerpAmount = 0f;
					}
					float y = MathHelper.SmoothStep(this.hiddenPosition, this.shownPosition, this.lerpAmount);
					base.Position = new Vector2(Hotbar.xPosition, y);
				}
				else
				{
					this.lerpAmount += .01f/*Mod.deltaTime */* Hotbar.moveSpeed;
					if (this.lerpAmount > 1f)
					{
						this.lerpAmount = 1f;
					}
					float y2 = MathHelper.SmoothStep(this.hiddenPosition, this.shownPosition, this.lerpAmount);
					base.Position = new Vector2(Hotbar.xPosition, y2);
				}
				if (mod.paintToolsHotbar.Visible || mod.quickTeleportHotbar.Visible || mod.quickClearHotbar.Visible || mod.npcButchererHotbar.Visible/* || mod.eventManagerHotbar.Visible*/)
				{
					int offset = mod.npcButchererHotbar.Visible ? (int)this.button.Width : 0;
					this.button.Position = new Vector2(0, -this.button.Height - Math.Max(0, (base.Position.Y - currentHotbar.Position.Y)));
					button.CenterXAxisToParentCenter(offset);
					arrow.Position = button.Position;
				}
				else
				{
					this.button.Position = new Vector2(0, -this.button.Height);
					button.CenterXAxisToParentCenter();
					arrow.Position = button.Position;
				}
				base.CenterXAxisToParentCenter();
				base.Update();
			}
			catch (Exception e)
			{
				ErrorLogger.Log(e.ToString());
			}
		}

		//public void EnableAllControls(bool login)
		//{
		//	//if (Mod.ctf.GameInProgress)
		//	//{
		//	//	this.SetCTFControls(ServerTools.group);
		//	//	return;
		//	//}
		//	for (int i = 0; i < this.buttonView.children.Count; i++)
		//	{
		//		this.buttonView.children[i].Visible = true;
		//	}
		//	//this.bLogin.Visible = login;
		//	//if (Main.netMode != 1)
		//	//{
		//	//	this.bGroupManager.Visible = false;
		//	//	this.bOnlinePlayers.Visible = false;
		//	//	this.bBackupWorld.Visible = false;
		//	//	this.bCTFSettings.Visible = false;
		//	//}
		//	this.Resize();
		//}

		//public void EnableControls(Group group)
		//{
		//	//if (Mod.ctf.GameInProgress)
		//	//{
		//	//	this.SetCTFControls(group);
		//	//	return;
		//	//}
		//	//if (group.admin)
		//	//{
		//	//	this.EnableAllControls(true);
		//	//	return;
		//	//}
		//	//this.DisableAllControls(true);
		//	//this.bToggleItemBrowser.Visible = group.itemBrowser;
		//	//this.bWaypoints.Visible = group.accessWaypoints;
		//	//this.bToggleBlockReach.Visible = group.blockReach;
		//	//this.bRevealMap.Visible = group.mapReveal;
		//	//this.bTime.Visible = group.timeControl;
		//	//this.bWeatherWindow.Visible = group.timeControl;
		//	//this.bToggleEnemies.Visible = group.disableEnemies;
		//	//this.bClearItems.Visible = group.clearItems;
		//	//this.bSpawnWindow.Visible = group.spawnNPCs;
		//	//this.bCTFSettings.Visible = group.startCTF;
		//	//if (!group.itemBrowser)
		//	//{
		//	//	Creative.itemBrowser.Visible = false;
		//	//}
		//	//if (!group.accessWaypoints)
		//	//{
		//	//	Waypoints.HideWindow();
		//	//}
		//	//if (!group.blockReach)
		//	//{
		//	//	Creative.fastItems = false;
		//	//}
		//	//Hotbar.groupWindow.Visible = false;
		//	//ServerTools.playersWindow.ClosePlayerInfo();
		//	//ServerTools.playersWindow.Visible = false;
		//	//if (group.kick || group.ban || group.teleportTo || group.snoop)
		//	//{
		//	//	this.bOnlinePlayers.Visible = true;
		//	//}
		//	this.Resize();
		//}

		//public void SetCTFControls(Group group)
		//{
		//	this.DisableAllControls(true);
		//	this.AddCTFControl(group);
		//}

		//public void AddCTFControl(Group group)
		//{
		//	//if (group.admin)
		//	//{
		//	//	this.bOnlinePlayers.Visible = true;
		//	//}
		//	//this.bCTFSettings.Visible = true;
		//	this.Resize();
		//}

		private bool ControlExists(UIView view)
		{
			foreach (UIView current in this.children)
			{
				if (current == view)
				{
					return true;
				}
			}
			return false;
		}

		//public void DisableAllControls(bool login)
		//{
		//	for (int i = 0; i < this.buttonView.children.Count; i++)
		//	{
		//		this.buttonView.children[i].Visible = false;
		//	}
		//	//this.bLogin.Visible = login;
		//	this.Resize();
		//}

		public void ChangedConfiguration()
		{
			DisableAllWindows();
			bToggleItemBrowser.Visible = ConfigurationLoader.personalConfiguration.ItemBrowser;
			bToggleNPCBrowser.Visible = ConfigurationLoader.personalConfiguration.NPCBrowser;
			bToggleRecipeBrowser.Visible = ConfigurationLoader.personalConfiguration.RecipeBrowser;
			MinionSlotBooster.button.Visible = ConfigurationLoader.personalConfiguration.MinionBooster;
			bToggleClearMenu.Visible = ConfigurationLoader.personalConfiguration.ClearMenu;
			bTogglePaintTools.Visible = ConfigurationLoader.personalConfiguration.PaintTools;
			bToggleExtendedCheat.Visible = ConfigurationLoader.personalConfiguration.ModExtensions;
			bCycleExtraAccessorySlots.Visible = ConfigurationLoader.personalConfiguration.ExtraAccessorySlots;
			bVacuum.Visible = ConfigurationLoader.personalConfiguration.Vacuum;
			bToggleNPCButcherer.Visible = ConfigurationLoader.personalConfiguration.Butcher;
			bToggleQuickTeleport.Visible = ConfigurationLoader.personalConfiguration.Waypoints;
			LightHack.button.Visible = ConfigurationLoader.personalConfiguration.LightHack;
			GodMode.button.Visible = ConfigurationLoader.personalConfiguration.GodMode;
			SpawnRateMultiplier.button.Visible = ConfigurationLoader.personalConfiguration.SpawnRate && SpawnRateMultiplier.HasPermission;
			//BossDowner.button.Visible = ConfigurationLoader.configuration.BossDowner;
			//bToggleEventManager.Visible = ConfigurationLoader.configuration.EventManager;
			//Main.NewText("bToggleItemBrowser " + bToggleItemBrowser.Visible);
			Resize();
		}

		//public void ChangedBossDowner()
		//{
		//	DisableAllWindows();
		//	//todo: implement All Towers / All mech bosses
		//	Resize();
		//}

		public void Resize()
		{
			float num = this.spacing;
			for (int i = 0; i < this.buttonView.children.Count; i++)
			{
				if (this.buttonView.children[i].Visible)
				{
					this.buttonView.children[i].X = num;
					num += this.buttonView.children[i].Width + this.spacing;
				}
			}
			base.Width = num;
			this.buttonView.Width = base.Width;
			this.button.CenterXAxisToParentCenter();
			this.arrow.Position = this.button.Position;
		}

		public void Hide()
		{
			this.hidden = true;
			this.arrow.SpriteEffect = SpriteEffects.None;
			if (mod.itemBrowser.selected && !mod.itemBrowser.hidden)
			{
				mod.itemBrowser.Hide();
			}
			if (mod.recipeBrowser.selected && !mod.recipeBrowser.hidden)
			{
				mod.recipeBrowser.Hide();
			}
			if (mod.npcBrowser.selected && !mod.npcBrowser.hidden)
			{
				mod.npcBrowser.Hide();
			}
			if (mod.extendedCheatMenu.selected && !mod.extendedCheatMenu.hidden)
			{
				mod.extendedCheatMenu.Hide();
			}
			if (mod.paintToolsHotbar.selected && !mod.paintToolsHotbar.hidden)
			{
				mod.paintToolsHotbar.Hide();
			}
			if (mod.paintToolsUI.selected && !mod.paintToolsUI.hidden)
			{
				mod.paintToolsUI.Hide();
			}
			if (mod.quickTeleportHotbar.selected && !mod.quickTeleportHotbar.hidden)
			{
				mod.quickTeleportHotbar.Hide();
			}
			if (mod.quickClearHotbar.selected && !mod.quickClearHotbar.hidden)
			{
				mod.quickClearHotbar.Hide();
			}
			if (mod.npcButchererHotbar.selected && !mod.npcButchererHotbar.hidden)
			{
				mod.npcButchererHotbar.Hide();
			}
			if (ConfigurationTool.configurationWindow.selected && !ConfigurationTool.configurationWindow.hidden)
			{
				ConfigurationTool.configurationWindow.Hide();
			}
			//if (BossDowner.bossDownerWindow.selected && !BossDowner.bossDownerWindow.hidden)
			//{
			//	BossDowner.bossDownerWindow.Hide();
			//}
			//if (mod.eventManagerHotbar.selected && !mod.eventManagerHotbar.hidden)
			//{
			//	mod.eventManagerHotbar.Hide();
			//}
		}

		public void Show()
		{
			this.hidden = false;
			this.arrow.SpriteEffect = SpriteEffects.FlipVertically;
			if (mod.itemBrowser.selected)
			{
				mod.itemBrowser.Show();
			}
			if (mod.recipeBrowser.selected)
			{
				mod.recipeBrowser.Show();
			}
			if (mod.npcBrowser.selected)
			{
				mod.npcBrowser.Show();
			}
			if (mod.extendedCheatMenu.selected)
			{
				mod.extendedCheatMenu.Show();
			}
			if (mod.paintToolsHotbar.selected)
			{
				mod.paintToolsHotbar.Show();
			}
			if (mod.paintToolsUI.selected)
			{
				mod.paintToolsUI.Show();
			}
			if (mod.quickTeleportHotbar.selected)
			{
				mod.quickTeleportHotbar.Show();
			}
			if (mod.quickClearHotbar.selected)
			{
				mod.quickClearHotbar.Show();
			}
			if (mod.npcButchererHotbar.selected)
			{
				mod.npcButchererHotbar.Show();
			}
			if (ConfigurationTool.configurationWindow.selected)
			{
				ConfigurationTool.configurationWindow.Show();
			}
			//if (BossDowner.bossDownerWindow.selected)
			//{
			//	BossDowner.bossDownerWindow.Show();
			//}
			//if (mod.eventManagerHotbar.selected)
			//{
			//	mod.eventManagerHotbar.Show();
			//}
		}
	}
}

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace CheatSheet.UI
//{
//	class CheatMenu : UIWindow
//	{
//		public Mod mod;
//		private static UIImage[] buttons = new UIImage[CheatSheet.ButtonTexture.Count];
//		private float spacing = 16f;

//		public CheatMenu(Mod mod)
//		{
//			this.mod = mod;
//			this.CanMove = true;
//			base.Width = 40 + spacing * 2;
//			base.Height = 400f;
//			Texture2D texture = mod.GetTexture("UI/closeButton");
//			UIImage uIImage = new UIImage(texture);
//			uIImage.Anchor = AnchorPosition.TopRight;
//			uIImage.Position = new Vector2(base.Width - this.spacing / 2, this.spacing / 2);
//			uIImage.onLeftClick += new EventHandler(this.bClose_onLeftClick);
//			this.AddChild(uIImage);

//			for (int j = 0; j < CheatSheet.ButtonTexture.Count; j++)
//			{
//				UIImage button = new UIImage(CheatSheet.ButtonTexture[j]);
//				Vector2 position = new Vector2(this.spacing, this.spacing);
//				button.Scale = 32f / Math.Max(CheatSheet.ButtonTexture[j].Width, CheatSheet.ButtonTexture[j].Height);

//				position.X += (float)(j % 1 * 40);
//				position.Y += (float)(j / 1 * 40);

//				if (CheatSheet.ButtonTexture[j].Height > CheatSheet.ButtonTexture[j].Width)
//				{
//					position.X += (32 - CheatSheet.ButtonTexture[j].Width) / 2;
//				}
//				else if (CheatSheet.ButtonTexture[j].Height < CheatSheet.ButtonTexture[j].Width)
//				{
//					position.Y += (32 - CheatSheet.ButtonTexture[j].Height) / 2;
//				}

//				button.Position = position;
//				button.Tag = j;
//				button.onLeftClick += new EventHandler(this.button_onLeftClick);
//				button.onHover += new EventHandler(this.button_onHover);
//				//	button.ForegroundColor = RecipeBrowser.buttonColor;
//				//	uIImage2.Tooltip = RecipeBrowser.categNames[j];
//				ExtendedCheatMenu.buttons[j] = button;
//				this.AddChild(button);
//			}
//		}

//		public override void Draw(SpriteBatch spriteBatch)
//		{
//			base.Draw(spriteBatch);

//			if (Visible && IsMouseInside())
//			{
//				Main.LocalPlayer.mouseInterface = true;
//				Main.LocalPlayer.showItemIcon = false;
//			}

//			float x = Main.fontMouseText.MeasureString(UIView.HoverText).X;
//			Vector2 vector = new Vector2((float)Main.mouseX, (float)Main.mouseY) + new Vector2(16f);
//			if (vector.Y > (float)(Main.screenHeight - 30))
//			{
//				vector.Y = (float)(Main.screenHeight - 30);
//			}
//			if (vector.X > (float)Main.screenWidth - x)
//			{
//				vector.X = (float)(Main.screenWidth - 460);
//			}
//			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, UIView.HoverText, vector.X, vector.Y, new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), Color.Black, Vector2.Zero, 1f);
//		}

//		public override void Update()
//		{
//			base.Update();
//		}

//		private void bClose_onLeftClick(object sender, EventArgs e)
//		{
//			base.Visible = false;
//		}

//		private void button_onLeftClick(object sender, EventArgs e)
//		{
//			UIImage uIImage = (UIImage)sender;
//			int num = (int)uIImage.Tag;

//			CheatSheet.ButtonClicked[num](0);
//		}

//		private void button_onHover(object sender, EventArgs e)
//		{
//			UIImage uIImage = (UIImage)sender;
//			int num = (int)uIImage.Tag;

//			uIImage.Tooltip = CheatSheet.ButtonTooltip[num]();
//		}
//	}
//}