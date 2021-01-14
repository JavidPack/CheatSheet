using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal enum ItemBrowserCategories
	{
		AllItems,
		Weapons,
		Tools,
		Armor,
		Accessories,
		Blocks,
		Ammo,
		Potions,
		Expert,
		Furniture,
		Pets,
		Mounts,

		//    Materials,
		ModItems,
	}

	internal class ItemBrowser : UISlideWindow
	{
		internal static string CSText(string key, string category = "ItemBrowser") => CheatSheet.CSText(category, key);
		private static string[] categNames = new string[]
		{
			CSText("AllItems"),
			CSText("Weapons"),
			CSText("Tools"),
			CSText("Armor"),
			CSText("Accessories"),
			CSText("Blocks"),
			CSText("Ammo"),
			CSText("Potions"),
			CSText("Expert"),
			CSText("Furniture"),
			CSText("Pets"),
			CSText("Mounts"),
      //      "Crafting Materials",
            CSText("CycleModSpecificItems"),
		};

		private static Texture2D[] categoryIcons = new Texture2D[]
		{
			Terraria.GameContent.TextureAssets.Item[ItemID.AlphabetStatueA].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.SilverBroadsword].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.SilverPickaxe].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.SilverChainmail].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.HermesBoots].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.DirtBlock].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.FlamingArrow].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.GreaterHealingPotion].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.WormScarf].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.Dresser].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.ZephyrFish].Value,
			Terraria.GameContent.TextureAssets.Item[ItemID.SlimySaddle].Value,
        //    Terraria.GameContent.TextureAssets.Item[ItemID.FallenStar].Value,
            Terraria.GameContent.TextureAssets.Item[ItemID.AlphabetStatueM].Value,
		};

		internal ItemView itemView;

		//	private static List<string> categoryNames = new List<string>();

		internal static UIImage[] bCategories;

		public static Dictionary<string, List<int>> ModToItems = new Dictionary<string, List<int>>();
		public static List<List<int>> categories = new List<List<int>>();

		private static Color buttonColor = new Color(190, 190, 190);

		private static Color buttonSelectedColor = new Color(209, 142, 13);

		private UITextbox textbox;

		private float spacing = 16f;

		public CheatSheet mod;

		public int lastModNameNumber = 0;

		public ItemBrowser(CheatSheet mod)
		{
			var a = new[] { ItemID.AlphabetStatueA, ItemID.SilverBroadsword, ItemID.SilverPickaxe, ItemID.SilverChainmail, ItemID.HermesBoots, ItemID.DirtBlock, ItemID.FlamingArrow, ItemID.GreaterHealingPotion, ItemID.WormScarf, ItemID.Dresser, ItemID.SlimySaddle, ItemID.AlphabetStatueM};
            foreach (var item in a)
            {
				Main.instance.LoadItem(item);
            }
			categoryIcons = new Texture2D[]
			{
				TextureAssets.Item[ItemID.AlphabetStatueA].Value,
				TextureAssets.Item[ItemID.SilverBroadsword].Value,
				TextureAssets.Item[ItemID.SilverPickaxe].Value,
				TextureAssets.Item[ItemID.SilverChainmail].Value,
				TextureAssets.Item[ItemID.HermesBoots].Value,
				TextureAssets.Item[ItemID.DirtBlock].Value,
				TextureAssets.Item[ItemID.FlamingArrow].Value,
				TextureAssets.Item[ItemID.GreaterHealingPotion].Value,
				TextureAssets.Item[ItemID.WormScarf].Value,
				TextureAssets.Item[ItemID.Dresser].Value,
				TextureAssets.Item[ItemID.ZephyrFish].Value,
				TextureAssets.Item[ItemID.SlimySaddle].Value,
			//  TextureAssets.Item[ItemID.FallenStar].Value,
				TextureAssets.Item[ItemID.AlphabetStatueM].Value,
			};



			categories.Clear();
			bCategories = new UIImage[categoryIcons.Length];
			this.itemView = new ItemView();
			this.mod = mod;
			this.CanMove = true;
			base.Width = this.itemView.Width + this.spacing * 2f;
			base.Height = 420f;
			this.itemView.Position = new Vector2(this.spacing, base.Height - this.spacing - this.itemView.Height);
			this.AddChild(this.itemView);
			this.ParseList2();
			Texture2D texture = mod.GetTexture("UI/closeButton").Value;
			UIImage uIImage = new UIImage(texture/*UIView.GetEmbeddedTexture("Images.closeButton.png")*/);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(base.Width - this.spacing, this.spacing);
			uIImage.onLeftClick += new EventHandler(this.bClose_onLeftClick);
			this.AddChild(uIImage);
			this.textbox = new UITextbox();
			this.textbox.Width = 100;
			this.textbox.Anchor = AnchorPosition.TopRight;
			this.textbox.Position = new Vector2(base.Width - this.spacing * 2f - uIImage.Width, this.spacing + 40);
			//	this.textbox.Position = new Vector2(base.Width - this.spacing * 2f - uIImage.Width, this.spacing * 2f + uIImage.Height);
			this.textbox.KeyPressed += new UITextbox.KeyPressedHandler(this.textbox_KeyPressed);
			this.AddChild(this.textbox);
			for (int j = 0; j < ItemBrowser.categoryIcons.Length; j++)
			{
				UIImage uIImage2 = new UIImage(ItemBrowser.categoryIcons[j]);
				Vector2 position = new Vector2(this.spacing, this.spacing);
				uIImage2.Scale = 32f / Math.Max(categoryIcons[j].Width, categoryIcons[j].Height);

				position.X += (float)(j % 12 * 40);
				position.Y += (float)(j / 12 * 40);

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
				uIImage2.onLeftClick += (s, e) => buttonClick(s, e, true);
				uIImage2.onRightClick += (s, e) => buttonClick(s, e, false);
				uIImage2.ForegroundColor = ItemBrowser.buttonColor;
				if (j == 0)
				{
					uIImage2.ForegroundColor = ItemBrowser.buttonSelectedColor;
				}
				uIImage2.Tooltip = ItemBrowser.categNames[j];
				ItemBrowser.bCategories[j] = uIImage2;
				this.AddChild(uIImage2);
			}
			itemView.selectedCategory = ItemBrowser.categories[0].ToArray();
			itemView.activeSlots = itemView.selectedCategory;
			itemView.ReorderSlots();
			return;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.cursorItemIconEnabled = false;
			}

			float x = FontAssets.MouseText.Value.MeasureString(UIView.HoverText).X;
			Vector2 vector = new Vector2((float)Main.mouseX, (float)Main.mouseY) + new Vector2(16f);
			if (vector.Y > (float)(Main.screenHeight - 30))
			{
				vector.Y = (float)(Main.screenHeight - 30);
			}
			if (vector.X > (float)Main.screenWidth - x)
			{
				vector.X = (float)(Main.screenWidth - 460);
			}
			Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, UIView.HoverText, vector.X, vector.Y, new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), Color.Black, Vector2.Zero, 1f);
		}

		public override void Update()
		{
			//if (!arrived)
			//{
			//	if (this.hidden)
			//	{
			//		this.lerpAmount -= .01f * ItemBrowser.moveSpeed;
			//		if (this.lerpAmount < 0f)
			//		{
			//			this.lerpAmount = 0f;
			//			arrived = true;
			//			this.Visible = false;
			//		}
			//		//float y = MathHelper.SmoothStep(this.hiddenPosition, this.shownPosition, this.lerpAmount);
			//		//base.Position = new Vector2(Hotbar.xPosition, y);
			//		base.Position = Vector2.SmoothStep(hiddenPosition, shownPosition, lerpAmount);
			//	}
			//	else
			//	{
			//		this.lerpAmount += .01f * ItemBrowser.moveSpeed;
			//		if (this.lerpAmount > 1f)
			//		{
			//			this.lerpAmount = 1f;
			//			arrived = true;
			//		}
			//		//float y2 = MathHelper.SmoothStep(this.hiddenPosition, this.shownPosition, this.lerpAmount);
			//		//base.Position = new Vector2(Hotbar.xPosition, y2);
			//		base.Position = Vector2.SmoothStep(hiddenPosition, shownPosition, lerpAmount);
			//	}
			//}

			//UIView.MousePrevLeftButton = UIView.MouseLeftButton;// (MasterView.previousMouseState.LeftButton == ButtonState.Pressed);
			//UIView.MouseLeftButton = Main.mouseLeft;// (MasterView.mouseState.LeftButton == ButtonState.Pressed);
			//UIView.MousePrevRightButton = UIView.MouseRightButton;//(MasterView.previousMouseState.RightButton == ButtonState.Pressed);
			//UIView.MouseRightButton = Main.mouseRight; //(MasterView.mouseState.RightButton == ButtonState.Pressed);
			//UIView.ScrollAmount = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 2;
			//UIView.HoverItem = UIView.EmptyItem;
			//UIView.HoverText = "";
			//UIView.HoverOverridden = false;

			if (!Main.playerInventory)
			{
				//base.Visible = false;
			}
			base.Update();
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			Hide();
			mod.hotbar.DisableAllWindows();
			//base.Visible = false;
		}

		private void buttonClick(object sender, EventArgs e, bool left)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;
			if (num == (int)ItemBrowserCategories.ModItems)
			{
				var mods = ItemBrowser.ModToItems.Keys.ToList();
				mods.Sort();
				if (mods.Count == 0) {
					Main.NewText("No Items have been added by mods.");
				}
				else {
					if(uIImage.ForegroundColor == ItemBrowser.buttonSelectedColor)
						lastModNameNumber = left ? (lastModNameNumber + 1) % mods.Count : (mods.Count + lastModNameNumber - 1) % mods.Count;
					string currentMod = mods[lastModNameNumber];
					this.itemView.selectedCategory = ItemBrowser.categories[0].Where(x => this.itemView.allItemsSlots[x].item.ModItem != null && this.itemView.allItemsSlots[x].item.ModItem.Mod.Name == currentMod).ToArray();
					this.itemView.activeSlots = this.itemView.selectedCategory;
					this.itemView.ReorderSlots();
					bCategories[num].Tooltip = ItemBrowser.categNames[num] + ": " + currentMod;
				}
			}
			else
			{
				this.itemView.selectedCategory = ItemBrowser.categories[num].ToArray();
				this.itemView.activeSlots = this.itemView.selectedCategory;
				this.itemView.ReorderSlots();
			}
			this.textbox.Text = "";
			UIImage[] array = ItemBrowser.bCategories;
			for (int j = 0; j < array.Length; j++)
			{
				UIImage uIImage2 = array[j];
				uIImage2.ForegroundColor = ItemBrowser.buttonColor;
			}
			uIImage.ForegroundColor = ItemBrowser.buttonSelectedColor;
		}

		private void textbox_KeyPressed(object sender, char key)
		{
			if (this.textbox.Text.Length <= 0)
			{
				this.itemView.activeSlots = this.itemView.selectedCategory;
				this.itemView.ReorderSlots();
				return;
			}
			List<int> list = new List<int>();
			int[] category = this.itemView.selectedCategory;
			for (int i = 0; i < category.Length; i++)
			{
				int num = category[i];
				Slot slot = this.itemView.allItemsSlots[num];
				if (slot.item.Name.ToLower().IndexOf(this.textbox.Text.ToLower(), StringComparison.Ordinal) != -1)
				{
					list.Add(num);
				}
			}
			if (list.Count > 0)
			{
				this.itemView.activeSlots = list.ToArray();
				this.itemView.ReorderSlots();
				return;
			}
			this.textbox.Text = this.textbox.Text.Substring(0, this.textbox.Text.Length - 1);
		}

		private void ParseList2()
		{
			//ItemBrowser.categoryNames = ItemBrowser.categNames.ToList<string>();
			for (int i = 0; i < ItemBrowser.categNames.Length; i++)
			{
				ItemBrowser.categories.Add(new List<int>());
				for (int j = 0; j < this.itemView.allItemsSlots.Length; j++)
				{
					Item item = itemView.allItemsSlots[j].item;
					//"Weapons",
					//"Tools",
					//"Armor",
					//"Accessories",
					//"Blocks",
					//"Ammo",
					//"Potions",
					//"Expert",
					//"Furniture"
					//"Pets"
					//"Mounts"
					//"Materials"
					if (i == 0)
					{
						ItemBrowser.categories[i].Add(j);
						if (j >= ItemID.Count) {
							string modName = ItemLoader.GetItem(j).Mod.Name;
							List<int> itemInMod;
							if (!ItemBrowser.ModToItems.TryGetValue(modName, out itemInMod))
								ItemBrowser.ModToItems.Add(modName, itemInMod = new List<int>());
							itemInMod.Add(j);
						}
					}
					else if (i == (int)ItemBrowserCategories.Weapons && item.damage > 0)
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Tools && (item.pick > 0 || item.axe > 0 || item.hammer > 0))
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Armor && (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1))
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Accessories && item.accessory)
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Blocks && (item.createTile != -1 || item.createWall != -1))
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Ammo && item.ammo != 0)
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Potions && (item.UseSound != null && item.UseSound.Style == 3))
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Expert && item.expert)
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Furniture && item.createTile != -1)
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Pets && (item.buffType > 0 && (Main.vanityPet[item.buffType] || Main.lightPet[item.buffType])))
					{
						ItemBrowser.categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Mounts && item.mountType != -1)
					{
						ItemBrowser.categories[i].Add(j);
					}
					//else if (i == (int)ItemBrowserCategories.Materials && (itemView.allItemsSlots[j].item.material || itemView.allItemsSlots[j].item.checkMat()))
					//{
					//    ItemBrowser.categories[i].Add(j);

					/*for (int b = 0; b < Recipe.numRecipes; b++)
                    {
                        for (int c = 0; c < Main.recipe[b].requiredItem.Length; c++)
                        {
                            if (Main.recipe[b].requiredItem[c].type == ItemID.Wire)
                            {
                                ItemBrowser.categories[i].Add(j);
                            }
                        }
                    }*/

					//&& (itemView.allItemsSlots[j].item.name.Substring(Math.Max(0, itemView.allItemsSlots[j].item.name.Length - 6)) == "Wrench")

					/*for (int b = 0; b < Recipe.numRecipes; b++)
                    {
                        bool hasItem = false;

                        for (int c = 0; c < Main.recipe[b].requiredItem.Length; c++)
                        {
                            if (Main.recipe[b].requiredItem[c].type == ItemID.Wire)
                            {
                                ItemBrowser.categories[i].Add(j);
                                hasItem = true;
                                break;
                            }
                        }
                        if (hasItem) continue;
                        //Main.recipe[b].createItem.name.Substring(Math.Max(0, Main.recipe[b].createItem.name.Length - 6)).ToLower() == "wrench")
                        if (Main.recipe[b].createItem.name.ToLower() == "wire")
                        {
                            ItemBrowser.categories[i].Add(j);
                            continue;
                        }
                    }*/
					//ItemBrowser.categories[i].Add(j);
					// }
				}
			}
			ItemBrowser.categories[(int)ItemBrowserCategories.Weapons] = ItemBrowser.categories[(int)ItemBrowserCategories.Weapons].OrderBy(x => itemView.allItemsSlots[x].item.damage).ToList();
			ItemBrowser.categories[(int)ItemBrowserCategories.Tools] = ItemBrowser.categories[(int)ItemBrowserCategories.Tools].OrderBy(x => itemView.allItemsSlots[x].item.pick).ToList();
			ItemBrowser.categories[(int)ItemBrowserCategories.Accessories] = ItemBrowser.categories[(int)ItemBrowserCategories.Accessories].OrderBy(x => itemView.allItemsSlots[x].item.rare).ToList();
			this.itemView.selectedCategory = ItemBrowser.categories[0].ToArray();
		}
	}
}