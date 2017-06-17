using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using System.IO;
using PhotoForce.App_Code;
using PhotoForce.MVVM;

namespace PhotoForce.Error_Management
{
    public class ShowErrorsViewModel : ViewModelBase
    {
        #region Initialization
        clsStatic objStatic = new clsStatic();
        #endregion

        #region Properties
        private DataView _errorLogData;

        public DataView errorLogData
        {
            get { return _errorLogData; }
            set { _errorLogData = value; NotifyPropertyChanged("errorLogData"); }
        }
        #endregion

        #region Constructors
        public ShowErrorsViewModel()
        {
            loadData();
        }
        public ShowErrorsViewModel(string tempRenameImages)
        {
            loadDataRename(tempRenameImages);
        }
        #endregion

        #region Methods
        public void loadData()
        {
            XDocument obj = XDocument.Load(objStatic.ErrorLogXML);
            var pdfAuthPath = obj.Descendants("error");
            var aa = pdfAuthPath.ToList();
            DataTable completeDt = aa.ToDataTable();
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Student");
            dt.Columns.Add("Grade");
            dt.Columns.Add("Message");
            dt.Columns.Add("DateTime");
            foreach (DataRow dataSources in completeDt.Rows)
            {
                string[] ew = dataSources["Parameters"].ToString().Split(',');
                DataRow dr = dt.NewRow();
                dr["Student"] = ew[0];
                dr["Grade"] = ew[1];
                dr["Message"] = dataSources["Message"];
                dr["DateTime"] = dataSources["dateTime"];
                dt.Rows.Add(dr);
            }
            errorLogData = dt.DefaultView;
        }
        public void loadDataRename(string type)
        {            
            if (type == "Import Images")
                loadDataStudentImages();
            else
                errorLogData = (DataView)clsDashBoard.getErrorLogRenameImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));  
        }
        public void loadDataStudentImages()
        {
            XDocument obj = XDocument.Load(objStatic.ErrorLogXML);
            var pdfAuthPath = obj.Descendants("error");
            var aa = pdfAuthPath.ToList();
            DataTable completeDt = aa.ToDataTable();
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Source");
            dt.Columns.Add("Method Name");
            dt.Columns.Add("Message");
            dt.Columns.Add("DateTime");
            foreach (DataRow dataSources in completeDt.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["Source"] = dataSources["Source"];
                dr["Method Name"] = dataSources["MethodName"];
                dr["Message"] = dataSources["Message"];
                dr["DateTime"] = dataSources["dateTime"];
                dt.Rows.Add(dr);
            }
            errorLogData = dt.DefaultView;
        }
        #endregion
    }

    public static class XElementExtensions
    {
        public static DataTable ToDataTable(this XElement element)
        {
            DataSet ds = new DataSet();
            string rawXml = element.ToString();
            ds.ReadXml(new StringReader(rawXml));
            return ds.Tables[0];
        }

        public static DataTable ToDataTable(this IEnumerable<XElement> elements)
        {
            return ToDataTable(new XElement("Root", elements));
        }
    }
}
