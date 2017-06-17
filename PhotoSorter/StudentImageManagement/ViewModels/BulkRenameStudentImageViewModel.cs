using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.Extensions;
using System.Text.RegularExpressions;
using System.ComponentModel;


namespace PhotoForce.StudentImageManagement
{
    public class BulkRenameStudentImageViewModel : ViewModelBase
    { 
        #region Initialization

        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        ArrayList arrStuImageId = new ArrayList();
        public bool isSave = false;
        int studentImageId = 0;
        //string previousPackage;
        //public StudentImage addEditStudentImage;
        public StudentImage _objImageDetails;
        #endregion

        #region Properties
        //private string _package;
        private string _homeRoom;
        private string _selectedRating;
        private List<ComboBoxItem> _rating;
        private string _ticketCode;
        private string _yearBookSelectedValue;
        private string _adminCDSelectedValue;
        private bool? _isYearBookSelected;
        private bool? _isAdminCDSelected;
        private bool? _isYearBookSelectedNo;
        private bool? _isAdminCDSelectedNo;
        private string _windowName;
        private string _saveButtonContent;
        //private List<OrderPackage> _packagesList;
        //private OrderPackage _cbSelectedPackage;
        //private string _txtQuantity;
        private string _custom1;
        private string _custom2;
        private string _custom3;
        private string _custom4;
        private string _custom5;

        public string custom1
        {
            get { return _custom1; }
            set { _custom1 = value; NotifyPropertyChanged("custom1"); }
        }
        public string custom2
        {
            get { return _custom2; }
            set { _custom2 = value; NotifyPropertyChanged("custom2"); }
        }
        public string custom3
        {
            get { return _custom3; }
            set { _custom3 = value; NotifyPropertyChanged("custom3"); }
        }
        public string custom4
        {
            get { return _custom4; }
            set { _custom4 = value; NotifyPropertyChanged("custom4"); }
        }
        public string custom5
        {
            get { return _custom5; }
            set { _custom5 = value; NotifyPropertyChanged("custom5"); }
        }
        //public string quantity
        //{
        //    get { return _txtQuantity; }
        //    set
        //    {
        //        _txtQuantity = value; NotifyPropertyChanged("quantity");

        //        if (!string.IsNullOrEmpty(quantity))
        //        {
        //            if (Regex.IsMatch(quantity, "^[0-9]+$", RegexOptions.IgnoreCase) == false)
        //            {
        //                quantity = previousPackage;
        //                MVVMMessageService.ShowMessage("Only numerics are allowed.");
        //                return;
        //            }
        //            else
        //            {
        //                previousPackage = quantity;
        //            }
        //        }
        //    }
        //}
        //public OrderPackage cbSelectedPackage
        //{
        //    get { return _cbSelectedPackage; }
        //    set { _cbSelectedPackage = value; NotifyPropertyChanged("cbSelectedPackage"); }
        //}
        //public List<OrderPackage> packagesList
        //{
        //    get { return _packagesList; }
        //    set { _packagesList = value; NotifyPropertyChanged("packagesList"); }
        //}
        public string saveButtonContent
        {
            get { return _saveButtonContent; }
            set { _saveButtonContent = value; NotifyPropertyChanged("saveButtonContent"); }
        }
        public string windowName
        {
            get { return _windowName; }
            set { _windowName = value; NotifyPropertyChanged("windowName"); }
        }
        public bool? isAdminCDNotSelected
        {
            get { return _isAdminCDSelectedNo; }
            set { _isAdminCDSelectedNo = value; NotifyPropertyChanged("isAdminCDNotSelected"); }
        }
        public bool? isYearBookNotSelected
        {
            get { return _isYearBookSelectedNo; }
            set { _isYearBookSelectedNo = value; NotifyPropertyChanged("isYearBookNotSelected"); }
        }
        public bool? isAdminCDSelected
        {
            get { return _isAdminCDSelected; }
            set { _isAdminCDSelected = value; NotifyPropertyChanged("isAdminCDSelected"); }
        }
        public bool? isYearBookSelected
        {
            get { return _isYearBookSelected; }
            set { _isYearBookSelected = value; NotifyPropertyChanged("isYearBookSelected"); }
        }
        public string adminCDSelectedValue
        {
            get { return _adminCDSelectedValue; }
            set { _adminCDSelectedValue = value; NotifyPropertyChanged("adminCDSelectedValue"); }
        }
        public string yearBookSelectedValue
        {
            get { return _yearBookSelectedValue; }
            set { _yearBookSelectedValue = value; NotifyPropertyChanged("yearBookSelectedValue"); }
        }
        public string ticketCode
        {
            get { return _ticketCode; }
            set { _ticketCode = value; NotifyPropertyChanged("ticketCode"); }
        }
        public List<ComboBoxItem> ratingData
        {
            get { return _rating; }
            set { _rating = value; NotifyPropertyChanged("ratingData"); }
        }
        public string selectedRating
        {
            get { return _selectedRating; }
            set { _selectedRating = value; NotifyPropertyChanged("selectedRating"); }
        }
        public string homeRoom
        {
            get { return _homeRoom; }
            set { _homeRoom = value; NotifyPropertyChanged("homeRoom"); }
        }
        //public string package
        //{
        //    get { return _package; }
        //    set { _package = value; NotifyPropertyChanged("package"); }
        //}
        #endregion

        #region Constructors
        public BulkRenameStudentImageViewModel(ArrayList tempStuImageId)
        {
            windowName = "Bulk Rename Student Image";
            saveButtonContent = "Bulk Rename";
            bindData();
            arrStuImageId = tempStuImageId;
        }
        public BulkRenameStudentImageViewModel(int tempStuImgId)
        {
            windowName = "Edit Student Image";
            saveButtonContent = "Save";
            studentImageId = tempStuImgId;
            arrStuImageId.Add(tempStuImgId);
            bindData();

            if (studentImageId != 0)
                bindStudentImgData();
        }

        #endregion

        #region Commands
        public RelayCommand BulkRenameCommand
        {
            get
            {
                return new RelayCommand(bulkRename);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand<string> YearBookCommand
        {
            get
            {
                return new RelayCommand<string>(yearBook);
            }
        }
        public RelayCommand<string> AdminCDCommand
        {
            get
            {
                return new RelayCommand<string>(adminCD);
            }
        }
        #endregion

        #region Methods
        private void bindData()
        { 
            //Rating data binding
            ratingData = new List<ComboBoxItem>();
            ratingData.Add(new ComboBoxItem { Name = " " });
            ratingData.Add(new ComboBoxItem { Name = "Clear" });
            for (int i = 0; i <= 9; i++)
            {
                ratingData.Add(new ComboBoxItem { Name = i.ToString() });
            }
            //packages data binding
            //packagesList = clsOrders.getAllOrderPackages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString)).OrderBy(op => op.Item).ToList();
        }
        private void yearBook(string tempYearBook)
        {
            yearBookSelectedValue = tempYearBook;
        }
        private void adminCD(string tempAdminCD)
        {
            adminCDSelectedValue = tempAdminCD;
        }
        private void bindStudentImgData()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                _objImageDetails = clsDashBoard.getStudentImageDetailsById(db, studentImageId);
                //package = Convert.ToString(_objImageDetails.Packages);
                homeRoom = Convert.ToString(_objImageDetails.HomeRoom);
                ticketCode = Convert.ToString(_objImageDetails.Ticketcode);
                selectedRating = Convert.ToString(_objImageDetails.Rating);
                custom1 = Convert.ToString(_objImageDetails.Custom1);
                custom2 = Convert.ToString(_objImageDetails.Custom2);
                custom3 = Convert.ToString(_objImageDetails.Custom3);
                custom4 = Convert.ToString(_objImageDetails.Custom4);
                custom5 = Convert.ToString(_objImageDetails.Custom5);
                if (Convert.ToBoolean(_objImageDetails.yearbook))
                    isYearBookSelected = true;
                else
                    isYearBookNotSelected = true;

                if (Convert.ToBoolean(_objImageDetails.Admincd))
                    isAdminCDSelected = true;
                else
                    isAdminCDNotSelected = true;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void bulkRename()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                int i = 0;

                //if (cbSelectedPackage != null)
                //{
                //    if (((cbSelectedPackage.Package != "UnMatched") && string.IsNullOrEmpty(quantity))
                //        || ((cbSelectedPackage.Package == null || cbSelectedPackage.Package == "UnMatched") && !string.IsNullOrEmpty(quantity)))
                //    {
                //        System.Windows.MessageBox.Show("Please select package and enter quantity.");
                //        return;
                //    }

                //    if (cbSelectedPackage.Package != "UnMatched")
                //    {
                //        package = cbSelectedPackage.Item.Trim() + "-" + quantity;
                //        i = clsDashBoard.UpdateMultiplePacka ges(db, package, arrStuImageId);
                //    }
                //}

                if (!string.IsNullOrEmpty(homeRoom))
                    i = clsDashBoard.UpdateMultipleHomeRoom(db, homeRoom, arrStuImageId);
                if (!string.IsNullOrEmpty(ticketCode))
                    i = clsDashBoard.UpdateMultipleTicketCode(db, ticketCode, arrStuImageId);
                if (!string.IsNullOrEmpty(selectedRating))
                    i = clsDashBoard.UpdateRating(db, selectedRating == "Clear" ? null : selectedRating, arrStuImageId);
                if (!string.IsNullOrEmpty(yearBookSelectedValue))
                    i = clsDashBoard.UpdateMultipleYearbook(db, yearBookSelectedValue, arrStuImageId);
                if (!string.IsNullOrEmpty(adminCDSelectedValue))
                    i = clsDashBoard.UpdateMultipleAdminCD(db, adminCDSelectedValue, arrStuImageId);
                if (!string.IsNullOrEmpty(custom1))
                    i = clsDashBoard.updateCustomValue(db, custom1, arrStuImageId, "custom1");
                if (!string.IsNullOrEmpty(custom2))
                    i = clsDashBoard.updateCustomValue(db, custom2, arrStuImageId, "custom2");
                if (!string.IsNullOrEmpty(custom3))
                    i = clsDashBoard.updateCustomValue(db, custom3, arrStuImageId, "custom3");
                if (!string.IsNullOrEmpty(custom4))
                    i = clsDashBoard.updateCustomValue(db, custom4, arrStuImageId, "custom4");
                if (!string.IsNullOrEmpty(custom5))
                    i = clsDashBoard.updateCustomValue(db, custom5, arrStuImageId, "custom5");
                if (i != 0)
                {
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
        private void windowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion

    }
}
