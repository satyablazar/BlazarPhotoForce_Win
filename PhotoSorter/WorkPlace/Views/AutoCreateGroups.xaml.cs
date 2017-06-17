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
    /// Interaction logic for AutoCreateGroups.xaml
    /// </summary>
    public partial class AutoCreateGroups : Window
    {
        public AutoCreateGroups(System.Collections.ArrayList tempArrShootId, int tempJobId, string tempJobName, int tempSchoolId, List<PhotoForce.App_Code.StudentImage> tempSelectedImages)
        {
            InitializeComponent();
            this.DataContext = new AutoCreateGroupsViewModel(tempArrShootId, tempJobId, tempJobName, tempSchoolId, tempSelectedImages);
        }
    }
}
