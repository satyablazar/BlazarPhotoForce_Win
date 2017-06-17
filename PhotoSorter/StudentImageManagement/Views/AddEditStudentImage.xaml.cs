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

namespace PhotoForce.StudentImageManagement
{
    /// <summary>
    /// Interaction logic for AddEditStudentImage.xaml
    /// </summary>
    public partial class AddEditStudentImage : Window
    {
        public AddEditStudentImage(int tempStuImgId, int tempSchoolId, int tempPhotoShootId)
        {
            InitializeComponent();
            this.DataContext = new AddEditStudentImageViewModel(tempStuImgId, tempSchoolId, tempPhotoShootId);
        }
    }
}
