using DevExpress.Xpf.Core;
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

namespace PhotoForce.WorkPlace.UserControls
{
    /// <summary>
    /// Interaction logic for DashBoardWorkflowItems.xaml
    /// </summary>
    public partial class DashBoardWorkflowItems : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public DashBoardWorkflowItems()
        {
            InitializeComponent();
        }

        private void mainDashboardWorkflowItems_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "DashBoardWorkflowItemLayout.xml"))
                //    dgWorkflowItemsImport.RestoreLayoutFromXml(appDataPath + "\\" + "DashBoardWorkflowItemLayout.xml");      

                if (File.Exists(appDataPath + "\\" + "DockDashBoardWorkflowItemsViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockWorkflowItems) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockDashBoardWorkflowItemsViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }      
            }
            else
            {
                //dgWorkflowItemsImport.SaveLayoutToXml(appDataPath + "\\" + "DashBoardWorkflowItemLayout.xml");

                if (dockWorkflowItems.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockWorkflowItems) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockDashBoardWorkflowItemsViewLayout.xml");
                }
            }
        }
    }
}
