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
    /// Interaction logic for SearchSchool.xaml
    /// </summary>
    public partial class SearchSchool : Window
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        #region Constructors
        public SearchSchool(string windowName, MainWindow mainWindowTitle)
        {
            InitializeComponent();
            this.DataContext = new SearchSchoolViewModel(windowName, mainWindowTitle);
        }
        public SearchSchool(string windowName)
        {
            InitializeComponent();
            this.DataContext = new SearchSchoolViewModel(windowName);
        }

        public SearchSchool(int maskId, string windowName)
        {
            InitializeComponent();
            this.DataContext = new SearchSchoolViewModel(maskId, windowName);
        }
        public SearchSchool(System.Collections.ArrayList arrStuId, string windowName)
        {
            InitializeComponent();
            this.DataContext = new SearchSchoolViewModel(arrStuId, windowName);
        }
        #endregion

        #region Saving and Restoring Layout
        //private void windowSchoolSearch_Loaded_1(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (File.Exists(appDataPath + "\\" + "SearchSchoolLayout.xml"))
        //            dgSearchSchools.RestoreLayoutFromXml(appDataPath + "\\" + "SearchSchoolLayout.xml");
        //    }
        //    catch (Exception ex)
        //    {
        //        clsStatic.WriteExceptionLogXML(ex);
        //    }
        //}
        //private void windowSchoolSearch_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    try
        //    {
        //        //FileStream _objFileStream = new FileStream(appDataPath + "\\" + "SearchSchoolLayout.xml", FileMode.Create);
        //        //dgSearchSchools.AddHandler(DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(GridLayout_AllowProperty));
        //        dgSearchSchools.SaveLayoutToXml(appDataPath + "\\" + "SearchSchoolLayout.xml");
        //        //_objFileStream.Close();
        //    }
        //    catch (Exception ex)
        //    {
                
        //        clsStatic.WriteExceptionLogXML(ex);
        //    }
        //}
        #endregion

        # region Layout property for filter
        //void GridLayout_AllowProperty(object sender, AllowPropertyEventArgs e)
        //{
        //    e.Allow = e.DependencyProperty != DevExpress.Xpf.Grid.GridControl.FilterStringProperty;
        //}
        # endregion

        private void windowSchoolSearch_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (File.Exists(appDataPath + "\\" + "SearchSchoolLayout.xml"))
                    dgSearchSchools.RestoreLayoutFromXml(appDataPath + "\\" + "SearchSchoolLayout.xml");

            }
            else
            {
                dgSearchSchools.SaveLayoutToXml(appDataPath + "\\" + "SearchSchoolLayout.xml");
            }
        }
    }
}
