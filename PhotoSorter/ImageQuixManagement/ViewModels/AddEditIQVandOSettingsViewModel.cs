using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.ImageQuixManagement
{
    public class AddEditIQVandOSettingsViewModel : ViewModelBase
    {
         #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        public IQVandoSetting _objVandoSettings;
        string isFrom;
        IQVandoSetting selectedVandoSetting;
        IQAccount selectedIQAccount;
        #endregion


        #region Properties
        string _iqVandoId;
        string _description;
        bool _isDefaultChecked;

        public bool isDefaultChecked
        {
            get { return _isDefaultChecked; }
            set { _isDefaultChecked = value; NotifyPropertyChanged("isDefaultChecked"); }
        }
        public string description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged(); }
        }
        public string iqVandoId
        {
            get { return _iqVandoId; }
            set 
            { 
                _iqVandoId = value; NotifyPropertyChanged("iqVandoId");

                if (!string.IsNullOrEmpty(iqVandoId) )
                {
                    try
                    {
                        Convert.ToInt32(value);
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage("Please enter valid Vando Id.");

                        iqVandoId = "";
                    }

                }
            }
        }
        #endregion

        #region  Constructors
        public AddEditIQVandOSettingsViewModel(string callFrom, IQVandoSetting tempVandoSetting, IQAccount tempIQAccount)
        {
            isFrom = callFrom; selectedVandoSetting = tempVandoSetting; selectedIQAccount = tempIQAccount;
            if (isFrom == "Edit-IQVandoSettings")
            {
                iqVandoId = selectedVandoSetting.IQVandoId.ToString();
                description = selectedVandoSetting.Description;
                isDefaultChecked = Convert.ToBoolean(selectedVandoSetting.IsDefault);
            }
        }
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(saveIQVandoSettings);
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
        private void saveIQVandoSettings()
        {
            if (!string.IsNullOrEmpty(iqVandoId.ToString()))
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                _objVandoSettings = new IQVandoSetting();
                try
                {
                    if (isFrom == "Edit-IQVandoSettings")
                    {
                        _objVandoSettings = (from IQV in db.IQVandoSettings where IQV.Id == selectedVandoSetting.Id select IQV).FirstOrDefault();
                        if (_objVandoSettings != null)
                        {
                            _objVandoSettings.IQVandoId = Convert.ToInt32(iqVandoId);
                            _objVandoSettings.Description = description;
                            _objVandoSettings.IsDefault = isDefaultChecked;

                            db.SubmitChanges();
                            isSave = true;
                        }
                    }
                    else if (isFrom == "New-IQVandoSettings")
                    {
                        _objVandoSettings = (from IQV in db.IQVandoSettings where IQV.IQVandoId == Convert.ToInt32(iqVandoId) select IQV).FirstOrDefault();
                        if (_objVandoSettings == null)
                        {
                            _objVandoSettings = new IQVandoSetting();
                            _objVandoSettings.IQVandoId = Convert.ToInt32(iqVandoId);
                            _objVandoSettings.Description = description;
                            _objVandoSettings.IsDefault = isDefaultChecked;
                            _objVandoSettings.IQAccountId = selectedIQAccount.Id;

                            db.IQVandoSettings.InsertOnSubmit(_objVandoSettings);
                            db.SubmitChanges();
                            isSave = true;
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage("VandO Settings with same Id already exists.");
                        }
                    }

                    if (isDefaultChecked && isSave)
                    {
                        clsImageQuix.updateIQVandoSettings(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), _objVandoSettings.Id, selectedIQAccount.Id);
                    }
                    DialogResult = false;
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
