using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.Extensions;
using System.Collections;
using PhotoForce.Student_Management;
using PhotoForce.GroupManagement;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Configuration;

namespace PhotoForce.Mask_Management
{
    public class SearchSchoolViewModel : ViewModelBase
    {
        #region Initializations
        AddEditMasks _objAddEditMasks;
        ExportStudent _objExportStudent;
        ImportStudent _objImportStudent;
        AddGroup _objAddGroup;
        MainWindow _objmain;
        int maskid = 0;
        string pageName = "";
        ArrayList arrStudImageId = new ArrayList();
        public bool isSave = false;
        //bool tempActive;
        //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        #endregion

        #region Properties
        IEnumerable<School> _dgSearchSchoolsData;
        private string _windowTitle;
        School _selectedSchool;
        bool _isActive;

        public bool isActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value; NotifyPropertyChanged("isActive");
                bindGrid(isActive);
            }
        }
        public School selectedSchool
        {
            get { return _selectedSchool; }
            set { _selectedSchool = value; NotifyPropertyChanged("selectedSchool"); }
        }
        public string windowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; NotifyPropertyChanged("windowTitle"); }
        }
        public IEnumerable<School> dgSearchSchoolsData
        {
            get { return _dgSearchSchoolsData; }
            set { _dgSearchSchoolsData = value; NotifyPropertyChanged("dgSearchSchoolsData"); }
        }
        #endregion

        #region Constructors
        public SearchSchoolViewModel(string windowName, MainWindow mainWindowTitle)
            : this()
        {
            try
            {
                _objmain = mainWindowTitle;
                pageName = windowName;
                if (pageName == "Set Default School")
                    windowTitle = pageName;
                //bindGrid(isActive);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        public SearchSchoolViewModel(string windowName)
            : this()
        {
            try
            {
                pageName = windowName;
                if (pageName == "Set Default School")
                    windowTitle = pageName;
                //bindGrid(isActive);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        public SearchSchoolViewModel(int tempMaskId, string windowName)
            : this()
        {
            try
            {
                maskid = tempMaskId;
                pageName = windowName;
                //bindGrid(isActive);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        public SearchSchoolViewModel(ArrayList tempStuImageIdArray, string Windowname)
            : this()
        {
            arrStudImageId = tempStuImageIdArray;
            pageName = Windowname;
            //bindGrid(isActive);
        }
        public SearchSchoolViewModel()
        {
            isActive = true;
            //if (config.AppSettings.Settings["IsActive"] == null)
            //{
            //    config.AppSettings.Settings.Add("IsActive", isActive.ToString());
            //    config.Save(ConfigurationSaveMode.Modified);
            //    tempActive = true;
            //}
            //else
            //{
            //    tempActive = Convert.ToBoolean(config.AppSettings.Settings["IsActive"].Value);    
            //}
        }
        # endregion

        #region Commands
        public RelayCommand SelectOKCommand
        {
            get
            {
                return new RelayCommand(selectOK);
            }
        }
        public RelayCommand SearchSchoolsDoubleClickCommand
        {
            get
            {
                return new RelayCommand(searchSchoolDoubleClick);
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

        # region Bind grid
        private void bindGrid(bool isActive)
        {
            dgSearchSchoolsData = clsDashBoard.getSchools(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), isActive);
        }
        # endregion

        private void selectOK()
        {
            try
            {
                if (selectedSchool == null) { MVVMMessageService.ShowMessage("Plese select a school."); return; }
                if (selectedSchool.SchoolName != clsSchool.defaultSchoolName)
                {
                    int schoolId = selectedSchool.ID;
                    string schoolName = selectedSchool.SchoolName;

                    if (pageName == "AddEditMask")
                    {
                        if (maskid != 0)
                            _objAddEditMasks = new AddEditMasks(maskid, "");
                        else
                            _objAddEditMasks = new AddEditMasks(0, "");
                    }
                    else if (pageName == "ExportStudents")
                    {
                        _objExportStudent = new ExportStudent(schoolId, schoolName, arrStudImageId);
                    }
                    else if (pageName == "ImportStudents")
                    {
                        _objImportStudent = new ImportStudent(schoolId, 0, schoolName);
                    }
                    else if (pageName == "AddEditGroups")
                    {
                        _objAddGroup = new AddGroup(schoolId, schoolName);
                    }
                    else if (pageName == "Set Default School")
                    {
                        setDefaultSchool();
                    }
                    DialogResult = false;
                    isSave = true;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        public void setDefaultSchool()
        {
            //int retval = clsDashBoard.UpdateDefaultSchool(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), schoolID);      no need to do this ,as we are updating to registry  //mohan
            bool setPhotoSorterRegistry = clsSchool.updateDefaultSchoolRegistry(selectedSchool.SchoolName, selectedSchool.ID);    //updating registry values 
        }
        private void searchSchoolDoubleClick()
        {
            selectOK();
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
