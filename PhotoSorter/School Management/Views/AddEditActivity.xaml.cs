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

namespace PhotoForce.School_Management
{
    /// <summary>
    /// Interaction logic for AddEditActivity.xaml
    /// </summary>
    public partial class AddEditActivity : Window
    {
        public AddEditActivity(PhotoForce.App_Code.School tempSchool)
        {
            InitializeComponent();
            this.DataContext = new AddEditActivityViewModel(tempSchool);
        }
        public AddEditActivity(PhotoForce.App_Code.Activity tempSelectedActivity)
        {
            InitializeComponent();
            this.DataContext = new AddEditActivityViewModel(tempSelectedActivity);
        }
    }
}
