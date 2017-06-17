using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Data;
using Excel;
using PhotoForce.Student_Management;
using PhotoForce.StudentImageManagement;
using PhotoForce.Error_Management;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System.Collections.ObjectModel;
using PhotoForce.GroupManagement;
using PhotoForce.School_Management;
using System.Windows;
using System.Diagnostics;
using System.Drawing;

namespace PhotoForce.WorkPlace.UserControls
{
    public class StudentsViewModel : ViewModelBase
    {
        #region Initialization
        AddEditStudent _objAddEditStudent;
        AddStudent _objAddStudent;
        CountDuplicateStudents _objCountDuplicateStudents;
        GeneratePassword _objGeneratePassword;
        ImportStudent _objImportStudent;
        ExportStudent _objExportStudent;
        BulkRenameStudentImage _objBulkRenameStudentImage;
        AssignStudent _objAssignStudent;
        CountStudents _objCountStudents;
        public String selectedGridName = ""; //#Mohan ; #NUnitTest
        string ImageFolderPath = "";
        GenerateStudentQRCode _objGenerateStudentQRCode;
        clsStatic objstatic = new clsStatic();
        //string strFile = "";
        string strFileToShow = "";
        string strFilereduced = "";
        //List<string> sortedColumn = new List<string>();
        ArrayList arrstudentPhotographyId;
        #endregion

        #region Properties
        ObservableCollection<Student> _dgStudentsData;
        Student _selectedStudent;
        ObservableCollection<Student> _selectedStudents;
        ObservableCollection<StudentImage> _dgStudentPhotosData;
        StudentImage _selectedStudentImage;
        ObservableCollection<StudentImage> _selectedStudentImages;
        List<Student> _VisibleData;
        private ImageSource _studentPhotoPreview;
        //search and group properties
        bool _studentShowGroupPanel;
        bool _studentPhotosShowGroupPanel;


        ShowSearchPanelMode _studentSearchPanelMode;
        ShowSearchPanelMode _studentPhotosSearchPanelMode;
        SearchControl _studentSearchControl;
        SearchControl _studentPhotosSearchControl;
        bool _isSearchControlVisible;
        Activity _selectedActivity;
        // List<Activity> _dgStudentActivities;
        private int _selectedIndex;

        public int selectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value; NotifyPropertyChanged("selectedIndex");
                if (selectedIndex == 0)
                {
                    (Application.Current as App).isNewVisible = false;
                }
                else
                {
                    (Application.Current as App).isNewVisible = true;
                    (Application.Current as App).isDragVisible = false; (Application.Current as App).isSearchVisible = false;
                    //selectedGridName = "Activities";
                }
            }
        }

        //sorted columns
        int _isFirstNameSorted;
        int _isLastNameSorted;
        int _isSchoolNameSorted;
        int _isTeacherSorted;
        int _isGradeSorted;

        public int isGradeSorted
        {
            get { return _isGradeSorted; }
            set { _isGradeSorted = value; NotifyPropertyChanged("isGradeSorted"); }
        }
        public int isTeacherSorted
        {
            get { return _isTeacherSorted; }
            set { _isTeacherSorted = value; NotifyPropertyChanged("isTeacherSorted"); }
        }
        public int isSchoolNameSorted
        {
            get { return _isSchoolNameSorted; }
            set { _isSchoolNameSorted = value; NotifyPropertyChanged("isSchoolNameSorted"); }
        }
        public int isLastNameSorted
        {
            get { return _isLastNameSorted; }
            set { _isLastNameSorted = value; NotifyPropertyChanged("isLastNameSorted"); }
        }
        public int isFirstNameSorted
        {
            get { return _isFirstNameSorted; }
            set { _isFirstNameSorted = value; NotifyPropertyChanged("isFirstNameSorted"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public SearchControl studentSearchControl
        {
            get { return _studentSearchControl; }
            set { _studentSearchControl = value; NotifyPropertyChanged("studentSearchControl"); }
        }
        public SearchControl studentPhotosSearchControl
        {
            get { return _studentPhotosSearchControl; }
            set { _studentPhotosSearchControl = value; NotifyPropertyChanged("studentPhotosSearchControl"); }
        }
        public ShowSearchPanelMode studentSearchPanelMode
        {
            get { return _studentSearchPanelMode; }
            set { _studentSearchPanelMode = value; NotifyPropertyChanged("studentSearchPanelMode"); }
        }
        public ShowSearchPanelMode studentPhotosSearchPanelMode
        {
            get { return _studentPhotosSearchPanelMode; }
            set { _studentPhotosSearchPanelMode = value; NotifyPropertyChanged("studentPhotosSearchPanelMode"); }
        }
        public bool studentShowGroupPanel
        {
            get { return _studentShowGroupPanel; }
            set { _studentShowGroupPanel = value; NotifyPropertyChanged("studentShowGroupPanel"); }
        }
        public bool studentPhotosShowGroupPanel
        {
            get { return _studentPhotosShowGroupPanel; }
            set { _studentPhotosShowGroupPanel = value; NotifyPropertyChanged("studentPhotosShowGroupPanel"); }
        }
        public ImageSource studentPhotoPreview
        {
            get { return _studentPhotoPreview; }
            set { _studentPhotoPreview = value; NotifyPropertyChanged("studentPhotoPreview"); }
        }
        public List<Student> VisibleData
        {
            get { return _VisibleData; }
            set { _VisibleData = value; NotifyPropertyChanged("VisibleData"); }
        }
        public ObservableCollection<StudentImage> selectedStudentImages
        {
            get { return _selectedStudentImages; }
            set { _selectedStudentImages = value; NotifyPropertyChanged("selectedStudentImages"); selectedGridName = "studentimages"; }
        }
        public ObservableCollection<Student> selectedStudents
        {
            get { return _selectedStudents; }
            set { _selectedStudents = value; NotifyPropertyChanged("selectedStudents"); selectedGridName = "student"; }
        }
        public StudentImage selectedStudentImage
        {
            get { return _selectedStudentImage; }
            set { _selectedStudentImage = value; NotifyPropertyChanged("selectedStudentImage"); }
        }
        public ObservableCollection<StudentImage> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        public Student selectedStudent
        {
            get { return _selectedStudent; }
            set { _selectedStudent = value; NotifyPropertyChanged("selectedStudent"); }
        }
        public ObservableCollection<Student> dgStudentsData
        {
            get { return _dgStudentsData; }
            set { _dgStudentsData = value; NotifyPropertyChanged("dgStudentsData"); }
        }
        public Activity selectedActivity
        {
            get { return _selectedActivity; }
            set { _selectedActivity = value; NotifyPropertyChanged("selectedActivity"); }
        }
        //public List<Activity> dgStudentActivities
        //{
        //    get { return _dgStudentActivities; }
        //    set { _dgStudentActivities = value; NotifyPropertyChanged("dgStudentActivities"); }
        //}
        #endregion

        #region Constructors
        public StudentsViewModel()
        {
            //selectedIndex = 0;
            VisibleData = new List<Student>(); dgStudentsData = new ObservableCollection<Student>(); dgStudentPhotosData = new ObservableCollection<StudentImage>();
            selectedStudentImages = new ObservableCollection<StudentImage>(); selectedStudents = new ObservableCollection<Student>();
            getDefaultSchool();
        }
        #endregion

        #region Commands
        public RelayCommand StudentGroupFocusCommand
        {
            get
            {
                return new RelayCommand(studentGroupFocus);
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
        /// <summary>
        /// this method used to get defult school
        /// </summary>
        internal void getDefaultSchool()
        {
            bindStudentGrid();
        }

        public void bindStudentGrid()
        {
            try
            {
                studentPhotoPreview = null;
                dgStudentsData = new ObservableCollection<Student>(clsStudent.getStudentsDetailsClass(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId));

                if (dgStudentsData.Count() > 0)
                {
                    selectedStudent = dgStudentsData.First();
                    dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.getStudentImageDetailsBySchool(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedStudent.ID, clsSchool.defaultSchoolId));
                }
                else
                    selectedStudent = new Student();

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        //added by Mohan
        /// <summary>
        /// this method used to assign student to the selected images
        /// </summary>
        internal void assignStudent()
        {
            int studentImageId = 0;
            ArrayList arrStudentImgId = new ArrayList();
            foreach (StudentImage stuImage in selectedStudentImages)
            {
                try
                {
                    studentImageId = Convert.ToInt32(stuImage.ID);
                    if (!arrStudentImgId.Contains(studentImageId))
                    {
                        arrStudentImgId.Add(studentImageId);
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
            if (arrStudentImgId.Count != 0)
            {
                _objAssignStudent = new AssignStudent(arrStudentImgId);
                _objAssignStudent.ShowDialog();
                if (((AssignStudentViewModel)(_objAssignStudent.DataContext)).isSave)
                    bindStudentGrid();
            }
            else
            {
                MVVMMessageService.ShowMessage(errorMessages.SELECT_STUDENT_IMAGE);
            }
        }

        //added by Mohan
        /// <summary>
        /// this method used to add selected images to existed/newly created group 
        /// </summary>
        internal void addImagesToGroup()
        {
            try
            {
                if (selectedStudentImage == null) { MVVMMessageService.ShowMessage("Please select studentImage(s). "); return; }
                ArrayList arrstuID = new ArrayList();
                int schoolID = clsSchool.defaultSchoolId;
                for (int i = 0; i < selectedStudentImages.Count; i++)
                    arrstuID.Add((selectedStudentImages[i]).ID);

                AddStudentsToGroup objAddStudentsToGroup = new AddStudentsToGroup(schoolID, arrstuID);
                objAddStudentsToGroup.ShowDialog();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method used to delete student record(s)
        /// </summary>
        public void deleteRecords() //#Mohan ; #NUnitTest
        {
            try
            {
                if (selectedGridName == "student")
                {
                    clsStatic.clearTempXML();
                    bool hasError = false;
                    string message = "", studentFirstName = "", studentLastName = "", name = "";
                    ArrayList arrStudId = new ArrayList();
                    ArrayList arrStudentToDelete = new ArrayList();
                    ArrayList arrStudentToShow = new ArrayList();

                    foreach (Student tempStudent in selectedStudents)
                    {
                        try
                        {
                            int studentId = Convert.ToInt32(tempStudent.ID);
                            studentFirstName = Convert.ToString(tempStudent.FirstName);
                            studentLastName = Convert.ToString(tempStudent.Lastname);
                            name = studentFirstName + " " + studentLastName;
                            if (!arrStudId.Contains(studentId))
                            {
                                arrStudId.Add(studentId);
                            }
                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                            MVVMMessageService.ShowMessage(ex.Message);
                        }
                    }
                    if (arrStudId.Count != 0)
                    {
                        if (arrStudId.Count == 1)
                        {
                            message = "Are you sure you want to delete student " + name + "?";
                        }
                        else
                        {
                            message = "Are you sure you want to delete multiple students?";
                        }
                        if (arrStudId.Count != 0)
                        {
                            string caption = "Confirmation";
                            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                            if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                            {
                                //Need to Check Students has images or not..
                                foreach (int studentId in arrStudId)
                                {
                                    int RetStudentImages = clsDashBoard.CountStudentImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), studentId);
                                    if (RetStudentImages <= 0)
                                        arrStudentToDelete.Add(studentId);
                                    else
                                    {
                                        hasError = true;
                                        arrStudentToShow.Add(studentId);
                                        clsErrorLog objError = new clsErrorLog();

                                        objError.Source = "Student";
                                        objError.MethodName = "Delete Student";
                                        objError.Message = "Student has images";
                                        IEnumerable<Student> _objStudentData = clsStudent.getStudentDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), studentId);
                                        foreach (var studentitem in _objStudentData)
                                        {
                                            objError.Parameters = studentitem.FirstName + " " + studentitem.Lastname + "," + studentitem.Grade;
                                        }
                                        clsStatic.WriteErrorLog(objError, objstatic.ExceptionLogXML);
                                        clsStatic.WriteErrorLog(objError, objstatic.ErrorLogXML);
                                    }
                                }
                                if (arrStudentToDelete.Count > 0)
                                {
                                    int delResult = clsStudent.deletestudents(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentToDelete);

                                    //Maintaing Log for Deleted Students .
                                    createDeletedRecordsLogFile("Students", dgStudentsData == null ? 0 : dgStudentsData.Count(), arrStudentToDelete == null ? 0 : arrStudentToDelete.Count);

                                    foreach (int stuId in arrStudId)
                                    {
                                        dgStudentsData.Remove(dgStudentsData.Where(i => i.ID == stuId).First());
                                    }
                                }
                                if (hasError == true)
                                {
                                    string delMessage = errorMessages.AFTER_DELETION_STUDENTS_CONFIRMATION_ERRORS;
                                    string delCaption = "Confirmation";
                                    System.Windows.MessageBoxButton delButtons = System.Windows.MessageBoxButton.YesNo;
                                    System.Windows.MessageBoxImage delIcon = System.Windows.MessageBoxImage.Question;
                                    if (System.Windows.MessageBox.Show(delMessage, delCaption, delButtons, delIcon) == System.Windows.MessageBoxResult.Yes)
                                    {
                                        ShowErrors _objShowErrors = new ShowErrors();
                                        _objShowErrors.ShowDialog();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage("please select student(s)");
                    }
                }
                else if (selectedGridName == "studentimages")
                {
                    if (selectedStudentImages.Count == 0) { MVVMMessageService.ShowMessage("please select student image(s)"); return; }
                    ArrayList arrStudentImages = new ArrayList();
                    string delImgMessage = ""; string photoshootIds = "";
                    try
                    {
                        foreach (StudentImage stuImage in selectedStudentImages)
                        {
                            int studentImgId = Convert.ToInt32(stuImage.ID);
                            if (!arrStudentImages.Contains(studentImgId))
                            {
                                arrStudentImages.Add(studentImgId);
                                photoshootIds = stuImage.PhotoShootID + ",";
                            }
                        }
                        photoshootIds = photoshootIds.Substring(0, photoshootIds.Length - 1);
                        if (arrStudentImages.Count == 1)
                            delImgMessage = errorMessages.BEFORE_DELETION_STUDENT_IMAGE_CONFIRMATION;
                        else
                            delImgMessage = errorMessages.BEFORE_DELETION_STUDENT_IMAGES_CONFIRMATION;

                        string delImgCaption = "Confirmation";
                        System.Windows.MessageBoxButton delImgButtons = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage delImgIcon = System.Windows.MessageBoxImage.Question;
                        if (System.Windows.MessageBox.Show(delImgMessage, delImgCaption, delImgButtons, delImgIcon) == System.Windows.MessageBoxResult.Yes)
                        {
                            GetRating _objGetRating = new GetRating(true, "StudentImages");
                            _objGetRating.ShowDialog();

                            //int totalRecordsCount = dgStudentPhotosData.Count();
                            //int deletedRecordsCount = arrStudentImages.Count;

                            if (((GetRatingViewModel)(_objGetRating.DataContext)).isContinue)
                            {
                                int result = clsDashBoard.deleteStudentImage(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentImages, photoshootIds);
                                foreach (int imgId in arrStudentImages)
                                {
                                    dgStudentPhotosData.Remove(dgStudentPhotosData.Where(i => i.ID == imgId).First());
                                }
                                selectImage();
                                //createDeletedRecordsLogFile("StudentImages", totalRecordsCount, deletedRecordsCount);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }
                //else if (selectedGridName == "Activities")
                //{
                //    if (selectedStudent != null && selectedActivity != null)
                //    {
                //        string message = "Are you sure you want to delete student activity " + selectedActivity.Subject + "?";
                //        string caption = "Confirmation";
                //        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                //        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                //        if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                //        {
                //            int result = clsActivities.deleteMultipleActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), new ArrayList { selectedActivity.Id });
                //            dgStudentActivities = clsActivities.getStudentActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedStudent.ID);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method used to count total images for each student in selected Photoshoot
        /// </summary>
        internal void countImages()
        {
            _objCountStudents = new CountStudents();
            _objCountStudents.ShowDialog();
            PhotoForce.App_Code.PhotographyJob tempSchoolYearSelectedItem = ((CountStudentsViewModel)(_objCountStudents.DataContext)).cbSchoolYearSelectedItem;
            if (((CountStudentsViewModel)(_objCountStudents.DataContext)).isSave)
            {
                arrstudentPhotographyId = new ArrayList();
                arrstudentPhotographyId.Add(tempSchoolYearSelectedItem.ID);
                _objCountDuplicateStudents = new CountDuplicateStudents(arrstudentPhotographyId, "Count Students", tempSchoolYearSelectedItem.JobName);
                if (((CountImagesViewModel)(_objCountDuplicateStudents.DataContext)).dgCountImagesData.Count != 0)
                    _objCountDuplicateStudents.ShowDialog();
            }
        }
        /// <summary>
        /// this method used to edit selected student one at a time
        /// </summary>
        internal void editRecord()
        {
            try
            {
                if (selectedGridName == "student")
                {
                    if (selectedStudent != null)
                    {
                        int studentID = selectedStudent.ID;
                        _objAddEditStudent = new AddEditStudent(studentID, 0, clsSchool.defaultSchoolId);
                        _objAddEditStudent.ShowDialog();
                        if (((AddEditStudentViewModel)(_objAddEditStudent.DataContext)).isSave)
                        {
                            int tempStudentIndex = dgStudentsData.Count <= 1 ? 0 : dgStudentsData.IndexOf(selectedStudent);
                            dgStudentsData.Remove(selectedStudent);

                            Student _objStudent = clsStudent.updateStudent(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), studentID);

                            dgStudentsData.Insert(tempStudentIndex, _objStudent);
                            selectedStudent = _objStudent;
                            selectedStudents.Add(selectedStudent);
                            //bindStudentGrid();
                        }
                    }
                }
                else if (selectedGridName == "studentimages")
                {
                    if (selectedStudentImage != null)
                    {
                        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                        int studentImgID = selectedStudentImage.ID;
                        _objBulkRenameStudentImage = new BulkRenameStudentImage(studentImgID);
                        _objBulkRenameStudentImage.ShowDialog();
                        if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                        {
                            //refreshGrid();
                            //dgStudentPhotosData = clsDashBoard.getStudentImageDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedStudent.ID);
                            StudentImage _objImageDetails = clsDashBoard.getStudentImageDetailsById(db, studentImgID);

                            int tempPhotoIndex = dgStudentPhotosData.Count <= 1 ? 0 : dgStudentPhotosData.IndexOf(selectedStudentImage);
                            dgStudentPhotosData.Remove(selectedStudentImage);
                            selectedStudentImages.Remove(selectedStudentImage);
                            dgStudentPhotosData.Insert(tempPhotoIndex, _objImageDetails);
                            selectedStudentImage = _objImageDetails;
                            selectedStudentImages.Add(selectedStudentImage);
                        }
                    }
                }
                //else if (selectedGridName == "Activities")
                //{
                //    activitiesGridDoubleClick();
                //}
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method used to create new student.
        /// </summary>
        internal void newRecord()
        {
            try
            {
                //if (selectedGridName == "Activities")
                //{
                //    AddEditActivity _objAddEditActivity = new AddEditActivity(new School { SchoolName = clsSchool.defaultSchoolName, ID = clsSchool.defaultSchoolId });
                //    _objAddEditActivity.ShowDialog();
                //    if (((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).isSave)
                //    {
                //        dgStudentActivities = clsActivities.getStudentActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedStudent.ID);
                //    }
                //}
                if (selectedGridName == "student")
                {
                    _objAddStudent = new AddStudent();
                    _objAddStudent.ShowDialog();
                    if (((AddStudentViewModel)(_objAddStudent.DataContext)).isSave)
                    {
                        dgStudentsData.Add(((AddStudentViewModel)(_objAddStudent.DataContext)).addEditStudent);
                        selectedStudent = ((AddStudentViewModel)(_objAddStudent.DataContext)).addEditStudent;
                        //bindStudentGrid();
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
        /// this method used to edit selected students more than one at a time
        /// </summary>
        internal void bulkRename()
        {
            if (selectedGridName == "student")
            {
                try
                {
                    if (selectedStudents.Count != 0)
                    {
                        int tempStudentId = 0;
                        ArrayList arrStudentsId = new ArrayList();
                        foreach (Student tempStudent in selectedStudents)
                        {
                            try
                            {
                                tempStudentId = Convert.ToInt32(tempStudent.ID);
                                if (!arrStudentsId.Contains(tempStudentId))
                                {
                                    arrStudentsId.Add(tempStudentId);
                                }
                            }
                            catch (Exception ex)
                            {
                                MVVMMessageService.ShowMessage(ex.Message);
                                clsStatic.WriteExceptionLogXML(ex);
                            }
                        }

                        BulkRenameStudent _objBulkRenameStudent = new BulkRenameStudent(arrStudentsId);
                        _objBulkRenameStudent.ShowDialog();
                        if (((BulkRenameStudentViewModel)(_objBulkRenameStudent.DataContext)).isSave)
                        {
                            refreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
            else if (selectedGridName == "studentimages")
            {
                if (selectedStudentImages.Count != 0)
                {
                    int stuImgId;
                    ArrayList arrStuId = new ArrayList();
                    foreach (StudentImage stuImage in selectedStudentImages)
                    {
                        try
                        {
                            stuImgId = Convert.ToInt32(stuImage.ID);
                            if (!arrStuId.Contains(stuImgId))
                            {
                                arrStuId.Add(stuImgId);
                            }
                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                            MVVMMessageService.ShowMessage(ex.Message);
                        }
                    }
                    BulkRenameStudentImage _objBulkRenameStudentImage = new BulkRenameStudentImage(arrStuId);
                    _objBulkRenameStudentImage.ShowDialog();
                    if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                    {
                        refreshGrid();
                    }
                }
            }
        }
        internal void studentImportFromStudent()
        {
            try
            {
                _objImportStudent = new ImportStudent(0, 0, "");
                _objImportStudent.ShowDialog();
                bindStudentGrid();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// This method is used to refresh the grid's after some change in data
        /// </summary>
        internal void refreshGrid()
        {
            PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            //bindStudentGrid();
            if (selectedStudent != null)
            {
                int studentId = selectedStudent.ID;
                if (selectedGridName == "student")
                {
                    dgStudentsData = new ObservableCollection<Student>(clsStudent.getStudentsDetailsClass(dbb, clsSchool.defaultSchoolId));
                    if (dgStudentsData.Count != 0)
                    {
                        dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.getStudentImageDetailsBySchool(dbb, dgStudentsData.First().ID, clsSchool.defaultSchoolId));
                    }
                }
                else if (selectedGridName == "studentimages")
                {
                    dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.getStudentImageDetailsBySchool(dbb, studentId, clsSchool.defaultSchoolId));
                }

            }
        }
        /// <summary>
        /// This method used to generate password for selected/filtered students or for students without password
        /// </summary>
        internal void generatePassword()
        {
            try
            {
                ArrayList arrStudId = new ArrayList();
                foreach (Student tempStudent in selectedStudents)
                {
                    int studentId = Convert.ToInt32(tempStudent.ID);
                    if (!arrStudId.Contains(studentId))
                    {
                        arrStudId.Add(studentId);
                    }
                }
                _objGeneratePassword = new GeneratePassword(arrStudId);
                _objGeneratePassword.ShowDialog();

                //if (_objGeneratePassword.DataContext)
                if (((GeneratePasswordViewModel)(_objGeneratePassword.DataContext)).isSave)
                {
                    MVVMMessageService.ShowMessage("Password assginment completed.");
                    bindStudentGrid();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method used to open source folder
        /// </summary>
        internal void OpenFolder()
        {
            if (dgStudentPhotosData.Count() != 0)
            {
                if (Directory.Exists(ImageFolderPath))
                {
                    string args = string.Format("/Select, \"{0}\"", strFileToShow);
                    ProcessStartInfo pfi = new ProcessStartInfo("Explorer.exe", args);

                    System.Diagnostics.Process.Start(pfi);
                }
                else
                    MVVMMessageService.ShowMessage(errorMessages.PATH_DOES_NOT_EXISTS);
            }
        }

        # region Search panel
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        internal void searchPanels()
        {
            if (selectedGridName == "student")
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
            else if (selectedGridName == "studentimages")
            {
                if (studentPhotosSearchControl == null || isSearchControlVisible == false)
                {
                    studentPhotosSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    studentPhotosSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
                }
            }
        }
        # endregion

        # region Group panels
        /// <summary>
        /// This method is used to group panels based on selected column
        /// </summary>
        internal void groupPanels()
        {
            if (selectedGridName == "student")
            {
                if (studentShowGroupPanel)
                    studentShowGroupPanel = false;
                else
                    studentShowGroupPanel = true;
            }
            else if (selectedGridName == "studentimages")
            {
                if (studentPhotosShowGroupPanel)
                    studentPhotosShowGroupPanel = false;
                else
                    studentPhotosShowGroupPanel = true;
            }
        }
        # endregion
        /// <summary>
        /// This method is used to export selected/filtered/all students
        /// </summary>
        internal void exportStudents()
        {
            if (selectedStudent == null) { MVVMMessageService.ShowMessage("Please select a student. "); return; }
            ArrayList arrSelectedStudID = new ArrayList();
            ArrayList arrFilteredStudID = new ArrayList();
            foreach (Student tempStudent in selectedStudents)
            {
                try
                {
                    int studentId = Convert.ToInt32(tempStudent.ID);
                    if (!arrSelectedStudID.Contains(studentId))
                    {
                        arrSelectedStudID.Add(studentId);
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }

            List<Student> tempStudentData = new List<Student>(dgStudentsData);
            foreach (Student tempStudent in VisibleData.Count == 0 ? tempStudentData : VisibleData)
            {
                try
                {
                    int studentId = Convert.ToInt32(tempStudent.ID);
                    if (!arrFilteredStudID.Contains(studentId))
                    {
                        arrFilteredStudID.Add(studentId);
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }

            }
            _objExportStudent = new ExportStudent(0, "", arrSelectedStudID, arrFilteredStudID);
            _objExportStudent.ShowDialog();
        }

        //Added By Mohan
        private Student getStudentDataTableRow(DataRow dr, bool updateThreeFields)
        {
            Student _objStudent = new Student();
            try
            {
                _objStudent.ID = Convert.ToInt32(dr["ID"]);

                if (updateThreeFields)
                {
                    if (!string.IsNullOrEmpty(dr["StudentID"].ToString())) { _objStudent.StudentID = dr["StudentID"].ToString(); }
                    if (!string.IsNullOrEmpty(dr["FirstName"].ToString())) { _objStudent.FirstName = dr["FirstName"].ToString(); }
                    if (!string.IsNullOrEmpty(dr["Lastname"].ToString())) { _objStudent.Lastname = dr["Lastname"].ToString(); }
                }
                _objStudent.Password = dr["Password"].ToString();
                _objStudent.Teacher = dr["Teacher"].ToString();
                _objStudent.Grade = dr["Grade"].ToString();
                _objStudent.Custom1 = dr["Custom1"].ToString();
                _objStudent.Custom2 = dr["Custom2"].ToString();
                _objStudent.Custom3 = dr["Custom3"].ToString();
                _objStudent.Custom4 = dr["Custom4"].ToString();
                _objStudent.Custom5 = dr["Custom5"].ToString();
                if (dr["DOB"].ToString() == "") { _objStudent.DOB = null; } else { _objStudent.DOB = Convert.ToDateTime(dr["DOB"]); }
                _objStudent.Address = dr["Address"].ToString();
                _objStudent.City = dr["City"].ToString();
                _objStudent.State = dr["State"].ToString();
                _objStudent.Zip = dr["Zip"].ToString();
                _objStudent.Phone = dr["Phone"].ToString();
                _objStudent.Emailaddress = dr["Emailaddress"].ToString();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
            return _objStudent;
        }

        internal void reImportStudents(bool updateThreeFields)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "Excel Files (*.xlsx)|*.xlsx|All files (*.*)|*.* " };
            var result = ofd.ShowDialog();
            if (result == false) return;
            string fullFileName = ofd.FileName;

            try
            {
                FileStream stream = File.Open(fullFileName, FileMode.Open, FileAccess.Read);
                //...
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //...
                //3. DataSet - Create column names from first row
                excelReader.IsFirstRowAsColumnNames = true;
                DataSet ds = excelReader.AsDataSet();

                //4. Free resources (IExcelDataReader is IDisposable)
                excelReader.Close();

                //checking wether any column is missed.
                DataRow dr = ds.Tables[0].Rows[0];
                try
                {
                    int ID = Convert.ToInt32(dr["ID"]);
                    string StudentID = ""; string FirstName = ""; string Lastname = "";
                    if (updateThreeFields)
                    {
                        if (string.IsNullOrEmpty(dr["StudentID"].ToString())) { StudentID = dr["StudentID"].ToString(); }
                        if (string.IsNullOrEmpty(dr["FirstName"].ToString())) { FirstName = dr["FirstName"].ToString(); }
                        if (string.IsNullOrEmpty(dr["Lastname"].ToString())) { Lastname = dr["Lastname"].ToString(); }
                    }

                    string Password = dr["Password"].ToString();
                    string Teacher = dr["Teacher"].ToString();
                    string Grade = dr["Grade"].ToString();
                    string Custom1 = dr["Custom1"].ToString();
                    string Custom2 = dr["Custom2"].ToString();
                    string Custom3 = dr["Custom3"].ToString();
                    string Custom4 = dr["Custom4"].ToString();
                    string Custom5 = dr["Custom5"].ToString();
                    //DateTime? DOB = dr["DOB"] == DBNull.Value ? DBNull.Value : Convert.ToDateTime(dr["DOB"]);
                    string DOB = dr["DOB"].ToString();
                    string Address = dr["Address"].ToString();
                    string City = dr["City"].ToString();
                    string State = dr["State"].ToString();
                    string Zip = dr["Zip"].ToString();
                    string Phone = dr["Phone"].ToString();
                    string Emailaddress = dr["Emailaddress"].ToString();
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage("Cannot find a required column(s)." + Environment.NewLine + "Please check the excel file.");
                    return;
                }


                List<Student> LstStudents = new List<Student>();
                //converting data table to list using data row
                LstStudents = new List<Student>(
                               (from dRow in ds.Tables[0].AsEnumerable()
                                select (getStudentDataTableRow(dRow, updateThreeFields)))
                               );

                if (LstStudents != null)
                {
                    updateMultipleStudents(LstStudents, updateThreeFields); //updating password,custom1,custom2,custom3 in DB
                    refreshGrid();
                    MVVMMessageService.ShowMessage("Student records are updated.");
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage("The process cannot access the file " + fullFileName + " because it is being used by another process.");
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        public void updateMultipleStudents(List<Student> lstStudents, bool updateThreeFields)
        {
            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            foreach (Student stu in lstStudents)
            {
                try
                {
                    if (stu.ID != 0)
                    {
                        Student tempStudent = new Student();
                        tempStudent = clsStudent.updateStudent(db, stu.ID);

                        if (tempStudent == null) { continue; }
                        tempStudent.Grade = stu.Grade;
                        tempStudent.Password = stu.Password; tempStudent.Phone = stu.Phone;
                        tempStudent.State = stu.State; tempStudent.Teacher = stu.Teacher;
                        tempStudent.Zip = stu.Zip; tempStudent.Address = stu.Address; tempStudent.Emailaddress = stu.Emailaddress;
                        tempStudent.City = stu.City; tempStudent.Custom1 = stu.Custom1; tempStudent.Custom2 = stu.Custom2;
                        tempStudent.Custom3 = stu.Custom3; tempStudent.Custom4 = stu.Custom4; tempStudent.Custom5 = stu.Custom5;
                        tempStudent.DOB = stu.DOB;

                        if (updateThreeFields)
                        {
                            tempStudent.StudentID = stu.StudentID;
                            tempStudent.FirstName = stu.FirstName;
                            tempStudent.Lastname = stu.Lastname;
                        }

                        db.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }

        }

        internal void imageWithoutStudent()
        {
            try
            {
                //   _getimages = new GetImagesWithoutStudent();
                //   _getimages.ShowDialog();
                //BindStudentGrid();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to generate QRCode fro selected/filtered/all Students
        /// </summary>
        internal void generateQRCode()
        {
            try
            {
                string selectedStudId = "";
                string FilteredStudId = "";
                ArrayList arrSelectedStudId = new ArrayList();
                ArrayList arrFilteredStudId = new ArrayList();
                ArrayList arrSchoolId = new ArrayList();
                ArrayList arrImportId = new ArrayList();
                foreach (Student tempStudent in selectedStudents)
                {
                    try
                    {
                        int studentId = Convert.ToInt32(tempStudent.ID);
                        int schoolId = Convert.ToInt32(tempStudent.StudentImport.School.ID);
                        int ImportBatchId = Convert.ToInt32(tempStudent.StudentImport.ID);
                        if (!arrSelectedStudId.Contains(studentId))
                        {
                            arrSelectedStudId.Add(studentId);
                            selectedStudId += studentId + ",";
                        }
                        if (!arrImportId.Contains(ImportBatchId))
                            arrImportId.Add(ImportBatchId);
                        if (!arrSchoolId.Contains(schoolId))
                            arrSchoolId.Add(schoolId);
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }

                if (arrSchoolId.Count <= 1)
                {
                    List<Student> tempStudentData = new List<Student>(dgStudentsData);
                    foreach (Student tempStudent in VisibleData.Count == 0 ? tempStudentData : VisibleData)
                    {
                        try
                        {
                            int studentID = Convert.ToInt32(tempStudent.ID);
                            int ImportBatchID = Convert.ToInt32(tempStudent.StudentImport.ID);
                            if (!arrFilteredStudId.Contains(studentID))
                            {
                                arrFilteredStudId.Add(studentID);
                                FilteredStudId += studentID + ",";
                            }
                            if (!arrImportId.Contains(ImportBatchID))
                                arrImportId.Add(ImportBatchID);
                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                            MVVMMessageService.ShowMessage(ex.Message);
                        }
                    }
                    //mohan
                    //if (isFirstNameSorted != -1) { sortedColumn.Add("FirstName"); } //By default data comes in sort order so no need to do sorting again
                    //if (isLastNameSorted != -1) { sortedColumn.Add("LastName"); }
                    //if (isTeacherSorted != -1) { sortedColumn.Add("Teacher"); }
                    //if (isGradeSorted != -1) { sortedColumn.Add("Grade"); }
                    //if (isSchoolNameSorted != -1) { sortedColumn.Add("SchoolName"); }

                    _objGenerateStudentQRCode = new GenerateStudentQRCode(arrSelectedStudId, arrFilteredStudId, arrImportId);//, sortedColumn.ToArray<string>()
                    _objGenerateStudentQRCode.ShowDialog();
                }
                else
                {
                    string delmessage = errorMessages.TWO_SCHOOL_SELECT_ERRORS;
                    MVVMMessageService.ShowMessage(delmessage);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }

        }
        /// <summary>
        /// Just to show the selected student image on imgStudentPhotoPreview grid
        /// </summary>
        /// <param name="gridName">Name of the grid</param>
        private void selectImage()
        {
            try
            {
                if (selectedStudentImage != null)
                {
                    int imageId = selectedStudentImage.ID;
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
        /// <summary>
        /// this method used to deactivate active students
        /// </summary>
        internal void deactivateStudents()
        {
            string message = "";
            ArrayList arrSelectedStudID = new ArrayList();
            if (selectedStudent != null)
            {
                foreach (Student tempStudent in selectedStudents)
                {
                    try
                    {
                        int sTudentID = tempStudent.ID;
                        if (!arrSelectedStudID.Contains(sTudentID))
                        {
                            arrSelectedStudID.Add(sTudentID);
                        }
                    }
                    catch (Exception ex)
                    { clsStatic.WriteExceptionLogXML(ex); }
                }
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (arrSelectedStudID.Count >= 1)
                {
                    message = errorMessages.BEFORE_DEACTIVATE_STUDENTS_CONFIRMATION1 + arrSelectedStudID.Count + errorMessages.BEFORE_DEACTIVATE_STUDENTS_CONFIRMATION2;
                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        int retVal = clsStudent.DeactivateStudent(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrSelectedStudID);
                        bindStudentGrid();
                    }
                }
                else
                {
                    MVVMMessageService.ShowMessage("Please select a student to de-activate");
                }
            }
        }

        internal void renameSourceImages()
        {
            try
            {
                if (selectedStudent == null) { MVVMMessageService.ShowMessage("Please select a student. "); return; }
                PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string message = "";
                // Need to Delete Data First from ErrorLog table...
                int delval = clsDashBoard.deleteErrorLog(db);

                ArrayList arrstudentimgid = new ArrayList();
                ObservableCollection<StudentImage> tempselectedImages = new ObservableCollection<StudentImage>();
                if (selectedGridName == "student")
                {
                    //message = errorMessages.BEFORE_RENAME_SOURCE_IMAGES1_SELECTED + errorMessages.BEFORE_RENAME_SOURCE_IMAGES2;
                    tempselectedImages = new ObservableCollection<StudentImage>(clsDashBoard.getStudentImageDetailsBySchool(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedStudent.ID, clsSchool.defaultSchoolId));
                    if (tempselectedImages.Count <= 0)
                    {
                        MVVMMessageService.ShowMessage("Selected student doesn't have images. ");
                        return;
                    }
                }
                else
                {
                    tempselectedImages = selectedStudentImages;
                    if (selectedStudentImages.Count == 0)
                    {
                        return;
                    }
                }

                message = errorMessages.BEFORE_RENAME_SOURCE_IMAGES1_SELECTED + errorMessages.BEFORE_RENAME_SOURCE_IMAGES2_SELECTED;
                foreach (StudentImage stuImage in tempselectedImages)
                {
                    try
                    {
                        int StudentImageID = stuImage.ID;
                        if (!arrstudentimgid.Contains(StudentImageID))
                            arrstudentimgid.Add(StudentImageID);
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }


                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    ProgressBarRename _objProgressBarRename = new ProgressBarRename(0, ImageFolderPath, "studentimages", arrstudentimgid);
                    if (((ProgressBarRenameViewModel)(_objProgressBarRename.DataContext)).isError)
                    {
                        string messagecl = "Error while renaming source images. Do you want to open the error log?";
                        string captioncl = "Confirmation";
                        System.Windows.MessageBoxButton buttonss = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage iconn = System.Windows.MessageBoxImage.Question;
                        if (MVVMMessageService.ShowMessage(messagecl, captioncl, buttonss, iconn) == System.Windows.MessageBoxResult.Yes)
                            Process.Start(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME + "\\errorLog.xml");
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage("Rename source images completed.");
                    }
                    refreshGrid();
                }

            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        internal void switchStudentFLNames()
        {
            if (selectedGridName == "student" && selectedStudents.Count > 0)
            {
                string message = "This proces will switch the selected student(s) first and last names. \nDo want to continue ?";
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    foreach (Student st in selectedStudents)
                    {
                        clsStudent.swapStudentFirstNLastNames(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), st.FirstName, st.Lastname, st.ID);
                    }
                    bindStudentGrid();
                }
            }
        }
        internal void deleteManualStudents()
        {
            if (selectedGridName == "student" && dgStudentsData.Count > 0)
            {
                string message = "This proces will delete the manual student(s) without images . \nDo want to continue ?";
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    int result = clsStudent.deleteManualStudents(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId);
                    if (result != 0)
                    {
                        bindStudentGrid();
                        // MVVMMessageService.ShowMessage( result + " manual student(s) were deleted."  );
                    }
                }
            }
        }
        private void studentTableMouseUp()
        {
            if (selectedStudent != null)
            {
                setVisibilityForStudentGrid();
                try
                {
                    strFileToShow = "";
                    dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.getStudentImageDetailsBySchool(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedStudent.ID, clsSchool.defaultSchoolId));
                    if (dgStudentPhotosData.Count() > 0)
                    {
                        selectedStudentImage = dgStudentPhotosData.First();
                        selectedGridName = "student";
                        selectImage();
                    }
                    else
                    {
                        studentPhotoPreview = null;
                    }
                    //dgStudentActivities = clsActivities.getStudentActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedStudent.ID);
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.ToString());
                }
            }
        }
        private void studentGroupFocus()
        {
            selectedGridName = "student";
            setVisibilityForStudentGrid();
        }
        private void studentGridMouseDoubleClick()
        {
            selectedGridName = "student";
            editRecord();
        }
        private void studentPhotosMouseDoubleClick()
        {
            selectedGridName = "studentimages";
            editRecord();
        }
        private void studentPhotosGroupFocus()
        {
            selectedGridName = "studentimages";
            setVisibilityForPhotosGrid();
        }
        private void studentPhotoPreviewMouseUp()
        {
            try
            {
                selectedGridName = "studentimages";
                string filePath = strFileToShow.ToString();
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
                        filePath = strFilereduced.ToString();
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
        private void studentPhotosTableMouseUp()
        {
            setVisibilityForPhotosGrid();
            if (selectedStudentImage != null)
            {
                ImageFolderPath = selectedStudentImage.PhotoShoot.ImageFolder;
                selectedGridName = "studentimages";
                selectImage();
            }
            else
            {
                (Application.Current as App).isOpenFolderVisible = false;
            }
        }

        #region Buttons visibility based on the grid selection
        /// <summary>
        /// this method used to set the buttons visibility for student grid selected
        /// </summary>
        private void setVisibilityForStudentGrid()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isSearchVisible = true; (Application.Current as App).isRenameSourceImagesVisible = true;
            (Application.Current as App).isDragVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isRefreshVisible = true;
        }
        /// <summary>
        /// this method used to set the buttons visibility for student-images grid selected
        /// </summary>
        private void setVisibilityForPhotosGrid()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = false;
            (Application.Current as App).isEditVisible = true; (Application.Current as App).isOpenFolderVisible = true;
            (Application.Current as App).isDeleteVisible = true; (System.Windows.Application.Current as App).isAddToGroupVisible = true;
            (Application.Current as App).isSearchVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (Application.Current as App).isRefreshVisible = true; (Application.Current as App).isAssignStudentVisible = true;
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isRefreshVisible = true;
            (Application.Current as App).isRenameSourceImagesVisible = true;
        }
        #endregion

        #endregion
    }
}
