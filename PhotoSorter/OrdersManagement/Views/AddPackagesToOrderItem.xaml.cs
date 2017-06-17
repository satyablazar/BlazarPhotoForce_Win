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

namespace PhotoForce.OrdersManagement
{
    /// <summary>
    /// Interaction logic for AddPackagesToOrderItem.xaml
    /// </summary>
    public partial class AddPackagesToOrderItem : Window
    {
        public AddPackagesToOrderItem(View_Order tempOrder)
        {
            InitializeComponent();
            this.DataContext = new AddPackagesToOrderItemViewModel(tempOrder);
        }
        public AddPackagesToOrderItem(StudentPhotoOrder tempStudentPhotoOrder)
        {
            InitializeComponent();
            this.DataContext = new AddPackagesToOrderItemViewModel(tempStudentPhotoOrder);
        }
    }
}
