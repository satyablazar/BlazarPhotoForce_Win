using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.PhotographyJobManagement;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.WorkPlace.UserControls;
using System.ComponentModel;
using PhotoForce.Extensions;
using System.Globalization;

namespace PhotoForce.Student_Management
{
    public class AddEditStudentViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Initialization
        int tempStudentId = 0;
        int studentImportId = 0;
        int defaultSchoolId = 0;
        string jobName = "";
        public bool isSave = false;
        Student addEditStudent;
        #endregion

        #region Properties

        IEnumerable<PhotoForce.App_Code.PhotographyJob> _cbPhotoJobsData;
        //string _studentId;
        //string _firstName;
        //string _lastName;
        string _teacher;
        string _grade;
        string _DOB;
        string _address;
        string _city;
        string _state;
        string _zip;
        string _phone;
        string _email;
        private string _custom1;
        private string _custom2;
        private string _custom3;
        private string _custom4;
        private string _custom5;
        //private int? _cbPhotoJobsSelectedValue;
        private PhotoForce.App_Code.PhotographyJob _cbPhotoJobsSelectedItem;
        string _title;
        //List<string> _studentTypeData;
        StudentType _selectedStudentType;
        string _schoolCampus;

        public string schoolCampus
        {
            get { return _schoolCampus; }
            set { _schoolCampus = value; NotifyPropertyChanged(); }
        }
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
        //public int? cbPhotoJobsSelectedValue
        //{
        //    get { return _cbPhotoJobsSelectedValue; }
        //    set { _cbPhotoJobsSelectedValue = value; NotifyPropertyChanged("cbPhotoJobsSelectedValue"); }
        //}
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
        public string DOB
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
        //public string lastName
        //{
        //    get { return _lastName; }
        //    set { _lastName = value; NotifyPropertyChanged("lastName"); }
        //}
        //public string firstName
        //{
        //    get { return _firstName; }
        //    set { _firstName = value; NotifyPropertyChanged("firstName"); }
        //}
        //public string studentId
        //{
        //    get { return _studentId; }
        //    set { _studentId = value; NotifyPropertyChanged("studentId"); }
        //}
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
        public AddEditStudentViewModel(int tempStuId, int tempStuImportId, int tempSchoolId)
        {
            //studentTypeData = new List<string>();
            //studentTypeData.Add("Student"); studentTypeData.Add("Staff"); studentTypeData.Add("Class"); studentTypeData.Add("Family"); studentTypeData.Add("Club"); studentTypeData.Add("Team"); studentTypeData.Add("Misc");            
            tempStudentId = tempStuId;
            studentImportId = tempStuImportId;
            defaultSchoolId = tempSchoolId;
            bindControls();
            bindPhotoJobs();
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(updateStudent);
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
        private void updateStudent()
        {
            try
            {
                if (tempStudentId != 0 && errorCount == 0 && !string.IsNullOrEmpty(selectedStudentType.ToString()))
                {
                    // Update
                    PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    addEditStudent = new Student();
                    addEditStudent = clsStudent.updateStudent(dbb, tempStudentId);
                    addEditStudent.StudentID = studentId;
                    addEditStudent.FirstName = firstName;
                    addEditStudent.Lastname = lastName;
                    addEditStudent.Teacher = teacher;
                    addEditStudent.Title = title;
                    addEditStudent.IsStudent = selectedStudentType.ToString();
                    addEditStudent.Grade = grade;
                    addEditStudent.SchoolCampus = schoolCampus;
                    addEditStudent.Custom1 = custom1;
                    addEditStudent.Custom2 = custom2;
                    addEditStudent.Custom3 = custom3;
                    addEditStudent.Custom4 = custom4;
                    addEditStudent.Custom5 = custom5;
                    //addEditStudent.SchoolYear = cbPhotoJobsSelectedItem.JobName;     //have to use photogaraphy job fk and not use this column //Mohan Sept 3rd 2015
                    //_objstudent.PhotographyJobID = Convert.ToInt32(cmbxphotojob.SelectedValue); //while updating foreign key operation is not valid due to the current state of the object 
                    addEditStudent.City = city;
                    addEditStudent.Address = address;
                    if (!string.IsNullOrEmpty(password))
                        addEditStudent.Password = password;
                    else
                        addEditStudent.Password = null;
                    //DateTime dt;
                    if (!string.IsNullOrEmpty(DOB))
                        addEditStudent.DOB = Convert.ToDateTime(DOB, CultureInfo.InvariantCulture).Date ;
                        //addEditStudent.DOB = DOB.Contains("AM") || DOB.Contains("PM") ? Convert.ToDateTime(DOB, CultureInfo.InvariantCulture).Date : Convert.ToDateTime(DOB, CultureInfo.InvariantCulture).Date;
                        //dt = DOB.Contains("AM") || DOB.Contains("PM") ? DateTime.ParseExact("26/02/2016 16:09:46.400", "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) : DateTime.ParseExact("26/02/2016 16:09:46.400", "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    else
                        addEditStudent.DOB = null;
                    addEditStudent.Emailaddress = email;
                    addEditStudent.Zip = zip;
                    addEditStudent.Phone = phone;
                    addEditStudent.State = state;
                    if (addEditStudent != null)
                    {
                        isSave = true;
                        dbb.SubmitChanges();
                        if (cbPhotoJobsSelectedValue != null)
                            clsStudent.UpdateSingleSchoolYear(dbb, cbPhotoJobsSelectedValue, tempStudentId);
                    }
                    else
                        MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                    DialogResult = false;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void newSchoolYear()
        {
            AddNewPhotographyJob _objAddNewPhotographyJob = new AddNewPhotographyJob();
            _objAddNewPhotographyJob.ShowDialog();
            if (((AddNewPhotographyJobViewModel)(_objAddNewPhotographyJob.DataContext)).isSave)
                bindPhotoJobs();
        }
        private void windowClose()
        {
            DialogResult = false;
            isSave = false;
        }

        private void bindPhotoJobs()
        {
            try
            {
                cbPhotoJobsData = clsDashBoard.getJobs(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), defaultSchoolId);
                cbPhotoJobsSelectedValue = (from n in cbPhotoJobsData where n.JobName == jobName select n.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

        }

        private void bindControls()
        {
            if (tempStudentId != 0)
            {
                try
                {
                    IEnumerable<Student> dt = clsStudent.getStudentDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempStudentId);
                    if (dt != null)
                    {
                        //isStudentChecked = (dt.First().IsStudent == null) ? false : (dt.First().IsStudent == "Yes" ? true : false);
                        //isTeacherChecked = (dt.First().IsStudent == null) ? false : !isStudentChecked;

                        selectedStudentType = dt.First().IsStudent == null ? StudentType.Student : StringToEnumConverter.ToEnum<StudentType>(dt.First().IsStudent);
                        title = dt.First().Title;
                        firstName = dt.First().FirstName.ToString();
                        lastName = dt.First().Lastname;
                        teacher = dt.First().Teacher;
                        grade = dt.First().Grade;
                        schoolCampus = dt.First().SchoolCampus;
                        custom1 = dt.First().Custom1;
                        custom2 = dt.First().Custom2;
                        custom3 = dt.First().Custom3;
                        custom4 = dt.First().Custom4;
                        custom5 = dt.First().Custom5;
                        DOB = Convert.ToString(dt.First().DOB);
                        address = dt.First().Address;
                        city = dt.First().City;
                        state = dt.First().State;
                        zip = dt.First().Zip;
                        phone = dt.First().Phone;
                        email = dt.First().Emailaddress;
                        password = dt.First().Password;
                        studentId = dt.First().StudentID;
                        if (dt.First().PhotographyJob != null)
                            jobName = dt.First().PhotographyJob.JobName;
                    }
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                    clsStatic.WriteExceptionLogXML(ex);
                }
            }
        }
        #endregion
    }
}
