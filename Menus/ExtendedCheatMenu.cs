using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal class ExtendedCheatMenu : UISlideWindow
	{
		internal static string CSText(string key, string category = "ExtendedCheatMenu") => CheatSheet.CSText(category, key);
		public Mod mod;
		private static UIImage[] buttons = new UIImage[CheatSheet.ButtonTexture.Count];
		private float spacing = 16f;

		public ExtendedCheatMenu(Mod mod)
		{
			buttons = new UIImage[CheatSheet.ButtonTexture.Count];
			this.mod = mod;
			this.CanMove = true;
			base.Width = spacing * 2;
			base.Height = spacing * 2;

			if (CheatSheet.ButtonTexture.Count == 0)
			{
				UILabel none = new UILabel(CSText("No Extension Cheat Mods Installed"));
				none.Scale = .3f;
				//none.OverridesMouse = false;
				//none.
				//none.MouseInside = (X => false);
				none.Position = new Vector2(spacing, spacing);
				AddChild(none);
				Height = 100;
				Width = 140;
			}

			if (CheatSheet.ButtonTexture.Count > 0)
			{
				int count = CheatSheet.ButtonTexture.Count;

				int cols = (count + 4) / 5;
				int rows = count >= 5 ? 5 : count;

				for (int j = 0; j < CheatSheet.ButtonTexture.Count; j++)
				{
					UIImage button = new UIImage(CheatSheet.ButtonTexture[j]);
					Vector2 position = new Vector2(this.spacing + 1, this.spacing + 1);
					button.Scale = 38f / Math.Max(CheatSheet.ButtonTexture[j].Width, CheatSheet.ButtonTexture[j].Height);

					position.X += (float)(j / rows * 40);
					position.Y += (float)(j % rows * 40);

					if (CheatSheet.ButtonTexture[j].Height > CheatSheet.ButtonTexture[j].Width)
					{
						position.X += (38 - CheatSheet.ButtonTexture[j].Width) / 2;
					}
					else if (CheatSheet.ButtonTexture[j].Height < CheatSheet.ButtonTexture[j].Width)
					{
						position.Y += (38 - CheatSheet.ButtonTexture[j].Height) / 2;
					}

					button.Position = position;
					button.Tag = j;
					button.onLeftClick += new EventHandler(this.button_onLeftClick);
					button.onHover += new EventHandler(this.button_onHover);
					//	button.ForegroundColor = RecipeBrowser.buttonColor;
					//	uIImage2.Tooltip = RecipeBrowser.categNames[j];
					ExtendedCheatMenu.buttons[j] = button;
					this.AddChild(button);
				}

				Width += 40 * cols;
				Height += 40 * rows;
			}

			Texture2D texture = mod.GetTexture("UI/closeButton");
			UIImage uIImage = new UIImage(texture);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(base.Width - this.spacing / 2, this.spacing / 2);
			uIImage.onLeftClick += new EventHandler(this.bClose_onLeftClick);
			this.AddChild(uIImage);
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
		}

		public override void Update()
		{
			base.Update();
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			Hide();
			//base.Visible = false;
		}

		private void button_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;

			CheatSheet.ButtonClicked[num]();
		}

		private void button_onHover(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;

			uIImage.Tooltip = CheatSheet.ButtonTooltip[num]();
		}
	}
}