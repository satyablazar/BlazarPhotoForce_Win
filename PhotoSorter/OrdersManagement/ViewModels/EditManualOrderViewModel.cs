using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.OrdersManagement
{
    public class EditManualOrderViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Intialization
        public bool isSave = false;
        PhotoSorterDBModelDataContext db;
        public int OrderId = 0; //#hema For NUnitTesting        
        public ObservableCollection<Order> ordersColl;
        public Order addEditOrder;
        string isEditOrders = null;
        string isFromOrders = "";
        #endregion

        #region Properties
        DateTime? _selectedVendorDate;
        bool _isFulfillEnable;
        bool _isFulfillChecked;
        bool _isFromBulkRename;
        List<string> _paymentMethod;
        string _selectedPaymentMethod;
        bool _isClearVendorOrderNo;
        string _shippingTrackingNo;

        public string shippingTrackingNo
        {
            get { return _shippingTrackingNo; }
            set { _shippingTrackingNo = value; NotifyPropertyChanged("shippingTrackingNo"); }
        }
        public bool isClearVendorOrderNo
        {
            get { return _isClearVendorOrderNo; }
            set { _isClearVendorOrderNo = value; NotifyPropertyChanged("isClearVendorOrderNo"); }
        }
        public string selectedPaymentMethod
        {
            get { return _selectedPaymentMethod; }
            set { _selectedPaymentMethod = value; NotifyPropertyChanged("selectedPaymentMethod"); }
        }
        public List<string> paymentMethod
        {
            get { return _paymentMethod; }
            set { _paymentMethod = value; NotifyPropertyChanged("paymentMethod"); }
        }
        public bool isFromEditOrder
        {
            get { return _isFromBulkRename; }
            set { _isFromBulkRename = value; NotifyPropertyChanged("isFromBulkRename"); }
        }
        public bool isFulfillChecked
        {
            get { return _isFulfillChecked; }
            set { _isFulfillChecked = value; NotifyPropertyChanged("isFulfillChecked"); }
        }
        public bool isFulfillEnable
        {
            get { return _isFulfillEnable; }
            set { _isFulfillEnable = value; NotifyPropertyChanged("isFulfillEnable"); }
        }
        public DateTime? selectedVendorDate
        {
            get { return _selectedVendorDate; }
            set { _selectedVendorDate = value; NotifyPropertyChanged("selectedVendorDate"); }
        }
        #endregion

        #region Properties New (Address)
        string _custAddress;
        string _custCity;
        string _custPostalCode;
        string _custCountry;
        string _custState;
        string _custPhone;
        string _shipAddress;
        string _shipCity;
        string _shipPostalCode;
        string _shipCountry;
        string _shipState;
        string _shipPhone;

        public string shipPhone
        {
            get { return _shipPhone; }
            set { _shipPhone = value; NotifyPropertyChanged(); }
        }
        public string shipState
        {
            get { return _shipState; }
            set { _shipState = value; NotifyPropertyChanged(); }
        }
        public string shipCountry
        {
            get { return _shipCountry; }
            set { _shipCountry = value; NotifyPropertyChanged(); }
        }
        public string shipPostalCode
        {
            get { return _shipPostalCode; }
            set { _shipPostalCode = value; NotifyPropertyChanged(); }
        }
        public string shipCity
        {
            get { return _shipCity; }
            set { _shipCity = value; NotifyPropertyChanged(); }
        }

        public string shipAddress
        {
            get { return _shipAddress; }
            set { _shipAddress = value; NotifyPropertyChanged(); }
        }
        public string custPhone
        {
            get { return _custPhone; }
            set { _custPhone = value; NotifyPropertyChanged(); }
        }
        public string custState
        {
            get { return _custState; }
            set { _custState = value; NotifyPropertyChanged(); }
        }
        public string custCountry
        {
            get { return _custCountry; }
            set { _custCountry = value; NotifyPropertyChanged(); }
        }
        public string custPostalCode
        {
            get { return _custPostalCode; }
            set { _custPostalCode = value; NotifyPropertyChanged(); }
        }
        public string custCity
        {
            get { return _custCity; }
            set { _custCity = value; NotifyPropertyChanged(); }
        }
        public string custAddress
        {
            get { return _custAddress; }
            set { _custAddress = value; NotifyPropertyChanged(); }
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
        //Create Order
        public EditManualOrderViewModel(string isFrom)
        {
            paymentMethod = new List<string>();
            paymentMethod.Add("Check"); paymentMethod.Add("Dollar"); paymentMethod.Add("V/MC/AmEx");
            selectedPaymentMethod = paymentMethod[0];
            isFromOrders = isFrom;
            isFromEditOrder = true;
            selectedVendorDate = DateTime.Now;
            OrderId = 0;
            isFulfillEnable = false; isFulfillChecked = false;
        }
        //For edit Order (general edit)
        public EditManualOrderViewModel(Order tempOrder, string isFrom)
        {
            paymentMethod = new List<string>();
            paymentMethod.Add("Check"); paymentMethod.Add("Dollar"); paymentMethod.Add("V/MC/AmEx");
            isFromOrders = isFrom;
            OrderId = tempOrder.Id;
            isFulfillEnable = true;
            isFromEditOrder = true;
            isClearVendorOrderNo = false;

            if (OrderId != 0)
            {
                selectedPaymentMethod = tempOrder.PaymentMethod;
                isFulfillChecked = (bool)tempOrder.Fulfilled;
                selectedVendorDate = tempOrder.VendorDate == null ? new DateTime() : (DateTime)tempOrder.VendorDate;
                vendorOrderNo = tempOrder.VendorOrderNo;
                shippingTrackingNo = tempOrder.TrackingNumber;
                custAddress = tempOrder.Cust_Address;
                custCity = tempOrder.Cust_City;
                custCountry = tempOrder.Cust_Country;
                custState = tempOrder.Cust_State;
                custPostalCode = tempOrder.Cust_PostalCode;
                custPhone = tempOrder.Cust_Phone;

                shipAddress = tempOrder.Ship_Address;
                shipCity = tempOrder.Ship_City;
                shipCountry = tempOrder.Ship_Country;
                shipState = tempOrder.Ship_State;
                shipPostalCode = tempOrder.Ship_PostalCode;
                shipPhone = tempOrder.Ship_Phone;
            }
        }
        //For Bulk Rename
        public EditManualOrderViewModel(ObservableCollection<Order> tempOrders, string isFrom)
        {
            paymentMethod = new List<string>();
            paymentMethod.Add("Check"); paymentMethod.Add("Dollar"); paymentMethod.Add("V/MC/AmEx");
            selectedPaymentMethod = paymentMethod[0];
            isFromOrders = isFrom;
            isFromEditOrder = false;
            ordersColl = new ObservableCollection<Order>();

            ordersColl = tempOrders;
            isFulfillEnable = true;
            selectedVendorDate = DateTime.Now;
        }
        //For Edit Orders (Tracking Number)
        //public EditManualOrderViewModel(ObservableCollection<Order> tempOrders, string str, string isFrom)
        //{
        //    paymentMethod = new List<string>();
        //    paymentMethod.Add("Check"); paymentMethod.Add("Dollar"); paymentMethod.Add("V/MC/AmEx");
        //    selectedPaymentMethod = paymentMethod[0];
        //    isFromOrders = isFrom;
        //    isEditOrders = str;
        //    ordersColl = new ObservableCollection<Order>();
        //    isFromEditOrder = true;
        //    ordersColl = tempOrders;
        //    isFulfillEnable = false;
        //    selectedVendorDate = DateTime.Now;
        //}
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(save);
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
        private void save()
        {

            if (selectedVendorDate.ToString() == "01/01/0001 00:00:00") { selectedVendorDate = null; }
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            #region general edit Order
            if (isFromEditOrder && isEditOrders != "isEditOrders")
            {
                if (isFromOrders == "fromManualOrders")
                {
                    
                    if (string.IsNullOrEmpty(selectedPaymentMethod)) { MVVMMessageService.ShowMessage("Payment method is mandatory."); return; }
                }
                else if (string.IsNullOrEmpty(vendorOrderNo) && !isClearVendorOrderNo) { MVVMMessageService.ShowMessage("Vendor order no. is mandatory."); return; }
                if (errorCount != 0) { return; }
                if (OrderId == 0)
                {
                    try
                    {

                        addEditOrder = new Order();

                        addEditOrder.CreatedOn = DateTime.Now;
                        addEditOrder.Fulfilled = false;
                        addEditOrder.VendorDate = selectedVendorDate;
                        addEditOrder.VendorOrderNo = vendorOrderNo;
                        addEditOrder.PaymentMethod = selectedPaymentMethod;
                        addEditOrder.ImportedBy = clsStatic.userName;
                        db.Orders.InsertOnSubmit(addEditOrder);
                        db.SubmitChanges();

                        isSave = true;
                        DialogResult = false;
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.Message);
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                }
                else
                {
                    addEditOrder = new Order();
                    addEditOrder = clsOrders.getOrderById(db, OrderId);
                    addEditOrder.Fulfilled = isFulfillChecked;
                    addEditOrder.VendorDate = selectedVendorDate;
                    addEditOrder.TrackingNumber = shippingTrackingNo;

                    addEditOrder.Cust_Address = custAddress;
                    addEditOrder.Cust_City = custCity;
                    addEditOrder.Cust_Country = custCountry;
                    addEditOrder.Cust_State = custState;
                    addEditOrder.Cust_PostalCode = custPostalCode;
                    addEditOrder.Cust_Phone = custPhone;

                    addEditOrder.Ship_Address = shipAddress;
                    addEditOrder.Ship_City = shipCity;
                    addEditOrder.Ship_Country = shipCountry;
                    addEditOrder.Ship_State = shipState;
                    addEditOrder.Ship_PostalCode = shipPostalCode;
                    addEditOrder.Ship_Phone = shipPhone;



                    if (isFromOrders == "fromManualOrders")
                    {
                        if (paymentMethod == null)
                            return;
                        addEditOrder.PaymentMethod = selectedPaymentMethod;
                    }
                    else if (isClearVendorOrderNo)
                        addEditOrder.VendorOrderNo = "";
                    else
                        addEditOrder.VendorOrderNo = vendorOrderNo;
                    db.SubmitChanges();

                    isSave = true;
                    DialogResult = false;
                }
            }
            #endregion

            #region EditOrder  //For tracking number
            //else if (isFromEditOrder && isEditOrders == "isEditOrders")
            //{
            //    if (string.IsNullOrEmpty(vendorOrderNo) && !isClearVendorOrderNo) { return; }
            //    foreach (Order or in ordersColl)
            //    {                    
            //        addEditOrder = new Order();
            //        addEditOrder = clsOrders.getOrderById(db, or.Id);
            //        addEditOrder.Fulfilled = isFulfillChecked;
            //        addEditOrder.VendorDate = selectedVendorDate;
            //        if (or.VendorOrderNo == null || or.VendorOrderNo == "")
            //        {
            //            addEditOrder.VendorOrderNo = vendorOrderNo;
            //        }
            //        else
            //            addEditOrder.VendorOrderNo = or.VendorOrderNo + ", " + vendorOrderNo;
            //        if (shippingTrackingNo != null )
            //        {
            //            addEditOrder.TrackingNumber = shippingTrackingNo;
            //        }
            //        if (isClearVendorOrderNo)
            //            addEditOrder.VendorOrderNo = "";                    

            //        db.SubmitChanges();
            //    }

            //    isSave = true;
            //    DialogResult = false;
            //}
            #endregion

            #region Bulk Edit Orders
            else
            {
                int[] orderIds = (from oc in ordersColl select oc.Id).ToArray();
                db.Orders
                        .Where(x => orderIds.Contains(x.Id))
                        .ToList()
                        .ForEach(a =>
                        {
                            a.Fulfilled = isFulfillChecked;
                            a.VendorDate = selectedVendorDate;

                            if (!string.IsNullOrEmpty(custAddress))
                                a.Cust_Address = custAddress;
                            if (!string.IsNullOrEmpty(custCity))
                                a.Cust_City = custCity;
                            if (!string.IsNullOrEmpty(custCountry))
                                a.Cust_Country = custCountry;
                            if (!string.IsNullOrEmpty(custState))
                                a.Cust_State = custState;
                            if (!string.IsNullOrEmpty(custPostalCode))
                                a.Cust_PostalCode = custPostalCode;
                            if (!string.IsNullOrEmpty(custPhone))
                                a.Cust_Phone = custPhone;

                            if (!string.IsNullOrEmpty(shipAddress))
                                a.Ship_Address = shipAddress;
                            if (!string.IsNullOrEmpty(shipCity))
                                a.Ship_City = shipCity;
                            if (!string.IsNullOrEmpty(shipCountry))
                                a.Ship_Country = shipCountry;
                            if (!string.IsNullOrEmpty(shipState))
                                a.Ship_State = shipState;
                            if (!string.IsNullOrEmpty(shipPostalCode))
                                a.Ship_PostalCode = shipPostalCode;
                            if (!string.IsNullOrEmpty(shipPhone))
                                a.Ship_Phone = shipPhone;
                        }
                                );

                db.SubmitChanges();

                isSave = true;
                DialogResult = false;
            }
            #endregion

        }
        private void cancel()
        {
            DialogResult = false;
            isSave = false;
        }
        #endregion
    }
}
