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

namespace PhotoForce.WorkflowManagement
{
    /// <summary>
    /// Interaction logic for AddEditWorkflowItems.xaml
    /// </summary>
    public partial class AddEditWorkflowItems : Window
    {
        public AddEditWorkflowItems()
        {
            InitializeComponent();
            this.DataContext = new AddEditWorkflowItemsViewModel();
        }

        public AddEditWorkflowItems(WorkflowItem tempWorkflowItem)
        {
            InitializeComponent();
            this.DataContext = new AddEditWorkflowItemsViewModel(tempWorkflowItem);
        }
    }
}
