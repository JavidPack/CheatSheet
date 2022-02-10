using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace CheatSheet.UI
{
	internal class UICheckbox : UIView
	{
		internal static Asset<Texture2D> checkboxTexture;
		internal static Asset<Texture2D> checkmarkTexture;

		private bool selected = false;

		public bool Selected
		{
			get { return selected; }
			set
			{
				if (value != selected)
				{
					selected = value;
					if (SelectedChanged != null)
						SelectedChanged(this, EventArgs.Empty);
				}
			}
		}

		public string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		public event EventHandler SelectedChanged;

		private const int spacing = 4;

		internal UILabel label;

		public UICheckbox(string text)
		{
			checkboxTexture = CheatSheet.instance.Assets.Request<Texture2D>("UI/checkBox", AssetRequestMode.ImmediateLoad);
			checkmarkTexture = CheatSheet.instance.Assets.Request<Texture2D>("UI/checkMark", AssetRequestMode.ImmediateLoad);

			label = new UILabel(text);
			label.Scale = .5f;
			label.Position = new Vector2(checkboxTexture.Width() + spacing, 0);
			this.AddChild(label);
			this.onLeftClick += new EventHandler(UICheckbox_onLeftClick);
		}

		private void UICheckbox_onLeftClick(object sender, EventArgs e)
		{
			this.Selected = !Selected;
		}

		protected override float GetHeight()
		{
			return label.Height;
		}

		protected override float GetWidth()
		{
			return checkboxTexture.Width() + spacing + label.Width;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Vector2 pos = DrawPosition + new Vector2(0, (float)label.Height / 2 - (float)checkboxTexture.Height() / 1.2f);
			spriteBatch.Draw(checkboxTexture.Value, pos, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);
			if (Selected)
				spriteBatch.Draw(checkboxTexture.Value, pos, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);

			base.Draw(spriteBatch);
		}
	}
}