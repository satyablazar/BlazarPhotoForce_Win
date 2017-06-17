using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using DevExpress.Xpf.Printing;
using System.Data;
using System.Collections;
using PhotoForce.App_Code;
using PhotoSaver;

namespace PhotoForce.Student_Management
{
    /// <summary>
    /// Interaction logic for StudentQRCodeEightStudentsGroupByTeacher.xaml
    /// </summary>
    public partial class StudentQRCodeEightStudentsGroupByTeacher : Window
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        XtraReportPreviewModel model = new XtraReportPreviewModel();
        public ArrayList arrStudentIds = new ArrayList();
        DataTable dtRetStudent;
        private MW6QRCode.Font QRCodeFontObj = new MW6QRCode.Font();
        #endregion

        #region Constructor
        public StudentQRCodeEightStudentsGroupByTeacher(ArrayList arrStudentID)
        { 
            InitializeComponent();
            arrStudentIds = arrStudentID;
            //XtraReport1 rep = new XtraReport1();
            //rep.DataSource = CreateData();
            //rep.CreateDocument(false);
            //model.Report = rep;
            //preview.Model = model;

            //QRXtraReport rep = new QRXtraReport();
            XtraReport1 rep = new XtraReport1();
            rep.Landscape = true;
            rep.DataSource = CreateData();
            rep.CreateDocument(false);
            model.Report = rep;
            preview.Model = model;


        }
        #endregion

        #region Methods
        List<OneStudentsDetailWithTeacher> CreateData()
        {
            List<OneStudentsDetailWithTeacher> lst = new List<OneStudentsDetailWithTeacher>();
            DataTable dtResults = createDataTable();
            OneStudentsDetailWithTeacher[] data = new OneStudentsDetailWithTeacher[dtResults.Rows.Count];

            for (int i = 0; i < data.Length; i++)
            {
                lst.Add(new OneStudentsDetailWithTeacher(dtResults.Rows[i]["ID"].ToString(), dtResults.Rows[i]["LastName"].ToString() + "," + dtResults.Rows[i]["FirstName"].ToString(), dtResults.Rows[i]["Teacher"].ToString() + ", " + dtResults.Rows[i]["Grade"].ToString(), dtResults.Rows[i]["SchoolName"].ToString() + "," + dtResults.Rows[i]["SchoolYear"].ToString(), dtResults.Rows[i]["Teacher"].ToString()));
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
        public class OneStudentsDetailWithTeacher
        {
            public string ID { get; set; }
            public string FirstLastName { get; set; }
            public string TeacherGrade { get; set; }
            public string SchoolNameYear { get; set; }
            public string TeacherName { get; set; }
            public string Custom5 { get; set; }

            public OneStudentsDetailWithTeacher(string id, string firstlastname, string teachergrade, string schoolnameyear, string teachername)
            {
                ID = id;
                SchoolNameYear = schoolnameyear;
                FirstLastName = firstlastname;
                if (teachergrade == ", ")
                    TeacherGrade = "";
                else
                    TeacherGrade = teachergrade;
                TeacherName = teachername;

                if (string.IsNullOrEmpty(TeacherName))
                    TeacherGrade = "\n No Teacher";
                //Custom5 = tempCustom5;
            }
        }
        #endregion

    }
}
