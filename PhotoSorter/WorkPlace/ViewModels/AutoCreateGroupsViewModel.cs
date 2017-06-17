using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Collections;

namespace PhotoForce.WorkPlace
{
    public class AutoCreateGroupsViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        ArrayList arrShootId = new ArrayList();
        int jobId = 0; int schoolId = 0;
        string jobName = "";
        List<StudentImage> selectedImages;
        #endregion

        #region Properties
        bool _isForAllShoots;
        bool _isSelectedImagesEnable;
        bool _isSelectedImagesChecked;

        public bool isSelectedImagesChecked
        {
            get { return _isSelectedImagesChecked; }
            set { _isSelectedImagesChecked = value; NotifyPropertyChanged("isSelectedImagesChecked"); }
        }
        public bool isSelectedImagesEnable
        {
            get { return _isSelectedImagesEnable; }
            set { _isSelectedImagesEnable = value; NotifyPropertyChanged("isSelectedImagesEnable"); }
        }
        public bool isForAllShoots
        {
            get { return _isForAllShoots; }
            set { _isForAllShoots = value; NotifyPropertyChanged("isForAllShoots"); }
        }
        #endregion

        #region Constructors
        public AutoCreateGroupsViewModel(ArrayList tempArrPhotoShootId, int tempJobId, string tempJobName, int tempSchoolId, List<StudentImage> tempSelectedImages)
        {
            selectedImages = new List<StudentImage>();

            selectedImages = tempSelectedImages;

            arrShootId = tempArrPhotoShootId;
            jobId = tempJobId;
            schoolId = tempSchoolId;
            jobName = tempJobName;

            if (selectedImages.Count > 0) { isSelectedImagesEnable = true; isSelectedImagesChecked = true; }
            else { isForAllShoots = true; }
        }
        #endregion

        #region Commands
        public RelayCommand CreateGroupsCommand
        {
            get
            {
                return new RelayCommand(createGroups);
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
        private void createGroups()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                int grpId = 0;

                int isStudentImagesWithoutTeacher = 0;
                ArrayList arrPhotoShootId = new ArrayList();
                if (isForAllShoots == true)
                {
                    IEnumerable<PhotoShoot> photoShootDetails = clsDashBoard.getPhotoShootDetailsbyJobid(db, jobId);
                    foreach (var tempPhotoShoot in photoShootDetails)
                    {
                        if (!arrPhotoShootId.Contains(tempPhotoShoot.PhotoShotID))
                            arrPhotoShootId.Add(tempPhotoShoot.PhotoShotID);
                    }
                }
                else
                {
                    foreach (var photoShootId in arrShootId)
                    {
                        arrPhotoShootId.Add(photoShootId);
                    }
                }
                isStudentImagesWithoutTeacher = clsDashBoard.getStudentImagesWithoutTeacherCountJobID(db, arrPhotoShootId);
                if (isSelectedImagesChecked && selectedImages.Count > 0)
                {
                    ArrayList selectedImageIds = new ArrayList();
                    foreach (StudentImage si in selectedImages)
                    {
                        if (!selectedImageIds.Contains(si.ID))
                            selectedImageIds.Add(si.ID);
                    }
                    int count = clsDashBoard.getStudentImagesWithoutTeacherCountSelectedImages(db, arrPhotoShootId, selectedImageIds);
                    if (MVVMMessageService.ShowMessage(count + errorMessages.AUTO_CREATE_GROUP_WITHOUT_TEACHER, "Photo Saver", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.No)
                        return;
                }
                else if (isStudentImagesWithoutTeacher > 0)
                {
                    if (MVVMMessageService.ShowMessage(isStudentImagesWithoutTeacher + errorMessages.AUTO_CREATE_GROUP_WITHOUT_TEACHER, "Photo Saver", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.No)
                        return;
                }

                ////auto generate groups
                ////get all the distinct values of the teachers
                List<string> lstTeachers = new List<string>();
                if (isSelectedImagesChecked)
                {
                    foreach (StudentImage img in selectedImages)
                    {
                        string teacherName = img.Student.Teacher;
                        if (string.IsNullOrEmpty(teacherName)) { teacherName = "No Teacher"; }
                        if (!lstTeachers.Contains(teacherName))
                        {
                            lstTeachers.Add(teacherName);
                        }
                    }
                }
                else
                    lstTeachers = clsDashBoard.getTeachersByPhotoJob(db, arrPhotoShootId);

                foreach (string teacher in lstTeachers)
                {
                    Group _objGroup = new Group();
                    _objGroup.SchoolID = schoolId;
                    _objGroup.Notes = "Auto-created";
                    _objGroup.GroupName = teacher + "-" + jobName;
                    _objGroup.createdOn = DateTime.Now;
                    _objGroup.hasClassPhoto = false;

                    // Check for already exist group name
                    grpId = clsGroup.getGroupId(db, teacher.Replace("'", "''") + "-" + jobName, schoolId);
                    List<int> studentImageIds = new List<int>();
                    if (isSelectedImagesChecked)
                    {
                        if (teacher == "No Teacher")
                            studentImageIds = (from si in selectedImages where (si.Student.Teacher == null || si.Student.Teacher == "") select si.ID).ToList();
                        else
                            studentImageIds = (from si in selectedImages where (si.Student.Teacher == teacher.Replace("'", "''")) select si.ID).ToList();
                    }
                    else
                        studentImageIds = clsDashBoard.getStudentImageIdsByTeacher(db, arrPhotoShootId, teacher.Replace("'", "''"));
                    if (grpId == 0)
                    {
                        //get all studentImages to be shown in this group
                        foreach (int imgId in studentImageIds)
                        {
                            GroupItem gi = new GroupItem();
                            gi.StudentPhotoID = imgId;
                            StudentImage tempRating = (from si in db.StudentImages where si.ID == imgId select si).FirstOrDefault();
                            if (tempRating.Rating != "1")
                                _objGroup.GroupItems.Add(gi);
                        }
                        db.Groups.InsertOnSubmit(_objGroup);
                    }
                    else
                    {
                        foreach (int photoShootId in studentImageIds)
                        {
                            int result = clsGroup.checkGroupItem(db, grpId, photoShootId);
                            GroupItem gi = new GroupItem();
                            if (result == 0)
                            {
                                gi.GroupID = grpId;
                                gi.StudentPhotoID = photoShootId;
                                StudentImage tempRating = (from si in db.StudentImages where si.ID == photoShootId select si).FirstOrDefault();
                                if (tempRating.Rating != "1")
                                    db.GroupItems.InsertOnSubmit(gi);
                            }
                        }
                    }
                }

                db.SubmitChanges();

                if (isForAllShoots == true)
                    MVVMMessageService.ShowMessage(lstTeachers.Count + errorMessages.AFTER_CREATING_GROUP_JOB_CONFIRMATION1 + jobName + errorMessages.AFTER_CREATING_GROUP_JOB_CONFIRMATION2 + errorMessages.AFTER_CREATING_GROUP_JOB_CONFIRMATION3 + jobName);
                else
                {
                    if (arrShootId.Count > 1)
                        MVVMMessageService.ShowMessage(lstTeachers.Count + errorMessages.AFTER_CREATING_GROUP_CONFIRMATION1 + errorMessages.AFTER_CREATING_GROUP_CONFIRMATION2 + errorMessages.AFTER_CREATING_GROUP_CONFIRMATION3 + jobName);
                    else
                        MVVMMessageService.ShowMessage(lstTeachers.Count + errorMessages.AFTER_CREATING_GROUP_ONE_CONFIRMATION1 + errorMessages.AFTER_CREATING_GROUP_CONFIRMATION2 + errorMessages.AFTER_CREATING_GROUP_CONFIRMATION3 + jobName);
                }
                DialogResult = false;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
