using PhotoForce.App_Code;
using PhotoForce.Extensions;
using PhotoForce.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkflowManagement
{
    public class AddCollectionItemsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        string selectedGridName = "";
        int workflowCollectionId;
        int photoshootId = 0;
        List<WorkflowItem> tempDgRightWorkflowData = new List<WorkflowItem>();
        string isFromTab = "";
        #endregion

        #region Properties
        string _availableRightItems;   // Available Workflow Items; Selected Workflow Items
        string _availableLeftItems;

        public string availableLeftItems
        {
            get { return _availableLeftItems; }
            set { _availableLeftItems = value; NotifyPropertyChanged(); }
        }
        public string availableRightItems
        {
            get { return _availableRightItems; }
            set { _availableRightItems = value; NotifyPropertyChanged(); }
        }

        #region Left Panel
        ObservableCollection<WorkflowItem> _dgLeftWorkflowsData;
        ObservableCollection<WorkflowItem> _leftSelectedWorkflows;
        WorkflowItem _leftSelectedWorkflow;

        public WorkflowItem leftSelectedWorkflow
        {
            get { return _leftSelectedWorkflow; }
            set { _leftSelectedWorkflow = value; NotifyPropertyChanged("leftSelectedWorkflow"); }
        }
        public ObservableCollection<WorkflowItem> leftSelectedWorkflows
        {
            get { return _leftSelectedWorkflows; }
            set { _leftSelectedWorkflows = value; NotifyPropertyChanged("leftSelectedWorkflows"); }
        }
        public ObservableCollection<WorkflowItem> dgLeftWorkflowsData
        {
            get { return _dgLeftWorkflowsData; }
            set { _dgLeftWorkflowsData = value; NotifyPropertyChanged("dgLeftWorkflowsData"); }
        }
        #endregion

        #region Right panel
        ObservableCollection<WorkflowItem> _dgRightWorkflowsData;
        ObservableCollection<WorkflowItem> _rightSelectedWorkflows;
        WorkflowItem _rightSelectedWorkflow;

        public WorkflowItem rightSelectedWorkflow
        {
            get { return _rightSelectedWorkflow; }
            set { _rightSelectedWorkflow = value; NotifyPropertyChanged("rightSelectedWorkflow"); }
        }
        public ObservableCollection<WorkflowItem> rightSelectedWorkflows
        {
            get { return _rightSelectedWorkflows; }
            set { _rightSelectedWorkflows = value; NotifyPropertyChanged("rightSelectedWorkflows"); }
        }
        public ObservableCollection<WorkflowItem> dgRightWorkflowsData
        {
            get { return _dgRightWorkflowsData; }
            set { _dgRightWorkflowsData = value; NotifyPropertyChanged("dgRightWorkflowsData"); }
        }
        #endregion

        #region Collections
        //ObservableCollection<WorkflowCollection> _collectionsData;
        //WorkflowCollection _selectedCollection;
        Visibility _isCollectionsVisible;

        public Visibility isCollectionsVisible
        {
            get { return _isCollectionsVisible; }
            set { _isCollectionsVisible = value; NotifyPropertyChanged("isCollectionsVisible"); }
        }
        //public WorkflowCollection selectedCollection
        //{
        //    get { return _selectedCollection; }
        //    set
        //    {
        //        _selectedCollection = value; NotifyPropertyChanged("selectedCollection");

        //        if (selectedCollection != null)
        //        {
        //            dgRightWorkflowsData = new ObservableCollection<WorkflowItem>(clsWorkflows.getAllWorkflowItems(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedCollection.Id));
        //        }
        //    }
        //}
        //public ObservableCollection<WorkflowCollection> collectionsData
        //{
        //    get { return _collectionsData; }
        //    set { _collectionsData = value; NotifyPropertyChanged("collectionsData"); }
        //}
        #endregion

        #endregion
        //string _sortOrderHeader;

        //public string sortOrderHeader
        //{
        //    get { return _sortOrderHeader; }
        //    set { _sortOrderHeader = value; NotifyPropertyChanged(); }
        //}
        Visibility _isQuantityVisible;

        public Visibility isQuantityVisible
        {
            get { return _isQuantityVisible; }
            set { _isQuantityVisible = value; NotifyPropertyChanged(); }
        }

        #region Constructor
        public AddCollectionItemsViewModel(int tempWorkflowCollectionId, string classType )
        {
            selectedGridName = ""; workflowCollectionId = tempWorkflowCollectionId;
            isCollectionsVisible = Visibility.Collapsed;
            bindData();
            if (classType == "Dashboard")
            {
                isQuantityVisible = Visibility.Hidden;
                isFromTab = "Workflow";
                availableLeftItems = "Available Workflow Items";
                availableRightItems = "Selected Workflow Items";
                tempDgRightWorkflowData = clsWorkflows.getAllWorkflowItems(db, workflowCollectionId);
            }
            else
            {
                isQuantityVisible = Visibility.Visible;
                isFromTab = "Equipment";
                availableLeftItems = "Available Equipment Items";
                availableRightItems = "Selected Equipment Items";
                tempDgRightWorkflowData = clsWorkflows.getAllEquipmentItems(db, workflowCollectionId);
            }
            dgRightWorkflowsData = new ObservableCollection<WorkflowItem>(tempDgRightWorkflowData);
            
            int tempworkflowId = (from ic in db.ItemClassTypes where ic.ClassType == isFromTab select ic.Id).FirstOrDefault();

            dgLeftWorkflowsData = new ObservableCollection<WorkflowItem>((from wi in db.WorkflowItems orderby wi.Id where !dgRightWorkflowsData.Contains(wi) && wi.ItemClassTypeId == tempworkflowId select wi).ToList());

        }
        public AddCollectionItemsViewModel(string isFrom, int tempPhotoshootId)
        {
            selectedGridName = isFrom;

            isCollectionsVisible = Visibility.Visible;
            photoshootId = tempPhotoshootId;
            bindData();

            
            if (isFrom == "Dashboard")
            {
                isQuantityVisible = Visibility.Hidden;
                availableLeftItems = "Available Workflow Items";
                availableRightItems = "Selected Workflow Items";
                tempDgRightWorkflowData = clsWorkflows.getWorkflowItemsWithPhotoshoot(db, photoshootId, "Workflow");

                dgRightWorkflowsData = new ObservableCollection<WorkflowItem>(tempDgRightWorkflowData);

                int tempworkflowId = (from ic in db.ItemClassTypes where ic.ClassType == "Workflow" select ic.Id).FirstOrDefault();
                dgLeftWorkflowsData = new ObservableCollection<WorkflowItem>((from wi in db.WorkflowItems orderby wi.Id where !dgRightWorkflowsData.Contains(wi) && wi.ItemClassTypeId == tempworkflowId select wi).ToList());
            }
            else
            {
                isQuantityVisible = Visibility.Visible;
                isCollectionsVisible = Visibility.Visible;
                availableLeftItems = "Available Equipment Items";
                availableRightItems = "Selected Equipment Items";
                tempDgRightWorkflowData = clsWorkflows.getWorkflowItemsWithPhotoshoot(db, photoshootId, "Equipment");

                dgRightWorkflowsData = new ObservableCollection<WorkflowItem>(tempDgRightWorkflowData);
                int tempEquipmentId = (from ic in db.ItemClassTypes where ic.ClassType == "Equipment" select ic.Id).FirstOrDefault();
                dgLeftWorkflowsData = new ObservableCollection<WorkflowItem>((from wi in db.WorkflowItems orderby wi.Id where !dgRightWorkflowsData.Contains(wi) && wi.ItemClassTypeId == tempEquipmentId select wi).ToList());
            }
            // collectionsData = new ObservableCollection<WorkflowCollection>((from cd in db.WorkflowCollections orderby cd.Id select cd).ToList());
        }
        #endregion

        #region Commands
        public RelayCommand MoveSelectedWorkflowsCommand
        {
            get
            {
                return new RelayCommand(moveSelectedWorkflows);
            }
        }
        public RelayCommand DeleteselectedWorkflowCommand
        {
            get
            {
                return new RelayCommand(deleteselectedWorkflow);
            }
        }
        //public RelayCommand WorkflowsLeftClickCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(loadData);
        //    }
        //}
        public RelayCommand WorkflowsDoubleClickCommand
        {
            get
            {
                return new RelayCommand(moveSelectedWorkflows);
            }
        }
        //public RelayCommand WorkflowsCollectionLeftClickCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(loadData);
        //    }
        //}
        public RelayCommand AddAndCloseCommand
        {
            get
            {
                return new RelayCommand(addAndClose);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand ManageCollectionsCommand
        {
            get
            {
                return new RelayCommand(manageCollections);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// intializing the Lists
        /// </summary>
        private void bindData()
        {            
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            dgLeftWorkflowsData = new ObservableCollection<WorkflowItem>(); leftSelectedWorkflows = new ObservableCollection<WorkflowItem>();
            dgRightWorkflowsData = new ObservableCollection<WorkflowItem>(); rightSelectedWorkflows = new ObservableCollection<WorkflowItem>();
            //collectionsData = new ObservableCollection<WorkflowCollection>();
        }
        private void addAndClose()
        {
            try
            {
                if (workflowCollectionId != 0)
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                    //Delete the items which are removed by the user .
                    List<int> tempData = (from trw in tempDgRightWorkflowData where !dgRightWorkflowsData.Contains(trw) select trw.Id).ToList();
                    if (tempData.Count > 0)
                    {
                        clsWorkflows.deleteCollectionItemsById(db, tempData, workflowCollectionId);
                    }

                    //Adding the items which are not there in the list , skip if already exists
                    foreach (WorkflowItem tempWorkflowItem in dgRightWorkflowsData)
                    {
                        if (tempDgRightWorkflowData.Count == 0 || !tempDgRightWorkflowData.Contains(tempWorkflowItem))
                        {
                            WorkflowCollectionItem _objWorkflowCollectionItem = new WorkflowCollectionItem();
                            _objWorkflowCollectionItem.WorkflowCollectionId = workflowCollectionId;
                            _objWorkflowCollectionItem.WorkflowItemId = tempWorkflowItem.Id;
                            _objWorkflowCollectionItem.SortOrder = tempWorkflowItem.SortOrder;
                            _objWorkflowCollectionItem.Quantity = tempWorkflowItem.Quantity;
                            db.WorkflowCollectionItems.InsertOnSubmit(_objWorkflowCollectionItem);
                            db.SubmitChanges();
                        }
                    }
                }
                //photoshoot id not equals zero means ,it came from dashboard and have to attach work flow items to photoshoot
                else if (photoshootId != 0)
                {
                    #region Creating PhotoForceWorkflowItem
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                    //Delete the items which are removed by the user .
                    List<int> tempData = (from trw in tempDgRightWorkflowData where !dgRightWorkflowsData.Contains(trw) select trw.Id).ToList();
                    if (tempData.Count > 0)
                    {
                        clsWorkflows.removePhotoshootWorkflows(db, tempData, photoshootId);
                    }

                    //Adding the items which are not there in the list , skip if already exists
                    foreach (WorkflowItem tempWorkflowItem in dgRightWorkflowsData)
                    {
                        if (tempDgRightWorkflowData.Count == 0 || !tempDgRightWorkflowData.Contains(tempWorkflowItem))
                        {
                            PhotoshootWorkflowItem _objPhotoshootWorkflowItem = new PhotoshootWorkflowItem();

                            _objPhotoshootWorkflowItem.CreatedBy = clsStatic.userName;
                            _objPhotoshootWorkflowItem.CreatedOn = DateTime.Now;
                            _objPhotoshootWorkflowItem.PhotoShootID = photoshootId;
                            _objPhotoshootWorkflowItem.WorkflowItemId = tempWorkflowItem.Id;
                            if (selectedGridName == "Dashboard")
                            {
                                _objPhotoshootWorkflowItem.SortOrder = tempWorkflowItem.SortOrder;
                                _objPhotoshootWorkflowItem.Status = "Not Started";
                                _objPhotoshootWorkflowItem.Quantity = tempWorkflowItem.Quantity;
                            }
                            if (_objPhotoshootWorkflowItem != null)
                            {
                                _objPhotoshootWorkflowItem.SortOrder = tempWorkflowItem.SortOrder;
                                _objPhotoshootWorkflowItem.Quantity = tempWorkflowItem.Quantity;
                                db.PhotoshootWorkflowItems.InsertOnSubmit(_objPhotoshootWorkflowItem);
                                db.SubmitChanges();
                            }

                            if (tempWorkflowItem.BeforeAfter != null)
                            {
                                double temp = (double)tempWorkflowItem.Offset;

                                if ((bool)tempWorkflowItem.BeforeAfter)
                                {
                                    _objPhotoshootWorkflowItem.DueDate = _objPhotoshootWorkflowItem.PhotoShoot.PhotoShotDate.Value.AddDays(-temp);
                                }
                                else
                                {
                                    _objPhotoshootWorkflowItem.DueDate = _objPhotoshootWorkflowItem.PhotoShoot.PhotoShotDate.Value.AddDays(temp);
                                }

                                db.SubmitChanges();
                            }
                        }
                    }
                    #endregion
                }
                isSave = true;
                DialogResult = false;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method used to add left side items to right side
        /// </summary>
        private void moveSelectedWorkflows()
        {
            if (leftSelectedWorkflows.Count > 0)
            {
                List<int> rightWorkflowIds = (from lsw in dgRightWorkflowsData select lsw.Id).ToList();
                List<WorkflowItem> tempWorkItems = (from lsw in leftSelectedWorkflows where !rightWorkflowIds.Contains(lsw.Id) select lsw).ToList();

                foreach (WorkflowItem workItem in tempWorkItems)
                {
                    dgRightWorkflowsData.Add(workItem);
                    dgLeftWorkflowsData.Remove(workItem);
                }
            }
        }

        /// <summary>
        /// This method used to remove workflow items
        ///we are deleting from DB just removing from the list and when user clicks on Add and Close we will do delete and add 
        /// </summary>
        private void deleteselectedWorkflow()
        {
            if (rightSelectedWorkflows.Count > 0)
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                ArrayList workflowIds = new ArrayList();

                foreach (WorkflowItem wi in rightSelectedWorkflows)
                    workflowIds.Add(wi.Id);

                foreach (int id in workflowIds)
                {
                    WorkflowItem temp = (from wi in dgRightWorkflowsData where wi.Id == id select wi).FirstOrDefault();
                    dgRightWorkflowsData.Remove(temp);
                    if (!dgLeftWorkflowsData.Contains(temp))
                    {
                        dgLeftWorkflowsData.Add(temp);
                    }
                }
            }
        }

        private void manageCollections()
        {
            ObservableCollection<WorkflowItem> _objtempWorkflowItemData = new ObservableCollection<WorkflowItem>();
            List<int> itemIds = new List<int>();

            AddRemoveCollectionItems _objAddRemoveCollectionItems = new AddRemoveCollectionItems(selectedGridName);
            _objAddRemoveCollectionItems.ShowDialog();

            if (((AddRemoveCollectionItemsViewModel)(_objAddRemoveCollectionItems.DataContext)).isSave)
            {
                WorkflowCollection tempItem = ((AddRemoveCollectionItemsViewModel)(_objAddRemoveCollectionItems.DataContext)).selectedItem;
                List<WorkflowItem> tempWorkflow = clsWorkflows.getAllWorkflowItems(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempItem.Id);

                string selectedOption = ((AddRemoveCollectionItemsViewModel)(_objAddRemoveCollectionItems.DataContext)).selectedOption;

                if (selectedOption == AddOrReplaceCollectionItems.RemoveItems.ToString())
                {
                    dgRightWorkflowsData = new ObservableCollection<WorkflowItem>(tempWorkflow);

                    _objtempWorkflowItemData = new ObservableCollection<WorkflowItem>((from wi in db.WorkflowItems orderby wi.Id where !dgRightWorkflowsData.Contains(wi) select wi).ToList());

                    if (selectedGridName == "Dashboard")
                    {
                        itemIds = (from wi in db.WorkflowItems where wi.ItemClassTypeId == 1 select wi.Id).ToList();
                    }
                    else
                    {
                        itemIds = (from wi in db.WorkflowItems where wi.ItemClassTypeId == 2 select wi.Id).ToList();
                    }

                    dgLeftWorkflowsData = new ObservableCollection<WorkflowItem>((from wi in _objtempWorkflowItemData orderby wi.Id where !dgRightWorkflowsData.Contains(wi) && itemIds.Contains(wi.Id) select wi).ToList());
                }

                List<int> rightWorkflowIds = (from lsw in dgRightWorkflowsData select lsw.Id).ToList();
                List<WorkflowItem> tempWorkItemsData = (from twd in tempWorkflow where !rightWorkflowIds.Contains(twd.Id) select twd).ToList();

                tempWorkItemsData.ForEach(x => dgRightWorkflowsData.Add(x));

                foreach (WorkflowItem workItem in dgRightWorkflowsData)
                {
                    dgLeftWorkflowsData.Remove((from di in dgLeftWorkflowsData where di.Id == workItem.Id select di).FirstOrDefault());
                }
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
