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
    /// Interaction logic for AddEditIQAccount.xaml
    /// </summary>
    public partial class AddEditIQAccount : Window
    {
        public AddEditIQAccount()
        {
            InitializeComponent();
            this.DataContext = new AddEditIQAccountViewModel();
        }
        public AddEditIQAccount(IQAccount selectedIQAccount)
        {
            InitializeComponent();
            this.DataContext = new AddEditIQAccountViewModel(selectedIQAccount);
        }
        public AddEditIQAccount(IQPriceSheet selectedIQPriceSheet, IQAccount selectedIQAccount)
        {
            InitializeComponent();
            this.DataContext = new AddEditIQAccountViewModel(selectedIQPriceSheet, selectedIQAccount);
        }
    }
}
