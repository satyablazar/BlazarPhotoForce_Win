using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Excel;
using PhotoForce.App_Code;

namespace PhotoForce.Helpers
{
    class DataLoader
    {
        public static DataTable ReadTextFile(string filePath)
        {
            string line;
            DataTable dt = new DataTable();
            int rowCount = 0;
            //int colCount = 0;
            // Read the file and display it line by line.
            using (StreamReader file = new StreamReader(@"" + filePath))
            {
                while ((line = file.ReadLine()) != null)
                {

                    DataRow dr = dt.NewRow();
                    char[] delimiters = new char[] { '\t' };
                    string[] parts = line.Split(delimiters, StringSplitOptions.None);
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (rowCount == 0)
                        {
                            dt.Columns.Add(parts[i].Trim());
                        }
                        else
                        {
                            if (i == 19) { continue; } //for olderr than 4.34 dbs ,while creating text file in export groups it is adding one tab at the end which causes a un handled exception.
                            dr[i] = parts[i].Trim();
                        }

                    }
                    if (rowCount > 0)
                    {
                        dt.Rows.Add(dr);
                    }
                    rowCount++;
                }

                file.Close();
            }
            // Suspend the screen.
            return dt;
        }

        public static DataTable getDataTableFromExcel(string fullFileName)
        {
            DataSet ds = new DataSet();
            try
            {
                FileStream stream = File.Open(fullFileName, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader;
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                if (fullFileName.EndsWith(".xls"))
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                //...
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                else
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);               
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                
                //...
                //3. DataSet - Create column names from first row
                excelReader.IsFirstRowAsColumnNames = true;
                ds = excelReader.AsDataSet();

                //4. Free resources (IExcelDataReader is IDisposable)
                excelReader.Close();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return null;
            }
            return ds.Tables[0];
        }
    }
}
