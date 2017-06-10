using CheatSheet.Menus;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CheatSheet.CustomUI
{
	//
	internal class UIHotbar : UIWindow
	{
		internal RasterizerState _rasterizerState = new RasterizerState
		{
			ScissorTestEnable = true
		};

		internal static float slideMoveSpeed = 8f;
		internal float lerpAmount;

		internal UIHotbar parentHotbar;

		internal float shownPosition
		{
			get
			{
				return (float)Main.screenHeight - base.Height * 2 - 12f + 6;
			}
		}

		internal float hiddenPosition
		{
			get
			{
				if (parentHotbar != null && parentHotbar.Position.Y == parentHotbar.shownPosition)
				{
					return (float)Main.screenHeight - base.Height - 12f;
				}
				//else if (mod.hotbar != null && !mod.hotbar.hidden && hidden)
				//{
				//	return (float)Main.screenHeight - base.Height - 12f;
				//}
				else
				{
					return (float)Main.screenHeight;
				}
			}
		}

		internal float spacing = 8f;

		public bool hidden;
		internal bool arrived;

		private bool _selected;

		internal bool selected
		{
			get { return _selected; }
			set
			{
				if (value == false)
				{
					hidden = true;
				}
				else
				{
					hidden = false;
					Visible = true;
				}
				arrived = false;
				_selected = value;
			}
		}

		internal static Color buttonUnselectedColor = Color.LightSkyBlue;
		internal static Color buttonSelectedColor = Color.White;
		internal static Color buttonSelectedHiddenColor = Color.Blue;

		internal void DoSlideMovement()
		{
			if (!arrived)
			{
				if (this.hidden)
				{
					this.lerpAmount -= .01f * slideMoveSpeed;
					if (this.lerpAmount < 0f)
					{
						this.lerpAmount = 0f;
						arrived = true;
						this.Visible = false;
					}
					float y = MathHelper.SmoothStep(this.hiddenPosition, this.shownPosition, this.lerpAmount);
					base.Position = new Vector2(Hotbar.xPosition, y);
				}
				else
				{
					this.lerpAmount += .01f * slideMoveSpeed;
					if (this.lerpAmount > 1f)
					{
						this.lerpAmount = 1f;
						arrived = true;
					}
					float y2 = MathHelper.SmoothStep(this.hiddenPosition, this.shownPosition, this.lerpAmount);
					base.Position = new Vector2(Hotbar.xPosition, y2);
				}
			}
		}
	}
}