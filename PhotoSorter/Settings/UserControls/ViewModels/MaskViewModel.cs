using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.Mask_Management;
using PhotoForce.Mask_Management.Views;
using System.Collections;
using System.Data;
using PhotoForce.Extensions;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using System.Collections.ObjectModel;

namespace PhotoForce.Settings.UserControls
{
    public class MaskViewModel : ViewModelBase
    {
        # region Initialization and Declaration
        int? getMaskId;
        AddEditMasks _objAddEditMasks;
        BulkRenameMaks _objBulkRenameMaks;
        # endregion

        #region Properties
        ObservableCollection<MaskDetailsItem> _dgAllMaskDetailsData;
        MaskDetailsItem _selectedMask;
        List<MaskDetailsItem> _selectedMasks;
        private bool _maskDetailShowGroupPanel;
        private ShowSearchPanelMode _maskDetailSearchPanelMode;
        private SearchControl _maskDetailSearchControl;
        private bool _isSearchControlVisible;

        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public SearchControl maskDetailSearchControl
        {
            get { return _maskDetailSearchControl; }
            set { _maskDetailSearchControl = value; NotifyPropertyChanged("searchControl"); }
        }
        public ShowSearchPanelMode maskDetailSearchPanelMode
        {
            get { return _maskDetailSearchPanelMode; }
            set { _maskDetailSearchPanelMode = value; NotifyPropertyChanged("maskDetailSearchPanelMode"); }
        }
        public bool maskDetailShowGroupPanel
        {
            get { return _maskDetailShowGroupPanel; }
            set { _maskDetailShowGroupPanel = value; NotifyPropertyChanged("maskDetailShowGroupPanel"); }
        }
        public List<MaskDetailsItem> selectedMasks
        {
            get { return _selectedMasks; }
            set { _selectedMasks = value; NotifyPropertyChanged("selectedMasks"); }
        }
        public MaskDetailsItem selectedMask
        {
            get { return _selectedMask; }
            set { _selectedMask = value; NotifyPropertyChanged("selectedMask"); }
        }
        public ObservableCollection<MaskDetailsItem> dgAllMaskDetailsData
        {
            get { return _dgAllMaskDetailsData; }
            set { _dgAllMaskDetailsData = value; NotifyPropertyChanged("dgAllMaskDetailsData"); }
        }
        #endregion

        # region Constructor
        public MaskViewModel()
        {
            selectedMasks = new List<MaskDetailsItem>(); dgAllMaskDetailsData = new ObservableCollection<MaskDetailsItem>();
            getAllMaskData();
        }
        # endregion

        #region Commands
        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(deleteMask);
            }
        }
        public RelayCommand MaskDetailsGridDoubleClickCommand
        {
            get
            {
                return new RelayCommand(editRecord);
            }
        }
        #endregion

        #region Methods

        # region Bind Grid
        private void getAllMaskData()
        {
            try
            {
                dgAllMaskDetailsData = new ObservableCollection<MaskDetailsItem>(clsDashBoard.BindMasks(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString)));
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }

        }
        # endregion

        internal void GetDefaultSchool()
        {
            getAllMaskData();
        }

        # region Delete masks
        internal void deleteMask()
        {
            try
            {
                string message = "", maskname = "";
                int delPhotoId = 0;
                ArrayList arrjobId = new ArrayList();
                foreach (MaskDetailsItem mask in selectedMasks)
                {
                    int Maskid = mask.maskId == null ? 0 : (int)mask.maskId;
                    maskname = mask.maskName;
                    if (!arrjobId.Contains(Maskid))
                    {
                        arrjobId.Add(Maskid);
                    }
                }
                if (arrjobId.Count == 1)
                {
                    message = "Are you sure you want to delete mask " + maskname + "?";
                }
                else
                {
                    message = "Are you sure you want to delete multiple masks?";
                }
                if (arrjobId.Count != 0)
                {
                    //int totalRecordsCount = dgAllMaskDetailsData.Count();
                    //int deletedRecordsCount = arrjobId.Count;

                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        delPhotoId = clsDashBoard.deleteMultipleMasks(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrjobId);
                        if (delPhotoId >= 0)
                        {
                            foreach (int mskId in arrjobId)
                            {
                                dgAllMaskDetailsData.Remove(dgAllMaskDetailsData.Where(i => i.maskId == mskId).First());
                            }
                        }
                        //getAllMaskData();

                        //createDeletedRecordsLogFile("Masks", totalRecordsCount, deletedRecordsCount);
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Add new record
        public void newRecord()
        {
            try
            {
                _objAddEditMasks = new AddEditMasks(0, "");
                _objAddEditMasks.ShowDialog();
                if (((AddEditMasksViewModel)(_objAddEditMasks.DataContext)).isSave)
                {
                    //getAllMaskData();
                    MaskDetailsItem tempMaskDetailsItem = ((AddEditMasksViewModel)(_objAddEditMasks.DataContext))._objMaskDetailsItem;
                    dgAllMaskDetailsData.Insert(dgAllMaskDetailsData.Count <= 1 ? 0 : dgAllMaskDetailsData.IndexOf(selectedMask), tempMaskDetailsItem);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Update mask in bulk
        internal void BulkRename()
        {
            try
            {
                int? Maskid = 0;
                ArrayList arrmaskId = new ArrayList();
                foreach (MaskDetailsItem mask in selectedMasks)
                {
                    Maskid = mask.maskId == null ? 0 : (int)mask.maskId;
                    if (!arrmaskId.Contains(Maskid))
                    {
                        arrmaskId.Add(Maskid);
                    }
                }
                if (arrmaskId.Count != 0)
                {
                    _objBulkRenameMaks = new BulkRenameMaks(arrmaskId);
                    _objBulkRenameMaks.ShowDialog();
                    if (((BulkRenameMaksViewModel)(_objBulkRenameMaks.DataContext)).isSave)
                        getAllMaskData();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Edit records
        internal void editRecord()
        {
            try
            {
                if (selectedMask != null)
                {
                    getMaskId = selectedMask.maskId == null ? 0 : selectedMask.maskId;
                    _objAddEditMasks = new AddEditMasks((int)getMaskId, selectedMask.maskName);
                    _objAddEditMasks.ShowDialog();
                    if (((AddEditMasksViewModel)(_objAddEditMasks.DataContext)).isSave)
                    {
                        int tempMaskDetailsIndex = dgAllMaskDetailsData.Count <= 1 ? 0 : dgAllMaskDetailsData.IndexOf(selectedMask);

                        MaskDetailsItem tempMaskDetailsItem = ((AddEditMasksViewModel)(_objAddEditMasks.DataContext))._objMaskDetailsItem;
                        //getAllMaskData();
                        dgAllMaskDetailsData.Remove(selectedMask);
                        dgAllMaskDetailsData.Insert(tempMaskDetailsIndex, tempMaskDetailsItem);
                        selectedMask = tempMaskDetailsItem;
                        selectedMasks.Add(selectedMask);
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Refresh grids
        internal void refreshGrid()
        {
            getAllMaskData();
        }
        # endregion

        # region Search panel
        internal void searchPanels()
        {
            if (maskDetailSearchControl == null || !isSearchControlVisible) //|| !AllMaskDetTableView.SearchControl.IsVisible)
            {
                maskDetailSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
            else
            {
                maskDetailSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
            }
        }
        # endregion

        # region Group panels
        internal void groupPanels()
        {
            if (maskDetailShowGroupPanel)
                maskDetailShowGroupPanel = false;
            else
                maskDetailShowGroupPanel = true;
        }
        # endregion

        internal void setButtonsVisibility()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
            (System.Windows.Application.Current as App).isNewVisible = true;
            (System.Windows.Application.Current as App).isEditVisible = true;
            (System.Windows.Application.Current as App).isDeleteVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (System.Windows.Application.Current as App).isDragVisible = true; (System.Windows.Application.Current as App).isSearchVisible = true;
        }

        #endregion
    }
}
