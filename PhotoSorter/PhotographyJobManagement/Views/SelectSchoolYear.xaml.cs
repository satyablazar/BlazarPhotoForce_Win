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
using DevExpress.Xpf.Core.Serialization;
using PhotoForce.App_Code;

namespace PhotoForce.PhotographyJobManagement
{
    /// <summary>
    /// Interaction logic for SelectSeason.xaml
    /// </summary>
    public partial class SelectSchoolYear : Window
    {
        #region Initialization
        //string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        #region Constructor
        //we are not using this Window , as we can directly edit school year by simply double click the photo shoot or by editing the photo shoot .
        public SelectSchoolYear(int tempPhotoJobId)
        {
            //InitializeComponent();
            //this.DataContext = new SelectSchoolYearViewModel(tempPhotoJobId);
        }
        #endregion

        #region Save And Restore Grid layout
        //private void Window_Loaded_1(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (File.Exists(appDataPath + "\\" + "selectReAssignSeasonLayout.xml"))
        //            dgSelectSchoolYear.RestoreLayoutFromXml(appDataPath + "\\" + "selectReAssignSeasonLayout.xml");
        //    }
        //    catch (Exception ex)
        //    { clsStatic.WriteExceptionLogXML(ex); }
        //}

        //private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    try
        //    {
        //        FileStream fstre = new FileStream(appDataPath + "\\" + "selectReAssignSeasonLayout.xml", FileMode.Create);
        //        dgSelectSchoolYear.AddHandler(DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(GridLayout_AllowProperty));
        //        dgSelectSchoolYear.SaveLayoutToStream(fstre);
        //        fstre.Close();
        //    }
        //    catch (Exception ex)
        //    { clsStatic.WriteExceptionLogXML(ex); }
        //}

        //# region Layout property for filter
        //void GridLayout_AllowProperty(object sender, AllowPropertyEventArgs e)
        //{
        //    e.Allow = e.DependencyProperty != DevExpress.Xpf.Grid.GridControl.FilterStringProperty;
        //}
        //# endregion
        #endregion
    }
}
