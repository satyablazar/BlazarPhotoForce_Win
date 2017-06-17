using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Data.SqlClient;
using System.Security.Permissions;
using System.IO;
using Microsoft.Win32;
using System.Collections;
using PhotoForce.WorkPlace;
using PhotoForce.App_Code;
using System.ComponentModel;

namespace PhotoForce
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        #region Initialization
        internal bool canConnect = false;
        #endregion

        #region Global Buttons
        bool _isAddToGroupVisible;
        bool _isNewVisible;
        bool _isEditVisible;
        bool _isDeleteVisible;
        bool _isDeleteAllVisible;
        bool _isRemoveFromGrpVisible;
        bool _isDragVisible;
        bool _isSearchVisible;
        bool _isImportFileVisible;
        bool _isRefreshVisible;
        //bool _isStuWithOutImageVisible;
        //bool _isImageWithOutStuVisible;
        bool _isIPTCVisible;
        bool _isAutoAssignClassVisible;
        bool _isAssignStudentVisible;
        bool _isBulkRenameVisible;
        bool _isOpenFolderVisible;
        bool _isOrderButtonsVisible;
        bool _isAddGroupPhotobtnVisible;
        bool _isAssignOrderVisible;
        bool _isEditOrdersVisible;
        bool _isImportOrdersVisible;
        bool _isRenameSourceImagesVisible;
        bool _isAddCollectionButtonVisible;

        public bool isRenameSourceImagesVisible
        {
            get { return _isRenameSourceImagesVisible; }
            set { _isRenameSourceImagesVisible = value; NotifyPropertyChanged("isRenameSourceImagesVisible"); }
        }
        public bool isImportOrdersVisible
        {
            get { return _isImportOrdersVisible; }
            set { _isImportOrdersVisible = value; NotifyPropertyChanged("isImportOrdersVisible"); }
        }
        public bool isEditOrdersVisible
        {
            get { return _isEditOrdersVisible; }
            set { _isEditOrdersVisible = value; NotifyPropertyChanged("isEditOrdersVisible"); }
        }
        public bool isAssignOrderVisible
        {
            get { return _isAssignOrderVisible; }
            set { _isAssignOrderVisible = value; NotifyPropertyChanged("isAssignOrderVisible"); }
        }

        public bool isAddGroupPhotobtnVisible
        {
            get { return _isAddGroupPhotobtnVisible; }
            set { _isAddGroupPhotobtnVisible = value; NotifyPropertyChanged("isAddGroupPhotobtnVisible"); }
        }
        public bool isOrderButtonsVisible   //used to show the buttons when Orders.xaml is in focus,this will also helps to hide when OrderItems.xaml is visible 
        {
            get { return _isOrderButtonsVisible; }
            set { _isOrderButtonsVisible = value; NotifyPropertyChanged("isOrderButtonsVisible"); }
        }
        public bool isOpenFolderVisible
        {
            get { return _isOpenFolderVisible; }
            set { _isOpenFolderVisible = value; NotifyPropertyChanged("isOpenFolderVisible"); }
        }
        public bool isBulkRenameVisible
        {
            get { return _isBulkRenameVisible; }
            set { _isBulkRenameVisible = value; NotifyPropertyChanged("isBulkRenameVisible"); }
        }
        public bool isNewVisible
        {
            get { return _isNewVisible; }
            set { _isNewVisible = value; NotifyPropertyChanged("isNewVisible"); }
        }
        public bool isEditVisible
        {
            get { return _isEditVisible; }
            set { _isEditVisible = value; NotifyPropertyChanged("isEditVisible"); }
        }
        public bool isDeleteVisible
        {
            get { return _isDeleteVisible; }
            set { _isDeleteVisible = value; NotifyPropertyChanged("isDeleteVisible"); }
        }
        public bool isDeleteAllVisible
        {
            get { return _isDeleteAllVisible; }
            set { _isDeleteAllVisible = value; NotifyPropertyChanged("isDeleteAllVisible"); }
        }
        public bool isRemoveFromGrpVisible
        {
            get { return _isRemoveFromGrpVisible; }
            set { _isRemoveFromGrpVisible = value; NotifyPropertyChanged("isRemoveFromGrpVisible"); }
        }
        public bool isDragVisible
        {
            get { return _isDragVisible; }
            set { _isDragVisible = value; NotifyPropertyChanged("isDragVisible"); }
        }
        public bool isSearchVisible
        {
            get { return _isSearchVisible; }
            set { _isSearchVisible = value; NotifyPropertyChanged("isSearchVisible"); }
        }
        public bool isImportFileVisible
        {
            get { return _isImportFileVisible; }
            set { _isImportFileVisible = value; NotifyPropertyChanged("isImportFileVisible"); }
        }
        public bool isRefreshVisible
        {
            get { return _isRefreshVisible; }
            set { _isRefreshVisible = value; NotifyPropertyChanged("isRefreshVisible"); }
        }
        public bool isAddToGroupVisible
        {
            get { return _isAddToGroupVisible; }
            set { _isAddToGroupVisible = value; NotifyPropertyChanged("isAddToGroupVisible"); }
        }
        //public bool isStuWithOutImageVisible
        //{
        //    get { return _isStuWithOutImageVisible; }
        //    set { _isStuWithOutImageVisible = value; NotifyPropertyChanged("isStuWithOutImageVisible"); }
        //}
        //public bool isImageWithOutStuVisible
        //{
        //    get { return _isImageWithOutStuVisible; }
        //    set { _isImageWithOutStuVisible = value; NotifyPropertyChanged("isImageWithOutStuVisible"); }
        //}
        public bool isIPTCVisible
        {
            get { return _isIPTCVisible; }
            set { _isIPTCVisible = value; NotifyPropertyChanged("isIPTCVisible"); }
        }
        public bool isAutoAssignClassVisible
        {
            get { return _isAutoAssignClassVisible; }
            set { _isAutoAssignClassVisible = value; NotifyPropertyChanged("isAutoAssignClassVisible"); }
        }
        public bool isAssignStudentVisible
        {
            get { return _isAssignStudentVisible; }
            set { _isAssignStudentVisible = value; NotifyPropertyChanged("isAssignStudentVisible"); }
        }
        public bool isAddCollectionButtonVisible
        {
            get { return _isAddCollectionButtonVisible; }
            set { _isAddCollectionButtonVisible = value; NotifyPropertyChanged("isAddCollectionButtonVisible"); }
        }
        #endregion

        #region Constructors
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;

            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            try
            {
                string AppDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME;
                if (!Directory.Exists(AppDataPath))
                {
                    Directory.CreateDirectory(AppDataPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        #endregion

        #region Methods
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            clsStatic.WriteExceptionLogXML(e);
            // print out the exception stack trace to a log
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            // hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            base.OnStartup(e);

            #region  code added Abhilasha // Modified By Mohan :-)
            if (!checkDBConnection())
            {
                MessageBoxResult result = MessageBox.Show(errorMessages.NO_DB_PRESENT, "Error");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                try
                {
                    PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    School getFirstSchool = clsSchool.getFirstSchool(db);
                    if (getFirstSchool != null)
                    {
                        clsSchool.defaultSchoolId = getFirstSchool.ID;
                        clsSchool.defaultSchoolName = getFirstSchool.SchoolName;

                        clsSchool.getDefaultSchoolFromRegistry(getFirstSchool.SchoolName, getFirstSchool.ID);   //check for photosorter registry key to get school name and id,if not creates a key.
                    }
                    else
                    {
                        clsSchool.getDefaultSchoolFromRegistry("Dummy", 0); //inserting dummy values 
                    }

                    bool checkForVersionTable = clsDashBoard.checkTableVersion(db);    //checks for versioning table
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var tempAssemblyVersion = assembly.GetName().Version.ToString();
                    int exeMajorVersion = Convert.ToInt32(assembly.GetName().Version.Major);
                    int exeMinorVersion = Convert.ToInt32(assembly.GetName().Version.Minor);
                    object dbVersion = "";
                    int opcode = 0;

                    MessageBoxButton buttons = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Question;
                    string message = errorMessages.DATABASE_UPDATE_CONFIRMATION;

                    tempAssemblyVersion = tempAssemblyVersion.ToString().Replace(".0.0", "");


                    clsVersion.neededUpdate(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsConnectionString.connectionString);

                    try
                    {
                        dbVersion = db.ExecuteQuery<string>("select PSVersion from tblversion").First();
                    }
                    catch (Exception)
                    {
                        dbVersion = db.ExecuteQuery<decimal>("select PSVersion from tblversion").FirstOrDefault().ToString();
                    }
                    if (dbVersion == "0") { MessageBox.Show("Database must have a version. Please contact FreedPhoto."); System.Diagnostics.Process.GetCurrentProcess().Kill(); }

                    int tempDBMajorVersion = Convert.ToInt32(dbVersion.ToString().Split('.')[0]);
                    int tempDBMinorVersion = Convert.ToInt32(dbVersion.ToString().Split('.')[1]);

                    double tempDBVer = Convert.ToDouble(dbVersion.ToString().Replace(".0.0", ""));

                    bool tempResA = false; bool tempResB = false;

                    if (tempDBMajorVersion == 3)
                    {
                        if (tempDBMinorVersion <= 90)
                        {
                            tempResA = true;
                        }
                    }
                    else if(tempDBMajorVersion==4)
                    {
                        if (tempDBMinorVersion > 10 && tempDBMinorVersion < 17)
                        {
                            tempResB = true;
                        }
                    }

                    if (tempResA || tempResB)
                    {                        
                        //WPFMessageBoxResult res = WPFMessageBox.customShow("Freed Photo", "Your DB is " + Convert.ToDouble(dbVersion.ToString().Replace(".0.0", "")) + " and needs to be upgraded. Alternatively, open another database.", "", WPFMessageBoxButtons.UpgradeOpen, WPFMessageBoxImage.Question);
                        
                        CustomMessageBox _objCustomMessageBox = new CustomMessageBox("Your DB is " + Convert.ToDouble(dbVersion.ToString().Replace(".0.0", "")) + " and needs to be upgraded.\n Alternatively, open another database.");
                        _objCustomMessageBox.ShowDialog();

                        string res = ((CustomMessageBoxViewModel)(_objCustomMessageBox.DataContext)).selectedOption;
                        if (res == "OpenDB")
                        {
                            ManageDBConnections _objManageDBConnections = new ManageDBConnections();
                            _objManageDBConnections.ShowDialog();
                            canConnect = ((ManageDBConnectionsViewModel)(_objManageDBConnections.DataContext)).isConnectionStringSelected;
                            if (canConnect == false)
                                System.Diagnostics.Process.GetCurrentProcess().Kill();
                            //else
                                //MessageBox.Show("DB is updated.");
                        }
                        else if (res == "UpgradeDB")
                        {
                            clsVersion.VersionUpdate(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), dbVersion.ToString(), tempAssemblyVersion.Replace(".0.0", ""), 0);

                            //MessageBox.Show("DB is upgraded."); 
                        }
                        else if (res == "Cancel")
                        {
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                        }
                    }
                    else
                    {
                        if (exeMinorVersion > tempDBMinorVersion)
                        {
                            if (MessageBox.Show(message, "Confirmation", buttons, icon) == MessageBoxResult.Yes)
                                clsVersion.VersionUpdate(db, dbVersion.ToString(), tempAssemblyVersion.Replace(".0.0", ""), opcode);
                        }
                        else if (tempDBMinorVersion > exeMinorVersion)
                        {
                            MessageBox.Show(errorMessages.EXECUTE_VERSION_LOWER_THAN_DB, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    clsStatic.WriteExceptionLogXML(ex);
                }
            }
            #endregion
        }
        private bool checkDBConnection()
        {

            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            //string tempConnectionString = @"Data Source=FREED-EDITING-1\\SQLEXPRESS;Initial Catalog=pf4;User ID=sa;Password=scanjet1";
            //using the same connection string in MainWindowViewModel to check wether PF connected to test db or not

            string tempConnectionString = @"Data Source=frvm02\sqlexpress,1522;Initial Catalog=photoforce4;User ID=sa;Password=scanjet88";

            try
            {
                string connectionDetails = clsConnectionString.getConnectionStringFromRegistry();

                //create Neal's connection string if there is no connection string avilable in registry.
                if (connectionDetails == "")
                    clsConnectionString.createNealConnectionString(tempConnectionString, "PhotoSaver.Properties.Settings.freedphotosorterConnectionString");

                canConnect = clsConnectionString.testConnection(clsConnectionString.connectionString);
                if (canConnect == false)
                {
                    ManageDBConnections _objManageDBConnections = new ManageDBConnections();
                    _objManageDBConnections.ShowDialog();
                    canConnect = ((ManageDBConnectionsViewModel)(_objManageDBConnections.DataContext)).isConnectionStringSelected;
                    if (canConnect == false)
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

            return canConnect;
        }
        public void setAllButtonsVisibility()
        {
            isNewVisible = false; isEditVisible = false; isDeleteVisible = false; isAssignStudentVisible = false; isOpenFolderVisible = false;
            isDeleteAllVisible = false; isRemoveFromGrpVisible = false; isDragVisible = false; isSearchVisible = false; isOrderButtonsVisible = false;
            isImportFileVisible = false; isRefreshVisible = false; isBulkRenameVisible = false; isAddGroupPhotobtnVisible = false; isAddToGroupVisible = false;
            isIPTCVisible = false; isAutoAssignClassVisible = false; isAssignOrderVisible = false; isEditOrdersVisible = false; isImportOrdersVisible = false;
            isRenameSourceImagesVisible = false; isAddCollectionButtonVisible = false;
            //isStuWithOutImageVisible = false;     //Deleted after 4.50 as we are using single UI for all photshoot validations //Mohan
            //isImageWithOutStuVisible = false; //we are not using this button
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Events
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // put your tracing or logging code here (I put a message box as an example)
            MessageBox.Show(e.ExceptionObject.ToString());
        }
        #endregion
    }
}
