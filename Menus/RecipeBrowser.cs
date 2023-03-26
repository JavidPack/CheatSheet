using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal enum RecipeBrowserCategories
	{
		AllRecipes,
		ModRecipes
	}

	internal class RecipeBrowserWindow : UISlideWindow
	{
		internal static string CSText(string key, string category = "RecipeBrowser") => CheatSheet.CSText(category, key);
		private static string[] categNames;

		private static Asset<Texture2D>[] categoryIcons;

		internal static RecipeView recipeView;
		public CheatSheet mod;

		//private static List<string> categoryNames = new List<string>();
		internal static UIImage[] bCategories;

		//private static GenericItemSlot[] lookupItem = new GenericItemSlot[1];
		internal static RecipeQuerySlot lookupItemSlot;

		internal static GenericItemSlot[] ingredients;
		//internal static GenericItemSlot[] tiles = new GenericItemSlot[maxRequirementsOld];

		public static List<List<int>> categories = new List<List<int>>();
		private static Color buttonColor = new Color(190, 190, 190);

		private static Color buttonSelectedColor = new Color(209, 142, 13);

		private UITextbox textbox;

		private float spacing = 16f;
		private float halfspacing = 8f;

		public int lastModNameNumber = 0;

		public Recipe selectedRecipe = null;
		internal bool selectedRecipeChanged = false;

		public const int maxRequirementsOld = 15;

		public static void LoadStatic()
		{
			categNames = new string[]
			{
				CSText("AllRecipes"),
				CSText("CycleModSpecificRecipes")
			};
		}

		public static void UnloadStatic()
		{
			categNames = null;
			categories.Clear();
			recipeView = null;
			categoryIcons = null;
			bCategories = null;
			lookupItemSlot = null;
			ingredients = null;
		}

		// 270 : 16 40 ?? 16

		public RecipeBrowserWindow(CheatSheet mod)
		{
			categories.Clear();
			categoryIcons = new Asset<Texture2D>[]
			{
				ModUtils.GetItemTexture(ItemID.AlphabetStatueA),
				ModUtils.GetItemTexture(ItemID.AlphabetStatueM),
			};
			bCategories = new UIImage[categoryIcons.Length];
			recipeView = new RecipeView();
			this.mod = mod;
			this.CanMove = true;
			base.Width = recipeView.Width + this.spacing * 2f;
			base.Height = 420f;
			recipeView.Position = new Vector2(this.spacing, this.spacing + 40);
			this.AddChild(recipeView);
			this.InitializeRecipeCategories();
			Asset<Texture2D> texture = mod.Assets.Request<Texture2D>("UI/closeButton", ReLogic.Content.AssetRequestMode.ImmediateLoad);
			UIImage uIImage = new UIImage(texture);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(base.Width - this.spacing, this.spacing);
			uIImage.onLeftClick += new EventHandler(this.bClose_onLeftClick);
			this.AddChild(uIImage);
			this.textbox = new UITextbox();
			this.textbox.Anchor = AnchorPosition.TopRight;
			this.textbox.Position = new Vector2(base.Width - this.spacing * 2f - uIImage.Width, this.spacing /** 2f + uIImage.Height*/);
			this.textbox.KeyPressed += new UITextbox.KeyPressedHandler(this.textbox_KeyPressed);
			this.AddChild(this.textbox);

			//lookupItemSlot = new Slot(0);
			lookupItemSlot = new RecipeQuerySlot();
			lookupItemSlot.Position = new Vector2(spacing, halfspacing);
			lookupItemSlot.Scale = .85f;
			//lookupItemSlot.functionalSlot = true;
			this.AddChild(lookupItemSlot);

			for (int j = 0; j < RecipeBrowserWindow.categoryIcons.Length; j++)
			{
				UIImage uIImage2 = new UIImage(RecipeBrowserWindow.categoryIcons[j]);
				Vector2 position = new Vector2(this.spacing + 48, this.spacing);
                Asset<Texture2D> iconAsset = categoryIcons[j];
                uIImage2.Scale = 32f / Math.Max(iconAsset.Width(), iconAsset.Height());

				position.X += (float)(j % 6 * 40);
				position.Y += (float)(j / 6 * 40);

				if (iconAsset.Height() > iconAsset.Width())
				{
					position.X += (32 - iconAsset.Width()) / 2;
				}
				else if (iconAsset.Height() < iconAsset.Width())
				{
					position.Y += (32 - iconAsset.Height()) / 2;
				}

				uIImage2.Position = position;
				uIImage2.Tag = j;
				uIImage2.onLeftClick += (s, e) => buttonClick(s, e, true);
				uIImage2.onRightClick += (s, e) => buttonClick(s, e, false);
				uIImage2.ForegroundColor = RecipeBrowserWindow.buttonColor;
				if (j == 0)
				{
					uIImage2.ForegroundColor = RecipeBrowserWindow.buttonSelectedColor;
				}
				uIImage2.Tooltip = RecipeBrowserWindow.categNames[j];
				RecipeBrowserWindow.bCategories[j] = uIImage2;
				this.AddChild(uIImage2);
			}

			//TODO dynamic UI based on the recipe length
			ingredients = new GenericItemSlot[maxRequirementsOld];
			for (int j = 0; j < maxRequirementsOld; j++)
			{
				GenericItemSlot genericItemSlot = new GenericItemSlot();
				Vector2 position = new Vector2(this.spacing, this.spacing);

				//position.X += j * 60 + 120;
				//position.Y += 250;

				position.X += 166 + (j % cols * 51);
				position.Y += 244 + (j / cols * 51);

				genericItemSlot.Position = position;
				genericItemSlot.Tag = j;
				RecipeBrowserWindow.ingredients[j] = genericItemSlot;
				this.AddChild(genericItemSlot, false);
			}

			recipeView.selectedCategory = RecipeBrowserWindow.categories[0].ToArray();
			recipeView.activeSlots = recipeView.selectedCategory;
			recipeView.ReorderSlots();
			return;
		}

		private const int cols = 5;

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.cursorItemIconEnabled = false;
			}

			if (Visible && Recipe.numRecipes > recipeView.allRecipeSlot.Length)
			{
				//			ErrorLogger.Log("New " + Recipe.numRecipes + " " + recipeView.allRecipeSlot.Length);

				recipeView.allRecipeSlot = new RecipeSlot[Recipe.numRecipes];
				for (int i = 0; i < recipeView.allRecipeSlot.Length; i++)
				{
					recipeView.allRecipeSlot[i] = new RecipeSlot(i);
				}

				this.InitializeRecipeCategories();

				recipeView.selectedCategory = RecipeBrowserWindow.categories[0].ToArray();
				recipeView.activeSlots = recipeView.selectedCategory;
				recipeView.ReorderSlots();
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

			float positionX = this.X + spacing;
			float positionY = this.Y + 270;//320;
			string text4;
			if (selectedRecipe != null && Visible)
			{
				Color color3 = new Color((int)((byte)((float)Main.mouseTextColor)), (int)((byte)((float)Main.mouseTextColor)), (int)((byte)((float)Main.mouseTextColor)), (int)((byte)((float)Main.mouseTextColor)));

				text4 = Lang.inter[21] + " " + Main.guideItem.Name;
				spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[22].Value, new Vector2((float)positionX, (float)(positionY)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				//	int num60 = Main.focusRecipe;
				int num61 = 0;
				int num62 = 0;

				//1.4 Fix cause idk how this code works exactly
				int[] requiredTile = selectedRecipe.requiredTile.ToArray();
				Array.Resize(ref requiredTile, maxRequirementsOld);
				for (int i = Math.Max(0, selectedRecipe.requiredTile.Count - 1); i < requiredTile.Length; i++)
				{
					if (requiredTile[i] == 0)
					{
						requiredTile[i] = -1;
					}
				}

				while (num62 < maxRequirementsOld)
				{
					int num63 = (num62 + 1) * 26;
					if (requiredTile[num62] == -1)
					{
						//if (num62 == 0 && !selectedRecipe.needWater && !selectedRecipe.needHoney && !selectedRecipe.needLava)
						if (num62 == 0 && selectedRecipe.Conditions.Count == 0)
						{
							spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[23].Value, new Vector2((float)positionX, (float)(positionY + num63)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
							break;
						}
						break;
					}
					else
					{
						num61++;
						spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.GetMapObjectName(MapHelper.TileToLookup(requiredTile[num62], 0)), new Vector2((float)positionX, (float)(positionY + num63)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
						num62++;
					}
				}
				foreach (var condition in selectedRecipe.Conditions)
				{
					int y = (num61 + 1) * 26;
					spriteBatch.DrawString(FontAssets.MouseText.Value, condition.Description.Value, new Vector2((float)positionX, (float)(positionY + y)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				}
				/*
				if (selectedRecipe.needWater)
				{
					int num64 = (num61 + 1) * 26;
					spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[53].Value, new Vector2((float)positionX, (float)(positionY + num64)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				}
				if (selectedRecipe.needHoney)
				{
					int num65 = (num61 + 1) * 26;
					spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[58].Value, new Vector2((float)positionX, (float)(positionY + num65)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				}
				if (selectedRecipe.needLava)
				{
					int num66 = (num61 + 1) * 26;
					spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[56].Value, new Vector2((float)positionX, (float)(positionY + num66)), color3, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				}
				*/
			}
			//else
			//{
			//	text4 = Lang.inter[24];
			//}
			//spriteBatch.DrawString(FontAssets.MouseText.Value, text4, new Vector2((float)(positionX + 50), (float)(positionY + 12)), new Microsoft.Xna.Framework.Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
		}

		public override void Update()
		{
			//	UIView.MousePrevLeftButton = UIView.MouseLeftButton;
			//	UIView.MouseLeftButton = Main.mouseLeft;
			//	UIView.MousePrevRightButton = UIView.MouseRightButton;
			//	UIView.MouseRightButton = Main.mouseRight;
			//	UIView.ScrollAmount = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 2;
			//	UIView.HoverItem = UIView.EmptyItem;
			//	UIView.HoverText = "";
			//	UIView.HoverOverridden = false;

			if (selectedRecipeChanged)
			{
				//ErrorLogger.Log("1");
				//foreach(var a in CheatSheet.ButtonClicked)
				//{
				//	Main.NewText(">");
				//	ErrorLogger.Log("button pressing");

				//	a(selectedRecipe.requiredItem[0].type);
				//	Main.NewText("<");
				//}

				selectedRecipeChanged = false;
				string oldname = Main.HoverItem.Name;
				for (int i = 0; i < ingredients.Length; i++)
				{
					if (i < selectedRecipe.requiredItem.Count && selectedRecipe.requiredItem[i].type > 0)
					{
						ingredients[i].item = selectedRecipe.requiredItem[i];

						string name;
						if (selectedRecipe.ProcessGroupsForText(selectedRecipe.requiredItem[i].type, out name))
						{
							Main.HoverItem.SetNameOverride(name);
						}
						/*
						if (selectedRecipe.anyIronBar && selectedRecipe.requiredItem[i].type == 22)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.GetItemNameValue(22));
						}
						else if (selectedRecipe.anyWood && selectedRecipe.requiredItem[i].type == 9)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.GetItemNameValue(9));
						}
						else if (selectedRecipe.anySand && selectedRecipe.requiredItem[i].type == 169)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.GetItemNameValue(169));
						}
						else if (selectedRecipe.anyFragment && selectedRecipe.requiredItem[i].type == 3458)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.misc[51]);
						}
						else if (selectedRecipe.anyPressurePlate && selectedRecipe.requiredItem[i].type == 542)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.misc[38]);
						}
						*/
						//else
						//{
						//	ModRecipe recipe = selectedRecipe as ModRecipe;
						//	if (recipe != null)
						//	{
						//		recipe.CraftGroupDisplayName(i);
						//	}
						//}

						if (Main.HoverItem.Name != oldname)
						{
							ingredients[i].item.SetNameOverride(Main.HoverItem.Name);
							Main.HoverItem.SetNameOverride(oldname);
						}
					}
					else
					{
						ingredients[i].item = null;
					}

					//				if (selectedRecipe.requiredTile[i] > -1)
					//				{
					//					tiles[i].item = selectedRecipe.requiredItem[i]
					//;
					//				}
					//				else
					//				{
					//					ingredients[i].item = null;
					//				}
					//				this.requiredItem[i] = new Item();
					//				this.requiredTile[i] = -1;
				}
			}

			base.Update();
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			if (lookupItemSlot.real && lookupItemSlot.item.stack > 0)
			{
				//Main.LocalPlayer.QuickSpawnItem(lookupItemSlot.item.type, lookupItemSlot.item.stack);
				//lookupItemSlot.item.SetDefaults(0);

				Player player = Main.LocalPlayer;
				lookupItemSlot.item.position = player.Center;
				Item item = player.GetItem(player.whoAmI, lookupItemSlot.item, GetItemSettings.GetItemInDropItemCheck);
				if (item.stack > 0)
				{
					int num = Item.NewItem(player.GetSource_Misc("PlayerDropItemCheck"), (int)player.position.X, (int)player.position.Y, player.width, player.height, item.type, item.stack, false, (int)lookupItemSlot.item.prefix, true, false);
					Main.item[num].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, null, num, 1f, 0f, 0f, 0, 0, 0);
					}
				}
				lookupItemSlot.item = new Item();

				recipeView.ReorderSlots();
			}

			Hide();
			mod.hotbar.DisableAllWindows();
			//base.Visible = false;
		}

		private void buttonClick(object sender, EventArgs e, bool left)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;
			if (num == (int)RecipeBrowserCategories.ModRecipes)
			{
				var mods = ModLoader.Mods.Select(x => x.Name).ToList();
				mods = mods.Intersect(RecipeBrowserWindow.categories[0].Select(x => (recipeView.allRecipeSlot[x].recipe /*as ModRecipe*/)?.Mod?.Name ?? null)).ToList();
				mods.Sort();
				if (mods.Count == 0) {
					Main.NewText("No Recipes have been added by mods.");
					return;
				}
				if (uIImage.ForegroundColor == RecipeBrowserWindow.buttonSelectedColor)
					lastModNameNumber = left ? (lastModNameNumber + 1) % mods.Count : (mods.Count + lastModNameNumber - 1) % mods.Count;
				string currentMod = mods[lastModNameNumber];
				recipeView.selectedCategory = RecipeBrowserWindow.categories[0].Where(x => recipeView.allRecipeSlot[x].recipe.Mod /*as ModRecipe*/ != null && (recipeView.allRecipeSlot[x].recipe/* as ModRecipe*/).Mod.Name == currentMod).ToArray();
				recipeView.activeSlots = recipeView.selectedCategory;
				recipeView.ReorderSlots();
				bCategories[num].Tooltip = RecipeBrowserWindow.categNames[num] + ": " + currentMod;
			}
			else
			{
				recipeView.selectedCategory = RecipeBrowserWindow.categories[num].ToArray();
				recipeView.activeSlots = recipeView.selectedCategory;
				recipeView.ReorderSlots();
			}
			this.textbox.Text = "";
			UIImage[] array = RecipeBrowserWindow.bCategories;
			for (int j = 0; j < array.Length; j++)
			{
				UIImage uIImage2 = array[j];
				uIImage2.ForegroundColor = RecipeBrowserWindow.buttonColor;
			}
			uIImage.ForegroundColor = RecipeBrowserWindow.buttonSelectedColor;
		}

		private void textbox_KeyPressed(object sender, char key)
		{
			if (this.textbox.Text.Length <= 0)
			{
				recipeView.activeSlots = recipeView.selectedCategory;
				recipeView.ReorderSlots();
				return;
			}
			List<int> list = new List<int>();
			int[] category = recipeView.selectedCategory;
			for (int i = 0; i < category.Length; i++)
			{
				int num = category[i];
				RecipeSlot slot = recipeView.allRecipeSlot[num];
				if (slot.recipe.createItem.Name.ToLower().IndexOf(this.textbox.Text.ToLower(), StringComparison.Ordinal) != -1)
				{
					list.Add(num);
				}
				//else
				//{
				//	for (int j = 0; j < slot.recipe.requiredItem.Length; i++)
				//	{
				//		if (slot.recipe.requiredItem[j].type > 0 && slot.recipe.requiredItem[j].name.ToLower().IndexOf(this.textbox.Text.ToLower(), StringComparison.Ordinal) != -1)
				//		{
				//			list.Add(num);
				//			break;
				//		}
				//	}
				//}
			}
			if (list.Count > 0)
			{
				recipeView.activeSlots = list.ToArray();
				recipeView.ReorderSlots();
				return;
			}
			this.textbox.Text = this.textbox.Text.Substring(0, this.textbox.Text.Length - 1);
		}

		private void InitializeRecipeCategories()
		{
			//	RecipeBrowser.categoryNames = RecipeBrowser.categNames.ToList<string>();
			for (int i = 0; i < RecipeBrowserWindow.categNames.Length; i++)
			{
				RecipeBrowserWindow.categories.Add(new List<int>());
				for (int j = 0; j < recipeView.allRecipeSlot.Length; j++)
				{
					if (i == 0)
					{
						RecipeBrowserWindow.categories[i].Add(j);
					}
					//else if (i == 1 && recipeView.allNPCSlot[j].npc.boss)
					//{
					//	RecipeBrowser.categories[i].Add(j);
					//}
					//else if (i == 2 && recipeView.allNPCSlot[j].npc.townNPC)
					//{
					//	RecipeBrowser.categories[i].Add(j);
					//}
				}
			}
			recipeView.selectedCategory = RecipeBrowserWindow.categories[0].ToArray();
		}
	}
}