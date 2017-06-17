using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.NavBar;
using PhotoForce.App_Code;
using PhotoForce.GroupManagement;
using PhotoForce.View_Management.UserControls;
using PhotoForce.WorkPlace.UserControls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoForce
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
        DashBoard _objDashBoard = new DashBoard(1);
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            DevExpress.Xpf.Core.ThemeManager.ApplicationThemeName = "Office2010Black";
            this.DataContext = new MainWindowViewModel();
            ps3NavBarControl.Loaded += ps3NavBarControl_Loaded;
            ps3NavBarControl.ActiveGroup = ps3NavBarControl.Groups[0];  //setting active group to "DashBoard"
        }
        #endregion

        #region events
        //collapsing narbargroup header
        void ps3NavBarControl_Loaded(object sender, RoutedEventArgs e)
        {
            NavBarGroupHeader header = LayoutHelper.FindElementByType<NavBarGroupHeader>(ps3NavBarControl);
            if(header != null)
                header.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void PhotoSorter_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //commented by mohan ; created a new button in validation to check this.
                //string checkForRenameSourceImages = ((DashBoardViewModel)(_objDashBoard.DataContext)).checkForRenameSourceImages();
                //if (!string.IsNullOrEmpty(checkForRenameSourceImages))
                //{
                //    string message = "Images have not been renamed for the following photoshoots: \n\n" + checkForRenameSourceImages + Environment.NewLine + "Do you want to do this before closing the application.";
                //    string caption = "Confirmation";
                //    MessageBoxButton buttons = MessageBoxButton.YesNo;
                //    MessageBoxImage icon = MessageBoxImage.Question;
                //    if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                //    {
                //        e.Cancel = true;
                //        return;
                //    }
                //}

                ((MainWindowViewModel)(this.DataContext)).deleteImages();
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        void Grid_AllowProperty(object sender, AllowPropertyEventArgs e)
        {
            e.Allow = e.DependencyProperty != DevExpress.Xpf.Grid.GridControl.FilterStringProperty;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
        #endregion


    }
}
