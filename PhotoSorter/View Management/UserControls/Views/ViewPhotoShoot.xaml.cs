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

namespace PhotoForce.View_Management.UserControls
{
    /// <summary>
    /// Interaction logic for ViewPhotoShoot.xaml
    /// </summary>
    public partial class ViewPhotoShoot : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public ViewPhotoShoot()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            //if (File.Exists(appDataPath + "\\" + "PhotoShootViewLayout.xml"))
            //    dgPhotoShoot.RestoreLayoutFromXml(appDataPath + "\\" + "PhotoShootViewLayout.xml");

            if (File.Exists(appDataPath + "\\" + "DockPhotoShootViewLayout.xml"))
            {
                WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockViewPhotoShoot) as WorkspaceManager;
                workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockPhotoShootViewLayout.xml");
                workspaceManager.ApplyWorkspace("workspace1");
            }
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            //dgPhotoShoot.SaveLayoutToXml(appDataPath + "\\" + "PhotoShootViewLayout.xml");

            if (dockViewPhotoShoot.ActiveDockItem != null)
            {
                WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockViewPhotoShoot) as WorkspaceManager;
                workspaceManager.CaptureWorkspace("workspace1");
                workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockPhotoShootViewLayout.xml");
            }
        }
    }
}
