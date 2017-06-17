using Ookii.Dialogs.Wpf;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.OrdersManagement;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class OrderPackagesViewModel : ViewModelBase
    {
        #region Initilizations
        PhotoSorterDBModelDataContext db;
        AddNewOrderPackage _objEditOrderPackage;
        //OrderPackage oldPackage;
        #endregion

        #region Properties
        private List<OrderPackage> _dgOrderPackagesData;
        private OrderPackage _selectedPackage;
        public OrderPackage selectedPackage
        {
            get { return _selectedPackage; }
            set
            {
                //if (_selectedPackage != null)
                //{
                //check wether user updated sort order || default price from UI 
                //if yes updated on DB aswell
                //if (!_selectedPackage.Equals(oldPackage))
                //{
                //    if ((selectedPackage.DefaultPrice) != null && (selectedPackage.DefaultPrice).Value >= 1000)
                //    {
                //        MVVMMessageService.ShowMessage("Default price should be less than 1000"); 
                //        selectedPackage.DefaultPrice = oldPackage.DefaultPrice;
                //        return;
                //    }
                //    updatePackage(selectedPackage);
                //}
                //}
                _selectedPackage = value; NotifyPropertyChanged("selectedPackage");

                //user can edit sort order , default price from UI  in order to update in DB store selectedPackage in a temp object and do a comparison at start
                //if (selectedPackage != null) { oldPackage = new OrderPackage { Id = selectedPackage.Id, DefaultPrice = selectedPackage.DefaultPrice, Item = selectedPackage.Item, Package = selectedPackage.Package, SimplePhotoItemId = selectedPackage.SimplePhotoItemId, SortOrder = selectedPackage.SortOrder }; }
            }
        }
        public List<OrderPackage> dgOrderPackagesData
        {
            get { return _dgOrderPackagesData; }
            set { _dgOrderPackagesData = value; NotifyPropertyChanged("dgOrderPackagesData"); }
        }
        #endregion

        #region Constructor
        public OrderPackagesViewModel()
        {
            bindData();
        }
        #endregion

        #region Commands
        //public RelayCommand EditPackageCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(EditPackage);
        //    }
        //}
        //public RelayCommand PackagesMouseDoubleClickCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(PackagesMouseDoubleClick);
        //    }
        //}
        public RelayCommand RowUpdateCommand
        {
            get
            {
                return new RelayCommand(updateRow);
            }
        }
        #endregion

        #region Methods

        internal void bindData()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                dgOrderPackagesData = clsOrders.getAllOrderPackages(db);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        //private void EditPackage()
        //{
        //    if (selectedPackage != null)
        //        editPackage();
        //    else
        //    { MVVMMessageService.ShowMessage("Please select a OrderPackage"); }
        //}

        //private void PackagesMouseDoubleClick()
        //{
        //    //selectedGridName = "student";
        //    editPackage();
        //}
        internal void editPackage()
        {
            try
            {
                if (selectedPackage != null)
                {
                    int PackageId = selectedPackage.Id;
                    _objEditOrderPackage = new AddNewOrderPackage(selectedPackage);
                    _objEditOrderPackage.ShowDialog();
                    if (((AddNewOrderPackageViewModel)(_objEditOrderPackage.DataContext)).isSave)
                        bindData();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        internal void newRecord()
        {
            AddNewOrderPackage _objAddNewOrderPackage = new AddNewOrderPackage();
            _objAddNewOrderPackage.ShowDialog();

            if (((AddNewOrderPackageViewModel)(_objAddNewOrderPackage.DataContext)).isSave)
            {
                bindData();
            }
        }

        //public void updatePackage(OrderPackage oldPackage)
        //{
        //    OrderPackage _objPackage = oldPackage;
        //    _objPackage.DefaultPrice = oldPackage.DefaultPrice;
        //    _objPackage.SortOrder = oldPackage.SortOrder;

        //    db.SubmitChanges();
        //int result = clsOrders.UpadteOrderPackagesData(db, item, package, billingCode, packageId);
        //MVVMMessageService.ShowMessage("Package updated successfully.");
        //}
        public void updateRow()
        {
            //MVVMMessageService.ShowMessage(selectedPackage.Item);
            db.SubmitChanges();
        }
        internal void buttonsVisiblityForPackages()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
        }
        #endregion
    }
}
