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

namespace PhotoForce.WorkPlace
{
    /// <summary>
    /// Interaction logic for Export.xaml
    /// </summary>
    public partial class Export : Window
    {
        public Export(Dictionary<int, string> tempStudentImages, ArrayList arrSelectedStudents, ArrayList tempArrGrpId, int countRows, List<App_Code.GroupItem> VisibleData)
        {
            InitializeComponent();
            this.DataContext = new ExportViewModel(tempStudentImages, arrSelectedStudents, tempArrGrpId, countRows, VisibleData);
        }
    }
}
