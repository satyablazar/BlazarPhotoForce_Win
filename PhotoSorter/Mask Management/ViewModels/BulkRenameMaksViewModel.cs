using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Collections;

namespace PhotoForce.Mask_Management
{
    public class BulkRenameMaksViewModel : ViewModelBase
    {
        # region Initialization and Declaration
        ArrayList arrMaskId = new ArrayList();
        public bool isSave=false;
        # endregion

        #region Properties
        string _maskName;

        public string maskName
        {
            get { return _maskName; }
            set { _maskName = value; NotifyPropertyChanged("maskName"); }
        }
        #endregion

        # region Constructor
        public BulkRenameMaksViewModel(ArrayList tempArrMaskId)
        {
            arrMaskId = tempArrMaskId;
        }
        # endregion

        # region Commands
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
        # endregion

        # region Methods
        private void bulkRename()
        {
            try
            {
                string message = "";
                if (arrMaskId.Count > 1)
                    message = errorMessages.BEFORE_RENAMING_SELECTED_MASKS_CONFIRMATION1 + arrMaskId.Count + errorMessages.BEFORE_RENAMING_SELECTED_MASKS_CONFIRMATION2;
                else
                    message = errorMessages.BEFORE_RENAMING_SELECTED_MASK_CONFIRMATION1 + arrMaskId.Count + errorMessages.BEFORE_RENAMING_SELECTED_MASK_CONFIRMATION2;
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    
                    int i = 0;
                    if (!string.IsNullOrEmpty(maskName))
                        i = clsDashBoard.UpdateMultipleMaskName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), maskName, arrMaskId);
                    if (i != 0)
                    {
                        isSave = true;
                        DialogResult = false;
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void windowClose()
        {
            isSave=false;
            DialogResult=false;
        }
        # endregion
    }
}
