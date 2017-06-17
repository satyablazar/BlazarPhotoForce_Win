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
    /// Interaction logic for CreateNewBatch.xaml
    /// </summary>
    public partial class CreateNewBatch : Window
    {
        public CreateNewBatch()
        {
            InitializeComponent();
            this.DataContext = new CreateNewBatchViewModel();

            txtBatchName.Focus();            
        }

        public CreateNewBatch(string isFrom, bool isWorkflowCollections)
        {
            InitializeComponent();
            this.DataContext = new CreateNewBatchViewModel(isFrom, isWorkflowCollections);

            txtBatchName.Focus();
        }
        public CreateNewBatch(string isFrom, WorkflowCollection collectionName)
        {
            InitializeComponent();
            this.DataContext = new CreateNewBatchViewModel(isFrom, collectionName);

            txtBatchName.Focus();
        }
    }
}
