using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using CheatSheet.UI;

namespace CheatSheet.Menus
{
	internal class ItemView : UIScrollView
	{
		private float spacing = 8f;

		public Slot[] allItemsSlots;

		private int[] _selectedCategory;

		public int[] activeSlots;

		private int slotSpace = 4;

		private int slotColumns = 10;

		private float slotSize = (float)Slot.backgroundTexture.Width * 0.85f;

		private int slotRows = 6;

		public int[] selectedCategory
		{
			get
			{
				return this._selectedCategory;
			}
			set
			{
				List<int> list = value.ToList<int>();
				for (int i = 0; i < list.Count; i++)
				{
					Slot slot = this.allItemsSlots[list[i]];
					if (slot.item.type == 0 || slot.item.toolTip == "You shouldn't have this")
					{
						list.RemoveAt(i);
						i--;
					}
				}
				this._selectedCategory = list.ToArray();
			}
		}

		public ItemView()
		{
			base.Width = (this.slotSize + (float)this.slotSpace) * (float)this.slotColumns + (float)this.slotSpace + 20f;
			base.Height = 300f;
			this.allItemsSlots = new Slot[Main.itemTexture.Length];
			for (int i = 0; i < this.allItemsSlots.Length; i++)
			{
				this.allItemsSlots[i] = new Slot(i);
			}
		//	this.allItemsSlots = (from s in this.allItemsSlots
		//						  select s).ToArray<Slot>();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}

		public void ReorderSlots()
		{
			base.ScrollPosition = 0f;
			base.ClearContent();
			for (int i = 0; i < this.activeSlots.Length; i++)
			{
				int num = i;
				Slot slot = this.allItemsSlots[this.activeSlots[num]];
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
