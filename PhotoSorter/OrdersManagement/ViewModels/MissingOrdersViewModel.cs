using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.OrdersManagement
{
    public class MissingOrdersViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext();
        string txtPath = "";
        string[] columnNames;
        public DataTable dt;
        #endregion

        #region Properties
        string _missingOrdersData;
        string _totalNumberOfOrdersMissing;
        string _name;
        string _windowName;
        Visibility _isBrowseVisible;

        public Visibility isBrowseVisible
        {
            get { return _isBrowseVisible; }
            set { _isBrowseVisible = value; NotifyPropertyChanged(); }
        }
        public string windowName
        {
            get { return _windowName; }
            set { _windowName = value; NotifyPropertyChanged(); }
        }
        public string name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged(); }
        }
        public string missingOrdersData
        {
            get { return _missingOrdersData; }
            set { _missingOrdersData = value; NotifyPropertyChanged("missingOrdersData"); }
        }
        public string totalNumberOfOrdersMissing
        {
            get { return _totalNumberOfOrdersMissing; }
            set { _totalNumberOfOrdersMissing = value; NotifyPropertyChanged("totalNumberOfOrdersMissing"); }
        }
        #endregion

        #region Constructor
        //if we have to show missing orders data
        public MissingOrdersViewModel()
        {
            isBrowseVisible = Visibility.Visible;
            name = "Missed Student Photo Order Ids";
            windowName = "Missing Orders";
        }
        //using to show not found image names ,in COPY RETOUCH IMAGES functionality
        //# also used to display orders those not exported
        //# aslo used to display skipped orders while importing
        public MissingOrdersViewModel(string missedImages, string isFrom)
        {
            isBrowseVisible = Visibility.Collapsed;
            if (isFrom == "from OrdersExport")
            {                
                name = "OrderId(s) of orders that doesn't have student image information in order details.";
                windowName = "Orders not exported";
                missingOrdersData = missedImages;
                totalNumberOfOrdersMissing = "";
            }
            else if (isFrom == "fromImportOrders")
            {
                name = "The following order(s) were skipped because they already exist :";
                windowName = "Import orders completed";
                missingOrdersData = missedImages;
                totalNumberOfOrdersMissing = "";
            }
            else if (isFrom == "fromImportOrdersWithErrors")
            {
                name = "The following order(s) were skipped because they already exist :";
                windowName = "Import orders completed";
                missingOrdersData = missedImages;
                totalNumberOfOrdersMissing = " Some orders could not be matched to records in PhotoForce. \n Please review the Orders Log column in the Orders grid.";
            }
            else
            {
                name = "Matching images were not found for the following images.";
                windowName = "Images Not Found";
                missingOrdersData = missedImages;
                totalNumberOfOrdersMissing = "";
            }
        }
        //# used to show failed orders while importing
        public MissingOrdersViewModel(string failedOrders, string callFrom, string destinationPath)
        {            
            if (callFrom == "failedImportOrders")
            {
                name = "Orders failed while imporiting are :";
                windowName = "Failed orders";
                missingOrdersData = failedOrders;
                totalNumberOfOrdersMissing = "Failed orders moved to : "+ destinationPath;
            }
        }
        #endregion

        #region Commands
        public RelayCommand BrowseExcelPathCommand
        {
            get
            {
                return new RelayCommand(browseExcelPath);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand CopyToClipBoardCommand
        {
            get
            {
                return new RelayCommand(copyToClipBoard);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// this method used to select Excel file from local disk
        /// </summary>
        void browseExcelPath()
        {
            missingOrdersData = null;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Excel Files (.xlsx)|*.XLSX";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                txtPath = dlg.FileName;
                if (!validateExcelFile()) { return; }

                #region Checking for missing orders
                if (dt.Rows.Count == 0)
                {
                    MVVMMessageService.ShowMessage("There are no orders to process.");
                    return;
                }
                DataView dv = dt.DefaultView;
                dv.Sort = "Order ID asc";
                dt = dv.ToTable();
                string orderIds = "";
                //int i = 0;
                int j = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    //i++;
                    try
                    {
                        if (String.IsNullOrEmpty((dr["Order ID"]).ToString())) { MVVMMessageService.ShowMessage("OrderId column can not be empty or null"); continue; }
                        bool _objtempOrder = clsOrders.getOrderDetails(db, Convert.ToInt32(dr["Order ID"]));
                        if (_objtempOrder == false)
                        {
                            if (!string.IsNullOrEmpty(orderIds))
                            {
                                j++;
                                orderIds = orderIds + ", " + dr["Order Id"].ToString();
                            }
                            else
                            {
                                j++;
                                orderIds = dr["Order ID"].ToString();
                            }
                            //if (i == 15)
                            //{
                            //    i = 0;
                            //    orderIds = orderIds + "\n";
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }
                missingOrdersData = orderIds;
                totalNumberOfOrdersMissing = "Missed orders " + j + ". Total orders " + dt.Rows.Count;
                #endregion
            }
        }
        void windowClose()
        {
            DialogResult = false;
        }
        private bool validateExcelFile()
        {
            if (string.IsNullOrEmpty(txtPath))
            {
                MVVMMessageService.ShowMessage("Please select folder path.");
                //isSave = false;
                return false;
            }
            else
            {
                columnNames = new string[] { "Order ID" };
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.OKCancel;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage("Your Excel file will now be validated. Required columns are: " + string.Join(", ", columnNames) + ".\nProceed?", "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.OK)
                {
                    //call validate excel file function here
                    //if not found any errors continue else return
                    if (!checkForRequiredColumns()) { return false; }
                    else
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// To validate excel file based on selected and required columns
        /// </summary>
        /// <returns></returns>
        private bool checkForRequiredColumns()
        {
            dt = PhotoForce.Helpers.DataLoader.getDataTableFromExcel(txtPath);
            if (dt == null) { MVVMMessageService.ShowMessage("Data not found. Please close excel file if it is opend and retry."); return false; }
            else
            {
                string tempName = "";
                DataRow dr = dt.Rows[0];

                try
                {
                    //checking wether any column is missed.
                    foreach (string column in columnNames)
                    {
                        int rowNo = 2;
                        tempName = column;
                        if (!string.IsNullOrEmpty(dr[column].ToString()))
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string res = row[column].ToString();
                                //if (column == "Order Number")
                                //{
                                //    if (!simplePhotoOrdersIds.Contains(res))
                                //    {
                                //        simplePhotoOrdersIds.Add(res);
                                //    }
                                //}
                                if (string.IsNullOrEmpty(res))
                                {
                                    MVVMMessageService.ShowMessage("Cannot find value in " + rowNo + " row for required column \"" + tempName + "\"." + Environment.NewLine + "Please check the excel file.");
                                    //simplePhotoOrdersIds = new List<string>();
                                    return false;
                                }
                                rowNo++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage("Cannot find required column " + tempName + "." + Environment.NewLine + "Please check the excel file.");
                    //simplePhotoOrdersIds = new List<string>();
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// To Copy missing order ids to clip board
        /// </summary>
        /// <returns></returns>
        private void copyToClipBoard()
        {
            Clipboard.SetText(missingOrdersData);

            MVVMMessageService.ShowMessage("Missing order id's copied to clipboard.");
        }
        #endregion
    }
}
