using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.OrdersManagement
{
    public class AddNewOrderPackageViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Intialization
        public bool isSave = false;
        PhotoSorterDBModelDataContext db;
        int packageId = 0;
        public OrderPackage _objPackage = new OrderPackage();
        #endregion

        #region Properties
        string _txtItem;
        string _txtPackage;
        string _txtBillingCode;
        decimal _defaultPrice;
        int _sortOrder;
        bool _seniors;
        private decimal _weight;

        public decimal weight
        {
            get { return _weight; }
            set { _weight = value; NotifyPropertyChanged(); }
        }
        public bool seniors
        {
            get { return _seniors; }
            set { _seniors = value; NotifyPropertyChanged("seniors"); }
        }
        public int sortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; NotifyPropertyChanged("sortOrder"); }
        }
        public decimal defaultPrice
        {
            get { return _defaultPrice; }
            set { _defaultPrice = value; NotifyPropertyChanged("defaultPrice"); }
        }
        public string billingCode
        {
            get { return _txtBillingCode; }
            set { _txtBillingCode = value; NotifyPropertyChanged("billingCode"); }
        }
        public string package
        {
            get { return _txtPackage; }
            set { _txtPackage = value; NotifyPropertyChanged("package"); }
        }
        public string item
        {
            get { return _txtItem; }
            set { _txtItem = value; NotifyPropertyChanged("item"); }
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
                validateOrderPackageEdit(ref message, columnName);
                return message;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="columnName"></param>
        internal void validateOrderPackageEdit(ref string message, string columnName)
        {
            switch (columnName)
            {
                case "billingCode":
                    if (string.IsNullOrEmpty(billingCode == null ? "" : billingCode.TrimEnd()))
                    {
                        message = "Billing Code is required."; errorCount++;
                    }
                    else if (!string.IsNullOrEmpty(package) && !string.IsNullOrEmpty(item))
                    {
                        errorCount = 0;
                    }
                    break;
                case "package":
                    if (string.IsNullOrEmpty(package))
                    {
                        message = "Package is required."; errorCount++;
                    }
                    else if (!string.IsNullOrEmpty(item) && !string.IsNullOrEmpty(billingCode))
                    {
                        errorCount = 0;
                    }
                    break;
                case "item":
                    if (string.IsNullOrEmpty(item == null ? "" : item.TrimEnd()))
                    {
                        message = "Item is required."; errorCount++;
                    }
                    else if (!string.IsNullOrEmpty(package) && !string.IsNullOrEmpty(billingCode))
                    {
                        errorCount = 0;
                    }
                    break;
            }
        }
        #endregion

        #endregion

        #region  Constructors
        public AddNewOrderPackageViewModel()
        {
        }
        public AddNewOrderPackageViewModel(OrderPackage tempOrderPackage)
        {
            package = tempOrderPackage.Package;
            item = tempOrderPackage.Item;
            billingCode = tempOrderPackage.SimplePhotoItemId;
            packageId = tempOrderPackage.Id;
            defaultPrice = tempOrderPackage.DefaultPrice == null ? 0 : (decimal)tempOrderPackage.DefaultPrice;
            sortOrder = tempOrderPackage.SortOrder == null ? 0 : (int)tempOrderPackage.SortOrder;
            weight = tempOrderPackage.Weight == null ? 0 : (decimal)tempOrderPackage.Weight;
        }
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(saveAndClose);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(cancel);
            }
        }
        #endregion

        #region Methods
        private void saveAndClose()
        {
            if (errorCount == 0)
            {
                _objPackage = new OrderPackage();
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (packageId == 0)
                {
                    if (defaultPrice >= 1000) { MVVMMessageService.ShowMessage("Default price should be less than 1000. "); return; }
                    _objPackage.Item = item;
                    _objPackage.Package = package;
                    _objPackage.DefaultPrice = defaultPrice;
                    _objPackage.SimplePhotoItemId = billingCode;
                    _objPackage.SortOrder = sortOrder;
                    _objPackage.Weight = weight;

                    if (_objPackage.Item != null && _objPackage.Package != null) //if (_objPackage != null)  For NUnitTesting By Hema
                    {
                        db.OrderPackages.InsertOnSubmit(_objPackage);
                        db.SubmitChanges();
                        isSave = true;
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                    }
                    DialogResult = false;
                }
                else
                {
                    if (package != "")
                    {
                        if (defaultPrice >= 1000) { MVVMMessageService.ShowMessage("Default price should be less than 1000. "); return; }
                        _objPackage = clsOrders.getOrderPackageById(db, packageId);
                        _objPackage.Item = item;
                        _objPackage.Package = package;
                        _objPackage.DefaultPrice = defaultPrice;
                        _objPackage.SimplePhotoItemId = billingCode;
                        _objPackage.SortOrder = sortOrder;
                        _objPackage.Weight = weight;

                        db.SubmitChanges();
                        //int result = clsOrders.UpadteOrderPackagesData(db, item, package, billingCode, packageId);
                        DialogResult = false;
                        isSave = true;
                        MVVMMessageService.ShowMessage("Package updated successfully.");
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage("Please add a package");
                    }
                }
            }
        }

        private void cancel()
        {
            DialogResult = false;
            isSave = false;
        }
        #endregion
    }
}
