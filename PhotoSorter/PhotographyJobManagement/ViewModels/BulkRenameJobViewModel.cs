using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.PhotographyJobManagement
{
    public class BulkRenameJobViewModel : ViewModelBase
    {
        #region Initilization
        public bool isSave = false;
        PhotoSorterDBModelDataContext db;
        public ArrayList arrJobId = new ArrayList();
        #endregion

        #region Properties
        private string _jobName;
        private DateTime _jobDate;

        public DateTime jobDate
        {
            get { return _jobDate; }
            set { _jobDate = value; NotifyPropertyChanged("jobDate"); }
        }

        public string jobName
        {
            get { return _jobName; }
            set { _jobName = value; NotifyPropertyChanged("jobName"); }
        }

        #endregion

        #region Constructors
        public BulkRenameJobViewModel(ArrayList tempJobId)
        {
            arrJobId = tempJobId;
            jobDate = DateTime.Now;
        }
        #endregion

        #region Commands
        public RelayCommand BulkRenameCommand
        {
            get
            {
                return new RelayCommand(bulkRename);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowCloseCommand);
            }
        }
        #endregion

        #region Methods
        private void bulkRename()
        {
            try
            {
                if (arrJobId.Count == 0)
                {
                    MVVMMessageService.ShowMessage("Please select school year(s) to rename.");
                    return;
                }
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                string message = "";
                if (arrJobId.Count > 1)
                    message = errorMessages.BEFORE_RENAMING_SELECTED_JOBS_CONFIRMATION1 + arrJobId.Count + errorMessages.BEFORE_RENAMING_SELECTED_JOBS_CONFIRMATION2;
                else
                    message = errorMessages.BEFORE_RENAMING_SELECTED_JOB_CONFIRMATION1 + arrJobId.Count + errorMessages.BEFORE_RENAMING_SELECTED_JOB_CONFIRMATION2;
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    int i = 0;
                    if (!string.IsNullOrEmpty(jobName))
                        i = clsDashBoard.UpdateMultipleJobName(db, jobName, arrJobId);
                    if (!string.IsNullOrEmpty(jobDate.ToString()))
                        i = clsDashBoard.UpdateMultipleJobDate(db, jobDate.ToString("yyyy-MM-dd HH:mm:ss"), arrJobId);
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
        private void windowCloseCommand()
        {
            DialogResult = false;
            isSave = false;
        }
        #endregion
    }
}
