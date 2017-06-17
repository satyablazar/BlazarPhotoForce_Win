using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.PhotographyJobManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.WorkPlace.UserControls
{
    public class PhotographyJobViewModel : ViewModelBase
    {
        #region Initialization
        AddNewPhotographyJob _objAddNewPhotographyJob;
        #endregion

        #region Properties
        ObservableCollection<PhotoForce.App_Code.PhotographyJob> _dgPhotographyJobData;
        PhotoForce.App_Code.PhotographyJob _selectedPhotographyJob;
        List<PhotoForce.App_Code.PhotographyJob> _selectedPhotographyJobs;
        bool _isSearchControlVisible;
        SearchControl _photographyJobSearchControl;
        ShowSearchPanelMode _photographyJobSearchPanelMode;
        bool _photographyJobShowGroupPanel;

        public ShowSearchPanelMode photographyJobSearchPanelMode
        {
            get { return _photographyJobSearchPanelMode; }
            set { _photographyJobSearchPanelMode = value; NotifyPropertyChanged("photographyJobSearchPanelMode"); }
        }
        public SearchControl photographyJobSearchControl
        {
            get { return _photographyJobSearchControl; }
            set { _photographyJobSearchControl = value; NotifyPropertyChanged("photographyJobSearchControl"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public bool photographyJobShowGroupPanel
        {
            get { return _photographyJobShowGroupPanel; }
            set { _photographyJobShowGroupPanel = value; NotifyPropertyChanged("photographyJobShowGroupPanel"); }
        }
        public List<PhotoForce.App_Code.PhotographyJob> selectedPhotographyJobs
        {
            get { return _selectedPhotographyJobs; }
            set { _selectedPhotographyJobs = value; NotifyPropertyChanged("selectedPhotographyJobs"); }
        }

        public PhotoForce.App_Code.PhotographyJob selectedPhotographyJob
        {
            get { return _selectedPhotographyJob; }
            set { _selectedPhotographyJob = value; NotifyPropertyChanged("selectedPhotographyJob"); }
        }

        public ObservableCollection<PhotoForce.App_Code.PhotographyJob> dgPhotographyJobData
        {
            get { return _dgPhotographyJobData; }
            set { _dgPhotographyJobData = value; NotifyPropertyChanged("dgPhotographyJobData"); }
        }
        #endregion

        #region Constructors
        public PhotographyJobViewModel()
        {
            selectedPhotographyJobs = new List<PhotoForce.App_Code.PhotographyJob>();
            refreshGrid();
        }
        #endregion

        #region Commands
        public RelayCommand PhotographyJobDoubleClick
        {
            get
            {
                return new RelayCommand(photographyJobDoubleClick);
            }
        }
        #endregion

        #region Methods
        internal void refreshGrid()
        {
            bindGrid();
        }
        internal void bindGrid()
        {
            try
            {
                dgPhotographyJobData = new ObservableCollection<App_Code.PhotographyJob>(clsDashBoard.getJobs(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId));
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void photographyJobDoubleClick()
        {
            editRecord();
        }
        internal void bulkRename()
        {
            if (selectedPhotographyJob != null)
            {
                int jobId;
                ArrayList joblID = new ArrayList();
                foreach (PhotoForce.App_Code.PhotographyJob photographyJob in selectedPhotographyJobs)
                {
                    try
                    {
                        jobId = Convert.ToInt32(photographyJob.ID);
                        if (!joblID.Contains(jobId))
                        {
                            joblID.Add(jobId);
                        }
                    }
                    catch (Exception ex)
                    { clsStatic.WriteExceptionLogXML(ex); }
                }
                BulkRenameJob _objBulkRenameJob = new BulkRenameJob(joblID);
                _objBulkRenameJob.ShowDialog();
                if (((BulkRenameJobViewModel)(_objBulkRenameJob.DataContext)).isSave)
                {
                    bindGrid();
                }
            }
        }
        internal void deleteJob()
        {
            string message = "", jobName = "";
            if (selectedPhotographyJob != null)
            {
                int delPhotoId = 0;
                ArrayList arrjobID = new ArrayList();
                foreach (PhotoForce.App_Code.PhotographyJob photographyJob in selectedPhotographyJobs)
                {
                    try
                    {
                        int PhotoID = Convert.ToInt32(photographyJob.ID);
                        jobName = Convert.ToString(photographyJob.JobName);
                        if (!arrjobID.Contains(PhotoID))
                        {
                            arrjobID.Add(PhotoID);
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                }
                if (arrjobID.Count == 0)
                {
                    MVVMMessageService.ShowMessage("please select photography job(s)");
                    return;
                }
                if (arrjobID.Count == 1)
                {
                    message = "Are you sure you want to delete photography job " + jobName + "?";
                }
                else
                {
                    message = "Are you sure you want to delete multiple photography jobs?";
                }
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    //int totalRecordsCount = dgPhotographyJobData.Count();
                    //int deletedRecordsCount = arrjobID.Count;

                    delPhotoId = clsDashBoard.deletePhotographyJob(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrjobID);
                    if (delPhotoId != 0)
                    {
                        foreach (int phjobId in arrjobID)
                        {
                            dgPhotographyJobData.Remove(dgPhotographyJobData.Where(i => i.ID == phjobId).First());
                        }
                        //createDeletedRecordsLogFile("PhotographyJob", totalRecordsCount, deletedRecordsCount);
                    }
                    //bindGrid();
                }
            }
        }
        internal void newRecord()
        {
            _objAddNewPhotographyJob = new AddNewPhotographyJob();
            _objAddNewPhotographyJob.ShowDialog();
            if (((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).isSave)
            {
                dgPhotographyJobData.Insert(selectedPhotographyJob == null ? 0 : dgPhotographyJobData.IndexOf(selectedPhotographyJob), ((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).newCollectivePhotographyJob);  //addEditPhotographyJob
                //selectedPhotographyJob = ((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).addEditPhotographyJob;
                //bindGrid();
            }
        }
        internal void editRecord()
        {   
            if (selectedPhotographyJob != null)
            {
                int JobID = selectedPhotographyJob.ID;

                _objAddNewPhotographyJob = new AddNewPhotographyJob(selectedPhotographyJob.JobName, Convert.ToInt32(selectedPhotographyJob.StartYear), Convert.ToInt32(selectedPhotographyJob.EndYear), JobID);
                _objAddNewPhotographyJob.ShowDialog();

                if (((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).isSave)
                {
                    int tempPhotographyJobIndex = dgPhotographyJobData.Count <= 1 ? 0 : dgPhotographyJobData.IndexOf(selectedPhotographyJob);
                    dgPhotographyJobData.Remove(selectedPhotographyJob);

                    dgPhotographyJobData.Insert(tempPhotographyJobIndex, ((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).addEditPhotographyJob);
                    
                    //dgPhotographyJobData.Insert(dgPhotographyJobData.Count <= 1 ? 0 : dgPhotographyJobData.IndexOf(selectedPhotographyJob), ((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).addEditPhotographyJob);
                    selectedPhotographyJob = ((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).addEditPhotographyJob;
                    selectedPhotographyJobs.Add(selectedPhotographyJob);
                    //bindGrid();
                }
            }
        }
        internal void searchPanels()
        {
            if (photographyJobSearchControl == null || !isSearchControlVisible) //!PhotoJobTableView.SearchControl.IsVisible
            {
                photographyJobSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
            else
            {
                photographyJobSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
            }
        }
        public void groupPanels()
        {
            try
            {
                if (photographyJobShowGroupPanel)
                    photographyJobShowGroupPanel = false;
                else
                    photographyJobShowGroupPanel = true;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        internal void importDataFile()
        {
            //we are not using "Import Student Images" button    //mohan
            //try
            //{
            //    if (selectedPhotographyJob != null)
            //    {
            //        int jobID = selectedPhotographyJob.ID;
            //        importStudentImages objImportStudentImages = new importStudentImages(jobID, "");
            //        objImportStudentImages.ShowDialog();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    clsStatic.WriteExceptionLogXML(ex);
            //    MessageBox.Show(ex.Message);
            //}
        }
        internal void setButtonsVisibility()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
            (System.Windows.Application.Current as App).isNewVisible = true;
            (System.Windows.Application.Current as App).isEditVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (System.Windows.Application.Current as App).isDeleteVisible = true; (System.Windows.Application.Current as App).isRefreshVisible = true;
            (System.Windows.Application.Current as App).isDragVisible = true; (System.Windows.Application.Current as App).isSearchVisible = true;
        }
        #endregion

    }
}
