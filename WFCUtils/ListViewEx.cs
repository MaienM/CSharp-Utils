using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFCUtils
{
    /// <summary>
    /// An extended version of the default ListView control that supports drag drop reordering of it's contents.
    /// </summary>
    public partial class ListViewEx : ListView
    {
        /// <summary>
        /// Create a new ListViewEx control.
        /// </summary>
        public ListViewEx()
            : base()
        {
            ItemDrag += OnItemDrag;
            DragEnter += OnDragEnter;
            DragDrop += OnDragDrop;
        }

        /// <summary>
        /// On drag start, set the correct effect.
        /// </summary>
        private void OnItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Link);
        }

        /// <summary>
        /// On drag enter, set the correct effect.
        /// </summary>
        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        /// <summary>
        /// On drag end, reorder.
        /// 
        /// If dropped above all other items, move to top.
        /// If dragged onto list items, move to closest edge between items.
        /// If dropped below all other items, move to bottom.
        /// </summary>
        private void OnDragDrop(object sender, DragEventArgs e)
        {
            // Get the end of the drag.
            Point targetPoint = PointToClient(new Point(e.X, e.Y));
            ListViewItem targetItem = GetItemAt(targetPoint.X, targetPoint.Y);

            // Not dragged onto another item, so first or last it is.
            if (targetItem == null)
            {
                // First.
                if (Items[0].Position.Y > targetPoint.Y)
                {
                    MoveSelectedTo(0);
                }

                // Last.
                else
                {
                    MoveSelectedTo(-1);
                }
            }

            // Dragged onto another item, so we go above/below that item, depending on whether 
            // it was dropped on the top or bottom half.
            else
            {
                // Top half.
                if (targetItem.Position.Y + targetItem.Bounds.Height / 2 > targetPoint.Y)
                {
                    MoveSelectedTo(targetItem.Index);
                }
                else
                {
                    MoveSelectedTo(targetItem.Index + 1);
                }
            }
        }

        /// <summary>
        /// Move the selected items to the given index.
        /// </summary>
        /// <param name="index">The index to move the items to</param>
        private void MoveSelectedTo(int index)
        {
            // Take care of negative indexes.
            if (index < 0)
            {
                index = Items.Count + 1 + index;
            }

            // Move the items.
            foreach (ListViewItem item in SelectedItems)
            {
                if (item.Index < index)
                {
                    index--;
                }
                Items.Remove(item);
                Items.Insert(index++, item);
            }
        }
    }
}
