using System;
using System.Collections.Generic;

namespace CSharpUtils.Logging
{
    public class MultiLogger : Logger
    {
        public List<Logger> Loggers { get; } = new List<Logger>();

        public MultiLogger(List<Logger> loggers)
        {
            Loggers.AddRange(loggers);
        }

        override public void WriteMessage(LogEventArgs logEvent)
        {
            foreach (Logger logger in Loggers)
            {
                logger.WriteMessage(logEvent);
            }
        }
    }
}
