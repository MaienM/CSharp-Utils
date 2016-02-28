using System;
using System.IO;

namespace CSharpUtils.Utils.StatusLogger
{
    public class StatusLoggerWriter
    {
        public static string Path { get; set; }

        public StatusLoggerWriter(LocalStatusLogger logger, string path)
        {
            Path = path;
            logger.Changed += this.OnChanged;
        }

        /// <summary>
        /// Clear the log file.
        /// </summary>
        public void Truncate()
        {
            if (!File.Exists(Path))
            {
                return;
            }

            using (FileStream fileStream = new FileStream(Path, FileMode.Truncate))
            {
                fileStream.SetLength(0);
            }
        }

        private void OnChanged(object sender, StatusChangeEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(Path, true))
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                if (e.NewValue == null)
                {
                    writer.WriteLine("[{0}] {1}: {2} -> {3}", timestamp, e.Key, e.OldValueString, e.NewValueString);
                }
                else
                {
                    writer.WriteLine("[{0}] {1}: {2}", timestamp, e.Key, e.NewValueString);
                }
            }
        }
    }
}
