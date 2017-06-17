using RandomLicenseGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsLicensing
    {
        static string connectionString = clsConnectionString.connectionString;
        static string licensingQueryString = "CREATE TABLE Licensing(" + "ID INTEGER IDENTITY(1,1) PRIMARY KEY," + "FirstName VarChar(50)," + "LastName VarChar(50)," + "Email VarChar(50)," + "Credits VarChar(max) NOT NULL," + "LicenseKey VarChar(150)," + "DOJ datetime);";
        static string creditLogQueryString = "CREATE TABLE CreditLog(" + "ID INTEGER IDENTITY(1,1) PRIMARY KEY," + "Credits VarChar(max) NOT NULL," + "LicenseKey VarChar(150) NOT NULL," + "Mode VarChar(15));";
        static bool isLicensingExists = false;
        public static bool isCreditLogExists = false;

        public static bool CheckForLicense(PhotoSorterDBModelDataContext db)
        {
            SqlCommand command;
            SqlCommand creditlogCommand;
            int? countLicensing = 0;
            int? countCreditLog = 0;
            SqlConnection connection = new SqlConnection(connectionString);
            string checkLicensingTable = @"SELECT 1 as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[Licensing]')";
            string checkCreditLogTable = @"SELECT 1 as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[CreditLog]')";

            command = new SqlCommand(checkLicensingTable, connection);
            creditlogCommand = new SqlCommand(checkCreditLogTable, connection);

            if (connection.State == ConnectionState.Closed)
            {
                try
                {
                    connection.Open();
                    countLicensing = (int?)command.ExecuteScalar();
                    countCreditLog = (int?)creditlogCommand.ExecuteScalar();

                    if (countLicensing != null)
                        isLicensingExists = true;
                    if (countCreditLog != null)
                        isCreditLogExists = true;

                }
                catch (Exception)
                {

                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return isLicensingExists;
        }

        public static void createLicenseKeytable(PhotoSorterDBModelDataContext db)
        {
            try
            {
                db.ExecuteCommand(licensingQueryString);
                db.SubmitChanges();
            }
            catch (Exception)
            {

            }
        }

        public static void createCreditLogtable(PhotoSorterDBModelDataContext db)
        {
            try
            {
                db.ExecuteCommand(creditLogQueryString);
                db.SubmitChanges();
            }
            catch (Exception)
            {

            }
        }

        public static List<Licensing> checkCreditsCount(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<Licensing>(
             "SELECT  l.FirstName, l.LastName,c.ID, c.Credits from CreditLog c JOIN Licensing l ON c.ID=(select max(ID) from CreditLog)"
             ).ToList();
        }
        internal static string getUserMail(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<string>("select Email from Licensing").Single();
        }
        internal static int checkKeyExistence(PhotoSorterDBModelDataContext db, string Key)
        {
            return db.ExecuteQuery<int>(
                "select COUNT(*) from CreditLog where LicenseKey LIKE '%" + Key + "%'"
                ).Single();
        }
        internal static int getCredtisCount(PhotoSorterDBModelDataContext db)
        {

            int maxId = (from i in db.CreditLogs
                         select i.ID).Max();

            string tempcredits = db.ExecuteQuery<string>("select Credits from CreditLog where ID=" + maxId).Single();
            int credits = Convert.ToInt32(RSAEncryptDecrypt.Decrypt(tempcredits));
            return credits;

        }
        internal static void updateCreditLog(PhotoSorterDBModelDataContext db, int Id, CreditLog creditLogObj)
        {
            var cust = (from c in db.CreditLogs
                        where c.ID == Id
                        select c).First();
            cust.LicenseKey = creditLogObj.LicenseKey;
            cust.Credits = creditLogObj.Credits;
            cust.Mode = creditLogObj.Mode;
            try
            {
                db.SubmitChanges();
            }
            catch (Exception)
            {

            }
        }

    }


    //it is bad practice to create classes manually in dbml,but inorder to insert the values after dynamic creation of table we created it here manually.

    #region License Key in DBML

    public partial class PhotoSorterDBModelDataContext : System.Data.Linq.DataContext
    {
        #region Extensibility Method Definitions
        partial void InsertLicensing(Licensing instance);
        partial void UpdateLicensing(Licensing instance);
        partial void DeleteLicensing(Licensing instance);

        partial void InsertCreditLog(CreditLog instance);
        partial void UpdateCreditLog(CreditLog instance);
        partial void DeleteCreditLog(CreditLog instance);
        #endregion
        public System.Data.Linq.Table<Licensing> Licensings
        {
            get
            {
                return this.GetTable<Licensing>();
            }
        }

        public System.Data.Linq.Table<CreditLog> CreditLogs
        {
            get
            {
                return this.GetTable<CreditLog>();
            }
        }
    }

    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Licensing")]
    public partial class Licensing : INotifyPropertyChanging, INotifyPropertyChanged
    {

        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _ID;

        private string _FirstName;

        private string _LastName;

        private string _Email;

        private string _Credits;

        private string _LicenseKey;

        private System.Nullable<System.DateTime> _DOJ;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnIDChanging(int value);
        partial void OnIDChanged();
        partial void OnFirstNameChanging(string value);
        partial void OnFirstNameChanged();
        partial void OnLastNameChanging(string value);
        partial void OnLastNameChanged();
        partial void OnEmailChanging(string value);
        partial void OnEmailChanged();
        partial void OnCreditsChanging(string value);
        partial void OnCreditsChanged();
        partial void OnLicenseKeyChanging(string value);
        partial void OnLicenseKeyChanged();
        partial void OnDOJChanging(System.Nullable<System.DateTime> value);
        partial void OnDOJChanged();
        #endregion

        public Licensing()
        {
            OnCreated();
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ID", AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                if ((this._ID != value))
                {
                    this.OnIDChanging(value);
                    this.SendPropertyChanging();
                    this._ID = value;
                    this.SendPropertyChanged("ID");
                    this.OnIDChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_FirstName", DbType = "VarChar(50)")]
        public string FirstName
        {
            get
            {
                return this._FirstName;
            }
            set
            {
                if ((this._FirstName != value))
                {
                    this.OnFirstNameChanging(value);
                    this.SendPropertyChanging();
                    this._FirstName = value;
                    this.SendPropertyChanged("FirstName");
                    this.OnFirstNameChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LastName", DbType = "VarChar(50)")]
        public string LastName
        {
            get
            {
                return this._LastName;
            }
            set
            {
                if ((this._LastName != value))
                {
                    this.OnLastNameChanging(value);
                    this.SendPropertyChanging();
                    this._LastName = value;
                    this.SendPropertyChanged("LastName");
                    this.OnLastNameChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Email", DbType = "VarChar(50)")]
        public string Email
        {
            get
            {
                return this._Email;
            }
            set
            {
                if ((this._Email != value))
                {
                    this.OnEmailChanging(value);
                    this.SendPropertyChanging();
                    this._Email = value;
                    this.SendPropertyChanged("Email");
                    this.OnEmailChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Credits", DbType = "VarChar(max) NOT NULL", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Credits
        {
            get
            {
                return this._Credits;
            }
            set
            {
                if ((this._Credits != value))
                {
                    this.OnCreditsChanging(value);
                    this.SendPropertyChanging();
                    this._Credits = value;
                    this.SendPropertyChanged("Credits");
                    this.OnCreditsChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LicenseKey", DbType = "VarChar(150)")]
        public string LicenseKey
        {
            get
            {
                return this._LicenseKey;
            }
            set
            {
                if ((this._LicenseKey != value))
                {
                    this.OnLicenseKeyChanging(value);
                    this.SendPropertyChanging();
                    this._LicenseKey = value;
                    this.SendPropertyChanged("LicenseKey");
                    this.OnLicenseKeyChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_DOJ", DbType = "DateTime")]
        public System.Nullable<System.DateTime> DOJ
        {
            get
            {
                return this._DOJ;
            }
            set
            {
                if ((this._DOJ != value))
                {
                    this.OnDOJChanging(value);
                    this.SendPropertyChanging();
                    this._DOJ = value;
                    this.SendPropertyChanged("DOJ");
                    this.OnDOJChanged();
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.CreditLog")]
    public partial class CreditLog : INotifyPropertyChanging, INotifyPropertyChanged
    {

        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _ID;

        private string _Credits;

        private string _LicenseKey;

        private string _Mode;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnIDChanging(int value);
        partial void OnIDChanged();
        partial void OnCreditsChanging(string value);
        partial void OnCreditsChanged();
        partial void OnLicenseKeyChanging(string value);
        partial void OnLicenseKeyChanged();
        partial void OnModeChanging(string value);
        partial void OnModeChanged();
        #endregion

        public CreditLog()
        {
            OnCreated();
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ID", AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                if ((this._ID != value))
                {
                    this.OnIDChanging(value);
                    this.SendPropertyChanging();
                    this._ID = value;
                    this.SendPropertyChanged("ID");
                    this.OnIDChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Credits", DbType = "VarChar(max) NOT NULL", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Credits
        {
            get
            {
                return this._Credits;
            }
            set
            {
                if ((this._Credits != value))
                {
                    this.OnCreditsChanging(value);
                    this.SendPropertyChanging();
                    this._Credits = value;
                    this.SendPropertyChanged("Credits");
                    this.OnCreditsChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LicenseKey", DbType = "VarChar(150) NOT NULL", CanBeNull = false)]
        public string LicenseKey
        {
            get
            {
                return this._LicenseKey;
            }
            set
            {
                if ((this._LicenseKey != value))
                {
                    this.OnLicenseKeyChanging(value);
                    this.SendPropertyChanging();
                    this._LicenseKey = value;
                    this.SendPropertyChanged("LicenseKey");
                    this.OnLicenseKeyChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Mode", DbType = "VarChar(15)")]
        public string Mode
        {
            get
            {
                return this._Mode;
            }
            set
            {
                if ((this._Mode != value))
                {
                    this.OnModeChanging(value);
                    this.SendPropertyChanging();
                    this._Mode = value;
                    this.SendPropertyChanged("Mode");
                    this.OnModeChanged();
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    #endregion

    enum CreditsMode
    {
        Added,
        Consumed
    }
}
