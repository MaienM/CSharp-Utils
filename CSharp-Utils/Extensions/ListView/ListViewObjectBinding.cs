using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFCUtils
{
    /// <summary>
    /// An extension for ListView controls that add support for drag drop reordering of it's contents.
    /// </summary>
    public class ListViewObjectBinding : Component
    {
        /// <summary>
        /// The ListView this reorderer belongs to.
        /// </summary>
        public ListView View
        {
            get
            {
                return _view;
            }
            set
            {
                if (_view != null)
                {
                    _view.ColumnClick -= OnColumnClick;
                }
                _view = value;
                if (_view != null)
                {
                    _view.ColumnClick += OnColumnClick;
                }
                Update();
            }
        }
        private ListView _view;

        /// <summary>
        /// The Collection that provides the data.
        /// </summary>
        public ICollection<Object> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                Update();
            }
        }
        private ICollection<Object> _data;

        /// <summary>
        /// The type of the data in the Data list.
        /// </summary>
        public Type DataType { get; set; }

        private void OnColumnClick(object sender, ColumnClickEventArgs e)
        {
            Console.WriteLine(e);
        }

        /// <summary>
        /// Update the listview with the data from the dataset.
        /// </summary>
        private void Update()
        {
            if (View == null || Data == null)
            {
                return;
            }

            // Make sure each data object has a ListViewItem.
            int index = 0;
            foreach (Object data in Data)
            {
                // Check if there is a ListViewItem.
                // If there is, move it.
                // If not, create one.
                ListViewItem item = View.Items[data.GetHashCode().ToString()];
                if (item != null) 
                {
                    View.Items.Remove(item);
                }
                else
                {
                    item = new ListViewItem();
                    item.Tag = data;
                    item.Name = data.GetHashCode().ToString();
                }
                View.Items.Insert(index++, item);

                // Update the item.
                UpdateItem(item);
            }

            // Remove the superfluous items.
            while (View.Items.Count > index)
            {
                View.Items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Update a ListViewItem.
        /// </summary>
        /// <param name="item">The item to update.</param>
        private void UpdateItem(ListViewItem item)
        {
            // Make sure each header has a corresponding subitem.
            int index = 0;
            foreach (ColumnHeader header in View.Columns)
            {
                // Check if the item exists in the subitem list.
                // If it does, move it.
                // If not, create one.
                System.Windows.Forms.ListViewItem.ListViewSubItem subItem = item.SubItems[header.Name];
                if (subItem != null)
                {
                    item.SubItems.Remove(subItem);
                }
                else
                {
                    subItem = new System.Windows.Forms.ListViewItem.ListViewSubItem(item, "");
                    subItem.Name = header.Name;
                }
                item.SubItems.Insert(index++, subItem);

                // Update the subitem.
                subItem.Text = DataType.GetProperty(header.Name).GetValue(item.Tag).ToString();
            }

            // Remove the superfluous items.
            while (item.SubItems.Count > index)
            {
                item.SubItems.RemoveAt(index);
            }
        }
    }
}
