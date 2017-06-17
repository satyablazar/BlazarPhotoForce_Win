using DocumentFormat.OpenXml.ExtendedProperties;
using PhotoForce.App_Code;
using PhotoForce.Extensions;
using PhotoForce.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoForce.OrdersManagement
{
    public class NewManualOrdersViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        Order tempSelectedOrder = null;
        public bool isSave = false;
        StudentPhotoOrder oldselectedOrderItem;
        string Photoshootpath = "";
        string imagePathToShow = "";
        bool noPaymentMethod = false;
        decimal? differenceInAmount = 0;
        string tempbillingCode = "";
        StudentImage tempStudentImage;
        #endregion

        #region Properties
        int _orderId;
        decimal? _amount;
        DataTable _cbExistingBatchData;
        DataRowView _selectedExistingBatch;
        DateTime _orderDate;
        ObservableCollection<StudentImage> _dgStudentPhotosData;
        ObservableCollection<StudentPhotoOrder> _dgOrderItemsData;
        ObservableCollection<StudentImage> _selectedStudentPhotos;
        StudentImage _selectedStudentPhoto;
        PaymentMethods _selectedPaymentMethod;
        ObservableCollection<StudentPhotoOrder> _selectedOrderItems;
        StudentPhotoOrder _selectedOrderItem;
        ImageSource _studentImageSource;
        ImageSource _groupImageSource;
        List<OrderPackage> _orderPackagesData;
        Visibility _studentDeteilsEnabled;
        string _psStudentDetails;
        ObservableCollection<Student> _cbStudentData;
        Student _selectedStudent;
        bool _studentIdFocused;
        string _shippingFirstName;
        string _shippingAddress;
        string _shippingLastName;
        string _shippingCity;
        string _shippingState;
        string _shippingPostalCode;
        int? _simplePhotoOrderNumber;
        bool _isAllStudentsChecked;

        public bool isAllStudentsChecked
        {
            get { return _isAllStudentsChecked; }
            set 
            { 
                _isAllStudentsChecked = value; NotifyPropertyChanged();

                cbStudentData = new ObservableCollection<Student>();
                selectedStudent = null;
                if (isAllStudentsChecked)
                    cbStudentData = new ObservableCollection<Student>((from st in db.Students orderby st.ID descending select st).ToList());
                else
                    cbStudentData = new ObservableCollection<Student>(clsOrders.getStudentsHaventOrdered(db, clsSchool.defaultSchoolId));

                if(selectedStudent == null)
                    psStudentDetails = "";
            }
        }
        public int? simplePhotoOrderNumber
        {
            get { return _simplePhotoOrderNumber; }
            set { _simplePhotoOrderNumber = value; NotifyPropertyChanged(); }
        }
        public string shippingPostalCode
        {
            get { return _shippingPostalCode; }
            set { _shippingPostalCode = value; NotifyPropertyChanged(); }
        }
        public string shippingState
        {
            get { return _shippingState; }
            set { _shippingState = value; NotifyPropertyChanged(); }
        }
        public string shippingCity
        {
            get { return _shippingCity; }
            set { _shippingCity = value; NotifyPropertyChanged(); }
        }
        public string shippingLastName
        {
            get { return _shippingLastName; }
            set { _shippingLastName = value; NotifyPropertyChanged(); }
        }
        public string shippingAddress
        {
            get { return _shippingAddress; }
            set { _shippingAddress = value; NotifyPropertyChanged(); }
        }
        public string shippingFirstName
        {
            get { return _shippingFirstName; }
            set { _shippingFirstName = value; NotifyPropertyChanged(); }
        }
        public Student selectedStudent
        {
            get { return _selectedStudent; }
            set
            {
                _selectedStudent = value; NotifyPropertyChanged();
                if (selectedStudent != null)
                {
                    studentDeteilsEnabled = Visibility.Visible;
                    dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.getStudentImagesbyStudentId(db, selectedStudent.ID.ToString()));
                    psStudentDetails = (selectedStudent.FirstName) + " " + (selectedStudent.Lastname) + (selectedStudent.PhotographyJob != null ? (", " + selectedStudent.PhotographyJob.School.SchoolName) : "") + (selectedStudent.PhotographyJob != null ? (", " + selectedStudent.PhotographyJob.JobName) : "");
                    if (dgStudentPhotosData.Count != 0)
                    {
                        selectedStudentPhoto = dgStudentPhotosData.First();
                        if (selectedStudentPhoto != null)
                        {
                            selectImage();
                        }
                    }
                }
                else
                {
                    dgStudentPhotosData = new ObservableCollection<StudentImage>();
                    selectedStudentPhoto = null;
                    psStudentDetails = "";
                    studentDeteilsEnabled = Visibility.Hidden;
                }
            }
        }

        public ObservableCollection<Student> cbStudentData
        {
            get { return _cbStudentData; }
            set { _cbStudentData = value; NotifyPropertyChanged(); }
        }
        public string psStudentDetails
        {
            get { return _psStudentDetails; }
            set { _psStudentDetails = value; NotifyPropertyChanged("psStudentDetails"); }
        }
        public Visibility studentDeteilsEnabled
        {
            get { return _studentDeteilsEnabled; }
            set { _studentDeteilsEnabled = value; NotifyPropertyChanged("studentDeteilsEnabled"); }
        }
        public List<OrderPackage> orderPackagesData
        {
            get { return _orderPackagesData; }
            set { _orderPackagesData = value; NotifyPropertyChanged(); }
        }
        public StudentPhotoOrder selectedOrderItem
        {
            get { return _selectedOrderItem; }
            set
            {
                if (selectedOrderItem != null)
                {
                    oldselectedOrderItem = selectedOrderItem;
                    if (selectedStudentPhoto == null)
                    {
                        MVVMMessageService.ShowMessage("Please select student image first.");
                        dgOrderItemsData.Remove(oldselectedOrderItem);
                        oldselectedOrderItem = null;
                    }
                }
                _selectedOrderItem = value; NotifyPropertyChanged("selectedOrderItem");
            }
        }
        public ObservableCollection<StudentPhotoOrder> selectedOrderItems
        {
            get { return _selectedOrderItems; }
            set { _selectedOrderItems = value; NotifyPropertyChanged("selectedOrderItems"); }
        }
        public PaymentMethods selectedPaymentMethod
        {
            get { return _selectedPaymentMethod; }
            set { _selectedPaymentMethod = value; NotifyPropertyChanged("selectedPaymentMethod"); }
        }
        public decimal? amount
        {
            get { return _amount; }
            set { _amount = value; NotifyPropertyChanged("amount"); }
        }
        public StudentImage selectedStudentPhoto
        {
            get { return _selectedStudentPhoto; }
            set { _selectedStudentPhoto = value; NotifyPropertyChanged("selectedStudentPhoto"); }
        }
        public ObservableCollection<StudentImage> selectedStudentPhotos
        {
            get { return _selectedStudentPhotos; }
            set { _selectedStudentPhotos = value; NotifyPropertyChanged("selectedStudentPhotos"); }
        }
        public ObservableCollection<StudentPhotoOrder> dgOrderItemsData
        {
            get { return _dgOrderItemsData; }
            set
            {
                _dgOrderItemsData = value; NotifyPropertyChanged("dgOrderItemsData");
            }
        }
        public ObservableCollection<StudentImage> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        public DateTime orderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; NotifyPropertyChanged("orderDate"); }
        }
        public DataRowView selectedExistingBatch
        {
            get { return _selectedExistingBatch; }
            set { _selectedExistingBatch = value; NotifyPropertyChanged("selectedExistingBatch"); }
        }
        public DataTable cbExistingBatchData
        {
            get { return _cbExistingBatchData; }
            set { _cbExistingBatchData = value; NotifyPropertyChanged("cbExistingBatchData"); }
        }
        public int orderId
        {
            get { return _orderId; }
            set { _orderId = value; NotifyPropertyChanged("orderId"); }
        }
        public ImageSource groupImageSource
        {
            get { return _groupImageSource; }
            set { _groupImageSource = value; NotifyPropertyChanged("groupImageSource"); }
        }
        public ImageSource studentImageSource
        {
            get { return _studentImageSource; }
            set { _studentImageSource = value; NotifyPropertyChanged("studentImageSource"); }
        }
        public bool studentIdFocused
        {
            get { return _studentIdFocused; }
            set { _studentIdFocused = value; NotifyPropertyChanged("studentIdFocused"); }
        }
        #endregion

        #region Constructor
        public NewManualOrdersViewModel() 
        {
            dgOrderItemsData = new ObservableCollection<StudentPhotoOrder>();
            dgStudentPhotosData = new ObservableCollection<StudentImage>();
            selectedStudentPhotos = new ObservableCollection<StudentImage>();
            selectedExistingBatch = null;            
            bindData();
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndNewCommand
        {
            get
            {
                return new RelayCommand(saveAndNew);
            }
        }
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
        public RelayCommand StudentImagePreviewCommand
        {
            get
            {
                return new RelayCommand(studentImagePreview);
            }
        }
        public RelayCommand StudentPhotosTableKeyUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotosMouseup);
            }
        }
        public RelayCommand OrderItemsMouseUpCommand
        {
            get
            {
                return new RelayCommand(OrderItemsMouseUp);
            }

        }
        public RelayCommand CreateNewBatchCommand
        {
            get
            {
                return new RelayCommand(createNewBatch);
            }
        }
        public RelayCommand DeleteOrderItemCommand
        {
            get
            {
                return new RelayCommand(deleteOrderItem);
            }
        }
        #endregion

        #region Methods
        void bindData()
        {
            //cbStudentData = new ObservableCollection<Student>();
            NextOrder tempOrder = clsOrders.getNextOrder(db);
            if (tempOrder != null)
            {
                orderId = (int)tempOrder.NextOrderId;
            }
            orderDate = DateTime.Now;
            
            //if(isAllStudentsChecked)
            //    cbStudentData = new ObservableCollection<Student>((from st in db.Students orderby st.ID descending select st).ToList());
            //else
            //    cbStudentData = new ObservableCollection<Student>(clsOrders.getStudentsHaventOrdered(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString),clsSchool.defaultSchoolId ));

            //cbStudentData = new ObservableCollection<Student>(clsOrders.getStudentsHaventOrdered(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId));

            orderPackagesData = clsOrders.getAllOrderPackages(db).OrderBy(op => op.SortOrder).ToList();
            cbExistingBatchData = clsOrders.getAllManualImports(db);

            selectedExistingBatch = cbExistingBatchData.Rows.Count == 0 ? null : cbExistingBatchData.DefaultView[0];

            isAllStudentsChecked = true;
        }
        /// <summary>
        /// this method will create order and then closes the window
        /// </summary>
        void saveAndClose()
        {
            saveOrderItems();
            if (!noPaymentMethod) { DialogResult = false; }
            noPaymentMethod = false;
        }
        /// <summary>
        /// this method will create new order and then waits to create the next order 
        /// </summary>
        void saveAndNew()
        {
            saveOrderItems();
            noPaymentMethod = false;
            studentIdFocused = true;
            shippingAddress = "";
            shippingFirstName = "";
            shippingLastName = "";
        }
        /// <summary>
        /// creates the OrderItems for the newly created order
        /// </summary>
        private void saveOrderItems()
        {
            #region CreateManualOrder
            if (selectedStudentPhoto != null)
            {
                bool isError = false;
                if (dgOrderItemsData.Count == 0) { MVVMMessageService.ShowMessage("First add order details and try again."); noPaymentMethod = true; return; }
                else
                {
                    //check the count of line items and compare with total amount
                    //if not match save the diff. amount in Orders table (Variance column)
                    //So it the order total is 100 and the line item total is 110, then write -10. So the formula is Order Total - sum(line item total)

                    decimal? tempTotalAmount = 0;
                    foreach (StudentPhotoOrder orderItem in dgOrderItemsData)
                    {
                        tempTotalAmount = tempTotalAmount + (orderItem.LabCost == null ? 0 : (orderItem.LabCost * orderItem.Quantity));
                    }
                    if (amount != tempTotalAmount)
                    {
                        string message = "Order totals do not match. Save anyway?";
                        string caption = "Confirmation";
                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.OKCancel;
                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                        if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.OK)
                        {
                            differenceInAmount = amount - tempTotalAmount;
                        }
                        else
                        {
                            noPaymentMethod = true;
                            return;
                        }
                    }
                }
                isSave = true;
                try
                {
                    if (tempSelectedOrder == null)
                    {
                        if ((string.IsNullOrEmpty(selectedPaymentMethod.ToString())) || (amount == 0 || amount == null)) { MVVMMessageService.ShowMessage("Please enter amount and select a payment method."); noPaymentMethod = true; return; }
                        createOrder();
                    }

                    //Create or add order items to that Order
                    foreach (StudentPhotoOrder orderItem in dgOrderItemsData)
                    {
                        if (orderItem.StudentImage == null) { MVVMMessageService.ShowMessage("Student Image is not attached to order item."); noPaymentMethod = true; return; }

                        string billingCode = orderItem.sp_SimplePhotoBillingCode != null ? orderItem.sp_SimplePhotoBillingCode : "";

                        #region update PackagePrice
                        if (!string.IsNullOrEmpty(billingCode))
                        {
                            OrderPackage op = new OrderPackage();
                            op = clsOrders.getOrderPackagebyBillingCode(db, billingCode);

                            if (op.DefaultPrice != orderItem.LabCost && orderItem.LabCost < 1000)
                            {
                                op.DefaultPrice = orderItem.LabCost;

                                db.SubmitChanges();
                            }

                        }
                        #endregion

                        StudentPhotoOrder _objStudentPhotoOrder = new StudentPhotoOrder { StudentImageId = orderItem.StudentImage.ID, Quantity = orderItem.Quantity, sp_SimplePhotoBillingCode = orderItem.sp_SimplePhotoBillingCode, GroupImageId = orderItem.GroupImageId };
                        billingCode = orderItem.sp_SimplePhotoBillingCode != null ? orderItem.sp_SimplePhotoBillingCode : "";
                        int? classPhoto = null;
                        if (billingCode == "M127" || billingCode == "M128" || billingCode == "M129" ||
                                           billingCode == "M130" || billingCode == "M131" || billingCode == "M132" || billingCode == "M133" || billingCode == "M136" || billingCode == "M171")
                        {
                            classPhoto = clsOrders.getGroupPhotoByImageId(db, selectedStudentPhoto.ID);
                        }

                        //StudentPhotoOrder _objStudentPhotoOrder = new StudentPhotoOrder { StudentImageId = orderItem.StudentImage.ID, Quantity = orderItem.Quantity, sp_SimplePhotoBillingCode = orderItem.sp_SimplePhotoBillingCode, LabCost = orderItem.LabCost , GroupImageId = orderItem.GroupImageId };
                        _objStudentPhotoOrder.StudentImage = new StudentImage();
                        _objStudentPhotoOrder.StudentImage = orderItem.StudentImage;
                        //based on the image name get all the details
                        billingCode = _objStudentPhotoOrder.sp_SimplePhotoBillingCode;
                        _objStudentPhotoOrder.OrderId = tempSelectedOrder.Id;
                        _objStudentPhotoOrder.SchoolId = clsSchool.defaultSchoolId;
                        _objStudentPhotoOrder.GroupImageId = classPhoto;
                        if (billingCode == "F135")
                        {
                            clsOrders.updateOrderRetouch(db, true, tempSelectedOrder.Id);
                        }

                        db.StudentPhotoOrders.InsertOnSubmit(_objStudentPhotoOrder);
                        db.SubmitChanges();

                        if (string.IsNullOrEmpty(billingCode))
                        {
                            int result2 = clsOrders.UpdateOrder(db, tempSelectedOrder.Id, true);
                        }

                        if (_objStudentPhotoOrder.StudentImage == null) { clsOrders.updateHasMissingImages(db, true, tempSelectedOrder.Id); }
                        else
                        {
                            string imgname = _objStudentPhotoOrder.StudentImage.ImageName;
                            int? photoshootid = _objStudentPhotoOrder.StudentImage.PhotoShootID;
                            string imagefolder = clsOrders.getImageFolder(db, photoshootid);
                            if (!File.Exists(imagefolder + "\\" + imgname))
                            {
                                clsOrders.updateHasMissingImages(db, true, tempSelectedOrder.Id);
                            }
                        }
                    }
                    tempSelectedOrder = null;
                }
                catch (Exception ex)
                {
                    isError = true;
                    MVVMMessageService.ShowMessage(ex.Message);
                    clsStatic.WriteExceptionLogXML(ex);
                }
                if (!isError)
                {
                    //Commented by hema we are not using next order ID   ## has to confirm
                    ////update the next order Id
                    //NextOrder _objNextOrder = clsOrders.getNextOrder(db);
                    //orderId = (int)_objNextOrder.NextOrderId;

                    //Clear the fileds
                    dgOrderItemsData = new ObservableCollection<StudentPhotoOrder>();
                    dgStudentPhotosData = new ObservableCollection<StudentImage>(); psStudentDetails = ""; studentDeteilsEnabled = Visibility.Hidden;
                    orderDate = DateTime.Now; amount = null; selectedStudent = new Student();
                    //countTotal();
                }
            }
            else
            {
                MVVMMessageService.ShowMessage("Please select student image first.");
            }
            #endregion
        }
        /// <summary>
        /// this method will create new Order
        /// </summary>
        private void createOrder()
        {
            try
            {
                Order _objOrder = new Order();
                _objOrder.CreatedOn = orderDate;// DateTime.Now;
                _objOrder.Fulfilled = false;
                _objOrder.IsExported = false;
                _objOrder.ImportedBy = clsStatic.userName;
                _objOrder.Retouch = false;
                _objOrder.isSimplePhotoBillingCodeFilled = false;
                _objOrder.OrdersImportId = Convert.ToInt32(selectedExistingBatch["Id"]);
                _objOrder.VendorDate = DateTime.Now;
                _objOrder.SimplePhotoOrderID = null;// Convert.ToInt32(orderTable.Rows[0]["orderID"]);
                _objOrder.ShippingCost = 0;
                _objOrder.PaymentMethod = selectedPaymentMethod.ToString();
                _objOrder.OrderType = "Manual Orders"; //same name used in Ordersviewmodel - line No 619
                _objOrder.OrderTotal = amount;
                _objOrder.Cust_FirstName = selectedStudent.FirstName;
                _objOrder.Cust_LastName = selectedStudent.Lastname;

                _objOrder.Ship_FirstName = shippingFirstName;
                _objOrder.Ship_LastName = shippingLastName;
                _objOrder.Ship_Address = shippingAddress;
                _objOrder.Ship_City = shippingCity;
                _objOrder.Ship_State = shippingState;
                _objOrder.Ship_PostalCode = shippingPostalCode;

                if (simplePhotoOrderNumber > 0)
                    _objOrder.SimplePhotoOrderID = simplePhotoOrderNumber;
                //_objOrder.Ship_Country = selectedStudent.State;

                _objOrder.Variance = differenceInAmount;

                if (_objOrder != null)
                {
                    db.Orders.InsertOnSubmit(_objOrder);
                    db.SubmitChanges();
                }
                tempSelectedOrder = _objOrder;
            }
            //orderId = _objOrder.Id;
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method used to create new importbatch
        /// </summary>
        private void createNewBatch()
        {
            try
            {
                CreateNewBatch _objCreateNewBatch = new CreateNewBatch();
                _objCreateNewBatch.ShowDialog();

                if (((CreateNewBatchViewModel)(_objCreateNewBatch.DataContext)).isSave)
                {
                    OrdersImport tempOrderImport = ((CreateNewBatchViewModel)(_objCreateNewBatch.DataContext))._objOrdersImport;

                    DataRow dRow = cbExistingBatchData.NewRow();
                    DataRowView dRowView = dRow.Table.DefaultView.AddNew();

                    dRow["Id"] = tempOrderImport.Id; dRow["CreatedOn"] = tempOrderImport.CreatedOn; dRow["Description"] = tempOrderImport.Description;
                    dRow["Notes"] = tempOrderImport.Notes; dRow["OrderType"] = tempOrderImport.OrderType;
                    dRow["SchoolID"] = clsSchool.defaultSchoolId;
                    dRow["SchoolName"] = clsSchool.defaultSchoolName;


                    string batchName = ((CreateNewBatchViewModel)(_objCreateNewBatch.DataContext)).newBatchName;
                    cbExistingBatchData.Rows.InsertAt(dRow, 0);
                    selectedExistingBatch = cbExistingBatchData.DefaultView[0];
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method used to close the window
        /// </summary>
        void windowClose()
        {
            DialogResult = false;
        }
        /// <summary>
        /// this method will delete the OrderItems from Order details grid
        /// </summary>
        private void deleteOrderItem()
        {
            if (selectedOrderItem != null)
            {
                dgOrderItemsData.Remove(selectedOrderItem);
            }
        }
        private void studentPhotosMouseup()
        {
            selectImage();
        }

        private void OrderItemsMouseUp()
        {
            if (selectedOrderItem != null)
            {
                //Quantity
                if (selectedOrderItem.Quantity == null)
                {
                    selectedOrderItem.Quantity = 1;
                }
                //Default Price
                //if (selectedOrderItem.LabCost == null)
                if (!string.IsNullOrEmpty(selectedOrderItem.sp_SimplePhotoBillingCode) && selectedOrderItem.sp_SimplePhotoBillingCode != tempbillingCode)
                {
                    tempbillingCode = selectedOrderItem.sp_SimplePhotoBillingCode;
                    OrderPackage tempPackage = clsOrders.getOrderPackagebyBillingCode(db, selectedOrderItem.sp_SimplePhotoBillingCode);
                    selectedOrderItem.LabCost = tempPackage.DefaultPrice;
                }
                //Student Image
                if (selectedOrderItem.StudentImage == null)
                {
                    if (selectedStudentPhoto != null && selectedOrderItem.Quantity != null)
                    {
                        if (dgOrderItemsData.Count == 0) { return; }
                        //Adding student image information to SPO
                        tempStudentImage = clsDashBoard.getStudentImageDetailsById(db, selectedStudentPhoto.ID);
                        dgOrderItemsData[dgOrderItemsData.Count - 1].StudentImage = tempStudentImage;
                    }
                }
                //show image preview
                selectImageForOrderItems();
            }
        }

        # region Select Image
        /// <summary>
        /// Just to show the selected student image on imgStudentPhotoPreview grid
        /// </summary>
        private void selectImage()
        {
            try
            {
                if (selectedStudentPhoto != null)
                {
                    int? ImageId = selectedStudentPhoto.ID;
                    int? groupImageId = selectedStudentPhoto.GroupClassPhotos.Count != 0 ? selectedStudentPhoto.GroupClassPhotos[0].studentImageId : null;
                    //groupImageId = selectedStudentPhoto.GroupImageId;

                    if (ImageId == null)
                    {
                        studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        showImagePreview("Preview", ImageId);
                    }
                    if (groupImageId == null)
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

        private void selectImageForOrderItems()
        {
            try
            {
                if (selectedOrderItem != null)
                {
                    int? ImageId = selectedOrderItem.StudentImageId;
                    int? groupImageId = (selectedOrderItem.GroupImageId != 0 || selectedOrderItem.GroupImageId != null) ? selectedOrderItem.GroupImageId : null;

                    if (ImageId == null)
                    {
                        studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        showImagePreview("Preview", ImageId);
                    }
                    if (groupImageId == null)
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
            StudentImage objStudentImage = clsDashBoard.getStudentImageDetailsById(db, Convert.ToInt32(tempImageId));
            if (objStudentImage != null)
            {
                Photoshootpath = objStudentImage.PhotoShoot.ImageFolder;
                string strFile = objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + objStudentImage.ImageName;
                imagePathToShow = objStudentImage.PhotoShoot.ImageFolder + "\\" + objStudentImage.ImageName;
                if (!File.Exists(strFile))
                    studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
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
        #endregion

    }
}