using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;

namespace PhotoForce.School_Management
{
    public class AddEditActivityTypeViewModel : ViewModelBase
    {
        #region Initialization
        public bool isSave;
        #endregion

        #region Properties
        private string _activityType;

        public string activityType
        {
            get { return _activityType; }
            set { _activityType = value; NotifyPropertyChanged("activityType"); }
        }
        #endregion

        #region Commands
        public RelayCommand AddActivityTypeCommand
        {
            get
            {
                return new RelayCommand(addActivityType);
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
        private void addActivityType()
        {
            try
            {
                PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                ActivityType _objActivityType = new ActivityType();
                _objActivityType.Type = activityType;

                db.ActivityTypes.InsertOnSubmit(_objActivityType);
                db.SubmitChanges();
                isSave = true; DialogResult = false;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
       
        }

        private void windowClose()
        {
            DialogResult = false; isSave = false;
        }
        #endregion
    }
}
