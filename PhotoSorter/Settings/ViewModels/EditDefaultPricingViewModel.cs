using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using PhotoForce.App_Code;
using PhotoForce.MVVM;

namespace PhotoForce.Settings
{
   public class EditDefaultPricingViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;
        public bool isSave = false;
        int packageId = 0;
        string pack = "";

        #endregion

        #region Properties
        string _package;
        float _price;

        public float price
        {
            get { return _price; }
            set { _price = value; NotifyPropertyChanged("price"); }
        }
        public string package
        {
            get { return _package; }
            set { _package = value; NotifyPropertyChanged("package"); }
        }
        #endregion

        public EditDefaultPricingViewModel(string Package, int tempPackageId, float Price)
        {
            package = Package;
            pack = Package;
            packageId = tempPackageId;
            price = Price;
        }

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(updateDefaultPackaging);
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
        public void updateDefaultPackaging()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                if (package != "" )
                {
                    int result = clsOrders.upadteDefaultPricing(db, package, packageId, price);
                    DialogResult = false;
                    isSave = true;
                    MVVMMessageService.ShowMessage("Package updated successfully.");
                }
                else
                {
                    MVVMMessageService.ShowMessage("Please add a package");
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        public void windowClose()
        {
            DialogResult = false;
            isSave = false;
        }
        #endregion


    }
}
