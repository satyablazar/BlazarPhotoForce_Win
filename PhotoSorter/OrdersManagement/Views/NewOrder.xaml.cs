using PhotoForce.App_Code;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
        public NewOrder(string isFromOrders)
        {
            InitializeComponent();
            this.DataContext = new EditManualOrderViewModel(isFromOrders);
        }
        //For edit Order (general edit)
        public NewOrder(Order tempOrder, string isFrom)
        {
            InitializeComponent();
            this.DataContext = new EditManualOrderViewModel(tempOrder, isFrom);
        }
        //For Bulk Rename
        public NewOrder(ObservableCollection<Order> tempOrder, string isFromOrders)
        {
            InitializeComponent();
            this.DataContext = new EditManualOrderViewModel(tempOrder, isFromOrders);
        }

        //public NewOrder(ObservableCollection<Order> tempOrder, string str, string isFromOrders)
        //{
        //    InitializeComponent();
        //    this.DataContext = new EditManualOrderViewModel(tempOrder, str, isFromOrders);
        //}

    }
}
