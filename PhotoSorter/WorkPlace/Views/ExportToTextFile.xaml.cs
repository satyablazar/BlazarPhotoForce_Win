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

namespace PhotoForce.WorkPlace
{
    /// <summary>
    /// Interaction logic for ExportToTextFile.xaml
    /// </summary>
    public partial class ExportToTextFile : Window
    {
        //we are not using this functioanlity
        //please delete the code , after the confirmation.
        public ExportToTextFile(int tempSchoolId, int tempJobId, int tempShootId, System.Collections.ArrayList tempArrStudents)
        {
            InitializeComponent();
            //this.DataContext=new ExportToTextFileViewModel(tempSchoolId,tempJobId,tempSchoolId,tempArrStudents);
        }
    }
}
