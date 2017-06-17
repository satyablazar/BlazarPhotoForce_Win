using PhotoForce.App_Code;
using PhotoForce.Extensions;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.OrdersManagement
{
    public class CreateNewBatchViewModel : ViewModelBase
    {
        #region Initialization
        public bool isSave;//changed from internal to public by hema-NUnit
        internal OrdersImport _objOrdersImport = new OrdersImport();
        string gridName = "";
        public WorkflowCollection _objWorkflowCollection;
        WorkflowCollection tempBatch;
        public bool tempWorkflowCollections;
        #endregion

        #region Properties
        string _newBatchName;
        string _lblname;
        List<string> _itemClassTypes;
        string _selectedItemClassType;
        Visibility _isClassTypeVisible;

        public Visibility isClassTypeVisible
        {
            get { return _isClassTypeVisible; }
            set { _isClassTypeVisible = value; NotifyPropertyChanged(); }
        }
        public string selectedItemClassType
        {
            get { return _selectedItemClassType; }
            set { _selectedItemClassType = value; NotifyPropertyChanged(); }
        }
        public List<string> itemClassTypes
        {
            get { return _itemClassTypes; }
            set { _itemClassTypes = value; NotifyPropertyChanged(); }
        }
        public string lblname
        {
            get { return _lblname; }
            set { _lblname = value; NotifyPropertyChanged("lblname"); }
        }
        public string newBatchName
        {
            get { return _newBatchName; }
            set { _newBatchName = value; NotifyPropertyChanged("newBatchName"); }
        }
        #endregion

        #region Constructor
        public CreateNewBatchViewModel()
        {
            lblname = "Batch Name :";
            isClassTypeVisible = Visibility.Collapsed;

        }
        public CreateNewBatchViewModel(string isFrom, bool isWorkflowCollections)
        {
            gridName = isFrom;
            lblname = "Collection Name :";
            tempWorkflowCollections = isWorkflowCollections;
            itemClassTypes = new List<string>();
            itemClassTypes.Add("Workflow"); itemClassTypes.Add("Equipment");
            isClassTypeVisible = Visibility.Visible;
        }
        public CreateNewBatchViewModel(string isFrom,WorkflowCollection editBatch)
        {
            lblname = "Collection Name :";

            tempBatch = editBatch;
            newBatchName = tempBatch.Name;
            gridName = isFrom;
            itemClassTypes = new List<string>();
            itemClassTypes.Add("Workflow"); itemClassTypes.Add("Equipment");
            isClassTypeVisible = Visibility.Collapsed;
        }
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(WindowClose);
            }
        }
        #endregion

        #region Methods
        //Creating new batch
        private void Save()
        {
            if (gridName == "Collections")
            {
                try
                {
                    if (tempBatch!=null )
                    {
                        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                        WorkflowCollection tempData = (from wcb in db.WorkflowCollections where wcb.Id == tempBatch.Id select wcb).FirstOrDefault();

                        if (tempBatch.Name != newBatchName)
                        {
                            tempData.Name = newBatchName;
                            db.SubmitChanges();
                            isSave = true;
                            DialogResult = false;
                        }
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(newBatchName) && !string.IsNullOrEmpty(selectedItemClassType))
                        //{
                            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                            _objWorkflowCollection = new WorkflowCollection();
                            _objWorkflowCollection.CreatedBy = clsStatic.userName;
                            _objWorkflowCollection.CreatedOn = DateTime.Now;
                            _objWorkflowCollection.Name = newBatchName;
                            if (tempWorkflowCollections)
                                selectedItemClassType = "Workflow";
                            else
                                selectedItemClassType = "Equipment";

                            _objWorkflowCollection.ItemClassTypeId = (from ic in db.ItemClassTypes where ic.ClassType == selectedItemClassType select ic.Id).FirstOrDefault();

                            if (_objWorkflowCollection != null)
                            {
                                db.WorkflowCollections.InsertOnSubmit(_objWorkflowCollection);
                                db.SubmitChanges();
                                isSave = true;
                                DialogResult = false;
                            }
                        //}
                        //else
                        //{
                        //    isSave = false;
                        //    if(string.IsNullOrEmpty(newBatchName))
                        //        MVVMMessageService.ShowMessage("Please enter collection name.");
                        //    else
                        //        MVVMMessageService.ShowMessage("Please enter class type.");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    isSave = false;
                    MVVMMessageService.ShowMessage(ex.Message);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(newBatchName))
                {
                    PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                    _objOrdersImport.Description = newBatchName;
                    _objOrdersImport.Notes = "";
                    _objOrdersImport.CreatedOn = DateTime.Now;
                    _objOrdersImport.OrderType = OrderTypeInOrdersImport.Manual.ToString();
                    _objOrdersImport.SchoolID = clsSchool.defaultSchoolId;
                    if (_objOrdersImport != null)
                    {
                        db.OrdersImports.InsertOnSubmit(_objOrdersImport);
                        db.SubmitChanges();
                    }

                    isSave = true;
                    DialogResult = false;
                }
                else
                {
                    MVVMMessageService.ShowMessage("Please enter batch name.");
                }
            }           
        }
        private void WindowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion

    }
}
