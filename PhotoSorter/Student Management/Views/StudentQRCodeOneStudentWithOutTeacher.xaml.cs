using DevExpress.Xpf.Printing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PhotoForce.App_Code;

namespace PhotoForce.Student_Management
{
    /// <summary>
    /// Interaction logic for StudentQRCodeOneStudentWithOutTeacher.xaml
    /// </summary>
    public partial class StudentQRCodeOneStudentWithOutTeacher : Window
    {
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        //XtraReportPreviewModel model = new XtraReportPreviewModel();
        ArrayList arrStudentIds = new ArrayList();
        DataTable dtRetStudent;
        private MW6QRCode.Font QRCodeFontObj = new MW6QRCode.Font();
        //string[] sortedColumn;

        public StudentQRCodeOneStudentWithOutTeacher(ArrayList arrStudentID)//, string[] studentGridSortedColumn
        { 
            InitializeComponent();
            this.DataContext = this;

            //sortedColumn = studentGridSortedColumn;
            arrStudentIds = arrStudentID;
            CollectionViewLink link = new CollectionViewLink();
            preview.Model = new LinkPreviewModel(link);


            // Create an ICollectionView object.
            link.CollectionView = CreateMonthCollectionView();

            // Provide export templates.
            link.DetailTemplate = (DataTemplate)Resources["QRDetails"];

            // Create a document.
            link.CreateDocument(true);
        }

        private ICollectionView CreateMonthCollectionView()
        {
            // Version :Indicates which version is used.
            int Version = 4;

            // Level
            //  0: L - recovery rate 7%
            //  1: M - recovery rate 15%
            //  2: Q - recovery rate 25%
            //  3: H - recovery rate 30%
            int Level = 3;

            // Mask:Indicates the mask pattern for improving the readability, this parameter can be one of the following values.
            //Value     Comment
            //0         Auto
            //1         Mask 0
            //2         Mask 1
            //3         Mask 2
            //4         Mask 3
            //5         Mask 4
            //6         Mask 5
            //7         Mask 6
            //8         Mask 7
            int Mask = 8;

            DataTable dtResults = createDataTable();
            OneStudentsDetailNoTeacher[] data = new OneStudentsDetailNoTeacher[dtResults.Rows.Count];
            for (int i = 0; i < data.Length; i++)
            {
                // encode data using QRCode
                QRCodeFontObj.Encode(dtResults.Rows[i]["ID"].ToString(), Version, Level, Mask);

                // How many rows?
                int RowCount = QRCodeFontObj.GetRows();

                // Produce string for DataMatrix font
                string EncodedMsg = "" + System.Convert.ToChar(13) + System.Convert.ToChar(10);
                for (int j = 0; j < RowCount; j++)
                {
                    EncodedMsg = EncodedMsg + QRCodeFontObj.GetRowStringAt(j);
                    EncodedMsg = EncodedMsg + System.Convert.ToChar(13) + System.Convert.ToChar(10);
                }
                data[i] = new OneStudentsDetailNoTeacher(EncodedMsg, dtResults.Rows[i]["LastName"].ToString() + ", " + dtResults.Rows[i]["FirstName"].ToString(), dtResults.Rows[i]["Teacher"].ToString() + ", " + dtResults.Rows[i]["Grade"].ToString(), dtResults.Rows[i]["SchoolName"].ToString() + ", " + dtResults.Rows[i]["SchoolYear"].ToString(), dtResults.Rows[i]["Teacher"].ToString(), dtResults.Rows[i]["Custom5"].ToString(), i, dtResults.Rows[i]["Password"].ToString(), dtResults.Rows[i]["Visit"].ToString());
            }
            CollectionViewSource source = new CollectionViewSource();
            List<OneStudentsDetailNoTeacher> lst = new List<OneStudentsDetailNoTeacher>();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != null)
                    lst.Add(data[i]);
            }
            source.Source = lst;

            return source.View;
        }

        public DataTable createDataTable()
        {
            DataTable dtResults = new DataTable();
            DataView dv;
            dtResults.Columns.Add("ID");
            dtResults.Columns.Add("FirstName");
            dtResults.Columns.Add("LastName");
            dtResults.Columns.Add("Teacher");
            dtResults.Columns.Add("Grade");
            dtResults.Columns.Add("StudentID");
            dtResults.Columns.Add("SchoolName");
            dtResults.Columns.Add("Custom5");
            dtResults.Columns.Add("SheetNumber");
            dtResults.Columns.Add("SchoolYear");
            dtResults.Columns.Add("Password");
            dtResults.Columns.Add("Visit");

            string[] columnNames = dtResults.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            DataRow dr;
            int j = 0;
            foreach (var item in arrStudentIds)
            {
                dtRetStudent = new DataTable();
                dtRetStudent = clsDashBoard.getAllStudents(db, Convert.ToInt32(item));
                dr = dtResults.NewRow();
                dr["ID"] = dtRetStudent.Rows[0]["ID"];
                dr["FirstName"] = dtRetStudent.Rows[0]["FirstName"];
                dr["LastName"] = dtRetStudent.Rows[0]["LastName"];
                dr["Teacher"] = dtRetStudent.Rows[0]["Teacher"];
                dr["Grade"] = dtRetStudent.Rows[0]["Grade"];
                dr["StudentID"] = dtRetStudent.Rows[0]["StudentID"];
                dr["Custom5"] = dtRetStudent.Rows[0]["Custom5"];
                dr["SchoolName"] = dtRetStudent.Rows[0]["SchoolName"];
                dr["SchoolYear"] = dtRetStudent.Rows[0]["SchoolYear"];
                dr["Password"] = dtRetStudent.Rows[0]["Password"];
                dr["Visit"] = dtRetStudent.Rows[0]["Visit"];
                dr["SheetNumber"] = ++j;
                dtResults.Rows.Add(dr.ItemArray);

            }
            dv = new DataView(dtResults);
            //if (sortedColumn != null)
            //{
            //    if (sortedColumn.Count() != 0)
            //    {
            //        foreach (string i in sortedColumn)
            //        {
            //            if (!columnNames.Contains(i)) { continue; }
            //            dv.Sort = i;
            //        }
            //    }
            //}

            dtResults = dv.ToTable();
            dtResults.AcceptChanges();
            return dtResults;
        }

        //List<OneStudentsDetailNoTeacher> CreateData()
        //{
        //    List<OneStudentsDetailNoTeacher> lst = new List<OneStudentsDetailNoTeacher>();
        //    DataTable dtResults = createDataTable();
        //    OneStudentsDetailNoTeacher[] data = new OneStudentsDetailNoTeacher[dtResults.Rows.Count];

        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        //string teachGrade = (i % 2 == 0) ? "1" : "2";
        //        lst.Add(new OneStudentsDetailNoTeacher(dtResults.Rows[i]["ID"].ToString(), dtResults.Rows[i]["LastName"].ToString() + "," + dtResults.Rows[i]["FirstName"].ToString(), dtResults.Rows[i]["Teacher"].ToString() + ", " + dtResults.Rows[i]["Grade"].ToString(), dtResults.Rows[i]["SchoolName"].ToString(), dtResults.Rows[i]["Teacher"].ToString(), dtResults.Rows[i]["Cust5"].ToString()));
        //    }
        //    return lst;

        //}

        public class OneStudentsDetailNoTeacher
        {
            public string ID { get; set; }
            public string FirstLastName { get; set; }
            public string TeacherGrade { get; set; }
            public string SchoolNameYear { get; set; }
            public string TeacherName { get; set; }
            public string Custom5 { get; set; }
            public string SheetNumber { get; set; }			
            public string StudentName { get; set; }
            public string Password { get; set; }
            public string VisitSiteAddress { get; set; }
            public OneStudentsDetailNoTeacher(string id, string firstlastname, string teachergrade, string schoolnameYear, string teachername, string tempCustom5, int SheetNo, string tempPassword, string siteAddress)
            {
                SheetNumber = (SheetNo + 1).ToString();
                ID = id;
                SchoolNameYear = schoolnameYear;
                FirstLastName = firstlastname;
                if (teachergrade == ", ")
                    TeacherGrade = "";
                else
                    TeacherGrade = teachergrade;
                TeacherName = teachername;
                Custom5 = tempCustom5;				
				StudentName = firstlastname;
                Password = tempPassword;

                VisitSiteAddress = string.IsNullOrEmpty(siteAddress) ? "www.freedpics.com" : siteAddress;
            }
        }

    }
}
