using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Collections;
using PhotoForce.Extensions;
using System.ComponentModel;
using System.Threading;


namespace PhotoForce.GroupManagement
{
    public class AddStudentsToGroupViewModel : ViewModelBase
    {
        # region Initialization and Declaration
        int schoolId;
        ArrayList arrStudentImageIds;
        public bool isSave = false;
        PhotoSorterDBModelDataContext db;
        bool isInProgress = false;
        bool isProgressCancelled = false;
        //CancellationTokenSource cts;
        WaitCursorViewModel _objWaitCursorViewModel; 
        # endregion

        #region Properties
        private IEnumerable<GroupExtraColumn> _dgAddStudentsToGroupData;
        private GroupExtraColumn _selectedRowItem;
        private System.Windows.Visibility _isVisibleProcessBar;
        private int _currentProgress;
        private string _processBarStatus;
        private int _processing;
        private int _minValue = 0;
        private int _maxValue = 100;

        public GroupExtraColumn selectedRowItem
        {
            get { return _selectedRowItem; }
            set { _selectedRowItem = value; NotifyPropertyChanged("selectedRowItem"); }
        }
        public IEnumerable<GroupExtraColumn> dgAddStudentsToGroupData
        {
            get { return _dgAddStudentsToGroupData; }
            set { _dgAddStudentsToGroupData = value; NotifyPropertyChanged("dgAddStudentsToGroupData"); }
        }
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; NotifyPropertyChanged("MaxValue"); }
        }
        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; NotifyPropertyChanged("MinValue"); }
        }
        public int Processing
        {
            get { return _processing; }
            set { _processing = value; NotifyPropertyChanged("Processing"); }
        }
        public string ProcessBarStatus
        {
            get { return _processBarStatus; }
            set { _processBarStatus = value; NotifyPropertyChanged("ProcessBarStatus"); }
        }
        public int CurrentProgress
        {
            get { return _currentProgress; }
            set { _currentProgress = value; NotifyPropertyChanged("CurrentProgress"); }
        }
        public System.Windows.Visibility IsVisibleProcessBar
        {
            get { return _isVisibleProcessBar; }
            set { _isVisibleProcessBar = value; NotifyPropertyChanged("IsVisibleProcessBar"); }
        }
        #endregion

        #region Constructors
        public AddStudentsToGroupViewModel(int sclId, ArrayList tempStdImageIds)
        {
            IsVisibleProcessBar = System.Windows.Visibility.Hidden;
            arrStudentImageIds = tempStdImageIds;
            schoolId = sclId;
            loadData();
        }
        #endregion

        #region Commands
        public RelayCommand AddToGroupCommand
        {
            get
            {
                return new RelayCommand(addToGroup);
            }
        }
        public RelayCommand AddNewGroupCommand
        {
            get
            {
                return new RelayCommand(addNewGroup);
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

        #region Threads
        # region Add Student Images to Group

        #region old code
        //private void bw_DoWork_AddToGroup(object sender, DoWorkEventArgs e)
        //{
        //    //int groupid = (int)e.Argument;
        //    //isSave = true;
        //    //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        //    //clsGroup.insertGroupItems(db, groupid, arrStudentImageIds);

        //    int groupid = selectedRowItem.ID;
        //    isSave = true;
        //    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        //    // clsGroup.insertGroupItems(db, groupid, arrStudentImageIds);
        //    MaxValue = arrStudentImageIds.Count;

        //    for (int i = 0; i < MaxValue; i++)
        //    {
        //        if (isProgressCancelled)
        //        {
        //            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
        //            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
        //            string message = "Are you sure, you want to cancel the operation?";
        //            if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
        //            {
        //                e.Cancel = true;
        //                DialogResult = false;
        //                return;
        //            }
        //            isProgressCancelled = false;
        //        }
        //        int id = (int)arrStudentImageIds[i];
        //        clsGroup.insertGroupItems(db, groupid, id);
        //        CurrentProgress++;
        //    }
        //}
        //# region RunWorkerCompleted
        //private void bw_RunWorkerCompleted_AddToGroup(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    IsVisibleProcessBar = System.Windows.Visibility.Collapsed;
        //    MVVMMessageService.ShowMessage("Student records added successfully.");
        //    DialogResult = false;
        //}
        //#endregion
        #endregion

        
        /// <summary>
        /// The method has an async modifier. 
        /// The return type is Task or Task T. 
        /// Here, it is Task T because the return statement returns an integer.
        /// The method name ends in "Async."
        /// 
        /// You can avoid performance bottlenecks and enhance the overall responsiveness of your application by using asynchronous programming.
        /// However, traditional techniques for writing asynchronous applications can be complicated, making them difficult to write, debug, and maintain.
        /// This async method will execute ,if you want to return anything to called method
        /// declare a return type 
        /// 
        /// This method will add selected images to group
        /// </summary>
        /// <returns></returns>
        private async Task addToGroupAsync()//CancellationToken ct
        {
            _objWaitCursorViewModel = new WaitCursorViewModel();
            int groupid = selectedRowItem.ID;
            isSave = true;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            MaxValue = arrStudentImageIds.Count;
            //if you want to return ,follow below code
            //int processCount = await Task.Run<int>
            await Task.Run(() =>
            {
                for (int i = 0; i < MaxValue; i++)
                {
                    if (isProgressCancelled)
                    {
                        System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                        string message = "Are you sure, you want to cancel the operation?";
                        if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                        {
                            break;
                        }
                        isProgressCancelled = false;
                    }
                    //ct.ThrowIfCancellationRequested();
                    int id = (int)arrStudentImageIds[i];
                    clsGroup.insertGroupItems(db, groupid, id);
                    CurrentProgress++;
                }
                return CurrentProgress;
            }); //,ct
            //return <T> if you declared method as a non void
            //return processCount;
        }
        # endregion

        void addToGroupCompleted()
        {
            _objWaitCursorViewModel.Dispose();
            if (!isProgressCancelled)
            {
                MVVMMessageService.ShowMessage("Student records added successfully.");
            }
            IsVisibleProcessBar = System.Windows.Visibility.Collapsed;
            DialogResult = false;
        }

        #endregion

        #region Methods
        private void loadData()
        {
            dgAddStudentsToGroupData = clsGroup.getGroupsBySchool(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), schoolId);
        }

        async void addToGroup()
        {
            try
            {
                if (arrStudentImageIds.Count == 0) { MVVMMessageService.ShowMessage(errorMessages.SELECT_STUDENT); return; }
                if (selectedRowItem == null) { MVVMMessageService.ShowMessage("Please select a group."); return; }

                string GroupName = selectedRowItem.GroupName;
                string message = "You are about to add " + arrStudentImageIds.Count + " record(s) to the group named " + GroupName + ".\nContinue?";
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    isInProgress = true;
                    IsVisibleProcessBar = System.Windows.Visibility.Visible;

                    #region old code
                    //BackgroundWorker bw = new BackgroundWorker();
                    //bw.WorkerSupportsCancellation = true;
                    //bw.DoWork += bw_DoWork_AddToGroup;
                    //bw.RunWorkerCompleted += bw_RunWorkerCompleted_AddToGroup;
                    //bw.RunWorkerAsync();
                    #endregion

                    //cts = new CancellationTokenSource();


                    // The await operator suspends getRatingOfStudentImages.
                    //  - AccessTheWebAsync can't continue until getRatingOfImagesAsync is complete.
                    //  - Meanwhile, control returns to the caller of getRatingOfStudentImages.
                    //  - Control resumes here when getRatingOfImagesAsync is complete. 
                    //  - The await operator then retrieves the result from getRatingOfImagesAsync if method has any return type.
                    await addToGroupAsync();

                    addToGroupCompleted();
                    isSave = true; //For NUnitTesting
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
            if (_objWaitCursorViewModel != null)
                _objWaitCursorViewModel.Dispose();
            if (isInProgress)
            {
                isProgressCancelled = true;
                //cts.Cancel();
            }
            else
            {
                isSave = false;
                DialogResult = false;
            }
        }

        private void addNewGroup()
        {
            try
            {
                AddGroup _objAddGroup = new AddGroup(schoolId, clsSchool.defaultSchoolName);
                _objAddGroup.ShowDialog();
                if (((AddGroupViewModel)(_objAddGroup.DataContext)).isSave)
                    loadData();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        #endregion
    }
}
