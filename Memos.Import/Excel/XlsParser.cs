using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.OleDb;
using Memos.Import.Exceptions;
using System.Globalization;

namespace Memos.Import.Excel
{
    public class XlsParser
    {
        private const string FileCannotBeOpen = "Excel file cannot be open";
        private const string GeneralExceptionWhileXlsParse = "Exception during Xls file parsing";
        private const string EmptyXlsFile = "Xls file is empty";
        private const string SheetDoesntExist = "Requested sheet doesn't exist";
        private string _filename = string.Empty;
        private int _skipHeaderNumber = 0;
        private int _sheetNumber = 0;

        /// <summary>
        /// Constructor by FileInfo
        /// </summary>
        /// <param name="fileInfo"></param>
        public XlsParser(FileInfo fileInfo, int skipHeaderNumber, int sheetNumber)
        {
            try
            {
                StreamReader _fileContent = new StreamReader(fileInfo.FullName);
            }
            catch (ParseException ex)
            {
                throw new ParseException(FileCannotBeOpen,ex);
            }
            _filename = fileInfo.FullName;
            _skipHeaderNumber = skipHeaderNumber;
            _sheetNumber = sheetNumber;
        }

        /// <summary>
        /// Parses Excel (xls) file
        /// </summary>
        /// <returns>List of DataRow from Excel file</returns>
        public List<DataRow> Parse()
        {
            List<DataRow> result = new List<DataRow>(100);
            string connect = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _filename + ";Extended Properties=" + (char)34 + "Excel 8.0;HDR=No;IMEX=1;" + (char)34;
            // Create new DataSet to hold information from the worksheet.
            DataSet ds = new DataSet();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connect))
                {
                    using (OleDbCommand cmd = new OleDbCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        //get sheet name
                        DataTable ExcelSheets = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                        if (ExcelSheets.Rows.Count < _sheetNumber + 1)
                            throw new ParseException(SheetDoesntExist);
                        string SpreadSheetName = string.Format("[{0}]", ExcelSheets.Rows[_sheetNumber]["TABLE_NAME"].ToString());

                        //create select query
                        cmd.CommandText = string.Format("SELECT * FROM {0}", SpreadSheetName);
                        OleDbDataAdapter objAdapter = new OleDbDataAdapter();
                        objAdapter.SelectCommand = cmd;
                        // Fill the DataSet with the information from the worksheet.
                        objAdapter.Fill(ds, "XlsImport");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseException(GeneralExceptionWhileXlsParse, ex);
            }
            
            if (ds.Tables.Count == 0)
                throw new ParseException(EmptyXlsFile);

            int rowCount = ds.Tables[0].Rows.Count;
            if (rowCount == 0)
                throw new ParseException(EmptyXlsFile);

            for (int i = 0; i < rowCount; i++)
            {
                if (i < _skipHeaderNumber) continue;
                DataRow row = ds.Tables[0].Rows[i];
                if (IsEmptyRow(row) == false)
                {
                    result.Add(row);
                }
            }

            return result;
        }

        private bool IsEmptyRow(DataRow row)
        {
            foreach (object field in row.ItemArray)
            {
                if (field != null && string.IsNullOrEmpty(field.ToString().Trim()) == false)
                    return false;
            }
            return true;
        }
    }
}
