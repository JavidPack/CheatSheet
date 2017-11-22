﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CheatSheet
{
	internal class ConfigurationLoader
	{
		private static string jsonDatabaseFilenamePersonal = "CheatSheetConfig.json";
		private static string jsonDatabaseFilenameServer = "CheatSheetConfig_Server.json";
		internal static PersonalConfiguration personalConfiguration;
		internal static ServerConfiguration serverConfiguration;

		internal static void Initialized()
		{
			personalConfiguration = new PersonalConfiguration();
			// Reset?
			Directory.CreateDirectory(Main.SavePath);
			string path = string.Concat(new object[]
				{
					Main.SavePath,
					Path.DirectorySeparatorChar,
					jsonDatabaseFilenamePersonal,
				});
			if (File.Exists(path))
			{
				using (StreamReader r = new StreamReader(path))
				{
					string json = r.ReadToEnd();
					personalConfiguration = JsonConvert.DeserializeObject<PersonalConfiguration>(json);
				}
			}

			serverConfiguration = new ServerConfiguration();
			Directory.CreateDirectory(Main.SavePath);
			path = string.Concat(new object[]
				{
					Main.SavePath,
					Path.DirectorySeparatorChar,
					jsonDatabaseFilenameServer,
				});
			if (File.Exists(path))
			{
				using (StreamReader r = new StreamReader(path))
				{
					string json = r.ReadToEnd();
					serverConfiguration = JsonConvert.DeserializeObject<ServerConfiguration>(json);
				}
			}
		}

		// Saves personal settings, and if not client, saves serversettings too.
		public static void SaveSetting()
		{
			Directory.CreateDirectory(Main.SavePath);
			string path = string.Concat(new object[]
				{
					Main.SavePath,
					Path.DirectorySeparatorChar,
					jsonDatabaseFilenamePersonal,
				});
			string json = JsonConvert.SerializeObject(personalConfiguration, Formatting.Indented);
			File.WriteAllText(path, json);

			// If not MP client.
			if (Main.netMode != 1)
			{
				Directory.CreateDirectory(Main.SavePath);
				path = string.Concat(new object[]
					{
					Main.SavePath,
					Path.DirectorySeparatorChar,
					jsonDatabaseFilenameServer,
					});
				json = JsonConvert.SerializeObject(serverConfiguration, Formatting.Indented);
				File.WriteAllText(path, json);
			}
		}
	}

	internal class PersonalConfiguration
	{
		public bool ItemBrowser = true;
		public bool NPCBrowser = true;
		public bool RecipeBrowser = true;
		public bool MinionBooster = true;
		public bool PaintTools = true;
		public bool ExtraAccessorySlots = true;
		public bool Butcher = true;
		public bool Waypoints = true;
		public bool SpawnRate = true;
		public bool ModExtensions = true;
		public bool ClearMenu = true;
		public bool Vacuum = true;
		public bool LightHack = true;

		//public bool BossDowner = true;
		//public bool EventManager = true;
		[JsonIgnore]
		public bool HiddenTest = true;

		//public int[] BannedVanillaNPCIDs {
		//	get {
		//		return Menus.NPCBrowser.filteredNPCSlots.ToArray();
		//	}
		//	set {
		//		Menus.NPCBrowser.filteredNPCSlots = new List<int>(value);
		//	}
		//}
	}

	internal class ServerConfiguration
	{
		public JSONNPC[] BannedNPCs
		{
			get
			{
				return Menus.NPCBrowser.filteredNPCSlots.Select(
					type => NPCLoader.GetNPC(type) == null ? new JSONNPC(null, Lang.GetNPCNameValue(type), type) : new JSONNPC(NPCLoader.GetNPC(type).mod.Name, NPCLoader.GetNPC(type).Name, 0)
				).ToArray();
			}
			set
			{
				// Vanilla are saved as Name and type, type is used, name is just for reading convenience.
				// Modded are stored as Classname
				List<int> loaded = value.Select(
					jsonnpc => jsonnpc.id != 0 ? jsonnpc.id : (ModLoader.GetMod(jsonnpc.mod)?.NPCType(jsonnpc.name) ?? -100)
				).ToList();
				loaded.RemoveAll(x => x == -100);
				Menus.NPCBrowser.filteredNPCSlots = loaded;
				Menus.NPCBrowser.needsUpdate = true;
			}
		}
	}

	//["mod"] = NPCLoader.GetNPC(type).mod.Name,
	//["name"] = Main.npcName[type],
	//Mod mod = ModLoader.GetMod(tag.GetString("mod"));
	//			int type = mod?.NPCType(tag.GetString("name")) ?? 0;
	//			if (type > 0)
	//				NPC.killCount[type] = tag.GetInt("count");
	public class JSONNPC
	{
		public string mod;
		public string name;
		public int id;

		public JSONNPC(string mod, string name, int id)
		{
			this.mod = mod;
			this.name = name;
			this.id = id;
			if (id != 0)
			{
				this.mod = "Terraria";
			}
		}

		// We only want to serialize id when name is null, meaning it's a vanilla npc. ModNPC have a guaranteed (ModName, Name) uniqueness. Name for vanilla is just convinience for editing json manually.
		public bool ShouldSerializeid()
		{
			return mod == "Terraria";
		}
	}
}