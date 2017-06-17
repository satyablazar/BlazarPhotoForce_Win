using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.Student_Management;
using PhotoForce.App_Code;
using PhotoForce.PhotoShoot_Management;
using System.Collections;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using System.Collections.ObjectModel;

namespace PhotoForce.View_Management.UserControls
{
    public class ViewPhotoShootViewModel : ViewModelBase
    {
        #region Initialization
        GetImagesWithoutStudent _objGetImagesWithoutStudent;
        #endregion

        #region Properties
        private ObservableCollection<PhotoShoot> _dgPhotoShootData;
        private PhotoShoot _selectedPhotoShoot;
        private List<PhotoShoot> _selectedPhotoShoots;
        private bool _photoShootTableShowGroupPanel;
        private ShowSearchPanelMode _photoShootSearchPanelMode;
        private SearchControl _photoShootSearchControl;
        private bool _isSearchControlVisible;

        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public SearchControl photoShootSearchControl
        {
            get { return _photoShootSearchControl; }
            set { _photoShootSearchControl = value; NotifyPropertyChanged("photoShootSearchControl"); }
        }
        public ShowSearchPanelMode photoShootSearchPanelMode
        {
            get { return _photoShootSearchPanelMode; }
            set { _photoShootSearchPanelMode = value; NotifyPropertyChanged("photoShootSearchPanelMode"); }
        }
        public bool photoShootTableShowGroupPanel
        {
            get { return _photoShootTableShowGroupPanel; }
            set { _photoShootTableShowGroupPanel = value; NotifyPropertyChanged("photoShootTableShowGroupPanel"); }
        }
        public List<PhotoShoot> selectedPhotoShoots
        {
            get { return _selectedPhotoShoots; }
            set { _selectedPhotoShoots = value; NotifyPropertyChanged("selectedPhotoShoots"); }
        }
        public PhotoShoot selectedPhotoShoot
        {
            get { return _selectedPhotoShoot; }
            set { _selectedPhotoShoot = value; NotifyPropertyChanged("selectedPhotoShoot"); }
        }
        public ObservableCollection<PhotoShoot> dgPhotoShootData
        {
            get { return _dgPhotoShootData; }
            set { _dgPhotoShootData = value; NotifyPropertyChanged("dgPhotoShootData"); }
        }
        #endregion

        #region Constructors
        public ViewPhotoShootViewModel()
        {
            selectedPhotoShoots = new List<PhotoShoot>();
            bindGrid();
        }
        #endregion

        #region Commands
        public RelayCommand PhotoShootGridDoubleClick
        {
            get
            {
                return new RelayCommand(editRecord);
            }
        }
        #endregion

        #region Methods
        internal void editRecord()
        {
            if (selectedPhotoShoot != null)
            {
                AddEditPhotoShoot addEditPhotoShoot = new AddEditPhotoShoot(selectedPhotoShoot.PhotoShotID, selectedPhotoShoot.PhotographyJob);
                addEditPhotoShoot.ShowDialog();
                if (((AddEditPhotoShootViewModel)(addEditPhotoShoot.DataContext)).isSave)
                {
                    int tempPhotoShootIndex = dgPhotoShootData.Count <= 1 ? 0 : dgPhotoShootData.IndexOf(selectedPhotoShoot);
                    dgPhotoShootData.Remove(selectedPhotoShoot);

                    dgPhotoShootData.Insert(tempPhotoShootIndex, ((AddEditPhotoShootViewModel)(addEditPhotoShoot.DataContext)).addedEditedPhotoShoot);
                    selectedPhotoShoot = ((AddEditPhotoShootViewModel)(addEditPhotoShoot.DataContext)).addedEditedPhotoShoot;
                    selectedPhotoShoots.Add(selectedPhotoShoot);
                 //   bindGrid();
                }
            }
        }

        public void deleteShoot()
        {
            try
            {
                string message = "", photoShootName = "";
                if (selectedPhotoShoots.Count != 0)
                {
                    int delPhotoId = 0;
                    ArrayList arrShootId = new ArrayList();
                    bool isError = false;
                    foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
                    {
                        try
                        {
                            int photoId = tempPhotoShoot.PhotoShotID;
                            photoShootName = tempPhotoShoot.PhotoShotName;
                            if (!arrShootId.Contains(photoId))
                            {
                                arrShootId.Add(photoId);
                            }
                        }
                        catch (Exception ex)
                        {
                            isError = true;
                            clsStatic.WriteExceptionLogXML(ex);
                        }
                    }
                    if (arrShootId.Count == 0)
                    {
                        MVVMMessageService.ShowMessage("please select photoshoot(s)");
                        return;
                    }
                    if (arrShootId.Count == 1)
                        message = "Are you sure you want to delete photoshoot " + photoShootName + "?";
                    else
                        message = "Are you sure you want to delete multiple photoshoots?";
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgPhotoShootData.Count();
                        //int deletedRecordsCount = arrShootId.Count;

                        delPhotoId = clsDashBoard.deleteBulkPhotoShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrShootId);
                        foreach (int imgId in arrShootId)
                        {
                            dgPhotoShootData.Remove(dgPhotoShootData.Where(i => i.PhotoShotID == imgId).First());
                        }
                        //bindGrid();
                        //createDeletedRecordsLogFile("Photoshoots", totalRecordsCount, deletedRecordsCount);
                    }
                    if (isError)
                        MVVMMessageService.ShowMessage(errorMessages.DELETION_WITH_ERRORS);
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
            try
            {
                if (selectedPhotoShoot != null)
                {
                    int shootID;
                    ArrayList arrPhotoShootId = new ArrayList();
                    foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
                    {
                        shootID = tempPhotoShoot.PhotoShotID;
                        if (!arrPhotoShootId.Contains(shootID))
                        {
                            arrPhotoShootId.Add(shootID);
                        }
                    }
                    BulkRenamePhotoShoot _objBulkRenamePhotoShoot = new BulkRenamePhotoShoot(arrPhotoShootId);
                    _objBulkRenamePhotoShoot.ShowDialog();
                    if (((BulkRenamePhotoShootViewModel)(_objBulkRenamePhotoShoot.DataContext)).isSave)
                    {
                        //dgPhotoShootData = clsDashBoard.getPhotoShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
                        refreshGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        internal void groupPanels()
        {
            if (photoShootTableShowGroupPanel)
                photoShootTableShowGroupPanel = false;
            else
                photoShootTableShowGroupPanel = true;
        }

        internal void searchPanels()
        {

            if (photoShootSearchControl == null || !isSearchControlVisible) //!PhotoShootTable.SearchControl.IsVisible
            {
                photoShootSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
            else
            {
                photoShootSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
            }
        }

        internal void refreshGrid()
        {
            bindGrid();
        }

        internal void imageWithoutStudent()
        {
            try
            {
                string PhotoShootName = "";
                if (selectedPhotoShoot != null)
                {
                    ArrayList arrShootID = new ArrayList();
                    foreach (PhotoShoot tempPhotoShoot in selectedPhotoShoots)
                    {
                        try
                        {
                            int PhotoID = tempPhotoShoot.PhotoShotID;
                            PhotoShootName = tempPhotoShoot.PhotoShotName;
                            if (!arrShootID.Contains(PhotoID))
                            {
                                arrShootID.Add(PhotoID);
                            }
                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                        }
                    }
                    if (arrShootID.Count > 0)
                    {
                        _objGetImagesWithoutStudent = new GetImagesWithoutStudent(arrShootID);
                        _objGetImagesWithoutStudent.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        public void bindGrid()
        {
            //string allJobId = "";
            //List<int> JobIds = clsDashBoard.getAllJobs(clsSchool.defaultSchoolId, new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            //if (JobIds.Count >= 0)
            //{
            //    if (JobIds.Count != 0)
            //    {
            //        foreach (int jobid in JobIds)
            //        {
            //            allJobId += jobid + ",";
            //        }
            //        allJobId = allJobId.Substring(0, allJobId.Length - 1);
            //    }
            //    else
            //        allJobId = "0";
            //    //dgPhotoShootData = new ObservableCollection<PhotoShoot>(clsDashBoard.getPhotoShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allJobId));
            //}
            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            dgPhotoShootData = new ObservableCollection<PhotoShoot> ((from ps in db.PhotoShoots orderby ps.PhotoShotID select ps).ToList());
        }

        internal void setButtonVisibility()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
            (System.Windows.Application.Current as App).isEditVisible = true;
            (System.Windows.Application.Current as App).isDeleteVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (System.Windows.Application.Current as App).isDragVisible = true; (System.Windows.Application.Current as App).isSearchVisible = true;
            (System.Windows.Application.Current as App).isRefreshVisible = true; 
        }
        #endregion
    }
}
