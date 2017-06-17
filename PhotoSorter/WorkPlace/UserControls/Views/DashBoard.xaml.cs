using DevExpress.Xpf.Core;
using PhotoForce.MVVM;
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
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public DashBoard(string selectedGridName)
        {
            InitializeComponent();
        }

        public DashBoard()
        {
            InitializeComponent();
        }

        //used in MainWindow to restore data grid layout
        public DashBoard(int temp)
        {
            InitializeComponent();
        }

        private void mainDashboard_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "PhotoShootLayout.xml"))
                //    dgPhotoShoot.RestoreLayoutFromXml(appDataPath + "\\" + "PhotoShootLayout.xml");
                //if (File.Exists(appDataPath + "\\" + "StudentImageLayout.xml"))
                //    dgStudentPhotos.RestoreLayoutFromXml(appDataPath + "\\" + "StudentImageLayout.xml");
                //if (File.Exists(appDataPath + "\\" + "WorkflowItemsLayout.xml"))
                //    dgWorkflowItemsImport.RestoreLayoutFromXml(appDataPath + "\\" + "WorkflowItemsLayout.xml");

                if (File.Exists(appDataPath + "\\" + "DockDashBoardPhotoShootViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockPhotoShoot) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockDashBoardPhotoShootViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgPhotoShoot.SaveLayoutToXml(appDataPath + "\\" + "PhotoShootLayout.xml");
                //dgStudentPhotos.SaveLayoutToXml(appDataPath + "\\" + "StudentImageLayout.xml");
                //dgWorkflowItemsImport.SaveLayoutToXml(appDataPath + "\\" + "WorkflowItemsLayout.xml");
                if (dockPhotoShoot.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockPhotoShoot) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockDashBoardPhotoShootViewLayout.xml");
                }
            }
        }
    }
}
