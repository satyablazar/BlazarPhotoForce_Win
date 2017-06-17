using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using PhotoForce.Connection_Management;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Windows;
using PhotoForce.Extensions;

namespace PhotoForce.WorkPlace
{
    public class ManageDBConnectionsViewModel : ViewModelBase
    {
        #region Initialization
        public bool isConnectionStringSelected = false;   //used in app.xaml.cs to check wether connection string is selected or not
        bool tempTestConnection = false;
        AddNewConnection _objAddNewConnection;
        public bool isFromOK = false; //NUnitTesting
        #endregion

        #region Properties
        private List<Connections> _dgDBConnectionsData;
        private Connections _selectedConnection;

        public Connections selectedConnection
        {
            get { return _selectedConnection; }
            set { _selectedConnection = value; NotifyPropertyChanged("selectedConnection"); }
        }
        public List<Connections> dgDBConnectionsData
        {
            get { return _dgDBConnectionsData; }
            set { _dgDBConnectionsData = value; NotifyPropertyChanged("dgDBConnectionsData"); }
        }

        #endregion

        #region Constructors
        public ManageDBConnectionsViewModel()
        {
            bindGrid();
        }
        #endregion

        #region Commands
        public RelayCommand NewConnectionCommand
        {
            get
            {
                return new RelayCommand(newConnection);
            }
        }
        public RelayCommand EditConnectionCommand
        {
            get
            {
                return new RelayCommand(editConnection);
            }
        }
        public RelayCommand TestConnectionCommand
        {
            get
            {
                return new RelayCommand(testConnection);
            }
        }
        public RelayCommand SelectOKCommand
        {
            get
            {
                return new RelayCommand(selectOK);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        #endregion

        #region Methods
        private void newConnection()
        {
            _objAddNewConnection = new AddNewConnection();
            _objAddNewConnection.ShowDialog();
            if (((AddNewConnectionViewModel)(_objAddNewConnection.DataContext)).isSave) //is there any best way to do this in MVVM??
            {
                bindGrid();
            }
        }
        private void editConnection()
        {
            if (selectedConnection != null)
            {
                _objAddNewConnection = new AddNewConnection(selectedConnection);
                _objAddNewConnection.ShowDialog();
                if (((AddNewConnectionViewModel)(_objAddNewConnection.DataContext)).isSave)
                {
                    bindGrid();
                }
            }
        }
        private void testConnection()
        {
            testConnectionString();
            if (tempTestConnection)
            {
                MVVMMessageService.ShowMessage(errorMessages.TEST_CONNECTION_SUCCESFUL);
            }
        }
        private void selectOK()
        {
            isFromOK = true;
            testConnectionString();
            if (tempTestConnection)
            {
                try
                {
                    //have to get the version of DB
                    PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(selectedConnection.ConnectionString);
                    string dbVersion;
                    try
                    {
                        dbVersion = db.ExecuteQuery<string>("select PSVersion from tblversion").First();
                    }
                    catch (Exception)
                    {
                        dbVersion = db.ExecuteQuery<decimal>("select PSVersion from tblversion").FirstOrDefault().ToString();
                    }

                    if (dbVersion == "0") { MVVMMessageService.ShowMessage("Database must have a version. Please contact FreedPhoto."); System.Diagnostics.Process.GetCurrentProcess().Kill(); }

                    double tempDBVersion = Convert.ToDouble(dbVersion.ToString().Replace(".0.0", ""));

                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var tempAssemblyVersion = assembly.GetName().Version.ToString();

                    if ((tempDBVersion <= 3.90) || (tempDBVersion > 4.10 && tempDBVersion < 4.17))
                    {

                        //WPFMessageBoxResult res = WPFMessageBox.customShow("Freed Photo", "Selected DB is " + tempDBVersion + " and needs to be upgraded. Alternatively, select another database.", "", WPFMessageBoxButtons.UpgradeChange, WPFMessageBoxImage.Question);

                        CustomMessageBox _objCustomMessageBox = new CustomMessageBox("Selected DB is " + tempDBVersion + " and needs to be upgraded.\n Alternatively, select another database.");
                        _objCustomMessageBox.ShowDialog();

                        string res = ((CustomMessageBoxViewModel)(_objCustomMessageBox.DataContext)).selectedOption;

                        if (res == ConnectionState.OpenDB.ToString())
                        {

                        }
                        else if (res == ConnectionState.UpgradeDB.ToString())
                        {
                            string caption2 = "Confirmation";
                            string message2 = "This operation is not reversible. Are you sure?";
                            System.Windows.MessageBoxButton buttons2 = System.Windows.MessageBoxButton.YesNo;
                            System.Windows.MessageBoxImage icon2 = System.Windows.MessageBoxImage.Question;
                            if (MVVMMessageService.ShowMessage(message2, caption2, buttons2, icon2) == System.Windows.MessageBoxResult.Yes)
                            {
                                clsVersion.neededUpdate(new PhotoSorterDBModelDataContext(selectedConnection.ConnectionString), selectedConnection.ConnectionString);   //checking for tblVersion and Users tables scripts

                                clsVersion.VersionUpdate(new PhotoSorterDBModelDataContext(selectedConnection.ConnectionString), dbVersion.ToString(), tempAssemblyVersion.Replace(".0.0", ""), 0);

                                clsConnectionString.connectionString = selectedConnection.ConnectionString; //updating connection string
                                clsConnectionString.updateCurrentConnectionString(selectedConnection.Name); //updating connection in Registry which we can used when user opens the program next time
                                isConnectionStringSelected = true;
                                DialogResult = false;

                                //MVVMMessageService.ShowMessage("DB is upgraded.");
                            }
                        }
                        else if (res == ConnectionState.Cancel.ToString())
                        {
                            DialogResult = false;
                        }
                    }
                    else
                    {
                        MessageBoxButton buttons = MessageBoxButton.OK;
                        MessageBoxImage icon = MessageBoxImage.Information;

                        int tempDBMinorVersion = Convert.ToInt32(dbVersion.ToString().Split('.')[1]);
                        int exeMinorVersion = Convert.ToInt32(assembly.GetName().Version.Minor);

                        if (exeMinorVersion > tempDBMinorVersion)
                        {
                            if (MVVMMessageService.ShowMessage("The database has now been set to " + selectedConnection.Name + ".\nThe program will now perform the upgrades.", "Information", buttons, icon) == MessageBoxResult.OK)
                                clsVersion.VersionUpdate(db, dbVersion.ToString(),  tempAssemblyVersion.Replace(".0.0", ""), 0);
                        }
                        else if (tempDBMinorVersion > exeMinorVersion)
                        {
                            MessageBox.Show(errorMessages.EXECUTE_VERSION_LOWER_THAN_DB, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        clsConnectionString.connectionString = selectedConnection.ConnectionString; //updating connection string
                        clsConnectionString.updateCurrentConnectionString(selectedConnection.Name); //updating connection in Registry which we can used when user opens the program next time
                        isConnectionStringSelected = true;
                        DialogResult = false;
                    }
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                    clsStatic.WriteExceptionLogXML(ex);
                }

            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        internal void bindGrid()
        {
            dgDBConnectionsData = clsConnectionString.getAllConnections();
            string currentConnection = clsConnectionString.getConnectionStringFromRegistry();
            selectedConnection = (from cc in dgDBConnectionsData where cc.ConnectionString == currentConnection select cc).FirstOrDefault();
        }
        private void testConnectionString()
        {
            if (selectedConnection != null)
            {
                tempTestConnection = clsConnectionString.testConnection(selectedConnection.ConnectionString);
                if (!tempTestConnection)
                {
                    if (isFromOK)
                        MVVMMessageService.ShowMessage("Unable to connect to database " + selectedConnection.Name + ". Please select another database.");
                    else
                        MVVMMessageService.ShowMessage(errorMessages.FAILED_TO_CONNECT);
                }
                isFromOK = false;
            }
        }
        #endregion
    }
}
