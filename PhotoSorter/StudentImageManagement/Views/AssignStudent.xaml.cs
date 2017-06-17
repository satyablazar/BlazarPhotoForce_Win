using System;
using System.Collections.Generic;
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
using DevExpress.Xpf.Core.Serialization;
using System.IO;
using PhotoForce.App_Code;
using DevExpress.Xpf.Core;

namespace PhotoForce.StudentImageManagement
{
    /// <summary>
    /// Interaction logic for AssignStudent.xaml
    /// </summary>
    public partial class AssignStudent : Window
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        #region Constructors
        public AssignStudent(System.Collections.ArrayList tempArrImageId)
        {
            InitializeComponent();
            this.DataContext = new AssignStudentViewModel(tempArrImageId);
        }
        #endregion

        #region Save And Restoring Layout
        //private void Window_Loaded_1(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (File.Exists(appDataPath + "\\" + "AssignStudentLayout.xml"))
        //            dgAssignStudent.RestoreLayoutFromXml(appDataPath + "\\" + "AssignStudentLayout.xml");
        //    }
        //    catch (Exception ex)
        //    { clsStatic.WriteExceptionLogXML(ex); }
        //}

        //private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    try
        //    {
        //        FileStream fstre = new FileStream(appDataPath + "\\" + "AssignStudentLayout.xml", FileMode.Create);
        //        dgAssignStudent.AddHandler(DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(GridLayout_AllowProperty));
        //        dgAssignStudent.SaveLayoutToStream(fstre);
        //        fstre.Close();
        //    }
        //    catch (Exception ex)
        //    { clsStatic.WriteExceptionLogXML(ex); }
        //}
        private void AssignStudent_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (File.Exists(appDataPath + "\\" + "AssignStudentLayout.xml"))
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockAssignStudents) as WorkspaceManager;
                    workspaceManager.LoadWorkspace("workspace1", appDataPath + "\\" + "AssignStudentLayout.xml");
                    workspaceManager.ApplyWorkspace("workspace1");
                }
            }
            else
            {
                if (dockAssignStudents.ActiveDockItem != null)
                {
                    WorkspaceManager workspaceManager = WorkspaceManager.GetWorkspaceManager(dockAssignStudents) as WorkspaceManager;
                    workspaceManager.CaptureWorkspace("workspace1");
                    workspaceManager.SaveWorkspace("workspace1", appDataPath + "\\" + "AssignStudentLayout.xml");
                }
            }
        }
        void GridLayout_AllowProperty(object sender, AllowPropertyEventArgs e)
        {
            e.Allow = e.DependencyProperty != DevExpress.Xpf.Grid.GridControl.FilterStringProperty;
        }
        #endregion

        
    }
}

