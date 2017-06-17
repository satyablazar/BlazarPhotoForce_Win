using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using PhotoForce.Extensions;

namespace PhotoForce.PhotographyJobManagement
{
    public class AddNewPhotographyJobViewModel : ViewModelBase
    {

        #region Initialization
        internal int jobNameCount = 1;
        internal int jobId = 0;
        internal string tempJobName = "";
        int presentSchoolYear = DateTime.Now.Year;
        int defaultSchoolId = 0;
        string defaultSchoolName = "";
        int? checkForSchoolYear = 0;
        public bool isSave = false;
        public int tempStartYear = 0;
        public int tempEndYear = 0;
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public PhotographyJob addEditPhotographyJob;
        public PhotographyJob newCollectivePhotographyJob;
        #endregion

        #region Properties

        private string _jobName;
        private List<ComboBoxItem> _startYear;
        private List<ComboBoxItem> _endYear;
        private string _startYearSelectedValue;
        private string _endYearSelectedValue;

        public List<ComboBoxItem> endYear
        {
            get { return _endYear; }
            set { _endYear = value; NotifyPropertyChanged("endYear"); }
        }
        public List<ComboBoxItem> startYear
        {
            get { return _startYear; }
            set { _startYear = value; NotifyPropertyChanged("startYear"); }
        }
        public string endYearSelectedValue
        {
            get { return _endYearSelectedValue; }
            set
            {
                _endYearSelectedValue = value; NotifyPropertyChanged("endYearSelectedValue");
                endYearSelection();
            }
        }
        public string startYearSelectedValue
        {
            get { return _startYearSelectedValue; }
            set { _startYearSelectedValue = value; NotifyPropertyChanged("startYearSelectedValue"); startYearSelection(); }
        }
        public string jobName
        {
            get { return _jobName; }
            set { _jobName = value; NotifyPropertyChanged("jobName"); }
        }


        #endregion

        #region Constructors
        public AddNewPhotographyJobViewModel()
        {
            fillSchoolYear();
        }
        public AddNewPhotographyJobViewModel(string jobName, int startYear, int endYear, int photoJobId)
        {
            tempStartYear = startYear;
            tempEndYear = endYear;
            jobNameCount = 0;
            jobId = photoJobId;
            tempJobName = jobName;
            fillSchoolYear();
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
        //Commented by hema --> Code changed as per neal request -->
        //private void saveAndClose()
        //{
        //    try
        //    {
        //        db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        //        if (string.IsNullOrEmpty(jobName)) { return; }
        //        if ((tempEndYear == 0 && tempStartYear > 0) || (tempEndYear > 0 && tempStartYear == 0)) { return; }
        //        if ((tempEndYear - tempStartYear) >= 2 || (tempEndYear < tempStartYear))
        //        {
        //            MVVMMessageService.ShowMessage(errorMessages.SKIP_SCHOOL_YEAR);
        //            return;
        //        }

        //        checkForSchoolYear = clsPhotoShoot.isSchoolYearExists(db, jobName.Trim(), defaultSchoolId);   //check for school year(i.e, job name),if already exists return photography job id.
        //        if (jobId == 0)
        //        {
        //            if (checkForSchoolYear == 0 || checkForSchoolYear == null)
        //            {
        //                if (tempEndYear == 0 && tempStartYear == 0) { MVVMMessageService.ShowMessage(errorMessages.SELECT_SCHOOL_YEAR); return; }
        //                if (defaultSchoolId != 0)
        //                {
        //                    addEditPhotographyJob = new PhotographyJob();
        //                    addEditPhotographyJob.JobName = jobName;
        //                    addEditPhotographyJob.JobDate = DateTime.Now;
        //                    addEditPhotographyJob.StartYear = tempStartYear.ToString();
        //                    addEditPhotographyJob.EndYear = tempEndYear.ToString();
        //                    addEditPhotographyJob.SchoolID = defaultSchoolId;
        //                    if (addEditPhotographyJob != null)
        //                    {
        //                        db.PhotographyJobs.InsertOnSubmit(addEditPhotographyJob);
        //                        db.SubmitChanges();
        //                        isSave = true;
        //                        DialogResult = false;
        //                    }
        //                }
        //                else
        //                {
        //                    MVVMMessageService.ShowMessage(errorMessages.SELECT_DEFAULT_SCHOOL);
        //                    isSave = false;
        //                    DialogResult = false;
        //                    return;
        //                }
        //            }
        //            else
        //                MVVMMessageService.ShowMessage("Unable to create a duplicate record - there is already a school year " + jobName + " for " + defaultSchoolName + ".");
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(startYearSelectedValue.Trim()))
        //            {
        //                if (checkForSchoolYear == 0 || checkForSchoolYear == jobId)        //check for school year(i.e, job name)
        //                {
        //                    addEditPhotographyJob = clsPhotoShoot.getPhotographyJob(db, jobId);
        //                    addEditPhotographyJob.StartYear = tempStartYear.ToString();
        //                    addEditPhotographyJob.EndYear = tempEndYear.ToString();
        //                    addEditPhotographyJob.JobName = jobName;
        //                    if (addEditPhotographyJob != null)
        //                    {
        //                        db.SubmitChanges();
        //                    }

        //                    //clsPhotoShoot.updateSchoolYear(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), jobId, tempStartYear.ToString(), tempEndYear.ToString(), jobName);
        //                    jobId = 0;
        //                    isSave = true;
        //                    DialogResult = false;
        //                }
        //                else
        //                {
        //                    MVVMMessageService.ShowMessage("Unable to create a duplicate record - there is already a school year " + jobName + " for " + defaultSchoolName + ".");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        clsStatic.WriteExceptionLogXML(ex);
        //        MVVMMessageService.ShowMessage(ex.Message);
        //    }
        //}
        private void saveAndClose()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (string.IsNullOrEmpty(jobName)) { return; }
                if ((tempEndYear == 0 && tempStartYear > 0) || (tempEndYear > 0 && tempStartYear == 0)) { return; }
                if ((tempEndYear - tempStartYear) >= 2 || (tempEndYear < tempStartYear))
                {
                    MVVMMessageService.ShowMessage(errorMessages.SKIP_SCHOOL_YEAR);
                    return;
                }

                checkForSchoolYear = clsPhotoShoot.isSchoolYearExists(db, jobName.Trim(), defaultSchoolId);   //check for school year(i.e, job name),if already exists return photography job id.

                if (jobId == 0)
                {
                    List<int> tempSchoolIds = clsPhotoShoot.isJobNameNotExists(db, jobName.Trim());   //check for school year(i.e, job name),if not exists then select that SchoolId.
                    List<int> allSchoolIds = clsSchool.getSelectedSchools(db, tempSchoolIds);
                    if (checkForSchoolYear == 0 || checkForSchoolYear == null)
                    {
                        foreach (int schoolId in allSchoolIds)
                        {

                            if (tempEndYear == 0 && tempStartYear == 0) { MVVMMessageService.ShowMessage(errorMessages.SELECT_SCHOOL_YEAR); return; }
                            if (schoolId != 0) //( defaultSchoolId != 0)
                            {
                                addEditPhotographyJob = new PhotographyJob();
                                addEditPhotographyJob.JobName = jobName;
                                addEditPhotographyJob.JobDate = DateTime.Now;
                                addEditPhotographyJob.StartYear = tempStartYear.ToString();
                                addEditPhotographyJob.EndYear = tempEndYear.ToString();
                                addEditPhotographyJob.SchoolID = schoolId; //defaultSchoolId;
                                if (addEditPhotographyJob != null)
                                {
                                    db.PhotographyJobs.InsertOnSubmit(addEditPhotographyJob);
                                    db.SubmitChanges();
                                    //isSave = true;
                                    //DialogResult = false;
                                }
                                if (defaultSchoolId == schoolId)
                                {
                                    newCollectivePhotographyJob = addEditPhotographyJob;
                                }
                            }
                            else
                            {
                                MVVMMessageService.ShowMessage(errorMessages.SELECT_DEFAULT_SCHOOL);
                                isSave = false;
                                DialogResult = false;
                                return;
                            }
                        }
                        isSave = true;
                        DialogResult = false;
                    }
                    else
                        MVVMMessageService.ShowMessage("Unable to create a duplicate record - there is already a school year " + jobName + " for " + defaultSchoolName + ".");
                }
                else
                {
                    if (!string.IsNullOrEmpty(startYearSelectedValue.Trim()))
                    {
                        if (checkForSchoolYear == 0 || checkForSchoolYear == jobId)        //check for school year(i.e, job name)
                        {
                            addEditPhotographyJob = clsPhotoShoot.getPhotographyJob(db, jobId);
                            addEditPhotographyJob.StartYear = tempStartYear.ToString();
                            addEditPhotographyJob.EndYear = tempEndYear.ToString();
                            addEditPhotographyJob.JobName = jobName;
                            if (addEditPhotographyJob != null)
                            {
                                db.SubmitChanges();
                            }

                            //clsPhotoShoot.updateSchoolYear(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), jobId, tempStartYear.ToString(), tempEndYear.ToString(), jobName);
                            jobId = 0;
                            isSave = true;
                            DialogResult = false;
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage("Unable to create a duplicate record - there is already a school year " + jobName + " for " + defaultSchoolName + ".");
                        }
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
            DialogResult = false; isSave = false;
        }
        private void startYearSelection()
        {
            jobName = "";
            if (startYearSelectedValue != null)
            {
                if (!string.IsNullOrWhiteSpace(startYearSelectedValue))
                {
                    tempStartYear = Convert.ToInt32(startYearSelectedValue);
                    if (jobNameCount != 0)
                    {
                        tempEndYear = tempStartYear + 1;
                        endYearSelectedValue = Convert.ToString(tempEndYear);
                    }
                }
                else
                {
                    tempStartYear = 0;
                    jobName = "";
                    return;
                }
            }
        }
        private void endYearSelection()
        {
            if (endYearSelectedValue != null)
            {
                if (!string.IsNullOrWhiteSpace(endYearSelectedValue))
                    tempEndYear = Convert.ToInt32(endYearSelectedValue);
                else
                {
                    tempEndYear = 0;
                    jobName = "";
                    return;
                }
                setJobName();
            }
        }
        private void setJobName()
        {
            if (jobNameCount == 0)
            {
                jobName = tempJobName;
                jobNameCount++;
            }
            else if (tempStartYear != 0 && tempEndYear != 0)
                jobName = tempStartYear + "-" + tempEndYear.ToString();
        }

        #region FillComboBox
        internal void fillSchoolYear()
        {
            bindSchoolYear();
            GetDefaultSchool();
            if (jobId != 0)
            {
                if (tempStartYear == 0 && tempEndYear == 0) { jobName = tempJobName; jobNameCount++; return; }
                startYearSelectedValue = tempStartYear.ToString();
                endYearSelectedValue = tempEndYear.ToString();
                return;
            }
            int presentSchoolYearCount = clsPhotoShoot.checkForSchoolYear(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), presentSchoolYear, defaultSchoolId);
            if (presentSchoolYearCount == 0)
            {
                startYearSelectedValue = presentSchoolYear.ToString();
                endYearSelectedValue = Convert.ToString(presentSchoolYear + 1);
            }

        }

        private void bindSchoolYear()
        {
            startYear = new List<ComboBoxItem>();
            endYear = new List<ComboBoxItem>();
            startYear.Add(new ComboBoxItem { Name = " " });
            endYear.Add(new ComboBoxItem { Name = " " });
            for (int AddSchoolYear = presentSchoolYear - 1; AddSchoolYear <= 2050; AddSchoolYear++)
            {
                startYear.Add(new ComboBoxItem { Name = AddSchoolYear.ToString() });
                endYear.Add(new ComboBoxItem { Name = AddSchoolYear.ToString() });
            }
        }

        internal void GetDefaultSchool()
        {
            defaultSchoolId = clsSchool.defaultSchoolId;
            defaultSchoolName = clsSchool.defaultSchoolName;
        }

        #endregion

        #endregion
    }
}
