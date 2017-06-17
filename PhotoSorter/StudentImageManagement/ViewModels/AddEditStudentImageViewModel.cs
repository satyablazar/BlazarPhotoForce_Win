using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using PhotoForce.Extensions;

namespace PhotoForce.StudentImageManagement
{
    public class AddEditStudentImageViewModel : ViewModelBase
    {
        #region Initialization
        int studentImageId = 0;
        int schoolId = 0;
        int photoShootId = 0;
        //static string GridName = "studentPhotos";
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        #endregion

        #region Properties
        private string _package;
        private string _homeRoom;
        private string _selectedRating;
        private List<ComboBoxItem> _rating;
        private string _ticketCode;
        private bool? _isYearBookSelected;
        private bool? _isAdminCDSelected;
        private bool? _isYearBookSelectedNo;
        private bool? _isAdminCDSelectedNo;

        public bool? isAdminCDNotSelected
        {
            get { return _isAdminCDSelectedNo; }
            set { _isAdminCDSelectedNo = value; NotifyPropertyChanged("isAdminCDNotSelected"); }
        }
        public bool? isYearBookNotSelected
        {
            get { return _isYearBookSelectedNo; }
            set { _isYearBookSelectedNo = value; NotifyPropertyChanged("isYearBookNotSelected"); }
        }
        public bool? isAdminCDSelected
        {
            get { return _isAdminCDSelected; }
            set { _isAdminCDSelected = value; NotifyPropertyChanged("isAdminCDSelected"); }
        }
        public bool? isYearBookSelected
        {
            get { return _isYearBookSelected; }
            set { _isYearBookSelected = value; NotifyPropertyChanged("isYearBookSelected"); }
        }
        public string ticketCode
        {
            get { return _ticketCode; }
            set { _ticketCode = value; NotifyPropertyChanged("ticketCode"); }
        }
        public List<ComboBoxItem> rating
        {
            get { return _rating; }
            set { _rating = value; NotifyPropertyChanged("rating"); }
        }
        public string selectedRating
        {
            get { return _selectedRating; }
            set { _selectedRating = value; NotifyPropertyChanged("selectedRating"); }
        }
        public string homeRoom
        {
            get { return _homeRoom; }
            set { _homeRoom = value; NotifyPropertyChanged("homeRoom"); }
        }
        public string package
        {
            get { return _package; }
            set { _package = value; NotifyPropertyChanged("package"); }
        }
        #endregion

        #region Constructors
        public AddEditStudentImageViewModel(int tempStuImgId, int tempSchoolId, int tempPhotoShootId)
        {
            studentImageId = tempStuImgId;
            schoolId = tempSchoolId;
            photoShootId = tempPhotoShootId;
            bindRating();

            if (studentImageId != 0)
                bindStudentImgData();
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(createEditStudentImage);
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
        private void bindStudentImgData()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                IEnumerable<StudentImage> dt = clsDashBoard.getStudentImgDetails(db, studentImageId);
                package = Convert.ToString(dt.First().Packages);
                homeRoom = Convert.ToString(dt.First().HomeRoom);
                ticketCode = Convert.ToString(dt.First().Ticketcode);
                selectedRating = Convert.ToString(dt.First().Rating);
                if (Convert.ToBoolean(dt.First().yearbook))
                    isYearBookSelected = true;
                else
                    isYearBookNotSelected = true;

                if (Convert.ToBoolean(dt.First().Admincd))
                    isAdminCDSelected = true;
                else
                    isAdminCDNotSelected = true;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void createEditStudentImage()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (studentImageId != 0)
                {
                    //update
                    StudentImage _objnewstudentimg = new StudentImage();
                    _objnewstudentimg = clsDashBoard.getStudentImageDetailsById(db, studentImageId);
                    _objnewstudentimg.Packages = package;
                    _objnewstudentimg.HomeRoom = homeRoom;
                    _objnewstudentimg.Ticketcode = ticketCode;
                    _objnewstudentimg.Rating = selectedRating == "Clear" ? null : selectedRating;

                    if (Convert.ToBoolean(isYearBookSelected))
                        _objnewstudentimg.yearbook = true;
                    else
                        _objnewstudentimg.yearbook = false;

                    if (Convert.ToBoolean(isAdminCDSelected))
                        _objnewstudentimg.Admincd = true;
                    else
                        _objnewstudentimg.Admincd = false;

                    if (_objnewstudentimg != null)
                    {
                        db.SubmitChanges();
                        isSave = true;
                    }
                    else
                        MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                    DialogResult = false;
                }
                //how can we create an image.
                //#Mohan
                //else
                //{
                //    //new
                //    StudentImage _objStudentImage = new StudentImage();
                //    _objStudentImage.SchoolID = schoolId;
                //    _objStudentImage.PhotoShootID = photoShootId;
                //    _objStudentImage.Packages = package;
                //    _objStudentImage.Rating = selectedRating == "Clear" ? null : selectedRating;

                //    if (Convert.ToBoolean(isYearBookSelected))
                //        _objStudentImage.yearbook = true;
                //    else
                //        _objStudentImage.yearbook = false;

                //    if (Convert.ToBoolean(isAdminCDSelected))
                //        _objStudentImage.Admincd = true;
                //    else
                //        _objStudentImage.Admincd = false;

                //    if (_objStudentImage != null)
                //    {
                //        db.StudentImages.InsertOnSubmit(_objStudentImage);
                //        db.SubmitChanges();
                //        isSave = true;
                //    }
                //    else
                //        MVVMMessageService.ShowMessage("Error found, Contact Administrator");
                //    DialogResult = false;
                //}
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage("Error While Saving. Please check the exception log for more details.");
            }
        }
        private void bindRating()
        {
            rating = new List<ComboBoxItem>();
            rating.Add(new ComboBoxItem { Name = " " });
            rating.Add(new ComboBoxItem { Name = "Clear" });
            for (int i = 1; i <= 5; i++)
            {
                rating.Add(new ComboBoxItem { Name = i.ToString() });
            }
        }
        private void windowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion
    }
}
