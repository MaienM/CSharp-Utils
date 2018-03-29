using System;
using System.IO;

namespace CSharpUtils.Logging
{
    public class FileLogger : Logger
    {       
        /// <summary>
        /// The path where the log file is stored.
        /// </summary>
        public string Path { get; set; }

        public FileLogger(string path)
        {
            this.Path = path;
        }
        
        override public void WriteMessage(LogEventArgs args)
        {
            // Write the message.
            using (StreamWriter sw = File.AppendText(Path))
            {
                sw.WriteLine(args.Message);
            }
        }
    }
}
