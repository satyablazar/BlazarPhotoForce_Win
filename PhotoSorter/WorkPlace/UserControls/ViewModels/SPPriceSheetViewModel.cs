using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.ImageQuixManagement;
using PhotoForce.MVVM;
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
    public class SPPriceSheetViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        #endregion

        #region Properties
        ObservableCollection<SimplePhotoPriceSheet> _dgSPPricesheetsData;
        ObservableCollection<SimplePhotoPriceSheet> _selectedSPPricesheets;
        SimplePhotoPriceSheet _selectedSPPricesheet;

        public ObservableCollection<SimplePhotoPriceSheet> dgSPPricesheetsData
        {
            get { return _dgSPPricesheetsData; }
            set { _dgSPPricesheetsData = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<SimplePhotoPriceSheet> selectedSPPricesheets
        {
            get { return _selectedSPPricesheets; }
            set { _selectedSPPricesheets = value; NotifyPropertyChanged(); }
        }
        public SimplePhotoPriceSheet selectedSPPricesheet
        {
            get { return _selectedSPPricesheet; }
            set { _selectedSPPricesheet = value; NotifyPropertyChanged(); }
        }
        #region Group & Search panels
        bool _pricesheetsShowGroupPanel;

        public bool pricesheetsShowGroupPanel
        {
            get { return _pricesheetsShowGroupPanel; }
            set { _pricesheetsShowGroupPanel = value; NotifyPropertyChanged(); }
        }

        ShowSearchPanelMode _pricesheetsSearchPanelMode;
        SearchControl _pricesheetsSearchControl;
        bool _isSearchControlVisible;

        public ShowSearchPanelMode pricesheetsSearchPanelMode
        {
            get { return _pricesheetsSearchPanelMode; }
            set { _pricesheetsSearchPanelMode = value; NotifyPropertyChanged(); }
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

        public SPPriceSheetViewModel()
        {
            dgSPPricesheetsData = new ObservableCollection<SimplePhotoPriceSheet>();
            selectedSPPricesheets = new ObservableCollection<SimplePhotoPriceSheet>();
            bindData();
        }

        #region Commands
        public RelayCommand SPPriceSheetTableKeyUpCommand
        {
            get
            {
                return new RelayCommand(SPPriceSheetTableKeyUp);
            }
        }
        public RelayCommand RowUpdateCommand
        {
            get
            {
                return new RelayCommand(inlineGridEdit);
            }
        }
        #endregion

        #region Methods
        internal void bindData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            dgSPPricesheetsData = new ObservableCollection<SimplePhotoPriceSheet>((from SPP in db.SimplePhotoPriceSheets orderby SPP.Id select SPP).ToList());
            if (dgSPPricesheetsData.Count > 0)
                selectedSPPricesheet = dgSPPricesheetsData.First();
        }
        internal void newPriceSheet()
        {
            AddEditPriceSheet _objAddEditPriceSheet = new AddEditPriceSheet();
            _objAddEditPriceSheet.ShowDialog();

            bindData();
            if (dgSPPricesheetsData.Count > 0)
                selectedSPPricesheet = dgSPPricesheetsData[dgSPPricesheetsData.Count - 1];
        }
        internal void editPriceSheet()
        {
            if (selectedSPPricesheet != null)
            {
                AddEditPriceSheet _objAddEditPriceSheet = new AddEditPriceSheet(selectedSPPricesheet);
                _objAddEditPriceSheet.ShowDialog();

                if (((AddEditPriceSheetViewModel)(_objAddEditPriceSheet.DataContext)).isSave)
                {
                    // selectedUser["UserName"] = (((AddEditUsersViewModel)(_objAddEditUsers.DataContext)).userName).ToString();
                    try
                    {
                        dgSPPricesheetsData.Insert(dgSPPricesheetsData.Count <= 1 ? 0 : dgSPPricesheetsData.IndexOf(selectedSPPricesheet), ((AddEditPriceSheetViewModel)(_objAddEditPriceSheet.DataContext))._objSPPriceSheet);
                        dgSPPricesheetsData.Remove(selectedSPPricesheet);
                        selectedSPPricesheet = ((AddEditPriceSheetViewModel)(_objAddEditPriceSheet.DataContext))._objSPPriceSheet;
                        selectedSPPricesheets.Add(selectedSPPricesheet);
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }
            }
        }
        internal void deletePriceSheet()
        {
            if (selectedSPPricesheets.Count > 0)
            {
                try
                {
                    string message = "";
                    if (selectedSPPricesheets.Count == 1)
                        message = "Are you sure you want to delete price sheet '" + selectedSPPricesheet.SPPriceSheetId.ToString() + "' ?";
                    else
                        message = "Are you sure you want to delete selected price sheets ?";
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //db.SubmitChanges();

                        ArrayList selectedIds = new ArrayList();
                        foreach (SimplePhotoPriceSheet tempUser in selectedSPPricesheets)
                            selectedIds.Add(tempUser.Id);

                        //int totalRecordsCount = dgSPPricesheetsData.Count();
                        //int deletedRecordsCount = selectedIds.Count;

                        clsDashBoard.deleteSPPriceSheets(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedIds);
                        bindData();
                        if (dgSPPricesheetsData.Count > 0)
                            selectedSPPricesheet = dgSPPricesheetsData[dgSPPricesheetsData.Count - 1];
                        //createDeletedRecordsLogFile("Price sheets", totalRecordsCount, deletedRecordsCount);
                    }
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
        }

        #region group and search panels
        # region Search panel
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        internal void searchPanels()
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
        # endregion

        # region Group panels
        /// <summary>
        /// This method is used to group panels based on selected column
        /// </summary>
        internal void groupPanels()
        {
            if (pricesheetsShowGroupPanel)
                pricesheetsShowGroupPanel = false;
            else
                pricesheetsShowGroupPanel = true;
        }
        # endregion
        #endregion
        private void inlineGridEdit()
        {
            try
            {
                db.SubmitChanges();
                //clsDashBoard.updateUser(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(selectedUser.Id), selectedUser.UserName.ToString());
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void SPPriceSheetTableKeyUp()
        {
            setButtonVisibility();
        }
        #region Buttons visibility
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
        #endregion
    }
}
