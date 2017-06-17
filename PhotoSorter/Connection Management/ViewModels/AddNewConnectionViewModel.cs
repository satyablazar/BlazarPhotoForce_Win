using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using System.Data.SqlClient;
using PhotoForce.App_Code;
using System.ComponentModel;

namespace PhotoForce.Connection_Management
{
    public class AddNewConnectionViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Initilization
        public bool isEdit = false;
        string folderName = "";
        internal bool isSave = false;
        #endregion

        #region Properties
        private string _connectionName;
        private string _databaseName;
        private string _serverName;
        private string _userName;
        //private string _password;
        private string _connectionString;
        
 
        public string userName
        {
            get { return _userName; }
            set { _userName = value; NotifyPropertyChanged("userName"); }
        }
        public string connectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; NotifyPropertyChanged("connectionString"); }
        }
        public string serverName
        {
            get { return _serverName; }
            set { _serverName = value; NotifyPropertyChanged("serverName"); }
        }
        public string databaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; NotifyPropertyChanged("databaseName"); }
        }
        public string connectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; NotifyPropertyChanged("connectionName"); }
        }
        #endregion

        #region IDataErrorInfo Members
        //public int errorCount = 0;

        #region Error Property
        /// <summary>
        /// 
        /// </summary>
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region this Property
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                string message = string.Empty;
                ValidateUserInput(ref message, columnName);
                return message;
            }
        }
        #endregion

        #region Input Input Data
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="columnName"></param>
        private void ValidateUserInput(ref string message, string columnName)
        {
            switch (columnName)
            {
                case "connectionName":
                    if (string.IsNullOrEmpty(connectionName))
                    {
                        message = "Connection Name is required."; errorCount++;
                    }
                    else
                    {
                        if (errorCount != 0)
                            errorCount--;
                    }
                    break;
                case "userName":
                    if (string.IsNullOrEmpty(userName))
                    {
                        message = "User name is required."; errorCount++;
                    }
                    else
                    {
                        if (errorCount != 0)
                            errorCount--;
                    }
                    break;
                case "serverName":
                    if (string.IsNullOrEmpty(serverName))
                    {
                        message = "Server name is required."; errorCount++;
                    }
                    else
                    {
                        if (errorCount != 0)
                            errorCount--;
                    }
                    break;
                case "databaseName":
                    if (string.IsNullOrEmpty(databaseName))
                    {
                        message = "Database name is required."; errorCount++;
                    }
                    else
                    {
                        if (errorCount != 0)
                            errorCount--;
                    }
                    break;
                case "password":
                    if (string.IsNullOrEmpty(password))
                    {
                        message = "password is required."; errorCount++;
                    }
                    else
                    {
                        if (errorCount != 0)
                            errorCount--;
                    }
                    break;

            }
        }
        #endregion

        #endregion

        #region Constructors
        public AddNewConnectionViewModel()
        {
            
        }
        public AddNewConnectionViewModel(Connections _objConnections)
        {
            isEdit = true;
            folderName = "";
            connectionName = _objConnections.Name;
            databaseName = _objConnections.DatabaseName;
            serverName = _objConnections.Server;
            userName = _objConnections.ConnectionString.Split('=')[3].Split(';')[0];
            password = _objConnections.ConnectionString.Split('=')[4].Split(';')[0];
            folderName = _objConnections.FolderName;
        }
        #endregion

        #region Commands
        public RelayCommand TestConnectionCommand
        {
            get
            {
                return new RelayCommand(testConnection);
            }
        }
        public RelayCommand ConnectionOKCommand
        {
            get
            {
                return new RelayCommand(connectionOK);
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
        private void testConnection()
        {
            if (errorCount == 0)
            {
                //if (string.IsNullOrEmpty(connectionName) || string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                //{
                //    MVVMMessageService.ShowMessage(errorMessages.ALL_FILEDS_MANDATORY); return;
                //}
                string connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";Persist Security Info=True;User ID=" + userName + ";Password=" + password;
                bool result = clsConnectionString.testConnection(connectionString);
                if (result)
                {
                    MVVMMessageService.ShowMessage(errorMessages.TEST_CONNECTION_SUCCESFUL);
                }
                else
                {
                    MVVMMessageService.ShowMessage(errorMessages.TEST_CONNECTION_FAIL);
                    return;
                }
            }
        }
        private void connectionOK()
        {
            if (errorCount == 0)
            {
                //if (string.IsNullOrEmpty(connectionName) || string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                //{
                //    MVVMMessageService.ShowMessage(errorMessages.ALL_FILEDS_MANDATORY); return;
                //}
                try
                {
                    //Create the connection string using the connection builder
                    var connectionBuilder = new SqlConnectionStringBuilder
                    {
                        DataSource = serverName,
                        InitialCatalog = databaseName,
                        UserID = userName,
                        Password = password
                    };

                    //Save changes 
                    #region New Code
                    if (isEdit)
                        isSave = clsConnectionString.createUpdateConnectionString(folderName, connectionName, connectionBuilder.ConnectionString);
                    else
                    {
                        if (clsConnectionString.checkForConnectionName(connectionName)) { MVVMMessageService.ShowMessage("There is already a connection exists with name " + connectionName + "."); return; }
                        isSave = clsConnectionString.createUpdateConnectionString("PS3_" + connectionName, connectionName, connectionBuilder.ConnectionString);
                    }

                    #endregion
                    DialogResult = false;
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
            isSave = false;
        }
        #endregion
    }
}
