using DevExpress.Xpf.Core;
using PhotoForce.App_Code;
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

namespace PhotoForce.OrdersManagement
{
    /// <summary>
    /// Interaction logic for NewManualOrders.xaml
    /// </summary>
    public partial class NewManualOrders : Window
    {
		string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;
		
        public NewManualOrders()
        {
            InitializeComponent();
            this.DataContext = new NewManualOrdersViewModel();
        }
		
		 private void newManualOrders_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {   
                //Rstoring Student Images Grid
                //if (File.Exists(appDataPath + "\\" + "NewManualOrdersStudentImageLayout.xml"))
                //    dgStudentPhotos.RestoreLayoutFromXml(appDataPath + "\\" + "NewManualOrdersStudentImageLayout.xml");

                if (File.Exists(appDataPath + "\\" + "DockNewManualOrdersStudentImageViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockManualOrders) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockNewManualOrdersStudentImageViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgStudentPhotos.SaveLayoutToXml(appDataPath + "\\" + "NewManualOrdersStudentImageLayout.xml");
                if (dockManualOrders.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockManualOrders) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockNewManualOrdersStudentImageViewLayout.xml");
                }
            }
        }

         //private void studentLookUpEditType_PopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
         //{
         //    DevExpress.Xpf.Grid.GridControl grid = studentLookUpEditType.GetGridControl();
         //    if (grid != null) grid.SaveLayoutToXml(appDataPath + "\\" + "StudentLookupInManualOrdersLayout.xml");
         //}

         //private void studentLookUpEditType_PopupOpened(object sender, RoutedEventArgs e)
         //{
         //    //Rstoring Student Lookup Grid
         //    if (File.Exists(appDataPath + "\\" + "StudentLookupInManualOrdersLayout.xml"))
         //    {
         //        DevExpress.Xpf.Grid.GridControl grid = studentLookUpEditType.GetGridControl();
         //        if (grid != null) grid.RestoreLayoutFromXml(appDataPath + "\\" + "StudentLookupInManualOrdersLayout.xml");
         //    }
         //}

         //private void lookUpEditType_PopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
         //{
         //    DevExpress.Xpf.Grid.GridControl grid = BatchlookUpEditType.GetGridControl();
         //    if (grid != null) grid.SaveLayoutToXml(appDataPath + "\\" + "BatchLookupInManualOrdersLayout.xml");
         //}

         //private void lookUpEditType_PopupOpened(object sender, RoutedEventArgs e)
         //{
         //    //Rstoring Batch Lookup Grid
         //    if (File.Exists(appDataPath + "\\" + "BatchLookupInManualOrdersLayout.xml"))
         //    {
         //        DevExpress.Xpf.Grid.GridControl grid = BatchlookUpEditType.GetGridControl();
         //        if (grid != null) grid.RestoreLayoutFromXml(appDataPath + "\\" + "BatchLookupInManualOrdersLayout.xml");
         //    }
         //}       
    }
}
