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
    public class EditGroupViewModel : ViewModelBase , IDataErrorInfo
    {
        #region Initialization
        public bool isSave = false; //#Mohan ; #NUnitTest
        int groupId = 0;
        int schoolId = 0;
        PhotoSorterDBModelDataContext db;
        public Group addEditGroup;
        #endregion

        #region Properties
        private string _groupName;
        private string _groupNotes;

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
        #endregion

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
        public EditGroupViewModel(int tempSclId, int grpId)
        {
            try
            {
                groupId = grpId;
                schoolId = tempSclId;
                if (groupId != 0)
                    bindControls();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
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
        private void save()
        {
            if (errorCount == 0)
            {
                try
                {
                    if (groupId != 0)
                    {
                        db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                        // Check for already existance of groupname with same school..
                        int countval = clsGroup.GetCheckGroupUpdate(db, groupName, schoolId, groupId);
                        if (countval > 0)
                        {
                            MVVMMessageService.ShowMessage(errorMessages.GROUP_NAME_ALREADY_EXISTS_DB);
                            return;
                        }
                        //Update group
                        addEditGroup = clsGroup.updateGroup(db, groupId);
                        addEditGroup.GroupName = groupName;
                        addEditGroup.Notes = groupNotes;
                        if (addEditGroup != null)
                        {
                            db.SubmitChanges();
                            DialogResult = false;
                            isSave = true;
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
        private void windowClose()
        {
            DialogResult = false;
            isSave = false;
        }
        private void bindControls()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            IEnumerable<Group> dt = clsGroup.getGroupDetails(db, groupId);
            groupName = Convert.ToString(dt.First().GroupName);
            groupNotes = Convert.ToString(dt.First().Notes);
        }
        #endregion
    }
}
