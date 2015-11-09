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

        private Dictionary<String, Object> data = new Dictionary<String, Object>();

        public static StatusLogger GetInstance()
        {
            if (instance == null)
            {
                instance = new StatusLogger();
            }
            return instance;
        }

        public void SetStatus(String key, bool value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, short value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, int value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, uint value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, long value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, ulong value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, double value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, float value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, string value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, char value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, char[] value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, byte value) { InnerSetStatus(key, value); }
        public void SetStatus(String key, byte[] value) { InnerSetStatus(key, value); }

        private void InnerSetStatus(String fullKey, Object value)
        {
            // Get the group and key.
            String key;
            Dictionary<String, Object> group;
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

        private void GetGroupAndKey(String key, out Dictionary<String, Object> group, out String remainingKey)
        {
            // Split the key.
            String[] parts = key.Split('.');

            // Find/create the group.
            group = this.data;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (!group.ContainsKey(parts[i]))
                {
                    group.Add(parts[i], new Dictionary<String, Object>());
                }
                else if (!(group[parts[i]] is Dictionary<String, Object>))
                {
                    throw new ArgumentException(String.Format("{0} is used as both a group and a key", String.Join(".", parts.Take(i + 1))));
                }
                group = group[parts[i]] as Dictionary<String, Object>;
            }

            // The last part of the key is the remaining key.
            remainingKey = parts.Last();
        }

        public static String FormatValue(Object value)
        {
            if (value == null) {
                return "null";
                }
            return value.ToString();
        }

        private void OnChanged(object sender, StatusChangeEventArgs e)
        {
            String timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            using (StreamWriter writer = new StreamWriter("statuslogger.log", true))
            {
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

        private void Dump(String key, Dictionary<String, Object> data)
        {
            foreach (KeyValuePair<String, Object> entry in data)
            {
                if (entry.Value is Dictionary<String, Object>)
                {
                    Dump(String.Format("{0}.{1}", key, entry.Key), entry.Value as Dictionary<String, Object>);
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
        public String key { get; private set; }
        public Object oldValue { get; private set; }
        public String oldValueString { get; private set; }
        public Object newValue { get; private set; }
        public String newValueString { get; private set; }

        public StatusChangeEventArgs(String key, Object oldValue, Object newValue)
        {
            this.key = key;
            this.oldValue = oldValue;
            this.oldValueString = StatusLogger.FormatValue(oldValue);
            this.newValue = newValue;
            this.newValueString = StatusLogger.FormatValue(newValue);
        }
    }
}