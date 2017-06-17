using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.View_Management;
using PhotoForce.View_Management.UserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.WorkPlace.UserControls
{
    public class ManageUsersViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        int rowIndex = 0;
        #endregion

        #region Properties
        private ObservableCollection<User> _dgUsersData;
        private ObservableCollection<User> _selectedUsers;
        private User _selectedUser;

        public User selectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value; NotifyPropertyChanged();
                if (selectedUser != null)
                {
                    rowIndex = dgUsersData.IndexOf(selectedUser);
                }
            }
        }
        public ObservableCollection<User> selectedUsers
        {
            get { return _selectedUsers; }
            set { _selectedUsers = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<User> dgUsersData
        {
            get { return _dgUsersData; }
            set { _dgUsersData = value; NotifyPropertyChanged(); }
        }

        #region search & group panels
        bool _manageUsersTableShowGroupPanel;
        ShowSearchPanelMode _manageUsersTableSearchPanelMode;
        SearchControl _manageUsersTableSearchControl;
        bool _manageUsersSearchControlVisible;
        bool _isSearchControlVisible;

        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged(); }
        }
        public bool manageUsersSearchControlVisible
        {
            get { return _manageUsersSearchControlVisible; }
            set { _manageUsersSearchControlVisible = value; NotifyPropertyChanged(); }
        }
        public SearchControl manageUsersTableSearchControl
        {
            get { return _manageUsersTableSearchControl; }
            set { _manageUsersTableSearchControl = value; NotifyPropertyChanged(); }
        }
        public ShowSearchPanelMode manageUsersTableSearchPanelMode
        {
            get { return _manageUsersTableSearchPanelMode; }
            set { _manageUsersTableSearchPanelMode = value; NotifyPropertyChanged(); }
        }
        public bool manageUsersTableShowGroupPanel
        {
            get { return _manageUsersTableShowGroupPanel; }
            set { _manageUsersTableShowGroupPanel = value; NotifyPropertyChanged(); }
        }
        #endregion

        #endregion

        #region Constructor
        public ManageUsersViewModel()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            selectedUsers = new ObservableCollection<User>();
            dgUsersData = new ObservableCollection<User>();
        }
        #endregion

        #region Commands
        public RelayCommand ManageUsersTableKeyUpCommand
        {
            get
            {
                return new RelayCommand(manageUsersTableKeyUp);
            }
        }
        public RelayCommand RowUpdateCommand
        {
            get
            {
                return new RelayCommand(inlineGridEdit);
            }
        }
        #endregion

        #region Methods
        internal void bindData()
        {
            try
            {
                dgUsersData = new ObservableCollection<User>((from user in db.Users select user).ToList()); //clsDashBoard.getUsersData(db);

                //if (dgUsersData.Count > 0) { return; }
                if (dgUsersData.Count > 0 && rowIndex < dgUsersData.Count) { selectedUser = dgUsersData[rowIndex]; }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        internal void editUser()
        {
            if (selectedUser != null)
            {
                AddEditUsers _objAddEditUsers = new AddEditUsers(selectedUser);
                _objAddEditUsers.ShowDialog();

                if (((AddEditUsersViewModel)(_objAddEditUsers.DataContext)).isSave)
                {
                    // selectedUser["UserName"] = (((AddEditUsersViewModel)(_objAddEditUsers.DataContext)).userName).ToString();
                    try
                    {
                        dgUsersData.Insert(dgUsersData.Count <= 1 ? 0 : dgUsersData.IndexOf(selectedUser), ((AddEditUsersViewModel)(_objAddEditUsers.DataContext)).tempSelectedUser);
                        dgUsersData.Remove(selectedUser);
                        selectedUser = ((AddEditUsersViewModel)(_objAddEditUsers.DataContext)).tempSelectedUser;
                        selectedUsers.Add(selectedUser);
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }
            }
        }
        internal void addNewUser()
        {
            AddEditUsers _objAddEditUsers = new AddEditUsers();
            _objAddEditUsers.ShowDialog();
            if (((AddEditUsersViewModel)(_objAddEditUsers.DataContext)).isSave)
            {
                bindData();
                selectedUser = dgUsersData[dgUsersData.Count - 1];
            }
        }
        internal void deleteUser()
        {
            if (selectedUsers.Count > 0)
            {
                try
                {
                    string message = "";
                    if (selectedUsers.Count == 1)
                        message = "Are you sure you want to delete user '" + selectedUser.UserName.ToString() + "' ?";
                    else
                        message = "Are you sure you want to delete selected users ?";
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //db.SubmitChanges();

                        ArrayList selectedIds = new ArrayList();
                        foreach (User tempUser in selectedUsers)
                            selectedIds.Add(tempUser.Id);

                        //int totalRecordsCount = dgUsersData.Count();
                        //int deletedRecordsCount = selectedIds.Count;

                        clsDashBoard.deleteUser(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedIds);
                        bindData();
                        //createDeletedRecordsLogFile("Users", totalRecordsCount, deletedRecordsCount);

                        if(dgUsersData.Count > 0)
                            selectedUser = dgUsersData[dgUsersData.Count - 1];
                    }
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
        }
        //internal void deleteAllUsers()
        //{
        //    if (dgUsersData.Rows.Count > 0)
        //    {
        //        try
        //        {
        //            string message = "Are you sure you want to delete all users ?";
        //            string caption = "Confirmation";
        //            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
        //            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
        //            if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
        //            {
        //                //db.SubmitChanges();
        //                clsDashBoard.deleteUser(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), 0);
        //                bindData();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MVVMMessageService.ShowMessage(ex.Message);
        //        }
        //    }
        //}
        private void inlineGridEdit()
        {
            try
            {
                db.SubmitChanges();
                //clsDashBoard.updateUser(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(selectedUser.Id), selectedUser.UserName.ToString());
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void manageUsersTableKeyUp()
        {
            setButtonVisibility();
        }
        /// <summary>
        /// This method is used to group panels based on selected column
        /// </summary>
        public void groupPanels()
        {
            try
            {
                if (manageUsersTableShowGroupPanel)
                    manageUsersTableShowGroupPanel = false;
                else
                    manageUsersTableShowGroupPanel = true;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        public void searchPanels()
        {
            try
            {
                if (isSearchControlVisible == false || manageUsersTableSearchControl == null)
                {
                    manageUsersTableSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    manageUsersTableSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void setButtonVisibility()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
            (System.Windows.Application.Current as App).isNewVisible = true; (System.Windows.Application.Current as App).isDeleteVisible = true;
            (System.Windows.Application.Current as App).isEditVisible = true; (System.Windows.Application.Current as App).isDragVisible = true;
            (System.Windows.Application.Current as App).isSearchVisible = true; (System.Windows.Application.Current as App).isRefreshVisible = true;
            //(System.Windows.Application.Current as App).isDeleteAllVisible = true;
        }
        #endregion
    }
}
