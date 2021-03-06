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
        /// The location where the files are stored.
        /// </summary>
        public static string directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

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
            exception = e;
        }

        /// <summary>
        /// Add a variable to the crash log.
        /// </summary>
        public void Dump(string key, object value)
        {
            data.Add(key, value);
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
            Dump(key, value);
        }

        public string Format()
        {
            using (StringWriter writer = new StringWriter())
            {
                writer.WriteLine("===== Exception =====");
                writer.WriteLine(exception.ToString());
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
            Directory.CreateDirectory(directory);
            string filename = Path.Combine(
                directory,
                string.Format("ErrorReport {0}.log", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss fff"))
            );
            File.WriteAllText(filename, Format());
            return filename;
        }
    }
}
