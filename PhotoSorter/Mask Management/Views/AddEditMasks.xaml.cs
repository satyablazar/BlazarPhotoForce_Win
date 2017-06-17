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

namespace PhotoForce.Mask_Management
{
    /// <summary>
    /// Interaction logic for AddEditMasks.xaml
    /// </summary>
    public partial class AddEditMasks : Window
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        #region Constructors
        public AddEditMasks(int tempMaskId, string tempMaskName)
        {
            InitializeComponent();
            this.DataContext = new AddEditMasksViewModel(tempMaskId, tempMaskName);
        }
        #endregion

        #region Save and Restoring Layout
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(appDataPath + "\\" + "MaskDetailsLayout.xml"))
                    dgMaskDetails.RestoreLayoutFromXml(appDataPath + "\\" + "MaskDetailsLayout.xml");
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                FileStream fstre = new FileStream(appDataPath + "\\" + "MaskDetailsLayout.xml", FileMode.Create);
                dgMaskDetails.AddHandler(DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(GridLayout_AllowProperty));
                dgMaskDetails.SaveLayoutToStream(fstre);
                fstre.Close();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        #endregion

        # region Grid layout allow property
        void GridLayout_AllowProperty(object sender, AllowPropertyEventArgs e)
        {
            e.Allow = e.DependencyProperty != DevExpress.Xpf.Grid.GridControl.FilterStringProperty;
        }
        # endregion
    }
}
