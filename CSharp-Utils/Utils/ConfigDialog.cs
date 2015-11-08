using System;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CSharpUtils.Utils
{
	public class ConfigHelper
	{
        public ApplicationSettingsBase Settings { get; set; }

        private Dictionary<String, Control> settingsMap = new Dictionary<String, Control>();

        /// <summary>
        /// Connect a propery to a control.
        /// </summary>
        /// <param name="propertyName">The property, as created in the ui/accessible through Properties.Settings.Default.X</param>
        /// <param name="control">The control that this property should map to</param>
        public void AddProperty(String propertyName, Control control)
        {
            if (Settings.Properties[propertyName] == null)
            {
                throw new KeyNotFoundException(String.Format("Unknown property {0}", propertyName));
            }
            settingsMap.Add(propertyName, control);
        }

        /// <summary>
        /// Load the settings from the config into the controls.
        /// </summary>
        public void Load() {
            // Load the settings into the ui.
            foreach (KeyValuePair<String, Control> entry in settingsMap)
            {
                entry.Value.Text = Settings.Properties[entry.Key].DefaultValue.ToString();
            }
		}

        /// <summary>
        /// Save the settings from the controls into the config.
        /// </summary>
		private void Save()
		{
            // Save the properties.
            foreach (KeyValuePair<String, Control> entry in settingsMap)
            {
                // Get the value.
                Object value = entry.Value.Text;
                if (entry.Value is NumericUpDown)
                {
                    value = (entry.Value as NumericUpDown).Value;
                }
                
                // Store the value.
                Settings.Properties[entry.Key].DefaultValue = Convert.ChangeType(value, Settings.Properties[entry.Key].PropertyType);
            }
			Settings.Save();
		}
	}
}
