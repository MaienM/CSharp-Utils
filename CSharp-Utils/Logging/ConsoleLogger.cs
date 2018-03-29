using System;

namespace CSharpUtils.Logging
{
    public class ConsoleLogger : Logger
    {
        override public void WriteMessage(LogEventArgs logEvent)
        {
            Console.WriteLine(logEvent.Message);
        }
    }
}
