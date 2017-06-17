using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.Extensions;
using System.IO;
using System.Data;
using RandomLicenseGenerator;
using PhotoForce.License_Management;
using PhotoForce.PhotographyJobManagement;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.Mask_Management;
using System.Transactions;

namespace PhotoForce.Student_Management
{
    public class ImportStudentViewModel : ViewModelBase
    {
        #region Initialization
        //PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public int maxImportId = 0;
        int Selectedrow = 0;
        SearchSchool _objSearchSchool;
        string fileName;
        internal bool isSave = false;
        //internal int credits = 0;
        #endregion

        #region Properties
        string _schoolName;
        string _fullPath;
        string _description;
        IEnumerable<PhotographyJob> _cbSchoolYearData;
        PhotographyJob _schoolYearSelectedItem;

        public PhotographyJob schoolYearSelectedItem
        {
            get { return _schoolYearSelectedItem; }
            set { _schoolYearSelectedItem = value; NotifyPropertyChanged("schoolYearSelectedItem"); }
        }
        public string description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("description"); }
        }
        public string fullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; NotifyPropertyChanged("fullPath"); }
        }

        public IEnumerable<PhotographyJob> cbSchoolYearData
        {
            get { return _cbSchoolYearData; }
            set { _cbSchoolYearData = value; NotifyPropertyChanged("cbSchoolYearData"); }
        }
        public string schoolName
        {
            get { return _schoolName; }
            set { _schoolName = value; NotifyPropertyChanged("schoolName"); }
        }
        #endregion

        #region Constructors
        public ImportStudentViewModel(int tempSchoolId, int rowHandle, string schoolName)
        {
            try
            {
                Selectedrow = rowHandle;
                GetDefaultSchool();
                bindComboBox();
            }
            catch (Exception ex)
            { clsStatic.WriteExceptionLogXML(ex); }
        }
        #endregion

        #region Commands
        public RelayCommand ImportStudentsCommand
        {
            get
            {
                return new RelayCommand(importStudents);
            }
        }
        public RelayCommand SelectFileCommand
        {
            get
            {
                return new RelayCommand(selectFile);
            }
        }
        public RelayCommand NewSchoolYearCommand
        {
            get
            {
                return new RelayCommand(newSchoolYear);
            }
        }
        public RelayCommand SearchCommand
        {
            get
            {
                return new RelayCommand(search);
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

        void bindComboBox()
        {
            try
            {
                cbSchoolYearData = clsDashBoard.getJobs(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// To get default school
        /// </summary>
        private void GetDefaultSchool()
        {
            schoolName = clsSchool.defaultSchoolName;
        }
        /// <summary>
        /// to close the window
        /// </summary>
        private void windowClose()
        {
            DialogResult = false;
            isSave = false;
        }

        private void search()
        {
            _objSearchSchool = new SearchSchool(0, "ImportStudents");
            _objSearchSchool.ShowDialog();
            schoolName = clsSchool.defaultSchoolName;
        }
        /// <summary>
        /// to create new School year
        /// </summary>
        private void newSchoolYear()
        {
            AddNewPhotographyJob _objAddNewPhotographyJob = new AddNewPhotographyJob();
            _objAddNewPhotographyJob.ShowDialog();
            bindComboBox();
        }
        /// <summary>
        /// to import students from excel file
        /// </summary>
        private void importStudents()
        {
            try
            {
                //Don't delete this code
                //if neal freed sell Photo force to some clients then we need of this credits
                //Commented by Mohan Tangella on 15-10-2015
                //int creditCount = clsLicensing.getCredtisCount(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));

                if (!string.IsNullOrEmpty(schoolName) && !string.IsNullOrEmpty(fullPath) && !string.IsNullOrEmpty(description))
                {
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    string message = errorMessages.BEFORE_IMPORT_STUDENT_DATA_CONFIRMATION1 + fileName + errorMessages.BEFORE_IMPORT_STUDENT_DATA_CONFIRMATION2 + schoolName + errorMessages.BEFORE_IMPORT_STUDENT_DATA_CONFIRMATION3;
                    if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        StudentImport _objStuImport = new StudentImport();
                        bool isError = false;
                        //DataTable dt = DataLoader.ReadTextFile(txtfullpath.Text);
                        DataTable dt = PhotoForce.Helpers.DataLoader.getDataTableFromExcel(fullPath);
                        if (dt == null) { return; }
                        else
                        {
                            string res = checkForRequiredColumns(dt);
                            if (res != "")
                            {
                                MVVMMessageService.ShowMessage(res);
                                return;
                            }
                        }
                        //Don't delete this code
                        //if neal freed sell Photo force to some clients then we need of this credits
                        //Commented by Mohan Tangella on 15-10-2015
                        //if (creditCount < dt.Rows.Count)
                        //{
                        //    string tempMessage = "Credits remaining: " + creditCount + Environment.NewLine + "Credits required for this import: " + dt.Rows.Count + Environment.NewLine + "Would you like to purchase additional credits now?";
                        //    if (WPFMessageBox.customShow("Freed Photo", "Your don't have enough credits for this operation", tempMessage, WPFMessageBoxButtons.YesNoCancel, WPFMessageBoxImage.Question) == WPFMessageBoxResult.Yes)
                        //    {
                        //        AddCredits addCredits = new AddCredits("");
                        //        addCredits.ShowDialog();
                        //    }
                        //    return;
                        //}
                        if (dt.Rows.Count > 0)
                        {
                            //string[] arrStudentType = new string[] { "Student", "Staff", "Class", "Family", "Club", "Team", "Misc" };
                            string[] arrStudentType = new string[] { "Student", "Staff", "StaffGroup", "Class", "Family", "Siblings", "Club", "Team", "Player", "Misc" };
                            //using (TransactionScope ts = new TransactionScope())
                            //{
                            //    using (PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString))
                            //    {
                            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                            #region Condition A
                            //Need to insert into StudentImport table first..
                            _objStuImport.Description = description;
                            _objStuImport.Notes = "from import";
                            _objStuImport.SchoolID = clsSchool.defaultSchoolId;
                            _objStuImport.CreatedOn = DateTime.Now;
                            string prevStudentId = ""; int tempStudentID = 0;

                            byte[] txtdtaBytes = File.ReadAllBytes(fullPath);
                            _objStuImport.DataFile = txtdtaBytes;
                            if (_objStuImport != null)
                            {
                                db.StudentImports.InsertOnSubmit(_objStuImport);
                                db.SubmitChanges();
                                maxImportId = clsDashBoard.getmaxImportId(db);
                            }
                            #endregion  // Condition A
                            //retval = clsStudent.insertImportedStudentData(dr, maxImportId, opcode, Convert.ToInt32(schoolYearSelectedItem.ID), dt.Columns.Count);
                            #region Condition B
                            #region Insert Student
                            foreach (DataRow dr in dt.Rows)
                            {
                                //create student with data row
                                Student createStudent = new Student();
                                try
                                {
                                    string studentId = Convert.ToString(dr["Student ID"]);
                                    if (string.IsNullOrEmpty(studentId)) { continue; }      //validating empty rows with Student Id
                                    if (string.IsNullOrEmpty(prevStudentId))
                                        prevStudentId = studentId;
                                    else
                                    {
                                        if (prevStudentId == (studentId))
                                        {
                                            //update studentDetails
                                            PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                            Student prevStudent = (from stu in dbb.Students where stu.ID == tempStudentID select stu).FirstOrDefault();
                                            if (prevStudent != null)
                                            {
                                                //this has to be previous student details and emailaddress --> alternative emailaddress
                                                if (string.IsNullOrEmpty(prevStudent.State))
                                                    prevStudent.State = Convert.ToString(dr["Emailaddress"]);
                                                else if (string.IsNullOrEmpty(prevStudent.City))
                                                    prevStudent.City = Convert.ToString(dr["Emailaddress"]);
                                                //if (string.IsNullOrEmpty(prevStudent.Address))
                                                //    prevStudent.Address = Convert.ToString(dr["Emailaddress"]);
                                                dbb.SubmitChanges();
                                            }
                                            prevStudentId = studentId;
                                            continue;
                                        }
                                        prevStudentId = studentId;
                                    }
                                    string schoolYear = Convert.ToString(dr["School Year"]);

                                    #region old code
                                    //retval = clsStudent.insertImportedStudentData(dr, maxImportId, opcode, Convert.ToInt32(schoolYearSelectedItem.ID), dt.Columns.Count);
                                    #endregion

                                    #region New Code
                                    if (dt.Columns.Count <= 12)
                                    {
                                        //param[0] = new SqlParameter("@opcode", opcode);
                                        createStudent.StudentImportID = maxImportId;
                                        createStudent.FirstName = Convert.ToString(dr["First Name"]) != null ? Convert.ToString(dr["First Name"]).Trim() : "";
                                        createStudent.Lastname = Convert.ToString(dr["Last name"]) != null ? Convert.ToString(dr["Last name"]).Trim() : "";
                                        createStudent.OfficialFirstName = Convert.ToString(dr["Official FN"]) != null ? Convert.ToString(dr["Official FN"]).Trim() : "";
                                        createStudent.OfficialLastName = Convert.ToString(dr["Official LN"]) != null ? Convert.ToString(dr["Official LN"]).Trim() : "";
                                        createStudent.StudentID = Convert.ToString(dr["Student ID"]);
                                        createStudent.Password = Convert.ToString(dr["Password"]);
                                        createStudent.Teacher = Convert.ToString(dr["Teacher"]);
                                        createStudent.Grade = Convert.ToString(dr["Grade"]);
                                        createStudent.Custom1 = Convert.ToString(dr["Custom1"]);
                                        createStudent.Custom2 = Convert.ToString(dr["Custom2"]);
                                        createStudent.Custom3 = Convert.ToString(dr["Custom3"]);
                                        createStudent.Custom4 = Convert.ToString(dr["Custom4"]);
                                        createStudent.Custom5 = Convert.ToString(dr["Custom5"]);
                                        createStudent.SchoolCampus = Convert.ToString(dr["SchoolCampus"]);
                                        createStudent.PhotographyJobID = Convert.ToInt32(schoolYearSelectedItem.ID); createStudent.Address = "";
                                        createStudent.DOB = null; createStudent.City = ""; createStudent.State = "";
                                        createStudent.Zip = ""; createStudent.Phone = ""; createStudent.Emailaddress = ""; createStudent.IsStudent = "";
                                        createStudent.RecordStatus = true;
                                    }
                                    else
                                    {

                                        //param[0] = new SqlParameter("@opcode", opcode);
                                        createStudent.StudentImportID = maxImportId;
                                        createStudent.FirstName = Convert.ToString(dr["First Name"]) != null ? Convert.ToString(dr["First Name"]).Trim() : ""; ;
                                        createStudent.Lastname = Convert.ToString(dr["Last name"]) != null ? Convert.ToString(dr["Last name"]).Trim() : "";
                                        createStudent.OfficialFirstName = Convert.ToString(dr["Official FN"]) != null ? Convert.ToString(dr["Official FN"]).Trim() : "";
                                        createStudent.OfficialLastName = Convert.ToString(dr["Official LN"]) != null ? Convert.ToString(dr["Official LN"]).Trim() : "";
                                        createStudent.StudentID = Convert.ToString(dr["Student ID"]);
                                        createStudent.Password = Convert.ToString(dr["Password"]);
                                        createStudent.Teacher = Convert.ToString(dr["Teacher"]);
                                        createStudent.Grade = Convert.ToString(dr["Grade"]);
                                        createStudent.Custom1 = Convert.ToString(dr["Custom1"]);
                                        createStudent.Custom2 = Convert.ToString(dr["Custom2"]);
                                        createStudent.Custom3 = Convert.ToString(dr["Custom3"]);
                                        createStudent.Custom4 = Convert.ToString(dr["Custom4"]);
                                        createStudent.Custom5 = Convert.ToString(dr["Custom5"]);
                                        createStudent.SchoolCampus = Convert.ToString(dr["SchoolCampus"]);
                                        createStudent.PhotographyJobID = Convert.ToInt32(schoolYearSelectedItem.ID); createStudent.Address = Convert.ToString(dr["Address"]);
                                        if (!string.IsNullOrEmpty(dr["DOB"].ToString()))
                                        {
                                            DateTime date = Convert.ToDateTime(dr["DOB"].ToString());
                                            createStudent.DOB = dr["DOB"].ToString() == "" ? null : (DateTime?)date.Date;//(DateTime?)((System.DateTime)(dr["DOB"])).Date;
                                        }
                                        createStudent.City = Convert.ToString(dr["City"]); createStudent.State = Convert.ToString(dr["State"]);
                                        createStudent.Zip = Convert.ToString(dr["Zip"]); createStudent.Phone = Convert.ToString(dr["Phone"]); createStudent.Emailaddress = Convert.ToString(dr["Emailaddress"]);

                                        string stuType = Convert.ToString(dr["Student Type"]);
                                        createStudent.IsStudent = arrStudentType.Contains(stuType) ? stuType : "Misc";
                                        createStudent.RecordStatus = true;
                                    }
                                    db.Students.InsertOnSubmit(createStudent);
                                    db.SubmitChanges();
                                    tempStudentID = createStudent.ID;
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    isError = true;
                                    clsStatic.WriteExceptionLogXML(ex);
                                    MVVMMessageService.ShowMessage(ex.Message);
                                }
                            }
                            #endregion
                            #endregion  // Condition B
                            //}
                            //ts.Complete();
                            //Don't delete this code
                            //if neal freed sell Photo force to some clients then we need of this credits
                            //Commented by Mohan Tangella on 15-10-2015

                            //updating credits
                            //CreditLog creditLog = new CreditLog();
                            //PhotoSorterDBModelDataContext db2 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                            //string result = RSAEncryptDecrypt.Encrypt(Convert.ToString(creditCount - dt.Rows.Count));
                            //creditLog.Credits = result;

                            //creditLog.LicenseKey = maxImportId.ToString();
                            //creditLog.Mode = CreditsMode.Consumed.ToString();
                            //if (creditLog != null)
                            //{
                            //    db2.CreditLogs.InsertOnSubmit(creditLog);
                            //    db2.SubmitChanges();
                            //    int tempCredits = Convert.ToInt32(RSAEncryptDecrypt.Decrypt(creditLog.Credits));
                            //    credits = tempCredits;
                            //}
                            //}
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage("No records found for import."); return;
                        }

                        if (!isError)
                            MVVMMessageService.ShowMessage("Import batch " + _objStuImport.ID + " has been imported successfully under " + clsSchool.defaultSchoolName);
                        else
                            MVVMMessageService.ShowMessage("File has been imported with Errors under " + clsSchool.defaultSchoolName + ". Please check the exception log for more details.");
                        DialogResult = false;
                        isSave = true;
                        string filename = fullPath;
                    }
                }
                else
                    MVVMMessageService.ShowMessage(errorMessages.IMPORT_STUDENT_CANNOT_EMPTY);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                //if (ex.Message == "Cannot find table 0.")
                //    MVVMMessageService.ShowMessage("");
                //else
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// validate the excel for required columns
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string checkForRequiredColumns(DataTable dt)
        {
            //checking wether any column is missed.
            DataRow dr = dt.Rows[0];
            try
            {
                string Password = dr["Password"].ToString();
                string DOB = dr["DOB"].ToString();
                string Custom1 = dr["Custom1"].ToString();
                string Custom2 = dr["Custom2"].ToString();
                string Custom3 = dr["Custom3"].ToString();
                string IsStudent = dr["Student Type"].ToString();
                //string IsStudent = dr["SchoolCampus"].ToString();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return "Cannot find required column(s)." + Environment.NewLine + "Please check the excel file.";
            }
            return "";
        }
        /// <summary>
        /// to select the excel files
        /// </summary>
        private void selectFile()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Excel Files (.xlsx)|*.XLSX";
                //dlg.Filter = "Text files (*.txt)|*.TXT";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    fullPath = dlg.FileName;
                    fileName = dlg.SafeFileName;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        #endregion
    }
}
