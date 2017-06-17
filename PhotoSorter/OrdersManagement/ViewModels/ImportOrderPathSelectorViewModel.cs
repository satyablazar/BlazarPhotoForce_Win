using Ookii.Dialogs.Wpf;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.OrdersManagement
{
    public class ImportOrderPathSelectorViewModel : ViewModelBase
    {
        #region Initialization
        public string folderPath = "";
        public bool isSave = false;
        string[] columnNames;
        public DataTable dt;
        #endregion

        #region Properties
        public string _txtPath;
        bool _isFromFTPsiteChecked;
        bool _isFromfolderChecked;
        bool _istxtAndbtnEnabled;
        bool _isFromExcelChecked;
        bool _isFromFTPsiteEnabled;
        bool _isFromGotPhotoChecked;

        public bool isFromGotPhotoChecked
        {
            get { return _isFromGotPhotoChecked; }
            set 
            {
                _isFromGotPhotoChecked = value; NotifyPropertyChanged();

                if (isFromGotPhotoChecked)
                {
                    isFromExcelChecked = true;
                    isFromFTPsiteChecked = false;
                    isFromfolderChecked = false;
                }
            }
        }
        public bool isFromFTPsiteEnabled
        {
            get { return _isFromFTPsiteEnabled; }
            set { _isFromFTPsiteEnabled = value; NotifyPropertyChanged("isFromFTPsiteEnabled"); }
        }

        public bool isFromExcelChecked
        {
            get { return _isFromExcelChecked; }
            set
            {
                _isFromExcelChecked = value; NotifyPropertyChanged("isFromExcelChecked");
                if (isFromFTPsiteChecked == true)
                    istxtAndbtnEnabled = false;
                else
                    istxtAndbtnEnabled = true;
            }
        }
        public bool isFromFTPsiteChecked
        {
            get { return _isFromFTPsiteChecked; }
            set
            {
                _isFromFTPsiteChecked = value; NotifyPropertyChanged("isFromFTPsiteChecked");
                if (isFromFTPsiteChecked)
                {
                    istxtAndbtnEnabled = false;
                }
                else
                    istxtAndbtnEnabled = true;
            }
        }
        public bool isFromfolderChecked
        {
            get { return _isFromfolderChecked; }
            set
            {
                _isFromfolderChecked = value; NotifyPropertyChanged("isFromfolderChecked");
                if (_isFromFTPsiteChecked)
                {
                    istxtAndbtnEnabled = false;                    
                }
                else
                    istxtAndbtnEnabled = true;
                if(isFromfolderChecked)
                    isFromGotPhotoChecked = false;
            }
        }
        public bool istxtAndbtnEnabled
        {
            get { return _istxtAndbtnEnabled; }
            set { _istxtAndbtnEnabled = value; NotifyPropertyChanged("istxtAndbtnEnabled"); }
        }
        public string txtPath
        {
            get { return _txtPath; }
            set { _txtPath = value; NotifyPropertyChanged("txtPath"); }
        }

        #endregion

        #region Construcor
        public ImportOrderPathSelectorViewModel()
        {
            string nealConnectionString = @"Data Source=fpsql\sqlexpress,1522;Initial Catalog=photoforce4;User ID=sa;Password=scanjet88";

            if (clsConnectionString.connectionString.ToLower() == nealConnectionString.ToLower())
            {
                isFromFTPsiteEnabled = true;
                isFromFTPsiteChecked = true;
                istxtAndbtnEnabled = false;
            }
            else
            {
                istxtAndbtnEnabled = true;
                isFromFTPsiteEnabled = false;
                isFromfolderChecked = true;                
            }
        }
        #endregion

        #region Commands
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(WindowClose);
            }
        }
        public RelayCommand BrowseCommand
        {
            get
            {
                return new RelayCommand(BrowsePath);
            }
        }
        public RelayCommand ImportCommand
        {
            get
            {
                return new RelayCommand(ImportOrders);
            }
        }

        #endregion

        #region Methods
        internal void BrowsePath()
        {
            if (isFromExcelChecked)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Excel Files (.xlsx)|*.XLSX";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtPath = dlg.FileName;
                    //fileName = dlg.SafeFileName;
                }
            }
            else
            {
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                var res = dlg.ShowDialog();
                if (res != false)
                    txtPath = dlg.SelectedPath;
            }
        }
        internal void ImportOrders()
        {
            if (isFromFTPsiteChecked)
            {
                isSave = true;
                folderPath = "";
                DialogResult = false;
            }
            else if (isFromfolderChecked)
            {
                if (string.IsNullOrEmpty(txtPath))
                {
                    MVVMMessageService.ShowMessage("Please select folder path.");
                    isSave = false;
                    return;
                }
                else
                {
                    folderPath = txtPath;
                    isSave = true; DialogResult = false;
                }
            }
            else if (isFromExcelChecked)
            {
                if (string.IsNullOrEmpty(txtPath))
                {
                    MVVMMessageService.ShowMessage("Please select folder path.");
                    isSave = false;
                    return;
                }
                else
                {
                    if(!isFromGotPhotoChecked)
                        columnNames = new string[] { "Image", "packages", "Quantity", "itemLabel", "Order Number" };
                                             //{ "Image", "Billing code", "Quantity", "itemLabel", "Order Number" };
                    else
                        columnNames = new string[] { "OrdersPhoto.original Filename", "OrdersPhoto.sku", "OrdersPhoto.count", "OrdersPhoto.product Name", "Order.orderkey" };
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.OKCancel;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage("Your Excel file will now be validated. Required columns are: "+ string.Join(", ",columnNames) +".\nProceed?", "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.OK)
                    {
                        //call validate excel file function here
                        //if not found any errors continue else return
                        if (!checkForRequiredColumns()) { isSave = false;  return; }
                        folderPath = txtPath;
                        isSave = true; DialogResult = false;
                    }
                }
            }
            else
            {
                MVVMMessageService.ShowMessage("Please select one option."); isSave = false; return;
            }
        }
        bool checkForRequiredColumns()
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
                                    MVVMMessageService.ShowMessage( "Cannot find value in " + rowNo + " row for required column \"" + tempName + "\"." + Environment.NewLine + "Please check the excel file.");
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
                    MVVMMessageService.ShowMessage( "Cannot find required column " + tempName + "." + Environment.NewLine + "Please check the excel file.");
                    //simplePhotoOrdersIds = new List<string>();
                    return false;
                }
            }
            return true;
        }
        internal void WindowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
