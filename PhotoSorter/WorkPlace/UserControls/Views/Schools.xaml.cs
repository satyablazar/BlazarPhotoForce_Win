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
    /// Interaction logic for Schools.xaml
    /// </summary>
    public partial class Schools : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public Schools()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "SchoolViewLayout.xml"))
                //    dgSchools.RestoreLayoutFromXml(appDataPath + "\\" + "SchoolViewLayout.xml");
                if (File.Exists(appDataPath + "\\" + "DockSchoolViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockSchools) as WorkspaceManager;                    
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockSchoolViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgSchools.SaveLayoutToXml(appDataPath + "\\" + "SchoolViewLayout.xml");
                if (dockSchools.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockSchools) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockSchoolViewLayout.xml");
                }
            }
        }
    }
}
