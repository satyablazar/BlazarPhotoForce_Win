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
using PhotoForce.App_Code;
using DevExpress.Xpf.Core.Serialization;

namespace PhotoForce.StudentImageManagement
{
    /// <summary>
    /// Interaction logic for CountImages.xaml
    /// </summary>
    public partial class CountImages : Window
    {

        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        #region Constructor
        public CountImages(System.Collections.ArrayList jobId, string selectedGrid, string jobName)
        {
            InitializeComponent();
            this.DataContext = new CountImagesViewModel(jobId, selectedGrid, jobName);
        }
        #endregion

        #region Save And Restoring Layout
        private void windowCountImages_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(appDataPath + "\\" + "GridCountImagesLayout.xml"))
                    dgCountImages.RestoreLayoutFromXml(appDataPath + "\\" + "GridCountImagesLayout.xml");
            }
            catch (Exception ex)
            { clsStatic.WriteExceptionLogXML(ex); }
        }

        private void windowCountImages_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                FileStream fstre = new FileStream(appDataPath + "\\" + "GridCountImagesLayout.xml", FileMode.Create);
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
