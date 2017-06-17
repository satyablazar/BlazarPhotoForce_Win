using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System.Collections;
using System.Collections.ObjectModel;

namespace PhotoForce.School_Management
{
    public class BulkRenameSchoolViewModel : ViewModelBase
    {
        #region Initialization
        public bool isSave = false;
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        ArrayList arrSchoolId = new ArrayList();
        string idRequired = "";
        string yearBook = "";
        string isActive = "";
        #endregion

        #region Properties

        private string _schoolName;
        private string _address1;
        private string _address2;
        private string _address3;
        private string _city;
        private string _state;
        private string _zip;
        private string _notes;
        private string _folderPath;
        private string _affiliation;

        public string affiliation
        {
            get { return _affiliation; }
            set { _affiliation = value; NotifyPropertyChanged(); }
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
        public BulkRenameSchoolViewModel(ArrayList tempArrSchoolId)
        {
            arrSchoolId = tempArrSchoolId;
            List<String> tempContactType = new List<string>(); contact1TypeData = new ObservableCollection<string>();
            contact2TypeData = new ObservableCollection<string>(); contact3TypeData = new ObservableCollection<string>();
            tempContactType.Add("Principal"); tempContactType.Add("Admin"); tempContactType.Add("Other"); tempContactType.Add("YB");
            //contact1TypeData.Add("Principal"); contact1TypeData.Add("Admin"); contact1TypeData.Add("Other"); contact1TypeData.Add("YB");
            //contact2TypeData.Add("Principal"); contact2TypeData.Add("Admin"); contact2TypeData.Add("Other"); contact2TypeData.Add("YB");
            //contact3TypeData.Add("Principal"); contact3TypeData.Add("Admin"); contact3TypeData.Add("Other"); contact3TypeData.Add("YB");

            contact1TypeData = new ObservableCollection<string>(tempContactType);
            contact2TypeData = new ObservableCollection<string>(tempContactType);
            contact3TypeData = new ObservableCollection<string>(tempContactType);
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
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand<string> YearBookCommand
        {
            get
            {
                return new RelayCommand<string>(selectedYearBook);
            }
        }
        public RelayCommand<string> IdRequiredCommand
        {
            get
            {
                return new RelayCommand<string>(selectedIdRequiredValue);
            }
        }
        public RelayCommand<string> IsActiveCommand
        {
            get
            {
                return new RelayCommand<string>(selectedIsActiveValue);
            }
        }
        #endregion

        #region Methods
        private void bulkRename()
        {
            try
            {
                if (arrSchoolId.Count == 0)
                {
                    MVVMMessageService.ShowMessage("Please select a school(s) to rename.");
                    return;
                }
                string message = "";
                if (arrSchoolId.Count > 1)
                    message = errorMessages.BEFORE_RENAMING_SELECTED_SCHOOLS_CONFIRMATION1 + arrSchoolId.Count + errorMessages.BEFORE_RENAMING_SELECTED_SCHOOLS_CONFIRMATION2;
                else
                    message = errorMessages.BEFORE_RENAMING_SELECTED_SCHOOL_CONFIRMATION1 + arrSchoolId.Count + errorMessages.BEFORE_RENAMING_SELECTED_SCHOOL_CONFIRMATION2;
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    int i = 0;
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    if (!string.IsNullOrEmpty(schoolName))
                        i = clsDashBoard.UpdateMultipleSchoolName(db, schoolName, arrSchoolId);
                    if (!string.IsNullOrEmpty(address1))
                        i = clsDashBoard.UpdateMultipleAddress1(db, address1, arrSchoolId);
                    if (!string.IsNullOrEmpty(address2))
                        i = clsDashBoard.UpdateMultipleAddress2(db, address2, arrSchoolId);
                    if (!string.IsNullOrEmpty(address3))
                        i = clsDashBoard.UpdateMultipleAddress3(db, address3, arrSchoolId);
                    if (!string.IsNullOrEmpty(city))
                        i = clsDashBoard.UpdateMultipleCity(db, city, arrSchoolId);
                    if (!string.IsNullOrEmpty(state))
                        i = clsDashBoard.UpdateMultipleState(db, state, arrSchoolId);
                    if (!string.IsNullOrEmpty(zip))
                        i = clsDashBoard.UpdateMultipleZip(db, zip, arrSchoolId);
                    if (!string.IsNullOrEmpty(notes))
                        i = clsDashBoard.UpdateMultipleNotes(db, notes, arrSchoolId);
                    if (yearBook != "")
                        i = clsDashBoard.UpdateMultipleYearbookForSchool(db, Convert.ToBoolean(yearBook), arrSchoolId);
                    if (idRequired != "")
                        i = clsDashBoard.UpdateMultipleIDRequiredForSchool(db, Convert.ToBoolean(idRequired), arrSchoolId);
                    if (isActive != "")
                        i = clsDashBoard.UpdateMultipleIsActiveValueForSchool(db, Convert.ToBoolean(isActive), arrSchoolId);
                    if (!string.IsNullOrEmpty(affiliation))
                        i = clsDashBoard.UpdateMultipleAffiliation(db, affiliation, arrSchoolId);
                    #region Conatct details
                    if (!string.IsNullOrEmpty(contact1Name))
                        i = clsDashBoard.UpdateMultipleConatctsName(db, 1 , contact1Name, arrSchoolId);
                    if (!string.IsNullOrEmpty(selectedContact1Type))
                        i = clsDashBoard.UpdateMultipleConatctsType(db, 1 , selectedContact1Type, arrSchoolId);
                    if (!string.IsNullOrEmpty(contact1Email))
                        i = clsDashBoard.UpdateMultipleConatctsEmail(db, 1, contact1Email, arrSchoolId);
                    if (!string.IsNullOrEmpty(contact1Notes))
                        i = clsDashBoard.UpdateMultipleConatctsNotes(db, 1, contact1Notes, arrSchoolId);

                    if (!string.IsNullOrEmpty(contact2Name))
                        i = clsDashBoard.UpdateMultipleConatctsName(db, 2, contact2Name, arrSchoolId);
                    if (!string.IsNullOrEmpty(selectedContact2Type))
                        i = clsDashBoard.UpdateMultipleConatctsType(db, 2 , selectedContact2Type, arrSchoolId);
                    if (!string.IsNullOrEmpty(contact2Email))
                        i = clsDashBoard.UpdateMultipleConatctsEmail(db, 2 , contact2Email, arrSchoolId);
                    if (!string.IsNullOrEmpty(contact2Notes))
                        i = clsDashBoard.UpdateMultipleConatctsNotes(db, 2, contact2Notes, arrSchoolId);

                    if (!string.IsNullOrEmpty(contact3Name))
                        i = clsDashBoard.UpdateMultipleConatctsName(db, 3 , contact3Name, arrSchoolId);
                    if (!string.IsNullOrEmpty(selectedContact3Type))
                        i = clsDashBoard.UpdateMultipleConatctsType(db, 3 , selectedContact3Type, arrSchoolId);
                    if (!string.IsNullOrEmpty(contact3Email))
                        i = clsDashBoard.UpdateMultipleConatctsEmail(db, 3, contact3Email, arrSchoolId);                    
                    #endregion

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
        private void selectedYearBook(string tempYearbook)
        {
            yearBook = tempYearbook;
        }
        private void selectedIdRequiredValue(string tempIdRequiredValue)
        {
            idRequired = tempIdRequiredValue;
        }
        private void selectedIsActiveValue(string tempIsActiveValue)
        {
            isActive = tempIsActiveValue;
        }
        private void windowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion
    }
}
