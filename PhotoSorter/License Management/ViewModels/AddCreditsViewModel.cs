using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.Extensions;
using RandomLicenseGenerator;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.ComponentModel;
using System.Globalization;

namespace PhotoForce.License_Management
{
    public class AddCreditsViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        internal bool isSave = false;
        public string myHtmlContent;
        public int keyCredits = 0;
        #endregion

        #region Properties
        private string _licenseKey;
        private string _email;
        private string _textToCopy;
        private string _newCredits;
        private bool _isTextToCopyEnable;

        public bool isTextToCopyEnable
        {
            get { return _isTextToCopyEnable; }
            set { _isTextToCopyEnable = value; NotifyPropertyChanged("isTextToCopyEnable"); }
        }

        public string newCredits
        {
            get { return _newCredits; }
            set { _newCredits = value; NotifyPropertyChanged("newCredits"); }
        }

        public string textToCopy
        {
            get { return _textToCopy; }
            set { _textToCopy = value; NotifyPropertyChanged("textToCopy"); }
        }

        public string email
        {
            get { return _email; }
            set { _email = value; NotifyPropertyChanged("email"); }
        }

        public string licenseKey
        {
            get { return _licenseKey; }
            set { _licenseKey = value; NotifyPropertyChanged("licenseKey"); }
        }
        #endregion

        #region IDataErrorInfo Members

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
                validateUserInput(ref message, columnName);
                return message;
            }
        }
        #endregion

        #endregion

        #region Constructors
        public AddCreditsViewModel(string tempNewCredits)
        {
            newCredits = tempNewCredits;
            email = clsLicensing.getUserMail(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
        }
        #endregion

        #region Commands
        public RelayCommand GenerateRequestCommand
        {
            get
            {
                return new RelayCommand(generateRequest);
            }
        }
        public RelayCommand SendRequestCommand
        {
            get
            {
                return new RelayCommand(sendRequest);
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
        private void generateRequest()
        {
            if (errorCount == 0 && credits > 0)
            {
                isTextToCopyEnable = true;
                myHtmlContent = "mailto:neal@freedphoto.com" + "?subject=License request for " + email + "&body=Please supply me with photo sorter Credits:    " + credits + "" + Environment.NewLine + "" + Environment.NewLine + "For Account:    " + email + "";
                textToCopy = myHtmlContent;
            }
        }
        private void sendRequest()
        {
            if (!string.IsNullOrEmpty(textToCopy))
            {
                System.Windows.Controls.WebBrowser myWebBrowser = new System.Windows.Controls.WebBrowser();
                myWebBrowser.Navigate(myHtmlContent);
                isTextToCopyEnable = false;
            }
        }
        private void selectOK()
        {
            if (!string.IsNullOrEmpty(licenseKey))
            {
                
                int creditLogId = 0;
                int? validateLicenseKey = clsLicensing.checkKeyExistence(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), licenseKey);
                if (validateLicenseKey >= 1)
                {
                    licenseKey = string.Empty;
                    MVVMMessageService.ShowMessage("Key Is Invalid ");
                }
                else
                {
                    string[] words;
                    string[] separators = { "_" };

                    string keydetails = RandomKeyGenerator.detailsFromKey(licenseKey);
                    try
                    {
                        if (keydetails != null || keydetails.Contains("_"))
                        {
                            words = keydetails.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                            string emailTemp = words[0];
                            keyCredits = Convert.ToInt32(words[1]);
                            DateTime tempDateTime = DateTime.ParseExact(words[2], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                            creditLogId = Convert.ToInt32(words[3]);
                            if (emailTemp != email)
                            {
                                MVVMMessageService.ShowMessage("Email Mismatch");
                                return;
                            }
                            if (words[4].Length < 4)
                            {
                                MVVMMessageService.ShowMessage("Please Enter a Valid Key");
                                licenseKey = string.Empty;
                                return;
                            }
                            credits = keyCredits;
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage("Please Enter a Valid Key ");
                            licenseKey = string.Empty;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage("Please Enter a Valid Key");
                        clsStatic.WriteExceptionLogXML(ex);
                        licenseKey = string.Empty;
                        return;
                    }
                    int creditsCount = clsLicensing.getCredtisCount(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
                    credits = creditsCount + credits;

                    CreditLog creditLog = new CreditLog();
                    string result = RSAEncryptDecrypt.Encrypt(Convert.ToString(credits));
                    creditLog.Credits = result;
                    creditLog.LicenseKey = licenseKey;
                    creditLog.Mode = CreditsMode.Added.ToString();
                    try
                    {
                        if (creditLog != null)
                        {
                            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                            db.CreditLogs.InsertOnSubmit(creditLog);
                            db.SubmitChanges();
                            MVVMMessageService.ShowMessage("You have added " + keyCredits + " Credits" + Environment.NewLine + "Credits Left  : " + credits);
                        }
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.Message);
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                    finally
                    {
                        DialogResult = false;
                        isSave = true;
                    }
                }

            }
            else
            {
                MVVMMessageService.ShowMessage("Please Enter a Valid Key");
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
