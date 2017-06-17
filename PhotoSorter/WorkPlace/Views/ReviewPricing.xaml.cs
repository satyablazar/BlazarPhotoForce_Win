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

namespace PhotoForce.WorkPlace
{
    /// <summary>
    /// Interaction logic for ReviewPricing.xaml
    /// </summary>
    public partial class ReviewPricing : Window
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        #region Constructors
        public ReviewPricing(int schoolId)
        {
            InitializeComponent();
            this.DataContext = new ReviewPricingViewModel(schoolId);
        }
        #endregion

        #region Saving and restore grid layout
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(appDataPath + "\\" + "ReviewPricingLayout.xml"))
                    gcPackages.RestoreLayoutFromXml(appDataPath + "\\" + "ReviewPricingLayout.xml");
            }
            catch (Exception ex)
            { clsStatic.WriteExceptionLogXML(ex); }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileStream fstre = new FileStream(appDataPath + "\\" + "ReviewPricingLayout.xml", FileMode.Create);
            gcPackages.AddHandler(DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(GridLayout_AllowProperty));
            gcPackages.SaveLayoutToStream(fstre);
            fstre.Close();
        }
        # region Layout property for filter
        void GridLayout_AllowProperty(object sender, AllowPropertyEventArgs e)
        {
            e.Allow = e.DependencyProperty != DevExpress.Xpf.Grid.GridControl.FilterStringProperty;
        }
        #endregion
        #endregion
    }
}
