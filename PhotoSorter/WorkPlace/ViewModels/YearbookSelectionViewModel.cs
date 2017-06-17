using PhotoForce.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;

namespace PhotoForce.WorkPlace
{
    public class YearbookSelectionViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        string className = "";
        static int jobOrShootId = 0;
        ArrayList arrStuImgId = new ArrayList();
        #endregion

        #region Properties
        private bool _isSelectedRecordsEnable;
        private bool _isAllRecordsChecked;
        private bool _isYesSelected;

        public bool isYesSelected
        {
            get { return _isYesSelected; }
            set { _isYesSelected = value; NotifyPropertyChanged("isYesSelected"); }
        }

        public bool isAllRecordsChecked
        {
            get { return _isAllRecordsChecked; }
            set { _isAllRecordsChecked = value; NotifyPropertyChanged("isAllRecordsChecked"); }
        }

        public bool isSelectedRecordsEnable
        {
            get { return _isSelectedRecordsEnable; }
            set { _isSelectedRecordsEnable = value; NotifyPropertyChanged("isSelectedRecordsEnable"); }
        }
        #endregion

        #region Constructors
        public YearbookSelectionViewModel(int photoJobId, ArrayList tempArrStuImgId, string tempClassName)
        {
            isYesSelected = true;
            jobOrShootId = photoJobId;
            arrStuImgId = tempArrStuImgId;
            className = tempClassName;
            if (arrStuImgId.Count == 0)
            {
                isSelectedRecordsEnable = false;
                isAllRecordsChecked = true;
            }
        }
        #endregion

        #region Commands
        public RelayCommand ApplyYearBookSelectionCommand
        {
            get
            {
                return new RelayCommand(applyYearBookSelection);
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
        private void applyYearBookSelection()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                int retval = 0;
                if (className == "PhotographyJob")
                {
                    try
                    {
                        if (isAllRecordsChecked == true)
                        {
                            if (isYesSelected == true)
                                // Need to update all records comes under this job
                                retval = clsDashBoard.UpdateYearbook(db,jobOrShootId, true);
                            else
                                retval = clsDashBoard.UpdateYearbook(db,jobOrShootId, false);
                            DialogResult = false;
                        }
                        else
                        {
                            if (isYesSelected == true)
                                retval = clsDashBoard.UpdateYearBookSelected(db, arrStuImgId, true);
                            else
                                retval = clsDashBoard.UpdateYearBookSelected(db, arrStuImgId, false);
                            DialogResult = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                }
                else
                {
                    try
                    {
                        if (isAllRecordsChecked == true)
                        {
                            if (isYesSelected == true)
                                // Need to update all records comes under this Shoot
                                retval = clsDashBoard.UpdateYearBookPhotoShoot(db, jobOrShootId, true);
                            else
                                retval = clsDashBoard.UpdateYearBookPhotoShoot(db, jobOrShootId, false);
                            DialogResult = false;

                        }
                        else
                        {
                            if (isYesSelected == true)
                                retval = clsDashBoard.UpdateYearBookSelected(db, arrStuImgId, true);
                            else
                                retval = clsDashBoard.UpdateYearBookSelected(db, arrStuImgId, false);
                            DialogResult = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                    }

                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion

    }
}
