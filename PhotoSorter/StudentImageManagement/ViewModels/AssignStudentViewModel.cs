using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Collections;
using PhotoForce.Student_Management;
using System.Data;

namespace PhotoForce.StudentImageManagement
{
    public class AssignStudentViewModel : ViewModelBase
    {
        #region Initializations
        PhotoSorterDBModelDataContext db;
        ArrayList arrStudentImageId = new ArrayList();
        public bool isSave = false;
        #endregion

        #region Properties
        DataTable _dgAssignStudentData;
        DataRowView _selectedStudent;

        public DataRowView selectedStudent
        {
            get { return _selectedStudent; }
            set { _selectedStudent = value; NotifyPropertyChanged("selectedStudent"); }
        }

        public DataTable dgAssignStudentData
        {
            get { return _dgAssignStudentData; }
            set { _dgAssignStudentData = value; NotifyPropertyChanged("dgAssignStudentData"); }
        }
        #endregion

        #region Constructors
        public AssignStudentViewModel(ArrayList tempArrImageId)
        {
            arrStudentImageId = tempArrImageId;
            bindGrid();
        }
        #endregion

        #region Commands
        public RelayCommand AddStudentCommand
        {
            get
            {
                return new RelayCommand(addStudent);
            }
        }
        public RelayCommand EditStudentCommand
        {
            get
            {
                return new RelayCommand(editStudent);
            }
        }
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(assignImagesToStudent);
            }
        }
        public RelayCommand dgAssignStudentDoubleClickCommand
        {
            get
            {
                return new RelayCommand(dgAssignStudentDoubleClick);
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
        private void addStudent()
        {
            AddStudent _objAddStudent = new AddStudent();
            _objAddStudent.ShowDialog();

            if (((AddStudentViewModel)(_objAddStudent.DataContext)).isSave)
            {
                bindGrid();
            }
        }
        private void editStudent()
        {
            if (selectedStudent == null) { return; };

            int id = Convert.ToInt32(selectedStudent.Row["ID"]);
            int importId = Convert.ToInt32(selectedStudent.Row["StudentImportID"]);
            int SchoolId = clsSchool.defaultSchoolId;
            AddEditStudent _objAddEditStudent = new AddEditStudent(id, importId, SchoolId);
            _objAddEditStudent.ShowDialog();

            if (((AddEditStudentViewModel)(_objAddEditStudent.DataContext)).isSave)
            {
                bindGrid();
            }
        }
        private void assignStudentId()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            int result = clsDashBoard.updateAssignStudent(db, Convert.ToInt32(selectedStudent.Row["ID"]), arrStudentImageId);
        }
        private void bindGrid()
        {
            dgAssignStudentData = new DataTable();
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            dgAssignStudentData = clsDashBoard.getStudentsDetails(db, clsSchool.defaultSchoolId);
        }
        private void assignImagesToStudent()
        {
            if (selectedStudent != null)
            {
                assignStudentId();
                isSave = true;
                DialogResult = false;
            }
            else
            {
                MVVMMessageService.ShowMessage("Please select a student.");
            }
        }
        private void dgAssignStudentDoubleClick()
        {
            if (selectedStudent != null)
            {
                assignStudentId();
            }
        }
        private void windowClose()
        {
            DialogResult = false;
            isSave = false;
        }
        #endregion
    }
}
