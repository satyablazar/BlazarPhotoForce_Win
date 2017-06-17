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
    /// Interaction logic for ImageQuixAccounts.xaml
    /// </summary>
    public partial class ImageQuixAccounts : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public ImageQuixAccounts()
        {
            InitializeComponent();
        }

        private void mainImageQuix_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //if (File.Exists(appDataPath + "\\" + "IQAccountsLayout.xml"))
                //    dgImageQuixAccounts.RestoreLayoutFromXml(appDataPath + "\\" + "IQAccountsLayout.xml");
                //if (File.Exists(appDataPath + "\\" + "IQPricesheetLayout.xml"))
                //    dgPricesheets.RestoreLayoutFromXml(appDataPath + "\\" + "IQPricesheetLayout.xml");
                //if (File.Exists(appDataPath + "\\" + "IQVandoSettingsLayout.xml"))
                //    dgIQVandoSettings.RestoreLayoutFromXml(appDataPath + "\\" + "IQVandoSettingsLayout.xml");

                if (File.Exists(appDataPath + "\\" + "DockImageQuixAccountViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockImageQuixAccounts) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockImageQuixAccountViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                //dgImageQuixAccounts.SaveLayoutToXml(appDataPath + "\\" + "IQAccountsLayout.xml");
                //dgPricesheets.SaveLayoutToXml(appDataPath + "\\" + "IQPricesheetLayout.xml");
                //dgIQVandoSettings.SaveLayoutToXml(appDataPath + "\\" + "IQVandoSettingsLayout.xml");

                if (dockImageQuixAccounts.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockImageQuixAccounts) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockImageQuixAccountViewLayout.xml");
                }
            }
        }
    }
}
