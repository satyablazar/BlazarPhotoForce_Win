
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AddCollectionItems.xaml
    /// </summary>
    public partial class AddCollectionItems : Window
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public AddCollectionItems(int workflowColletionId, string classType)
        {
            InitializeComponent();
            this.DataContext = new AddCollectionItemsViewModel(workflowColletionId, classType);
        }
        public AddCollectionItems(string isFrom,int photoshootId)
        {
            InitializeComponent();
            this.DataContext = new AddCollectionItemsViewModel(isFrom, photoshootId);
        }

        private void addCollectionItems_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (File.Exists(appDataPath + "\\" + "AddCollectionItemsLeftLayout.xml"))
                    dgWorkflows.RestoreLayoutFromXml(appDataPath + "\\" + "AddCollectionItemsLeftLayout.xml");
                if (File.Exists(appDataPath + "\\" + "AddCollectionItemsRightLayout.xml"))
                    dgWorkflowsCollections.RestoreLayoutFromXml(appDataPath + "\\" + "AddCollectionItemsRightLayout.xml");
            }
            else
            {
                dgWorkflows.SaveLayoutToXml(appDataPath + "\\" + "AddCollectionItemsLeftLayout.xml");
                dgWorkflowsCollections.SaveLayoutToXml(appDataPath + "\\" + "AddCollectionItemsRightLayout.xml");
            }
        }
    }
}
