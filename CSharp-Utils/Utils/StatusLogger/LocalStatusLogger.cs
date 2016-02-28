using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpUtils.Utils.StatusLogger
{
    public class LocalStatusLogger : BaseStatusLogger
    {
        private static LocalStatusLogger instance;
        
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public static LocalStatusLogger GetInstance()
        {
            return instance ?? (instance = new LocalStatusLogger());
        }

        public void SetStatus(string key, bool value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, bool? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, short value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, short? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, int value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, int? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, uint value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, uint? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, long value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, long? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, ulong value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, ulong? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, double value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, double? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, float value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, float? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, string value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, char value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, char? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, char[] value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, byte value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, byte? value) { this.InnerSetStatus(key, value); }
        public void SetStatus(string key, byte[] value) { this.InnerSetStatus(key, value); }

        private void InnerSetStatus(string fullKey, object value)
        {
            // Get the group and key.
            string key;
            Dictionary<string, object> group;
            this.GetGroupAndKey(fullKey, out group, out key);

            // Log the data change.
            if (group.ContainsKey(key))
            {
                if (FormatValue(group[key]) != FormatValue(value))
                {
                    this.Changed(this, new StatusChangeEventArgs(fullKey, group[key], value));
                }
            }
            else
            {
                this.Changed(this, new StatusChangeEventArgs(fullKey, null, value));
            }

            // Set the value.
            group[key] = value;
        }

        private void GetGroupAndKey(string key, out Dictionary<string, object> group, out string remainingKey)
        {
            // Split the key.
            string[] parts = key.Split('.');

            // Find/create the group.
            group = this._data;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (group == null)
                {
                    break;
                }

                if (!group.ContainsKey(parts[i]))
                {
                    group.Add(parts[i], new Dictionary<string, object>());
                }
                else if (!(group[parts[i]] is Dictionary<string, object>))
                {
                    throw new ArgumentException($"{string.Join(".", parts.Take(i + 1))} is used as both a group and a key");
                }
                group = group[parts[i]] as Dictionary<string, object>;
            }

            // The last part of the key is the remaining key.
            remainingKey = parts.Last();
        }

        public static string FormatValue(object value)
        {
            return value?.ToString() ?? "null";
        }

        public void Dump()
        {
            this.Dump("", this._data);
        }

        private void Dump(string key, Dictionary<string, object> data)
        {
            foreach (KeyValuePair<string, object> entry in data)
            {
                Dictionary<string, object> value = entry.Value as Dictionary<string, object>;
                if (value != null)
                {
                    this.Dump($"{key}.{entry.Key}", value);
                }
                else
                {
                    Console.WriteLine($"{key}.{entry.Key}: {FormatValue(entry.Value)}");
                }
            }
        }
    }
}