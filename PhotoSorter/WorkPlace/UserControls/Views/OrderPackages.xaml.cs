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
    /// Interaction logic for OrderPackages.xaml
    /// </summary>
    public partial class OrderPackages : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public OrderPackages()
        {
            InitializeComponent();
        }
        //private void grid_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        //{
        //    if (e.Column.FieldName == "Total")
        //    {
        //        e.Value = Convert.ToInt32(dgOrderPackagesImport.GetCellValueByListIndex(e.ListSourceRowIndex, "SortOrder")) *
        //            Convert.ToDouble(dgOrderPackagesImport.GetCellValueByListIndex(e.ListSourceRowIndex, "DefaultPrice"));
        //    } CustomUnboundColumnData="grid_CustomUnboundColumnData"
        //}

        private void OrderPackages_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "OrderPackagesLayout.xml"))
                //    dgOrderPackagesImport.RestoreLayoutFromXml(appDataPath + "\\" + "OrderPackagesLayout.xml");
                if (File.Exists(appDataPath + "\\" + "DockOrderPackagesViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockOrderPackages) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockOrderPackagesViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgOrderPackagesImport.SaveLayoutToXml(appDataPath + "\\" + "OrderPackagesLayout.xml");
                if (dockOrderPackages.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockOrderPackages) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockOrderPackagesViewLayout.xml");
                }
            }
        }
    }
}
