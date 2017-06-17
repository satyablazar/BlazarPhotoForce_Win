using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ImportOrdersProgressBar.xaml
    /// </summary>
    public partial class ImportOrdersProgressBar : Window
    {
        public ImportOrdersProgressBar(string folderPath, bool isExcelChecked, bool isGotPhotoChecked, DataTable excelData)
        {
            InitializeComponent();
            this.DataContext = new ImportOrdersProgressBarViewModel(folderPath, isExcelChecked, isGotPhotoChecked,excelData);
        }
    }
}
