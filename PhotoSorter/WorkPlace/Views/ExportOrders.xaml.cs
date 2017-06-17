using PhotoForce.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoForce.WorkPlace
{
    /// <summary>
    /// Interaction logic for ExportOrders.xaml
    /// </summary>
    public partial class ExportOrders : Window
    {
        public ExportOrders(Dictionary<int, string> tempStudentImages, ArrayList arrStudentImageIds, ArrayList tempSelectedStudImageIds,
             Dictionary<int, string> tempOrderBillingCodeInfo, bool tempHasVisibleData, List<Order> orders, ArrayList orderIds, ArrayList selectedOrderIds)
        {
            InitializeComponent();
            this.DataContext = new ExportViewModel(tempStudentImages, arrStudentImageIds, tempSelectedStudImageIds, tempOrderBillingCodeInfo, tempHasVisibleData,orders, orderIds, selectedOrderIds);
        }
    }
}
