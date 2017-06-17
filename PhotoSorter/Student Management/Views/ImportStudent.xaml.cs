using System;
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
    /// Interaction logic for ImportStudent.xaml
    /// </summary>
    public partial class ImportStudent : Window
    {
        public ImportStudent(int tempSchoolId, int rowHandle, string schoolName)
        {
            InitializeComponent();
            this.DataContext = new ImportStudentViewModel(tempSchoolId, rowHandle, schoolName);
        }
    }
}
