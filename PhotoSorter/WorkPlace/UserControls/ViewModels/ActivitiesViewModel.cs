using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotoForce.MVVM;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.School_Management;
using System.Collections;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class ActivitiesViewModel : ViewModelBase
    {
        #region Intialization

        public string activitiesType = "";
        PhotoSorterDBModelDataContext db;
        #endregion

        #region Properties
        ObservableCollection<Activity> _dgActivitiesData;
        ObservableCollection<Activity> _selectedActivities;
        Activity _selectedActivity;
        bool _activityTableShowGroupPanel;
        SearchControl _activityTableSearchControl;
        ShowSearchPanelMode _activityTableSearchPanelMode;
        bool _isSearchControlVisible;


        public bool activityTableShowGroupPanel
        {
            get { return _activityTableShowGroupPanel; }
            set { _activityTableShowGroupPanel = value; NotifyPropertyChanged("activityTableShowGroupPanel"); }
        }
        public SearchControl activityTableSearchControl
        {
            get { return _activityTableSearchControl; }
            set { _activityTableSearchControl = value; NotifyPropertyChanged("activityTableSearchControl"); }
        }
        public ShowSearchPanelMode activityTableSearchPanelMode
        {
            get { return _activityTableSearchPanelMode; }
            set { _activityTableSearchPanelMode = value; NotifyPropertyChanged("activityTableSearchPanelMode"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public Activity selectedActivity
        {
            get { return _selectedActivity; }
            set { _selectedActivity = value; NotifyPropertyChanged("selectedActivity"); }
        }
        public ObservableCollection<Activity> selectedActivities
        {
            get { return _selectedActivities; }
            set { _selectedActivities = value; NotifyPropertyChanged("selectedActivities"); }
        }
        public ObservableCollection<Activity> dgActivitiesData
        {
            get { return _dgActivitiesData; }
            set { _dgActivitiesData = value; NotifyPropertyChanged("dgActivitiesData"); }
        }
        #endregion

        #region Constructor
        public ActivitiesViewModel()
        {
            dgActivitiesData = new ObservableCollection<Activity>(); selectedActivities = new ObservableCollection<Activity>();
            loadData();
        }
        #endregion

        #region Commands
        public RelayCommand ActivitiesGridDoubleClickCommand
        {
            get
            {
                return new RelayCommand(activitiesGridDoubleClick);
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
        private void activitiesGridDoubleClick()
        {
            editRecord();
        }
        public void loadData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (activitiesType == "My")
                dgActivitiesData = new ObservableCollection<Activity>(clsActivities.getMyActivities(db));
            else
                dgActivitiesData = new ObservableCollection<Activity>(clsActivities.getAllActivities(db));
        }
        internal void editRecord()
        {
            if (selectedActivity != null)
            {
                AddEditActivity _objAddEditActivity = new AddEditActivity(selectedActivity);
                _objAddEditActivity.ShowDialog();
                if (((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).isSave)
                {
                    int tempActivityIndex = dgActivitiesData.Count <= 1 ? 0 : dgActivitiesData.IndexOf(selectedActivity);
                    dgActivitiesData.Remove(selectedActivity);
                    dgActivitiesData.Insert(tempActivityIndex, ((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).addEditActivity);
                    selectedActivity = ((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).addEditActivity;
                    selectedActivities.Add(selectedActivity);
                }
            }
        }
        internal void newRecord()
        {
            AddEditActivity _objAddEditActivity = new AddEditActivity(new School { SchoolName = clsSchool.defaultSchoolName, ID = clsSchool.defaultSchoolId });
            _objAddEditActivity.ShowDialog();
            if (((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).isSave)
            {
                dgActivitiesData.Add(((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).addEditActivity);
                selectedActivity = ((AddEditActivityViewModel)(_objAddEditActivity.DataContext)).addEditActivity;
            }
        }
        internal void deleteRecord()
        {
            if (selectedActivity == null) { return; }
            string message = "";
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
                //int totalRecordsCount = dgActivitiesData.Count();
                //int deletedRecordsCount = activityIds.Count;

                int result = clsActivities.deleteMultipleActivities(db, activityIds);
                if (result != 0)
                {
                    foreach (int actId in activityIds)
                    {
                        dgActivitiesData.Remove(dgActivitiesData.Where(i => i.Id == actId).First());
                    }
                    //createDeletedRecordsLogFile("Activities", totalRecordsCount, deletedRecordsCount);
                }
            }

        }
        internal void groupPanels()
        {
            if (activityTableShowGroupPanel)
                activityTableShowGroupPanel = false;
            else
                activityTableShowGroupPanel = true;
        }
        internal void searchPanels()
        {
            if (activityTableSearchControl == null || !isSearchControlVisible)
            {
                activityTableSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
            else
            {
                activityTableSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
            }
        }
        private void rowUpdate()
        {
            db.SubmitChanges();
        }
        internal void setVisibilityForButtons()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isDragVisible = true;
            (Application.Current as App).isSearchVisible = true;
        }
        #endregion

    }
}
