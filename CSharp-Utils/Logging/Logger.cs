using System;
using System.Text.RegularExpressions;

namespace CSharpUtils.Logging
{
	public enum LogLevel
	{
		DEBUG,
		INFO,
		ERROR
	}

	public class LogEventArgs : EventArgs
	{
		public LogLevel Level { get; private set; }
		public string Message { get; private set; }

		public LogEventArgs(LogLevel level, string message)
		{
			Level = level;
			Message = message;
		}
	}

    /// <summary>
    /// A generic logger.
    /// </summary>
	abstract public class Logger
	{
        /// <summary>
        /// The timestamp format (datetime) of the messages.
        /// </summary>
        public string TimestampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// An event that is fired whenever a log message is received.
        /// </summary>
	    public EventHandler<LogEventArgs> Message = (object sender, LogEventArgs e) => {};

		/// <summary>
		/// Log a message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Log(LogLevel level, string message = null, params object[] args)
		{
			// Process the message.
			if (!string.IsNullOrWhiteSpace(message))
			{
				// Create the timestamp and indent.
				string data = string.Format("[{0}-{1}] ", DateTime.Now.ToString(TimestampFormat), Enum.GetName(typeof(LogLevel), level)[0]);
				string indent = Regex.Replace(data, "[^\\s]", " ");

				// Split the messages in lines.
				string[] lines = string.Format(message, args).Split(Environment.NewLine.ToCharArray());

				// Merge the lines back together.
				message = data + lines[0];
				for (int i = 1; i < lines.Length; i++)
				{
					string line = lines[i].Trim();
					if (!string.IsNullOrWhiteSpace(line))
					{
						message += Environment.NewLine + indent + line;
					}
				}
			}
			else
			{
				message = "";
			}

            // Output the message.
            LogEventArgs logArgs = new LogEventArgs(level, message);
			Message(this, logArgs);
            WriteMessage(logArgs);
		}

        abstract public void WriteMessage(LogEventArgs logEvent);
	}
}
