using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CheatSheet.Menus
{
	internal class NPCSlot : UIView
	{
		//public Item item = new Item();

		public int npcType = -1;
		public int netID = 0;
		public NPC npc = new NPC();
		//private static NPC syncNPC;
		public string displayName = "";

		public bool isBoss;
		public bool isTown;
		public bool isFiltered;

		public int index = 0;

		public static Texture2D backgroundTexture = Main.inventoryBack9Texture;
		public static Texture2D filteredBackgroundTexture = Main.inventoryBack5Texture;

		public bool functionalSlot;
		private bool rightClicking;

		public NPCSlot(Vector2 position, int npcNum, int index)
		{
			base.Position = position;
			this.Init(npcNum, index);
		}

		public NPCSlot(int npcNum, int index)
		{
			this.Init(npcNum, index);
		}

		private void Init(int npcNum, int index)
		{
			//npcType = npcNum;
			base.Scale = 0.85f;
			this.index = index;
			isFiltered = false;
			//	npc.netDefaults(npcNum);
			npc.SetDefaults(npcNum);
			npcType = npc.type;
			netID = npc.netID;
			//displayName = Lang.GetNPCNameValue(npcType);
			displayName = Lang.GetNPCNameValue(netID);
			//syncNPC = (NPC)npc.Clone();
			//npcType = npc.type;
			//	this.isBoss = npc.boss;
			//	this.isTown = npc.townNPC;
			//	ErrorLogger.Log("sD" + npcNum + " " + npc.type + " " + npc.boss);
			base.onLeftClick += new EventHandler(this.Slot2_onLeftClick);
			base.onRightClick += (s, e) =>
			{
				if (Main.netMode == 1)
				{
					// in MP, we request an npc be filtered.
					var message = CheatSheet.instance.GetPacket();
					message.Write((byte)CheatSheetMessageType.RequestFilterNPC);
					message.Write(netID);
					message.Write(!isFiltered);
					message.Send();
				}
				else
				{
					// in SP, we filter, mark browser as dirty, then save.
					bool desired = !isFiltered;
					NPCBrowser.FilterNPC(netID, desired);
					NPCBrowser.needsUpdate = true;
					ConfigurationLoader.SaveSetting();
				}
			};
			base.onHover += new EventHandler(this.Slot2_onHover);
		}

		protected override float GetWidth()
		{
			return (float)Slot.backgroundTexture.Width * base.Scale;
		}

		protected override float GetHeight()
		{
			return (float)Slot.backgroundTexture.Height * base.Scale;
		}

		private void Slot2_onHover(object sender, EventArgs e)
		{
			UIView.HoverText = displayName + (npc.modNPC != null ? " [" + npc.modNPC.mod.Name + "]" : "") + (isFiltered ? " [DISABLED]" : "");
			NPCBrowser.hoverNpc = npc;
			//UIView.HoverItem = this.item.Clone();
			//	hovering = true;
		}

		//public override void Update()
		//{
		//	if(UIView.)
		//}

		private void Slot2_onLeftClick(object sender, EventArgs e)
		{
			if (isFiltered) return;
			HandleNPC(npcType, netID, false);
		}

		//public static void HandleFilterRequest(int netID, int whoAmI = 0, bool forceHandle = false)
		//{
		//	/*isFiltered = !isFiltered;
		//	int useValue = type;
		//	if (isFiltered)
		//	{
		//		if (!NPCBrowser.filteredNPCSlots.Contains(useValue))
		//		{
		//			NPCBrowser.filteredNPCSlots.Add(useValue);
		//		}
		//	}
		//	else
		//	{
		//		if (NPCBrowser.filteredNPCSlots.Contains(useValue))
		//		{
		//			NPCBrowser.filteredNPCSlots.Remove(useValue);
		//		}
		//	}*/
		//	// if server message or SP
		//	bool syncData = forceHandle || Main.netMode == 0;
		//	if (syncData)
		//	{
		//		HandleFilter(netID, whoAmI, forceHandle);
		//	}
		//	else
		//	{
		//		SyncFilterRequest(netID, whoAmI);
		//	}
		//}

		//private static void SyncFilterRequest(int netID, int whoAmI = 0)
		//{
		//	var netMessage = CheatSheet.instance.GetPacket();
		//	netMessage.Write((byte)CheatSheetMessageType.RequestToggleNPCSpawn);
		//	netMessage.Write(netID);
		//	netMessage.Write(whoAmI);
		//	netMessage.Send();
		//}

		//private static void HandleFilter(int netID, int whoAmI = 0, bool syncData = false)
		//{
		//	int useValue = netID;
		//	NPCSlot slot = ((CheatSheet)CheatSheet.instance).npcBrowser.npcView.allNPCSlot[whoAmI];
		//	slot.isFiltered = !slot.isFiltered;
		//	if (slot.isFiltered)
		//	{
		//		if (!NPCBrowser.filteredNPCSlots.Contains(useValue))
		//		{
		//			NPCBrowser.filteredNPCSlots.Add(useValue);
		//		}
		//	}
		//	else
		//	{
		//		if (NPCBrowser.filteredNPCSlots.Contains(useValue))
		//		{
		//			NPCBrowser.filteredNPCSlots.Remove(useValue);
		//		}
		//	}
		//	NPCBrowser.needsUpdate = true;
		//	ConfigurationLoader.SaveSetting();
		//}

		//public static void HandleFilterNPC(int whoAmI, bool forceHandle = false)
		//{
		//	bool syncData = forceHandle || Main.netMode == 0;
		//	if (syncData)
		//	{
		//		FilterNPC(whoAmI, forceHandle);
		//	}
		//	else
		//	{
		//		SyncFilterNPC(whoAmI);
		//	}
		//}

		//private static void SyncFilterNPC(int whoAmI)
		//{
		//	var netMessage = CheatSheet.instance.GetPacket();
		//	netMessage.Write((byte)CheatSheetMessageType.FilterNPC);
		//	netMessage.Write(whoAmI);
		//	netMessage.Send();
		//}

		//private static void FilterNPC(int whoAmI, bool syncData = false)
		//{
		//	//Main.NewText("deleting " + Main.npc[whoAmI].ToString());
		//	Main.npc[whoAmI].netDefaults(0);
		//	Main.npc[whoAmI].active = false;
		//	if (syncData)
		//	{
		//		NetMessage.SendData(MessageID.SyncNPC, -1, -1, "", whoAmI);
		//	}
		//}

		public static void HandleNPC(int type, int syncID = 0, bool forceHandle = false, int whoAmI = 0)
		{
			bool syncData = forceHandle || Main.netMode == 0;
			if (syncData)
			{
				SpawnNPC(type, forceHandle, syncID, whoAmI);
			}
			else
			{
				SyncNPC(type, syncID);
			}
		}

		private static void SyncNPC(int type, int syncID = 0)
		{
			var netMessage = CheatSheet.instance.GetPacket();
			netMessage.Write((byte)CheatSheetMessageType.SpawnNPC);
			netMessage.Write(type);
			netMessage.Write(syncID);
			netMessage.Send();
		}

		private static void SpawnNPC(int type, bool syncData = false, int syncID = 0, int whoAmI = 0)
		{
			Player player;
			if (!syncData)
			{
				player = Main.LocalPlayer;
			}
			else
			{
				player = Main.player[whoAmI];
			}
			int x = (int)player.Bottom.X + player.direction * 200;
			int y = (int)player.Bottom.Y;
			int index = NPC.NewNPC(x, y, type);
			if (syncID < 0)
			{
				//NPC refNPC = new NPC();
				//refNPC.netDefaults(syncID);
				Main.npc[index].SetDefaults(syncID);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Main.instance.LoadNPC(npcType);

			Texture2D useBackgroundTexture = isFiltered ? filteredBackgroundTexture : backgroundTexture;

			spriteBatch.Draw(useBackgroundTexture, base.DrawPosition, null, base.BackgroundColor, 0f, Vector2.Zero, base.Scale, SpriteEffects.None, 0f);
			Texture2D texture2D = Main.npcTexture[npcType];
			Rectangle rectangle2;
			rectangle2 = new Rectangle(0, 0, Main.npcTexture[npcType].Width, Main.npcTexture[npcType].Height / Main.npcFrameCount[npcType]);

			float num = 1f;
			float num2 = (float)Slot.backgroundTexture.Width * base.Scale * 0.6f;
			if ((float)rectangle2.Width > num2 || (float)rectangle2.Height > num2)
			{
				if (rectangle2.Width > rectangle2.Height)
				{
					num = num2 / (float)rectangle2.Width;
				}
				else
				{
					num = num2 / (float)rectangle2.Height;
				}
			}
			Vector2 drawPosition = base.DrawPosition;
			drawPosition.X += (float)Slot.backgroundTexture.Width * base.Scale / 2f - (float)rectangle2.Width * num / 2f;
			drawPosition.Y += (float)Slot.backgroundTexture.Height * base.Scale / 2f - (float)rectangle2.Height * num / 2f;

			//Color color =  new Color(1f, 1f, 1f);//this.item.GetColor(Color.White);
			//Color color = (npc.color != new Color(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue)) ? new Color(npc.color.R, npc.color.G, npc.color.B, 255f) : new Color(1f, 1f, 1f);
			//spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(rectangle2), color, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			//if (this.item.color != default(Color))
			//{
			//	spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(rectangle2), this.item.GetColor(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			//}
			spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(rectangle2), Color.White, 0, Vector2.Zero, num, SpriteEffects.None, 0f);
			if (npc.color != default(Color))
			{
				spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(rectangle2), npc.color, 0, Vector2.Zero, num, SpriteEffects.None, 0f);
			}
			base.Draw(spriteBatch);
		}
	}
}

/*
			Microsoft.Xna.Framework.Color color9 = Lighting.GetColor((int)((double)Main.npc[i].position.X + (double)Main.npc[i].width * 0.5) / 16, (int)(((double)Main.npc[i].position.Y + (double)Main.npc[i].height * 0.5) / 16.0));

	Main.spriteBatch.Draw(Main.npcTexture[type],
	new Vector2(Main.npc[i].position.X - Main.screenPosition.X + (float)(Main.npc[i].width / 2) - (float)Main.npcTexture[type].Width * Main.npc[i].scale / 2f + vector10.X * Main.npc[i].scale, Main.npc[i].position.Y - Main.screenPosition.Y + (float)Main.npc[i].height - (float)Main.npcTexture[type].Height * Main.npc[i].scale / (float)Main.npcFrameCount[type] + 4f + vector10.Y * Main.npc[i].scale + num66 + num65 + Main.npc[i].gfxOffY),
	new Microsoft.Xna.Framework.Rectangle?(frame4),
	Main.npc[i].GetAlpha(color9),
	Main.npc[i].rotation,
	vector10,
	Main.npc[i].scale,
	spriteEffects,
	0f);
									if (Main.npc[i].color != default(Microsoft.Xna.Framework.Color))
									{
										Main.spriteBatch.Draw(Main.npcTexture[type], new Vector2(Main.npc[i].position.X - Main.screenPosition.X + (float)(Main.npc[i].width / 2) - (float)Main.npcTexture[type].Width * Main.npc[i].scale / 2f + vector10.X * Main.npc[i].scale, Main.npc[i].position.Y - Main.screenPosition.Y + (float)Main.npc[i].height - (float)Main.npcTexture[type].Height * Main.npc[i].scale / (float)Main.npcFrameCount[type] + 4f + vector10.Y * Main.npc[i].scale + num66 + num65 + Main.npc[i].gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame4), Main.npc[i].GetColor(color9), Main.npc[i].rotation, vector10, Main.npc[i].scale, spriteEffects, 0f);
									}
*/
