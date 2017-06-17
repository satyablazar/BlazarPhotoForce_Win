using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Data;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Threading;
using PhotoForce.Helpers;
using System.Data.SqlClient;
using PhotoForce.GroupManagement;
using System.Drawing;
using Ookii.Dialogs.Wpf;
using System.Text.RegularExpressions;
using System.ComponentModel;
using PhotoForce.Extensions;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
//using System.IO.Compression;

namespace PhotoForce.WorkPlace
{
    public class ExportViewModel : ViewModelBase
    {
        #region Initialization
        List<GroupClassPhoto> objGroupClassPhotos = new List<GroupClassPhoto>();
        School _objSchool = new School();
        int allGroupId = 0;
        string newPath = "";
        int schoolId;
        int[] arrStudentImageIds;
        int[] temparrStudents;
        ArrayList arrGroupId;
        //ArrayList arrStudID;
        //List<Student> studentsList = new List<Student>();
        PhotoShoot objImageFolder = new PhotoShoot();
        List<GroupClassPhoto> objGroupPhoto;
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        School _objsc = new School();
        List<int> lstManualOrderIds;
        //string schoolpath = "";
        int CountRows = 0;
        string jobName = "";
        string ImageFolder = "";
        //string imagepath = "";
        //string currentdatetime = "";
        string folderName = "";
        string foldernamee = "";
        //string path = "";
        //string folderNameToSave = "";
        string studentid = "";
        string studentFirstName = "", studentLastName = "";
        string sequentialNumber = "";
        string strImgExtenssion = "";
        string MoveImageName = "";
        String imageName = "";
        string studentIDPK = null;
        //string password = "";
        string missingImages = "Missingimages.txt";
        internal bool isMissedImages = false;
        bool isSourceFolderNotFound = false;
        Dictionary<string, string> imgPwdWithStuID = new Dictionary<string, string>();
        internal bool isSave = false;
        BackgroundWorker worker = new BackgroundWorker();
        bool isInProgress = false;
        bool isProgressCancelled = false;
        private List<GroupItem> VisibleData;
        ArrayList selectedStudents;
        ArrayList selectedStudImageIds = new ArrayList();
        Dictionary<int, string> dicStuImgGroupClassInfo = new Dictionary<int, string>();
        Dictionary<int, string> dicOrderItemsBillingCode = new Dictionary<int, string>();
        ArrayList allOrderIds = new ArrayList();
        List<Order> allOrders = new List<Order>();
        ArrayList tempSelectedOrderIds = new ArrayList();//by hema 09/12/2015
        bool isFromOrders;
        string missedImagesfolderPath = "";
        bool isMissingOrderImages = false;
        string tempFolderName = "";//Added by Hema 
        string folderPathtxt = ""; // Added by Hema
        //string isRetouch = "";
        WaitCursorViewModel _objWaitCursorViewModel;
        string exportType = "";
        private bool isImageQuixChecked;
        string studentsWithoutPasswords = "";
        List<GroupClassPhoto> iqGroupClassPhotos = new List<GroupClassPhoto>();
        string zipfilepath = ""; string createdNewPath = ""; public bool isMissiedImages = true;
        string tempImage = "";
        string tempDestinationPath = "";
        #endregion

        #region Properties
        private IEnumerable<tblmask> _masksData;
        private bool _isAddImgSerialNo;
        private bool _isEachStudentChecked;
        private bool _isAppendPwdChecked;
        private string _alternateFullPath;
        private bool _isRenameFileChecked;
        private tblmask _selectedMask;
        private bool _isBottomButtonsEnabled;
        private System.Windows.Visibility _isProgressBarVisible;
        private bool _isExportSelectedEnable;
        private bool _isExportAllImagesChecked;
        private bool _isReducedImagesChecked;
        private bool _isWebsiteFormatChecked;
        private bool _isSimplePhotoFTPChecked;
        private bool _isZenfolioChecked;
        //private bool _isMillersChecked;
        private bool _isPSPAChecked;
        private string _maskSyntax;
        int _currentProgress;
        int _minValue;
        int _maxValue;
        bool _isEachStudentFolderEnable;
        bool _isRenameFileEnable;
        bool _isExportSelectedChecked;
        bool _isExportAllEnable;
        bool _isDigitalImagesChecked;
        bool _isFiltredOrdersSelected;
        private bool _isAddImgSerialNoEnabled;
        public int _selectedExportType;
        private bool _isMillersExportChecked;
        private bool _isFreePhotoExportChecked;
        private bool _isCreateDataFileEnabled;
        private bool _isShiprushExportChecked;
        private bool _isTopGridEnabled;
        private bool _isAppendGPPwdChecked;
        public bool isAppendGPPwdChecked
        {
            get { return _isAppendGPPwdChecked; }
            set { _isAppendGPPwdChecked = value; NotifyPropertyChanged(); }
        }
        public bool isTopGridEnabled
        {
            get { return _isTopGridEnabled; }
            set { _isTopGridEnabled = value; NotifyPropertyChanged(); }
        }
        public bool isShiprushExportChecked
        {
            get { return _isShiprushExportChecked; }
            set
            {
                _isShiprushExportChecked = value; NotifyPropertyChanged();
                if (isShiprushExportChecked)
                {
                    //isBottomButtonsEnabled = false; 
                    isTopGridEnabled = false;
                }
            }
        }
        public bool isCreateDataFileEnabled
        {
            get { return _isCreateDataFileEnabled; }
            set { _isCreateDataFileEnabled = value; NotifyPropertyChanged(); }
        }
        public bool isFreePhotoExportChecked
        {
            get { return _isFreePhotoExportChecked; }
            set
            {
                _isFreePhotoExportChecked = value; NotifyPropertyChanged();
                if (isFreePhotoExportChecked)
                {
                    isBottomButtonsEnabled = true; isTopGridEnabled = true;
                }
            }
        }
        public bool isMillersExportChecked
        {
            get { return _isMillersExportChecked; }
            set
            {
                _isMillersExportChecked = value; NotifyPropertyChanged();
                if (isMillersExportChecked)
                {
                    isBottomButtonsEnabled = true; isTopGridEnabled = true;
                }
            }
        }
        public int selectedExportType
        {
            get { return _selectedExportType; }
            set
            {
                _selectedExportType = value; NotifyPropertyChanged("selectedPhotoShootIndex");

                if (selectedExportType == 1)
                {
                    //if (arrGroupId.Count > 1)
                    //{
                    //    MVVMMessageService.ShowMessage("ImageQuix can only be exported for one group at a time. Please select one group and try again.");

                    //    _selectedExportType = 0;
                    //}
                    //else
                    //{
                    exportType = "ImageQuix";
                    isAddImgSerialNo = false;
                    isEachStudentChecked = false;
                    isRenameFileChecked = false;
                    isEachStudentFolderEnable = false;
                    isRenameFileEnable = false;
                    isAddImgSerialNoEnabled = false;

                    isImageQuixChecked = true;

                    isWebsiteFormatChecked = false;
                    isZenfolioChecked = false;
                    isPSPAChecked = false;
                    if (arrGroupId.Count > 1)
                        isExportSelectedEnable = false;
                    else
                        isExportSelectedEnable = true;
                    isExportPathEnable = true;
                    // }
                }
                if (selectedExportType == 0)
                {
                    exportType = "Standard";
                    isEachStudentFolderEnable = true;
                    isRenameFileEnable = true;
                    isAddImgSerialNoEnabled = true;
                    isImageQuixChecked = false;
                    if (arrGroupId != null && arrGroupId.Count > 1)
                        isExportSelectedEnable = false;
                    else
                        isExportSelectedEnable = true;
                    isExportPathEnable = true;
                    isWebsiteFormatChecked = true;
                }
                if (selectedExportType == 2 || selectedExportType == 3)
                {
                    if (selectedExportType == 2)
                        exportType = "SimplePhoto";
                    if (selectedExportType == 3)
                        exportType = "GotPhoto";
                    isImageQuixChecked = false;
                    isExportFTPChecked = true;
                    if (arrGroupId.Count > 1)
                        isExportSelectedEnable = false;
                    else
                        isExportSelectedEnable = true;
                }
            }
        }
        //public bool isImageQuixChecked
        //{
        //    get { return _isImageQuixChecked; }
        //    set
        //    {
        //        _isImageQuixChecked = value; NotifyPropertyChanged("isImageQuixChecked");

        //        if (isImageQuixChecked == true)
        //        {
        //            isAddImgSerialNo = false;
        //            isEachStudentChecked = false;
        //            isRenameFileChecked = false;
        //            isEachStudentFolderEnable = false;
        //            isRenameFileEnable = false;
        //            isAddImgSerialNoEnabled = false;
        //        }
        //        else
        //        {
        //            isEachStudentFolderEnable = true;
        //            isRenameFileEnable = true;
        //            isAddImgSerialNoEnabled = true;
        //        }
        //    }
        //}
        public bool isAddImgSerialNoEnabled
        {
            get { return _isAddImgSerialNoEnabled; }
            set { _isAddImgSerialNoEnabled = value; NotifyPropertyChanged("isAddImgSerialNoEnabled"); }
        }
        public bool isFiltredOrdersSelected
        {
            get { return _isFiltredOrdersSelected; }
            set { _isFiltredOrdersSelected = value; NotifyPropertyChanged("isFiltredOrdersSelected"); }
        }
        public bool isDigitalImagesChecked
        {
            get { return _isDigitalImagesChecked; }
            set { _isDigitalImagesChecked = value; NotifyPropertyChanged("isDigitalImagesChecked"); }
        }
        public bool isExportAllEnable
        {
            get { return _isExportAllEnable; }
            set { _isExportAllEnable = value; NotifyPropertyChanged("isExportAllEnable"); }
        }
        public bool isExportSelectedChecked
        {
            get { return _isExportSelectedChecked; }
            set { _isExportSelectedChecked = value; NotifyPropertyChanged("isExportSelectedChecked"); }
        }
        public bool isRenameFileEnable
        {
            get { return _isRenameFileEnable; }
            set { _isRenameFileEnable = value; NotifyPropertyChanged("isRenameFileEnable"); }
        }
        public bool isEachStudentFolderEnable
        {
            get { return _isEachStudentFolderEnable; }
            set { _isEachStudentFolderEnable = value; NotifyPropertyChanged("isEachStudentFolderEnable"); }
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
        public string maskSyntax
        {
            get { return _maskSyntax; }
            set { _maskSyntax = value; NotifyPropertyChanged("maskSyntax"); }
        }
        public bool isPSPAChecked
        {
            get { return _isPSPAChecked; }
            set
            {
                _isPSPAChecked = value;
                if (_isPSPAChecked == true) { isEachStudentChecked = false; isEachStudentFolderEnable = false; isRenameFileChecked = false; isRenameFileEnable = false; }
                NotifyPropertyChanged("isPSPAChecked");
            }
        }
        public bool isZenfolioChecked
        {
            get { return _isZenfolioChecked; }
            set
            {
                _isZenfolioChecked = value;
                NotifyPropertyChanged("isZenfolioChecked");
                if (_isZenfolioChecked == true) { isEachStudentChecked = false; isEachStudentFolderEnable = false; isRenameFileEnable = false; isAddImgSerialNoEnabled = false; }
            }
        }
        //public bool isMillersChecked
        //{
        //    get { return _isMillersChecked; }
        //    set
        //    {
        //        _isMillersChecked = value;
        //        NotifyPropertyChanged("isMillersChecked");
        //        if (isMillersChecked == true)
        //        { isEachStudentChecked = false; isEachStudentFolderEnable = false; }
        //    }
        //}
        public bool isSimplePhotoFTPChecked
        {
            get { return _isSimplePhotoFTPChecked; }
            set
            {
                _isSimplePhotoFTPChecked = value; NotifyPropertyChanged();
                if (isSimplePhotoFTPChecked) { isEachStudentChecked = false; isEachStudentFolderEnable = false; isRenameFileChecked = false; isRenameFileEnable = false; }
            }
        }
        public bool isWebsiteFormatChecked
        {
            get { return _isWebsiteFormatChecked; }
            set
            {
                _isWebsiteFormatChecked = value; NotifyPropertyChanged("isWebsiteFormatChecked");
                if (_isWebsiteFormatChecked == true) { isEachStudentChecked = true; isEachStudentFolderEnable = true; isRenameFileEnable = true; }
            }
        }
        public bool isReducedImagesChecked
        {
            get { return _isReducedImagesChecked; }
            set { _isReducedImagesChecked = value; NotifyPropertyChanged("isReducedImagesChecked"); }
        }
        public bool isExportAllImagesChecked
        {
            get { return _isExportAllImagesChecked; }
            set { _isExportAllImagesChecked = value; NotifyPropertyChanged("isExportAllImagesChecked"); }
        }
        public bool isExportSelectedEnable
        {
            get { return _isExportSelectedEnable; }
            set { _isExportSelectedEnable = value; NotifyPropertyChanged("isExportSelectedEnable"); }
        }
        public System.Windows.Visibility isProgressBarVisible
        {
            get { return _isProgressBarVisible; }
            set { _isProgressBarVisible = value; NotifyPropertyChanged("isProgressBarVisible"); }
        }
        public bool isBottomButtonsEnabled
        {
            get { return _isBottomButtonsEnabled; }
            set { _isBottomButtonsEnabled = value; NotifyPropertyChanged("isBottomButtonsEnabled"); }
        }
        public tblmask selectedMask
        {
            get { return _selectedMask; }
            set { _selectedMask = value; NotifyPropertyChanged("selectedMask"); maskSelection(); }
        }
        public bool isRenameFileChecked
        {
            get { return _isRenameFileChecked; }
            set { _isRenameFileChecked = value; NotifyPropertyChanged("isRenameFileChecked"); }
        }
        public string alternateFullPath
        {
            get { return _alternateFullPath; }
            set { _alternateFullPath = value; NotifyPropertyChanged("alternateFullPath"); }
        }
        public bool isAppendPwdChecked
        {
            get { return _isAppendPwdChecked; }
            set { _isAppendPwdChecked = value; NotifyPropertyChanged("isAppendPwdChecked"); }
        }
        public bool isEachStudentChecked
        {
            get { return _isEachStudentChecked; }
            set
            {
                _isEachStudentChecked = value; NotifyPropertyChanged("isEachStudentChecked");
                if (isEachStudentChecked == false)
                    isAppendPwdChecked = false;
            }
        }
        public bool isAddImgSerialNo
        {
            get { return _isAddImgSerialNo; }
            set { _isAddImgSerialNo = value; NotifyPropertyChanged("isAddImgSerialNo"); }
        }
        public IEnumerable<tblmask> masksData
        {
            get { return _masksData; }
            set
            {
                _masksData = value;
                NotifyPropertyChanged("masksData");
            }
        }
        #endregion

        #region ImageQuix Properies
        bool _isPreOrderChecked;
        bool _isGreenScreenChecked;
        string _txtTitle;
        List<string> _jobTypeData;
        DateTime? _dpEventDate;
        DateTime? _dpShipDate;
        string _txtWelcomeMessage;
        string _txtPassword;
        string _txtReference;
        List<string> _imageSizeData;
        bool _selectedisHidden;
        //string _txtKeyword;
        string _selectedJobType;
        string _selectedImageSize;
        bool _isOnePwd;
        bool _isIndividualPwd;
        ObservableCollection<IQAccount> _iqAccountsData;
        IQAccount _selectedIQAccount;
        ObservableCollection<IQPriceSheet> _cbIQPricesheetData;
        ObservableCollection<IQVandoSetting> _cbIQVandoSettingsData;
        IQPriceSheet _selectedIQPricesheet;
        bool _isPasswordEnabled;
        IQVandoSetting _selectedIQVandoSettings;

        public IQVandoSetting selectedIQVandoSettings
        {
            get { return _selectedIQVandoSettings; }
            set { _selectedIQVandoSettings = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<IQVandoSetting> cbIQVandoSettingsData
        {
            get { return _cbIQVandoSettingsData; }
            set { _cbIQVandoSettingsData = value; NotifyPropertyChanged(); }
        }
        public bool isPasswordEnabled
        {
            get { return _isPasswordEnabled; }
            set { _isPasswordEnabled = value; NotifyPropertyChanged("isPasswordEnabled"); }
        }
        public IQPriceSheet selectedIQPricesheet
        {
            get { return _selectedIQPricesheet; }
            set { _selectedIQPricesheet = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<IQPriceSheet> cbIQPricesheetData
        {
            get { return _cbIQPricesheetData; }
            set { _cbIQPricesheetData = value; NotifyPropertyChanged(); }
        }
        public IQAccount selectedIQAccount
        {
            get { return _selectedIQAccount; }
            set
            {
                _selectedIQAccount = value; NotifyPropertyChanged();
                if (selectedIQAccount != null)
                {
                    cbIQPricesheetData = new ObservableCollection<IQPriceSheet>((from IQP in db.IQPriceSheets where IQP.IQAccountId == selectedIQAccount.Id orderby IQP.Id select IQP).ToList());
                    cbIQVandoSettingsData = new ObservableCollection<IQVandoSetting>((from IQV in db.IQVandoSettings where IQV.IQAccountId == selectedIQAccount.Id orderby IQV.Id select IQV).ToList());
                    selectedIQPricesheet = cbIQPricesheetData.FirstOrDefault();
                    selectedIQVandoSettings = cbIQVandoSettingsData.FirstOrDefault();
                }
            }
        }
        public ObservableCollection<IQAccount> iqAccountsData
        {
            get { return _iqAccountsData; }
            set { _iqAccountsData = value; NotifyPropertyChanged(); }
        }
        public bool isIndividualPwd
        {
            get { return _isIndividualPwd; }
            set
            {
                _isIndividualPwd = value; NotifyPropertyChanged("isIndividualPwd");
                if (isIndividualPwd) { txtPassword = string.Empty; isPasswordEnabled = false; }
            }
        }
        public bool isOnePwd
        {
            get { return _isOnePwd; }
            set
            {
                _isOnePwd = value; NotifyPropertyChanged("isOnePwd");
                if (isOnePwd) { isPasswordEnabled = true; }
            }
        }
        public string selectedImageSize
        {
            get { return _selectedImageSize; }
            set { _selectedImageSize = value; NotifyPropertyChanged("selectedImageSize"); }
        }
        public string selectedJobType
        {
            get { return _selectedJobType; }
            set { _selectedJobType = value; NotifyPropertyChanged("selectedJobType"); }
        }
        //public string txtKeyword
        //{
        //    get { return _txtKeyword; }
        //    set { _txtKeyword = value; NotifyPropertyChanged("txtKeyword"); }
        //}
        public bool selectedisHidden
        {
            get { return _selectedisHidden; }
            set { _selectedisHidden = value; NotifyPropertyChanged("selectedisHidden"); }
        }
        public List<string> imageSizeData
        {
            get { return _imageSizeData; }
            set { _imageSizeData = value; NotifyPropertyChanged("imageSizeData"); }
        }
        public string txtReference
        {
            get { return _txtReference; }
            set { _txtReference = value; NotifyPropertyChanged("txtReference"); }
        }
        public string txtPassword
        {
            get { return _txtPassword; }
            set { _txtPassword = value; NotifyPropertyChanged("txtPassword"); }
        }
        public string txtWelcomeMessage
        {
            get { return _txtWelcomeMessage; }
            set { _txtWelcomeMessage = value; NotifyPropertyChanged("txtWelcomeMessage"); }
        }
        public DateTime? dpShipDate
        {
            get { return _dpShipDate; }
            set { _dpShipDate = value; NotifyPropertyChanged("dpShipDate"); }
        }
        public DateTime? dpEventDate
        {
            get { return _dpEventDate; }
            set { _dpEventDate = value; NotifyPropertyChanged("dpEventDate"); }
        }
        public List<string> jobTypeData
        {
            get { return _jobTypeData; }
            set { _jobTypeData = value; NotifyPropertyChanged("jobTypeData"); }
        }
        public string txtTitle
        {
            get { return _txtTitle; }
            set { _txtTitle = value; NotifyPropertyChanged("txtTitle"); }
        }
        public bool isGreenScreenChecked
        {
            get { return _isGreenScreenChecked; }
            set { _isGreenScreenChecked = value; NotifyPropertyChanged("isGreenScreenChecked"); }
        }
        public bool isPreOrderChecked
        {
            get { return _isPreOrderChecked; }
            set { _isPreOrderChecked = value; NotifyPropertyChanged("isPreOrderChecked"); }
        }
        #endregion

        #region SimplePhoto Properties
        bool _isExportFolderChecked;
        bool _isExportFTPChecked;
        string _txtSPTitle;
        string txtSPOrderNumber;
        DateTime? _dpSPDate;
        bool _isMackUpPhotoshootChecked;
        bool _isUseFileNameFromDriveChecked;

        public bool isUseFileNameFromDriveChecked
        {
            get { return _isUseFileNameFromDriveChecked; }
            set { _isUseFileNameFromDriveChecked = value; NotifyPropertyChanged(); }
        }
        public bool isMackUpPhotoshootChecked
        {
            get { return _isMackUpPhotoshootChecked; }
            set { _isMackUpPhotoshootChecked = value; NotifyPropertyChanged(); }
        }
        //ObservableCollection<User> _cbSPPhotographerData;
        //User _selectedSPPhotographer;
        ObservableCollection<SimplePhotoPriceSheet> _cbSPPricesheetData;
        SimplePhotoPriceSheet _selectedSPPricesheet;
        bool _isExportPathEnable;
        //string _txtPhotographerId;

        //public string txtPhotographerId
        //{
        //    get { return _txtPhotographerId; }
        //    set { _txtPhotographerId = value; NotifyPropertyChanged(); }
        //}
        public bool isExportPathEnable
        {
            get { return _isExportPathEnable; }
            set { _isExportPathEnable = value; NotifyPropertyChanged(); }
        }
        public SimplePhotoPriceSheet selectedSPPricesheet
        {
            get { return _selectedSPPricesheet; }
            set { _selectedSPPricesheet = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<SimplePhotoPriceSheet> cbSPPricesheetData
        {
            get { return _cbSPPricesheetData; }
            set { _cbSPPricesheetData = value; NotifyPropertyChanged(); }
        }
        //public User selectedSPPhotographer
        //{
        //    get { return _selectedSPPhotographer; }
        //    set { _selectedSPPhotographer = value; }
        //}
        //public ObservableCollection<User> cbSPPhotographerData
        //{
        //    get { return _cbSPPhotographerData; }
        //    set { _cbSPPhotographerData = value; NotifyPropertyChanged(); }
        //}
        public DateTime? dpSPDate
        {
            get { return _dpSPDate; }
            set { _dpSPDate = value; NotifyPropertyChanged(); }
        }
        public string txtSPTitle
        {
            get { return _txtSPTitle; }
            set { _txtSPTitle = value; NotifyPropertyChanged(); }
        }
        public bool isExportFTPChecked
        {
            get { return _isExportFTPChecked; }
            set
            {
                _isExportFTPChecked = value; NotifyPropertyChanged();

                if (isExportFTPChecked)
                    isExportPathEnable = false;
            }
        }
        public bool isExportFolderChecked
        {
            get { return _isExportFolderChecked; }
            set
            {
                _isExportFolderChecked = value; NotifyPropertyChanged();
                if (isExportFolderChecked)
                    isExportPathEnable = true;
            }
        }
        #endregion

        #region Constructor
        //Normal Export
        public ExportViewModel(Dictionary<int, string> tempStudentImages, ArrayList arrSelectedStudents, ArrayList tempArrGrpId, int countrows,
            List<GroupItem> tempVisibleData)
        {
            try
            {
                selectedExportType = 0;
                dpEventDate = null;
                dpShipDate = null;
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                VisibleData = new List<GroupItem>();
                schoolId = clsSchool.defaultSchoolId;
                _objSchool = (from sc in db.Schools where sc.ID == schoolId select sc).FirstOrDefault();
                dicStuImgGroupClassInfo = tempStudentImages;
                VisibleData = tempVisibleData;

                temparrStudents = dicStuImgGroupClassInfo.Keys.ToArray(); selectedStudents = arrSelectedStudents;
                arrStudentImageIds = dicStuImgGroupClassInfo.Keys.ToArray(); CountRows = countrows;

                isExportAllEnable = true;
                isSimplePhotoFTPChecked = true; isBottomButtonsEnabled = true; isCreateDataFileEnabled = true; isExportAllImagesChecked = true; isReducedImagesChecked = true; isExportSelectedChecked = false;
                isEachStudentChecked = true; isAddImgSerialNo = true; isExportSelectedEnable = true; isEachStudentFolderEnable = true; isRenameFileEnable = true;

                if (arrStudentImageIds.Count() == 0 || countrows > 1)
                {
                    isExportSelectedEnable = false;
                }

                minValue = 0; currentProgress = 0; isProgressBarVisible = Visibility.Collapsed;

                arrGroupId = tempArrGrpId;
                if (isImageQuixChecked)
                {
                    isAddImgSerialNo = false;
                    isAddImgSerialNoEnabled = false;
                }
                else
                    isAddImgSerialNoEnabled = true;
                bindMaskComboBox();

                jobTypeData = new List<string>();
                imageSizeData = new List<string>();

                jobTypeData.Add("wedding"); jobTypeData.Add("portrait"); jobTypeData.Add("seniors"); jobTypeData.Add("underclass_spring"); jobTypeData.Add("underclass_fall"); jobTypeData.Add("sports"); jobTypeData.Add("event");
                imageSizeData.Add("web"); imageSizeData.Add("standard"); imageSizeData.Add("largeFormat");
                //why we need this code #Mohan
                //imagepath = clsDashBoard.getSettingByName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), "ImageFolderLocation").settingValue.Trim();
                //currentdatetime = DateTime.Now.ToString("yyyyMMdd-HHmm");

                //cbSPPhotographerData = new ObservableCollection<User>();
                //txtPhotographerId = "freedphoto";
                cbSPPricesheetData = new ObservableCollection<SimplePhotoPriceSheet>();
                //cbSPPhotographerData = new ObservableCollection<User>((from sph in db.Users select sph).ToList());
                //if (cbSPPhotographerData.Count > 0)
                //    selectedSPPhotographer = cbSPPhotographerData[0];
                cbSPPricesheetData = new ObservableCollection<SimplePhotoPriceSheet>((from spp in db.SimplePhotoPriceSheets select spp).ToList());
                if (cbSPPricesheetData.Count > 0)
                    selectedSPPricesheet = cbSPPricesheetData[0];
                dpSPDate = DateTime.Now;

                iqAccountsData = new ObservableCollection<IQAccount>(); cbIQPricesheetData = new ObservableCollection<IQPriceSheet>();
                iqAccountsData = new ObservableCollection<IQAccount>((from IQA in db.IQAccounts orderby IQA.Id select IQA).ToList());
                selectedIQAccount = iqAccountsData.FirstOrDefault();
                if (arrGroupId.Count > 1)
                    isExportSelectedEnable = false;
                else
                    isExportSelectedEnable = true;

            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        //Orders Export
        public ExportViewModel(Dictionary<int, string> tempStudentImages, ArrayList tempStuImageIds,
            ArrayList tempSelectedStudImageIds, Dictionary<int, string> tempOrderBillingCodeInfo, bool tempHasVisibleData, List<Order> orders, ArrayList orderIds, ArrayList selectedOrderIds)
        {
            isFromOrders = true;
            isMillersExportChecked = true;
            schoolId = clsSchool.defaultSchoolId;
            tempSelectedOrderIds = selectedOrderIds;
            dicOrderItemsBillingCode = tempOrderBillingCodeInfo;
            dicStuImgGroupClassInfo = tempStudentImages;
            //studentsList = tempStudents;
            allOrders = new List<Order>();


            allOrderIds = orderIds; allOrders = orders;
            temparrStudents = dicStuImgGroupClassInfo.OfType<int>().ToArray(); selectedStudents = tempStuImageIds; //here selected students means student image ids of selected orders
            selectedStudImageIds = tempSelectedStudImageIds;
            arrStudentImageIds = dicOrderItemsBillingCode.OfType<int>().ToArray();  //taking student photo order ids ,used to copy images and to identify Billing code

            isBottomButtonsEnabled = true; isCreateDataFileEnabled = true; isExportAllImagesChecked = false; isReducedImagesChecked = true; isExportAllEnable = false;
            isAddImgSerialNo = true; isExportSelectedEnable = true; isExportSelectedChecked = true; isRenameFileEnable = true;

            isEachStudentChecked = true; isRenameFileChecked = false;
            //selectedVendor = "Miller"; isMillersChecked = true;


            minValue = 0; currentProgress = 0; isProgressBarVisible = Visibility.Collapsed;

            bindMaskComboBox();

            if (selectedStudImageIds.Count == 0)
            {
                isExportSelectedEnable = false; isExportSelectedChecked = false;
            }
            if (tempHasVisibleData)
            {
                isExportAllEnable = true; isFiltredOrdersSelected = true;
            }

        }
        #endregion

        #region Commands

        public RelayCommand CreateDataFileCommand
        {
            get
            {
                return new RelayCommand(createDataFile);
            }
        }
        public RelayCommand ExportImagesCommand
        {
            get
            {
                return new RelayCommand(exportImages);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand BrowseAlternateFolderCommand
        {
            get
            {
                return new RelayCommand(browseAlternateFolder);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// this method used to export images (from both group and orders)
        /// </summary>
        async void exportImages()
        {
            try
            {
                if (isShiprushExportChecked)
                {
                    createDataFile();
                    return;
                }

                #region for SimplePhoto export type
                if (exportType == "SimplePhoto" || exportType == "GotPhoto")
                {
                    //if (arrGroupId.Count > 1 && isExportSelectedChecked)
                    //{
                    //    MVVMMessageService.ShowMessage("You can export only one group at a time using simplePhotoFTP");
                    //    return;
                    //}
                    if (string.IsNullOrEmpty(txtSPTitle) || string.IsNullOrEmpty(dpSPDate == null ? "" : (dpSPDate.Value.Date.ToString())) || string.IsNullOrEmpty(selectedSPPricesheet.SPPriceSheetId.ToString()))
                    {
                        return;
                    }
                }
                #endregion

                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                Boolean FlagValueCheck = true;
                if (!Directory.Exists(alternateFullPath) && !isExportFTPChecked)
                {
                    FlagValueCheck = false;
                    MessageBox.Show(errorMessages.ALTERNATIVE_PATH_DOES_NOT_EXIST);
                    return;
                }

                if (FlagValueCheck == true)     //if School Path or Alternative path exists
                {
                    if (isRenameFileChecked == true && selectedMask == null)     //if no mask selected
                    {
                        MessageBox.Show(errorMessages.SELECT_MASK);
                        return;
                    }

                    if (isFromOrders)
                    {
                        if (isFiltredOrdersSelected)
                            arrStudentImageIds = selectedStudents.ToArray(typeof(int)) as int[];
                        else
                            arrStudentImageIds = selectedStudImageIds.ToArray(typeof(int)) as int[];
                    }
                    else
                    {
                        if (isExportAllImagesChecked == true)      //if export all images selected
                            arrStudentImageIds = VisibleData.Count == 0 ? clsGroup.GetStudentImgIdOnMultiGroupid(db, arrGroupId) : temparrStudents;   // its array of StudentImagesID, not array of students
                        else
                            arrStudentImageIds = selectedStudents.ToArray(typeof(int)) as int[];
                    }

                    #region conditions for ImageQuix
                    if (isImageQuixChecked == true)
                    {
                        //bool hasGroupClassImage = false;
                        iqGroupClassPhotos = new List<GroupClassPhoto>();
                        if ((isOnePwd == true || isIndividualPwd == true))
                        {
                            if (isOnePwd == true)
                            {
                                if (string.IsNullOrEmpty(txtPassword))
                                {
                                    MVVMMessageService.ShowMessage("password is a mandatory field.");
                                    return;
                                }
                            }
                            else if (isIndividualPwd == true)
                            {
                                if (!string.IsNullOrEmpty(txtPassword))
                                {
                                    MVVMMessageService.ShowMessage("Please remove password to continue.");
                                    return;
                                }
                            }

                            foreach (int groupId in arrGroupId)
                            {
                                iqGroupClassPhotos.AddRange(clsGroup.getClassPhotoByGroup(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), groupId));

                                //if (iqGroupClassPhotos.Count() > 0)
                                //{
                                //    hasGroupClassImage = true;
                                //}
                            }
                            //if (hasGroupClassImage == true && isStandardChecked == true)
                            //{
                            //    MVVMMessageService.ShowMessage(" Export contain group class image(s) so export can't allowed in standard format.\n Please select subject type export.");
                            //    return;
                            //}
                            //if (hasGroupClassImage == true && isOnePwd == true)
                            //{
                            //    MVVMMessageService.ShowMessage("Selected group(s) contain group class image(s), so export can't be done in standard format.");
                            //    return;
                            //}
                            //else if (hasNoGroupClassImage == true && isSubjectChecked == true)
                            //{
                            //    MVVMMessageService.ShowMessage("Selected group(s) doesn't contain group class image(s), so export can't be done in subject format.");
                            //    return;
                            //}
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage("Please select password type.");
                            return;
                        }
                    }
                    #endregion

                    if (arrStudentImageIds.Count() == 0) { return; }

                    //Show confirmation message first...
                    string message = "";

                    message = "You are about to copy " + arrStudentImageIds.Count() + " image(s) and group class images(s) if any.";
                    if (tempSelectedOrderIds.Count > 1)
                        message = "You are about to copy " + tempSelectedOrderIds.Count + " Orders.";

                    if (isExportAllImagesChecked && isImageQuixChecked)
                    {
                        if (arrGroupId.Count > 1)
                        {
                            message = "You are about to export " + arrGroupId.Count + " group(s).";
                        }
                    }

                    if (isWebsiteFormatChecked == false || isZenfolioChecked == false)
                    {
                        if (isAddImgSerialNo == false)  //if add images serial number checkbox is not ticked
                            message += " The system will save duplicate file names according to Windows conventions, e.g. filename-copy(1).jpg.";
                    }

                    message += "\nDo you want to continue?";

                    string caption = "Confirmation";
                    MessageBoxButton buttons = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Question;
                    if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                    {
                        isBottomButtonsEnabled = false; isCreateDataFileEnabled = false;
                        Boolean ContainsFile = false;

                        ContainsFile = isDirectoryContainFiles(alternateFullPath);

                        if ((ContainsFile == false))
                        {
                            // The await operator suspends exportImages.
                            //  - exportImages can't continue until createFolderForGrade is complete.
                            //  - Meanwhile, control returns to the caller of exportImages.
                            //  - Control resumes here when createFolderForGrade is complete. 
                            //  - The await operator then retrieves the result from createFolderForGrade if method has any return type.
                            if (exportType == "SimplePhoto" || exportType == "GotPhoto")
                            {
                                isSimplePhotoFTPChecked = true;
                                await createFolderForSPGallery();
                            }
                            else if (isPSPAChecked)        //if PSPA Yearbook Standard radio button is selected  || isSimplePhotoFTPChecked
                            {
                                await createFolderForGrade();//ThreadPool.QueueUserWorkItem(funcCreateFolderForGrade);//
                            }
                            else
                                ThreadPool.QueueUserWorkItem(funcCopyPhotos);
                        }
                        else
                        {
                            //call new window ,similar to message box and check wether user selected 'delete folders' option or not.
                            DeleteFoldersMessage _objDeleteFoldersMessage = new DeleteFoldersMessage();
                            _objDeleteFoldersMessage.ShowDialog();
                            if (((DeleteFoldersMessageViewModel)(_objDeleteFoldersMessage.DataContext)).isCancelled == false)
                            {
                                if (((DeleteFoldersMessageViewModel)(_objDeleteFoldersMessage.DataContext)).isDeleteFoldersSelected)
                                {
                                    System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(alternateFullPath);

                                    foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                                    {
                                        file.Delete();
                                    }
                                    foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                                    {
                                        dir.Delete(true);
                                    }

                                    if (exportType == "SimplePhoto" || exportType == "GotPhoto")
                                    {
                                        isSimplePhotoFTPChecked = true;
                                        await createFolderForSPGallery();
                                    }
                                    else if (isPSPAChecked)        //if PSPA Yearbook Standard radio button is selected   || isSimplePhotoFTPChecked
                                    {
                                        await createFolderForGrade();//ThreadPool.QueueUserWorkItem(funcCreateFolderForGrade);//
                                    }
                                    else
                                        ThreadPool.QueueUserWorkItem(funcCopyPhotos);
                                }
                                else
                                {
                                    if (exportType == "SimplePhoto" || exportType == "GotPhoto")
                                    {
                                        isSimplePhotoFTPChecked = true;
                                        await createFolderForSPGallery();
                                    }
                                    else if (isPSPAChecked)        //if PSPA Yearbook Standard radio button is selected  || isSimplePhotoFTPChecked
                                    {
                                        await createFolderForGrade();//ThreadPool.QueueUserWorkItem(funcCreateFolderForGrade);//
                                    }
                                    else
                                        ThreadPool.QueueUserWorkItem(funcCopyPhotos);
                                }
                            }
                            else
                            {
                                isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
                isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
            }
        }
        /// <summary>
        /// used to close the window
        /// </summary>
        private void windowClose()
        {
            if (isInProgress)
            {
                isProgressCancelled = true;
            }
            else
            {
                isProgressCancelled = true;
                DialogResult = false;
                isSave = false;
            }
        }
        /// <summary>
        /// this method used to select the mask 
        /// </summary>
        private void maskSelection()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string Getsyntax = "";
                DataTable _objmskDetails = new DataTable();
                _objmskDetails = clsDashBoard.GetMaskDetails(db, selectedMask.MaskID);
                for (int cnt = 0; cnt < _objmskDetails.Rows.Count; cnt++)
                {
                    Getsyntax += _objmskDetails.Rows[cnt]["MaskDetail1"];
                }
                maskSyntax = Getsyntax.Replace("_", "__");
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method used to create datafile
        /// </summary>
        private void createDataFile()
        {
            try
            {
                if (string.IsNullOrEmpty(alternateFullPath))
                {
                    MVVMMessageService.ShowMessage("Destination path should not be empty.");
                    return;
                }
                string message = "Are you sure you want to create a DataFile?";
                string caption = "Confirmation";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Question;
                if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                {
                    if (!isShiprushExportChecked)
                    {
                        #region Creating datafile for millers/freedphoto export
                        Boolean ContainsFile = false;
                        ContainsFile = isDirectoryContainFiles(alternateFullPath);

                        if ((ContainsFile == false))
                        {
                            writeDataToFile();
                        }
                        else
                        {
                            //MessageBox.Show(errorMessages.FOLDER_NOT_EMPTY);
                            //call new window ,similar to message box and check wether user selected 'delete folders' option or not.
                            DeleteFoldersMessage _objDeleteFoldersMessage = new DeleteFoldersMessage();
                            _objDeleteFoldersMessage.ShowDialog();
                            if (((DeleteFoldersMessageViewModel)(_objDeleteFoldersMessage.DataContext)).isCancelled == false)
                            {
                                if (((DeleteFoldersMessageViewModel)(_objDeleteFoldersMessage.DataContext)).isDeleteFoldersSelected)
                                {
                                    System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(alternateFullPath);

                                    foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                                    {
                                        file.Delete();
                                    }
                                    foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                                    {
                                        dir.Delete(true);
                                    }

                                    writeDataToFile();
                                }
                                else
                                {
                                    writeDataToFile();
                                }
                            }
                        }
                        #endregion
                    }
                    else if (isShiprushExportChecked)
                    {
                        try
                        {
                            string fileName = "Shiprush.txt";
                            //DataTable tempOrderItems = new DataTable();
                            string pathToSave = alternateFullPath;

                            if (!Directory.Exists(pathToSave))
                            {
                                MVVMMessageService.ShowMessage(errorMessages.PATH_DOES_NOT_EXISTS);
                                return;
                            }

                            string filePath = pathToSave + "\\" + fileName;


                            DataTable dtRes2 = GetContentAsDataTable();
                            CreateExcelFile.CreateExcelDocument(dtRes2, filePath.Replace("txt", "xlsx"));

                            CreateExcelFile.convertXLSXToXLS(filePath.Replace("txt", "xls"), filePath.Replace("txt", "xlsx"));
                            //if(File.Exists(filePath.Replace("txt", "xlsx")))
                            //{
                            //    File.Delete(filePath.Replace("txt", "xlsx"));
                            //}
                            bool isError = false;
                            if (!isError)
                            {
                                // File.Copy(filePath.Replace("txt", "xlsx"), backupFilePath, true); //create one more file as back up so neal can work with original file
                                string message1 = " Data file created successfully.\n Do you want to open the file path?";//errorMessages.GET_EXPORT_DATA1 + tempOrderItems.Rows.Count + errorMessages.GET_EXPORT_DATA2;
                                string caption1 = "Confirmation";
                                System.Windows.MessageBoxButton buttons1 = System.Windows.MessageBoxButton.YesNo;
                                System.Windows.MessageBoxImage iconn1 = System.Windows.MessageBoxImage.Question;
                                if (MVVMMessageService.ShowMessage(message1, caption1, buttons1, iconn1) == System.Windows.MessageBoxResult.Yes)
                                {
                                    Process.Start(pathToSave);
                                }
                                DialogResult = false;
                            }
                            else
                            {
                                MVVMMessageService.ShowMessage(errorMessages.EXPORT_WITH_ERRORS);
                            }
                        }
                        catch (Exception ex)
                        {
                            MVVMMessageService.ShowMessage(ex.Message);
                            clsStatic.WriteExceptionLogXML(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MessageBox.Show(ex.Message);
            }
        }

        public DataTable GetContentAsDataTable(bool IgnoreHideColumns = false)
        {
            try
            {
                DataTable tempTable = new DataTable();
                tempTable = clsOrders.getOrderDetailsbyOrderID(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempSelectedOrderIds);
                decimal? tempWeight = 0; decimal? weight = 0; decimal? weightOunce = 0; int i = 0;

                if (tempTable.Columns.Count == 0) return null;
                DataTable dtSource = new DataTable();
                dtSource.Columns.Add("SimplePhotoOrderId");
                dtSource.Columns.Add("OrderID");
                dtSource.Columns.Add("Contact");
                dtSource.Columns.Add("Company");
                dtSource.Columns.Add("Address1");
                dtSource.Columns.Add("Phone");
                dtSource.Columns.Add("City");
                dtSource.Columns.Add("State");
                dtSource.Columns.Add("PostalCode");
                dtSource.Columns.Add("Email");
                dtSource.Columns.Add("Weight-Ounces");
                dtSource.Columns.Add("TotalWeight");
                dtSource.Columns.Add("TrackingNumber");
                dtSource.Columns.Add("isShipped");
                foreach (DataRow row in tempTable.Rows)
                {
                    i++; tempWeight = 0; weight = 0; weightOunce = 0;
                    List<StudentPhotoOrder> tempItemsTable = new List<StudentPhotoOrder>();
                    //tempItemsTable = clsOrders.getGetAllOrderItems(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(row["OrderID"]));
                    tempItemsTable = (from oi in db.StudentPhotoOrders where oi.OrderId == Convert.ToInt32(row["OrderID"]) select oi).ToList();
                    foreach (StudentPhotoOrder soi in tempItemsTable)
                    {
                        if (soi.sp_SimplePhotoBillingCode != null)
                        {
                            OrderPackage tempPackage = (from op in db.OrderPackages where op.SimplePhotoItemId == soi.sp_SimplePhotoBillingCode select op).FirstOrDefault();

                            if (tempPackage != null)
                            {
                                if (tempPackage.Weight != null)
                                    tempWeight += (tempPackage.Weight * Convert.ToInt32((string.IsNullOrEmpty(soi.Quantity.ToString()) ? 0 : soi.Quantity)));
                                else if (tempWeight == null)
                                    tempWeight = 0;
                            }
                        }
                    }
                    tempWeight = Convert.ToDecimal(tempWeight);
                    weightOunce = tempWeight + Convert.ToDecimal(3.4);
                    weight = Convert.ToDecimal(weightOunce / 16);

                    DataRow drNewRow = dtSource.NewRow();

                    foreach (DataColumn col in dtSource.Columns)
                    {
                        if (col.ColumnName == "SimplePhotoOrderId")
                            drNewRow["SimplePhotoOrderId"] = row["SimplePhotoOrderID"];
                        if (col.ColumnName == "OrderID")
                            drNewRow["OrderID"] = row["OrderId"];
                        else if (col.ColumnName == "Contact")
                            drNewRow["Contact"] = row["Ship_FirstName"] + " " + row["Ship_LastName"];
                        else if (col.ColumnName == "Company")
                            drNewRow["Company"] = "";
                        else if (col.ColumnName == "Address1")
                            drNewRow["Address1"] = row["Address1"];
                        else if (col.ColumnName == "Phone")
                            drNewRow["Phone"] = row["Phone"];
                        else if (col.ColumnName == "City")
                            drNewRow["City"] = row["City"];
                        else if (col.ColumnName == "State")
                            drNewRow["State"] = row["State"];
                        else if (col.ColumnName == "PostalCode")
                            drNewRow["PostalCode"] = row["PostalCode"];
                        else if (col.ColumnName == "Email")
                            drNewRow["Email"] = row["EmailAddress"];
                        else if (col.ColumnName == "Weight-Ounces")
                            drNewRow["Weight-Ounces"] = weightOunce;
                        else if (col.ColumnName == "TotalWeight")
                            drNewRow["TotalWeight"] = weight;
                        else if (col.ColumnName == "TrackingNumber")
                            drNewRow["TrackingNumber"] = row["TrackingNumber"];
                        else if (col.ColumnName == "isShipped")
                            drNewRow["isShipped"] = ((row["isShipped"] == "True" || row["isShipped"] == "true") ? "TRUE" : "FALSE");
                    }
                    dtSource.Rows.Add(drNewRow);

                    //if (i == tempTable.Rows.Count)
                    //{
                    //    DataRow drNewRow1 = dtSource.NewRow();
                    //    totalWeight = Math.Ceiling(Convert.ToDecimal(totalWeight));
                    //    drNewRow1["TotalWeight"] = "Total Weight = " + totalWeight;
                    //    dtSource.Rows.Add(drNewRow1);
                    //}
                }
                return dtSource;
            }
            catch { return null; }
        }

        private void writeDataToFile()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            FileInfo fi = new FileInfo(alternateFullPath + "DataFile.txt");
            DirectoryInfo di = new DirectoryInfo(alternateFullPath);
            if (!di.Exists)
            {
                di.Create();
            }
            if (isExportAllImagesChecked == true)
                arrStudentImageIds = VisibleData.Count == 0 ? clsGroup.GetStudentImgIdOnMultiGroupid(db, arrGroupId) : temparrStudents;
            createTextFile();

            if (MessageBox.Show(errorMessages.CREATED_DATA_FILE, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process.Start(alternateFullPath);
            }
        }

        private void browseAlternateFolder()
        {
            try
            {
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                var res = dlg.ShowDialog();
                if (res != false)
                    alternateFullPath = dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        public void bindMaskComboBox()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                masksData = clsDashBoard.getMasks(db, schoolId);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method used to create folder based on grade
        /// </summary>


        #region PSPA and Simple Photo
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
        /// </summary>
        /// <returns></returns>
        async Task createFolderForGrade()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                String _imageName = "";
                string _imagenamenew = "";
                string _studentid = "";
                string _teacher = "";
                string _studentFirstName = "", _studentLastName = "", _studentGrade = "";
                string _sequentialnumber = "";
                String _destinationPath = "";
                int i = 0;
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    _objWaitCursorViewModel = new WaitCursorViewModel();
                });
                //_objWaitCursorViewModel = new WaitCursorViewModel();
                StreamWriter sw = new StreamWriter(alternateFullPath + "\\test1238.txt", true);
                StreamWriter sw1 = new StreamWriter(alternateFullPath + "\\test1239.txt", true);

                List<GroupItem> _objGroupItems = null;
                List<GroupItem> _objClassPhotosToGroupItem = new List<GroupItem>();

                #region ExportAllImagesChecked
                //worker.DoWork += delegate(object s, DoWorkEventArgs args)
                //{
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
                await Task.Run(() =>
                {
                    if (isExportAllImagesChecked == true)
                    {
                        //foreach (int groupId in arrGroupId)
                        //{
                        #region  new code
                        _objGroupItems = VisibleData.Count == 0 ? clsGroup.getstudentImagesByGroupPSPA(db, arrGroupId) : VisibleData;

                        List<GroupClassPhoto> _objGroupClassPhotos = clsGroup.getClassPhotoByGroupPSPA(db, arrGroupId);       //getting class photos by group id

                        foreach (GroupClassPhoto classPhoto in _objGroupClassPhotos)    //adding class photos to group items ,so that we can export along with group items
                        {
                            GroupItem _objGroupItem = new GroupItem();
                            _objGroupItem.StudentImage = classPhoto.StudentImage;

                            _objClassPhotosToGroupItem.Add(_objGroupItem);
                        }
                        _objGroupItems.AddRange(_objClassPhotosToGroupItem);
                        _objGroupItems = _objGroupItems.OrderBy(x => x.StudentImage.Student.Grade).ToList();

                        maxValue = _objGroupItems.Count();
                        #endregion

                        try
                        {
                            foreach (GroupItem objGroupItem in _objGroupItems)
                            {

                                if (isProgressCancelled)
                                {
                                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                    string message = "Are you sure, you want to cancel the operation?";
                                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                    {
                                        ///args.Cancel = true;
                                        DialogResult = false;
                                        return;
                                    }
                                    isProgressCancelled = false;
                                }
                                currentProgress++;

                                _imageName = objGroupItem.StudentImage.ImageName;
                                _studentid = Convert.ToString(objGroupItem.StudentImage.Student.StudentID);
                                _studentFirstName = objGroupItem.StudentImage.Student.FirstName;
                                _studentLastName = objGroupItem.StudentImage.Student.Lastname;
                                _teacher = objGroupItem.StudentImage.Student.Teacher;
                                if (objGroupItem.StudentImage.Student.Grade != "" && objGroupItem.StudentImage.Student.Grade != null)
                                    _studentGrade = objGroupItem.StudentImage.Student.Grade;
                                else
                                    _studentGrade = "No Grade";
                                _sequentialnumber = _imageName.Substring(_imageName.LastIndexOf('_') + 1);
                                ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder;

                                newPath = objGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_reduced\\";
                                ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                if (isReducedImagesChecked == true)
                                {
                                    if (Directory.Exists(newPath + "\\" + _imageName))
                                        ImageFolder = newPath;
                                    else
                                        ImageFolder = newPath;
                                }
                                else if (isDigitalImagesChecked)
                                {
                                    ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_DD\\";

                                    bool isFolderNotFound = digitalImageFolderNotFound();
                                    if (isFolderNotFound)
                                    {
                                        isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                }
                                jobName = objGroupItem.StudentImage.PhotoShoot.PhotographyJob.JobName;
                                _destinationPath = alternateFullPath;


                                if (Directory.Exists(_destinationPath))
                                {
                                    # region Add photos as per their Grade
                                    try
                                    {
                                        if (File.Exists(ImageFolder + "\\" + _imageName))
                                        {
                                            #region PSPA
                                            if (isPSPAChecked)
                                            {
                                                _imagenamenew = _studentLastName + "_" + _studentFirstName + "_" + _sequentialnumber;
                                                if (!Directory.Exists(_destinationPath + "\\" + _studentGrade))
                                                {
                                                    Directory.CreateDirectory(_destinationPath + "\\" + _studentGrade);

                                                    sw.Close();
                                                    sw = new StreamWriter(_destinationPath + "\\" + _studentGrade + "\\" + _studentGrade + ".txt", true);

                                                    File.Copy(ImageFolder + "\\" + _imageName, _destinationPath + "\\" + _studentGrade + "\\" + _imagenamenew, true);
                                                }
                                                else
                                                    File.Copy(ImageFolder + "\\" + _imageName, _destinationPath + "\\" + _studentGrade + "\\" + _imagenamenew, true);

                                                sw.WriteLine("YearBook" + "\t" + _studentGrade + "\t" + _imagenamenew + "\t" + _studentGrade + "\t" + _studentLastName + "\t" + _studentFirstName + "\t" + _teacher);

                                                #region Index.txt file
                                                if (i == 0)
                                                {
                                                    i++;
                                                    sw1.Close();
                                                    sw1 = new StreamWriter(alternateFullPath + "\\" + "Index" + ".txt", true);
                                                }
                                                sw1.WriteLine("YearBook" + "\t" + _destinationPath + "\\" + _studentGrade + "\t" + _studentGrade + "\t" + _imagenamenew + "\t" + _studentGrade + "\t" + _studentLastName + "\t" + _studentFirstName + "\t" + _teacher);
                                                #endregion
                                            }
                                            #endregion
                                            isProgressBarVisible = Visibility.Visible;

                                        }

                                        # region Add Group Photo in every grade folder //#Commented

                                        //IEnumerable<GroupClassPhoto> _objGroupClassPhotos = clsGroup.getClassPhotoByGroup(db, Convert.ToInt32(objGroupItem.GroupID));
                                        //foreach (GroupClassPhoto _objClsPht in _objGroupClassPhotos)
                                        //{
                                        //    try
                                        //    {
                                        //        string Imagenamegrp = _objClsPht.StudentImage.ImageName;
                                        //        _studentFirstName = _objClsPht.StudentImage.Student.FirstName;
                                        //        _studentLastName = _objClsPht.StudentImage.Student.Lastname;
                                        //        _sequentialnumber = Imagenamegrp.Substring(Imagenamegrp.LastIndexOf('_') + 1);

                                        //        if (isReducedImagesChecked != true)
                                        //        {
                                        //            ImageFolder = _objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                        //        }

                                        //        if (File.Exists(ImageFolder + "\\" + Imagenamegrp))
                                        //        {
                                        //            _imagenamenew = _studentLastName + "_" + _studentFirstName + "_" + _sequentialnumber;
                                        //            if (!Directory.Exists(_destinationPath + "\\" + _studentGrade))
                                        //            {
                                        //                Directory.CreateDirectory(_destinationPath + "\\" + _studentGrade);
                                        //                File.Copy(ImageFolder + "\\" + Imagenamegrp, _destinationPath + "\\" + _studentGrade + "\\" + _imagenamenew, true);
                                        //            }
                                        //            else
                                        //                File.Copy(ImageFolder + "\\" + Imagenamegrp, _destinationPath + "\\" + _studentGrade + "\\" + _imagenamenew, true);
                                        //            isProgressBarVisible = Visibility.Visible;
                                        //        }

                                        //    }
                                        //    catch (Exception ex)
                                        //    {
                                        //        clsStatic.WriteExceptionLogXML(ex);
                                        //        MVVMMessageService.ShowMessage(ex.Message);
                                        //    }
                                        //}
                                        # endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        clsStatic.WriteExceptionLogXML(ex);
                                        MVVMMessageService.ShowMessage(ex.Message);
                                        return;
                                    }
                                    # endregion
                                }
                                else
                                {
                                    MessageBox.Show(errorMessages.ALTERNATIVE_PATH_DOES_NOT_EXIST);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                            MVVMMessageService.ShowMessage(ex.Message);
                            return;
                        }
                        finally
                        {
                            sw.Close();
                            sw1.Close();
                            File.Delete(alternateFullPath + "\\test1238.txt");
                            File.Delete(alternateFullPath + "\\test1239.txt");
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                _objWaitCursorViewModel.Dispose();
                            });
                        }
                        //createTextFile();

                        string messagecl = errorMessages.AFTER_COPY_OPEN_LOCATION_CONFIRMATION;
                        string captioncl = "Confirmation";
                        MessageBoxButton buttonss = MessageBoxButton.YesNo;
                        MessageBoxImage iconn = MessageBoxImage.Question;
                        if (MessageBox.Show(messagecl, captioncl, buttonss, iconn) == MessageBoxResult.Yes)
                            Process.Start(alternateFullPath);

                        isSave = true;
                        DialogResult = false;
                    }
                #endregion

                    #region else
                    else
                    {
                        DataTable _objStudentImageDetails = new DataTable();
                        _objStudentImageDetails = clsDashBoard.getImageNameLastNameFirstName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentImageIds);
                        int count = _objStudentImageDetails.Rows.Count * 2;
                        int k = 0;

                        DataView tempGroupItemsDataView = _objStudentImageDetails.DefaultView;
                        tempGroupItemsDataView.Sort = "Grade";
                        DataTable tempGroupItemsData = tempGroupItemsDataView.ToTable();

                        //tempGroupItemsData.Rows.Count changed to arrStudentImageIds.Count() //#Mohan //29-5-2015
                        //Orders can have duplicate student images i.e, one student image can be present in more than one order
                        //so instead of looping through results ,loop through all student image ids
                        //it won't be a problem for group export because , in group export "arrStudentImageIds" will insert only unique student image ids
                        maxValue = tempGroupItemsData.Rows.Count;
                        for (k = 0; k < tempGroupItemsData.Rows.Count; k++)
                        {
                            try
                            {
                                //DataRow dr = _objStudentImageDetails.Rows
                                //    .Cast<DataRow>()
                                //    .Where(x => Convert.ToInt32(x["ID"]) == arrStudentImageIds[k]).FirstOrDefault();

                                if (isProgressCancelled)
                                {
                                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                    string message = "Are you sure, you want to cancel the operation?";
                                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                    {
                                        ///args.Cancel = true;
                                        DialogResult = false;
                                        return;
                                    }
                                    isProgressCancelled = false;
                                }
                                currentProgress++;


                                DataRow dr = tempGroupItemsData.Rows[k];

                                _teacher = Convert.ToString(dr["Teacher"]);
                                _imageName = Convert.ToString(dr["ImageName"]);
                                _studentid = Convert.ToString(dr["StudentIDPk"]);
                                _studentFirstName = Convert.ToString(dr["FirstName"]);
                                _studentLastName = Convert.ToString(dr["LastName"]);
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Grade"])))
                                    _studentGrade = Convert.ToString(dr["Grade"]);
                                else
                                    _studentGrade = "No Grade";
                                _sequentialnumber = _imageName.Substring(_imageName.LastIndexOf('_') + 1);
                                int PhotoId = Convert.ToInt32(dr["PhotoShootID"]);
                                objImageFolder = clsDashBoard.getImageFolder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), PhotoId);
                                ImageFolder = objImageFolder.ImageFolder;

                                newPath = objImageFolder.ImageFolder + "\\_reduced\\";
                                ImageFolder = objImageFolder.ImageFolder;

                                if (isReducedImagesChecked == true)
                                {
                                    if (File.Exists(newPath + "\\" + _imageName))
                                        ImageFolder = newPath;
                                    else
                                        ImageFolder = newPath;
                                }
                                else if (isDigitalImagesChecked)
                                {
                                    ImageFolder = objImageFolder.ImageFolder + "\\_DD\\";

                                    bool isFolderNotFound = digitalImageFolderNotFound();
                                    if (isFolderNotFound)
                                    {
                                        isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    ImageFolder = objImageFolder.ImageFolder;
                                }
                                jobName = objImageFolder.PhotographyJob.JobName;

                                _destinationPath = alternateFullPath;
                                if (Directory.Exists(_destinationPath))
                                {
                                    # region Add phots as per their Grade
                                    try
                                    {
                                        if (File.Exists(ImageFolder + "\\" + _imageName))
                                        {
                                            _imagenamenew = _studentLastName + "_" + _studentFirstName + "_" + _sequentialnumber;
                                            if (!Directory.Exists(_destinationPath + "\\" + _studentGrade))
                                            {
                                                Directory.CreateDirectory(_destinationPath + "\\" + _studentGrade);

                                                sw.Close();
                                                sw = new StreamWriter(_destinationPath + "\\" + _studentGrade + "\\" + _studentGrade + ".txt", true);

                                                File.Copy(ImageFolder + "\\" + _imageName, _destinationPath + "\\" + _studentGrade + "\\" + _imagenamenew, true);
                                            }
                                            else
                                                File.Copy(ImageFolder + "\\" + _imageName, _destinationPath + "\\" + _studentGrade + "\\" + _imagenamenew, true);

                                            sw.WriteLine("YearBook" + "\t" + _studentGrade + "\t" + _imagenamenew + "\t" + _studentGrade + "\t" + _studentLastName + "\t" + _studentFirstName + "\t" + _teacher);

                                            #region Index.txt
                                            if (i == 0)
                                            {
                                                i++;
                                                sw1.Close();
                                                sw1 = new StreamWriter(alternateFullPath + "\\" + "Index" + ".txt", true);
                                            }
                                            sw1.WriteLine("YearBook" + "\t" + _destinationPath + "\\" + _studentGrade + "\t" + _studentGrade + "\t" + _imagenamenew + "\t" + _studentGrade + "\t" + _studentLastName + "\t" + _studentFirstName + "\t" + _teacher);
                                            #endregion
                                            isProgressBarVisible = Visibility.Visible;
                                        }
                                    # endregion

                                    }
                                    catch (Exception ex)
                                    {
                                        clsStatic.WriteExceptionLogXML(ex);
                                        MVVMMessageService.ShowMessage(ex.Message);
                                        return;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(errorMessages.ALTERNATIVE_PATH_DOES_NOT_EXIST);
                                }
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                                return;
                            }
                        }

                        sw.Close();
                        sw1.Close();
                        File.Delete(alternateFullPath + "\\test1238.txt");
                        File.Delete(alternateFullPath + "\\test1239.txt");
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            _objWaitCursorViewModel.Dispose();
                        });
                        //createTextFile();
                        string messagecl = errorMessages.AFTER_COPY_OPEN_LOCATION_CONFIRMATION;
                        string captioncl = "Confirmation";
                        MessageBoxButton buttonss = MessageBoxButton.YesNo;
                        MessageBoxImage iconn = MessageBoxImage.Question;
                        if (MessageBox.Show(messagecl, captioncl, buttonss, iconn) == MessageBoxResult.Yes)
                            Process.Start(alternateFullPath);

                        isSave = true;
                        DialogResult = false;

                    }
                    #endregion

                });

                PSPAAsyncCompleted();

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        async Task createFolderForSPGallery()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                String _imageName = "";
                string _imagenamenew = "";
                string _studentid = "";
                string _teacher = "";
                string _studentFirstName = "", _studentLastName = "", _studentGrade = "";
                string _sequentialnumber = "";
                String _destinationPath = ""; string studentID = "";
                List<string> tempGroupImagesNames = new List<string>();
                List<GroupItem> ImagesGroupsByStudent = new List<GroupItem>();

                string path = AppDomain.CurrentDomain.BaseDirectory;

                //string test1238 = (string.IsNullOrEmpty(alternateFullPath) ? path : alternateFullPath) + "\\test1238.txt";
                //string test1239 = (string.IsNullOrEmpty(alternateFullPath) ? path : alternateFullPath) + "\\test1239.txt";
                string test1240 = (string.IsNullOrEmpty(alternateFullPath) ? path : alternateFullPath) + "\\test1240.xml";


                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    _objWaitCursorViewModel = new WaitCursorViewModel();
                });

                //StreamWriter sw = new StreamWriter(test1238, true);
                //StreamWriter sw1 = new StreamWriter(test1239, true);

                FileStream fs = new FileStream(test1240, FileMode.Create);
                System.Xml.XmlTextWriter w = new System.Xml.XmlTextWriter(fs, Encoding.UTF8);

                List<GroupItem> _objGroupItems = null;
                List<GroupItem> _objClassPhotosToGroupItem = new List<GroupItem>();

                #region ExportAllImagesChecked
                if (isProgressCancelled)
                {
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    string message = "Are you sure, you want to cancel the operation?";
                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        DialogResult = false;
                        return;
                    }
                    isProgressCancelled = false;
                }
                await Task.Run(() =>
                {
                    //using(TransactionScope ts = new TransactionScope())
                    //{
                    //    using (PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString))
                    //    {
                    #region Condition A

                    #region export images and creating xml file
                    if (isExportAllImagesChecked == true || isExportSelectedChecked)
                    {
                        // #region  new code
                        if (isExportAllImagesChecked)
                        {
                            _objGroupItems = VisibleData.Count == 0 ? clsGroup.getstudentImagesByGroupPSPA(db, arrGroupId) : VisibleData;

                        }
                        else
                        {
                            _objGroupItems = new List<GroupItem>();
                            _objGroupItems = clsGroup.getstudentSelectyedImagesByGroupSP(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(arrGroupId[0]), arrStudentImageIds);
                        }

                        #region Group class photos
                        List<GroupClassPhoto> _objGroupClassPhotos = clsGroup.getClassPhotoByGroupPSPA(db, arrGroupId);       //getting class photos by group id

                        foreach (GroupClassPhoto classPhoto in _objGroupClassPhotos)    //adding class photos to group items ,so that we can export along with group items
                        {
                            GroupItem _objGroupItem = new GroupItem();
                            _objGroupItem.StudentImage = classPhoto.StudentImage;
                            _objGroupItem.GroupID = classPhoto.GroupId;
                            _objClassPhotosToGroupItem.Add(_objGroupItem);
                        }
                        //_objGroupItems.AddRange(_objClassPhotosToGroupItem);
                        #endregion


                        #region Table SimplePhotoExportBatch
                        db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                        SimplePhotoExportBatch _objSimplePhotoExportBatch = new SimplePhotoExportBatch();
                        _objSimplePhotoExportBatch.ExportType = isExportFTPChecked == true ? "FTP" : "Folder";
                        _objSimplePhotoExportBatch.ExportPath = isExportFTPChecked == true ? "ftp://ftp2.digitaleventpics.com" : alternateFullPath;
                        _objSimplePhotoExportBatch.CreatedOn = DateTime.Now;
                        _objSimplePhotoExportBatch.CreatedBy = clsStatic.userName;
                        _objSimplePhotoExportBatch.GroupsExported = arrGroupId[0].ToString();
                        _objSimplePhotoExportBatch.NoOfImages = (_objGroupItems.Count + _objGroupClassPhotos.Count);
                        db.SimplePhotoExportBatches.InsertOnSubmit(_objSimplePhotoExportBatch);
                        db.SubmitChanges();

                        txtSPOrderNumber = _objSimplePhotoExportBatch.Id.ToString();
                        #endregion

                        _objGroupItems = _objGroupItems.OrderBy(x => x.StudentImage.Student.Grade).ToList();

                        maxValue = _objGroupItems.Count();

                        _objGroupItems = _objGroupItems.OrderByDescending(x => x.StudentImage.StudentIDPK).ToList();

                        //#endregion

                        try
                        {
                            #region
                            _objGroupItems = _objGroupItems.OrderByDescending(x => x.StudentImage.StudentIDPK).ToList();
                            ArrayList groupClassImageIds = new ArrayList();
                            foreach (GroupItem objGroupItem in _objGroupItems)
                            {
                                int? groupId = objGroupItem.GroupID;

                                if (isProgressCancelled)
                                {
                                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                    string message = "Are you sure, you want to cancel the operation?";
                                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                    {
                                        DialogResult = false;
                                        return;
                                    }
                                    isProgressCancelled = false;
                                }
                                currentProgress++;

                                _imageName = objGroupItem.StudentImage.ImageName;
                                _studentid = Convert.ToString(objGroupItem.StudentImage.Student.StudentID);
                                _studentFirstName = objGroupItem.StudentImage.Student.FirstName;
                                _studentLastName = objGroupItem.StudentImage.Student.Lastname;
                                _teacher = objGroupItem.StudentImage.Student.Teacher;
                                if (objGroupItem.StudentImage.Student.Grade != "" && objGroupItem.StudentImage.Student.Grade != null)
                                    _studentGrade = objGroupItem.StudentImage.Student.Grade;
                                else
                                    _studentGrade = "No Grade";
                                _sequentialnumber = _imageName.Substring(_imageName.LastIndexOf('_') + 1);
                                ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder;

                                newPath = objGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_reduced\\";
                                ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                if (isReducedImagesChecked == true)
                                {
                                    if (Directory.Exists(newPath + "\\" + _imageName))
                                        ImageFolder = newPath;
                                    else
                                        ImageFolder = newPath;
                                }
                                else if (isDigitalImagesChecked)
                                {
                                    ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_DD\\";

                                    bool isFolderNotFound = digitalImageFolderNotFound();
                                    if (isFolderNotFound)
                                    {
                                        isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                }
                                jobName = objGroupItem.StudentImage.PhotoShoot.PhotographyJob.JobName;
                                if (isExportFTPChecked)
                                {
                                    _destinationPath = path + "\\SPFTPExport" + "\\" + txtSPOrderNumber; //_destinationPath = path + "\\SPFTPExport";
                                    createdNewPath = path + "\\SPFTPExport"; //createdNewPath = path + "\\SPFTPExport";
                                    zipfilepath = path + "\\SPFTPExport" + "\\" + txtSPOrderNumber; //zipfilepath = _destinationPath + "\\" + txtSPOrderNumber;
                                    if (!Directory.Exists(_destinationPath))
                                    {
                                        Directory.CreateDirectory(_destinationPath);
                                        // create group photos here inside each folder
                                    }
                                }
                                else
                                    _destinationPath = alternateFullPath;


                                if (Directory.Exists(_destinationPath))
                                {
                                    # region Add photos as per their Grade
                                    try
                                    {
                                        if (File.Exists(ImageFolder + "\\" + _imageName))
                                        {
                                            isMissiedImages = false;
                                            #region SimplePhoto FTP
                                            if (isSimplePhotoFTPChecked)
                                            {
                                                //int ii = 1;
                                                if (isExportFolderChecked)
                                                {
                                                    if (isAppendGPPwdChecked == true && exportType == "GotPhoto")          // if Append Password is selected
                                                    {
                                                        password = Convert.ToString(objGroupItem.StudentImage.Student.Password);
                                                        if (password != "" && password != null)
                                                        {
                                                            password = "~" + password;
                                                        }
                                                    }
                                                }
                                                if (isExportFTPChecked)
                                                    alternateFullPath = _destinationPath;
                                                if (isMackUpPhotoshootChecked)
                                                {
                                                    if (isExportFolderChecked)
                                                    {
                                                        if (isAppendGPPwdChecked == true && exportType == "GotPhoto")          // if Append Password is selected
                                                        {
                                                            _destinationPath = _destinationPath + "\\" + txtSPOrderNumber + "\\" + objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + password + "_MU";
                                                        }
                                                    }
                                                    else
                                                        _destinationPath = _destinationPath + "\\" + txtSPOrderNumber + "\\" + objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + "_MU";

                                                }
                                                else
                                                {
                                                    if (isExportFolderChecked)
                                                    {
                                                        if (isAppendGPPwdChecked == true && exportType == "GotPhoto")          // if Append Password is selected
                                                        {
                                                            _destinationPath = _destinationPath + "\\" + txtSPOrderNumber + "\\" + objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + password;
                                                        }
                                                        else
                                                            _destinationPath = _destinationPath + "\\" + txtSPOrderNumber + "\\" + objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim();
                                                    }
                                                    else
                                                        _destinationPath = _destinationPath + "\\" + txtSPOrderNumber + "\\" + objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim();
                                                }
                                                if (!isUseFileNameFromDriveChecked)
                                                    _imagenamenew = _studentLastName + "_" + _studentFirstName + "_" + _sequentialnumber;
                                                else
                                                    _imagenamenew = objGroupItem.StudentImage.ImageName;

                                                if (string.IsNullOrEmpty(studentID) || studentID == objGroupItem.StudentImage.StudentIDPK.ToString())
                                                {
                                                    studentID = objGroupItem.StudentImage.StudentIDPK.ToString();
                                                    if (!Directory.Exists(_destinationPath))
                                                    {
                                                        groupClassImageIds = new ArrayList();
                                                        Directory.CreateDirectory(_destinationPath);

                                                        File.Copy(ImageFolder + "\\" + _imageName, _destinationPath + "\\" + _imagenamenew, true);
                                                        //if (!isExportFTPChecked)
                                                        //fs = new FileStream(alternateFullPath + "\\" + txtSPOrderNumber + "\\" + txtSPOrderNumber + ".xml", FileMode.Create);
                                                        fs = new FileStream(alternateFullPath + "\\" + txtSPOrderNumber + ".xml", FileMode.Create);
                                                        //else
                                                        // fs = new FileStream(alternateFullPath + "\\" + txtSPOrderNumber + "\\" + txtSPOrderNumber + ".xml", FileMode.Create);
                                                        w.Flush(); w.Close();
                                                        w = new System.Xml.XmlTextWriter(fs, Encoding.UTF8);

                                                        w.Formatting = System.Xml.Formatting.Indented;
                                                        w.WriteStartDocument();
                                                        w.WriteStartElement("export");
                                                        w.WriteAttributeString("photographerID", _objSchool.Studio == null ? "" : (string.IsNullOrEmpty(_objSchool.Studio.StudioName) ? "" : _objSchool.Studio.StudioName));
                                                        w.WriteAttributeString("username", _objSchool.Studio == null ? "" : (string.IsNullOrEmpty(_objSchool.Studio.PrimaryContact) ? "" : _objSchool.Studio.PrimaryContact));
                                                        w.WriteAttributeString("orderNumber", txtSPOrderNumber);
                                                        w.WriteAttributeString("groupTitle", txtSPTitle);
                                                        w.WriteAttributeString("shootDate", dpSPDate.Value.ToShortDateString());
                                                        w.WriteStartElement("gallery");
                                                        w.WriteAttributeString("password", objGroupItem.StudentImage.Student == null ? "" : (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Password) ? "" : objGroupItem.StudentImage.Student.Password.ToString()));
                                                        if (!isMackUpPhotoshootChecked)
                                                        {
                                                            w.WriteAttributeString("galleryName", objGroupItem.StudentImage.Student == null ? "" : (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim())));
                                                        }
                                                        else
                                                        {
                                                            w.WriteAttributeString("galleryName", objGroupItem.StudentImage.Student == null ? "" : (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + "_MU")));
                                                        }
                                                        //w.WriteAttributeString("email", "");
                                                        w.WriteAttributeString("email", objGroupItem.StudentImage.Student == null ? "" : (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Emailaddress) ? "" : (objGroupItem.StudentImage.Student.Emailaddress.Trim() + (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Address) ? "" : "," + objGroupItem.StudentImage.Student.Address))));
                                                        w.WriteStartElement("image");
                                                        w.WriteAttributeString("name", _imagenamenew);
                                                        if (!isMackUpPhotoshootChecked)
                                                            w.WriteAttributeString("roll", objGroupItem.StudentImage.Student == null ? "" : (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim())));
                                                        else
                                                            w.WriteAttributeString("roll", objGroupItem.StudentImage.Student == null ? "" : (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + "_MU")));
                                                        w.WriteAttributeString("width", "");
                                                        w.WriteAttributeString("height", "");
                                                        w.WriteEndElement();

                                                        #region for group class photos
                                                        foreach (GroupItem clsGroupItem in _objClassPhotosToGroupItem)
                                                        {
                                                            if (groupId != clsGroupItem.GroupID)
                                                            {
                                                                continue;
                                                            }
                                                            groupClassImageIds.Add(clsGroupItem.StudentPhotoID);
                                                            string ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;

                                                            newPath = clsGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_reduced\\";
                                                            ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                                            if (isReducedImagesChecked == true)
                                                            {
                                                                if (Directory.Exists(newPath + "\\" + _imageName))
                                                                    ImageFolder1 = newPath;
                                                                else
                                                                    ImageFolder1 = newPath;
                                                            }
                                                            else if (isDigitalImagesChecked)
                                                            {
                                                                ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_DD\\";

                                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                                if (isFolderNotFound)
                                                                {
                                                                    isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                                                                    return;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                                            }

                                                            string imageName = clsGroupItem.StudentImage.ImageName;

                                                            if (File.Exists(ImageFolder1 + "\\" + imageName))
                                                            {
                                                                //imageName;
                                                                string imagenamenew = "";
                                                                if (!isUseFileNameFromDriveChecked)
                                                                    imagenamenew = clsGroupItem.StudentImage.Student.Lastname + "_" + clsGroupItem.StudentImage.Student.FirstName + "_" + imageName.Substring(imageName.LastIndexOf('_') + 1);
                                                                else
                                                                    imagenamenew = clsGroupItem.StudentImage.ImageName;

                                                                tempImage = imagenamenew;
                                                                tempDestinationPath = _destinationPath;
                                                                if (exportType == "GotPhoto")
                                                                {
                                                                    if (!Directory.Exists(_destinationPath + "\\Groups"))
                                                                        Directory.CreateDirectory(_destinationPath + "\\Groups");

                                                                    File.Copy(ImageFolder1 + "\\" + imageName, _destinationPath + "\\Groups" + "\\" + imagenamenew, true);
                                                                }
                                                                else
                                                                    File.Copy(ImageFolder1 + "\\" + imageName, _destinationPath + "\\" + imagenamenew, true);
                                                                w.WriteStartElement("image");
                                                                w.WriteAttributeString("name", imagenamenew);
                                                                if (!isMackUpPhotoshootChecked)
                                                                    w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.ToString() + "_" + objGroupItem.StudentImage.Student.FirstName.ToString()));
                                                                else
                                                                    w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.ToString() + "_" + objGroupItem.StudentImage.Student.FirstName.ToString() + "_MU"));
                                                                w.WriteAttributeString("width", "");
                                                                w.WriteAttributeString("height", "");
                                                                w.WriteEndElement();
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        File.Copy(ImageFolder + "\\" + _imageName, _destinationPath + "\\" + _imagenamenew, true);
                                                        w.WriteStartElement("image");
                                                        w.WriteAttributeString("name", _imagenamenew);
                                                        if (!isMackUpPhotoshootChecked)
                                                            w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim()));
                                                        else
                                                            w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + "_MU"));
                                                        w.WriteAttributeString("width", "");
                                                        w.WriteAttributeString("height", "");
                                                        w.WriteEndElement();


                                                        #region for group class photos
                                                        foreach (GroupItem clsGroupItem in _objClassPhotosToGroupItem)
                                                        {

                                                            if (groupClassImageIds.Contains(clsGroupItem.StudentPhotoID))
                                                                continue;
                                                            else if (groupId != clsGroupItem.GroupID)
                                                            {
                                                                continue;
                                                            }
                                                            groupClassImageIds.Add(clsGroupItem.StudentPhotoID);
                                                            string ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;

                                                            newPath = clsGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_reduced\\";
                                                            ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                                            if (isReducedImagesChecked == true)
                                                            {
                                                                if (Directory.Exists(newPath + "\\" + _imageName))
                                                                    ImageFolder1 = newPath;
                                                                else
                                                                    ImageFolder1 = newPath;
                                                            }
                                                            else if (isDigitalImagesChecked)
                                                            {
                                                                ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_DD\\";

                                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                                if (isFolderNotFound)
                                                                {
                                                                    isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                                                                    return;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                                            }

                                                            string imageName = clsGroupItem.StudentImage.ImageName;

                                                            if (File.Exists(ImageFolder1 + "\\" + imageName))
                                                            {
                                                                string imagenamenew = "";// clsGroupItem.StudentImage.Student.Lastname.Trim() + "_" + clsGroupItem.StudentImage.Student.FirstName.Trim() + "_" + imageName.Substring(imageName.LastIndexOf('_') + 1); ;
                                                                if (!isUseFileNameFromDriveChecked)
                                                                    imagenamenew = clsGroupItem.StudentImage.Student.Lastname.Trim() + "_" + clsGroupItem.StudentImage.Student.FirstName.Trim() + "_" + imageName.Substring(imageName.LastIndexOf('_') + 1);
                                                                else
                                                                    imagenamenew = clsGroupItem.StudentImage.ImageName;
                                                                //tempImage = imageName;
                                                                if (exportType == "GotPhoto")
                                                                {
                                                                    if (!Directory.Exists(_destinationPath + "\\Groups"))
                                                                        Directory.CreateDirectory(_destinationPath + "\\Groups");

                                                                    File.Copy(ImageFolder1 + "\\" + imageName, _destinationPath + "\\Groups" + "\\" + imagenamenew, true);
                                                                }
                                                                else
                                                                    File.Copy(ImageFolder1 + "\\" + imageName, _destinationPath + "\\" + imagenamenew, true);
                                                                if (tempImage == imagenamenew && tempDestinationPath == _destinationPath) { continue; }
                                                                tempImage = imagenamenew;
                                                                tempDestinationPath = _destinationPath;
                                                                w.WriteStartElement("image");
                                                                w.WriteAttributeString("name", imagenamenew);
                                                                if (!isMackUpPhotoshootChecked)
                                                                    w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim()));
                                                                else
                                                                    w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + "_MU"));
                                                                w.WriteAttributeString("width", "");
                                                                w.WriteAttributeString("height", "");
                                                                w.WriteEndElement();
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                }
                                                else
                                                {
                                                    if (!Directory.Exists(_destinationPath))
                                                    {
                                                        Directory.CreateDirectory(_destinationPath);
                                                        groupClassImageIds = new ArrayList();
                                                    }
                                                    w.WriteEndElement();
                                                    w.WriteStartElement("gallery");
                                                    w.WriteAttributeString("password", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Password) ? "" : objGroupItem.StudentImage.Student.Password.ToString());
                                                    if (!isMackUpPhotoshootChecked)
                                                        w.WriteAttributeString("galleryName", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim()));
                                                    else
                                                        w.WriteAttributeString("galleryName", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + "_MU"));
                                                    //w.WriteAttributeString("email", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Emailaddress.ToString()) ? "" : objGroupItem.StudentImage.Student.Emailaddress.ToString());
                                                    //w.WriteAttributeString("email", "");
                                                    w.WriteAttributeString("email", objGroupItem.StudentImage.Student == null ? "" : (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Emailaddress) ? "" : (objGroupItem.StudentImage.Student.Emailaddress.Trim() + (string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Address) ? "" : "," + objGroupItem.StudentImage.Student.Address))));
                                                    studentID = objGroupItem.StudentImage.StudentIDPK.ToString();

                                                    File.Copy(ImageFolder + "\\" + _imageName, _destinationPath + "\\" + _imagenamenew, true);
                                                    w.WriteStartElement("image");
                                                    w.WriteAttributeString("name", _imagenamenew);
                                                    if (!isMackUpPhotoshootChecked)
                                                        w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim()));
                                                    else
                                                        w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + "_MU"));
                                                    w.WriteAttributeString("width", "");
                                                    w.WriteAttributeString("height", "");
                                                    w.WriteEndElement();

                                                    #region for group class photos
                                                    foreach (GroupItem clsGroupItem in _objClassPhotosToGroupItem)
                                                    {
                                                        if (groupId != clsGroupItem.GroupID)
                                                        {
                                                            continue;
                                                        }
                                                        groupClassImageIds.Add(clsGroupItem.StudentPhotoID);
                                                        string ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;

                                                        newPath = clsGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_reduced\\";
                                                        ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                                        if (isReducedImagesChecked == true)
                                                        {
                                                            if (Directory.Exists(newPath + "\\" + _imageName))
                                                                ImageFolder1 = newPath;
                                                            else
                                                                ImageFolder1 = newPath;
                                                        }
                                                        else if (isDigitalImagesChecked)
                                                        {
                                                            ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_DD\\";

                                                            bool isFolderNotFound = digitalImageFolderNotFound();
                                                            if (isFolderNotFound)
                                                            {
                                                                isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                                                                return;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ImageFolder1 = clsGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                                        }

                                                        string imageName = clsGroupItem.StudentImage.ImageName;

                                                        if (File.Exists(ImageFolder1 + "\\" + imageName))
                                                        {

                                                            string imagenamenew = ""; // clsGroupItem.StudentImage.Student.Lastname.Trim() + "_" + clsGroupItem.StudentImage.Student.FirstName.Trim() + "_" + imageName.Substring(imageName.LastIndexOf('_') + 1); ;

                                                            if (!isUseFileNameFromDriveChecked)
                                                                imagenamenew = clsGroupItem.StudentImage.Student.Lastname.Trim() + "_" + clsGroupItem.StudentImage.Student.FirstName.Trim() + "_" + imageName.Substring(imageName.LastIndexOf('_') + 1);
                                                            else
                                                                imagenamenew = clsGroupItem.StudentImage.ImageName;
                                                            if (exportType == "GotPhoto")
                                                            {
                                                                if (!Directory.Exists(_destinationPath + "\\Groups"))
                                                                    Directory.CreateDirectory(_destinationPath + "\\Groups");
                                                                File.Copy(ImageFolder1 + "\\" + imageName, _destinationPath + "\\Groups" + "\\" + imagenamenew, true);
                                                            }
                                                            else
                                                                File.Copy(ImageFolder1 + "\\" + imageName, _destinationPath + "\\" + imagenamenew, true);
                                                            if (tempImage == imagenamenew && tempDestinationPath == _destinationPath) { continue; }
                                                            tempImage = imagenamenew;
                                                            tempDestinationPath = _destinationPath;
                                                            w.WriteStartElement("image");
                                                            w.WriteAttributeString("name", imagenamenew);
                                                            if (!isMackUpPhotoshootChecked)
                                                                w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim()));
                                                            else
                                                                w.WriteAttributeString("roll", string.IsNullOrEmpty(objGroupItem.StudentImage.Student.Lastname) ? "" : (objGroupItem.StudentImage.Student.Lastname.Trim() + "_" + objGroupItem.StudentImage.Student.FirstName.Trim() + "_MU"));
                                                            w.WriteAttributeString("width", "");
                                                            w.WriteAttributeString("height", "");
                                                            w.WriteEndElement();
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }
                                            #endregion
                                            isProgressBarVisible = Visibility.Visible;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        clsStatic.WriteExceptionLogXML(ex);
                                        MVVMMessageService.ShowMessage(ex.Message);
                                        return;
                                    }
                                    # endregion
                                }
                                else
                                {
                                    MessageBox.Show(errorMessages.ALTERNATIVE_PATH_DOES_NOT_EXIST);
                                }
                            } // end of foreach loop for gorup items
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                            MVVMMessageService.ShowMessage(ex.Message);
                            return;
                        }
                        finally
                        {
                            //sw.Close();
                            //sw1.Close();
                            //File.Delete(test1238);
                            //File.Delete(test1239);
                            if (!isMissiedImages)
                            {
                                w.WriteEndElement(); w.WriteEndElement();
                            }
                            w.Flush();
                            w.Close();
                            fs.Close();
                            File.Delete(test1240);
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                _objWaitCursorViewModel.Dispose();
                            });

                        }
                        //createTextFile();
                        if (!isExportFTPChecked)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                _objWaitCursorViewModel.Dispose();
                            });
                            string messagecl = errorMessages.AFTER_COPY_OPEN_LOCATION_CONFIRMATION;
                            string captioncl = "Confirmation";
                            MessageBoxButton buttonss = MessageBoxButton.YesNo;
                            MessageBoxImage iconn = MessageBoxImage.Question;
                            if (MessageBox.Show(messagecl, captioncl, buttonss, iconn) == MessageBoxResult.Yes)
                                Process.Start(alternateFullPath);
                        }
                        else
                        {
                            MessageBoxButton buttonss = MessageBoxButton.OK;
                            MessageBoxImage iconn = MessageBoxImage.Information;
                            #region zip file n moving to ftp server
                            string zippedfilepath = "";
                            zippedfilepath = CompressFile(zipfilepath);

                            //MVVMMessageService.ShowMessage(createdNewPath + "\nGet lost from here.. ;-)");
                            //return;

                            string uploadFileName = "", uploadUrl = "";
                            //Stream requestStream = null;
                            uploadFileName = new FileInfo(zippedfilepath).Name;
                            uploadUrl = "ftp://ftp2.digitaleventpics.com";
                            FileStream fs2 = new FileStream(zippedfilepath, FileMode.Open, FileAccess.Read);
                            try
                            {
                                //old code
                                //long FileSize = new FileInfo(zippedfilepath).Length; // File size of file being uploaded.
                                //Byte[] buffer = new Byte[FileSize];
                                //fs2.Read(buffer, 0, buffer.Length);
                                //fs2.Close();
                                //string ftpUrl = string.Format("{0}/{1}", uploadUrl, uploadFileName);
                                //FtpWebRequest requestObj = FtpWebRequest.Create(ftpUrl) as FtpWebRequest;
                                //requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                                //requestObj.Credentials = new NetworkCredential("dp2fspg", "sp5194PCFZS");
                                //requestStream = requestObj.GetRequestStream();
                                //requestStream.Write(buffer, 0, buffer.Length);
                                //requestStream.Flush();
                                //requestStream.Close();
                                //requestObj = null;
                                //end old code

                                string ftpUrl = string.Format("{0}/{1}", uploadUrl, uploadFileName);
                                FtpWebRequest requestObj = FtpWebRequest.Create(ftpUrl) as FtpWebRequest;
                                requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                                requestObj.Credentials = new NetworkCredential("dp2fspg", "sp5194PCFZS");
                                //requestStream = requestObj.GetRequestStream();

                                using (Stream requestStream = requestObj.GetRequestStream())
                                {
                                    fs2.CopyTo(requestStream);
                                }

                                requestObj = null;
                                MVVMMessageService.ShowMessage("File upload/transfer successful.", "Successful", buttonss, iconn);
                            }

                            catch (Exception ex)
                            {
                                MVVMMessageService.ShowMessage("File upload/transfer Failed.\r\nError Message:\r\n" + ex.Message);
                                clsStatic.WriteExceptionLogXML(ex);
                            }
                            finally
                            {
                                if (fs2 != null)
                                {
                                    fs2.Close(); fs2.Dispose();
                                }

                                System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(createdNewPath);

                                foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                                {
                                    file.Delete();
                                }
                                foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                                {
                                    dir.Delete(true);
                                }
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    _objWaitCursorViewModel.Dispose();
                                });
                            }

                            #endregion
                        }
                        isSave = true;
                        DialogResult = false;
                    }
                    #endregion

                    #endregion

                    //    }
                    //    ts.Complete();
                    //}
                });
                #endregion

                PSPAAsyncCompleted();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Compressing .bak database file to .zip
        /// </summary>
        /// <param name="sourceFileName">.bak file name including path</param>
        /// fixed path --> C:\\DbBackup
        /// <returns>"path + / + .zip" file</returns>
        private string CompressFile(string sourceFileName)
        {
            zipfilepath = zipfilepath + ".zip";
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                try
                {
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    zip.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Always;
                    zip.AddDirectory(sourceFileName, "");
                    zip.Save(zipfilepath);
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
            return zipfilepath;
        }

        /// <summary>
        /// separates the data to create datafile 
        /// </summary>
        private void createTextFile()
        {
            try
            {
                List<Student> lstStudent = new List<Student>();
                List<StudentImage> templstStudentImages = new List<StudentImage>();
                List<View_Order> lstOrderStudImages = new List<View_Order>();//by hema 09/12/2015
                Student tempStudent;
                string fileName = "";

                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                //changed by hema
                if (isExportSelectedChecked)
                {
                    lstOrderStudImages = clsOrders.getOrderImagesFromOrderId(db, tempSelectedOrderIds);
                }
                else
                    lstOrderStudImages = clsOrders.getOrderImagesFromOrderId(db, allOrderIds);

                List<StudentImage> lstStudentImages = new List<StudentImage>();
                //int[] GroupclsImgs;
                ArrayList tempGroupclsImgs = new ArrayList();

                if (!isFromOrders && isImageQuixChecked == false)
                {
                    lstStudentImages = clsDashBoard.GetStudentsByImageIdArray(db, arrStudentImageIds);
                }

                #region Is From Orders
                //should change this code //Mohan
                if (isFromOrders)
                {
                    #region Properties
                    List<View_Order> MStudentImages = new List<View_Order>();
                    List<View_Order> RStudentImages = new List<View_Order>();
                    List<View_Order> SStudentImages = new List<View_Order>();
                    List<View_Order> FStudentImages = new List<View_Order>();
                    List<View_Order> NoBillingCode = new List<View_Order>();
                    List<View_Order> MissedImages = new List<View_Order>();
                    List<View_Order> RetouchImages = new List<View_Order>();
                    List<View_Order> StandardImages = new List<View_Order>();

                    lstManualOrderIds = new List<int>();

                    #endregion

                    #region create data
                    foreach (View_Order stu in lstOrderStudImages)
                    {
                        //if (stu.ImageName != "Order.jpg")
                        //{
                        bool Standard = false; bool retouch = false; bool isManual = false; List<int> richmond = new List<int>();
                        if (stu.sp_SimplePhotoBillingCode == "F135" || stu.sp_SimplePhotoBillingCode == "M134") { continue; }
                        tempFolderName = dicOrderItemsBillingCode[stu.StudentPhotoOrderId];

                        string ImageNameNew = stu.ImageName;
                        string imgExtension = ImageNameNew.Substring(ImageNameNew.LastIndexOf("."));
                        int? photoIdTxt = stu.PhotoShootID;
                        int newPhotoIdTxt = photoIdTxt.Value;
                        objImageFolder = clsDashBoard.getImageFolder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), newPhotoIdTxt);

                        ImageFolder = objImageFolder.ImageFolder;
                        if (isReducedImagesChecked) { ImageFolder = ImageFolder + "\\_reduced"; }
                        else if (isDigitalImagesChecked)
                        {
                            ImageFolder = ImageFolder + "\\_DD";

                            bool isFolderNotFound = digitalImageFolderNotFound();
                            if (isFolderNotFound)
                            {
                                return;
                            }
                        }
                        int orderId = Convert.ToInt32(stu.OrderId);

                        if (!lstManualOrderIds.Contains(orderId))
                        {
                            isManual = clsOrders.isManualOrder(db, orderId);
                            if (isManual) { lstManualOrderIds.Add(orderId); }
                        }

                        retouch = clsOrders.isRetouchTrue(db, orderId, (int)stu.StudentImageId);
                        if (retouch == true)
                        {
                            //folderName = "RETOUCH";
                        }
                        else
                        {
                            //retouch = false;
                            Standard = clsOrders.isStandardTrue(db, orderId, (int)stu.StudentImageId);
                            {
                                //if (Standard == true)
                                //{
                                //Commented by Mohan as per Don Wiid suggestion
                                //folderName = "STANDARD";  //At the moment, any photos with this action is being exported to a STANDARD folder. This is wrong - it should go to M folder, since Millers apply the standard retouching.
                                //folderName = "M";
                                //}
                            }
                        }
                        richmond = clsOrders.isRichmondTrue(db);

                        if (!File.Exists(ImageFolder + "\\" + stu.ImageName))
                            MissedImages.Add(stu);
                        else if (richmond.Contains(Convert.ToInt32(stu.OrderId)))
                            RStudentImages.Add(stu);
                        else if (retouch == true)   //premium retouch
                            RetouchImages.Add(stu);
                        else if (Standard == true)  //standard retouch by miller's
                            StandardImages.Add(stu);
                        else if (tempFolderName == "M")
                            MStudentImages.Add(stu);
                        //else if (tempFolderName == "R")
                        //    RStudentImages.Add(stu);
                        else if (tempFolderName == "S")
                            SStudentImages.Add(stu);
                        else if (tempFolderName == "F")
                            FStudentImages.Add(stu);
                        else
                            NoBillingCode.Add(stu);
                        //}
                        retouch = false;
                    }
                    #endregion

                    List<Tuple<List<View_Order>, string>> dictWriteDataToTextFie;

                    #region For Each Folder DataFile
                    if (MissedImages.Count > 0)
                    {
                        isMissedImages = true;
                        folderPathtxt = alternateFullPath + "\\" + missingImages;
                        DeleteTextFile(folderPathtxt);

                        dictWriteDataToTextFie = new List<Tuple<List<View_Order>, string>>();
                        dictWriteDataToTextFie.Add(new Tuple<List<View_Order>, string>(MissedImages, ""));
                        TextFile(dictWriteDataToTextFie, false);
                    }

                    if (RStudentImages.Count > 0 && Directory.Exists(alternateFullPath + "\\" + "R"))
                    {
                        folderPathtxt = alternateFullPath + "\\" + "R" + "\\" + "Datafile.txt";
                        DeleteTextFile(folderPathtxt);

                        dictWriteDataToTextFie = new List<Tuple<List<View_Order>, string>>();
                        dictWriteDataToTextFie.Add(new Tuple<List<View_Order>, string>(RStudentImages, ""));
                        TextFile(dictWriteDataToTextFie, false);
                    }
                    if (SStudentImages.Count > 0 && Directory.Exists(alternateFullPath + "\\" + "S"))
                    {
                        folderPathtxt = alternateFullPath + "\\" + "S" + "\\" + "Datafile.txt";
                        DeleteTextFile(folderPathtxt);

                        dictWriteDataToTextFie = new List<Tuple<List<View_Order>, string>>();
                        dictWriteDataToTextFie.Add(new Tuple<List<View_Order>, string>(SStudentImages, ""));
                        TextFile(dictWriteDataToTextFie, false);
                    }
                    if (FStudentImages.Count > 0 && Directory.Exists(alternateFullPath + "\\" + "F"))
                    {
                        folderPathtxt = alternateFullPath + "\\" + "F" + "\\" + "Datafile.txt";
                        DeleteTextFile(folderPathtxt);

                        dictWriteDataToTextFie = new List<Tuple<List<View_Order>, string>>();
                        dictWriteDataToTextFie.Add(new Tuple<List<View_Order>, string>(FStudentImages, ""));
                        TextFile(dictWriteDataToTextFie, false);
                    }
                    if (NoBillingCode.Count > 0 && Directory.Exists(alternateFullPath + "\\" + "No Billing Code"))
                    {
                        folderPathtxt = alternateFullPath + "\\" + "No Billing Code" + "\\" + "DataFile.txt";
                        DeleteTextFile(folderPathtxt);

                        dictWriteDataToTextFie = new List<Tuple<List<View_Order>, string>>();
                        dictWriteDataToTextFie.Add(new Tuple<List<View_Order>, string>(NoBillingCode, ""));
                        TextFile(dictWriteDataToTextFie, false);
                    }
                    if (RetouchImages.Count > 0 && Directory.Exists(alternateFullPath + "\\" + "RETOUCH"))
                    {
                        //isRetouch = "PREMIUM";
                        folderPathtxt = alternateFullPath + "\\" + "RETOUCH" + "\\" + "DataFile.txt";
                        DeleteTextFile(folderPathtxt);

                        dictWriteDataToTextFie = new List<Tuple<List<View_Order>, string>>();
                        dictWriteDataToTextFie.Add(new Tuple<List<View_Order>, string>(RetouchImages, "PREMIUM"));
                        TextFile(dictWriteDataToTextFie, true);
                        //isRetouch = "";
                    }
                    if ((MStudentImages.Count > 0 || StandardImages.Count > 0) && Directory.Exists(alternateFullPath + "\\" + "M"))
                    {
                        folderPathtxt = alternateFullPath + "\\" + "M" + "\\" + "Datafile.txt";
                        DeleteTextFile(folderPathtxt);

                        dictWriteDataToTextFie = new List<Tuple<List<View_Order>, string>>();
                        dictWriteDataToTextFie.Add(new Tuple<List<View_Order>, string>(MStudentImages, ""));
                        if (StandardImages.Count > 0) { dictWriteDataToTextFie.Add(new Tuple<List<View_Order>, string>(StandardImages, "TRUE")); }
                        TextFile(dictWriteDataToTextFie, true);
                    }
                    //if (StandardImages.Count > 0 && Directory.Exists(alternateFullPath + "\\" + "M"))
                    //{
                    //    isRetouch = "TRUE";
                    //    folderPathtxt = alternateFullPath + "\\" + "M" + "\\" + "DataFile.txt";
                    //    DeleteTextFile(folderPathtxt);
                    //    TextFile(StandardImages, true);
                    //    isRetouch = "";
                    //}
                    #endregion
                }

                #endregion

                else if (isImageQuixChecked == true) //Added else before if
                {
                    fileName = "DataFile.json";
                }
                else if (isZenfolioChecked == false) //Added else before if
                {
                    fileName = "DataFile.txt";
                }
                else //MCPS
                {
                    fileName = "IDLINK.txt";
                }

                if (!isFromOrders)
                {
                    folderPathtxt = alternateFullPath + "\\" + fileName;
                    if (isImageQuixChecked)
                    {
                        missedImagesfolderPath = alternateFullPath + "\\" + "_" + missingImages;
                    }
                    else
                    {
                        missedImagesfolderPath = alternateFullPath + "\\" + missingImages;
                    }

                    if (File.Exists(folderPathtxt))
                    {
                        File.Delete(folderPathtxt);
                    }
                }

                #region website format
                if (isWebsiteFormatChecked || isZenfolioChecked)
                {
                    string newImageName = "";
                    System.IO.StreamWriter missedImages = new System.IO.StreamWriter(missedImagesfolderPath, true);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(folderPathtxt, true))
                    {
                        //if (isWebsiteFormatChecked == true)
                        if (!isZenfolioChecked)
                        {
                            file.WriteLine("Image Name\tFirst Name\tLast Name\tStudent ID\tTeacher\tGrade\tPassword\tSchool\tLocation\tPackages\tPhone\tCustom1\tCustom2\tCustom3\tCustom4\tCustom5\tCustom6\tCustom7\tCustom8\tEmailaddress");
                            missedImages.WriteLine("Image Name\tFirst Name\tLast Name\tStudent ID\tTeacher\tGrade\tPassword\tSchool\tLocation\tPackages\tPhone\tCustom1\tCustom2\tCustom3\tCustom4\tCustom5\tCustom6\tCustom7\tCustom8\tEmailaddress");
                        }

                        foreach (StudentImage objStudentImage in lstStudentImages)
                        {
                            try
                            {
                                if (objStudentImage.Student == null)
                                {
                                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                    int id = clsDashBoard.getStudentIdByImageId(db, objStudentImage.ID);
                                    if ((from a in lstStudent where a.ID == id select a).FirstOrDefault() == null)
                                    {
                                        tempStudent = clsStudent.updateStudent(db, id);
                                        lstStudent.Add(tempStudent);
                                    }
                                    else
                                    {
                                        tempStudent = lstStudent.FirstOrDefault(s => s.ID == id);
                                    }
                                }
                                else
                                {
                                    tempStudent = objStudentImage.Student;
                                }
                                string ImageNameNew = objStudentImage.ImageName;
                                string imgExtension = ImageNameNew.Substring(ImageNameNew.LastIndexOf("."));
                                int? photoIdTxt = objStudentImage.PhotoShootID;
                                int newPhotoIdTxt = photoIdTxt.Value;
                                objImageFolder = clsDashBoard.getImageFolder(db, newPhotoIdTxt);

                                ImageFolder = objImageFolder.ImageFolder;
                                if (isDigitalImagesChecked)
                                {
                                    ImageFolder = ImageFolder + "\\_DD";

                                    bool isFolderNotFound = digitalImageFolderNotFound();
                                    if (isFolderNotFound)
                                    {
                                        return;
                                    }
                                }
                                else if (isReducedImagesChecked) { ImageFolder = ImageFolder + "\\_reduced"; }


                                if (isRenameFileChecked == true)
                                {
                                    newImageName = "";
                                    DataTable dtMaskDetails = clsDashBoard.GetMaskDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedMask.MaskID);
                                    // Need to create a dynamic query
                                    string query = "";
                                    for (int j = 0; j < dtMaskDetails.Rows.Count; j++)
                                    {
                                        try
                                        {
                                            if (dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "school name" || dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "schoolname")
                                                query = query + "s.schoolname" + ",";
                                            else if (dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "grade")
                                                query = query + "st.grade" + ",";
                                            else if (dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "teacher")
                                                query = query + "st.teacher" + ",";
                                            else if (dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "first name" || dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "firstname")
                                                query = query + "st.firstname" + ",";
                                            else if (dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "last name" || dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "lastname")
                                                query = query + "st.lastname" + ",";
                                            else if (dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "password")
                                                query = query + "st.Password" + ",";
                                            else if (dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "student id" || dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "studentid")
                                                query = query + "st.StudentID" + ",";
                                            else if (dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "image number" || dtMaskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "imagenumber")
                                                query = query + "sm.ImageNumber" + ",";
                                            else
                                                query = query + "'" + dtMaskDetails.Rows[j]["MaskDetail1"].ToString() + "' as Label" + ",";
                                        }
                                        catch (Exception ex)
                                        {
                                            clsStatic.WriteExceptionLogXML(ex);
                                            MVVMMessageService.ShowMessage(ex.Message);
                                        }
                                    }
                                    query = query.Substring(0, query.Length - 1);
                                    string orignalQuery = "";
                                    DataSet ds = new DataSet();

                                    int imagId = objStudentImage.ID;
                                    string img = imagId.ToString();
                                    SqlParameter[] param = new SqlParameter[1];
                                    param[0] = new SqlParameter("@ID", schoolId);
                                    orignalQuery = "select " + query + " from StudentImage sm inner join school s on s.id = sm.SchoolID inner join student st on sm.studentidpk = st.id where s.ID = @ID and sm.id in(" + img + ")";
                                    ds = WCFSQLHelper.getDataSetText_SP(orignalQuery, param);


                                    foreach (DataRow dtr in ds.Tables[0].Rows)
                                    {

                                        foreach (DataColumn c in ds.Tables[0].Columns)
                                        {
                                            newImageName = newImageName + (dtr[c.ColumnName]).ToString();
                                        }

                                        newImageName = newImageName.Substring(0, newImageName.Length);
                                        string newsequntialnumber = objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('_') + 1);
                                        if (isAddImgSerialNo == true)
                                            newImageName = newImageName + "_" + newsequntialnumber;
                                        else
                                            newImageName = newImageName + imgExtension;

                                        //objStudentImage.ImageName = newImageName;

                                    }
                                }

                                if (isWebsiteFormatChecked == true)
                                {
                                    string serialNumber = "";
                                    string groupName = templstStudentImages.Contains(objStudentImage) ? "" : (dicStuImgGroupClassInfo.ContainsKey(objStudentImage.ID) ? dicStuImgGroupClassInfo[objStudentImage.ID] : "");

                                    if (File.Exists(ImageFolder + "\\" + objStudentImage.ImageName))
                                    {
                                        if (isRenameFileChecked) { objStudentImage.ImageName = newImageName; }
                                        else
                                        {
                                            if (isAddImgSerialNo)
                                            {
                                                serialNumber = objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('_') + 1, objStudentImage.ImageName.LastIndexOf('.') - objStudentImage.ImageName.LastIndexOf('_') - 1);
                                                objStudentImage.ImageName = objStudentImage.ImageName.Remove(objStudentImage.ImageName.LastIndexOf('.')) + "_" + serialNumber + objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('.'));
                                            }
                                        }
                                        file.WriteLine(
                                            objStudentImage.ImageName + "\t" + (!string.IsNullOrEmpty(tempStudent.FirstName) ? tempStudent.FirstName.Trim() : "") + "\t" + (!string.IsNullOrEmpty(tempStudent.Lastname) ? tempStudent.Lastname.Trim() : "") + "\t" + tempStudent.StudentID + "\t" +
                                            tempStudent.Teacher + "\t" + tempStudent.Grade + "\t" + tempStudent.Password + "\t" + objStudentImage.School.SchoolName + "\t" +
                                            objStudentImage.PhotoShoot.ImageFolder + "\t" +
                                            objStudentImage.Packages + "\t" + tempStudent.Phone + "\t" + tempStudent.Custom1 + "\t" + tempStudent.Custom2 + "\t" +
                                            tempStudent.Custom3 + "\t" + tempStudent.Custom4 + "\t" + tempStudent.Custom5 + "\t" +
                                            groupName + "\t" + objStudentImage.Custom7 + "\t" + objStudentImage.Custom8 + "\t" + objStudentImage.Student.Emailaddress
                                            );
                                    }
                                    else
                                    {
                                        isMissedImages = true;
                                        if (isRenameFileChecked) { objStudentImage.ImageName = newImageName; }
                                        else
                                        {
                                            if (isAddImgSerialNo)
                                            {
                                                serialNumber = objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('_') + 1, objStudentImage.ImageName.LastIndexOf('.') - objStudentImage.ImageName.LastIndexOf('_') - 1);
                                                objStudentImage.ImageName = objStudentImage.ImageName.Remove(objStudentImage.ImageName.LastIndexOf('.')) + "_" + serialNumber + objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('.'));
                                            }
                                        }
                                        missedImages.WriteLine(
                                           objStudentImage.ImageName + "\t" + (!string.IsNullOrEmpty(tempStudent.FirstName) ? tempStudent.FirstName.Trim() : "") + "\t" + (!string.IsNullOrEmpty(tempStudent.Lastname) ? tempStudent.Lastname.Trim() : "") + "\t" + tempStudent.StudentID + "\t" +
                                           tempStudent.Teacher + "\t" + tempStudent.Grade + "\t" + tempStudent.Password + "\t" + objStudentImage.School.SchoolName + "\t" +
                                           objStudentImage.PhotoShoot.ImageFolder + "\t" +
                                           objStudentImage.Packages + "\t" + tempStudent.Phone + "\t" + tempStudent.Custom1 + "\t" + tempStudent.Custom2 + "\t" +
                                           tempStudent.Custom3 + "\t" + tempStudent.Custom4 + "\t" + tempStudent.Custom5 + "\t" +
                                           groupName + "\t" + objStudentImage.Custom7 + "\t" + objStudentImage.Custom8 + "\t" + objStudentImage.Student.Emailaddress
                                           );
                                    }
                                }
                                else if (isZenfolioChecked)
                                {
                                    string serialNumber = "";
                                    string groupName = templstStudentImages.Contains(objStudentImage) ? "" : (dicStuImgGroupClassInfo.ContainsKey(objStudentImage.ID) ? dicStuImgGroupClassInfo[objStudentImage.ID] : "");

                                    if (File.Exists(ImageFolder + "\\" + objStudentImage.ImageName))
                                    {
                                        if (isRenameFileChecked) { objStudentImage.ImageName = newImageName; }
                                        else
                                        {
                                            if (isAddImgSerialNo)
                                            {
                                                serialNumber = objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('_') + 1, objStudentImage.ImageName.LastIndexOf('.') - objStudentImage.ImageName.LastIndexOf('_') - 1);
                                                objStudentImage.ImageName = objStudentImage.ImageName.Remove(objStudentImage.ImageName.LastIndexOf('.')) + "_" + serialNumber + objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('.'));
                                            }
                                        }
                                        //file.WriteLine(tempStudent.ID + "," + tempStudent.ID + ".jpg");
                                        file.WriteLine(tempStudent.StudentID + "," + tempStudent.StudentID + ".jpg");
                                        //12-10-2016 //Commented by Mohan , As per Neal request for MCPS export - image name should be same as Student unique id
                                    }
                                    else
                                    {
                                        isMissedImages = true;
                                        if (isRenameFileChecked) { objStudentImage.ImageName = newImageName; }
                                        else
                                        {
                                            if (isAddImgSerialNo)
                                            {
                                                serialNumber = objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('_') + 1, objStudentImage.ImageName.LastIndexOf('.') - objStudentImage.ImageName.LastIndexOf('_') - 1);
                                                objStudentImage.ImageName = objStudentImage.ImageName.Remove(objStudentImage.ImageName.LastIndexOf('.')) + "_" + serialNumber + objStudentImage.ImageName.Substring(objStudentImage.ImageName.LastIndexOf('.'));
                                            }
                                        }
                                        missedImages.WriteLine(tempStudent.StudentID + "," + tempStudent.StudentID + ".jpg");
                                    }
                                }
                            }

                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }
                        missedImages.Dispose();
                    }
                }
                #endregion

                #region ImageQuix
                else if (isImageQuixChecked)
                {
                    try
                    {
                        #region Initialization
                        List<JsonGroupImages> _tempGroupClassImagesData = new List<JsonGroupImages>(); //JsonGroup Data(GroupId, GroupName, GroupImages)                                
                        List<JsonGroupImages> _tempMissedImagesData = new List<JsonGroupImages>(); //JsonGroup Data(GroupId, GroupName, GroupImages)
                        List<Subjects> _objSubjects = new List<Subjects>();  // GroupClass Photo Student details as subject

                        ArrayList Id = new ArrayList();
                        List<string> tempGroupImagesNames = new List<string>();  // student images list
                        List<string> tempGroupClassImagesNames = new List<string>();  // student images list
                        List<string> missedImagesList = new List<string>(); // missed student images list
                        List<Student> tempStudentListString = new List<Student>(); // GroupClass Photo student details
                        List<string> tempStandardListString = new List<string>(); // GroupClass Photo student details
                        List<int> tempStudentIds = new List<int>();
                        List<GroupClassPhoto> tempGCMissedImages = new List<GroupClassPhoto>(); // missed groupclassImages list

                        bool hasGroupClassPhoto = true;
                        string destinationPath = "";
                        #endregion

                        #region check for Datafile existence
                        destinationPath = alternateFullPath + "\\" + txtTitle + "\\" + fileName;

                        if (File.Exists(destinationPath))
                        {
                            File.Delete(destinationPath);
                        }
                        #endregion

                        App_Code.Group tempGroupName = new App_Code.Group();

                        // #region grouping as jsonfiles

                        #region check for group class photo
                        if (iqGroupClassPhotos.Count() > 0)
                        {
                            foreach (GroupClassPhoto image in iqGroupClassPhotos)
                            {
                                if (!Id.Contains(image.GroupId))
                                {
                                    Id.Add(image.GroupId);

                                    tempGroupName = (from grp in db.Groups where grp.ID == image.GroupId select grp).FirstOrDefault();
                                }

                                #region check for image folder esitence
                                ImageFolder = image.StudentImage.PhotoShoot.ImageFolder.ToString();
                                newPath = ImageFolder + "\\_reduced";
                                if (isReducedImagesChecked == true)   //if we are using the reduced images
                                {
                                    ImageFolder = newPath;
                                }
                                else if (isDigitalImagesChecked)
                                {
                                    ImageFolder = image.StudentImage.PhotoShoot.ImageFolder.ToString() + "\\_DD";

                                    bool isFolderNotFound = digitalImageFolderNotFound();
                                    if (isFolderNotFound)
                                    {
                                        ///args.Cancel = true; worker = new BackgroundWorker();
                                        return;
                                    }
                                }
                                #endregion

                                if (Directory.Exists(ImageFolder))
                                {
                                    tempGroupClassImagesNames = new List<string>();
                                    tempGroupClassImagesNames.Add(image.StudentImage.ImageName.ToString());
                                    //Assigning data to JsonGroups
                                    _tempGroupClassImagesData.AddRange(new List<JsonGroupImages>{
                                                                  new JsonGroupImages {oID= tempGroupName.ID,name = tempGroupName.GroupName,images = tempGroupClassImagesNames}
                                                             });
                                }
                                else
                                {
                                    tempGCMissedImages.Add(image);
                                }
                            }

                        }
                        //else if (isIndividualPwd == true)
                        //    hasGroupClassPhoto = true;
                        #endregion

                        foreach (int groupId in arrGroupId)
                        {
                            string studentID = ""; int imagescount = 0;
                            tempGroupImagesNames = new List<string>();
                            tempStandardListString = new List<string>();
                            missedImagesList = new List<string>();
                            ArrayList tempGroupId = new ArrayList();

                            tempGroupId.Add(groupId);

                            DataTable objGroupItems = new DataTable();
                            if (isExportAllImagesChecked == true)
                                objGroupItems = clsDashBoard.getGroupNameAndGroupImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempGroupId, true, arrStudentImageIds);
                            else
                                objGroupItems = clsDashBoard.getGroupNameAndGroupImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempGroupId, false, arrStudentImageIds);

                            DataView dv = objGroupItems.DefaultView;
                            dv.Sort = "StudentIDPK desc";
                            objGroupItems = dv.ToTable();

                            // #endregion

                            foreach (DataRow objGroupItem in objGroupItems.Rows)
                            {
                                imagescount++;
                                # region Check for image folder existence
                                ImageFolder = objGroupItem["ImageFolder"].ToString();
                                newPath = ImageFolder + "\\_reduced";
                                if (isReducedImagesChecked == true)   //if we are using the reduced images
                                {
                                    ImageFolder = newPath;
                                }
                                else if (isDigitalImagesChecked)
                                {
                                    ImageFolder = objGroupItem["ImageFolder"].ToString() + "\\_DD";

                                    bool isFolderNotFound = digitalImageFolderNotFound();
                                    if (isFolderNotFound)
                                    {
                                        ///args.Cancel = true; worker = new BackgroundWorker();
                                        return;
                                    }
                                }
                                #endregion

                                if (hasGroupClassPhoto == true)
                                {
                                    if (Directory.Exists(ImageFolder))
                                    {
                                        if (string.IsNullOrEmpty(studentID) || studentID == objGroupItem["StudentIDPK"].ToString())
                                        {
                                            studentID = objGroupItem["StudentIDPK"].ToString();
                                            tempGroupImagesNames.Add(objGroupItem["ImageName"].ToString());
                                        }
                                        else
                                        {
                                            tempStudentListString = new List<Student>(clsStudent.getStudentDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(studentID)));

                                            // checking for if student has password or not
                                            if (string.IsNullOrEmpty(tempStudentListString[0].Password) && !tempStudentIds.Contains(tempStudentListString[0].ID))
                                            {
                                                studentsWithoutPasswords += tempStudentListString[0].FirstName + " " + tempStudentListString[0].Lastname + ",";

                                                tempStudentIds.Add(tempStudentListString[0].ID);
                                            }

                                            _objSubjects.AddRange(new List<Subjects> {
                                                                            new Subjects {firstName = tempStudentListString[0].FirstName, lastName = tempStudentListString[0].Lastname, code = tempStudentListString[0].Password, images = tempGroupImagesNames,galleryGroupOID=(objGroupItem["GroupId"]==null?0:Convert.ToInt32(objGroupItem["GroupId"]))}
                                                                         });

                                            studentID = objGroupItem["StudentIDPK"].ToString();
                                            tempGroupImagesNames = new List<string>();
                                            tempGroupImagesNames.Add(objGroupItem["ImageName"].ToString());
                                        }

                                    }
                                    else
                                        missedImagesList.Add(objGroupItem["ImageName"].ToString());

                                    if (imagescount == objGroupItems.Rows.Count && Directory.Exists(ImageFolder))
                                    {
                                        tempStudentListString = new List<Student>(clsStudent.getStudentDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(studentID)));

                                        // checking for if student has password or not
                                        if (string.IsNullOrEmpty(tempStudentListString[0].Password) && !tempStudentIds.Contains(tempStudentListString[0].ID))
                                        {
                                            studentsWithoutPasswords += tempStudentListString[0].FirstName + " " + tempStudentListString[0].Lastname + ",";

                                            tempStudentIds.Add(tempStudentListString[0].ID);
                                        }

                                        _objSubjects.AddRange(new List<Subjects> {
                                                                            new Subjects {firstName = tempStudentListString[0].FirstName, lastName = tempStudentListString[0].Lastname, code = tempStudentListString[0].Password, images = tempGroupImagesNames,galleryGroupOID=(objGroupItem["GroupId"]==null?0:Convert.ToInt32(objGroupItem["GroupId"]))}
                                                                         });
                                    }
                                }
                                else
                                {
                                    if (!Directory.Exists(ImageFolder))
                                        missedImagesList.Add(objGroupItem["ImageName"].ToString());
                                }

                            } // end of for loop for individual group images

                            if (tempGCMissedImages.Count > 0)
                            {
                                foreach (GroupClassPhoto imag in tempGCMissedImages)
                                {
                                    if (imag.GroupId == Convert.ToInt32(objGroupItems.Rows[0]["GroupId"]))
                                        missedImagesList.Add(imag.StudentImage.ImageName);
                                }
                            }
                            //Creating JsonGroupImages file for missed images 
                            if (missedImagesList.Count > 0)
                            {
                                _tempMissedImagesData.AddRange(new List<JsonGroupImages>{
                                                                  new JsonGroupImages {oID= Convert.ToInt32(objGroupItems.Rows[0]["GroupId"]),name = (objGroupItems.Rows[0]["GroupName"].ToString() ),images = missedImagesList}
                                                             });
                            }

                        }// end of for loop for array group Ids


                        // Creating Json file for missedImages
                        if (_tempMissedImagesData.Count > 0)
                            jsonDataFile(false, null, _tempMissedImagesData, missedImagesfolderPath);
                        else
                            jsonDataFile(true, _objSubjects, _tempGroupClassImagesData, destinationPath);

                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        #region JsonFile for Groups

        private void jsonDataFile(bool isHasGroupClassPhoto, List<Subjects> _objSubjects, List<JsonGroupImages> lstJsonGroupImages, string destinationPath)
        {
            if (isHasGroupClassPhoto)
            {
                Gallery _objGallery = new Gallery();

                _objGallery.subjects = _objSubjects;

                _objGallery.galleryType = "subject";
                _objGallery.id = -1;
                _objGallery.title = txtTitle;//clsSchool.defaultSchoolName;
                _objGallery.jobType = selectedJobType;//"underclass_fall";
                _objGallery.eventDate = dpEventDate == null ? null : dpEventDate.Value.Date.ToString("yyyy-MM-dd");//DateTime.Today.Date.ToString("yyyy-MM-dd");
                _objGallery.startShipDate = dpShipDate == null ? null : dpShipDate.Value.Date.ToString("yyyy-MM-dd");//DateTime.Today.Date.ToString("yyyy-MM-dd");
                _objGallery.welcomeMessage = txtWelcomeMessage;
                //_objGallery.password = txtPassword;
                _objGallery.priceSheet = selectedIQPricesheet == null ? "" : ("pricesheet/" + (selectedIQPricesheet.IQPriceSheetId).ToString());
                _objGallery.isGreenScreen = isGreenScreenChecked;
                _objGallery.isPreOrder = isPreOrderChecked;
                _objGallery.reference = txtReference;
                _objGallery.imageSize = selectedImageSize;
                _objGallery.keyword = txtPassword;
                _objGallery.groups = lstJsonGroupImages;
                _objGallery.galleryConfig = selectedIQVandoSettings == null ? "" : ("galleryconfig/" + (selectedIQVandoSettings.IQVandoId).ToString());

                JsonFileData _objJsonFileData = new JsonFileData { gallery = _objGallery, publishState = "INITIAL_METADATA" };

                string json = JsonConvert.SerializeObject(_objJsonFileData, Formatting.Indented);

                //write string to file
                System.IO.File.WriteAllText(destinationPath, json);
            }
            else
            {
                gallery _objgallery = new gallery();

                _objgallery.galleryType = "standard";
                _objgallery.id = -1;
                _objgallery.title = txtTitle;//clsSchool.defaultSchoolName;
                _objgallery.jobType = selectedJobType;//"underclass_fall";
                _objgallery.eventDate = dpEventDate == null ? null : dpEventDate.Value.Date.ToString("yyyy-MM-dd"); //DateTime.Today.Date.ToString("yyyy-MM-dd");
                _objgallery.startShipDate = dpShipDate == null ? "" : dpShipDate.Value.Date.ToString("yyyy-MM-dd");//DateTime.Today.Date.ToString("yyyy-MM-dd");
                _objgallery.welcomeMessage = txtWelcomeMessage;
                //_objgallery.password = txtPassword;
                _objgallery.priceSheet = selectedIQPricesheet == null ? null : ("pricesheet/" + (selectedIQPricesheet.IQPriceSheetId).ToString());
                _objgallery.isGreenScreen = isGreenScreenChecked;
                _objgallery.isPreOrder = isPreOrderChecked;
                _objgallery.reference = txtReference;
                _objgallery.imageSize = selectedImageSize;
                _objgallery.keyword = txtPassword;
                _objgallery.groups = lstJsonGroupImages;
                _objgallery.galleryConfig = selectedIQVandoSettings == null ? "" : ("galleryconfig/" + (selectedIQVandoSettings.IQVandoId).ToString()); ;

                StandardJsonFileData _objStandardJsonFileData = new StandardJsonFileData { gallery = _objgallery, publishState = "INITIAL_METADATA" };

                string json = JsonConvert.SerializeObject(_objStandardJsonFileData, Formatting.Indented);

                //write string to file
                System.IO.File.WriteAllText(destinationPath, json);
            }
        }

        #endregion

        #region Text File for Orders
        /// <summary>
        /// create the datafile for orders
        /// </summary>
        /// <param name="dctOrderStudImages"></param>
        /// <param name="isFROMMillers">if its true it will create Order.jpg</param>
        #region For Text File
        void TextFile(List<Tuple<List<View_Order>, string>> dctOrderStudImages, bool isFROMMillers)
        {
            int tempOrderId = 0;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(folderPathtxt, true))
            {
                //we are filling order import batch id in student id
                file.WriteLine("Image Name\tFirst Name\tLast Name\tStudent id\tTeacher\tGrade\tactivity\tpackages\tretouching\tcustom1\tTicket Code\tGroup Image Name\tGroup Image Path\tOrder Number\tShip To First Name\tShip To Last Name\tAddress\tCity\tState\tZip\tCountry\tEmail\tOrderId\tVendor Order Id\tVendor Date");
                View_Order tempViewOrder = new View_Order();
                //decimal? totalweight = 0;
                for (int i = 0; i < dctOrderStudImages.Count; i++)
                {
                    try
                    {
                        string tempImageFolder = "";
                        string isRetouch = "";
                        //decimal? weight = 0;

                        List<View_Order> lstOrderStudImages = new List<View_Order>();

                        lstOrderStudImages = dctOrderStudImages.ElementAt(i).Item1;
                        isRetouch = dctOrderStudImages.ElementAt(i).Item2;

                        foreach (View_Order orderDetail in lstOrderStudImages.OrderBy(t => t.OrderId))
                        {

                            //string[] quantity = orderDetail.Package.Contains('-') ? orderDetail.Package.Split('-') : null;
                            //if (quantity != null)
                            //{
                            //    OrderPackage tempPackage = (from op in db.OrderPackages where op.SimplePhotoItemId == orderDetail.sp_SimplePhotoBillingCode select op).FirstOrDefault();

                            //    if (tempPackage != null)
                            //    {
                            //        weight = (tempPackage.Weight * Convert.ToInt32(quantity[1]));

                            //        totalweight = totalweight + weight;
                            //    }
                            //}

                            int? tempImportBatchId = (from iii in allOrders where iii.Id == orderDetail.OrderId select iii.OrdersImportId).FirstOrDefault();
                            if (tempOrderId != (int)orderDetail.OrderId && tempOrderId != 0 && isFROMMillers == true && !lstManualOrderIds.Contains((int)orderDetail.OrderId) && !isFreePhotoExportChecked)
                            {
                                file.WriteLine(
                                    "Order.jpg" + "\t" + tempViewOrder.Cust_FirstName + "\t" + tempViewOrder.Cust_LastName + "\t" + orderDetail.SimplePhotoOrderID + "\t" + "" + "\t" + "" + "\t" + tempViewOrder.SchoolName
                                    + "\t" + "AY-1" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + tempViewOrder.SimplePhotoOrderID + "\t" + tempViewOrder.Ship_FirstName
                                    + "\t" + tempViewOrder.Ship_LastName + "\t" + tempViewOrder.Ship_Address + "\t" + tempViewOrder.Ship_City + "\t" + tempViewOrder.Ship_State
                                    + "\t" + tempViewOrder.Ship_PostalCode + "\t" + tempViewOrder.Ship_Country + "\t" + tempViewOrder.Cust_Email + "\t" + tempViewOrder.OrderId + "\t" + "" + "\t" + ""
                                    );
                            }
                            //orderDetail.Package == "AH-1" ? "I-1" : orderDetail.Package + "\t" + orderDetail.Package == "AH-1" ? isRetouch : "" + "\t" + orderDetail.Custom1 + "\t" +
                            string billingCode = orderDetail.sp_SimplePhotoBillingCode;
                            string GroupImageName = "";
                            if (billingCode == "M127" || billingCode == "M128" || billingCode == "M129" ||
                                           billingCode == "M130" || billingCode == "M131" || billingCode == "M132" || billingCode == "M133" || billingCode == "M136" || billingCode == "M171")
                            {
                                GroupImageName = dicStuImgGroupClassInfo[(int)orderDetail.StudentImageId];
                            }
                            else
                                GroupImageName = "";
                            file.WriteLine(
                                orderDetail.ImageName + "\t" + orderDetail.Cust_FirstName + "\t" + orderDetail.Cust_LastName + "\t" + orderDetail.SimplePhotoOrderID + "\t" +
                                orderDetail.Teacher + "\t" + orderDetail.Grade + "\t" + orderDetail.SchoolName + "\t" + orderDetail.Package + "\t"
                                + isRetouch + "\t" + orderDetail.Custom1 + "\t" + orderDetail.Ticketcode + "\t"
                                + GroupImageName + "\t" + tempImageFolder
                                + "\t" + orderDetail.SimplePhotoOrderID + "\t" + orderDetail.Ship_FirstName + "\t" + orderDetail.Ship_LastName
                                + "\t" + orderDetail.Ship_Address + "\t" + orderDetail.Ship_City + "\t" + orderDetail.Ship_State + "\t" + orderDetail.Ship_PostalCode
                                + "\t" + orderDetail.Ship_Country + "\t" + orderDetail.Cust_Email + "\t" + orderDetail.OrderId + "\t" + "" + "\t" + ""
                                );

                            //don't need to do include group class photo in datafile.txt
                            //StudentImage objGroupClassImage = clsOrders.getGroupImageIdForOrders(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), (int)orderDetail.StudentPhotoOrderId, (int)orderDetail.OrderId);
                            //if (objGroupClassImage != null)
                            //{
                            //    file.WriteLine(
                            //    objGroupClassImage.ImageName + "\t" + objGroupClassImage.FirstName + "\t" + objGroupClassImage.Lastname + "\t" + "" + "\t" +
                            //    objGroupClassImage.Teacher + "\t" + objGroupClassImage.Grade + "\t" + "" + "\t" + objGroupClassImage.Packages + "\t" + "" + "\t" + objGroupClassImage.Custom1 + "\t" +
                            //    objGroupClassImage.Ticketcode + "\t" + "" + "\t" + tempImageFolder
                            //    + "\t" + "" + "\t" + "" + "\t" + ""
                            //    + "\t" + objGroupClassImage.Address + "\t" + objGroupClassImage.City + "\t" + objGroupClassImage.State + "\t" + objGroupClassImage.Zip
                            //    + "\t" + "" + "\t" + ""
                            //    );
                            //}

                            tempViewOrder = orderDetail;
                            tempOrderId = (int)orderDetail.OrderId;
                        }
                        // missedImages.Dispose();

                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }

                if (isFROMMillers && !lstManualOrderIds.Contains((int)tempViewOrder.OrderId) && !isFreePhotoExportChecked)
                {
                    file.WriteLine(
                                    "Order.jpg" + "\t" + tempViewOrder.Cust_FirstName + "\t" + tempViewOrder.Cust_LastName + "\t" + tempViewOrder.SimplePhotoOrderID + "\t" + "" + "\t" + "" + "\t" + tempViewOrder.SchoolName
                                    + "\t" + "AY-1" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + tempViewOrder.SimplePhotoOrderID + "\t" + tempViewOrder.Ship_FirstName
                                    + "\t" + tempViewOrder.Ship_LastName + "\t" + tempViewOrder.Ship_Address + "\t" + tempViewOrder.Ship_City + "\t" + tempViewOrder.Ship_State
                                    + "\t" + tempViewOrder.Ship_PostalCode + "\t" + tempViewOrder.Ship_Country + "\t" + tempViewOrder.Cust_Email + "\t" + tempViewOrder.OrderId + "\t" + "" + "\t" + ""
                                    );
                }

                #region for total weight
                //file.WriteLine(
                //               "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + ""
                //                    + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + ""
                //                    + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + ""
                //                    + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "Total weight = " + totalweight
                //                    );
                #endregion
            }
        }
        #endregion

        /// <summary>
        /// delete the datafile from folder
        /// </summary>
        /// <param name="folderPathtxt"></param>                                  
        #region To delete Text File by Hema
        void DeleteTextFile(string folderPathtxt)
        {
            if (File.Exists(folderPathtxt))
            {
                File.Delete(folderPathtxt);
            }
        }
        #endregion

        #endregion

        private void createGroupPhotos(string type)
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string folderPathtxtgrp = "";
                string foldname = "";
                string passwd = "";
                ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();
                //double value = 0;
                //Assign Group photo as well.
                for (int grpidd = 0; grpidd < arrGroupId.Count; grpidd++)
                {
                    IEnumerable<GroupItem> _objGroupItem;
                    if (isExportAllImagesChecked != true)
                        _objGroupItem = clsGroup.getImagesFromGroupItemForSelected(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(arrGroupId[grpidd]), arrStudentImageIds);
                    else
                        _objGroupItem = clsGroup.getImagesFromGroupItem(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(arrGroupId[grpidd]));
                    foreach (GroupItem _objdt in _objGroupItem)
                    {
                        objGroupPhoto = (List<GroupClassPhoto>)clsGroup.getGroupPhotoByGrpId(db, Convert.ToInt32(arrGroupId[grpidd]));
                        if (isAppendPwdChecked == true)
                        {
                            passwd = Convert.ToString(_objdt.StudentImage.Student.Password);
                            if (passwd != "" && passwd != null)
                            {
                                passwd = "~" + passwd;
                            }
                            foldname = folderName = Convert.ToString(_objdt.StudentImage.Student.Lastname) + "_" + Convert.ToString(_objdt.StudentImage.Student.FirstName) + passwd;
                        }
                        else
                            foldname = Convert.ToString(_objdt.StudentImage.Student.Lastname) + "_" + Convert.ToString(_objdt.StudentImage.Student.FirstName);

                        foldname = foldname.Replace("\\", "");
                        foldname = clsDashBoard.SanitizeFileName(foldname);

                        # region FunctionForFolder
                        if (type == "Folder")
                        {
                            if (objGroupPhoto.Count != 0)
                            {
                                folderPathtxtgrp = alternateFullPath + "\\" + foldname;
                                if (Directory.Exists(folderPathtxtgrp))
                                {
                                    foreach (GroupClassPhoto objClsPht in objGroupPhoto)
                                    {
                                        try
                                        {
                                            string Imagenamegrp = objClsPht.StudentImage.ImageName;

                                            if (isDigitalImagesChecked)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                if (isFolderNotFound)
                                                {
                                                    return;
                                                }
                                            }
                                            else if (isReducedImagesChecked != true)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                            }
                                            else
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                            }


                                            if (File.Exists(ImageFolder + "\\" + Imagenamegrp))
                                            {
                                                File.Copy(ImageFolder + "\\" + Imagenamegrp, folderPathtxtgrp + "\\" + Imagenamegrp, false);
                                                isProgressBarVisible = Visibility.Visible;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            clsStatic.WriteExceptionLogXML(ex);
                                            MVVMMessageService.ShowMessage(ex.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(folderPathtxtgrp);
                                    foreach (GroupClassPhoto objClsPht in objGroupPhoto)
                                    {
                                        try
                                        {
                                            string Imagenamegrpp = objClsPht.StudentImage.ImageName;

                                            if (isDigitalImagesChecked)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                if (isFolderNotFound)
                                                {
                                                    return;
                                                }
                                            }
                                            else if (isReducedImagesChecked != true)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                            }
                                            else
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                            }

                                            if (File.Exists(ImageFolder + "\\" + Imagenamegrpp))
                                            {
                                                File.Copy(ImageFolder + "\\" + Imagenamegrpp, folderPathtxtgrp + "\\" + Imagenamegrpp, true);
                                                isProgressBarVisible = Visibility.Visible;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            clsStatic.WriteExceptionLogXML(ex);
                                            MVVMMessageService.ShowMessage(ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                        # endregion

                        # region FunctionRename
                        else if (type == "Rename")
                        {
                            if (objGroupPhoto.Count != 0)
                            {
                                folderPathtxtgrp = alternateFullPath;
                                if (Directory.Exists(folderPathtxtgrp))
                                {
                                    foreach (GroupClassPhoto objClsPht in objGroupPhoto)
                                    {
                                        try
                                        {
                                            string Imagenamegrp = objClsPht.StudentImage.ImageName;

                                            if (isDigitalImagesChecked)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                if (isFolderNotFound)
                                                {
                                                    return;
                                                }
                                            }
                                            else if (isReducedImagesChecked != true)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                            }
                                            else
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                            }

                                            if (File.Exists(ImageFolder + "\\" + Imagenamegrp))
                                            {
                                                File.Copy(ImageFolder + "\\" + Imagenamegrp, folderPathtxtgrp + "\\" + Imagenamegrp, true);

                                                isProgressBarVisible = Visibility.Visible;

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            clsStatic.WriteExceptionLogXML(ex);
                                            MVVMMessageService.ShowMessage(ex.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(folderPathtxtgrp);
                                    foreach (GroupClassPhoto objClsPht in objGroupPhoto)
                                    {
                                        try
                                        {
                                            string Imagenamegrpp = objClsPht.StudentImage.ImageName;

                                            if (isDigitalImagesChecked)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                if (isFolderNotFound)
                                                {
                                                    return;
                                                }
                                            }
                                            else if (isReducedImagesChecked != true)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                            }
                                            else
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                            }
                                            if (File.Exists(ImageFolder + "\\" + Imagenamegrpp))
                                            {
                                                File.Copy(ImageFolder + "\\" + Imagenamegrpp, folderPathtxtgrp + "\\" + Imagenamegrpp, true);
                                                isProgressBarVisible = Visibility.Visible;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            clsStatic.WriteExceptionLogXML(ex);
                                            MVVMMessageService.ShowMessage(ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                        # endregion

                        # region FunctionForBoth
                        else if (type == "Both")
                        {
                            if (objGroupPhoto.Count != 0)
                            {
                                string path = "";

                                if (isEachStudentChecked == true)
                                    path = alternateFullPath + "\\" + foldname;
                                else
                                    path = alternateFullPath;

                                if (Directory.Exists(path))
                                {
                                    foreach (GroupClassPhoto objClsPht in objGroupPhoto)
                                    {
                                        try
                                        {
                                            string Imagenamegrp = objClsPht.StudentImage.ImageName;

                                            if (isDigitalImagesChecked)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                if (isFolderNotFound)
                                                {
                                                    return;
                                                }
                                            }
                                            else if (isReducedImagesChecked != true)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                            }
                                            else
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                            }
                                            if (File.Exists(ImageFolder + "\\" + Imagenamegrp))
                                            {
                                                File.Copy(ImageFolder + "\\" + Imagenamegrp, path + "\\" + Imagenamegrp, true);

                                                isProgressBarVisible = Visibility.Visible;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            clsStatic.WriteExceptionLogXML(ex);
                                            MVVMMessageService.ShowMessage(ex.Message);
                                        }
                                    }

                                }
                                else
                                {
                                    Directory.CreateDirectory(path);
                                    foreach (GroupClassPhoto objClsPht in objGroupPhoto)
                                    {
                                        try
                                        {
                                            string Imagenamegrpp = objClsPht.StudentImage.ImageName;
                                            if (isDigitalImagesChecked)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                if (isFolderNotFound)
                                                {
                                                    return;
                                                }
                                            }
                                            else if (isReducedImagesChecked != true)
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                            }
                                            else
                                            {
                                                ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                            }
                                            if (File.Exists(ImageFolder + "\\" + Imagenamegrpp))
                                            {
                                                File.Copy(ImageFolder + "\\" + Imagenamegrpp, path + "\\" + Imagenamegrpp, true);
                                                isProgressBarVisible = Visibility.Visible;

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            clsStatic.WriteExceptionLogXML(ex);
                                            MVVMMessageService.ShowMessage(ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                        # endregion
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MessageBox.Show(ex.Message);
            }
        }

        async void funcCopyPhotos(object aa)
        {
            try
            {
                await EndProcessBar();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }

        }

        public void createIPTC(string _studentIDPK, string _Password)
        {
            try
            {
                if (string.IsNullOrEmpty(_Password))    //check wether student has password or not.
                {

                    ArrayList arrstudID = new ArrayList();
                    if (!arrstudID.Contains(_studentIDPK))
                    {
                        if (imgPwdWithStuID.Count != 0)
                        {
                            if (imgPwdWithStuID.ContainsKey(_studentIDPK)) { _Password = imgPwdWithStuID[_studentIDPK]; }
                            else
                                arrstudID.Add(_studentIDPK);
                        }
                        else
                            arrstudID.Add(_studentIDPK);

                    }
                    //Generate password
                    if (_Password == null)
                    {
                        GeneratePassword _GeneratePassword = new GeneratePassword(arrstudID);
                        _Password = ((GeneratePasswordViewModel)(_GeneratePassword.DataContext)).generateStudentPassword();
                    }
                    if (!imgPwdWithStuID.ContainsKey(_studentIDPK))
                        imgPwdWithStuID.Add(_studentIDPK, _Password);

                }
                string img_path = ImageFolder + "\\" + imageName;
                if (File.Exists(img_path))
                {
                    #region New IPTC Code
                    try
                    {
                        var bitmap = new Aurigma.GraphicsMill.Bitmap(img_path);
                        var settings = new Aurigma.GraphicsMill.Codecs.JpegSettings(70);

                        var iptc = new Aurigma.GraphicsMill.Codecs.IptcDictionary();
                        string keyWords = _Password; //overrides the Keywords field,if it is already contains data
                        iptc[Aurigma.GraphicsMill.Codecs.IptcDictionary.Keyword] = keyWords;
                        settings.Iptc = iptc;

                        bitmap.Save(img_path, settings);
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message + "\n\nExport View Model " + "Method Name = createIPTC() , StudentIDPk = " + _studentIDPK);
                clsStatic.WriteExceptionLogXML(ex);
            }

        }
        /// <summary>
        /// this method used to copy images (to export photos from group and orders)
        /// </summary>
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
        /// this method used to copy images (to export photos from group and orders)
        /// </summary>
        /// <returns></returns>
        async Task EndProcessBar()
        {
            try
            {
                isInProgress = true;
                String destinationPath = "";
                int studentId = 0;
                string moveImageName = "";
                string tempFolderType = "";
                List<View_Order> templstStudentImages = new List<View_Order>();
                string[] orderImagesBillingCodes = new string[] { "M", "RETOUCH", "STANDARD" };
                alternateFullPath = alternateFullPath.TrimEnd('\\');

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    _objWaitCursorViewModel = new WaitCursorViewModel();
                });

                #region if "Create a folder for each student" is selected and "Rename the image using mask" in not selected

                if ((isEachStudentChecked == true) && (isRenameFileChecked == false))      //if "Create a folder for each student" is selected and "Rename the image using mask" in not selected
                {
                    ///worker.DoWork += delegate(object s, DoWorkEventArgs args)
                    ///{
                    await Task.Run(() =>
                    {
                        #region if "export all images" is selected
                        if (isExportAllImagesChecked == true)      //if "export all images" is selected
                        {
                            ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();
                            foreach (int groupId in arrGroupId)
                            {
                                #region Threading
                                IEnumerable<GroupItem> objGroupItems = VisibleData.Count == 0 ? clsGroup.getstudentImagesByGroup(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), groupId) : VisibleData;   //get all the student images in a group
                                maxValue = objGroupItems.Count();
                                foreach (GroupItem objGroupItem in objGroupItems)
                                {

                                    try
                                    {
                                        if (isProgressCancelled)
                                        {
                                            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                            string message = "Are you sure, you want to cancel the operation?";
                                            if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                            {
                                                ///args.Cancel = true;
                                                DialogResult = false;
                                                return;
                                            }
                                            isProgressCancelled = false;
                                        }
                                        currentProgress++;

                                        //if (isSourceFolderNotFound) { break; }

                                        imageName = objGroupItem.StudentImage.ImageName;
                                        studentid = Convert.ToString(objGroupItem.StudentImage.Student.StudentID);
                                        studentIDPK = Convert.ToString(objGroupItem.StudentImage.StudentIDPK);
                                        studentFirstName = objGroupItem.StudentImage.Student.FirstName;
                                        studentLastName = objGroupItem.StudentImage.Student.Lastname;
                                        try
                                        {
                                            sequentialNumber = imageName.Substring(imageName.LastIndexOf('_') + 1).Split('.')[0];
                                        }
                                        catch { }
                                        strImgExtenssion = imageName.Substring(imageName.LastIndexOf('.')); //this will get the extenssion of the image
                                        ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                        password = Convert.ToString(objGroupItem.StudentImage.Student.Password);
                                        newPath = ImageFolder + "\\_reduced";
                                        if (isReducedImagesChecked == true)   //if we are using the reduced images
                                        {
                                            ImageFolder = newPath;
                                        }
                                        else if (isDigitalImagesChecked)
                                        {
                                            ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                            bool isFolderNotFound = digitalImageFolderNotFound();
                                            if (isFolderNotFound)
                                            {
                                                ///args.Cancel = true; worker = new BackgroundWorker();
                                                return;
                                            }
                                        }

                                        #region check source folder exist or not folder

                                        if (Directory.Exists(ImageFolder))
                                        {
                                            var fileCount = (from file in Directory.EnumerateFiles(ImageFolder, "*.jpg", SearchOption.AllDirectories)
                                                             select file).Count();
                                            if (fileCount == 0) { MessageBox.Show(errorMessages.SOURCE_FOLDER_NO_IMAGES); return; }
                                            jobName = objGroupItem.StudentImage.PhotoShoot.PhotographyJob.JobName;

                                            //insert IPTC data
                                            if (isZenfolioChecked == true)
                                            {
                                                createIPTC(studentIDPK, password);
                                            }

                                            if (isAppendPwdChecked == true)          // if Append Password is selected
                                            {
                                                password = Convert.ToString(objGroupItem.StudentImage.Student.Password);
                                                if (password != "" && password != null)
                                                {
                                                    password = "~" + password;
                                                }
                                                folderName = studentLastName + "_" + studentFirstName + password;
                                            }

                                            else
                                                folderName = studentLastName + "_" + studentFirstName;


                                            folderName = folderName.Replace("\\", "");
                                            folderName = clsDashBoard.SanitizeFileName(folderName);         //remove any special character

                                            destinationPath = alternateFullPath + "\\" + folderName;

                                            if (!Directory.Exists(destinationPath))
                                            {

                                                Directory.CreateDirectory(destinationPath);

                                                // create group photos here inside each folder

                                                IEnumerable<GroupClassPhoto> objGroupClassPhotos = clsGroup.getClassPhotoByGroup(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(objGroupItem.GroupID));
                                                foreach (GroupClassPhoto objClsPht in objGroupClassPhotos)
                                                {
                                                    try
                                                    {
                                                        string Imagenamegrp = objClsPht.StudentImage.ImageName;


                                                        if (isDigitalImagesChecked)
                                                        {
                                                            ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                                            bool isFolderNotFound = digitalImageFolderNotFound();
                                                            if (isFolderNotFound)
                                                            {
                                                                ///args.Cancel = true; worker = new BackgroundWorker();
                                                                return;
                                                            }
                                                        }
                                                        else if (isReducedImagesChecked != true)
                                                        {
                                                            ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                                        }
                                                        else
                                                        {
                                                            ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                                        }

                                                        if (File.Exists(ImageFolder + "\\" + Imagenamegrp))
                                                        {
                                                            //following method not able to rename automatically when file already exists
                                                            //File.Copy(ImageFolder + "\\" + Imagenamegrp, destinationPath + "\\" + Imagenamegrp, true);

                                                            String[] source = new String[1];
                                                            String[] dest = new String[1];
                                                            source[0] = ImageFolder + "\\" + Imagenamegrp;
                                                            dest[0] = destinationPath + "\\" + Imagenamegrp;
                                                            fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                                            //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                                            fo.SourceFiles = source;
                                                            fo.DestFiles = dest;
                                                            fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                                            bool RetVal = fo.DoOperation();

                                                            isProgressBarVisible = Visibility.Visible;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        clsStatic.WriteExceptionLogXML(ex);
                                                        MVVMMessageService.ShowMessage(ex.Message);
                                                    }
                                                }

                                            }
                                            if (isReducedImagesChecked == true)
                                            {
                                                ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\" + "_reduced";
                                            }
                                            else if (isDigitalImagesChecked)
                                            {
                                                ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder + "\\_DD";
                                            }
                                            else
                                            {
                                                ImageFolder = objGroupItem.StudentImage.PhotoShoot.ImageFolder;
                                            }

                                            if (isZenfolioChecked == false)
                                            {
                                                //if (isAddImgSerialNo == true)
                                                //    MoveImageName = studentLastName + "_" + studentFirstName + "_" + sequentialNumber + strImgExtenssion;
                                                //else
                                                //    MoveImageName = studentLastName + "_" + studentFirstName + strImgExtenssion;

                                                if (isAddImgSerialNo == true)
                                                    MoveImageName = imageName.Remove(imageName.LastIndexOf('.')) + "_" + sequentialNumber + strImgExtenssion;
                                                else
                                                    MoveImageName = imageName;
                                            }

                                            MoveImageName = MoveImageName.Replace("\\", "");
                                            MoveImageName = clsDashBoard.SanitizeFileName(MoveImageName);
                                            // Need to move photos from this location as per by lastname and firstname
                                            if (File.Exists(ImageFolder + "\\" + imageName))
                                            {

                                                //File.Copy(sourceImagePath, destImagePath, true);
                                                //File.Copy(ImageFolder + "\\" + imageName, destinationPath + "\\" + MoveImageName, true);

                                                String[] source = new String[1];
                                                String[] dest = new String[1];
                                                source[0] = ImageFolder + "\\" + imageName;
                                                dest[0] = destinationPath + "\\" + MoveImageName;
                                                fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                                //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                                fo.SourceFiles = source;
                                                fo.DestFiles = dest;
                                                fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                                bool RetVal = fo.DoOperation();

                                                isProgressBarVisible = Visibility.Visible;
                                            }
                                        }
                                        else
                                        {
                                            //MessageBox.Show(errorMessages.SOURCE_FOLDER_DOESNOT_EXISTS);
                                            isSourceFolderNotFound = true;
                                        }
                                        #endregion
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
                        #endregion

                        #region if only for selected StudentImages
                        else
                        {
                            // Need to get imagename, lastname and firstname of student image
                            ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();
                            DataTable _selectedStudentImages = new DataTable();
                            //lstStudentImages = clsDashBoard.GetStudentsByImageIdArray(db, arrStudentImageIds);

                            if (isFromOrders)
                            {
                                if (isExportSelectedChecked)
                                {
                                    _selectedStudentImages = Conversions.ToDataTable(clsOrders.getOrderImagesFromOrderId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempSelectedOrderIds));
                                }
                                else
                                {
                                    _selectedStudentImages = Conversions.ToDataTable(clsOrders.getOrderImagesFromOrderId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allOrderIds));
                                }
                                //using studentphotoorderid instead of student image id //changed by mohan
                                //Commented by hema              //_selectedStudentImages = Conversions.ToDataTable(clsOrders.getOrderImagesFromOrderId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allOrderIds));
                                maxValue = _selectedStudentImages.Rows.Count;
                                //Added by hema
                                if (isExportSelectedChecked)
                                {
                                    templstStudentImages = new List<View_Order>(clsOrders.getOrderImagesFromOrderId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempSelectedOrderIds));
                                }
                                else
                                {
                                    templstStudentImages = new List<View_Order>(clsOrders.getOrderImagesFromOrderId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allOrderIds));
                                }
                                //commented by hema  //templstStudentImages = new List<View_Order>(clsOrders.getOrderImagesFromOrderId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allOrderIds));
                            }
                            else
                            {
                                _selectedStudentImages = clsDashBoard.getImageNameLastNameFirstName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentImageIds);//changed by mohan
                                maxValue = arrStudentImageIds.Count();
                            }

                            //maxValue = _selectedStudentImages.Rows.Count; _objdt.Rows.Count; //Changed By Mohan 29-5-2015
                            //Orders can have duplicate student images i.e, one student image can be present in more than one order
                            //so instead of looping through results ,loop through all student image ids
                            //it won't be a problem for group export because , in group export "arrStudentImageIds" will insert only unique student image ids

                            for (int k = 0; k < maxValue; k++)
                            {
                                try
                                {
                                    if (isProgressCancelled)
                                    {
                                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                        string message = "Are you sure, you want to cancel the operation?";
                                        if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                        {
                                            ///args.Cancel = true;
                                            DialogResult = false;
                                            return;
                                        }
                                        isProgressCancelled = false;
                                    }
                                    currentProgress++;
                                    DataRow dr;
                                    //if (isSourceFolderNotFound) { break; }

                                    if (isFromOrders)
                                    {
                                        dr = _selectedStudentImages.Rows[k];
                                        if (dr == null)
                                        {
                                            orderItemNotFound("Student image id associated with student photo order id ", arrStudentImageIds[k]);
                                            continue;
                                        }
                                        studentId = Convert.ToInt32(dr["StudentImageId"]); //stduent image id
                                    }
                                    else
                                    {
                                        dr = _selectedStudentImages.Rows
                                      .Cast<DataRow>()
                                      .Where(x => Convert.ToInt32(x["ID"]) == arrStudentImageIds[k]).FirstOrDefault();

                                        if (dr == null)
                                        {
                                            orderItemNotFound("Student image id ", arrStudentImageIds[k]);
                                            continue;
                                        }
                                        studentId = Convert.ToInt32(dr["ID"]);
                                        password = Convert.ToString(dr["Password"]);
                                    }

                                    imageName = Convert.ToString(dr["ImageName"]);

                                    //string method = "copy";
                                    sequentialNumber = imageName.Substring(imageName.LastIndexOf('_') + 1, imageName.LastIndexOf('.') - imageName.LastIndexOf('_') - 1);
                                    strImgExtenssion = imageName.Substring(imageName.LastIndexOf('.')); //this will get the extenssion of the image

                                    int PhotoId = Convert.ToInt32(dr["PhotoShootID"]);
                                    objImageFolder = clsDashBoard.getImageFolder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), PhotoId);
                                    ImageFolder = objImageFolder.ImageFolder;
                                    jobName = objImageFolder.PhotographyJob.JobName;

                                    newPath = objImageFolder.ImageFolder + "\\_reduced\\";
                                    ImageFolder = objImageFolder.ImageFolder;
                                    if (isReducedImagesChecked == true)
                                    {
                                        ImageFolder = newPath;
                                    }
                                    else if (isDigitalImagesChecked)
                                    {
                                        ImageFolder = objImageFolder.ImageFolder + "\\_DD";

                                        bool isFolderNotFound = digitalImageFolderNotFound();
                                        if (isFolderNotFound)
                                        {
                                            ///args.Cancel = true; worker = new BackgroundWorker();
                                            return;
                                        }
                                    }

                                    //insert IPTC data
                                    if (isZenfolioChecked == true)
                                    {
                                        createIPTC(studentId.ToString(), password);
                                    }

                                    if (!isFromOrders)
                                    {
                                        //Check if password checkbox is checked..
                                        if (isAppendPwdChecked == true)
                                        {
                                            password = Convert.ToString(dr["Password"]);
                                            if (password != "" && password != null)
                                            {
                                                password = "~" + password;
                                            }
                                            folderName = Convert.ToString(dr["lastname"]) + "_" + Convert.ToString(dr["firstname"]) + password;
                                        }
                                        else
                                            folderName = Convert.ToString(dr["lastname"]) + "_" + Convert.ToString(dr["firstname"]);
                                    }
                                    else
                                    {
                                        int OrderId = Convert.ToInt32(dr["OrderId"]);
                                        bool retouch = clsOrders.isRetouchTrue(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), OrderId, studentId);   //checking for premium retouch
                                        List<int> richmond = clsOrders.isRichmondTrue(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
                                        if (richmond.Contains(Convert.ToInt32(dr["OrderId"])))
                                        {
                                            folderName = "R";
                                        }
                                        else if (retouch == true)
                                        {
                                            folderName = "RETOUCH";
                                        }
                                        else
                                        {
                                            retouch = false;
                                            retouch = clsOrders.isStandardTrue(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), OrderId, studentId);
                                            if (retouch == true)
                                            {
                                                //Commented by Mohan as per Don Wiid suggestion
                                                //folderName = "STANDARD";  //At the moment, any photos with this action is being exported to a STANDARD folder. This is wrong - is should go to M folder, since Millers apply the standard retouching.
                                                folderName = "M";
                                            }
                                            else
                                            {
                                                int studentPhotoOrderId = Convert.ToInt32(dr["studentPhotoOrderId"]);
                                                folderName = dicOrderItemsBillingCode[studentPhotoOrderId];
                                            }
                                        }
                                        retouch = false;
                                        tempFolderType = folderName;
                                    }

                                    folderName = folderName.Replace("\\", "");
                                    folderName = clsDashBoard.SanitizeFileName(folderName);

                                    if (!isFromOrders)
                                    {
                                        //if (isAddImgSerialNo == true)
                                        //    moveImageName = Convert.ToString(dr["lastname"]) + "_" + Convert.ToString(dr["firstname"]) + "_" + sequentialNumber + strImgExtenssion;
                                        //else
                                        //    moveImageName = Convert.ToString(dr["lastname"]) + "_" + Convert.ToString(dr["firstname"]) + strImgExtenssion;

                                        if (isAddImgSerialNo == true)
                                            moveImageName = imageName.Remove(imageName.LastIndexOf('.')) + "_" + sequentialNumber + strImgExtenssion;
                                        else
                                            moveImageName = imageName;
                                    }
                                    else
                                    {
                                        moveImageName = dr["imagename"].ToString();
                                    }

                                    destinationPath = alternateFullPath + "\\" + folderName;
                                    moveImageName = moveImageName.Replace("\\", "");
                                    moveImageName = clsDashBoard.SanitizeFileName(moveImageName);

                                    #region check the existence of source folder
                                    if (Directory.Exists(ImageFolder))
                                    {
                                        var fileCount = (from file in Directory.EnumerateFiles(ImageFolder, "*.jpg", SearchOption.AllDirectories)
                                                         select file).Count();
                                        if (fileCount == 0) { MessageBox.Show(errorMessages.SOURCE_FOLDER_NO_IMAGES); return; }
                                        if (!Directory.Exists(destinationPath))
                                        {
                                            Directory.CreateDirectory(destinationPath);
                                            // create group photos here inside each folder
                                        }
                                        if (File.Exists(ImageFolder + "\\" + imageName))
                                        {
                                            #region Orders
                                            if (isFromOrders)
                                            {
                                                if (!File.Exists(destinationPath + "\\" + "Order.jpg") && (orderImagesBillingCodes.Contains(tempFolderType)) && !isFreePhotoExportChecked)
                                                {
                                                    string imagesPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                                                    //File.Copy(sourceImagePath, destImagePath, true);
                                                    //File.Copy(imagesPath + "\\TemplatesCoordinates\\Order.jpg", destinationPath + "\\" + "Order.jpg", true);

                                                    fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                                    //fo.OwnerWindow = new WindowInteropHelper(this).Handle;        //#Mohan
                                                    fo.SourceFiles = new string[] { imagesPath + "\\TemplatesCoordinates\\Order.jpg" };
                                                    fo.DestFiles = new string[] { destinationPath + "\\" + "Order.jpg" };
                                                    fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                                    bool tempRetVal = fo.DoOperation();
                                                }
                                                if (File.Exists(destinationPath + "\\" + moveImageName)) { continue; }

                                                #region For OrderGroupClassPhoto

                                                StudentImage objGroupClassImage = clsOrders.getGroupImageIdForOrders(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), (int)templstStudentImages[k].StudentPhotoOrderId, (int)templstStudentImages[k].OrderId);

                                                if (objGroupClassImage == null) { }
                                                else
                                                {
                                                    try
                                                    {
                                                        string groupImageFolder = "";
                                                        string Imagenamegrp = objGroupClassImage.ImageName;
                                                        if (!File.Exists(destinationPath + "\\" + Imagenamegrp)) //{ continue; }
                                                        {
                                                            if (File.Exists(destinationPath + "\\" + Imagenamegrp)) { continue; }

                                                            if (isDigitalImagesChecked)
                                                            {
                                                                groupImageFolder = objGroupClassImage.PhotoShoot.ImageFolder + "\\_DD";

                                                                bool isFolderNotFound = digitalImageFolderNotFound();
                                                                if (isFolderNotFound)
                                                                {
                                                                    ///args.Cancel = true; worker = new BackgroundWorker();
                                                                    return;
                                                                }
                                                            }
                                                            else if (isReducedImagesChecked != true)
                                                            {
                                                                groupImageFolder = objGroupClassImage.PhotoShoot.ImageFolder;
                                                            }

                                                            else
                                                            {
                                                                groupImageFolder = objGroupClassImage.PhotoShoot.ImageFolder + "\\_reduced";
                                                            }

                                                            //int PhotoId1 = (int)objGroupClassImage.PhotoShootID;
                                                            //PhotoShoot objImageFolder1 = clsDashBoard.getImageFolder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), PhotoId1);
                                                            //string ImageFolder1 = objImageFolder1.ImageFolder;
                                                            //jobName = objImageFolder.PhotographyJob.JobName;


                                                            if (File.Exists(groupImageFolder + "\\" + Imagenamegrp))
                                                            {

                                                                //File.Copy(sourceImagePath, destImagePath, true);
                                                                //File.Copy(groupImageFolder + "\\" + Imagenamegrp, destinationPath + "\\" + Imagenamegrp, true);

                                                                String[] source1 = new String[1];
                                                                String[] dest1 = new String[1];
                                                                source1[0] = groupImageFolder + "\\" + Imagenamegrp;
                                                                dest1[0] = destinationPath + "\\" + Imagenamegrp;
                                                                fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                                                //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                                                fo.SourceFiles = source1;
                                                                fo.DestFiles = dest1;
                                                                fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                                                bool RetVal1 = fo.DoOperation();

                                                                isProgressBarVisible = Visibility.Visible;
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        clsStatic.WriteExceptionLogXML(ex);
                                                        MVVMMessageService.ShowMessage(ex.Message);
                                                    }
                                                    //}
                                                    objGroupClassPhotos = null;
                                                }
                                                #endregion
                                            }
                                            #endregion

                                            #region for GroupClassPhoto (for group export)
                                            else
                                            {
                                                if (objGroupClassPhotos.Count == 0)
                                                {
                                                    allGroupId = (int)arrGroupId[0];
                                                    objGroupClassPhotos = clsGroup.getClassPhotoByGroup(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(allGroupId));
                                                }
                                                foreach (GroupClassPhoto objClsPht in objGroupClassPhotos)
                                                {
                                                    try
                                                    {
                                                        string Imagenamegrp = objClsPht.StudentImage.ImageName;

                                                        if (File.Exists(destinationPath + "\\" + Imagenamegrp)) { continue; }

                                                        if (isDigitalImagesChecked)
                                                        {
                                                            ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                                            bool isFolderNotFound = digitalImageFolderNotFound();
                                                            if (isFolderNotFound)
                                                            {
                                                                ///args.Cancel = true; worker = new BackgroundWorker();
                                                                return;
                                                            }
                                                        }
                                                        else if (isReducedImagesChecked != true)
                                                        {
                                                            ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                                        }

                                                        else
                                                        {
                                                            ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                                        }

                                                        if (File.Exists(ImageFolder + "\\" + Imagenamegrp))
                                                        {
                                                            //following method not able to rename automatically when file already exists
                                                            //File.Copy(ImageFolder + "\\" + Imagenamegrp, destinationPath + "\\" + Imagenamegrp, true);

                                                            String[] source1 = new String[1];
                                                            String[] dest1 = new String[1];
                                                            source1[0] = ImageFolder + "\\" + Imagenamegrp;
                                                            dest1[0] = destinationPath + "\\" + Imagenamegrp;
                                                            fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                                            //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                                            fo.SourceFiles = source1;
                                                            fo.DestFiles = dest1;
                                                            fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                                            bool RetVal1 = fo.DoOperation();

                                                            isProgressBarVisible = Visibility.Visible;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        clsStatic.WriteExceptionLogXML(ex);
                                                        MVVMMessageService.ShowMessage(ex.Message);
                                                    }
                                                }
                                            }
                                            #endregion

                                            //    if (File.Exists(ImageFolder )) { File.Delete(destinationPath); }
                                            //File.Copy(ImageFolder + "\\" + imageName, destinationPath + "\\" + moveImageName, true);

                                            String[] source = new String[1];
                                            String[] dest = new String[1];
                                            source[0] = ImageFolder + "\\" + imageName;
                                            dest[0] = destinationPath + "\\" + moveImageName;
                                            fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                            //fo.OwnerWindow = new WindowInteropHelper(this).Handle;        //#Mohan
                                            fo.SourceFiles = source;
                                            fo.DestFiles = dest;
                                            fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                            bool RetVal = fo.DoOperation();

                                            isProgressBarVisible = Visibility.Visible;
                                        }
                                    }
                                    else
                                    {
                                        //MessageBox.Show(errorMessages.SOURCE_FOLDER_DOESNOT_EXISTS);
                                        isSourceFolderNotFound = true;
                                    }
                                    #endregion
                                }

                                catch (Exception ex)
                                {
                                    clsStatic.WriteExceptionLogXML(ex);
                                    MVVMMessageService.ShowMessage(ex.Message);
                                }
                            }
                            //why we need this???  Commented by #Mohan
                            //createGroupPhotos("Folder");
                        }
                        #endregion

                    });
                    createTextFile();
                    isProgressBarVisible = Visibility.Hidden;

                    string messagecl = "";
                    if (isSourceFolderNotFound) { isBottomButtonsEnabled = true; isCreateDataFileEnabled = true; isMissedImages = true; }
                    if (!isMissedImages)
                    {
                        if (!string.IsNullOrEmpty(missedImagesfolderPath)) { File.Delete(missedImagesfolderPath); }
                        if (isMissingOrderImages && isFromOrders)
                        {
                            messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                        }
                        else
                        {

                            messagecl = errorMessages.AFTER_COPY_OPEN_LOCATION_CONFIRMATION;
                        }
                    }
                    else
                    {
                        messagecl = "Some images were not found in the source folder. For details please review Missingimages.txt in the destination folder.\nDo you want to open the image folder?";
                        if (isMissingOrderImages && isFromOrders)
                        {
                            messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                        }
                    }

                    string captioncl = "Information";
                    MessageBoxButton buttonss = MessageBoxButton.YesNo;
                    MessageBoxImage iconn = MessageBoxImage.Question;
                    if (MessageBox.Show(messagecl, captioncl, buttonss, iconn) == MessageBoxResult.Yes)
                        Process.Start(alternateFullPath);

                    isSave = true; isMissingOrderImages = false;
                    DialogResult = false;
                    //}

                    ///};


                    //background worker support cancellation
                    ///worker.WorkerSupportsCancellation = true;

                    // Configure the function to run when completed
                    ///worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                    // Launch the worker
                    ///worker.RunWorkerAsync();
                    exportOrdersCompleted();

                }
                #endregion

                #region if "Create a folder for each student" is false and "Rename the image using mask" is also false
                else if ((isEachStudentChecked == false) && (isRenameFileChecked == false) && isImageQuixChecked == false)          //if "Create a folder for each student" is false and "Rename the image using mask" is also false
                {
                    ///worker.DoWork += delegate(object s, DoWorkEventArgs args)
                    ///{
                    ///
                    ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();
                    // Need to get imagename, lastname and firstname of student image
                    DataTable _objStudentImageDetails = new DataTable();
                    _objStudentImageDetails = clsDashBoard.getImageNameLastNameFirstName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentImageIds);

                    //{
                    //    _objStudentImageDetails = clsDashBoard.getGroupNameAndGroupImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrGroupId);
                    //}

                    maxValue = arrStudentImageIds.Count();
                    //maxValue = _objStudentImageDetails.Rows.Count; //Changed By Mohan 29-5-2015
                    //Orders can have duplicate student images i.e, one student image can be present in more than one order
                    //so instead of looping through results ,loop through all student image ids
                    //it won't be a problem for group export because , in group export "arrStudentImageIds" will insert only unique student image ids
                    await Task.Run(() =>
                    {
                        #region
                        for (int k = 0; k < maxValue; k++)
                        {
                            try
                            {
                                if (isProgressCancelled)
                                {
                                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                    string message = "Are you sure, you want to cancel the operation?";
                                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                    {
                                        ///args.Cancel = true;
                                        DialogResult = false;
                                        return;
                                    }
                                    isProgressCancelled = false;
                                }
                                currentProgress++;
                                //if (isSourceFolderNotFound) { break; }    //this line leads to problem when only one image has missed out of multiple images. //Commented by Mohan

                                DataRow dr = _objStudentImageDetails.Rows
                                      .Cast<DataRow>()
                                      .Where(x => Convert.ToInt32(x["ID"]) == arrStudentImageIds[k]).FirstOrDefault();

                                if (dr == null)
                                {
                                    orderItemNotFound("Student image id ", arrStudentImageIds[k]);
                                    continue;
                                }

                                string MoveImageName = "";
                                string studentID = Convert.ToString(dr["StudentID"]);
                                imageName = Convert.ToString(dr["ImageName"]);
                                string studentid = Convert.ToString(dr["studentidpk"]);
                                string studentname = Convert.ToString(dr["FirstName"]);
                                password = Convert.ToString(dr["Password"]);
                                //string method = "copy";
                                sequentialNumber = imageName.Substring(imageName.LastIndexOf('_') + 1, imageName.LastIndexOf('.') - imageName.LastIndexOf('_') - 1);
                                strImgExtenssion = imageName.Substring(imageName.LastIndexOf('.')); //this will get the extension of the image

                                int PhotoId = Convert.ToInt32(dr["PhotoShootID"]);
                                objImageFolder = clsDashBoard.getImageFolder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), PhotoId);

                                ImageFolder = objImageFolder.ImageFolder;

                                newPath = objImageFolder.ImageFolder + "\\_reduced\\";
                                ImageFolder = objImageFolder.ImageFolder;
                                if (isReducedImagesChecked == true)
                                {
                                    ImageFolder = newPath;
                                }
                                else if (isDigitalImagesChecked)
                                {
                                    ImageFolder = objImageFolder.ImageFolder + "\\_DD";

                                    bool isFolderNotFound = digitalImageFolderNotFound();
                                    if (isFolderNotFound)
                                    {
                                        ///args.Cancel = true; worker = new BackgroundWorker();
                                        return;
                                    }
                                }

                                if (Directory.Exists(ImageFolder))
                                {
                                    jobName = objImageFolder.PhotographyJob.JobName;

                                    //insert IPTC data
                                    if (isZenfolioChecked == true)
                                    {
                                        createIPTC(studentid, password);
                                    }

                                    //if (isAddImgSerialNo == true)
                                    //    MoveImageName = Convert.ToString(dr["lastname"]) + "_" + Convert.ToString(dr["firstname"]) + "_" + sequentialNumber + strImgExtenssion;
                                    //else
                                    //    MoveImageName = Convert.ToString(dr["lastname"]) + "_" + Convert.ToString(dr["firstname"]) + strImgExtenssion;

                                    if (isAddImgSerialNo == true)
                                        MoveImageName = imageName.Remove(imageName.LastIndexOf('.')) + "_" + sequentialNumber + strImgExtenssion;
                                    else
                                        MoveImageName = imageName;

                                    folderName = folderName.Replace("\\", "");
                                    folderName = clsDashBoard.SanitizeFileName(folderName);

                                    destinationPath = alternateFullPath + "\\" + folderName;
                                    //}
                                    MoveImageName = MoveImageName.Replace("\\", "");
                                    MoveImageName = clsDashBoard.SanitizeFileName(MoveImageName);

                                    // Need to move photos from this location as per by lastname and firstname
                                    if (File.Exists(ImageFolder + "\\" + imageName))
                                    {

                                        //File.Copy(sourceImagePath, destImagePath, true);
                                        //File.Copy(ImageFolder + "\\" + imageName, destinationPath + "\\" + MoveImageName, true);

                                        String[] source = new String[1];
                                        String[] dest = new String[1];
                                        source[0] = ImageFolder + "\\" + imageName;
                                        if (isZenfolioChecked && !isFromOrders)
                                            dest[0] = destinationPath + "\\" + studentID + ".jpg";
                                        else
                                            dest[0] = destinationPath + "\\" + MoveImageName;
                                        fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                        //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                        fo.SourceFiles = source;
                                        fo.DestFiles = dest;
                                        fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                        bool RetVal = fo.DoOperation();


                                        isProgressBarVisible = Visibility.Visible;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show(errorMessages.SOURCE_FOLDER_DOESNOT_EXISTS);
                                    isSourceFolderNotFound = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }
                        #endregion

                        createGroupPhotos("Both");
                        createTextFile();
                        isProgressBarVisible = Visibility.Hidden;

                        string messagecl = "";
                        if (isSourceFolderNotFound) { isBottomButtonsEnabled = true; isCreateDataFileEnabled = true; isMissedImages = true; }

                        if (!isMissedImages)
                        {
                            File.Delete(missedImagesfolderPath);
                            if (isMissingOrderImages && isFromOrders)
                            {
                                messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                            }
                            else
                            {
                                messagecl = errorMessages.AFTER_COPY_OPEN_LOCATION_CONFIRMATION;
                            }
                        }
                        else
                        {
                            messagecl = "Some images were not found in the source folder. For details please review Missingimages.txt in the destination folder.\n Do you want to open the image folder?";
                            if (isMissingOrderImages && isFromOrders)
                            {
                                messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                            }
                        }

                        string captioncl = "Confirmation";
                        MessageBoxButton buttonss = MessageBoxButton.YesNo;
                        MessageBoxImage iconn = MessageBoxImage.Question;
                        if (MessageBox.Show(messagecl, captioncl, buttonss, iconn) == MessageBoxResult.Yes)
                            Process.Start(alternateFullPath);

                        isSave = true; isMissingOrderImages = false;
                        isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                        DialogResult = false;
                    });
                    ///};

                    //our background worker support cancellation
                    ///worker.WorkerSupportsCancellation = true;

                    // Configure the function to run when completed
                    ///worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                    // Launch the worker
                    ///worker.RunWorkerAsync();
                    exportOrdersCompleted();
                }
                #endregion

                #region if "Create a folder for each student" is false and "Rename the image using mask" is also false and ImageQuix is true
                else if ((isEachStudentChecked == false) && (isRenameFileChecked == false) && isImageQuixChecked == true)   /// Image quix is selected
                {
                    ///worker.DoWork += delegate(object s, DoWorkEventArgs args)
                    ///{

                    await Task.Run(() =>
                    {
                        #region if "export all images" is selected
                        ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();
                        //foreach (int groupId in arrGroupId)
                        //{
                        #region Threading
                        DataTable objGroupItems = new DataTable();
                        if (isExportAllImagesChecked == true)
                            objGroupItems = clsDashBoard.getGroupNameAndGroupImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrGroupId, true, arrStudentImageIds);
                        else
                            objGroupItems = clsDashBoard.getGroupNameAndGroupImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrGroupId, false, arrStudentImageIds);

                        //IEnumerable<GroupItem> objGroupItems = VisibleData.Count == 0 ? clsGroup.getstudentImagesByGroupID(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), groupId) : VisibleData;   //get all the student images in a group
                        maxValue = objGroupItems.Rows.Count;
                        destinationPath = alternateFullPath + "\\" + txtTitle;

                        foreach (DataRow objGroupItem in objGroupItems.Rows)
                        {

                            try
                            {
                                if (isProgressCancelled)
                                {
                                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                    string message = "Are you sure, you want to cancel the operation?";
                                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                    {
                                        ///args.Cancel = true;
                                        DialogResult = false;
                                        return;
                                    }
                                    isProgressCancelled = false;
                                }
                                currentProgress++;

                                //if (isSourceFolderNotFound) { break; }

                                imageName = objGroupItem["ImageName"].ToString();
                                try
                                {
                                    sequentialNumber = imageName.Substring(imageName.LastIndexOf('_') + 1).Split('.')[0];
                                }
                                catch { }
                                strImgExtenssion = imageName.Substring(imageName.LastIndexOf('.')); //this will get the extenssion of the image
                                ImageFolder = objGroupItem["ImageFolder"].ToString();
                                //password = objGroupItem["Password"].ToString();
                                newPath = ImageFolder + "\\_reduced";
                                if (isReducedImagesChecked == true)   //if we are using the reduced images
                                {
                                    ImageFolder = newPath;
                                }
                                else if (isDigitalImagesChecked)
                                {
                                    ImageFolder = objGroupItem["ImageFolder"].ToString() + "\\_DD";

                                    bool isFolderNotFound = digitalImageFolderNotFound();
                                    if (isFolderNotFound)
                                    {
                                        ///args.Cancel = true; worker = new BackgroundWorker();
                                        return;
                                    }
                                }

                                #region check source folder exist or not folder

                                if (Directory.Exists(ImageFolder))
                                {
                                    var fileCount = (from file in Directory.EnumerateFiles(ImageFolder, "*.jpg", SearchOption.AllDirectories)
                                                     select file).Count();
                                    if (fileCount == 0) { MessageBox.Show(errorMessages.SOURCE_FOLDER_NO_IMAGES); return; }
                                    //jobName = objGroupItem["JobName"].ToString();

                                    folderName = objGroupItem["GroupName"].ToString();

                                    folderName = folderName.Replace("\\", "");
                                    folderName = clsDashBoard.SanitizeFileName(folderName);         //remove any special character


                                    if (!Directory.Exists(destinationPath))
                                    {

                                        Directory.CreateDirectory(destinationPath);
                                    }
                                    if (isReducedImagesChecked == true)
                                    {
                                        ImageFolder = objGroupItem["ImageFolder"].ToString() + "\\" + "_reduced";
                                    }
                                    else if (isDigitalImagesChecked)
                                    {
                                        ImageFolder = objGroupItem["ImageFolder"].ToString() + "\\_DD";
                                    }
                                    else
                                    {
                                        ImageFolder = objGroupItem["ImageFolder"].ToString();
                                    }

                                    MoveImageName = imageName;

                                    MoveImageName = MoveImageName.Replace("\\", "");
                                    MoveImageName = clsDashBoard.SanitizeFileName(MoveImageName);
                                    // Need to move photos from this location as per by lastname and firstname
                                    if (File.Exists(ImageFolder + "\\" + imageName))
                                    {

                                        //File.Copy(sourceImagePath, destImagePath, true);
                                        //File.Copy(ImageFolder + "\\" + imageName, destinationPath + "\\" + MoveImageName, true);

                                        String[] source = new String[1];
                                        String[] dest = new String[1];
                                        source[0] = ImageFolder + "\\" + imageName;
                                        dest[0] = destinationPath + "\\" + MoveImageName;
                                        fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                        //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                        fo.SourceFiles = source;
                                        fo.DestFiles = dest;
                                        fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                        bool RetVal = fo.DoOperation();

                                        isProgressBarVisible = Visibility.Visible;
                                    }
                                }
                                else
                                {
                                    //MessageBox.Show(errorMessages.SOURCE_FOLDER_DOESNOT_EXISTS);
                                    isSourceFolderNotFound = true;
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }

                        #endregion
                            // }
                        }

                        // create group photos here inside each folder                        
                        #region GroupClassPhoto
                        if (iqGroupClassPhotos.Count > 0)
                        {
                            foreach (GroupClassPhoto objClsPht in iqGroupClassPhotos)
                            {
                                try
                                {
                                    string Imagenamegrp = objClsPht.StudentImage.ImageName;

                                    if (isDigitalImagesChecked)
                                    {
                                        ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_DD";

                                        bool isFolderNotFound = digitalImageFolderNotFound();
                                        if (isFolderNotFound)
                                        {
                                            ///args.Cancel = true; worker = new BackgroundWorker();
                                            return;
                                        }
                                    }
                                    else if (isReducedImagesChecked != true)
                                    {
                                        ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                    }
                                    else
                                    {
                                        ImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder + "\\_reduced";
                                    }

                                    if (File.Exists(ImageFolder + "\\" + Imagenamegrp))
                                    {
                                        //following method not able to rename automatically when file already exists
                                        //File.Copy(ImageFolder + "\\" + Imagenamegrp, destinationPath + "\\" + Imagenamegrp, true);

                                        String[] source = new String[1];
                                        String[] dest = new String[1];
                                        source[0] = ImageFolder + "\\" + Imagenamegrp;
                                        dest[0] = destinationPath + "\\" + Imagenamegrp;
                                        fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                        //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                        fo.SourceFiles = source;
                                        fo.DestFiles = dest;
                                        fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                        bool RetVal = fo.DoOperation();

                                        isProgressBarVisible = Visibility.Visible;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    clsStatic.WriteExceptionLogXML(ex);
                                    MVVMMessageService.ShowMessage(ex.Message);
                                }
                            }
                        }
                        #endregion

                        #endregion

                    });

                    #region after Images moved
                    createTextFile();

                    isProgressBarVisible = Visibility.Hidden;

                    string messagecl = "";
                    if (isSourceFolderNotFound) { isBottomButtonsEnabled = true; isCreateDataFileEnabled = true; isMissedImages = true; }

                    if (studentsWithoutPasswords.Length > 1)
                    {
                        studentsWithoutPasswords = studentsWithoutPasswords.Substring(0, studentsWithoutPasswords.Length - 1);
                        MVVMMessageService.ShowMessage("Following student(s) doesn't have password \n" + studentsWithoutPasswords);
                    }
                    if (!isMissedImages)
                    {

                        if (!string.IsNullOrEmpty(missedImagesfolderPath)) { File.Delete(missedImagesfolderPath); }
                        if (isMissingOrderImages && isFromOrders)
                        {
                            messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                        }
                        else
                        {
                            //MVVMMessageService.ShowMessage("Export complete. The following elements are unresolved:" + Environment.NewLine + "galleryType: null, eventDate: null, expirationDate: null");
                            messagecl = errorMessages.AFTER_COPY_OPEN_LOCATION_CONFIRMATION;
                        }
                    }
                    else
                    {
                        messagecl = "Some images were not found in the source folder. For details please review Missingimages.txt in the destination folder.\nDo you want to open the image folder?";
                        if (isMissingOrderImages && isFromOrders)
                        {
                            messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                        }
                    }

                    string captioncl = "Information";
                    MessageBoxButton buttonss = MessageBoxButton.YesNo;
                    MessageBoxImage iconn = MessageBoxImage.Question;
                    if (MessageBox.Show(messagecl, captioncl, buttonss, iconn) == MessageBoxResult.Yes)
                        Process.Start(alternateFullPath);

                    isSave = true; isMissingOrderImages = false;
                    DialogResult = false;

                    exportOrdersCompleted();
                    #endregion

                }
                #endregion

                #region if "Rename the image using mask" is true and "Create a folder for each student" is false
                else if ((isRenameFileChecked == true) && (isEachStudentChecked == false))
                {
                    ///worker.DoWork += delegate(object s, DoWorkEventArgs args)
                    ///{
                    ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();
                    string studentid = "";
                    string studentname = "";
                    //string method = "copy";
                    DataTable _objmskDetails = new DataTable();
                    DataTable _objStudentImages = new DataTable();
                    _objStudentImages = clsDashBoard.getImageNameLastNameFirstName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentImageIds);
                    _objmskDetails = clsDashBoard.GetMaskDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedMask.MaskID);
                    maxValue = arrStudentImageIds.Count();

                    //maxValue = _objStudentImages.Rows.Count; //Changed By Mohan 29-5-2015
                    //Orders can have duplicate student images i.e, one student image can be present in more than one order
                    //so instead of looping through results ,loop through all student image ids
                    //it won't be a problem for group export because , in group export "arrStudentImageIds" will insert only unique student image ids

                    // Need to create a dynamic query
                    string query = "";
                    await Task.Run(() =>
                    {
                        for (int j = 0; j < _objmskDetails.Rows.Count; j++)
                        {
                            try
                            {
                                if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "school name" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "schoolname")
                                    query = query + "s.schoolname" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "grade")
                                    query = query + "st.grade" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "teacher")
                                    query = query + "st.teacher" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "first name" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "firstname")
                                    query = query + "st.firstname" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "last name" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "lastname")
                                    query = query + "st.lastname" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "password")
                                    query = query + "st.Password" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "student id" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "studentid")
                                    query = query + "st.StudentID" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "image number" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "imagenumber")
                                    query = query + "sm.ImageNumber" + ",";
                                else
                                    query = query + "'" + _objmskDetails.Rows[j]["MaskDetail1"].ToString() + "' as Label" + ",";
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }
                        query = query.Substring(0, query.Length - 1);
                        string OrignalQuery = "";
                        DataSet ds = new DataSet();
                        SqlParameter[] param = new SqlParameter[1];
                        param[0] = new SqlParameter("@ID", schoolId);
                        String imageId = "";
                        // Need to get lastname and firstname of student image
                        for (int i = 0; i < arrStudentImageIds.Count(); i++)
                        {
                            imageId += arrStudentImageIds[i].ToString() + ",";
                        }
                        imageId = imageId.Substring(0, imageId.Length - 1);
                        OrignalQuery = "select " + query + ",sm.ID from StudentImage sm inner join school s on s.id = sm.SchoolID inner join student st on sm.studentidpk = st.id where s.ID = @ID and sm.id in(" + imageId + ")";
                        ds = WCFSQLHelper.getDataSetText_SP(OrignalQuery, param);
                        string NewImagename = "";
                        int k = 0;
                        int jj = 0;
                        int studentImageId = 0;
                        Directory.CreateDirectory(alternateFullPath);//+ "\\" + "_export" + "\\" + txtfoldname.Text + "");
                        foreach (DataRow dtr in ds.Tables[0].Rows)
                        {
                            try
                            {
                                foreach (DataColumn c in ds.Tables[0].Columns)
                                {
                                    if (c.ColumnName.ToString() == "ID") { studentImageId = Convert.ToInt32(dtr[c.ColumnName]); continue; }
                                    NewImagename = NewImagename + (dtr[c.ColumnName]).ToString();
                                }

                                NewImagename = NewImagename.Substring(0, NewImagename.Length);

                                for (k = jj; k < maxValue; k++)
                                {
                                    try
                                    {
                                        if (isProgressCancelled)
                                        {
                                            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                            string message = "Are you sure, you want to cancel the operation?";
                                            if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                            {
                                                ///args.Cancel = true;
                                                DialogResult = false;
                                                return;
                                            }
                                            isProgressCancelled = false;
                                        }
                                        currentProgress++;

                                        //if (isSourceFolderNotFound) { return; }

                                        DataRow dr = _objStudentImages.Rows
                                                        .Cast<DataRow>()
                                                        .Where(x => Convert.ToInt32(x["ID"]) == studentImageId).FirstOrDefault();

                                        if (dr == null)
                                        {
                                            orderItemNotFound("Student image id ", arrStudentImageIds[k]);
                                            continue;
                                        }

                                        imageName = Convert.ToString(dr["ImageName"]);
                                        studentid = Convert.ToString(dr["StudentIDPk"]);
                                        studentname = Convert.ToString(dr["FirstName"]);
                                        password = Convert.ToString(dr["Password"]);

                                        sequentialNumber = imageName.Substring(imageName.LastIndexOf('_') + 1, imageName.LastIndexOf('.') - imageName.LastIndexOf('_') - 1);
                                        strImgExtenssion = imageName.Substring(imageName.LastIndexOf('.')); //this will get the extenssion of the image
                                        NewImagename = NewImagename.Replace("__", "_");

                                        //insert IPTC data
                                        if (isZenfolioChecked == true)
                                        {
                                            createIPTC(studentid, password);
                                        }

                                        if (isAddImgSerialNo == true)
                                            NewImagename = NewImagename + "_" + sequentialNumber + strImgExtenssion;
                                        else
                                            NewImagename = NewImagename + strImgExtenssion;

                                        int PhotoId = Convert.ToInt32(dr["PhotoShootID"]);
                                        objImageFolder = clsDashBoard.getImageFolder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), PhotoId);
                                        ImageFolder = objImageFolder.ImageFolder;
                                        newPath = objImageFolder.ImageFolder + "\\_reduced\\";
                                        if (isReducedImagesChecked == true)
                                        {
                                            ImageFolder = newPath;
                                        }
                                        else if (isDigitalImagesChecked)
                                        {
                                            ImageFolder = objImageFolder.ImageFolder + "\\_DD";

                                            bool isFolderNotFound = digitalImageFolderNotFound();
                                            if (isFolderNotFound)
                                            {
                                                ///args.Cancel = true; worker = new BackgroundWorker();
                                                return;
                                            }
                                        }

                                        jobName = objImageFolder.PhotographyJob.JobName;
                                        k = arrStudentImageIds.Count();// _objStudentImages.Rows.Count;     Changed By Mohan 29-5-2015


                                    }
                                    catch (Exception ex)
                                    {
                                        clsStatic.WriteExceptionLogXML(ex);
                                        MVVMMessageService.ShowMessage(ex.Message);
                                    }

                                    jj = jj + 1;
                                }

                                if (Directory.Exists(ImageFolder))
                                {
                                    destinationPath = alternateFullPath + "\\" + folderName;

                                    NewImagename = NewImagename.Replace("\\", "");
                                    NewImagename = clsDashBoard.SanitizeFileName(NewImagename);
                                    // Need to move photos from this location as per by lastname and firstname
                                    if (File.Exists(ImageFolder + "\\" + imageName))
                                    {

                                        //File.Copy(ImageFolder + "\\" + imageName, destinationPath + "\\" + NewImagename, true);

                                        String[] source = new String[1];
                                        String[] dest = new String[1];
                                        source[0] = ImageFolder + "\\" + imageName;
                                        dest[0] = destinationPath + "\\" + NewImagename;
                                        fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                        //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                        fo.SourceFiles = source;
                                        fo.DestFiles = dest;
                                        fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                        bool RetVal = fo.DoOperation();

                                        isProgressBarVisible = Visibility.Visible;
                                    }
                                    //Assign Group photo as well.
                                    NewImagename = "";
                                }
                                else
                                {
                                    //MessageBox.Show(errorMessages.SOURCE_FOLDER_DOESNOT_EXISTS);
                                    isSourceFolderNotFound = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                MVVMMessageService.ShowMessage(ex.Message);
                                clsStatic.WriteExceptionLogXML(ex);
                            }
                        }
                        createGroupPhotos("Rename");
                        createTextFile();
                        isProgressBarVisible = Visibility.Hidden;

                        string messagecl = "";
                        if (isSourceFolderNotFound) { isBottomButtonsEnabled = true; isCreateDataFileEnabled = true; isMissedImages = true; }
                        if (!isMissedImages)
                        {
                            File.Delete(missedImagesfolderPath);
                            if (isMissingOrderImages && isFromOrders)
                            {
                                messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                            }
                            else
                            {
                                messagecl = errorMessages.AFTER_COPY_OPEN_LOCATION_CONFIRMATION;
                            }
                        }
                        else
                        {
                            messagecl = "Some images were not found in the source folder. For details please review Missingimages.txt in the destination folder.\n Do you want to open the image folder?";
                            if (isMissingOrderImages && isFromOrders)
                            {
                                messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                            }
                        }

                        string captioncl = "Confirmation";
                        MessageBoxButton buttonss = MessageBoxButton.YesNo;
                        MessageBoxImage iconn = MessageBoxImage.Question;
                        if (MessageBox.Show(messagecl, captioncl, buttonss, iconn) == MessageBoxResult.Yes)
                            Process.Start(alternateFullPath);

                        isSave = true; isMissingOrderImages = false;
                        isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                        DialogResult = false;
                    });
                    ///};

                    //our background worker support cancellation
                    ///worker.WorkerSupportsCancellation = true;

                    // Configure the function to run when completed
                    ///worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                    // Launch the worker
                    ///worker.RunWorkerAsync();
                    exportOrdersCompleted();
                }
                #endregion

                #region if "Rename the image using mask" is true and "Create a folder for each student" is also true
                else
                {
                    ///worker.DoWork += delegate(object s, DoWorkEventArgs args)
                    ///{
                    ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();
                    // Need to get imagename, lastname and firstname of student image
                    string studentid = "";
                    string studentname = "";
                    //string method = "copy";
                    DataTable _objStudentImageDetails = new DataTable();
                    _objStudentImageDetails = clsDashBoard.getImageNameLastNameFirstName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentImageIds);

                    maxValue = arrStudentImageIds.Count();
                    //maxValue = _objStudentImageDetails.Rows.Count; //Changed By Mohan 29-5-2015
                    //Orders can have duplicate student images i.e, one student image can be present in more than one order
                    //so instead of looping through results ,loop through all student image ids
                    //it won't be a problem for group export because , in group export "arrStudentImageIds" will insert only unique student image ids


                    //Need to create a folder inside _export with datetime format yyyyMMdd-HHmm
                    // After creating a folder name, assign image name as per mask syntax
                    DataTable _objmskDetails = new DataTable();
                    _objmskDetails = clsDashBoard.GetMaskDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedMask.MaskID);
                    string query = "";
                    await Task.Run(() =>
                    {
                        for (int j = 0; j < _objmskDetails.Rows.Count; j++)
                        {
                            try
                            {
                                if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "school name" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "schoolname")
                                    query = query + "s.schoolname" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "grade")
                                    query = query + "st.grade" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "teacher")
                                    query = query + "st.teacher" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "first name" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "firstname")
                                    query = query + "st.firstname" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "last name" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "lastname")
                                    query = query + "st.lastname" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "password")
                                    query = query + "st.Password" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "student id" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "studentid")
                                    query = query + "st.StudentID" + ",";
                                else if (_objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "image number" || _objmskDetails.Rows[j]["MaskDetail1"].ToString().ToLower() == "imagenumber")
                                    query = query + "sm.ImageNumber" + ",";
                                else
                                    query = query + "'" + _objmskDetails.Rows[j]["MaskDetail1"].ToString() + "' as Label" + ",";
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }
                        query = query.Substring(0, query.Length - 1);
                        string OrignalQuery = "";
                        DataSet ds = new DataSet();
                        SqlParameter[] param1 = new SqlParameter[1];
                        param1[0] = new SqlParameter("@ID", schoolId);
                        String imageId = "";
                        // Need to get lastname and firstname of student image
                        for (int i = 0; i < arrStudentImageIds.Count(); i++)
                        {
                            imageId += arrStudentImageIds[i].ToString() + ",";
                        }
                        imageId = imageId.Substring(0, imageId.Length - 1);
                        OrignalQuery = "select " + query + ",sm.ID from StudentImage sm inner join school s on s.id = sm.SchoolID inner join student st on sm.studentidpk = st.id where s.ID = @ID and sm.id in(" + imageId + ")";
                        ds = WCFSQLHelper.getDataSetText_SP(OrignalQuery, param1);
                        string NewImagename = "";
                        int kk = 0;
                        int jj = 0;
                        string Imagename = "";
                        int studentImageId = 0;
                        foreach (DataRow dtr in ds.Tables[0].Rows)
                        {
                            try
                            {
                                foreach (DataColumn c in ds.Tables[0].Columns)
                                {
                                    if (c.ColumnName.ToString() == "ID") { studentImageId = Convert.ToInt32(dtr[c.ColumnName]); continue; }
                                    NewImagename = NewImagename + (dtr[c.ColumnName]).ToString();
                                }

                                NewImagename = NewImagename.Substring(0, NewImagename.Length);
                                for (kk = jj; kk < maxValue; kk++)
                                {
                                    try
                                    {
                                        if (isProgressCancelled)
                                        {
                                            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                            string message = "Are you sure, you want to cancel the operation?";
                                            if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                            {
                                                ///args.Cancel = true;
                                                DialogResult = false;
                                                return;
                                            }
                                            isProgressCancelled = false;
                                        }
                                        currentProgress++;
                                        string MoveImageName = "";

                                        DataRow dr = _objStudentImageDetails.Rows
                                                        .Cast<DataRow>()
                                            //.Where(x => studentImageId == arrStudentImageIds[kk]).FirstOrDefault();
                                                        .Where(x => Convert.ToInt32(x["ID"]) == studentImageId).FirstOrDefault();

                                        if (dr == null)
                                        {
                                            orderItemNotFound("Student image id ", arrStudentImageIds[kk]);
                                            continue;
                                        }

                                        Imagename = Convert.ToString(dr["ImageName"]);

                                        string sequentialnumber = Imagename.Substring(Imagename.LastIndexOf('_') + 1, Imagename.LastIndexOf('.') - Imagename.LastIndexOf('_') - 1);
                                        string strImgExtenssion = Imagename.Substring(Imagename.LastIndexOf('.')); //this will get the extenssion of the image

                                        //Imagename = Convert.ToString(_objStudentImageDetails.Rows[kk]["ImageName"]);
                                        if (isAppendPwdChecked == true)
                                        {
                                            password = Convert.ToString(dr["Password"]);
                                            if (password != "" && password != null)
                                            {
                                                password = "~" + password;
                                            }
                                            foldernamee = Convert.ToString(dr["lastname"]) + "_" + Convert.ToString(dr["firstname"]) + password;
                                        }
                                        else
                                            foldernamee = Convert.ToString(dr["lastname"]) + "_" + Convert.ToString(dr["firstname"]);

                                        if (isAddImgSerialNo == true)
                                            MoveImageName = Convert.ToString(dr["lastname"]) + Convert.ToString(dr["firstname"]) + "_" + sequentialnumber + strImgExtenssion;
                                        else
                                            MoveImageName = Convert.ToString(dr["lastname"]) + Convert.ToString(dr["firstname"]) + strImgExtenssion;

                                        studentid = Convert.ToString(dr["StudentIDPk"]);
                                        studentname = Convert.ToString(dr["FirstName"]);

                                        if (isAddImgSerialNo == true)
                                            NewImagename = NewImagename + "_" + sequentialnumber + strImgExtenssion;
                                        else
                                            NewImagename = NewImagename + strImgExtenssion;

                                        NewImagename = NewImagename.Replace("__", "_");

                                        int PhotoId = Convert.ToInt32(dr["PhotoShootID"]);
                                        objImageFolder = clsDashBoard.getImageFolder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), PhotoId);

                                        ImageFolder = objImageFolder.ImageFolder;
                                        newPath = ImageFolder + "\\_reduced\\";

                                        if (isReducedImagesChecked == true)
                                        {
                                            ImageFolder = newPath;
                                        }
                                        else if (isDigitalImagesChecked)
                                        {
                                            ImageFolder = objImageFolder.ImageFolder + "\\_DD";

                                            bool isFolderNotFound = digitalImageFolderNotFound();
                                            if (isFolderNotFound)
                                            {
                                                ///args.Cancel = true; worker = new BackgroundWorker();
                                                return;
                                            }
                                        }

                                        jobName = objImageFolder.PhotographyJob.JobName;
                                        kk = _objStudentImageDetails.Rows.Count;
                                    }
                                    catch (Exception ex)
                                    {
                                        clsStatic.WriteExceptionLogXML(ex);
                                        MVVMMessageService.ShowMessage(ex.Message);
                                    }
                                }
                                jj = jj + 1;

                                if (Directory.Exists(ImageFolder))
                                {
                                    foldernamee = foldernamee.Replace("\\", "");
                                    foldernamee = clsDashBoard.SanitizeFileName(foldernamee);
                                    NewImagename = NewImagename.Replace("\\", "");
                                    NewImagename = clsDashBoard.SanitizeFileName(NewImagename);
                                    destinationPath = alternateFullPath + "\\" + foldernamee;
                                    if (!Directory.Exists(destinationPath))
                                        Directory.CreateDirectory(destinationPath);
                                    // Need to move photos from this location as per by mask syntax
                                    if (File.Exists(ImageFolder + "\\" + Imagename))
                                    {
                                        //File.Copy(sourceImagePath, destImagePath, true);
                                        //File.Copy(ImageFolder + "\\" + Imagename, destinationPath + "\\" + NewImagename, true);

                                        String[] source = new String[1];
                                        String[] dest = new String[1];
                                        source[0] = ImageFolder + "\\" + Imagename;
                                        dest[0] = destinationPath + "\\" + NewImagename;
                                        fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                                        //fo.OwnerWindow = new WindowInteropHelper(this).Handle;    //mohan
                                        fo.SourceFiles = source;
                                        fo.DestFiles = dest;
                                        fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;
                                        bool RetVal = fo.DoOperation();

                                        isProgressBarVisible = Visibility.Visible;
                                    }
                                    //Assign Group photo as well.
                                    NewImagename = "";
                                }
                                else
                                {
                                    isSourceFolderNotFound = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }
                        createGroupPhotos("Both");
                        createTextFile();
                        isProgressBarVisible = Visibility.Hidden;

                        string messagecl = "";
                        if (isSourceFolderNotFound) { isBottomButtonsEnabled = true; isCreateDataFileEnabled = true; isMissedImages = true; }

                        if (!isMissedImages)
                        {
                            File.Delete(missedImagesfolderPath);
                            if (isMissingOrderImages && isFromOrders)
                            {
                                messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                            }
                            else
                            {
                                messagecl = errorMessages.AFTER_COPY_OPEN_LOCATION_CONFIRMATION;
                            }
                        }
                        else
                        {
                            messagecl = "Some images were not found in the source folder. For details please review Missingimages.txt in the destination folder.\n Do you want to open the image folder?";
                            if (isMissingOrderImages && isFromOrders)
                            {
                                messagecl = "Orders exported with errors. Please check error log for details.Do you want to open the image folder?";
                            }
                        }

                        string captioncl = "Confirmation";
                        MessageBoxButton buttonss = MessageBoxButton.YesNo;
                        MessageBoxImage iconn = MessageBoxImage.Question;
                        if (MessageBox.Show(messagecl, captioncl, buttonss, iconn) == MessageBoxResult.Yes)
                            Process.Start(alternateFullPath);

                        isSave = true; isMissingOrderImages = false;
                        isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
                        DialogResult = false;
                    });
                    ///};

                    //our background worker support cancellation
                    ///worker.WorkerSupportsCancellation = true;

                    // Configure the function to run when completed
                    ///worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                    // Launch the worker
                    ///worker.RunWorkerAsync();
                    exportOrdersCompleted();
                }
                #endregion
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
                isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
            }
        }

        /// <summary>
        /// This worker_RunWorkerCompleted is called when the worker is finished.
        /// </summary>
        /// <param name="sender">The worker as Object, but it can be cast to a worker.</param>
        /// <param name="e">The RunWorkerCompletedEventArgs object.</param>
        void exportOrdersCompleted()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                _objWaitCursorViewModel.Dispose();
            });
            isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
        }

        void PSPAAsyncCompleted()
        {

            isBottomButtonsEnabled = true; isCreateDataFileEnabled = true;
        }

        #region Support Methods
        private bool digitalImageFolderNotFound()
        {
            bool res = false;
            if (!Directory.Exists(ImageFolder))
            {
                MVVMMessageService.ShowMessage("No DigitalImages Folder found in " + ImageFolder + ".");//+ ". Generate the reduced images first, then try this operation again."
                isInProgress = false;
                res = true;
            }
            else
            {
                var fileCount = (from file in Directory.EnumerateFiles(ImageFolder, "*.jpg", SearchOption.TopDirectoryOnly)
                                 select file).Count();
                if (fileCount == 0)
                {
                    MVVMMessageService.ShowMessage("No DigitalImages Folder found in " + ImageFolder + ". Generate the DigitalImages first, then try this operation again.");//+ ". Generate the reduced images first, then try this operation again."
                    isInProgress = false;
                    res = true;
                }
            }
            return res;
        }

        public bool isDirectoryContainFiles(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return false;
                return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Any();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return false;
            }
        }

        private void orderItemNotFound(string type, int studentImageId)
        {
            isMissingOrderImages = true;
            clsErrorLog _objclsErrorLog = new clsErrorLog();
            string message = type + studentImageId + " is deleted from any of Photshoot/Student grid but not deleted in Orders.";
            _objclsErrorLog.Message = message;
            clsStatic.WriteErrorLog(_objclsErrorLog, "MissingOrderImages");
        }
        #endregion

        #endregion
    }
}
