using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;

namespace PhotoForce.Student_Management
{
    public class CountStudentsViewModel : ViewModelBase
    {
        #region Initialization
        public bool isSave = false;     //#Mohan ; #NUnitTest
        PhotoSorterDBModelDataContext db;
        #endregion

        #region Properties
        private IEnumerable<PhotographyJob> _cbSchoolYearData;
        private PhotographyJob _cbSchoolYearSelectedItem;

        public PhotographyJob cbSchoolYearSelectedItem
        {
            get { return _cbSchoolYearSelectedItem; }
            set { _cbSchoolYearSelectedItem = value; NotifyPropertyChanged("cbSchoolYearSelectedItem"); }
        }
        public IEnumerable<PhotographyJob> cbSchoolYearData
        {
            get { return _cbSchoolYearData; }
            set { _cbSchoolYearData = value; NotifyPropertyChanged("cbSchoolYearData"); }
        }
        #endregion

        #region Constructors
        public CountStudentsViewModel()
        {
            bindSchoolYear();
        }
        #endregion

        #region Commands
        public RelayCommand SelectOKCommand
        {
            get
            {
                return new RelayCommand(selectOK);
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
        public void bindSchoolYear()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            cbSchoolYearData = clsDashBoard.getJobs(db, clsSchool.defaultSchoolId);
        }
        private void selectOK()
        {
            if (cbSchoolYearSelectedItem != null)
            {
                isSave = true;
                DialogResult = false;
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
