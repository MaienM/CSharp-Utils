using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Utils
{
    public class StatusLogger
    {
        private static StatusLogger instance;

        public static bool Enabled { get; set; } = true;
        public static string Path { get; set; } = "statuslogger.txt";

        private StatusLogger()
        {
            Changed += OnChanged;

            // Truncate log file.
            if (File.Exists("statuslogger.log"))
            {
                using (FileStream fileStream = new FileStream("statuslogger.log", FileMode.Truncate))
                {
                    fileStream.SetLength(0);
                }
            }
        }

        public EventHandler<StatusChangeEventArgs> Changed = (object sender, StatusChangeEventArgs e) => { };

        private Dictionary<string, Object> data = new Dictionary<string, Object>();

        public static StatusLogger GetInstance()
        {
            if (instance == null)
            {
                instance = new StatusLogger();
            }
            return instance;
        }

        public void SetStatus(string key, bool value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, short value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, int value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, uint value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, long value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, ulong value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, double value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, float value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, string value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, char value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, char[] value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, byte value) { InnerSetStatus(key, value); }
        public void SetStatus(string key, byte[] value) { InnerSetStatus(key, value); }

        private void InnerSetStatus(string fullKey, Object value)
        {
            // Get the group and key.
            string key;
            Dictionary<string, Object> group;
            GetGroupAndKey(fullKey, out group, out key);

            // Log the data change.
            if (group.ContainsKey(key))
            {
                if (FormatValue(group[key]) != FormatValue(value))
                {
                    Changed(this, new StatusChangeEventArgs(fullKey, group[key], value));
                }
            }
            else
            {
                Changed(this, new StatusChangeEventArgs(fullKey, null, value));
            }

            // Set the value.
            group[key] = value;
        }

        private void GetGroupAndKey(string key, out Dictionary<string, Object> group, out string remainingKey)
        {
            // Split the key.
            string[] parts = key.Split('.');

            // Find/create the group.
            group = this.data;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (!group.ContainsKey(parts[i]))
                {
                    group.Add(parts[i], new Dictionary<string, Object>());
                }
                else if (!(group[parts[i]] is Dictionary<string, Object>))
                {
                    throw new ArgumentException(string.Format("{0} is used as both a group and a key", string.Join(".", parts.Take(i + 1))));
                }
                group = group[parts[i]] as Dictionary<string, Object>;
            }

            // The last part of the key is the remaining key.
            remainingKey = parts.Last();
        }

        public static string FormatValue(Object value)
        {
            if (value == null) {
                return "null";
                }
            return value.ToString();
        }

        private void OnChanged(object sender, StatusChangeEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            using (StreamWriter writer = new StreamWriter(Path, true))
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                if (e.newValue == null)
                {
                    writer.WriteLine("[{0}] {1}: {2} -> {3}", timestamp, e.key, e.oldValueString, e.newValueString);
                }
                else
                {
                    writer.WriteLine("[{0}] {1}: {2}", timestamp, e.key, e.newValueString);
                }
            }
        }

        public void Dump()
        {
            Dump("", data);
        }

        private void Dump(string key, Dictionary<string, Object> data)
        {
            foreach (KeyValuePair<string, Object> entry in data)
            {
                if (entry.Value is Dictionary<string, Object>)
                {
                    Dump(string.Format("{0}.{1}", key, entry.Key), entry.Value as Dictionary<string, Object>);
                }
                else
                {
                    Console.WriteLine("{0}.{1}: {2}", key, entry.Key, FormatValue(entry.Value));
                }
            }
        }
    }

    public class StatusChangeEventArgs
    {
        public string key { get; private set; }
        public Object oldValue { get; private set; }
        public string oldValueString { get; private set; }
        public Object newValue { get; private set; }
        public string newValueString { get; private set; }

        public StatusChangeEventArgs(string key, Object oldValue, Object newValue)
        {
            this.key = key;
            this.oldValue = oldValue;
            this.oldValueString = StatusLogger.FormatValue(oldValue);
            this.newValue = newValue;
            this.newValueString = StatusLogger.FormatValue(newValue);
        }
    }
}