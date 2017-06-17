using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Collections;
using PhotoForce.PhotographyJobManagement;

namespace PhotoForce.PhotoShoot_Management
{
    public class BulkRenamePhotoShootViewModel : ViewModelBase
    {
        # region Initialization and Declaration
        ArrayList arrShootId;
        public bool isSave = false;
        # endregion

        #region Properties
        private string _photoShootName;
        private DateTime _photoShootDate;
        private IEnumerable<PhotographyJob> _cbPhotographyJobData;
        private int? _selectedJobId;

        public int? selectedJobId
        {
            get { return _selectedJobId; }
            set { _selectedJobId = value; NotifyPropertyChanged("selectedJobId"); }
        }
        public IEnumerable<PhotographyJob> cbPhotographyJobData
        {
            get { return _cbPhotographyJobData; }
            set { _cbPhotographyJobData = value; NotifyPropertyChanged("cbPhotographyJobData"); }
        }
        public DateTime photoShootDate
        {
            get { return _photoShootDate; }
            set { _photoShootDate = value; NotifyPropertyChanged("photoShootDate"); }
        }
        public string photoShootName
        {
            get { return _photoShootName; }
            set { _photoShootName = value; NotifyPropertyChanged("photoShootName"); }
        }
        #endregion

        #region Constructors
        public BulkRenamePhotoShootViewModel(ArrayList tempArrShootId)
        {
            arrShootId = tempArrShootId;
            photoShootDate = DateTime.Now;
            BindPhotoJobs(clsSchool.defaultSchoolId);
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(bulkRenamePhotoShoot);
            }
        }
        public RelayCommand NewSchoolYearCommand
        {
            get
            {
                return new RelayCommand(newSchoolYear);
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
        private void BindPhotoJobs(int schoolId)
        {
            try
            {
                cbPhotographyJobData = clsDashBoard.getJobs(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), schoolId);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void bulkRenamePhotoShoot()
        {
            try
            {
                if (arrShootId.Count == 0)
                {
                    MVVMMessageService.ShowMessage("Please select photoshoot(s) rename."); return;
                }
                string message = "";
                if (arrShootId.Count > 1)
                    message = errorMessages.BEFORE_RENAMING_SELECTED_PHOTOSHOOTS_CONFIRMATION1 + arrShootId.Count + errorMessages.BEFORE_RENAMING_SELECTED_PHOTOSHOOTS_CONFIRMATION2;
                else
                    message = errorMessages.BEFORE_RENAMING_SELECTED_PHOTOSHOOT_CONFIRMATION1 + arrShootId.Count + errorMessages.BEFORE_RENAMING_SELECTED_PHOTOSHOOT_CONFIRMATION2;
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    int i = 0;
                    if (!string.IsNullOrEmpty(photoShootName))
                        i = clsDashBoard.updateMultiplePhotoShootName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), photoShootName, arrShootId);
                    if (!string.IsNullOrEmpty(photoShootDate.ToString()))
                        i = clsDashBoard.updateMultiplePhotoShootDate(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), photoShootDate.ToString("yyyy-MM-dd HH:mm:ss"), arrShootId);
                    if (!string.IsNullOrEmpty(selectedJobId.ToString()))
                        i = clsDashBoard.updateMultiplePhotographyJobId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedJobId, arrShootId);
                    if (i != 0)
                    {
                        DialogResult = false;
                        isSave = true;
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void newSchoolYear()
        {
            AddNewPhotographyJob _objAddNewPhotographyJob = new AddNewPhotographyJob();
            _objAddNewPhotographyJob.ShowDialog();
            BindPhotoJobs(clsSchool.defaultSchoolId);
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
