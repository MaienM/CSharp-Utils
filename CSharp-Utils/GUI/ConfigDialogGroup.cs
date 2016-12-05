using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using CSharpUtils.Utils;

#if ENTITY_FRAMEWORK
using System.Data.Entity;
#endif

namespace CSharpUtils.GUI
{
    partial class ConfigDialog<T>
    {
        protected sealed class ConfigDialogGroup : GroupBox
        {
            private readonly ConfigDialog<T> _dialog;
            private readonly TableLayoutPanel _panel;

            public ConfigDialogGroup(ConfigDialog<T> dialog)
            {
                // Store the parent dialog.
                _dialog = dialog;

                // Set the groupbox properties.
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                AutoSize = true;

                // Create the inner panel.
                _panel = new TableLayoutPanel
                {
                    AutoSize = true,
                    ColumnCount = 2,
                    Dock = DockStyle.Fill,
                    RowCount = 0
                };
                _panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
                _panel.ColumnStyles.Add(new ColumnStyle());
                Controls.Add(_panel);
            }

            #region Adding settings
            // ReSharper disable UnusedMethodReturnValue.Global
            /// <summary>
            ///     Add a boolean property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The control for the property</returns>
            public CheckBox AddBoolean(string name, string label)
            {
                // Create and register the control.
                CheckBox control = new CheckBox();
                _dialog.AddProperty(name, control);

                // Setup the control.
                AddControl(label, control);

                return control;
            }

            /// <summary>
            ///     Add a numeric property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The control for the property</returns>
            public NumericUpDown AddNumeric(string name, string label)
            {
                // Create and register the control.
                NumericUpDown control = new NumericUpDown
                {
                    Minimum = decimal.MinValue,
                    Maximum = decimal.MaxValue
                };
                _dialog.AddProperty(name, control);

                // Setup the control.
                AddControl(label, control);

                return control;
            }

            /// <summary>
            ///     Add a textual property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The control for the property</returns>
            public TextBox AddString(string name, string label)
            {
                // Create and register the control.
                TextBox control = new TextBox();
                _dialog.AddProperty(name, control);

                // Setup the control.
                AddControl(label, control);

                return control;
            }

            /// <summary>
            ///     Add a masked textual property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <param name="mask">The mask of the control</param>
            /// <returns>The control for the property</returns>
            public MaskedTextBox AddMaskedString(string name, string label, string mask)
            {
                // Create and register the control.
                MaskedTextBox control = new MaskedTextBox(mask);
                _dialog.AddProperty(name, control);

                // Setup the control.
                AddControl(label, control);

                return control;
            }

            /// <summary>
            ///     Add a textual property, with a button, to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <param name="icon">The icon to add to the text box</param>
            /// <returns>The control for the property</returns>
            private Tuple<TextBox, Label> AddTextWithIcon(string name, string label, Icon icon)
            {
                // Create and register the main control.
                TextBox control = new TextBox { Dock = DockStyle.Fill };
                _dialog.AddProperty(name, control);

                // Create the button.
                Label iconControl = new Label
                {
                    Image = icon.ToBitmap(),
                    Dock = DockStyle.Right,
                    MaximumSize = new Size(control.Height + 2, control.Height + 2)
                };

                // Add the icon to the text box, and make sure the text does not go below the icon.
                control.Controls.Add(iconControl);
                User32.SendMessage(control.Handle, 0xd3, (IntPtr)2, (IntPtr)(iconControl.Width << 16));

                // Setup the control.
                AddControl(label, control);

                return new Tuple<TextBox, Label>(control, iconControl);
            }

            /// <summary>
            ///     Add a folder path property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The controls for the property</returns>
            public Tuple<TextBox, Label, FolderBrowserDialog> AddFolderPicker(string name, string label)
            {
                Tuple<TextBox, Label> controls = AddTextWithIcon(name, label, Resources.folder);

                // Create the folder browser.
                FolderBrowserDialog browser = new FolderBrowserDialog();

                // Setup the button handler.
                controls.Item2.Click += (sender, e) =>
                {
                    browser.SelectedPath = controls.Item1.Text;
                    if (browser.ShowDialog() == DialogResult.OK)
                    {
                        controls.Item1.Text = browser.SelectedPath;
                    }
                };

                return new Tuple<TextBox, Label, FolderBrowserDialog>(controls.Item1, controls.Item2, browser);
            }

            /// <summary>
            ///     Add a file path property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <param name="create">Whether to use a picker to create new files</param>
            /// <returns>The controls for the property</returns>
            public Tuple<TextBox, Label, FileDialog> AddFilePicker(string name, string label, bool create)
            {
                Tuple<TextBox, Label> controls = AddTextWithIcon(name, label, create ? Resources.save : Resources.file);

                // Create the file browser.
                FileDialog browser = create ? (FileDialog)new SaveFileDialog() : new OpenFileDialog();

                // Setup the button handler.
                controls.Item2.Click += (sender, e) =>
                {
                    browser.FileName = controls.Item1.Text;
                    if (browser.ShowDialog() == DialogResult.OK)
                    {
                        controls.Item1.Text = browser.FileName;
                    }
                };

                return new Tuple<TextBox, Label, FileDialog>(controls.Item1, controls.Item2, browser);
            }

            #if ENTITY_FRAMEWORK
            /// <summary>
            ///     Add a table-backed dataset to this group.
            /// </summary>
            /// <param name="context">The context of the dataset</param>
            /// <param name="dataSet">The dataset to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The control for the property</returns>
            public DataGridView AddDataGridView<TD>(DbContext context, DbSet<TD> dataSet, string label) where TD : class
            {
                // Create the main control.
                DataGridView control = new DataGridView
                {
                    AllowUserToAddRows = true,
                    AllowUserToDeleteRows = true,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
                };

                // On loading, setup the control with the dataset.
                _dialog.Loading += (sender, args) =>
                {
                    // Create the binding list for the data.
                    BindingList<TD> data = new BindingList<TD>(dataSet.ToList());

                    // Setup the gridview with the binding list.
                    control.DataSource = data;

                    // Add the event handlers to process adding and removing rows.
                    control.UserAddedRow += (o, innerArgs) =>
                    {
                        dataSet.Add(data[innerArgs.Row.Index - 1]);
                    };
                    control.UserDeletingRow += (o, innerArgs) =>
                    {
                        dataSet.Remove(data[innerArgs.Row.Index]);
                    };
                };
                
                // On saving, save the context associated with the dataset.
                _dialog.Saving += (sender, args) =>
                {
                    context.SaveChanges();
                };

                // Setup the control.
                AddControl(label, control);

                return control;
            }
            #endif
            // ReSharper enable UnusedMethodReturnValue.Global
            #endregion

            /// <summary>
            ///     Add a control to the group.
            /// </summary>
            /// <param name="control">The control to add</param>
            public void AddControl(Control control)
            {
                control.Dock = DockStyle.Fill;
                _panel.RowCount += 1;
                _panel.RowStyles.Add(new RowStyle());
                _panel.Controls.Add(control, 0, _panel.RowCount - 1);
                _panel.SetColumnSpan(control, 2);
            }

            /// <summary>
            ///     Add a control to this group.
            /// </summary>
            /// <param name="label">The label of the control</param>
            /// <param name="control">The control</param>
            public void AddControl(string label, Control control)
            {
                // Setup the control's basic properties.
                control.Dock = DockStyle.Fill;

                // Create the label.
                Label labelControl = new Label
                {
                    Text = label,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Add the elements to the groupbox.
                _panel.RowCount += 1;
                _panel.RowStyles.Add(new RowStyle());
                _panel.Controls.Add(labelControl, 0, _panel.RowCount - 1);
                _panel.Controls.Add(control, 1, _panel.RowCount - 1);
            }
        }
    }
}
