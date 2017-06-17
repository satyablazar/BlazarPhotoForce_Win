using PhotoForce.App_Code;
using PhotoForce.Extensions;
using PhotoForce.MVVM;
using PhotoForce.PhotographyJobManagement;
using PhotoForce.WorkPlace.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.Student_Management
{
    public class AddStudentViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public bool isSave = false;
        StudentImport _objStudentImport = new StudentImport();
        int maxImportId = 0;
        public Student addEditStudent;
        #endregion

        #region Properties

        IEnumerable<PhotoForce.App_Code.PhotographyJob> _cbPhotoJobsData;
        string _teacher;
        string _grade;
        DateTime? _DOB;
        string _address;
        string _city;
        string _state;
        string _zip;
        string _phone;
        string _email;
        //string _password;
        private string _custom1;
        private string _custom2;
        private string _custom3;
        private string _custom4;
        private string _custom5;

        PhotoForce.App_Code.PhotographyJob _cbPhotoJobsSelectedItem;
        string _title;
        //List<string> _studentTypeData;
        StudentType _selectedStudentType;

        public StudentType selectedStudentType
        {
            get { return _selectedStudentType; }
            set { _selectedStudentType = value; NotifyPropertyChanged("selectedStudentType"); }
        }
        //public List<string> studentTypeData
        //{
        //    get { return _studentTypeData; }
        //    set { _studentTypeData = value; NotifyPropertyChanged("studentTypeData"); }
        //}
        public string title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged("title"); }
        }
        public PhotoForce.App_Code.PhotographyJob cbPhotoJobsSelectedItem
        {
            get { return _cbPhotoJobsSelectedItem; }
            set { _cbPhotoJobsSelectedItem = value; NotifyPropertyChanged("cbPhotoJobsSelectedItem"); }
        }
        public string custom1
        {
            get { return _custom1; }
            set { _custom1 = value; NotifyPropertyChanged("custom1"); }
        }
        public string custom2
        {
            get { return _custom2; }
            set { _custom2 = value; NotifyPropertyChanged("custom2"); }
        }
        public string custom3
        {
            get { return _custom3; }
            set { _custom3 = value; NotifyPropertyChanged("custom3"); }
        }
        public string custom4
        {
            get { return _custom4; }
            set { _custom4 = value; NotifyPropertyChanged("custom4"); }
        }
        public string custom5
        {
            get { return _custom5; }
            set { _custom5 = value; NotifyPropertyChanged("custom5"); }
        }
        //public string password
        //{
        //    get { return _password; }
        //    set { _password = value; NotifyPropertyChanged("password"); }
        //}
        public string email
        {
            get { return _email; }
            set { _email = value; NotifyPropertyChanged("email"); }
        }
        public string phone
        {
            get { return _phone; }
            set { _phone = value; NotifyPropertyChanged("phone"); }
        }
        public string zip
        {
            get { return _zip; }
            set { _zip = value; NotifyPropertyChanged("zip"); }
        }
        public string state
        {
            get { return _state; }
            set { _state = value; NotifyPropertyChanged("state"); }
        }
        public string city
        {
            get { return _city; }
            set { _city = value; NotifyPropertyChanged("city"); }
        }
        public string address
        {
            get { return _address; }
            set { _address = value; NotifyPropertyChanged("address"); }
        }
        public DateTime? DOB
        {
            get { return _DOB; }
            set { _DOB = value; NotifyPropertyChanged("DOB"); }
        }
        public string grade
        {
            get { return _grade; }
            set { _grade = value; NotifyPropertyChanged("grade"); }
        }
        public string teacher
        {
            get { return _teacher; }
            set { _teacher = value; NotifyPropertyChanged("teacher"); }
        }
        public IEnumerable<PhotoForce.App_Code.PhotographyJob> cbPhotoJobsData
        {
            get { return _cbPhotoJobsData; }
            set { _cbPhotoJobsData = value; NotifyPropertyChanged("cbPhotoJobsData"); }
        }
        #endregion

        #region IDataErrorInfo Members

        #region Error Property
        /// <summary>
        /// 
        /// </summary>
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region this Property
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                string message = string.Empty;
                validateUserInput(ref message, columnName);
                return message;
            }
        }
        #endregion

        #endregion

        #region Constructors
        public AddStudentViewModel()
        {
            bindPhotoJobs(clsSchool.defaultSchoolId);
            //studentTypeData = new List<string>();
            //studentTypeData.Add("Student"); studentTypeData.Add("Staff"); studentTypeData.Add("Class"); studentTypeData.Add("Family"); studentTypeData.Add("Club"); studentTypeData.Add("Team"); studentTypeData.Add("Misc");
            selectedStudentType = StudentType.Student;
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(addNewStudent);
            }
        }
        public RelayCommand NewSchoolYearCommand
        {
            get
            {
                return new RelayCommand(newSchoolYear);
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
        private void bindPhotoJobs(int schoolId)
        {
            try
            {
                cbPhotoJobsData = clsDashBoard.getJobs(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), schoolId);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

        }
        private void addNewStudent()
        {
            if (errorCount == 0 && !string.IsNullOrEmpty(selectedStudentType.ToString()))
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                maxImportId = clsDashBoard.getMaxImportIdForSchool(db, clsSchool.defaultSchoolId);
                if (maxImportId == 0)
                {
                    _objStudentImport.Description = "from import";
                    _objStudentImport.Notes = "from import";
                    _objStudentImport.SchoolID = clsSchool.defaultSchoolId;
                    _objStudentImport.CreatedOn = DateTime.Now;

                    if (_objStudentImport != null)
                    {
                        db.StudentImports.InsertOnSubmit(_objStudentImport);
                        db.SubmitChanges();
                        maxImportId = clsDashBoard.getMaxImportIdForSchool(db, clsSchool.defaultSchoolId);
                    }
                }
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                addEditStudent = new Student();
                addEditStudent.StudentImportID = maxImportId;
                addEditStudent.FirstName = firstName;
                addEditStudent.Lastname = lastName;
                addEditStudent.Teacher = teacher;
                addEditStudent.Grade = grade;
                addEditStudent.Title = title;
                addEditStudent.IsStudent = selectedStudentType.ToString();
                addEditStudent.Custom1 = custom1;
                addEditStudent.Custom2 = custom2;
                addEditStudent.Custom3 = custom3;
                addEditStudent.Custom4 = custom4;
                addEditStudent.Custom5 = custom5;
                addEditStudent.CreatedOn = DateTime.Now;
                //addEditStudent.SchoolYear = cbPhotoJobsSelectedItem.JobName;   //have to use photogaraphy job fk and not use this column //Mohan Sept 3rd 2015
                addEditStudent.PhotographyJobID = cbPhotoJobsSelectedItem.ID;
                addEditStudent.City = city;
                addEditStudent.StudentID = studentId;
                addEditStudent.Address = address;
                if (!string.IsNullOrEmpty(password))
                    addEditStudent.Password = password;
                else
                    addEditStudent.Password = null;

                if (DOB != null)
                    addEditStudent.DOB = Convert.ToDateTime(DOB);
                else
                    addEditStudent.DOB = null;
                addEditStudent.Emailaddress = email;
                addEditStudent.Zip = zip;
                addEditStudent.Phone = phone;
                addEditStudent.State = state;
                addEditStudent.RecordStatus = true;
                if (addEditStudent != null)
                {
                    db.Students.InsertOnSubmit(addEditStudent);
                    db.SubmitChanges();
                    isSave = true;
                }
                else
                    MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                DialogResult = false;
            }
        }
        private void newSchoolYear()
        {
            AddNewPhotographyJob _objAddNewPhotographyJob = new AddNewPhotographyJob();
            _objAddNewPhotographyJob.ShowDialog();
            if (((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).isSave)
                bindPhotoJobs(clsSchool.defaultSchoolId);

        }
        private void windowClose()
        {
            DialogResult = false;
            isSave = false;
        }
        #endregion
    }
}
