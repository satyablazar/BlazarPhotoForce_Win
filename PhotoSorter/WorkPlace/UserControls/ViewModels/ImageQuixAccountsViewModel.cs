using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.ImageQuixManagement;
using PhotoForce.MVVM;
using PhotoForce.Student_Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.ViewModels
{
    public class ImageQuixAccountsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        string selectedGrid = "";
        #endregion

        #region Properties
        ObservableCollection<IQAccount> _dgImageQuixAccountsData;
        ObservableCollection<IQAccount> _selectedImageQuixAccounts;
        IQAccount _selectedImageQuixAccount;
        ObservableCollection<IQPriceSheet> _dgPricesheetsData;
        ObservableCollection<IQPriceSheet> _selectedPricesheets;
        IQPriceSheet _selectedPricesheet;
        ObservableCollection<IQVandoSetting> _dgIQVandoSettingsData;
        ObservableCollection<IQVandoSetting> _selectedIQVandoSettings;
        IQVandoSetting _selectedIQVandoSetting;
        public int _selectedIQAccountIndex;
        public int selectedIQAccountIndex
        {
            get { return _selectedIQAccountIndex; }
            set
            {
                _selectedIQAccountIndex = value; NotifyPropertyChanged("selectedPhotoShootIndex");

                if (selectedIQAccountIndex == 0)
                {
                    selectedGrid = "Pricesheet";
                    if (selectedIQAccount != null)
                        fillPricesheets();
                    else
                    {
                        dgPricesheetsData = new ObservableCollection<IQPriceSheet>();
                    }
                }
                if (selectedIQAccountIndex == 1)
                {
                    selectedGrid = "VandoSettings";
                    dgIQVandoSettingsData = new ObservableCollection<IQVandoSetting>();
                    selectedIQVandoSettings = new ObservableCollection<IQVandoSetting>();
                    if (selectedIQAccount != null)
                        fillVandoSettings();
                    else
                        dgIQVandoSettingsData = new ObservableCollection<IQVandoSetting>();

                }
            }
        }
        public IQVandoSetting selectedIQVandoSetting
        {
            get { return _selectedIQVandoSetting; }
            set { _selectedIQVandoSetting = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<IQVandoSetting> selectedIQVandoSettings
        {
            get { return _selectedIQVandoSettings; }
            set { _selectedIQVandoSettings = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<IQVandoSetting> dgIQVandoSettingsData
        {
            get { return _dgIQVandoSettingsData; }
            set { _dgIQVandoSettingsData = value; NotifyPropertyChanged(); }
        }
        public IQPriceSheet selectedPricesheet
        {
            get { return _selectedPricesheet; }
            set { _selectedPricesheet = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<IQPriceSheet> selectedPricesheets
        {
            get { return _selectedPricesheets; }
            set { _selectedPricesheets = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<IQPriceSheet> dgPricesheetsData
        {
            get { return _dgPricesheetsData; }
            set { _dgPricesheetsData = value; NotifyPropertyChanged(); }
        }
        public IQAccount selectedIQAccount
        {
            get { return _selectedImageQuixAccount; }
            set
            {
                _selectedImageQuixAccount = value; NotifyPropertyChanged();

                if (selectedIQAccountIndex == 0)
                    fillPricesheets();
                else
                    fillVandoSettings();
            }
        }
        public ObservableCollection<IQAccount> selectedImageQuixAccounts
        {
            get { return _selectedImageQuixAccounts; }
            set { _selectedImageQuixAccounts = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<IQAccount> dgImageQuixAccountsData
        {
            get { return _dgImageQuixAccountsData; }
            set { _dgImageQuixAccountsData = value; NotifyPropertyChanged(); }
        }

        #region Group & Search panels
        bool _imageQuixAccountShowGroupPanel;
        bool _pricesheetsShowGroupPanel;
        bool _vandoSettingsShowGroupPanel;

        public bool vandoSettingsShowGroupPanel
        {
            get { return _vandoSettingsShowGroupPanel; }
            set { _vandoSettingsShowGroupPanel = value; NotifyPropertyChanged(); }
        }
        public bool imageQuixAccountShowGroupPanel
        {
            get { return _imageQuixAccountShowGroupPanel; }
            set { _imageQuixAccountShowGroupPanel = value; NotifyPropertyChanged(); }
        }
        public bool pricesheetsShowGroupPanel
        {
            get { return _pricesheetsShowGroupPanel; }
            set { _pricesheetsShowGroupPanel = value; NotifyPropertyChanged(); }
        }


        ShowSearchPanelMode _imageQuixAccountSearchPanelMode;
        ShowSearchPanelMode _pricesheetsSearchPanelMode;
        ShowSearchPanelMode _vandoSettingsSearchPanelMode;
        SearchControl _imageQuixAccountSearchControl;
        SearchControl _pricesheetsSearchControl;
        SearchControl _vandoSettingsSearchControl;
        bool _isSearchControlVisible;

        public ShowSearchPanelMode vandoSettingsSearchPanelMode
        {
            get { return _vandoSettingsSearchPanelMode; }
            set { _vandoSettingsSearchPanelMode = value; NotifyPropertyChanged(); }
        }
        public SearchControl vandoSettingsSearchControl
        {
            get { return _vandoSettingsSearchControl; }
            set { _vandoSettingsSearchControl = value; NotifyPropertyChanged(); }
        }
        public ShowSearchPanelMode imageQuixAccountSearchPanelMode
        {
            get { return _imageQuixAccountSearchPanelMode; }
            set { _imageQuixAccountSearchPanelMode = value; NotifyPropertyChanged(); }
        }
        public ShowSearchPanelMode pricesheetsSearchPanelMode
        {
            get { return _pricesheetsSearchPanelMode; }
            set { _pricesheetsSearchPanelMode = value; NotifyPropertyChanged(); }
        }
        public SearchControl imageQuixAccountSearchControl
        {
            get { return _imageQuixAccountSearchControl; }
            set { _imageQuixAccountSearchControl = value; NotifyPropertyChanged(); }
        }
        public SearchControl pricesheetsSearchControl
        {
            get { return _pricesheetsSearchControl; }
            set { _pricesheetsSearchControl = value; NotifyPropertyChanged(); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged(); }
        }
        #endregion

        #endregion

        #region Constructor
        public ImageQuixAccountsViewModel()
        {
            dgImageQuixAccountsData = new ObservableCollection<IQAccount>(); selectedImageQuixAccounts = new ObservableCollection<IQAccount>();
            dgPricesheetsData = new ObservableCollection<IQPriceSheet>(); selectedPricesheets = new ObservableCollection<IQPriceSheet>();
            dgIQVandoSettingsData = new ObservableCollection<IQVandoSetting>(); selectedIQVandoSettings = new ObservableCollection<IQVandoSetting>();

            bindData();
        }
        #endregion

        #region Commands
        //public RelayCommand ImageQuixAccountsGridDoubleClickCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(editImagequixAccount);
        //    }
        //}
        public RelayCommand ImageQuixAccountsMouseUpCommand
        {
            get
            {
                return new RelayCommand(imageQuixAccountsMouseUp);
            }
        }
        //public RelayCommand PricesheetsGridDoubleClickCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(editPricesheet);
        //    }
        //}
        public RelayCommand PricesheetsMouseUpCommand
        {
            get
            {
                return new RelayCommand(priceSheetsMouseUp);
            }
        }
        public RelayCommand RowUpdateCommand
        {
            get
            {
                return new RelayCommand(rowUpdate);
            }
        }
        public RelayCommand IQVandoSettingsMouseUpCommand
        {
            get
            {
                return new RelayCommand(vandoSettingsMouseUp);
            }
        }
        #endregion

        #region Methods
        internal void bindData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

            dgImageQuixAccountsData = new ObservableCollection<IQAccount>((from IQA in db.IQAccounts orderby IQA.Id select IQA).ToList());
            if (dgImageQuixAccountsData.Count > 0)
            {
                selectedIQAccount = dgImageQuixAccountsData.First();
                fillPricesheets();
            }
        }
        /// <summary>
        /// this method is used to load workflow collection items
        /// </summary>
        private void fillPricesheets()
        {
            if (selectedIQAccount != null)
            {
                //dgPricesheetsData = new ObservableCollection<IQPriceSheet>(clsImageQuix.getAllIQPriceSheets(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedIQAccount.Id));
                dgPricesheetsData = new ObservableCollection<IQPriceSheet>(((from IQP in db.IQPriceSheets where IQP.IQAccountId == selectedIQAccount.Id orderby IQP.Id select IQP).ToList()));
                if (dgPricesheetsData.Count > 0)
                    selectedPricesheet = dgPricesheetsData.First();
            }
        }
        private void fillVandoSettings()
        {
            if (selectedIQAccount != null)
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                dgIQVandoSettingsData = new ObservableCollection<IQVandoSetting>(((from IQV in db.IQVandoSettings where IQV.IQAccountId == selectedIQAccount.Id orderby IQV.Id select IQV).ToList()));
                if (dgIQVandoSettingsData.Count > 0)
                    selectedIQVandoSetting = dgIQVandoSettingsData.First();
            }
        }
        internal void newImageQuix()
        {
            if (selectedGrid == "Accounts")
                newImagequixAccount();
            else if (selectedGrid == "Pricesheet")
                newPricesheet();
            else if (selectedGrid == "VandoSettings")
                newVandoSettings();
        }
        internal void editImageQuix()
        {
            if (selectedGrid == "Accounts")
                editImagequixAccount();
            else if (selectedGrid == "Pricesheet")
                editPricesheet();
            else if (selectedGrid == "VandoSettings")
                editVandoSettings();
        }
        private void editImagequixAccount()
        {
            if (selectedIQAccount != null)
            {
                AddEditIQAccount _objAddEditIQAccount = new AddEditIQAccount(selectedIQAccount);
                _objAddEditIQAccount.ShowDialog();

                if (((AddEditIQAccountViewModel)(_objAddEditIQAccount.DataContext)).isSave)
                {
                    //bindData();
                    int tempIQAccountIndex = dgImageQuixAccountsData.Count <= 1 ? 0 : dgImageQuixAccountsData.IndexOf(selectedIQAccount);
                    dgImageQuixAccountsData.Remove(selectedIQAccount);
                    selectedImageQuixAccounts.Remove(selectedIQAccount);

                    dgImageQuixAccountsData.Insert(tempIQAccountIndex, ((AddEditIQAccountViewModel)(_objAddEditIQAccount.DataContext))._objIQAccount);

                    selectedIQAccount = ((AddEditIQAccountViewModel)(_objAddEditIQAccount.DataContext))._objIQAccount;
                    selectedImageQuixAccounts.Add(selectedIQAccount);
                }
            }
        }
        private void editPricesheet()
        {
            if (selectedPricesheet != null)
            {

                AddEditIQAccount _objAddEditIQAccount = new AddEditIQAccount(selectedPricesheet, selectedIQAccount);
                _objAddEditIQAccount.ShowDialog();
                if (((AddEditIQAccountViewModel)(_objAddEditIQAccount.DataContext)).isSave)
                {
                    //bindData();

                    int tempPricesheetIndex = dgPricesheetsData.Count <= 1 ? 0 : dgPricesheetsData.IndexOf(selectedPricesheet);
                    dgPricesheetsData.Remove(selectedPricesheet);
                    selectedPricesheets.Remove(selectedPricesheet);

                    dgPricesheetsData.Insert(tempPricesheetIndex, ((AddEditIQAccountViewModel)(_objAddEditIQAccount.DataContext))._objIQPriceSheet);

                    selectedPricesheet = ((AddEditIQAccountViewModel)(_objAddEditIQAccount.DataContext))._objIQPriceSheet;
                    selectedPricesheets.Add(selectedPricesheet);
                }
            }
        }
        private void editVandoSettings()
        {
            if (selectedIQVandoSetting != null)
            {

                AddEditIQVandOSettings _objAddEditIQVandoSettings = new AddEditIQVandOSettings("Edit-IQVandoSettings", selectedIQVandoSetting, selectedIQAccount);
                _objAddEditIQVandoSettings.ShowDialog();

                if (((AddEditIQVandOSettingsViewModel)(_objAddEditIQVandoSettings.DataContext)).isSave)
                {
                    //bindData();

                    int tempVandoSettingsIndex = dgIQVandoSettingsData.Count <= 1 ? 0 : dgIQVandoSettingsData.IndexOf(selectedIQVandoSetting);
                    dgIQVandoSettingsData.Remove(selectedIQVandoSetting);
                    selectedIQVandoSettings.Remove(selectedIQVandoSetting);

                    dgIQVandoSettingsData.Insert(tempVandoSettingsIndex, ((AddEditIQVandOSettingsViewModel)(_objAddEditIQVandoSettings.DataContext))._objVandoSettings);

                    selectedIQVandoSetting = ((AddEditIQVandOSettingsViewModel)(_objAddEditIQVandoSettings.DataContext))._objVandoSettings;
                    selectedIQVandoSettings.Add(selectedIQVandoSetting);
                }
            }
        }
        private void newImagequixAccount()
        {
            if (selectedGrid == "Accounts")
            {
                AddEditIQAccount _objAddEditIQAccount = new AddEditIQAccount();
                _objAddEditIQAccount.ShowDialog();

                if (((AddEditIQAccountViewModel)(_objAddEditIQAccount.DataContext)).isSave)
                {
                    bindData();
                }
            }
        }
        internal void newPricesheet()
        {
            if (selectedGrid == "Pricesheet" && selectedIQAccount != null)
            {
                AddEditIQAccount _objAddEditIQAccount = new AddEditIQAccount(null, selectedIQAccount);
                _objAddEditIQAccount.ShowDialog();

                if (((AddEditIQAccountViewModel)(_objAddEditIQAccount.DataContext)).isSave)
                {
                    fillPricesheets();
                }
            }
            else
            {
                MVVMMessageService.ShowMessage("Please select an account.");
            }
        }
        private void newVandoSettings()
        {
            if (selectedGrid == "VandoSettings" && selectedIQAccount != null)
            {
                AddEditIQVandOSettings _objAddEditIQVandoSettings = new AddEditIQVandOSettings("New-IQVandoSettings", selectedIQVandoSetting, selectedIQAccount);
                _objAddEditIQVandoSettings.ShowDialog();

                if (((AddEditIQVandOSettingsViewModel)(_objAddEditIQVandoSettings.DataContext)).isSave)
                {
                    fillVandoSettings();
                }
            }
            else
            {
                MVVMMessageService.ShowMessage("Please select an account.");
            }
        }
        internal void delete()
        {
            List<int> selectedIds = new List<int>();
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (selectedGrid == "Accounts")
            {
                string message = "";
                if (selectedIQAccount != null)
                {
                    selectedIds = (from iqa in selectedImageQuixAccounts select iqa.Id).ToList();

                    if (selectedIds.Count == 1)
                    {
                        message = "Are you sure you want to delete a Account?";
                    }
                    else
                    {
                        message = "Are you sure you want to delete multiple Accounts?";
                    }
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgImageQuixAccountsData.Count();
                        //int deletedRecordsCount = selectedIds.Count;

                        clsImageQuix.deleteIQAccouts(db, selectedIds);
                        bindData();

                        //createDeletedRecordsLogFile("IQ Accounts", totalRecordsCount, deletedRecordsCount);
                    }
                }
            }
            else if (selectedGrid == "Pricesheet")
            {
                string message = "";
                if (selectedPricesheet != null)
                {
                    selectedIds = (from iqp in selectedPricesheets select iqp.Id).ToList();

                    if (selectedIds.Count == 1)
                    {
                        message = "Are you sure you want to delete a price sheet?";
                    }
                    else
                    {
                        message = "Are you sure you want to delete multiple price sheets?";
                    }
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgPricesheetsData.Count();
                        //int deletedRecordsCount = selectedIds.Count;

                        clsImageQuix.deletePricesheets(db, selectedIds, selectedIQAccount.Id);
                        fillPricesheets();

                        //createDeletedRecordsLogFile("IQ Pricesheets", totalRecordsCount, deletedRecordsCount);
                    }
                }
            }
            else if (selectedGrid == "VandoSettings")
            {
                string message = "";
                if (selectedIQVandoSetting != null)
                {
                    selectedIds = (from iqv in selectedIQVandoSettings select iqv.Id).ToList();

                    if (selectedIds.Count == 1)
                    {
                        message = "Are you sure you want to delete selected Vando setting?";
                    }
                    else
                    {
                        message = "Are you sure you want to delete multiple Vando Settings?";
                    }
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgIQVandoSettingsData.Count();
                        //int deletedRecordsCount = selectedIds.Count;

                        clsImageQuix.deleteVandoSettings(db, selectedIds, selectedIQAccount.Id);
                        fillVandoSettings();

                        //createDeletedRecordsLogFile("Vando settings", totalRecordsCount, deletedRecordsCount);
                    }
                }
            }
        }
        private void imageQuixAccountsMouseUp()
        {
            selectedGrid = "Accounts";
            setButtonVisibilityForCollections();
            if (selectedIQAccountIndex == 0)
                fillPricesheets();
            else
                fillVandoSettings();
        }
        private void priceSheetsMouseUp()
        {
            selectedGrid = "Pricesheet";
            setButtonVisibilityForCollections();
        }
        private void vandoSettingsMouseUp()
        {
            selectedGrid = "VandoSettings";
            setButtonVisibilityForCollections();
        }
        private void rowUpdate()
        {
            db.SubmitChanges();

            if (selectedIQVandoSetting.IsDefault == true)
            {
                clsImageQuix.updateIQVandoSettings(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedIQVandoSetting.Id, selectedIQAccount.Id);
                fillVandoSettings();
            }
        }
        #region group and search panels
        # region Search panel
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        internal void searchPanels()
        {
            if (selectedGrid == "Accounts")
            {
                if (imageQuixAccountSearchControl == null || isSearchControlVisible == false)
                {
                    imageQuixAccountSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    imageQuixAccountSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
                }
            }
            else if (selectedGrid == "Pricesheet")
            {
                if (pricesheetsSearchControl == null || isSearchControlVisible == false)
                {
                    pricesheetsSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    pricesheetsSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
                }
            }
            else if (selectedGrid == "VandoSettings")
            {
                if (vandoSettingsSearchControl == null || isSearchControlVisible == false)
                {
                    vandoSettingsSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    vandoSettingsSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
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
            if (selectedGrid == "Accounts")
            {
                if (imageQuixAccountShowGroupPanel)
                    imageQuixAccountShowGroupPanel = false;
                else
                    imageQuixAccountShowGroupPanel = true;
            }
            else if (selectedGrid == "Pricesheet")
            {
                if (pricesheetsShowGroupPanel)
                    pricesheetsShowGroupPanel = false;
                else
                    pricesheetsShowGroupPanel = true;
            }
            else if (selectedGrid == "VandoSettings")
            {
                if (vandoSettingsShowGroupPanel)
                    vandoSettingsShowGroupPanel = false;
                else
                    vandoSettingsShowGroupPanel = true;
            }
        }
        # endregion
        #endregion

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
        }
        #endregion
        #endregion
    }
}
