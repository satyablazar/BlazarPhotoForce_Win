using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Collections;
using System.Data;
using PhotoForce.Extensions;

namespace PhotoForce.StudentImageManagement
{
    public class CountImagesViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        ArrayList arrJobID = null;
        string selectedSchoolYear;
        //internal int studentCount;
        List<CountImagesNStudents> filterData = new List<CountImagesNStudents>();
        #endregion

        #region Properties
        string _windowName;
        List<CountImagesNStudents> _dgCountImagesData;
        int? _studentCount;

        public int? countToCreateGroup
        {
            get { return _studentCount; }
            set { _studentCount = value; NotifyPropertyChanged("countToCreateGroup"); }
        }

        public List<CountImagesNStudents> dgCountImagesData
        {
            get { return _dgCountImagesData; }
            set { _dgCountImagesData = value; NotifyPropertyChanged("dgCountImagesData"); }
        }
        public string windowName
        {
            get { return _windowName; }
            set { _windowName = value; NotifyPropertyChanged("windowName"); }
        }
        #endregion

        #region Constructors
        public CountImagesViewModel(ArrayList tempJobId, string selectedGrid, string jobName)
        {
            arrJobID = tempJobId;
            windowName = selectedGrid;
            selectedSchoolYear = jobName;
            bindImages();
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
        public void bindImages()
        {
            try
            {
                if (windowName == "Count Images")
                    dgCountImagesData = clsDashBoard.CountImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrJobID);
                else
                {
                    dgCountImagesData = clsStudent.CountImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrJobID);
                    if (dgCountImagesData.Count() == 0)
                        MVVMMessageService.ShowMessage("There are no duplicate students for school year " + selectedSchoolYear + ".");
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void createGroups()
        {
            try
            {
                filterData = (from cid in dgCountImagesData where cid.Total > countToCreateGroup select cid).ToList();

                if (filterData.Count == 0) { MVVMMessageService.ShowMessage("Cannot create groups."); return; }
                
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                int grpId = 0;

                foreach (CountImagesNStudents student in filterData)
                {
                    Group _objGroup = new Group();
                    _objGroup.SchoolID = clsSchool.defaultSchoolId;
                    _objGroup.Notes = "Auto-created";
                    _objGroup.GroupName = "Validation" + ":" + student.Name;
                    _objGroup.createdOn = DateTime.Now;
                    _objGroup.hasClassPhoto = false;

                    // Check for already exist group name
                    grpId = clsGroup.getGroupId(db, "Validation" + ":" + student.Name.Replace("'", "''"), clsSchool.defaultSchoolId);
                    List<int> studentImageIds = new List<int>();

                    studentImageIds = clsDashBoard.getStudentImageIdsByStudentId(db, student.StudentId, arrJobID);

                    if (grpId != 0) { int res = clsGroup.deleteGroup(db, new ArrayList { grpId }); }

                    //get all studentImages to be shown in this group
                    foreach (int imgId in studentImageIds)
                    {
                        GroupItem gi = new GroupItem();
                        gi.StudentPhotoID = imgId;
                        _objGroup.GroupItems.Add(gi);
                    }
                    db.Groups.InsertOnSubmit(_objGroup);

                }

                db.SubmitChanges();

                if (filterData.Count > 1)
                    MVVMMessageService.ShowMessage(filterData.Count + " groups created" + errorMessages.AFTER_CREATING_STUDENT_GROUP_CONFIRMATION2 );
                else
                    MVVMMessageService.ShowMessage(filterData.Count + " group created" + errorMessages.AFTER_CREATING_STUDENT_GROUP_CONFIRMATION2 );
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
