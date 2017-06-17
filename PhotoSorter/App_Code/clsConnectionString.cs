using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsConnectionString
    {
        #region Initialization
        private static string _connectionString = "";

        public static string connectionString
        {
            get { return clsConnectionString._connectionString; }
            set { clsConnectionString._connectionString = value; }
        }
        #endregion

        #region Methods
        public static bool createNealConnectionString(string tempConnectionString, string tempName)
        {
            try
            {
                //delete old sub key if exists
                RegistryKey photoSorterConnectionString = Registry.CurrentUser.OpenSubKey(@"Software\Photo Sorter\PS3ConnectionString");
                if (photoSorterConnectionString != null)
                {
                    Registry.CurrentUser.DeleteSubKeyTree(@"Software\Photo Sorter\PS3ConnectionString");
                    connectionString = "";
                }

                //instead of database we are storing connection strings in 'Connection Strings' Key
                createUpdateConnectionString("PS3_" + tempName, tempName, tempConnectionString);


                connectionString = tempConnectionString;

                //update this connection string as cuurent connection string
                updateCurrentConnectionString(tempName);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return false;
            }
            return true;
        }

        public static string getConnectionStringFromRegistry()
        {
            connectionString = "";
            string name = "";
            RegistryKey photoSorterConnectionString = Registry.CurrentUser.OpenSubKey(@"Software\Photo Sorter\CurrentConnectionString");
            if (photoSorterConnectionString != null)
            {
                string tempConnectionString = photoSorterConnectionString.GetValue("ConnectionString").ToString();
                name = photoSorterConnectionString.GetValue("Name").ToString();


                string server = tempConnectionString.Split(';')[0].Split('=')[1].ToString();

                if (server.Contains("\"") && server.Contains("fpsql") && (server.Contains("sqlexpress,1522") || server.Trim().Contains("sqlexpress, 1522")))
                {
                    connectionString = @"Data Source=fpsql\sqlexpress,1522;Initial Catalog=photoforce4;User ID=sa;Password=scanjet88";
                    updateCurrentConnectionString(name);
                }
                else
                {
                    connectionString = tempConnectionString;
                }
            }
            else
            {
                updateCurrentConnectionString(name);
            }
            return connectionString;
        }

        public static void updateCurrentConnectionString(string name)
        {
            RegistryKey photoSorterConnectionString = Registry.CurrentUser.CreateSubKey(@"Software\Photo Sorter");
            using (RegistryKey PS3ConnectionString = photoSorterConnectionString.CreateSubKey("CurrentConnectionString"))
            {
                // Create data for the currnet ConnectionString subkey.
                PS3ConnectionString.SetValue("ConnectionString", connectionString, RegistryValueKind.String);
                PS3ConnectionString.SetValue("Name", name, RegistryValueKind.String);
            }
        }

        public static List<Connections> getAllConnections()
        {
            Connections objConnections = new Connections();
            Dictionary<string, string[]> dicConnectionStrings = new Dictionary<string, string[]>();
            RegistryKey photoSorterConnectionString = Registry.CurrentUser.OpenSubKey(@"Software\Photo Sorter\Connection Strings");
            if (photoSorterConnectionString != null)
            {
                foreach (String subkeyName in photoSorterConnectionString.GetSubKeyNames())
                {
                    dicConnectionStrings.Add(photoSorterConnectionString.OpenSubKey(subkeyName).GetValue("Name").ToString(), new string[] { photoSorterConnectionString.OpenSubKey(subkeyName).GetValue("ConnectionString").ToString(), subkeyName });
                }
            }

            return objConnections.convertArrayListForBinding(dicConnectionStrings);
        }
        public static ArrayList getAllConnectionNames()
        {
            Connections objConnections = new Connections();
            ArrayList connectionStringList = new ArrayList();
            RegistryKey photoSorterConnectionString = Registry.CurrentUser.OpenSubKey(@"Software\Photo Sorter\Connection Strings");
            if (photoSorterConnectionString != null)
            {
                foreach (String subkeyName in photoSorterConnectionString.GetSubKeyNames())
                {
                    connectionStringList.Add(photoSorterConnectionString.OpenSubKey(subkeyName).GetValue("Name").ToString());
                }
            }
            return connectionStringList;
        }

        public static bool checkForConnectionName(string name)
        {
            ArrayList connectionNameList = getAllConnectionNames();
            if (connectionNameList.Contains(name))
            {
                return true;
            }
            return false;

        }
        internal static bool createUpdateConnectionString(string subKeyName, string tempName, string tempConnectionString)
        {
            bool isSave = false;

            #region new code
            try
            {
                RegistryKey PhotoForce = Registry.CurrentUser.CreateSubKey(@"Software\Photo Sorter\Connection Strings");
                using (RegistryKey PS3ConnectionString = PhotoForce.CreateSubKey(subKeyName))
                {
                    // Create data for the PS3ConnectionString subkey.
                    PS3ConnectionString.SetValue("ConnectionString", tempConnectionString, RegistryValueKind.String);
                    PS3ConnectionString.SetValue("Name", tempName, RegistryValueKind.String);
                }
                isSave = true;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
            clsConnectionString.connectionString = tempConnectionString;
            #endregion

            return isSave;
        }

        internal static bool testConnection(string tempConnectionString)
        {
            //Data Source=developer09;Initial Catalog=FreedPhotoSorter3ToMVVM;User ID=sa;Password=pass@word1
            bool canConnect = false;
            SqlConnection connection = new SqlConnection(tempConnectionString);
            try
            {
                using (connection)
                {
                    connection.Open();
                    canConnect = true;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
            finally
            {
                connection.Close();
            }
            return canConnect;
        }


        #endregion
    }

    /// <summary>
    /// connections class is used to convert dictionary to ArrayList .,so that we can  directly bind arraylist to Grid
    /// </summary>
    public class Connections
    {
        #region Properties
        private string _Name;
        private string _Server;
        private string _DatabaseName;
        private string _ConnectionString;
        private string _FolderName;
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return this._Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// Server
        /// </summary>
        public string Server
        {
            get { return this._Server; }
            set { _Server = value; }
        }
        /// <summary>
        /// Database
        /// </summary>
        public string DatabaseName
        {
            get { return this._DatabaseName; }
            set { _DatabaseName = value; }
        }
        /// <summary>
        /// connection string
        /// </summary>
        public string ConnectionString
        {
            get { return this._ConnectionString; }
            set { _ConnectionString = value; }
        }
        /// <summary>
        /// Folder Name
        /// </summary>
        public string FolderName
        {
            get { return this._FolderName; }
            //set { _FolderName = value; }
        }
        #endregion

        #region Constructors
        public Connections(string connectionName, string server, string database, string connectionString, string folderName)
        {
            this._Name = connectionName;
            this._Server = server;
            this._DatabaseName = database;
            this._ConnectionString = connectionString;
            this._FolderName = folderName;
        }
        public Connections()
        {

        }
        #endregion

        #region Methods
        public List<Connections> convertArrayListForBinding(Dictionary<string, string[]> lstConnections)
        {
            List<Connections> list = new List<Connections>();
            foreach (var item in lstConnections)
            {
                if (!string.IsNullOrEmpty(item.Value[0]))
                {
                    list.Add(new Connections(item.Key, item.Value[0].Split('=')[1].Split(';')[0], item.Value[0].Split('=')[2].Split(';')[0], item.Value[0], item.Value[1]));
                }
            }
            return list;
        }
        #endregion

    }
}
