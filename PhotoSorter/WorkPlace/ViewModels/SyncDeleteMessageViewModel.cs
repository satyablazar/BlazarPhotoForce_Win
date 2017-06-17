using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.IO;
using System.ComponentModel;

namespace PhotoForce.WorkPlace
{
    public class SyncDeleteMessageViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        string imageId = "";
        Boolean errorExist;
        IList<StudentImage> _objStudentImage;
        #endregion

        #region Properties
        private int _minValue;
        private int _maxValue;
        private int _currentProgress;

        public int currentProgress
        {
            get { return _currentProgress; }
            set { _currentProgress = value; NotifyPropertyChanged("currentProgress"); }
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
        #endregion

        #region Constructors
        public SyncDeleteMessageViewModel()
        {
            ThreadPoolMethod();
        }
        #endregion

        #region Methods
        async void ThreadPoolMethod()
        {
            #region Old code
            //BackgroundWorker worker = new BackgroundWorker();
            //worker.DoWork += worker_DoWork;
            //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            //worker.RunWorkerAsync();
            #endregion
            //cts = new CancellationTokenSource();

            // The await operator suspends getRatingOfStudentImages.
            //  - AccessTheWebAsync can't continue until getRatingOfImagesAsync is complete.
            //  - Meanwhile, control returns to the caller of getRatingOfStudentImages.
            //  - Control resumes here when getRatingOfImagesAsync is complete. 
            //  - The await operator then retrieves the result from asyncDeleteMessage if method has any return type.
            await asyncDeleteMessage();
            asyncDeleteMessageCompleted();
        }

        #region Old code
        //void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (errorExist)
        //    {
        //        string message = errorMessages.DELETE_PHOTOS_ERROR;
        //        MVVMMessageService.ShowMessage(message);
        //    }
        //    else
        //    {
        //        string message = errorMessages.DELETE_PHOTOS_SUCCESSFUL_MSG;
        //        MVVMMessageService.ShowMessage(message);
        //    }
        //    DialogResult = false;
        //}

        //void worker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    _objStudentImage = clsDashBoard.getDeletedStudentimg(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
        //    minValue = 0;
        //    maxValue = _objStudentImage.Count;
        //    //Stores the value of the ProgressBar
        //    currentProgress = 0;

        //    foreach (StudentImage tempStuImage in _objStudentImage)
        //    {
        //        try
        //        {
        //            currentProgress ++;
        //            string tempImgName = tempStuImage.ImageName;
        //            string path = tempStuImage.PhotoShoot.ImageFolder;
        //            imageId = imageId + tempStuImage.ID + ",";
        //            if (File.Exists(path + "\\" + tempImgName))
        //            {
        //                File.Delete(path + "\\" + tempImgName);
        //            }
        //            imageId = imageId.Substring(0, imageId.Length - 1);
        //            int delStuID = clsDashBoard.deletestudentimage(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempStuImage.ID);
        //        }
        //        catch (Exception ex)
        //        {
        //            errorExist = true;
        //            clsStatic.WriteExceptionLogXML(ex);
        //        }
        //    }
            
        //}
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
        /// </summary>
        /// <returns></returns>
        async Task asyncDeleteMessage()
        {
            _objStudentImage = clsDashBoard.getDeletedStudentimg(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
            minValue = 0;
            maxValue = _objStudentImage.Count;
            //Stores the value of the ProgressBar
            currentProgress = 0;
            await Task.Run(() =>
                {
                    foreach (StudentImage tempStuImage in _objStudentImage)
                    {
                        try
                        {
                            currentProgress++;
                            string tempImgName = tempStuImage.ImageName;
                            string path = tempStuImage.PhotoShoot.ImageFolder;
                            imageId = imageId + tempStuImage.ID + ",";
                            if (File.Exists(path + "\\" + tempImgName))
                            {
                                File.Delete(path + "\\" + tempImgName);
                            }
                            imageId = imageId.Substring(0, imageId.Length - 1);
                            int delStuID = clsDashBoard.deletestudentimage(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempStuImage.ID);
                        }
                        catch (Exception ex)
                        {
                            errorExist = true;
                            clsStatic.WriteExceptionLogXML(ex);
                        }
                    }
                });
        }
        void asyncDeleteMessageCompleted()
        {
            if (errorExist)
            {
                string message = errorMessages.DELETE_PHOTOS_ERROR;
                MVVMMessageService.ShowMessage(message);
            }
            else
            {
                string message = errorMessages.DELETE_PHOTOS_SUCCESSFUL_MSG;
                MVVMMessageService.ShowMessage(message);
            }
            DialogResult = false;
        }
        #endregion
    }
}
