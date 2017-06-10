using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;

namespace CheatSheet.Menus
{
	internal class EventManagerHotbar : UIHotbar
	{
		public UIView buttonView;

		public UIImage bGoblinInvasion;
		public UIImage bBloodmoon;
		public UIImage bSlimerain;
		public UIImage bFrostlegion;
		public UIImage bSolarEclipse;
		public UIImage bPirateInvasion;
		public UIImage bPumpkinMoon;
		public UIImage bFrostMoon;
		public UIImage bMartianMadness;
		public UIImage bStopEvents;

		internal bool mouseDown;
		internal bool justMouseDown;

		private CheatSheet mod;

		public EventManagerHotbar(CheatSheet mod)
		{
			this.mod = mod;
			//parentHotbar = mod.hotbar;

			this.buttonView = new UIView();
			base.Visible = false;

			// Button images
			bGoblinInvasion = new UIImage(Main.itemTexture[ItemID.GoblinBattleStandard]);
			bBloodmoon = new UIImage(Main.itemTexture[ItemID.PiggyBank]);
			bSlimerain = new UIImage(Main.itemTexture[ItemID.RoyalGel]);
			bFrostlegion = new UIImage(Main.itemTexture[ItemID.SnowGlobe]);
			bSolarEclipse = new UIImage(Main.itemTexture[ItemID.SolarTablet]);
			bPirateInvasion = new UIImage(Main.itemTexture[ItemID.PirateMap]);
			bPumpkinMoon = new UIImage(Main.itemTexture[ItemID.PumpkinMoonMedallion]);
			bFrostMoon = new UIImage(Main.itemTexture[ItemID.NaughtyPresent]);
			bMartianMadness = new UIImage(Main.itemTexture[ItemID.MartianSaucerTrophy]);
			bStopEvents = new UIImage(Main.itemTexture[ItemID.AlphabetStatueX]);

			// Button tooltips
			bGoblinInvasion.Tooltip = "Summon a Goblin Invasion";
			bBloodmoon.Tooltip = "Start a bloodmoon";
			bSlimerain.Tooltip = "Start a slimerain";
			bFrostlegion.Tooltip = "Summon a Frost Legion";
			bSolarEclipse.Tooltip = "Start a solar eclipse";
			bPirateInvasion.Tooltip = "Summon a Pirate Invasion";
			bPumpkinMoon.Tooltip = "Start a pumpkin moon";
			bFrostMoon.Tooltip = "Start a frost moon";
			bMartianMadness.Tooltip = "Start a martian madness";
			bStopEvents.Tooltip = "Stop all events";

			// Button EventHandlers
			bGoblinInvasion.onLeftClick += new EventHandler(this.bGoblinInvasion_onLeftClick);
			bBloodmoon.onLeftClick += new EventHandler(this.bBloodmoon_onLeftClick);
			bSlimerain.onLeftClick += new EventHandler(this.bSlimerain_onLeftClick);
			bFrostlegion.onLeftClick += new EventHandler(this.bFrostlegion_onLeftClick);
			bSolarEclipse.onLeftClick += new EventHandler(this.bSolarEclipse_onLeftClick);
			bPirateInvasion.onLeftClick += new EventHandler(this.bPirateInvasion_onLeftClick);
			bPumpkinMoon.onLeftClick += new EventHandler(this.bPumpkinMoon_onLeftClick);
			bFrostMoon.onLeftClick += new EventHandler(this.bFrostMoon_onLeftClick);
			bMartianMadness.onLeftClick += new EventHandler(this.bMartianMadness_onLeftClick);
			bStopEvents.onLeftClick += new EventHandler(this.bStopEvents_onLeftClick);

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
			buttonView.AddChild(bGoblinInvasion);
			buttonView.AddChild(bBloodmoon);
			buttonView.AddChild(bSlimerain);
			buttonView.AddChild(bFrostlegion);
			buttonView.AddChild(bSolarEclipse);
			buttonView.AddChild(bPirateInvasion);
			buttonView.AddChild(bPumpkinMoon);
			buttonView.AddChild(bFrostMoon);
			buttonView.AddChild(bMartianMadness);
			buttonView.AddChild(bStopEvents);

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

		private void bStopEvents_onLeftClick(object sender, EventArgs e)
		{
			//Reset invasions
			Main.invasionType = 0;
			Main.invasionX = 0.0;
			Main.invasionSize = 0;
			Main.invasionDelay = 0;
			Main.invasionWarn = 0;
			Main.invasionSizeStart = 0;
			Main.invasionProgressNearInvasion = false;
			Main.invasionProgressMode = 2;
			Main.invasionProgressIcon = 0;
			Main.invasionProgress = 0;
			Main.invasionProgressMax = 0;
			Main.invasionProgressWave = 0;
			Main.invasionProgressDisplayLeft = 0;
			Main.invasionProgressAlpha = 0.0f;
			//Reset others
			Main.stopMoonEvent(); // pumpkin / snow - moon
			Main.eclipse = false;
			Main.StopSlimeRain();
			Main.bloodMoon = false;
		}

		private void bMartianMadness_onLeftClick(object sender, EventArgs e)
		{
			if (Main.CanStartInvasion(4)) Main.StartInvasion(4);
		}

		private void bFrostMoon_onLeftClick(object sender, EventArgs e)
		{
			Main.dayTime = false;
			Main.stopMoonEvent();
			if (Main.netMode != 1)
			{
				Main.NewText(Lang.misc[34].Value, (byte)50, byte.MaxValue, (byte)130, false);
				Main.startSnowMoon();
			}
			else
				NetMessage.SendData(61, -1, -1, null, Main.LocalPlayer.whoAmI, -5f, 0.0f, 0.0f, 0, 0, 0);
		}

		private void bPumpkinMoon_onLeftClick(object sender, EventArgs e)
		{
			Main.dayTime = false;
			Main.stopMoonEvent();
			// REQUIRED
			if (Main.netMode != 1)
			{
				Main.NewText(Lang.misc[31].Value, (byte)50, byte.MaxValue, (byte)130, false);
				Main.startPumpkinMoon();
			}
			else
			{
				NetMessage.SendData(61, -1, -1, null, Main.LocalPlayer.whoAmI, -4f, 0.0f, 0.0f, 0, 0, 0);
			}
		}

		private void bPirateInvasion_onLeftClick(object sender, EventArgs e)
		{
			if (Main.CanStartInvasion(3)) Main.StartInvasion(3);
		}

		private void bSolarEclipse_onLeftClick(object sender, EventArgs e)
		{
			Main.dayTime = true;
			Main.time = 32400.1;

			if (Main.netMode != 1)
			{
				AchievementsHelper.NotifyProgressionEvent(1);
				Main.eclipse = true;
				AchievementsHelper.NotifyProgressionEvent(2);
				if (Main.eclipse)
				{
					if (Main.netMode == 0)
					{
						Main.NewText(Lang.misc[20].Value, (byte)50, byte.MaxValue, (byte)130, false);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(Lang.misc[20].ToNetworkText(), new Color(50, 255, 130), -1);
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(7, -1, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
				}
			}
		}

		private void bFrostlegion_onLeftClick(object sender, EventArgs e)
		{
			if (Main.CanStartInvasion(2)) Main.StartInvasion(2);
		}

		private void bSlimerain_onLeftClick(object sender, EventArgs e)
		{
			if (Main.netMode != 1 && !Main.gameMenu || Main.netMode == 2)
			{
				//(!Main.raining && Main.slimeRainTime == 0.0 && (!Main.bloodMoon && !Main.eclipse) && (!Main.snowMoon && !Main.pumpkinMoon && Main.invasionType == 0))
				Main.raining = false;
				Main.slimeRainTime = 0.0;
				Main.stopMoonEvent(); // pumpkin / snow
				Main.bloodMoon = false;
				Main.eclipse = false;
				Main.invasionType = 0;
				//Actual
				Main.StartSlimeRain(true);
			}
		}

		private void bBloodmoon_onLeftClick(object sender, EventArgs e)
		{
			Main.dayTime = false;
			Main.time = 0.0;
			if (Main.moonPhase == 0) Main.moonPhase++;
			WorldGen.spawnEye = false;
			Main.bloodMoon = true;
			AchievementsHelper.NotifyProgressionEvent(4);
			if (Main.netMode == 0)
				Main.NewText(Lang.misc[8].Value, 50, 255, 130, false);
			else if (Main.netMode == 2)
				NetMessage.BroadcastChatMessage(Lang.misc[8].ToNetworkText(), new Microsoft.Xna.Framework.Color(50, 255, 130), -1);
		}

		private void bGoblinInvasion_onLeftClick(object sender, EventArgs e)
		{
			if (Main.CanStartInvasion()) Main.StartInvasion();
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
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, this._rasterizerState);
				//	Rectangle scissorRectangle = new Rectangle((int)base.X- (int)base.Width, (int)base.Y, (int)base.Width, (int)base.Height);
				//Parent.Position.Y
				//		Main.NewText((int)Parent.Position.Y + " " + (int)shownPosition);
				//	Rectangle scissorRectangle = new Rectangle((int)(base.X - base.Width / 2), (int)(shownPosition), (int)base.Width, (int)base.Height);
				Rectangle scissorRectangle = new Rectangle((int)(base.X - base.Width / 2), (int)(shownPosition), (int)base.Width, (int)(mod.hotbar.Position.Y - shownPosition));
				if (scissorRectangle.X < 0)
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
				}
				Rectangle scissorRectangle2 = spriteBatch.GraphicsDevice.ScissorRectangle;
				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

				base.Draw(spriteBatch);

				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
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