using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.OrdersManagement
{
    public class ImagesWithoutOrdersViewModel : ViewModelBase
    {
        #region Properties
        List<Group> _cbSelectGroupData;
        ObservableCollection<OrdersImport> _cbExistingBatchData;
        OrdersImport _selectedExistingBatch;
        Group _selectedGroup;
        ObservableCollection<GroupItem> _dgStudentPhotosData;
        ObservableCollection<GroupItem> _selectedStudentPhotos;

        public ObservableCollection<GroupItem> selectedStudentPhotos
        {
            get { return _selectedStudentPhotos; }
            set { _selectedStudentPhotos = value; NotifyPropertyChanged("selectedStudentPhotos"); }
        }
        public ObservableCollection<GroupItem> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        public Group selectedGroup
        {
            get { return _selectedGroup; }
            set { _selectedGroup = value; NotifyPropertyChanged("selectedGroup"); }
        }
        public OrdersImport selectedExistingBatch
        {
            get { return _selectedExistingBatch; }
            set { _selectedExistingBatch = value; NotifyPropertyChanged("selectedExistingBatch"); }
        }
        public ObservableCollection<OrdersImport> cbExistingBatchData
        {
            get { return _cbExistingBatchData; }
            set { _cbExistingBatchData = value; NotifyPropertyChanged("cbExistingBatchData"); }
        }
        public List<Group> cbSelectGroupData
        {
            get { return _cbSelectGroupData; }
            set { _cbSelectGroupData = value; NotifyPropertyChanged("cbSelectGroupData"); }
        }
        #endregion

        #region Constructors
        public ImagesWithoutOrdersViewModel()
        {
            cbExistingBatchData = new ObservableCollection<OrdersImport>();
            cbSelectGroupData = new List<Group>(); selectedStudentPhotos = new ObservableCollection<GroupItem>();
            dgStudentPhotosData = new ObservableCollection<GroupItem>();
            bindData();
        }
        #endregion

        #region Commands
        public RelayCommand ShowResultsCommand
        {
            get
            {
                return new RelayCommand(showResults);
            }
        }
        public RelayCommand CreateNewBatchCommand
        {
            get
            {
                return new RelayCommand(createNewBatch);
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
        private void bindData()
        {
            cbSelectGroupData = clsGroup.getAllGroups(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId).ToList();
            selectedGroup = cbSelectGroupData != null ? cbSelectGroupData.FirstOrDefault() : null;
            cbExistingBatchData = new ObservableCollection<OrdersImport>(clsOrders.getOrderImportBatchs(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString)).ToList());
            selectedExistingBatch = cbExistingBatchData != null ? cbExistingBatchData.FirstOrDefault() : null;
        }
        private void showResults()
        {
            if (selectedGroup != null && selectedExistingBatch != null)
            {
                dgStudentPhotosData = new ObservableCollection<GroupItem>(clsGroup.getstudentImagesWithoutOrdersByGroupAndBatchIds(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedGroup.ID, selectedExistingBatch.Id));
            }
            else
            {
                MVVMMessageService.ShowMessage("Please select group and batch.");
            }
        }

        void createNewBatch()
        {
            CreateNewBatch _objCreateNewBatch = new CreateNewBatch();
            _objCreateNewBatch.ShowDialog();

            if (((CreateNewBatchViewModel)(_objCreateNewBatch.DataContext)).isSave)
            {
                OrdersImport tempOrderImport = ((CreateNewBatchViewModel)(_objCreateNewBatch.DataContext))._objOrdersImport;

                string batchName = ((CreateNewBatchViewModel)(_objCreateNewBatch.DataContext)).newBatchName;
                cbExistingBatchData.Insert(0, tempOrderImport);
                selectedExistingBatch = tempOrderImport;
            }
        }

        void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
