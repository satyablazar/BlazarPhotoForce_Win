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

namespace PhotoForce.Student_Management
{
    /// <summary>
    /// Interaction logic for EditImportBatches.xaml
    /// </summary>
    public partial class EditImportBatches : Window
    {
        public EditImportBatches(System.Data.DataRowView studentImportSelectedItem)
        {
            InitializeComponent();
            this.DataContext = new EditImportBatchesViewModel(studentImportSelectedItem);
        }

        //public EditImportBatches(IQPriceSheet selectedIQPriceSheet, IQAccount selectedIQAccount)
        //{
        //    InitializeComponent();
        //    this.DataContext = new EditImportBatchesViewModel(selectedIQPriceSheet, selectedIQAccount);
        //}
    }
}
