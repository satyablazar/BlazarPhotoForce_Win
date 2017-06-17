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
    /// Interaction logic for BulkRenameStudentImage.xaml
    /// </summary>
    public partial class BulkRenameStudentImage : Window
    {
        public BulkRenameStudentImage(System.Collections.ArrayList tempStuImageId)
        {
            InitializeComponent();
            this.DataContext = new BulkRenameStudentImageViewModel(tempStuImageId);
        }
        public BulkRenameStudentImage(int tempStuImgId)
        {
            InitializeComponent();
            this.DataContext = new BulkRenameStudentImageViewModel(tempStuImgId);
        }
    }
}
