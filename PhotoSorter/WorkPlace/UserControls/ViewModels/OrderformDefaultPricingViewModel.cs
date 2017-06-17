using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Data.SqlClient;
using Ookii.Dialogs.Wpf;
using PhotoForce.Settings;
namespace PhotoForce.WorkPlace.UserControls
{
    public class OrderformDefaultPricingViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        EditDefaultPricing _editDefaultPricing;
        #endregion

        #region Properties
        IEnumerable<DefaultPackage> _dgDefaultPricingData;
        private DefaultPackage _selectedefaultPackage;

        public DefaultPackage SelectedefaultPackage
        {
            get { return _selectedefaultPackage; }
            set { _selectedefaultPackage = value; NotifyPropertyChanged("SelectedefaultPackage"); }
        }

        public IEnumerable<DefaultPackage> dgDefaultPricingData
        {
            get { return _dgDefaultPricingData; }
            set { _dgDefaultPricingData = value; NotifyPropertyChanged("dgDefaultPricingData"); }
        }
        #endregion

        #region Constructors
        public OrderformDefaultPricingViewModel()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            dgDefaultPricingData = clsDashBoard.getDefaultPricing(db);
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
        public RelayCommand PackagesMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(editPackage);
            }
        }
        public RelayCommand EditPackageCommand
        {
            get
            {
                return new RelayCommand(EditPackage);
            }
        }
        public RelayCommand RowUpdateCommand
        {
            get
            {
                return new RelayCommand(rowUpdate);
            }
        }
        #endregion

        #region Methods
        private void EditPackage()
        {
            if (SelectedefaultPackage != null)
                editPackage();
            else
            { MVVMMessageService.ShowMessage("Please select a OrderDefaultPricing"); }
        }

        private void saveAndClose()
        {
            try
            {
                //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                int retval = 0;
                int i = 0;
                IEnumerable<DefaultPackage> _objdefpack;
                _objdefpack = clsDashBoard.getDefaultPricing(db);
                foreach (DefaultPackage defpac in _objdefpack)
                {
                    try
                    {
                        i++;
                        SqlParameter[] param = new SqlParameter[3];
                        param[0] = new SqlParameter("@ID", Convert.ToInt32(defpac.ID));
                        param[1] = new SqlParameter("@Package", Convert.ToString(defpac.package));
                        param[2] = new SqlParameter("@Price", Convert.ToDouble(defpac.price));
                        retval = WCFSQLHelper.executeNonQuery_SP("sp_UpdateDefaultPackage", param);
                        if (i == _objdefpack.Count())
                        {
                            MVVMMessageService.ShowMessage("Data updated successfully.");
                            dgDefaultPricingData = clsDashBoard.getDefaultPricing(db);
                        }
                    }
                    catch (Exception ex)
                    {
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void bindData()
        {
            try
            {
                dgDefaultPricingData = clsDashBoard.getDefaultPricing(db);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        public void editPackage()
        {
            try
            {
                if (SelectedefaultPackage != null)
                {
                    int PackageId = SelectedefaultPackage.ID;
                    _editDefaultPricing = new EditDefaultPricing(SelectedefaultPackage.package, SelectedefaultPackage.ID, (float)SelectedefaultPackage.price);
                    _editDefaultPricing.ShowDialog();
                    if (((EditDefaultPricingViewModel)(_editDefaultPricing.DataContext)).isSave)
                        bindData();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void rowUpdate()
        {
            db.SubmitChanges();
        }
        internal void setVisibilityForButtons()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
        }

        #endregion
    }
}
