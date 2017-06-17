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

namespace PhotoForce.ImageQuixManagement
{
    /// <summary>
    /// Interaction logic for AddEditPriceSheet.xaml
    /// </summary>
    public partial class AddEditPriceSheet : Window
    {
        public AddEditPriceSheet()
        {
            InitializeComponent();
            this.DataContext = new AddEditPriceSheetViewModel();
        }
        public AddEditPriceSheet(SimplePhotoPriceSheet selectedPriceSheet)
        {
            InitializeComponent();
            this.DataContext = new AddEditPriceSheetViewModel(selectedPriceSheet);
        }
    }
}
