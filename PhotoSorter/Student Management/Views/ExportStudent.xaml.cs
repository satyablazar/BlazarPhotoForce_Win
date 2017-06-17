using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Interaction logic for ExportStudent.xaml
    /// </summary>
    public partial class ExportStudent : Window
    {
        public ExportStudent(int schoolId, string schoolName, ArrayList tempArrStuId)
        {
            InitializeComponent();
            this.DataContext = new ExportStudentViewModel(schoolId, schoolName, tempArrStuId);
        }

        public ExportStudent(int schoolId, string schoolName, ArrayList arrstuId, ArrayList arrAllFilteredLst)
        {
            InitializeComponent();
            this.DataContext = new ExportStudentViewModel(schoolId, schoolName, arrstuId, arrAllFilteredLst);
        }

        public ExportStudent(int schoolId, string schoolName)
        {
            InitializeComponent();
            this.DataContext = new ExportStudentViewModel(schoolId, schoolName);
        }
    }
}
