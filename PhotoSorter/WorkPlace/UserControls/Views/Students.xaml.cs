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
    /// Interaction logic for Students.xaml
    /// </summary>
    public partial class Students : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public Students()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "StudentLayout.xml"))
                //    dgStudents.RestoreLayoutFromXml(appDataPath + "\\" + "StudentLayout.xml");
                //if (File.Exists(appDataPath + "\\" + "StudentImagesFromStudentLayout.xml"))
                //    dgStudentPhotos.RestoreLayoutFromXml(appDataPath + "\\" + "StudentImagesFromStudentLayout.xml");

                if (File.Exists(appDataPath + "\\" + "DockStudentsViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockStudents) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockStudentsViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgStudents.SaveLayoutToXml(appDataPath + "\\" + "StudentLayout.xml");
                //dgStudentPhotos.SaveLayoutToXml(appDataPath + "\\" + "StudentImagesFromStudentLayout.xml");

                if (dockStudents.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockStudents) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockStudentsViewLayout.xml");                    
                }
            }
        }
    }
}
