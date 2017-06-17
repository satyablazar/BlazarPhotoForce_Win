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

namespace PhotoForce.WorkflowManagement
{
    /// <summary>
    /// Interaction logic for EditWorkflowItems.xaml
    /// </summary>
    public partial class EditPhotoshootWorkflowItems : Window
    {
        public EditPhotoshootWorkflowItems(ObservableCollection<PhotoshootWorkflowItem> selectedWorkflowItems)
        {
            InitializeComponent();
            this.DataContext = new EditPhotoshootWorkflowItemsViewModel(selectedWorkflowItems);
        }

        public EditPhotoshootWorkflowItems(PhotoshootWorkflowItem selectedWorkflow)
        {
            InitializeComponent();
            this.DataContext = new EditPhotoshootWorkflowItemsViewModel(selectedWorkflow);
        }
    }
}
