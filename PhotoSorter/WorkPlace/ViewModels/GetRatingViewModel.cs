using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.IO;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace PhotoForce.WorkPlace
{
    public class GetRatingViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        int photoShootId = 0;
        string selectedGrid = "";
        public bool isSave = false;
        List<StudentImage> visibleData;
        bool isFromDashboard = false;
        public bool isContinue = false;
        bool isProgressCancelled = false;
        //BackgroundWorker worker;
        #endregion

        #region Properties
        string _getImageRatingMessage;
        bool _isReducedChecked;
        bool _isReducedVisible;
        string _windowTitle;
        bool _isGetRatingButtonsVisible;
        bool _isProgressbarButtonsVisible;
        int _maxValue;
        int _minValue;
        int _currentProgress;
        string _fileName;
        string _statusLabel;

        public string statusLabel
        {
            get { return _statusLabel; }
            set { _statusLabel = value; NotifyPropertyChanged("statusLabel"); }
        }
        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; NotifyPropertyChanged("fileName"); }
        }
        public int maxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; NotifyPropertyChanged("maxValue"); }
        }
        public int minValue
        {
            get { return _minValue; }
            set { _minValue = value; NotifyPropertyChanged("minValue"); }
        }
        public int currentProgress
        {
            get { return _currentProgress; }
            set { _currentProgress = value; NotifyPropertyChanged("currentProgress"); }
        }
        public bool isProgressbarButtonsVisible
        {
            get { return _isProgressbarButtonsVisible; }
            set { _isProgressbarButtonsVisible = value; NotifyPropertyChanged("isProgressbarButtonsVisible"); }
        }
        public bool isGetRatingButtonsVisible
        {
            get { return _isGetRatingButtonsVisible; }
            set { _isGetRatingButtonsVisible = value; NotifyPropertyChanged("isGetRatingButtonsVisible"); }
        }
        public string windowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; NotifyPropertyChanged("windowTitle"); }
        }
        public bool isReducedVisible
        {
            get { return _isReducedVisible; }
            set { _isReducedVisible = value; NotifyPropertyChanged("isReducedVisible"); }
        }
        public bool isReducedChecked
        {
            get { return _isReducedChecked; }
            set { _isReducedChecked = value; NotifyPropertyChanged("isReducedChecked"); }
        }
        public string getImageRatingMessage
        {
            get { return _getImageRatingMessage; }
            set { _getImageRatingMessage = value; NotifyPropertyChanged("getImageRatingMessage"); }
        }
        #endregion

        #region Constructors
        public GetRatingViewModel(int tempPhotoShootId, string tempSelctedGrid, int gridRowCount, List<StudentImage> tempVisibleData)
        {
            visibleData = new List<StudentImage>();

            windowTitle = "Get Image Rating";
            visibleData = tempVisibleData;
            photoShootId = tempPhotoShootId;
            selectedGrid = tempSelctedGrid;

            isReducedChecked = true; isReducedVisible = true; isGetRatingButtonsVisible = true;
            minValue = 0;

            string message = errorMessages.GET_RATING_PHOTOJOBANDSHOOT_SELECTED1_NEW + gridRowCount + errorMessages.GET_RATING_PHOTOJOBANDSHOOT_SELECTED2_NEW;
            getImageRatingMessage = message;
        }
        //Nothing Related to Get Rating ,But used to show the window when user trying to delete photoshoots
        //#Mohan
        public GetRatingViewModel(bool fromDashboard, string windowName)
        {
            //hashtag
            if (windowName == "Photoshoots")
            {
                windowTitle = "Delete Photoshoot(s)";
                getImageRatingMessage = "When you delete this photoshoot(s), it will also delete the student photo records in the grid below, and where the student photo records appear in orders.\nIt will also delete workflows related to it.\n\nAre you sure? ";
            }
            else if (windowName == "StudentImages")
            {
                windowTitle = "Delete Student Images";
                getImageRatingMessage = "When you delete this studentimage(s), it will also delete where the student photo records appear in orders.\n\nAre you sure? ";
            }
            else if (windowName == "GroupImages")
            {
                windowTitle = "Delete Group Images";
                getImageRatingMessage = "When you delete this groupimage(s), it will also delete where the student photo records appear in orders.\n\nAre you sure? ";
            }
            isReducedVisible = false; isReducedChecked = false;
            isGetRatingButtonsVisible = true;
            isFromDashboard = fromDashboard;


        }
        #endregion

        #region Commands
        public RelayCommand ContinueCommand
        {
            get
            {
                return new RelayCommand(getRatingOfStudentImages);
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
        async void getRatingOfStudentImages()
        {
            if (isFromDashboard)
            {
                isContinue = true;
                DialogResult = false;
            }
            else
            {
                
                isReducedVisible = false; isGetRatingButtonsVisible = false;
                isProgressbarButtonsVisible = true;

                #region Old code
                //worker = new BackgroundWorker();
                //worker.DoWork += delegate(object s, DoWorkEventArgs args)
                //{
                //    getRatingOfImages(s, args);
                //};

                ////our background worker support cancellation
                //worker.WorkerSupportsCancellation = true;

                //// Configure the function to run when completed
                //worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                //// Launch the worker
                //worker.RunWorkerAsync();
                #endregion


                // The await operator suspends getRatingOfStudentImages.
                //  - AccessTheWebAsync can't continue until getRatingOfImagesAsync is complete.
                //  - Meanwhile, control returns to the caller of getRatingOfStudentImages.
                //  - Control resumes here when getRatingOfImagesAsync is complete. 
                //  - The await operator then retrieves the result from getRatingOfImagesAsync if method has any return type.

                await getRatingOfImagesAsync();
                ratingCompleted();
            }
        }


        /// <summary>
        /// This worker_RunWorkerCompleted is called when the worker is finished.
        /// </summary>
        /// <param name="sender">The worker as Object, but it can be cast to a worker.</param>
        /// <param name="e">The RunWorkerCompletedEventArgs object.</param>
        //void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    DialogResult = false; isProgressbarButtonsVisible = false;
        //    MVVMMessageService.ShowMessage("Get rating of student images completed.");
        //}

        void ratingCompleted()
        {
            if (!isProgressCancelled)
            {
                MVVMMessageService.ShowMessage("Get rating of student images completed.");
            }
            DialogResult = false; isProgressbarButtonsVisible = false;
        }

        private void windowClose()
        {
            if (isProgressbarButtonsVisible)
            {
                isProgressCancelled = true;
            }
            else
            {
                DialogResult = false;
                isSave = false;
                isContinue = false;
            }
        }

        public bool isDirectoryContainFiles(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return false;
                return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Any();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return false;
            }
        }

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
        /// This method will get the image rating of selected photoshoot
        /// </summary>
        /// <returns></returns>
        async Task getRatingOfImagesAsync()
        {
            try
            {
                StudentImage _objStudentImage = new StudentImage();
                List<StudentImage> studentImageList;
                List<PhotoShoot> photoShootsList;

                #region Un-Used Code
                //There is no way to rate images based on photographyjob/schoolyear/season
                //Commented by Mohan Tangella
                //#Mohan
                //if (selectedGrid == "PhotographyJob")
                //{
                //    # region Rating For PhotoJob
                //    try
                //    {
                //        db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                //        photoShootsList = clsDashBoard.getPhotoShootDetailsById(db, photoShootId, 0);
                //        for (int i = 0; i < photoShootsList.Count; i++)
                //        {
                //            studentImageList = visibleData.Count == 0 ? clsDashBoard.getStudentImgDetailsByShootId(db, photoShootsList[i].PhotoShotID) : visibleData;

                //            for (int j = 0; j < studentImageList.Count; j++)
                //            {
                //                string imagePathExif = photoShootsList[i].ImageFolder + "\\" + studentImageList[j].ImageName;
                //                if (System.IO.File.Exists(imagePathExif))
                //                {
                //                    #region Old Code
                //                    FileStream fs = new FileStream(imagePathExif, FileMode.Open, FileAccess.Read);

                //                    BitmapFrame bitmapFrame = BitmapFrame.Create(fs, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                //                    BitmapMetadata bitmapMetadata = bitmapFrame.Metadata as BitmapMetadata;

                //                    if (bitmapMetadata != null)
                //                    {
                //                        if ((bitmapMetadata.Format.ToLower() == "jpg") || (bitmapMetadata.Format.ToLower() == "jpeg"))
                //                        {
                //                            string tempRating = Convert.ToString(bitmapMetadata.Rating);
                //                            _objStudentImage = clsDashBoard.updateStudentImage(db, studentImageList[j].ID);
                //                            _objStudentImage.Rating = tempRating;
                //                            if (_objStudentImage != null)
                //                                db.SubmitChanges();
                //                        }
                //                    }
                //                    #endregion
                //                }
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        clsStatic.WriteExceptionLogXML(ex);
                //        MVVMMessageService.ShowMessage(ex.Message);
                //    }
                //    # endregion
                //}
                //else
                //{
                #endregion

                # region Rating For Photoshoot
                try
                {
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    photoShootsList = clsDashBoard.getPhotoShootDetailsById(db, 0, photoShootId);
                    string imgFolderName = photoShootsList.First().ImageFolder;
                    string reducedimgpath = imgFolderName + "\\" + "_reduced";
                    if (isReducedChecked == true)
                    {
                        if (Directory.Exists(reducedimgpath))
                        {
                            if (isDirectoryContainFiles(reducedimgpath))
                            {
                                imgFolderName = imgFolderName + "\\" + "_reduced";
                            }
                            else
                            {
                                MVVMMessageService.ShowMessage(errorMessages.UNABLE_TO_GET_PHOTO_RATING); return;
                            }
                        }
                        else
                        {
                            MVVMMessageService.ShowMessage(errorMessages.UNABLE_TO_GET_PHOTO_RATING); return;
                        }
                    }

                    studentImageList = visibleData.Count == 0 ? clsDashBoard.getStudentImgDetailsByShootId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), photoShootId) : visibleData;
                    statusLabel = "Get Rating Of...";
                    maxValue = studentImageList.Count;

                    await Task.Run(() =>
                    {
                        for (int j = 0; j < studentImageList.Count; j++)
                        {

                            if (isProgressCancelled)
                            {
                                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                                string message = "Are you sure, you want to cancel the operation?";
                                if (MVVMMessageService.ShowMessage(message, "Confirmation", buttons, icon) == System.Windows.MessageBoxResult.Yes)
                                {
                                    DialogResult = false;
                                    return;
                                }
                                isProgressCancelled = false;
                            }

                            string imageName = studentImageList[j].ImageName;
                            string imagePathExif = imgFolderName + "\\" + imageName;
                            if (System.IO.File.Exists(imagePathExif))
                            {
                                currentProgress++;
                                fileName = imageName + " (" + j + " of " + studentImageList.Count + " )";
                                #region Old Code
                                FileStream fs = new FileStream(imagePathExif, FileMode.Open, FileAccess.Read);

                                BitmapFrame bitmapFrame = BitmapFrame.Create(fs, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                                BitmapMetadata bitmapMetadata = bitmapFrame.Metadata as BitmapMetadata;

                                if (bitmapMetadata != null)
                                {
                                    if ((bitmapMetadata.Format.ToLower() == "jpg") || (bitmapMetadata.Format.ToLower() == "jpeg"))
                                    {
                                        string tempRating = Convert.ToString(bitmapMetadata.Rating);
                                        int updateRating = clsDashBoard.updateRating(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempRating, Convert.ToInt32(studentImageList[j].ID));
                                        isSave = true;
                                    }
                                }
                                #endregion
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    clsStatic.WriteExceptionLogXML(ex);
                    MVVMMessageService.ShowMessage(ex.Message);
                }
                # endregion
                //}

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
