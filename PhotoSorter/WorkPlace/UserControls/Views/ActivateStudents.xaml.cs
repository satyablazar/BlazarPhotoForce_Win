﻿using DevExpress.Xpf.Core;
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
    /// Interaction logic for ActivateStudents.xaml
    /// </summary>
    public partial class ActivateStudents : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public ActivateStudents()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (File.Exists(appDataPath + "\\" + "DockActivateStudentsViewLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockDeactivateStudents) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "DockActivateStudentsViewLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                if (dockDeactivateStudents.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockDeactivateStudents) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "DockActivateStudentsViewLayout.xml");
                }
            }
        }
    }
}
