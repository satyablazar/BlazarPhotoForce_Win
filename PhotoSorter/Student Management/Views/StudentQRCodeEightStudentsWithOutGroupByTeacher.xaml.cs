using DevExpress.Xpf.Printing;
using PhotoForce.App_Code;
using PhotoSaver;
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

namespace PhotoForce.Student_Management
{
    /// <summary>
    /// Interaction logic for StudentQRCodeEightStudentsWithOutGroupByTeacher.xaml
    /// </summary>
    public partial class StudentQRCodeEightStudentsWithOutGroupByTeacher : Window
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        XtraReportPreviewModel model = new XtraReportPreviewModel();
        ArrayList arrStudentIds = new ArrayList();
        DataTable dtRetStudent;
        private MW6QRCode.Font QRCodeFontObj = new MW6QRCode.Font();
        #endregion

        #region Constructor
        public StudentQRCodeEightStudentsWithOutGroupByTeacher(ArrayList arrStudentID)
        { 
            InitializeComponent();

            arrStudentIds = arrStudentID;
            XtraReport1NoTeacher repteacher = new XtraReport1NoTeacher();
            repteacher.DataSource = CreateData();
            repteacher.CreateDocument(false);
            model.Report = repteacher;
            preview.Model = model;
        }
        #endregion

        #region Methods
        List<EightStudentsDetailWithOutTeacher> CreateData()
        {
            List<EightStudentsDetailWithOutTeacher> lst = new List<EightStudentsDetailWithOutTeacher>();
            DataTable dtResults = createDataTable();
            EightStudentsDetailWithOutTeacher[] data = new EightStudentsDetailWithOutTeacher[dtResults.Rows.Count];

            for (int i = 0; i < data.Length; i++)
            {
                lst.Add(new EightStudentsDetailWithOutTeacher(dtResults.Rows[i]["ID"].ToString(), dtResults.Rows[i]["LastName"].ToString() + "," + dtResults.Rows[i]["FirstName"].ToString(), dtResults.Rows[i]["Teacher"].ToString() + ", " + dtResults.Rows[i]["Grade"].ToString(), dtResults.Rows[i]["SchoolName"].ToString() + "," + dtResults.Rows[i]["SchoolYear"].ToString(), dtResults.Rows[i]["Teacher"].ToString()));
            }

            return lst;

        }

        private DataTable createDataTable()
        {
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("ID");
            dtResults.Columns.Add("FirstName");
            dtResults.Columns.Add("LastName");
            dtResults.Columns.Add("Teacher");
            dtResults.Columns.Add("Grade");
            dtResults.Columns.Add("SchoolName");
            dtResults.Columns.Add("SchoolYear");
            //dtResults.Columns.Add("Custom5");
            //dtResults.Columns.Add("SheetNumber");

            //for (int i = 0; i < 8; i++)
            //{
            //    DataRow dr = dtResults.NewRow();
            //    foreach (DataColumn dc in dtResults.Columns)
            //    {
            //        if (dc.ToString() == "ID") { dr[dc.ToString()] = "TestID" + i; }
            //        else if (dc.ToString() == "Teacher") { dr[dc.ToString()] = "Teacher" + i; }
            //        else if (dc.ToString() == "SchoolName") { dr[dc.ToString()] = "SchoolName" + i; }
            //        else if (dc.ToString() == "FirstName") { dr[dc.ToString()] = "FirstName" + i; }
            //        else if (dc.ToString() == "LastName") { dr[dc.ToString()] = "LastName" + i; }
            //        else dr[dc.ToString()] = "Test" + i;
            //    }

            //    dtResults.Rows.Add(dr);
            //}

            foreach (var item in arrStudentIds)
            {
                dtRetStudent = new DataTable();
                dtRetStudent = clsDashBoard.getAllStudents(db, Convert.ToInt32(item));

                DataRow dr = dtResults.NewRow();
                foreach (DataColumn dc in dtResults.Columns)
                {
                    //dr["ID"] = dtRetStudent.Rows[0]["ID"];
                    //dr["FirstName"] = dtRetStudent.Rows[0]["FirstName"];
                    //dr["LastName"] = dtRetStudent.Rows[0]["LastName"];
                    //dr["Teacher"] = dtRetStudent.Rows[0]["Teacher"];
                    //dr["Grade"] = dtRetStudent.Rows[0]["Grade"];
                    //dr["StudentID"] = dtRetStudent.Rows[0]["StudentID"];
                    //dr["Custom5"] = dtRetStudent.Rows[0]["Custom5"];
                    //dr["SchoolName"] = dtRetStudent.Rows[0]["SchoolName"];


                    if (dc.ToString() == "ID") { dr[dc.ToString()] = dtRetStudent.Rows[0]["ID"]; }
                    else if (dc.ToString() == "FirstName") { dr[dc.ToString()] = dtRetStudent.Rows[0]["FirstName"]; }
                    else if (dc.ToString() == "LastName") { dr[dc.ToString()] = dtRetStudent.Rows[0]["LastName"]; }
                    else if (dc.ToString() == "Teacher") { dr[dc.ToString()] = dtRetStudent.Rows[0]["Teacher"]; }
                    else if (dc.ToString() == "Grade") { dr[dc.ToString()] = dtRetStudent.Rows[0]["Grade"]; }
                    else if (dc.ToString() == "StudentID") { dr[dc.ToString()] = dtRetStudent.Rows[0]["StudentID"]; }
                    else if (dc.ToString() == "Custom5") { dr[dc.ToString()] = dtRetStudent.Rows[0]["Custom5"]; }
                    else if (dc.ToString() == "SchoolName") { dr[dc.ToString()] = dtRetStudent.Rows[0]["SchoolName"]; }
                    else if (dc.ToString() == "SchoolYear") { dr[dc.ToString()] = dtRetStudent.Rows[0]["SchoolYear"]; }
                    else dr[dc.ToString()] = dtRetStudent.Rows[0]["ID"];
                }
                dtResults.Rows.Add(dr.ItemArray);

            }

            return dtResults;
        }
        #endregion

        #region classes
        public class EightStudentsDetailWithOutTeacher
        {
            public string ID { get; set; }
            public string FirstLastName { get; set; }
            public string TeacherGrade { get; set; }
            public string SchoolNameYear { get; set; }
            public string TeacherName { get; set; }
            public string Custom5 { get; set; }

            public EightStudentsDetailWithOutTeacher(string id, string firstlastname, string teachergrade, string schoolnameyear, string teachername)
            {
                ID = id;
                SchoolNameYear = schoolnameyear;
                FirstLastName = firstlastname;
                if (teachergrade == ", ")
                    TeacherGrade = "";
                else
                    TeacherGrade = teachergrade;
                TeacherName = teachername;
                //Custom5 = tempCustom5;
            }
        }
        #endregion
    }
}
