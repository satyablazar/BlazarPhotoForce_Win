using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using PhotoForce.Student_Management;
using PhotoForce.StudentImageManagement;
using System.Diagnostics;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using System.Collections.ObjectModel;
using System.Windows;
using System.Drawing;

namespace PhotoForce.WorkPlace.UserControls
{
    public class UniversalStudentSearchViewModel : ViewModelBase
    {
        #region Initialization
        //string strFile = "";
        string strFileToShow = "";
        string strFileReduced = "";
        String selectedGridName = "";
        public int NewImageId = 0;
        PhotoSorterDBModelDataContext db;
        AddEditStudent _objAddEditStudent;
        BulkRenameStudentImage _objBulkRenameStudentImage;
        #endregion

        #region Properties
        IEnumerable<StudentImage> _dgStudentPhotosData;
        Student _selectedStudent;
        ObservableCollection<Student> _selectedStudents;
        StudentImage _selectedStudentPhoto;
        ObservableCollection<StudentImage> _selectedStudentPhotos;
        ImageSource _studentPhotoPreview;
        IEnumerable<Student> _dgStudentsData;
        SearchControl _studentSearchControl;
        ShowSearchPanelMode _studentSearchPanelMode;
        bool _isSearchControlVisible;
        string _univerasalSearchString;

        public string univerasalSearchString
        {
            get { return _univerasalSearchString; }
            set { _univerasalSearchString = value; NotifyPropertyChanged("univerasalSearchString"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public ShowSearchPanelMode studentSearchPanelMode
        {
            get { return _studentSearchPanelMode; }
            set { _studentSearchPanelMode = value; NotifyPropertyChanged("studentSearchPanelMode"); }
        }
        public SearchControl studentSearchControl
        {
            get { return _studentSearchControl; }
            set { _studentSearchControl = value; NotifyPropertyChanged("studentSearchControl"); }
        }
        public IEnumerable<Student> dgStudentsData
        {
            get { return _dgStudentsData; }
            set { _dgStudentsData = value; NotifyPropertyChanged("dgStudentsData"); }
        }
        public ImageSource studentPhotoPreview
        {
            get { return _studentPhotoPreview; }
            set { _studentPhotoPreview = value; NotifyPropertyChanged("studentPhotoPreview"); }
        }
        public ObservableCollection<StudentImage> selectedStudentPhotos
        {
            get { return _selectedStudentPhotos; }
            set { _selectedStudentPhotos = value; NotifyPropertyChanged("selectedStudentPhotos"); }
        }
        public StudentImage selectedStudentPhoto
        {
            get { return _selectedStudentPhoto; }
            set { _selectedStudentPhoto = value; NotifyPropertyChanged("selectedStudentPhoto"); }
        }
        public ObservableCollection<Student> selectedStudents
        {
            get { return _selectedStudents; }
            set { _selectedStudents = value; NotifyPropertyChanged("selectedStudents"); }
        }
        public Student selectedStudent
        {
            get { return _selectedStudent; }
            set { _selectedStudent = value; NotifyPropertyChanged("selectedStudent"); }
        }
        public IEnumerable<StudentImage> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        #endregion

        #region Constructor
        public UniversalStudentSearchViewModel()
        {
            selectedStudents = new ObservableCollection<Student>(); selectedStudentPhotos = new ObservableCollection<StudentImage>();
        }
        #endregion

        #region Commands
        public RelayCommand UniversalStudentGroupFocusCommand
        {
            get
            {
                return new RelayCommand(universalStudentGroupFocus);
            }
        }
        public RelayCommand StudentGridMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(studentGridMouseDoubleClick);
            }
        }
        public RelayCommand StudentTableMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentTableMouseUp);
            }
        }
        public RelayCommand StudentPhotosGroupFocusCommand
        {
            get
            {
                return new RelayCommand(studentPhotosGroupFocus);
            }
        }
        public RelayCommand StudentPhotosMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(studentPhotosMouseDoubleClick);
            }
        }
        public RelayCommand StudentPhotosTableMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotosTableMouseUp);
            }
        }
        public RelayCommand StudentPhotoPreviewMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotoPreviewMouseUp);
            }
        }
        #endregion

        #region Methods

        private void studentGridMouseDoubleClick()
        {
            selectedGridName = "student";
            editRecord();
        }
        private void studentTableMouseUp()
        {
            if (selectedStudent != null)
            {
                setVisibilityForUniversalStudent();
                try
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    NewImageId = selectedStudent.ID;
                    dgStudentPhotosData = clsDashBoard.getStudentImageDetailsBySchool(db, selectedStudent.ID, selectedStudent.StudentImport != null ? selectedStudent.StudentImport.School.ID : clsSchool.defaultSchoolId);
                    selectedGridName = "student";
                    selectImage();
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.InnerException.ToString());
                }
            }
        }

        private void studentPhotosMouseDoubleClick()
        {
            if (selectedStudentPhoto != null)
            {
                selectedGridName = "studentphoto";
                editRecord();
            }
        }
        private void studentPhotosTableMouseUp()
        {
            if (selectedStudentPhoto != null)
            {
                selectedGridName = "studentphoto";
                setVisibilityForUniversalStudent();
                selectImage();
            }
        }
        private void studentPhotoPreviewMouseUp()
        {
            try
            {
                selectedGridName = "studentphoto";
                string filePath = strFileToShow.ToString();
                filePath = "file:///" + filePath;
                if (filePath.StartsWith("file"))
                {
                    filePath = filePath.Substring(8, filePath.Length - 8);
                    if (File.Exists(filePath))
                    {
                        Process.Start(filePath);
                    }
                    else
                    {
                        filePath = strFileReduced.ToString();
                        filePath = "file:///" + filePath;
                        try
                        {
                            Process.Start(filePath);
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
        private void universalStudentGroupFocus()
        {
            selectedGridName = "student";
            setVisibilityForUniversalStudent();
        }
        private void studentPhotosGroupFocus()
        {
            selectedGridName = "studentphoto";
            setVisibilityForPhotosGrid();
        }

        public void bindStudentGrid(string searchString)
        {
            univerasalSearchString = searchString;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            try
            {
                dgStudentsData = clsStudent.getStudentsByUniversalSearch(db, searchString);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void newRecord()
        {
            try
            {
                AddStudent _objAddStudent = new AddStudent();
                _objAddStudent.ShowDialog();
                if (((AddStudentViewModel)(_objAddStudent.DataContext)).isSave)
                    bindStudentGrid(univerasalSearchString);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void editRecord()
        {
            try
            {
                if (selectedGridName == "student")
                {
                    if (selectedStudent != null)
                    {
                        int studentID = selectedStudent.ID;
                        _objAddEditStudent = new AddEditStudent(studentID, 0, selectedStudent.StudentImport != null ? selectedStudent.StudentImport.School.ID : clsSchool.defaultSchoolId);
                        _objAddEditStudent.ShowDialog();
                        if (((AddEditStudentViewModel)(_objAddEditStudent.DataContext)).isSave)
                            bindStudentGrid(univerasalSearchString);
                    }
                }
                else
                {
                    int studentImgID = selectedStudentPhoto.ID;
                    _objBulkRenameStudentImage = new BulkRenameStudentImage(studentImgID);
                    _objBulkRenameStudentImage.ShowDialog();
                    if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                    {
                        bindStudentGrid(univerasalSearchString);
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void searchPanels()
        {
            if (studentSearchControl == null || !isSearchControlVisible)
            {
                studentSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
        }
        private void selectImage()
        {
            try
            {
                if (selectedStudentPhoto != null)
                {
                    int imageId = selectedStudentPhoto.ID;
                    PhotoSorterDBModelDataContext db1 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    StudentImage _objStudentImage = clsDashBoard.getStudentImageDetailsById(db1, imageId);
                    if (_objStudentImage == null) { return; }

                    //strFile = _objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + _objStudentImage.ImageName;
                    strFileToShow = _objStudentImage.PhotoShoot.ImageFolder + "\\" + _objStudentImage.ImageName;
                    strFileReduced = _objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + _objStudentImage.ImageName;
                    if (!File.Exists(strFileReduced))
                        strFileReduced = strFileToShow;
                    if (!File.Exists(strFileReduced))
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
                        using (FileStream fs = new FileStream(strFileReduced, FileMode.Open, FileAccess.Read))
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
                    studentPhotoPreview = null;
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


        internal void setVisibilityForUniversalStudent()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
        }
        internal void setVisibilityForPhotosGrid()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isEditVisible = true;
        }
        #endregion
    }
}
