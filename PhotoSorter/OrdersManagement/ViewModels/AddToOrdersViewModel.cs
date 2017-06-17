using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoForce.OrdersManagement
{
    public class AddToOrdersViewModel : ViewModelBase
    {
        #region Initialization
        int OrderId;
        public bool isSave = false; //#Mohan ; #NUnitTest
        //string strFile = "";
        string strFileToShow = "";
        string strFilereduced = "";
        string universalSearchString = "";
        PhotoSorterDBModelDataContext db;
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;
        public ObservableCollection<StudentPhotoOrder> newStudentPhotoOrders = new ObservableCollection<StudentPhotoOrder>();
        public int classPhotoId = 0;
        int orderDetailId = 0;
        int SchoolID = 0;
        #endregion

        #region Properties
        List<StudentImage> _dgAddToOrders;
        ObservableCollection<StudentImage> _selectedPhotos;
        IEnumerable<PhotographyJob> _cbPhotographyJobData;
        ImageSource _studentPhotoPreview;
        StudentImage _selectedPhoto;
        int _selectedJobId;
        int _selectedSchoolId;
        List<School> _cbSchoolData;
        bool _isGroupButtonVisible;
        bool _isBottomButtonsVisible;
        string _searchLable;
        string _windowTitle;
        bool _isAssignButtonVisible;

        public bool isAssignButtonVisible
        {
            get { return _isAssignButtonVisible; }
            set { _isAssignButtonVisible = value; NotifyPropertyChanged("isAssignButtonVisible"); }
        }
        public string windowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; NotifyPropertyChanged("windowTitle"); }
        }
        public string searchLable
        {
            get { return _searchLable; }
            set { _searchLable = value; NotifyPropertyChanged("searchLable"); }
        }
        public bool isBottomButtonsVisible
        {
            get { return _isBottomButtonsVisible; }
            set { _isBottomButtonsVisible = value; NotifyPropertyChanged("isBottomButtonsVisible"); }
        }
        public bool isGroupButtonVisible
        {
            get { return _isGroupButtonVisible; }
            set { _isGroupButtonVisible = value; NotifyPropertyChanged("isGroupButtonVisible"); }
        }
        public List<School> cbSchoolData
        {
            get { return _cbSchoolData; }
            set { _cbSchoolData = value; NotifyPropertyChanged("cbSchoolData"); }
        }
        public int selectedSchoolId
        {
            get { return _selectedSchoolId; }
            set
            {
                universalSearchString = "";
                _selectedSchoolId = value; NotifyPropertyChanged("selectedSchoolId");
                loadPhotographyJobs();
            }
        }

        public int selectedJobId
        {
            get { return _selectedJobId; }
            set
            {
                _selectedJobId = value; NotifyPropertyChanged("selectedJobId");
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                dgAddToOrders = clsOrders.getImagesForOrders(db, OrderId, selectedSchoolId, selectedJobId);
            }
        }
        public ObservableCollection<StudentImage> selectedPhotos
        {
            get { return _selectedPhotos; }
            set { _selectedPhotos = value; NotifyPropertyChanged("selectedPhotos"); }
        }
        public IEnumerable<PhotographyJob> cbPhotographyJobData
        {
            get { return _cbPhotographyJobData; }
            set { _cbPhotographyJobData = value; NotifyPropertyChanged("cbPhotographyJobData"); }
        }
        public StudentImage selectedPhoto
        {
            get { return _selectedPhoto; }
            set { _selectedPhoto = value; NotifyPropertyChanged("selectedPhoto"); }
        }
        public ImageSource studentPhotoPreview
        {
            get { return _studentPhotoPreview; }
            set { _studentPhotoPreview = value; NotifyPropertyChanged("studentPhotoPreview"); }
        }
        public List<StudentImage> dgAddToOrders
        {
            get { return _dgAddToOrders; }
            set { _dgAddToOrders = value; NotifyPropertyChanged("dgAddToOrders"); }
        }
        #endregion

        #region Constructors
        public AddToOrdersViewModel(int tempOrderId, bool isForGroupImage)
        {
            if (isForGroupImage)
            {
                windowTitle = "Select Group Image";
                searchLable = "Group photo search";
                isBottomButtonsVisible = false; isAssignButtonVisible = false;
                isGroupButtonVisible = true;
            }
            else
            {
                windowTitle = "Add To Orders";
                searchLable = "Student Photo Search";
                isBottomButtonsVisible = true; isGroupButtonVisible = false; isAssignButtonVisible = false;
            }
            dgAddToOrders = new List<StudentImage>(); selectedPhotos = new ObservableCollection<StudentImage>();
            OrderId = tempOrderId;
            loadData();
        }
        public AddToOrdersViewModel(int orderDetail, int SchoolId)
        {
            SchoolID = SchoolId;
            orderDetailId = orderDetail;
            windowTitle = "Select Image";
            searchLable = "Search for Image";
            isBottomButtonsVisible = false; isGroupButtonVisible = false;
            isAssignButtonVisible = true;

            dgAddToOrders = new List<StudentImage>(); selectedPhotos = new ObservableCollection<StudentImage>();
            loadData();
        }
        #endregion

        #region Commands
        public RelayCommand StudentPhotoPreviewMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotoPreviewMouseUp);
            }
        }
        public RelayCommand StudentPhotoMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotoMouseUp);
            }
        }
        public RelayCommand AddCommand
        {
            get
            {
                return new RelayCommand(add);
            }
        }
        public RelayCommand AddAndCloseCommand
        {
            get
            {
                return new RelayCommand(addAndClose);
            }
        }
        public RelayCommand AddGroupImageCommand
        {
            get
            {
                return new RelayCommand(addGroupImage);
            }
        }
        public RelayCommand AssignImageCommand
        {
            get
            {
                return new RelayCommand(assignImage);
            }
        }
        public RelayCommand<string> UniversalImageSearchCommand
        {
            get
            {
                return new RelayCommand<string>(universalImageSearch);
            }
        }
        public RelayCommand<object> RestoreLayoutGroupCommand
        {
            get
            {
                return new RelayCommand<object>(restore);
            }
        }
        public RelayCommand<object> SaveLayoutGroupCommand
        {
            get
            {
                return new RelayCommand<object>(saveLayout);
            }
        }
        public RelayCommand<object> RestoreGridLayoutCommand
        {
            get
            {
                return new RelayCommand<object>(restoreGrid);
            }
        }
        public RelayCommand<object> SaveGridLayoutCommand
        {
            get
            {
                return new RelayCommand<object>(saveGridLayout);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(close);
            }
        }
        #endregion

        #region Methods
        private void loadData()
        {
            loadSchools();
        }
        private void loadPhotographyJobs()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            cbPhotographyJobData = clsDashBoard.getJobs(db, selectedSchoolId);

            if (cbPhotographyJobData.Count() != 0)
            {
                selectedJobId = cbPhotographyJobData.First().ID;
            }
            else
            {
                dgAddToOrders = null;
            }
        }
        private void loadSchools()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            cbSchoolData = clsSchool.getAllSchools(db);
            if (SchoolID == 0)
                selectedSchoolId = clsSchool.defaultSchoolId;
            else
                selectedSchoolId = SchoolID;
        }
        private void addImagesToOrder()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            //have to add photos to order
            //create new studentphotoorder objects
            if (selectedPhotos != null)
            {
                foreach (StudentImage img in selectedPhotos)
                {
                    StudentPhotoOrder _objStudentPhotoOrder = new StudentPhotoOrder();
                    _objStudentPhotoOrder.StudentImageId = img.ID;
                    _objStudentPhotoOrder.OrderId = OrderId;
                    _objStudentPhotoOrder.SchoolId = (int)img.SchoolID;
                    db.StudentPhotoOrders.InsertOnSubmit(_objStudentPhotoOrder);
                    db.SubmitChanges();

                    newStudentPhotoOrders.Add(_objStudentPhotoOrder);
                }
                
                isSave = true;
            }
        }
        private void add()
        {
            addImagesToOrder();
            universalSearch(false);
        }
        private void addAndClose()
        {
            addImagesToOrder();
            DialogResult = false;
        }
        private void assignImage()
        {
            if (selectedPhoto != null)
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                StudentPhotoOrder _objStudentPhotoOrder = new StudentPhotoOrder();
                _objStudentPhotoOrder = clsOrders.getStudentPhotoOrderById(db, orderDetailId);
                _objStudentPhotoOrder.StudentImageId = selectedPhoto.ID;

                db.SubmitChanges();

                isSave = true;
                DialogResult = false;
            }
        }
        private void addGroupImage()
        {
            if (selectedPhoto != null)
            {
                classPhotoId = selectedPhoto.ID;
                isSave = true;
                DialogResult = false;
            }
        }
        private void close()
        {
            DialogResult = false;
        }
        private void universalImageSearch(string searchString)
        {
            universalSearchString = searchString;
            universalSearch(true);
        }
        private void restore(object grdLayout)
        {
            if (File.Exists(appDataPath + "\\" + "AddToOrdersLayoutGroup1.xml"))
            {
                ((DockLayoutManager)grdLayout).RestoreLayoutFromXml(appDataPath + "\\" + "AddToOrdersLayoutGroup1.xml");
            }
        }
        private void saveLayout(object grdLayout)
        {
            ((DockLayoutManager)grdLayout).SaveLayoutToXml(appDataPath + "\\" + "AddToOrdersLayoutGroup1.xml");
        }
        private void restoreGrid(object grdLayout)
        {
            if (File.Exists(appDataPath + "\\" + "AddToOrdersLayout1.xml"))
            {
                ((GridControl)grdLayout).RestoreLayoutFromXml(appDataPath + "\\" + "AddToOrdersLayout1.xml");
            }
        }
        private void saveGridLayout(object grdLayout)
        {
            ((GridControl)grdLayout).SaveLayoutToXml(appDataPath + "\\" + "AddToOrdersLayout1.xml");
        }
        private void universalSearch(bool isFromUniversalSearch)
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            try
            {
                if (isFromUniversalSearch)
                {
                    dgAddToOrders = clsOrders.getImagesBySeachString(db, OrderId, universalSearchString);
                }
                else
                {
                    dgAddToOrders = clsOrders.getImagesForOrders(db, OrderId, selectedSchoolId, selectedJobId);
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void studentPhotoMouseUp()
        {
            selectImage();
        }
        private void studentPhotoPreviewMouseUp()
        {
            try
            {
                string filePath = strFileToShow;
                filePath = "file:///" + filePath;
                if (filePath.StartsWith("file"))
                {
                    filePath = filePath.Substring(8, filePath.Length - 8);
                    if (File.Exists(filePath))
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                    else
                    {
                        filePath = strFilereduced;
                        filePath = "file:///" + filePath;
                        try
                        {
                            System.Diagnostics.Process.Start(filePath);
                        }
                        catch (Exception ex)
                        {
                            MVVMMessageService.ShowMessage(ex.Message);
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

        private void selectImage()
        {
            try
            {
                if (selectedPhoto != null)
                {
                    int imageId = selectedPhoto.ID;
                    PhotoSorterDBModelDataContext db1 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    StudentImage _objStudentImage = clsDashBoard.getStudentImageDetailsById(db1, imageId);
                    if (_objStudentImage == null) { return; }

                    //strFile = _objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + _objStudentImage.ImageName;
                    strFileToShow = _objStudentImage.PhotoShoot.ImageFolder + "\\" + _objStudentImage.ImageName;
                    strFilereduced = _objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + _objStudentImage.ImageName;
                    if (!File.Exists(strFilereduced))
                        strFilereduced = strFileToShow;
                    if (!File.Exists(strFilereduced))
                    {
                        studentPhotoPreview = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        #region Old Code
                        //System.Drawing.Image img = System.Drawing.Image.FromFile(strFilereduced);
                        //if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        //{
                        //    decoderForJpeg(strFilereduced);
                        //}
                        #endregion
                        using (FileStream fs = new FileStream(strFilereduced, FileMode.Open, FileAccess.Read))
                        {
                            using (Image original = Image.FromStream(fs))
                            {
                                if (original.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                                {
                                    decoderForJpeg(fs);
                                }
                            }
                        }
                    }
                }
                else
                {
                    studentPhotoPreview = null;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        private void decoderForJpeg(FileStream stream)
        {
            //using (var stream = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            BitmapImage thumb = new BitmapImage();

            stream.Seek(0, SeekOrigin.Begin);

            thumb.BeginInit();
            thumb.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
            thumb.CacheOption = BitmapCacheOption.OnLoad;
            thumb.StreamSource = stream;
            thumb.EndInit();

            studentPhotoPreview = thumb;
            //}
        }

        #endregion
    }
}
