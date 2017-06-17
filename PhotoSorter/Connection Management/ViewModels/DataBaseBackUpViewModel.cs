//using Microsoft.SqlServer.Management.Smo;
using Ookii.Dialogs.Wpf;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.Connection_Management
{
    public class DataBaseBackUpViewModel : ViewModelBase
    {
        #region Initialization
        List<Connections> dgConnactions = new List<Connections>();
        Connections selectedConnection = null;
        string zippedfilepath = "";
        string destinationFolder = AppDomain.CurrentDomain.BaseDirectory + "\\DbBackup"; // "C:\\DbBackup";
        string path="";
        #endregion

        #region Properties
        string _fileLocation;
        string _dataBaseName;
        bool _isBackUpEnabled;
        string _serverName;
        Visibility _progressVisibility;
        int _maxValue;
        int _minValue;
        int _currentProgress;
        Visibility _processVisibility;
        string _txtprogress;

        public string txtprogress
        {
            get { return _txtprogress; }
            set { _txtprogress = value; NotifyPropertyChanged(); }
        }
        public Visibility processVisibility
        {
            get { return _processVisibility; }
            set { _processVisibility = value; NotifyPropertyChanged("processVisibility"); }
        }
        public System.Windows.Visibility progressVisibility
        {
            get { return _progressVisibility; }
            set { _progressVisibility = value; NotifyPropertyChanged("progressVisibility"); }
        }
        //public bool isInProgress
        //{
        //    get { return _isInProgress; }
        //    set { _isInProgress = value; NotifyPropertyChanged("isInProgress"); }
        //}
        public int maxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; NotifyPropertyChanged("maxValue"); }
        }
        public int minValue
        {
            get { return _minValue; }
            set { _minValue = value; NotifyPropertyChanged("minValue"); }
        }
        public int currentProgress
        {
            get { return _currentProgress; }
            set { _currentProgress = value; NotifyPropertyChanged("currentProgress"); }
        }
        public string serverName
        {
            get { return _serverName; }
            set { _serverName = value; NotifyPropertyChanged("serverName"); }
        }

        public string dataBaseName
        {
            get { return _dataBaseName; }
            set { _dataBaseName = value; NotifyPropertyChanged("dataBaseName"); }
        }
        public bool isBackUpEnabled
        {
            get { return _isBackUpEnabled; }
            set { _isBackUpEnabled = value; NotifyPropertyChanged("isBackUpEnabled"); }
        }

        public string fileLocation
        {
            get { return _fileLocation; }
            set { _fileLocation = value; NotifyPropertyChanged("fileLocation"); }
        }
        #endregion

        #region Constructor
        public DataBaseBackUpViewModel()
        {
            progressVisibility = Visibility.Collapsed; processVisibility = Visibility.Collapsed; txtprogress = "";
            dgConnactions = clsConnectionString.getAllConnections();
            string currentConnection = clsConnectionString.getConnectionStringFromRegistry();
            selectedConnection = (from cc in dgConnactions where cc.ConnectionString == currentConnection select cc).FirstOrDefault();

            string tempConnectionString = clsConnectionString.connectionString;

            string[] splitserver = tempConnectionString.Split(';');
            string[] servername = splitserver[0].Split('=');
            string[] splitdatabase = tempConnectionString.Split(';');
            string[] databasename = splitdatabase[1].Split('=');

            string Server = servername[1].ToString();
            string DataBaseName = databasename[1].ToString();

            isBackUpEnabled = true;
            dataBaseName = " " + DataBaseName ;
            serverName = Server;
            path = @"\\" + serverName + "\"$";
            //bindGrid();
            minValue = 0; maxValue = 100;

            //destinationFolder = @"\developer94\e$\00Hemachandra\0TFS_PF_05\PhotoForce\PhotoSorter\bin\Debug\DbBackup";

            //F:\0TFS_PF_05\PhotoForce\PhotoForce\PhotoSorter\bin\Debug\\DbBackup
        }
        #endregion

        #region Commands
        //public RelayCommand BrowseDestinationPathCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(browseDestinationPath);
        //    }
        //}
        public RelayCommand BackUpcommand
        {
            get
            {
                return new RelayCommand(backup1);
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

        private void browseDestinationPath()
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            var res = dlg.ShowDialog();
            if (res != false)
            {
                fileLocation = dlg.SelectedPath;
                isBackUpEnabled = true;
            }
        }
        async void backup1()
        {
            processVisibility = Visibility.Visible;
            txtprogress = "Taking backup ...";
            await backUp();
        }
        async Task backUp()
        {
            try
            {
                currentProgress = 0;
                progressVisibility = Visibility.Visible;
                currentProgress += 5;
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                //path = path.Replace('"', 'c');
                //path = @"\\developer40\c$\wamp\www";
                //dlg.SelectedPath = path; // @"F:\0TFS_PF_05\PhotoForce\PhotoForce\PhotoSorterSetup\bin\Debug";
                var res = dlg.ShowDialog();
                if (res != false)
                    destinationFolder = dlg.SelectedPath;
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                    // create group photos here inside each folder
                }
                fileLocation = destinationFolder;
                if (string.IsNullOrEmpty(fileLocation)) { isBackUpEnabled = false; MVVMMessageService.ShowMessage("Please select destination path. "); return; }
                isBackUpEnabled = false;
                dgConnactions = clsConnectionString.getAllConnections();
                string currentConnection = clsConnectionString.getConnectionStringFromRegistry();
                selectedConnection = (from cc in dgConnactions where cc.ConnectionString == currentConnection select cc).FirstOrDefault();
                currentProgress += 10;
                string tempConnectionString = clsConnectionString.connectionString;

                string[] splitserver = tempConnectionString.Split(';');
                string[] servername = splitserver[0].Split('=');
                string[] splitdatabase = tempConnectionString.Split(';');
                string[] databasename = splitdatabase[1].Split('=');

                string server = servername[1].ToString();
                string dataBaseName = databasename[1].ToString();
                currentProgress += 5;
                await Task.Run(() =>
            {
                backupDatabase(dataBaseName);

                minValue = 0; maxValue = 100;               currentProgress = 0;
                txtprogress = "Moving file to FTP server.";
                //moveToFTPServer();
                deleteFilelocation(fileLocation);
                currentProgress += 20;
            });
                processVisibility = Visibility.Collapsed;
                progressVisibility = Visibility.Collapsed;
                MVVMMessageService.ShowMessage("File moved successfully.");

                //commented by hema same as BackupDatabase();
                #region backup
                //conn = new SqlConnection(tempConnectionString);
                //conn.Open();
                //string sqlbak = @"BACKUP DATABASE " + dataBaseName + " TO DISK = '" + fileLocation + @"\" + dataBaseName + @".bak'";
                //cmd = new SqlCommand(sqlbak, conn);
                //cmd.ExecuteNonQuery();
                #endregion
                //string message = "The backup of database '" + dataBaseName + "' completed successfully .\n  Do you want to open the output folder? ";
                //string caption = "Confirmation";
                //System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                //System.Windows.MessageBoxImage iconn = System.Windows.MessageBoxImage.Question;
                //if (MVVMMessageService.ShowMessage(message, caption, buttons, iconn) == System.Windows.MessageBoxResult.Yes)
                //    Process.Start(fileLocation);
                DialogResult = false;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                isBackUpEnabled = true;
            }
        }
        /// <summary>
        /// Current Database will be back up to the local folder
        /// and then after compressing back to .zip .bak file will be deleted
        /// </summary>
        /// <param name="databaseName">Name of the database</param>
        public void backupDatabase(string databaseName)
        {
            currentProgress += 10;
            string filePath = buildBackupPathWithFilename(databaseName);

            using (var connection = new SqlConnection(clsConnectionString.getConnectionStringFromRegistry()))
            {
                var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", databaseName, filePath);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    currentProgress += 20;
                    command.CommandTimeout = 0;
                    command.ExecuteNonQuery();
                    currentProgress += 30;
                }
                zippedfilepath = CompressFile(filePath);
                currentProgress += 10;
                deleteTextFile(filePath);
                currentProgress += 10;

                //MVVMMessageService.ShowMessage("File moved successfully.");
            }
        }
        /// <summary>
        /// Database name convertion in local folder
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        private string buildBackupPathWithFilename(string databaseName)
        {
            string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now.ToString("yyyy-MM-dd"));

            return Path.Combine(fileLocation, filename);
        }
        /// <summary>
        /// Compressing .bak database file to .zip
        /// </summary>
        /// <param name="sourceFileName">.bak file name including path</param>
        /// fixed path --> C:\\DbBackup
        /// <returns>"path + / + .zip" file</returns>
        private static string CompressFile(string sourceFileName)
        {
            using (ZipArchive archive = ZipFile.Open(Path.ChangeExtension(sourceFileName, ".zip"), ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(sourceFileName, Path.GetFileName(sourceFileName));
            }
            return Path.ChangeExtension(sourceFileName, ".zip");
        }
        /// <summary>
        /// this method used to delete .bak database back-up file from the local folder 
        /// </summary>
        /// <param name="folderPathtxt">.bak file location(local folder path)</param>
        /// fixed path ---> C:\\DbBackup
        void deleteTextFile(string folderPathtxt)
        {
            if (File.Exists(folderPathtxt))
            {
                File.Delete(folderPathtxt);
            }            
        }
        /// <summary>
        /// this method tries to delete .zip database back-up file from local disk
        /// </summary>
        /// <param name="alternateFullPath">.zip file path(from local disk)</param>
        /// fixed path --->  C:\\DbBackup
        void deleteFilelocation(string alternateFullPath)
        {
            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(alternateFullPath);

            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
            {
                dir.Delete(true);
            }
            if (File.Exists(alternateFullPath))
            {
                File.Delete(alternateFullPath);
            }  
        }

        #region New Code
        /// <summary>
        /// this method used to move .zip database back-up file from local folder(C:\\DbBackup) to FTP server(ftp://ftp.freedphoto.com/web_users/pf4backup) 
        /// </summary>
        void moveToFTPServer()
        {
            //string file = "D:\\RP-3160-driver.zip";
            //opening the file for read.
            currentProgress += 10;
            string uploadFileName = "", uploadUrl = "";
            uploadFileName = new FileInfo(zippedfilepath).Name;
            uploadUrl = "ftp://ftp.freedphoto.com/web_users/pf4backup";
            FileStream fs = new FileStream(zippedfilepath, FileMode.Open, FileAccess.Read);
            try
            {
                long FileSize = new FileInfo(zippedfilepath).Length; // File size of file being uploaded.
                currentProgress += 10;
                Byte[] buffer = new Byte[FileSize];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                fs = null;
                currentProgress += 10;
                string ftpUrl = string.Format("{0}/{1}", uploadUrl, uploadFileName);
                FtpWebRequest requestObj = FtpWebRequest.Create(ftpUrl) as FtpWebRequest;
                requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                requestObj.Credentials = new NetworkCredential("simplephoto", "ociK3#37");
                currentProgress += 10;
                Stream requestStream = requestObj.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                currentProgress += 10;
                requestStream.Flush();
                currentProgress += 10;
                requestStream.Close();
                currentProgress += 20;
                requestObj = null;
                //MVVMMessageService.ShowMessage("File upload/transfer Successed.", "Successed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception )
            {
                if (fs != null)
                {
                    fs.Close();
                }
                //MVVMMessageService.ShowMessage("File upload/transfer Failed.\r\nError Message:\r\n" + ex.Message, "Successed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion
        
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
