using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

//TODO: projectiles, buffs/debuffs MP compatible

namespace CheatSheet.Menus
{
	internal class QuickClearHotbar : UIHotbar
	{
		internal static string CSText(string key, string category = "QuickClear") => CheatSheet.CSText(category, key);
		public UIView buttonView;
		public UIImage bItems;
		public UIImage bProjectiles;
		public UIImage bBuffs;
		public UIImage bDebuffs;

		internal bool mouseDown;
		internal bool justMouseDown;

		private CheatSheet mod;

		public QuickClearHotbar(CheatSheet mod)
		{
			this.mod = mod;
			//parentHotbar = mod.hotbar;

			this.buttonView = new UIView();
			base.Visible = false;

			// Button images
			bItems = new UIImage(Main.itemTexture[ItemID.WoodenSword]);
			bProjectiles = new UIImage(Main.itemTexture[ItemID.WoodenArrow]);
			bBuffs = new UIImage(Main.buffTexture[BuffID.Honey]);
			bDebuffs = new UIImage(Main.buffTexture[BuffID.Poisoned]);

			// Button tooltips
			bItems.Tooltip = CSText("Clear dropped items");
			bProjectiles.Tooltip = CSText("Clear projectiles");
			bBuffs.Tooltip = CSText("Clear buffs");
			bDebuffs.Tooltip = CSText("Clear debuffs");

			// Button EventHandlers
			bItems.onLeftClick += (s, e) =>
			{
				HandleQuickClear();
			};
			bProjectiles.onLeftClick += (s, e) =>
			{
				HandleQuickClear(1);
			};
			bBuffs.onLeftClick += (s, e) =>
			{
				HandleQuickClear(2);
			};
			bDebuffs.onLeftClick += (s, e) =>
			{
				HandleQuickClear(3);
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
			buttonView.AddChild(bItems);
			buttonView.AddChild(bProjectiles);
			buttonView.AddChild(bBuffs);
			buttonView.AddChild(bDebuffs);

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

		public static void HandleQuickClear(int clearType = 0, bool forceHandle = false, int whoAmI = 0)
		{
			bool syncData = forceHandle || Main.netMode == 0;
			if (syncData)
			{
				ClearObjects(clearType, forceHandle, whoAmI);
			}
			else
			{
				SyncQuickClear(clearType);
			}
		}

		private static void SyncQuickClear(int clearType = 0)
		{
			var netMessage = CheatSheet.instance.GetPacket();
			netMessage.Write((byte)CheatSheetMessageType.QuickClear);
			netMessage.Write(clearType);
			netMessage.Send();
		}

		private static void ClearObjects(int clearType = 0, bool syncData = false, int whoAmI = 0)
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

			switch (clearType)
			{
				case 0:
					HandleClearItems(syncData);
					break;

				case 1:
					HandleClearProjectiles(syncData);
					break;

				case 2:
					HandleClearBuffs(player);
					break;

				case 3:
					HandleClearBuffs(player, true);
					break;

				default:
					break;
			}
		}

		private static void HandleClearItems(bool syncData = false)
		{
			for (int i = 0; i < Main.item.Length; i++)
			{
				if (!syncData)
				{
					Main.item[i].active = false;
				}
				else
				{
					Main.item[i].SetDefaults(0);
					NetMessage.SendData(21, -1, -1, null, i, 0f, 0f, 0f, 0);
				}
			}
		}

		public static void HandleClearProjectiles(bool syncData = false)
		{
			for (int i = 0; i < Main.projectile.Length; i++)
			{
				if (Main.projectile[i].active)
				{
					Main.projectile[i].Kill();
					//Main.projectile[i].SetDefaults(0);
					if (syncData)
					{
						NetMessage.SendData(27, -1, -1, null, i, 0f, 0f, 0f, 0);
					}
				}
			}
		}

		public static void HandleClearBuffs(Player player, bool debuffsOnly = false)
		{
			// buffs are only syncing when added for PvP
			for (int b = 0; b < 22; b++)
			{
				if (debuffsOnly && (!Main.debuff[player.buffType[b]])) continue;

				if (player.buffType[b] > 0)
				{
					player.buffTime[b] = 0;
					player.buffType[b] = 0;
					if (debuffsOnly)
					{
						for (int i = 0; i < 21; i++)
						{
							if (player.buffTime[i] == 0 || player.buffType[i] == 0)
							{
								for (int j = i + 1; j < 22; j++)
								{
									player.buffTime[j - 1] = player.buffTime[j];
									player.buffType[j - 1] = player.buffType[j];
									player.buffTime[j] = 0;
									player.buffType[j] = 0;
								}
							}
						}
					}
				}
			}
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