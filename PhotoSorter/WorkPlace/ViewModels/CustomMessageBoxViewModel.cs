using PhotoForce.Extensions;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.WorkPlace
{
    public class CustomMessageBoxViewModel : ViewModelBase
    {
         #region Initialization 
        public bool isSave = true;
        public string selectedOption = "";
        #endregion

        #region Properties
        string _dbAlertMessage;

        public string dbAlertMessage
        {
            get { return _dbAlertMessage; }
            set { _dbAlertMessage = value; NotifyPropertyChanged("dbAlertMessage"); }
        }
        #endregion

        #region Constructor
        public CustomMessageBoxViewModel(string message)
        {
            dbAlertMessage = message;
        }
        #endregion

        #region Commands
        public RelayCommand OpenDBCommand
        {
            get
            {
                return new RelayCommand(openDB);
            }
        }
        public RelayCommand UpgradeNowCommand
        {
            get
            {
                return new RelayCommand(upgradeNow);
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
        private void openDB()
        {
            selectedOption = ConnectionState.OpenDB.ToString();
            DialogResult = false;
        }
        private void upgradeNow()
        {
            selectedOption = ConnectionState.UpgradeDB.ToString(); DialogResult = false;
        }
        private void windowClose()
        {
            selectedOption = ConnectionState.Cancel.ToString(); DialogResult = false;
        }

        #endregion
    }
}
