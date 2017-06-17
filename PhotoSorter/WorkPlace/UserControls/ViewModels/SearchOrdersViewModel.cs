using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class SearchOrdersViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);        
        bool isSuccess = true;
        #endregion

        #region Properties
        int _selectedIndex;

        #region PnlSearchCriteria Properties
        ObservableCollection<OrderPackage> _dgOrderPackagesData;
        ObservableCollection<OrderPackage> _selectedOrderPackages;
        OrderPackage _selectedOrderPackage;
        ObservableCollection<OrderPackage> _dgSelectedOrderPackagesData;
        ObservableCollection<OrderPackage> _newOrderPackages;
        OrderPackage _newOrderPackage;
        ObservableCollection<StudentPhotoOrder> _galleryGroupData;
        StudentPhotoOrder _selectedGalleryGroup;
        ObservableCollection<OrdersImport> _importBatchData;
        OrdersImport _selectedImportBatch;
        //string _firstName;
        //string _lastName;

        public ObservableCollection<OrderPackage> dgOrderPackagesData
        {
            get { return _dgOrderPackagesData; }
            set { _dgOrderPackagesData = value; NotifyPropertyChanged("dgOrderPackagesData"); }
        }
        public ObservableCollection<OrderPackage> selectedOrderPackages
        {
            get { return _selectedOrderPackages; }
            set { _selectedOrderPackages = value; NotifyPropertyChanged("selectedOrderPackages"); }
        }
        public OrderPackage selectedOrderPackage
        {
            get { return _selectedOrderPackage; }
            set { _selectedOrderPackage = value; NotifyPropertyChanged("selectedOrderPackage"); }
        }

        public ObservableCollection<OrderPackage> dgSelectedOrderPackagesData
        {
            get { return _dgSelectedOrderPackagesData; }
            set { _dgSelectedOrderPackagesData = value; NotifyPropertyChanged("dgSelectedOrderPackagesData"); }
        }
        public ObservableCollection<OrderPackage> newOrderPackages
        {
            get { return _newOrderPackages; }
            set { _newOrderPackages = value; NotifyPropertyChanged("newOrderPackages"); }
        }
        public OrderPackage newOrderPackage
        {
            get { return _newOrderPackage; }
            set { _newOrderPackage = value; NotifyPropertyChanged("newOrderPackage"); }
        }
        public ObservableCollection<StudentPhotoOrder> galleryGroupData
        {
            get { return _galleryGroupData; }
            set { _galleryGroupData = value; NotifyPropertyChanged("galleryGroupData"); }
        }
        public StudentPhotoOrder selectedGalleryGroup
        {
            get { return _selectedGalleryGroup; }
            set { _selectedGalleryGroup = value; NotifyPropertyChanged("selectedGalleryGroup"); }
        }
        public ObservableCollection<OrdersImport> importBatchData
        {
            get { return _importBatchData; }
            set { _importBatchData = value; NotifyPropertyChanged("importBatchData"); }
        }
        public OrdersImport selectedImportBatch
        {
            get { return _selectedImportBatch; }
            set { _selectedImportBatch = value; NotifyPropertyChanged("selectedImportBatch"); }
        }
        //public string firstName
        //{
        //    get { return _firstName; }
        //    set { _firstName = value; NotifyPropertyChanged("firstName"); }
        //}
        //public string lastName
        //{
        //    get { return _lastName; }
        //    set { _lastName = value; NotifyPropertyChanged("lastName"); }
        //}
        #endregion

        #region pnlResult Properties
        ObservableCollection<Order> _dgOrdersData;
        ObservableCollection<Order> _selectedOrders;
        List<Order> _VisibleData;

        public ObservableCollection<Order> dgOrdersData
        {
            get { return _dgOrdersData; }
            set { _dgOrdersData = value; NotifyPropertyChanged("dgOrdersData"); }
        }
        public ObservableCollection<Order> selectedOrders
        {
            get { return _selectedOrders; }
            set { _selectedOrders = value; NotifyPropertyChanged("selectedOrders"); }
        }
        public List<Order> VisibleData
        {
            get { return _VisibleData; }
            set { _VisibleData = value; NotifyPropertyChanged("VisibleData"); }
        }
        #endregion

        public int selectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value; NotifyPropertyChanged("selectedIndex");
                if (selectedIndex == 0)
                {
                    loadData();
                }
                else
                {
                    bindData();
                }
            }
        }
        #endregion

        #region Constructor
        public SearchOrdersViewModel()
        {
            VisibleData = new List<Order>();
            dgOrderPackagesData = new ObservableCollection<OrderPackage>(); selectedOrderPackages = new ObservableCollection<OrderPackage>();
            dgSelectedOrderPackagesData = new ObservableCollection<OrderPackage>(); newOrderPackages = new ObservableCollection<OrderPackage>();
            galleryGroupData = new ObservableCollection<StudentPhotoOrder>(); importBatchData = new ObservableCollection<OrdersImport>();
            dgOrdersData = new ObservableCollection<Order>(); selectedOrders = new ObservableCollection<Order>();
            selectedIndex = 0;
            loadData();
        }
        #endregion

        #region Commands
        public RelayCommand MoveSelectedPackagesCommand
        {
            get
            {
                return new RelayCommand(moveSelectedPackages);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand DeleteOrderPackageCommand
        {
            get
            {
                return new RelayCommand(deleteOrderPackage);
            }
        }
        public RelayCommand OrderPackageDoubleClickCommand
        {
            get
            {
                return new RelayCommand(moveSelectedPackages);
            }
        }
        #endregion

        #region Methods
        internal void loadData()
        {
            try
            {
                if (isSuccess)
                {
                    dgSelectedOrderPackagesData = new ObservableCollection<OrderPackage>();
                    selectedImportBatch = null;
                    selectedGalleryGroup = null;
                    firstName = ""; lastName = "";
                    newOrderPackages = new ObservableCollection<OrderPackage>();

                    #region Load OrderPackagesData
                    dgOrderPackagesData = new ObservableCollection<OrderPackage>((from Op in db.OrderPackages orderby Op.Id descending select Op).ToList().OrderBy(op => op.SortOrder));
                    #endregion

                    #region Load GalleryGroupData
                    List<StudentPhotoOrder> tempData = (from Spo in db.StudentPhotoOrders where Spo.sp_GroupName != null orderby Spo.Id descending select Spo).ToList();
                    galleryGroupData = new ObservableCollection<StudentPhotoOrder>(tempData.GroupBy(test => test.sp_GroupName).Select(grp => grp.First()).OrderBy(gn => gn.sp_GroupName).ToList());
                    #endregion

                    #region Load ImportBatchData
                    importBatchData = new ObservableCollection<OrdersImport>((from Oi in db.OrdersImports orderby Oi.CreatedOn descending select Oi).ToList());
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void moveSelectedPackages()
        {
            try
            {
                if (selectedOrderPackages.Count > 0)
                {
                    foreach (OrderPackage op in selectedOrderPackages)
                    {
                        if (dgSelectedOrderPackagesData.Count > 0)
                        {
                            if (dgSelectedOrderPackagesData.Contains(op))
                                continue;
                            else
                                dgSelectedOrderPackagesData.Add(op);
                        }
                        else
                        {
                            dgSelectedOrderPackagesData.Add(op);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void bindData()
        {
            try
            {
                string billingCodes = "";
                if (selectedImportBatch != null && selectedGalleryGroup != null && dgSelectedOrderPackagesData.Count > 0 && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                {
                    foreach (OrderPackage op in dgSelectedOrderPackagesData)
                    {
                        billingCodes = billingCodes + "'" + op.SimplePhotoItemId + "',";
                    }
                    billingCodes = billingCodes.TrimEnd(billingCodes[billingCodes.Length - 1]);

                    dgOrdersData = new ObservableCollection<Order>(clsOrders.getStudentPhotoOrdersByGrpNameImportBatchBillingCode(db, billingCodes, selectedImportBatch.Id, selectedGalleryGroup.sp_GroupName, firstName, lastName));
                    isSuccess = true;
                }
                else if (selectedImportBatch == null && selectedGalleryGroup == null && dgSelectedOrderPackagesData.Count <= 0 && string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                {
                    isSuccess = false;
                    selectedIndex = 0;
                }
                else
                {
                    isSuccess = false;
                    if (selectedGalleryGroup == null)
                    {
                        MVVMMessageService.ShowMessage("Please select group name.");
                    }
                    else if (selectedImportBatch == null)
                    {
                        MVVMMessageService.ShowMessage("Please select import batch.");
                    }
                    else if (dgSelectedOrderPackagesData.Count <= 0)
                    {
                        MVVMMessageService.ShowMessage("Please select order packages. ");
                    }
                    else if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                    {
                        MVVMMessageService.ShowMessage("Please select firstname and lastname.");
                    }
                    selectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        bool noclassimage = false;
        internal void exportOrders()
        {
            if (selectedIndex == 0) { return; }
            try
            {
                noclassimage = false;
                if (selectedOrders.Count == 0 && (VisibleData == null || VisibleData.Count == 0)) { MVVMMessageService.ShowMessage("Please select a order."); return; }

                PhotoSorterDBModelDataContext ddb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                Dictionary<int, string> dicStudentImages = new Dictionary<int, string>();
                Dictionary<int, string> dicOrderItemBillingCode = new Dictionary<int, string>();
                ArrayList stuImageIds = new ArrayList();    //it will hold all the stud image ids i.e, visible data or stud image ids or both(i mean visible data contains both data) #Mohan
                ArrayList selectedStuImageIds = new ArrayList(); //holds stud image ids of only selected orders
                ArrayList tempOrderIds = new ArrayList((from so in selectedOrders select so.Id).ToArray());
                bool hasFiltered = false;
                List<Order> allOrders = new List<Order>();
                ArrayList tempselectedOrderIds = new ArrayList();//By hema 09/12/2015
                int exportedOrdersCount = 0;
                bool hasEmptyStuImage = false;
                bool isManual = false; bool isXml = false;

                List<Order> tempSelectedOrders = new List<Order>(selectedOrders);
                if (tempSelectedOrders.Count > 0) //By hema 09/12/2015
                {
                    foreach (Order order in tempSelectedOrders)
                    {
                        tempselectedOrderIds.Add(order.Id);
                    }
                }
                foreach (Order order in VisibleData.Count < dgOrdersData.Count && VisibleData.Count != 0 ? VisibleData : tempSelectedOrders)
                {
                    if (order.OrderType == "Manual Orders" && isManual == false)
                    {
                        isManual = true;
                    }
                    if (order.OrderType != "Manual Orders" && isXml == false)
                    {
                        isXml = true;
                    }

                    if (isManual && isXml) { MVVMMessageService.ShowMessage("Export contain both Manual and Non-Manual Orders. Please select any one of the type to export."); return; }
                    List<StudentPhotoOrder> lstOrderItems = clsOrders.getImagesIdsByOrder(db, order.Id);

                    //check wether order details contain any empty stu image info.
                    if ((from loi in lstOrderItems where loi.StudentImageId == null select loi).FirstOrDefault() != null)
                    {
                        hasEmptyStuImage = true;
                        continue;
                    }

                    if (lstOrderItems == null || lstOrderItems.Count == 0) { continue; }

                    allOrders.Add(order);
                    if (order.IsExported == true) { exportedOrdersCount++; }

                    foreach (StudentPhotoOrder orderItem in lstOrderItems)
                    {
                        if (orderItem.sp_SimplePhotoBillingCode != null)
                        {
                            string billingCode = orderItem.sp_SimplePhotoBillingCode.Trim();
                            if (billingCode == "M127" || billingCode == "M128" || billingCode == "M129" ||
                                        billingCode == "M130" || billingCode == "M131" || billingCode == "M132" || billingCode == "M133" || billingCode == "M136" || billingCode == "M171")
                            {
                                StudentImage objGroupClassImage = clsOrders.getGroupImageIdForOrders(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), (int)orderItem.Id, (int)orderItem.OrderId);
                                if (objGroupClassImage == null)
                                    noclassimage = true; ;
                            }
                        }
                        if (orderItem.StudentImageId == null) { continue; }
                        int studentImageId = (int)orderItem.StudentImageId;

                        string tempGroupImageName = orderItem.GroupImageId.GetValueOrDefault() == 0 ? "" : clsDashBoard.getStudentImageDetailsById(db, (int)orderItem.GroupImageId).ImageName;

                        dicOrderItemBillingCode.Add(orderItem.Id, string.IsNullOrEmpty(orderItem.sp_SimplePhotoBillingCode) ? "No Billing Code" : orderItem.sp_SimplePhotoBillingCode[0].ToString());

                        if (!dicStudentImages.ContainsKey(studentImageId))
                        {
                            dicStudentImages.Add(studentImageId, tempGroupImageName);
                        }
                        stuImageIds.Add(studentImageId);
                        if (tempOrderIds.Contains(order.Id))
                        {
                            selectedStuImageIds.Add(studentImageId);
                        }
                        //}
                    }
                }
                if (allOrders.Count == 0 && hasEmptyStuImage == true) { MVVMMessageService.ShowMessage("one or more orders doesn't have student image information in order details."); return; }
                if (allOrders.Count == 0) { return; }
                if (VisibleData.Count < dgOrdersData.Count && VisibleData.Count != 0) { hasFiltered = true; }

                ArrayList tempAllOrderIds = new ArrayList((from ii in allOrders select ii.Id).ToArray());

                ExportOrders _objExportOrders = new ExportOrders(dicStudentImages, stuImageIds, selectedStuImageIds, dicOrderItemBillingCode, hasFiltered, allOrders,tempAllOrderIds, tempselectedOrderIds);
                //By hema 09/12/2015
                //ExportOrders _objExportOrders = new ExportOrders(dicStudentImages, stuImageIds, selectedStuImageIds, dicOrderItemBillingCode, hasFiltered, orderIds);

                if (exportedOrdersCount > 0 || hasEmptyStuImage == true || noclassimage == true)
                {
                    string message = ""; string message2 = "";
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;

                    if (exportedOrdersCount > 0)
                    {
                        message = tempSelectedOrders.Count > 0 ? "few orders has already been exported.\n" : exportedOrdersCount + " order(s) has already been exported.\n";
                    }
                    if (hasEmptyStuImage)
                    {
                        message = message + "one or more orders excluded from export. They don't have student image information in order details.\n";
                    }
                    if (noclassimage == true)
                    {
                        message2 = "There are order items that include class photos, but no group image is assigned.  Are you sure you want to continue with the export?";
                        if (MVVMMessageService.ShowMessage(message2, caption, buttons, icon) != System.Windows.MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }
                    message += "Are you sure you want to export ?";
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        _objExportOrders.ShowDialog();
                    }
                }
                else
                {
                    _objExportOrders.ShowDialog();
                }

                if (((ExportViewModel)(_objExportOrders.DataContext)).isSave == true)
                {
                    foreach (int OrderId in tempAllOrderIds)
                    {
                        Order _objOrder = clsOrders.getOrderById(db, OrderId);
                        _objOrder.ExportedBy = clsStatic.userName;
                        _objOrder.ExportDate = DateTime.Now;
                        _objOrder.IsExported = true;
                        _objOrder.ExportFolder = ((ExportViewModel)(_objExportOrders.DataContext)).alternateFullPath;

                        db.SubmitChanges();
                    }
                    loadData();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void deleteOrderPackage()
        {
            try
            {
                ArrayList arrPackageIds = new ArrayList();
                if (newOrderPackages.Count > 0)
                {
                    foreach (OrderPackage op in newOrderPackages)
                    {
                        arrPackageIds.Add(op.Id);
                    }
                    foreach (int Id in arrPackageIds)
                    {
                        dgSelectedOrderPackagesData.Remove((from sop in dgSelectedOrderPackagesData where sop.Id == Id select sop).First());
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
