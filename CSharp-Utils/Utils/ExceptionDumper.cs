using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectDumper;

namespace CSharpUtils.Utils
{
    /// <summary>
    /// Class that makes it easier to generate crash logs with enough information to solve the issue.
    /// </summary>
    public class ExceptionDumper
    {
        /// <summary>
        /// The exception.
        /// </summary>
        private Exception exception; 

        /// <summary>
        /// The extra data.
        /// </summary>
        private Dictionary<String, Object> data = new Dictionary<String, Object>();

        public ExceptionDumper(Exception e)
        {
            this.exception = e;
        }

        /// <summary>
        /// Add a variable to the crash log.
        /// </summary>
        public void Dump(String key, Object value)
        {
            this.data.Add(key, value);
        }

        /// <summary>
        /// Add the contents of a file to the crash log.
        /// </summary>
        public void DumpFile(String key, String path)
        {
            String value = "";
            try
            {
                using (StreamReader reader = File.OpenText(path))
                {
                    value = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                value = "Unable to read file";
            }
            this.Dump(key, value);
        }

        /// <summary>
        /// Save the error log.
        /// </summary>
        /// <returns>The path of the log</returns>
        public String Save()
        {
            String filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                String.Format("ErrorReport {0}.log", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss fff"))
            );
            using (StreamWriter writer = new StreamWriter(filename, false))
            {
                writer.WriteLine("===== Exception =====");
                writer.WriteLine(this.exception.ToString());
                writer.WriteLine();
                writer.WriteLine("===== Dump =====");
                foreach (KeyValuePair<String, Object> data in this.data)
                {
                    Dumper.Dump(data.Value, data.Key, writer);
                    writer.WriteLine();
                }
            }
            return filename;
        }
    }
}
