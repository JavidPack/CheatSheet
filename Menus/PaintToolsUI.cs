using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
    internal class PaintToolsUI : UISlideWindow
	{
        internal UIImageListButton btnSnap;

        public PaintToolsView view;
        public Mod mod;

        public static List<List<int>> categories = new List<List<int>>();
        private static Color buttonColor = new Color(190, 190, 190);

        private static Color buttonSelectedColor = new Color(209, 142, 13);

        private float spacing = 16f;
        private int menuIconSize = 28;
        private int menuMargin = 4;

        public int lastModNameNumber = 0;

        public static List<int> filteredNPCSlots = new List<int>();

        internal static bool needsUpdate = true;

        public PaintToolsUI(Mod mod)
        {
            categories.Clear();
            this.view = new PaintToolsView();
            this.mod = mod;
            this.CanMove = true;
            base.Width = this.view.Width + this.spacing * 2f;
            base.Height = 35f + this.view.Height + this.spacing * 2f;
            this.view.Position = new Vector2(this.spacing, 55f);
            this.AddChild(this.view);

            Texture2D texture = mod.GetTexture("UI/closeButton");
            UIImage uIImage = new UIImage(texture);
            uIImage.Anchor = AnchorPosition.TopRight;
            uIImage.Position = new Vector2(base.Width - this.spacing, this.spacing);
            uIImage.onLeftClick += new EventHandler(this.bClose_onLeftClick);
            this.AddChild(uIImage);

            var snaptexture = mod.GetTexture("UI/Snap");
            btnSnap = new UIImageListButton(
                (new ImageList(mod.GetTexture("UI/Snap"), 28, 28)).listTexture,
                new List<object>() {
                    SnapType.TopLeft, SnapType.TopCenter, SnapType.TopRight,
                    SnapType.LeftCenter, SnapType.Center, SnapType.RightCenter,
                    SnapType.BottomLeft, SnapType.BottomCenter, SnapType.BottomRight,
                },
                new List<string>() {
                    "Snap: TopLeft", "Snap: TopCenter", "Snap: TopRight",
                    "Snap: LeftCenter", "Snap: Center", "Snap: RightCenter",
                    "Snap: BottomLeft", "Snap: BottomCenter", "Snap: BottomRight",
                },
                4);
            btnSnap.onLeftClick += (a, b) => btnSnap.NextIamge();
            btnSnap.onRightClick += (a, b) => btnSnap.PrevIamge();
            btnSnap.Position = new Vector2(this.spacing, this.spacing);
            this.AddChild(btnSnap);

            var position = btnSnap.Position;

            uIImage = new UIImage(Main.itemTexture[ItemID.TrashCan]);
            position = position.Offset(btnSnap.Width + this.spacing, 0);
            uIImage.Position = position;
            uIImage.onLeftClick += (a, b) => view.RemoveSelectedItem();
            uIImage.Tooltip = "Delete selection";
            this.AddChild(uIImage);

            uIImage = new UIImage(Main.itemTexture[ItemID.AlphabetStatueI]);
            position = position.Offset(uIImage.Width + this.spacing, 0);
            uIImage.Position = position;
            uIImage.onLeftClick += (a, b) => PaintToolsEx.Import(this.view);
            uIImage.Tooltip = "Import data";
            this.AddChild(uIImage);

            uIImage = new UIImage(Main.itemTexture[ItemID.AlphabetStatueE]);
            position = position.Offset(uIImage.Width + this.spacing, 0);
            uIImage.Position = position;
            uIImage.onLeftClick += (a, b) => PaintToolsEx.Export(this.view);
            uIImage.Tooltip = "Export data";
            this.AddChild(uIImage);
        }

        public SnapType SnapType
        {
            get
            {
                return btnSnap.GetValue<SnapType>();
            }
        }

        public void AddSlot(StampInfo stampInfo)
        {
            view.Add(new PaintToolsSlot(stampInfo));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Visible && IsMouseInside())
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.LocalPlayer.showItemIcon = false;
            }
        }

        public override void Update()
        {
            //if (needsUpdate)
            //{
            //    foreach (var npcslot in npcView.allNPCSlot)
            //    {
            //        npcslot.isFiltered = filteredNPCSlots.Contains(npcslot.netID);
            //    }
            //    needsUpdate = false;
            //}
            //UIView.MousePrevLeftButton = UIView.MouseLeftButton;
            //UIView.MouseLeftButton = Main.mouseLeft;
            //UIView.MousePrevRightButton = UIView.MouseRightButton;
            //UIView.MouseRightButton = Main.mouseRight;
            //UIView.ScrollAmount = PlayerInput.ScrollWheelDeltaForUI;
            UIView.HoverItem = UIView.EmptyItem;
            UIView.HoverText = "";
            UIView.HoverOverridden = true;

            base.Update();
        }

        private void bClose_onLeftClick(object sender, EventArgs e)
        {
            Hide();
        }
    }

    internal enum SnapType
    {
        TopLeft,
        TopCenter,
        TopRight,
        LeftCenter,
        Center,
        RightCenter,
        BottomLeft,
        BottomCenter,
        BottomRight,
    };

    internal class Snap
    {
        public SnapType type;
        public Vector2 position;
        public int width;
        public int height;
        public bool resultTilePosition;
        public bool constrainToAxis;
        public int constrainedX;
        public int constrainedY;

        public static Vector2 GetSnapPosition(SnapType type, int width, int height, bool constrainToAxis, int constrainedX, int constrainedY, bool resultTilePosition)
        {
            Snap snap = new Snap();
            snap.type = type;
            snap.width = width;
            snap.height = height;
            snap.constrainToAxis = constrainToAxis;
            snap.constrainedX = constrainedX;
            snap.constrainedY = constrainedY;
            snap.resultTilePosition = resultTilePosition;
            return snap.GetSnapPosition();
        }

        public Vector2 GetSnapPosition()
        {
            Vector2 result = position;
            Vector2 evenOffset = Vector2.Zero;
            if (width % 2 == 0 && (type == SnapType.TopCenter || type == SnapType.Center || type == SnapType.BottomCenter))
            {
                evenOffset.X = 1;
            }
            if (height % 2 == 0 && (type == SnapType.LeftCenter || type == SnapType.Center || type == SnapType.RightCenter))
            {
                evenOffset.Y = 1;
            }
            position = (Main.MouseWorld + evenOffset * 8).ToTileCoordinates().ToVector2();
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            SnapType snapType = type;
            if (Main.LocalPlayer.gravDir == -1f)
            {
                switch (snapType)
                {
                    case SnapType.TopLeft:
                        snapType = SnapType.BottomLeft;
                        break;
                    case SnapType.TopCenter:
                        snapType = SnapType.BottomCenter;
                        break;
                    case SnapType.TopRight:
                        snapType = SnapType.BottomRight;
                        break;
                    case SnapType.BottomLeft:
                        snapType = SnapType.TopLeft;
                        break;
                    case SnapType.BottomCenter:
                        snapType = SnapType.TopCenter;
                        break;
                    case SnapType.BottomRight:
                        snapType = SnapType.TopRight;
                        break;
                }
            }
            switch (snapType)
            {
                case SnapType.TopLeft:
                    break;
                case SnapType.TopCenter:
                    position = position.Offset(-halfWidth, 0);
                    break;
                case SnapType.TopRight:
                    position = position.Offset(-width + 1, 0);
                    break;
                case SnapType.LeftCenter:
                    position = position.Offset(0, -halfHeight);
                    break;
                case SnapType.Center:
                    position = position.Offset(-halfWidth, -halfHeight);
                    break;
                case SnapType.RightCenter:
                    position = position.Offset(-width + 1, -halfHeight);
                    break;
                case SnapType.BottomLeft:
                    position = position.Offset(0, -height + 1);
                    break;
                case SnapType.BottomCenter:
                    position = position.Offset(-halfWidth, -height + 1);
                    break;
                case SnapType.BottomRight:
                    position = position.Offset(-width + 1, -height + 1);
                    break;
            }

            if (constrainToAxis)
            {
                if (constrainedX != -1)
                {
                    position.X = constrainedX;
                }
                if (constrainedY != -1)
                {
                    position.Y = constrainedY;
                }
            }

            if (!resultTilePosition)
            {
                position = (position * 16f) - Main.screenPosition;
                if (Main.LocalPlayer.gravDir == -1f)
                {
                    position.Y = (float)Main.screenHeight - position.Y;
                    position.Y -= height * 16;
                }
            }
            result = position;
            return result;
        }
    }
}