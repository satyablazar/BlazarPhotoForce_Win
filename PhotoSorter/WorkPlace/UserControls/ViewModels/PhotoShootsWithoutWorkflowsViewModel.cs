using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class PhotoShootsWithoutWorkflowsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        int prevStudioID = 0;
        string prevPhotography = "";
        #endregion

        #region Properties
        ObservableCollection<PhotoShoot> _selectedPhotoShoots;
        ObservableCollection<string> _cbPhotographyJobData;
        ObservableCollection<Studio> _cbStudioData;
        ObservableCollection<PhotoShoot> _dgPhotoShootData;
        Studio _selectedStudio;
        string _selectedPhotographyJob;
        private bool _photoshootTableShowGroupPanel;
        private bool _isPhotoshootSearchControlVisible;
        private ShowSearchPanelMode _photoshootTableSearchPanelMode;
        private SearchControl _photoshootTableSearchControl;

        public bool photoshootTableShowGroupPanel
        {
            get { return _photoshootTableShowGroupPanel; }
            set { _photoshootTableShowGroupPanel = value; NotifyPropertyChanged(); }
        }
        public bool isPhotoshootSearchControlVisible
        {
            get { return _isPhotoshootSearchControlVisible; }
            set { _isPhotoshootSearchControlVisible = value; NotifyPropertyChanged(); }
        }
        public ShowSearchPanelMode photoshootTableSearchPanelMode
        {
            get { return _photoshootTableSearchPanelMode; }
            set { _photoshootTableSearchPanelMode = value; NotifyPropertyChanged(); }
        }
        public SearchControl photoshootTableSearchControl
        {
            get { return _photoshootTableSearchControl; }
            set { _photoshootTableSearchControl = value; NotifyPropertyChanged(); }
        }
        public string selectedPhotographyJob
        {
            get { return _selectedPhotographyJob; }
            set 
            { 
                _selectedPhotographyJob = value; NotifyPropertyChanged();
                if (!string.IsNullOrEmpty(_selectedPhotographyJob) && prevPhotography != _selectedPhotographyJob)
                {
                    prevPhotography = _selectedPhotographyJob;

                    if (prevStudioID > 0)
                    {
                        fillPhotoShootData(selectedStudio.StudioName, _selectedPhotographyJob);
                    }
                }
            }
        }
        public Studio selectedStudio
        {
            get { return _selectedStudio; }
            set 
            {
                _selectedStudio = value; NotifyPropertyChanged();
                if (_selectedStudio != null && prevStudioID != _selectedStudio.Id)
                {
                    prevStudioID = _selectedStudio.Id;
                    if (_selectedStudio.Id > 0 && !string.IsNullOrEmpty(prevPhotography))
                    {
                        fillPhotoShootData(_selectedStudio.StudioName, selectedPhotographyJob);
                    }
                }
            }
        }
        public ObservableCollection<PhotoShoot> dgPhotoShootData
        {
            get { return _dgPhotoShootData; }
            set { _dgPhotoShootData = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<Studio> cbStudioData
        {
            get { return _cbStudioData; }
            set { _cbStudioData = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<string> cbPhotographyJobData
        {
            get { return _cbPhotographyJobData; }
            set { _cbPhotographyJobData = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<PhotoShoot> selectedPhotoShoots
        {
            get { return _selectedPhotoShoots; }
            set { _selectedPhotoShoots = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructor
        public PhotoShootsWithoutWorkflowsViewModel()
        {
            selectedPhotoShoots = new ObservableCollection<PhotoShoot>();
            cbPhotographyJobData = new ObservableCollection<string>();
            cbStudioData = new ObservableCollection<Studio>();
            dgPhotoShootData = new ObservableCollection<PhotoShoot>();
            loadData();
        }
        #endregion

        #region Methods
        internal void loadData()
        {
            selectedStudio = null;
            selectedPhotographyJob = null;
            cbPhotographyJobData = new ObservableCollection<string>((from pj in db.PhotographyJobs select pj.JobName).ToList());

            cbPhotographyJobData = new ObservableCollection<string>(clsDashBoard.getPhotographyJobData(db));

            cbStudioData = new ObservableCollection<Studio>((from stu in db.Studios select stu).ToList());
            dgPhotoShootData = new ObservableCollection<PhotoShoot>(clsDashBoard.getPhotoShootsWithoutWorkflows(db,"","").ToList());

            setButtonVisibility();
        }
        private void fillPhotoShootData(string studioName, string PhotographyJobName)
        {
            dgPhotoShootData = new ObservableCollection<PhotoShoot>(clsDashBoard.getPhotoShootsWithoutWorkflows(db, studioName, PhotographyJobName).ToList());
        }

        /// <summary>
        /// This method is used to group panels based on selected column
        /// </summary>
        public void groupPanels()
        {
            try
            {

                if (photoshootTableShowGroupPanel)
                    photoshootTableShowGroupPanel = false;
                else
                    photoshootTableShowGroupPanel = true;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        public void searchPanels()
        {
            try
            {
                if (isPhotoshootSearchControlVisible == false || photoshootTableSearchControl == null)
                {
                    photoshootTableSearchPanelMode = ShowSearchPanelMode.Always; isPhotoshootSearchControlVisible = true;
                }
                else
                {
                    photoshootTableSearchPanelMode = ShowSearchPanelMode.Never; isPhotoshootSearchControlVisible = false;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        private void setButtonVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            
        }

        #endregion
    }
}
