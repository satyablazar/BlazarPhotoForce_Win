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
    /// Interaction logic for RestoreImages.xaml
    /// </summary>
    public partial class RestoreImages : Window
    {
        public RestoreImages(IEnumerable<App_Code.GroupItem> tempStudentGroupsColl, int tempGroupId, string tempGroupName)
        {
            InitializeComponent();
            this.DataContext = new RestoreImagesViewModel(tempStudentGroupsColl, tempGroupId, tempGroupName);
        }
    }
}
