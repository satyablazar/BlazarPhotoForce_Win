using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;

namespace PhotoForce.GroupManagement
{
    public class DeleteFoldersMessageViewModel : ViewModelBase
    {
        #region Initialization
        internal bool isCancelled = true;
        #endregion

        #region Properties
        private bool _isDeleteFoldersSelected;

        public bool isDeleteFoldersSelected
        {
            get { return _isDeleteFoldersSelected; }
            set { _isDeleteFoldersSelected = value; NotifyPropertyChanged("isDeleteFoldersSelected"); }
        }
        
        #endregion

        #region Constructors
        public DeleteFoldersMessageViewModel()
        {

        }
        #endregion

        #region Commands
        public RelayCommand ExportCommand
        {
            get
            {
                return new RelayCommand(export);
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
        private void export()
        {
            isCancelled = false;
            DialogResult = false;
        }
        private void windowClose()
        {
            isCancelled = true;
            DialogResult = false;
        }
        #endregion
    }
}
