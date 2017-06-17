using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.Extensions;
using PhotoForce.MVVM;
using PhotoForce.OrdersManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace PhotoForce.WorkPlace.UserControls
{
    //--|| जय श्रीराम ||--
    public class OrdersViewModel : ViewModelBase
    {
        #region Initialization
        string imagePathToShow = "";
        PhotoSorterDBModelDataContext db;
        string selectedGrid = "";
        string Photoshootpath = "";
        public List<int> lstOrderId = new List<int>(); //NUnitTesting    
        string txtPath = "";
        //string[] columnNames;
        public DataTable dt;
        #endregion

        #region Properties
        ObservableCollection<View_Order> _dgOrderItemsData;
        ImageSource _studentImageSource;
        ImageSource _groupImageSource;
        View_Order _selectedStudentPhoto;
        string _orderNo;
        string _orderDate;
        string _vendorOrderDate;
        string _vendorOrderId;
        bool? _isFulfilled;
        bool _isFulfillEnable;

        #region Group and search
        bool _isOrdersSearchControlVisible;
        ShowSearchPanelMode _ordersViewTableSearchPanelMode;
        SearchControl _ordersViewTableSearchControl;
        bool _ordersViewTableShowGroupPanel;
        ShowSearchPanelMode _studentSearchPanelMode;
        SearchControl _studentSearchControl;
        bool _studentShowGroupPanel;

        public bool studentShowGroupPanel
        {
            get { return _studentShowGroupPanel; }
            set { _studentShowGroupPanel = value; NotifyPropertyChanged("studentShowGroupPanel"); }
        }
        public SearchControl studentSearchControl
        {
            get { return _studentSearchControl; }
            set { _studentSearchControl = value; NotifyPropertyChanged("studentSearchControl"); }
        }
        public ShowSearchPanelMode studentSearchPanelMode
        {
            get { return _studentSearchPanelMode; }
            set { _studentSearchPanelMode = value; NotifyPropertyChanged("studentSearchPanelMode"); }
        }
        public bool ordersViewTableShowGroupPanel
        {
            get { return _ordersViewTableShowGroupPanel; }
            set { _ordersViewTableShowGroupPanel = value; NotifyPropertyChanged("ordersViewTableShowGroupPanel"); }
        }
        public SearchControl ordersViewTableSearchControl
        {
            get { return _ordersViewTableSearchControl; }
            set { _ordersViewTableSearchControl = value; NotifyPropertyChanged("ordersViewTableSearchControl"); }
        }
        public ShowSearchPanelMode ordersViewTableSearchPanelMode
        {
            get { return _ordersViewTableSearchPanelMode; }
            set { _ordersViewTableSearchPanelMode = value; NotifyPropertyChanged("ordersViewTableSearchPanelMode"); }
        }
        public bool isOrdersSearchControlVisible
        {
            get { return _isOrdersSearchControlVisible; }
            set { _isOrdersSearchControlVisible = value; NotifyPropertyChanged("isOrdersSearchControlVisible"); }
        }

        bool _ordersTableShowGroupPanel;
        SearchControl _ordersTableSearchControl;
        ShowSearchPanelMode _ordersTableSearchPanelMode;
        bool _isSearchControlVisible;
        #endregion

        ObservableCollection<View_Order> _selectedOrderItems;
        string _logData;
        ObservableCollection<Order> _dgOrdersData;
        ObservableCollection<Order> _selectedOrders;
        Order _selectedOrder;
        List<Order> _VisibleData;
        ObservableCollection<Student> _dgStudentsData;
        ObservableCollection<Student> _selectedStudents;
        Student _selectedStudent;
        ObservableCollection<ComboBoxItem> _cbGallerygroupData;
        string _galleryGroupName = "";
        Visibility _isGalleryGroupVisible;

        public Visibility isGalleryGroupVisible
        {
            get { return _isGalleryGroupVisible; }
            set { _isGalleryGroupVisible = value; NotifyPropertyChanged(); }
        }
        public string galleryGroupName
        {
            get { return _galleryGroupName; }
            set
            {
                _galleryGroupName = value; NotifyPropertyChanged();
                loadGalleryData();
            }
        }
        public ObservableCollection<ComboBoxItem> cbGallerygroupData
        {
            get { return _cbGallerygroupData; }
            set { _cbGallerygroupData = value; NotifyPropertyChanged(); }
        }
        public Student selectedStudent
        {
            get { return _selectedStudent; }
            set { _selectedStudent = value; NotifyPropertyChanged("selectedStudent"); }
        }
        public ObservableCollection<Student> selectedStudents
        {
            get { return _selectedStudents; }
            set { _selectedStudents = value; NotifyPropertyChanged("selectedStudents"); }
        }
        public ObservableCollection<Student> dgStudentsData
        {
            get { return _dgStudentsData; }
            set { _dgStudentsData = value; NotifyPropertyChanged("dgStudentsData"); }
        }
        public List<Order> VisibleData
        {
            get { return _VisibleData; }
            set { _VisibleData = value; NotifyPropertyChanged("VisibleData"); }
        }
        public Order selectedOrder
        {
            get { return _selectedOrder; }
            set { _selectedOrder = value; NotifyPropertyChanged("selectedOrder"); }
        }
        public ObservableCollection<Order> selectedOrders
        {
            get { return _selectedOrders; }
            set { _selectedOrders = value; NotifyPropertyChanged("selectedOrders"); }
        }
        public ObservableCollection<Order> dgOrdersData
        {
            get { return _dgOrdersData; }
            set { _dgOrdersData = value; NotifyPropertyChanged("dgOrdersData"); }
        }
        public string logData
        {
            get { return _logData; }
            set { _logData = value; NotifyPropertyChanged("logData"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public ShowSearchPanelMode ordersTableSearchPanelMode
        {
            get { return _ordersTableSearchPanelMode; }
            set { _ordersTableSearchPanelMode = value; NotifyPropertyChanged("ordersTableSearchPanelMode"); }
        }
        public bool ordersTableShowGroupPanel
        {
            get { return _ordersTableShowGroupPanel; }
            set { _ordersTableShowGroupPanel = value; NotifyPropertyChanged("ordersTableShowGroupPanel"); }
        }
        public SearchControl ordersTableSearchControl
        {
            get { return _ordersTableSearchControl; }
            set { _ordersTableSearchControl = value; NotifyPropertyChanged("ordersTableSearchControl"); }
        }
        public ObservableCollection<View_Order> selectedOrderItems
        {
            get { return _selectedOrderItems; }
            set { _selectedOrderItems = value; NotifyPropertyChanged("selectedOrderItems"); }
        }
        public bool isFulfillEnable
        {
            get { return _isFulfillEnable; }
            set { _isFulfillEnable = value; NotifyPropertyChanged("isFulfillEnable"); }
        }
        public bool? isFulfilled
        {
            get { return _isFulfilled; }
            set { _isFulfilled = value; NotifyPropertyChanged("isFulfilled"); }
        }
        public string orderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; NotifyPropertyChanged("orderDate"); }
        }
        public string vendorOrderDate
        {
            get { return _vendorOrderDate; }
            set { _vendorOrderDate = value; NotifyPropertyChanged("vendorOrderDate"); }
        }
        public string vendorOrderId
        {
            get { return _vendorOrderId; }
            set { _vendorOrderId = value; NotifyPropertyChanged("vendorOrderId"); }
        }
        public string orderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; NotifyPropertyChanged("orderNo"); }
        }
        public View_Order selectedStudentPhoto
        {
            get { return _selectedStudentPhoto; }
            set { _selectedStudentPhoto = value; NotifyPropertyChanged("selectedStudentPhoto"); }
        }
        public ImageSource studentImageSource
        {
            get { return _studentImageSource; }
            set { _studentImageSource = value; NotifyPropertyChanged("studentImageSource"); }
        }
        public ObservableCollection<View_Order> dgOrderItemsData
        {
            get { return _dgOrderItemsData; }
            set { _dgOrderItemsData = value; NotifyPropertyChanged("dgOrderItemsData"); }
        }
        public ImageSource groupImageSource
        {
            get { return _groupImageSource; }
            set { _groupImageSource = value; NotifyPropertyChanged("groupImageSource"); }
        }
        #endregion

        #region Constructors
        public OrdersViewModel()
        {
            VisibleData = new List<Order>();
            dgOrderItemsData = new ObservableCollection<View_Order>(); dgOrdersData = new ObservableCollection<Order>();
            selectedOrderItems = new ObservableCollection<View_Order>(); selectedOrders = new ObservableCollection<Order>();
            dgStudentsData = new ObservableCollection<Student>(); selectedStudents = new ObservableCollection<Student>();
            isFulfilled = false;

            loadData();
        }
        #endregion

        #region Commands
        public RelayCommand StudentImagePreviewCommand
        {
            get
            {
                return new RelayCommand(studentImagePreview);
            }
        }
        public RelayCommand<string> OrdersTableKeyUpCommand
        {
            get
            {
                return new RelayCommand<string>(ordersTableKeyUp);
            }
        }
        public RelayCommand<string> OrderItemsTableKeyUpCommand
        {
            get
            {
                return new RelayCommand<string>(orderItemsMouseup);
            }
        }
        public RelayCommand OrdersDoubleClickCommand
        {
            get
            {
                return new RelayCommand(ordersPanelDoubleClick);
            }
        }
        public RelayCommand OrdersPanelGotFocusCommand
        {
            get
            {
                return new RelayCommand(orderPanelGotFocus);
            }
        }
        public RelayCommand OrderItemsPanelGotFocusCommand
        {
            get
            {
                return new RelayCommand(orderItemsPanelGotFocus);
            }
        }
        public RelayCommand OrderItemsDoubleClickCommand
        {
            get
            {
                return new RelayCommand(orderItemsDoubleClick);
            }
        }
        public RelayCommand StudentTableMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentTableMouseUp);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// this method loads the data to all the grids
        /// </summary>
        public void loadData()
        {
            isGalleryGroupVisible = Visibility.Collapsed;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            dgOrdersData = new ObservableCollection<Order>(clsOrders.getAllOrders(db));
            if (dgOrdersData.Count != 0)
            {
                selectedOrder = dgOrdersData.First();
                loadOrderDetails(galleryGroupName);
            }
            else
            {
                dgOrderItemsData = new ObservableCollection<View_Order>();
                groupImageSource = null; studentImageSource = null;
            }
        }
        /// <summary>
        /// we are using this method to load the data when user selects Orders By Students in Orders tab
        /// </summary>
        /// <param name="isFromOrdersByStudents"></param>
        public void loadData(bool isFromOrdersByStudents)
        {
            isGalleryGroupVisible = Visibility.Collapsed;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            dgStudentsData = new ObservableCollection<Student>(clsOrders.studentsInOrders(db));
            if (dgStudentsData.Count != 0)
            {
                dgOrdersData = new ObservableCollection<Order>(clsOrders.ordersForStudent(db, dgStudentsData.First().ID));
                if (dgOrdersData.Count != 0)
                {
                    selectedOrder = dgOrdersData.First();
                    loadOrderDetails(galleryGroupName);
                }
            }
            else
            {
                dgOrderItemsData = new ObservableCollection<View_Order>();
                groupImageSource = null; studentImageSource = null;
            }
        }
        /// <summary>
        /// we are using this method to load the data when user selects Orders By Galler Group in Orders tab
        /// </summary>
        /// <param name="isFromOrdersByGalleryGroup"></param>
        public void loadData(int isFromOrdersByGalleryGroup)
        {
            isGalleryGroupVisible = Visibility.Visible;
            cbGallerygroupData = new ObservableCollection<ComboBoxItem>();
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            List<string> tempData = clsOrders.getGalleryGroupNames(db).ToList(); ;
            tempData.ForEach(a =>
            {
                cbGallerygroupData.Add(new ComboBoxItem { Name = a });
            });
            if (cbGallerygroupData.Count > 0)
            {
                galleryGroupName = cbGallerygroupData.First().Name;
            }

        }
        internal void loadGalleryData()
        {
            dgOrdersData = new ObservableCollection<Order>(clsOrders.getOrdersByGalleryName(db, galleryGroupName));
            if (dgOrdersData.Count != 0)
            {
                selectedOrder = dgOrdersData.First();
                loadOrderDetails(galleryGroupName);
            }
            else
            {
                dgOrderItemsData = new ObservableCollection<View_Order>();
                groupImageSource = null; studentImageSource = null;
            }
        }
        internal void addImagesToOrders()
        {
            if (selectedOrder != null)
            {
                //have to call Add To Orders Window
                AddToOrders _objAddToOrders = new AddToOrders(selectedOrder.Id, false);
                _objAddToOrders.ShowDialog();

                if (((AddToOrdersViewModel)(_objAddToOrders.DataContext)).isSave)
                {
                    List<int> ids = ((AddToOrdersViewModel)(_objAddToOrders.DataContext)).newStudentPhotoOrders.Select(sp => sp.Id).ToList();
                    int[] array = ids.ToArray();
                    ObservableCollection<View_Order> tempOrdersData = new ObservableCollection<View_Order>(clsOrders.getImagesOrdersFromIds(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), array, selectedOrder.Id));

                    for (int i = 0; i < tempOrdersData.Count; i++)
                    {
                        dgOrderItemsData.Add(tempOrdersData[i]);
                    }
                    checkForEmptyBillingCode();
                }
            }
            else
            {
                MVVMMessageService.ShowMessage("Select an order to add images.");
            }
        }
        public void newRecord(string isFrom)
        {
            NewManualOrders _objNewManualOrders = new NewManualOrders();
            _objNewManualOrders.ShowDialog();
            if (isFrom == "")
                loadData();
            else if (isFrom == "1") //Orders by gallery group name
                loadData(1);
            else
                loadData(true); //Orders by students

            selectedOrder = (from or in dgOrdersData orderby or.CreatedOn descending select or).FirstOrDefault();
            if (VisibleData.Count == (dgOrdersData.Count - 1))
                VisibleData = new List<Order>();
            if (selectedOrder != null)
            {
                if (isFrom == "")
                    loadOrderDetails("");
                else if (isFrom == "1")
                    loadOrderDetails(galleryGroupName);
                else
                    loadOrderDetails("");
            }
            else
            {
                dgOrderItemsData = new ObservableCollection<View_Order>();
                groupImageSource = null; studentImageSource = null;
            }
        }
        internal void removeOrderItem()
        {
            //select order items 
            //delete from order
            ArrayList arrPhotoOrderId = new ArrayList();
            if (selectedOrderItems.Count != 0)
            {

                //delete order items with Student Photo Order Id , so that it is easy to delete multiple order items having diff. order Ids
                //orderId = selectedOrderItems.First().OrderId;
                foreach (View_Order orderItem in selectedOrderItems)
                {
                    if (orderItem != null)
                    {
                        arrPhotoOrderId.Add(orderItem.StudentPhotoOrderId);
                    }
                }
                if (arrPhotoOrderId.Count == 0) { return; }
                string message = errorMessages.REMOVE_IMAGES_FROM_GROUP1 + arrPhotoOrderId.Count + " order items. ";
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    int res = clsOrders.removeImagesFromOrder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrPhotoOrderId);
                    foreach (int stuPhotoOrdId in arrPhotoOrderId)
                    {
                        dgOrderItemsData.Remove((from ord in dgOrderItemsData where ord.StudentPhotoOrderId == stuPhotoOrdId select ord).First());
                    }

                    checkForEmptyBillingCode();
                    //loadOrderDetails();
                }
            }
        }
        internal void editRecord()
        {
            if (selectedGrid == "OrderItems")
            {
                orderItemsDoubleClick();
            }
            else
            {
                if (selectedOrder == null) { return; }
                NewOrder _objEditManualOrderViewModel = new NewOrder(selectedOrder, "fromOrders");
                _objEditManualOrderViewModel.ShowDialog();

                if (((EditManualOrderViewModel)(_objEditManualOrderViewModel.DataContext)).isSave)
                {
                    int tempOrderIndex = dgOrdersData.Count <= 1 ? 0 : dgOrdersData.IndexOf(selectedOrder);
                    dgOrdersData.Remove(selectedOrder);

                    dgOrdersData.Insert(tempOrderIndex, ((EditManualOrderViewModel)(_objEditManualOrderViewModel.DataContext)).addEditOrder);
                    selectedOrder = ((EditManualOrderViewModel)(_objEditManualOrderViewModel.DataContext)).addEditOrder;
                    selectedOrders.Add(selectedOrder);
                }

            }
        }
        /// <summary>
        /// used to edit OrderItemBilliingCodes for a selected order
        /// </summary>
        /// <param name="isFrom">Orders or OrderByGalleryGroup</param>
        internal void editBillingCode(string isFrom)
        {
            //get all the order items whose billing code is null or empty for selected Order
            //open EditOrderItembillingCodeWindow
            //reload all the orders
            //you are done
            if (selectedOrder != null)
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                List<int> orderItems = clsOrders.getNullBillingCodeByOrderId(db, selectedOrder.Id);

                if (orderItems.Count > 0)
                {
                    EditOrderItemBilliingCode _objEditOrderItemBilliingCode = new EditOrderItemBilliingCode(orderItems);
                    _objEditOrderItemBilliingCode.ShowDialog();

                    if (((EditOrderItemBilliingCodeViewModel)(_objEditOrderItemBilliingCode.DataContext)).isSave)
                    {
                        if (isFrom == "")
                            loadOrderDetails("");
                        else if (isFrom == "1")
                            loadOrderDetails(galleryGroupName);
                    }
                }
                else
                {
                    MVVMMessageService.ShowMessage("No order items found without a billing code.");
                }
            }
        }
        public void deleteOrder()   //#Mohan ; #NUnitTest
        {
            try
            {
                string message = "";

                lstOrderId = (from soi in selectedOrders select soi.Id).Distinct().ToList();

                if (lstOrderId.Count != 0)
                {
                    if (lstOrderId.Count > 1)
                        message = "Deleting order will aslo delete related student photo orders. \n\n Are you sure you want to delete Multiple Orders ? ";
                    else
                        message = "Deleting order will aslo delete related student photo orders. \n\n Are you sure you want to delete Order " + selectedOrder.Id + " ? ";

                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgOrdersData.Count();
                        //int deletedRecordsCount = lstOrderId.Count;

                        int res = clsOrders.deleteOrdersById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), lstOrderId);
                        if (res != 0)
                        {
                            //loadData();
                            foreach (int ordId in lstOrderId)
                            {
                                dgOrdersData.Remove(dgOrdersData.Where(i => i.Id == ordId).First());
                            }

                            if (dgOrdersData.Count == 0) { dgOrderItemsData = null; }

                            if (selectedOrder != null) { loadOrderDetails(""); }

                            orderNo = ""; orderDate = ""; vendorOrderId = ""; vendorOrderDate = ""; isFulfilled = false;
                            studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                            groupImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));

                            //createDeletedRecordsLogFile("Orders", totalRecordsCount, deletedRecordsCount);
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
        bool noclassimage = false;
        /// <summary>
        /// this method used to export Orders
        /// It excludes orders those doesn't have studentImage info in order details
        /// </summary>
        internal void exportOrders()
        {
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
                string ordersWithoutImages = "";
                int exportedOrdersCount = 0;
                bool hasEmptyStuImage = false;
                bool isManual = false; bool isXml = false;

                List<Order> tempSelectedOrders = new List<Order>(selectedOrders);
                if (tempSelectedOrders.Count > 0) //By hema 09/12/2015
                {
                    foreach (Order order in tempSelectedOrders)
                    {
                        List<StudentPhotoOrder> lstOrderItems = clsOrders.getImagesIdsByOrder(db, order.Id);

                        //check wether order details contain any empty stu image info.
                        if ((from loi in lstOrderItems where loi.StudentImageId == null select loi).FirstOrDefault() != null)
                        {
                            hasEmptyStuImage = true;
                            continue;
                        }
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
                        if (string.IsNullOrEmpty(ordersWithoutImages))
                            ordersWithoutImages = ordersWithoutImages + order.Id;
                        else
                            ordersWithoutImages = ordersWithoutImages + ", " + order.Id;
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
                        else
                        {
                            if (!string.IsNullOrEmpty(tempGroupImageName))
                            {
                                dicStudentImages[studentImageId] = tempGroupImageName;
                            }
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

                ExportOrders _objExportOrders = new ExportOrders(dicStudentImages, stuImageIds, selectedStuImageIds, dicOrderItemBillingCode, hasFiltered, allOrders, tempAllOrderIds, tempselectedOrderIds);
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
                    if (!string.IsNullOrEmpty(ordersWithoutImages))
                    {
                        MissingOrders _objMissingOrders = new MissingOrders(ordersWithoutImages, "from OrdersExport");
                        _objMissingOrders.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// This method used to select path from which orders to be import
        /// It contactains 3 types 1)-XML files from FTP site(only for neal) 2)-XML files from local disk 3)-excel file from local disk
        /// </summary>
        internal void importOrdersPathSelector()
        {
            ImportOrderPathSelector _objImportOrderPathSelector = new ImportOrderPathSelector();
            _objImportOrderPathSelector.ShowDialog();

            if (((ImportOrderPathSelectorViewModel)(_objImportOrderPathSelector.DataContext)).isSave)
            {
                string folderPath = ((ImportOrderPathSelectorViewModel)(_objImportOrderPathSelector.DataContext)).folderPath;
                bool fromExcel = ((ImportOrderPathSelectorViewModel)(_objImportOrderPathSelector.DataContext)).isFromExcelChecked;
                DataTable dt = ((ImportOrderPathSelectorViewModel)(_objImportOrderPathSelector.DataContext)).dt;
                bool isGotPhotoChecked = ((ImportOrderPathSelectorViewModel)(_objImportOrderPathSelector.DataContext)).isFromGotPhotoChecked;
                importOrdersProgressBar(folderPath, fromExcel, isGotPhotoChecked, dt);
            }
        }
        /// <summary>
        /// This method used to import orders from Excel or XMl file(s)
        /// </summary>
        /// <param name="folderPath"> this is for XMl type files(those from FTP server (or) local disk) </param>
        /// <param name="isFromExcel"> this is to import orders from excel</param>
        /// <param name="dt"></param>
        internal void importOrdersProgressBar(string folderPath, bool isFromExcel, bool isGotPhotoChecked, DataTable dt)
        {
            ImportOrdersProgressBar _objImportOrdersProgressBar = new ImportOrdersProgressBar(folderPath, isFromExcel, isGotPhotoChecked, dt);
            _objImportOrdersProgressBar.ShowDialog();

            if (((ImportOrdersProgressBarViewModel)(_objImportOrdersProgressBar.DataContext)).xmlFileCount)
            {
                List<int> lstMissedBillingCode = ((ImportOrdersProgressBarViewModel)(_objImportOrdersProgressBar.DataContext)).lstMissedBillingCode;
                if (lstMissedBillingCode.Count > 0)
                {
                    //show editable grid that user can edit billing code of each order item
                    //show billing codes in combobox
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    string message = "Some order items found with no billing code.\n\nDo you want to open those orders to edit the billing code ?";
                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        EditOrderItemBilliingCode _objEditOrderItemBilliingCode = new EditOrderItemBilliingCode(lstMissedBillingCode);
                        _objEditOrderItemBilliingCode.ShowDialog();
                    }
                }
                loadData();
            }
        }

        internal void shiprushTracking()
        {
            //MVVMMessageService.ShowMessage("Implement");
            try
            {
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Information;
                string message = "This method will update the Tracking number and isShipped information in Orders based on the OrderId value in the excel.\nRequired columns are: OrderId,Tracking number,isShipped.\nDo you want to continue?";
                if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.Filter = "Excel Files (.xlsx)|*.XLSX|Excel Files (.xls)|*.xls";
                    //dlg.Filter = "Excel Files (.xlsx)|*.XLSX ";
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        txtPath = dlg.FileName;
                        dt = PhotoForce.Helpers.DataLoader.getDataTableFromExcel(txtPath);

                        if (dt == null || dt.Rows.Count == 0)
                        {
                            MVVMMessageService.ShowMessage("There is no information in excel sheet.");
                            return;
                        }
                        else
                        {
                            string res = checkForRequiredColumns(dt, "TrackingnumberUpdate");
                            if (res != "")
                            {
                                MVVMMessageService.ShowMessage(res);
                                return;
                            }
                        }

                        #region Update orders through excel
                        DataView dv = dt.DefaultView;
                        dv.Sort = "OrderID asc";
                        dt = dv.ToTable();

                        //List<Tuple<int, int>> tempFailedUpdate;
                        string tempFailedUpdateOrders = "";
                        bool isError = true;

                        List<int> tempOrderIDs = new List<int>();

                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                //MVVMMessageService.ShowMessage("OrderId column can not be empty or null", "ShipRush Tracking", MessageBoxButton.OK, MessageBoxImage.Warning); 
                                if (String.IsNullOrEmpty((dr["OrderId"]).ToString())) { continue; }
                                int tempOrderId = Convert.ToInt32(dr["OrderId"]);
                                if (!tempOrderIDs.Contains(tempOrderId))
                                {
                                    tempOrderIDs.Add(tempOrderId);
                                    Order _objtempOrder = clsOrders.getOrderDetailsByOrderId(db, tempOrderId);
                                    if (_objtempOrder != null)
                                    {
                                        if ((!string.IsNullOrEmpty(dr["TrackingNumber"].ToString())) || (!string.IsNullOrEmpty(dr["isShipped"].ToString())))
                                        {
                                            _objtempOrder.TrackingNumber = dr["TrackingNumber"].ToString() != "" ? Convert.ToString(dr["TrackingNumber"]) : null;
                                            _objtempOrder.isShipped = dr["isShipped"].ToString() != "" ? (dr["isShipped"].ToString().ToLower() == "y" ? (bool?)true : null) : null;

                                            db.SubmitChanges();
                                            isError = false;
                                        }
                                        else
                                        {
                                            tempFailedUpdateOrders += tempOrderId.ToString() + "\t";
                                        }
                                    }
                                    else
                                    {
                                        tempFailedUpdateOrders += tempOrderId.ToString() + "\t";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                isError = true;
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }

                        if (tempFailedUpdateOrders != "")
                        {
                            MVVMMessageService.ShowMessage("The following orders are failed to update. OrderId \n" + tempFailedUpdateOrders);
                        }

                        if (!isError)
                        {
							MVVMMessageService.ShowMessage("Shiprush tracking completed successfully.");
                            loadData();
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }

        }

        private void studentTableMouseUp()
        {
            if (selectedStudent != null)
            {
                setVisibilityForStudentGrid();
                try
                {
                    //strFileToShow = "";
                    dgOrdersData = new ObservableCollection<Order>(clsOrders.ordersForStudent(db, selectedStudent.ID));
                    if (dgOrdersData.Count != 0)
                    {
                        selectedOrder = dgOrdersData.First();
                        loadOrderDetails("");
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.ToString());
                }
            }
        }
        private void studentImagePreview()
        {
            try
            {
                string filePath = imagePathToShow.ToString();
                if (File.Exists(filePath))
                {
                    Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void orderItemsMouseup(string isFromOrdersByStudent)
        {
            selectedGrid = "OrderItems";
            buttonsVisibilityForOrderDetails();

            if (Convert.ToBoolean(isFromOrdersByStudent)) { (Application.Current as App).isOrderButtonsVisible = false; (Application.Current as App).isAddGroupPhotobtnVisible = false; }

            selectImage();
        }
        private void ordersTableKeyUp(string isFromOrdersByStudent)
        {
            try
            {
                buttonsVisibilityForOrders();

                if (Convert.ToBoolean(isFromOrdersByStudent)) { (Application.Current as App).isOrderButtonsVisible = false; (Application.Current as App).isAddGroupPhotobtnVisible = false; }
                imagePathToShow = "";
                loadOrderDetails("");
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// galleryGroup Name used in showing orders based on selected gallery group
        /// </summary>
        /// <param name="galleryGroupName"></param>
        private void loadOrderDetails(string galleryGroupName)
        {
            if (selectedOrder != null)
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (string.IsNullOrEmpty(galleryGroupName))
                {
                    dgOrderItemsData = new ObservableCollection<View_Order>(clsOrders.getOrderItemsById(db, selectedOrder.Id));
                }
                else
                {
                    dgOrderItemsData = new ObservableCollection<View_Order>(clsOrders.getItemsByGalleryNameAndId(db, galleryGroupName, selectedOrder.Id));
                }
                if (dgOrderItemsData.Count != 0)
                {
                    selectedStudentPhoto = dgOrderItemsData.First();
                    selectImage();
                }
                else
                {
                    studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    groupImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                }
            }
        }
        private void ordersPanelDoubleClick()
        {
            selectedGrid = "Orders";
            editRecord();

        }
        void orderItemsDoubleClick()
        {
            if (selectedStudentPhoto != null)
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (selectedStudentPhoto.StudentImageId == null) { return; }
                AddPackagesToOrderItem _objAddPackagesToOrderItem = new AddPackagesToOrderItem(selectedStudentPhoto);
                _objAddPackagesToOrderItem.ShowDialog();

                if (((AddPackagesToOrderItemViewModel)(_objAddPackagesToOrderItem.DataContext)).isSave)
                {
                    int tempOrderItemIndex = dgOrderItemsData.Count <= 1 ? 0 : dgOrderItemsData.IndexOf(selectedStudentPhoto);
                    dgOrderItemsData.Remove(selectedStudentPhoto);

                    dgOrderItemsData.Insert(tempOrderItemIndex, ((AddPackagesToOrderItemViewModel)(_objAddPackagesToOrderItem.DataContext)).OrderItem);
                    selectedStudentPhoto = ((AddPackagesToOrderItemViewModel)(_objAddPackagesToOrderItem.DataContext)).OrderItem;
                    selectedOrderItems.Add(selectedStudentPhoto);

                    checkForEmptyBillingCode();
                }
            }
        }
        /// <summary>
        /// 1) tries to update Fulfilled flag based on SimplePhotoBillingCode is null (or) empty 
        /// 2) tries to update retouch flag based on SimplePhotoBillingCode = 'F135'
        /// 3) tries to upadte hasNotes flag based on Comments != null (or)  empty
        /// </summary>
        void checkForEmptyBillingCode()
        {
            if (selectedOrder == null) { return; }
            View_Order OrderItem = clsOrders.getAllOrderItemsBillingCodes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedOrder.Id);
            if (OrderItem == null && selectedOrder.isSimplePhotoBillingCodeFilled == true)
            {
                updateBillingCodeFlag(false);
            }
            else if (OrderItem != null && selectedOrder.isSimplePhotoBillingCodeFilled == false)
            {
                updateBillingCodeFlag(true);
            }

            //bool Retouch = OrderItem == null ? false : clsOrders.isRetouchTrue(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedOrder.Id, (int)OrderItem.StudentImageId);
            //if (Retouch)
            View_Order Retouch = (from vo in db.View_Orders where vo.OrderId == selectedOrder.Id && vo.sp_SimplePhotoBillingCode == "F135" select vo).FirstOrDefault();
            if (Retouch != null)
                updateOrderRetouchFlag(true);
            else
                updateOrderRetouchFlag(false);
            bool HasNotes = clsOrders.updateOrderHasNotes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedOrder.Id);
            if (HasNotes == true)
            {
                int tempOrderItemIndex = dgOrdersData.Count <= 1 ? 0 : dgOrdersData.IndexOf(selectedOrder);

                Order tempOrder = clsOrders.getOrderById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), (int)selectedOrder.Id);
                dgOrdersData.Remove(selectedOrder);
                dgOrdersData.Insert(tempOrderItemIndex, tempOrder);
                selectedOrder = tempOrder;
            }

            #region//update StandardRetouch
            List<StudentPhotoOrder> OrderItemsList = (from oi in db.StudentPhotoOrders where oi.OrderId == (selectedOrder.Id) select oi).ToList();
            bool isM134Exists = false; bool isM134SubExists = false;
            foreach (StudentPhotoOrder oi in OrderItemsList)
            {
                if (oi.sp_SimplePhotoBillingCode == "M134")
                    isM134Exists = true;
                else if (oi.sp_SimplePhotoBillingCode == "M101" || oi.sp_SimplePhotoBillingCode == "M102" || oi.sp_SimplePhotoBillingCode == "M127" || oi.sp_SimplePhotoBillingCode == "M128" || oi.sp_SimplePhotoBillingCode == "S138" || oi.sp_SimplePhotoBillingCode == "S139")
                    isM134SubExists = true;

                if (isM134Exists && isM134SubExists)
                {
                    if (selectedOrder.IsStandardRetouch == false)
                    {
                        clsOrders.updateOrderStandardRetouch(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), true, (int)selectedOrder.Id);
                        replaceOrder(selectedOrder.Id);
                    }
                    break;
                }
            }
            if((!isM134Exists || !isM134SubExists) && selectedOrder.IsStandardRetouch == true)
            {
                {
                    clsOrders.updateOrderStandardRetouch(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), false, (int)selectedOrder.Id);
                    replaceOrder(selectedOrder.Id);
                }
            }
            #endregion
        }
        private void replaceOrder(int Id)
        {
            Order tempOrder = clsOrders.getOrderById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Id);
            int tempOrderItemIndex = dgOrdersData.Count <= 1 ? 0 : dgOrdersData.IndexOf(selectedOrder);

            dgOrdersData.Remove(selectedOrder);
            dgOrdersData.Insert(tempOrderItemIndex, tempOrder);
            selectedOrder = tempOrder;
        }
        /// <summary>
        /// tries to update Fulfilled flag based on SimplePhotoBillingCode is null (or) empty 
        /// </summary>
        /// <param name="value">false if null</param>
        void updateBillingCodeFlag(bool value)
        {
            int result3 = clsOrders.UpdateOrder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), (int)selectedOrder.Id, value);

            Order tempOrder = clsOrders.getOrderById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), (int)selectedOrder.Id);
            int tempOrderItemIndex = dgOrdersData.Count <= 1 ? 0 : dgOrdersData.IndexOf(selectedOrder);

            dgOrdersData.Remove(selectedOrder);
            dgOrdersData.Insert(tempOrderItemIndex, tempOrder);
            selectedOrder = tempOrder;
            //loadData();
        }
        /// <summary>
        /// tries to update retouch flag based on SimplePhotoBillingCode = 'F135'
        /// </summary>
        /// <param name="value">true if SimplePhotoBillingCode = 'F135'</param>
        void updateOrderRetouchFlag(bool value)
        {
            clsOrders.updateOrderRetouch(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), value, (int)selectedOrder.Id);

            Order tempOrder = clsOrders.getOrderById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), (int)selectedOrder.Id);
            int tempOrderItemIndex = dgOrdersData.Count <= 1 ? 0 : dgOrdersData.IndexOf(selectedOrder);

            dgOrdersData.Remove(selectedOrder);
            dgOrdersData.Insert(tempOrderItemIndex, tempOrder);
            selectedOrder = tempOrder;
        }
        /// <summary>
        /// The system will attempt to assign the class photo using existing groups
        /// </summary>
        internal void autoCreateGroupPhotos()
        {
            //Mohan Tangella
            //have to get all the images within the order
            //foreach all the order items
            //with the studentimageid findout how many groups it associated with
            //group count is > 1 ,check wether any one of the group has class photo
            //if it contains multiple groups and multiple class photos return top most class photo.
            //add the class photo id in studentphotoorder table for respective order item.
            //you are done.

            int i = 0;
            string message = "The system will now attempt to assign the class photo using existing groups.\n\n Continue?";
            MessageBoxButton mbButtons = MessageBoxButton.OKCancel;
            MessageBoxImage mbImage = MessageBoxImage.Question;

            if (MVVMMessageService.ShowMessage(message, "Confirmation", mbButtons, mbImage) == System.Windows.MessageBoxResult.OK)
            {
                try
                {

                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    List<int?> lstOrderIds = (from soi in dgOrderItemsData select soi.OrderId).Distinct().ToList();

                    if (lstOrderIds.Count != 0)
                    {
                        foreach (int orderIdItem in lstOrderIds)
                        {
                            List<StudentPhotoOrder> lstImages = clsOrders.getImagesIdsByOrder(db, orderIdItem);
                            if (lstImages.Count != 0)
                            {
                                foreach (StudentPhotoOrder orderItem in lstImages)
                                {
                                    if (orderItem.StudentImageId == null) { continue; }
                                    int? classPhoto = clsOrders.getGroupPhotoByImageId(db, orderItem.StudentImageId);
                                    //update classPhoto in student photo order
                                    orderItem.GroupImageId = classPhoto;
                                    db.SubmitChanges();

                                    if (classPhoto != 0) { i++; }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                    clsStatic.WriteExceptionLogXML(ex);
                }
            }
            if (i > 0)
            {
                MVVMMessageService.ShowMessage("Class photo assigned to " + i + " Order items.");
                loadData();
            }
        }
        /// <summary>
        /// this method tries to assign groupClassPhoto to the selected orderItem
        /// </summary>
        internal void addClassPhotoToOrderItem()
        {
            if (selectedStudentPhoto != null)
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                StudentPhotoOrder _objStudentPhotoOrder = clsOrders.getStudentPhotoOrderById(db, selectedStudentPhoto.StudentPhotoOrderId);
                AddToOrders _objAddToOrders = new AddToOrders((int)selectedStudentPhoto.OrderId, true);
                _objAddToOrders.ShowDialog();

                if (((AddToOrdersViewModel)(_objAddToOrders.DataContext)).isSave)
                {
                    int classPhoto = ((AddToOrdersViewModel)(_objAddToOrders.DataContext)).classPhotoId;
                    selectedStudentPhoto.GroupImageId = classPhoto;
                    _objStudentPhotoOrder.GroupImageId = classPhoto;
                    db.SubmitChanges();
                    selectImage();
                }
            }
        }
        /// <summary>
        /// This method tries to find the images which are present in Photo Force but not found in hard disk
        /// </summary>
        internal void checkMissingImages()
        {
            string message = "This method tries to find the images which are present in Photo Force but cannot found in hard disk.\nDo you want to continue?";
            string caption = "Confirmation";
            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
            if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
            {
                List<int> lstOrderIds = (from soi in dgOrdersData select soi.Id).Distinct().ToList();
                foreach (int orderid in lstOrderIds)
                {
                    List<StudentPhotoOrder> lstImages = clsOrders.getImagesIdsByOrder(db, orderid);
                    foreach (StudentPhotoOrder img in lstImages)
                    {
                        if (img.StudentImage == null) { clsOrders.updateHasMissingImages(db, true, orderid); break; }
                        string imgname = img.StudentImage.ImageName;
                        int? photoshootid = img.StudentImage.PhotoShootID;
                        string imagefolder = clsOrders.getImageFolder(db, photoshootid);
                        if (!File.Exists(imagefolder + "\\" + imgname))
                        {
                            clsOrders.updateHasMissingImages(db, true, orderid);
                            break;
                        }
                    }
                }
                dgOrdersData = new ObservableCollection<Order>(clsOrders.getAllOrders(db));
                MVVMMessageService.ShowMessage("Orders missing images information updated.");
            }
        }
        /// <summary>
        /// To edit more than one order at a time
        /// </summary>
        internal void bulkEditOrders()
        {
            if (selectedOrders.Count != 0)
            {
                UpdateOrderTrackingNumber _objUpdateOrderTrackingNumber = new UpdateOrderTrackingNumber(selectedOrders);
                _objUpdateOrderTrackingNumber.ShowDialog();

                if (((UpdateOrderTrackingNumberViewModel)(_objUpdateOrderTrackingNumber.DataContext)).isSave)
                {
                    dgOrdersData = new ObservableCollection<Order>(clsOrders.getAllOrders(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString)));
                }
            }
        }
        int? OrderId;
        int SchoolID = 0;
        AddToOrders _objAddToOrders;
        /// <summary>
        /// this method tries to assign StudentImage to the orderItem if orderItem doesn't contain image details
        /// </summary>
        internal void assignOrderImages()
        {
            if (selectedStudentPhoto != null)
            {

                if (selectedStudentPhoto.StudentImageId != null) { MVVMMessageService.ShowMessage("Order item already attached to an image."); return; }
                //if (OrderId == selectedStudentPhoto.OrderId )
                //{
                _objAddToOrders = new AddToOrders(selectedStudentPhoto.StudentPhotoOrderId, SchoolID);
                _objAddToOrders.ShowDialog();
                //}
                //else
                //{
                //    _objAddToOrders = new AddToOrders(selectedStudentPhoto.StudentPhotoOrderId, 0);
                //    _objAddToOrders.ShowDialog();
                //}
                if (((AddToOrdersViewModel)(_objAddToOrders.DataContext)).isSave)
                {
                    OrderId = selectedStudentPhoto.OrderId;
                    SchoolID = ((AddToOrdersViewModel)(_objAddToOrders.DataContext)).selectedSchoolId;

                    dgOrderItemsData = new ObservableCollection<View_Order>(clsOrders.getOrderItemsById(db, (int)selectedStudentPhoto.OrderId));
                }
            }
        }
        /// <summary>
        /// this method finds the all images that doesn't have orders from a group and importbatch
        /// </summary>
        internal void imagesWithoutOrders()
        {
            ImagesWithoutOrders _objImagesWithoutOrders = new ImagesWithoutOrders();
            _objImagesWithoutOrders.ShowDialog();
        }

        # region Select Image
        /// <summary>
        /// Just to show the selected student image on imgStudentPhotoPreview grid
        /// </summary>
        private void selectImage()
        {
            try
            {
                int? ImageId = 0;
                int? groupImageId = 0;
                if (selectedStudentPhoto != null)
                {
                    ImageId = selectedStudentPhoto.StudentImageId;
                    groupImageId = selectedStudentPhoto.GroupImageId;

                    if (ImageId == null)
                    {
                        studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        showImagePreview("Preview", ImageId);
                    }
                    if (groupImageId.GetValueOrDefault() == 0)
                    {
                        groupImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        showImagePreview("Group", groupImageId);
                    }
                }
                else
                {
                    studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    groupImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                }

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        void showImagePreview(string imageTitle, int? tempImageId)
        {
            StudentImage objStudentImage = clsDashBoard.getStudentImageDetailsById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(tempImageId));
            if (objStudentImage != null)
            {
                Photoshootpath = objStudentImage.PhotoShoot.ImageFolder;
                string strFile = objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + objStudentImage.ImageName;
                imagePathToShow = objStudentImage.PhotoShoot.ImageFolder + "\\" + objStudentImage.ImageName;
                if (!File.Exists(strFile))
                {
                    if(imageTitle == "Group")
                        groupImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    else
                        studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                }
                else
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(strFile);
                    if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                    {
                        decoderForJpeg(strFile, imageTitle);
                    }
                }
            }
        }

        private void decoderForJpeg(string strFile, string imageTitle)
        {
            using (var stream = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapImage thumb = new BitmapImage();
                stream.Seek(0, SeekOrigin.Begin);
                thumb.BeginInit();
                thumb.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
                thumb.CacheOption = BitmapCacheOption.OnLoad;
                //thumb.DecodePixelWidth = 200;

                if (imageTitle == "Group")
                {
                    thumb.StreamSource = stream;
                    thumb.EndInit();
                    groupImageSource = thumb;
                }
                else
                {
                    thumb.StreamSource = stream;
                    thumb.EndInit();
                    studentImageSource = thumb;
                }
            }
        }
        # endregion
        /// <summary>
        /// this method to navigate to the images source folder 
        /// </summary>
        internal void openFolder()
        {
            if (Directory.Exists(Photoshootpath))
            {
                string args = string.Format("/Select, \"{0}\"", imagePathToShow);
                ProcessStartInfo pfi = new ProcessStartInfo("Explorer.exe", args);

                System.Diagnostics.Process.Start(pfi);
            }
            else
            {
                MVVMMessageService.ShowMessage(errorMessages.PATH_DOES_NOT_EXISTS);
            }
        }
        /// <summary>
        /// used for button visibility if Orders panel got focus
        /// </summary>
        void orderPanelGotFocus()
        {
            buttonsVisibilityForOrders();
        }
        /// <summary>
        /// used for button visibility if OrderItems panel got focus
        /// </summary>
        void orderItemsPanelGotFocus()
        {
            buttonsVisibilityForOrderDetails();
        }
        public void setVisibilityForStudentGrid()
        {
            selectedGrid = "Students";
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            //(Application.Current as App).isEditOrdersVisible = true; (Application.Current as App).isAssignOrderVisible = true;
            //(Application.Current as App).isImportOrdersVisible = true;
        }
        public void buttonsVisibilityForOrderDetails()
        {
            selectedGrid = "OrderItems";
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true; (Application.Current as App).isOpenFolderVisible = true;
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true; (Application.Current as App).isAddGroupPhotobtnVisible = true;
            (Application.Current as App).isRemoveFromGrpVisible = true; (Application.Current as App).isOrderButtonsVisible = true;
            (Application.Current as App).isEditOrdersVisible = true; (Application.Current as App).isAssignOrderVisible = true;
            (Application.Current as App).isImportOrdersVisible = true;
        }
        void buttonsVisibilityForOrders()
        {
            selectedGrid = "Orders";
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isOrderButtonsVisible = true;
            (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true; (Application.Current as App).isAddGroupPhotobtnVisible = true;
            (Application.Current as App).isRefreshVisible = true; (Application.Current as App).isBulkRenameVisible = true;
            (Application.Current as App).isEditOrdersVisible = true; (Application.Current as App).isAssignOrderVisible = true;
            (Application.Current as App).isImportOrdersVisible = true;
        }
        internal void searchPanels()
        {
            if (selectedGrid == "OrderItems")
            {
                if (ordersTableSearchControl == null || !isSearchControlVisible) //|| !ordersTableView.SearchControl.IsVisible)
                {
                    ordersTableSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    ordersTableSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
                }
            }
            else if (selectedGrid == "Orders")
            {
                if (ordersViewTableSearchControl == null || !isOrdersSearchControlVisible) //|| !ordersTableView.SearchControl.IsVisible)
                {
                    ordersViewTableSearchPanelMode = ShowSearchPanelMode.Always; isOrdersSearchControlVisible = true;
                }
                else
                {
                    ordersViewTableSearchPanelMode = ShowSearchPanelMode.Never; isOrdersSearchControlVisible = false;
                }
            }
            else
            {
                if (studentSearchControl == null || isSearchControlVisible == false)
                {
                    studentSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    studentSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
                }
            }
        }
        internal void groupPanels()
        {
            if (selectedGrid == "OrderItems")
            {
                if (ordersTableShowGroupPanel)
                    ordersTableShowGroupPanel = false;
                else
                    ordersTableShowGroupPanel = true;
            }
            else if (selectedGrid == "Orders")
            {
                if (ordersViewTableShowGroupPanel)
                    ordersViewTableShowGroupPanel = false;
                else
                    ordersViewTableShowGroupPanel = true;
            }
            else
            {
                if (studentShowGroupPanel)
                    studentShowGroupPanel = false;
                else
                    studentShowGroupPanel = true;
            }
        }
        /// <summary>
        /// Used to edit more than one orderItem at a time
        /// </summary>
        internal void bulkRename()
        {
            if (selectedGrid == "OrderItems")
            {
                orderItemsDoubleClick();
            }
            else
            {
                if (selectedOrders.Count != 0)
                {
                    NewOrder _objEditOrder = new NewOrder(selectedOrders, "fromOrders");
                    _objEditOrder.ShowDialog();

                    if (((EditManualOrderViewModel)(_objEditOrder.DataContext)).isSave)
                    {
                        dgOrdersData = new ObservableCollection<Order>(clsOrders.getAllOrders(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString)));
                    }
                }
            }
        }
        internal void hasNotes()
        {
            int count = 0;
            List<int> lstOrderIds = (from soi in dgOrdersData select soi.Id).Distinct().ToList();
            foreach (int orderid in lstOrderIds)
            {
                bool hasNotes = clsOrders.updateOrderHasNotes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), orderid);
                if (hasNotes)
                {
                    ++count;
                }
            }
            dgOrdersData = new ObservableCollection<Order>(clsOrders.getAllOrders(db));
            MVVMMessageService.ShowMessage("Orders 'Has Notes' information updated.\n" + count + " Order(s) with 'Has Notes'.");
        }
        /// <summary>
        /// This method will update the Vendor order number in photo force based on the OrderId value in the excel
        /// </summary>
        internal void updateOrdersThroughExcel()
        {
            try
            {
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Information;
                string message = "This method will update the Vendor order number in photo force based on the OrderId value in the excel.Required columns are: OrderId,Vendor Order Id,Vendor Date.\nDo you want to continue?";
                if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.Filter = "Excel Files (.xlsx)|*.XLSX";
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        txtPath = dlg.FileName;
                        dt = PhotoForce.Helpers.DataLoader.getDataTableFromExcel(txtPath);

                        if (dt == null || dt.Rows.Count == 0)
                        {
                            MVVMMessageService.ShowMessage("There is no information in excel sheet.");
                            return;
                        }
                        else
                        {
                            string res = checkForRequiredColumns(dt, "VendorOrderUpdate");
                            if (res != "")
                            {
                                MVVMMessageService.ShowMessage(res);
                                return;
                            }
                        }

                        #region Update orders through excel
                        DataView dv = dt.DefaultView;
                        dv.Sort = "OrderID asc";
                        dt = dv.ToTable();

                        //List<Tuple<int, int>> tempFailedUpdate;
                        string tempFailedUpdateOrders = "";
                        bool isError = true;

                        List<int> tempOrderIDs = new List<int>();

                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                if (String.IsNullOrEmpty((dr["OrderId"]).ToString())) { MVVMMessageService.ShowMessage("OrderId column can not be empty or null"); continue; }
                                int tempOrderId = Convert.ToInt32(dr["OrderId"]);
                                if (!tempOrderIDs.Contains(tempOrderId))
                                {
                                    tempOrderIDs.Add(tempOrderId);
                                    Order _objtempOrder = clsOrders.getOrderDetailsByOrderId(db, tempOrderId);
                                    if (_objtempOrder != null)
                                    {
                                        _objtempOrder.VendorOrderNo = Convert.ToString(dr["Vendor Order Id"]);
                                        _objtempOrder.VendorDate = dr["Vendor Date"].ToString() == "" ? null : (DateTime?)((System.DateTime)(dr["Vendor Date"])).Date;
                                        db.SubmitChanges();
                                        isError = false;
                                    }
                                    else
                                    {
                                        //tempFailedUpdate = new List<Tuple<int, int>>();
                                        //tempFailedUpdate.Add(new  Tuple<int, int>(Convert.ToInt32(dr["OrderId"]), Convert.ToInt32(dr["Student id"])));
                                        tempFailedUpdateOrders += tempOrderId.ToString() + "\t";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                isError = true;
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }

                        if (tempFailedUpdateOrders != "")
                        {
                            MVVMMessageService.ShowMessage("The following orders are failed to update. e.g.OrderId \n" + tempFailedUpdateOrders);
                        }

                        if (!isError)
                        {
                            loadData();
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        public string checkForRequiredColumns(DataTable dt, string isFor)
        {
            //checking wether any column is missed.
            DataRow dr = dt.Rows[0];
            try
            {
                if (isFor == "TrackingnumberUpdate") //VendorOrderUpdate
                {
                    string OrderId = dr["OrderId"].ToString();
                    string TrackingNumber = dr["TrackingNumber"].ToString();
                    string isShipped = dr["isShipped"].ToString();
                }
                else
                {
                    string OrderId = dr["OrderId"].ToString();
                    string VendorOrderNumber = dr["Vendor Order Id"].ToString();
                    string VendorDate = dr["Vendor Date"].ToString();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return "Cannot find required column(s)." + Environment.NewLine + "Please check the excel file.";
            }
            return "";
        }
        #endregion
    }
}
