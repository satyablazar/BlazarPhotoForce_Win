using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.WorkflowManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class WorkflowItemsViewModel : ViewModelBase
    {
        #region Initialization
        //WorkflowItem oldCollectionItem;
        PhotoSorterDBModelDataContext db;
        #endregion

        #region Properties
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

        ObservableCollection<WorkflowItem> _dgWorkflowItemsData;
        ObservableCollection<WorkflowItem> _selectedWorkflowItems;
        WorkflowItem _selectedWorkflowItem;

        public ObservableCollection<WorkflowItem> dgWorkflowItemsData
        {
            get { return _dgWorkflowItemsData; }
            set { _dgWorkflowItemsData = value; NotifyPropertyChanged("dgWorkflowItemsData"); }
        }
        public ObservableCollection<WorkflowItem> selectedWorkflowItems
        {
            get { return _selectedWorkflowItems; }
            set { _selectedWorkflowItems = value; NotifyPropertyChanged("selectedWorkflowItems"); }
        }
        public WorkflowItem selectedWorkflowItem
        {
            get { return _selectedWorkflowItem; }
            set
            {

                //if (_selectedWorkflowItem != null)
                //{
                //    //check wether user updated sort order
                //    //if yes updated on DB aswell
                //    if (!_selectedWorkflowItem.Equals(oldCollectionItem))
                //    {
                //        updateSortOrder(selectedWorkflowItem);
                //    }
                //}
                _selectedWorkflowItem = value; NotifyPropertyChanged("selectedWorkflowItem");

                //user can edit sort order from UI  in order to update in DB store selectedWorkflowItem in a temp object and do a comparison at start
                // if (selectedWorkflowItem != null) { oldCollectionItem = new WorkflowItem { Assignedto = selectedWorkflowItem.Assignedto, SortOrder = selectedWorkflowItem.SortOrder, Type = selectedWorkflowItem.Type, Description = selectedWorkflowItem.Description, Status = selectedWorkflowItem.Status, Id = selectedWorkflowItem.Id }; }
            }
        }

        #endregion

        #region Constructor
        public WorkflowItemsViewModel()
        {
            dgWorkflowItemsData = new ObservableCollection<WorkflowItem>();
            selectedWorkflowItems = new ObservableCollection<WorkflowItem>();
            loadData();
        }
        #endregion

        #region Commands
        public RelayCommand WorkflowItemMouseLeftClickCommand
        {
            get
            {
                return new RelayCommand(workflowItemMouseLeftClick);
            }
        }
        public RelayCommand WorkflowItemMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(workflowItemMouseDoubleClick);
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
        public void loadData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            int tempworkflowId = (from ic in db.ItemClassTypes where ic.ClassType == "Workflow" select ic.Id).FirstOrDefault();
            dgWorkflowItemsData = new ObservableCollection<WorkflowItem>((from wi in db.WorkflowItems where wi.ItemClassTypeId == tempworkflowId select wi).ToList());

            setButtonVisibility();
        }

        internal void newWorkflowItem()
        {
            AddEditWorkflowItems _objAddEditWorkflowItems = new AddEditWorkflowItems();
            _objAddEditWorkflowItems.ShowDialog();

            if (((AddEditWorkflowItemsViewModel)(_objAddEditWorkflowItems.DataContext)).isSave)
            {
                dgWorkflowItemsData.Add(((AddEditWorkflowItemsViewModel)(_objAddEditWorkflowItems.DataContext))._objWorkflowItem);
                selectedWorkflowItem = ((AddEditWorkflowItemsViewModel)(_objAddEditWorkflowItems.DataContext))._objWorkflowItem;
            }
        }
        internal void editWorkflowItem()
        {
            if (selectedWorkflowItem != null)
            {
                AddEditWorkflowItems _objAddEditWorkflowItems = new AddEditWorkflowItems(selectedWorkflowItem);
                _objAddEditWorkflowItems.ShowDialog();

                if (((AddEditWorkflowItemsViewModel)(_objAddEditWorkflowItems.DataContext)).isSave)
                {
                    int tempIndex = dgWorkflowItemsData.Count <= 1 ? 0 : dgWorkflowItemsData.IndexOf(selectedWorkflowItem);
                    dgWorkflowItemsData.Remove(selectedWorkflowItem);

                    dgWorkflowItemsData.Insert(tempIndex, ((AddEditWorkflowItemsViewModel)(_objAddEditWorkflowItems.DataContext))._objWorkflowItem);
                    selectedWorkflowItem = ((AddEditWorkflowItemsViewModel)(_objAddEditWorkflowItems.DataContext))._objWorkflowItem;
                    selectedWorkflowItems.Add(selectedWorkflowItem);

                    updateCollectionItems(selectedWorkflowItem);
                }
            }
        }

        private void updateCollectionItems(WorkflowItem selectedItemId)
        {
            //WorkflowCollectionItem _objCollectionItem = (from ci in db.WorkflowCollectionItems where ci.WorkflowItemId == selectedItemId.Id select ci).FirstOrDefault();

            //if (_objCollectionItem != null)
            //{
            //    _objCollectionItem.SortOrder = selectedItemId.SortOrder;
            //    db.SubmitChanges();
            //}
            clsWorkflows.updateCollectionItemSortOrder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedItemId.Id, Convert.ToInt32(selectedItemId.Quantity), Convert.ToInt32(selectedItemId.SortOrder),"Workflow");
        }
        internal void deleteWorkflow()
        {
            try
            {
                if (selectedWorkflowItems.Count > 0)
                {
                    string message;

                    List<int> arrWorkflowIDs = (from swi in selectedWorkflowItems select swi.Id).ToList();
                    //foreach (WorkflowItem tempworkflowItem in selectedWorkflowItems)
                    //{
                    //    try
                    //    {
                    //        int workflowId = Convert.ToInt32(tempworkflowItem.Id);
                    //        if (!arrWorkflowIDs.Contains(workflowId))
                    //        {
                    //            arrWorkflowIDs.Add(workflowId);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        clsStatic.WriteExceptionLogXML(ex);
                    //    }
                    //}

                    if (selectedWorkflowItems.Count == 1)
                        message = "Are you sure you want to delete workflow?";
                    else
                        message = "Are you sure you want to delete multiple workflows?";

                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        int result = clsWorkflows.deleteWorkflow(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrWorkflowIDs);

                        if (result != 0)
                        {
                            foreach (int phstId in arrWorkflowIDs)
                            {
                                dgWorkflowItemsData.Remove(dgWorkflowItemsData.Where(i => i.Id == phstId).First());
                            }
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

        #region Search & Group Panels
        internal void searchPanels()
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
        internal void groupPanels()
        {

            if (workflowItemsShowGroupPanel)
                workflowItemsShowGroupPanel = false;
            else
                workflowItemsShowGroupPanel = true;
        }
        #endregion

        private void workflowItemMouseLeftClick()
        {
            //setButtonVisibility();
        }
        private void workflowItemMouseDoubleClick()
        {
            editWorkflowItem();
        }
        public void updateSortOrder(WorkflowItem oldCollectionItem)
        {
            //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (oldCollectionItem.SortOrder == 0 || oldCollectionItem.SortOrder.ToString() == "")
                return;
            int result = clsWorkflows.updateWorkflowItem(db, oldCollectionItem);

            //int result = clsOrders.UpadteOrderPackagesData(db, item, package, billingCode, packageId);
            //MVVMMessageService.ShowMessage("Package updated successfully.");
        }
        private void rowUpdate()
        {
            db.SubmitChanges();
            updateCollectionItems(selectedWorkflowItem);
        }

        /// <summary>
        /// this method used for buttons visibility
        /// </summary>
        public void setButtonVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDeleteVisible = true;
        }
        #endregion
    }
}
