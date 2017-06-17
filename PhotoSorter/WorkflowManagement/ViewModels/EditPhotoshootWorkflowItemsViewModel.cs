using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.WorkflowManagement
{
    public class EditPhotoshootWorkflowItemsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        int schoolId = 0;
        public PhotoshootWorkflowItem addEditWorkflowItem;
        PhotoshootWorkflowItem tempWorkflowItem;
        ObservableCollection<PhotoshootWorkflowItem> tempWorkflowItems;
        #endregion

        #region Properties
        string _selectedStatus;
        private List<string> _workflowStatus;
        string _assignedTo;
        private DateTime? _dueDate;
        List<User> _cbUsersData;
        //private DateTime? _completedBy;
        private DateTime? _completedOn;
        string _completedBy;
        List<User> _cbCompletedByData;

        public List<User> cbCompletedByData
        {
            get { return _cbCompletedByData; }
            set { _cbCompletedByData = value; NotifyPropertyChanged(); }
        }
        public DateTime? completedOn
        {
            get { return _completedOn; }
            set { _completedOn = value; NotifyPropertyChanged(); }
        }
        public string completedBy
        {
            get { return _completedBy; }
            set { _completedBy = value; NotifyPropertyChanged(); }
        }
        //int _offSet;
        //bool _beforeDueDateChecked;

        //public bool beforeDueDateChecked
        //{
        //    get { return _beforeDueDateChecked; }
        //    set { _beforeDueDateChecked = value; NotifyPropertyChanged(); }
        //}
        //public int offSet
        //{
        //    get { return _offSet; }
        //    set { _offSet = value; NotifyPropertyChanged(); }
        //}
        public List<User> cbUsersData
        {
            get { return _cbUsersData; }
            set { _cbUsersData = value; NotifyPropertyChanged(); }
        }
        public List<string> workflowStatus
        {
            get { return _workflowStatus; }
            set { _workflowStatus = value; NotifyPropertyChanged("workflowStatus"); }
        }
        public DateTime? dueDate
        {
            get { return _dueDate; }
            set { _dueDate = value; NotifyPropertyChanged("dueDate"); }
        }
        public string assignedTo
        {
            get { return _assignedTo; }
            set { _assignedTo = value; NotifyPropertyChanged("assignedTo"); }
        }
        public string selectedStatus
        {
            get { return _selectedStatus; }
            set { _selectedStatus = value; NotifyPropertyChanged("selectedStatus"); }
        }
        #endregion

        #region constructors
        public EditPhotoshootWorkflowItemsViewModel(ObservableCollection<PhotoshootWorkflowItem> selectedWorkflowItems)
        {
            workflowStatus = new List<string>(); cbUsersData = new List<User>(); cbCompletedByData = new List<User>();
            workflowStatus.Add("Not Started"); workflowStatus.Add("Started"); workflowStatus.Add("Completed");
            cbUsersData = clsUsers.getAllUsers(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            cbCompletedByData = clsUsers.getAllUsers(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            dueDate = null;
            completedBy = null;
            completedOn = null;
            tempWorkflowItem = null;
            tempWorkflowItems = selectedWorkflowItems;
        }
        //edit photoshoot workflow item (Dashboard->Photoshoot->workflows->edit)
        public EditPhotoshootWorkflowItemsViewModel(PhotoshootWorkflowItem selectedWorkflow)
        {
            workflowStatus = new List<string>(); cbUsersData = new List<User>(); cbCompletedByData = new List<User>();
            workflowStatus.Add("Not Started"); workflowStatus.Add("Started"); workflowStatus.Add("Completed");

            cbUsersData = clsUsers.getAllUsers(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            cbCompletedByData = clsUsers.getAllUsers(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            schoolId = clsSchool.defaultSchoolId;
            tempWorkflowItem = selectedWorkflow;
            assignedTo = selectedWorkflow.Assignedto;
            selectedStatus = selectedWorkflow.Status;
            dueDate = selectedWorkflow.DueDate;
            completedBy = selectedWorkflow.CompletedBy ;
            completedOn = selectedWorkflow.CompletedOn ;
        }
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(editWorkflowItem);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        #endregion

        #region Methods
        private void editWorkflowItem()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            ArrayList photoShootWorkflowItemIds = new ArrayList();
            try
            {
                if (tempWorkflowItem != null)
                {
                    addEditWorkflowItem = (from wi in db.PhotoshootWorkflowItems where wi.Id == tempWorkflowItem.Id select wi).FirstOrDefault();
                    PhotoShoot tempPhotoShoot = (from ps in db.PhotoShoots where ps.PhotoShotID == addEditWorkflowItem.PhotoShootID select ps).FirstOrDefault();
                    addEditWorkflowItem.Assignedto = assignedTo;
                    addEditWorkflowItem.Status = selectedStatus;

                    //if (beforeDueDateChecked)
                    //    addEditWorkflowItem.DueDate = (tempPhotoShoot.PhotoShotDate).Value.AddDays(-offSet);
                    //else
                    addEditWorkflowItem.DueDate = dueDate;
                    addEditWorkflowItem.CompletedBy = completedBy;
                    addEditWorkflowItem.CompletedOn = completedOn;

                    if (addEditWorkflowItem != null)
                    {
                        db.SubmitChanges();
                        isSave = true;
                        DialogResult = false;
                    }
                }
                else
                {
                    int result = 0;
                    foreach (PhotoshootWorkflowItem pwi in tempWorkflowItems)
                    {
                        photoShootWorkflowItemIds.Add(pwi.Id);
                    }
                    if (!string.IsNullOrEmpty(assignedTo))
                    {
                        result = clsWorkflows.updateAllPhotoShootWorkflowItemsAssignedTo(db, photoShootWorkflowItemIds, assignedTo);
                    }
                    if (!string.IsNullOrEmpty(selectedStatus))
                    {
                        result = clsWorkflows.updateAllPhotoShootWorkflowItemsStatus(db, photoShootWorkflowItemIds, selectedStatus);
                    }
                    if (dueDate != null)
                    {
                        result = clsWorkflows.updateAllPhotoShootWorkflowItemsdueDate(db, photoShootWorkflowItemIds, (Convert.ToDateTime(dueDate)).ToString("yyyy-MM-dd"));
                    }
                    if (completedOn != null)
                    {
                        result = clsWorkflows.updateAllPhotoShootWorkflowItemsCompletedOn(db, photoShootWorkflowItemIds, (Convert.ToDateTime(completedOn)).ToString("yyyy-MM-dd"));
                    }
                    if (!string.IsNullOrEmpty(completedBy))
                    {
                        result = clsWorkflows.updateAllPhotoShootWorkflowItemsCompletedBy(db, photoShootWorkflowItemIds, completedBy);
                    }
                    if (result != 0)
                    {
                        isSave = true;
                        DialogResult = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isSave = false;
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
