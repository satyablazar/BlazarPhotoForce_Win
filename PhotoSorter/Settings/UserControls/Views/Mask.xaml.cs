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

namespace PhotoForce.Settings.UserControls
{
    /// <summary>
    /// Interaction logic for Mask.xaml
    /// </summary>
    public partial class Mask : UserControl
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        #region Constructors
        public Mask()
        {
            InitializeComponent();
        }
        #endregion

        #region Restore Layout
        private void Mask_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (File.Exists(appDataPath + "\\" + "MaskDetailsLayout.xml"))
                dgAllMaskDetails.RestoreLayoutFromXml(appDataPath + "\\" + "MaskDetailsLayout.xml");
        }
        #endregion

        private void Masks_Unloaded_1(object sender, RoutedEventArgs e)
        {
            dgAllMaskDetails.SaveLayoutToXml(appDataPath + "\\" + "MaskDetailsLayout.xml");
        }
    }
}
