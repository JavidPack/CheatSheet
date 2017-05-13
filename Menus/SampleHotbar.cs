using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CheatSheet.CustomUI;
using Microsoft.Xna.Framework;
using CheatSheet.UI;
using Microsoft.Xna.Framework.Graphics;

namespace CheatSheet.Menus
{
    internal class SampleHotbar : UIHotbar
    {
        public UIView buttonView;
        public UIImage bSampleButton;

        internal bool mouseDown;
        internal bool justMouseDown;

        private CheatSheet mod;

        public SampleHotbar(CheatSheet mod)
        {
            this.mod = mod;
            //parentHotbar = mod.hotbar;

            this.buttonView = new UIView();
            base.Visible = false;

            // Button images
            bSampleButton = new UIImage(Main.itemTexture[ItemID.Paintbrush]);

            // Button tooltips
            bSampleButton.Tooltip = "Sample Tooltip";

            // Button EventHandlers
            bSampleButton.onLeftClick += new EventHandler(this.bSampleButton_onLeftClick);
            bSampleButton.onRightClick += (s, e) =>
            {
                // Sample handling
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
            buttonView.AddChild(bSampleButton);

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

        private void bSampleButton_onLeftClick(object sender, EventArgs e)
        {
            // Sample handling left click
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
