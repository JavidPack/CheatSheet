using CheatSheet.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

// TODO: move windows below inventory
// TODO: Filter recipes with unobtainables.
// TODO: debugmode, stat menu (from CMM)

// netMode 0 single player, netMode 1 multiplayer, netMode 2 Server
// netMode type 21 sync Main.item

namespace CheatSheet
{
	internal class CheatSheet : Mod
	{
		internal static ModHotKey ToggleCheatSheetHotbarHotKey;
		internal static CheatSheet instance;
		internal static Dictionary<string, ModTranslation> translations; // reference to private field.
		internal Hotbar hotbar;
		internal ItemBrowser itemBrowser;
		internal NPCBrowser npcBrowser;
		internal RecipeBrowserWindow recipeBrowser;
		internal ExtendedCheatMenu extendedCheatMenu;
		internal PaintToolsHotbar paintToolsHotbar;
		internal PaintToolsUI paintToolsUI;
		internal QuickTeleportHotbar quickTeleportHotbar;
		internal QuickClearHotbar quickClearHotbar;
		internal NPCButchererHotbar npcButchererHotbar;
		internal EventManagerHotbar eventManagerHotbar;

		public CheatSheet()
		{
		}

		// to do: debugmode, stat

		public override void Load()
		{
			// Since we are using hooks not in older versions, and since ItemID.Count changed, we need to do this.
			if (ModLoader.version < new Version(0, 10, 1, 3))
			{
				throw new Exception("\nThis mod uses functionality only present in the latest tModLoader. Please update tModLoader to use this mod\n\n");
			}
			instance = this;

			ButtonClicked.Clear();
			ButtonTexture.Clear();
			ButtonTooltip.Clear();

			ToggleCheatSheetHotbarHotKey = RegisterHotKey("Toggle Cheat Sheet Hotbar", "K");

			if (Main.rand == null)
			{
				Main.rand = new Terraria.Utilities.UnifiedRandom();
			}

			FieldInfo translationsField = typeof(Mod).GetField("translations", BindingFlags.Instance | BindingFlags.NonPublic);
			translations = (Dictionary<string, ModTranslation>)translationsField.GetValue(this);
			//LoadTranslations();
		}

		public override void Unload()
		{
			PaintToolsSlot.CurrentSelect = null;
			AllItemsMenu.singleSlotArray = null;
			UI.UICheckbox.checkboxTexture = null;
			UI.UICheckbox.checkmarkTexture = null;
			UI.UIScrollBar.ScrollbarTexture = null;
			UI.UIScrollView.ScrollbgTexture = null;
			UI.UITextbox.textboxBackground = null;
			//UI.UIView.closeTexture = null;
			ItemBrowser.bCategories = null;
			RecipeBrowserWindow.ingredients = null;
			RecipeBrowserWindow.bCategories = null;
			NPCBrowser.tooltipNpc = null;
			NPCBrowser.hoverNpc = null;
			NPCBrowser.bCategories = null;
			if (itemBrowser != null)
				itemBrowser.itemView = null;
			itemBrowser = null;
			npcBrowser = null;
			recipeBrowser = null;
			if (hotbar != null)
			{
				hotbar.buttonView?.RemoveAllChildren();
				hotbar.buttonView = null;
				hotbar = null;
			}
			instance = null;
			ToggleCheatSheetHotbarHotKey = null;
			RecipeBrowserWindow.recipeView = null;
			RecipeBrowserWindow.lookupItemSlot = null;
			ConfigurationTool.cheatSheet = null;
			ConfigurationTool.configurationWindow = null;
			Hotbar.loginTexture = null;
			Hotbar.logoutTexture = null;
			ConfigurationTool.button = null;
			SpawnRateMultiplier.button = null;
			MinionSlotBooster.button = null;
			LightHack.button = null;
			GodMode.button = null;
		}

		internal static string CSText(string category, string key)
		{	
			category = category != null ? category : String.Empty;
			key = key != null ? key : String.Empty; 

			string returnTrans = translations[$"Mods.CheatSheet.{category}.{key}"].GetTranslation(Language.ActiveCulture); 
			
			return returnTrans != null ? returnTrans : String.Empty;
			
			// This isn't good until after load....can revert after fixing static initializers for string[]
			// return Language.GetTextValue($"Mods.CheatSheet.{category}.{key}");
		}

		/*
		private void LoadTranslations()
		{
			var modTranslationDictionary = new Dictionary<string, ModTranslation>();

			var translationFiles = new List<string>();
			foreach (var item in File)
			{
				if (item.Key.StartsWith("Localization"))
					translationFiles.Add(item.Key);
			}
			foreach (var translationFile in translationFiles)
			{
				string translationFileContents = System.Text.Encoding.UTF8.GetString(GetFileBytes(translationFile));
				GameCulture culture = GameCulture.FromName(Path.GetFileNameWithoutExtension(translationFile));
				Dictionary<string, Dictionary<string, string>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(translationFileContents);
				foreach (KeyValuePair<string, Dictionary<string, string>> category in dictionary)
					foreach (KeyValuePair<string, string> kvp in category.Value)
					{
						ModTranslation mt;
						string key = category.Key + "." + kvp.Key;
						if (!modTranslationDictionary.TryGetValue(key, out mt))
							modTranslationDictionary[key] = mt = CreateTranslation(key);
						mt.AddTranslation(culture, kvp.Value);
					}
			}

			foreach (var value in modTranslationDictionary.Values)
			{
				AddTranslation(value);
			}
		}
		*/

		//public override void PreSaveAndQuit()
		//{
		//	SpawnRateMultiplier.HasPermission = true;
		//	CheatSheet.instance.hotbar.ChangedConfiguration();
		//}

		public override void PostSetupContent()
		{
			ConfigurationLoader.Initialized();
			try
			{
				Mod herosMod = ModLoader.GetMod("HEROsMod");
				if (herosMod != null)
				{
					SetupHEROsModIntegration(herosMod);
				}
			}
			catch (Exception e)
			{
				ErrorLogger.Log("CheatSheet->HEROsMod PostSetupContent Error: " + e.StackTrace + e.Message);
			}
		}

		private void SetupHEROsModIntegration(Mod herosMod)
		{
			// Add Permissions always.
			herosMod.Call(
				// Special string
				"AddPermission",
				// Permission Name
				"ModifySpawnRateMultiplier",
				// Permission Display Name
				"Modify Spawn Rate Multiplier"
			);
			// Add Buttons only to non-servers (otherwise the server will crash, since textures aren't loaded on servers)
			if (!Main.dedServ)
			{
				herosMod.Call(
					// Special string
					"AddSimpleButton",
					// Name of Permission governing the availability of the button/tool
					"ModifySpawnRateMultiplier",
					// Texture of the button. 38x38 is recommended for HERO's Mod. Also, a white outline on the icon similar to the other icons will look good.
					Main.itemTexture[ItemID.WaterCandle],
					// A method that will be called when the button is clicked
					(Action)SpawnRateMultiplier.HEROsButtonPressed,
					// A method that will be called when the player's permissions have changed
					(Action<bool>)SpawnRateMultiplier.HEROsPermissionChanged,
					// A method that will be called when the button is hovered, returning the Tooltip
					(Func<string>)SpawnRateMultiplier.HEROsTooltip
				);
			}
		}

		public override void AddRecipeGroups()
		{
			if (!Main.dedServ)
			{
				try
				{
					itemBrowser = new ItemBrowser(this);
					itemBrowser.SetDefaultPosition(new Vector2(80, 300));
					itemBrowser.Visible = false;

					npcBrowser = new NPCBrowser(this);
					npcBrowser.SetDefaultPosition(new Vector2(30, 180));
					npcBrowser.Visible = false;

					recipeBrowser = new RecipeBrowserWindow(this);
					recipeBrowser.SetDefaultPosition(new Vector2(30, 180));
					recipeBrowser.Visible = false;

					extendedCheatMenu = new ExtendedCheatMenu(this);
					extendedCheatMenu.SetDefaultPosition(new Vector2(120, 180));
					extendedCheatMenu.Visible = false;

					paintToolsHotbar = new PaintToolsHotbar(this);
					//	paintToolsHotbar.SetDefaultPosition(new Microsoft.Xna.Framework.Vector2(120, 180));
					paintToolsHotbar.Visible = false;
					paintToolsHotbar.Hide();

					paintToolsUI = new PaintToolsUI(this);
					paintToolsUI.SetDefaultPosition(new Vector2(30, 180));
					paintToolsUI.Visible = false;

					quickTeleportHotbar = new QuickTeleportHotbar(this);
					quickTeleportHotbar.Visible = false;
					quickTeleportHotbar.Hide();

					quickClearHotbar = new QuickClearHotbar(this);
					quickClearHotbar.Visible = false;
					quickClearHotbar.Hide();

					npcButchererHotbar = new NPCButchererHotbar(this);
					npcButchererHotbar.Visible = false;
					npcButchererHotbar.Hide();

					//eventManagerHotbar = new EventManagerHotbar(this);
					//eventManagerHotbar.Visible = false;
					//eventManagerHotbar.Hide();

					hotbar = new Hotbar(this);
					//hotbar.Position = new Microsoft.Xna.Framework.Vector2(120, 180);
					hotbar.Visible = true;
					hotbar.Hide();
				}
				catch (Exception e)
				{
					ErrorLogger.Log(e.ToString());
				}
			}
		}

		internal const int DefaultNumberOnlineToLoad = 30;
		public int numberOnlineToLoad = 0;

		public override void UpdateUI(GameTime gameTime)
		{
			base.UpdateUI(gameTime);

			if (PaintToolsEx.schematicsToLoad != null && numberOnlineToLoad > 0 && CheatSheet.instance.paintToolsUI.view.childrenToRemove.Count == 0)
			{
				PaintToolsEx.LoadSingleSchematic();
				//CheatSheet.instance.paintToolsUI.view.ReorderSlots();
			}

			if (PaintToolsSlot.updateNeeded)
			{
				bool oneUpdated = false;
				foreach (var item in paintToolsUI.view.slotList)
				{
					if (item.texture == Main.magicPixel)
					{
						item.texture = item.MakeThumbnail(item.stampInfo);
						oneUpdated = true;
						break;
					}
				}
				if (!oneUpdated)
					PaintToolsSlot.updateNeeded = false;
			}
		}

		//public override void PostDrawFullscreenMap(ref string mouseText)
		//{
		//	Main.spriteBatch.DrawString(Main.fontMouseText, "Testing Testing", new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), Color.Pink, 0.0f, new Vector2(), 0.8f, SpriteEffects.None, 0.0f);
		//}

		private int lastmode = -1;

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.netMode != lastmode)
			{
				lastmode = Main.netMode;
				if (Main.netMode == 0)
				{
					SpawnRateMultiplier.HasPermission = true;
				}
				CheatSheet.instance.hotbar.ChangedConfiguration();
			}
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"CheatSheet: All Cheat Sheet",
					delegate
					{
						AllItemsMenu menu = (AllItemsMenu)this.GetGlobalItem("AllItemsMenu");
						menu.DrawUpdateAll(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"CheatSheet: Paint Tools",
					delegate
					{
						AllItemsMenu menu = (AllItemsMenu)this.GetGlobalItem("AllItemsMenu");
						menu.DrawUpdatePaintTools(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.Game)
				);
			}

			MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"CheatSheet: Extra Accessories",
					delegate
					{
						AllItemsMenu menu = (AllItemsMenu)this.GetGlobalItem("AllItemsMenu");
						menu.DrawUpdateExtraAccessories(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		//public override void PostDrawInterface(SpriteBatch spriteBatch)
		//{
		//	//Main.spriteBatch.DrawString(Main.fontMouseText, "Drawn Always", new Vector2(Main.screenWidth/2, Main.screenHeight/2 + 20), Color.Aquamarine, 0.0f, new Vector2(), 0.8f, SpriteEffects.None, 0.0f);
		//	AllItemsMenu menu = (AllItemsMenu)this.GetGlobalItem("AllItemsMenu");
		//	menu.DrawUpdateAll(spriteBatch);
		//}

		//public override void PostUpdateInput()
		//{
		//	if (!Main.gameMenu)
		//	{
		//		//UIView.UpdateUpdateInput();
		//		AllItemsMenu menu = (AllItemsMenu)this.GetGlobalItem("AllItemsMenu");
		//		menu.UpdateInput();
		//	}
		//}

		//public override void UpdateMusic(ref int music)
		//{
		//	PreviousKeyState = Main.keyState;
		//}

		private KeyboardState PreviousKeyState;

		public void RegisterButton(Texture2D texture, Action buttonClickedAction, Func<string> tooltip)
		{
			ButtonClicked.Add(buttonClickedAction);
			ButtonTexture.Add(texture);
			ButtonTooltip.Add(tooltip);
			//ErrorLogger.Log("1 "+ButtonClicked.Count);
			//ErrorLogger.Log("2 "+ ButtonTexture.Count);
			//ErrorLogger.Log("3 "+ ButtonTooltip.Count);
		}

		internal static List<Action> ButtonClicked = new List<Action>();
		internal static List<Texture2D> ButtonTexture = new List<Texture2D>();
		internal static List<Func<string>> ButtonTooltip = new List<Func<string>>();

		public override object Call(params object[] args)
		{
			string message = args[0] as string;
			if (message == "AddButton_Test")
			{
				ErrorLogger.Log("Button Adding...");
				RegisterButton(args[1] as Texture2D, args[2] as Action, args[3] as Func<string>);
				ErrorLogger.Log("...Button Added");
			}
			return null;
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			CheatSheetMessageType msgType = (CheatSheetMessageType)reader.ReadByte();
			string key;

			switch (msgType)
			{
				case CheatSheetMessageType.SpawnNPC:
					int npcType = reader.ReadInt32();
					int netID = reader.ReadInt32();
					NPCSlot.HandleNPC(npcType, netID, true, whoAmI);
					key = "Mods.CheatSheet.MobBrowser.SpawnNPCNotification";
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key, netID, Netplay.Clients[whoAmI].Name), Color.Azure);
					//message = "Spawned " + netID + " by " + Netplay.Clients[whoAmI].Name;
					//NetMessage.SendData(25, -1, -1, message, 255, Color.Azure.R, Color.Azure.G, Color.Azure.B, 0);
					break;

				case CheatSheetMessageType.QuickClear:
					int clearType = reader.ReadInt32();
					switch (clearType)
					{
						case 0:
							key = "Mods.CheatSheet.QuickClear.ItemClearNotification";
							//message = "Items were cleared by ";
							break;

						case 1:
							key = "Mods.CheatSheet.QuickClear.ProjectileClearNotification";
							//message = "Projectiles were cleared by ";
							break;

						default:
							key = "";
							break;
					}
					//message += Netplay.Clients[whoAmI].Name;
					QuickClearHotbar.HandleQuickClear(clearType, true, whoAmI);
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key, Netplay.Clients[whoAmI].Name), Color.Azure);
					//NetMessage.SendData(25, -1, -1, message, 255, Color.Azure.R, Color.Azure.G, Color.Azure.B, 0);
					break;

				case CheatSheetMessageType.VacuumItems:
					Hotbar.HandleVacuum(true, whoAmI);
					key = "Mods.CheatSheet.Vacuum.VacuumNotification";
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key, Netplay.Clients[whoAmI].Name), Color.Azure);
					//message = "Items on the ground were vacuumed by " + Netplay.Clients[whoAmI].Name;
					//NetMessage.SendData(25, -1, -1, message, 255, Color.Azure.R, Color.Azure.G, Color.Azure.B, 0);
					break;

				case CheatSheetMessageType.ButcherNPCs:
					NPCButchererHotbar.HandleButcher(reader.ReadInt32(), true);
					key = "Mods.CheatSheet.Butcherer.ButcherNotification";
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key, Netplay.Clients[whoAmI].Name), Color.Azure);
					//message = "NPCs were butchered by " + Netplay.Clients[whoAmI].Name;
					//NetMessage.SendData(25, -1, -1, message, 255, Color.Azure.R, Color.Azure.G, Color.Azure.B, 0);
					break;

				case CheatSheetMessageType.TeleportPlayer:
					QuickTeleportHotbar.HandleTeleport(reader.ReadInt32(), true, whoAmI);
					break;

				case CheatSheetMessageType.SetSpawnRate:
					SpawnRateMultiplier.HandleSetSpawnRate(reader, whoAmI);
					break;

				case CheatSheetMessageType.SpawnRateSet:
					SpawnRateMultiplier.HandleSpawnRateSet(reader, whoAmI);
					break;

				case CheatSheetMessageType.RequestFilterNPC:
					int netID2 = reader.ReadInt32();
					bool desired = reader.ReadBoolean();
					NPCBrowser.FilterNPC(netID2, desired);
					ConfigurationLoader.SaveSetting();

					var packet = GetPacket();
					packet.Write((byte)CheatSheetMessageType.InformFilterNPC);
					packet.Write(netID2);
					packet.Write(desired);
					packet.Send();
					break;

				case CheatSheetMessageType.InformFilterNPC:
					int netID3 = reader.ReadInt32();
					bool desired2 = reader.ReadBoolean();
					NPCBrowser.FilterNPC(netID3, desired2);
					NPCBrowser.needsUpdate = true;
					break;
				//case CheatSheetMessageType.RequestToggleNPCSpawn:
				//	NPCSlot.HandleFilterRequest(reader.ReadInt32(), reader.ReadInt32(), true);
				//	break;
				default:
					ErrorLogger.Log("CheatSheet: Unknown Message type: " + msgType);
					break;
			}
		}

		public static Rectangle GetClippingRectangle(SpriteBatch spriteBatch, Rectangle r)
		{
			//Vector2 vector = new Vector2(this._innerDimensions.X, this._innerDimensions.Y);
			//Vector2 position = new Vector2(this._innerDimensions.Width, this._innerDimensions.Height) + vector;
			Vector2 vector = new Vector2(r.X, r.Y);
			Vector2 position = new Vector2(r.Width, r.Height) + vector;
			vector = Vector2.Transform(vector, Main.UIScaleMatrix);
			position = Vector2.Transform(position, Main.UIScaleMatrix);
			Rectangle result = new Rectangle((int)vector.X, (int)vector.Y, (int)(position.X - vector.X), (int)(position.Y - vector.Y));
			int width = spriteBatch.GraphicsDevice.Viewport.Width;
			int height = spriteBatch.GraphicsDevice.Viewport.Height;
			result.X = Utils.Clamp<int>(result.X, 0, width);
			result.Y = Utils.Clamp<int>(result.Y, 0, height);
			result.Width = Utils.Clamp<int>(result.Width, 0, width - result.X);
			result.Height = Utils.Clamp<int>(result.Height, 0, height - result.Y);
			return result;
		}
	}

	public static class CheatSheetInterface
	{
		public static void RegisterButton(Mod mod, Texture2D texture, Action buttonClickedAction, Func<string> tooltip)
		{
			if (!Main.dedServ && mod != null && mod is CheatSheet)
			{
				((CheatSheet)mod).RegisterButton(texture, buttonClickedAction, tooltip);
			}
		}

		public static void RegisterButton(Mod mod, CheatSheetButton csb)
		{
			if (!Main.dedServ && mod != null && mod is CheatSheet)
			{
				((CheatSheet)mod).RegisterButton(csb.texture, csb.buttonClickedAction, csb.tooltip);
			}
		}

		/// <summary>
		/// Returns all extra accessories for the given player
		/// </summary>
		public static IEnumerable<Item> GetExtraAccessories(Player player)
		{
			return player.GetModPlayer<CheatSheetPlayer>().ExtraAccessories;
		}

		/// <summary>
		/// Returns all extra enabled accessories for the given player
		/// </summary>
		public static IEnumerable<Item> GetEnabledExtraAccessories(Player player)
		{
			var cheatSheetPlayer = player.GetModPlayer<CheatSheetPlayer>();
			return cheatSheetPlayer.ExtraAccessories.Take(cheatSheetPlayer.numberExtraAccessoriesEnabled);
		}

		/// <summary>
		/// Returns the extra accessory item at the given index
		/// Returns null if the index is out of bounds
		/// </summary>
		public static Item GetExtraAccessory(Player player, int index)
		{
			if (index < 0 || index >= CheatSheetPlayer.MaxExtraAccessories) return null;
			return player.GetModPlayer<CheatSheetPlayer>().ExtraAccessories[index];
		}
	}

	public class CheatSheetButton
	{
		internal Texture2D texture;

		//internal Action buttonClickedAction;
		//internal Func<string> tooltip;
		public CheatSheetButton(Texture2D texture/*, Action buttonClickedAction, Func<string> tooltip*/)
		{
			this.texture = texture;
			//	this.buttonClickedAction = buttonClickedAction;
			//	this.tooltip = tooltip;
		}

		public virtual void buttonClickedAction()
		{
		}

		public virtual string tooltip()
		{
			return "";
		}
	}

	internal enum CheatSheetMessageType : byte
	{
		SpawnNPC,
		QuickClear,
		VacuumItems,
		ButcherNPCs,
		TeleportPlayer,
		SetSpawnRate,
		SpawnRateSet,
		FilterNPC,
		RequestToggleNPCSpawn,
		RequestFilterNPC,
		InformFilterNPC,
	}

	static class CheatSheetUtilities
	{
		private static Uri reporturl = new Uri("http://javid.ddns.net/tModLoader/jopojellymods/report.php");

		internal static void ReportException(Exception e)
		{
			ErrorLogger.Log("CheatSheet: " + e.Message + e.StackTrace);
			try
			{
				ReportData data = new ReportData(e);
				data.additionaldata = "Loaded Mods: " + string.Join(", ", ModLoader.GetLoadedMods());
				string jsonpayload = JsonConvert.SerializeObject(data);
				using (WebClient client = new WebClient())
				{
					var values = new NameValueCollection
					{
						{ "jsonpayload", jsonpayload },
					};
					client.UploadValuesAsync(reporturl, "POST", values);
				}
			}
			catch { }
		}

		class ReportData
		{
			public string mod;
			public string modversion;
			public string tmodversion;
			public string platform;
			public string errormessage;
			public string additionaldata;

			public ReportData()
			{
				tmodversion = ModLoader.version.ToString();
				modversion = CheatSheet.instance.Version.ToString();
				mod = "CheatSheet";
				platform = ModLoader.compressedPlatformRepresentation;
			}

			public ReportData(Exception e) : this()
			{
				errormessage = e.Message + e.StackTrace;
			}
		}
	}
}
