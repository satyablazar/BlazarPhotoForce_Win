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
    /// Interaction logic for UniversalStudentSearch.xaml
    /// </summary>
    public partial class UniversalStudentSearch : UserControl
    {
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;

        public UniversalStudentSearch()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (File.Exists(appDataPath + "\\" + "UniversalStudentLayout.xml"))
                    dgStudents.RestoreLayoutFromXml(appDataPath + "\\" + "UniversalStudentLayout.xml");
                if (File.Exists(appDataPath + "\\" + "UniversalStudentImagesLayout.xml"))
                    dgStudentPhotos.RestoreLayoutFromXml(appDataPath + "\\" + "UniversalStudentImagesLayout.xml");
            }
            else
            {
                dgStudents.SaveLayoutToXml(appDataPath + "\\" + "UniversalStudentLayout.xml");
                dgStudentPhotos.SaveLayoutToXml(appDataPath + "\\" + "UniversalStudentImagesLayout.xml");
            }
        }
    }
}
