using DevExpress.Xpf.Core.Serialization;
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

namespace PhotoForce.GroupManagement
{
    /// <summary>
    /// Interaction logic for AddStudentsToGroup.xaml
    /// </summary>
    public partial class AddStudentsToGroup : Window
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        #region Constructors
        public AddStudentsToGroup(int sclId, System.Collections.ArrayList arrStdImageIds)
        {
            InitializeComponent();
            this.DataContext = new AddStudentsToGroupViewModel(sclId, arrStdImageIds);
        }
        #endregion

        #region Saving and restoring grid layout
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(appDataPath + "\\" + "AddToStudentLayout.xml"))
                    dgAddStudentsToGroup.RestoreLayoutFromXml(appDataPath + "\\" + "AddToStudentLayout.xml");
            }
            catch (Exception ex)
            { clsStatic.WriteExceptionLogXML(ex); }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                FileStream fstre = new FileStream(appDataPath + "\\" + "AddToStudentLayout.xml", FileMode.Create);
                dgAddStudentsToGroup.AddHandler(DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(GridLayout_AllowProperty));
                dgAddStudentsToGroup.SaveLayoutToStream(fstre);
                fstre.Close();
            }
            catch (Exception ex)
            { clsStatic.WriteExceptionLogXML(ex); }
        }
        void GridLayout_AllowProperty(object sender, AllowPropertyEventArgs e)
        {
            e.Allow = e.DependencyProperty != DevExpress.Xpf.Grid.GridControl.FilterStringProperty;
        }
        #endregion
    }
}
