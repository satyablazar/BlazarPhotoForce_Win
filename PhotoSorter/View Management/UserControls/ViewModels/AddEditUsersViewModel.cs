using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.View_Management
{
    public class AddEditUsersViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        public User tempSelectedUser;
        #endregion

        #region Properties
        string _userName;
        string _photographerID;
        string _studioName;
        string _address;
        string _city;
        string _zip;
        string _phone;

        public string phone
        {
            get { return _phone; }
            set { _phone = value; NotifyPropertyChanged(); }
        }
        public string zip
        {
            get { return _zip; }
            set { _zip = value; NotifyPropertyChanged(); }
        }
        public string city
        {
            get { return _city; }
            set { _city = value; NotifyPropertyChanged(); }
        }
        public string address
        {
            get { return _address; }
            set { _address = value; NotifyPropertyChanged(); }
        }
        public string studioName
        {
            get { return _studioName; }
            set { _studioName = value; NotifyPropertyChanged(); }
        }
        public string photographerID
        {
            get { return _photographerID; }
            set { _photographerID = value; NotifyPropertyChanged(); }
        }
        public string userName
        {
            get { return _userName; }
            set { _userName = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructor
        public AddEditUsersViewModel()
        {
        }
        public AddEditUsersViewModel(User selectedUser)
        {
            tempSelectedUser = selectedUser;
            userName = selectedUser.UserName.ToString();
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
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(studioName) && !string.IsNullOrEmpty(photographerID))
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                try
                {
                    if (tempSelectedUser != null)
                    {
                        tempSelectedUser = (from usr in db.Users where usr.Id == tempSelectedUser.Id select usr).FirstOrDefault();
                        //clsDashBoard.updateUser(db, Convert.ToInt32(tempSelectedUser.Id), userName);
                        tempSelectedUser.UserName = userName;
                        db.SubmitChanges();
                        isSave = true;
                        DialogResult = false;
                    }
                    else
                    {
                        //clsDashBoard.createNewUser(db, userName);
                        tempSelectedUser = new User();
                        tempSelectedUser.UserName = userName;
                        db.Users.InsertOnSubmit(tempSelectedUser);
                        db.SubmitChanges();
                        isSave = true; DialogResult = false;
                    }
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
        }

        private void windowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion
    }
}
