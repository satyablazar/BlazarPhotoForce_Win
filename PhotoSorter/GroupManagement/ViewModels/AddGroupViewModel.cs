using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.ComponentModel;

namespace PhotoForce.GroupManagement
{
    public class AddGroupViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Initialization
        int schoolId = 0;
        int groupId = 0;
        internal bool isSave = false;
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public Group addEditGroup;
        #endregion

        #region Properties
        private string _groupName;
        private string _groupNotes;
        private string _schoolName;

        public string groupName
        {
            get { return _groupName; }
            set { _groupName = value; NotifyPropertyChanged("groupName"); }
        }
        public string groupNotes
        {
            get { return _groupNotes; }
            set { _groupNotes = value; NotifyPropertyChanged("groupNotes"); }
        }
        public string schoolName
        {
            get { return _schoolName; }
            set { _schoolName = value; NotifyPropertyChanged("schoolName"); }
        }
        # endregion

        #region IDataErrorInfo Members
        //public int errorCount = 0;

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
                ValidateUserInput(ref message, columnName);
                return message;
            }
        }
        #endregion

        #region Input Data
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="columnName"></param>
        private void ValidateUserInput(ref string message, string columnName)
        {
            switch (columnName)
            {
                case "groupName":
                    if (string.IsNullOrEmpty(groupName))
                    {
                        message = "Group Name is required."; errorCount++;
                    }
                    else
                    {
                        if (errorCount != 0)
                            errorCount--;
                    }
                    break;
            }
        }
        #endregion

        #endregion

        #region Constructors
        public AddGroupViewModel(int tempSchoolId, string tempSchoolName, int grpId)
        {
            schoolId = tempSchoolId;
            schoolName = tempSchoolName;
            groupId = grpId;

            try
            {
                schoolId = tempSchoolId;
                schoolName = tempSchoolName;
                groupId = grpId;
                if (tempSchoolId != 0)
                    schoolName = clsSchool.defaultSchoolName;
                if (groupId != 0)
                {
                    BindControls();
                }
                else
                {
                    schoolName = clsSchool.defaultSchoolName;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }

        }
        public AddGroupViewModel(int tempSchoolId, string tempSchoolName)
        {
            schoolId = tempSchoolId;
            schoolName = tempSchoolName;
        }
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(save);
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
        /// <summary>
        /// used to create new Group
        /// </summary>
        private void save()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (errorCount == 0)
                {
                    if (schoolId != 0) //schoolId != null && 
                    {
                        // Check for already existance of groupname with same school..
                        int countval = clsGroup.GetCountGroupUnderSchool(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), groupName, schoolId);
                        if (countval > 0)
                        {
                            MVVMMessageService.ShowMessage(errorMessages.GROUP_NAME_ALREADY_EXISTS_DB);
                            return;
                        }
                        //Add New group
                        addEditGroup = new Group();
                        addEditGroup.GroupName = groupName;
                        addEditGroup.Notes = groupNotes;
                        addEditGroup.SchoolID = schoolId;
                        addEditGroup.createdOn = DateTime.Now;
                        if (addEditGroup != null)
                        {
                            db.Groups.InsertOnSubmit(addEditGroup);
                            db.SubmitChanges();

                            isSave = true;
                            DialogResult = false;
                        }
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage(errorMessages.SELECT_DEFAULT_SCHOOL);
                        isSave = false;
                        DialogResult = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// this method will close the window
        /// </summary>
        private void windowClose()
        {
            DialogResult = false;
            isSave = false;
        }

        private void BindControls()
        {
            IEnumerable<Group> dt = clsGroup.getGroupDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), groupId);
            groupName = Convert.ToString(dt.First().GroupName);
            groupNotes = Convert.ToString(dt.First().Notes);
        }
        #endregion
    }
}
