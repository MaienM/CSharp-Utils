using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpUtils.Extensions.ListViewExtensions
{
    /// <summary>
    /// An extension for ListView controls that add support for drag drop reordering of it's contents.
    /// </summary>
    public class ListViewReorderer : Component
    {
        /// <summary>
        /// The ListView this reorderer belongs to.
        /// </summary>
        public ListView View {
            get
            {
                return _view;
            }
            set {
                if (_view != null)
                {
                    _view.ItemDrag -= OnItemDrag;
                    _view.DragEnter -= OnDragEnter;
                    _view.DragDrop -= OnDragDrop;
                }
                _view = value;
                if (_view != null)
                {
                    _view.ItemDrag += OnItemDrag;
                    _view.DragEnter += OnDragEnter;
                    _view.DragDrop += OnDragDrop;
                }
            }
        }
        private ListView _view;

        /// <summary>
        /// This event is thrown when the list is reordered through drag and drop.
        /// </summary>
        public event ReorderEventHandler Reorder;

        /// <summary>
        /// The delegate for handling a Reorder event.
        /// </summary>
        /// <param name="sender">The source control</param>
        /// <param name="e">The event</param>
        public delegate void ReorderEventHandler(object sender, ListViewReorderEventArgs e);

        /// <summary>
        /// On drag start, set the correct effect.
        /// </summary>
        private void OnItemDrag(object sender, ItemDragEventArgs e)
        {
            View.DoDragDrop(e.Item, DragDropEffects.Link);
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
            Point targetPoint = View.PointToClient(new Point(e.X, e.Y));
            ListViewItem targetItem = View.GetItemAt(targetPoint.X, targetPoint.Y);

            // Not dragged onto another item, so first or last it is.
            if (targetItem == null)
            {
                // First.
                if (View.Items[0].Position.Y > targetPoint.Y)
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
                index = View.Items.Count + 1 + index;
            }

            // Build the event.
            ListViewReorderEventArgs e = new ListViewReorderEventArgs();
            e.Index = index;

            // Move the items.
            foreach (ListViewItem item in View.SelectedItems)
            {
                if (item.Index < index)
                {
                    index--;
                }
                View.Items.Remove(item);
                View.Items.Insert(index++, item);
                e.Items.Add(item);
            }

            // Trigger the event.
            if (Reorder != null) {
                Reorder(View, e);
            }
        }
    }

    /// <summary>
    /// Event arguments for a ListViewEx.Reorder event.
    /// </summary>
    public class ListViewReorderEventArgs : EventArgs
    {
        /// <summary>
        /// The items that were moved.
        /// </summary>
        public List<ListViewItem> Items = new List<ListViewItem>();

        /// <summary>
        /// The index the items were moved to.
        /// </summary>
        public int Index;
    }
}
