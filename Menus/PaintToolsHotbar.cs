using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ObjectData;

namespace CheatSheet.Menus
{
	internal class PaintToolsHotbar : UIHotbar
	{
		//public static float xPosition = 78f;
		//	public UIImage bIncreaseBrushSize;
		//	public UIImage bDecreaseBrushSize;
		internal static string CSText(string key, string category = "PaintTools") => CheatSheet.CSText(category, key);
		public UIView buttonView;
		public UIImage bStampTiles;
		public UIImage bEyeDropper;
		public UIImage bUndo;
		public UIImage bFlipHorizontal;
		public UIImage bFlipVertical;
		public UIImage bToggleTransparentSelection;

		private CheatSheet mod;

		// Idea: shift alt select to add subtract from selection.

		//	public int brushSize = 1;
		//	public int[,] BrushTileType = new int[10, 10];
		public TileData[,] StampTiles = new TileData[0, 0];
		public Stack<Tuple<Point, TileData[,]>> UndoHistory = new Stack<Tuple<Point, TileData[,]>>();
		public StampInfo stampInfo;

		internal bool StampToolActive;
		internal bool EyeDropperActive;
		internal bool TransparentSelectionEnabled = false;
		internal bool leftMouseDown;
		internal bool justLeftMouseDown;
		internal int startTileX = -1;
		internal int startTileY = -1;
		internal int lastMouseTileX = -1;
		internal int lastMouseTileY = -1;

		static MethodInfo GetTileDrawTextureMethodInfo;

		public PaintToolsHotbar(CheatSheet mod)
		{
			this.mod = mod;
			//parentHotbar = mod.hotbar;

			this.buttonView = new UIView();
			base.Visible = false;
			//base.UpdateWhenOutOfBounds = true;

			//		bDecreaseBrushSize = new UIImage(Terraria.GameContent.TextureAssets.Item[ItemID.CopperShortsword]);

			//		bIncreaseBrushSize = new UIImage(Terraria.GameContent.TextureAssets.Item[ItemID.CrossNecklace]);

			bStampTiles = new UIImage(ModUtils.GetItemTexture(ItemID.Paintbrush));

			bUndo = new UIImage(ModUtils.GetItemTexture(ItemID.AlphabetStatueU));

			bEyeDropper = new UIImage(ModUtils.GetItemTexture(ItemID.EmptyDropper));

			bFlipHorizontal = new UIImage(mod.Assets.Request<Texture2D>("CustomUI/Horizontal", ReLogic.Content.AssetRequestMode.ImmediateLoad));

			bFlipVertical = new UIImage(mod.Assets.Request<Texture2D>("CustomUI/Vertical", ReLogic.Content.AssetRequestMode.ImmediateLoad));

			bToggleTransparentSelection = new UIImage(TextureAssets.Buff[BuffID.Invisibility]);


			//		this.bIncreaseBrushSize.Tooltip = "    Increase Brush Size";
			//		this.bDecreaseBrushSize.Tooltip = "    Decrease Brush Size";
			bStampTiles.Tooltip = CSText("PaintTiles");
			bEyeDropper.Tooltip = CSText("EyeDropper");
			bUndo.Tooltip = CSText("Undo");
			bFlipHorizontal.Tooltip = CSText("FlipHorizontal");
			bFlipVertical.Tooltip = CSText("FlipVertical");
			bToggleTransparentSelection.Tooltip = CSText("TransparentSelectionOff");

			//		this.bIncreaseBrushSize.onLeftClick += (s, e) => brushSize = Math.Min(10, brushSize + 1);
			//		this.bDecreaseBrushSize.onLeftClick += (s, e) => brushSize = Math.Max(1, brushSize - 1);
			bStampTiles.onLeftClick += new EventHandler(this.bTogglePaintTiles_onLeftClick);
			bEyeDropper.onLeftClick += new EventHandler(this.bToggleEyeDropper_onLeftClick);
			bFlipVertical.onLeftClick += (s, e) =>
			{
				for (int i = 0; i < StampTiles.GetLength(0); i++)
				{
					for (int j = 0; j < StampTiles.GetLength(1) / 2; j++)
					{
						Utils.Swap(ref StampTiles[i, j], ref StampTiles[i, StampTiles.GetLength(1) - 1 - j]);
					}
				}
				if (stampInfo != null)
				{
					stampInfo.bFlipVertical = !stampInfo.bFlipVertical;
				}
			};
			bUndo.onLeftClick += (a, b) => bUndo_onClick(false);
			bUndo.onRightClick += (a, b) => bUndo_onClick(true);
			bFlipHorizontal.onLeftClick += (s, e) =>
			{
				for (int j = 0; j < StampTiles.GetLength(1); j++)
				{
					for (int i = 0; i < StampTiles.GetLength(0) / 2; i++)
					{
						Utils.Swap(ref StampTiles[i, j], ref StampTiles[StampTiles.GetLength(0) - 1 - i, j]);
					}
				}
				if (stampInfo != null)
				{
					stampInfo.bFlipHorizontal = !stampInfo.bFlipHorizontal;
				}
			};
			bToggleTransparentSelection.onLeftClick += (s, e) => { TransparentSelectionEnabled = !TransparentSelectionEnabled; bToggleTransparentSelection.Tooltip = TransparentSelectionEnabled ? CSText("TransparentSelectionOn") : CSText("TransparentSelectionOff"); };

			onMouseDown += (s, e) =>
			{
				if (!Main.LocalPlayer.mouseInterface && !mod.hotbar.MouseInside && !mod.hotbar.button.MouseInside && !UIView.MouseRightButton)
				{
					leftMouseDown = true;
					Main.LocalPlayer.mouseInterface = true;
				}
			};
			onMouseUp += (s, e) =>
			{
				if (!Main.LocalPlayer.mouseInterface && !mod.hotbar.MouseInside && !mod.hotbar.button.MouseInside && (!UIView.MousePrevRightButton || (UIView.MousePrevLeftButton && !UIView.MouseLeftButton)))
				{
					justLeftMouseDown = true; leftMouseDown = false; /*startTileX = -1; startTileY = -1;*/
				}
			};

			//UpdateWhenOutOfBounds = true;

			//	buttonView.AddChild(bDecreaseBrushSize);
			//		buttonView.AddChild(bIncreaseBrushSize);
			buttonView.AddChild(bStampTiles);
			buttonView.AddChild(bEyeDropper);
			buttonView.AddChild(bUndo);
			buttonView.AddChild(bFlipHorizontal);
			buttonView.AddChild(bFlipVertical);
			buttonView.AddChild(bToggleTransparentSelection);

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

		private void bUndo_onClick(bool right)
		{
			if (UndoHistory.Count == 0)
			{
				Main.NewText(CSText("NoUndo"));
				return;
			}
			if (right)
			{
				for (int i = 0; i < 10; i++)
				{
					Undo();
					if (UndoHistory.Count == 0)
						break;
				}
			}
			else
			{
				Undo();
			}
			//if (UndoHistory.Count > 0)
			Main.NewText(UndoHistory.Count + " " + CSText("UndoLeft"));
		}

		private void Undo()
		{
			if (UndoHistory.Count == 0)
			{
				//Main.NewText("There are no actions left in history to undo.");
			}
			else
			{
				var HistoryItem = UndoHistory.Pop();
				UpdateUndoTooltip();
				Point UndoOrigin = HistoryItem.Item1;
				TileData[,] UndoTiles = HistoryItem.Item2;
				int width = UndoTiles.GetLength(0);
				int height = UndoTiles.GetLength(1);
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						if (WorldGen.InWorld(x + UndoOrigin.X, y + UndoOrigin.Y) && UndoTiles[x, y] != null)
						{
							//Tile target = Framing.GetTileSafely(x + UndoOrigin.X, y + UndoOrigin.Y);
							//target.CopyFrom(UndoTiles[x, y]);
							//Main.tile[x + UndoOrigin.X, y + UndoOrigin.Y] = UndoTiles[x, y];
							UndoTiles[x, y].CopyToTile(Main.tile[x + UndoOrigin.X, y + UndoOrigin.Y]);
						}
					}
				}
				if (Main.netMode == 1)
				{
					NetMessage.SendTileSquare(-1, UndoOrigin.X + width / 2, UndoOrigin.Y + height / 2, Math.Max(width, height));
				}
			}
		}

		internal void UpdateUndoTooltip()
		{
			bUndo.Tooltip = CSText("Undo") + " " + UndoHistory.Count + " " + CSText("Undo2");
		}

		protected override bool IsMouseInside()
		{
			//if (hidden) return false;
			if (EyeDropperActive || StampToolActive) return true;
			return base.IsMouseInside();
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
			//Main.LocalPlayer.showItemIcon = false;
			//if (Visible && !base.IsMouseInside() && (StampToolActive || EyeDropperActive))
			//{
			//	if (!Main.LocalPlayer.mouseInterface)
			//	{
			//		//		Main.LocalPlayer.showItemIcon = true;
			//		DrawBrush();
			//	}
			//}

			//	base.Draw(spriteBatch);

			if (Visible && (base.IsMouseInside() /*|| button.MouseInside*/))
			{
				Main.LocalPlayer.mouseInterface = true;
				//Main.LocalPlayer.showItemIcon = false;
			}

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
				if (UIView.MouseRightButton)
				{
					Main.LocalPlayer.mouseInterface = false;
				}
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

		public void DrawGameScale(SpriteBatch spriteBatch)
		{
			try
			{
				if (Visible && !base.IsMouseInside() && (StampToolActive || EyeDropperActive))
				{
					if (!Main.LocalPlayer.mouseInterface)
					{
						DrawBrush();
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		internal static Color buffColor(Color newColor, float R, float G, float B, float A)
		{
			newColor.R = (byte)((float)newColor.R * R);
			newColor.G = (byte)((float)newColor.G * G);
			newColor.B = (byte)((float)newColor.B * B);
			newColor.A = (byte)((float)newColor.A * A);
			return newColor;
		}

		private void DrawBrush()
		{
			if (EyeDropperActive)
			{
				Vector2 upperLeft;
				Vector2 lowerRight;
				if (leftMouseDown)
				{
					upperLeft = new Vector2(Math.Min(startTileX, lastMouseTileX), Math.Min(startTileY, lastMouseTileY));
					lowerRight = new Vector2(Math.Max(startTileX, lastMouseTileX) + 1, Math.Max(startTileY, lastMouseTileY) + 1);
				}
				else
				{
					upperLeft = Main.MouseWorld.ToTileCoordinates().ToVector2();
					lowerRight = upperLeft.Offset(1, 1);
				}
				Vector2 upperLeftScreen = upperLeft * 16f;
				Vector2 lowerRightScreen = lowerRight * 16f;
				upperLeftScreen -= Main.screenPosition;
				lowerRightScreen -= Main.screenPosition;
				if (Main.LocalPlayer.gravDir == -1f)
				{
					upperLeftScreen.Y = (float)Main.screenHeight - upperLeftScreen.Y;// - 16f;
					lowerRightScreen.Y = (float)Main.screenHeight - lowerRightScreen.Y;// - 16f;

					Utils.Swap(ref upperLeftScreen.Y, ref lowerRightScreen.Y);
				}

				DrawSelectedRectangle(upperLeftScreen, lowerRight - upperLeft, 1f, leftMouseDown);
			}
			else if (StampToolActive && stampInfo != null)
			{
				int width = StampTiles.GetLength(0);
				int height = StampTiles.GetLength(1);
				Vector2 vector = Snap.GetSnapPosition(CheatSheet.instance.paintToolsUI.SnapType, width, height, constrainToAxis, constrainedX, constrainedY, false);

				if (!leftMouseDown && !Main.LocalPlayer.mouseInterface)
				{
					DrawPreview(Main.spriteBatch, stampInfo.Tiles, vector);
				}

				DrawSelectedRectangle(vector, new Vector2(width, height), leftMouseDown ? 1f : .25f, false);
			}
		}

		private void DrawSelectedRectangle(Vector2 upperLeftScreen, Vector2 brushSize, float r, bool drawBack = true)
		{
			Rectangle value = new Rectangle(0, 0, 1, 1);
			//float r = 1f;
			float g = 0.9f;
			float b = 0.1f;
			float a = 1f;
			float scale = 0.6f;
			Color color = buffColor(Color.White, r, g, b, a);
			if (drawBack)
			{
				Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, upperLeftScreen, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, 16f * brushSize, SpriteEffects.None, 0f);
			}
			b = 0.3f;
			g = 0.95f;
			scale = (a = 1f);
			color = buffColor(Color.White, r, g, b, a);
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, upperLeftScreen + Vector2.UnitX * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, upperLeftScreen + Vector2.UnitX * 16f * brushSize.X, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, upperLeftScreen + Vector2.UnitY * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, upperLeftScreen + Vector2.UnitY * 16f * brushSize.Y, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);

			Vector2 pos = Main.MouseScreen.Offset(48, 24);
			Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, $"{brushSize.X} x {brushSize.Y}", pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
		}

		private void bToggleEyeDropper_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			if (EyeDropperActive)
			{
				DisableAllWindows();
			}
			else
			{
				DisableAllWindows();
				EyeDropperActive = true;
				uIImage.ForegroundColor = buttonSelectedColor;
			}
		}

		private void bTogglePaintTiles_onLeftClick(object sender, EventArgs e)
		{
			if (stampInfo == null || StampTiles.GetLength(0) == 0)
			{
				Main.NewText(CSText("PriorBrush"));
			}
			else
			{
				UIImage uIImage = (UIImage)sender;
				if (StampToolActive)
				{
					DisableAllWindows();
				}
				else
				{
					DisableAllWindows();
					StampToolActive = true;
					uIImage.ForegroundColor = buttonSelectedColor;
				}
			}
		}

		private void DisableAllWindows()
		{
			bStampTiles.ForegroundColor = buttonUnselectedColor;
			bEyeDropper.ForegroundColor = buttonUnselectedColor;
			StampToolActive = false;
			EyeDropperActive = false;
			startTileX = startTileY = lastMouseTileX = lastMouseTileY = -1;
		}

		public override void Update()
		{
			DoSlideMovement();
			base.CenterXAxisToParentCenter();
			base.Update();
		}

		public void UpdateGameScale()
		{
			try
			{
				Update2();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public void Update2()
		{
			Player player = Main.LocalPlayer;
			if (selected && (EyeDropperActive || StampToolActive))
			{
				//			player.mouseInterface = true;
				//			player.showItemIcon = true;
				if (EyeDropperActive)
				{
					//		Main.LocalPlayer.showItemIconText = "Click to select pallete";
					player.cursorItemIconID = ItemID.EmptyDropper;
					if (leftMouseDown)
					{
						Point point = (Main.MouseWorld).ToTileCoordinates();
						//Point point = (Main.MouseWorld + (brushSize % 2 == 0 ? Vector2.One * 8 : Vector2.Zero)).ToTileCoordinates();
						//point.X -= brushSize / 2;
						//point.Y -= brushSize / 2;
						if (startTileX == -1)
						{
							startTileX = point.X;
							startTileY = point.Y;
							lastMouseTileX = -1;
							lastMouseTileY = -1;
						}

						//if (lastMouseTileX != point.X && lastMouseTileY != point.Y)
						{
							//for (int x = 0; x < brushSize; x++)
							//{
							//	for (int y = 0; y < brushSize; y++)
							//	{
							//		if (WorldGen.InWorld(x + point.X, y + point.Y))
							//		{
							//			Tile target = Framing.GetTileSafely(x + point.X, y + point.Y);
							//			BrushTiles[x, y].CopyFrom(target);
							//			//	Main.NewText("{x}, {y}");
							//		}
							//	}
							//}
							lastMouseTileX = point.X;
							lastMouseTileY = point.Y;
						}
					}
					if (justLeftMouseDown)
					{
						if (startTileX != -1 && startTileY != -1 && lastMouseTileX != -1 && lastMouseTileY != -1)
						{
							Vector2 upperLeft = new Vector2(Math.Min(startTileX, lastMouseTileX), Math.Min(startTileY, lastMouseTileY));
							Vector2 lowerRight = new Vector2(Math.Max(startTileX, lastMouseTileX), Math.Max(startTileY, lastMouseTileY));

							int minX = (int)upperLeft.X;
							int maxX = (int)lowerRight.X + 1;
							int minY = (int)upperLeft.Y;
							int maxY = (int)lowerRight.Y + 1;

							//ErrorLogger.Log(string.Format("JustDown2 {0} {1} {2} {3}", minX, minY, maxX, maxY));

							StampTiles = new TileData[maxX - minX, maxY - minY];

							for (int i = 0; i < maxX - minX; i++)
							{
								for (int j = 0; j < maxY - minY; j++)
								{
									StampTiles[i, j] = new TileData();
								}
							}

							for (int x = minX; x < maxX; x++)
							{
								for (int y = minY; y < maxY; y++)
								{
									if (WorldGen.InWorld(x, y))
									{
										if (Main.tile[x, y].TileType == TileID.Count)
										{
										}
										if (Main.tile[x, y].HasTile)
										{
											WorldGen.TileFrame(x, y, true, false);
										}
										if (Main.tile[x, y].WallType > 0)
										{
											Framing.WallFrame(x, y, true);
										}

										Tile target = Framing.GetTileSafely(x, y);
										StampTiles[x - minX, y - minY] = new TileData(target);
										if (Main.tile[x, y].TileType == TileID.Count)
											StampTiles[x - minX, y - minY] = new TileData();
										if (Main.tileContainer[Main.tile[x, y].TileType])
											StampTiles[x - minX, y - minY] = new TileData();
									}
								}
							}

							//Main.NewText("EyeDropper: width height" + (maxX - minX) + " " + (maxY - minY));
							CheatSheet.instance.paintToolsUI.AddSlot(PaintToolsEx.GetStampInfo(StampTiles));
							//CalculateItemCost(StampTiles);
						}
						//Main.NewText("EyeDropper: x,y,min max " + minX + " " + maxX + " " + minY + " " + maxY);

						startTileX = -1;
						startTileY = -1;
						lastMouseTileX = -1;
						lastMouseTileY = -1;
						justLeftMouseDown = false;
					}
				}
				if (StampToolActive)
				{
					player.cursorItemIconID = ItemID.Paintbrush;
					//		Main.LocalPlayer.showItemIconText = "Click to paint";
					if (leftMouseDown && stampInfo != null)
					{
						int width = StampTiles.GetLength(0);
						int height = StampTiles.GetLength(1);
						//Vector2 brushsize = new Vector2(width, height);
						//Vector2 evenOffset = Vector2.Zero;
						//if (width % 2 == 0)
						//{
						//	evenOffset.X = 1;
						//}
						//if (height % 2 == 0)
						//{
						//	evenOffset.Y = 1;
						//}
						//Point point = (Main.MouseWorld + evenOffset * 8).ToTileCoordinates();
						////Point point = (Main.MouseWorld + (brushSize % 2 == 0 ? Vector2.One * 8 : Vector2.Zero)).ToTileCoordinates();
						//point.X -= width / 2;
						//point.Y -= height / 2;
						////Vector2 vector = new Vector2(point.X, point.Y) * 16f;
						////vector -= Main.screenPosition;
						////if (Main.LocalPlayer.gravDir == -1f)
						////{
						////	vector.Y = (float)Main.screenHeight - vector.Y - 16f;
						////}

						Point point = Snap.GetSnapPosition(CheatSheet.instance.paintToolsUI.SnapType, width, height, constrainToAxis, constrainedX, constrainedY, true).ToPoint();

						if (startTileX == -1)
						{
							startTileX = point.X;
							startTileY = point.Y;
							lastMouseTileX = -1;
							lastMouseTileY = -1;
						}

						if (Main.keyState.IsKeyDown(Keys.LeftShift))
						{
							constrainToAxis = true;
							if (constrainedStartX == -1 && constrainedStartY == -1)
							{
								constrainedStartX = point.X;
								constrainedStartY = point.Y;
							}

							if (constrainedX == -1 && constrainedY == -1)
							{
								if (constrainedStartX != point.X)
								{
									constrainedY = point.Y;
								}
								else if (constrainedStartY != point.Y)
								{
									constrainedX = point.X;
								}
							}
							if (constrainedX != -1)
							{
								point.X = constrainedX;
							}
							if (constrainedY != -1)
							{
								point.Y = constrainedY;
							}
						}
						else
						{
							constrainToAxis = false;
							constrainedX = -1;
							constrainedY = -1;
							constrainedStartX = -1;
							constrainedStartY = -1;
						}

						if (lastMouseTileX != point.X || lastMouseTileY != point.Y)
						{
							lastMouseTileX = point.X;
							lastMouseTileY = point.Y;
							//Main.NewText("StartTileX " + startTileX);
							UndoHistory.Push(Tuple.Create(point, new TileData[width, height]));
							UpdateUndoTooltip();
							for (int x = 0; x < width; x++)
							{
								for (int y = 0; y < height; y++)
								{
									if (WorldGen.InWorld(x + point.X, y + point.Y) && StampTiles[x, y] != null)
									{
										Tile target = Framing.GetTileSafely(x + point.X, y + point.Y);
										UndoHistory.Peek().Item2[x, y] = new TileData(target);
										int cycledX = ((x + point.X - startTileX) % width + width) % width;
										int cycledY = ((y + point.Y - startTileY) % height + height) % height;
										if (TransparentSelectionEnabled) // What about just walls?
										{
											if (StampTiles[cycledX, cycledY].TileWallWireStateData.HasTile)
											{
												//target.CopyFrom(StampTiles[cycledX, cycledY]);
												StampTiles[cycledX, cycledY].CopyToTile(target);
											}
										}
										else
										{
											//target.CopyFrom(StampTiles[cycledX, cycledY]);
											StampTiles[cycledX, cycledY].CopyToTile(target);
										}
									}
								}
							}
							// TODO: Experiment with WorldGen.stopDrops = true;
							// TODO: Button to ignore TileFrame?
							for (int i = point.X; i < point.X + width; i++)
							{
								for (int j = 0; j < point.Y + height; j++)
								{
									WorldGen.SquareTileFrame(i, j, false); // Need to do this after stamp so neighbors are correct.
									if (Main.netMode == 1 && Framing.GetTileSafely(i, j).LiquidAmount > 0)
									{
										NetMessage.sendWater(i, j); // Does it matter that this is before sendtilesquare?
									}
								}
							}
							if (Main.netMode == 1)
							{
								NetMessage.SendTileSquare(-1, point.X + width / 2, point.Y + height / 2, Math.Max(width, height));
							}
						}
					}
					else
					{
						startTileX = -1;
						startTileY = -1;
						constrainToAxis = false;
						constrainedX = -1;
						constrainedY = -1;
						constrainedStartX = -1;
						constrainedStartY = -1;
					}
				}
				Main.LocalPlayer.cursorItemIconEnabled = true;
			}
		}

		//private void CalculateItemCost(Tile[,] stampTiles)
		//{
		//	Dictionary<int, int> itemsNeeded = new Dictionary<int, int>();

		//	for (int i = 0; i < stampTiles.GetLength(0); i++)
		//	{
		//		for (int j = 0; j < stampTiles.GetLength(1); j++)
		//		{
		//			Tile tile = stampTiles[i, j];
		//			int style = 0;
		//			int alternate = 0;
		//			TileObjectData.GetTileInfo(tile, ref style, ref alternate);

		//			var tod = TileObjectData.GetTileData(tile);
		//			//tod.
		//			if (tile.active())
		//			{
		//				int tileType = tile.type;
		//			}
		//		}
		//	}
		//}

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

		private bool preHidePaintTiles;
		private bool preHideEyeDropper;
		private bool constrainToAxis;
		private int constrainedX;
		private int constrainedY;
		private int constrainedStartX;
		private int constrainedStartY;

		public void Hide()
		{
			preHideEyeDropper = EyeDropperActive;
			preHidePaintTiles = StampToolActive;

			StampToolActive = false;
			EyeDropperActive = false;

			hidden = true;
			arrived = false;
		}

		public void Show()
		{
			mod.hotbar.currentHotbar = this;
			arrived = false;
			hidden = false;
			Visible = true;

			EyeDropperActive = preHideEyeDropper;
			StampToolActive = preHidePaintTiles;
		}

		public static void DrawPreview(SpriteBatch sb, TileData[,] BrushTiles, Vector2 position, float scale = 1f)
		{
			Color color = Color.White;
			color.A = 160;
			int width = BrushTiles.GetLength(0);
			int height = BrushTiles.GetLength(1);
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					TileData tile = BrushTiles[x, y];
					if (tile.WallTypeData.Type > 0)
					{
						Main.instance.LoadWall(tile.WallTypeData.Type);
						Texture2D textureWall;
						//			if (PaintToolsEx.canDrawColorWall(tile) && tile.type < Main.wallAltTexture.GetLength(0) && Main.wallAltTexture[tile.type, tile.wallColor()] != null)
						//				textureWall = Main.wallAltTexture[tile.type, tile.wallColor()];
						//			else
						//				textureWall = TextureAssets.Wall[tile.wall].Value;

						if (GetTileDrawTextureMethodInfo == null)
						{
							GetTileDrawTextureMethodInfo = typeof(WallDrawing).GetMethod("GetTileDrawTexture", BindingFlags.Instance | BindingFlags.NonPublic);
						}

						// TODO: needs a Tile instance
						//textureWall = (Texture2D)GetTileDrawTextureMethodInfo.Invoke( Main.instance.WallsRenderer, new object[] { tile, -1, -1});
						textureWall = GetTileDrawTexture(tile, -1, -1);

						//textureWall = WallDrawing.GetTileDrawTexture(tile, -1, -1); // x/y unused

						int wallFrame = Main.wallFrame[tile.WallTypeData.Type] * 180;
						Rectangle value = new Rectangle(tile.TileWallWireStateData.WallFrameX, tile.TileWallWireStateData.WallFrameY + wallFrame, 32, 32);
						Vector2 pos = position + new Vector2(x * 16 - 8, y * 16 - 8);
						sb.Draw(textureWall, pos * scale, value, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
					}
					if (tile.LiquidData.Amount > 14)
					{
						Texture2D textureWater;
						if (tile.LiquidData.LiquidType == LiquidID.Honey)
							textureWater = LiquidRenderer.Instance._liquidTextures[11].Value.Offset(16, 48, 16, 16);
						else if (tile.LiquidData.LiquidType == LiquidID.Lava)
							textureWater = LiquidRenderer.Instance._liquidTextures[1].Value.Offset(16, 48, 16, 16);
						else
							textureWater = LiquidRenderer.Instance._liquidTextures[0].Value.Offset(16, 48, 16, 16);
						int waterSize = (tile.LiquidData.Amount + 1) / 16;
						Vector2 pos = position + new Vector2(x * 16, y * 16 + (16 - waterSize));
						sb.Draw(textureWater, pos * scale, new Rectangle(0, 16 - waterSize, 16, waterSize), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
					}
					if (tile.TileWallWireStateData.HasTile) // Tile
					{
						Main.instance.LoadTiles(tile.TileTypeData.Type);
						Texture2D texture = TextureAssets.Tile[tile.TileTypeData.Type].Value;
						Rectangle? value = new Rectangle(tile.TileWallWireStateData.TileFrameX, tile.TileWallWireStateData.TileFrameY, 16, 16/* tileData.CoordinateWidth, tileData.CoordinateHeights[j - (int)op.ObjectStart.Y]*/);
						Vector2 pos = position + new Vector2(x * 16, y * 16);
						sb.Draw(texture, pos * scale, value, color, 0f, Vector2.Zero, scale, /*spriteEffects*/SpriteEffects.None, 0f);
					}
				}
			}
			// Draw deleted tiles with non transparent selection?
		}


		// Copied from tmod since original code needs Tile instance
		static FieldInfo _paintSystemFieldInfo;
		static TilePaintSystemV2 _paintSystem;
		private static Texture2D GetTileDrawTexture(TileData tile, int tileX, int tileY)
		{
			if (_paintSystemFieldInfo == null)
			{
				_paintSystemFieldInfo = typeof(WallDrawing).GetField("_paintSystem", BindingFlags.Instance | BindingFlags.NonPublic);
				_paintSystem = (TilePaintSystemV2)_paintSystemFieldInfo.GetValue(Main.instance.WallsRenderer);
			}

			Texture2D result = TextureAssets.Wall[tile.WallTypeData.Type].Value;
			int wall = tile.WallTypeData.Type;
			Texture2D texture2D = _paintSystem.TryGetWallAndRequestIfNotReady(wall, tile.TileWallWireStateData.WallColor);
			if (texture2D != null)
				result = texture2D;

			return result;
		}

		/*public static void DrawPreview(SpriteBatch sb, StampInfo info, Vector2 position)
		{
			int maxX = info.Textures.GetLength(0);
			int maxY = info.Textures.GetLength(1);
			bool isHR = info.bFlipHorizontal;
			bool isVR = info.bFlipVertical ^ Main.LocalPlayer.gravDir == -1f;

			Texture2D[,] textures = new Texture2D[maxX, maxY];
			for (int x = 0; x < maxX; x++)
			{
				for (int y = 0; y < maxY; y++)
				{
					textures[isHR ? maxX - x - 1 : x, isVR ? maxY - y - 1 : y] = info.Textures[x, y];
				}
			}

			SpriteEffects effects = SpriteEffects.None;
			if (isHR)
				effects |= SpriteEffects.FlipHorizontally;
			if (isVR)
				effects |= SpriteEffects.FlipVertically;

			for (int x = 0; x < info.Textures.GetLength(0); x++)
			{
				Vector2 pos = position;
				pos.X += x * ModUtils.TextureMaxTile * 16;
				for (int y = 0; y < info.Textures.GetLength(1); y++)
				{
					sb.Draw(textures[x, y], pos, null, Color.White * 0.6f, 0f, Vector2.Zero, 1f, effects, 0f);
					pos.Y += ModUtils.TextureMaxTile * 16;
				}
			}
		}*/

		public static void Smooth()
		{
			//progress.Message = Lang.gen[60];
			for (int k = 20; k < Main.maxTilesX - 20; k++)
			{
				float value = (float)k / (float)Main.maxTilesX;
				//progress.Set(value);
				for (int l = 20; l < Main.maxTilesY - 20; l++)
				{
					if (Main.tile[k, l].TileType != 48 && Main.tile[k, l].TileType != 137 && Main.tile[k, l].TileType != 232 && Main.tile[k, l].TileType != 191 && Main.tile[k, l].TileType != 151 && Main.tile[k, l].TileType != 274)
					{
						if (!Main.tile[k, l - 1].HasTile)
						{
							if (WorldGen.SolidTile(k, l) && TileID.Sets.CanBeClearedDuringGeneration[(int)Main.tile[k, l].TileType])
							{
								if (!Main.tile[k - 1, l].IsHalfBlock && !Main.tile[k + 1, l].IsHalfBlock && Main.tile[k - 1, l].Slope == 0 && Main.tile[k + 1, l].Slope == 0)
								{
									if (WorldGen.SolidTile(k, l + 1))
									{
										if (!WorldGen.SolidTile(k - 1, l) && !Main.tile[k - 1, l + 1].IsHalfBlock && WorldGen.SolidTile(k - 1, l + 1) && WorldGen.SolidTile(k + 1, l) && !Main.tile[k + 1, l - 1].HasTile)
										{
											if (WorldGen.genRand.Next(2) == 0)
											{
												WorldGen.SlopeTile(k, l, 2);
											}
											else
											{
												WorldGen.PoundTile(k, l);
											}
										}
										else if (!WorldGen.SolidTile(k + 1, l) && !Main.tile[k + 1, l + 1].IsHalfBlock && WorldGen.SolidTile(k + 1, l + 1) && WorldGen.SolidTile(k - 1, l) && !Main.tile[k - 1, l - 1].HasTile)
										{
											if (WorldGen.genRand.Next(2) == 0)
											{
												WorldGen.SlopeTile(k, l, 1);
											}
											else
											{
												WorldGen.PoundTile(k, l);
											}
										}
										else if (WorldGen.SolidTile(k + 1, l + 1) && WorldGen.SolidTile(k - 1, l + 1) && !Main.tile[k + 1, l].HasTile && !Main.tile[k - 1, l].HasTile)
										{
											WorldGen.PoundTile(k, l);
										}
										if (WorldGen.SolidTile(k, l))
										{
											if (WorldGen.SolidTile(k - 1, l) && WorldGen.SolidTile(k + 1, l + 2) && !Main.tile[k + 1, l].HasTile && !Main.tile[k + 1, l + 1].HasTile && !Main.tile[k - 1, l - 1].HasTile)
											{
												WorldGen.KillTile(k, l, false, false, false);
											}
											else if (WorldGen.SolidTile(k + 1, l) && WorldGen.SolidTile(k - 1, l + 2) && !Main.tile[k - 1, l].HasTile && !Main.tile[k - 1, l + 1].HasTile && !Main.tile[k + 1, l - 1].HasTile)
											{
												WorldGen.KillTile(k, l, false, false, false);
											}
											else if (!Main.tile[k - 1, l + 1].HasTile && !Main.tile[k - 1, l].HasTile && WorldGen.SolidTile(k + 1, l) && WorldGen.SolidTile(k, l + 2))
											{
												if (WorldGen.genRand.Next(5) == 0)
												{
													WorldGen.KillTile(k, l, false, false, false);
												}
												else if (WorldGen.genRand.Next(5) == 0)
												{
													WorldGen.PoundTile(k, l);
												}
												else
												{
													WorldGen.SlopeTile(k, l, 2);
												}
											}
											else if (!Main.tile[k + 1, l + 1].HasTile && !Main.tile[k + 1, l].HasTile && WorldGen.SolidTile(k - 1, l) && WorldGen.SolidTile(k, l + 2))
											{
												if (WorldGen.genRand.Next(5) == 0)
												{
													WorldGen.KillTile(k, l, false, false, false);
												}
												else if (WorldGen.genRand.Next(5) == 0)
												{
													WorldGen.PoundTile(k, l);
												}
												else
												{
													WorldGen.SlopeTile(k, l, 1);
												}
											}
										}
									}
									if (WorldGen.SolidTile(k, l) && !Main.tile[k - 1, l].HasTile && !Main.tile[k + 1, l].HasTile)
									{
										WorldGen.KillTile(k, l, false, false, false);
									}
								}
							}
							else if (!Main.tile[k, l].HasTile && Main.tile[k, l + 1].TileType != 151 && Main.tile[k, l + 1].TileType != 274)
							{
								if (Main.tile[k + 1, l].TileType != 190 && Main.tile[k + 1, l].TileType != 48 && Main.tile[k + 1, l].TileType != 232 && WorldGen.SolidTile(k - 1, l + 1) && WorldGen.SolidTile(k + 1, l) && !Main.tile[k - 1, l].HasTile && !Main.tile[k + 1, l - 1].HasTile)
								{
									WorldGen.PlaceTile(k, l, (int)Main.tile[k, l + 1].TileType, false, false, -1, 0);
									if (WorldGen.genRand.Next(2) == 0)
									{
										WorldGen.SlopeTile(k, l, 2);
									}
									else
									{
										WorldGen.PoundTile(k, l);
									}
								}
								if (Main.tile[k - 1, l].TileType != 190 && Main.tile[k - 1, l].TileType != 48 && Main.tile[k - 1, l].TileType != 232 && WorldGen.SolidTile(k + 1, l + 1) && WorldGen.SolidTile(k - 1, l) && !Main.tile[k + 1, l].HasTile && !Main.tile[k - 1, l - 1].HasTile)
								{
									WorldGen.PlaceTile(k, l, (int)Main.tile[k, l + 1].TileType, false, false, -1, 0);
									if (WorldGen.genRand.Next(2) == 0)
									{
										WorldGen.SlopeTile(k, l, 1);
									}
									else
									{
										WorldGen.PoundTile(k, l);
									}
								}
							}
						}
						else if (!Main.tile[k, l + 1].HasTile && WorldGen.genRand.Next(2) == 0 && WorldGen.SolidTile(k, l) && !Main.tile[k - 1, l].IsHalfBlock && !Main.tile[k + 1, l].IsHalfBlock && Main.tile[k - 1, l].Slope == SlopeType.Solid && Main.tile[k + 1, l].Slope == SlopeType.Solid && WorldGen.SolidTile(k, l - 1))
						{
							if (WorldGen.SolidTile(k - 1, l) && !WorldGen.SolidTile(k + 1, l) && WorldGen.SolidTile(k - 1, l - 1))
							{
								WorldGen.SlopeTile(k, l, 3);
							}
							else if (WorldGen.SolidTile(k + 1, l) && !WorldGen.SolidTile(k - 1, l) && WorldGen.SolidTile(k + 1, l - 1))
							{
								WorldGen.SlopeTile(k, l, 4);
							}
						}
					}
				}
			}
			for (int m = 20; m < Main.maxTilesX - 20; m++)
			{
				for (int n = 20; n < Main.maxTilesY - 20; n++)
				{
					if (WorldGen.genRand.Next(2) == 0 && !Main.tile[m, n - 1].HasTile && Main.tile[m, n].TileType != 137 && Main.tile[m, n].TileType != 48 && Main.tile[m, n].TileType != 232 && Main.tile[m, n].TileType != 191 && Main.tile[m, n].TileType != 151 && Main.tile[m, n].TileType != 274 && Main.tile[m, n].TileType != 75 && Main.tile[m, n].TileType != 76 && WorldGen.SolidTile(m, n) && Main.tile[m - 1, n].TileType != 137 && Main.tile[m + 1, n].TileType != 137)
					{
						if (WorldGen.SolidTile(m, n + 1) && WorldGen.SolidTile(m + 1, n) && !Main.tile[m - 1, n].HasTile)
						{
							WorldGen.SlopeTile(m, n, 2);
						}
						if (WorldGen.SolidTile(m, n + 1) && WorldGen.SolidTile(m - 1, n) && !Main.tile[m + 1, n].HasTile)
						{
							WorldGen.SlopeTile(m, n, 1);
						}
					}
					if (Main.tile[m, n].Slope == SlopeType.SlopeDownLeft && !WorldGen.SolidTile(m - 1, n))
					{
						WorldGen.SlopeTile(m, n, 0);
						WorldGen.PoundTile(m, n);
					}
					if (Main.tile[m, n].Slope == SlopeType.SlopeDownRight && !WorldGen.SolidTile(m + 1, n))
					{
						WorldGen.SlopeTile(m, n, 0);
						WorldGen.PoundTile(m, n);
					}
				}
			}
			Main.tileSolid[137] = true;
			Main.tileSolid[190] = false;
			Main.tileSolid[192] = false;
		}
	}
}