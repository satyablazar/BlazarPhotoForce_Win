using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;

namespace PhotoForce.PhotographyJobManagement
{
    public class SelectSchoolYearViewModel : ViewModelBase
    {
        //#region Initialization
        //int photoshootId = 0;
        //internal bool isSave = false;
        //#endregion

        //#region Properties
        //IEnumerable<PhotographyJob> _dgSelectSchoolYearData;
        //PhotographyJob _selectedSchoolYear;

        //public PhotographyJob selectedSchoolYear
        //{
        //    get { return _selectedSchoolYear; }
        //    set { _selectedSchoolYear = value; NotifyPropertyChanged("selectedSchoolYear"); }
        //}

        //public IEnumerable<PhotographyJob> dgSelectSchoolYearData
        //{
        //    get { return _dgSelectSchoolYearData; }
        //    set { _dgSelectSchoolYearData = value; NotifyPropertyChanged("dgSelectSchoolYearData"); }
        //}
        //#endregion

        #region Constructors
        //we are not using Select school year window.
        public SelectSchoolYearViewModel(int photoid)
        {
            //photoshootId = photoid;
            //BindSchoolYears();
        }
        #endregion

        //#region Commands
        //public RelayCommand OkCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(okClick);
        //    }
        //}
        //public RelayCommand SchoolYearGridDoubleClickCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(schoolYearGridDoubleClick);
        //    }
        //}
        //public RelayCommand WindowCloseCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(windowClose);
        //    }
        //}
        //#endregion

        //#region Methods
        //public void BindSchoolYears()
        //{
        //    dgSelectSchoolYearData = clsDashBoard.getJobs(clsSchool.defaultSchoolId, new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
        //}
        //private void updateSchoolYear()
        //{
        //    if (selectedSchoolYear == null)
        //    {
        //        MVVMMessageService.ShowMessage("Please select a school year.");
        //    }
        //    else
        //    {
        //        int photographyJobId = selectedSchoolYear.ID;
        //        string jobName = selectedSchoolYear.JobName;
        //        string photoShootName = clsDashBoard.getPhotoShootDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), photoshootId).FirstOrDefault().PhotoShotName;
        //        string caption = "Confirmation";
        //        string message = photoShootName + " will now be associated with the school year " + jobName + ".";
        //        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
        //        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
        //        if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
        //        {
        //            int retval = clsDashBoard.UpdatePhotoShootSeason(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), photographyJobId, photoshootId);
        //            if (retval > 0)
        //            {
        //                isSave = true;
        //                DialogResult = false;
        //            }
        //            else
        //                MVVMMessageService.ShowMessage("Record Not Saved.");

        //        }

        //    }

        //}

        //private void okClick()
        //{
        //    updateSchoolYear();
        //}
        //private void schoolYearGridDoubleClick()
        //{
        //    updateSchoolYear();
        //}
        //private void windowClose()
        //{
        //    DialogResult = false;
        //}
        //#endregion
    }
}
