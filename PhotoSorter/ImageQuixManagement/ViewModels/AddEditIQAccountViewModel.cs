using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.ImageQuixManagement
{
    public class AddEditIQAccountViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        string isFrom = "";
        IQAccount selectedIQAccount;
        public IQAccount _objIQAccount;
        public IQPriceSheet _objIQPriceSheet;
        public bool isSave = false;
        IQPriceSheet selectedIQPriceSheet;
        #endregion

        #region Properties
        string _iqAccountCode;
        string _description;
        string _labelName;

        public string labelName
        {
            get { return _labelName; }
            set { _labelName = value; NotifyPropertyChanged("labelName"); }
        }
        public string description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged(); }
        }
        public string iqAccountCode
        {
            get { return _iqAccountCode; }
            set 
            {
                _iqAccountCode = value; NotifyPropertyChanged("iqAccountCode");

                if (labelName == "  Pricesheet Id :" && !string.IsNullOrEmpty(iqAccountCode))
                {
                    try
                    {
                        Convert.ToInt32(value);
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage("Please enter valid Pricesheet Id.");

                        iqAccountCode = "";
                    }
                }
            }
        }
        #endregion

        #region  Constructors
        //For new IQAccount
        public AddEditIQAccountViewModel()
        {
            //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            isFrom = "New-IQAccount";
            labelName = "Account Code :";
        }
        //For edit selected IQAccount
        public AddEditIQAccountViewModel(IQAccount IQAccount)
        {
            isFrom = "Edit-IQAccount";
            labelName = "Account Code :";
            iqAccountCode = IQAccount.IQAccountCode;
            description = IQAccount.Description;

            selectedIQAccount = IQAccount;
        }
        public AddEditIQAccountViewModel(IQPriceSheet iqPriceSheet, IQAccount iqAccount)
        {
            labelName = "  Pricesheet Id :";
            selectedIQPriceSheet = iqPriceSheet;
            selectedIQAccount = iqAccount;
            if (iqPriceSheet == null)
            {
                description = "";
                iqAccountCode = "";
                isFrom = "New-PriceSheet";
            }
            else
            {
                description = selectedIQPriceSheet.Description;
                iqAccountCode = selectedIQPriceSheet.IQPriceSheetId.ToString();
                isFrom = "Edit-PriceSheet";
            }

        }
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(saveIQAccount);
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
        private void saveIQAccount()
        {
            if (!string.IsNullOrEmpty(iqAccountCode))
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                _objIQAccount = new IQAccount();
                try
                {
                    if (isFrom == "Edit-IQAccount")
                    {
                        _objIQAccount = (from IQA in db.IQAccounts where IQA.Id == selectedIQAccount.Id select IQA).FirstOrDefault();

                        if (_objIQAccount != null)
                        {
                            _objIQAccount.IQAccountCode = iqAccountCode;
                            _objIQAccount.Description = description;

                            db.SubmitChanges();
                            isSave = true;
                        }
                    }
                    else if (isFrom == "New-IQAccount")
                    {
                        _objIQAccount.IQAccountCode = iqAccountCode;
                        _objIQAccount.Description = description;

                        db.IQAccounts.InsertOnSubmit(_objIQAccount);
                        db.SubmitChanges();
                        isSave = true;
                    }
                    else if(!string.IsNullOrEmpty(iqAccountCode)) //Here IQAccount code means Pricesheet code
                    {
                        if (isFrom == "New-PriceSheet")
                        {
                            _objIQPriceSheet = (from IQP in db.IQPriceSheets where IQP.IQPriceSheetId == Convert.ToInt32(iqAccountCode) && IQP.IQAccountId == selectedIQAccount.Id select IQP).FirstOrDefault();
                            if (_objIQPriceSheet == null)
                            {
                                _objIQPriceSheet = new IQPriceSheet();

                                _objIQPriceSheet.Description = description;
                                _objIQPriceSheet.IQPriceSheetId = Convert.ToInt32(iqAccountCode);
                                _objIQPriceSheet.IQAccountId = selectedIQAccount.Id;

                                db.IQPriceSheets.InsertOnSubmit(_objIQPriceSheet);
                                db.SubmitChanges();

                                isSave = true;
                            }
                            else
                            {
                                MVVMMessageService.ShowMessage("Pricesheet with same Id already exists.");
                            }
                            
                        }
                        else if (isFrom == "Edit-PriceSheet")
                        {
                            _objIQPriceSheet = (from IQP in db.IQPriceSheets where IQP.Id == selectedIQPriceSheet.Id && IQP.IQAccountId == selectedIQAccount.Id select IQP).FirstOrDefault();
                            if (_objIQPriceSheet != null)
                            {
                                _objIQPriceSheet.Description = description;
                                _objIQPriceSheet.IQPriceSheetId = Convert.ToInt32(iqAccountCode);

                                db.SubmitChanges();
                                isSave = true;
                            }
                        }
                        DialogResult = false;
                    }
                    DialogResult = false;
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
