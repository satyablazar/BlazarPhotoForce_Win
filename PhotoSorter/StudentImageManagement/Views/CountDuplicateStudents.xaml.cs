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

namespace PhotoForce.StudentImageManagement
{
    /// <summary>
    /// Interaction logic for CountDuplicateStudents.xaml
    /// </summary>
    public partial class CountDuplicateStudents : Window
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        public CountDuplicateStudents(System.Collections.ArrayList jobId, string selectedGrid, string jobName)
        {
            InitializeComponent();
            this.DataContext = new CountImagesViewModel(jobId, selectedGrid, jobName);
        }

        #region Save And Restoring Layout
        private void windowCountImages_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(appDataPath + "\\" + "GridCountStudentsLayout.xml"))
                    dgCountImages.RestoreLayoutFromXml(appDataPath + "\\" + "GridCountStudentsLayout.xml");
            }
            catch (Exception ex)
            { clsStatic.WriteExceptionLogXML(ex); }
        }

        private void windowCountImages_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                FileStream fstre = new FileStream(appDataPath + "\\" + "GridCountStudentsLayout.xml", FileMode.Create);
                dgCountImages.AddHandler(DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(GridLayout_AllowProperty));
                dgCountImages.SaveLayoutToStream(fstre);
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
