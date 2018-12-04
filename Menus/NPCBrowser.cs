using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal enum NPCBrowserCategories
	{
		AllNPCs,
		Bosses,
		TownNPC,
		netID,
		FilteredNPCs,
		ModNPCs
	}

	internal class NPCBrowser : UISlideWindow
	{
		internal static string CSText(string key, string category = "MobBrowser") => CheatSheet.CSText(category, key);
		internal static NPC tooltipNpc;
		internal static NPC hoverNpc;
		internal Texture2D[] textures;

		private static string[] categNames = new string[]
		{
			CSText("AllNPCs"),
			CSText("Bosses"),
			CSText("TownNPCs"),
			CSText("NetIDNPCs"),
			CSText("FilteredNPCs"),
			CSText("CycleModSpecificNPCs")
		};

		private static Texture2D[] categoryIcons = new Texture2D[]
		{
			Main.itemTexture[ItemID.AlphabetStatueA],
			Main.itemTexture[ItemID.AlphabetStatueB],
			Main.itemTexture[ItemID.AlphabetStatueT],
			Main.itemTexture[ItemID.AlphabetStatueN],
			Main.itemTexture[ItemID.AlphabetStatueF],
			Main.itemTexture[ItemID.AlphabetStatueM],
		};

		private bool swapFilter = false;

		public NPCView npcView;
		public CheatSheet mod;

		//	private static List<string> categoryNames = new List<string>();
		internal static UIImage[] bCategories;

		public static List<List<int>> categories = new List<List<int>>();
		private static Color buttonColor = new Color(190, 190, 190);

		private static Color buttonSelectedColor = new Color(209, 142, 13);

		private UITextbox textbox;

		private float spacing = 16f;

		public int lastModNameNumber = 0;

		private float numWidth = categNames.Length - 5; // when adding more filtering buttons, decreases textbar size

		// filteredNPCSlots represents currently loaded npc.
		public static List<int> filteredNPCSlots = new List<int>();

		internal static bool needsUpdate = true;

		// 270 : 16 40 ?? 16

		public NPCBrowser(CheatSheet mod)
		{
			categories.Clear();
			this.npcView = new NPCView();
			this.mod = mod;
			this.CanMove = true;
			base.Width = this.npcView.Width + this.spacing * 2f;
			base.Height = 300f; // 272f
			this.npcView.Position = new Vector2(this.spacing, base.Height - this.npcView.Height - this.spacing * 3f);
			this.AddChild(this.npcView);
			this.ParseList2();
			Texture2D texture = mod.GetTexture("UI/closeButton");
			UIImage uIImage = new UIImage(texture);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(base.Width - this.spacing, this.spacing);
			uIImage.onLeftClick += new EventHandler(this.bClose_onLeftClick);
			this.AddChild(uIImage);
			this.textbox = new UITextbox();
			this.textbox.Anchor = AnchorPosition.BottomLeft;
			//this.textbox.Position = new Vector2(base.Width - this.spacing * 2f + uIImage.Width * numWidth * 2, this.spacing /** 2f + uIImage.Height*/);
			this.textbox.Position = new Vector2(this.spacing, base.Height - this.spacing);
			this.textbox.KeyPressed += new UITextbox.KeyPressedHandler(this.textbox_KeyPressed);
			this.AddChild(this.textbox);
			bCategories = new UIImage[categoryIcons.Length];
			for (int j = 0; j < NPCBrowser.categoryIcons.Length; j++)
			{
				UIImage uIImage2 = new UIImage(NPCBrowser.categoryIcons[j]);
				Vector2 position = new Vector2(this.spacing, this.spacing);
				uIImage2.Scale = 32f / Math.Max(categoryIcons[j].Width, categoryIcons[j].Height);

				position.X += (float)(j % 6 * 40);
				position.Y += (float)(j / 6 * 40);

				if (categoryIcons[j].Height > categoryIcons[j].Width)
				{
					position.X += (32 - categoryIcons[j].Width) / 2;
				}
				else if (categoryIcons[j].Height < categoryIcons[j].Width)
				{
					position.Y += (32 - categoryIcons[j].Height) / 2;
				}

				uIImage2.Position = position;
				uIImage2.Tag = j;
				uIImage2.onLeftClick += new EventHandler(this.button_onLeftClick);
				uIImage2.ForegroundColor = NPCBrowser.buttonColor;
				if (j == 0)
				{
					uIImage2.ForegroundColor = NPCBrowser.buttonSelectedColor;
				}
				uIImage2.Tooltip = NPCBrowser.categNames[j];
				NPCBrowser.bCategories[j] = uIImage2;
				this.AddChild(uIImage2);
			}
			npcView.selectedCategory = NPCBrowser.categories[0].ToArray();
			npcView.activeSlots = npcView.selectedCategory;
			npcView.ReorderSlots();
			textures = new Texture2D[]
			{
				mod.GetTexture("UI/NPCLifeIcon"),
				mod.GetTexture("UI/NPCDamageIcon"),
				mod.GetTexture("UI/NPCDefenseIcon"),
				mod.GetTexture("UI/NPCKnockbackIcon"),
			};
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			if (Visible && IsMouseInside())
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

			if (hoverNpc != null)
			{
				if (tooltipNpc == null || tooltipNpc.netID != hoverNpc.netID)
				{
					tooltipNpc = new NPC();
					tooltipNpc.SetDefaults(hoverNpc.netID);
				}

				string[] texts = { $"{tooltipNpc.lifeMax}", $"{tooltipNpc.defDamage}", $"{tooltipNpc.defDefense}", $"{tooltipNpc.knockBackResist:0.##}" };
				Vector2 pos = new Vector2(vector.X, vector.Y + 24);
				for (int i = 0; i < textures.Length; i++)
				{
					spriteBatch.Draw(textures[i], pos, Color.White);
					pos.X += textures[i].Width + 4;
					Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, texts[i], pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
					pos.X += Main.fontMouseText.MeasureString(texts[i]).X + 8;
				}
			}
		}

		public override void Update()
		{
			if (needsUpdate)
			{
				foreach (var npcslot in npcView.allNPCSlot)
				{
					npcslot.isFiltered = filteredNPCSlots.Contains(npcslot.netID);
				}
				needsUpdate = false;
			}
			var mouseState = Mouse.GetState();
			UIView.MousePrevLeftButton = UIView.MouseLeftButton;
			UIView.MouseLeftButton = mouseState.LeftButton==ButtonState.Pressed;
			UIView.MousePrevRightButton = UIView.MouseRightButton;
			UIView.MouseRightButton = mouseState.RightButton==ButtonState.Pressed;
			UIView.ScrollAmount = PlayerInput.ScrollWheelDeltaForUI;
			//UIView.ScrollAmount = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 2;
			//UIView.HoverItem = UIView.EmptyItem;
			UIView.HoverText = "";
			UIView.HoverOverridden = false;
			hoverNpc = null;

			base.Update();
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			Hide();
			mod.hotbar.DisableAllWindows();
			//base.Visible = false;
		}

		private void button_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;
			if (num == (int)NPCBrowserCategories.ModNPCs)
			{
				string[] mods = ModLoader.GetLoadedMods();
				string currentMod = mods[lastModNameNumber];
				lastModNameNumber = (lastModNameNumber + 1) % mods.Length;
				if (currentMod == "ModLoader")
				{
					currentMod = mods[lastModNameNumber];
					lastModNameNumber = (lastModNameNumber + 1) % mods.Length;
				}
				this.npcView.selectedCategory = NPCBrowser.categories[0].Where(x => npcView.allNPCSlot[x].npcType >= NPCID.Count && NPCLoader.GetNPC(npcView.allNPCSlot[x].npcType).mod.Name == currentMod).ToArray();
				this.npcView.activeSlots = this.npcView.selectedCategory;
				this.npcView.ReorderSlots();
				bCategories[num].Tooltip = NPCBrowser.categNames[num] + ": " + currentMod;
			}
			else if (num == (int)NPCBrowserCategories.FilteredNPCs)
			{
				swapFilter = !swapFilter;
				if (swapFilter)
				{
					this.npcView.selectedCategory = NPCBrowser.categories[0].Where(x => npcView.allNPCSlot[x].isFiltered).ToArray();
				}
				else
				{
					this.npcView.selectedCategory = NPCBrowser.categories[0].Where(x => !npcView.allNPCSlot[x].isFiltered).ToArray();
				}
				this.npcView.activeSlots = this.npcView.selectedCategory;
				this.npcView.ReorderSlots();
				bCategories[num].Tooltip = NPCBrowser.categNames[num] + " [" + (swapFilter ? "DISABLED" : "ENABLED") + " NPCs]";
			}
			else
			{
				this.npcView.selectedCategory = NPCBrowser.categories[num].ToArray();
				this.npcView.activeSlots = this.npcView.selectedCategory;
				this.npcView.ReorderSlots();
			}
			this.textbox.Text = "";
			UIImage[] array = NPCBrowser.bCategories;
			for (int j = 0; j < array.Length; j++)
			{
				UIImage uIImage2 = array[j];
				uIImage2.ForegroundColor = NPCBrowser.buttonColor;
			}
			uIImage.ForegroundColor = NPCBrowser.buttonSelectedColor;
		}

		private void textbox_KeyPressed(object sender, char key)
		{
			if (this.textbox.Text.Length <= 0)
			{
				this.npcView.activeSlots = this.npcView.selectedCategory;
				this.npcView.ReorderSlots();
				return;
			}
			List<int> list = new List<int>();
			int[] category = this.npcView.selectedCategory;
			for (int i = 0; i < category.Length; i++)
			{
				int num = category[i];
				NPCSlot slot = this.npcView.allNPCSlot[num];
				if (slot.displayName.ToLower().IndexOf(this.textbox.Text.ToLower(), StringComparison.Ordinal) != -1)
				{
					list.Add(num);
				}
			}
			if (list.Count > 0)
			{
				this.npcView.activeSlots = list.ToArray();
				this.npcView.ReorderSlots();
				return;
			}
			this.textbox.Text = this.textbox.Text.Substring(0, this.textbox.Text.Length - 1);
		}

		private void ParseList2()
		{
			//	NPCBrowser.categoryNames = NPCBrowser.categNames.ToList<string>();
			for (int i = 0; i < NPCBrowser.categNames.Length; i++)
			{
				NPCBrowser.categories.Add(new List<int>());
				for (int j = 0; j < this.npcView.allNPCSlot.Length; j++)
				{
					if (i == 0)
					{
						NPCBrowser.categories[i].Add(j);
					}
					else if (i == 1 && npcView.allNPCSlot[j].npc.boss)
					{
						NPCBrowser.categories[i].Add(j);
					}
					else if (i == 2 && npcView.allNPCSlot[j].npc.townNPC)
					{
						NPCBrowser.categories[i].Add(j);
					}
					else if (i == 3 && npcView.allNPCSlot[j].npc.netID < 0)
					{
						NPCBrowser.categories[i].Add(j);
					}
				}
			}
			this.npcView.selectedCategory = NPCBrowser.categories[0].ToArray();
		}

		// Server and Client capable.
		internal static void FilterNPC(int netID, bool desired)
		{
			if (desired)
			{
				if (!filteredNPCSlots.Contains(netID))
				{
					filteredNPCSlots.Add(netID);
				}
			}
			else
			{
				if (filteredNPCSlots.Contains(netID))
				{
					filteredNPCSlots.Remove(netID);
				}
			}
		}
	}
}