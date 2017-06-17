using DevExpress.Xpf.Grid;
using DevExpress.XtraPrinting;
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
using DevExpress.Xpf.Printing;

namespace PhotoForce.Student_Management
{
    /// <summary>
    /// Interaction logic for PrintStudentsQRCode.xaml
    /// </summary>
    public partial class PrintStudentsQRCode : Window
    {
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        ArrayList arrStudentIds = new ArrayList();
        DataTable dtRetStudent;

        public PrintStudentsQRCode(ArrayList arrStudentID)
        {
            InitializeComponent();
            arrStudentIds = arrStudentID;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Create a link and bind it to the PrintPreview instance.
            CollectionViewLink link = new CollectionViewLink();
            preview.Model = new LinkPreviewModel(link);

            // Create an ICollectionView object.
            link.CollectionView = CreateMonthCollectionView();

            // Provide export templates.
            link.DetailTemplate = (DataTemplate)Resources["StudentName"];

            link.Margins = new System.Drawing.Printing.Margins(80, 100, 96, 0);

            link.GroupInfos.Add(new DevExpress.Xpf.Printing.GroupInfo((DataTemplate)Resources["TeacherName"]));

            // Create a document.
            link.CreateDocument(true);
            XtraPageSettingsBase pageSettings = link.PrintingSystem.PageSettings;
        }

        private ICollectionView CreateMonthCollectionView()
        {
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("ID");
            dtResults.Columns.Add("FirstName");
            dtResults.Columns.Add("LastName");
            dtResults.Columns.Add("Teacher");
            dtResults.Columns.Add("Grade");
            dtResults.Columns.Add("StudentID");
            dtResults.Columns.Add("SchoolName");
            dtResults.Columns.Add("Custom5");
            DataRow dr;
            foreach (var item in arrStudentIds)
            {
                dtRetStudent = new DataTable();
                dtRetStudent = clsDashBoard.getAllStudents(db, Convert.ToInt32(item));
                dr = dtResults.NewRow();
                dr = dtRetStudent.Rows[0];
                dtResults.Rows.Add(dr.ItemArray);
            }
            dtResults.AcceptChanges();
            StudentsDetail[] data = new StudentsDetail[dtResults.Rows.Count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new StudentsDetail("*" + dtResults.Rows[i]["ID"].ToString() + "*", dtResults.Rows[i]["FirstName"].ToString() + " " + dtResults.Rows[i]["LastName"].ToString() + ", " + "ID=" + dtResults.Rows[i]["StudentID"].ToString(), dtResults.Rows[i]["Grade"].ToString() + ", " + dtResults.Rows[i]["Teacher"].ToString(), dtResults.Rows[i]["SchoolName"].ToString(), "", "", "", "");
            }
            CollectionViewSource source = new CollectionViewSource();
            List<StudentsDetail> lst = new List<StudentsDetail>();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != null)
                    lst.Add(data[i]);
            }
            source.Source = lst;


            return source.View;

        }

        public class StudentsDetail
        {
            public string ID { get; set; }
            public string FirstLastName { get; set; }
            public string TeacherGrade { get; set; }
            public string SchoolName { get; set; }

            public string ID1 { get; set; }
            public string FirstLastName1 { get; set; }
            public string TeacherGrade1 { get; set; }
            public string SchoolName1 { get; set; }

            public StudentsDetail(string id, string firstlastname, string teachergrade, string schoolname, string id1, string firstlastname1, string teachergrade1, string schoolname1)
            {
                ID = id;
                SchoolName = schoolname;
                FirstLastName = firstlastname;
                FirstLastName1 = firstlastname1;
                if (teachergrade == ", ")
                    TeacherGrade = "";
                else
                    TeacherGrade = teachergrade;
                if (teachergrade1 == ", ")
                    TeacherGrade1 = "";
                else
                    TeacherGrade1 = teachergrade1;
                ID1 = id1;
                SchoolName1 = schoolname1;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsActive)
                this.Close();
        }

    }
}
