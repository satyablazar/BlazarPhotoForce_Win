using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.Student_Management
{
    public class ReImportStudentsViewModel : ViewModelBase
    {

        #region Intialization
        public bool isSave = false;
        #endregion

        #region Properties
        public bool _isUpdateSplFields;

        public bool isUpdateSplFields
        {
            get { return _isUpdateSplFields; }
            set
            {
                _isUpdateSplFields = value; NotifyPropertyChanged("isUpdateSplFields");
                //if user opt to update first name , last name and student id show him a warning message
                if (isUpdateSplFields)
                {
                    string tempMessage = "You should have a backup before updating important student information. Proceed?";
                    if (MVVMMessageService.ShowMessage(tempMessage, "Warning", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.No)
                    {
                        DialogResult = false;
                        isUpdateSplFields = false;
                        isSave = false;
                    }
                }
            }
        }
        #endregion

        #region Constructors
        public ReImportStudentsViewModel()
        {
        }
        #endregion

        #region Commands
        public RelayCommand YesCommand
        {
            get
            {
                return new RelayCommand(yes);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(no);
            }
        }
        #endregion

        #region Methods
        private void yes()
        {
            DialogResult = false;
            isSave = true;
        }
        private void no()
        {
            DialogResult = false;
            isSave = false;
        }
        #endregion


    }
}
