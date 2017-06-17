using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.Extensions;
using PhotoForce.MVVM;
using PhotoForce.WorkflowManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class DashBoardWorkflowItemsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        PhotoshootWorkflowItem oldCollectionItem;
        string selectedButton = "";
        #endregion

        #region Properties
        ObservableCollection<PhotoshootWorkflowItem> _dgWorkflowItemsData;
        PhotoshootWorkflowItem _selectedWorkflowItem;
        ObservableCollection<PhotoshootWorkflowItem> _selectedWorkflowItems;
        Visibility _notesPreviewVisibility;
        ObservableCollection<ComboBoxItem> _workflowItemNotes;
        Visibility _notesTextVisibility;
        string _notes;
        ComboBoxItem _selectedNotes;
        private string _filterString;

        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value; NotifyPropertyChanged();
            }
        }
        public ComboBoxItem selectedNotes
        {
            get { return _selectedNotes; }
            set { _selectedNotes = value; NotifyPropertyChanged("selectedNotes"); }
        }
        public string notes
        {
            get { return _notes; }
            set { _notes = value; NotifyPropertyChanged(); }
        }
        public Visibility notesTextVisibility
        {
            get { return _notesTextVisibility; }
            set { _notesTextVisibility = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<ComboBoxItem> workflowItemNotes
        {
            get { return _workflowItemNotes; }
            set { _workflowItemNotes = value; NotifyPropertyChanged(); }
        }
        public Visibility notesPreviewVisibility
        {
            get { return _notesPreviewVisibility; }
            set { _notesPreviewVisibility = value; NotifyPropertyChanged("notesPreviewVisibility"); }
        }

        public ObservableCollection<PhotoshootWorkflowItem> selectedWorkflowItems
        {
            get { return _selectedWorkflowItems; }
            set { _selectedWorkflowItems = value; NotifyPropertyChanged(); }
        }
        public PhotoshootWorkflowItem selectedWorkflowItem
        {
            get { return _selectedWorkflowItem; }
            set
            {

                if (_selectedWorkflowItem != null)
                {
                    //check wether user updated sort order || default price from UI 
                    //if yes updated on DB aswell
                    if (!_selectedWorkflowItem.Equals(oldCollectionItem))
                    {
                        updateSortOrder(selectedWorkflowItem);
                    }
                }
                _selectedWorkflowItem = value; NotifyPropertyChanged("selectedWorkflowItem");

                //user can edit sort order , default price from UI  in order to update in DB store selectedPackage in a temp object and do a comparison at start
                if (selectedWorkflowItem != null) { oldCollectionItem = new PhotoshootWorkflowItem { Assignedto = selectedWorkflowItem.Assignedto, SortOrder = selectedWorkflowItem.SortOrder, Status = selectedWorkflowItem.Status, DueDate = selectedWorkflowItem.DueDate, CreatedOn = selectedWorkflowItem.CreatedOn, CreatedBy = selectedWorkflowItem.CreatedBy, Id = selectedWorkflowItem.Id }; }
            }
        }
        public ObservableCollection<PhotoshootWorkflowItem> dgWorkflowItemsData
        {
            get { return _dgWorkflowItemsData; }
            set { _dgWorkflowItemsData = value; NotifyPropertyChanged("dgWorkflowItemsData"); }
        }
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
        #endregion

        #region Constructor
        public DashBoardWorkflowItemsViewModel()
        {
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
        public RelayCommand AddNotesVisibleCommand
        {
            get
            {
                return new RelayCommand(addNotesVisible);
            }
        }
        public RelayCommand AddNotesCommand
        {
            get
            {
                return new RelayCommand(addEditNotes);
            }
        }
        public RelayCommand EditNotesVisibleCommand
        {
            get
            {
                return new RelayCommand(editNotes);
            }
        }
        public RelayCommand DeleteNotesVisibleCommand
        {
            get
            {
                return new RelayCommand(deleteNotes);
            }
        }
        public RelayCommand SaveFilterStringCommand
        {
            get
            {
                return new RelayCommand(saveFilterString);
            }
        }
        public RelayCommand ApplyFilterStringCommand
        {
            get
            {
                return new RelayCommand(applyFilterString);
            }
        }
        #endregion

        #region Methods
        internal void loadData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString); notesTextVisibility = Visibility.Collapsed;
            dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(); selectedWorkflowItems = new ObservableCollection<PhotoshootWorkflowItem>();
            dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getPSWorkflowItems(db));
            workflowItemNotes = new ObservableCollection<ComboBoxItem>();
        }
        internal void editRecord()
        {
            workflowItemMouseDoubleClick();
        }
        private void workflowItemMouseLeftClick()
        {
            setButtonVisibility();
            if (selectedWorkflowItem == null) { return; }
            if (notesTextVisibility == Visibility.Visible)
            {
                notesTextVisibility = Visibility.Collapsed;
                notes = "";
            }
            //photoshootWorkflowItemId = selectedWorkflowItem.Id;

            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            //Load Notes
            workflowItemNotes = new ObservableCollection<ComboBoxItem>(clsWorkflows.getNotesForWorkflowItem(db, (int)selectedWorkflowItem.WorkflowItemId, (int)selectedWorkflowItem.PhotoShootID));

            ObservableCollection<ComboBoxItem> tempWorkflowItemNotes = new ObservableCollection<ComboBoxItem>(clsWorkflows.getNotesForWorkflowItem(db, (int)selectedWorkflowItem.WorkflowItemId, (int)selectedWorkflowItem.PhotoShootID));
            if (tempWorkflowItemNotes.Count > 0)
            {
                string workflowNotes = tempWorkflowItemNotes[0].Name.ToString();
                string[] lines = workflowNotes.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                foreach (string s in lines)
                {
                    if (string.IsNullOrEmpty(s)) { continue; }
                    workflowItemNotes.Add(new ComboBoxItem { Name = s });
                }
            }
            else
                workflowItemNotes = new ObservableCollection<ComboBoxItem>();
        }
        private void workflowItemMouseDoubleClick()
        {
            if (selectedWorkflowItem != null)
            {
                EditPhotoshootWorkflowItems _objEditWorkflowItems = new EditPhotoshootWorkflowItems(selectedWorkflowItem);
                _objEditWorkflowItems.ShowDialog();

                if (((EditPhotoshootWorkflowItemsViewModel)(_objEditWorkflowItems.DataContext)).isSave)
                {
                    int tempIndex = dgWorkflowItemsData.Count <= 1 ? 0 : dgWorkflowItemsData.IndexOf(selectedWorkflowItem);
                    dgWorkflowItemsData.Remove(selectedWorkflowItem);

                    dgWorkflowItemsData.Insert(tempIndex, ((EditPhotoshootWorkflowItemsViewModel)(_objEditWorkflowItems.DataContext)).addEditWorkflowItem);
                    selectedWorkflowItem = ((EditPhotoshootWorkflowItemsViewModel)(_objEditWorkflowItems.DataContext)).addEditWorkflowItem;
                    selectedWorkflowItems.Add(selectedWorkflowItem);
                }
            }
        }
        internal void bulkRename()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (selectedWorkflowItems.Count > 0)
            {
                EditPhotoshootWorkflowItems _objEditWorkflowItems = new EditPhotoshootWorkflowItems(selectedWorkflowItems);
                _objEditWorkflowItems.ShowDialog();

                if (((EditPhotoshootWorkflowItemsViewModel)(_objEditWorkflowItems.DataContext)).isSave)
                    dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getPSWorkflowItems(db));
            }
        }
        internal void deleteRecords()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            string message = "";
            ArrayList photoShootWorkflowItemIds = new ArrayList();
            List<string> PhotoshootNames = new List<string>();
            if (selectedWorkflowItems.Count > 0)
            {
                int deletedRecords = selectedWorkflowItems.Count;
                if (selectedWorkflowItems.Count == 1)
                {
                    message = "Are you sure you want to delete selected workflow?";
                }
                else
                {
                    message = "Are you sure you want to delete multiple workflows?";
                }
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    foreach (PhotoshootWorkflowItem pwi in selectedWorkflowItems)
                    {
                        photoShootWorkflowItemIds.Add(pwi.Id);
                        if (!PhotoshootNames.Contains(pwi.PhotoShoot.PhotoShotName))
                            PhotoshootNames.Add(pwi.PhotoShoot.PhotoShotName);
                    }

                    int result = clsWorkflows.deletePhotoshootWorkflowItems(db, photoShootWorkflowItemIds);
                    dgWorkflowItemsData = new ObservableCollection<PhotoshootWorkflowItem>(clsWorkflows.getPSWorkflowItems(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString)));
                    string tempPhotoshootNames = string.Join(",", PhotoshootNames);

                    #region Logfile
                    clsErrorLog objError = new clsErrorLog();

                    objError.Source = "WorkflowItems";
                    objError.MethodName = "Delete WorkflowItems from dashboard";
                    objError.Message = "Photoshoot WorkflowItems log file. \n Schoolname : " + clsSchool.defaultSchoolName + "\n Photoshoot name(s) : " + tempPhotoshootNames + " \n Action: Record(s) deleted \n Deleted records : " + deletedRecords + " \n Total records : " + dgWorkflowItemsData.Count + "\n";
                    objError.UserComments = clsStatic.userName;
                    objError.DateTime = DateTime.Now;

                    clsStatic.WriteErrorLog(objError, "Photoshoot WorkflowItems Info.");
                    //clsStatic.WriteErrorLog(objError, objstatic.ErrorLogXML);

                    #endregion

                }
            }
        }

        private void addNotesVisible()
        {
            selectedButton = "AddButton";
            if (notesTextVisibility == Visibility.Visible)
            {
                notesTextVisibility = Visibility.Collapsed;
                notes = "";
            }
            else
                notesTextVisibility = Visibility.Visible;
        }

        private void addEditNotes()
        {
            if (!string.IsNullOrEmpty(notes) && selectedWorkflowItem != null)
            {
                PhotoshootWorkflowItem tempPSWorkflowItem = new PhotoshootWorkflowItem();
                if (selectedButton == "AddButton")
                {
                    string timestamp = "----- Created by:   " + clsStatic.userName + " " + DateTime.Now + " -----";

                    notes = timestamp + Environment.NewLine + notes;

                    tempPSWorkflowItem = clsWorkflows.getPhotoWorkflowItem(db, (int)selectedWorkflowItem.Id);
                    tempPSWorkflowItem.Notes = notes + Environment.NewLine + Environment.NewLine + tempPSWorkflowItem.Notes;

                    db.SubmitChanges();

                    workflowItemNotes.Insert(0, new ComboBoxItem { Name = notes });

                    notes = "";
                    notesTextVisibility = Visibility.Collapsed;
                }
                else if (selectedButton == "EditButton")
                {
                    string newNotes = "";

                    tempPSWorkflowItem = clsWorkflows.getPhotoWorkflowItem(db, selectedWorkflowItem.Id);

                    string[] tempselectedNotes = selectedNotes.Name.Split(new string[] { "\n" }, StringSplitOptions.None);

                    string[] lines = tempPSWorkflowItem.Notes.Split(new string[] { "\r\n\r\n", "\n" }, StringSplitOptions.None);

                    workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                    for (int i = 0; i < lines.Count() - 1; i += 2)
                    {
                        if (lines[i] == tempselectedNotes[0])
                        {
                            lines[i + 1] = notes;
                        }
                        if (!string.IsNullOrEmpty(newNotes))
                        {
                            newNotes = newNotes + (Environment.NewLine + Environment.NewLine) + (lines[i].TrimEnd('\r') + "\n" + lines[i + 1]);
                        }
                        else
                            newNotes = lines[i].TrimEnd('\r') + "\n" + lines[i + 1];
                    }

                    if (!string.IsNullOrEmpty(newNotes))
                    {
                        tempPSWorkflowItem.Notes = newNotes;
                        db.SubmitChanges();
                        workflowItemMouseLeftClick();
                    }

                }

                #region update hasNotes field
                if (!string.IsNullOrEmpty(tempPSWorkflowItem.Notes) && (tempPSWorkflowItem.HasNotes == false || tempPSWorkflowItem.HasNotes == null))
                {
                    clsWorkflows.updatePhotoShootWorkflowItemshasNotes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedWorkflowItem, "from Edit/Add");

                    loadData();
                }
                #endregion
            }
            else
            {
                if (selectedWorkflowItem == null)
                    MVVMMessageService.ShowMessage("Please select workflow item.");
                else
                    MVVMMessageService.ShowMessage("Please fill notes.");
            }
        }

        private void editNotes()
        {
            selectedButton = "EditButton";
            if (selectedNotes != null)
            {
                if (notesTextVisibility == Visibility.Visible)
                {
                    notesTextVisibility = Visibility.Collapsed;
                    notes = "";
                }
                else
                {
                    notesTextVisibility = Visibility.Visible;
                    string[] tempNotes = selectedNotes.Name.Split(new string[] { "\n" }, StringSplitOptions.None);
                    if (tempNotes.Count() > 2)
                    {
                        for (int i = 1; i < tempNotes.Count(); i++)
                            notes += tempNotes[i];
                    }
                    else
                        notes = tempNotes[1];
                }
            }
        }
        private void deleteNotes()
        {
            selectedButton = "DeleteButton";
            if (selectedNotes != null && selectedWorkflowItem != null)
            {
                notesTextVisibility = Visibility.Collapsed;
                notes = "";


                string newNotes = "";

                PhotoshootWorkflowItem tempPSWorkflowItem = clsWorkflows.getPhotoWorkflowItem(db, selectedWorkflowItem.Id);

                string[] tempselectedNotes = selectedNotes.Name.Split(new string[] { "\n" }, StringSplitOptions.None);

                string[] lines = tempPSWorkflowItem.Notes.Split(new string[] { "\r\n\r\n", "\n" }, StringSplitOptions.None);

                int maxcount = lines.Count() % 2 == 0 ? lines.Count() : lines.Count() - 1;

                workflowItemNotes = new ObservableCollection<ComboBoxItem>();
                for (int i = 0; i < maxcount; i += 2)
                {
                    if (lines[i] == tempselectedNotes[0])
                    {
                        lines[i] = "";
                        lines[i + 1] = notes;
                    }
                    if (!string.IsNullOrEmpty(newNotes))
                    {
                        if (lines[i] == "")
                        {
                        }
                        else
                        {
                            newNotes = newNotes + (Environment.NewLine + Environment.NewLine) + (lines[i].TrimEnd('\r') + "\n" + lines[i + 1]);
                        }
                    }
                    else if (lines[i] != "")
                        newNotes = lines[i].TrimEnd('\r') + "\n" + lines[i + 1];
                }

                if (newNotes == "")
                    newNotes = null;
                tempPSWorkflowItem.Notes = newNotes;
                db.SubmitChanges();

                #region update hasNotes field
                if (string.IsNullOrEmpty(tempPSWorkflowItem.Notes) && tempPSWorkflowItem.HasNotes == true)
                {
                    clsWorkflows.updatePhotoShootWorkflowItemshasNotes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedWorkflowItem, "from Delete");

                    loadData();
                }
                #endregion

                workflowItemMouseLeftClick();
            }
            else
            {
                if (selectedWorkflowItem == null)
                    MVVMMessageService.ShowMessage("Please select workflow item.");
                else if (selectedNotes == null)
                    MVVMMessageService.ShowMessage("Please select notes.");
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
        public void updateSortOrder(PhotoshootWorkflowItem oldCollectionItem)
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (oldCollectionItem.SortOrder == null || oldCollectionItem.SortOrder.ToString() == "")
                return;
            int result = clsWorkflows.updatePhotoShootWorkflowItems(db, oldCollectionItem);

            //int result = clsOrders.UpadteOrderPackagesData(db, item, package, billingCode, packageId);
            //MVVMMessageService.ShowMessage("Package updated successfully.");
        }
        /// <summary>
        /// this method used for buttons visibility
        /// </summary>
        public void setButtonVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isBulkRenameVisible = true; (Application.Current as App).isEditVisible = true;
            (Application.Current as App).isDeleteVisible = true;
        }


        #endregion

        private void applyFilterString()
        {
            string tempString = (from workflowFilter in db.Settings where workflowFilter.settingName == "WorkflowFilterString" select workflowFilter.settingValue).FirstOrDefault();

            if (string.IsNullOrEmpty(tempString))
                MVVMMessageService.ShowMessage("There are no saved filters.");
            else
            {
                Regex re = new Regex("@");
                FilterString = re.Replace(tempString, "'");
            }
        }
        private void saveFilterString()
        {
            if (!string.IsNullOrEmpty(FilterString))
            {
                int isWorkflowFilterAvilable = (from workflowFilter in db.Settings where workflowFilter.settingName == "WorkflowFilterString" select workflowFilter).Count();
                clsWorkflows.SaveOrInsertWorkflowFilterString(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), FilterString, isWorkflowFilterAvilable > 0 ? true : false);
            }
        }

    }
}
