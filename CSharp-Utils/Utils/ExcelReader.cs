using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace CSharpUtils.Utils
{
    public static class ExcelReader
    {        
        /// <summary>
        /// Load an excel file.
        /// </summary>
        /// <param name="path">The path to the excel file to load</param>
        public static OleDbConnection LoadFile(string path)
        {
            // Determine the connection string for the input file
            string extension = Path.GetExtension(path);
            string format;
            switch (extension)
            {
                case ".xls":
                    format = "Excel 8.0";
                    break;

                case ".xlsx":
                    format = "Excel 12.0";
                    break;

                default:
                    throw new Exception(string.Format("Unknown input file extension {0}", extension));
            }
            string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='{1};';", path, format);

            // Connect
            OleDbConnection connection = new OleDbConnection(connString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Get a list of all sheets in a file.
        /// </summary>
        /// <param name="connection">The connection to the file</param>
        /// <returns>A list of all sheet names in the file</returns>
        public static List<string> GetAllSheets(OleDbConnection connection)
        {
            List<string> sheets = new List<string>();
            foreach (DataRow row in connection.GetSchema("Tables").Rows)
            {
                sheets.Add((string)row["TABLE_NAME"]);
            }
            return sheets;
        }

        /// <summary>
        /// Get a list of all columns names in a sheet.
        /// </summary>
        /// <param name="connection">The connection to the file</param>
        /// <param name="sheet">The name of the sheet</param>
        /// <returns>A list of all column names in the sheet</returns>
        public static List<string> GetAllColumns(OleDbConnection connection, string sheet)
        {
            List<string> columns = new List<string>();
            OleDbCommand command = new OleDbCommand(string.Format("SELECT TOP 1 * FROM [{0}]", sheet), connection);
            DataTable dt = new DataTable();
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            adapter.Fill(dt);
            foreach (DataColumn column in dt.Columns)
            {
                columns.Add(column.ColumnName);
            }
            return columns;
        }

        /// <summary>
        /// Get a DataTable for a sheet.
        /// </summary>
        /// <param name="connection">The connection to the file</param>
        /// <param name="sheet">The name of the sheet</param>
        /// <returns>A DataTable with the data of the sheet</returns>
        public static DataTable GetDataTable(OleDbConnection connection, string sheet)
        {
            List<string> columns = new List<string>();
            OleDbCommand command = new OleDbCommand(string.Format("SELECT * FROM [{0}]", sheet), connection);
            DataTable dt = new DataTable();
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            adapter.Fill(dt);
            return dt;
        }
    }
}
