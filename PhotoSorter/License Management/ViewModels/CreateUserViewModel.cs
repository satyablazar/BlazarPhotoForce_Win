using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using RandomLicenseGenerator;
using System.Globalization;

namespace PhotoForce.License_Management
{
    public class CreateUserViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public bool isCreditLogExists;
        #endregion

        #region Properties
        private string _email;
        //private string _firstName;
        //private string _lastName;
        //private int _credits;
        private string _licenseKey;
        private DateTime _DOJ;
        private bool _isCancelled;
        string result;

        public bool isCancelled
        {
            get { return _isCancelled; }
            set { _isCancelled = value; NotifyPropertyChanged("isCancelled"); }
        }

        public DateTime DOJ
        {
            get { return _DOJ; }
            set { _DOJ = value; NotifyPropertyChanged("DOJ"); }
        }

        public string licenseKey
        {
            get { return _licenseKey; }
            set { _licenseKey = value; NotifyPropertyChanged("licenseKey"); }
        }

        //public int credits
        //{
        //    get { return _credits; }
        //    set { _credits = value; NotifyPropertyChanged("credits"); }
        //}

        //public string lastName
        //{
        //    get { return _lastName; }
        //    set { _lastName = value; NotifyPropertyChanged("lastName"); }
        //}

        //public string firstName
        //{
        //    get { return _firstName; }
        //    set { _firstName = value; NotifyPropertyChanged("firstName"); }
        //}

        public string email
        {
            get { return _email; }
            set { _email = value; NotifyPropertyChanged("email"); }
        }

        #endregion

        #region Constructors
        public CreateUserViewModel(bool isExists)
        {
            isCancelled = true;
            isCreditLogExists = isExists;
            DOJ = DateTime.Today.Date;
        }
        #endregion

        #region Commands
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand RegisterCommand
        {
            get
            {
                return new RelayCommand(register);
            }
        }
        #endregion

        #region Methods
        private void register()
        {
            if (string.IsNullOrEmpty(licenseKey)) { MVVMMessageService.ShowMessage("Please enter a license key."); return; }
            try
            {
                clsLicensing.createLicenseKeytable(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
                if (!isCreditLogExists)
                {
                    clsLicensing.createCreditLogtable(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
            Licensing licensing = new Licensing();
            string[] words;
            string[] separators = { "_" };

            string keydetails = RandomKeyGenerator.detailsFromKey(licenseKey);
            try
            {
                if (keydetails != null || keydetails.Contains("_"))
                {
                    words = keydetails.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    string emailTemp = words[0];
                    credits = Convert.ToInt32(words[1]);
                    DateTime tempDateTime = DateTime.ParseExact(words[2], "MM/dd/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// Convert.ToDateTime(words[2]);
                    if (emailTemp != email)
                    {
                        MVVMMessageService.ShowMessage("Email Mismatch");
                        return;
                    }
                }
                else
                {
                    MVVMMessageService.ShowMessage("Key Is Invalid ");
                    licenseKey = string.Empty;
                    return;
                }
            }
            catch (Exception)
            {
                MVVMMessageService.ShowMessage("Key Is Invalid ");
                licenseKey = string.Empty;
                return;
            }
            licensing.Email = email;
            licensing.FirstName = firstName;
            licensing.LastName = lastName;
            licensing.LicenseKey = licenseKey;
            licensing.DOJ = DOJ;
            result = RSAEncryptDecrypt.Encrypt(Convert.ToString(credits));


            licensing.Credits = result;

            try
            {
                if (licensing != null)
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    db.Licensings.InsertOnSubmit(licensing);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
            insertCreditLog();
        }
        private void windowClose()
        {
            if (isCancelled == true)
            {
                if (MVVMMessageService.ShowMessage("Do you want to abort the registration?", "Freed Photo", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.No)
                {
                    DialogResult = true;
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }
        public void insertCreditLog()
        {
            CreditLog creditLog = new CreditLog();
            creditLog.LicenseKey = licenseKey;
            creditLog.Credits = result;
            creditLog.Mode = CreditsMode.Added.ToString();
            try
            {
                if (creditLog != null)
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    db.CreditLogs.InsertOnSubmit(creditLog);
                    db.SubmitChanges();
                    isCancelled = false;
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

            MVVMMessageService.ShowMessage("You have added " + credits + " Credits");
            DialogResult = false;
        }
        #endregion
    }
}
