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

namespace PhotoForce.GroupManagement
{
    /// <summary>
    /// Interaction logic for ManageGroups.xaml
    /// </summary>
    public partial class ManageGroups : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public ManageGroups()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "GroupLayout.xml"))
                //    dgGroups.RestoreLayoutFromXml(appDataPath + "\\" + "GroupLayout.xml");
                //if (File.Exists(appDataPath + "\\" + "ClassPhotosLayout.xml"))
                //    dgClassPhotos.RestoreLayoutFromXml(appDataPath + "\\" + "ClassPhotosLayout.xml");
                //if (File.Exists(appDataPath + "\\" + "StudentPhotosLayout.xml"))
                //    dgStudentPhotos.RestoreLayoutFromXml(appDataPath + "\\" + "StudentPhotosLayout.xml");

                if (File.Exists(appDataPath + "\\" + "DockGroupsViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockManageGroups) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockGroupsViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgGroups.SaveLayoutToXml(appDataPath + "\\" + "GroupLayout.xml");
                //dgClassPhotos.SaveLayoutToXml(appDataPath + "\\" + "ClassPhotosLayout.xml");
                //dgStudentPhotos.SaveLayoutToXml(appDataPath + "\\" + "StudentPhotosLayout.xml");

                if (dockManageGroups.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockManageGroups) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockGroupsViewLayout.xml");                    
                }
            }
        }
    }
}
