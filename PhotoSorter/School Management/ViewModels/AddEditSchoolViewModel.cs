using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using PhotoForce.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PhotoForce.School_Management
{
    public class AddEditSchoolViewModel : ViewModelBase, IDataErrorInfo
    {

        #region Initialization
        int schoolId = 0;
        public bool isSave = false;
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public School addEditSchool;
        #endregion

        #region IDataErrorInfo Members
        #region Error Property
        /// <summary>
        /// 
        /// </summary>
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region this Property
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                string message = string.Empty;
                validateUserInput(ref message, columnName);
                return message;
            }
        }
        #endregion

        #endregion

        #region Properties
        School _schoolData;

        private string _schoolName;
        private string _address1;
        private string _address2;
        private string _address3;
        private string _city;
        private string _state;
        private string _zip;
        private string _notes;
        private string _folderPath;
        private bool _yearBookYes;
        private bool _yearBookNo;
        private bool _idRequiredYes;
        private bool _idRequiredNo;
        private bool? _isActive;
        private int? _students;
        private string _schoolYear;
        //private string _selectedRating;
        private List<Studio> _cbStudioData;
        private Studio _selectedStudio;
        private int? _calendars;
        private int? _pencils;
        private int? _idCards;
        private DateTime? _dtContractExpiration;
        private string _visitAddress;
        private string _affiliation;

        public string affiliation
        {
            get { return _affiliation; }
            set { _affiliation = value; NotifyPropertyChanged(); }
        }

        public string visitAddress
        {
            get { return _visitAddress; }
            set { _visitAddress = value; NotifyPropertyChanged(); }
        }
        public DateTime? dtContractExpiration
        {
            get { return _dtContractExpiration; }
            set { _dtContractExpiration = value; NotifyPropertyChanged(); }
        }
        public int? idCards
        {
            get { return _idCards; }
            set { _idCards = value; NotifyPropertyChanged(); }
        }
        public int? pencils
        {
            get { return _pencils; }
            set { _pencils = value; NotifyPropertyChanged(); }
        }
        public int? calendars
        {
            get { return _calendars; }
            set { _calendars = value; NotifyPropertyChanged(); }
        }
        public Studio selectedStudio
        {
            get { return _selectedStudio; }
            set { _selectedStudio = value; NotifyPropertyChanged(); }
        }
        public List<Studio> cbStudioData
        {
            get { return _cbStudioData; }
            set { _cbStudioData = value; NotifyPropertyChanged(); }
        }
        public bool? isActive
        {
            get { return _isActive; }
            set { _isActive = value; NotifyPropertyChanged("isActive"); }
        }        
        //public string selectedRating
        //{
        //    get { return _selectedRating; }
        //    set { _selectedRating = value; NotifyPropertyChanged("selectedRating"); }
        //}
        public string schoolYear
        {
            get { return _schoolYear; }
            set { _schoolYear = value; NotifyPropertyChanged("schoolYear"); }
        }
        public int? students
        {
            get { return _students; }
            set { _students = value; NotifyPropertyChanged("students"); }
        }
        public bool idRequiredNo
        {
            get { return _idRequiredNo; }
            set { _idRequiredNo = value; NotifyPropertyChanged("idRequiredNo"); }
        }
        public bool idRequiredYes
        {
            get { return _idRequiredYes; }
            set { _idRequiredYes = value; NotifyPropertyChanged("idRequiredYes"); }
        }
        public bool yearBookNo
        {
            get { return _yearBookNo; }
            set { _yearBookNo = value; NotifyPropertyChanged("yearBookNo"); }
        }
        public bool yearBookYes
        {
            get { return _yearBookYes; }
            set { _yearBookYes = value; NotifyPropertyChanged("yearBookYes"); }
        }
        public string folderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; NotifyPropertyChanged("folderPath"); }
        }
        public string notes
        {
            get { return _notes; }
            set { _notes = value; NotifyPropertyChanged("notes"); }
        }
        public string zip
        {
            get { return _zip; }
            set { _zip = value; NotifyPropertyChanged("zip"); }
        }
        public string state
        {
            get { return _state; }
            set { _state = value; NotifyPropertyChanged("state"); }
        }
        public string city
        {
            get { return _city; }
            set { _city = value; NotifyPropertyChanged("city"); }
        }
        public string address3
        {
            get { return _address3; }
            set { _address3 = value; NotifyPropertyChanged("address3"); }
        }
        public string address2
        {
            get { return _address2; }
            set { _address2 = value; NotifyPropertyChanged("address2"); }
        }
        public string address1
        {
            get { return _address1; }
            set { _address1 = value; NotifyPropertyChanged("address1"); }
        }
        public string schoolName
        {
            get { return _schoolName; }
            set { _schoolName = value; NotifyPropertyChanged("schoolName"); }
        }
        public School schoolData
        {
            get { return _schoolData; }
            set { _schoolData = value; NotifyPropertyChanged("schoolData"); }
        }
        #endregion

        #region Contact Properties
        string _contact1Name;
        string _contact2Name;
        string _contact3Name;
        string _contact1Email;
        string _contact2Email;
        string _contact3Email;
        ObservableCollection<String> _contact1TypeData;
        ObservableCollection<String> _contact2TypeData;
        ObservableCollection<String> _contact3TypeData;
        string _selectedContact1Type;
        string _selectedContact2Type;
        string _selectedContact3Type;
        string _contact1Notes;
        string _contact2Notes;
        string _contact3Notes;

        public string contact3Notes
        {
            get { return _contact3Notes; }
            set { _contact3Notes = value; NotifyPropertyChanged(); }
        }
        public string contact2Notes
        {
            get { return _contact2Notes; }
            set { _contact2Notes = value; NotifyPropertyChanged(); }
        }
        public string contact1Notes
        {
            get { return _contact1Notes; }
            set { _contact1Notes = value; NotifyPropertyChanged(); }
        }
        public string selectedContact3Type
        {
            get { return _selectedContact3Type; }
            set { _selectedContact3Type = value; NotifyPropertyChanged(); }
        }
        public string selectedContact2Type
        {
            get { return _selectedContact2Type; }
            set { _selectedContact2Type = value; NotifyPropertyChanged(); }
        }
        public string selectedContact1Type
        {
            get { return _selectedContact1Type; }
            set { _selectedContact1Type = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<String> contact3TypeData
        {
            get { return _contact3TypeData; }
            set { _contact3TypeData = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<String> contact2TypeData
        {
            get { return _contact2TypeData; }
            set { _contact2TypeData = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<String> contact1TypeData
        {
            get { return _contact1TypeData; }
            set { _contact1TypeData = value; NotifyPropertyChanged(); }
        }
        public string contact3Email
        {
            get { return _contact3Email; }
            set { _contact3Email = value; NotifyPropertyChanged(); }
        }
        public string contact2Email
        {
            get { return _contact2Email; }
            set { _contact2Email = value; NotifyPropertyChanged(); }
        }
        public string contact1Email
        {
            get { return _contact1Email; }
            set { _contact1Email = value; NotifyPropertyChanged(); }
        }
        public string contact3Name
        {
            get { return _contact3Name; }
            set { _contact3Name = value; NotifyPropertyChanged(); }
        }
        public string contact2Name
        {
            get { return _contact2Name; }
            set { _contact2Name = value; NotifyPropertyChanged(); }
        }
        public string contact1Name
        {
            get { return _contact1Name; }
            set { _contact1Name = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructors
        public AddEditSchoolViewModel(int tempSchoolId)
        {
            try
            {
                //Rating data binding
                cbStudioData = new List<Studio>();
                //ratingData.Add(new ComboBoxItem { Name = " " });
                //ratingData.Add(new ComboBoxItem { Name = "Clear" });
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                cbStudioData = (from stu in db.Studios select stu).ToList();
                //for (int i = 0; i <= 9; i++)
                //{
                //    ratingData.Add(new ComboBoxItem { Name = i.ToString() });
                //}
                schoolId = tempSchoolId;

                List<String> tempContactType = new List<string>(); contact1TypeData = new ObservableCollection<string>();
                contact2TypeData = new ObservableCollection<string>(); contact3TypeData = new ObservableCollection<string>();
                tempContactType.Add("Principal"); tempContactType.Add("Admin"); tempContactType.Add("Other"); tempContactType.Add("YB");
                //contact1TypeData.Add("Principal"); contact1TypeData.Add("Admin"); contact1TypeData.Add("Other"); contact1TypeData.Add("YB");
                //contact2TypeData.Add("Principal"); contact2TypeData.Add("Admin"); contact2TypeData.Add("Other"); contact2TypeData.Add("YB");
                //contact3TypeData.Add("Principal"); contact3TypeData.Add("Admin"); contact3TypeData.Add("Other"); contact3TypeData.Add("YB");

                contact1TypeData = new ObservableCollection<string>(tempContactType);
                contact2TypeData = new ObservableCollection<string>(tempContactType);
                contact3TypeData = new ObservableCollection<string>(tempContactType);
                if (schoolId != 0)
                {
                    schoolData = clsDashBoard.getSchoolDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), schoolId);
                    if (schoolData != null)
                    {
                        schoolName = schoolData.SchoolName.ToString();
                        address1 = Convert.ToString(schoolData.Address1);
                        address2 = Convert.ToString(schoolData.Address2);
                        address3 = Convert.ToString(schoolData.Address3);
                        city = Convert.ToString(schoolData.City);
                        state = Convert.ToString(schoolData.State);
                        zip = Convert.ToString(schoolData.Zip);
                        notes = Convert.ToString(schoolData.Notes);
                        folderPath = schoolData.folderPath;
                        ratingData = Convert.ToInt32(schoolData.Rating);
                        students = schoolData.Students;
                        schoolYear = schoolData.SchoolYear;
                        isActive = schoolData.IsActive;
                        if (schoolData.StudioId != null)
                            selectedStudio = cbStudioData.Where(x => x.Id == (int)schoolData.StudioId).FirstOrDefault();
                        else
                            selectedStudio = null;
                        idCards = schoolData.IDCards;
                        calendars = schoolData.Calendars;
                        pencils = schoolData.Pencils;
                        dtContractExpiration = schoolData.ContractExpiration; 
                        if (Convert.ToBoolean(schoolData.YearBookRequired))
                            yearBookYes = true;
                        else
                            yearBookNo = true;

                        if (Convert.ToBoolean(schoolData.IDRequired))
                            idRequiredYes = true;
                        else
                            idRequiredNo = true;

                        contact1Name = schoolData.Contact1Name;
                        selectedContact1Type = schoolData.Contact1Type;
                        contact1Email = schoolData.Contact1Email;
                        contact1Notes = schoolData.Contact1Notes;
                        
                        contact2Name = schoolData.Contact2Name;
                        selectedContact2Type = schoolData.Contact2Type;
                        contact2Email = schoolData.Contact2Email;
                        contact2Notes = schoolData.Contact2Notes;

                        contact3Name = schoolData.Contact3Name;
                        selectedContact3Type = schoolData.Contact3Type;
                        contact3Email = schoolData.Contact3Email;
                        contact3Notes = schoolData.Contact3Notes;

                        visitAddress = schoolData.Visit;
                        affiliation = schoolData.Affiliation;
                    }
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        #endregion

        #region Commands
        public RelayCommand BrowseSchoolPathCommand
        {
            get
            {
                return new RelayCommand(browseSchoolPath);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(createUpdateSchool);
            }
        }
        #endregion

        #region Methods
        private void browseSchoolPath()
        {
            try
            {
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                if (folderPath != "")
                {
                    if (File.Exists(folderPath))
                        dlg.SelectedPath = folderPath;
                }
                else
                    dlg.SelectedPath = clsDashBoard.getSettingByName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), "ImageFolderLocation").settingValue.Trim();
                var res = dlg.ShowDialog();
                if (res != false)
                    folderPath = dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// this method is used to close the window
        /// </summary>
        private void windowClose()
        {
            DialogResult = false;
        }
        /// <summary>
        /// this method is used to create or update School
        /// </summary>
        private void createUpdateSchool()
        {
            try
            {
                if (errorCount == 0 && !string.IsNullOrEmpty(folderPath))
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    int schlCount = 0;

                    if (schoolId == 0)
                        schlCount = clsDashBoard.getSchoolByPath(db, folderPath, 0);
                    else
                        schlCount = clsDashBoard.getSchoolByPath(db, folderPath, schoolId);

                    string ImageFolderPath = folderPath;
                    addEditSchool = new School();
                    if (schoolId != 0)
                    {
                        //Check for existance
                        //int cnt = Convert.ToInt32(_objDataTable.Rows[0]["countimg"]);
                        ////if (schlCount != 0)
                        ////{
                        ////    if (System.Windows.MessageBox.Show(errorMessages.SCHOOL_NAME_ALREADY_EXISTS_FOLDER, "Confirmation", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        ////    {
                        ////        return;
                        ////    }
                        ////}
                        ////else
                        ////{
                        ////    //create the folder
                        ////    try
                        ////    {
                        ////        Directory.CreateDirectory(ImageFolderPath);
                        ////    }
                        ////    catch (Exception ex)
                        ////    {
                        ////        MVVMMessageService.ShowMessage(ex.Message);
                        ////        clsStatic.WriteExceptionLogXML(ex);
                        ////    }
                        //}

                        if (string.IsNullOrEmpty(schoolName)) { MVVMMessageService.ShowMessage("Please fill school name and Folder path."); return; }

                        addEditSchool = clsDashBoard.updateSchool(db, schoolId);
                        addEditSchool.SchoolName = schoolName.Trim();
                        addEditSchool.Address1 = address1;
                        addEditSchool.Address2 = address2;
                        addEditSchool.Address3 = address3;
                        addEditSchool.City = city;
                        addEditSchool.State = state;
                        addEditSchool.Zip = zip;
                        addEditSchool.Notes = notes;
                        addEditSchool.folderPath = folderPath;
                        addEditSchool.YearBookRequired = yearBookYes == true ? true : false;
                        addEditSchool.IDRequired = idRequiredYes == true ? true : false;
                        //newly added columns
                        addEditSchool.Students = students;
                        addEditSchool.Rating = ratingData.ToString();
                        addEditSchool.SchoolYear = schoolYear;
                        addEditSchool.IsActive = isActive;
                        
                        addEditSchool.StudioId = selectedStudio != null ? (int?)(selectedStudio.Id) : null;
                        addEditSchool.IDCards = idCards > 0 ? idCards : null;
                        addEditSchool.Calendars = calendars > 0 ? calendars : null;
                        addEditSchool.Pencils = pencils > 0 ? pencils : null;
                        addEditSchool.ContractExpiration = dtContractExpiration;

                        addEditSchool.Contact1Name = contact1Name;
                        addEditSchool.Contact1Type = selectedContact1Type;
                        addEditSchool.Contact1Email = contact1Email;
                        addEditSchool.Contact1Notes = contact1Notes;

                        addEditSchool.Contact2Name = contact2Name;
                        addEditSchool.Contact2Type = selectedContact2Type;
                        addEditSchool.Contact2Email = contact2Email;
                        addEditSchool.Contact2Notes = contact2Notes;

                        addEditSchool.Contact3Name = contact3Name;
                        addEditSchool.Contact3Type = selectedContact3Type;
                        addEditSchool.Contact3Email = contact3Email;
                        addEditSchool.Contact3Notes = contact3Notes;
                        addEditSchool.Visit = visitAddress;
                        addEditSchool.Affiliation = affiliation;
                        if (addEditSchool != null)
                        {
                            db.SubmitChanges();
                            DialogResult = false;
                            isSave = true;
                        }
                        else
                            MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                    }
                    else
                    {
                        if (schlCount != 0)
                        {
                            if (System.Windows.MessageBox.Show(errorMessages.SCHOOL_NAME_ALREADY_EXISTS_FOLDER, "Confirmation", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                            {
                                return;
                            }
                        }
                        else
                        {
                            //create the folder
                            Directory.CreateDirectory(ImageFolderPath);
                        }

                        if (string.IsNullOrEmpty(schoolName)) { MVVMMessageService.ShowMessage("Please fill school name and Folder path."); return; }

                        addEditSchool.SchoolName = schoolName.Trim();
                        addEditSchool.Address1 = address1;
                        addEditSchool.Address2 = address2;
                        addEditSchool.Address3 = address3;
                        addEditSchool.City = city;
                        addEditSchool.State = state;
                        addEditSchool.Zip = zip;
                        addEditSchool.Notes = notes;
                        addEditSchool.folderPath = folderPath;

                        addEditSchool.YearBookRequired = yearBookYes == true ? true : false;
                        addEditSchool.IDRequired = idRequiredYes == true ? true : false;
                        //newly added columns
                        addEditSchool.Students = students;
                        addEditSchool.Rating = ratingData.ToString();
                        addEditSchool.SchoolYear = schoolYear;
                        addEditSchool.IsActive = isActive;
                        addEditSchool.StudioId = selectedStudio != null ? (int?)(selectedStudio.Id) : null;
                        addEditSchool.IDCards = idCards > 0 ? idCards : null;
                        addEditSchool.Calendars = calendars > 0 ? calendars : null;
                        addEditSchool.Pencils = pencils > 0 ? pencils : null;
                        addEditSchool.ContractExpiration = dtContractExpiration;

                        addEditSchool.Contact1Name = contact1Name;
                        addEditSchool.Contact1Type = selectedContact1Type;
                        addEditSchool.Contact1Email = contact1Email;
                        addEditSchool.Contact1Notes = contact1Notes;

                        addEditSchool.Contact2Name = contact2Name;
                        addEditSchool.Contact2Type = selectedContact2Type;
                        addEditSchool.Contact2Email = contact2Email;
                        addEditSchool.Contact2Notes = contact2Notes;

                        addEditSchool.Contact3Name = contact3Name;
                        addEditSchool.Contact3Type = selectedContact3Type;
                        addEditSchool.Contact3Email = contact3Email;
                        addEditSchool.Contact3Notes = contact3Notes;
                        addEditSchool.Visit = string.IsNullOrEmpty(visitAddress) ? "" : visitAddress;
                        addEditSchool.Affiliation = affiliation;

                        if (addEditSchool != null)
                        {
                            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                            db.Schools.InsertOnSubmit(addEditSchool);
                            db.SubmitChanges();

                            //instead of using storedprocs we are directly hitting DB
                            //Changed By #Mohan
                            //int retval = 0;
                            //int maxId = Convert.ToInt32(clsDashBoard.getMaxSchoolId(db));
                            //SqlParameter[] param = new SqlParameter[1];
                            //param[0] = new SqlParameter("@SchoolID", maxId);
                            //retval = WCFSQLHelper.executeScaler_SP("sp_InsertPackages", param);

                            //Insert school Packages
                            clsSchool.insertSchoolPackages(db, addEditSchool.ID);

                            DialogResult = false;
                            isSave = true;
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                        }
                    }
                    DialogResult = false;
                }
                else if (errorCount == 0 && string.IsNullOrEmpty(folderPath))
                    MVVMMessageService.ShowMessage(errorMessages.FILL_SCHOOL_PATH);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        #endregion
    }
}
