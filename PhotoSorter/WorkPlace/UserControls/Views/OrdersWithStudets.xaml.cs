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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoForce.WorkPlace.UserControls
{
    /// <summary>
    /// Interaction logic for OrdersWithStudets.xaml
    /// </summary>
    public partial class OrdersWithStudets : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public OrdersWithStudets()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                //if (File.Exists(appDataPath + "\\" + "OrdersByStudentLayout.xml"))
                //    dgOrders.RestoreLayoutFromXml(appDataPath + "\\" + "OrdersByStudentLayout.xml");

                if (File.Exists(appDataPath + "\\" + "DockOrdersByStudentViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockOrdersByStudents) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockOrdersByStudentViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgOrders.SaveLayoutToXml(appDataPath + "\\" + "OrdersByStudentLayout.xml");

                if (dockOrdersByStudents.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockOrdersByStudents) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockOrdersByStudentViewLayout.xml");
                }
            }
        }
    }
}
