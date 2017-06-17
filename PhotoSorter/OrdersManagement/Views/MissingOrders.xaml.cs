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
    /// Interaction logic for MissingOrders.xaml
    /// </summary>
    public partial class MissingOrders : Window
    {
        public MissingOrders()
        {
            InitializeComponent();
            this.DataContext = new MissingOrdersViewModel();
        }
        //used to show missing images in COPY RETOUCH IMAGES
        //# also used to display orders those not exported
        public MissingOrders(string missingImages, string isFrom)
        {
            InitializeComponent();
            this.DataContext = new MissingOrdersViewModel(missingImages, isFrom);
        }

        //# used to display failed orders while importing orders
        public MissingOrders(string failedOrders, string callFrom, string destinationPath)
        {
            InitializeComponent();
            this.DataContext = new MissingOrdersViewModel(failedOrders, callFrom, destinationPath);
        }
    }
}
