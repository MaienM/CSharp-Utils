using System;

namespace CSharpUtils.Utils.StatusLogger
{
    public class BaseStatusLogger
    {
        public EventHandler<StatusChangeEventArgs> Changed = (sender, e) => { };
    }

    public class StatusChangeEventArgs
    {
        public DateTime Timestamp { get; private set; }
        public string Key { get; private set; }
        public object OldValue { get; private set; }
        public string OldValueString { get; private set; }
        public object NewValue { get; private set; }
        public string NewValueString { get; private set; }

        public StatusChangeEventArgs(string key, object oldValue, object newValue)
        {
            this.Timestamp = new DateTime();
            this.Key = key;
            this.OldValue = oldValue;
            this.OldValueString = LocalStatusLogger.FormatValue(oldValue);
            this.NewValue = newValue;
            this.NewValueString = LocalStatusLogger.FormatValue(newValue);
        }
    }
}
