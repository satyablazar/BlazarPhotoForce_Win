using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AddToOrders.xaml
    /// </summary>
    public partial class AddToOrders : Window
    {
        public AddToOrders(int orderId,bool isForGroupImage)
        {
            InitializeComponent();
            this.DataContext = new AddToOrdersViewModel(orderId, isForGroupImage);

            txtUniversalStudentSearch.Focus();
        }

        public AddToOrders(int orderDetailId, int SchoolId)
        {
            InitializeComponent();
            this.DataContext = new AddToOrdersViewModel(orderDetailId, SchoolId);

            txtUniversalStudentSearch.Focus();
        }
    }
}
