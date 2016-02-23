using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;

using CSharpUtils.Utils;

namespace CSharpUtils.GUI
{
    public partial class ConfigDialog<T> : Form
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        /// <summary>
        ///     Occurs when the config is being loaded.
        /// </summary>
        public EventHandler<EventArgs> Loading = (s, e) => { };

        /// <summary>
        ///     Occurs after the config has been loaded.
        /// </summary>
        public EventHandler<EventArgs> Loaded = (s, e) => { };

        /// <summary>
        ///     Occurs before the config is saved.
        ///     This event can be cancelled to stop the save from happening.
        /// </summary>
        public EventHandler<CancelEventArgs> PreviewSaving = (s, e) => { };

        /// <summary>
        ///     Occurs when the config is being saved.
        ///     This happens after all confirmations have happened, so at this point it is certain the save will happen.
        /// </summary>
        public EventHandler<EventArgs> Saving = (s, e) => { };

        /// <summary>
        ///     Occurs after the config has been saved.
        /// </summary>
        public EventHandler<EventArgs> Saved = (s, e) => { };
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore FieldCanBeMadeReadOnly.Global

        private readonly T _settings;
        private readonly Dictionary<string, Control> _settingsMap = new Dictionary<string, Control>();
        private readonly TableLayoutPanel _panel;
        private readonly Dictionary<string, ConfigDialogGroup> _groups = new Dictionary<string, ConfigDialogGroup>();

        protected ConfigDialog(T settings)
        {
            this._settings = settings;

            // Setup the form.
            this.AutoSize = true;
            this.MinimumSize = new Size(500, 500);
            this.Text = "Settings";

            // Create the main panel.
            this._panel = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                RowCount = 0
            };
            this._panel.ColumnStyles.Add(new ColumnStyle());
            this.Controls.Add(this._panel);

            // Create the button panel.
            Button buttonSave = new Button
            {
                Text = "Save",
                DialogResult = DialogResult.OK
            };
            buttonSave.Click += (sender, e) =>
            {
                if (!this.SaveSettings())
                {
                    return;
                }
                this.Close();
            };
            Button buttonCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel
            };
            buttonCancel.Click += (sender, e) =>
            {
                this.Close();
            };
            FlowLayoutPanel panelButton = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft
            };
            panelButton.Controls.Add(buttonSave);
            panelButton.Controls.Add(buttonCancel);
            this.Controls.Add(panelButton);

            // Some default event handlers.
            this.Shown += this.ShownLoadSetttings;
            this.PreviewSaving += this.PreviewSavingServicesRestart;
            this.Saved += this.SavedServicesRestart;
        }

        /// <summary>
        ///     The services that this settings screen is relevant for.
        /// </summary>
        protected ServicesCollection Services { private get; set; }

        private void PreviewSavingServicesRestart(object sender, CancelEventArgs e)
        {
            if (this.Services == null)
            {
                return;
            }

            this.Saved -= this.SavedServicesRestart;
            if (this.Services.GetStatuses().Values.Any(s => s == ServiceControllerStatus.Running))
            {
                // Prompt to confirm saving/restarting.
                DialogResult dialogResult = MessageBox.Show(
                    "The service(s) that is/are currently running will now be restarted. Are you sure you want to proceed? (Services that are not running will NOT be started).", 
                    "Save/restart service(s)", 
                    MessageBoxButtons.YesNo
                );
                if (dialogResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.Saved += this.SavedServicesRestart;
                }
            }
        }

        private void SavedServicesRestart(object sender, EventArgs e)
        {
            if (this.Services == null)
            {
                return;
            }

            // Restart only running services.
            ServicesCollection runningServices = new ServicesCollection();
            runningServices.AddRange(from entry in this.Services.GetStatuses() where entry.Value == ServiceControllerStatus.Running select entry.Key);
            runningServices.InvokeRestart();
        }

        private void ShownLoadSetttings(object sender, EventArgs e)
        {
            // Automatically load the settings when the form is shown.
            this.LoadSettings();
        }

        /// <summary>
        ///     Add a header logo.
        /// </summary>
        /// <param name="logo">The logo</param>
        protected void AddHeader(Image logo)
        {
            Label headerLabel = new Label
            {
                Dock = DockStyle.Top,
                Image = logo,
                ImageAlign = ContentAlignment.MiddleCenter,
                MinimumSize = logo.Size + new Size(0, 20)
            };
            this.Controls.Add(headerLabel);
        }

        /// <summary>
        ///     Create a new config group.
        /// </summary>
        /// <param name="name">The human-readable name/label of the group</param>
        /// <returns>The new group</returns>
        protected ConfigDialogGroup AddGroup(string name)
        {
            if (this._groups.ContainsKey(name))
            {
                throw new ArgumentException($"Group name already in use: {name}", nameof(name));
            }

            // Create the group
            ConfigDialogGroup group = new ConfigDialogGroup(this);
            group.Text = name;

            // Store the group
            this._panel.RowCount += 1;
            this._panel.RowStyles.Add(new RowStyle());
            this._panel.Controls.Add(group, 0, this._panel.RowCount - 1);
            this._groups.Add(name, group);

            return group;
        }

        /// <summary>
        ///     Connect a propery to a control.
        /// </summary>
        /// <param name="propertyName">The property, as created in the ui/accessible through Properties.Settings.Default.X</param>
        /// <param name="control">The control that this property should map to</param>
        private void AddProperty(string propertyName, Control control)
        {
            PropertyInfo info = typeof (T).GetProperty(propertyName);
            if (info == null)
            {
                throw new KeyNotFoundException($"Unknown property {propertyName}");
            }
            this._settingsMap.Add(propertyName, control);
        }

        /// <summary>
        ///     Get the PropertyInfo for a given control.
        ///     This can be used to get/set the value of the control to save/load it.
        /// </summary>
        /// <param name="control">The control to get the PropertyInfo for</param>
        /// <returns>The PropertyInfo for the main data property for the type of the control</returns>
        private static PropertyInfo GetPropertyForControl(Control control)
        {
            if (control is CheckBox)
            {
                return typeof (CheckBox).GetProperty("Checked");
            }
            if (control is NumericUpDown)
            {
                return typeof (NumericUpDown).GetProperty("Value");
            }
            return typeof (Control).GetProperty("Text");
        }

        /// <summary>
        ///     Load the settings from the config into the controls.
        /// </summary>
        private void LoadSettings()
        {
            // Load the settings into the ui.
            Type type = typeof (T);
            foreach (KeyValuePair<string, Control> entry in this._settingsMap)
            {
                PropertyInfo settingsPropertyInfo = type.GetProperty(entry.Key);
                PropertyInfo controlPropertyInfo = GetPropertyForControl(entry.Value);
                controlPropertyInfo.SetValue(entry.Value,
                    Convert.ChangeType(settingsPropertyInfo.GetValue(this._settings), controlPropertyInfo.PropertyType));
            }

            // Fire the events.
            this.Loading(this, new EventArgs());
            this.Loaded(this, new EventArgs());
        }

        /// <summary>
        ///     Save the settings from the controls into the config.
        /// </summary>
        private bool SaveSettings()
        {
            // Check whether the save should continue.
            CancelEventArgs e = new CancelEventArgs();
            this.PreviewSaving(this, e);
            if (e.Cancel)
            {
                return false;
            }

            // Set the properties.
            Type type = typeof (T);
            foreach (KeyValuePair<string, Control> entry in this._settingsMap)
            {
                PropertyInfo settingsPropertyInfo = type.GetProperty(entry.Key);
                PropertyInfo controlPropertyInfo = GetPropertyForControl(entry.Value);
                settingsPropertyInfo.SetValue(this._settings,
                    Convert.ChangeType(controlPropertyInfo.GetValue(entry.Value), settingsPropertyInfo.PropertyType));
            }
            this.Saving(this, new EventArgs());

            // Save the config.
            type.GetMethod("Save").Invoke(this._settings, new object[0]);
            this.Saved(this, new EventArgs());

            return true;
        }
    }
}