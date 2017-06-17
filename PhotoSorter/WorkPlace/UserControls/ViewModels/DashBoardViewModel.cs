using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System.Windows.Media;
using System.Data;
using System.Collections;
using PhotoForce.StudentImageManagement;
using PhotoForce.PhotoShoot_Management;
using PhotoForce.Student_Management;
using PhotoForce.PhotographyJobManagement;
using PhotoForce.School_Management;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;
using PhotoForce.Extensions;
using System.Drawing;
using System.Drawing.Drawing2D;
using PhotoForce.Helpers;
using Ookii.Dialogs.Wpf;
using PhotoForce.GroupManagement;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using Aurigma.GraphicsMill.Codecs;
using PhotoForce.OrdersManagement;
using PhotoForce.WorkflowManagement;

namespace PhotoForce.WorkPlace.UserControls
{
    public class DashBoardViewModel : ViewModelBase
    {
        #region Initialization
        BulkRenameStudentImage _objBulkRenameStudentImage;
        AddEditPhotoShoot addEditPhotoShoot;
        CountImages countimg;
        ImportStudent _objstuimp;
        YearbookSelection _objyrbksel;
        GetRating _objimgrt;
        SyncDeleteMessage _objsyncdelimg;
        AssignStudent _objAssignStudent;
        DeviceScanning _objDeviceScanning;
        //ValidateAdminCD valAdminCd;
        //AdminYearBookCD adminyearcd;
        AutoCreateGroups _objautocreategroup;
        clsStatic objstatic = new clsStatic();
        clsErrorLog objError = new clsErrorLog();
        //String selectedGridName = "school";
        //string ImageFolderPath = "";
        string PhotoShootName = "";
        string Photoshootpath = "";
        static string strFileToShow = "";
        string strFilereduced = "";
        int photo = 0;
        PhotoSorterDBModelDataContext db;
        ProgressBarRename _objProgressBarRename;
        int selectedPhotoShootId = 0;
        PhotoshootWorkflowItem oldCollectionItem;
        int photoshootWorkflowItemId = 0;
        string selectedButton = "";
        #endregion

        #region Properties
        ObservableCollection<PhotoShoot> _dgPhotoShootData;
        ObservableCollection<StudentImage> _dgStudentPhotosData;
        //List<Activity> _dgPhotoShootActivities;
        //Activity _selectedActivity;
        PhotoShoot _selectedPhotoShoot;
        StudentImage _selectedStudentImage;
        ImageSource _studentImagePreview;
        ObservableCollection<StudentImage> _selectedStudentImages;
        ObservableCollection<PhotoShoot> _selectedPhotoShoots;
        private bool _photosTableShowGroupPanel;
        private bool _isSearchControlVisible;
        private ShowSearchPanelMode _photosTableSearchPanelMode;
        private SearchControl _photosTableSearchControl;
        private string _selectedGridName;
        private int _selectedPhotoShootCount;
        private int _selectedStudentPhotosCount;
        List<StudentImage> _VisibleData;
        private int _selectedIndex;
        //photoshoot search , group panels 
        private bool _photoshootTableShowGroupPanel;
        private bool _isPhotoshootSearchControlVisible;
        private ShowSearchPanelMode _photoshootTableSearchPanelMode;
        private SearchControl _photoshootTableSearchControl;
        Visibility _studentImagePreviewVisibility;
        Visibility _notesPreviewVisibility;
        ObservableCollection<ComboBoxItem> _workflowItemNotes;
        Visibility _notesTextVisibility;
        string _notes;

        public string notes
        {
            get { return _notes; }
            set { _notes = value; NotifyPropertyChanged(); }
        }
        public Visibility notesTextVisibility
        {
            get { return _notesTextVisibility; }
            set { _notesTextVisibility = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<ComboBoxItem> workflowItemNotes
        {
            get { return _workflowItemNotes; }
            set { _workflowItemNotes = value; NotifyPropertyChanged(); }
        }
        public Visibility notesPreviewVisibility
        {
            get { return _notesPreviewVisibility; }
            set { _notesPreviewVisibility = value; NotifyPropertyChanged("notesPreviewVisibility"); }
        }
        public Visibility studentImagePreviewVisibility
        {
            get { return _studentImagePreviewVisibility; }
            set { _studentImagePreviewVisibility = value; NotifyPropertyChanged("studentImagePreviewVisibility"); }
        }

        #region Workflow
        private int _selectedPhotoShootIndex;
        ObservableCollection<PhotoshootWorkflowItem> _dgWorkflowItemsData;
        ObservableCollection<PhotoshootWorkflowItem> _selectedWorkflowItems;
        PhotoshootWorkflowItem _selectedWorkflowItem;
        ComboBoxItem _selectedNotes;
        ObservableCollection<PhotoshootWorkflowItem> _dgEquipmentData;
        ObservableCollection<PhotoshootWorkflowItem> _selectedEquipmentItems;
        PhotoshootWorkflowItem _selectedEquipmentItem;

        public ObservableCollection<PhotoshootWorkflowItem> selectedEquipmentItems
        {
            get { return _selectedEquipmentItems; }
            set { _selectedEquipmentItems = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<PhotoshootWorkflowItem> dgEquipmentData
        {
            get { return _dgEquipmentData; }
            set { _dgEquipmentData = value; NotifyPropertyChanged(); }
        }
        public ComboBoxItem selectedNotes
        {
            get { return _selectedNotes; }
            set { _selectedNotes = value; NotifyPropertyChanged("selectedNotes"); }
        }
        public ObservableCollection<PhotoshootWorkflowItem> dgWorkflowItemsData
        {
            get { return _dgWorkflowItemsData; }
            set { _dgWorkflowItemsData = value; NotifyPropertyChanged("dgWorkflowItemsData"); }
        }
        public ObservableCollection<PhotoshootWorkflowItem> selectedWorkflowItems
        {
            get { return _selectedWorkflowItems; }
            set { _selectedWorkflowItems = value; NotifyPropertyChanged("selectedWorkflowItems"); }
        }
        public PhotoshootWorkflowItem selectedWorkflowItem
        {
            get { return _selectedWorkflowItem; }
            set
            {

                if (_selectedWorkflowItem != null)
                {
                    //check wether user updated sort order || default price from UI 
                    //if yes updated on DB aswell
                    if (!_selectedWorkflowItem.Equals(oldCollectionItem))
                    {
                        updateSortOrder(selectedWorkflowItem);
                    }
                }
                _selectedWorkflowItem = value; NotifyPropertyChanged("selectedWorkflowItem");

                //user can edit sort order , default price from UI  in order to update in DB store selectedPackage in a temp object and do a comparison at start
                if (selectedWorkflowItem != null) { oldCollectionItem = new PhotoshootWorkflowItem { Assignedto = selectedWorkflowItem.Assignedto, SortOrder = selectedWorkflowItem.SortOrder, Status = selectedWorkflowItem.Status, DueDate = selectedWorkflowItem.DueDate, CreatedOn = selectedWorkflowItem.CreatedOn, CreatedBy = selectedWorkflowItem.CreatedBy, Id = selectedWorkflowItem.Id }; }
            }
        }
        public PhotoshootWorkflowItem selectedEquipmentItem
        {
            get { return _selectedEquipmentItem; }
            set
            {
                _selectedEquipmentItem = value; NotifyPropertyChanged();
                equipmentItemMouseLeftClick();
            }
        }

        public int selectedPhotoShootIndex
        {
            get { return _selectedPhotoShootIndex; }
            set
            {
                _selectedPhotoShootIndex = value; NotifyPropertyChanged("selectedPhotoShootIndex");

                if (selectedPhotoShootIndex == 0)
                {
                    selectedGridName = "studentPhotos";
                    notesPreviewVisibility = Visibility.Collapsed;
                    studentImagePreviewVisibility = Visibility.Visible;
                    if (selectedPhotoShoot != null)
                        selectPhotoShoot(selectedPhotoShoot.PhotoShotID);
                    else
                    {
                        dgStudentPhotosData = new ObservableCollection<StudentImage>();
                        selectImage();
                    }
                }
                if (selectedPhotoShootIndex == 1)
                {
                    selectedGridName = "workflow";
                    studentImagePreviewVisibility = Visibility.Collapsed;
                    notesPreviewVisibility = Visibility.Visible;

                    if (selectedPhotoShoot != null)
                        selectPhotoShoot(selectedPhotoShoot.PhotoShotID);
                    else
                        dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>();
                    setButtonVisibilityforWorkflowItems();

                }
                if (selectedPhotoShootIndex == 2)
                {
                    selectedGridName = "equipment";
                    studentImagePreviewVisibility = Visibility.Collapsed;
                    notesPreviewVisibility = Visibility.Collapsed;

                    if (selectedPhotoShoot != null)
                        selectPhotoShoot(selectedPhotoShoot.PhotoShotID);
                    else
                        dgEquipmentData = new ObservableCollection<PhotoshootWorkflowItem>();

                    (Application.Current as App).isAddCollectionButtonVisible = false; (Application.Current as App).isAddEquipmentItemsButtonVisible = true;

                    (Application.Current as App).isEditVisible = true;
                }

            }
        }
        #region Search & Group Panels
        bool _workflowItemsShowGroupPanel;
        ShowSearchPanelMode _workflowItemsSearchPanelMode;
        SearchControl _workflowItemsSearchControl;
        bool _workflowItemsSearchControlVisible;

        public bool workflowItemsSearchControlVisible
        {
            get { return _workflowItemsSearchControlVisible; }
            set { _workflowItemsSearchControlVisible = value; NotifyPropertyChanged("workflowItemsSearchControlVisible"); }
        }

        public SearchControl workflowItemsSearchControl
        {
            get { return _workflowItemsSearchControl; }
            set { _workflowItemsSearchControl = value; NotifyPropertyChanged("workflowItemsSearchControl"); }
        }
        public ShowSearchPanelMode workflowItemsSearchPanelMode
        {
            get { return _workflowItemsSearchPanelMode; }
            set { _workflowItemsSearchPanelMode = value; NotifyPropertyChanged("workflowItemsSearchPanelMode"); }
        }
        public bool workflowItemsShowGroupPanel
        {
            get { return _workflowItemsShowGroupPanel; }
            set { _workflowItemsShowGroupPanel = value; NotifyPropertyChanged("workflowItemsShowGroupPanel"); }
        }
        #endregion
        #endregion

        #region Equipment
        bool _equipmentItemsShowGroupPanel;
        ShowSearchPanelMode _equipmentItemsSearchPanelMode;
        SearchControl _equipmentItemsSearchControl;
        bool _equipmentItemsSearchVisible;

        public bool equipmentItemsSearchVisible
        {
            get { return _equipmentItemsSearchVisible; }
            set { _equipmentItemsSearchVisible = value; NotifyPropertyChanged(); }
        }

        public SearchControl equipmentItemsSearchControl
        {
            get { return _equipmentItemsSearchControl; }
            set { _equipmentItemsSearchControl = value; NotifyPropertyChanged(); }
        }

        public ShowSearchPanelMode equipmentItemsSearchPanelMode
        {
            get { return _equipmentItemsSearchPanelMode; }
            set { _equipmentItemsSearchPanelMode = value; NotifyPropertyChanged(); }
        }

        public bool equipmentItemsShowGroupPanel
        {
            get { return _equipmentItemsShowGroupPanel; }
            set { _equipmentItemsShowGroupPanel = value; NotifyPropertyChanged(); }
        }

        #endregion

        public int selectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value; NotifyPropertyChanged("selectedIndex");
                if (selectedIndex == 0)
                {
                    (Application.Current as App).isNewVisible = false; notesPreviewVisibility = Visibility.Collapsed;
                    (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
                }
                else
                {
                    (Application.Current as App).isNewVisible = true;
                    (Application.Current as App).isDragVisible = false; (Application.Current as App).isSearchVisible = false;
                    selectedGridName = "Activities";
                }
            }
        }
        //public Activity selectedActivity
        //{
        //    get { return _selectedActivity; }
        //    set { _selectedActivity = value; NotifyPropertyChanged("selectedActivity"); }
        //}
        //public List<Activity> dgPhotoShootActivities
        //{
        //    get { return _dgPhotoShootActivities; }
        //    set { _dgPhotoShootActivities = value; NotifyPropertyChanged("dgPhotoShootActivities"); }
        //}
        public List<StudentImage> VisibleData
        {
            get { return _VisibleData; }
            set { _VisibleData = value; NotifyPropertyChanged("VisibleData"); }
        }
        public int selectedStudentPhotosCount
        {
            get { return _selectedStudentPhotosCount; }
            set { _selectedStudentPhotosCount = value; NotifyPropertyChanged("selectedStudentPhotosCount"); }
        }
        public int selectedPhotoShootCount
        {
            get { return _selectedPhotoShootCount; }
            set { _selectedPhotoShootCount = value; NotifyPropertyChanged("selectedPhotoShootCount"); }
        }
        public string selectedGridName
        {
            get { return _selectedGridName; }
            set { _selectedGridName = value; NotifyPropertyChanged("selectedGridName"); }
        }
        public bool photosTableShowGroupPanel
        {
            get { return _photosTableShowGroupPanel; }
            set { _photosTableShowGroupPanel = value; NotifyPropertyChanged("photosTableShowGroupPanel"); }
        }
        public ShowSearchPanelMode photosTableSearchPanelMode
        {
            get { return _photosTableSearchPanelMode; }
            set { _photosTableSearchPanelMode = value; NotifyPropertyChanged("photosTableSearchPanelMode"); }
        }
        public SearchControl photosTableSearchControl
        {
            get { return _photosTableSearchControl; }
            set { _photosTableSearchControl = value; NotifyPropertyChanged("photosTableSearchControl"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public ObservableCollection<PhotoShoot> selectedPhotoShoots
        {
            get { return _selectedPhotoShoots; }
            set { _selectedPhotoShoots = value; NotifyPropertyChanged("selectedPhotoShoots"); }
        }
        public ImageSource studentImagePreview
        {
            get { return _studentImagePreview; }
            set { _studentImagePreview = value; NotifyPropertyChanged("studentImagePreview"); }
        }
        public StudentImage selectedStudentPhoto
        {
            get { return _selectedStudentImage; }
            set { _selectedStudentImage = value; NotifyPropertyChanged("selectedStudentPhoto"); selectedStudentPhotosCount = selectedStudentPhotos.Count; }
        }
        public PhotoShoot selectedPhotoShoot
        {
            get { return _selectedPhotoShoot; }
            set
            {
                _selectedPhotoShoot = value; NotifyPropertyChanged("selectedPhotoShoot"); selectedPhotoShootCount = selectedPhotoShoots.Count;
                photoShootGridLeftClick();
            }
        }
        public ObservableCollection<StudentImage> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        public ObservableCollection<PhotoShoot> dgPhotoShootData
        {
            get { return _dgPhotoShootData; }
            set { _dgPhotoShootData = value; NotifyPropertyChanged("dgPhotoShootData"); }
        }
        public ObservableCollection<StudentImage> selectedStudentPhotos
        {
            get { return _selectedStudentImages; }
            set { _selectedStudentImages = value; }
        }

        public bool photoshootTableShowGroupPanel
        {
            get { return _photoshootTableShowGroupPanel; }
            set { _photoshootTableShowGroupPanel = value; NotifyPropertyChanged("photoshootTableShowGroupPanel"); }
        }
        public bool isPhotoshootSearchControlVisible
        {
            get { return _isPhotoshootSearchControlVisible; }
            set { _isPhotoshootSearchControlVisible = value; NotifyPropertyChanged("isPhotoshootSearchControlVisible"); }
        }
        public ShowSearchPanelMode photoshootTableSearchPanelMode
        {
            get { return _photoshootTableSearchPanelMode; }
            set { _photoshootTableSearchPanelMode = value; NotifyPropertyChanged("photoshootTableSearchPanelMode"); }
        }
        public SearchControl photoshootTableSearchControl
        {
            get { return _photoshootTableSearchControl; }
            set { _photoshootTableSearchControl = value; NotifyPropertyChanged("photoshootTableSearchControl"); }
        }
        #endregion

        #region Constructor
        public DashBoardViewModel(string tempGridName)
        {
            selectedStudentPhotos = new ObservableCollection<StudentImage>(); selectedPhotoShoots = new ObservableCollection<PhotoShoot>();
            selectedIndex = 0; photosTableShowGroupPanel = false; notesTextVisibility = Visibility.Collapsed; workflowItemNotes = new ObservableCollection<ComboBoxItem>();
            selectedWorkflowItems = new ObservableCollection<PhotoshootWorkflowItem>();
            dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>();
            selectedEquipmentItems = new ObservableCollection<PhotoshootWorkflowItem>();
            dgEquipmentData = new ObservableCollection<PhotoshootWorkflowItem>();

            selectedGridName = tempGridName;
            loadPhotoShoot(clsSchool.defaultSchoolId);
            RenameFailedImages();
            (Application.Current as App).isDuplicatePhotoShootsButtonVisible = true;
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
        public RelayCommand StudentPhotosTableKeyUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotosTableKeyUp);
            }
        }
        public RelayCommand StudentPhotosDoubleClickCommand
        {
            get
            {
                return new RelayCommand(studentPhotosDoubleClick);
            }
        }
        public RelayCommand StudentPhotosPanelFocusCommand
        {
            get
            {
                return new RelayCommand(studentPhotosPanelFocus);
            }
        }
        public RelayCommand PhotoShootGridDoubleClickCommand
        {
            get
            {
                return new RelayCommand(photoShootGridDoubleClick);
            }
        }
        public RelayCommand PhotoShootTableViewMouseUpCommand
        {
            get
            {
                return new RelayCommand(photoShootTableViewMouseUp);
            }
        }
        public RelayCommand EqipmentPanelFocusCommand
        {
            get
            {
                return new RelayCommand(photoShootsGridButtonsVisibility);
            }

        }
        public RelayCommand PhotoShootGridLeftClickCommand
        {
            get
            {
                return new RelayCommand(photoShootGridLeftClick);
            }
        }

        #region Workflow
        public RelayCommand WorkflowItemMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(workflowItemMouseDoubleClick);
            }
        }
        public RelayCommand WorkflowItemMouseLeftClickCommand
        {
            get
            {
                return new RelayCommand(workflowItemMouseLeftClick);
            }
        }
        public RelayCommand AddNotesVisibleCommand
        {
            get
            {
                return new RelayCommand(addNotesVisible);
            }
        }
        public RelayCommand AddNotesCommand
        {
            get
            {
                return new RelayCommand(addEditNotes);
            }
        }
        public RelayCommand EditNotesVisibleCommand
        {
            get
            {
                return new RelayCommand(editNotes);
            }
        }
        public RelayCommand DeleteNotesVisibleCommand
        {
            get
            {
                return new RelayCommand(deleteNotes);
            }
        }
        public RelayCommand EquipmentItemMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(equipmentItemMouseDoubleClick);
            }
        }
        public RelayCommand EquipmentItemMouseLeftClickCommand
        {
            get
            {
                return new RelayCommand(equipmentItemMouseLeftClick);
            }
        }
        #endregion

        #endregion

        #region Methods

        #region  code added Abhilasha
        async void RenameFailedImages()
        {
            try
            {
                //BackgroundWorker bw = new BackgroundWorker();
                //bw.DoWork += bw_DoWork_RenameImages;
                //bw.RunWorkerCompleted += bw_RunWorkerCompleted_RenameImages;
                //bw.RunWorkerAsync();

                await tempRenameFailedImages();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        async Task tempRenameFailedImages()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                await Task.Run(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        clsErrorLog.displaymsg = false;
                        DataTable stud = clsDashBoard.getFailedRenameStudents(db);

                        //PhotoSorterDBModelDataContext dbrename = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                        int k = 0;
                        for (k = 0; k < stud.Rows.Count; k++)
                        {
                            selectedPhotoShootId = Convert.ToInt32(stud.Rows[k]["PhotoshootID"]);
                            string path = clsDashBoard.GetPhotoShootPath(db, selectedPhotoShootId);
                            int studentid = Convert.ToInt32(stud.Rows[k]["ID"]);
                            string Gridname = "studentPhotos";
                            ArrayList arrstudentimgid = new ArrayList();

                            arrstudentimgid.Add(studentid);
                            _objProgressBarRename = new ProgressBarRename(selectedPhotoShootId, path, Gridname, arrstudentimgid);      //mohan
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        private void bw_RunWorkerCompleted_RenameImages(object sender, RunWorkerCompletedEventArgs e)
        {
        }
        #endregion

        public void loadPhotoShoot(int defaultschoolid)
        {
            try
            {
                VisibleData = new List<StudentImage>();
                studentImagePreview = null;
                dgPhotoShootData = new ObservableCollection<PhotoShoot>();
                dgStudentPhotosData = new ObservableCollection<StudentImage>();
                workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>();

                string allJobId = "";
                List<int> jobIds = clsDashBoard.getAllJobs(clsSchool.defaultSchoolId, new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
                if (jobIds.Count > 0)
                {
                    allJobId = string.Join(",", jobIds);
                    dgPhotoShootData = new ObservableCollection<PhotoShoot>(clsDashBoard.getPhotoShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allJobId));
                    if (dgPhotoShootData != null)
                    {
                        selectedPhotoShoot = dgPhotoShootData.FirstOrDefault();
                    }
                    else
                    {
                        selectedPhotoShoot = new PhotoShoot();
                    }
                }
                else
                    selectedPhotoShoot = new PhotoShoot();

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
        #region Refresh
        internal void refreshGrids()
        {
            try
            {
                if (selectedGridName == "PhotoShoot")
                {
                    refreshPhotoShoot(clsSchool.defaultSchoolId);
                }
                else if (selectedGridName == "studentPhotos")
                    Refre();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        internal void refreshPhotoShoot(int tempDefaultSchoolId)
        {
            string AllJobID = "";
            int schoolID = tempDefaultSchoolId;
            dgPhotoShootData = new ObservableCollection<PhotoShoot>();
            List<int> JobIds = clsDashBoard.getAllJobs(schoolID, new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            if (JobIds.Count > 0)
            {
                foreach (int jobid in JobIds)
                {
                    AllJobID += jobid + ",";
                }
                AllJobID = AllJobID.Substring(0, AllJobID.Length - 1);

                dgPhotoShootData = new ObservableCollection<PhotoShoot>(clsDashBoard.getPhotoShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), AllJobID));

                dgStudentPhotosData = null;
                studentImagePreview = null;
            }
        }

        public void Refre()
        {
            try
            {
                selectPhotoShoot(selectedPhotoShootId);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        internal void selectPhotoShoot(int photoShootId)
        {
            try
            {
                if (selectedPhotoShoot == null) { return; }
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                //ImageFolderPath = ((Setting)clsDashBoard.getSettingByName(dbref, "ImageFolderLocation")).settingValue;    
                if (selectedPhotoShootIndex == 0)
                {
                    //int selectedId = selectedStudentPhoto != null ?dgStudentPhotosData.IndexOf(selectedStudentPhoto) : 0;
                    //ArrayList Ids = new ArrayList();

                    //foreach (StudentImage si in selectedStudentPhotos)
                    //{
                    //    Ids.Add(dgStudentPhotosData.IndexOf(si));
                    //}
                    dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.GetStudentImgfromPhotoShoot(db, selectedPhotoShootId));
                    if (dgStudentPhotosData.Count() != 0)
                    {
                        selectedStudentPhoto = dgStudentPhotosData.FirstOrDefault();
                        //selectedStudentPhoto = selectedId > 0 ? dgStudentPhotosData[selectedId] : dgStudentPhotosData.FirstOrDefault();
                        //foreach (int id in Ids)
                        //{
                        //    selectedStudentPhotos.Add(dgStudentPhotosData[id]);
                        //}
                        selectImage();
                    }
                    else
                    {
                        studentImagePreview = null;
                        (Application.Current as App).isOpenFolderVisible = false;
                    }
                }
                if (selectedPhotoShootIndex == 1)
                {
                    dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getAllPhotoshootWorkflowItem(db, selectedPhotoShoot.PhotoShotID, "Workflow"));
                    workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                }
                else if (selectedGridName == "equipment" || selectedPhotoShootIndex == 2)
                {
                    dgEquipmentData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getAllPhotoshootWorkflowItem(db, selectedPhotoShoot.PhotoShotID, "Equipment"));
                }
                photo = photoShootId;

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Just to show the selected student image on imgStudentPhotoPreview grid
        /// </summary>
        /// <param name="gridName">Name of the grid</param>
        private void selectImage()
        {
            try
            {
                if (selectedStudentPhoto != null)
                {
                    int ImageId = selectedStudentPhoto.ID;
                    PhotoSorterDBModelDataContext dbref = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    StudentImage objStudentImage = clsDashBoard.getStudentImageDetailsById(dbref, ImageId);
                    if (objStudentImage == null) { return; }

                    //strFile = objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + objStudentImage.ImageName;
                    strFileToShow = objStudentImage.PhotoShoot.ImageFolder + "\\" + objStudentImage.ImageName;
                    strFilereduced = objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + objStudentImage.ImageName;
                    if (!File.Exists(strFilereduced))
                        strFilereduced = strFileToShow;
                    if (!File.Exists(strFilereduced))
                    {
                        studentImagePreview = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
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
                        #region Old Code
                        //System.Drawing.Image img = System.Drawing.Image.FromFile(strFilereduced);
                        //if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        //{
                        //    decoderForJpeg(strFilereduced);
                        //}
                        #endregion
                    }
                }
                else
                {
                    studentImagePreview = null;
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
            //thumb.DecodePixelWidth = 200;
            thumb.StreamSource = stream;
            thumb.EndInit();
            studentImagePreview = thumb;
            //}
        }
        /// <summary>
        /// This method is used to create new record
        /// </summary>
        internal void newRecord()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (selectedGridName == "PhotoShoot")
                {
                    //MVVMMessageService.ShowMessage("Unable to create a photoshoot with this action. Please use the Import button to create a photoshoot.");
                    AddEditPhotoShoot _objAddEditPhotoShoot = new AddEditPhotoShoot();
                    _objAddEditPhotoShoot.ShowDialog();

                    if (((AddEditPhotoShootViewModel)(_objAddEditPhotoShoot.DataContext)).isSave)
                    {
                        refreshPhotoShoot(clsSchool.defaultSchoolId);
                    }
                }
                //else if (selectedGridName == "Activities")
                //{
                //    AddEditActivity _objAddEditActivity = new AddEditActivity(new School { SchoolName = clsSchool.defaultSchoolName, ID = clsSchool.defaultSchoolId });
                //    _objAddEditActivity.ShowDialog();
                //    if (((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).isSave)
                //    {
                //        dgPhotoShootActivities = clsActivities.getPhotoShootActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedPhotoShoot.PhotoShotID);
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
        /// This method is used to delete selected record
        /// </summary>
        public void delete()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string message = "";
                if (selectedGridName == "PhotoShoot")
                {
                    int result = 0;

                    if (selectedPhotoShoot != null)
                    {
                        ArrayList arrshootID = new ArrayList();
                        arrshootID = new ArrayList((from sps in selectedPhotoShoots select sps.PhotoShotID).ToList());
                        if (arrshootID.Count == 1)
                        {
                            message = "Are you sure you want to delete a photoshoot ?";
                        }
                        else
                        {
                            message = "Are you sure you want to delete multiple photoshoots?";
                        }
                        string caption = "Confirmation";
                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                        if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                        {
                            GetRating _objGetRating = new GetRating(true, "Photoshoots");
                            _objGetRating.ShowDialog();

                            //int totalRecordsCount = dgPhotoShootData.Count();
                            //int deletedRecordsCount = arrshootID.Count;

                            if (((GetRatingViewModel)(_objGetRating.DataContext)).isContinue)
                            {
                                result = clsDashBoard.deleteBulkPhotoShoot(db, arrshootID);
                                if (result != 0)
                                {
                                    foreach (int phstId in arrshootID)
                                    {
                                        dgPhotoShootData.Remove(dgPhotoShootData.Where(i => i.PhotoShotID == phstId).First());
                                    }
                                }
                            }
                            photoShootTableViewMouseUp();
                            //refreshGrids();

                            //createDeletedRecordsLogFile("Dashboard PhotoShoots", totalRecordsCount, deletedRecordsCount);
                        }
                    }
                }
                else if (selectedGridName == "studentPhotos")
                {
                    string imagename = ""; string photoshootIds = "";
                    if (selectedStudentPhoto != null)
                    {
                        ArrayList arrStudImageId = new ArrayList();
                        foreach (StudentImage stuImage in selectedStudentPhotos)
                        {
                            try
                            {
                                int stuimgID = stuImage.ID;
                                imagename = Convert.ToString(stuImage.ImageName);
                                if (!arrStudImageId.Contains(stuimgID))
                                {
                                    arrStudImageId.Add(stuimgID);
                                    photoshootIds = stuImage.PhotoShootID + ",";
                                }
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }
                        photoshootIds = photoshootIds.Substring(0, photoshootIds.Length - 1);
                        if (arrStudImageId.Count == 0)
                        {
                            MVVMMessageService.ShowMessage(errorMessages.SELECT_STUDENT_IMAGE);
                            return;
                        }
                        if (arrStudImageId.Count == 1)
                        {
                            message = "Are you sure you want to delete student image " + imagename + "?";
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
                            //why this code???  //#Mohan
                            //for (int i = 0; i < arrstudID.Count; i++)
                            //{
                            //    IList<StudentImage> _stuimg;
                            //    _stuimg = clsDashBoard.getStudimageAndPhotoShootFromId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(arrstudID[i]));
                            //    string imgname = _stuimg.First().ImageName;
                            //    string path = _stuimg.First().PhotoShoot.ImageFolder;
                            //}
                            #region Old Code
                            //Commented By Mohan
                            //Delete images from database as well..
                            // Need to update Recordstatus as Delete into tblstudentimage ....
                            //updtStuID = clsDashBoard.updateRecordStatus(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrstudID, "Delete");
                            //foreach (int imgId in arrstudID)
                            //{
                            //    dgStudentPhotosData.Remove((from spd in dgStudentPhotosData where spd.ID == imgId select spd).First());
                            //}
                            #endregion

                            GetRating _objGetRating = new GetRating(true, "StudentImages");
                            _objGetRating.ShowDialog();

                            //int totalRecordsCount = dgStudentPhotosData.Count();
                            //int deletedRecordsCount = arrStudImageId.Count;

                            if (((GetRatingViewModel)(_objGetRating.DataContext)).isContinue)
                            {
                                int result = clsDashBoard.deleteStudentImage(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudImageId, photoshootIds);
                                foreach (int imgId in arrStudImageId)
                                {
                                    dgStudentPhotosData.Remove((from spd in dgStudentPhotosData where spd.ID == imgId select spd).First());
                                }
                                selectImage();
                            }

                            selectImage();
                            //refreshGrids();
                            //createDeletedRecordsLogFile("StudentImages", totalRecordsCount, deletedRecordsCount);
                        }
                    }
                }
                else if (selectedGridName == "Activities")
                {
                    //if (selectedPhotoShoot != null && selectedActivity != null)
                    //{
                    //    message = "Are you sure you want to delete photoshoot activity " + selectedActivity.Subject + "?";
                    //    string caption = "Confirmation";
                    //    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    //    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    //    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    //    {
                    //        int result = clsActivities.deleteMultipleActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), new ArrayList { selectedActivity.Id });
                    //        dgPhotoShootActivities = clsActivities.getPhotoShootActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedPhotoShoot.PhotoShotID);
                    //    }
                    //}
                }
                else if (selectedGridName == "workflow")
                {
                    ArrayList photoShootWorkflowItemIds = new ArrayList();
                    if (selectedWorkflowItems.Count > 0)
                    {
                        int deletedRecords = selectedWorkflowItems.Count;
                        if (selectedWorkflowItems.Count == 1)
                        {
                            message = "Are you sure you want to delete a workflow?";
                        }
                        else
                        {
                            message = "Are you sure you want to delete multiple workflows?";
                        }
                        string caption = "Confirmation";
                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                        if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                        {
                            foreach (PhotoshootWorkflowItem pwi in selectedWorkflowItems)
                            {
                                photoShootWorkflowItemIds.Add(pwi.Id);
                            }

                            int result = clsWorkflows.deletePhotoshootWorkflowItems(db, photoShootWorkflowItemIds);
                            dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getAllPhotoshootWorkflowItem(db, selectedPhotoShoot.PhotoShotID, "Workflow"));

                            #region Logfile
                            clsErrorLog objError = new clsErrorLog();

                            objError.Source = "Photoshoot WorkflowItems";
                            objError.MethodName = "Delete Photoshoot WorkflowItems";
                            objError.Message = "Photoshoot WorkflowItems log file. \n Schoolname : " + clsSchool.defaultSchoolName + "\n Photoshoot name : " + selectedPhotoShoot.PhotoShotName + " \n Action: Record(s) deleted \n Deleted records count : " + deletedRecords + "\n Photoshoot workflows count : " + dgWorkflowItemsData.Count + "\n";
                            objError.UserComments = clsStatic.userName;
                            objError.DateTime = DateTime.Now;

                            clsStatic.WriteErrorLog(objError, "Photoshoot WorkflowItems Info.");
                            //clsStatic.WriteErrorLog(objError, objstatic.ErrorLogXML);

                            #endregion
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
        /// <summary>
        /// This method is used to edit selected records(one or more than one at a time)
        /// </summary>
        internal void bulkRename()
        {
            try
            {
                if (selectedGridName == "PhotoShoot")
                {
                    if (selectedPhotoShoot != null)
                    {
                        int ShootID;
                        ArrayList arrShootID = new ArrayList();
                        foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
                        {
                            try
                            {
                                ShootID = Convert.ToInt32(tempPhotoShoot.PhotoShotID);
                                if (!arrShootID.Contains(ShootID))
                                {
                                    arrShootID.Add(ShootID);
                                }
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }
                        BulkRenamePhotoShoot _objBulkRenamePhotoShoot = new BulkRenamePhotoShoot(arrShootID);
                        _objBulkRenamePhotoShoot.ShowDialog();
                        if (((BulkRenamePhotoShootViewModel)(_objBulkRenamePhotoShoot.DataContext)).isSave)
                            refreshGrids();
                    }
                }
                else if (selectedGridName == "studentPhotos")
                {
                    if (selectedStudentPhotos.Count != 0)
                    {
                        int stuImgId;
                        ArrayList arrStuId = new ArrayList();
                        foreach (StudentImage stuImage in selectedStudentPhotos)
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
                            refreshGrids();
                    }
                }
                else if (selectedGridName == "workflow")
                {
                    if (selectedWorkflowItems.Count > 0)
                    {
                        EditPhotoshootWorkflowItems _objEditWorkflowItems = new EditPhotoshootWorkflowItems(selectedWorkflowItems);
                        _objEditWorkflowItems.ShowDialog();

                        if (((EditPhotoshootWorkflowItemsViewModel)(_objEditWorkflowItems.DataContext)).isSave)
                            dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getAllPhotoshootWorkflowItem(db, selectedPhotoShoot.PhotoShotID, "Workflow"));
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        //added by mohan
        //add one photo per teacher to respective groups 
        /// <summary>
        /// This method is used to add class Photo to group
        /// </summary>
        internal void addClassPhotosToGroup()
        {
            //get student images of a selected group .
            //select one recored per teacher.
            //loop each image through all the groups.
            //if group name matches with image name add as group photo
            //you are done.
            if (dgStudentPhotosData != null)
            {
                try
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    int i = 0;
                    List<StudentImage> tempStuImages = new List<StudentImage>();
                    ArrayList tempTeacherNames = new ArrayList();
                    foreach (StudentImage stuImage in dgStudentPhotosData)
                    {
                        if (!tempTeacherNames.Contains(stuImage.Student.Teacher))
                        {
                            tempTeacherNames.Add(stuImage.Student.Teacher);
                            tempStuImages.Add(stuImage);
                        }
                    }

                    IEnumerable<GroupExtraColumn> groupsListBySchoolId = clsGroup.getGroupsBySchool(db, clsSchool.defaultSchoolId);

                    if (groupsListBySchoolId.Count() != 0)
                    {
                        string unMatchedTeacherNames = "";
                        foreach (StudentImage stuImage in tempStuImages)
                        {
                            string tempImageName = "";
                            if (string.IsNullOrWhiteSpace(stuImage.Student.Teacher)) { stuImage.Student.Teacher = "No Teacher"; }
                            //List<GroupExtraColumn> temp = (from n in groupsListBySchoolId where n.GroupName.Split('-')[0] == stuImage.Student.Teacher orderby n.ID descending select n).ToList();
                            GroupExtraColumn groupData = (from n in groupsListBySchoolId where n.GroupName.Split('-')[0] == stuImage.Student.Teacher orderby n.ID descending select n).FirstOrDefault();

                            if (groupData == null) //groupData = null means there is mismatch between teacher name in group and group name
                            { 
                                unMatchedTeacherNames = unMatchedTeacherNames + "\n" + stuImage.Student.Teacher;
                                continue; 
                            }
                            IEnumerable<GroupClassPhoto> tempClassPhoto = clsGroup.getClassPhotoByGroup(db, groupData.ID);

                            List<StudentImage> _fourGroupClassImages = (from gci in dgStudentPhotosData where gci.Student.Teacher == stuImage.Student.Teacher select gci).Take(4).ToList();

                            foreach (StudentImage stuClsImage in _fourGroupClassImages)
                            {
                                GroupClassPhoto checkForClassPhoto = (from n in tempClassPhoto where n.StudentImage.ImageName == stuClsImage.ImageName select n).FirstOrDefault(); //checking wether group contains SAME image as class photo or not.
                                if (checkForClassPhoto != null) { tempImageName = checkForClassPhoto.StudentImage.ImageName; }
                                if (tempImageName != stuClsImage.ImageName)
                                {
                                    i++;
                                    //set group image
                                    GroupClassPhoto _objgrpclspht = new GroupClassPhoto();
                                    // Need to insert it into Group Class Photo Table..
                                    _objgrpclspht.GroupId = groupData.ID;
                                    _objgrpclspht.studentImageId = stuClsImage.ID;
                                    if (_objgrpclspht != null)
                                    {
                                        db.GroupClassPhotos.InsertOnSubmit(_objgrpclspht);
                                        db.SubmitChanges();

                                        //update hasClassPhotoValue in group table
                                        int res = clsGroup.updateGroupHasClassPhoto(db, true, groupData.ID);

                                        // Delete from GroupItem Table..
                                        int delval = clsGroup.deleteGroupItem(db, groupData.ID, stuClsImage.ID);
                                    }
                                }
                            }

                            //GroupClassPhoto checkForClassPhoto = (from n in tempClassPhoto where n.StudentImage.ImageName == stuImage.ImageName select n).FirstOrDefault(); //checking wether group contains SAME image as class photo or not.
                            //if (checkForClassPhoto != null) { tempImageName = checkForClassPhoto.StudentImage.ImageName; }
                            //if (tempImageName != stuImage.ImageName)
                            //{
                            //    i++;
                            //    //set group image
                            //    GroupClassPhoto _objgrpclspht = new GroupClassPhoto();
                            //    // Need to insert it into Group Class Photo Table..
                            //    _objgrpclspht.GroupId = groupData.ID;
                            //    _objgrpclspht.studentImageId = stuImage.ID;
                            //    if (_objgrpclspht != null)
                            //    {
                            //        db.GroupClassPhotos.InsertOnSubmit(_objgrpclspht);
                            //        db.SubmitChanges();

                            //        //update hasClassPhotoValue in group table
                            //        int res = clsGroup.updateGroupHasClassPhoto(db, true, groupData.ID);

                            //        // Delete from GroupItem Table..
                            //        int delval = clsGroup.deleteGroupItem(db, groupData.ID, stuImage.ID);
                            //    }
                            //}
                        }
                        if (unMatchedTeacherNames != "")
                        {
                            MVVMMessageService.ShowMessage("The following teacher names could not be matched to groups:" + Environment.NewLine + unMatchedTeacherNames);
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage("Class photos added to " + i + " group(s).");
                        }
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage("There are no groups for selected school. Create groups first before assigning class photos");
                    }
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                    clsStatic.WriteExceptionLogXML(ex);
                }
            }
            else
            {
                MVVMMessageService.ShowMessage("Please select a photoshoot first.");
            }

        }

        internal void exportToCSV()
        {
            //we are not using this functioanlity
            //please delete the code , after the confirmation.
            //try
            //{
            //    if (selectedStudentPhoto != null)
            //    {
            //        int studentImageId;

            //        ArrayList arrStudents = new ArrayList();
            //        int schoolID = clsSchool.defaultSchoolId;

            //        //Need to calculate jobid based on schoolid.
            //        int jobId = 0;

            //        foreach (StudentImage stuImage in selectedStudentPhotos)
            //        {
            //            try
            //            {
            //                studentImageId = Convert.ToInt32(stuImage.ID);
            //                if (!arrStudents.Contains(studentImageId))
            //                {
            //                    arrStudents.Add(studentImageId);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                clsStatic.WriteExceptionLogXML(ex);
            //            }
            //        }
            //        ExportToTextFile obj = new ExportToTextFile(schoolID, jobId, selectedPhotoShootID, arrStudents);
            //        obj.ShowDialog();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    clsStatic.WriteExceptionLogXML(ex);
            //    MVVMMessageService.ShowMessage(ex.Message);
            //}
        }

        internal void importQRDataFile()
        {
            try
            {
                string SchoolName = clsSchool.defaultSchoolName;
                if (selectedPhotoShoot != null && string.IsNullOrEmpty(selectedPhotoShoot.ImageFolder))
                {
                    UploadStudentImages _objUploadStudentImages = new UploadStudentImages(SchoolName, selectedPhotoShoot);
                    _objUploadStudentImages.ShowDialog();
                    if (((UploadStudentImagesViewModel)(_objUploadStudentImages.DataContext)).isSave)
                    {
                        PhotoShoot tempPhotoshoot = ((UploadStudentImagesViewModel)(_objUploadStudentImages.DataContext))._objPhotoShoot;
                        selectedGridName = "PhotoShoot";
                        dgPhotoShootData.Add(tempPhotoshoot);
                        selectedPhotoShoot = tempPhotoshoot;
                        photoShootTableViewMouseUp();
                        refreshGrids();
                    }
                }
                else
                    MVVMMessageService.ShowMessage("Please select empty photoshoot.");
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// This method is used to add images to existed/newly created group
        /// </summary>
        internal void addImagesToGroup()
        {
            try
            {
                if (selectedStudentPhoto == null) { MVVMMessageService.ShowMessage("Please select studentImage(s). "); return; }
                ArrayList arrstuID = new ArrayList();
                int schoolID = clsSchool.defaultSchoolId;
                for (int i = 0; i < selectedStudentPhotos.Count; i++)
                    arrstuID.Add((selectedStudentPhotos[i]).ID);

                AddStudentsToGroup objAddStudentsToGroup = new AddStudentsToGroup(schoolID, arrstuID);
                objAddStudentsToGroup.ShowDialog();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        internal void studentImportFromDashboard()
        {
            try
            {
                int schoolId = clsSchool.defaultSchoolId;
                string schoolname = clsSchool.defaultSchoolName;
                int rowhndle = 0;
                _objstuimp = new ImportStudent(schoolId, rowhndle, schoolname);
                _objstuimp.ShowDialog();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// This method is used to edit selected record (one at a time)
        /// </summary>
        internal void editRecord()
        {
            try
            {
                if (selectedGridName == "PhotoShoot")
                {
                    if (selectedPhotoShoot == null) { return; }
                    addEditPhotoShoot = new AddEditPhotoShoot(selectedPhotoShoot.PhotoShotID, selectedPhotoShoot.PhotographyJob);
                    addEditPhotoShoot.ShowDialog();
                    if (((AddEditPhotoShootViewModel)(addEditPhotoShoot.DataContext)).isSave)
                    {
                        dgPhotoShootData.Remove(selectedPhotoShoot);
                        dgPhotoShootData.Insert(dgPhotoShootData.Count <= 1 ? 0 : dgPhotoShootData.IndexOf(selectedPhotoShoot), ((AddEditPhotoShootViewModel)(addEditPhotoShoot.DataContext)).addEditPhotoShoot);
                        selectedPhotoShoot = ((AddEditPhotoShootViewModel)(addEditPhotoShoot.DataContext)).addedEditedPhotoShoot;
                        selectedPhotoShoots.Add(selectedPhotoShoot);
                        //refreshGrids();
                        //updating WorkflowItems
                        dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getAllPhotoshootWorkflowItem(db, selectedPhotoShoot.PhotoShotID, "Workflow"));
                        workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                    }
                }
                else if (selectedGridName == "studentPhotos")
                {
                    if (selectedStudentPhoto != null)
                    {
                        int studentImgId = selectedStudentPhoto.ID;
                        _objBulkRenameStudentImage = new BulkRenameStudentImage(studentImgId);
                        _objBulkRenameStudentImage.ShowDialog();
                        if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                        {
                            //refreshGrids();
                            StudentImage _objImageDetails = clsDashBoard.getStudentImageDetailsById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), studentImgId);
                            int tempPhotoIndex = dgStudentPhotosData.Count <= 1 ? 0 : dgStudentPhotosData.IndexOf(selectedStudentPhoto);
                            dgStudentPhotosData.Remove(selectedStudentPhoto);
                            selectedStudentPhotos.Remove(selectedStudentPhoto);
                            dgStudentPhotosData.Insert(tempPhotoIndex, _objImageDetails);
                            selectedStudentPhoto = _objImageDetails;
                            selectedStudentPhotos.Add(selectedStudentPhoto);
                        }
                    }
                }
                else if (selectedGridName == "workflow")
                {
                    if (selectedWorkflowItem != null)
                        workflowItemMouseDoubleClick();
                }
                else if (selectedGridName == "equipment")
                {
                    if (selectedEquipmentItem != null)
                        equipmentItemMouseDoubleClick();
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

        internal void getRating()
        {
            if (selectedPhotoShoot != null)
            {
                try
                {
                    //You are going to get rating of 100 images. Are you sure you want to continue?
                    int shootID = selectedPhotoShoot.PhotoShotID;
                    _objimgrt = new GetRating(shootID, "PhotoShoot", VisibleData.Count == 0 ? dgStudentPhotosData.Count : VisibleData.Count, VisibleData);
                    _objimgrt.ShowDialog();
                    if (((GetRatingViewModel)(_objimgrt.DataContext)).isSave)
                        selectPhotoShoot(shootID);
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
        }

        internal void updateYearbook()
        {
            try
            {
                string GetClassName = "";
                int StudentImageID = 0;
                int joborShootId = 0;
                ArrayList arrstudentimgid = new ArrayList();
                GetClassName = "PhotoShoot";
                joborShootId = selectedPhotoShoot.PhotoShotID;
                foreach (StudentImage stuImage in selectedStudentPhotos)
                {
                    try
                    {
                        StudentImageID = stuImage.ID;
                        if (!arrstudentimgid.Contains(StudentImageID))
                        {
                            arrstudentimgid.Add(StudentImageID);
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                }

                _objyrbksel = new YearbookSelection(joborShootId, arrstudentimgid, GetClassName);
                _objyrbksel.ShowDialog();
                dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.getStudentsByPhotoShootId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), joborShootId));
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// it will auto create teachers group for selected Photoshoot(s)
        /// </summary>
        internal void autoCreateTeacherGroups()
        {
            try
            {
                int photojobid = 0; int shootid = 0; int schoolid = 0;
                string photojobname = "";
                ArrayList arrshootid = new ArrayList();
                ArrayList arrjobid = new ArrayList();
                string message = errorMessages.BEFORE_AUTO_GENERATE_GROUP1 + errorMessages.BEFORE_AUTO_GENERATE_GROUP2;
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {

                    schoolid = clsSchool.defaultSchoolId;
                    foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
                    {
                        try
                        {
                            photojobid = tempPhotoShoot.PhotographyJob.ID;
                            photojobname = tempPhotoShoot.PhotographyJob.JobName;
                            shootid = tempPhotoShoot.PhotoShotID;
                            if (!arrshootid.Contains(shootid))
                            {
                                arrshootid.Add(shootid);
                            }
                            if (!arrjobid.Contains(photojobid))
                            {
                                arrjobid.Add(photojobid);
                            }
                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                            MVVMMessageService.ShowMessage(ex.Message);
                        }
                    }
                    if (arrjobid.Count == 1)
                    {

                        if (selectedStudentPhotos.Count != dgStudentPhotosData.Count() && selectedStudentPhotos.Count > 0)
                            VisibleData = new List<StudentImage>(selectedStudentPhotos);
                        else if (selectedStudentPhotos.Count == 0)
                            VisibleData = new List<StudentImage>();
                        else
                            VisibleData = new List<StudentImage>(selectedStudentPhotos);

                        _objautocreategroup = new AutoCreateGroups(arrshootid, photojobid, photojobname, schoolid, VisibleData);
                        _objautocreategroup.ShowDialog();
                    }
                    else
                        MVVMMessageService.ShowMessage(errorMessages.SELECT_RECORDS_SAME_SEASON);
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to rename source images int format lastname_firstname_serialno.jpg
        /// </summary>
        internal void renameSourceImages()
        {
            try
            {
                if (selectedPhotoShoot == null) { MVVMMessageService.ShowMessage("Please select a photoShoot. "); return; }
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string message = "";
                // Need to Delete Data First from ErrorLog table...
                int delval = clsDashBoard.deleteErrorLog(db);

                ArrayList arrstudentimgid = new ArrayList();
                if (selectedGridName == "PhotoShoot")
                {
                    message = errorMessages.BEFORE_RENAME_SOURCE_IMAGES1 + PhotoShootName + errorMessages.BEFORE_RENAME_SOURCE_IMAGES2;
                }
                else
                {
                    message = errorMessages.BEFORE_RENAME_SOURCE_IMAGES1_SELECTED + errorMessages.BEFORE_RENAME_SOURCE_IMAGES2_SELECTED;
                    foreach (StudentImage stuImage in selectedStudentPhotos)
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
                }

                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    _objProgressBarRename = new ProgressBarRename(selectedPhotoShootId, Photoshootpath, selectedGridName, arrstudentimgid);
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
                        //update rename source image flag
                        clsDashBoard.updateRenameSourceImagesPhotShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedPhotoShootId);
                        MVVMMessageService.ShowMessage("Rename source images completed.");
                    }
                    Refre();
                }

            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to open source folder
        /// </summary>
        internal void openFolder()
        {
            if (Directory.Exists(Photoshootpath))
            {
                string args = string.Format("/Select, \"{0}\"", strFileToShow);
                ProcessStartInfo pfi = new ProcessStartInfo("Explorer.exe", args);

                System.Diagnostics.Process.Start(pfi);
            }
            else
            {
                MVVMMessageService.ShowMessage(errorMessages.PATH_DOES_NOT_EXISTS);
            }
        }

        internal void SyncDeleteImages()
        {
            _objsyncdelimg = new SyncDeleteMessage();
            _objsyncdelimg.ShowDialog();
        }

        public void ReSyncImageNames()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                IEnumerable<StudentImage> _objStudentPhotosData = clsDashBoard.getStudentsByPhotoShootId(db, photo);

                foreach (StudentImage stuImage in _objStudentPhotosData)
                {
                    string imgName = stuImage.ImageName;

                    string sequentialNumber = imgName.Substring(imgName.LastIndexOf('_') + 1);
                    string path = stuImage.PhotoShoot.ImageFolder;

                    string[] files = System.IO.Directory.GetFiles(path, "*_" + sequentialNumber);

                    string filename = "";
                    if (files.Count() > 0)
                    {
                        filename = System.IO.Path.GetFileName(files[0]);

                    }

                    string[] names = filename.Split('_');
                    string _firstname = "";
                    string _lastname = "";
                    string _seqno = "";
                    if (names.Count() == 3)
                    {
                        _lastname = names[0].ToString(); //lastname
                        _firstname = names[1].ToString(); //firstname
                        _seqno = names[2].ToString(); //sequence number

                        int imgid = 0;
                        imgid = stuImage.ID;
                        StudentImage _objStuimg = new StudentImage();
                        _objStuimg = clsDashBoard.updateStudentImgName(db, imgid);
                        _objStuimg.FirstName = _firstname;
                        _objStuimg.Lastname = _lastname;
                        if (_objStuimg != null)
                            db.SubmitChanges();
                    }
                }
                selectedGridName = "studentPhotos";
                Refre();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        //don't delete untill confirmation.
        //we are not using this method , as we can directly edit school year by simply double click the photo shoot or by editing the photo shoot .
        //public void ReAssignPhotoshootToSchoolYear()
        //{
        //    if (selectedPhotoShoot != null)
        //    {
        //        string message = errorMessages.RASSIGN_PHOTOSHOOT_SEASON;
        //        if (selectedStudentImages.Count > 1)
        //        {
        //            MVVMMessageService.ShowMessage(message);
        //        }
        //        else
        //        {
        //            int photoShootId = selectedPhotoShoot.PhotoShotID;
        //            _objSelectSchoolYear = new SelectSchoolYear(photoShootId);
        //            _objSelectSchoolYear.ShowDialog();
        //            if (((SelectSchoolYearViewModel)(_objSelectSchoolYear.DataContext)).isSave)
        //                refreshGrids();
        //        }
        //    }
        //}
        /// <summary>
        /// This method is used to generate reduced images
        /// </summary>
        internal async void GenerateReducedImages()
        {
            try
            {
                if (selectedPhotoShoot == null) { MVVMMessageService.ShowMessage("Please select a photoShoot. "); return; }
                string message = errorMessages.GENERATE_REDUCED_IMAGES;
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    Photoshootpath = selectedPhotoShoot.ImageFolder;
                    int photoshootid = selectedPhotoShootId;
                    Boolean type = true;
                    clsDashBoard.updatePhotoshoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), type, photoshootid);
                    #region old Code
                    //BackgroundWorker bw = new BackgroundWorker();
                    //bw.DoWork += bw_DoWork_ReduceImages;
                    //bw.RunWorkerCompleted += bw_RunWorkerCompleted_ReduceImages;
                    //bw.RunWorkerAsync(Photoshootpath);
                    #endregion

                    //cts = new CancellationTokenSource();


                    // The await operator suspends getRatingOfStudentImages.
                    //  - AccessTheWebAsync can't continue until getRatingOfImagesAsync is complete.
                    //  - Meanwhile, control returns to the caller of getRatingOfStudentImages.
                    //  - Control resumes here when getRatingOfImagesAsync is complete. 
                    //  - The await operator then retrieves the result from getRatingOfImagesAsync if method has any return type.

                    if (Directory.Exists(Photoshootpath))
                        await ReduceImages(Photoshootpath);

                    reducedImagesCreationCompleted();
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        private void reducedImagesCreationCompleted()
        {
            MVVMMessageService.ShowMessage(errorMessages.REDUCE_IMAGES_SUCESSFULL);
        }

        //private void bw_DoWork_ReduceImages(object sender, DoWorkEventArgs e)
        //{
        //    string path = (string)e.Argument;
        //    if (Directory.Exists(path))
        //        ReduceImages(path);
        //    //graphicMillerResize(path);
        //}

        async Task ReduceImages(string photoshootFolder)
        {
            if (!Directory.Exists(photoshootFolder + "\\_reduced"))
            {
                Directory.CreateDirectory(photoshootFolder + "\\_reduced");
            }
            await Task.Run(() =>
            {
                foreach (var file in Directory.GetFiles(photoshootFolder))
                {
                    try
                    {
                        if (File.Exists(photoshootFolder + "\\" + System.IO.Path.GetFileName(file)))
                        {
                            int newHeight = 0;
                            int newWidth = 0;
                            string nm = System.IO.Path.GetFileName(file);
                            string nam = file.Substring(file.LastIndexOf('.'));
                            if ((nm.Contains("jpg")) || nm.Contains("JPG"))
                            {
                                GetImageatCenter(200, file, out newHeight, out newWidth);
                                System.Drawing.Size size = new System.Drawing.Size(newWidth * 3, newHeight * 3);
                                System.Drawing.Image img = System.Drawing.Image.FromFile(file.ToString());

                                int sourceWidth = img.Width;
                                int sourceHeight = img.Height;

                                float nPercent = 0;
                                float nPercentW = 0;
                                float nPercentH = 0;

                                nPercentW = ((float)size.Width / (float)sourceWidth);
                                nPercentH = ((float)size.Height / (float)sourceHeight);

                                if (nPercentH < nPercentW)
                                    nPercent = nPercentH;
                                else
                                    nPercent = nPercentW;

                                int destWidth = (int)(sourceWidth * nPercent);
                                int destHeight = (int)(sourceHeight * nPercent);

                                Bitmap b = new Bitmap(destWidth, destHeight);
                                Graphics g = Graphics.FromImage((System.Drawing.Image)b);
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                                g.DrawImage(img, 0, 0, destWidth, destHeight);

                                var newSize = new System.Drawing.Size(destWidth, destHeight);

                                using (var bmpInput = System.Drawing.Image.FromFile(file))
                                {
                                    using (var bmpOutput = new Bitmap(bmpInput, newSize))
                                    {
                                        foreach (var id in bmpInput.PropertyIdList)
                                            bmpOutput.SetPropertyItem(bmpInput.GetPropertyItem(id));
                                        bmpOutput.SetResolution(300.0f, 300.0f);
                                        bmpOutput.Save(photoshootFolder + "\\_reduced\\" + System.IO.Path.GetFileName(file), System.Drawing.Imaging.ImageFormat.Jpeg);
                                        Graphics grPhoto = Graphics.FromImage(bmpOutput);
                                        grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                                        grPhoto.Save();
                                    }
                                }
                                g.Dispose();
                                GC.Collect();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                        return;
                    }
                }

            });
        }

        //mohan

        //private void graphicMillerResize(string photoshootFolder)
        //{
        //    float dpi = 120f;
        //    5 inch * 120 dots per inch
        //    int size = (int)(5f * dpi);

        //    if (!Directory.Exists(photoshootFolder + "\\_reduced"))
        //    {
        //        Directory.CreateDirectory(photoshootFolder + "\\_reduced");
        //    }
        //    foreach (var file in Directory.GetFiles(photoshootFolder))
        //    {
        //        string inputPath = photoshootFolder + "\\" + System.IO.Path.GetFileName(file);
        //        string outputPath = photoshootFolder + "\\_reduced\\" + System.IO.Path.GetFileName(file);
        //        try
        //        {
        //            if (File.Exists(inputPath))
        //            {
        //                using (var losslessJpeg = new Aurigma.GraphicsMill.Codecs.LosslessJpeg(inputPath))
        //                {
        //                     IPTC
        //                    if (losslessJpeg.Iptc == null)
        //                    {
        //                        losslessJpeg.Iptc = new Aurigma.GraphicsMill.Codecs.IptcDictionary();
        //                    }

        //                     EXIF
        //                    if (losslessJpeg.Exif == null)
        //                    {
        //                        losslessJpeg.Exif = new Aurigma.GraphicsMill.Codecs.ExifDictionary();
        //                    }
        //                     XMP
        //                    var xmp = new Aurigma.GraphicsMill.Codecs.XmpData();
        //                    if (losslessJpeg.Xmp != null)
        //                    {
        //                        xmp.Load(losslessJpeg.Xmp);
        //                    }
        //                    losslessJpeg.Xmp = xmp.Save();

        //                    using (var patchBitmap = new Aurigma.GraphicsMill.Bitmap(inputPath))
        //                    {
        //                        if (patchBitmap.Width < patchBitmap.Height)
        //                        {
        //                            Set value of height param to 0, so it is calculated based on width paranm
        //                            patchBitmap.Transforms.Resize(size, 0, Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.High);
        //                        }
        //                        else
        //                        {
        //                            Set value of width param to 0, so it is calculated based on height paranm
        //                            patchBitmap.Transforms.Resize(0, size, Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.High);
        //                        }

        //                        Set dpi
        //                        patchBitmap.DpiX = dpi;
        //                        patchBitmap.DpiY = dpi;

        //                        losslessJpeg.WritePatched(outputPath, new System.Drawing.Point(), patchBitmap);
        //                    }
        //                    losslessJpeg.Write(outputPath);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            clsStatic.WriteExceptionLogXML(ex);
        //            MVVMMessageService.ShowMessage(ex.Message);
        //        }
        //    }


        //}

        private void GetImageatCenter(int intImageSize, string ImgPath, out int newHeight, out int newWidth)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(ImgPath);

            newHeight = img.Height;
            newWidth = img.Width;
            if (img.Height > intImageSize && img.Width > intImageSize)
            {
                if (img.Height > img.Width)
                {
                    newHeight = intImageSize;
                    newWidth = (int)((intImageSize * img.Width) / img.Height);
                }
                else if (img.Width > img.Height)
                {
                    newWidth = intImageSize;
                    newHeight = (int)((intImageSize * img.Height) / img.Width);
                }

                else
                {
                    newWidth = newHeight = intImageSize;
                }

            }
            else if (img.Height > intImageSize || img.Width > intImageSize)
            {
                if (img.Height > intImageSize)
                {
                    newHeight = intImageSize;
                    newWidth = (int)((intImageSize * img.Width) / img.Height);

                }
                else if (img.Width > intImageSize)
                {
                    newWidth = intImageSize;
                    newHeight = (int)((intImageSize * img.Height) / img.Width);

                }
            }
        }
        /// <summary>
        /// This method is used to assign student to the selected images
        /// </summary>
        internal void assignStudent()
        {
            int studentImageId = 0;
            ArrayList arrStudentImgId = new ArrayList();
            foreach (StudentImage stuImage in selectedStudentPhotos)
            {
                try
                {
                    studentImageId = stuImage.ID;
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
                    refreshGrids();
            }
            else
            {
                MVVMMessageService.ShowMessage(errorMessages.SELECT_STUDENT_IMAGE);
            }
        }

        internal void printDeviceData(bool isQRCode)
        {
            string schoolName = clsSchool.defaultSchoolName;
            _objDeviceScanning = new DeviceScanning(schoolName);
            _objDeviceScanning.ShowDialog();
        }

        //Commented after 4.50 as we are using single UI for all photshoot validations //Mohan
        //internal void studentWithoutImage()
        //{
        //    if (selectedPhotoShoot != null)
        //    {
        //        int jobId = 0;
        //        string jobName = "";
        //        ArrayList arrShootId = new ArrayList();
        //        ArrayList arrshootName = new ArrayList();
        //        ArrayList arrJobId = new ArrayList();
        //        foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
        //        {
        //            try
        //            {
        //                jobId = tempPhotoShoot.PhotographyJob.ID;
        //                jobName = Convert.ToString(tempPhotoShoot.PhotographyJob.JobName);
        //                if (!arrJobId.Contains(jobId))
        //                    arrJobId.Add(jobId);
        //            }
        //            catch (Exception ex)
        //            {
        //                clsStatic.WriteExceptionLogXML(ex);
        //                MVVMMessageService.ShowMessage(ex.Message);
        //            }
        //        }
        //        if (arrJobId.Count == 1)
        //        {
        //            IEnumerable<PhotoShoot> _objshoots = clsDashBoard.getPhotoShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), jobId);
        //            foreach (var item in _objshoots)
        //            {
        //                if (!arrShootId.Contains(item.PhotoShotID))
        //                    arrShootId.Add(item.PhotoShotID);
        //                arrshootName.Add(item.PhotoShotName);
        //            }
        //            GetStudentsWithoutImage _objstudentwithoutstudent = new GetStudentsWithoutImage(arrShootId, clsSchool.defaultSchoolName, jobName, clsSchool.defaultSchoolId, arrshootName);
        //            _objstudentwithoutstudent.ShowDialog();
        //        }
        //        else
        //            MVVMMessageService.ShowMessage(errorMessages.SELECT_PHOTOSHOOTS_FROM_ONE_SEASON);
        //    }
        //}

        //mohan
        internal void generateJIF()
        {
            try
            {
                if (selectedPhotoShoot != null)
                {
                    string message = errorMessages.CREATE_ITPC_DATA;
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        try
                        {
                            PhotoSorterDBModelDataContext dbup = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                            int shootID = selectedPhotoShoot.PhotoShotID;
                            selectedPhotoShootId = shootID;
                            PhotoShootName = selectedPhotoShoot.PhotoShotName;
                            Photoshootpath = selectedPhotoShoot.ImageFolder;
                            if (Directory.Exists(Photoshootpath))
                            {
                                List<StudentImage> photoShootImages = clsDashBoard.GetStudentImgfromPhotoShoot(dbup, shootID).ToList();
                                if (photoShootImages.Count == 0)
                                {
                                    MVVMMessageService.ShowMessage(errorMessages.PATH_NO_IMAGES);
                                    return;
                                }

                                string[] insertIPTC = new string[2];
                                int photoShootImagesCount = photoShootImages.Count;

                                foreach (StudentImage studentImage in photoShootImages)
                                {
                                    if (string.IsNullOrEmpty(studentImage.Password))
                                    {
                                        //Generate password
                                        GeneratePassword _GeneratePassword = new GeneratePassword(new ArrayList(studentImage.ID));
                                        studentImage.Password = ((GeneratePasswordViewModel)(_GeneratePassword.DataContext)).generateStudentPassword();
                                    }

                                    string original_img_path = Photoshootpath + "\\" + studentImage.ImageName;
                                    string reduced_img_path = Photoshootpath + "\\" + "_reduced" + "\\" + studentImage.ImageName;

                                    if (File.Exists(original_img_path))
                                    {
                                        if (File.Exists(reduced_img_path))
                                        {
                                            insertIPTC = new string[] { original_img_path, reduced_img_path };
                                        }
                                        else
                                        {
                                            insertIPTC = new string[] { original_img_path };
                                        }


                                        foreach (string img in insertIPTC)
                                        {

                                            #region New Code
                                            string tempPath = img.ToLower().Replace(".jpg", "_PF1.jpg");

                                            using (var losslessJpeg = new Aurigma.GraphicsMill.Codecs.LosslessJpeg(img))
                                            {
                                                // IPTC
                                                if (losslessJpeg.Iptc == null)
                                                {
                                                    losslessJpeg.Iptc = new Aurigma.GraphicsMill.Codecs.IptcDictionary();
                                                }

                                                string keyWords = studentImage.StudentID + "," + studentImage.Password;
                                                losslessJpeg.Iptc[Aurigma.GraphicsMill.Codecs.IptcDictionary.Keyword] = keyWords;

                                                // EXIF
                                                if (losslessJpeg.Exif == null)
                                                {
                                                    losslessJpeg.Exif = new Aurigma.GraphicsMill.Codecs.ExifDictionary();
                                                }
                                                // XMP
                                                var xmp = new Aurigma.GraphicsMill.Codecs.XmpData();
                                                if (losslessJpeg.Xmp != null)
                                                {
                                                    xmp.Load(losslessJpeg.Xmp);
                                                }
                                                losslessJpeg.Xmp = xmp.Save();

                                                losslessJpeg.Write(tempPath);
                                            }

                                            System.IO.File.Delete(img);
                                            System.IO.File.Move(tempPath, img);
                                            #endregion

                                            #region Old code
                                            //if (string.IsNullOrEmpty(i.Password))
                                            //{
                                            //    ArrayList arrstudID = new ArrayList();
                                            //    if (!arrstudID.Contains(i.ID))
                                            //    {
                                            //        arrstudID.Add(i.ID);
                                            //    }
                                            //    //Generate password
                                            //    GeneratePassword _GeneratePassword = new GeneratePassword(arrstudID);
                                            //    i.Password = ((GeneratePasswordViewModel)(_GeneratePassword.DataContext)).generateStudentPassword();
                                            //}

                                            //#region IPTC Code
                                            //string original_img_path = Photoshootpath + "\\" + i.ImageName;
                                            //string reduced_img_path = Photoshootpath + "\\" + "_reduced" + "\\" + i.ImageName;


                                            //if (File.Exists(original_img_path))
                                            //{
                                            //    if (File.Exists(reduced_img_path))
                                            //    {
                                            //        insertIPTC = new string[] { original_img_path, reduced_img_path };
                                            //    }
                                            //    else
                                            //        insertIPTC = new string[] { original_img_path };
                                            //    foreach (string img in insertIPTC)
                                            //    {
                                            //        #region New IPTC Code
                                            //        try
                                            //        {
                                            //            var bitmap = new Aurigma.GraphicsMill.Bitmap(img);
                                            //            var settings = new Aurigma.GraphicsMill.Codecs.JpegSettings();

                                            //            var iptc = new Aurigma.GraphicsMill.Codecs.IptcDictionary();
                                            //            string keyWords = i.StudentID + "," + i.Password;
                                            //            iptc[Aurigma.GraphicsMill.Codecs.IptcDictionary.Keyword] = keyWords;
                                            //            settings.Iptc = iptc;

                                            //            bitmap.Save(img, settings);
                                            //        }
                                            //        catch (Exception ex)
                                            //        {
                                            //            clsStatic.WriteExceptionLogXML(ex);
                                            //            MVVMMessageService.ShowMessage(ex.Message);
                                            //        }

                                            //        #endregion
                                            //    }
                                            //}
                                            //#endregion
                                            #endregion
                                        }
                                    }
                                }

                                var info = Aurigma.GraphicsMill.Licensing.LicenseInfoProvider.LicenseInfo;
                                DateTime dt1 = info.MaintenanceExpirationDate;
                                DateTime dt2 = DateTime.Today;
                                if ((dt1 - dt2).TotalDays == 7)
                                {
                                    MVVMMessageService.ShowMessage("Maintenance period expires on " + info.MaintenanceExpirationDate.ToString());
                                }
                                MVVMMessageService.ShowMessage(errorMessages.GENERATED_IPTC_ALL_IMAGES);
                            }
                            else
                            {
                                MVVMMessageService.ShowMessage(errorMessages.PATH_DOES_NOT_EXISTS);
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
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }


        }

        # region For Count Images and Validation

        internal void CountImages()
        {
            ImageCountForAll();
        }

        //Commented after 4.50 as we are using single UI for all photshoot validations //Mohan
        //internal void CountAdminCD()
        //{
        //    try
        //    {
        //        if (selectedPhotoShoot != null)
        //        {
        //            int JobID = 0;
        //            int photoShootID = 0;
        //            ArrayList arrjobid = new ArrayList();
        //            ArrayList arrPhotoShootID = new ArrayList();
        //            foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
        //            {
        //                try
        //                {
        //                    photoShootID = Convert.ToInt32(tempPhotoShoot.PhotoShotID);
        //                    JobID = Convert.ToInt32((tempPhotoShoot).PhotographyJob.ID);
        //                    if (!arrjobid.Contains(JobID))
        //                        arrjobid.Add(JobID);
        //                    if (!arrPhotoShootID.Contains(photoShootID))
        //                        arrPhotoShootID.Add(photoShootID);

        //                }
        //                catch (Exception ex)
        //                { clsStatic.WriteExceptionLogXML(ex); }
        //            }
        //            if (arrjobid.Count == 1)
        //            {
        //                adminyearcd = new AdminYearBookCD(arrPhotoShootID, arrjobid, "Admincd");
        //                adminyearcd.ShowDialog();
        //            }
        //            else
        //                MVVMMessageService.ShowMessage(errorMessages.SELECT_PHOTOSHOOTS_FROM_ONE_SEASON);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        clsStatic.WriteExceptionLogXML(ex);
        //        MVVMMessageService.ShowMessage(ex.Message);
        //    }
        //}

        //Commented after 4.50 as we are using single UI for all photshoot validations //Mohan
        //internal void CountYearbookD()
        //{
        //    try
        //    {
        //        if (selectedPhotoShoot != null)
        //        {
        //            int JobID = 0;
        //            int photoShootID = 0;
        //            ArrayList arrjobid = new ArrayList();
        //            ArrayList arrPhotoShootID = new ArrayList();
        //            foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
        //            {
        //                try
        //                {
        //                    JobID = Convert.ToInt32(tempPhotoShoot.PhotographyJob.ID);
        //                    photoShootID = Convert.ToInt32(tempPhotoShoot.PhotoShotID);
        //                    if (!arrjobid.Contains(JobID))
        //                        arrjobid.Add(JobID);
        //                    if (!arrPhotoShootID.Contains(photoShootID))
        //                        arrPhotoShootID.Add(photoShootID);

        //                }
        //                catch (Exception ex)
        //                { clsStatic.WriteExceptionLogXML(ex); }
        //            }
        //            if (arrjobid.Count == 1)
        //            {
        //                adminyearcd = new AdminYearBookCD(arrPhotoShootID, arrjobid, "Yearbookcd");
        //                adminyearcd.ShowDialog();
        //            }
        //            else
        //                MVVMMessageService.ShowMessage(errorMessages.SELECT_PHOTOSHOOTS_FROM_ONE_SEASON);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        clsStatic.WriteExceptionLogXML(ex);
        //        MVVMMessageService.ShowMessage(ex.Message);
        //    }
        //}


        private void ImageCountForAll()
        {
            try
            {
                if (selectedPhotoShoot != null)
                {
                    int PhotoShootID = 0;
                    ArrayList arrjobid = new ArrayList();
                    foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
                    {
                        try
                        {
                            PhotoShootID = Convert.ToInt32(tempPhotoShoot.PhotoShotID);
                            if (!arrjobid.Contains(PhotoShootID))
                                arrjobid.Add(PhotoShootID);

                        }
                        catch (Exception ex)
                        { clsStatic.WriteExceptionLogXML(ex); }
                    }

                    countimg = new CountImages(arrjobid, "Count Images", "");
                    countimg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        //internal void validatePhotoshoots()
        //{
        //ArrayList arrjobID = new ArrayList();
        //ArrayList arrPhotoShootID = new ArrayList();
        //int JobID = 0;
        //int photoShootID = 0;
        //foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
        //{
        //    try
        //    {

        //        JobID = Convert.ToInt32(tempPhotoShoot.PhotographyJob.ID);
        //        photoShootID = Convert.ToInt32(tempPhotoShoot.PhotoShotID);
        //        if (!arrjobID.Contains(JobID))
        //            arrjobID.Add(JobID);
        //        if (!arrPhotoShootID.Contains(photoShootID))
        //            arrPhotoShootID.Add(photoShootID);
        //    }
        //    catch (Exception ex)
        //    {
        //        clsStatic.WriteExceptionLogXML(ex);
        //    }
        //}
        //if (arrjobID.Count == 1)
        //{
        //valAdminCd = new ValidateAdminCD();
        //valAdminCd.Show();
        //}
        //else
        //    MVVMMessageService.ShowMessage(errorMessages.SELECT_PHOTOSHOOTS_FROM_ONE_SEASON);
        //}

        # endregion

        public string checkForRenameSourceImages()
        {
            string photoShootNames = "";
            string allJobId = "";
            IEnumerable<PhotoShoot> photoShootNamesList = null;
            List<int> JobIds = clsDashBoard.getAllJobs(clsSchool.defaultSchoolId, new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            if (JobIds.Count > 0)
            {
                foreach (int jobid in JobIds)
                {
                    allJobId += jobid + ",";
                }

                allJobId = allJobId.Substring(0, allJobId.Length - 1);
                if (clsDashBoard.getPhotoShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allJobId).Count() != 0)
                    photoShootNamesList = clsDashBoard.getPhotoShootNames(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allJobId);
                if (photoShootNamesList != null)
                {
                    if (photoShootNamesList.Count() == 1)
                    {
                        photoShootNames += photoShootNamesList.First().PhotoShotName;
                    }
                    else
                    {
                        foreach (var i in photoShootNamesList)
                        {
                            photoShootNames += i.PhotoShotName + "\n";
                        }
                    }
                }
            }
            return photoShootNames;
        }

        private void studentPhotoPreviewMouseUp()
        {
            try
            {
                string filePath = strFileToShow.ToString();
                if (File.Exists(filePath))
                {
                    Process.Start(filePath);

                    //Process photoViewer = new Process();
                    //photoViewer.StartInfo.FileName = Environment.Is64BitOperatingSystem == true ? Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\PhotoELF\\PhotoELF.exe" : Environment.GetEnvironmentVariable("ProgramFiles") + "\\PhotoELF\\PhotoELF.exe";
                    //photoViewer.StartInfo.Arguments = filePath;
                    //photoViewer.Start();
                }
                //else
                //{
                //    filePath = strFilereduced.ToString();
                //    filePath = "file:///" + filePath;
                //    try
                //    {
                //        Process.Start(filePath);
                //    }
                //    catch (Exception ex)
                //    {
                //        MVVMMessageService.ShowMessage(ex.Message);
                //    }
                //}
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void studentPhotosTableKeyUp()
        {
            studentPhotosGridButtonsVisibility();
            selectedGridName = "studentPhotos";
            selectImage();
        }
        //private void activitiesGridMouseUp()
        //{
        //    activitiesGridButtonsVisibility();
        //    selectedGridName = "Activities";
        //}
        private void studentPhotosDoubleClick()
        {
            try
            {
                //MainWindow objMain = (MainWindow)MainWindow.GetWindow(this);      //mohan
                //objMain.bNew.IsEnabled = false;
                selectedGridName = "studentPhotos";
                editRecord();
                //if (selectedStudentPhoto != null)
                //{
                //    int studentImgID = selectedStudentPhoto.ID;
                //    _objBulkRenameStudentImage = new BulkRenameStudentImage(studentImgID);
                //    _objBulkRenameStudentImage.ShowDialog();
                //    if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                //        refreshGrids();
                //}

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void studentPhotosPanelFocus()
        {
            studentPhotosGridButtonsVisibility();
            selectedGridName = "studentPhotos";
            //mohan
            //MainWindow objMain = (MainWindow)MainWindow.GetWindow(this);
            //objMain.bNew.IsEnabled = false;
        }
        private void photoShootGridDoubleClick()
        {
            selectedGridName = "PhotoShoot";
            editRecord();
        }
        private void photoShootTableViewMouseUp()
        {
            try
            {
                selectedGridName = "PhotoShoot"; strFileToShow = "";

                if (selectedPhotoShoot != null)
                {
                    PhotoSorterDBModelDataContext dbup = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    selectedPhotoShootId = selectedPhotoShoot.PhotoShotID;
                    PhotoShootName = selectedPhotoShoot.PhotoShotName;
                    Photoshootpath = selectedPhotoShoot.ImageFolder;
                    //dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.GetStudentImgfromPhotoShoot(dbup, selectedPhotoShootId));
                    selectPhotoShoot(selectedPhotoShoot.PhotoShotID);
                    //dgPhotoShootActivities = clsActivities.getPhotoShootActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedPhotoShootId);

                    if (dgStudentPhotosData != null && dgStudentPhotosData.Count() != 0)
                    {
                        selectedStudentPhoto = dgStudentPhotosData.FirstOrDefault();
                        selectImage();
                    }
                    else
                    {
                        studentImagePreview = null;
                        (Application.Current as App).isOpenFolderVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        //private void activitiesGridDoubleClick()
        //{
        //    if (selectedPhotoShoot != null && selectedActivity != null)
        //    {
        //        AddEditActivity _objAddEditActivity = new AddEditActivity(selectedActivity);
        //        _objAddEditActivity.ShowDialog();
        //        if (((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).isSave)
        //        {
        //            dgPhotoShootActivities = clsActivities.getPhotoShootActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedPhotoShoot.PhotoShotID);
        //        }
        //    }
        //}

        #region Buttons Visibility Based on the Grid Selection
        private void photoShootsGridButtonsVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            if (selectedPhotoShootIndex == 2)
            {
                (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
                (Application.Current as App).isAddCollectionButtonVisible = false; (Application.Current as App).isAddEquipmentItemsButtonVisible = true;
            }
            else
            {
                (Application.Current as App).isNewVisible = true;
                (Application.Current as App).isEditVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
                (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isRenameSourceImagesVisible = true;
                (Application.Current as App).isAutoAssignClassVisible = true; (Application.Current as App).isIPTCVisible = true;
                (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
                (Application.Current as App).isRefreshVisible = true; (Application.Current as App).isAddToGroupVisible = true;
                (Application.Current as App).isOpenFolderVisible = true; (Application.Current as App).isAddCollectionButtonVisible = true; (Application.Current as App).isAddEquipmentItemsButtonVisible = false;
            }
            //(Application.Current as App).isStuWithOutImageVisible = true;
        }
        private void studentPhotosGridButtonsVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isEditVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isAddToGroupVisible = true;
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isRefreshVisible = true; (Application.Current as App).isAssignStudentVisible = true;
            (Application.Current as App).isOpenFolderVisible = true; (Application.Current as App).isRenameSourceImagesVisible = true;
        }
        private void activitiesGridButtonsVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true;
            (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDeleteVisible = true;
            (Application.Current as App).isRefreshVisible = true;
        }
        #endregion
        public void updateSortOrder(PhotoshootWorkflowItem oldCollectionItem)
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (oldCollectionItem.SortOrder == null || oldCollectionItem.SortOrder.ToString() == "")
                return;
            int result = clsWorkflows.updatePhotoShootWorkflowItems(db, oldCollectionItem);

            //int result = clsOrders.UpadteOrderPackagesData(db, item, package, billingCode, packageId);
            //MVVMMessageService.ShowMessage("Package updated successfully.");
        }


        internal void createDuplicatePhotoShoots()
        {
            try
            {
                if (selectedPhotoShoots.Count > 0)
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                    foreach (PhotoShoot selectedPS in selectedPhotoShoots)
                    {
                        #region PhotographyJob
                        PhotoForce.App_Code.PhotographyJob tempJob = (from pj in db.PhotographyJobs where pj.ID == selectedPS.PhotographyjobID select pj).FirstOrDefault();

                        int startYear = (Convert.ToInt32(tempJob.StartYear) + 1);
                        int endYear = (Convert.ToInt32(tempJob.EndYear) + 1);
                        string jobName = (startYear.ToString() + "-" + endYear.ToString());

                        if (tempJob.JobName.ToLower().Contains("spring"))
                            jobName = jobName + " Spring";
                        else if (tempJob.JobName.ToLower().Contains("fall"))
                            jobName = jobName + " Fall";

                        int? isJobExists = clsPhotoShoot.isSchoolYearExists(db, jobName.Trim(), (int)tempJob.SchoolID); // //check for school year(i.e, job name),if already exists return photography job id.
                        if (isJobExists <= 0)
                        {
                            PhotoForce.App_Code.PhotographyJob _objPhotographyJob = new App_Code.PhotographyJob();
                            _objPhotographyJob.JobName = jobName;
                            _objPhotographyJob.JobDate = DateTime.Now;
                            _objPhotographyJob.StartYear = startYear.ToString();
                            _objPhotographyJob.EndYear = endYear.ToString();
                            _objPhotographyJob.SchoolID = tempJob.SchoolID; //defaultSchoolId;
                            if (_objPhotographyJob != null)
                            {
                                db.PhotographyJobs.InsertOnSubmit(_objPhotographyJob);
                                db.SubmitChanges();
                                isJobExists = _objPhotographyJob.ID;
                            }
                        }
                        #endregion

                        PhotoShoot newPhotoShoot = new PhotoShoot();
                        //addEditPhotoShoot.PhotographyjobID = photographyJobId;
                        newPhotoShoot.Job_ = selectedPS.Job_;
                        newPhotoShoot.PhotoShotName = selectedPS.PhotoShotName.Trim();
                        newPhotoShoot.PhotoShotDate = DateTime.Now.Date;
                        //addEditPhotoShoot.ImageFolder = photoShootPath.Trim(); 
                        newPhotoShoot.PhotographyjobID =  isJobExists;
                        if (selectedPS.PhotoshootType != null)
                            newPhotoShoot.PhotoshootType = selectedPS.PhotoshootType;
                        newPhotoShoot.Season = selectedPS.Season == null ? "" : selectedPS.Season.ToString();
                        if (newPhotoShoot != null)
                        {
                            db.PhotoShoots.InsertOnSubmit(newPhotoShoot);
                            db.SubmitChanges();
                        }

                        List<PhotoshootWorkflowItem> tempWorkflows = (from psw in db.PhotoshootWorkflowItems where psw.PhotoShootID == selectedPS.PhotoShotID select psw).ToList();

                        if (tempWorkflows.Count > 0)
                        {
                            foreach (PhotoshootWorkflowItem psw in tempWorkflows)
                            {
                                PhotoshootWorkflowItem _objPhotoShootWorkflowItem = new PhotoshootWorkflowItem();

                                _objPhotoShootWorkflowItem.PhotoShootID = newPhotoShoot.PhotoShotID;
                                _objPhotoShootWorkflowItem.WorkflowItemId = psw.WorkflowItemId;
                                _objPhotoShootWorkflowItem.Notes = psw.Notes;
                                _objPhotoShootWorkflowItem.DueDate = psw.DueDate.Value.AddYears(1);
                                _objPhotoShootWorkflowItem.CreatedOn = DateTime.Now;
                                _objPhotoShootWorkflowItem.CreatedBy = psw.CreatedBy;
                                _objPhotoShootWorkflowItem.Assignedto = psw.Assignedto;
                                _objPhotoShootWorkflowItem.Status = psw.Status;
                                _objPhotoShootWorkflowItem.SortOrder = psw.SortOrder;
                                _objPhotoShootWorkflowItem.HasNotes = psw.HasNotes;
                                _objPhotoShootWorkflowItem.CompletedBy = ""; // psw.CompletedBy;
                                _objPhotoShootWorkflowItem.CompletedOn = null; // psw.CompletedOn != null ? psw.CompletedOn.Value.AddYears(1) : psw.CompletedOn;
                                _objPhotoShootWorkflowItem.Quantity = psw.Quantity;

                                db.PhotoshootWorkflowItems.InsertOnSubmit(_objPhotoShootWorkflowItem);
                                db.SubmitChanges();
                            }
                        }

                    }
                    refreshPhotoShoot(Convert.ToInt32(selectedPhotoShoot.PhotographyJob.SchoolID));
                }
                else
                    MVVMMessageService.ShowMessage("Please select at least one photoshoot to auto create.");
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }

        }

        #region Workflow Methods
        internal void addCollectionItems(string isFromTab)
        {
            if (selectedPhotoShoot == null) { MVVMMessageService.ShowMessage("Please select a Photoshoot to proceed."); return; }
            if (isFromTab == "Dashboard")
            {
                if (dgWorkflowItemsData != null)
                {
                    int bfaWorkflowscount = dgWorkflowItemsData.Count;
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    AddCollectionItems _objAddCollectionItems = new AddCollectionItems("Dashboard", selectedPhotoShoot.PhotoShotID);
                    _objAddCollectionItems.ShowDialog();

                    if (((AddCollectionItemsViewModel)(_objAddCollectionItems.DataContext)).isSave)
                    {
                        dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getAllPhotoshootWorkflowItem(db, selectedPhotoShoot.PhotoShotID, "Workflow"));

                        #region Logfile
                        clsErrorLog objError = new clsErrorLog();

                        objError.Source = "Photoshoot WorkflowItems";
                        objError.MethodName = "Adding WorkflowItems to Photoshoot(s)";
                        objError.Message = "Photoshoot WorkflowItems log file. \n Schoolname : " + clsSchool.defaultSchoolName + "\n Photoshoot name : " + selectedPhotoShoot.PhotoShotName + " \n Action: Record(s) Added \n Added records count : " + (dgWorkflowItemsData.Count - bfaWorkflowscount) + "\n Photoshoot workflows count : " + dgWorkflowItemsData.Count + "\n";
                        objError.UserComments = clsStatic.userName;
                        objError.DateTime = DateTime.Now;

                        clsStatic.WriteErrorLog(objError, "Photoshoot WorkflowItems Info.");
                        //clsStatic.WriteErrorLog(objError, objstatic.ErrorLogXML);

                        #endregion
                    }
                }
            }
            else if (isFromTab == "Equipment")
            {
                //int bfaWorkflowscount = dgWorkflowItemsData.Count;
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                AddCollectionItems _objAddCollectionItems = new AddCollectionItems("Equipment", selectedPhotoShoot.PhotoShotID);
                _objAddCollectionItems.ShowDialog();

                if (((AddCollectionItemsViewModel)(_objAddCollectionItems.DataContext)).isSave)
                {
                    dgEquipmentData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getAllPhotoshootWorkflowItem(db, selectedPhotoShoot.PhotoShotID, "Equipment"));

                    #region Logfile
                    //clsErrorLog objError = new clsErrorLog();

                    //objError.Source = "Photoshoot WorkflowItems";
                    //objError.MethodName = "Adding WorkflowItems to Photoshoot(s)";
                    //objError.Message = "Photoshoot WorkflowItems log file. \n Schoolname : " + clsSchool.defaultSchoolName + "\n Photoshoot name : " + selectedPhotoShoot.PhotoShotName + " \n Action: Record(s) Added \n Added records count : " + (dgWorkflowItemsData.Count - bfaWorkflowscount) + "\n Photoshoot workflows count : " + dgWorkflowItemsData.Count + "\n";
                    //objError.UserComments = clsStatic.userName;
                    //objError.DateTime = DateTime.Now;

                    //clsStatic.WriteErrorLog(objError, "Photoshoot WorkflowItems Info.");
                    //clsStatic.WriteErrorLog(objError, objstatic.ErrorLogXML);

                    #endregion
                }
            }
            else
                MVVMMessageService.ShowMessage("Please select a PhotoShoot");

        }

        private void addNotesVisible()
        {
            selectedButton = "AddButton";
            if (notesTextVisibility == Visibility.Visible)
            {
                notesTextVisibility = Visibility.Collapsed;
                notes = "";
            }
            else
                notesTextVisibility = Visibility.Visible;
        }

        private void addEditNotes()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            PhotoshootWorkflowItem tempPSWorkflowItem = new PhotoshootWorkflowItem();
            if (!string.IsNullOrEmpty(notes) && selectedWorkflowItem != null && selectedPhotoShoot != null)
            {
                if (selectedButton == "AddButton")
                {
                    string timestamp = "----- Created by:   " + clsStatic.userName + " " + DateTime.Now + " -----";

                    notes = timestamp + Environment.NewLine + notes;

                    tempPSWorkflowItem = clsWorkflows.getPhotoWorkflowItem(db, photoshootWorkflowItemId);
                    tempPSWorkflowItem.Notes = notes + Environment.NewLine + Environment.NewLine + tempPSWorkflowItem.Notes;

                    db.SubmitChanges();

                    workflowItemNotes.Insert(0, new ComboBoxItem { Name = notes });

                    notes = "";
                    notesTextVisibility = Visibility.Collapsed;
                }
                else if (selectedButton == "EditButton")
                {
                    string newNotes = "";

                    tempPSWorkflowItem = clsWorkflows.getPhotoWorkflowItem(db, photoshootWorkflowItemId);

                    string[] tempselectedNotes = selectedNotes.Name.Split(new string[] { "\n" }, StringSplitOptions.None);

                    string[] lines = tempPSWorkflowItem.Notes.Split(new string[] { "\r\n\r\n", "\n" }, StringSplitOptions.None);

                    workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                    for (int i = 0; i < lines.Count() - 1; i += 2)
                    {
                        if (lines[i] == tempselectedNotes[0])
                        {
                            lines[i + 1] = notes;
                        }
                        if (!string.IsNullOrEmpty(newNotes))
                        {
                            newNotes = newNotes + (Environment.NewLine + Environment.NewLine) + (lines[i].TrimEnd('\r') + "\n" + lines[i + 1]);
                        }
                        else
                            newNotes = lines[i].TrimEnd('\r') + "\n" + lines[i + 1];
                    }

                    if (!string.IsNullOrEmpty(newNotes))
                    {
                        tempPSWorkflowItem.Notes = newNotes;
                        db.SubmitChanges();
                        workflowItemMouseLeftClick();
                    }

                }
                #region update hasNotes field
                if (!string.IsNullOrEmpty(tempPSWorkflowItem.Notes) && (tempPSWorkflowItem.HasNotes == false || tempPSWorkflowItem.HasNotes == null))
                {
                    clsWorkflows.updatePhotoShootWorkflowItemshasNotes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedWorkflowItem, "from Edit/Add");

                    selectPhotoShoot(selectedPhotoShoot.PhotoShotID);

                }
                #endregion
            }
            else
            {
                if (selectedWorkflowItem == null)
                    MVVMMessageService.ShowMessage("Please select workflow item.");
                else
                    MVVMMessageService.ShowMessage("Please fill notes.");
            }
        }

        private void editNotes()
        {
            selectedButton = "EditButton";
            if (selectedNotes != null)
            {
                if (notesTextVisibility == Visibility.Visible)
                {
                    notesTextVisibility = Visibility.Collapsed;
                    notes = "";
                }
                else
                {
                    notesTextVisibility = Visibility.Visible;
                    string[] tempNotes = selectedNotes.Name.Split(new string[] { "\n" }, StringSplitOptions.None);
                    notes = tempNotes[1];
                }
            }
        }
        /// <summary>
        /// this method used to delete selected notes of the selected workflowItem
        /// </summary>
        private void deleteNotes()
        {
            selectedButton = "DeleteButton";
            if (selectedNotes != null && selectedWorkflowItem != null && selectedPhotoShoot != null)
            {
                notesTextVisibility = Visibility.Collapsed;
                notes = "";


                string newNotes = "";

                PhotoshootWorkflowItem tempPSWorkflowItem = clsWorkflows.getPhotoWorkflowItem(db, photoshootWorkflowItemId);

                string[] tempselectedNotes = selectedNotes.Name.Split(new string[] { "\n" }, StringSplitOptions.None);

                string[] lines = tempPSWorkflowItem.Notes.Split(new string[] { "\r\n\r\n", "\n" }, StringSplitOptions.None);

                int maxcount = lines.Count() % 2 == 0 ? lines.Count() : lines.Count() - 1;

                workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                for (int i = 0; i < maxcount; i += 2)
                {
                    if (lines[i] == tempselectedNotes[0])
                    {
                        lines[i] = "";
                        lines[i + 1] = notes;
                    }
                    if (!string.IsNullOrEmpty(newNotes))
                    {
                        if (lines[i] == "")
                        {
                        }
                        else
                        {
                            newNotes = newNotes + (Environment.NewLine + Environment.NewLine) + (lines[i].TrimEnd('\r') + "\n" + lines[i + 1]);
                        }
                    }
                    else if (lines[i] != "")
                        newNotes = lines[i].TrimEnd('\r') + "\n" + lines[i + 1];
                }

                if (newNotes == "")
                    newNotes = null;
                tempPSWorkflowItem.Notes = newNotes;
                db.SubmitChanges();

                #region update hasNotes field
                if (string.IsNullOrEmpty(tempPSWorkflowItem.Notes) && tempPSWorkflowItem.HasNotes == true)
                {
                    clsWorkflows.updatePhotoShootWorkflowItemshasNotes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedWorkflowItem, "from Delete");

                    selectPhotoShoot(selectedPhotoShoot.PhotoShotID);

                }
                #endregion
                workflowItemMouseLeftClick();
            }
            else
            {
                if (selectedWorkflowItem == null)
                    MVVMMessageService.ShowMessage("Please select workflow item.");
                else
                    MVVMMessageService.ShowMessage("Please select notes.");
            }
        }
        private void workflowItemMouseLeftClick()
        {
            setButtonVisibilityforWorkflowItems();
            if (selectedPhotoShoot != null)
            {

                if (selectedWorkflowItem == null) { return; }
                if (notesTextVisibility == Visibility.Visible)
                {
                    notesTextVisibility = Visibility.Collapsed;
                    notes = "";
                }

                selectedGridName = "workflow";

                photoshootWorkflowItemId = selectedWorkflowItem.Id;

                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                //Load Notes
                //workflowItemNotes = new ObservableCollection<ComboBoxItem>(clsWorkflows.getNotesForWorkflowItem(db, (int)selectedWorkflowItem.WorkflowItemId, selectedPhotoShoot.PhotoShotID));

                ObservableCollection<ComboBoxItem> tempWorkflowItemNotes = new ObservableCollection<ComboBoxItem>(clsWorkflows.getNotesForWorkflowItem(db, (int)selectedWorkflowItem.WorkflowItemId, selectedPhotoShoot.PhotoShotID));
                if (tempWorkflowItemNotes.Count > 0)
                {
                    string workflowNotes = tempWorkflowItemNotes[0].Name.ToString();
                    string[] lines = workflowNotes.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                    workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                    foreach (string s in lines)
                    {
                        if (string.IsNullOrEmpty(s)) { continue; }
                        workflowItemNotes.Add(new ComboBoxItem { Name = s });
                    }
                }
                else
                    workflowItemNotes = new ObservableCollection<ComboBoxItem>();
            }
            else
                workflowItemNotes = new ObservableCollection<ComboBoxItem>();
        }
        private void workflowItemMouseDoubleClick()
        {
            if (selectedWorkflowItem != null)
            {
                EditPhotoshootWorkflowItems _objEditWorkflowItems = new EditPhotoshootWorkflowItems(selectedWorkflowItem);
                _objEditWorkflowItems.ShowDialog();

                if (((EditPhotoshootWorkflowItemsViewModel)(_objEditWorkflowItems.DataContext)).isSave)
                {
                    int tempIndex = dgWorkflowItemsData.Count <= 1 ? 0 : dgWorkflowItemsData.IndexOf(selectedWorkflowItem);
                    dgWorkflowItemsData.Remove(selectedWorkflowItem);

                    dgWorkflowItemsData.Insert(tempIndex, ((EditPhotoshootWorkflowItemsViewModel)(_objEditWorkflowItems.DataContext)).addEditWorkflowItem);
                    selectedWorkflowItem = ((EditPhotoshootWorkflowItemsViewModel)(_objEditWorkflowItems.DataContext)).addEditWorkflowItem;
                    selectedWorkflowItems.Add(selectedWorkflowItem);
                }
            }
        }
        private void equipmentItemMouseDoubleClick()
        {
            if (selectedEquipmentItem != null)
            {
                AddEditEquipmentItems _objAddEditEquipmentItems = new AddEditEquipmentItems(selectedEquipmentItem);
                _objAddEditEquipmentItems.ShowDialog();

                if (((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext)).isSave)
                {
                    int tempIndex = dgEquipmentData.Count <= 1 ? 0 : dgEquipmentData.IndexOf(selectedEquipmentItem);
                    dgEquipmentData.Remove(selectedEquipmentItem);

                    dgEquipmentData.Insert(tempIndex, ((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext))._objPhotoshootWorkflowItem);
                    selectedEquipmentItem = ((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext))._objPhotoshootWorkflowItem;
                    selectedEquipmentItems.Add(selectedEquipmentItem);
                }
            }
        }
        private void photoShootGridLeftClick()
        {
            (Application.Current as App).isNewVisible = true;
            (Application.Current as App).isEditVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isRenameSourceImagesVisible = true;
            (Application.Current as App).isAutoAssignClassVisible = true; (Application.Current as App).isIPTCVisible = true;
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isRefreshVisible = true; (Application.Current as App).isAddToGroupVisible = true;
            (Application.Current as App).isOpenFolderVisible = true; (Application.Current as App).isAddCollectionButtonVisible = false;
            (Application.Current as App).isAddEquipmentItemsButtonVisible = false; (Application.Current as App).isDuplicatePhotoShootsButtonVisible = true;
        }
        private void equipmentItemMouseLeftClick()
        {
            selectedGridName = "equipment";
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isAddCollectionButtonVisible = false; (Application.Current as App).isAddEquipmentItemsButtonVisible = true;
            (Application.Current as App).isEditVisible = true;

        }
        public void setButtonVisibilityforWorkflowItems()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isBulkRenameVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isAddCollectionButtonVisible = true; (Application.Current as App).isAddEquipmentItemsButtonVisible = false;
        }
        #endregion
        /// <summary>
        /// This method is used to group panels based on selected column
        /// </summary>
        public void groupPanels()
        {
            try
            {
                if (selectedGridName == "studentPhotos")
                {
                    if (photosTableShowGroupPanel)
                        photosTableShowGroupPanel = false;
                    else
                        photosTableShowGroupPanel = true;
                }
                else if (selectedGridName == "PhotoShoot")
                {
                    if (photoshootTableShowGroupPanel)
                        photoshootTableShowGroupPanel = false;
                    else
                        photoshootTableShowGroupPanel = true;
                }
                else if (selectedGridName == "workflow")
                {
                    if (workflowItemsShowGroupPanel)
                        workflowItemsShowGroupPanel = false;
                    else
                        workflowItemsShowGroupPanel = true;
                }
                else if (selectedGridName == "equipment")
                {
                    if (equipmentItemsShowGroupPanel)
                        equipmentItemsShowGroupPanel = false;
                    else
                        equipmentItemsShowGroupPanel = true;
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        public void searchPanels()
        {
            try
            {
                if (selectedGridName == "studentPhotos")
                {
                    if (isSearchControlVisible == false || photosTableSearchControl == null)
                    {
                        photosTableSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                    }
                    else
                    {
                        photosTableSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
                    }
                }
                else if (selectedGridName == "PhotoShoot")
                {
                    if (isPhotoshootSearchControlVisible == false || photoshootTableSearchControl == null)
                    {
                        photoshootTableSearchPanelMode = ShowSearchPanelMode.Always; isPhotoshootSearchControlVisible = true;
                    }
                    else
                    {
                        photoshootTableSearchPanelMode = ShowSearchPanelMode.Never; isPhotoshootSearchControlVisible = false;
                    }
                }
                else if (selectedPhotoShootIndex == 1)
                {
                    if (workflowItemsSearchControl == null || !workflowItemsSearchControlVisible) //|| !ordersTableView.SearchControl.IsVisible)
                    {
                        workflowItemsSearchPanelMode = ShowSearchPanelMode.Always; workflowItemsSearchControlVisible = true;
                    }
                    else
                    {
                        workflowItemsSearchPanelMode = ShowSearchPanelMode.Never; workflowItemsSearchControlVisible = false;
                    }
                }
                else if (selectedPhotoShootIndex == 2)
                {
                    if (equipmentItemsSearchControl == null || !equipmentItemsSearchVisible) //|| !ordersTableView.SearchControl.IsVisible)
                    {
                        equipmentItemsSearchPanelMode = ShowSearchPanelMode.Always; equipmentItemsSearchVisible = true;
                    }
                    else
                    {
                        equipmentItemsSearchPanelMode = ShowSearchPanelMode.Never; equipmentItemsSearchVisible = false;
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
        /// this method used for buttons visibility for WorkflowItems
        /// </summary>
        public void setButtonVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            //(Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDuplicatePhotoShootsButtonVisible = true;
            // (Application.Current as App).isDeleteVisible = true;
        }
        #endregion
    }
}
