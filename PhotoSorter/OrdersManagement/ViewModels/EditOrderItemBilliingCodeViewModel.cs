using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.OrdersManagement
{
    public class EditOrderItemBilliingCodeViewModel : ViewModelBase
    {
        #region Initialization
        public bool isSave = false;
        PhotoSorterDBModelDataContext db ;
        #endregion

        #region Properties
        List<StudentPhotoOrder> _lstStudentPhotoOrders;
        ObservableCollection<StudentPhotoOrder> _selectedStudentPhotoOrders;
        List<OrderPackage> _billingCodes;
        StudentPhotoOrder _selectedOrderItem;
        string _selecedBillingCode;
        OrderPackage _selectedPackage;

        public OrderPackage selectedPackage
        {
            get { return _selectedPackage; }
            set 
            {
                _selectedPackage = value; NotifyPropertyChanged("selectedPackage"); 
            }
        }
        public string selecedBillingCode
        {
            get { return _selecedBillingCode; }
            set { _selecedBillingCode = value; NotifyPropertyChanged("selecedBillingCode"); }
        }
        public List<OrderPackage> billingCodes
        {
            get { return _billingCodes; }
            set { _billingCodes = value; NotifyPropertyChanged("billingCodes"); }
        }
        public ObservableCollection<StudentPhotoOrder> selectedStudentPhotoOrders
        {
            get { return _selectedStudentPhotoOrders; }
            set { _selectedStudentPhotoOrders = value; NotifyPropertyChanged("selectedStudentPhotoOrders"); }
        }
        public StudentPhotoOrder selectedOrderItem
        {
            get { return _selectedOrderItem; }
            set { _selectedOrderItem = value; NotifyPropertyChanged("selectedOrderItem"); }
        }
        public List<StudentPhotoOrder> lstStudentPhotoOrders
        {
            get { return _lstStudentPhotoOrders; }
            set { _lstStudentPhotoOrders = value; NotifyPropertyChanged("lstStudentPhotoOrders"); }
        }
        #endregion

        #region Constructors
        public EditOrderItemBilliingCodeViewModel(List<int> studentPhotoOrdersList)
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            lstStudentPhotoOrders = clsOrders.getStudentPhotoOrdersByItemIds(db,studentPhotoOrdersList); 
            selectedStudentPhotoOrders = new ObservableCollection<StudentPhotoOrder>();

            bindingData();
        }
        #endregion

        #region Commands
        public RelayCommand UpdateCommand
        {
            get
            {
                return new RelayCommand(update);
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
        private void bindingData()
        {
                billingCodes = clsOrders.getAllOrderPackages(db).OrderBy(op => op.Item).ToList();
        }
        private void update()
        {
                int i = 0;
                try
                {
                    foreach (StudentPhotoOrder orderItem in lstStudentPhotoOrders)
                    {
                        if (!string.IsNullOrEmpty(orderItem.sp_SimplePhotoBillingCode))
                        {
                            //update Billing code in student photo order
                            clsOrders.updateOrderItemBillingCode(db, orderItem.Id, orderItem.sp_SimplePhotoBillingCode);
                            //update package in student image table

                            string orderpackage = clsOrders.getOrderPackage(db, orderItem.sp_SimplePhotoBillingCode);
                            if (orderpackage.Length > 1)
                            {
                                //have to change this code,i've added quantity as 1 by default for e.g: A-1
                                //#Mohan
                                clsDashBoard.updatePackages(db, orderpackage.Trim() + "-1", (int)orderItem.StudentImageId);
                            }
                            i++;
                            isSave = true;
                        }
                    }
                    MVVMMessageService.ShowMessage(i + " Rows updated.");
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                    clsStatic.WriteExceptionLogXML(ex);
                }
                DialogResult = false;
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
