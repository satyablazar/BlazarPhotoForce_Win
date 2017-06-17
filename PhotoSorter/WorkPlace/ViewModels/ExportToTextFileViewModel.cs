using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace PhotoForce.WorkPlace
{
    public class ExportToTextFileViewModel : ViewModelBase
    {
        //we are not using this class
        //#region Initialization
        //string imagePath = "", schoolName = "", jobName = "", currentDateTime = "", folderPath = "";
        //int PhotographyJobId, schoolId, photoShootId;
        //ArrayList arrStudents;
        //PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        //School _objSchool = new School();
        //PhotographyJob _objPhotographyJob = new PhotographyJob();
        //#endregion

        //#region Properties
        //string _fullPathContent;
        //string _folderName;
        //string _fileName;
        //bool _isExportAllChecked;
        //bool _isRemoveImageMissing;

        //public bool isRemoveImageMissing
        //{
        //    get { return _isRemoveImageMissing; }
        //    set { _isRemoveImageMissing = value; NotifyPropertyChanged("isRemoveImageMissing"); }
        //}
        //public bool isExportAllChecked
        //{
        //    get { return _isExportAllChecked; }
        //    set { _isExportAllChecked = value; NotifyPropertyChanged("isExportAllChecked"); }
        //}
        //public string fileName
        //{
        //    get { return _fileName; }
        //    set { _fileName = value; NotifyPropertyChanged("fileName"); }
        //}
        //public string folderName
        //{
        //    get { return _folderName; }
        //    set { _folderName = value; NotifyPropertyChanged("folderName"); }
        //}
        //public string fullPathContent
        //{
        //    get { return _fullPathContent; }
        //    set { _fullPathContent = value; NotifyPropertyChanged("fullPathContent"); }
        //}
        //#endregion

        #region Constructors
        public ExportToTextFileViewModel(int tempSchoolId, int tempJobId, int tempShootId, ArrayList tempArrStudents)
        {
            //fileName = "DataFile.txt"; isExportAllChecked = true;
            //schoolId = tempSchoolId;
            //PhotographyJobId = tempJobId;
            //photoShootId = tempShootId;
            //arrStudents = tempArrStudents;
            //bindData();
        }
        #endregion

        //#region Commands
        //public RelayCommand ExportCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(export);
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
        //private void bindData()
        //{
        //    _objSchool = clsDashBoard.getSchoolName(db, schoolId);
        //    schoolName = _objSchool.SchoolName;
        //    _objPhotographyJob = clsDashBoard.getJobName(db, PhotographyJobId);
        //    jobName = _objPhotographyJob.JobName;
        //    imagePath = clsDashBoard.getSettingByName(db, "ImageFolderLocation").settingValue.Trim();
        //    currentDateTime = DateTime.Now.ToString("yyyyMMdd-HHmm");
        //    fullPathContent = "Photo Sorter Directory Structure" + "\\" + schoolName + "\\" + jobName + "\\" + "__exportDataFiles" + "\\";
        //    folderName = currentDateTime;
        //}
        //private void export()
        //{
        //    folderPath = imagePath + "\\" + schoolName + "\\" + jobName + "\\" + "_exportDataFiles" + "\\" + folderName;
        //    if (folderName == "")
        //        folderName = DateTime.Now.ToString("yyyyMMdd-HHmm");

        //    if (!Directory.Exists(folderPath))
        //        Directory.CreateDirectory(folderPath);
        //    String tempFileName = "";
        //    if (fileName == "")
        //        tempFileName = "DataFile.txt";
        //    else if (!fileName.EndsWith(".txt"))
        //        tempFileName = fileName + ".txt";
        //    else
        //        tempFileName = fileName;
        //    List<StudentImage> objStudents;
        //    if (isExportAllChecked == true)
        //    {
        //        if (photoShootId == 0)      //means records the selected row was photographyJob
        //        {
        //            objStudents = (List<StudentImage>)clsDashBoard.getStudents(db, PhotographyJobId);
        //        }
        //        else
        //        {
        //            objStudents = (List<StudentImage>)clsDashBoard.getStudentsByPhotoShootId(db, photoShootId);
        //        }
        //    }
        //    else
        //    {
        //        objStudents = (List<StudentImage>)clsDashBoard.GetStudentsByImageIdArray(db, arrStudents);
        //    }

        //    String imageName = imagePath + "\\" + schoolName + "\\" + jobName + "\\" + "Students" + "\\" + "Hi Res Jpgs" + "\\";
        //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(folderPath + "\\" + tempFileName, true))
        //    {
        //        file.WriteLine("Image Name\tQuixiID\tFirst Name\tLast Name\tStudent ID\tTeacher\tGrade\tHome Room\tDOB\tAddress\tCity\tState\tZip\tPhone\tCustom1\tCustom2\tCustom3\tCustom4\tCustom5\tCustom6\tCustom7\tCustom8\tPackages\tTicket Code\tEmail Address");
        //        foreach (StudentImage objStd in objStudents)
        //        {
        //            if (isRemoveImageMissing == true)
        //            {
        //                if (!File.Exists(imageName + objStd.ImageName))
        //                    continue;
        //            }
        //            file.WriteLine(
        //                objStd.ImageName + "\t" + objStd.QuixieID + "\t" + objStd.FirstName + "\t" + objStd.Lastname + "\t" + objStd.StudentID + "\t" +
        //                objStd.Teacher + "\t" + objStd.Grade + "\t" + objStd.HomeRoom + "\t" + objStd.DOB + "\t" + objStd.Address + "\t" + objStd.City + "\t" + objStd.State + "\t" +
        //                objStd.Zip + "\t" + objStd.Phone + "\t" + objStd.Custom1 + "\t" + objStd.Custom2 + "\t" + objStd.Custom3 + "\t" + objStd.Custom4 + "\t" + objStd.Custom5 + "\t" +
        //                objStd.Custom6 + "\t" + objStd.Custom7 + "\t" + objStd.Custom8 + "\t" + objStd.Packages
        //                );
        //        }
        //    }
        //    Process.Start(folderPath);
        //}
        //private void windowClose()
        //{
        //    DialogResult = false;
        //}
        //#endregion
    }
}
