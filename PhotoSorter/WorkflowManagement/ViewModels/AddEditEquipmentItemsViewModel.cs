using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkflowManagement
{
    public class AddEditEquipmentItemsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        public WorkflowItem _objWorkflowItem;
        WorkflowItem selectedWorkflowItem;
        PhotoshootWorkflowItem tempSelectedPSWorkflowItem;
        public PhotoshootWorkflowItem _objPhotoshootWorkflowItem; 
        string isFrom = "";
        bool isSaveAndNew = false;
        ArrayList tempEquipmentIds = new ArrayList();
        #endregion
        
        #region Properties
        string _equipmentNotes;
        string _description;
        string _assignedTo;
        int _quantity;
        List<User> _cbUsersData;
        int _offset;
        bool _beforeDueDateChecked;
        bool _isDescriptionEnabled;
        Visibility _isSaveAndNewBtnVisibility;
        int? _sortOrder;

        public int? sortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; NotifyPropertyChanged(); }
        }
        public Visibility isSaveAndNewBtnVisibility
        {
            get { return _isSaveAndNewBtnVisibility; }
            set { _isSaveAndNewBtnVisibility = value; NotifyPropertyChanged(); }
        }
        public bool isDescriptionEnabled
        {
            get { return _isDescriptionEnabled; }
            set { _isDescriptionEnabled = value; NotifyPropertyChanged(); }
        }
        public List<User> cbUsersData
        {
            get { return _cbUsersData; }
            set { _cbUsersData = value; NotifyPropertyChanged(); }
        }
        public int quantity
        {
            get { return _quantity; }
            set { _quantity = value; NotifyPropertyChanged("quantity"); }
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
        public string equipmentNotes
        {
            get { return _equipmentNotes; }
            set { _equipmentNotes = value; NotifyPropertyChanged("equipmentNotes"); }
        }
        #endregion

        #region Constructors
        public AddEditEquipmentItemsViewModel()
        {
            isSaveAndNew = false;
            sortOrder = 0;
            isDescriptionEnabled = true;
            isSaveAndNewBtnVisibility = Visibility.Visible;
            isFrom = "";
            cbUsersData = new List<User>();
            bindData();
        }
        //Edit Equipment Item
        public AddEditEquipmentItemsViewModel(WorkflowItem tempWorkflowItem)
            : this()
        {
            isSaveAndNew = false;
            isSaveAndNewBtnVisibility = Visibility.Collapsed;
            isDescriptionEnabled = true;
            isFrom = "";
            selectedWorkflowItem = tempWorkflowItem;
            assignedTo = selectedWorkflowItem.Assignedto;
            description = selectedWorkflowItem.Description;
            quantity = (int)selectedWorkflowItem.Quantity;
            equipmentNotes = selectedWorkflowItem.Notes;
            sortOrder = selectedWorkflowItem.SortOrder;
        }
        //Edit PhotoShoot Equipment Item
        public AddEditEquipmentItemsViewModel(PhotoshootWorkflowItem tempWorkflowItem)
            : this()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            isSaveAndNew = false;
            isSaveAndNewBtnVisibility = Visibility.Collapsed;
            isDescriptionEnabled = false;
            isFrom = "PhotoshootWorkflow";
            tempSelectedPSWorkflowItem = tempWorkflowItem;
            description = (from wi in db.WorkflowItems where wi.Id == tempWorkflowItem.WorkflowItemId select wi.Description).FirstOrDefault();
            assignedTo = tempWorkflowItem.Assignedto;
            quantity = Convert.ToInt32(tempWorkflowItem.Quantity);
            equipmentNotes = tempWorkflowItem.Notes;
            sortOrder = tempWorkflowItem.SortOrder != null? tempWorkflowItem.SortOrder : 0;
        }
        //Bulk edit EquipmentItems
        public AddEditEquipmentItemsViewModel(ArrayList SelectedIds)
            : this()
        {
            tempEquipmentIds = SelectedIds;
            isSaveAndNew = false;
            isSaveAndNewBtnVisibility = Visibility.Collapsed;
            isDescriptionEnabled = true;
            isFrom = "BulkEdit";
            //selectedWorkflowItem = tempWorkflowItem;
            assignedTo = "";
            description = "";
            quantity = 0;
            equipmentNotes = "";
            sortOrder = 0;
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
        public RelayCommand SaveAndNewCommand
        {
            get
            {
                return new RelayCommand(saveAndNew);
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
        }
        private void saveAndNew()
        {
            if (!string.IsNullOrEmpty(description))
            {
                isSaveAndNew = true;
                saveAndClose();

                description = "";
                equipmentNotes = "";
                quantity = 0;
                assignedTo = null;
                sortOrder = null;
            }

        }
        private void saveAndClose()
        {
            try
            {
                #region creating new workflow
                if (!string.IsNullOrEmpty(description) || isFrom == "PhotoshootWorkflow" || isFrom == "BulkEdit")
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    if (isFrom == "BulkEdit")
                    {
                        try
                        {
                            foreach (int equipmentId in tempEquipmentIds)
                            {
                                _objWorkflowItem = (from wi in db.WorkflowItems where wi.Id == equipmentId select wi).FirstOrDefault();
                                if (!string.IsNullOrEmpty(assignedTo))
                                    _objWorkflowItem.Assignedto = assignedTo;
                                if (!string.IsNullOrEmpty(description))
                                _objWorkflowItem.Description = description;
                                if (!string.IsNullOrEmpty(equipmentNotes))
                                    _objWorkflowItem.Notes = equipmentNotes;
                                if(quantity > 0)
                                    _objWorkflowItem.Quantity = quantity;
                                if (sortOrder!= null && sortOrder > 0)
                                    _objWorkflowItem.SortOrder = (int)sortOrder;

                                if (_objWorkflowItem != null)
                                {
                                    db.SubmitChanges();
                                }
                            }
                            isSave = true;
                            DialogResult = false;
                        }
                        catch (Exception ex)
                        {
                            isSave = false;
                            DialogResult = false;
                            MVVMMessageService.ShowMessage(ex.Message);
                        }
                    }
                    #region Add Edit EquipmentItem
                    else if (isFrom != "PhotoshootWorkflow")
                    {
                        //Editing a workflow item
                        if (selectedWorkflowItem != null)
                        {
                            _objWorkflowItem = (from wi in db.WorkflowItems where wi.Id == selectedWorkflowItem.Id select wi).FirstOrDefault();

                            _objWorkflowItem.Assignedto = assignedTo;
                            _objWorkflowItem.Description = description;
                            _objWorkflowItem.SortOrder = sortOrder != null ? (int)sortOrder : 0;
                            _objWorkflowItem.Notes = equipmentNotes;
                            _objWorkflowItem.Quantity = quantity;

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
                            _objWorkflowItem.Assignedto = assignedTo;
                            _objWorkflowItem.Quantity = quantity;
                            _objWorkflowItem.Notes = equipmentNotes;                            
                            _objWorkflowItem.SortOrder = sortOrder != null ? (int)sortOrder : 0;
                            _objWorkflowItem.ItemClassTypeId = (from ic in db.ItemClassTypes where ic.ClassType == "Equipment" select ic.Id).FirstOrDefault();

                            if (_objWorkflowItem != null)
                            {
                                db.WorkflowItems.InsertOnSubmit(_objWorkflowItem);
                                db.SubmitChanges();
                            }
                            #endregion

                            if (!isSaveAndNew)
                            {
                                isSave = true;
                                DialogResult = false;
                            }
                            isSaveAndNew = false;
                        }
                    }
                    #endregion

                    #region Edit Photoshoot equipmentItem
                    else if (tempSelectedPSWorkflowItem != null )
                    {
                        #region Creating WorkflowItem
                        _objPhotoshootWorkflowItem = new PhotoshootWorkflowItem();
                        _objPhotoshootWorkflowItem = (from pwi in db.PhotoshootWorkflowItems where pwi.Id == tempSelectedPSWorkflowItem.Id select pwi).FirstOrDefault();
                        _objPhotoshootWorkflowItem.Assignedto = assignedTo;
                        _objPhotoshootWorkflowItem.SortOrder = sortOrder;
                        _objPhotoshootWorkflowItem.Notes = equipmentNotes;
                        _objPhotoshootWorkflowItem.Quantity = quantity;
                        //_objPhotoshootWorkflowItem.ItemClassTypeId = (from ic in db.ItemClassTypes where ic.ClassType == "Equipment" select ic.Id).FirstOrDefault();

                        if (_objPhotoshootWorkflowItem != null)
                        {                            
                            db.SubmitChanges();

                            isSave = true;
                            DialogResult = false;
                        }
                        #endregion

                    }
                    #endregion
                }
                else
                {
                    isSave = false;
                    //if (string.IsNullOrEmpty(assignedTo))
                    //    MVVMMessageService.ShowMessage("Please select workflow status.");
                    //else if (string.IsNullOrEmpty(equipmentNotes))
                    //    MVVMMessageService.ShowMessage("Please select workflow type.");

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
