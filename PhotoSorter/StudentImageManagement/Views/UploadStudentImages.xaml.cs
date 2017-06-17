using PhotoForce.App_Code;
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
    /// Interaction logic for UploadStudentImages.xaml
    /// </summary>
    public partial class UploadStudentImages : Window
    {
        public UploadStudentImages(string tempSchoolName, PhotoShoot selectedPhotoShoot)
        {
            InitializeComponent();
            this.DataContext = new UploadStudentImagesViewModel(tempSchoolName, selectedPhotoShoot);
        }
    }
}
