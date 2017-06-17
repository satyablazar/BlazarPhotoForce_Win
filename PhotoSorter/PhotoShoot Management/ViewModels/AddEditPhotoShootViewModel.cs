using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.PhotographyJobManagement;
using Ookii.Dialogs.Wpf;
using System.Collections.ObjectModel;
using PhotoForce.Extensions;

namespace PhotoForce.PhotoShoot_Management
{
    public class AddEditPhotoShootViewModel : ViewModelBase
    {
        # region Initialization and Declaration
        internal int photoShootId = 0;
        int photographyJobId = 0;
        string startPath = "";
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        string jobName = "";
        int schoolId = 0;
        public bool isSave = false;
        public PhotoShoot addEditPhotoShoot;
        # endregion

        #region Properties
        private string _photoShootName;
        private DateTime _photoShootDate;

        private IEnumerable<PhotographyJob> _cbPhotographyJobData;
        private int? _selectedJobId;
        PhotoShoot _addedEditedPhotoShoot;
        bool _photoShootPathEnable;
        string _photoShootPath;
        string _txtJob;
        bool _isScheduledChecked;
        bool _isOnCalenderChecked;
        private ObservableCollection<PhotoshootTypeTable> _cbPhotoshootTypeData;
        private int? _selectedPhotoshootType;
        private Season _selectedSeason;

        public Season selectedSeason
        {
            get { return _selectedSeason; }
            set { _selectedSeason = value; NotifyPropertyChanged(); }
        }
        public int? selectedPhotoshootType
        {
            get { return _selectedPhotoshootType; }
            set { _selectedPhotoshootType = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<PhotoshootTypeTable> cbPhotoshootTypeData
        {
            get { return _cbPhotoshootTypeData; }
            set { _cbPhotoshootTypeData = value; NotifyPropertyChanged(); }
        }
        public bool isOnCalenderChecked
        {
            get { return _isOnCalenderChecked; }
            set { _isOnCalenderChecked = value; NotifyPropertyChanged(); }
        }
        public bool isScheduledChecked
        {
            get { return _isScheduledChecked; }
            set { _isScheduledChecked = value; NotifyPropertyChanged(); }
        }
        public string txtJob
        {
            get { return _txtJob; }
            set { _txtJob = value; NotifyPropertyChanged("txtJob"); }
        }
        public string photoShootPath
        {
            get { return _photoShootPath; }
            set { _photoShootPath = value; NotifyPropertyChanged("photoShootPath"); }
        }
        public bool photoShootPathEnable
        {
            get { return _photoShootPathEnable; }
            set { _photoShootPathEnable = value; NotifyPropertyChanged("photoShootPathEnable"); }
        }
        public PhotoShoot addedEditedPhotoShoot
        {
            get { return _addedEditedPhotoShoot; }
            set { _addedEditedPhotoShoot = value; NotifyPropertyChanged("addedEditedPhotoShoot"); }
        }

        public int? selectedJobId
        {
            get { return _selectedJobId; }
            set { _selectedJobId = value; NotifyPropertyChanged("selectedJobId"); }
        }
        public IEnumerable<PhotographyJob> cbPhotographyJobData
        {
            get { return _cbPhotographyJobData; }
            set { _cbPhotographyJobData = value; NotifyPropertyChanged("cbPhotographyJobData"); }
        }
        public DateTime photoShootDate
        {
            get { return _photoShootDate; }
            set { _photoShootDate = value; NotifyPropertyChanged("photoShootDate"); }
        }
        public string photoShootName
        {
            get { return _photoShootName; }
            set { _photoShootName = value; NotifyPropertyChanged("photoShootName"); }
        }
        #endregion

        #region Constructors
        public AddEditPhotoShootViewModel(int tempPhotoShootId, PhotographyJob tempPhotographyId)
        {
            try
            {
                photoShootPathEnable = true;

                photoShootId = tempPhotoShootId;
                photographyJobId = tempPhotographyId.ID;
                schoolId = (int)tempPhotographyId.SchoolID;

                //if (photographyJobId != 0)
                //{
                //    PhotoSorterDBModelDataContext db1 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                //    PhotographyJob objPhotoGraphyJob = clsDashBoard.getJobName(db1, photographyJobId);
                //    //startPath = ((Setting)clsDashBoard.getSettingByName(db1, "ImageFolderLocation")).settingValue;        why this code??
                //    //startPath += "\\" + objPhotoGraphyJob.School.SchoolName + "\\" + objPhotoGraphyJob.JobName + "\\";
                //}                

                if (photoShootId != 0)
                    BindPhotoShootControls();
                BindPhotoJobs(schoolId);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        public AddEditPhotoShootViewModel()
        {
            BindPhotoJobs(clsSchool.defaultSchoolId);
            photoShootDate = DateTime.Now;

            photoShootPathEnable = false;
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(createUpdatePhotoShoot);
            }
        }
        public RelayCommand NewSchoolYearCommand
        {
            get
            {
                return new RelayCommand(newSchoolYear);
            }
        }
        public RelayCommand BrowsePhotoShootFolderCommand
        {
            get
            {
                return new RelayCommand(browseLocation);
            }
        }
        public RelayCommand NewShootTypeCommand
        {
            get
            {
                return new RelayCommand(createNewShootType);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        #endregion

        #region Methods

        # region Fill controls
        public void BindPhotoShootControls()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                IEnumerable<PhotoShoot> photoShootDetails = clsDashBoard.getPhotoShootDetails(db, photoShootId);                
                photoShootName = photoShootDetails.First().PhotoShotName;
                DateTime dtt = new DateTime();
                DateTime.TryParse(photoShootDetails.First().PhotoShotDate.ToString(), out dtt);
                photoShootDate = dtt;
                photoShootPath = photoShootDetails.First().ImageFolder;
                jobName = photoShootDetails.First().PhotographyJob.JobName;
                txtJob = photoShootDetails.First().Job_ == null ? "" : photoShootDetails.First().Job_;
                isOnCalenderChecked = photoShootDetails.First().OnCalendar == null ? false : (bool)photoShootDetails.First().OnCalendar;
                isScheduledChecked = photoShootDetails.First().Scheduled == null ? false : (bool)photoShootDetails.First().Scheduled;
                selectedPhotoshootType = photoShootDetails.First().PhotoshootType;
                selectedSeason = photoShootDetails.First().Season == null ? Season.Fall : StringToEnumConverter.ToEnum<Season>(photoShootDetails.First().Season);

                //if (cbPhotoshootTypeData.Count > 0 && selectedPhotoshootType == null)
                //{
                //    selectedPhotoshootType = cbPhotoshootTypeData.FirstOrDefault().Id;
                //}

            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void BindPhotoJobs(int schoolId)
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                cbPhotoshootTypeData = new ObservableCollection<PhotoshootTypeTable>((from pst in db.PhotoshootTypeTables select pst).ToList());
                cbPhotographyJobData = clsDashBoard.getJobs(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), schoolId);

                if (photographyJobId != 0)
                {
                    selectedJobId = (from n in cbPhotographyJobData where n.ID == photographyJobId select n.ID).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

        }
        # endregion

        private void createUpdatePhotoShoot()
        {
            try
            {
                //string ImageFolderPath = ((Setting)clsDashBoard.getSettingByName(db1, "ImageFolderLocation")).settingValue;       //why this code???      //mohan
                if (photoShootName != "" && photoShootDate.ToShortDateString() != "" && selectedJobId > 0)
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    if (photoShootId != 0)
                    {
                        int tempPhotoShootID = 0;

                        #region Condition A
                        //Update
                        addEditPhotoShoot = new PhotoShoot();
                        addEditPhotoShoot = clsDashBoard.updatePhotoShoot(db, photoShootId);
                        addEditPhotoShoot.PhotoShotName = photoShootName.Trim();
                        addEditPhotoShoot.PhotoShotDate = photoShootDate;
                        addEditPhotoShoot.PhotographyjobID = selectedJobId;
                        addEditPhotoShoot.RenameSourceImages = false;
                        if (selectedPhotoshootType != null)
                            addEditPhotoShoot.PhotoshootType = selectedPhotoshootType;
                        addEditPhotoShoot.Job_ = txtJob;
                        addEditPhotoShoot.OnCalendar = isOnCalenderChecked;
                        addEditPhotoShoot.Scheduled = isScheduledChecked;
                        addEditPhotoShoot.Season = selectedSeason == null ? "" : selectedSeason.ToString();
                        //_objphtsht.PhotographyjobID = Convert.ToInt32(cmbxphotojob.SelectedValue);      //while updating foreign key operation is not valid due to the current state of the object 

                        addEditPhotoShoot.ImageFolder = photoShootPath == null ? null : photoShootPath.Trim().TrimEnd(new[] { '/', '\\' });
                        if (addEditPhotoShoot != null)
                        {
                            db.SubmitChanges();
                            tempPhotoShootID = addEditPhotoShoot.PhotoShotID;
                            //int i = clsPhotoShoot.updatePhotographyJobID(db, selectedJobId, photoShootId);

                            //in dashboard instead of going to DB , we are add this photoshoot object to itemsSource
                            addedEditedPhotoShoot = addEditPhotoShoot;
                        }
                        else
                            MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                        #endregion

                        #region Condition B
                        if (tempPhotoShootID > 0)
                        {
                            List<PhotoshootWorkflowItem> tempPhotoshootWorkflowItems = clsWorkflows.getAllPhotoshootWorkflowItem(db, tempPhotoShootID, "Workflow");

                            foreach (PhotoshootWorkflowItem ps in tempPhotoshootWorkflowItems)
                            {
                                WorkflowItem _objWorkflowItem = (from wi in db.WorkflowItems where wi.Id == ps.WorkflowItemId select wi).FirstOrDefault();

                                if (_objWorkflowItem != null)
                                {
                                    if (_objWorkflowItem.BeforeAfter == true)
                                    {
                                        ps.DueDate = addedEditedPhotoShoot.PhotoShotDate.Value.AddDays(-Math.Abs(Convert.ToInt32(_objWorkflowItem.Offset)));
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        ps.DueDate = addedEditedPhotoShoot.PhotoShotDate.Value.AddDays(Math.Abs(Convert.ToInt32(_objWorkflowItem.Offset)));
                                        db.SubmitChanges();
                                    }
                                }
                            }
                            DialogResult = false;
                            isSave = true;
                        }
                        #endregion

                    }
                    else
                    {
                        //New
                        addEditPhotoShoot = new PhotoShoot();
                        //addEditPhotoShoot.PhotographyjobID = photographyJobId;
                        addEditPhotoShoot.Job_ = txtJob;
                        addEditPhotoShoot.PhotoShotName = photoShootName.Trim();
                        addEditPhotoShoot.PhotoShotDate = photoShootDate;
                        //addEditPhotoShoot.ImageFolder = photoShootPath.Trim(); 
                        addEditPhotoShoot.PhotographyjobID = selectedJobId;
                        if (selectedPhotoshootType != null)
                            addEditPhotoShoot.PhotoshootType = selectedPhotoshootType;
                        addEditPhotoShoot.Season = selectedSeason == null ? "" : selectedSeason.ToString();
                        if (addEditPhotoShoot != null)
                        {
                            db.PhotoShoots.InsertOnSubmit(addEditPhotoShoot);
                            db.SubmitChanges();
                            DialogResult = false;
                            isSave = true;
                            //in dashboard instead of going to DB , we are add this photoshoot object to itemsSource
                            addedEditedPhotoShoot = addEditPhotoShoot;
                        }
                        else
                            MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                    }
                }
                else
                    MVVMMessageService.ShowMessage(errorMessages.FILL_ALL_VALUES);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void createNewShootType()
        {
            AddNewPhotoshootType _objAddNewPhotoshootType = new AddNewPhotoshootType();
            _objAddNewPhotoshootType.ShowDialog();

            if (((AddNewPhotoshootTypeViewModel)(_objAddNewPhotoshootType.DataContext)).isSave)
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                cbPhotoshootTypeData = new ObservableCollection<PhotoshootTypeTable>((from pst in db.PhotoshootTypeTables select pst).ToList());
                if (cbPhotoshootTypeData.Count > 0 && selectedPhotoshootType == null)
                {
                    selectedPhotoshootType = cbPhotoshootTypeData.FirstOrDefault().Id;
                }
            }
        }
        private void newSchoolYear()
        {
            AddNewPhotographyJob _objAddNewPhotographyJob = new AddNewPhotographyJob();
            _objAddNewPhotographyJob.ShowDialog();
            BindPhotoJobs(clsSchool.defaultSchoolId);
        }
        private void browseLocation()
        {
            try
            {
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                if (photoShootId != 0 && photoShootPath != "")
                    dlg.SelectedPath = photoShootPath;
                else
                    dlg.SelectedPath = startPath;
                var res = dlg.ShowDialog();
                if (res != false)
                    photoShootPath = dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
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
