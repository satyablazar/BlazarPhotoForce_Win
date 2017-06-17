using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;

namespace PhotoForce.School_Management
{
    public class DeviceScanningViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        string tempSchoolName = "";
        PrintDeviceQRCode _objdevicebarcode;
        #endregion

        #region Properties
        string _schoolName;
        DateTime _date;
        string _syncText;

        public string syncText
        {
            get { return _syncText; }
            set { _syncText = value; NotifyPropertyChanged("syncText"); }
        }
        public DateTime date
        {
            get { return _date; }
            set { _date = value; NotifyPropertyChanged("date"); }
        }
        public string schoolName
        {
            get { return _schoolName; }
            set { _schoolName = value; NotifyPropertyChanged("schoolName"); }
        }
        #endregion

        #region Constructors
        public DeviceScanningViewModel(string sclName)
        {
            tempSchoolName = sclName;
            FillControls(tempSchoolName);
        }
        #endregion

        #region Commands
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand PrintCommand
        {
            get
            {
                return new RelayCommand(print);
            }
        }
        #endregion

        #region Methods
        private void FillControls(string SchoolName)
        {
            schoolName = SchoolName;
            date = DateTime.Now;
        }
        private void print()
        {
            if (syncText != "" && syncText != null)
            {
                _objdevicebarcode = new PrintDeviceQRCode(schoolName, syncText);
                _objdevicebarcode.ShowDialog();
            }
            else
                MVVMMessageService.ShowMessage("Please enter Scanner Syc");
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion


    }
}
