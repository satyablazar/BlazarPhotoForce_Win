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
    /// Interaction logic for LockedPhotos.xaml
    /// </summary>
    public partial class LockedPhotos : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public LockedPhotos()
        {
            InitializeComponent();
        }
        private void LockedPhotos_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "LockedPhotosLayout.xml"))
                //    dgLockedPhoto.RestoreLayoutFromXml(appDataPath + "\\" + "LockedPhotosLayout.xml");

                if (File.Exists(appDataPath + "\\" + "DockLockedPhotosViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockLockedPhotos) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockLockedPhotosViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgLockedPhoto.SaveLayoutToXml(appDataPath + "\\" + "LockedPhotosLayout.xml");

                if (dockLockedPhotos.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockLockedPhotos) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockLockedPhotosViewLayout.xml");
                }
            }
        }
    }
}
