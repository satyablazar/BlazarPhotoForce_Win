using PhotoForce.App_Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoForce.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged
    { 
        #region Message Service
        private IMessageBoxService _mvvmMessageService;

        public IMessageBoxService MVVMMessageService
        {
            get
            {
                if (_mvvmMessageService == null)
                {
                    _mvvmMessageService = new MessageService();
                }
                return _mvvmMessageService;
            }
            set
            {
                _mvvmMessageService = value;
            }
        }
        #endregion

        #region Initialization
        Regex regex = new Regex("^[a-zA-Z0-9]*$");
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Dialog Close Property
        private bool? _DialogResult;
        public bool? DialogResult
        {
            get { return _DialogResult; }
            set { _DialogResult = value; NotifyPropertyChanged("DialogResult"); }
        }
        #endregion

        #region Validation Input Data
        #region Properties
        internal int errorCount { get; set; }

        string _studentId;
        string _firstName;
        string _lastName;
        int? _cbPhotoJobsSelectedValue;
        string _vendorOrderNo;
        int _credits;
        string _password;
        private int _ratingData;
        //string _paymentMethod;

        //public string paymentMethod
        //{
        //    get { return _paymentMethod; }
        //    set { _paymentMethod = value; NotifyPropertyChanged("paymentMethod"); }
        //}
        public string password
        {
            get { return _password; }
            set { _password = value; NotifyPropertyChanged(); } //"password"
        }
        //public string _fulfilledName;

        //public string fulfilledName
        //{
        //    get { return _fulfilledName; }
        //    set
        //    {
        //        _fulfilledName = value; NotifyPropertyChanged("fulfilledName");
        //    }
        //}

        public string vendorOrderNo
        {
            get { return _vendorOrderNo; }
            set { _vendorOrderNo = value; NotifyPropertyChanged("vendorOrderNo"); }
        }
        public int credits
        {
            get { return _credits; }
            set { _credits = value; NotifyPropertyChanged("credits"); }
        }
        public string studentId
        {
            get { return _studentId; }
            set { _studentId = value; NotifyPropertyChanged("studentId"); }
        }
        public string firstName
        {
            get { return _firstName; }
            set { _firstName = value; NotifyPropertyChanged("firstName"); }
        }
        public string lastName
        {
            get { return _lastName; }
            set { _lastName = value; NotifyPropertyChanged("lastName"); }
        }
        public int? cbPhotoJobsSelectedValue
        {
            get { return _cbPhotoJobsSelectedValue; }
            set { _cbPhotoJobsSelectedValue = value; NotifyPropertyChanged("cbPhotoJobsSelectedValue"); }
        }
        public int ratingData
        {
            get { return _ratingData; }
            set { _ratingData = value; NotifyPropertyChanged("ratingData"); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="columnName"></param>
        internal void validateUserInput(ref string message, string columnName)
        {
            switch (columnName)
            {
                case "studentId":
                    if (string.IsNullOrEmpty(studentId))
                    {
                        message = "Student Id is required."; errorCount++;
                    }
                    else if (!string.IsNullOrEmpty(studentId) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) && cbPhotoJobsSelectedValue != 0 && cbPhotoJobsSelectedValue != null)//(errorCount != 0)
                    {
                        errorCount = 0;
                    }
                    break;
                case "firstName":
                    if (string.IsNullOrEmpty(firstName))
                    {
                        message = "First Name is required."; errorCount++;
                    }
                    else if (!string.IsNullOrEmpty(studentId) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) && cbPhotoJobsSelectedValue != 0 && cbPhotoJobsSelectedValue != null)//(errorCount != 0)
                    {
                        errorCount = 0;
                    }
                    break;
                case "lastName":
                    if (string.IsNullOrEmpty(lastName))
                    {
                        message = "Last Name is required."; errorCount++;
                    }
                    else if (!string.IsNullOrEmpty(studentId) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) && cbPhotoJobsSelectedValue != 0 && cbPhotoJobsSelectedValue != null)//(errorCount != 0)
                    {
                        errorCount = 0;
                    }
                    break;
                case "password":
                    if (!string.IsNullOrEmpty(password))
                    {
                        if ((regex.IsMatch(password)))
                            errorCount = 0;
                        else
                        { message = "Please enter valid password (alphanumeric only)"; errorCount++; }
                    }
                    break;
                case "cbPhotoJobsSelectedValue":
                    if (cbPhotoJobsSelectedValue == null || cbPhotoJobsSelectedValue == 0)
                    {
                        message = "select a school year"; errorCount++;
                    }
                    else if (!string.IsNullOrEmpty(studentId) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) && cbPhotoJobsSelectedValue != 0 && cbPhotoJobsSelectedValue != null)//(errorCount != 0)
                    {
                        errorCount = 0;
                    }
                    break;
                case "credits":
                    if (columnName == "credits")
                    {
                        if (credits <= 0)
                        {
                            message = "Please enter valid Credits"; errorCount++;
                        }
                        else
                        {
                            if (errorCount != 0)
                                errorCount = 0;
                        }
                    }

                    break;
                case "vendorOrderNo":
                    if (columnName == "vendorOrderNo")
                    {
                        if (!Regex.IsMatch(string.IsNullOrEmpty(vendorOrderNo) ? "s" : vendorOrderNo, @"^(\d{0,9}|\d{0,9})$"))
                        {
                            message = "Please enter valid number"; errorCount++;
                        }
                        else
                        {
                            if (errorCount != 0)
                                errorCount = 0;
                        }
                    }
                    break;
                case "ratingData":
                    if (ratingData < 0 || ratingData > 10)
                    {
                        message = "Rating should be between 0-10."; errorCount++;
                    }
                    else
                    {
                        if (errorCount != 0)
                            errorCount = 0;
                    }
                    break;
                //case "paymentMethod":
                //    if (string.IsNullOrEmpty(paymentMethod))
                //    {
                //        message = "payment method is required."; errorCount++;
                //    }
                //    else 
                //        errorCount = 0;
                //    break;
            }
        }
        #endregion

        #endregion

        #region Log file for deleted records
        internal void createDeletedRecordsLogFile(string selectedGrid, int totalRecords, int deletedRecords)
        {
            //clsErrorLog objError = new clsErrorLog();
            //objError.Source = selectedGrid;
            //objError.MethodName = "Deleting " + selectedGrid;
            //objError.Message = selectedGrid + " log file. \n Schoolname : " + clsSchool.defaultSchoolName + "\n User name : " + clsStatic.userName + " \n Total records count : " + totalRecords +
            //                    "\n Action: Record(s) deleted \n Deleted records count : " + deletedRecords + " \n Remaining records count : " + (totalRecords - deletedRecords) + "\n";
            //objError.UserComments = clsStatic.userName;
            //objError.DateTime = DateTime.Now;
            //clsStatic.WriteErrorLog(objError, selectedGrid + " Info.");
            ////clsStatic.WriteErrorLog(objError, objstatic.ErrorLogXML);
        }
        #endregion
    }
}
