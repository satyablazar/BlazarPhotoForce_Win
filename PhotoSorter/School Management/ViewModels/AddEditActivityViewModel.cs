using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.School_Management
{
    public class AddEditActivityViewModel : ViewModelBase
    {
        #region Intialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public bool isSave = false;
        int tempActivityId;
        int schoolId = 0;
        public Activity addEditActivity;
        #endregion

        #region Properties
        string _userName;
        string _subject;
        DateTime? _activityDate;
        string _selectedStatus;
        int _selectedType;
        string _shortDescription;
        string _notes;
        string _schoolName;
        int _selectedJob;
        int? _selectedStudent;
        int? _selectedPhotoShoot;
        List<ActivityType> _cbActivityTypeData;
        IEnumerable<PhotographyJob> _cbPhotographyJobData;
        List<Student> _cbStudentData;
        List<PhotoShoot> _cbPhotoshootsData;

        public List<PhotoShoot> cbPhotoshootsData
        {
            get { return _cbPhotoshootsData; }
            set { _cbPhotoshootsData = value; NotifyPropertyChanged("cbPhotoshootsData"); }
        }
        public List<Student> cbStudentData
        {
            get { return _cbStudentData; }
            set { _cbStudentData = value; NotifyPropertyChanged("cbStudentData"); }
        }
        public List<ActivityType> cbActivityTypeData
        {
            get { return _cbActivityTypeData; }
            set { _cbActivityTypeData = value; NotifyPropertyChanged("cbActivityTypeData"); }
        }
        public IEnumerable<PhotographyJob> cbPhotographyJobData
        {
            get { return _cbPhotographyJobData; }
            set { _cbPhotographyJobData = value; NotifyPropertyChanged("cbPhotographyJobData"); }
        }
        public int? selectedPhotoShoot
        {
            get { return _selectedPhotoShoot; }
            set { _selectedPhotoShoot = value; NotifyPropertyChanged("selectedPhotoShoot"); }
        }
        public int? selectedStudent
        {
            get { return _selectedStudent; }
            set { _selectedStudent = value; NotifyPropertyChanged("selectedStudent"); }
        }
        public int selectedJob
        {
            get { return _selectedJob; }
            set { _selectedJob = value; NotifyPropertyChanged("selectedJob"); }
        }
        public string schoolName
        {
            get { return _schoolName; }
            set { _schoolName = value; NotifyPropertyChanged("schoolName"); }
        }
        public string notes
        {
            get { return _notes; }
            set { _notes = value; NotifyPropertyChanged("notes"); }
        }
        public string shortDescription
        {
            get { return _shortDescription; }
            set { _shortDescription = value; NotifyPropertyChanged("shortDescription"); }
        }
        public int selectedType
        {
            get { return _selectedType; }
            set { _selectedType = value; NotifyPropertyChanged("selectedType"); }
        }
        public string selectedStatus
        {
            get { return _selectedStatus; }
            set { _selectedStatus = value; NotifyPropertyChanged("selectedStatus"); }
        }
        public DateTime? activityDate
        {
            get { return _activityDate; }
            set { _activityDate = value; NotifyPropertyChanged("activityDate"); }
        }
        public string subject
        {
            get { return _subject; }
            set { _subject = value; NotifyPropertyChanged("subject"); }
        }
        public string userName
        {
            get { return _userName; }
            set { _userName = value; NotifyPropertyChanged("userName"); }
        }
        #endregion

        #region Constructor
        public AddEditActivityViewModel(School tempSchool)
        {
            userName = clsStatic.userName;
            schoolName = tempSchool.SchoolName;
            schoolId = tempSchool.ID;
            activityDate = DateTime.Now;
            loadData();
        }
        public AddEditActivityViewModel(Activity tempSelectedActivity)
        {
            tempActivityId = tempSelectedActivity.Id;
            schoolName = tempSelectedActivity.School.SchoolName;
            loadData();

            if (tempActivityId != 0)
            {
                userName = tempSelectedActivity.User.UserName;
                selectedType = tempSelectedActivity.Type;
                notes = tempSelectedActivity.Notes;
                selectedJob = tempSelectedActivity.PhotographyJobId;
                selectedPhotoShoot = tempSelectedActivity.PhotoShootId;
                shortDescription = tempSelectedActivity.ShortDescription;
                selectedStatus = tempSelectedActivity.Status.TrimEnd(new char[] { ' ' });
                selectedStudent = tempSelectedActivity.StudentId;
                subject = tempSelectedActivity.Subject;
                activityDate = tempSelectedActivity.ActivityDate;
            }
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(saveAndClose);
            }
        }
        public RelayCommand AddActivityTypeCommand
        {
            get
            {
                return new RelayCommand(addActivityType);
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
        private void addActivityType()
        {
            AddEditActivityType _objAddEditActivityType = new AddEditActivityType();
            _objAddEditActivityType.ShowDialog();

            if (((AddEditActivityTypeViewModel)(_objAddEditActivityType.DataContext)).isSave)
            {
                cbActivityTypeData = clsActivities.getAllActivitiyTypes(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            }
        }
        private void loadData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            cbPhotographyJobData = clsDashBoard.getJobs(db, clsSchool.defaultSchoolId);
            cbActivityTypeData = clsActivities.getAllActivitiyTypes(db);
            cbStudentData = clsActivities.getAllStudents(db);
            cbPhotoshootsData = clsActivities.getAllPhotoshoots(db);
        }
        private void saveAndClose()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (tempActivityId != 0)
            {
                addEditActivity = new Activity();
                addEditActivity = clsActivities.getActivitiy(db, tempActivityId);
                addEditActivity.Type = selectedType;
                addEditActivity.Notes = notes;
                addEditActivity.PhotographyJobId = selectedJob;
                addEditActivity.PhotoShootId = selectedPhotoShoot;
                addEditActivity.ShortDescription = shortDescription;
                addEditActivity.Status = selectedStatus;
                addEditActivity.StudentId = selectedStudent;
                addEditActivity.Subject = subject;
                addEditActivity.ActivityDate = activityDate;
                db.SubmitChanges();
            }
            else if (selectedType != 0 && selectedStatus != null &&  selectedJob != 0 && (selectedStudent != null || selectedPhotoShoot != null))
            {
                addEditActivity = new Activity();
                addEditActivity.Type = selectedType;
                addEditActivity.UserName = clsStatic.userId;
                addEditActivity.Notes = notes;
                addEditActivity.PhotographyJobId = selectedJob;
                addEditActivity.PhotoShootId = selectedPhotoShoot;
                addEditActivity.ShortDescription = shortDescription;
                addEditActivity.Status = selectedStatus;
                addEditActivity.StudentId = selectedStudent;
                addEditActivity.Subject = subject;
                addEditActivity.ActivityDate = activityDate;
                addEditActivity.SchoolId = schoolId;
                db.Activities.InsertOnSubmit(addEditActivity);
                db.SubmitChanges();
            }
            else
            {
                if (selectedType == 0 && selectedStatus == null && selectedJob == 0 && (selectedStudent == null || selectedPhotoShoot == null))
                    return;
                else if (selectedType == 0)
                    MVVMMessageService.ShowMessage("Please Select Type");
                else if(selectedStatus == null)
                    MVVMMessageService.ShowMessage("Please Select Status");
                else if (selectedJob == 0)
                    MVVMMessageService.ShowMessage("Please Select SchoolYear");
                else if (selectedStudent == null && selectedPhotoShoot == null )
                    MVVMMessageService.ShowMessage("Please Select Student or PhotoShoot");                            
                return;                
            }

            DialogResult = false; isSave = true;
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
