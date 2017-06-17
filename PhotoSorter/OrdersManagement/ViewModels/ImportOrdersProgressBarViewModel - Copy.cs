using Excel;
using Ookii.Dialogs.Wpf;
using PhotoForce.App_Code;
using PhotoForce.Extensions;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Xml;

namespace PhotoForce.OrdersManagement
{
    public class ImportOrdersProgressBarViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        string ftpUri = "ftp://ftp.freedphoto.com/web_users/spupload";
        string ftpUserName = "simplephoto";
        string ftpPassword = "ociK3#37";
        internal bool xmlFileCount = false;
        List<DirectoryItem> listing;
        internal List<int> lstMissedBillingCode;
        List<string> fileNames = new List<string>();
        string[] lstXMLFiles = null;
        //bool isFromDownload = false;
        List<DirectoryItem> returnValue = new List<DirectoryItem>();
        string localOrdersPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Orders";
        int? simplePhotoOrderId = 0;
        bool isProgressCancelled = false;
        string importFromLocalPath = "";
        bool isFromLocalDisk = false;
        DataTable dataTableFromExcel = new DataTable();
        string alreadyExistedOrders = "";
        bool isOrdersAlreadyExist = false;
        int xmlFilesCount = 0;
        int? orderImportBatchId = 0;
        string failedOrderIds = "";
        string localDiskPath = "";
        WaitCursorViewModel _objWaitCursorViewModel;
        #endregion

        #region Properties
        bool isError = false;
        bool isFTPConnected = false;
        bool _isInProgress;
        Visibility _progressVisibility;
        int _maxValue;
        int _minValue;
        int _currentProgress;
        Visibility _processVisibility;
        string _fileName;
        string _statusLabel;

        public string statusLabel
        {
            get { return _statusLabel; }
            set { _statusLabel = value; NotifyPropertyChanged("statusLabel"); }
        }

        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; NotifyPropertyChanged("fileName"); }
        }

        public Visibility processVisibility
        {
            get { return _processVisibility; }
            set { _processVisibility = value; NotifyPropertyChanged("processVisibility"); }
        }
        public System.Windows.Visibility progressVisibility
        {
            get { return _progressVisibility; }
            set { _progressVisibility = value; NotifyPropertyChanged("progressVisibility"); }
        }
        public bool isInProgress
        {
            get { return _isInProgress; }
            set { _isInProgress = value; NotifyPropertyChanged("isInProgress"); }
        }
        public int maxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; NotifyPropertyChanged("maxValue"); }
        }
        public int minValue
        {
            get { return _minValue; }
            set { _minValue = value; NotifyPropertyChanged("minValue"); }
        }
        public int currentProgress
        {
            get { return _currentProgress; }
            set { _currentProgress = value; NotifyPropertyChanged("currentProgress"); }
        }
        #endregion

        #region Constructor
        public ImportOrdersProgressBarViewModel(string folder, bool isFromExcel, DataTable excelData)
        {
            importFromLocalPath = folder;
            dataTableFromExcel = excelData;
            //isFromDownload = tempFromDownload;
            progressVisibility = Visibility.Collapsed;
            maxValue = 20; statusLabel = "Processing...";
            processOrdersItems(folder, isFromExcel);
        }
        #endregion

        #region Commands
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        #endregion

        #region importOrders
        /// <summary>
        /// This starts the counter as a background process.
        /// </summary>
        async void processOrdersItems(string folder, bool isFromExcel)
        {
            ///worker = new BackgroundWorker();
            progressVisibility = System.Windows.Visibility.Visible;
            currentProgress = 0; minValue = 0; currentProgress = 0;
            listing = new List<DirectoryItem>();

            ///worker.DoWork += delegate(object s, DoWorkEventArgs args)
            ///{
            if (!string.IsNullOrEmpty(folder) && !isFromExcel)
            {
                isFromLocalDisk = true;
                localDiskPath = folder;
                listing = getDirectoryInformation(folder);

                // The await operator suspends processOrdersItems.
                //  - processOrdersItems can't continue until importOrders2 is complete.
                //  - Meanwhile, control returns to the caller of processOrdersItems.
                //  - Control resumes here when importOrders2 is complete. 
                //  - The await operator then retrieves the result from importOrders2 if method has any return type.
                if (listing.Count > 0)
                    await importOrders2();
                else
                    await closeWindow();
            }
            else if (!string.IsNullOrEmpty(folder) && isFromExcel)
            {
                if (dataTableFromExcel == null) { return; }
                else
                {
                    // The await operator suspends processOrdersItems.
                    //  - processOrdersItems can't continue until importOrders3 is complete.
                    //  - Meanwhile, control returns to the caller of processOrdersItems.
                    //  - Control resumes here when importOrders3 is complete. 
                    //  - The await operator then retrieves the result from importOrders3 if method has any return type.
                    await importOrders3();
                }
            }
            else
            {
                listing = getDirectoryInformation(ftpUri, ftpUserName, ftpPassword);
                // The await operator suspends processOrdersItems.
                //  - processOrdersItems can't continue until importOrders2 is complete.
                //  - Meanwhile, control returns to the caller of processOrdersItems.
                //  - Control resumes here when importOrders2 is complete. 
                //  - The await operator then retrieves the result from importOrders2 if method has any return type.
                await importOrders2();
            }
            ///};

            progressVisibility = System.Windows.Visibility.Visible;
            //our background worker support cancellation
            ///worker.WorkerSupportsCancellation = true;

            // Configure the function to run when completed
            ///worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            // Launch the worker
            ///worker.RunWorkerAsync();

            importOrdersAsyncCompleted();
        }
        #endregion

        /// <summary>
        /// This worker_RunWorkerCompleted is called when the worker is finished.
        /// </summary>
        /// <param name="sender">The worker as Object, but it can be cast to a worker.</param>
        /// <param name="e">The RunWorkerCompletedEventArgs object.</param>
        void importOrdersAsyncCompleted()
        {
            _objWaitCursorViewModel = new WaitCursorViewModel();
            _objWaitCursorViewModel.Dispose();
            DialogResult = false; processVisibility = Visibility.Collapsed;
            progressVisibility = Visibility.Collapsed;
            //isInProgress = true; 
        }
        async Task closeWindow()
        {
            await Task.Run(() =>
                {                    
                    MVVMMessageService.ShowMessage("There are no orders to process.");                
                });
        }
        //used to import from XML files i.e, FTP and from local folder
        /// <summary>
        /// The method has an async modifier. 
        /// The return type is Task or Task T. 
        /// Here, it is Task T because the return statement returns an integer.
        /// The method name ends in "Async."
        /// 
        /// You can avoid performance bottlenecks and enhance the overall responsiveness of your application by using asynchronous programming.
        /// However, traditional techniques for writing asynchronous applications can be complicated, making them difficult to write, debug, and maintain.
        /// This async method will execute ,if you want to return anything to called method
        /// declare a return type 
        ///
        /// This method will ImportOrders from LocalFolder or FTP server (of XML type)
        /// </summary>
        /// <returns></returns>
        async Task importOrders2()
        {

            if (listing.Count == 0)
            {
                MVVMMessageService.ShowMessage("There are no orders to process.");
                //args.Cancel = true;
                xmlFileCount = false;
                return;
            }

            if (isFTPConnected)
            {
                orderImportBatchId = createOrdersImportBatch("XML - from ftp server.");
            }
            else if (isFromLocalDisk)
            {
                orderImportBatchId = createOrdersImportBatch("XML - from local drive.");
            }

            maxValue = listing.Count + 1;
            statusLabel = "Processing...";
            currentProgress++;
            _objWaitCursorViewModel = new WaitCursorViewModel();
            lstMissedBillingCode = new List<int>();
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            int processedFileCount = 1;
            xmlFilesCount = listing.Count;

            bool isM134Exists = false;
            bool isM134SubExists = false;
            bool isNewOrder = false;

            //maxValue = listing.Count;

            //have to loop through avilable xml files
            //after completing the process, move file in remote location from folder "spupload" to "processed" 
            #region foreach through list of xml files
            await Task.Run(() =>
            {
                foreach (DirectoryItem lst in listing)
                {
                    if (isProgressCancelled)
                    {
                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                        string message = "Are you sure, you want to cancel the operation?";
                        if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                        {
                            //args.Cancel = true;
                            DialogResult = false;
                            return;
                        }
                        isProgressCancelled = false;
                    }

                    processVisibility = Visibility.Visible;
                    fileName = lst.Name + " (" + processedFileCount + " of " + listing.Count + " )";

                    processedFileCount++;
                    currentProgress++;
                    xmlFileCount = true;
                    try
                    {
                        string xmlData = "";
                        //if (isFromDownload)
                        xmlData = readXMLData(lst.BaseUri.ToString(), lst.Name);
                        //else
                        //xmlData = readXMLDataFromFTP(lst.BaseUri.ToString(), lst.Name);

                        if (!string.IsNullOrEmpty(xmlData))
                        {
                            int tempOrderId = 0; string tempLog = "";
                            DataSet ds = new DataSet();
                            StringReader reader = new StringReader(xmlData);

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(xmlData);
                            ds.ReadXml(reader);

                            DataTable orderTable; DataTable orderDetail; XmlNodeList galleryInfoList; DataTable shippingInfo;
                            int orderId = 0; string importFrom = "";
                            if (doc.DocumentElement.Name == "order")
                            {
                                importFrom = "xml";
                                orderTable = ds.Tables["order"];
                                orderDetail = ds.Tables["orderDetail"];
                                shippingInfo = ds.Tables["shippingInfo"];

                                galleryInfoList = doc.SelectNodes("/order/galleryInfo");

                                orderId = Convert.ToInt32(orderTable.Rows[0]["orderID"]);
                            }
                            else
                            {
                                importFrom = "imageQuix";
                                orderTable = ds.Tables["orderInfo"];
                                orderDetail = ds.Tables["orderInfo"];

                                shippingInfo = ds.Tables["shipping"];

                                galleryInfoList = doc.SelectNodes("orders/order/serviceOrders/serviceOrder/items");//doc.SelectNodes("/order/galleryInfo");
                                orderId = Convert.ToInt32(orderTable.Rows[0]["id"]);
                            }

                            #region Old code

                            //DataTable imgNodeTable = ds.Tables["imgnode"];
                            //DataTable galleryInfoTable = ds.Tables["galleryInfo"];
                            DataTable customerInfo = ds.Tables["customerInfo"];
                            #endregion
                            
                            //Create Order record                                 
                            bool OrderExist = clsOrders.getOrderDetails(db, orderId);

                            if (!OrderExist)
                            {
                                using (TransactionScope ts = new TransactionScope())
                                {
                                    using (PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString))
                                    {
                                        #region Condition A
                                        #region Create Order
                                        isNewOrder = true; isM134Exists = false; isM134SubExists = false;
                                        //Commented by hema 
                                        // bcz we are not using "FP-OrderId"

                                        //update the next order Id
                                        //NextOrder _objNextOrder = clsOrders.getNextOrder(dbb);
                                        //orderId = (int)_objNextOrder.NextOrderId;

                                        //check here wether priceListName in 
                                        Order _objOrder = new Order();
                                        _objOrder.CreatedOn = DateTime.Now;
                                        _objOrder.Fulfilled = false;
                                        _objOrder.IsExported = false;
                                        _objOrder.ImportedBy = clsStatic.userName;
                                        _objOrder.Retouch = false;
                                        _objOrder.IsStandardRetouch = false;
                                        _objOrder.isSimplePhotoBillingCodeFilled = false;
                                        _objOrder.OrdersImportId = orderImportBatchId == 0 ? null : orderImportBatchId;
                                        if (importFrom == "imageQuix")
                                        {
                                            //_objOrder.Studio = orderTable.Rows[0]["photographerID"].ToString();
                                            _objOrder.VendorDate = orderTable.Rows[0]["date"] == null ? null : (DateTime?)Convert.ToDateTime(orderTable.Rows[0]["date"], new System.Globalization.CultureInfo("en-US"));
                                            _objOrder.OrderTotal = Convert.ToDecimal(orderTable.Rows[0]["total"]);
                                            if (orderDetail == null)
                                                _objOrder.ShippingCost = null;
                                            else
                                                _objOrder.ShippingCost = Convert.ToDecimal(orderDetail.Rows[0]["subtotal"]);

                                            //_objOrder.ShippingCost = Convert.ToDecimal(orderDetail.Rows[0]["subtotal"]);
                                            _objOrder.Cust_FirstName = customerInfo.Rows[0]["name"].ToString();

                                            //Shipping Information
                                            _objOrder.Ship_FirstName = shippingInfo.Rows[0]["name"].ToString();
                                            //_objOrder.Ship_LastName = shippingInfo.Rows[0]["LastName"].ToString();
                                            _objOrder.Ship_Address = shippingInfo.Rows[0]["address"].ToString();
                                            _objOrder.Ship_City = shippingInfo.Rows[0]["city"].ToString();
                                            _objOrder.Ship_State = shippingInfo.Rows[0]["state"].ToString();
                                            _objOrder.Ship_PostalCode = shippingInfo.Rows[0]["postal"].ToString();
                                            _objOrder.Ship_Country = shippingInfo.Rows[0]["country"].ToString();
                                            //_objOrder.Ship_UseBillToShip = shippingInfo.Rows[0]["useBilltoShip"].ToString();
                                            //_objOrder.Ship_Email = shippingInfo.Rows[0]["email"].ToString();
                                            _objOrder.OrderType = "Image Quix";

                                        }
                                        else
                                        {
                                            _objOrder.Studio = orderTable.Rows[0]["photographerID"].ToString();
                                            _objOrder.VendorDate = orderTable.Rows[0]["creationDate"] == null ? null : (DateTime?)Convert.ToDateTime(orderTable.Rows[0]["creationDate"], new System.Globalization.CultureInfo("en-US"));
                                            _objOrder.OrderTotal = Convert.ToDecimal(orderTable.Rows[0]["orderTotal"]);
                                            if (orderDetail == null)
                                            {
                                                _objOrder.ShippingCost = null;
                                                _objOrder.Title = null;
                                                _objOrder.Description = null;
                                                _objOrder.ProductCode = null;
                                            }
                                            else
                                            {
                                                _objOrder.ShippingCost = Convert.ToDecimal(orderDetail.Rows[0]["amount"]);
                                                _objOrder.Title = orderDetail.Rows[0]["title"].ToString();
                                                _objOrder.Description = orderDetail.Rows[0]["description"].ToString();
                                                _objOrder.ProductCode = orderDetail.Rows[0]["productCode"].ToString();
                                            }
                                            _objOrder.Cust_FirstName = customerInfo.Rows[0]["FirstName"].ToString();
                                            _objOrder.Cust_LastName = customerInfo.Rows[0]["LastName"].ToString();
                                            _objOrder.Cust_Address = customerInfo.Rows[0]["Address"].ToString();
                                            _objOrder.Cust_City = customerInfo.Rows[0]["City"].ToString();
                                            _objOrder.Cust_State = customerInfo.Rows[0]["State"].ToString();
                                            _objOrder.Cust_PostalCode = customerInfo.Rows[0]["PostalCode"].ToString();
                                            _objOrder.Cust_Country = customerInfo.Rows[0]["Country"].ToString();

                                            //Shipping Information
                                            _objOrder.Ship_FirstName = shippingInfo.Rows[0]["FirstName"].ToString();
                                            _objOrder.Ship_LastName = shippingInfo.Rows[0]["LastName"].ToString();
                                            _objOrder.Ship_Address = shippingInfo.Rows[0]["Address"].ToString();
                                            _objOrder.Ship_City = shippingInfo.Rows[0]["City"].ToString();
                                            _objOrder.Ship_State = shippingInfo.Rows[0]["State"].ToString();
                                            _objOrder.Ship_PostalCode = shippingInfo.Rows[0]["PostalCode"].ToString();
                                            _objOrder.Ship_Country = shippingInfo.Rows[0]["Country"].ToString();
                                            _objOrder.Ship_UseBillToShip = shippingInfo.Rows[0]["useBilltoShip"].ToString();
                                            _objOrder.OrderType = "Simple Photo Order";

                                        }
                                        //_objOrder.VendorOrderNo = orderTable.Rows[0]["orderID"].ToString();  
                                        // Vendor order id is differernt from simple photo id. once we place the orders to Vendor(say Millers) they will give you an unique id  for that order,we have to put that id
                                        //in this filed .similar to flipkart ,once you place the order they will give you order id which is unique so that you can track the order.
                                        _objOrder.SimplePhotoOrderID = orderId;
                                        simplePhotoOrderId = _objOrder.SimplePhotoOrderID;

                                        //commented by Mohan ,we dont have equal data in ImageQuix XML file
                                        //_objOrder.Title = orderDetail.Rows[0]["title"].ToString();
                                        //_objOrder.Description = orderDetail.Rows[0]["description"].ToString();
                                        //_objOrder.ProductCode = orderDetail.Rows[0]["productCode"].ToString();
                                        _objOrder.XMLOrder = xmlData;

                                        //Customer Information

                                        //commented by Mohan, we dont hv this col in ImageQuix
                                        //_objOrder.Cust_LastName = customerInfo.Rows[0]["LastName"].ToString(); 
                                        //_objOrder.Cust_Address = customerInfo.Rows[0]["Address"].ToString();
                                        //_objOrder.Cust_City = customerInfo.Rows[0]["City"].ToString();
                                        //_objOrder.Cust_State = customerInfo.Rows[0]["State"].ToString();
                                        //_objOrder.Cust_PostalCode = customerInfo.Rows[0]["PostalCode"].ToString();
                                        //_objOrder.Cust_Country = customerInfo.Rows[0]["Country"].ToString();
                                        _objOrder.Cust_Email = customerInfo.Rows[0]["email"].ToString();
                                        _objOrder.Cust_Phone = customerInfo.Rows[0]["phone"].ToString();


                                        _objOrder.Ship_Phone = shippingInfo.Rows[0]["phone"].ToString();

                                        dbb.Orders.InsertOnSubmit(_objOrder);
                                        dbb.SubmitChanges();

                                        tempOrderId = _objOrder.Id;

                                        //StudentPhotoOrder _objStuPhotoOrder = new StudentPhotoOrder();
                                        //_objStuPhotoOrder.OrderId = tempOrderId;
                                        //_objStuPhotoOrder.SchoolId = clsSchool.defaultSchoolId;

                                        //db.StudentPhotoOrders.InsertOnSubmit(_objStuPhotoOrder);
                                        //db.SubmitChanges();
                                        #endregion
                                        #endregion  // Condition A

                                        #region Condition B
                                        #region Loop Through Gallery Info
                                        //for (int i = 0; i < galleryInfoTable.Rows.Count; i++)
                                        //{

                                        //    if (isProgressCancelled)
                                        //    {
                                        //        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                        //        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                        //        string message = "Are you sure, you want to cancel the operation?";
                                        //        if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                        //        {
                                        //            args.Cancel = true;
                                        //            DialogResult = false;
                                        //            return;
                                        //        }
                                        //        isProgressCancelled = false;
                                        //    }

                                        //    //get the gallery details
                                        //    DataRow gallery = galleryInfoTable.Rows[i];
                                        //    List<DataRow> entryDetailsByGalleryId = ds.Tables["Entry"].Rows
                                        //              .Cast<DataRow>()
                                        //              .Where(x => Convert.ToInt32(x["galleryInfo_Id"]) == i).ToList();

                                        //    //loop through all the entries of a gallery
                                        //    //each entry will have one imagenode.
                                        //    //maxValue = entryDetailsByGalleryId.Count;
                                        //    foreach (DataRow dr in entryDetailsByGalleryId)
                                        //    {
                                        //        int? classPhoto = null;
                                        //        //based on the image name get all the details
                                        //        string imageName = ds.Tables["imgnode"].Rows[Convert.ToInt32(dr["Entry_Id"])]["img"].ToString();

                                        //        //string comment = (from m in ds.Tables["Comments"].AsEnumerable() where m.Field<int>("Entry_Id") == Convert.ToInt32(dr["Entry_Id"]) select m["Comments_Text"].ToString()).FirstOrDefault();

                                        //        StudentImage imageDetails = clsStudent.getStudentSeasonImageByName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), imageName);
                                        //        if (imageDetails != null)
                                        //        {
                                        //            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                        //            //have to set the group photo only for specific billing codes
                                        //            if (dr["billingCode"].ToString() == "M127" || dr["billingCode"].ToString() == "M128" ||
                                        //                dr["billingCode"].ToString() == "M129" || dr["billingCode"].ToString() == "M130" ||
                                        //                dr["billingCode"].ToString() == "M131" || dr["billingCode"].ToString() == "M132" || dr["billingCode"].ToString() == "M133")
                                        //            {
                                        //                classPhoto = clsOrders.getGroupPhotoByImageId(db, imageDetails.ID);
                                        //            }

                                        //            StudentPhotoOrder _objStudentPhotoOrder = new StudentPhotoOrder();
                                        //            _objStudentPhotoOrder.StudentImageId = imageDetails.ID;
                                        //            _objStudentPhotoOrder.OrderId = tempOrderId;
                                        //            _objStudentPhotoOrder.SchoolId = (int)imageDetails.SchoolID;
                                        //            _objStudentPhotoOrder.GroupImageId = classPhoto;
                                        //            _objStudentPhotoOrder.sp_SimplePhotoBillingCode = (string)dr["billingCode"];
                                        //            _objStudentPhotoOrder.sp_ItemDescription = (string)dr["itemLabel"];
                                        //            _objStudentPhotoOrder.sp_GroupName = (string)gallery["groupName"];
                                        //            _objStudentPhotoOrder.LabCost = Convert.ToDecimal(dr["labCost"].ToString() == "" ? 0 : dr["labCost"]);
                                        //            _objStudentPhotoOrder.sp_Name = (string)gallery["name"];
                                        //            _objStudentPhotoOrder.sp_Password = (string)gallery["password"];
                                        //            _objStudentPhotoOrder.sp_PriceListName = (string)gallery["priceListName"];
                                        //            _objStudentPhotoOrder.VenueName = (string)gallery["venueName"];
                                        //            _objStudentPhotoOrder.sp_ProductType = (string)dr["productType"];
                                        //            _objStudentPhotoOrder.sp_JobNumber = (string)gallery["jobNumber"];
                                        //            _objStudentPhotoOrder.Quantity = Convert.ToInt32(dr["qty"]);
                                        //            //_objStudentPhotoOrder.Comments = comment;
                                        //            db.StudentPhotoOrders.InsertOnSubmit(_objStudentPhotoOrder);
                                        //            db.SubmitChanges();

                                        //            if (string.IsNullOrEmpty(dr["billingCode"].ToString()))
                                        //            {
                                        //                lstMissedBillingCode.Add(_objStudentPhotoOrder);
                                        //                //int res = clsDashBoard.updatePackages(db, "", imageDetails.ID); //clearing the package details of student image .
                                        //                //_objOrder.isSimplePhotoBillingCodeFilled = true;
                                        //                ////db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                        //                //db.SubmitChanges();
                                        //                int result2 = clsOrders.UpdateOrder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), _objOrder.Id, true);
                                        //            }
                                        //            //else
                                        //            //{
                                        //            //    //update packages of student image
                                        //            //    #region update student package
                                        //            //    int qty = Convert.ToInt32(dr["qty"]);
                                        //            //    string packageItem = clsOrders.getPackageItemById(db, dr["billingCode"].ToString()).Trim();
                                        //            //    //updatePackages
                                        //            //    int res = clsDashBoard.updatePackages(db, packageItem + "-" + qty, imageDetails.ID);
                                        //            //    #endregion
                                        //            //}
                                        //        }
                                        //        else
                                        //        {
                                        //            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                        //            //get the order tempOrderId
                                        //            Order tempOrderDetail = clsOrders.getOrderById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempOrderId);
                                        //            tempLog = tempLog + "" + imageName + " Details not found.\t";
                                        //            //save image details in log file
                                        //            isError = true;
                                        //        }
                                        //    }
                                        //}
                                        ////update log for particluar order
                                        //int result = clsOrders.UpdateOrderLog(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempLog, tempOrderId);
                                        #endregion

                                        #region New Code (Loop Through Gallery Info)

                                        for (int i = 0; i < galleryInfoList.Count; i++)
                                        {    
                                            if (isProgressCancelled)
                                            {
                                                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                                string message = "Are you sure, you want to cancel the operation?";
                                                if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                                {
                                                    //args.Cancel = true;
                                                    // isProgressCancelled = true;
                                                    DialogResult = false;
                                                    //_objWaitCursorViewModel.Dispose();
                                                    return;
                                                }
                                                isProgressCancelled = false;
                                            }
                                            //get the gallery details
                                            XmlNode gallery = galleryInfoList[i];
                                            XmlNodeList entryList;
                                            if (importFrom == "imageQuix")
                                                entryList = gallery.SelectNodes("item");
                                            else
                                                entryList = gallery.SelectNodes("Entry");

                                            //loop through all the entries of a gallery
                                            //each entry will have one imagenode.
                                            foreach (XmlNode entry in entryList)
                                            {
                                                int? classPhoto = null;
                                                string imageName; string billingCode = "";
                                                //based on the image name get all the details
                                                if (importFrom == "imageQuix")
                                                {
                                                    imageName = entry["nodes"]["image"]["name"].InnerText; //entry.ChildNodes[11].ChildNodes[2].FirstChild.InnerText;
                                                    billingCode = entry["sku"].InnerText;// entry.Attributes["billingCode"].Value;
                                                }
                                                else
                                                {
                                                    imageName = entry.ChildNodes[0].Attributes["img"].Value;
                                                    billingCode = entry.Attributes["billingCode"].Value;
                                                }
                                                StudentImage imageDetails = clsStudent.getStudentSeasonImageByName(dbb, imageName);
                                                StudentPhotoOrder _objStudentPhotoOrder = new StudentPhotoOrder();
                                                // db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                                if (imageDetails == null)
                                                {
                                                    //get the order tempOrderId
                                                    Order tempOrderDetail = clsOrders.getOrderById(dbb, tempOrderId);
                                                    tempLog = tempLog + "" + imageName + " Details not found.\t";
                                                    //save image details in log file
                                                    isError = true;
                                                }
                                                else
                                                {
                                                    //have to set the group photo only for specific billing codes
                                                    if (billingCode == "M127" || billingCode == "M128" || billingCode == "M129" ||
                                                        billingCode == "M130" || billingCode == "M131" || billingCode == "M132" || billingCode == "M133" || billingCode == "M136" || billingCode == "M171")
                                                    {
                                                        classPhoto = clsOrders.getGroupPhotoByImageId(dbb, imageDetails.ID);
                                                    }

                                                    _objStudentPhotoOrder.StudentImageId = imageDetails.ID;
                                                    _objStudentPhotoOrder.SchoolId = (int)imageDetails.SchoolID;
                                                }
                                                _objStudentPhotoOrder.OrderId = tempOrderId;
                                                _objStudentPhotoOrder.GroupImageId = classPhoto;
                                                _objStudentPhotoOrder.sp_SimplePhotoBillingCode = billingCode;
                                                if (importFrom == "imageQuix")
                                                {
                                                    _objStudentPhotoOrder.sp_ItemDescription = entry["name"].InnerText;//entry.ChildNodes[3].InnerText;
                                                    _objStudentPhotoOrder.sp_ProductType = entry["type"].InnerText;//entry.ChildNodes[2].InnerText;
                                                    _objStudentPhotoOrder.Quantity = entry["quantity"].InnerText == null ? 0 : Convert.ToInt32(entry["quantity"].InnerText);

                                                    //_objStudentPhotoOrder.crop_coordinates = (entry.ChildNodes[11].ChildNodes[3].ChildNodes[0].FirstChild.InnerText + "," + entry.ChildNodes[11].ChildNodes[3].ChildNodes[1].FirstChild.InnerText);
                                                    //_objStudentPhotoOrder.crop_dimensions = (entry.ChildNodes[11].ChildNodes[3].ChildNodes[2].FirstChild.InnerText + "," + entry.ChildNodes[11].ChildNodes[3].ChildNodes[3].FirstChild.InnerText);
                                                    //_objStudentPhotoOrder.crop_orientation = entry.ChildNodes[11].ChildNodes[3].ChildNodes[4].FirstChild.InnerText;

                                                    _objStudentPhotoOrder.crop_coordinates = (entry["nodes"]["crop"]["x"].InnerText + "," + entry["nodes"]["crop"]["y"].InnerText);
                                                    _objStudentPhotoOrder.crop_dimensions = (entry["nodes"]["crop"]["width"].InnerText + "," + entry["nodes"]["crop"]["height"].InnerText);
                                                    _objStudentPhotoOrder.crop_orientation = entry["nodes"]["crop"]["orientation"].InnerText;

                                                    _objStudentPhotoOrder.sp_GroupName = entry["galleryID"].InnerText;//entry.ChildNodes[1].FirstChild.InnerText;
                                                }
                                                else
                                                {
                                                    _objStudentPhotoOrder.sp_ItemDescription = entry.Attributes["itemLabel"].Value;
                                                    _objStudentPhotoOrder.sp_GroupName = gallery.Attributes["groupName"].Value;
                                                    _objStudentPhotoOrder.LabCost = Decimal.Parse(entry.Attributes["labCost"].Value == "" ? "0" : entry.Attributes["labCost"].Value);
                                                    _objStudentPhotoOrder.sp_Name = gallery.Attributes["name"].Value;
                                                    _objStudentPhotoOrder.sp_Password = gallery.Attributes["password"].Value;
                                                    _objStudentPhotoOrder.sp_PriceListName = gallery.Attributes["priceListName"].Value;
                                                    _objStudentPhotoOrder.VenueName = gallery.Attributes["venueName"].Value;
                                                    _objStudentPhotoOrder.sp_ProductType = entry.Attributes["productType"].Value;
                                                    _objStudentPhotoOrder.sp_JobNumber = gallery.Attributes["jobNumber"].Value;
                                                    _objStudentPhotoOrder.Quantity = Convert.ToInt32(entry.Attributes["qty"].Value);
                                                }
                                                if (billingCode == "F135")
                                                {
                                                    clsOrders.updateOrderRetouch(dbb, true, _objOrder.Id);
                                                }
                                                if (billingCode == "M134")
                                                {
                                                    isM134Exists = true;
                                                }
                                                if (billingCode == "M101" || billingCode == "M102" || billingCode == "M127" || billingCode == "M128" || billingCode == "M138" || billingCode == "M139")
                                                {
                                                    isM134SubExists = true;
                                                }
                                                if (entry.NextSibling != null)
                                                {
                                                    XmlNode textEntry = entry.NextSibling.NextSibling;
                                                    if (textEntry != null) { _objStudentPhotoOrder.Comments = textEntry.FirstChild != null ? textEntry.FirstChild.InnerText : ""; }
                                                }
                                                dbb.StudentPhotoOrders.InsertOnSubmit(_objStudentPhotoOrder);
                                                dbb.SubmitChanges();

                                                if (string.IsNullOrEmpty(billingCode))
                                                {
                                                    lstMissedBillingCode.Add(_objStudentPhotoOrder.Id);
                                                    int result2 = clsOrders.UpdateOrder(dbb, _objOrder.Id, true);
                                                }
                                                if (_objStudentPhotoOrder.StudentImage == null) { clsOrders.updateHasMissingImages(dbb, true, _objOrder.Id); }
                                                else
                                                {
                                                    string imgname = _objStudentPhotoOrder.StudentImage.ImageName;
                                                    int? photoshootid = _objStudentPhotoOrder.StudentImage.PhotoShootID;
                                                    string imagefolder = clsOrders.getImageFolder(dbb, photoshootid);
                                                    if (!File.Exists(imagefolder + "\\" + imgname))
                                                    {
                                                        clsOrders.updateHasMissingImages(dbb, true, _objOrder.Id);
                                                    }
                                                }
                                            }
                                        }
                                        //update log for particluar order
                                        int result = clsOrders.UpdateOrderLog(dbb, tempLog, tempOrderId);
                                        //updateIsStandardRetouch
                                        if (isNewOrder && isM134Exists && isM134SubExists)
                                        {
                                            //impl....
                                            clsOrders.updateOrderStandardRetouch(dbb, true, _objOrder.Id);
                                            isNewOrder = false;
                                        }
                                        //update HasNotes
                                        if (tempOrderId != 0)
                                        {
                                            bool hasNotes = clsOrders.updateOrderHasNotes(dbb, tempOrderId);
                                        }
                                        #endregion

                                        #endregion // Condition B
                                    }
                                    ts.Complete();
                                }

                                if (!isFromLocalDisk)
                                {
                                    moveFile(ftpUserName, ftpPassword, ftpUri, ftpUri + "/processed", lst.Name);
                                }
                                //moveFile(ftpUserName, ftpPassword, lst.BaseUri.ToString(), lst.BaseUri.ToString() + "/processed", lst.Name);
                            }
                            else
                            {
                                isOrdersAlreadyExist = true;
                                alreadyExistedOrders = alreadyExistedOrders + Convert.ToInt32(orderTable.Rows[0]["orderID"]) + ", ";
                            }
                        }
                        else
                        {
                            if (xmlFilesCount != 0) { xmlFilesCount--; }
                            if (!isFromLocalDisk)
                            {
                                moveFile(ftpUserName, ftpPassword, ftpUri, ftpUri + "/failedimport", lst.Name);
                            }
                            //moveFile(ftpUserName, ftpPassword, lst.BaseUri.ToString(), lst.BaseUri.ToString() + "/failedimport", lst.Name);
                            if (isFromLocalDisk)
                            {
                                moveFileToLocalFolder(localDiskPath, localDiskPath + "/FailedOrders", lst.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isError = true;
                        MVVMMessageService.ShowMessage("Error while processing " + lst.Name + "\n\n" + ex.Message);
                        clsStatic.WriteExceptionLogXML(ex);
                        if (!isFromLocalDisk)
                        {
                            moveFile(ftpUserName, ftpPassword, ftpUri, ftpUri + "/failedimport", lst.Name);
                        }
                        else if (isFromLocalDisk)
                        {
                            moveFileToLocalFolder(localDiskPath, localDiskPath + "/FailedOrders", lst.Name);
                        }
                    }
                }
            });
            #endregion

            isFailedOrders(localDiskPath + "\\FailedOrders");
            isOrdersError();

            #region Delete XML files(i.e, orders) in local directory
            if (!isFromLocalDisk)
            {
                System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(localOrdersPath);

                foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            #endregion

        }

        //used to import from Excel file
        /// <summary>
        /// The method has an async modifier. 
        /// The return type is Task or Task T. 
        /// Here, it is Task T because the return statement returns an integer.
        /// The method name ends in "Async."
        /// 
        /// You can avoid performance bottlenecks and enhance the overall responsiveness of your application by using asynchronous programming.
        /// However, traditional techniques for writing asynchronous applications can be complicated, making them difficult to write, debug, and maintain.
        /// This async method will execute ,if you want to return anything to called method
        /// declare a return type 
        ///
        /// This method will ImportOrders from excel file
        /// </summary>
        /// <returns></returns>
        async Task importOrders3()
        {
            if (dataTableFromExcel.Rows.Count == 0)
            {
                //args.Cancel = true;
                xmlFileCount = false;

                MVVMMessageService.ShowMessage("There are no orders to process.");
                return;
            }
            _objWaitCursorViewModel = new WaitCursorViewModel();
            orderImportBatchId = createOrdersImportBatch("from excel file.");

            maxValue = dataTableFromExcel.Rows.Count + 1;
            statusLabel = "Processing...";
            currentProgress++;

            lstMissedBillingCode = new List<int>();
            string tempLog = "";
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            int processedFileCount = 1;
            int tempOrderId = 0; int previousOrderNumber = 0; xmlFilesCount = 0;

            DataView dv = dataTableFromExcel.DefaultView;
            dv.Sort = "Order Number asc";
            dataTableFromExcel = dv.ToTable();

            bool isNewOrder = false;
            bool isM134Exists = false;
            bool isM134SubExists = false;

            await Task.Run(() =>
            {
                foreach (DataRow dr in dataTableFromExcel.Rows)
                {
                    if (isProgressCancelled)
                    {
                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                        string message = "Are you sure, you want to cancel the operation?";
                        if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                        {
                            //args.Cancel = true;
                            DialogResult = false;
                            return;
                        }
                        isProgressCancelled = false;
                    }

                    processVisibility = Visibility.Visible;

                    processedFileCount++;
                    currentProgress++;
                    xmlFileCount = true;

                    try
                    {
                        if (dr["Image"].ToString() == "Order.jpg") { continue; }
                        bool OrderExist = clsOrders.getOrderDetails(db, Convert.ToInt32(dr["Order Number"]));
                        using (TransactionScope ts = new TransactionScope())
                        {
                            using (PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString))
                            {
                                #region Condition A
                                if (!OrderExist)
                                {
                                    xmlFilesCount++;
                                    isNewOrder = true; isM134SubExists = false; isM134Exists = false;

                                    #region Create Order
                                    //Commented by hema 
                                    // bcz we are not using "FP-OrderId"
                                    //NextOrder _objNextOrder = clsOrders.getNextOrder(dbb);
                                    //orderId = (int)_objNextOrder.NextOrderId;

                                    previousOrderNumber = Convert.ToInt32(dr["Order Number"]);

                                    Order _objOrder = new Order();
                                    _objOrder.CreatedOn = DateTime.Now;
                                    _objOrder.Fulfilled = false;
                                    _objOrder.IsExported = false;
                                    _objOrder.Retouch = false;
                                    _objOrder.IsStandardRetouch = false;
                                    _objOrder.isSimplePhotoBillingCodeFilled = false;
                                    _objOrder.ImportedBy = clsStatic.userName;
                                    _objOrder.OrdersImportId = orderImportBatchId == 0 ? null : orderImportBatchId;
                                    //_objOrder.VendorDate = dr["creationDate"] == null ? null : (DateTime?)Convert.ToDateTime(dr["creationDate"], new System.Globalization.CultureInfo("en-US")); //No-Column in Excel //Mohan
                                    _objOrder.SimplePhotoOrderID = Convert.ToInt32(dr["Order Number"]);
                                    simplePhotoOrderId = _objOrder.SimplePhotoOrderID;
                                    //_objOrder.OrderTotal = Convert.ToDecimal(dr["orderTotal"]);//No-Column in Excel //Mohan
                                    //_objOrder.ShippingCost = Convert.ToDecimal(dr["amount"]);//No-Column in Excel //Mohan
                                    //_objOrder.OrderType = dr["type"].ToString();  //No-Column in Excel //Mohan
                                    //_objOrder.Title = dr["title"].ToString();//No-Column in Excel //Mohan
                                    //_objOrder.Description = dr["description"].ToString();//No-Column in Excel //Mohan
                                    //_objOrder.ProductCode = dr["productCode"].ToString();//No-Column in Excel //Mohan


                                    //Customer Information
                                    _objOrder.Cust_FirstName = dr["First Name"].ToString();
                                    _objOrder.Cust_LastName = dr["Last Name"].ToString();
                                    _objOrder.Cust_Email = dr["Email"].ToString();

                                    //No-Column in Excel //Mohan
                                    //Un Commented by hema for card (https://trello.com/c/xm6bm65H/883-import-orders-from-excel-this-imports-the-item-from-the-billing-code-column-but-when-we-are-entering-manual-orders-into-the-spre)
                                    _objOrder.Cust_Address = dr["Address"].ToString();
                                    _objOrder.Cust_City = dr["City"].ToString();
                                    _objOrder.Cust_State = dr["State"].ToString();
                                    _objOrder.Cust_PostalCode = dr["Zip"].ToString();
                                    _objOrder.Cust_Country = dr["Country"].ToString();  //up to here un commented by hema

                                    //_objOrder.Cust_Phone = dr["phone"].ToString();

                                    //Shipping Information
                                    _objOrder.Ship_FirstName = dr["Ship To First Name"].ToString();
                                    _objOrder.Ship_LastName = dr["Ship To Last Name"].ToString();
                                    _objOrder.Ship_Address = dr["Address"].ToString();
                                    _objOrder.Ship_City = dr["City"].ToString();
                                    _objOrder.Ship_State = dr["State"].ToString();
                                    _objOrder.Ship_PostalCode = dr["Zip"].ToString();
                                    _objOrder.Ship_Country = dr["Country"].ToString();
                                    //_objOrder.Ship_UseBillToShip = dr["useBilltoShip"].ToString();
                                    //_objOrder.Ship_Phone = dr["phone"].ToString();

                                    //photographerID
                                    //_objOrder.Studio = dr["photographerID"].ToString();

                                    dbb.Orders.InsertOnSubmit(_objOrder);
                                    dbb.SubmitChanges();

                                    tempOrderId = _objOrder.Id;

                                    //StudentPhotoOrder _objStuPhotoOrder = new StudentPhotoOrder();
                                    //_objStuPhotoOrder.OrderId = tempOrderId;
                                    //_objStuPhotoOrder.SchoolId = clsSchool.defaultSchoolId;

                                    //db.StudentPhotoOrders.InsertOnSubmit(_objStuPhotoOrder);
                                    //db.SubmitChanges();
                                    #endregion
                                }
                                #endregion  // Condition A

                                #region Condition B
                                //for (int i = 0; i < galleryInfoList.Count; i++)
                                //{
                                if (tempOrderId != 0 && previousOrderNumber == Convert.ToInt32(dr["Order Number"]))
                                {
                                    #region New Code (Loop Through Gallery Info)
                                    if (isProgressCancelled)
                                    {
                                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                        string message = "Are you sure, you want to cancel the operation?";
                                        if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                        {
                                            //args.Cancel = true;
                                            isProgressCancelled = true;
                                            DialogResult = false;
                                            return;
                                        }
                                        isProgressCancelled = false;
                                    }

                                    //get the gallery details

                                    //XmlNode gallery = galleryInfoList[i];

                                    //XmlNodeList entryList = gallery.SelectNodes("Entry");

                                    //loop through all the entries of a gallery

                                    //each entry will have one imagenode.

                                    //foreach (XmlNode entry in entryList)
                                    //{
                                    int? classPhoto = null;
                                    //based on the image name get all the details
                                    string imageName = dr["Image"].ToString();
                                    string billingCode = "";



                                    StudentImage imageDetails = clsStudent.getStudentSeasonImageByName(dbb, imageName);

                                    StudentPhotoOrder _objStudentPhotoOrder = new StudentPhotoOrder();
                                    // db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                    string tempPackage = dr["packages"].ToString();
                                    //string ItemDescription = "";
                                    if (!string.IsNullOrEmpty(tempPackage))
                                    {
                                        db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                        OrderPackage tempBillingCode = (from op in db.OrderPackages where op.Item == tempPackage select op).FirstOrDefault(); 
                                        billingCode = tempBillingCode.SimplePhotoItemId;
                                        //ItemDescription = tempBillingCode.Package;
                                    }
                                    else
                                        billingCode = dr["Billing code"].ToString();
                                    if (imageDetails == null)
                                    {
                                        //get the order tempOrderId
                                        Order tempOrderDetail = clsOrders.getOrderById(dbb, tempOrderId);
                                        tempLog = tempLog + "" + imageName + " Details not found.\t";
                                        //save image details in log file
                                        isError = true;
                                    }
                                    else
                                    {
                                        //have to set the group photo only for specific billing codes
                                        if (billingCode == "M127" || billingCode == "M128" || billingCode == "M129" ||
                                            billingCode == "M130" || billingCode == "M131" || billingCode == "M132" || billingCode == "M133" || billingCode == "M136" || billingCode == "M171")
                                        {
                                            classPhoto = clsOrders.getGroupPhotoByImageId(dbb, imageDetails.ID);
                                        }

                                        _objStudentPhotoOrder.StudentImageId = imageDetails.ID;
                                        _objStudentPhotoOrder.SchoolId = (int)imageDetails.SchoolID;
                                    }
                                    _objStudentPhotoOrder.OrderId = tempOrderId;
                                    _objStudentPhotoOrder.GroupImageId = classPhoto;
                                    _objStudentPhotoOrder.sp_SimplePhotoBillingCode = billingCode;
                                    _objStudentPhotoOrder.sp_ItemDescription = dr["itemLabel"].ToString();
                                    _objStudentPhotoOrder.Quantity = Convert.ToInt32(dr["Quantity"]);

                                    //_objStudentPhotoOrder.sp_GroupName = dr["groupName"].ToString();
                                    //_objStudentPhotoOrder.LabCost = Convert.ToDecimal(dr["labCost"] == "" ? "0" : dr["labCost"]);
                                    //_objStudentPhotoOrder.sp_Name = dr["name"].ToString();
                                    //_objStudentPhotoOrder.sp_Password = dr["password"].ToString();
                                    //_objStudentPhotoOrder.sp_PriceListName = dr["priceListName"].ToString();
                                    //_objStudentPhotoOrder.VenueName = dr["venueName"].ToString();
                                    //_objStudentPhotoOrder.sp_ProductType = dr["productType"].ToString();
                                    //_objStudentPhotoOrder.sp_JobNumber = dr["jobNumber"].ToString();

                                    if (billingCode == "F135")
                                    {
                                        clsOrders.updateOrderRetouch(dbb, true, tempOrderId);
                                    }
                                    if (billingCode == "M134")
                                    {
                                        isM134Exists = true;
                                    }
                                    if (billingCode == "M101" || billingCode == "M102" || billingCode == "M127" || billingCode == "M128" || billingCode == "S138" || billingCode == "S139")
                                    {
                                        isM134SubExists = true;
                                    }
                                    //if (entry.NextSibling != null)
                                    //{
                                    //    XmlNode textEntry = entry.NextSibling.NextSibling;
                                    //    if (textEntry != null) { _objStudentPhotoOrder.Comments = textEntry.FirstChild.InnerText; }

                                    //}

                                    dbb.StudentPhotoOrders.InsertOnSubmit(_objStudentPhotoOrder);
                                    dbb.SubmitChanges();

                                    if (string.IsNullOrEmpty(billingCode))
                                    {
                                        lstMissedBillingCode.Add(_objStudentPhotoOrder.Id);
                                        int result2 = clsOrders.UpdateOrder(dbb, tempOrderId, true);
                                    }

                                    if (_objStudentPhotoOrder.StudentImage == null) { clsOrders.updateHasMissingImages(dbb, true, tempOrderId); }
                                    else
                                    {
                                        string imgname = _objStudentPhotoOrder.StudentImage.ImageName;
                                        int? photoshootid = _objStudentPhotoOrder.StudentImage.PhotoShootID;
                                        string imagefolder = clsOrders.getImageFolder(dbb, photoshootid);
                                        if (!File.Exists(imagefolder + "\\" + imgname))
                                        {
                                            clsOrders.updateHasMissingImages(dbb, true, tempOrderId);
                                        }
                                    }


                                    //}
                                    //}
                                    //update log for particluar order
                                    int result = clsOrders.UpdateOrderLog(dbb, tempLog, tempOrderId);

                                    //updateIsStandardRetouch
                                    if (isNewOrder && isM134Exists && isM134SubExists)
                                    {
                                        //impl....
                                        clsOrders.updateOrderStandardRetouch(dbb, true, tempOrderId);
                                        isNewOrder = false;
                                    }

                                    #endregion
                                }
                                else
                                {
                                    isOrdersAlreadyExist = true;
                                    alreadyExistedOrders = alreadyExistedOrders + Convert.ToInt32(dr["Order Number"]) + ", ";
                                }
                                #endregion // Condition B
                            }
                            ts.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        isError = true;
                        MVVMMessageService.ShowMessage("Error while processing " + dr["Order Number"] + "\n\n" + ex.Message);
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                }
            });
            if (true)
            {
                string message = "Do you want to update 'Has Notes' for all orders.?";
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    int count = 0;
                    List<int> lstOrderIds = (from soi in db.Orders select soi.Id).Distinct().ToList();
                    foreach (int orderid in lstOrderIds)
                    {
                        bool hasNotes = clsOrders.updateOrderHasNotes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), orderid);
                        if (hasNotes)
                        {
                            ++count;
                        }
                    }
                    MVVMMessageService.ShowMessage("Orders 'Has Notes' information updated.\n" + count + " Order(s) with 'Has Notes'.");
                }
            }
            isOrdersError();
        }

        #region Import Orders Related

        public List<DirectoryItem> getDirectoryInformation(string address, string username, string password)
        {
            //SetMethodRequiresCWD();
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(address);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(username, password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            fileNames = new List<string>();
            lstXMLFiles = null;

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        fileNames.Add(reader.ReadLine());
                    }

                    lstXMLFiles = (from sd in fileNames where sd.EndsWith(".xml") || sd.EndsWith(".XML") select sd.Substring(sd.IndexOf('/') + 1)).ToArray();

                    isFTPConnected = true;
                }

                //if (!isFromDownload)
                //{
                //    foreach (string line in lstXMLFiles)
                //    {
                //        // Create directory info
                //        DirectoryItem item = new DirectoryItem();
                //        item.BaseUri = new Uri(address);
                //        item.DateCreated = new DateTime();
                //        item.IsDirectory = false;
                //        item.Name = line;

                //        returnValue.Add(item);
                //    }
                //}
                //else
                //{
                returnValue = downloadFile();
                //}
            }
            catch (Exception ex)
            {
                isFTPConnected = false;
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
            return returnValue;
        }

        public List<DirectoryItem> getDirectoryInformation(string folderPath)
        {
            try
            {
                //string folderPath = "";
                //VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                //var res = dlg.ShowDialog();
                //if (res != false)
                //    folderPath = dlg.SelectedPath;

                if (!string.IsNullOrEmpty(folderPath))
                {
                    lstXMLFiles = Directory.GetFiles(folderPath)
                           .Where(p => p.EndsWith(".xml") || p.EndsWith(".XML"))
                           .ToArray();

                    returnValue = filesAsDirectoryItem(lstXMLFiles);
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
            return returnValue;
        }

        //public List<DirectoryItem> getExcelDirectoryInformation(string folderPath)
        //{
        //    try
        //    {
        //        //string folderPath = "";
        //        //VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
        //        //var res = dlg.ShowDialog();
        //        //if (res != false)
        //        //    folderPath = dlg.SelectedPath;

        //        if (!string.IsNullOrEmpty(folderPath))
        //        {
        //            lstXMLFiles = Directory.GetFiles(folderPath)
        //                   .Where(p => p.ToLower().EndsWith(".excel"))
        //                   .ToArray();

        //            returnValue = filesAsDirectoryItem(lstXMLFiles);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MVVMMessageService.ShowMessage(ex.Message);
        //        clsStatic.WriteExceptionLogXML(ex);
        //    }
        //    return returnValue;
        //}

        public List<DirectoryItem> downloadFile()
        {
            try
            {
                if (!Directory.Exists(localOrdersPath))
                {
                    Directory.CreateDirectory(localOrdersPath);
                }

                statusLabel = "Downloading...";
                maxValue = lstXMLFiles.Count();
                //string appPath = Path.GetDirectoryName(Applicati);
                using (WebClient ftpClient = new WebClient())
                {
                    ftpClient.Credentials = new System.Net.NetworkCredential("simplephoto", "ociK3#37");

                    for (int i = 0; i <= lstXMLFiles.Count() - 1; i++)
                    {
                        currentProgress++;
                        int j = i + 1;
                        fileName = lstXMLFiles[i].ToString() + " (" + j + " of " + lstXMLFiles.Count() + " )";
                        string tempName = "\\" + lstXMLFiles[i].ToString();
                        string path = ftpUri + tempName;
                        string trnsfrpth = localOrdersPath + tempName;
                        ftpClient.DownloadFile(path, trnsfrpth);
                    }
                }
                returnValue = filesAsDirectoryItem(lstXMLFiles);
            }
            catch (WebException ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
            maxValue = 20;
            currentProgress = 0;
            return returnValue;
        }
        List<DirectoryItem> filesAsDirectoryItem(string[] lstXMLFiles)
        {
            List<DirectoryItem> tempFileData = new List<DirectoryItem>();
            foreach (string line in lstXMLFiles)
            {
                // Create directory info
                DirectoryItem item = new DirectoryItem();
                item.BaseUri = isFromLocalDisk == true ? new Uri(importFromLocalPath) : new Uri(localOrdersPath);
                item.DateCreated = new DateTime();
                item.IsDirectory = false;
                item.Name = isFromLocalDisk == true ? line.Substring(line.LastIndexOf('\\') + 1) : line;
                tempFileData.Add(item);
            }
            return tempFileData;
        }

        public string moveFile(string username, string tempPassword, string ftpfrompath, string ftptopath, string filename)
        {
            string retval = string.Empty;

            FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpfrompath + "/" + filename);
            ftp.Method = WebRequestMethods.Ftp.Rename;
            ftp.Credentials = new NetworkCredential(username, tempPassword);
            ftp.UsePassive = true;
            ftp.RenameTo = getRelativePath(ftpfrompath, ftptopath) + filename;
            //Stream requestStream = ftp.GetRequestStream();


            FtpWebResponse ftpresponse = (FtpWebResponse)ftp.GetResponse();

            Stream responseStream = ftpresponse.GetResponseStream();

            StreamReader reader = new StreamReader(responseStream);

            return reader.ReadToEnd();
        }
        /// <summary>
        /// This moveFileToLocalFolder(browsedPath + FailedOrders) is called when error occur while importing XML orders from localdrive.
        /// </summary>
        /// sourceFolder represents the browsedPath
        /// destinationfolder represents the (browsedPath + FailedOrders)
        /// file name represents the failed xml file name
        private void moveFileToLocalFolder(string sourceFolder, string destinationFolder, string fileName)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
                // create group photos here inside each folder
            }

            string sourceFile = System.IO.Path.Combine(sourceFolder, fileName);
            string destFile = System.IO.Path.Combine(destinationFolder, fileName);
            if (!System.IO.File.Exists(destFile))
            {
                System.IO.File.Move(sourceFile, destFile);
                failedOrderIds = failedOrderIds + fileName + ", ";
            }
            else
            {
                System.IO.File.Delete(destFile);
                System.IO.File.Move(sourceFile, destFile);
                failedOrderIds = failedOrderIds + fileName + ", ";
            }

        }

        public string getRelativePath(string ftpBasePath, string ftpToPath)
        {
            string[] arrBasePath = ftpBasePath.Split("/".ToCharArray());
            string[] arrToPath = ftpToPath.Split("/".ToCharArray());

            int basePathCount = arrBasePath.Count();
            int levelChanged = basePathCount;
            for (int iIndex = 0; iIndex < basePathCount; iIndex++)
            {
                if (arrToPath.Count() > iIndex)
                {
                    if (arrBasePath[iIndex] != arrToPath[iIndex])
                    {
                        levelChanged = iIndex;
                        break;
                    }
                }
            }
            int HowManyBack = basePathCount - levelChanged;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < HowManyBack; i++)
            {
                sb.Append("../");
            }
            for (int i = levelChanged; i < arrToPath.Count(); i++)
            {
                sb.Append(arrToPath[i]);
                sb.Append("/");
            }

            return sb.ToString();
        }

        private string readXMLData(string uri, string fileName)
        {
            string xmlString = System.IO.File.ReadAllText(uri.Replace("file:///", "") + "\\" + fileName);

            //readXMLNodeData(uri, fileName);
            xmlString = replaceSpecialCharacters(xmlString, true);

            return xmlString;
        }

        //private void readXMLNodeData(string uri, string fileName)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(uri.Replace("file:///", "") + "\\" + fileName);
        //}

        string replaceSpecialCharacters(string text, bool toDT)
        {
            string res = "";
            if (toDT)
            {
                res = text.Replace("&", "&amp;");
            }
            else
            {
                res = text.Replace("&amp;", "&");
            }
            return res;
        }

        //we are not using this method as we are processing the orders by downloading them
        //private string readXMLDataFromFTP(string uri, string fileName)
        //{
        //    string fileString = "";
        //    WebClient request = new WebClient();
        //    string url = uri + "/" + fileName;
        //    request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

        //    try
        //    {
        //        byte[] newFileData = request.DownloadData(url);
        //        fileString = System.Text.Encoding.UTF8.GetString(newFileData);
        //    }
        //    catch (WebException e)
        //    {
        //        String status = ((FtpWebResponse)e.Response).StatusDescription;
        //        MVVMMessageService.ShowMessage(status);
        //        clsStatic.WriteExceptionLogXML(e);
        //    }
        //    catch (Exception ex)
        //    {
        //        MVVMMessageService.ShowMessage(ex.Message);
        //    }
        //    return fileString;
        //}

        internal void isOrdersError()
        {
            if (_objWaitCursorViewModel != null)
                _objWaitCursorViewModel.Dispose();
            #region isError
            if (isError && !isOrdersAlreadyExist)
            {
                MessageBox.Show("Import orders completed. \nSome orders could not be matched to records in PhotoForce. Please review the Orders Log column in the Orders grid");
            }
            else if (isError && isOrdersAlreadyExist)
            {
                alreadyExistedOrders = alreadyExistedOrders.Substring(0, alreadyExistedOrders.Length - 2);
                //# commented by hema
                //MessageBox.Show("Import orders completed. \nSome orders could not be matched to records in PhotoForce. \nThe following orders were skipped because they already exist :" + alreadyExistedOrders + "\nPlease review the Orders Log column in the Orders grid");

                ///# added by hema for copying(Custom message box)
                MissingOrders _objMissingOrders = new MissingOrders(alreadyExistedOrders, "fromImportOrdersWithErrors");
                _objMissingOrders.ShowDialog();
            }
            else if (isOrdersAlreadyExist)
            {
                alreadyExistedOrders = alreadyExistedOrders.Substring(0, alreadyExistedOrders.Length - 2);
                //# commented by hema
                //MessageBox.Show("Import orders completed. \nThe following orders were skipped because they already exist :" + alreadyExistedOrders);                

                ///# used to show skipped orders while importing(Custom message box)
                MissingOrders _objMissingOrders = new MissingOrders(alreadyExistedOrders, "fromImportOrders");
                _objMissingOrders.ShowDialog();
            }
            else
            {
                MessageBox.Show(xmlFilesCount + " Orders imported successfully.");
            }
            #endregion
        }
        /// <summary>
        /// isFailedOrders shows the error message with failed xml file names
        /// </summary>
        /// <param name="destinationPath">represents the destinationPath where failed orders moved </param>
        private void isFailedOrders(string destinationPath)
        {
            if (isError && isFromLocalDisk)
            {

                if (failedOrderIds.Length > 0)
                {
                    if (_objWaitCursorViewModel != null)
                        _objWaitCursorViewModel.Dispose();

                    failedOrderIds = failedOrderIds.Remove(failedOrderIds.Length - 2);
                    //MVVMMessageService.ShowMessage(" Orders failed while imporiting are : " + failedOrderIds + ".\n Failed orders moved to :" + destinationPath);

                    //# used to show failed orders while importing(custom message box)
                    MissingOrders _objMissingOrders = new MissingOrders(failedOrderIds, "failedImportOrders", destinationPath);
                    _objMissingOrders.ShowDialog();
                }
            }
        }
        int createOrdersImportBatch(string typeOfImport)
        {
            int tempOrderImportBatchId = 0;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            OrdersImport _objOrdersImport = new OrdersImport();
            _objOrdersImport.Description = typeOfImport;
            _objOrdersImport.Notes = "";
            _objOrdersImport.CreatedOn = DateTime.Now;
            if (typeOfImport == "from excel file.")
                _objOrdersImport.OrderType = OrderTypeInOrdersImport.Excel.ToString();
            else
                _objOrdersImport.OrderType = OrderTypeInOrdersImport.Auto.ToString();
            _objOrdersImport.SchoolID = null;
            if (_objOrdersImport != null)
            {
                db.OrdersImports.InsertOnSubmit(_objOrdersImport);
                db.SubmitChanges();
                tempOrderImportBatchId = _objOrdersImport.Id;
            }
            return tempOrderImportBatchId;
        }

        #endregion

        void windowClose()
        {
            if (_objWaitCursorViewModel != null)
                _objWaitCursorViewModel.Dispose();
            isProgressCancelled = true;
        }
    }

    public class DirectoryItem
    {
        public Uri BaseUri;

        public string AbsolutePath
        {
            get
            {
                return string.Format("{0}/{1}", BaseUri, Name);
            }
        }

        public DateTime DateCreated;
        public bool IsDirectory;
        public string Name;
        public List<DirectoryItem> Items;
    }
}
