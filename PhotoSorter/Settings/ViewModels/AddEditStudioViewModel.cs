using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.Settings
{
    public class AddEditStudioViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        public Studio _objStudio = new Studio();
        #endregion

        #region Properties
        string _pfStudioId;
        string _studioName;
        string _primaryContact;
        string _emailAddress;
        string _address;
        string _city;
        string _state;
        string _phone;
        string _zip;

        public string zip
        {
            get { return _zip; }
            set { _zip = value; NotifyPropertyChanged(); }
        }
        public string phone
        {
            get { return _phone; }
            set { _phone = value; NotifyPropertyChanged(); }
        }
        public string state
        {
            get { return _state; }
            set { _state = value; NotifyPropertyChanged(); }
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
        public string emailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; NotifyPropertyChanged(); }
        }
        public string primaryContact
        {
            get { return _primaryContact; }
            set { _primaryContact = value; NotifyPropertyChanged(); }
        }
        public string studioName
        {
            get { return _studioName; }
            set { _studioName = value; NotifyPropertyChanged(); }
        }
        public string pfStudioId
        {
            get { return _pfStudioId; }
            set { _pfStudioId = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructor
        public AddEditStudioViewModel()
        {
            _objStudio = null;
        }
        public AddEditStudioViewModel(Studio tempStudio)
        {
            _objStudio = tempStudio;
            bindData();
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(saveAndClose);
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
        private void bindData()
        {
            pfStudioId = _objStudio.PFStudioId;
            studioName = _objStudio.StudioName;
            primaryContact = _objStudio.PrimaryContact;
            emailAddress = _objStudio.Email;
            address = _objStudio.Address;
            city = _objStudio.City;
            state = _objStudio.State;
            phone = _objStudio.Phone;
            zip = _objStudio.Zip;
        }
        private void saveAndClose()
        {
            try
            {
                if (string.IsNullOrEmpty(pfStudioId) || string.IsNullOrEmpty(studioName) || string.IsNullOrEmpty(primaryContact)) { return; }

                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (_objStudio == null)
                {
                    _objStudio = new Studio();
                    _objStudio.PFStudioId = pfStudioId;
                    _objStudio.StudioName = studioName;
                    _objStudio.PrimaryContact = primaryContact;
                    _objStudio.Email = emailAddress;
                    _objStudio.Address = address;
                    _objStudio.City = city;
                    _objStudio.State = state;
                    _objStudio.Phone = phone;
                    _objStudio.Zip = zip;

                    db.Studios.InsertOnSubmit(_objStudio);
                    
                }
                else
                {
                    _objStudio = (from stu in db.Studios where stu.Id == _objStudio.Id select stu).FirstOrDefault();

                    _objStudio.PFStudioId = pfStudioId;
                    _objStudio.StudioName = studioName;
                    _objStudio.PrimaryContact = primaryContact;
                    _objStudio.Email = emailAddress;
                    _objStudio.Address = address;
                    _objStudio.City = city;
                    _objStudio.State = state;
                    _objStudio.Phone = phone;
                    _objStudio.Zip = zip;
                }
                db.SubmitChanges();
                isSave = true;
                DialogResult = false;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
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
