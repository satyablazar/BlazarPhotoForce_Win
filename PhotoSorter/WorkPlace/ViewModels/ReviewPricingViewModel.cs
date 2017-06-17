using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using PhotoForce.App_Code;
using PhotoForce.MVVM;

namespace PhotoForce.WorkPlace
{
    public class ReviewPricingViewModel : ViewModelBase
    {
        #region Initialization
        static int schoolId = 0;
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        public bool isError = false; //NUnitTesting
        #endregion

        #region Properties
        private IEnumerable<Package> _packages;

        public IEnumerable<Package> packages
        {
            get { return _packages; }
            set { _packages = value; NotifyPropertyChanged("packages"); }
        }
        #endregion

        #region Constructors
        public ReviewPricingViewModel(int tempSchoolId)
        {
            schoolId = tempSchoolId;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            packages = clsDashBoard.getPackagesBySchoolId(db, schoolId);
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
        private void saveAndClose()
        {
            try
            {
                
                foreach (Package tempPackage in packages)
                {
                    try
                    {
                        int retval = 0;
                        SqlParameter[] param = new SqlParameter[3];
                        param[0] = new SqlParameter("@ID", tempPackage.ID.ToString());
                        param[1] = new SqlParameter("@Package", Convert.ToString(tempPackage.package1));
                        param[2] = new SqlParameter("@Price", Convert.ToDouble(tempPackage.price.ToString()));
                        retval = WCFSQLHelper.executeNonQuery_SP("sp_UpdatePackage", param);
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                        isError = true;
                    }
                }

                if (!isError)
                {
                    DialogResult = false;
                }
                else
                    MVVMMessageService.ShowMessage("Prize updated with some errors. Please check the exception Log for Details.");
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
