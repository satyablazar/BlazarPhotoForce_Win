using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoForce.OrdersManagement
{
    public class AddPackagesToOrderItemViewModel : ViewModelBase
    {
        #region Initialization
        string previousPackage;
        public View_Order OrderItem;
        public StudentPhotoOrder tempStudentPhotoOrder;
        public bool isSave = false;
        public string Package1 = null; //Changed to public by #hema
        string Package2 = null;
        string tempBillingCode = null;
        string tempPackage = null;
        PhotoSorterDBModelDataContext db;
        public string billingCode;
        bool isFromOrders = false;
        #endregion

        #region Properties
        string _quantity;
        OrderPackage _selectedOrderPackage;

        public OrderPackage selectedOrderPackage
        {
            get { return _selectedOrderPackage; }
            set
            {
                _selectedOrderPackage = value; NotifyPropertyChanged("selectedOrderPackage");
                //if (i > 0)
                //{
                //    orderpackage = clsOrders.getOrderPackage(db, _selectedBillingCode).Trim();
                //    createBillingCode();
                //}
            }
        }
        List<OrderPackage> _cbOrderPackagesData;

        public List<OrderPackage> cbOrderPackagesData
        {
            get { return _cbOrderPackagesData; }
            set { _cbOrderPackagesData = value; NotifyPropertyChanged("cbOrderPackagesData"); }
        }

        public string quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value; NotifyPropertyChanged("quantity");


                if (!string.IsNullOrEmpty(quantity))
                {
                    if (Regex.IsMatch(quantity, "^[0-9]+$", RegexOptions.IgnoreCase) == false)
                    {
                        quantity = previousPackage;
                        MVVMMessageService.ShowMessage("Only numerics are allowed.");
                        return;
                    }
                    else
                    {
                        previousPackage = quantity;
                    }
                }
            }
        }
        #endregion

        #region Constructors

        public AddPackagesToOrderItemViewModel(View_Order tempOrderItem)
        {
            isFromOrders = true;
            cbOrderPackagesData = new List<OrderPackage>();
            OrderItem = tempOrderItem;
            tempBillingCode = tempOrderItem.sp_SimplePhotoBillingCode;
            tempPackage = tempOrderItem.Package;
            LoadData();
        }

        public AddPackagesToOrderItemViewModel(StudentPhotoOrder studentPhotoOrder)
        {
            isFromOrders = false;
            cbOrderPackagesData = new List<OrderPackage>();
            tempStudentPhotoOrder = studentPhotoOrder;
            tempBillingCode = tempStudentPhotoOrder.sp_SimplePhotoBillingCode;
            if (tempStudentPhotoOrder.Quantity != null)
                quantity = Convert.ToString(tempStudentPhotoOrder.Quantity);
            LoadData();
        }
        #endregion

        #region Commands
        public RelayCommand AddPackagesQuantityCommand
        {
            get
            {
                return new RelayCommand(addPackages);
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
        //internal void createBillingCode()
        //{
        //    if (i != 0)
        //    {
        //        s = orderpackage;
        //        if (s.Trim().Length >= 1)
        //        {
        //            string[] words = Regex.Split(s, "-");
        //            if (words.Length > 1)
        //            {
        //                Package1 = words[0];
        //                Package2 = words[1];
        //                quantity = Package2;
        //            }
        //            else if (words.Length == 1)
        //            { quantity = null; Package1 = words[0]; }
        //            else
        //            { quantity = null; Package1 = orderpackage; }
        //        }
        //    }

        //}
        internal void LoadData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            try
            {
                // Split string on spaces.
                // ... This will separate all the words.
                if (tempPackage != null && tempPackage.Length > 1)
                {
                    string[] words = Regex.Split(tempPackage, "-");
                    if (words.Length > 1)
                    {
                        Package1 = words[0];
                        Package2 = words[1];
                        quantity = Package2;
                    }
                    else
                    {
                        quantity = null; Package1 = words[0];
                    }
                }

                cbOrderPackagesData = clsOrders.getAllOrderBullingCodes(db);
                if (cbOrderPackagesData != null)
                {
                    selectedOrderPackage = (from op in cbOrderPackagesData where op.SimplePhotoItemId == tempBillingCode select op).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        private void addPackages()
        {
            if (isFromOrders)
            {
                if (selectedOrderPackage == null || string.IsNullOrEmpty(quantity)) { return; }
                if (OrderItem.StudentImageId != 0)
                {
                    int? classPhoto = null;
                    string Pack = quantity;
                    if (selectedOrderPackage.SimplePhotoItemId != null)
                    {
                        billingCode = selectedOrderPackage.SimplePhotoItemId.TrimEnd();
                        //Pack = selectedOrderPackage.Item.Trim() + "-" + quantity;
                        if (billingCode == "M127" || billingCode == "M128" || billingCode == "M129" ||
                                               billingCode == "M130" || billingCode == "M131" || billingCode == "M132" || billingCode == "M133" || billingCode == "M136" || billingCode == "M171")
                        {
                            classPhoto = clsOrders.getGroupPhotoByImageId(db, OrderItem.StudentImageId);
                        }
                        if (classPhoto == 0)
                            classPhoto = null;
                    }
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    clsDashBoard.updateOrderPackages(db, Convert.ToInt32(quantity), (int)OrderItem.StudentImageId, (int)OrderItem.StudentPhotoOrderId, billingCode, classPhoto);
                    OrderItem = clsOrders.getViewOrderByOrderDetailId(db, (int)OrderItem.StudentPhotoOrderId);
                    isSave = true;
                    DialogResult = false;
                }
            }
            else
            {
                try
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    if (selectedOrderPackage == null || string.IsNullOrEmpty(quantity)) { return; }
                    if (tempStudentPhotoOrder.StudentImageId != 0)
                    {
                        int? classPhoto = null;
                        if (selectedOrderPackage.SimplePhotoItemId != null)
                        {
                            billingCode = selectedOrderPackage.SimplePhotoItemId.TrimEnd();
                            //Pack = selectedOrderPackage.Item.Trim() + "-" + quantity;
                            if (billingCode == "M127" || billingCode == "M128" || billingCode == "M129" ||
                                                   billingCode == "M130" || billingCode == "M131" || billingCode == "M132" || billingCode == "M133" || billingCode == "M136" || billingCode == "M171")
                            {
                                classPhoto = clsOrders.getGroupPhotoByImageId(db, tempStudentPhotoOrder.StudentImageId);
                            }
                            if (classPhoto == 0)
                                classPhoto = null;
                        }
                        clsDashBoard.updateOrderPackages(db, Convert.ToInt32(quantity), (int)tempStudentPhotoOrder.StudentImageId, (int)tempStudentPhotoOrder.Id, billingCode, classPhoto);
                        tempStudentPhotoOrder = clsOrders.getStudentPhotoOrderById(db, (int)tempStudentPhotoOrder.Id);
                        isSave = true;
                        DialogResult = false;
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
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
