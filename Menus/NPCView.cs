using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace CheatSheet.Menus
{
	internal class NPCView : UIScrollView
	{
		private float spacing = 8f;

		public NPCSlot[] allNPCSlot;

		public NPCSlot[] negativeNPCSlots;

		private int[] _selectedCategory;

		public int[] activeSlots;

		private int slotSpace = 4;

		private int slotColumns = 8;

		public int negativeSlots = 65; // number of netIDs < 0

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
					NPCSlot slot = this.allNPCSlot[list[i]];
					if (slot.npcType == 0)
					{
						list.RemoveAt(i);
						i--;
					}
				}
				this._selectedCategory = list.ToArray();
			}
		}

		public NPCView()
		{
			base.Width = (this.slotSize + (float)this.slotSpace) * (float)this.slotColumns + (float)this.slotSpace + 20f;
			base.Height = 200f;
			this.allNPCSlot = new NPCSlot[Main.npcTexture.Length + negativeSlots];
			for (int i = 0; i < this.allNPCSlot.Length; i++)
			{
				int type = (i >= this.allNPCSlot.Length - negativeSlots) ? -(i - this.allNPCSlot.Length + negativeSlots) : i;
				this.allNPCSlot[i] = new NPCSlot(type, i);
			}
			//	this.allNPCSlot = (from s in this.allNPCSlot
			//					   select s).ToArray<NPCSlot>();
		}

		/*	this.allNPCSlot = new NPCSlot[Main.npcTexture.Length + 65];
			int index = 0;
			for (int i = -65; i < Main.npcTexture.Length; i++)
			{
				//if (i == 0) continue;
				this.allNPCSlot[index] = new NPCSlot(i);
				index++;
			}*/

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
				NPCSlot slot = this.allNPCSlot[this.activeSlots[num]];
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