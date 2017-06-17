using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.Student_Management
{
    public class EditImportBatchesViewModel : ViewModelBase
    {
        #region Initialization
        public bool isSave = false;
        DataRowView selectedStudentImport;
        //IQPriceSheet selectedIQPriceSheet;
        //IQAccount selectedIQAccount;
        //string callFrom;
        //public IQPriceSheet _objIQPriceSheet;
        #endregion

        #region Properties
        //Item Description :
        string _labelName;

        public string labelName
        {
            get { return _labelName; }
            set { _labelName = value; NotifyPropertyChanged("labelName"); }
        }
        public string _itemDescription;

        public string itemDescription
        {
            get { return _itemDescription; }
            set { _itemDescription = value; NotifyPropertyChanged("itemDescription"); }
        }
        #endregion

        #region Constructor
        public EditImportBatchesViewModel(DataRowView studentImportSelectedItem)
        {
            labelName = "Item Description :";
            //callFrom = "Import-Batches";
            selectedStudentImport = studentImportSelectedItem;
            itemDescription = selectedStudentImport["Description"].ToString();
        }
        //public EditImportBatchesViewModel(IQPriceSheet iqPriceSheet, IQAccount iqAccount)
        //{
        //    labelName = "   Description :";
        //    selectedIQPriceSheet = iqPriceSheet;
        //    selectedIQAccount = iqAccount;
        //    if (iqPriceSheet == null )
        //    {
        //        itemDescription = "";
        //        callFrom = "New-PriceSheet";
        //    }
        //    else
        //    {
        //        itemDescription = selectedIQPriceSheet.Description;
        //        callFrom = "Edit-PriceSheet";
        //    }

        //}
        #endregion

        #region Commands
        public RelayCommand SaveCommand
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
        void save()
        {
            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (string.IsNullOrEmpty(itemDescription)) { MVVMMessageService.ShowMessage("Plaese enter item description."); return; }
            //if (callFrom == "Import-Batches")
            //{
                StudentImport editImportDescription = new StudentImport();
                editImportDescription = clsDashBoard.getSelectedStudentImportData(db, clsSchool.defaultSchoolId, (int)selectedStudentImport["ID"]);
                if (editImportDescription != null)
                {
                    editImportDescription.Description = itemDescription;
                    db.SubmitChanges();
                    isSave = true;
                    DialogResult = false;
                }
            //}
            //else
            //{
            //    _objIQPriceSheet = new IQPriceSheet();
            //    if (callFrom == "New-PriceSheet")
            //    {
            //        _objIQPriceSheet.Description = itemDescription;
            //        _objIQPriceSheet.IQAccountId = selectedIQAccount.Id;

            //        db.IQPriceSheets.InsertOnSubmit(_objIQPriceSheet);
            //        db.SubmitChanges();
            //    }
            //    else if (callFrom == "Edit-PriceSheet")
            //    {
            //        _objIQPriceSheet = (from IQP in db.IQPriceSheets where IQP.Id == selectedIQPriceSheet.Id && IQP.IQAccountId == selectedIQAccount.Id select IQP).FirstOrDefault();

            //        _objIQPriceSheet.Description = itemDescription;

            //        db.SubmitChanges();
            //        isSave = true;
            //    }
            //    DialogResult = false;
            //}

        }

        void windowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion

    }
}
