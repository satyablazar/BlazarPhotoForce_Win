using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Collections;
using System.ComponentModel;

namespace PhotoForce.GroupManagement
{
    public class BulkRenameGroupViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Initilization
        public bool isSave = false;
        PhotoSorterDBModelDataContext db;
        public ArrayList arrGroupId = new ArrayList();  //#Mohan ; #NUnitTest
        #endregion

        #region Properties
        private string _groupName;
        private string _groupNotes;

        public string groupNotes
        {
            get { return _groupNotes; }
            set { _groupNotes = value; NotifyPropertyChanged("groupNotes"); }
        }
        public string groupName
        {
            get { return _groupName; }
            set { _groupName = value; NotifyPropertyChanged("groupName"); }
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
        public BulkRenameGroupViewModel(ArrayList grpId)
        {
            arrGroupId = grpId;
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
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        #endregion

        #region Methods
        private void bulkRename()
        {
            if (errorCount == 0)
            {
                try
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    string message = "";
                    if (arrGroupId.Count != 0)
                    {
                        if (arrGroupId.Count > 1)
                            message = errorMessages.BEFORE_RENAMING_SELECTED_GROUPS_CONFIRMATION1 + arrGroupId.Count + errorMessages.BEFORE_RENAMING_SELECTED_GROUPS_CONFIRMATION2;
                        else
                            message = errorMessages.BEFORE_RENAMING_SELECTED_GROUP_CONFIRMATION1 + arrGroupId.Count + errorMessages.BEFORE_RENAMING_SELECTED_GROUP_CONFIRMATION2;
                        string caption = "Confirmation";
                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                        if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                        {
                            int i = 0;
                            if (!string.IsNullOrEmpty(groupName))
                                i = clsGroup.UpdateMultipleGroupName(db, groupName, arrGroupId);
                            if (!string.IsNullOrEmpty(groupNotes))
                                i = clsGroup.UpdateMultipleGroupNotes(db, groupNotes, arrGroupId);
                            if (i != 0)
                            {
                                isSave = true;
                                DialogResult = false;
                            }
                        }
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage(errorMessages.SELECT_BULK_RENAME_GROUP);
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
        #endregion
    }
}
