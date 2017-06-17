using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using System.Collections.ObjectModel;

namespace PhotoForce.OrdersManagement
{
    public class UpdateOrderTrackingNumberViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        public ObservableCollection<Order> ordersColl;
        public Order addEditOrder;
        #endregion

        #region Properties
        string _shippingTrackingNo;
        bool _isShippedChecked;

        public bool isShippedChecked
        {
            get { return _isShippedChecked; }
            set { _isShippedChecked = value; NotifyPropertyChanged(); }
        }
        public string shippingTrackingNo
        {
            get { return _shippingTrackingNo; }
            set { _shippingTrackingNo = value; NotifyPropertyChanged("shippingTrackingNo"); }
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


        #region Constructor
        public UpdateOrderTrackingNumberViewModel(ObservableCollection<Order> tempOrders)
        {
            ordersColl = new ObservableCollection<Order>();
            ordersColl = tempOrders;
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
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
                return new RelayCommand(windowClose);
            }
        }
        #endregion

        #region Methods
        private void saveAndClose()
        {
            if (!string.IsNullOrEmpty(shippingTrackingNo) && !string.IsNullOrEmpty(vendorOrderNo) )
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                if (string.IsNullOrEmpty(vendorOrderNo) ) { return; }
                foreach (Order or in ordersColl)
                {
                    addEditOrder = new Order();
                    addEditOrder = clsOrders.getOrderById(db, or.Id);
                    //addEditOrder.Fulfilled = isFulfillChecked;
                    //addEditOrder.VendorDate = selectedVendorDate;
                    if (or.VendorOrderNo == null || or.VendorOrderNo == "")
                    {
                        addEditOrder.VendorOrderNo = vendorOrderNo;
                    }
                    else
                        addEditOrder.VendorOrderNo = or.VendorOrderNo + ", " + vendorOrderNo;
                    if (shippingTrackingNo != null)
                    {
                        addEditOrder.TrackingNumber = shippingTrackingNo;
                    }
                    addEditOrder.isShipped = isShippedChecked;
                    //if (isClearVendorOrderNo)
                    //    addEditOrder.VendorOrderNo = "";
                    isSave = true;
                    db.SubmitChanges();
                }

                isSave = true;
                DialogResult = false;
            }
            else
                return;
        }
        private void windowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion
    }
}
