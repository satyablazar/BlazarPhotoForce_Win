using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.WorkPlace
{
    public class LoginViewModel : ViewModelBase
    {
        #region Initialization
        public bool isLogin = false;
        string connectionString = "";
        #endregion

        #region Properties
        string _userName;

        public string userName
        {
            get { return _userName; }
            set { _userName = value; NotifyPropertyChanged("userName"); }
        }
        #endregion

        #region Constructors
        public LoginViewModel(string tempConnectionString)
        {
            connectionString = tempConnectionString;
        }
        #endregion

        #region Commands
        public RelayCommand LoginCommand
        {
            get
            {
                return new RelayCommand(login);
            }
        }
        public RelayCommand RegisterCommand
        {
            get
            {
                return new RelayCommand(register);
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
        private void login()
        {
            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(connectionString);
            if (string.IsNullOrEmpty(userName)) { MVVMMessageService.ShowMessage("Please enter User Name."); }
            else
            {
                User tempUser = clsUsers.checkForUserName(db, userName);
                if (tempUser == null) { MVVMMessageService.ShowMessage("user name you entered was in-correct."); isLogin = false; }
                else
                {
                    clsStatic.userName = tempUser.UserName; clsStatic.userId = tempUser.Id;
                    isLogin = true; DialogResult = false;
                }

            }
        }
        private void register()
        {
            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(connectionString);
            if (string.IsNullOrEmpty(userName)) { MVVMMessageService.ShowMessage("Please enter User Name."); }
            else
            {
                User tempUser = clsUsers.checkForUserName(db, userName);
                if (tempUser == null)
                {
                    User _objUser = new User();
                    _objUser.UserName = userName;
                    db.Users.InsertOnSubmit(_objUser);
                    db.SubmitChanges();
                    MVVMMessageService.ShowMessage("User with name " + userName + " created succesfully. Please use same name to login.");
                    isLogin = false;
                }
                else
                {
                    MVVMMessageService.ShowMessage("User with same name already exists.");
                    isLogin = false;
                }

            }

        }
        private void windowClose()
        {
            DialogResult = false; isLogin = false;
        }
        #endregion
    }
}
