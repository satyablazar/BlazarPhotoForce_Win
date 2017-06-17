using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.PhotoShoot_Management
{
    public class AddNewPhotoshootTypeViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        #endregion

        #region Properties
        string _photoshootTypeName;

        public string photoshootTypeName
        {
            get { return _photoshootTypeName; }
            set { _photoshootTypeName = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructor
        public AddNewPhotoshootTypeViewModel()
        {
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
        private void saveAndClose()
        {
            if (!string.IsNullOrEmpty(photoshootTypeName))
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                PhotoshootTypeTable _objPhotoshootTypeTable = new PhotoshootTypeTable();
                _objPhotoshootTypeTable.PhotoshootType = photoshootTypeName;

                db.PhotoshootTypeTables.InsertOnSubmit(_objPhotoshootTypeTable);
                db.SubmitChanges();

                isSave = true;
                DialogResult = false;
            }
            else
                return;
        }
        private void windowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion
    }
}
