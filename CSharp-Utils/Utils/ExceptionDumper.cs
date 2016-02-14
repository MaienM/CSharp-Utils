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
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public ExceptionDumper(Exception e)
        {
            this.exception = e;
        }

        /// <summary>
        /// Add a variable to the crash log.
        /// </summary>
        public void Dump(string key, object value)
        {
            this.data.Add(key, value);
        }

        /// <summary>
        /// Add the contents of a file to the crash log.
        /// </summary>
        public void DumpFile(string key, string path)
        {
            string value = "";
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

        public string Format()
        {
            using (StringWriter writer = new StringWriter())
            {
                writer.WriteLine("===== Exception =====");
                writer.WriteLine(this.exception.ToString());
                writer.WriteLine();
                writer.WriteLine("===== Dump =====");
                foreach (KeyValuePair<string, object> data in this.data)
                {
                    Dumper.Dump(data.Value, data.Key, writer);
                    writer.WriteLine();
                }
                return writer.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Save the error log.
        /// </summary>
        /// <returns>The path of the log</returns>
        public string SaveToFile()
        {
            string filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                string.Format("ErrorReport {0}.log", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss fff"))
            );
            return filename;
        }
    }
}
