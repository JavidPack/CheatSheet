using CheatSheet.Menus;
using CheatSheet.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;

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
		internal Hotbar hotbar;
		internal ItemBrowser itemBrowser;
		internal NPCBrowser npcBrowser;
		internal RecipeBrowserWindow recipeBrowser;
		internal ExtendedCheatMenu extendedCheatMenu;
		internal PaintToolsHotbar paintToolsHotbar;
		internal QuickTeleportHotbar quickTeleportHotbar;
		internal QuickClearHotbar quickClearHotbar;
		internal NPCButchererHotbar npcButchererHotbar;
		internal EventManagerHotbar eventManagerHotbar;

		// once 0.9.1.1 is out, this can be removed.
		public CheatSheet()
		{
			//Properties = new ModProperties()
			//{
			//	Autoload = true,
			//	AutoloadGores = true,
			//	AutoloadSounds = true
			//};
		}

		// to do: debugmode, stat

		public override void Load()
		{
			// Since we are using hooks not in older versions, and since ItemID.Count changed, we need to do this.
			if (ModLoader.version < new Version(0, 9, 2, 1))
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
		}

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

		//public override void PostDrawFullscreenMap(ref string mouseText)
		//{
		//	Main.spriteBatch.DrawString(Main.fontMouseText, "Testing Testing", new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), Color.Pink, 0.0f, new Vector2(), 0.8f, SpriteEffects.None, 0.0f);
		//}

		int lastmode = -1;
		public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
			if(Main.netMode != lastmode)
			{
				lastmode = Main.netMode;
				if(Main.netMode == 0)
				{
					SpawnRateMultiplier.HasPermission = true;
				}
				CheatSheet.instance.hotbar.ChangedConfiguration();
			}
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new MethodSequenceListItem(
					"CheatSheet: All Cheat Sheet",
					delegate
					{
						AllItemsMenu menu = (AllItemsMenu)this.GetGlobalItem("AllItemsMenu");
						menu.DrawUpdateAll(Main.spriteBatch);
						return true;
					},
					null)
				);
			}

			MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new MethodSequenceListItem(
					"CheatSheet: Extra Accessories",
					delegate
					{
						AllItemsMenu menu = (AllItemsMenu)this.GetGlobalItem("AllItemsMenu");
						menu.DrawUpdateExtraAccessories(Main.spriteBatch);
						return true;
					},
					null)
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

		KeyboardState PreviousKeyState;

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
			string message = "CheatSheet: Unknown Message type: " + msgType;

			switch (msgType)
			{
				case CheatSheetMessageType.SpawnNPC:
					int npcType = reader.ReadInt32();
					int netID = reader.ReadInt32();
					NPCSlot.HandleNPC(npcType, netID, true, whoAmI);
					message = "Spawned " + netID + " by " + Netplay.Clients[whoAmI].Name;
					NetMessage.SendData(25, -1, -1, message, 255, Color.Azure.R, Color.Azure.G, Color.Azure.B, 0);
					break;
				case CheatSheetMessageType.QuickClear:
					int clearType = reader.ReadInt32();
					switch (clearType)
					{
						case 0:
							message = "Items were cleared by ";
							break;
						case 1:
							message = "Projectiles were cleared by ";
							break;
						default:
							break;
					}
					message += Netplay.Clients[whoAmI].Name;
					QuickClearHotbar.HandleQuickClear(clearType, true, whoAmI);

					NetMessage.SendData(25, -1, -1, message, 255, Color.Azure.R, Color.Azure.G, Color.Azure.B, 0);
					break;
				case CheatSheetMessageType.VacuumItems:
					Hotbar.HandleVacuum(true, whoAmI);
					message = "Items on the ground were vacuumed by " + Netplay.Clients[whoAmI].Name;
					NetMessage.SendData(25, -1, -1, message, 255, Color.Azure.R, Color.Azure.G, Color.Azure.B, 0);
					break;
				case CheatSheetMessageType.ButcherNPCs:
					NPCButchererHotbar.HandleButcher(reader.ReadInt32(), true);
					message = "NPCs were butchered by " + Netplay.Clients[whoAmI].Name;
					NetMessage.SendData(25, -1, -1, message, 255, Color.Azure.R, Color.Azure.G, Color.Azure.B, 0);
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
					ErrorLogger.Log(message);
					break;
			}
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

	enum CheatSheetMessageType : byte
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
}
