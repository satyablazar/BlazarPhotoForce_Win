using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using PhotoForce.StudentImageManagement;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections;
using System.Windows.Media;
using PhotoForce.WorkPlace;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Drawing;
using PhotoForce.OrdersManagement;
using Excel;
using System.Data;

namespace PhotoForce.GroupManagement
{
    public class ManageGroupsViewModel : ViewModelBase
    {
        # region Initialization
        PhotoSorterDBModelDataContext db;
        AddGroup _objAddGroup;
        EditGroup _objAddNewGroup;
        BulkRenameStudentImage _objBulkRenameStudentImage;
        string ImageFolderPath = "";
        public static String selectedGridName = "group";
        public int groupId = 0;
        public int selectedIndex = 0;
        int? studentIDPK = null;
        string imagePathToShow = "";
        public string groupImageName = "";
        public string imageFolder = "";
        //string password = "";
        int studentImageId = 0;
        # endregion

        #region Properties
        private ObservableCollection<Group> _dgGroupsData;
        private ObservableCollection<GroupItem> _dgStudentPhotosData;
        private List<GroupClassPhoto> _dgClassPhotosData;
        private ObservableCollection<GroupItem> _VisibleData;
        private GroupItem _selectedStudentPhoto;
        private GroupClassPhoto _selectedClassPhoto;
        private ImageSource _studentImageSource;
        private Group _selectedGroup;
        private ObservableCollection<Group> _selectedGroups;
        private ObservableCollection<GroupItem> _selectedStudentPhotos;
        private SearchControl _groupsTableSearchControl;
        private bool _isGroupsSearchControlVisible;
        private ShowSearchPanelMode _groupsShowSearchPanelMode;
        private bool _groupsTableShowGroupPanel;
        private SearchControl _groupPhotosSearchControl;
        private bool _isGroupPhotosSearchControlVisible;
        private ShowSearchPanelMode _groupPhotosShowSearchPanelMode;
        private bool _groupPhotosTableShowGroupPanel;
        private SearchControl _studentPhotosSearchControl;
        private bool _isStudentPhotosSearchControlVisible;
        private ShowSearchPanelMode _studentPhotosShowSearchPanelMode;
        private bool _studentPhotosTableShowGroupPanel;

        public SearchControl studentPhotosSearchControl
        {
            get { return _studentPhotosSearchControl; }
            set { _studentPhotosSearchControl = value; NotifyPropertyChanged("studentPhotosSearchControl"); }
        }
        public bool isStudentPhotosSearchControlVisible
        {
            get { return _isStudentPhotosSearchControlVisible; }
            set { _isStudentPhotosSearchControlVisible = value; NotifyPropertyChanged("isStudentPhotosSearchControlVisible"); }
        }
        public ShowSearchPanelMode studentPhotosShowSearchPanelMode
        {
            get { return _studentPhotosShowSearchPanelMode; }
            set { _studentPhotosShowSearchPanelMode = value; NotifyPropertyChanged("studentPhotosShowSearchPanelMode"); }
        }
        public bool studentPhotosTableShowGroupPanel
        {
            get { return _studentPhotosTableShowGroupPanel; }
            set { _studentPhotosTableShowGroupPanel = value; NotifyPropertyChanged("studentPhotosTableShowGroupPanel"); }
        }
        public bool isGroupPhotosSearchControlVisible
        {
            get { return _isGroupPhotosSearchControlVisible; }
            set { _isGroupPhotosSearchControlVisible = value; NotifyPropertyChanged("isGroupPhotosSearchControlVisible"); }
        }
        public ShowSearchPanelMode groupPhotosShowSearchPanelMode
        {
            get { return _groupPhotosShowSearchPanelMode; }
            set { _groupPhotosShowSearchPanelMode = value; NotifyPropertyChanged("groupPhotosShowSearchPanelMode"); }
        }
        public bool groupPhotosTableShowGroupPanel
        {
            get { return _groupPhotosTableShowGroupPanel; }
            set { _groupPhotosTableShowGroupPanel = value; NotifyPropertyChanged("groupPhotosTableShowGroupPanel"); }
        }
        public SearchControl groupPhotosSearchControl
        {
            get { return _groupPhotosSearchControl; }
            set { _groupPhotosSearchControl = value; NotifyPropertyChanged("groupPhotosSearchControl"); }
        }
        public bool groupsTableShowGroupPanel
        {
            get { return _groupsTableShowGroupPanel; }
            set { _groupsTableShowGroupPanel = value; NotifyPropertyChanged("groupsTableShowGroupPanel"); }
        }
        public ShowSearchPanelMode groupsShowSearchPanelMode
        {
            get { return _groupsShowSearchPanelMode; }
            set { _groupsShowSearchPanelMode = value; NotifyPropertyChanged("groupsShowSearchPanelMode"); }
        }
        public bool isGroupsSearchControlVisible
        {
            get { return _isGroupsSearchControlVisible; }
            set { _isGroupsSearchControlVisible = value; NotifyPropertyChanged("isGroupsSearchControlVisible"); }
        }
        public SearchControl groupsTableSearchControl
        {
            get { return _groupsTableSearchControl; }
            set { _groupsTableSearchControl = value; NotifyPropertyChanged("groupsTableSearchControl"); }
        }
        public ObservableCollection<GroupItem> selectedStudentPhotos
        {
            get { return _selectedStudentPhotos; }
            set { _selectedStudentPhotos = value; NotifyPropertyChanged("selectedStudentPhotos"); }
        }
        public ObservableCollection<Group> selectedGroups
        {
            get { return _selectedGroups; }
            set { _selectedGroups = value; NotifyPropertyChanged("selectedGroups"); }
        }
        public Group selectedGroup
        {
            get { return _selectedGroup; }
            set { _selectedGroup = value; NotifyPropertyChanged("selectedGroup"); }
        }
        public ImageSource studentImageSource
        {
            get { return _studentImageSource; }
            set { _studentImageSource = value; NotifyPropertyChanged("studentImageSource"); }
        }
        public GroupClassPhoto selectedClassPhoto
        {
            get { return _selectedClassPhoto; }
            set { _selectedClassPhoto = value; NotifyPropertyChanged("selectedClassPhoto"); }
        }
        public ObservableCollection<GroupItem> VisibleData
        {
            get { return _VisibleData; }
            set { _VisibleData = value; NotifyPropertyChanged("VisibleData"); }
        }
        public List<GroupClassPhoto> dgClassPhotosData
        {
            get { return _dgClassPhotosData; }
            set { _dgClassPhotosData = value; NotifyPropertyChanged("dgClassPhotosData"); }
        }
        public ObservableCollection<GroupItem> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        public GroupItem selectedStudentPhoto
        {
            get { return _selectedStudentPhoto; }
            set { _selectedStudentPhoto = value; NotifyPropertyChanged("selectedStudentPhoto"); }
        }
        public ObservableCollection<Group> dgGroupsData
        {
            get { return _dgGroupsData; }
            set { _dgGroupsData = value; NotifyPropertyChanged("dgGroupsData"); }
        }
        #endregion

        #region Constructors
        public ManageGroupsViewModel()
        {
            selectedGroups = new ObservableCollection<Group>(); selectedStudentPhotos = new ObservableCollection<GroupItem>();
            loadData();
            //ImageFolderPath = ((Setting)clsDashBoard.getSettingByName(db, "ImageFolderLocation")).settingValue;
        }
        #endregion

        #region Commands
        public RelayCommand GroupPanelGotFocusCommand
        {
            get
            {
                return new RelayCommand(groupPanelGotFocus);
            }
        }
        public RelayCommand GroupsGridMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(dgGroupsMouseDoubleClick);
            }
        }
        public RelayCommand GroupsTableViewMouseUpCommand
        {
            get
            {
                return new RelayCommand(groupsTableMouseUp);
            }
        }
        public RelayCommand ClassPanelPhotosGotFocusCommand
        {
            get
            {
                return new RelayCommand(classPhotosPanelGotFocus);
            }
        }
        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(delete);
            }
        }
        public RelayCommand ClassPhotosTableMouseUpCommand
        {
            get
            {
                return new RelayCommand(classPhotosTableMouseUp);
            }
        }
        public RelayCommand StudentPhotosPanelGotFocusCommand
        {
            get
            {
                return new RelayCommand(studentPhotosPanelGotFocus);
            }
        }
        public RelayCommand StudentPhotosPanelDoubleClickCommand
        {
            get
            {
                return new RelayCommand(studentPhotosPanelDoubleClick);
            }
        }
        public RelayCommand StudentPhotosTableKeyUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotosTableKeyUp);
            }
        }
        public RelayCommand StudentImagePreviewCommand
        {
            get
            {
                return new RelayCommand(studentImagePreview);
            }
        }
        #endregion

        #region Methods
        private void groupPanelGotFocus()
        {
            selectedGridName = "group";
            setButtonVisibilityForGroups();
        }

        private void classPhotosPanelGotFocus()
        {
            selectedGridName = "groupPhotos";
        }

        private void dgGroupsMouseDoubleClick()
        {
            try
            {
                selectedGridName = "group";
                editRecord();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        private void studentPhotosPanelGotFocus()
        {
            selectedGridName = "studentPhotos";
            setButtonVisibilityForPhotos();
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

        private void groupsTableMouseUp()
        {
            try
            {
                selectedGridName = "group";
                if (selectedGroup != null)
                {
                    setButtonVisibilityForGroups();
                    groupId = selectedGroup.ID;

                    dgStudentPhotosData = new ObservableCollection<GroupItem>(clsGroup.getstudentImagesByGroup(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), groupId));
                    dgClassPhotosData = clsGroup.getClassPhotoByGroup(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), groupId);

                    if (dgStudentPhotosData.Count() > 0)
                    {
                        selectedStudentPhoto = dgStudentPhotosData.First();
                        selectImage("StudentPhotoGrid");
                    }
                    else
                    {
                        studentImageSource = null;
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
        /// used to delete group(s)
        /// </summary>
        public void deleteGroup()
        {
            try
            {
                string message = "", groupname = "";
                if (selectedGroups.Count > 0)
                {
                    int delRetval = 0;
                    ArrayList arrgrpId = new ArrayList();
                    foreach (Group grp in selectedGroups)
                    {
                        int GroupId = grp.ID;
                        groupname = grp.GroupName;
                        if (!arrgrpId.Contains(GroupId))
                        {
                            arrgrpId.Add(GroupId);
                        }
                    }
                    if (arrgrpId.Count == 1)
                    {
                        message = "Are you sure you want to delete group " + groupname + "?";
                    }
                    else
                    {
                        message = "Are you sure you want to delete multiple groups?";
                    }
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgGroupsData.Count();
                        //int deletedRecordsCount = arrgrpId.Count;

                        delRetval = clsGroup.deleteGroup(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrgrpId);
                        if (delRetval != 0)
                        {
                            foreach (int grpId in arrgrpId)
                            {
                                dgGroupsData.Remove(dgGroupsData.Where(i => i.ID == grpId).First());
                            }
                            //createDeletedRecordsLogFile("Groups", totalRecordsCount, deletedRecordsCount);
                        }
                        //loadData();
                        try
                        {
                            dgStudentPhotosData = null;
                        }
                        catch (Exception ex)
                        {
                            MVVMMessageService.ShowMessage(ex.Message);
                        }

                        studentImageSource = null;
                        dgClassPhotosData = null;
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        private void classPhotosTableMouseUp()
        {
            try
            {
                if (selectedClassPhoto != null)
                {
                    selectImage("ClassPhotoGrid");
                }
                selectedGridName = "groupPhotos";
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        private void studentPhotosPanelDoubleClick()
        {
            selectedGridName = "studentPhotos";
            editRecord();
        }

        private void studentPhotosTableKeyUp()
        {
            try
            {
                selectedGridName = "studentPhotos";
                if (selectedStudentPhoto != null)
                {
                    setButtonVisibilityForPhotos();
                    selectImage("StudentPhotoGrid");
                }
                else
                {
                    (Application.Current as App).isOpenFolderVisible = false;
                }

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        # region Rename data in bulk
        /// <summary>
        /// This method is used to rename data in bulk
        /// </summary>
        internal void bulkRename()
        {
            if (selectedGridName == "group")
            {
                try
                {
                    if (selectedGroups.Count != 0)
                    {
                        int grpId;
                        ArrayList arrGroupId = new ArrayList();
                        foreach (Group grp in selectedGroups)
                        {
                            try
                            {
                                grpId = grp.ID;
                                if (!arrGroupId.Contains(grpId))
                                {
                                    arrGroupId.Add(grpId);
                                }
                            }
                            catch (Exception ex)
                            { clsStatic.WriteExceptionLogXML(ex); }
                        }
                        BulkRenameGroup _objBulkRenameGroup = new BulkRenameGroup(arrGroupId);
                        _objBulkRenameGroup.ShowDialog();
                        if (((BulkRenameGroupViewModel)(_objBulkRenameGroup.DataContext)).isSave)
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
            else if (selectedGridName == "studentPhotos")
            {
                if (selectedStudentPhotos.Count != 0)
                {
                    int stuImageId;
                    ArrayList arrStudentId = new ArrayList();
                    foreach (GroupItem grpItem in selectedStudentPhotos)
                    {
                        try
                        {
                            stuImageId = grpItem.StudentImage.ID;
                            if (!arrStudentId.Contains(stuImageId))
                            {
                                arrStudentId.Add(stuImageId);
                            }
                        }
                        catch (Exception ex)
                        { clsStatic.WriteExceptionLogXML(ex); }
                    }
                    BulkRenameStudentImage _objBulkRenameStudentImage = new BulkRenameStudentImage(arrStudentId);
                    _objBulkRenameStudentImage.ShowDialog();
                    if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                    {
                        refreshGrid();
                    }
                }
            }
        }
        # endregion

        # region Refresh grid
        /// <summary>
        /// This method is used to refresh the grid's after some change in data
        /// </summary>
        internal void refreshGrid()
        {
            if (selectedGroup != null)
            {
                int groupid = selectedGroup.ID;
                if (selectedGridName == "group")
                {
                    PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                    dgGroupsData = new ObservableCollection<Group>(clsGroup.getAllGroups(dbb, clsSchool.defaultSchoolId));
                    dgStudentPhotosData = new ObservableCollection<GroupItem>(clsGroup.getstudentImagesByGroup(dbb, groupid));
                    dgClassPhotosData = clsGroup.getClassPhotoByGroup(dbb, groupid);
                }
                else if (selectedGridName == "studentPhotos")
                {
                    PhotoSorterDBModelDataContext db3 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    dgStudentPhotosData = new ObservableCollection<GroupItem>(clsGroup.getstudentImagesByGroup(db3, groupid));
                }
                else if (selectedGridName == "groupPhotos")
                {
                    PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    dgClassPhotosData = clsGroup.getClassPhotoByGroup(db, groupid);
                }
            }

        }
        # endregion

        internal void loadData()
        {
            try
            {
                VisibleData = new ObservableCollection<GroupItem>();
                dgGroupsData = new ObservableCollection<Group>(clsGroup.getAllGroups(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId));
                if (dgGroupsData.Count() == 0)
                {
                    dgStudentPhotosData = null; dgClassPhotosData = null; selectedGroup = new Group();
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

        }

        # region Select Image
        /// <summary>
        /// Just to show the selected student image on imgStudentPhotoPreview grid
        /// </summary>
        /// <param name="gridName">Name of the grid</param>
        private void selectImage(string gridName)
        {
            try
            {
                int? ImageId = 0;
                if (gridName == "StudentPhotoGrid" && selectedStudentPhoto != null)
                    ImageId = selectedStudentPhoto.StudentPhotoID;
                else if (gridName == "ClassPhotoGrid" && selectedClassPhoto != null)
                    ImageId = selectedClassPhoto.studentImageId;
                else
                {
                    studentImageSource = null;
                    return;
                }
                StudentImage objStudentImage = clsDashBoard.getStudentImageDetailsById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(ImageId));
                if (objStudentImage != null)
                {
                    ImageFolderPath = objStudentImage.PhotoShoot.ImageFolder;
                    string strFile = objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + objStudentImage.ImageName;
                    imagePathToShow = objStudentImage.PhotoShoot.ImageFolder + "\\" + objStudentImage.ImageName;

                    if (!File.Exists(strFile))
                        strFile = imagePathToShow;

                    if (!File.Exists(strFile))
                    {
                        studentImageSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        #region Old Code
                        //System.Drawing.Image img = System.Drawing.Image.FromFile(strFile);
                        //if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        //{
                        //    decoderForJpeg(strFile);
                        //}
                        #endregion
                        using (FileStream fs = new FileStream(strFile, FileMode.Open, FileAccess.Read))
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

            studentImageSource = thumb;
            //}
        }
        # endregion

        # region Add To group
        /// <summary>
        /// to create new group
        /// </summary>
        internal void newRecord()
        {
            try
            {
                _objAddGroup = new AddGroup(clsSchool.defaultSchoolId, "", 0);
                _objAddGroup.ShowDialog();
                if (((PhotoForce.GroupManagement.AddGroupViewModel)(_objAddGroup.DataContext)).isSave)
                {
                    dgGroupsData.Add(((PhotoForce.GroupManagement.AddGroupViewModel)(_objAddGroup.DataContext)).addEditGroup);
                    selectedGroup = ((PhotoForce.GroupManagement.AddGroupViewModel)(_objAddGroup.DataContext)).addEditGroup;
                    //loadData();
                }

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        //added by Mohan
        /// <summary>
        /// to add selected images to selected group
        /// </summary>
        internal void addImagesToGroup()
        {
            try
            {
                if (selectedStudentPhoto == null) { MVVMMessageService.ShowMessage("Please select studentImage(s). "); return; }
                ArrayList arrstuID = new ArrayList();
                int schoolID = clsSchool.defaultSchoolId;
                for (int i = 0; i < selectedStudentPhotos.Count; i++)
                    arrstuID.Add((selectedStudentPhotos[i]).StudentPhotoID);

                AddStudentsToGroup objAddStudentsToGroup = new AddStudentsToGroup(schoolID, arrstuID);
                objAddStudentsToGroup.ShowDialog();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Edit records
        /// <summary>
        /// This method is used to edit the records
        /// </summary>
        internal void editRecord()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            try
            {
                if (selectedGridName == "group")
                {
                    if (selectedGroup == null) { return; }
                    int GrouplID = selectedGroup.ID;
                    Group _objGroup = clsGroup.getGroupDetails(db, GrouplID).SingleOrDefault();
                    _objAddNewGroup = new EditGroup(_objGroup.School.ID, GrouplID);
                    _objAddNewGroup.ShowDialog();
                    if (((EditGroupViewModel)(_objAddNewGroup.DataContext)).isSave)
                    {
                        int tempGroupIndex = dgGroupsData.Count <= 1 ? 0 : dgGroupsData.IndexOf(selectedGroup);
                        dgGroupsData.Remove(selectedGroup);

                        Group tempGroup = ((EditGroupViewModel)(_objAddNewGroup.DataContext)).addEditGroup;

                        dgGroupsData.Insert(tempGroupIndex, tempGroup);
                        selectedGroup = tempGroup;
                        selectedGroups.Add(tempGroup);
                        //dgGroupsData = clsGroup.getAllGroups(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId);
                    }
                }
                else if (selectedGridName == "studentPhotos")
                {
                    if (selectedStudentPhoto == null) { return; }
                    int studentImageId = selectedStudentPhoto.StudentImage.ID;
                    _objBulkRenameStudentImage = new BulkRenameStudentImage(studentImageId);
                    _objBulkRenameStudentImage.ShowDialog();
                    if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                    {
                        //refreshGrid();
                        GroupItem _objGroupItem = (from gi in db.GroupItems
                                                   join si in db.StudentImages on gi.StudentPhotoID equals si.ID
                                                   join s in db.Students
                                                       on si.StudentIDPK equals s.ID
                                                   where gi.GroupID == groupId && si.ID == studentImageId && s.RecordStatus == true
                                                   select gi).FirstOrDefault(); 
                        //StudentImage _objImageDetails = clsDashBoard.getStudentImageDetailsById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), studentImageId);
                        int tempPhotoIndex = dgStudentPhotosData.Count <= 1 ? 0 : dgStudentPhotosData.IndexOf(selectedStudentPhoto);
                        dgStudentPhotosData.Remove(selectedStudentPhoto);
                        selectedStudentPhotos.Remove(selectedStudentPhoto);
                        dgStudentPhotosData.Insert(tempPhotoIndex, _objGroupItem);
                        selectedStudentPhoto = _objGroupItem;
                        selectedStudentPhotos.Add(selectedStudentPhoto);
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion
        /// <summary>
        /// This function will add self-selected yearbook images into the group . The images will be given a rating of 6, and all duplicates will be removed, leaving only one image per student.
        /// </summary>
        internal void importYearbookSelection()
        {
            if (selectedGroup != null)
            {
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Information;
                string message = "This function will add self-selected yearbook images into the group " + selectedGroup.GroupName + ". The images will be given a rating of 6, and all duplicates will be removed, leaving only one image per student.\n\nProceed with selection of the Excel sheet? (it needs to have a column YearbookPose).";
                if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {

                    bool isError = false;
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

                        if (ds.Tables.Count != 0)
                        {
                            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                            DataTable dt = ds.Tables[0];
                            try
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    int updateGroupItemId;
                                    StudentImage si = clsDashBoard.getStudentImageDetailsByName(db, row["YearbookPose"].ToString());
                                    if (si != null)
                                    {
                                        List<GroupItem> groupItemsForStudent = (from ii in dgStudentPhotosData where ii.StudentImage.StudentIDPK == si.StudentIDPK select ii).ToList();

                                        if (groupItemsForStudent.Count == 0)
                                        {
                                            //update stuent image with 6 - star rating
                                            clsDashBoard.updateRating(db, "6", si.ID);

                                            clsGroup.insertGroupItems(db, selectedGroup.ID, si.ID);
                                        }
                                        else
                                        {
                                            if (groupItemsForStudent.Count > 1)
                                            {
                                                //keep one item and delete remaining items ,if count > 1
                                                updateGroupItemId = groupItemsForStudent[groupItemsForStudent.Count - 1].ID;
                                                List<int> remove5StarPhotos = groupItemsForStudent.Select(a => a.ID).Take(groupItemsForStudent.Count - 1).ToList();
                                                int res = clsGroup.deleteGroupItems(db, remove5StarPhotos);
                                                foreach (int imgId in remove5StarPhotos)
                                                {
                                                    dgStudentPhotosData.Remove((from spd in dgStudentPhotosData where spd.ID == imgId select spd).First());
                                                }
                                            }
                                            else
                                            {
                                                updateGroupItemId = groupItemsForStudent.First().ID;
                                            }

                                            //update stuent image with 6 - star rating
                                            clsDashBoard.updateRating(db, "6", si.ID);

                                            //update group item with student yearbook image selection
                                            clsGroup.updateGroupItem(db, updateGroupItemId, si.ID);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                isError = true;
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }

                        if (!isError)
                        {
                            groupsTableMouseUp();
                            MVVMMessageService.ShowMessage("Group items updated.");
                        }

                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage("The process cannot access the file " + fullFileName + " because it is being used by another process.");
                        clsStatic.WriteExceptionLogXML(ex);
                    }


                }
            }
            else
            {
                MVVMMessageService.ShowMessage("Plese select a group.");
            }
        }

        # region Generate pdf
        /// <summary>
        /// This method is used to generate Pdf.
        /// </summary>
        internal void generatePDF()
        {
            try
            {
                if (selectedStudentPhoto != null)
                {
                    int JobId = 0;
                    int studentId = 0;
                    ArrayList arrStudents = new ArrayList();
                    foreach (GroupItem grpItem in (VisibleData.Count == dgStudentPhotosData.Count) ? selectedStudentPhotos : (VisibleData.Count > 0 ? VisibleData : selectedStudentPhotos))
                    {
                        studentId = Convert.ToInt32(grpItem.StudentImage.StudentIDPK);
                        JobId = grpItem.StudentImage.PhotoShoot.PhotographyJob.ID;
                        if (!arrStudents.Contains(studentId))
                        {
                            arrStudents.Add(studentId);
                        }
                    }

                    ArrayList arrGroupId = new ArrayList();
                    foreach (Group grp in selectedGroups)
                    {
                        int groupId = grp.ID;
                        if (groupId != 0)
                            arrGroupId.Add(groupId);
                    }

                    GeneratePDF _objGeneratePDF = new GeneratePDF(clsSchool.defaultSchoolId, JobId, arrStudents, arrGroupId, (VisibleData.Count == dgStudentPhotosData.Count) ? false : (VisibleData.Count > 0 ? true : false));
                    _objGeneratePDF.ShowDialog();
                    //PhotoSorterDBModelDataContext dbAllGroups = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    //dgGroupsData = clsGroup.getAllGroups(dbAllGroups, clsSchool.defaultSchoolId);
                    //groupsTableMouseUp();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Export Photo's
        /// <summary>
        /// This method is used to Export Photos
        /// </summary>
        internal void exportPhoto()
        {
            try
            {
                if (selectedGroup == null) { MVVMMessageService.ShowMessage("Please select a group."); return; }
                if (dgStudentPhotosData.Count == 0) { return; }

                PhotoSorterDBModelDataContext ddb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                int groupId = selectedGroup.ID;
                Dictionary<int, string> dicStudentImages = new Dictionary<int, string>();
                //ArrayList arrStudentImageIds = new ArrayList();
                ArrayList selectedStuImageIds = new ArrayList();
                ArrayList arrGroupId = new ArrayList();
                if (selectedGroups.Count >= 1)
                {
                    int count = 0;
                    int prevSchoolId = 0;
                    foreach (Group grp in selectedGroups)
                    {
                        if (count == 0)
                            prevSchoolId = grp.School.ID;
                        else
                        {
                            if (prevSchoolId != grp.School.ID)
                            {
                                MVVMMessageService.ShowMessage("Export student photos for different schools is not allowed, Please select same school.");
                                return;
                            }
                        }
                        count++;

                        if (!arrGroupId.Contains(grp.ID))
                            arrGroupId.Add(grp.ID);

                        string grpClassPhotoName = clsGroup.getGroupClassImageName(ddb, grp.ID);

                        IEnumerable<GroupItem> _objGroupItems = clsGroup.getstudentImagesByGroup(ddb, grp.ID);
                        if (selectedGroups.Count > 1)
                            VisibleData = new ObservableCollection<GroupItem>();
                        foreach (GroupItem grpItem in VisibleData.Count == 0 ? _objGroupItems : VisibleData)
                        {
                            try
                            {
                                studentImageId = grpItem.StudentImage.ID;
                                if (!dicStudentImages.ContainsKey(studentImageId))
                                {
                                    dicStudentImages.Add(studentImageId, grpClassPhotoName);
                                }
                            }
                            catch (Exception ex)
                            {
                                MVVMMessageService.ShowMessage(ex.Message + "\n\nStudent Image = " + studentImageId + ", Group Photo name = " + grpClassPhotoName);
                                clsStatic.WriteExceptionLogXML(ex);
                            }


                            //if (!arrStudentImageIds.Contains(studentImageId))
                            //{
                            //    arrStudentImageIds.Add(studentImageId);
                            //    if (!arrGroupId.Contains(grp.ID))
                            //        arrGroupId.Add(grp.ID);

                            //if (!studentsList.Contains(grpItem.StudentImage.Student))
                            //{
                            //    studentsList.Add(grpItem.StudentImage.Student);
                            //}
                            //}
                        }
                    }
                }

                foreach (GroupItem grpItem in selectedStudentPhotos)
                {
                    if (!selectedStuImageIds.Contains(grpItem.StudentImage.ID))
                    {
                        selectedStuImageIds.Add(grpItem.StudentImage.ID);
                    }
                }
                Export _objExport = new Export(dicStudentImages, selectedStuImageIds, arrGroupId, selectedGroups.Count, new List<GroupItem>(VisibleData));
                _objExport.ShowDialog();
                //I dont think we need following lines of code
                //if (((ExportViewModel)(_objExport.DataContext)).isSave)
                //{
                //    dgGroupsData = clsGroup.getAllGroups(ddb, clsSchool.defaultSchoolId);
                //    groupsTableMouseUp();
                //}
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        #region IPTC

        internal void GenerateJIF()
        {
            createIptc(dgStudentPhotosData);
        }

        public void createIptc(IEnumerable<GroupItem> _studentsPhotosColl)
        {
            if (_studentsPhotosColl == null) { return; }
            string message = errorMessages.CREATE_ITPC_DATA_GROUP_IMAGE;
            string caption = "Confirmation";
            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
            if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
            {
                string group_img_path = "";
                string group_img_reduced_path = "";
                if (selectedClassPhoto != null)
                {
                    studentIDPK = selectedClassPhoto.StudentImage.StudentIDPK;
                    groupImageName = selectedClassPhoto.StudentImage.ImageName;
                    imageFolder = selectedClassPhoto.StudentImage.PhotoShoot.ImageFolder;
                    studentImageId = selectedClassPhoto.StudentImage.ID;
                    group_img_path = imageFolder + "\\" + groupImageName;
                    group_img_reduced_path = imageFolder + "\\" + "_reduced" + "\\" + groupImageName;
                }

                Dictionary<int?, string> dicStudents = new Dictionary<int?, string>();
                ArrayList arrStudents = new ArrayList();
                ArrayList groupImageKeyWords = new ArrayList();
                string[] imagePaths;
                string tempImageFolder = _studentsPhotosColl.First().StudentImage.PhotoShoot.ImageFolder;
                foreach (var item in _studentsPhotosColl)
                {

                    if (dicStudents.Count != 0)
                    {
                        if (dicStudents.ContainsKey(item.StudentImage.StudentIDPK)) { password = dicStudents[item.StudentImage.StudentIDPK]; }
                        else
                        {
                            password = item.StudentImage.Student.Password;// clsStudent.getStudentPassword(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), item.StudentImage.StudentIDPK);
                        }
                    }
                    else
                    {
                        password = item.StudentImage.Student.Password;//clsStudent.getStudentPassword(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), item.StudentImage.StudentIDPK);
                    }

                    if (password == null)
                    {
                        arrStudents.Add(item.StudentImage.StudentIDPK);
                        GeneratePassword _objGeneratePassword = new GeneratePassword(arrStudents);
                        password = ((GeneratePasswordViewModel)(_objGeneratePassword.DataContext)).generateStudentPassword();
                    }

                    if (!dicStudents.ContainsKey(item.StudentImage.StudentIDPK))
                        dicStudents.Add(item.StudentImage.StudentIDPK, password);
                    if (groupImageKeyWords.Count == 0) { groupImageKeyWords.Add(password); }
                    else
                    {
                        if (!groupImageKeyWords.Contains(password)) { groupImageKeyWords.Add(password); }
                    }
                    if (item.StudentImage.ImageName == groupImageName) { continue; }
                    #region IPTC Code
                    string original_img_path = tempImageFolder + "\\" + item.StudentImage.ImageName;
                    string reduced_img_path = tempImageFolder + "\\" + "_reduced" + "\\" + item.StudentImage.ImageName;


                    if (File.Exists(original_img_path))
                    {
                        if (File.Exists(reduced_img_path))
                        {
                            imagePaths = new string[] { original_img_path, reduced_img_path };
                        }
                        else
                            imagePaths = new string[] { original_img_path };

                        writeIPTC(imagePaths, item.StudentImage.StudentIDPK, password, "", false);  //writes iptc info. to images
                    }
                    #endregion

                }

                string keywordsString = string.Join(",", groupImageKeyWords.ToArray());

                if (File.Exists(group_img_path))
                {
                    if (File.Exists(group_img_reduced_path))
                    {
                        imagePaths = new string[] { group_img_path, group_img_reduced_path };
                    }
                    else
                        imagePaths = new string[] { group_img_path };

                    writeIPTC(imagePaths, studentIDPK, "", keywordsString, true);

                    //MVVMMessageService.ShowMessage("Student passwords are inserted into group images " + groupImageName + ".");
                }

                MVVMMessageService.ShowMessage("IPTC information inserted into group images " + groupImageName + ".");
            }
        }

        private void writeIPTC(string[] tempImagePaths, int? tempstudentID, string tempPassword, string tempkeywordsString, bool isGroupImage)
        {
            foreach (string img in tempImagePaths)
            {
                try
                {
                    string tempPath = img.ToLower().Replace(".jpg", "_PF1.jpg");

                    #region New Code
                    using (var losslessJpeg = new Aurigma.GraphicsMill.Codecs.LosslessJpeg(img))
                    {
                        // IPTC
                        if (losslessJpeg.Iptc == null)
                        {
                            losslessJpeg.Iptc = new Aurigma.GraphicsMill.Codecs.IptcDictionary();
                        }

                        string keyWords = tempstudentID + "," + tempPassword;
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
                    #endregion

                    #region Old Code
                    //using (var jpegReader = new Aurigma.GraphicsMill.Codecs.JpegReader(sourcePath))
                    //using (var resizer = new Aurigma.GraphicsMill.Transforms.Resize(jpegReader.Width, jpegReader.Height))
                    //using (var jpegWriter = new Aurigma.GraphicsMill.Codecs.JpegWriter(sourcePath.ToLower().Replace(".jpg", "_987.jpg")))
                    //{
                    //    //Read EXIF
                    //    var exif = jpegReader.Exif;
                    //    //Check if loaded image contains EXIF metadata
                    //    if (exif == null)
                    //    {
                    //        exif = new Aurigma.GraphicsMill.Codecs.ExifDictionary();
                    //    }

                    //    //Read IPTC
                    //    var iptc = jpegReader.Iptc;
                    //    //Check if loaded image contains IPTC metadata
                    //    if (iptc == null)
                    //    {
                    //        iptc = new Aurigma.GraphicsMill.Codecs.IptcDictionary();
                    //    }
                    //    if (isGroupImage)
                    //        iptc[Aurigma.GraphicsMill.Codecs.IptcDictionary.Keyword] = tempkeywordsString;
                    //    else
                    //        iptc[Aurigma.GraphicsMill.Codecs.IptcDictionary.Keyword] = tempstudentID + "," + tempPassword;


                    //    //Read XMP
                    //    var xmp = new Aurigma.GraphicsMill.Codecs.XmpData();
                    //    //Check if loaded image contains XMP metadata
                    //    if (jpegReader.Xmp != null)
                    //    {
                    //        xmp.Load(jpegReader.Xmp);
                    //    }

                    //    //Write metadata
                    //    jpegWriter.Exif = exif;
                    //    jpegWriter.Iptc = iptc;
                    //    jpegWriter.Xmp = xmp.Save();

                    //    Aurigma.GraphicsMill.Pipeline.Run(jpegReader + resizer + jpegWriter);
                    //}
                    #endregion

                    System.IO.File.Delete(img);
                    System.IO.File.Move(tempPath, img);
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }

            }
        }
        #endregion

        # region Add group class photo
        /// <summary>
        /// This method is used to add group class photo's
        /// </summary>
        public void AddGroupClassPhoto()
        {
            try
            {
                if (selectedGroup != null)
                {
                    groupId = selectedGroup.ID;
                    PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    foreach (GroupItem grpItem in selectedStudentPhotos)
                    {
                        GroupClassPhoto _objGroupClassPhoto = new GroupClassPhoto();
                        studentIDPK = grpItem.StudentImage.StudentIDPK;
                        groupImageName = grpItem.StudentImage.ImageName;
                        imageFolder = grpItem.StudentImage.PhotoShoot.ImageFolder;
                        studentImageId = Convert.ToInt32(grpItem.StudentImage.ID);
                        // Need to insert it into Group Class Photo Table..
                        _objGroupClassPhoto.GroupId = groupId;
                        _objGroupClassPhoto.studentImageId = studentImageId;
                        if (_objGroupClassPhoto != null)
                        {
                            db.GroupClassPhotos.InsertOnSubmit(_objGroupClassPhoto);
                            db.SubmitChanges();
                            // Delete from GroupItem Table..
                            int result = clsGroup.deleteGroupItem(db, groupId, studentImageId);
                        }
                    }
                    dgStudentPhotosData = new ObservableCollection<GroupItem>(clsGroup.getstudentImagesByGroup(db, groupId));
                    dgClassPhotosData = clsGroup.getClassPhotoByGroup(db, groupId);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        # endregion

        /// <summary>
        /// Delete image from group image.
        /// </summary>
        # region Remove images from group photos
        public void delete()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string message = "Are you sure you want to remove image(s) from group photos?";
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    if (selectedClassPhoto != null)
                    {
                        int grpId = Convert.ToInt32(selectedClassPhoto.GroupId);
                        int StudentImgId = Convert.ToInt32(selectedClassPhoto.studentImageId);
                        int result = clsGroup.deleteFromGroupPhoto(db, grpId, StudentImgId);

                        if (result != 0)
                        {
                            GroupItem _objGroupItem = new GroupItem();
                            _objGroupItem.GroupID = grpId;
                            _objGroupItem.StudentPhotoID = StudentImgId;
                            if (_objGroupItem != null)
                            {
                                db.GroupItems.InsertOnSubmit(_objGroupItem);
                                db.SubmitChanges();
                                dgStudentPhotosData = new ObservableCollection<GroupItem>(clsGroup.getstudentImagesByGroup(db, grpId));
                                dgClassPhotosData = clsGroup.getClassPhotoByGroup(db, grpId);
                            }
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
        #endregion

        # region Remove group items
        /// <summary>
        /// This method is used to remove group items
        /// </summary>
        public void removeGroupItem()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (selectedGroup != null && selectedStudentPhotos.Count != 0)
                {

                    int groupId = selectedGroup.ID;
                    int result = 0;
                    string delImgMessage = "";
                    List<int> arrGroupItemId = new List<int>();

                    arrGroupItemId = selectedStudentPhotos.Select(a => a.ID).ToList();
                    //old code commented by mohan
                    //foreach (GroupItem grpItem in selectedStudentPhotos)
                    //{
                    //    int groupItemId = Convert.ToInt32(grpItem.ID);
                    //    if (!arrGroupItemId.Contains(groupItemId))
                    //    {
                    //        arrGroupItemId.Add(groupItemId);
                    //    }
                    //}
                    #region DeleteImages from all
                    if (arrGroupItemId.Count == 1)
                        delImgMessage = errorMessages.REMOVE_IMAGES_FROM_GROUP1 + selectedStudentPhotos.Count + errorMessages.REMOVE_IMAGES_FROM_GROUP2 + selectedGroup.GroupName + ".";
                    else
                        delImgMessage = errorMessages.REMOVE_IMAGES_FROM_GROUP1 + selectedStudentPhotos.Count + errorMessages.REMOVE_IMAGES_FROM_GROUP2 + selectedGroup.GroupName + "."; ;

                    string delImgCaption = "Confirmation";
                    System.Windows.MessageBoxButton delImgButtons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage delImgIcon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(delImgMessage, delImgCaption, delImgButtons, delImgIcon) == System.Windows.MessageBoxResult.Yes)
                    {
                        result = clsGroup.deleteGroupItems(db, arrGroupItemId);
                        foreach (int imgId in arrGroupItemId)
                        {
                            dgStudentPhotosData.Remove((from spd in dgStudentPhotosData where spd.ID == imgId select spd).First());
                        }

                    }
                    selectImage("StudentPhotoGrid");
                    dgClassPhotosData = clsGroup.getClassPhotoByGroup(db, groupId);
                    #endregion

                }
                else
                {
                    MVVMMessageService.ShowMessage(errorMessages.SELECT_STUDENT_IMAGE);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Search panels
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        internal void searchPanels()
        {
            if (selectedGridName == "group")
            {
                if (groupsTableSearchControl == null || isGroupsSearchControlVisible == false)
                {
                    groupsShowSearchPanelMode = ShowSearchPanelMode.Always; isGroupsSearchControlVisible = true;
                }
                else
                {
                    groupsShowSearchPanelMode = ShowSearchPanelMode.Never; isGroupsSearchControlVisible = false;
                }
            }
            else if (selectedGridName == "groupPhotos")
            {
                if (groupPhotosSearchControl == null || isGroupPhotosSearchControlVisible == false)
                {
                    groupPhotosShowSearchPanelMode = ShowSearchPanelMode.Always; isGroupPhotosSearchControlVisible = true;
                }
                else
                {
                    groupPhotosShowSearchPanelMode = ShowSearchPanelMode.Never; isGroupPhotosSearchControlVisible = false;
                }
            }
            else if (selectedGridName == "studentPhotos")
            {
                if (studentPhotosSearchControl == null || isStudentPhotosSearchControlVisible == false)
                {
                    studentPhotosShowSearchPanelMode = ShowSearchPanelMode.Always; isStudentPhotosSearchControlVisible = true;
                }
                else
                {
                    studentPhotosShowSearchPanelMode = ShowSearchPanelMode.Never; isStudentPhotosSearchControlVisible = false;
                }
            }
        }
        # endregion

        # region Group Panels
        /// <summary>
        /// This method is used to group panels
        /// </summary>
        internal void groupPanels()
        {
            if (selectedGridName == "group")
            {
                if (groupsTableShowGroupPanel)
                    groupsTableShowGroupPanel = false;
                else
                    groupsTableShowGroupPanel = true;
            }
            else if (selectedGridName == "groupPhotos")
            {
                if (groupPhotosTableShowGroupPanel)
                    groupPhotosTableShowGroupPanel = false;
                else
                    groupPhotosTableShowGroupPanel = true;
            }
            else if (selectedGridName == "studentPhotos")
            {
                if (studentPhotosTableShowGroupPanel)
                    studentPhotosTableShowGroupPanel = false;
                else
                    studentPhotosTableShowGroupPanel = true;
            }

        }
        # endregion

        #region Restore Images
        public void restoreHighResImages()
        {
            if (selectedGroup == null) { MVVMMessageService.ShowMessage("Please select a group."); return; }
            string groupName = selectedGroup.GroupName;
            int groupId = selectedGroup.ID;
            RestoreImages _objRestoreImages = new RestoreImages(dgStudentPhotosData, groupId, groupName);
            _objRestoreImages.ShowDialog();
        }
        #endregion

        #region AssignStudent
        /// <summary>
        /// This method is used to assign student to selected images
        /// </summary>
        internal void assignStudent()
        {
            int studentImageId = 0;
            ArrayList arrStudentImgId = new ArrayList();
            foreach (GroupItem grpItem in selectedStudentPhotos)
            {
                try
                {
                    studentImageId = Convert.ToInt32(grpItem.StudentImage.ID);
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
                AssignStudent _objAssignStudent = new AssignStudent(arrStudentImgId);
                _objAssignStudent.ShowDialog();
                if (((AssignStudentViewModel)(_objAssignStudent.DataContext)).isSave)
                    dgStudentPhotosData = new ObservableCollection<GroupItem>(clsGroup.getstudentImagesByGroup(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), groupId));
            }
            else
            {
                MVVMMessageService.ShowMessage(errorMessages.SELECT_STUDENT_IMAGE);
            }
        }
        #endregion

        /// <summary>
        /// This method is used to open source folder of images
        /// </summary>
        internal void OpenFolder()
        {
            if (Directory.Exists(ImageFolderPath))
            {
                string args = string.Format("/Select, \"{0}\"", imagePathToShow);
                ProcessStartInfo pfi = new ProcessStartInfo("Explorer.exe", args);

                System.Diagnostics.Process.Start(pfi);
            }
            else
            {
                MVVMMessageService.ShowMessage(errorMessages.PATH_DOES_NOT_EXISTS);
            }
        }

        #region Ribbon visibility based on Grid Selection
        /// <summary>
        /// This method is used for buttons visibility for Group panel
        /// </summary>
        private void setButtonVisibilityForGroups()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true;
            (Application.Current as App).isEditVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isRefreshVisible = true;
            (Application.Current as App).isIPTCVisible = true;
        }
        /// <summary>
        /// This method is used for buttons visibility for GroupItems(studentImages) panel
        /// </summary>
        private void setButtonVisibilityForPhotos()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isRemoveFromGrpVisible = true;
            (Application.Current as App).isEditVisible = true; (Application.Current as App).isOpenFolderVisible = true;
            (Application.Current as App).isSearchVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (Application.Current as App).isRefreshVisible = true; (Application.Current as App).isAssignStudentVisible = true;
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isRefreshVisible = true;
            (Application.Current as App).isAddToGroupVisible = true;
        }
        #endregion

        #endregion

    }
}
