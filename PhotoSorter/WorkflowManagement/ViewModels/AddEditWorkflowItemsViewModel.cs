using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.PhotographyJobManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.WorkflowManagement
{
    public class AddEditWorkflowItemsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        public WorkflowItem _objWorkflowItem;
        WorkflowItem selectedWorkflowItem;
        #endregion
        
        #region Properties
        List<string> _workflowTypes;
        List<string> _workflowStatus;
        string _selectedStatus;
        string _selectedWorkflowType;
        string _description;
        string _assignedTo;
        int _sortOrder;
        List<User> _cbUsersData;
        int _offset;
        bool _beforeDueDateChecked;

        public bool beforeDueDateChecked
        {
            get { return _beforeDueDateChecked; }
            set { _beforeDueDateChecked = value; NotifyPropertyChanged(); }
        }
        public int offset
        {
            get { return _offset; }
            set { _offset = value; NotifyPropertyChanged(); }
        }
        public List<User> cbUsersData
        {
            get { return _cbUsersData; }
            set { _cbUsersData = value; NotifyPropertyChanged(); }
        }
        public int sortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; NotifyPropertyChanged("sortOrder"); }
        }
        public string assignedTo
        {
            get { return _assignedTo; }
            set { _assignedTo = value; NotifyPropertyChanged("assignedTo"); }
        }
        public string description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("description"); }
        }
        public string selectedWorkflowType
        {
            get { return _selectedWorkflowType; }
            set { _selectedWorkflowType = value; NotifyPropertyChanged("selectedWorkflowType"); }
        }
        public string selectedStatus
        {
            get { return _selectedStatus; }
            set { _selectedStatus = value; NotifyPropertyChanged("selectedStatus"); }
        }
        public List<string> workflowTypes
        {
            get { return _workflowTypes; }
            set { _workflowTypes = value; NotifyPropertyChanged("workflowTypes"); }
        }
        public List<string> workflowStatus
        {
            get { return _workflowStatus; }
            set { _workflowStatus = value; NotifyPropertyChanged("workflowStatus"); }
        }
        #endregion

        #region Constructors
        public AddEditWorkflowItemsViewModel()
        {
            cbUsersData = new List<User>();
            workflowTypes = new List<string>(); workflowStatus = new List<string>();
            workflowTypes.Add("Workflow"); workflowTypes.Add("Deliverable"); workflowTypes.Add("Service Item");
            workflowStatus.Add("Not Started"); workflowStatus.Add("Started"); workflowStatus.Add("Done");
            beforeDueDateChecked = true;

            bindData();
            selectedStatus = "Not Started";
        }
        public AddEditWorkflowItemsViewModel(WorkflowItem tempWorkflowItem)
            : this()
        {
            selectedWorkflowItem = tempWorkflowItem;
            selectedWorkflowType = selectedWorkflowItem.Type;
            assignedTo = selectedWorkflowItem.Assignedto;
            selectedStatus = selectedWorkflowItem.Status;
            description = selectedWorkflowItem.Description;
            sortOrder = selectedWorkflowItem.SortOrder;
            offset = selectedWorkflowItem.Offset == null ? 0 : (int)selectedWorkflowItem.Offset;
            beforeDueDateChecked = selectedWorkflowItem.BeforeAfter == null ? false : (bool)selectedWorkflowItem.BeforeAfter;
        }
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(saveAndClose);
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

        private void bindData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            cbUsersData = clsUsers.getAllUsers(db);

            if (selectedWorkflowItem == null)
            {
                sortOrder = clsWorkflows.getSortOrder(db) + 1;
            }
        }
        private void saveAndClose()
        {
            try
            {
                #region creating new workflow
                if (!string.IsNullOrEmpty(selectedStatus) && !string.IsNullOrEmpty(selectedWorkflowType))
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                    //Editing a workflow item
                    if (selectedWorkflowItem != null)
                    {
                        _objWorkflowItem = (from wi in db.WorkflowItems where wi.Id == selectedWorkflowItem.Id select wi).FirstOrDefault();

                        _objWorkflowItem.Type = selectedWorkflowType;
                        _objWorkflowItem.Assignedto = assignedTo;
                        _objWorkflowItem.Status = selectedStatus;
                        _objWorkflowItem.Description = description;
                        _objWorkflowItem.SortOrder = sortOrder;
                        _objWorkflowItem.Offset = offset;
                        _objWorkflowItem.BeforeAfter = beforeDueDateChecked;
                        //_objWorkflowItem.ItemClassTypeId = (from ic in db.ItemClassTypes where ic.ClassType == "Workflow" select ic.Id).FirstOrDefault();

                        if (_objWorkflowItem != null)
                        {
                            db.SubmitChanges();
                            isSave = true;
                            DialogResult = false;
                        }
                    }
                    else
                    {
                        //if (offset == 0) { MVVMMessageService.ShowMessage("Please enter a offset value."); return; }

                        #region Creating WorkflowItem
                        _objWorkflowItem = new WorkflowItem();
                        _objWorkflowItem.Description = description;
                        _objWorkflowItem.Type = selectedWorkflowType;
                        _objWorkflowItem.Assignedto = assignedTo;
                        _objWorkflowItem.Status = selectedStatus;
                        _objWorkflowItem.SortOrder = sortOrder;
                        _objWorkflowItem.Offset = offset;
                        _objWorkflowItem.BeforeAfter = beforeDueDateChecked;
                        _objWorkflowItem.ItemClassTypeId = (from ic in db.ItemClassTypes where ic.ClassType == "Workflow" select ic.Id).FirstOrDefault();

                        if (_objWorkflowItem != null)
                        {
                            db.WorkflowItems.InsertOnSubmit(_objWorkflowItem);
                            db.SubmitChanges();
                        }
                        #endregion

                        isSave = true;
                        DialogResult = false;
                    }
                }
                else
                {
                    isSave = false;
                    if (string.IsNullOrEmpty(selectedStatus))
                        MVVMMessageService.ShowMessage("Please select workflow status.");
                    else if (string.IsNullOrEmpty(selectedWorkflowType))
                        MVVMMessageService.ShowMessage("Please select workflow type.");

                }
                #endregion
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
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
