using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.Student_Management;
using PhotoForce.School_Management;
using System.Collections;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System.Windows;
using System.Collections.ObjectModel;

namespace PhotoForce.WorkPlace.UserControls
{
    public class SchoolsViewModel : ViewModelBase
    {
        #region Initialization
        AddEditSchool _objAddEditSchool;
        ImportStudent _objImportStudent;
        ReviewPricing _objReviewPricing;
        string selectedGrid = "";
        PhotoSorterDBModelDataContext db;
        List<School> selectedSchoolsByOrder = new List<School>();
        #endregion

        #region Properties
        ObservableCollection<School> _dgSchoolNameData;
        ObservableCollection<School> _selectedSchools;
        School _selectedSchool;
        bool _schoolTableShowGroupPanel;
        SearchControl _schoolTableSearchControl;
        ShowSearchPanelMode _schoolTableSearchPanelMode;
        bool _isSearchControlVisible;
        Activity _selectedActivity;
        List<Activity> _dgSchoolActivitiesData;
        List<Activity> _selectedActivities;

        public List<Activity> selectedActivities
        {
            get { return _selectedActivities; }
            set { _selectedActivities = value; NotifyPropertyChanged("selectedActivities"); }
        }
        public Activity selectedActivity
        {
            get { return _selectedActivity; }
            set { _selectedActivity = value; NotifyPropertyChanged("selectedActivity"); }
        }
        public List<Activity> dgSchoolActivitiesData
        {
            get { return _dgSchoolActivitiesData; }
            set { _dgSchoolActivitiesData = value; NotifyPropertyChanged("dgSchoolActivitiesData"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public ShowSearchPanelMode schoolTableSearchPanelMode
        {
            get { return _schoolTableSearchPanelMode; }
            set { _schoolTableSearchPanelMode = value; NotifyPropertyChanged("schoolTableSearchPanelMode"); }
        }
        public SearchControl schoolTableSearchControl
        {
            get { return _schoolTableSearchControl; }
            set { _schoolTableSearchControl = value; NotifyPropertyChanged("schoolTableSearchControl"); }
        }
        public bool schoolTableShowGroupPanel
        {
            get { return _schoolTableShowGroupPanel; }
            set { _schoolTableShowGroupPanel = value; NotifyPropertyChanged("schoolTableShowGroupPanel"); }
        }
        public ObservableCollection<School> selectedSchools
        {
            get { return _selectedSchools; }
            set
            {
                _selectedSchools = value; NotifyPropertyChanged("selectedSchools");

            }
        }
        public School selectedSchool
        {
            get { return _selectedSchool; }
            set
            {
                _selectedSchool = value; NotifyPropertyChanged("selectedSchool");
                //if (selectedSchools.Count > 1)
                //{
                //    selectedSchoolsByOrder = new List<School>();
                //    for (int i = selectedSchools.Count - 1; i >= 0; i--)
                //    {
                //        selectedSchoolsByOrder.Add(selectedSchools[i]);
                //    }
                //}
                //selectedSchoolsByOrder.Add(selectedSchool);
                //if (selectedSchoolsByOrder.Count < 3 && selectedSchools.Count < 3 && selectedSchools.Count > 0)
                //{
                //    selectedSchoolsByOrder.AddRange((from ssc in selectedSchools where selectedSchools));
                //}
                //else if (selectedSchools.Count > 0)
                //{
                //    selectedSchoolsByOrder = new List<School>();
                //    selectedSchoolsByOrder.AddRange(selectedSchools);
                //}
            }
        }
        public ObservableCollection<School> dgSchoolsData
        {
            get { return _dgSchoolNameData; }
            set { _dgSchoolNameData = value; NotifyPropertyChanged("dgSchoolsData"); }
        }
        #endregion

        #region Constructors
        public SchoolsViewModel()
        {
            dgSchoolsData = new ObservableCollection<School>();
            selectedSchools = new ObservableCollection<School>(); selectedActivities = new List<Activity>();
            bindGrid();
        }
        #endregion

        #region Commands
        public RelayCommand SchoolNameGridDoubleClick
        {
            get
            {
                return new RelayCommand(schoolNameGridDoubleClick);
            }
        }
        public RelayCommand SchoolActivitiesGroupFocusCommand
        {
            get
            {
                return new RelayCommand(schoolActivitiesGroup);
            }
        }
        public RelayCommand SchoolTableMouseUpCommand
        {
            get
            {
                return new RelayCommand(schoolTableMouseUp);
            }
        }
        public RelayCommand ActivitiesGridDoubleClickCommand
        {
            get
            {
                return new RelayCommand(editRecord);
            }
        }
        public RelayCommand SchoolsGroupFocusCommand
        {
            get
            {
                return new RelayCommand(schoolsGroupFocus);
            }
        }
        public RelayCommand RowUpdateCommand
        {
            get
            {
                return new RelayCommand(rowUpdate);
            }
        }
        #endregion

        #region Methods
        private void schoolNameGridDoubleClick()
        {
            try
            {
                selectedGrid = "School";
                editRecord();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void schoolsGroupFocus()
        {
            selectedGrid = "School";
            setVisibilityForButtons();
        }
        internal void bindGrid()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            dgSchoolsData = new ObservableCollection<School>(clsDashBoard.getSchools(db));
        }
        /// <summary>
        /// to edit selected school
        /// </summary>
        internal void editRecord()
        {
            try
            {
                //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (selectedSchool != null)
                {
                    int schoolID = selectedSchool.ID;
                    if (selectedGrid == "Activities")
                    {
                        if (selectedActivity != null)
                        {
                            AddEditActivity _objAddEditActivity = new AddEditActivity(selectedActivity);
                            _objAddEditActivity.ShowDialog();
                            if (((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).isSave)
                            {
                                dgSchoolActivitiesData = clsActivities.getSchoolActivities(db, selectedSchool.ID);
                            }
                        }
                    }
                    else
                    {
                        _objAddEditSchool = new AddEditSchool(schoolID);
                        _objAddEditSchool.ShowDialog();
                        if (((AddEditSchoolViewModel)(_objAddEditSchool.DataContext)).isSave)
                        {
                            int tempSchoolIndex = dgSchoolsData.Count <= 1 ? 0 : dgSchoolsData.IndexOf(selectedSchool);
                            dgSchoolsData.Remove(selectedSchool);

                            dgSchoolsData.Insert(tempSchoolIndex, ((AddEditSchoolViewModel)(_objAddEditSchool.DataContext)).addEditSchool);

                            selectedSchool = ((AddEditSchoolViewModel)(_objAddEditSchool.DataContext)).addEditSchool;
                            selectedSchools.Add(selectedSchool);
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
        /// to create new school
        /// </summary>
        internal void newRecord()
        {
            //if (selectedGrid == "Activities")
            //{
            //    AddEditActivity _objAddEditActivity = new AddEditActivity(selectedSchool);
            //    _objAddEditActivity.ShowDialog();
            //    if (((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).isSave)
            //    {
            //        dgSchoolActivitiesData = clsActivities.getSchoolActivities(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedSchool.ID);
            //    }
            //}
            //else   //# By Hema
            _objAddEditSchool = new AddEditSchool(0);
            _objAddEditSchool.ShowDialog();
            if (((AddEditSchoolViewModel)(_objAddEditSchool.DataContext)).isSave)
            {
                dgSchoolsData.Insert(selectedSchool == null ? 0 : dgSchoolsData.IndexOf(selectedSchool), ((AddEditSchoolViewModel)(_objAddEditSchool.DataContext)).addEditSchool);
                selectedSchool = ((AddEditSchoolViewModel)(_objAddEditSchool.DataContext)).addEditSchool;
                //bindGrid();
            }
        }
        /// <summary>
        /// to delete school(s)  
        /// </summary>
        public void deleteRecords()       //#Mohan ; #NUnitTest
        {
            try
            {
                //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string message = "", schoolName = "";
                if (selectedGrid == "Activities")
                {
                    ArrayList activityIds = new ArrayList();
                    foreach (Activity tempActivity in selectedActivities)
                    {
                        activityIds.Add(tempActivity.Id);
                    }
                    if (activityIds.Count == 1)
                        message = "Are you sure you want to delete " + selectedActivity.Subject + " activitiy?";
                    else
                        message = "Are you sure you want to delete " + activityIds.Count + " activities?";

                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        int result = clsActivities.deleteMultipleActivities(db, activityIds);
                        dgSchoolActivitiesData = clsActivities.getSchoolActivities(db, selectedSchool.ID);
                    }
                }
                else
                {
                    if (selectedSchool != null)
                    {
                        int delRetVal = 0;
                        ArrayList arrSchoolId = new ArrayList();
                        foreach (School scl in selectedSchools)
                        {
                            int schoolId = Convert.ToInt32(scl.ID);
                            schoolName = Convert.ToString(scl.SchoolName);
                            if (!arrSchoolId.Contains(schoolId))
                            {
                                arrSchoolId.Add(schoolId);
                            }
                        }
                        if (arrSchoolId.Count == 0)
                        {
                            MVVMMessageService.ShowMessage("please select school(s)");
                            return;
                        }
                        if (arrSchoolId.Count == 1)
                        {
                            message = "Are you sure you want to delete school " + schoolName + "?";
                        }
                        else
                        {
                            message = "Are you sure you want to delete multiple schools?";
                        }
                        string caption = "Confirmation";
                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                        if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                        {
                            //int totalRecordsCount = dgSchoolsData.Count();
                            //int deletedRecordsCount = arrSchoolId.Count;

                            delRetVal = clsDashBoard.deleteSchools(db, arrSchoolId);
                            if (delRetVal >= 0)
                            {
                                foreach (int schId in arrSchoolId)
                                {
                                    dgSchoolsData.Remove(dgSchoolsData.Where(i => i.ID == schId).First());
                                }
                                //createDeletedRecordsLogFile("Schools", totalRecordsCount, deletedRecordsCount);
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
        internal void schoolTableMouseUp()
        {
            selectedGrid = "School";
            if (selectedSchool != null)
            {
                //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                setVisibilityForButtons();
                dgSchoolActivitiesData = clsActivities.getSchoolActivities(db, selectedSchool.ID);
            }
        }
        /// <summary>
        /// this method used to edit more than one school at a time
        /// </summary>
        internal void BulkRename()
        {
            try
            {
                if (selectedSchool != null)
                {
                    int schoolID;
                    ArrayList arrSchoolID = new ArrayList();
                    foreach (School scl in selectedSchools)
                    {
                        schoolID = Convert.ToInt32(scl.ID);
                        if (!arrSchoolID.Contains(schoolID))
                        {
                            arrSchoolID.Add(schoolID);
                        }
                    }
                    BulkRenameSchool _objBulkRenameSchool = new BulkRenameSchool(arrSchoolID);
                    _objBulkRenameSchool.ShowDialog();
                    if (((BulkRenameSchoolViewModel)(_objBulkRenameSchool.DataContext)).isSave)
                    {
                        bindGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void schoolActivitiesGroup()
        {
            selectedGrid = "Activities";
        }
        /// <summary>
        /// This method is used to refresh the grid's after some change in data
        /// </summary>
        internal void RefreshGrid()
        {
            bindGrid();
        }

        internal void StudentImportFromSchool()
        {
            try
            {
                if (selectedSchool != null)
                {
                    int schoolID = selectedSchool.ID;
                    string schoolname = selectedSchool.SchoolName;
                    _objImportStudent = new ImportStudent(schoolID, 0, schoolname);
                    _objImportStudent.ShowDialog();
                    bindGrid();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        /// <summary>
        /// This method is used to search panels
        /// </summary>
        internal void searchPanels()
        {
            if (schoolTableSearchControl == null || !isSearchControlVisible) //|| !SchoolTableView.SearchControl.IsVisible)
            {
                schoolTableSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
            else
            {
                schoolTableSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
            }
        }
        /// <summary>
        /// This method is used to group panels based on selected column
        /// </summary>
        internal void groupPanels()
        {
            if (schoolTableShowGroupPanel)
                schoolTableShowGroupPanel = false;
            else
                schoolTableShowGroupPanel = true;
        }

        internal void SetDefaultPackages()
        {
            if (selectedSchool != null)
            {
                int schoolId = selectedSchool.ID;
                _objReviewPricing = new ReviewPricing(schoolId);
                _objReviewPricing.ShowDialog();
            }
        }

        private void rowUpdate()
        {
            db.SubmitChanges();
        }

        /// <summary>
        /// this method used to merge two shools 
        /// </summary>
        /// <param>first selected school merged</param>
        /// <param>second selected school survivor</param>
        internal void mergeSchools()
        {
            List<int> Ids = new List<int>();
            string mergedSchoolName = "";
            if (selectedSchools.Count == 2)
            {
                foreach (School sc in selectedSchools)
                {
                    if (sc.ID != selectedSchool.ID)
                    {
                        Ids.Add(sc.ID);
                        mergedSchoolName = sc.SchoolName;
                    }
                }
                Ids.Add(selectedSchool.ID);

                string message = "This process will merge '" + mergedSchoolName + "' school with '" + selectedSchool.SchoolName + "' school.'" + mergedSchoolName + "' school will be marked as inactive.\nDo you want to Continue?";
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    message = "This process is not reversible. Are you sure?";
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        bool result = clsSchool.mergeSchools(db, Ids, mergedSchoolName, selectedSchool.SchoolName);
                        if (result)
                        {
                            MVVMMessageService.ShowMessage("Schools merged successfully.");
                            bindGrid();
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage("Schools merged with few errors.Please check exception log file for more details.");
                        }
                    }
                }
            }
        }
        internal void setVisibilityForButtons()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDeleteVisible = false; (Application.Current as App).isDragVisible = true;
            (Application.Current as App).isSearchVisible = true; (Application.Current as App).isRefreshVisible = true;
            (System.Windows.Application.Current as App).isBulkRenameVisible = true;
        }
        #endregion
    }
}
