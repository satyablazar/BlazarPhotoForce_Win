using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using PhotoForce.Helpers;

namespace PhotoForce.WorkPlace
{
    public class GeneratePasswordViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        ArrayList arrSelectedStudentIds = new ArrayList();

        Student _objStudent = new Student();
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string inputString = "", min = "", sec = "";
        bool isAllStudents = false;
        internal bool isSelectedStudents = false;
        internal bool isSave =  false;
        #endregion

        #region Properties
        bool _isSelectedStudentsEnable = false;

        public bool isSelectedStudentsEnable
        {
            get { return _isSelectedStudentsEnable; }
            set { _isSelectedStudentsEnable = value; NotifyPropertyChanged("isSelectedStudentsEnable"); }
        }
        #endregion

        #region Constructors
        public GeneratePasswordViewModel(ArrayList tempStudentsArray)
        {
            arrSelectedStudentIds = tempStudentsArray;
            if (arrSelectedStudentIds.Count != 0)
                isSelectedStudentsEnable = true;
        }
        #endregion

        #region Commands
        public RelayCommand GeneratePasswordCommand
        {
            get
            {
                return new RelayCommand(generatePassword);
            }
        }
        public RelayCommand<string> SelectedStudentsCommand
        {
            get
            {
                return new RelayCommand<string>(selectedStudents);
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
        private void windowClose()
        {
            DialogResult = false;
        }
        private void generatePassword()
        {
            try
            {
                if (isAllStudents == true)
                {
                    DataTable dtGetAllStudents = clsStudent.getAllStudents(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId);
                    //dtGetAllStudents = clsStudent.getAllStudents(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));     //this method generate the pwd's for students of all schools instead of the default school   //mohan
                    if (dtGetAllStudents != null)
                    {
                        isSave = true;
                        foreach (DataRow row in dtGetAllStudents.Rows)
                        {
                            try
                            {
                                object value = row["Password"];

                                if ((value == DBNull.Value) || (value.ToString() == ""))
                                {
                                    int ID = Convert.ToInt32(row["ID"]);
                                    string lstnm = Convert.ToString(row["Lastname"]);
                                    if (string.IsNullOrEmpty(lstnm))
                                        lstnm = "LN";
                                    else if (lstnm.Length < 2)
                                        lstnm += lstnm;
                                    lstnm = lstnm.Substring(0, 2);
                                    lstnm = lstnm.ToUpper();
                                    string fstnm = Convert.ToString(row["FirstName"]);
                                    if (string.IsNullOrEmpty(fstnm))
                                        fstnm = "FN";
                                    else if (fstnm.Length < 2)
                                        fstnm += fstnm;
                                    fstnm = fstnm.Substring(0, 2);
                                    fstnm = fstnm.ToUpper();
                                    string Dtetym = Convert.ToString(row["CreatedOn"]);
                                    min = Dtetym != "" ? Dtetym.Split(':').ToArray()[1] : "";

                                    string ticks = DateTime.Now.Ticks.ToString();
                                    sec = ticks.Substring(ticks.Length - 2, 2);

                                    inputString = lstnm + fstnm + sec + min;
                                    string tempUserId = Convert.ToString(ID);
                                    if (tempUserId.Length == 1)
                                        tempUserId = "0" + tempUserId;

                                    var regexItem = new Regex("^[a-zA-Z0-9]*$");
                                    if (!regexItem.IsMatch(inputString))
                                    {
                                        inputString = Regex.Replace(inputString, "[^0-9a-zA-Z]+", "");
                                    }
                                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                    var tempPasswod = inputString.Swap(alphabet, tempUserId);

                                    var regexItem1 = new Regex("^[a-hj-np-zA-HJ-NP-Z1-9]*$");
                                    if (!regexItem1.IsMatch(tempPasswod))
                                    {
                                        tempPasswod = Regex.Replace(tempPasswod, "[^a-hj-np-zA-HJ-NP-Z1-9]+", "5");
                                    }

                                    _objStudent = clsStudent.updateStudent(db, ID);
                                    _objStudent.Password = tempPasswod;
                                    if (_objStudent != null)
                                    {
                                        db.SubmitChanges();
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
                    DialogResult = false;
                }
                else if (isSelectedStudents == true)
                {
                    string s = generateStudentPassword();
                    DialogResult = false;
                }
                else
                {
                    MVVMMessageService.ShowMessage("Please select a option first.");
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal string generateStudentPassword()
        {
            DataTable dtSelectedStudentsData = clsStudent.getSelectedStudents(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrSelectedStudentIds);
            if (dtSelectedStudentsData != null)
            {
                isSave = true;
                foreach (DataRow row in dtSelectedStudentsData.Rows)
                {
                    try
                    {
                        int ID = Convert.ToInt32(row["ID"]);
                        string lstnm = Convert.ToString(row["Lastname"]);
                        if (string.IsNullOrEmpty(lstnm))
                            lstnm = "LN";
                        else if (lstnm.Length < 2)
                            lstnm += lstnm;
                        lstnm = lstnm.Substring(0, 2);
                        lstnm = lstnm.ToUpper();
                        string fstnm = Convert.ToString(row["FirstName"]);
                        if (string.IsNullOrEmpty(fstnm))
                            fstnm = "FN";
                        else if (fstnm.Length < 2)
                            fstnm += fstnm;
                        fstnm = fstnm.Substring(0, 2);
                        fstnm = fstnm.ToUpper();
                        string Dtetym = Convert.ToString(row["CreatedOn"]);
                        min = Dtetym != "" ? Dtetym.Split(':').ToArray()[1] : "";

                        string ticks = DateTime.Now.Ticks.ToString();
                        sec = ticks.Substring(ticks.Length - 2, 2);

                        inputString = lstnm + fstnm + sec + min;
                        string UseId = Convert.ToString(ID);
                        if (UseId.Length == 1)
                            UseId = "0" + UseId;

                        var regexItem = new Regex("^[a-zA-Z0-9]*$");
                        if (!regexItem.IsMatch(inputString))
                        {
                            inputString = Regex.Replace(inputString, "[^0-9a-zA-Z]+", "");
                        }

                        db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                        var pasword = inputString.Swap(alphabet, UseId);

                        var regexItem1 = new Regex("^[a-hj-np-zA-HJ-NP-Z1-9]*$");
                        if (!regexItem1.IsMatch(pasword))
                        {
                            pasword = Regex.Replace(pasword, "[^a-hj-np-zA-HJ-NP-Z1-9]+", "5");
                        }

                        _objStudent = clsStudent.updateStudent(db, ID);
                        _objStudent.Password = pasword;
                        if (_objStudent != null)
                        {
                            db.SubmitChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }
            }
            return _objStudent.Password;
        }
        private void selectedStudents(string tempSelection)
        {
            if (tempSelection == "All")
            {
                isAllStudents = true;
            }
            else
            {
                isSelectedStudents = true;
            }
        }
        #endregion
    }
}
