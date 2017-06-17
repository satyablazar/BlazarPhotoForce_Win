using PhotoForce.App_Code;
using System;
using System.Collections;
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
    /// Interaction logic for AddEditEquipmentItems.xaml
    /// </summary>
    public partial class AddEditEquipmentItems : Window
    {
        public AddEditEquipmentItems()
        {
            InitializeComponent();
            this.DataContext = new AddEditEquipmentItemsViewModel();
        }
        //Edit equipment items
        public AddEditEquipmentItems(WorkflowItem tempWorkflowItem)
        {
            InitializeComponent();
            this.DataContext = new AddEditEquipmentItemsViewModel(tempWorkflowItem);
        }
        // Edit Photoshoot equipment items
        public AddEditEquipmentItems(PhotoshootWorkflowItem tempWorkflowItem)
        {
            InitializeComponent();
            this.DataContext = new AddEditEquipmentItemsViewModel(tempWorkflowItem);
        }
        //bulk edit equipment items
        public AddEditEquipmentItems(ArrayList selectedIds)
        {
            InitializeComponent();
            this.DataContext = new AddEditEquipmentItemsViewModel(selectedIds);
        }
    }
}
