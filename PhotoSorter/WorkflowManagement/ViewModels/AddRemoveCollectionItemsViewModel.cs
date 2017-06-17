using PhotoForce.App_Code;
using PhotoForce.Extensions;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkflowManagement
{
    public class AddRemoveCollectionItemsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public bool isSave = false;
        public string selectedOption = "";
        public WorkflowCollection selectedItem;
        string selectedTab = "";
        int tempworkflowId = 0;
        #endregion

        #region Properties
        ObservableCollection<WorkflowCollection> _collectionsData;
        WorkflowCollection _selectedCollection;
        Visibility _isCollectionsVisible;
        bool _isRemoveItemsChecked;
        bool _isAddItemsChecked;

        public bool isAddItemsChecked
        {
            get { return _isAddItemsChecked; }
            set { _isAddItemsChecked = value; NotifyPropertyChanged("isAddItemsChecked"); }
        }
        public bool isRemoveItemsChecked
        {
            get { return _isRemoveItemsChecked; }
            set { _isRemoveItemsChecked = value; NotifyPropertyChanged("isRemoveItemsChecked"); }
        }
        public Visibility isCollectionsVisible
        {
            get { return _isCollectionsVisible; }
            set { _isCollectionsVisible = value; NotifyPropertyChanged("isCollectionsVisible"); }
        }
        public WorkflowCollection selectedCollection
        {
            get { return _selectedCollection; }
            set
            {
                _selectedCollection = value; NotifyPropertyChanged("selectedCollection");
            }
        }
        public ObservableCollection<WorkflowCollection> collectionsData
        {
            get { return _collectionsData; }
            set { _collectionsData = value; NotifyPropertyChanged("collectionsData"); }
        }
        #endregion

        #region Constructor
        public AddRemoveCollectionItemsViewModel(string isFrom)
        {
            selectedTab = isFrom == "Dashboard" ? "Workflow" : "Equipment";
            collectionsData = new ObservableCollection<WorkflowCollection>();

            tempworkflowId = (from ic in db.ItemClassTypes where ic.ClassType == selectedTab select ic.Id).FirstOrDefault();
            if (tempworkflowId > 0)
                collectionsData = new ObservableCollection<WorkflowCollection>((from cd in db.WorkflowCollections orderby cd.Id where cd.ItemClassTypeId == tempworkflowId select cd).ToList());
        }

        #endregion

        #region Commands
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(saveAndClose);
            }
        }
        #endregion

        #region Methods
        private void saveAndClose()
        {
            if (selectedCollection != null)
            {
                selectedItem = selectedCollection;
                if (isAddItemsChecked)
                    selectedOption = AddOrReplaceCollectionItems.AddItems.ToString();
                else if (isRemoveItemsChecked)
                {
                    string message = "Existing items will be replaced. are you sure ?";
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        selectedOption = AddOrReplaceCollectionItems.RemoveItems.ToString();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MVVMMessageService.ShowMessage("Please select an option.");
                    return;
                }
                isSave = true;
                DialogResult = false;
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
