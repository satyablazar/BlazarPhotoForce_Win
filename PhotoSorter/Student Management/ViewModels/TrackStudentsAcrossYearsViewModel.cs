using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.WorkPlace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoForce.Student_Management
{
    public class TrackStudentsAcrossYearsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        string Photoshootpath = "";
        string imagePathToShow = "";
        int schoolID;
        #endregion

        #region Properties
        ObservableCollection<Student> _cbStudentData;
        Student _selectedStudent;
        ObservableCollection<StudentImage> _dgStudentPhotosData;
        ObservableCollection<StudentImage> _selectedStudentImages;
        ImageSource _studentImageSource;
        StudentImage _selectedStudentImage;
        string _studentFirstName;
        string _studentLastName;
        string _schoolName;
        bool _isCreateGroupBtnEnabled;
        Visibility _progressVisibility;
        int _maxValue;
        int _minValue;
        int _currentProgress;
        bool _isDateFilterDateEnabled;
        DateTime? _selectedFromDate;
        DateTime? _selectedEndDate;

        public DateTime? selectedEndDate
        {
            get { return _selectedEndDate; }
            set
            {
                _selectedEndDate = value; NotifyPropertyChanged();

                if (selectedEndDate != null)
                {
                    bindStudentData();
                }
            }
        }
        public DateTime? selectedFromDate
        {
            get { return _selectedFromDate; }
            set
            {
                _selectedFromDate = value; NotifyPropertyChanged();

                if (selectedFromDate != null)
                {
                    bindStudentData();
                }
            }
        }
        public bool isDateFilterDateEnabled
        {
            get { return _isDateFilterDateEnabled; }
            set { _isDateFilterDateEnabled = value; NotifyPropertyChanged(); }
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
        public Visibility progressVisibility
        {
            get { return _progressVisibility; }
            set { _progressVisibility = value; NotifyPropertyChanged("progressVisibility"); }
        }
        public bool isCreateGroupBtnEnabled
        {
            get { return _isCreateGroupBtnEnabled; }
            set { _isCreateGroupBtnEnabled = value; NotifyPropertyChanged(); }
        }
        public ImageSource studentImageSource
        {
            get { return _studentImageSource; }
            set { _studentImageSource = value; NotifyPropertyChanged(); }
        }
        public string schoolName
        {
            get { return _schoolName; }
            set { _schoolName = value; NotifyPropertyChanged(); }
        }
        public string studentLastName
        {
            get { return _studentLastName; }
            set { _studentLastName = value; NotifyPropertyChanged(); }
        }
        public string studentFirstName
        {
            get { return _studentFirstName; }
            set { _studentFirstName = value; NotifyPropertyChanged(); }
        }
        public StudentImage selectedStudentImage
        {
            get { return _selectedStudentImage; }
            set { _selectedStudentImage = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<StudentImage> selectedStudentImages
        {
            get { return _selectedStudentImages; }
            set { _selectedStudentImages = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<Student> cbStudentData
        {
            get { return _cbStudentData; }
            set { _cbStudentData = value; NotifyPropertyChanged(); }
        }
        public Student selectedStudent
        {
            get { return _selectedStudent; }
            set
            {
                _selectedStudent = value; NotifyPropertyChanged();
                if (selectedStudent != null)
                {
                    dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.getStudentImagesAcrossYearsbyStudentId(db, (selectedStudent.Lastname + "_" + selectedStudent.FirstName + "_%")));
                    studentFirstName = (selectedStudent.FirstName);
                    studentLastName = (selectedStudent.Lastname);
                    schoolName = (selectedStudent.StudentImport != null ? (selectedStudent.StudentImport.School.SchoolName) : "");
                    if (dgStudentPhotosData.Count != 0)
                    {
                        isCreateGroupBtnEnabled = true;
                        selectedStudentImage = dgStudentPhotosData.First();
                        if (selectedStudentImage != null)
                        {
                            selectImage();
                        }
                    }
                    else
                    {
                        dgStudentPhotosData = new ObservableCollection<StudentImage>();
                        selectedStudentImage = null;
                        isCreateGroupBtnEnabled = false;
                    }
                }
                else
                {
                    dgStudentPhotosData = new ObservableCollection<StudentImage>();
                    studentFirstName = "";
                    studentLastName = "";
                    selectedStudentImage = null;
                    isCreateGroupBtnEnabled = false;
                }
            }
        }
        public ObservableCollection<StudentImage> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        #endregion

        #region Constructor
        public TrackStudentsAcrossYearsViewModel()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            dgStudentPhotosData = new ObservableCollection<StudentImage>(); selectedStudentImages = new ObservableCollection<StudentImage>();
            cbStudentData = new ObservableCollection<Student>();
            schoolID = clsSchool.defaultSchoolId;
            //cbStudentData = new ObservableCollection<Student>((from p in db.Students where p.RecordStatus == true && p.StudentImport.SchoolID == schoolID orderby p.StudentImport.ID, p.Lastname select p).ToList());
            isCreateGroupBtnEnabled = false;
            progressVisibility = Visibility.Collapsed;
            bindStudentData();
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
        public RelayCommand StudentPhotosTableKeyUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotosMouseup);
            }
        }
        public RelayCommand CreateActiveGroupCommand
        {
            get
            {
                return new RelayCommand(createGroup);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand RemoveSelectedImagesCommand
        {
            get
            {
                return new RelayCommand(removeSelectedImages);
            }
        }
        #endregion

        #region Methods
        private void bindStudentData()
        {
            if (selectedFromDate != null && selectedEndDate != null)
                cbStudentData = new ObservableCollection<Student>((from p in db.Students where p.RecordStatus == true && p.StudentImport.SchoolID == schoolID && p.CreatedOn >= selectedFromDate && p.CreatedOn <= selectedEndDate orderby p.StudentImport.ID, p.Lastname select p).ToList());
            else
            {
                cbStudentData = new ObservableCollection<Student>((from p in db.Students where p.RecordStatus == true && p.StudentImport.SchoolID == schoolID orderby p.StudentImport.ID, p.Lastname select p).ToList());
            }
        }

        async void createGroup()
        {
            if (dgStudentPhotosData.Count > 0)
                await createActiveGroup();
            else
                return;
        }
        async Task createActiveGroup()
        {
            try
            {
                maxValue = 1; currentProgress = 0;
                progressVisibility = Visibility.Visible;
                isCreateGroupBtnEnabled = false;
                Cursor.Current = Cursors.WaitCursor;
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                await Task.Run(() =>
                {
                    maxValue = dgStudentPhotosData.Count;

                    Group _objGroup = new Group();
                    _objGroup.createdOn = DateTime.Now;
                    _objGroup.GroupName = studentFirstName + "_" + studentLastName + "_" + selectedStudent.StudentID;
                    _objGroup.Notes = studentFirstName + "_" + studentLastName + " images acrosss years.";
                    _objGroup.SchoolID = selectedStudent.PhotographyJob.SchoolID;

                    db.Groups.InsertOnSubmit(_objGroup);
                    db.SubmitChanges();
                    int tempGroupID = _objGroup.ID;

                    if (tempGroupID > 0)
                    {
                        foreach (StudentImage si in dgStudentPhotosData)
                        {
                            currentProgress++;
                            GroupItem _objGroupItem = new GroupItem();
                            _objGroupItem.GroupID = tempGroupID;
                            _objGroupItem.StudentPhotoID = si.ID;
                            db.GroupItems.InsertOnSubmit(_objGroupItem);
                            db.SubmitChanges();
                        }
                    }
                });
                Cursor.Current = Cursors.Default;
                progressVisibility = Visibility.Collapsed;
                MVVMMessageService.ShowMessage("Group created successfully with name " + studentFirstName + "_" + studentLastName + "_" + selectedStudent.StudentID + " .");
                isCreateGroupBtnEnabled = false;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        /// <summary>
        /// it will auto create teachers group for selected Photoshoot(s)
        /// </summary>        
        private void studentPhotosMouseup()
        {
            selectImage();
        }

        # region Select Image
        /// <summary>
        /// Just to show the selected student image on imgStudentPhotoPreview grid
        /// </summary>
        private void selectImage()
        {
            try
            {
                if (selectedStudentImage != null)
                {
                    int? ImageId = selectedStudentImage.ID;
                    int? groupImageId = selectedStudentImage.GroupClassPhotos.Count != 0 ? selectedStudentImage.GroupClassPhotos[0].studentImageId : null;
                    //groupImageId = selectedStudentImage.GroupImageId;

                    if (ImageId == null)
                    {
                        studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        showImagePreview("Preview", ImageId);
                    }
                }
                else
                {
                    studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
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
                }
                else
                {
                    thumb.StreamSource = stream;
                    thumb.EndInit();
                    studentImageSource = thumb;
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
        # endregion

        private void windowClose()
        {
            DialogResult = false;
        }

        private void removeSelectedImages()
        {
            if (selectedStudentImages.Count > 0)
            {
                ArrayList arrImageIds = new ArrayList();
                string message = "";
                foreach (StudentImage si in selectedStudentImages)
                {
                    if (!arrImageIds.Contains(si.ID))
                    {
                        arrImageIds.Add(si.ID);
                    }
                }
                if (arrImageIds.Count != 0)
                {
                    if (arrImageIds.Count == 1)
                    {
                        message = "Are you sure you want to delete image '" + selectedStudentImage.ImageName + "' ?";
                    }
                    else
                    {
                        message = "Are you sure you want to delete multiple student images?";
                    }

                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        foreach (int ImageId in arrImageIds)
                        {
                            dgStudentPhotosData.Remove(dgStudentPhotosData.Where(i => i.ID == ImageId).First());
                        }
                    }
                }
            }
            else
                MVVMMessageService.ShowMessage("Please select image(s).");
        }
        #endregion
    }
}
