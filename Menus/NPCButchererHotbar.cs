using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace CheatSheet.Menus
{
	internal class NPCButchererHotbar : UIHotbar
	{
		internal static int[] DoNotButcher = { NPCID.TargetDummy, NPCID.CultistDevote, NPCID.CultistArcherBlue, NPCID.CultistTablet, NPCID.DD2LanePortal, NPCID.DD2EterniaCrystal };
		internal static string CSText(string key, string category = "Butcherer") => CheatSheet.CSText(category, key);
		public UIView buttonView;
		public UIImage bButcherHostiles;
		public UIImage bButcherBoth;
		public UIImage bButcherTownNPCs;

		internal bool mouseDown;
		internal bool justMouseDown;

		private CheatSheet mod;

		public NPCButchererHotbar(CheatSheet mod)
		{
			this.mod = mod;
			//parentHotbar = mod.hotbar;

			this.buttonView = new UIView();
			base.Visible = false;

			// Button images
			bButcherHostiles = new UIImage(Terraria.GameContent.TextureAssets.Item[ItemID.DemonHeart].Value);
			bButcherBoth = new UIImage(Terraria.GameContent.TextureAssets.Item[ItemID.CrimsonHeart].Value);
			bButcherTownNPCs = new UIImage(Terraria.GameContent.TextureAssets.Item[ItemID.Heart].Value);

			// Button tooltips
			bButcherHostiles.Tooltip = CSText("ButcherHostileNPCs");
			bButcherBoth.Tooltip = CSText("ButcherHostileAndFriendlyNPCs");
			bButcherTownNPCs.Tooltip = CSText("ButcherFriendlyNPCs");

			// Button EventHandlers
			bButcherHostiles.onLeftClick += (s, e) =>
			{
				HandleButcher(0);
			};
			bButcherBoth.onLeftClick += (s, e) =>
			{
				HandleButcher(1);
			};
			bButcherTownNPCs.onLeftClick += (s, e) =>
			{
				HandleButcher(2);
			};

			// Register mousedown
			onMouseDown += (s, e) =>
			{
				if (!Main.LocalPlayer.mouseInterface && !mod.hotbar.MouseInside && !mod.hotbar.button.MouseInside)
				{
					mouseDown = true;
					Main.LocalPlayer.mouseInterface = true;
				}
			};
			onMouseUp += (s, e) => { justMouseDown = true; mouseDown = false; /*startTileX = -1; startTileY = -1;*/ };

			// ButtonView
			buttonView.AddChild(bButcherHostiles);
			buttonView.AddChild(bButcherBoth);
			buttonView.AddChild(bButcherTownNPCs);

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
			this.Resize();
		}

		public static void HandleButcher(int butcherType = 0, bool forceHandle = false)
		{
			bool syncData = forceHandle || Main.netMode == 0;
			if (syncData)
			{
				ButcherNPCs(butcherType, forceHandle);
			}
			else
			{
				SyncButcher(butcherType);
			}
		}

		private static void SyncButcher(int butcherType = 0)
		{
			var netMessage = CheatSheet.instance.GetPacket();
			netMessage.Write((byte)CheatSheetMessageType.ButcherNPCs);
			netMessage.Write(butcherType);
			netMessage.Send();
		}

		private static void ButcherNPCs(int butcherType = 0, bool syncData = false, int indexRange = -1)
		{
			//case 28 msgType == 28
			/*
            writer.Write((short)number); // index
            writer.Write((short)number2); // damage
            writer.Write(number3); // knockback
            writer.Write((byte)(number4 + 1f)); // hit direction
            writer.Write((byte)number5); // crit ( 1==crit true, else false )

            int num86 = (int)this.reader.ReadInt16();
            int num87 = (int)this.reader.ReadInt16();
            float num88 = this.reader.ReadSingle();
            int num89 = (int)(this.reader.ReadByte() - 1);
            byte b7 = this.reader.ReadByte();
            */
			// 0 == hostiles
			// 1 == hostiles & town NPCs // friendlies
			// 2 == town NPCs // friendlies only
			for (int i = 0; i < Main.maxNPCs; i++) // Iteration
			{
				if (Main.npc[i].active && CheckNPC(i))
				{
					if (butcherType == 0 && (Main.npc[i].townNPC || Main.npc[i].friendly)) continue;
					else if (butcherType == 2 && (!Main.npc[i].townNPC || !Main.npc[i].friendly)) continue;
					//always run for the visual effects (damage drawn and sounds) for client
					Main.npc[i].StrikeNPCNoInteraction(Main.npc[i].lifeMax, 0f, -Main.npc[i].direction, true);
					if (syncData) // syncData does not do visuals
					{
						NetMessage.SendData(28, -1, -1, null, i, Main.npc[i].lifeMax, 0f, -Main.npc[i].direction, 1);
						// type, -1, -1, msg, index, damage, knockback, direction, crit
					}
				}
			}
		}

		private static bool CheckNPC(int index)
		{
			return !DoNotButcher.Contains(Main.npc[index].type);
		}

		private static bool CheckNPC(NPC npc)
		{
			return CheckNPC(npc.whoAmI);
		}

		public override void Update()
		{
			DoSlideMovement();

			base.CenterXAxisToParentCenter();
			base.Update();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, this._rasterizerState, null, Main.UIScaleMatrix);
				//	Rectangle scissorRectangle = new Rectangle((int)base.X- (int)base.Width, (int)base.Y, (int)base.Width, (int)base.Height);
				//Parent.Position.Y
				//		Main.NewText((int)Parent.Position.Y + " " + (int)shownPosition);
				//	Rectangle scissorRectangle = new Rectangle((int)(base.X - base.Width / 2), (int)(shownPosition), (int)base.Width, (int)base.Height);
				Rectangle scissorRectangle = new Rectangle((int)(base.X - base.Width / 2), (int)(shownPosition), (int)base.Width, (int)(mod.hotbar.Position.Y - shownPosition));
				/*if (scissorRectangle.X < 0)
				{
					scissorRectangle.Width += scissorRectangle.X;
					scissorRectangle.X = 0;
				}
				if (scissorRectangle.Y < 0)
				{
					scissorRectangle.Height += scissorRectangle.Y;
					scissorRectangle.Y = 0;
				}
				if ((float)scissorRectangle.X + base.Width > (float)Main.screenWidth)
				{
					scissorRectangle.Width = Main.screenWidth - scissorRectangle.X;
				}
				if ((float)scissorRectangle.Y + base.Height > (float)Main.screenHeight)
				{
					scissorRectangle.Height = Main.screenHeight - scissorRectangle.Y;
				}*/
				scissorRectangle = CheatSheet.GetClippingRectangle(spriteBatch, scissorRectangle);
				Rectangle scissorRectangle2 = spriteBatch.GraphicsDevice.ScissorRectangle;
				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

				base.Draw(spriteBatch);

				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
			}

			//	base.Draw(spriteBatch);

			if (Visible && (base.IsMouseInside() /*|| button.MouseInside*/))
			{
				Main.LocalPlayer.mouseInterface = true;
				//Main.LocalPlayer.showItemIcon = false;
			}

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
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

		protected override bool IsMouseInside()
		{
			return hidden ? false : base.IsMouseInside();
		}

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
		}

		public void Hide()
		{
			hidden = true;
			arrived = false;
		}

		public void Show()
		{
			mod.hotbar.currentHotbar = this;
			arrived = false;
			hidden = false;
			Visible = true;
		}
	}
}