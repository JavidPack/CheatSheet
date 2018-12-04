using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace CheatSheet.UI
{
	internal class UIView
	{
		public delegate void ClickEventHandler(object sender, byte button);

		//internal static Texture2D closeTexture;

		//public static UIView exclusiveControl = null;

		private static bool gameMouseOverwritten = false;

		protected static bool MouseLeftButton = false;

		protected static bool MousePrevLeftButton = false;

		protected static bool MouseRightButton = false;

		protected static bool MousePrevRightButton = false;

		public static int ScrollAmount = 0;

		public static string HoverText = "";

		//public static Item HoverItem = new Item();

		//protected static readonly Item EmptyItem = new Item();

		public static bool HoverOverridden = false;

		private Vector2 _position = Vector2.Zero;

		protected bool mouseForChildrenHandled;

		public List<UIView> children = new List<UIView>();

		internal List<UIView> childrenToRemove = new List<UIView>();

		private static bool mouseUpHandled = false;

		private static bool mouseDownHandled = false;

		protected bool leftButtonDown;

		protected bool rightButtonDown;

		private bool mousePreviouslyIn;
		private string _tooltip = "";

		private bool _updateWhenOutOfBounds;

		private float width;

		private float height;

		public event EventHandler onHover;

		public event EventHandler onLeftClick;

		public event EventHandler onRightClick;

		public event EventHandler onMouseEnter;

		public event EventHandler onMouseLeave;

		public event UIView.ClickEventHandler onMouseDown;

		public event UIView.ClickEventHandler onMouseUp;

		protected static int MouseX => Main.mouseX;

		protected static int MouseY
		{
			get
			{
				return Main.mouseY;
			}
		}

		protected static Texture2D dummyTexture
		{
			get
			{
				return null;// Mod.dummyTexture;
			}
		}

		protected static GraphicsDevice graphics
		{
			get
			{
				return Main.graphics.GraphicsDevice;
			}
		}

		public Vector2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		public float X
		{
			get
			{
				return this._position.X;
			}
			set
			{
				this._position.X = value;
			}
		}

		public float Y
		{
			get
			{
				return this._position.Y;
			}
			set
			{
				this._position.Y = value;
			}
		}

		public Vector2 DrawPosition
		{
			get
			{
				if (this.Parent != null)
				{
					return this.Parent.DrawPosition + this.Position + this.Offset - this.Parent.Origin;
				}
				return this.Position + this.Offset;
			}
		}

		public float Width
		{
			get
			{
				return this.GetWidth();
			}
			set
			{
				this.SetWidth(value);
			}
		}

		public float Height
		{
			get
			{
				return this.GetHeight();
			}
			set
			{
				this.SetHeight(value);
			}
		}

		public UIView Parent
		{
			get;
			set;
		}

		public bool MouseInside
		{
			get
			{
				return this.IsMouseInside();
			}
		}

		public int ChildCount
		{
			get
			{
				return this.children.Count;
			}
		}

		public Color ForegroundColor { get; set; } = Color.White;

		public Color BackgroundColor { get; set; } = Color.White;

		public AnchorPosition Anchor { get; set; } = AnchorPosition.TopLeft;

		public Vector2 Origin
		{
			get
			{
				return this.GetOrigin();
			}
		}

		public Vector2 Offset { get; set; } = Vector2.Zero;

		public float Scale { get; set; } = 1f;

		public float Opacity { get; set; } = 1f;

		public bool Visible { get; set; } = true;

		public bool OverridesMouse { get; set; } = true;

		public string Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value.Length > 0 && this._tooltip.Length == 0)
				{
					this.onHover += new EventHandler(this.DisplayTooltip);
				}
				else if (value.Length == 0 && this._tooltip.Length > 0)
				{
					this.onHover -= new EventHandler(this.DisplayTooltip);
				}
				this._tooltip = value;
			}
		}

		public bool UpdateWhenOutOfBounds
		{
			get
			{
				return this._updateWhenOutOfBounds;
			}
			set
			{
				this._updateWhenOutOfBounds = value;
			}
		}

		public object Tag
		{
			get;
			set;
		}

		public virtual void Update()
		{
			if (this.Parent == null)
			{
				UIView.mouseDownHandled = false;
				UIView.mouseUpHandled = false;
				UIView.gameMouseOverwritten = false;
			}
			this.mouseForChildrenHandled = false;
			if (this.Visible)
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					UIView uIView = this.children[this.children.Count - 1 - i];
					if (uIView.UpdateWhenOutOfBounds || uIView.InParent())
					{
						this.children[this.children.Count - 1 - i].Update();
					}
				}
				while (this.childrenToRemove.Count > 0)
				{
					this.children.Remove(this.childrenToRemove[0]);
					this.childrenToRemove.RemoveAt(0);
				}
				if ((/*UIView.exclusiveControl == null &&*/ this.Parent == null) /*|| this == UIView.exclusiveControl*/)
				{
					this.HandleMouseInput();
				}
			}
		}

		private void DisplayTooltip(object sender, EventArgs e)
		{
			UIView.HoverText = ((UIView)sender).Tooltip;
		}

		private void OverWriteGameMouseInput()
		{
			UIView.gameMouseOverwritten = true;
			Main.mouseLeft = false;
			Main.mouseLeftRelease = false;
			Main.mouseRight = false;
			Main.mouseLeft = false;
			//Main.oldMouseState = Main.mouseState; // TODO
			UIView.HoverOverridden = true;
		}

		private bool InParent()
		{
			float num = this.Parent.Height;
			return (this.Position.Y + this.Offset.Y >= 0f || this.Position.Y + this.Offset.Y + this.Height >= 0f) && (this.Position.Y + this.Offset.Y <= num || this.Position.Y + this.Offset.Y + this.Height <= num);
		}

		private void HandleMouseInput()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				UIView uIView = this.children[this.children.Count - 1 - i];
				if (uIView.Visible && (uIView.Parent == null || uIView.UpdateWhenOutOfBounds || (uIView.InParent() && (uIView.Parent.MouseInside || uIView.Parent.UpdateWhenOutOfBounds) && !uIView.Parent.mouseForChildrenHandled)))
				{
					uIView.HandleMouseInput();
				}
			}
			if (this.MouseInside)
			{
				EventHandler arg_8F_0 = this.onMouseLeave;
				if (this.Parent != null)
				{
					this.Parent.mouseForChildrenHandled = true;
					if (this.OverridesMouse)
					{
						this.OverWriteGameMouseInput();
					}
				}
				if (this.onHover != null)
				{
					this.onHover(this, new EventArgs());
				}
				if (!this.mousePreviouslyIn && this.onMouseEnter != null)
				{
					this.onMouseEnter(this, new EventArgs());
				}
				if (!UIView.MousePrevLeftButton && UIView.MouseLeftButton)
				{
					this.leftButtonDown = true;
					if (this.onMouseDown != null && !UIView.mouseDownHandled)
					{
						this.onMouseDown(this, 0);
					}
				}
				if (UIView.MousePrevLeftButton && !UIView.MouseLeftButton)
				{
					if (this.onMouseUp != null && !UIView.mouseUpHandled)
					{
						this.onMouseUp(this, 0);
					}
					if (this.leftButtonDown && this.onLeftClick != null)
					{
						this.onLeftClick(this, EventArgs.Empty);
					}
				}
				if (!UIView.MousePrevRightButton && UIView.MouseRightButton)
				{
					this.rightButtonDown = true;
					if (this.onMouseDown != null)
					{
						this.onMouseDown(this, 1);
					}
				}
				if (UIView.MousePrevRightButton && !UIView.MouseRightButton)
				{
					if (this.onMouseUp != null)
					{
						this.onMouseUp(this, 1);
					}
					if (this.rightButtonDown && this.onRightClick != null)
					{
						this.onRightClick(this, EventArgs.Empty);
					}
				}
				this.mousePreviouslyIn = true;
			}
			else
			{
				EventHandler arg_1E5_0 = this.onMouseLeave;
				if (this.mousePreviouslyIn && this.onMouseLeave != null)
				{
					this.onMouseLeave(this, new EventArgs());
				}
				this.mousePreviouslyIn = false;
			}
			if (!UIView.MouseLeftButton)
			{
				this.leftButtonDown = false;
			}
			if (!UIView.MouseRightButton)
			{
				this.rightButtonDown = false;
			}
		}

		protected virtual void SetWidth(float width)
		{
			this.width = width;
		}

		protected virtual void SetHeight(float height)
		{
			this.height = height;
		}

		protected virtual float GetWidth()
		{
			return this.width;
		}

		protected virtual float GetHeight()
		{
			return this.height;
		}

		protected virtual Vector2 GetOrigin()
		{
			float x = this.Width / 2f;
			float y = this.Height / 2f;
			if (this.Anchor == AnchorPosition.TopLeft)
			{
				return Vector2.Zero;
			}
			if (this.Anchor == AnchorPosition.Left)
			{
				return new Vector2(0f, y);
			}
			if (this.Anchor == AnchorPosition.Right)
			{
				return new Vector2(this.Width, y);
			}
			if (this.Anchor == AnchorPosition.Top)
			{
				return new Vector2(x, 0f);
			}
			if (this.Anchor == AnchorPosition.Bottom)
			{
				return new Vector2(x, this.Height);
			}
			if (this.Anchor == AnchorPosition.Center)
			{
				return new Vector2(x, y);
			}
			if (this.Anchor == AnchorPosition.TopRight)
			{
				return new Vector2(this.Width, 0f);
			}
			if (this.Anchor == AnchorPosition.BottomLeft)
			{
				return new Vector2(0f, this.Height);
			}
			if (this.Anchor == AnchorPosition.BottomRight)
			{
				return new Vector2(this.Width, this.Height);
			}
			return Vector2.Zero;
		}

		protected virtual bool IsMouseInside()
		{
			Vector2 vector = this.DrawPosition - this.Origin;
			return (float)UIView.MouseX >= vector.X && (float)UIView.MouseX <= vector.X + this.Width && (float)UIView.MouseY >= vector.Y && (float)UIView.MouseY <= vector.Y + this.Height;
		}

		protected virtual Vector2 GetParentCenter()
		{
			float num = (float)Main.screenWidth;
			float num2 = (float)Main.screenHeight;
			if (this.Parent != null)
			{
				num = this.Parent.Width;
				num2 = this.Parent.Height;
			}
			return new Vector2(num / 2f, num2 / 2f);
		}

		public void CenterToParent()
		{
			this.Position = this.GetParentCenter();
		}

		public void CenterXAxisToParentCenter(int offset = 0)
		{
			this.Position = new Vector2(this.GetParentCenter().X - 40 + offset, this.Position.Y);
		}

		public void CenterYAxisToParentCenter(int offset = 0)
		{
			this.Position = new Vector2(this.Position.X, this.GetParentCenter().Y + offset);
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (this.Visible)
			{
				int childCount = this.ChildCount;
				for (int i = 0; i < this.ChildCount; i++)
				{
					if (this.ChildCount != childCount)
					{
						return;
					}
					UIView uIView = this.children[i];
					if ((uIView.UpdateWhenOutOfBounds || uIView.InParent()) && uIView.Visible)
					{
						uIView.Draw(spriteBatch);
					}
				}
				// Debug Draw UIView area.
				//if(Parent != null)
				//{
				//	spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(Position.X + Parent.X), (int)(Position.Y + Parent.Y), (int)Width, (int)Height), foregroundColor * 0.6f);
				//}
			}
		}

		public UIView GetChild(int index)
		{
			return this.children[index];
		}

		public UIView GetLastChild()
		{
			return this.children[this.children.Count - 1];
		}

		public virtual void AddChild(UIView view)
		{
			view.Parent = this;
			view.onMouseDown += new UIView.ClickEventHandler(this.view_onMouseDown);
			view.onMouseUp += new UIView.ClickEventHandler(this.view_onMouseUp);
			this.children.Add(view);
		}

		public virtual void AddChild(UIView view, bool defaultMouseUpDownHandle)
		{
			view.Parent = this;
			if (defaultMouseUpDownHandle)
			{
				view.onMouseDown += new UIView.ClickEventHandler(this.view_onMouseDown);
				view.onMouseUp += new UIView.ClickEventHandler(this.view_onMouseUp);
			}
			this.children.Add(view);
		}

		public void RemoveAllChildren()
		{
			this.children.Clear();
		}

		private void view_onMouseUp(object sender, byte button)
		{
			UIView.mouseUpHandled = true;
		}

		private void view_onMouseDown(object sender, byte button)
		{
			UIView.mouseDownHandled = true;
		}

		public void RemoveChild(UIView view)
		{
			this.childrenToRemove.Add(view);
		}

		public UIView()
		{
			//closeTexture = CheatSheet.instance.GetTexture("UI/closeButton");
		}

		//public static Texture2D GetEmbeddedTexture(string name)
		//{
		//	Assembly executingAssembly = Assembly.GetExecutingAssembly();
		//	Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("Terraria." + name);
		//	if (manifestResourceStream != null)
		//	{
		//		return Texture2D.FromStream(UIView.graphics, manifestResourceStream);
		//	}
		//	return null;
		//}
	}
}