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
    /// Interaction logic for Orders.xaml
    /// </summary>
    public partial class Orders : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public Orders()
        {
            InitializeComponent();
        }

        private void mainOrders_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "OrdersLayout.xml"))
                //    dgOrders.RestoreLayoutFromXml(appDataPath + "\\" + "OrdersLayout.xml");
                //if (File.Exists(appDataPath + "\\" + "OrderItemsLayout.xml"))
                //    dgOrderItems.RestoreLayoutFromXml(appDataPath + "\\" + "OrderItemsLayout.xml");   

                if (File.Exists(appDataPath + "\\" + "DockOrdersViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockOrders) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockOrdersViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgOrders.SaveLayoutToXml(appDataPath + "\\" + "OrdersLayout.xml");
                //dgOrderItems.SaveLayoutToXml(appDataPath + "\\" + "OrderItemsLayout.xml");

                if (dockOrders.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockOrders) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockOrdersViewLayout.xml");
                }
            }
        }
    }
}
