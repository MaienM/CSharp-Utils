using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;

namespace CSharpUtils.Utils
{

    public class ConfigDialog<T> : Form
	{
        private T settings;
        private Dictionary<string, ConfigDialogGroup> groups = new Dictionary<string, ConfigDialogGroup>();
        private Dictionary<string, Control> settingsMap = new Dictionary<string, Control>();

        public TableLayoutPanel Panel { get; private set; }

        /// <summary>
        /// The services that this settings screen is relevant for.
        /// </summary>
        public ServicesCollection Services { get; set; }

        public ConfigDialog(T settings) : base()
        {
            this.settings = settings;

            // Setup the form.
            AutoSize = true;
            MinimumSize = new Size(500, 500);
            Text = "Settings";

            // Create the main panel.
            Panel = new TableLayoutPanel();
            Panel.AutoSize = true;
            Panel.ColumnCount = 1;
            Panel.ColumnStyles.Add(new ColumnStyle());
            Panel.Dock = DockStyle.Fill;
            Panel.RowCount = 0;
            Controls.Add(Panel);

            // Create the button panel.
            Button buttonSave = new Button();
            buttonSave.Text = "Save";
            buttonSave.DialogResult = DialogResult.OK;
            buttonSave.Click += (object sender, EventArgs e) =>
            {
                // Prompt to confirm saving/restarting.
                if (Services != null)
                {
                    DialogResult dialogResult = MessageBox.Show("The service(s) will now be restarted if currently running. Are you sure you want to proceed?", "Save/restart service(s)", MessageBoxButtons.YesNo);
                    if (dialogResult != DialogResult.Yes)
                    {
                        return;
                    }
                }

                // Save the config.
                Save();

                // Restart services if needed.
                if (Services != null)
                {
                    Services.InvokeRestart();
                }

                // Close the window.
                Close();
            };
            Button buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Click += (object sender, EventArgs e) =>
            {
                Close();
            };
            FlowLayoutPanel panelButton = new FlowLayoutPanel();
            panelButton.AutoSize = true;
            panelButton.Controls.Add(buttonSave);
            panelButton.Controls.Add(buttonCancel);
            panelButton.Dock = DockStyle.Bottom;
            panelButton.FlowDirection = FlowDirection.RightToLeft;
            Controls.Add(panelButton);

            // Test stuff.
            KeyUp += ConfigDialog_KeyUp;
            KeyPreview = true;
            Shown += ConfigDialog_Shown;
        }

        private void ConfigDialog_Shown(object sender, EventArgs e)
        {
            Activate();
        }

        private void ConfigDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Add a header logo.
        /// </summary>
        /// <param name="logo">The logo</param>
        public void AddHeader(Image logo)
        {
            Label headerLabel = new Label();
            headerLabel.Dock = DockStyle.Top;
            headerLabel.Image = logo;
            headerLabel.ImageAlign = ContentAlignment.MiddleCenter;
            headerLabel.MinimumSize = headerLabel.Image.Size + new Size(0, 20);
            Controls.Add(headerLabel);
        }

        /// <summary>
        /// Create a new config group.
        /// </summary>
        /// <param name="name">The human-readable name/label of the group</param>
        /// <returns>The new group</returns>
        public ConfigDialogGroup AddGroup(string name)
        {
            if (groups.ContainsKey(name))
            {
                throw new ArgumentException(string.Format("Group name already in use: {0}", name), "name");
            }

            // Create the group
            ConfigDialogGroup group = new ConfigDialogGroup(this);
            group.Text = name;

            // Store the group
            Panel.RowCount += 1;
            Panel.RowStyles.Add(new RowStyle());
            Panel.Controls.Add(group, 0, Panel.RowCount - 1);
            groups.Add(name, group);

            return group;
        }

        /// <summary>
        /// Connect a propery to a control.
        /// </summary>
        /// <param name="propertyName">The property, as created in the ui/accessible through Properties.Settings.Default.X</param>
        /// <param name="control">The control that this property should map to</param>
        public void AddProperty(string propertyName, Control control)
        {
            PropertyInfo info = typeof(T).GetProperty(propertyName);
            if (info == null)
            {
                throw new KeyNotFoundException(string.Format("Unknown property {0}", propertyName));
            }
            settingsMap.Add(propertyName, control);
        }

        private PropertyInfo GetPropertyForControl(Control control)
        {
            if (control is CheckBox)
            {
                return typeof(CheckBox).GetProperty("Checked");
            }
            else if (control is NumericUpDown)
            {
                return typeof(NumericUpDown).GetProperty("Value");
            }
            else
            {
                return typeof(Control).GetProperty("Text");
            }
        }

        /// <summary>
        /// Load the settings from the config into the controls.
        /// </summary>
        public void LoadSettings()
        {
            // Load the settings into the ui.
            Type type = typeof(T);
            foreach (KeyValuePair<string, Control> entry in settingsMap)
            {
                PropertyInfo info = type.GetProperty(entry.Key);
                PropertyInfo controlPropertyInfo = GetPropertyForControl(entry.Value);
                controlPropertyInfo.SetValue(entry.Value, Convert.ChangeType(info.GetValue(settings), controlPropertyInfo.PropertyType));
            }
		}

        /// <summary>
        /// Save the settings from the controls into the config.
        /// </summary>
		private void Save()
		{
            // Save the properties.
            Type type = typeof(T);
            foreach (KeyValuePair<string, Control> entry in settingsMap)
            {
                PropertyInfo info = type.GetProperty(entry.Key);
                PropertyInfo controlPropertyInfo = GetPropertyForControl(entry.Value);
                info.SetValue(settings, Convert.ChangeType(controlPropertyInfo.GetValue(entry.Value), info.PropertyType));
            }
            type.GetMethod("Save").Invoke(settings, new object[0]);
		}

        public class ConfigDialogGroup : GroupBox
        {
            public ConfigDialog<T> Dialog { get; private set; }

            public TableLayoutPanel Panel { get; private set; }
            
            public ConfigDialogGroup(ConfigDialog<T> dialog)
            {
                // Store the parent dialog.
                Dialog = dialog;

                // Set the groupbox properties.
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                AutoSize = true;

                // Create the inner panel.
                Panel = new TableLayoutPanel();
                Panel.AutoSize = true;
                Panel.ColumnCount = 2;
                Panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
                Panel.ColumnStyles.Add(new ColumnStyle());
                Panel.Dock = DockStyle.Fill;
                Panel.RowCount = 0;
                Controls.Add(Panel);
            }

            /// <summary>
            /// Add a control to the group.
            /// </summary>
            /// <param name="control">The control to add</param>
            public void AddControl(Control control)
            {
                control.Dock = DockStyle.Fill;
                Panel.RowCount += 1;
                Panel.RowStyles.Add(new RowStyle());
                Panel.Controls.Add(control, 0, Panel.RowCount - 1);
                Panel.SetColumnSpan(control, 2);
            }

            /// <summary>
            /// Add a boolean property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The control for the property</returns>
            public CheckBox AddBoolean(string name, string label)
            {
                // Create and register the control.
                CheckBox control = new CheckBox();
                Dialog.AddProperty(name, control);

                // Setup the control.
                AddControl(label, control);

                return control;
            }

            /// <summary>
            /// Add a numeric property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The control for the property</returns>
            public NumericUpDown AddNumeric(string name, string label)
            {
                // Create and register the control.
                NumericUpDown control = new NumericUpDown();
                control.Minimum = decimal.MinValue;
                control.Maximum = decimal.MaxValue;
                Dialog.AddProperty(name, control);

                // Setup the control.
                AddControl(label, control);

                return control;
            }

            /// <summary>
            /// Add a textual property to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The control for the property</returns>
            public TextBox AddString(string name, string label)
            {
                // Create and register the control.
                TextBox control = new TextBox();
                Dialog.AddProperty(name, control);

                // Setup the control.
                AddControl(label, control);

                return control;
            }

            /// <summary>
            /// Add a textual property, with a button, to this group.
            /// </summary>
            /// <param name="name">The property to control</param>
            /// <param name="label">The label of the property</param>
            /// <returns>The control for the property</returns>
            private Tuple<TextBox, Label> AddTextWithIcon(string name, string label, Icon icon)
            {
                // Create and register the main control.
                TextBox control = new TextBox();
                Dialog.AddProperty(name, control);
                control.Dock = DockStyle.Fill;

                // Create the button.
                Label iconControl = new Label();
                iconControl.Image = icon.ToBitmap();
                iconControl.Dock = DockStyle.Right;
                iconControl.MaximumSize = new Size(control.Height + 2, control.Height + 2);

                // Add the icon to the text box, and make sure the text does not go below the icon.
                control.Controls.Add(iconControl);
                User32.SendMessage(control.Handle, 0xd3, (IntPtr)2, (IntPtr)(iconControl.Width << 16));

                // Setup the control.
                AddControl(label, control);

                return new Tuple<TextBox, Label>(control, iconControl);
            }

            /// <summary>
            /// Add a folder path property to this group.
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
                controls.Item2.Click += (object sender, EventArgs e) =>
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
            /// Add a file path property to this group.
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
                controls.Item2.Click += (object sender, EventArgs e) =>
                {
                    browser.FileName = controls.Item1.Text;
                    if (browser.ShowDialog() == DialogResult.OK)
                    {
                        controls.Item1.Text = browser.FileName;
                    }
                };

                return new Tuple<TextBox, Label, FileDialog>(controls.Item1, controls.Item2, browser);
            }

            /// <summary>
            /// Add a property to this group.
            /// </summary>
            /// <param name="label">The label of the property</param>
            /// <param name="control">The control for the property</param>
            private void AddControl(string label, Control control)
            {
                // Setup the control's basic properties.
                control.Dock = DockStyle.Fill;

                // Create the label.
                Label labelControl = new Label();
                labelControl.Text = label;
                labelControl.Dock = DockStyle.Fill;
                labelControl.TextAlign = ContentAlignment.MiddleLeft;

                // Add the elements to the groupbox.
                Panel.RowCount += 1;
                Panel.RowStyles.Add(new RowStyle());
                Panel.Controls.Add(labelControl, 0, Panel.RowCount - 1);
                Panel.Controls.Add(control, 1, Panel.RowCount - 1);
            }
        }
	}
}
