using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Utils
{
    /// <summary>
    /// Class that makes it easy to write to a temp file that will be moved to it's final destination once it has fully been written.
    /// </summary>
    public class TempWriter
    {
        /// <summary>
        /// The temp file path.
        /// </summary>
        public string TempPath { get; private set; }

        /// <summary>
        /// Stream writer for the temp file.
        /// </summary>
        public StreamWriter Writer { get; private set; }

        /// <summary>
        /// The ultimate destination path.
        /// </summary>
        public string FinalPath { get; private set; }

        public TempWriter(string path)
        {
            FinalPath = path;

            TempPath = Path.GetTempFileName();
            FileStream file = File.OpenWrite(TempPath);
            Writer = new StreamWriter(file);
        }

        public void Close()
        {
            Writer.Close();

            // Move the file to it's final destination.
            if (File.Exists(FinalPath))
            {
                File.Delete(FinalPath);
            }
            Directory.CreateDirectory(Path.GetDirectoryName(FinalPath));
            File.Move(TempPath, FinalPath);
        }
    }
}
