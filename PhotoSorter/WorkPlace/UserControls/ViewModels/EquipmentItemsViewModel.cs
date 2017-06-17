using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.WorkflowManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class EquipmentItemsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        #endregion

        #region Properties

        ObservableCollection<WorkflowItem> _dgEquipmentItemsData;
        ObservableCollection<WorkflowItem> _selectedEquipmentItems;
        WorkflowItem _selectedEquipmentItem;

        public ObservableCollection<WorkflowItem> dgEquipmentItemsData
        {
            get { return _dgEquipmentItemsData; }
            set { _dgEquipmentItemsData = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<WorkflowItem> selectedEquipmentItems
        {
            get { return _selectedEquipmentItems; }
            set { _selectedEquipmentItems = value; NotifyPropertyChanged("selectedEquipmentItems"); }
        }
        public WorkflowItem selectedEquipmentItem
        {
            get { return _selectedEquipmentItem; }
            set { _selectedEquipmentItem = value; NotifyPropertyChanged(); }
        }

        #region Search and group panels
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
        #endregion

        #region Constructor
        public EquipmentItemsViewModel()
        {
            dgEquipmentItemsData = new ObservableCollection<WorkflowItem>();
            selectedEquipmentItems = new ObservableCollection<WorkflowItem>();
            loadData();
        }
        #endregion

        #region Commands
        public RelayCommand WorkflowItemMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(editWorkflowItem);
            }
        }
        public RelayCommand equipmentItemMouseLeftClickCommand
        {
            get
            {
                return new RelayCommand(setButtonVisibility);
            }
        }
        #endregion

        #region Methods
        internal void loadData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            int tempequipmentId = (from ic in db.ItemClassTypes where ic.ClassType == "Equipment" select ic.Id).FirstOrDefault();
            dgEquipmentItemsData = new ObservableCollection<WorkflowItem>((from wi in db.WorkflowItems where wi.ItemClassTypeId == tempequipmentId select wi).ToList());
            if (dgEquipmentItemsData != null && dgEquipmentItemsData.Count > 0)
                selectedEquipmentItem = dgEquipmentItemsData[0];

            //setButtonVisibility();
        }


        internal void newWorkflowItem()
        {
            AddEditEquipmentItems _objAddEditEquipmentItems = new AddEditEquipmentItems();
            _objAddEditEquipmentItems.ShowDialog();

            if (((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext)).isSave)
            {
                dgEquipmentItemsData.Add(((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext))._objWorkflowItem);
                selectedEquipmentItem = ((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext))._objWorkflowItem;
                loadData();
            }
        }
        internal void editWorkflowItem()
        {
            if (selectedEquipmentItem != null )
            {
                AddEditEquipmentItems _objAddEditEquipmentItems = new AddEditEquipmentItems(selectedEquipmentItem);
                _objAddEditEquipmentItems.ShowDialog();

                if (((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext)).isSave)
                {
                    int tempIndex = dgEquipmentItemsData.Count <= 1 ? 0 : dgEquipmentItemsData.IndexOf(selectedEquipmentItem);
                    dgEquipmentItemsData.Remove(selectedEquipmentItem);

                    dgEquipmentItemsData.Insert(tempIndex, ((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext))._objWorkflowItem);
                    selectedEquipmentItem = ((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext))._objWorkflowItem;
                    selectedEquipmentItems.Add(selectedEquipmentItem);

                    updateCollectionItems(selectedEquipmentItem);
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
            clsWorkflows.updateCollectionItemSortOrder(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedItemId.Id, Convert.ToInt32(selectedItemId.Quantity), Convert.ToInt32(selectedItemId.SortOrder),"Equipment");
        }

        internal void deleteWorkflow()
        {
            try
            {
                if (selectedEquipmentItems.Count > 0)
                {
                    string message;

                    List<int> arrWorkflowIDs = (from swi in selectedEquipmentItems select swi.Id).ToList();
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

                    if (selectedEquipmentItems.Count == 1)
                        message = "Are you sure you want to delete equipment?";
                    else
                        message = "Are you sure you want to delete multiple equipments?";

                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgEquipmentItemsData.Count();
                        //int deletedRecordsCount = arrWorkflowIDs.Count;
                        int result = clsWorkflows.deleteWorkflow(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrWorkflowIDs);

                        if (result != 0)
                        {
                            foreach (int phstId in arrWorkflowIDs)
                            {
                                dgEquipmentItemsData.Remove(dgEquipmentItemsData.Where(i => i.Id == phstId).First());
                            }

                            //createDeletedRecordsLogFile("EquipmentItems", totalRecordsCount, deletedRecordsCount);
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
        internal void bulkRename()
        {
            if (selectedEquipmentItems.Count > 0)
            {

                ArrayList arrSelectedEquipmentIDs = new ArrayList();
                int EquipmentId;
                foreach (WorkflowItem tempEquipmentItem in selectedEquipmentItems)
                {
                    try
                    {
                        EquipmentId = Convert.ToInt32(tempEquipmentItem.Id);
                        if (!arrSelectedEquipmentIDs.Contains(EquipmentId))
                        {
                            arrSelectedEquipmentIDs.Add(EquipmentId);
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }

                AddEditEquipmentItems _objAddEditEquipmentItems = new AddEditEquipmentItems(arrSelectedEquipmentIDs);
                _objAddEditEquipmentItems.ShowDialog();

                if (((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext)).isSave)
                {
                    //int tempIndex = dgEquipmentItemsData.Count <= 1 ? 0 : dgEquipmentItemsData.IndexOf(selectedEquipmentItem);
                    //dgEquipmentItemsData.Remove(selectedEquipmentItem);

                    //dgEquipmentItemsData.Insert(tempIndex, ((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext))._objWorkflowItem);
                    //selectedEquipmentItem = ((AddEditEquipmentItemsViewModel)(_objAddEditEquipmentItems.DataContext))._objWorkflowItem;
                    //selectedEquipmentItems.Add(selectedEquipmentItem);
                    loadData();
                }
            }
        }
        #region Search & Group Panels
        internal void searchPanels()
        {
            if (equipmentItemsSearchControl == null || !equipmentItemsSearchVisible) 
            {
                equipmentItemsSearchPanelMode = ShowSearchPanelMode.Always; equipmentItemsSearchVisible = true;
            }
            else
            {
                equipmentItemsSearchPanelMode = ShowSearchPanelMode.Never; equipmentItemsSearchVisible = false;
            }
        }
        internal void groupPanels()
         {

            if (equipmentItemsShowGroupPanel)
                equipmentItemsShowGroupPanel = false;
            else
                equipmentItemsShowGroupPanel = true;
        }
        #endregion

        /// <summary>
        /// this method used for buttons visibility
        /// </summary>
        public void setButtonVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDeleteVisible = true; (Application.Current as App).isBulkRenameVisible = true;
        }
        #endregion
    }
}
