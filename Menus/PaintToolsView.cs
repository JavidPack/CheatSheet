using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace CheatSheet.Menus
{
	internal class PaintToolsView : UIScrollView
	{
		private float spacing = 8f;
		private int slotSpace = 4;
		private int slotColumns = 4;
		private int slotRows = 2;
		internal List<PaintToolsSlot> slotList = new List<PaintToolsSlot>();

		public PaintToolsView()
		{
			base.Width = (PaintToolsSlot.slotSize + (float)this.slotSpace) * (float)this.slotColumns + (float)this.slotSpace + 20f;
			base.Height = (PaintToolsSlot.slotSize + (float)this.slotSpace) * (float)this.slotRows + (float)this.slotSpace + 20f;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}

		public int Count
		{
			get { return slotList.Count; }
		}

		public void Add(PaintToolsSlot slot)
		{
			slotList.Insert(0, slot);
			slot.Select();
			ReorderSlots();
		}

		public void AddEndDontSelect(PaintToolsSlot slot)
		{
			slotList.Add(slot);
			//slot.Select();
			ReorderSlots();
		}

		public void Add(PaintToolsSlot[] slots)
		{
			slotList.InsertRange(0, slots);
			slotList[0].Select();
			ReorderSlots();
		}

		public void RemoveSelectedItem()
		{
			if (PaintToolsSlot.CurrentSelect != null)
			{
				int index = slotList.IndexOf(PaintToolsSlot.CurrentSelect);
				slotList.Remove(PaintToolsSlot.CurrentSelect);
				PaintToolsSlot.CurrentSelect = null;
				CheatSheet.instance.paintToolsHotbar.StampTiles = new Tile[0, 0];
				CheatSheet.instance.paintToolsHotbar.stampInfo = null;
				CheatSheet.instance.paintToolsUI.infoPanel.Visible = false;
				CheatSheet.instance.paintToolsUI.submitPanel.Visible = false;
				if (slotList.Count > 0 && index > -1)
					slotList[index >= slotList.Count ? index - 1 : index].Select();
				ReorderSlots();
			}
		}

		public void RemoveAllOnline()
		{
			slotList.RemoveAll(x => x.browserID > 0);
			if (!slotList.Contains(PaintToolsSlot.CurrentSelect))
			{
				PaintToolsSlot.CurrentSelect = null;
				CheatSheet.instance.paintToolsHotbar.StampTiles = new Tile[0, 0];
				CheatSheet.instance.paintToolsHotbar.stampInfo = null;
				CheatSheet.instance.paintToolsUI.infoPanel.Visible = false;
				CheatSheet.instance.paintToolsUI.submitPanel.Visible = false;
			}
			ReorderSlots();
		}

		public void ReorderSlots()
		{
			base.ScrollPosition = 0f;
			base.ClearContent();
			for (int i = 0; i < slotList.Count; i++)
			{
				int num = i;
				PaintToolsSlot slot = this.slotList[num];
				int num2 = i % this.slotColumns;
				int num3 = i / this.slotColumns;
				float x = (float)this.slotSpace + (float)num2 * (slot.Width + (float)this.slotSpace);
				float y = (float)this.slotSpace + (float)num3 * (slot.Height + (float)this.slotSpace);
				slot.Position = new Vector2(x, y);
				slot.Offset = Vector2.Zero;
				this.AddChild(slot);
			}
			base.ContentHeight = base.GetLastChild().Y + base.GetLastChild().Height + this.spacing;
		}
	}
}