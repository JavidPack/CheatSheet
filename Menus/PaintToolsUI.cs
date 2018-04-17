using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal class PaintToolsUI : UISlideWindow
	{
		internal static string CSText(string key, string category = "PaintTools") => CheatSheet.CSText(category, key);
		internal UIImageListButton btnSnap;

		internal UIView infoPanel;
		internal UILabel infoMessage;
		internal UIImage upVoteButton;
		internal UIImage downVoteButton;

		internal UIView submitPanel;
		internal UILabel submitLabel;
		internal UITextbox submitInput;
		internal UIImage submitButton;

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
					CSText("SnapTopLeft"), CSText("SnapTopCenter"), CSText("SnapTopRight"),
					CSText("SnapLeftCenter"), CSText("SnapCenter"), CSText("SnapRightCenter"),
					CSText("SnapBottomLeft"), CSText("SnapBottomCenter"), CSText("SnapBottomRight"),
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
			uIImage.Tooltip = CSText("DeleteSelection");
			this.AddChild(uIImage);

			uIImage = new UIImage(Main.itemTexture[ItemID.AlphabetStatueI]);
			position = position.Offset(uIImage.Width + this.spacing, 0);
			uIImage.Position = position;
			uIImage.onLeftClick += (a, b) => PaintToolsEx.Import(this.view);
			uIImage.Tooltip = CSText("ImportData");
			this.AddChild(uIImage);

			uIImage = new UIImage(Main.itemTexture[ItemID.AlphabetStatueE]);
			position = position.Offset(uIImage.Width + this.spacing, 0);
			uIImage.Position = position;
			uIImage.onLeftClick += (a, b) => PaintToolsEx.Export(this.view);
			uIImage.Tooltip = CSText("ExportData");
			this.AddChild(uIImage);

			uIImage = new UIImage(Main.itemTexture[ItemID.AlphabetStatueW]);
			position = position.Offset(uIImage.Width + this.spacing, 0);
			uIImage.Position = position;
			uIImage.onLeftClick += (a, b) => PaintToolsEx.OnlineImport(this.view);
			uIImage.Tooltip = "Load Online Schematics Database";
			this.AddChild(uIImage);

			infoPanel = new UIView();
			position = position.Offset(uIImage.Width + this.spacing, 0);
			infoPanel.Position = position;
			infoPanel.Y = 6;
			infoPanel.Width = 210;
			infoPanel.Height = 44;
			infoPanel.ForegroundColor = Color.Thistle;
			AddChild(infoPanel);

			infoMessage = new UILabel("Message Here");
			infoMessage.Scale = 0.35f;
			infoMessage.Position = new Vector2(30, 10);
			infoPanel.AddChild(infoMessage);

			upVoteButton = new UIImage(CheatSheet.instance.GetTexture("UI/VoteUp"));
			upVoteButton.Position = new Vector2(0, 0);
			upVoteButton.onLeftClick += (a, b) => Vote(true);
			upVoteButton.Tooltip = "Vote Up";
			infoPanel.AddChild(upVoteButton);

			downVoteButton = new UIImage(CheatSheet.instance.GetTexture("UI/VoteDown"));
			downVoteButton.Position = new Vector2(0, 24);
			downVoteButton.onLeftClick += (a, b) => Vote(false);
			downVoteButton.Tooltip = "Vote Down";
			infoPanel.AddChild(downVoteButton);

			infoPanel.Visible = false;

			submitPanel = new UIView();
			submitPanel.Position = position;
			submitPanel.Y = 6;
			submitPanel.Width = 210;
			submitPanel.Height = 44;
			AddChild(submitPanel);

			submitLabel = new UILabel("Submit Name:");
			submitLabel.Scale = 0.35f;
			submitLabel.Position = new Vector2(0, 0);
			submitPanel.AddChild(submitLabel);

			submitInput = new UITextbox();
			submitInput.Position = new Vector2(0, 20);
			submitInput.Width = 200;
			submitPanel.AddChild(submitInput);

			submitButton = new UIImage(Terraria.Graphics.TextureManager.Load("Images/UI/ButtonCloudActive"));
			submitButton.Position = new Vector2(178, -2);
			submitButton.onLeftClick += (a, b) => Submit();
			submitButton.Tooltip = "Submit to Schematics Browser";
			submitPanel.AddChild(submitButton);

			submitPanel.Visible = false;
		}

		private static Uri submiturl = new Uri("http://javid.ddns.net/tModLoader/jopojellymods/CheatSheet_Schematics_Submit.php");
		private static bool submitWait = false;
		private void Submit()
		{
			if (PaintToolsSlot.CurrentSelect == null)
				return;
			if (PaintToolsSlot.CurrentSelect.browserID == -1)
			{
				Main.NewText("Already submitted.");
				return;
			}
			if (PaintToolsSlot.CurrentSelect.browserID != 0)
				return;
			if (submitWait)
			{
				Main.NewText("Be patient.");
				return;
			}
			try
			{
				using (WebClient client = new WebClient())
				{
					var steamIDMethodInfo = typeof(Main).Assembly.GetType("Terraria.ModLoader.ModLoader").GetProperty("SteamID64", BindingFlags.Static | BindingFlags.NonPublic);
					string steamid64 = (string)steamIDMethodInfo.GetValue(null, null);
					string base64tiles = PaintToolsEx.SaveTilesToBase64(PaintToolsSlot.CurrentSelect.stampInfo.Tiles);
					if (string.IsNullOrEmpty(base64tiles))
					{
						Main.NewText("Oops, base64tiles is bad.");
					}
					else if (string.IsNullOrEmpty(submitInput.Text))
					{
						Main.NewText("Please name your creation.");
					}
					else if (base64tiles.Length > 5000)
					{
						Main.NewText("Selection too big for now.");
					}
					else
					{
						submitWait = true;
						var values = new NameValueCollection
						{
							{ "version", CheatSheet.instance.Version.ToString() },
							{ "steamid64", steamid64 },
							{ "name", submitInput.Text },
							{ "tiledata", base64tiles },
						};
						PaintToolsSlot.CurrentSelect.browserID = -1;
						CheatSheet.instance.paintToolsUI.submitPanel.Visible = false;
						client.UploadValuesCompleted += new UploadValuesCompletedEventHandler(SubmitComplete);
						client.UploadValuesAsync(submiturl, "POST", values);
					}
				}
			}
			catch
			{
				Main.NewText("Schematics Server problem 5");
				submitWait = false;
			}
		}

		private void SubmitComplete(object sender, UploadValuesCompletedEventArgs e)
		{
			try
			{
				if (!e.Cancelled)
				{
					string response = Encoding.UTF8.GetString(e.Result);
					JObject jsonObject = JObject.Parse(response);
					if (jsonObject["message"] != null)
					{
						Main.NewText((string)jsonObject["message"]);
					}
					else
					{
						Main.NewText("Schematics Server problem 12");
					}
				}
				else
				{
					Main.NewText("Schematics Server problem 2");
				}
			}
			catch
			{
				Main.NewText("Schematics Server problem 3");
			}
			submitWait = false;
		}

		private static Uri voteurl = new Uri("http://javid.ddns.net/tModLoader/jopojellymods/CheatSheet_Schematics_Vote.php");

		private void Vote(bool up)
		{
			if (PaintToolsSlot.CurrentSelect == null)
				return;

			int voteInt = up ? 1 : -1;
			if (PaintToolsSlot.CurrentSelect.vote == voteInt)
			{
				Main.NewText("You already voted");
				return;
			}

			PaintToolsSlot.CurrentSelect.rating += voteInt - PaintToolsSlot.CurrentSelect.vote;
			CheatSheet.instance.paintToolsUI.infoMessage.Text = PaintToolsSlot.CurrentSelect.browserName + ": " + PaintToolsSlot.CurrentSelect.rating;
			PaintToolsSlot.CurrentSelect.vote = voteInt;
			CheatSheet.instance.paintToolsUI.upVoteButton.ForegroundColor = Color.White;
			CheatSheet.instance.paintToolsUI.downVoteButton.ForegroundColor = Color.White;
			if (PaintToolsSlot.CurrentSelect.vote == 1)
				CheatSheet.instance.paintToolsUI.upVoteButton.ForegroundColor = Color.Gray;
			if (PaintToolsSlot.CurrentSelect.vote == -1)
				CheatSheet.instance.paintToolsUI.downVoteButton.ForegroundColor = Color.Gray;
			try
			{
				using (WebClient client = new WebClient())
				{
					var steamIDMethodInfo = typeof(Main).Assembly.GetType("Terraria.ModLoader.ModLoader").GetProperty("SteamID64", BindingFlags.Static | BindingFlags.NonPublic);
					string steamid64 = (string)steamIDMethodInfo.GetValue(null, null);
					var values = new NameValueCollection
					{
						{ "version", CheatSheet.instance.Version.ToString() },
						{ "steamid64", steamid64 },
						{ "id", PaintToolsSlot.CurrentSelect.browserID.ToString() },
						{ "vote", up ? "1" : "-1" },
					};
					client.UploadValuesCompleted += new UploadValuesCompletedEventHandler(VoteComplete);
					client.UploadValuesAsync(voteurl, "POST", values);
				}
			}
			catch
			{
			}
		}

		internal static void VoteComplete(object sender, UploadValuesCompletedEventArgs e)
		{
			if (!e.Cancelled)
			{
				Main.NewText("Thanks for voting.");
			}
			else
			{
				Main.NewText("Schematics Server voting problem");
			}
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