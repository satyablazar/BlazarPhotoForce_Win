using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.ImageQuixManagement
{
    public class AddEditPriceSheetViewModel : ViewModelBase
    {

        #region Initialization
        PhotoSorterDBModelDataContext db;
        public SimplePhotoPriceSheet _objSPPriceSheet = new SimplePhotoPriceSheet();
        public bool isSave = false;
        #endregion

        #region Properties
        string _priceSheetID;
        string _description;

        public string description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged(); }
        }
        public string priceSheetID
        {
            get { return _priceSheetID; }
            set { _priceSheetID = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructor
        public AddEditPriceSheetViewModel()
        {
            _objSPPriceSheet = null;
        }
        public AddEditPriceSheetViewModel(SimplePhotoPriceSheet selectedPriceSheet)
        {
            _objSPPriceSheet = selectedPriceSheet;

            priceSheetID = selectedPriceSheet.SPPriceSheetId.ToString();
            description = selectedPriceSheet.Description;
        }
        #endregion

        #region Commands
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                return new RelayCommand(save);
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
        private void save()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                if (_objSPPriceSheet != null)
                {
                    _objSPPriceSheet = (from SPP in db.SimplePhotoPriceSheets where SPP.Id == _objSPPriceSheet.Id select SPP).FirstOrDefault();

                    if (_objSPPriceSheet != null)
                    {
                        _objSPPriceSheet.SPPriceSheetId = Convert.ToInt32(priceSheetID);
                        _objSPPriceSheet.Description = description;

                        db.SubmitChanges();
                        isSave = true; DialogResult = false;
                    }
                }
                else
                {
                    _objSPPriceSheet = new SimplePhotoPriceSheet();

                    _objSPPriceSheet.SPPriceSheetId = Convert.ToInt32(priceSheetID);
                    _objSPPriceSheet.Description = description;

                    db.SimplePhotoPriceSheets.InsertOnSubmit(_objSPPriceSheet);
                    db.SubmitChanges();
                    isSave = true; DialogResult = false;
                }

            }
            catch (Exception ex)
            {
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
