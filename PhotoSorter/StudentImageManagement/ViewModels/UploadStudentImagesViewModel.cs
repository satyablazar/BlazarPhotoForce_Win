using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using SoftekBarcodeLib3;
using System.IO;
using PhotoForce.Error_Management;
using Ookii.Dialogs.Wpf;
using PhotoForce.PhotographyJobManagement;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Transactions;
using System.Collections.ObjectModel;
using PhotoForce.WorkflowManagement;

namespace PhotoForce.StudentImageManagement
{
    public class UploadStudentImagesViewModel : ViewModelBase
    {
        #region Initialization
        int schoolId = 0;
        int maxImportId = 0;
        int GetPhotoMax = 0;
        string SchoolName = "";
        string Fullpath = "";
        string tempFullpath = "";
        string qrDecode = "";
        string ReducedImagesPath = "";
        int lastStudentId = 0;
        string lastStudentPassword = "";
        string tempFirstName = "";
        int studentIdForDummy = 0;
        int IDForDummy = 0;
        clsStatic objstatic = new clsStatic();
        Student _objDummyStudent = new Student();
        StudentImage _objStudentImage = new StudentImage();
        BarcodeReader barcode;
        int nBarCode = 0;
        string ImageName;
        internal bool isSave = false;
        //BackgroundWorker worker = new BackgroundWorker();
        PhotoSorterDBModelDataContext db;
        bool isProgressCancelled = false;
        public PhotoShoot _objPhotoShoot = new PhotoShoot();
        WaitCursorViewModel _objWaitCursorViewModel;
        int? photoJobSelectedItem;
        PhotoShoot _tempSelectedPhotoShoot;
        #endregion

        #region Properties
        // private IEnumerable<PhotographyJob> _cbPhotographyJobData;
        string _lblSchoolName;
        bool _isReducedImagesChecked;
        // PhotographyJob _photoJobSelectedItem;
        //string _shootName;
        private string _photoShootPath;
        System.Windows.Visibility _progressVisibility;
        bool _isInProgress;
        int _currentProgress;
        int _minValue;
        int _maxValue;
        bool _isControlsDisableAfterUpload;
        string _selectedPhotoShoot;

        public string selectedPhotoShoot
        {
            get { return _selectedPhotoShoot; }
            set { _selectedPhotoShoot = value; NotifyPropertyChanged("selectedPhotoShoot"); }
        }
        public bool isControlsDisableAfterUpload
        {
            get { return _isControlsDisableAfterUpload; }
            set { _isControlsDisableAfterUpload = value; NotifyPropertyChanged("isControlsDisableAfterUpload"); }
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
        public bool isInProgress
        {
            get { return _isInProgress; }
            set { _isInProgress = value; NotifyPropertyChanged("isInProgress"); }
        }
        public System.Windows.Visibility progressVisibility
        {
            get { return _progressVisibility; }
            set { _progressVisibility = value; NotifyPropertyChanged("progressVisibility"); }
        }
        public string photoShootPath
        {
            get { return _photoShootPath; }
            set { _photoShootPath = value; NotifyPropertyChanged(); }//"photoShootPath"
        }
        //public string shootName
        //{
        //    get { return _shootName; }
        //    set { _shootName = value; NotifyPropertyChanged("shootName"); }
        //}
        //public PhotographyJob photoJobSelectedItem
        //{
        //    get { return _photoJobSelectedItem; }
        //    set { _photoJobSelectedItem = value; NotifyPropertyChanged("photoJobSelectedItem"); }
        //}
        public bool isReducedImagesChecked
        {
            get { return _isReducedImagesChecked; }
            set { _isReducedImagesChecked = value; NotifyPropertyChanged("isReducedImagesChecked"); }
        }
        public string lblSchoolName
        {
            get { return _lblSchoolName; }
            set { _lblSchoolName = value; NotifyPropertyChanged("lblSchoolName"); }
        }
        //public IEnumerable<PhotographyJob> cbPhotographyJobData
        //{
        //    get { return _cbPhotographyJobData; }
        //    set { _cbPhotographyJobData = value; NotifyPropertyChanged("cbPhotographyJobData"); }
        //}
        #endregion

        #region Constructor
        public UploadStudentImagesViewModel(string tempSchoolName, PhotoShoot selectedPhotoShoot)
        {
            progressVisibility = System.Windows.Visibility.Collapsed;
            isReducedImagesChecked = true;  //while uploading QR code reads from Reduced path
            SchoolName = tempSchoolName;
            schoolId = clsSchool.defaultSchoolId;
            _tempSelectedPhotoShoot = selectedPhotoShoot;
            bindPhotoJobs(selectedPhotoShoot);
            bindData();
            initializeSoftekBarcode();
            isInProgress = true; minValue = 0; currentProgress = 0; isControlsDisableAfterUpload = true;
        }
        #endregion

        #region Commands
        //public RelayCommand NewSchoolYearCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(newSchoolYear);
        //    }
        //}
        public RelayCommand BrowsePhotoShootFolderCommand
        {
            get
            {
                return new RelayCommand(BrowsePhotoShootFolder);
            }
        }
        public RelayCommand UploadImagesCommand
        {
            get
            {
                return new RelayCommand(uploadImages);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        //public RelayCommand CreateNewPhotoShootCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(createNewPhotoShoot);
        //    }
        //}
        #endregion

        #region Methods
        //private void createNewPhotoShoot()
        //{
        //    AddEditWorkflowItems _objAddEditWorkflowItems = new AddEditWorkflowItems();
        //    _objAddEditWorkflowItems.ShowDialog();
        //    if (((AddEditWorkflowItemsViewModel)(_objAddEditWorkflowItems.DataContext)).isSave)
        //    {
        //        photoShootData = new ObservableCollection<PhotoShoot>(clsWorkflows.getWorkflowPhotoShoots(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), schoolId));
        //    }
        //}

        private void bindPhotoJobs(PhotoShoot tempPhotoShoot)
        {
            selectedPhotoShoot = tempPhotoShoot.PhotoShotName;
        }

        private void initializeSoftekBarcode()
        {
            #region Softek QR Reader

            // For the purposes of this demo we first give a path to the installation folder
            // and the class adds either x86 or x64 to the end and tries to load the other dll files.
            // If that fails (perhaps this project has been moved) then we give no path and the class
            // assumes that the dll files will be somewhere on the PATH.
            try
            {
                barcode = new BarcodeReader();
            }
            catch (System.DllNotFoundException ex)
            {
                barcode = new SoftekBarcodeLib3.BarcodeReader();
                clsStatic.WriteExceptionLogXML(ex);
            }


            // Enter your license key here
            // You can get a trial license key from sales@bardecode.com
            //barcode.LicenseKey = "31AFD14A54FD84FFB23A384B884580CA";  //trial version 
            //barcode.LicenseKey = "KQV5AC424IJB092C97SQMVC4MM5MH5DB";    //7.5.1 key
            barcode.LicenseKey = "P7BAU88X6HEE3M8GP03XEYAM1ZSWN2DB";    //7.6.1
            barcode.ReadQRCode = true;

            // Databar Options is a mask that controls which type of databar barcodes will be read and whether or not
            // the software will look for a quiet zone around the barcode.
            barcode.DatabarOptions = 255;



            // If you want to read more than one barcode then set Multiple Read to 1
            // Setting MutlipleRead to False will make the recognition faster
            barcode.MultipleRead = true;

            // If you know the max number of barcodes for a single page then increase speed by setting MaxBarcodesPerPage
            barcode.MaxBarcodesPerPage = 0;

            // In certain conditions (MultipleRead = false or MaxBarcodesPerPage = 1) the SDK can make fast scan of an image before performing the normal scan. This is useful if only 1 bar code is expected per page.
            barcode.UseFastScan = true;

            // You may need to set a small quiet zone if your barcodes are close to text and pictures in the image.
            // A value of zero uses the default.
            barcode.QuietZoneSize = 0;

            // LineJump controls the frequency at which scan lines in an image are sampled.
            // The default is 9 - decrease this for difficult barcodes.
            barcode.LineJump = 1;

            // Set the direction that the barcode reader should scan for barcodes
            // The value is a mask where 1 = Left to Right, 2 = Top to Bottom, 4 = Right To Left, 8 = Bottom to Top
            barcode.ScanDirection = 15;

            // SkewTolerance controls the angle of skew that the barcode toolkit will tolerate. By default
            // the toolkit checks for barcodes along horizontal and vertical lines in an image. This works 
            // OK for most barcodes because even at an angle it is possible to pass a line through the entire
            // length. SkewTolerance can range from 0 to 5 and allows for barcodes skewed to an angle of 45
            // degrees.
            barcode.SkewTolerance = 0;

            // Read most skewed linear barcodes without the need to set SkewTolerance. Currently applies to Codabar, Code 25, Code 39 and Code 128 barcodes only.
            barcode.SkewedLinear = true;

            // Read most skewed datamatrix barcodes without the need to set SkewTolerance
            barcode.SkewedDatamatrix = true;


            // ColorProcessingLevel controls how much time the toolkit will searching a color image for a barcode.
            // The default value is 2 and the range of values is 0 to 5. If ColorThreshold is non-zero then 
            // ColorProcessingLevel is effectively set to 0.
            barcode.ColorProcessingLevel = 2;

            // MaxLength and MinLength can be used to specify the number of characters you expect to
            // find in a barcode. This can be useful to increase accuracy or if you wish to ignore some
            // barcodes in an image.
            barcode.MinLength = 4;
            barcode.MaxLength = 999;

            // MedianFilter is a useful way to clean up higher resolution images where the black bars contain white dots
            // and the spaces contain black dots. It does not work if the space between bars is only 1 pixel wide.
            barcode.MedianFilter = false;

            // ReportUnreadBarcodes can be used to warn of the presence of a barcode on a page that the SDK has not been able to decode.
            // It currently has the following important limitations:
            // 1. An unread linear barcode will only be reported if no other barcode was found in the same page.
            // 2. The height of the area for an unread linear barcode will only cover a portion of the barcode.
            // 3. Only 2-D barcodes that fail to error correct will be reported.
            // 4. The barcode type and value will both be set to UNREAD for all unread barcodes.
            // 5. The reporting of unread linear barcodes takes no account of settings for individual barcode types. For example, if ReadCode39 is True and 
            // an image contains a single Code 39 barcode then this will be reported as an unread barcode.
            // 6. 2-D barcodes are only reported as unread if the correct barcode types have been enabled.
            // 7. Not all unread barcodes will be detected. 
            //
            // The value is a mask with the following values: 1 = linear barcodes, 2 = Datamatrix, 4 = QR-Code, 8 = PDF-417
            barcode.ReportUnreadBarcodes = 0;

            // Time out for reading a barcode from a page in ms. Note that this does not include the time to load the page.
            // 0 means no time out.
            barcode.TimeOut = 5000;

            // Flags for handling PDF files
            // PdfImageOnly defaults to true and indicates that the PDF documents are simple images.
            barcode.PdfImageOnly = true;

            // The PdfImageExtractOptions mask controls how images are removed from PDF documents (when PdfImageOnly is True)
            // 1 = Enable fast extraction
            // 2 = Auto-invert black and white images
            // 4 = Auto-merge strips
            // 8 = Auto-correct photometric values in black and white images
            barcode.PdfImageExtractOptions = 15;

            // The PdfImageRasterOptions mask controls how images are rasterized when PdfImageOnly is false or when image extraction fails
            // 1 = Use alternative pdf-to-tif conversion function
            // 2 = Always use pdf-to-tif conversion rather than loading the rasterized image directly into memory
            barcode.PdfImageRasterOptions = 0;

            // PdfDpi and PdfBpp control what sort of image the PDF document is rasterized into
            barcode.PdfDpi = 300;
            barcode.PdfBpp = 8;


            #endregion
        }

        private void bindData()
        {
            lblSchoolName = SchoolName;
        }

        //#region IPTC Code
        //private void insertIptc(string password, int studentID)
        //{
        //    string[] insertIPTC = new string[2];
        //    if (string.IsNullOrEmpty(password))
        //    {
        //        ArrayList arrstudID = new ArrayList();
        //        if (!arrstudID.Contains(studentID))
        //        {
        //            arrstudID.Add(studentID);
        //        }
        //        //Generate password
        //        GeneratePassword _GeneratePassword = new GeneratePassword(arrstudID);
        //        password = _GeneratePassword.generateStudentPassword();
        //    }


        //    string original_img_path = tempFullpath + "\\" + ImageName;
        //    string reduced_img_path = ReducedImagesPath + "\\" + ImageName;


        //    if (File.Exists(original_img_path))
        //    {
        //        if (File.Exists(reduced_img_path))
        //        {
        //            insertIPTC = new string[] { original_img_path, reduced_img_path };
        //        }
        //        else
        //            insertIPTC = new string[] { original_img_path };
        //        foreach (string img in insertIPTC)
        //        {
        //            #region New IPTC Code
        //            try
        //            {
        //                var bitmap = new Aurigma.GraphicsMill.Bitmap(img);

        //                var settings = new Aurigma.GraphicsMill.Codecs.JpegSettings();

        //                var iptc = new Aurigma.GraphicsMill.Codecs.IptcDictionary();
        //                string keyWords = studentID + "," + password; //overrides the Keywords field,if it is already contains data
        //                iptc[Aurigma.GraphicsMill.Codecs.IptcDictionary.Keyword] = keyWords;
        //                settings.Iptc = iptc;
        //                bitmap.Save(img, settings);
        //            }
        //            catch (Exception ex)
        //            {
        //                clsStatic.WriteExceptionLogXML(ex);
        //            }

        //            #endregion
        //        }
        //    }
        //#endregion

        //}

        #region applying 1-star
        private void DefaultRating(string original_img_path, string reduced_img_path)
        {
            try
            {
                string tempPath = "";
                bool hasMSRatingID = false;
                string[] applyRating = new string[2];

                if (isReducedImagesChecked == true)
                {
                    applyRating = new string[] { original_img_path, reduced_img_path };
                }
                else
                {
                    applyRating = new string[] { original_img_path };
                }

                foreach (string img in applyRating)
                {
                    tempPath = img.ToLower().Replace(".jpg", "_1.jpg");

                    using (var losslessJpeg = new Aurigma.GraphicsMill.Codecs.LosslessJpeg(img))
                    {
                        // IPTC
                        if (losslessJpeg.Iptc == null)
                        {
                            losslessJpeg.Iptc = new Aurigma.GraphicsMill.Codecs.IptcDictionary();
                        }
                        // EXIF
                        if (losslessJpeg.Exif == null)
                        {
                            losslessJpeg.Exif = new Aurigma.GraphicsMill.Codecs.ExifDictionary();
                        }

                        // XMP
                        var xmp = new Aurigma.GraphicsMill.Codecs.XmpData();
                        //if (losslessJpeg.Xmp != null)
                        //{
                        //    xmp.Load(losslessJpeg.Xmp);
                        //}
                        //Remove the XmpRating tag if it is already exists
                        if (xmp.Contains(Aurigma.GraphicsMill.Codecs.XmpTagNames.XmpRating))
                        {
                            xmp.Remove(Aurigma.GraphicsMill.Codecs.XmpTagNames.XmpRating);
                        }
                        var msRatingId = "MicrosoftPhoto:Rating";

                        if (xmp.Contains(msRatingId))
                        {
                            hasMSRatingID = true;
                            xmp.Remove(msRatingId);
                        }
                        //Add the XmpRating tag (simple value)
                        var node = new Aurigma.GraphicsMill.Codecs.XmpValueNode(
                            Aurigma.GraphicsMill.Codecs.XmpNodeType.SimpleProperty, "1",
                            Aurigma.GraphicsMill.Codecs.XmpTagNames.XmpRating);
                        xmp.AddNode(node);

                        if (hasMSRatingID)
                        {
                            var node2 = new Aurigma.GraphicsMill.Codecs.XmpValueNode(
                                       Aurigma.GraphicsMill.Codecs.XmpNodeType.SimpleProperty, "1",
                                       msRatingId);
                            xmp.AddNode(node2);
                        }

                        losslessJpeg.Xmp = xmp.Save();

                        losslessJpeg.Write(tempPath);
                    }

                    System.IO.File.Delete(img);
                    System.IO.File.Move(tempPath, img);
                }

            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        #endregion

        #region update star rating in DB
        private string getRatingForImages(string imagePath)
        {
            string tempRating = 0.ToString();
            try
            {
                #region Old Code
                FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

                BitmapFrame bitmapFrame = BitmapFrame.Create(fs, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                BitmapMetadata bitmapMetadata = bitmapFrame.Metadata as BitmapMetadata;

                #endregion

                if (bitmapMetadata != null)
                {
                    if ((bitmapMetadata.Format.ToLower() == "jpg") || (bitmapMetadata.Format.ToLower() == "jpeg"))
                    {
                        tempRating = Convert.ToString(bitmapMetadata.Rating);
                    }
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

            return tempRating;
        }
        #endregion

        private void BrowsePhotoShootFolder()
        {
            try
            {
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                var res = dlg.ShowDialog();
                if (res != false)
                    photoShootPath = dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
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
        /// import all the student images into photoshoot
        /// </summary>
        /// <returns></returns>
        async Task importStudentImagesAsync()
        {
            int diffSchool = 0;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            progressVisibility = System.Windows.Visibility.Visible;
            _objWaitCursorViewModel = new WaitCursorViewModel();
            var FilesPath = Directory.EnumerateFiles(Fullpath, "*.*", SearchOption.TopDirectoryOnly).Where(k => k.ToLower().EndsWith(".jpg"));

            Student _objStudent = new Student();
            int count = 0;
            maxValue = FilesPath.Count();

            await Task.Run(() =>
            {
                foreach (String FilePath in FilesPath)
                {
                    try
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

                        currentProgress++;
                        ImageName = FilePath.Substring(FilePath.LastIndexOf('\\') + 1);

                        #region creating reduced image
                        //string imageExtension = FilePath.Substring(FilePath.LastIndexOf('.') + 1);
                        //if (imageExtension.ToLower() == "jpg")
                        //    graphicMillerResize(FilePath, ReducedImagesPath + "\\" + ImageName);    //reducing original image

                        #endregion

                        string qrResult = "";
                        if (isReducedImagesChecked == true)
                        {
                            nBarCode = barcode.ScanBarCode(ReducedImagesPath + "\\" + ImageName);   //reading qr code from reduce image
                        }
                        else
                            nBarCode = barcode.ScanBarCode(FilePath);   //reading qr code from original image

                        if (nBarCode <= -6)
                        {
                            MVVMMessageService.ShowMessage("License key error: either an evaluation key has expired or the license key is not valid for processing pdf documents\r\n");
                            qrDecode = "";
                        }
                        else if (nBarCode < 0)
                        {
                            qrDecode = "";
                        }
                        else if (nBarCode == 0)
                        {
                            qrDecode = "";
                        }
                        else
                        {
                            for (int i = 1; i <= nBarCode; i++)
                            {
                                qrResult = barcode.GetBarString(i);
                            }
                        }

                        // No clean up necessary - all resources freed each time you call ScanBarCode or 
                        // when the object is destroyed.

                        if (!string.IsNullOrEmpty(qrResult))
                        {
                            qrDecode = (qrResult).Replace("MW6 Demo", "");
                            DefaultRating(FilePath, ReducedImagesPath + "\\" + ImageName);        //applying 1-star for QR code images
                        }
                        try
                        {
                            if (qrDecode != "")
                            {
                                if (lastStudentId != Convert.ToInt32(qrDecode))
                                    _objStudent = clsStudent.updateStudent(db, Convert.ToInt32(qrDecode));

                                if (diffSchool == 0 && _objStudent != null)
                                {
                                    if (_objStudent.StudentImport.SchoolID != clsSchool.defaultSchoolId)
                                    {
                                        MVVMMessageService.ShowMessage("Photshoot contains images belongs different school. Please check and import again.");
                                        diffSchool++;
                                    }
                                }
                            }

                            if (_objStudent == null) { qrDecode = ""; }

                        }
                        catch (Exception)
                        {
                            //if student id not found in DB, we can catch here and assign to dummy student
                            qrDecode = "";
                        }

                        if (string.IsNullOrEmpty(qrDecode))
                        {
                            #region TransactionScope Rollback
                            using (TransactionScope ts = new TransactionScope())
                            {
                                using (PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString))
                                {
                                    #region //condition A
                                    StudentImage _objstudimg = new StudentImage();
                                    if (lastStudentId != 0)
                                    {
                                        studentIdForDummy = lastStudentId;
                                        _objstudimg.FirstName = tempFirstName;
                                    }
                                    else
                                    {
                                        if (count == 0)
                                        {
                                            #region cresating a dummy student
                                            string checkForFirstName = clsStudent.getStudentFirstName(dbb, "Manual Import");

                                            string lastIndexVal = "";
                                            if (!string.IsNullOrEmpty(checkForFirstName))
                                            {
                                                lastIndexVal = checkForFirstName.Substring(checkForFirstName.Length - 1);
                                            }
                                            //string lastIndexVal = checkForFirstName.Substring(checkForFirstName.Length - 1);
                                            Int32 a;
                                            if (Int32.TryParse(lastIndexVal, out a))
                                            {
                                                int nameindex = Convert.ToInt32(lastIndexVal) + 1;
                                                _objDummyStudent.FirstName = "Manual Import " + nameindex + "";
                                            }
                                            else
                                                _objDummyStudent.FirstName = "Manual Import 1";

                                            _objDummyStudent.Lastname = SchoolName;
                                            _objDummyStudent.StudentImportID = maxImportId;
                                            //_objDummyStudent.SchoolYear = photoJobSelectedItem.JobName;   //have to use photogaraphy job fk and not use this column //Mohan Sept 3rd 2015
                                            _objDummyStudent.PhotographyJobID = photoJobSelectedItem;
                                            _objDummyStudent.CreatedOn = DateTime.Now;
                                            _objDummyStudent.RecordStatus = true;
                                            _objDummyStudent.IsStudent = "Student";
                                            if (_objDummyStudent != null)
                                            {
                                                dbb.Students.InsertOnSubmit(_objDummyStudent);
                                                dbb.SubmitChanges();

                                                studentIdForDummy = clsStudent.getMaxStudentId(dbb);
                                                IDForDummy = studentIdForDummy;

                                                # region Need to update StudentIDForDummy into StudentID
                                                int result = clsStudent.updateStudentId(dbb, studentIdForDummy);
                                                #  endregion

                                            }
                                            count++;
                                            #endregion
                                        }
                                        _objstudimg.FirstName = _objDummyStudent.FirstName;
                                        lastStudentPassword = "";
                                    }
                                    #endregion

                                    #region Condition B
                                    _objstudimg.ImageName = Convert.ToString(ImageName);
                                    _objstudimg.StudentIDPK = studentIdForDummy;
                                    string[] imgnumber = _objstudimg.ImageName.Substring(_objstudimg.ImageName.LastIndexOf('_') + 1).Split('.');
                                    _objstudimg.ImageNumber = imgnumber[0];
                                    _objstudimg.RecordStatus = "Active";
                                    _objstudimg.SchoolID = schoolId;
                                    _objstudimg.yearbook = false;
                                    _objstudimg.Admincd = false;
                                    _objstudimg.Rating = getRatingForImages(FilePath);  //star rating
                                    _objstudimg.PhotoShootID = GetPhotoMax;
                                    dbb.StudentImages.InsertOnSubmit(_objstudimg);
                                    dbb.SubmitChanges();
                                    #endregion
                                }
                                ts.Complete();
                            }
                            #endregion
                            #region Insert IPTC data
                            //insertIptc(lastStudentPassword, StudentIDForDummy);
                            #endregion
                        }
                        else
                        {
                            lastStudentId = Convert.ToInt32(qrDecode);
                            lastStudentPassword = _objStudent.Password;
                            StudentImage _objstudimg = new StudentImage();
                            _objstudimg.ImageName = ImageName;
                            _objstudimg.FirstName = _objStudent.FirstName;
                            tempFirstName = _objStudent.FirstName;
                            _objstudimg.Teacher = _objStudent.Teacher;
                            _objstudimg.Lastname = _objStudent.Lastname;
                            _objstudimg.Grade = _objStudent.Grade;
                            _objstudimg.StudentIDPK = Convert.ToInt32(_objStudent.ID);
                            string[] imgnumber = _objstudimg.ImageName.Substring(_objstudimg.ImageName.LastIndexOf('_') + 1).Split('.');
                            _objstudimg.ImageNumber = imgnumber[0];
                            _objstudimg.RecordStatus = "Active";
                            _objstudimg.SchoolID = schoolId;
                            _objstudimg.yearbook = false;
                            _objstudimg.Admincd = false;
                            _objstudimg.Rating = getRatingForImages(FilePath);   //update star rating in db .
                            _objstudimg.PhotoShootID = GetPhotoMax;
                            db.StudentImages.InsertOnSubmit(_objstudimg);

                            #region Insert IPTC data
                            //insertIptc(_objStudent.Password, _objStudent.ID);
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        //throw new ImportImagesException("fuck this shit");
                        clsStatic.WriteExceptionLogXML(ex);
                        clsErrorLog objError = new clsErrorLog();
                        objError.Source = "Student Images Import";
                        objError.MethodName = "Import Student";
                        objError.Message = ex.Message;
                        clsStatic.WriteErrorLogImportImages(objError, objstatic.ErrorLogXML);

                        System.Windows.MessageBoxButton buttonss = System.Windows.MessageBoxButton.YesNo;
                        System.Windows.MessageBoxImage iconn = System.Windows.MessageBoxImage.Question;
                        string messagee = errorMessages.FILE_IMPORTED_ERRORS1 + _tempSelectedPhotoShoot.PhotoShotName + errorMessages.FILE_IMPORTED_ERRORS2;
                        if (MVVMMessageService.ShowMessage(messagee, "Confirmation", buttonss, iconn) == System.Windows.MessageBoxResult.Yes)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                ShowErrors objShowErrors = new ShowErrors("Import Images");
                                objShowErrors.ShowDialog();
                            });

                        }
                        DialogResult = false;
                        isSave = true;
                        return;
                    }
                }
            });
            db.SubmitChanges();
            isSave = true;
            progressVisibility = System.Windows.Visibility.Visible;
            GC.Collect();
            MVVMMessageService.ShowMessage(errorMessages.QR_IMAGES_IMPORTED_SUCCESSFULLY + _tempSelectedPhotoShoot.PhotoShotName);
            DialogResult = false;
        }

        private void windowClose()
        {
            if (!isInProgress)
            {
                isProgressCancelled = true;
            }
            else
            {
                DialogResult = false;
                isSave = false;
            }
        }

        #region ProgressBar
        /// <summary>
        /// This starts the counter as a background process.
        /// </summary>
        async void uploadImages()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            // If we are in progress already, don't do anything
            if (!isInProgress)
                return;

            isInProgress = false;

            // Configure the function that will run when started
            //worker.DoWork += delegate(object s, DoWorkEventArgs args)
            //{
            isControlsDisableAfterUpload = false;
            if (selectedPhotoShoot != null && photoShootPath != "")
            {
                _objPhotoShoot = (from psh in db.PhotoShoots where psh.PhotoShotID == _tempSelectedPhotoShoot.PhotoShotID select psh).FirstOrDefault();
                Fullpath = photoShootPath;
                tempFullpath = Fullpath;
                ReducedImagesPath = Fullpath + "\\_reduced";
                if (isReducedImagesChecked == true)
                {
                    //Fullpath = ReducedImagesPath;
                    if (!Directory.Exists(ReducedImagesPath))
                    {
                        Directory.CreateDirectory(ReducedImagesPath);
                        MVVMMessageService.ShowMessage("No reduced images found in " + Fullpath + ". Generate the reduced images first, then try this operation again.");
                        isControlsDisableAfterUpload = true;
                        isInProgress = true;
                        return;
                    }
                    else
                    {
                        var fileCount = (from file in Directory.EnumerateFiles(ReducedImagesPath, "*.jpg", SearchOption.TopDirectoryOnly)
                                         select file).Count();
                        if (fileCount == 0)
                        {
                            MVVMMessageService.ShowMessage("No reduced images found in " + ReducedImagesPath + ". Generate the reduced images first, then try this operation again.");
                            isControlsDisableAfterUpload = true;
                            isInProgress = true;
                            return;
                        }

                        //Fullpath = ReducedImagesPath;
                    }
                }

                int Getpathcount = clsDashBoard.GetCountForPath(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Fullpath);
                if (Getpathcount == 0)
                {
                    //PhotoSorterDBModelDataContext db1 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    //_objPhotoShoot.PhotoShotName = shootName;
                    //_objPhotoShoot.PhotoShotDate = DateTime.Now;
                    //_objPhotoShoot.PhotographyjobID = photoJobSelectedItem.ID;
                    _objPhotoShoot.ImageFolder = photoShootPath;
                    _objPhotoShoot.isReduced = true;

                    if (_objPhotoShoot != null)
                    {
                        //db1.PhotoShoots.InsertOnSubmit(_objPhotoShoot);
                        db.SubmitChanges();
                        photoJobSelectedItem = _objPhotoShoot.PhotographyjobID;
                        //GetPhotoMax = clsDashBoard.getMaxPhotoShootId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
                        GetPhotoMax = _tempSelectedPhotoShoot.PhotoShotID;
                        try
                        {
                            # region Insert New Student Import ID
                            //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                            StudentImport _objstudentimport = new StudentImport();
                            _objstudentimport.SchoolID = schoolId;
                            _objstudentimport.Description = "from manual";
                            _objstudentimport.Notes = "from manual";
                            _objstudentimport.CreatedOn = DateTime.Now;
                            if (_objstudentimport != null)
                            {
                                db.StudentImports.InsertOnSubmit(_objstudentimport);
                                db.SubmitChanges();
                                maxImportId = clsDashBoard.getmaxImportId(db);
                            }
                            # endregion

                            // The await operator suspends uploadImages.
                            //  - AccessTheWebAsync can't continue until importStudentImagesAsync is complete.
                            //  - Meanwhile, control returns to the caller of uploadImages.
                            //  - Control resumes here when importStudentImagesAsync is complete. 
                            //  - The await operator then retrieves the result from importStudentImagesAsync if method has any return type.
                            await importStudentImagesAsync();
                            uploadStudentImagesComplete();
                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                            DialogResult = false;
                        }
                    }
                }
                else
                {
                    MVVMMessageService.ShowMessage("Unable to import - folder already in use.");
                    isControlsDisableAfterUpload = true;
                    isInProgress = true;
                }

            }
            else
            {
                MVVMMessageService.ShowMessage(errorMessages.FIELDS_CANNOT_BE_EMPTY);
                isControlsDisableAfterUpload = true;
                isInProgress = true;
                return;
            }
            //};

            ////our background worker support cancellation
            //worker.WorkerSupportsCancellation = true;

            //// Configure the function to run when completed
            //worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            //// Launch the worker
            //worker.RunWorkerAsync();
        }

        /// <summary>
        /// This worker_RunWorkerCompleted is called when the worker is finished.
        /// </summary>
        /// <param name="sender">The worker as Object, but it can be cast to a worker.</param>
        /// <param name="e">The RunWorkerCompletedEventArgs object.</param>
        //void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    isInProgress = true; isControlsDisableAfterUpload = true;
        //}

        void uploadStudentImagesComplete()
        {
            _objWaitCursorViewModel.Dispose();
            isInProgress = true;
            isControlsDisableAfterUpload = true;
        }

        #endregion

        #endregion
    }
}
