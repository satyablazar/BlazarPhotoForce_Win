using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.PhotographyJobManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.Student_Management
{
    public class BulkRenameStudentViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        ArrayList arrStuId = new ArrayList();
        public bool isSave = false;
        #endregion

        //# commanted by hema (has to ask mohan)
        #region IDataErrorInfo Members

        #region Error Property
        /// <summary>
        /// 
        /// </summary>
        //public string Error
        //{
        //    get { throw new NotImplementedException(); }
        //}
        #endregion

        #region this Property
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        //public string this[string columnName]
        //{
        //    get
        //    {
        //        string message = string.Empty;
        //        validateUserInput(ref message, columnName);
        //        return message;
        //    }
        //}
        #endregion

        #endregion

        #region Properties

        IEnumerable<PhotographyJob> _cbPhotoJobsData;
        string _teacher;
        string _grade;
        string _DOB;
        string _address;
        string _city;
        string _state;
        string _zip;
        string _phone;
        string _email;
        string _studentPassword;
        private string _custom1;
        private string _custom2;
        private string _custom3;
        private string _custom4;
        private string _custom5;
        private PhotographyJob _cbPhotoJobsSelectedItem;        
        string _title;
        List<string> _studentTypeData;
        string _selectedStudentType;
        string _schoolCampus;

        public string schoolCampus
        {
            get { return _schoolCampus; }
            set { _schoolCampus = value; NotifyPropertyChanged(); }
        }
        public string selectedStudentType
        {
            get { return _selectedStudentType; }
            set { _selectedStudentType = value; NotifyPropertyChanged("selectedStudentType"); }
        }
        public List<string> studentTypeData
        {
            get { return _studentTypeData; }
            set { _studentTypeData = value; NotifyPropertyChanged("studentTypeData"); }
        }
        public string title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged("title"); }
        }

        //clear field properties
        private bool _isClearTeacher;
        private bool _isClearGrade;
        private bool _isClearDOB;
        private bool _isClearAddress;
        private bool _isClearCity;
        private bool _isClearState;
        private bool _isClearZip;
        private bool _isClearPhone;
        private bool _isClearEmail;
        private bool _isClearPassword;
        private bool _isClearSchoolCampus;
        private bool _isClearCustom1; private bool _isClearCustom2; private bool _isClearCustom3; private bool _isClearCustom4; private bool _isClearCustom5;

        public bool isClearSchoolCampus
        {
            get { return _isClearSchoolCampus; }
            set { _isClearSchoolCampus = value; NotifyPropertyChanged(); }
        }
        public bool isClearCustom5
        {
            get { return _isClearCustom5; }
            set { _isClearCustom5 = value; NotifyPropertyChanged("isClearCustom5"); }
        }
        public bool isClearCustom4
        {
            get { return _isClearCustom4; }
            set { _isClearCustom4 = value; NotifyPropertyChanged("isClearCustom4"); }
        }
        public bool isClearCustom3
        {
            get { return _isClearCustom3; }
            set { _isClearCustom3 = value; NotifyPropertyChanged("isClearCustom3"); }
        }
        public bool isClearCustom2
        {
            get { return _isClearCustom2; }
            set { _isClearCustom2 = value; NotifyPropertyChanged("isClearCustom2"); }
        }
        public bool isClearCustom1
        {
            get { return _isClearCustom1; }
            set { _isClearCustom1 = value; NotifyPropertyChanged("isClearCustom1"); }
        }
        public bool isClearPassword
        {
            get { return _isClearPassword; }
            set { _isClearPassword = value; NotifyPropertyChanged("isClearPassword"); }
        }
        public bool isClearEmail
        {
            get { return _isClearEmail; }
            set { _isClearEmail = value; NotifyPropertyChanged("isClearEmail"); }
        }
        public bool isClearPhone
        {
            get { return _isClearPhone; }
            set { _isClearPhone = value; NotifyPropertyChanged("isClearPhone"); }
        }
        public bool isClearZip
        {
            get { return _isClearZip; }
            set { _isClearZip = value; NotifyPropertyChanged("isClearZip"); }
        }
        public bool isClearState
        {
            get { return _isClearState; }
            set { _isClearState = value; NotifyPropertyChanged("isClearState"); }
        }
        public bool isClearCity
        {
            get { return _isClearCity; }
            set { _isClearCity = value; NotifyPropertyChanged("isClearCity"); }
        }
        public bool isClearAddress
        {
            get { return _isClearAddress; }
            set { _isClearAddress = value; NotifyPropertyChanged("isClearAddress"); }
        }
        public bool isClearDOB
        {
            get { return _isClearDOB; }
            set { _isClearDOB = value; NotifyPropertyChanged("isClearDOB"); }
        }
        public bool isClearGrade
        {
            get { return _isClearGrade; }
            set { _isClearGrade = value; NotifyPropertyChanged("isClearGrade"); }
        }
        public bool isClearTeacher
        {
            get { return _isClearTeacher; }
            set { _isClearTeacher = value; NotifyPropertyChanged("isClearTeacher"); }
        }
        public PhotographyJob cbPhotoJobsSelectedItem
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
        public string studentPassword
        {
            get { return _studentPassword; }
            set { _studentPassword = value; NotifyPropertyChanged("studentPassword"); }
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
        public IEnumerable<PhotographyJob> cbPhotoJobsData
        {
            get { return _cbPhotoJobsData; }
            set { _cbPhotoJobsData = value; NotifyPropertyChanged("cbPhotoJobsData"); }
        }
        #endregion

        #region Constructors
        public BulkRenameStudentViewModel(ArrayList studentId)
        {
            arrStuId = studentId;
            studentTypeData = new List<string>();
            //studentTypeData.Add("Student"); studentTypeData.Add("Staff"); studentTypeData.Add("Class"); studentTypeData.Add("Family"); studentTypeData.Add("Club"); studentTypeData.Add("Team"); studentTypeData.Add("Misc");
            studentTypeData.Add("Student"); studentTypeData.Add("Staff"); studentTypeData.Add("StaffGroup"); studentTypeData.Add("Class"); studentTypeData.Add("Family"); studentTypeData.Add("Siblings"); studentTypeData.Add("Club"); studentTypeData.Add("Team"); studentTypeData.Add("Player"); studentTypeData.Add("Misc");
            selectedStudentType = "";//studentTypeData[0];
            BindPhotoJobs(clsSchool.defaultSchoolId);
        }
        #endregion

        #region Commands
        public RelayCommand BulkRenameCommand
        {
            get
            {
                return new RelayCommand(bulkRename);
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
        private void BindPhotoJobs(int Schoolid)
        {
            try
            {
                cbPhotoJobsData = clsDashBoard.getJobs(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Schoolid);
                cbPhotoJobsSelectedItem = null;
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
                BindPhotoJobs(clsSchool.defaultSchoolId);
        }
        private void bulkRename()
        {
            if (errorCount == 0)
            {
                try
                {
                    int count = clearFieldCount();
                    string message = "";
                    if (count > 0)
                    {
                        message = "You have selected to clear certain fields. The fields for all " + arrStuId.Count + " records will be cleared. OK to proceed?";
                    }
                    else
                    {
                        if (arrStuId.Count > 1)
                            message = errorMessages.BEFORE_RENAMING_SELECTED_STUDENTS_CONFIRMATION1 + arrStuId.Count + errorMessages.BEFORE_RENAMING_SELECTED_STUDENTS_CONFIRMATION2;
                        else
                            message = errorMessages.BEFORE_RENAMING_SELECTED_STUDENT_CONFIRMATION1 + arrStuId.Count + errorMessages.BEFORE_RENAMING_SELECTED_STUDENT_CONFIRMATION2;
                    }
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        int i = 0;
                        //# commented by hema(has to ask mohan)
                        #region Password validation
                        //if (isClearPassword == true)
                        //    i = clsStudent.UpdateMultiplePasswords(db, null, arrStuId);
                        //else
                        //{
                        //    if (!string.IsNullOrEmpty(password))
                        //    {
                        //        Regex regex = new Regex("^[a-zA-Z0-9]*$");
                        //        if (regex.IsMatch(password))
                        //            i = clsStudent.UpdateMultiplePasswords(db, password, arrStuId);
                        //        else
                        //        { i = 1; MVVMMessageService.ShowMessage("Plaese enter valid password."); return; }
                        //    }
                        //}
                        #endregion

                        if (!string.IsNullOrEmpty(studentId))
                            i = clsStudent.UpdateMultipleStudentID(db, studentId, arrStuId);

                        if (!string.IsNullOrEmpty(firstName))
                            i = clsStudent.UpdateMultipleStudentFirstName(db, firstName, arrStuId);

                        if (!string.IsNullOrEmpty(lastName))
                            i = clsStudent.UpdateMultipleStudentLastName(db, lastName, arrStuId);

                        if (!string.IsNullOrEmpty(title))
                            i = clsStudent.UpdateMultipleStudentTitle(db, title, arrStuId);

                        if (!string.IsNullOrEmpty(selectedStudentType))
                            i = clsStudent.UpdateMultipleStudentIdentity(db, selectedStudentType, arrStuId);

                        if (!string.IsNullOrEmpty(schoolCampus))
                            i = clsStudent.UpdateMultipleSchoolCampus(db, schoolCampus, arrStuId);

                        if (isClearTeacher == true)
                            i = clsStudent.UpdateMultipleStudentTeacher(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(teacher))
                                i = clsStudent.UpdateMultipleStudentTeacher(db, teacher, arrStuId);
                        }
                        if (isClearGrade == true)
                            i = clsStudent.UpdateMultipleStudentGrade(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(grade))
                                i = clsStudent.UpdateMultipleStudentGrade(db, grade, arrStuId);
                        }
                        if (isClearCustom1 == true)
                            i = clsStudent.UpdateMultipleCustom1(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(custom1))
                                i = clsStudent.UpdateMultipleCustom1(db, custom1, arrStuId);
                        }
                        if (isClearCustom2 == true)
                            i = clsStudent.UpdateMultipleCustom2(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(custom2))
                                i = clsStudent.UpdateMultipleCustom2(db, custom2, arrStuId);
                        }
                        if (isClearCustom3 == true)
                            i = clsStudent.UpdateMultipleCustom3(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(custom3))
                                i = clsStudent.UpdateMultipleCustom3(db, custom3, arrStuId);
                        }
                        if (isClearCustom4 == true)
                            i = clsStudent.UpdateMultipleCustom4(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(custom4))
                                i = clsStudent.UpdateMultipleCustom4(db, custom4, arrStuId);
                        }
                        if (isClearCustom5 == true)
                            i = clsStudent.UpdateMultipleCustom5(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(custom5))
                                i = clsStudent.UpdateMultipleCustom5(db, custom5, arrStuId);
                        }
                        if (isClearAddress == true)
                            i = clsStudent.UpdateMultipleAddress(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(address))
                                i = clsStudent.UpdateMultipleAddress(db, address, arrStuId);
                        }
                        if (isClearCity == true)
                            i = clsStudent.UpdateMultipleCitystudent(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(city))
                                i = clsStudent.UpdateMultipleCitystudent(db, city, arrStuId);
                        }
                        if (isClearState == true)
                            i = clsStudent.UpdateMultipleStatestudent(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(state))
                                i = clsStudent.UpdateMultipleStatestudent(db, state, arrStuId);
                        }
                        if (isClearDOB == true)
                            i = clsStudent.UpdateMultipleDOB(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(DOB))
                                i = clsStudent.UpdateMultipleDOB(db, DOB, arrStuId);
                        }
                        if (isClearZip == true)
                            i = clsStudent.UpdateMultipleZipstudent(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(zip))
                                i = clsStudent.UpdateMultipleZipstudent(db, zip, arrStuId);
                        }
                        if (isClearPhone == true)
                            i = clsStudent.UpdateMultiplePhone(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(phone))
                                i = clsStudent.UpdateMultiplePhone(db, phone, arrStuId);
                        }
                        if (isClearEmail == true)
                            i = clsStudent.UpdateMultipleEmailAddress(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(email))
                                i = clsStudent.UpdateMultipleEmailAddress(db, email, arrStuId);
                        }
                        if (isClearPassword == true)
                            i = clsStudent.UpdateMultiplePasswords(db, null, arrStuId);
                        else
                        {
                            if (!string.IsNullOrEmpty(studentPassword))
                                i = clsStudent.UpdateMultiplePasswords(db, studentPassword, arrStuId);
                        }
                        if (cbPhotoJobsSelectedItem != null)
                            i = clsStudent.UpdateMultipleSchoolYear(db, Convert.ToInt32(cbPhotoJobsSelectedValue), arrStuId); //changed by mohan;established a relation b/w student and photographyjob  //,cbPhotoJobsSelectedItem.JobName
                        if (i != 0)
                        {
                            isSave = true;
                            DialogResult = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
        }
        private int clearFieldCount()
        {
            int count = 0;
            if (isClearPassword == true) { count++; } if (isClearTeacher == true) { count++; }
            if (isClearGrade == true) { count++; } if (isClearDOB == true) { count++; }
            if (isClearAddress == true) { count++; } if (isClearCity == true) { count++; }
            if (isClearState == true) { count++; } if (isClearZip == true) { count++; }
            if (isClearPhone == true) { count++; } if (isClearEmail == true) { count++; }
            if (isClearPassword == true) { count++; } if (isClearCustom1 == true) { count++; }
            if (isClearCustom2 == true) { count++; } if (isClearCustom3 == true) { count++; }
            if (isClearCustom4 == true) { count++; } if (isClearCustom5 == true) { count++; }
            return count;
        }
        private void windowClose()
        {
            DialogResult = false;
            isSave = false;
        }
        #endregion
    }
}
