using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.OrdersManagement;
using PhotoForce.WorkflowManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class WorkflowCollectionsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        string selectedGrid = "";
        WorkflowCollectionItem oldCollectionItem;
        public bool isWorkflowCollections;
        #endregion

        #region Properties
        ObservableCollection<WorkflowCollection> _dgWorkflowCollectionsData;
        ObservableCollection<WorkflowCollection> _selectedWorkflowCollections;
        WorkflowCollection _selectedWorkflowCollection;
        ObservableCollection<WorkflowCollectionItem> _dgWorkflowCollectionItemsData;
        ObservableCollection<WorkflowCollectionItem> _selectedWorkflowCollectionItems;
        WorkflowCollectionItem _selectedWorkflowCollectionItem;

        public WorkflowCollectionItem selectedWorkflowCollectionItem
        {
            get { return _selectedWorkflowCollectionItem; }
            set
            {
                if (_selectedWorkflowCollectionItem != null)
                {
                    //check wether user updated sort order
                    //if yes updated on DB aswell
                    if (!_selectedWorkflowCollectionItem.Equals(oldCollectionItem))
                    {
                        updateSortOrder(selectedWorkflowCollectionItem);
                    }
                }
                _selectedWorkflowCollectionItem = value; NotifyPropertyChanged("selectedWorkflowCollectionItem");

                //user can edit sort order , default price from UI  in order to update in DB store selectedWorkflowCollectionItem in a temp object and do a comparison at start
                if (selectedWorkflowCollectionItem != null) { oldCollectionItem = new WorkflowCollectionItem { Id = selectedWorkflowCollectionItem.Id, SortOrder = selectedWorkflowCollectionItem.SortOrder }; }
            }
        }
        public ObservableCollection<WorkflowCollectionItem> selectedWorkflowCollectionItems
        {
            get { return _selectedWorkflowCollectionItems; }
            set { _selectedWorkflowCollectionItems = value; NotifyPropertyChanged("selectedWorkflowCollectionItems"); }
        }
        public ObservableCollection<WorkflowCollectionItem> dgWorkflowCollectionItemsData
        {
            get { return _dgWorkflowCollectionItemsData; }
            set { _dgWorkflowCollectionItemsData = value; NotifyPropertyChanged("dgWorkflowCollectionItemsData"); }
        }
        public WorkflowCollection selectedWorkflowCollection
        {
            get { return _selectedWorkflowCollection; }
            set { _selectedWorkflowCollection = value; NotifyPropertyChanged("selectedWorkflowCollection"); }
        }
        public ObservableCollection<WorkflowCollection> selectedWorkflowCollections
        {
            get { return _selectedWorkflowCollections; }
            set { _selectedWorkflowCollections = value; NotifyPropertyChanged("selectedWorkflowCollections"); }
        }
        public ObservableCollection<WorkflowCollection> dgWorkflowCollectionsData
        {
            get { return _dgWorkflowCollectionsData; }
            set { _dgWorkflowCollectionsData = value; NotifyPropertyChanged("dgWorkflowCollectionsData"); }
        }

        #region Group & Search panels
        bool _workflowCollectionShowGroupPanel;
        bool _workflowCollectionItemsShowGroupPanel;

        public bool workflowCollectionShowGroupPanel
        {
            get { return _workflowCollectionShowGroupPanel; }
            set { _workflowCollectionShowGroupPanel = value; NotifyPropertyChanged(); }
        }
        public bool workflowCollectionItemsShowGroupPanel
        {
            get { return _workflowCollectionItemsShowGroupPanel; }
            set { _workflowCollectionItemsShowGroupPanel = value; NotifyPropertyChanged(); }
        }


        ShowSearchPanelMode _workflowCollectionSearchPanelMode;
        ShowSearchPanelMode _workflowCollectionItemsSearchPanelMode;
        SearchControl _workflowCollectionSearchControl;
        SearchControl _workflowCollectionItemsSearchControl;
        bool _isSearchControlVisible;

        public ShowSearchPanelMode workflowCollectionSearchPanelMode
        {
            get { return _workflowCollectionSearchPanelMode; }
            set { _workflowCollectionSearchPanelMode = value; NotifyPropertyChanged(); }
        }
        public ShowSearchPanelMode workflowCollectionItemsSearchPanelMode
        {
            get { return _workflowCollectionItemsSearchPanelMode; }
            set { _workflowCollectionItemsSearchPanelMode = value; NotifyPropertyChanged(); }
        }
        public SearchControl workflowCollectionSearchControl
        {
            get { return _workflowCollectionSearchControl; }
            set { _workflowCollectionSearchControl = value; NotifyPropertyChanged(); }
        }
        public SearchControl workflowCollectionItemsSearchControl
        {
            get { return _workflowCollectionItemsSearchControl; }
            set { _workflowCollectionItemsSearchControl = value; NotifyPropertyChanged(); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged(); }
        }
        #endregion
        #endregion
        //string _sortOrderHeader;

        //public string sortOrderHeader
        //{
        //    get { return _sortOrderHeader; }
        //    set { _sortOrderHeader = value; NotifyPropertyChanged(); }
        //}
        #region Constructor
        public WorkflowCollectionsViewModel()
        {
            dgWorkflowCollectionsData = new ObservableCollection<WorkflowCollection>(); selectedWorkflowCollections = new ObservableCollection<WorkflowCollection>();
            dgWorkflowCollectionItemsData = new ObservableCollection<WorkflowCollectionItem>(); selectedWorkflowCollectionItems = new ObservableCollection<WorkflowCollectionItem>();

            //bindData(isWorkflowCollections);
        }
        #endregion

        #region Commands
        public RelayCommand WorkflowCollectionsGridDoubleClickCommand
        {
            get
            {
                return new RelayCommand(editWorkflowCollection);
            }
        }
        public RelayCommand WorkflowCollectionsMouseUpCommand
        {
            get
            {
                return new RelayCommand(workflowCollectionsMouseUp);
            }
        }
        public RelayCommand WorkflowCollectionItemsMouseUpCommand
        {
            get
            {
                return new RelayCommand(workflowCollectionItemsMouseUp);
            }
        }
        #endregion

        #region Methods
        internal void bindData(bool isFromWorkflowCollections)
        {
            isWorkflowCollections = isFromWorkflowCollections;
            //if (isWorkflowCollections)
            //    sortOrderHeader = "Sort Order";
            //else
            //    sortOrderHeader = "Qnty";
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            //dgWorkflowCollectionsData = new ObservableCollection<WorkflowCollection>((from wc in db.WorkflowCollections orderby wc.Id select wc).ToList());

            dgWorkflowCollectionsData = new ObservableCollection<WorkflowCollection>(clsWorkflows.getCollectionItems(db,isWorkflowCollections));

            if (dgWorkflowCollectionsData.Count > 0)
            {
                selectedWorkflowCollection = dgWorkflowCollectionsData.First();
                fillWorkflowCollectionItems();
            }
            else
            {
                dgWorkflowCollectionItemsData = new ObservableCollection<WorkflowCollectionItem>();
                selectedWorkflowCollection = null;
            }
        }
        /// <summary>
        /// this method is used to load workflow collection items
        /// </summary>
        private void fillWorkflowCollectionItems()
        {
            if (selectedWorkflowCollection != null)
            {
                dgWorkflowCollectionItemsData = new ObservableCollection<WorkflowCollectionItem>(clsWorkflows.getAllCollectionItems(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedWorkflowCollection.Id));

                if (dgWorkflowCollectionItemsData.Count > 0)
                    selectedWorkflowCollectionItem = dgWorkflowCollectionItemsData.First();
            }
        }
        internal void editWorkflowCollection()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (selectedWorkflowCollection != null)
            {
                CreateNewBatch _objCreateNewBatch = new CreateNewBatch("Collections",selectedWorkflowCollection);
                _objCreateNewBatch.ShowDialog();

                if (((CreateNewBatchViewModel)(_objCreateNewBatch.DataContext)).isSave)
                {
                    bindData(isWorkflowCollections);
                }
            }
        }
        internal void newWorkflowCollection()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (selectedGrid == "WorkflowCollections")
            {
                CreateNewBatch _objCreateNewBatch = new CreateNewBatch("Collections", isWorkflowCollections);
                _objCreateNewBatch.ShowDialog();

                if (((CreateNewBatchViewModel)(_objCreateNewBatch.DataContext)).isSave)
                {
                    bindData(isWorkflowCollections);
                }
            }
        }
        internal void delete()
        {
            List<int> selectedIds = new List<int>();
            if (selectedGrid == "WorkflowCollections")
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string message = "";
                if (selectedWorkflowCollection != null)
                {
                    selectedIds = (from swc in selectedWorkflowCollections select swc.Id).ToList();

                    if (selectedIds.Count == 1)
                    {
                        message = "Are you sure you want to delete a Collection?";
                    }
                    else
                    {
                        message = "Are you sure you want to delete multiple Collections?";
                    }
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgWorkflowCollectionsData.Count();
                        //int deletedRecordsCount = selectedIds.Count;

                        clsWorkflows.deleteWorkflowCollection(db, selectedIds);
                        bindData(isWorkflowCollections);

                        //createDeletedRecordsLogFile("Workflow Collections", totalRecordsCount, deletedRecordsCount);
                    }
                }
            }
            else
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string message = "";
                if (selectedWorkflowCollectionItem != null)
                {
                    selectedIds = (from swc in selectedWorkflowCollectionItems select swc.Id).ToList();

                    if (selectedIds.Count == 1)
                    {
                        message = "Are you sure you want to delete a Collection Item?";
                    }
                    else
                    {
                        message = "Are you sure you want to delete multiple Collection Items?";
                    }
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgWorkflowCollectionItemsData.Count();
                        //int deletedRecordsCount = selectedIds.Count;

                        clsWorkflows.deleteWorkflowCollectionItems(db, selectedIds);
                        fillWorkflowCollectionItems();

                        //createDeletedRecordsLogFile("Workflow Items", totalRecordsCount, deletedRecordsCount);
                    }
                }
            }
        }
        private void workflowCollectionsMouseUp()
        {
            selectedGrid = "WorkflowCollections";
            setButtonVisibilityForCollections();

            fillWorkflowCollectionItems();
        }
        private void workflowCollectionItemsMouseUp()
        {
            selectedGrid = "WorkflowCollectionItems";
            (Application.Current as App).isNewVisible = false;
            (Application.Current as App).isEditVisible = false; 
        }
        internal void addCollectionItems(string isFrom )
        {
            if (selectedWorkflowCollection != null)
            {              

                if (isFrom == "Dashboard")
                {
                    int tempworkflowId = (from ic in db.ItemClassTypes where ic.ClassType == "Equipment" select ic.Id).FirstOrDefault();

                    if (selectedWorkflowCollection.ItemClassTypeId == tempworkflowId) { MVVMMessageService.ShowMessage("Please select workflow collection of workflow type."); return; }
                }
                else if (isFrom == "Equipment")
                {
                    int tempworkflowId = (from ic in db.ItemClassTypes where ic.ClassType == "Workflow" select ic.Id).FirstOrDefault();

                    if (selectedWorkflowCollection.ItemClassTypeId == tempworkflowId) { MVVMMessageService.ShowMessage("Please select workflow collection of equipment type."); return; }
                }
                AddCollectionItems _objAddCollectionItems = new AddCollectionItems(selectedWorkflowCollection.Id, isFrom);
                _objAddCollectionItems.ShowDialog();

                fillWorkflowCollectionItems();
            }
        }

        #region group and search panels
        # region Search panel
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        internal void searchPanels()
        {
            if (selectedGrid == "WorkflowCollections")
            {
                if (workflowCollectionSearchControl == null || isSearchControlVisible == false)
                {
                    workflowCollectionSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    workflowCollectionSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
                }
            }
            else if (selectedGrid == "WorkflowCollectionItems")
            {
                if (workflowCollectionItemsSearchControl == null || isSearchControlVisible == false)
                {
                    workflowCollectionItemsSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    workflowCollectionItemsSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
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
            if (selectedGrid == "WorkflowCollections")
            {
                if (workflowCollectionShowGroupPanel)
                    workflowCollectionShowGroupPanel = false;
                else
                    workflowCollectionShowGroupPanel = true;
            }
            else if (selectedGrid == "WorkflowCollectionItems")
            {
                if (workflowCollectionItemsShowGroupPanel)
                    workflowCollectionItemsShowGroupPanel = false;
                else
                    workflowCollectionItemsShowGroupPanel = true;
            }
        }
        # endregion
        #endregion
        public void updateSortOrder(WorkflowCollectionItem oldCollectionItem)
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (oldCollectionItem.SortOrder == null || oldCollectionItem.SortOrder.ToString() == "")
                return;
            int result = clsWorkflows.upadteWorkflowCollectionItem(db, oldCollectionItem);
            //MVVMMessageService.ShowMessage("Package updated successfully.");
        } 
        #region Buttons visibility
        /// <summary>
        /// this method used for buttons visibility
        /// </summary>
        public void setButtonVisibilityForCollections()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDeleteVisible = true;
            if (isWorkflowCollections)
            {
                (Application.Current as App).isAddCollectionButtonVisible = true;
                (Application.Current as App).isAddEquipmentItemsButtonVisible = false;
            }
            else
            {
                (Application.Current as App).isAddCollectionButtonVisible = false;
                (Application.Current as App).isAddEquipmentItemsButtonVisible = true;
            }
        }
        #endregion
        #endregion
    }
}
