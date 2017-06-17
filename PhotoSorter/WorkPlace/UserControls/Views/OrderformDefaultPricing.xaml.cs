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
    /// Interaction logic for OrderformDefaultPricing.xaml
    /// </summary>
    public partial class OrderformDefaultPricing : UserControl
    {

        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public OrderformDefaultPricing()
        {
            InitializeComponent();
        }

        private void OrderformDefaultPricing_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "OrderformDefaultPricingLayout.xml"))
                //    dgDefaultPricing.RestoreLayoutFromXml(appDataPath + "\\" + "OrderformDefaultPricingLayout.xml");

                if (File.Exists(appDataPath + "\\" + "DockOrderformDefaultPricingViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockDefaultPricing) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockOrderformDefaultPricingViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgDefaultPricing.SaveLayoutToXml(appDataPath + "\\" + "OrderformDefaultPricingLayout.xml");

                if (dockDefaultPricing.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockDefaultPricing) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockOrderformDefaultPricingViewLayout.xml");
                }
            }
        }
    }
}
