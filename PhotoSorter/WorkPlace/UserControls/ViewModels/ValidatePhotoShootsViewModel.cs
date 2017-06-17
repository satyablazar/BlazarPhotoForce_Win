using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System.Collections;
using System.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using PhotoForce.StudentImageManagement;
using System.Diagnostics;
using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using PhotoForce.Extensions;
using PhotoForce.Student_Management;
using PhotoForce.Error_Management;

namespace PhotoForce.WorkPlace.UserControls
{
    public class ValidatePhotoShootsViewModel : ViewModelBase
    {
        #region Initialization
        int[] arrPhotographyJobId;
        int[] arrPhotoShootId;
        int[] arrImportBatchId;
        bool res = false;
        string strFileToShow = "";
        string curserFrom = "";
        AssignStudent _objAssignStudent;
        BulkRenameStudentImage _objBulkRenameStudentImage;
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        clsStatic objstatic = new clsStatic();
        bool isFirstCall = true;
        #endregion

        #region Properties

        #region StudentDetailsGrid Properties
        ObservableCollection<StudentImage> _dgStudentPhotosData;
        ObservableCollection<StudentImage> _selectedStudentPhotos;
        StudentImage _selectedStudentPhoto;
        ImageSource _studentImagePreview;
        ImageSource _firstImagePreview;

        public ImageSource lastImagePreview
        {
            get { return _firstImagePreview; }
            set { _firstImagePreview = value; NotifyPropertyChanged(); }
        }
        public ImageSource studentImagePreview
        {
            get { return _studentImagePreview; }
            set { _studentImagePreview = value; NotifyPropertyChanged("studentImagePreview"); }
        }
        public StudentImage selectedStudentPhoto
        {
            get { return _selectedStudentPhoto; }
            set { _selectedStudentPhoto = value; NotifyPropertyChanged("selectedStudentPhoto"); }
        }
        public ObservableCollection<StudentImage> selectedStudentPhotos
        {
            get { return _selectedStudentPhotos; }
            set { _selectedStudentPhotos = value; NotifyPropertyChanged("selectedStudentPhotos"); }
        }
        public ObservableCollection<StudentImage> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        #endregion

        DataTable _dg5StarData;
        DataTable _dgWithout5StarData;
        List<CountImagesNStudents> _dgCountImagesData;
        List<PhotoShoot> _dgPhotoShootData;
        ObservableCollection<PhotoShoot> _selectedPhotoShoots;
        PhotoShoot _selectedPhotoShoot;
        //StudentImport _selectedImportBatch;
        ObservableCollection<StudentImport> _selectedImportBatches;
        List<StudentImport> _dgStudentImportData;
        int _selectedIndex;
        IList<Student> _dgStudentsWithoutImagesData;
        bool _studentDetailsGridVisibility;
        DataRowView _selectedStudent;
        ObservableCollection<DataTable> _selectedStudents;
        private bool _photosTableShowGroupPanel;
        private bool _isSearchControlVisible;
        private ShowSearchPanelMode _photosTableSearchPanelMode;
        private SearchControl _photosTableSearchControl;
        ObservableCollection<Student> _selectedStudentsWithoutImages;
        ObservableCollection<CountImagesNStudents> _selectedCountImagesBatchs;
        CountImagesNStudents _selectedStudentFromCountImages;
        string _imgCaption;
        bool _firstImagePreviewVisibility;
        StudentImport _selectedImportBatch;

        public StudentImport selectedImportBatch
        {
            get { return _selectedImportBatch; }
            set { _selectedImportBatch = value; NotifyPropertyChanged(); }
        }
        public bool lastImagePreviewVisibility
        {
            get { return _firstImagePreviewVisibility; }
            set { _firstImagePreviewVisibility = value; NotifyPropertyChanged(); }
        }
        public string imgCaption
        {
            get { return _imgCaption; }
            set { _imgCaption = value; NotifyPropertyChanged(); }
        }
        public CountImagesNStudents selectedStudentFromCountImages
        {
            get { return _selectedStudentFromCountImages; }
            set { _selectedStudentFromCountImages = value; NotifyPropertyChanged("selectedStudentFromCountImages"); }
        }
        public ObservableCollection<CountImagesNStudents> selectedCountImagesBatchs
        {
            get { return _selectedCountImagesBatchs; }
            set { _selectedCountImagesBatchs = value; NotifyPropertyChanged("selectedCountImagesBatchs"); }
        }
        public List<CountImagesNStudents> dgCountImagesData
        {
            get { return _dgCountImagesData; }
            set { _dgCountImagesData = value; NotifyPropertyChanged("dgCountImagesData"); }
        }
        public ObservableCollection<Student> selectedStudentsWithoutImages
        {
            get { return _selectedStudentsWithoutImages; }
            set { _selectedStudentsWithoutImages = value; NotifyPropertyChanged("selectedStudentsWithoutImages"); }
        }
        public SearchControl photosTableSearchControl
        {
            get { return _photosTableSearchControl; }
            set { _photosTableSearchControl = value; NotifyPropertyChanged("photosTableSearchControl"); }
        }
        public ShowSearchPanelMode photosTableSearchPanelMode
        {
            get { return _photosTableSearchPanelMode; }
            set { _photosTableSearchPanelMode = value; NotifyPropertyChanged("photosTableSearchPanelMode"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public bool photosTableShowGroupPanel
        {
            get { return _photosTableShowGroupPanel; }
            set { _photosTableShowGroupPanel = value; NotifyPropertyChanged("photosTableShowGroupPanel"); }
        }
        public ObservableCollection<DataTable> selectedStudents
        {
            get { return _selectedStudents; }
            set { _selectedStudents = value; NotifyPropertyChanged("selectedStudents"); }
        }

        public DataRowView selectedStudent
        {
            get { return _selectedStudent; }
            set { _selectedStudent = value; NotifyPropertyChanged("selectedStudent"); curserFrom = "StudentData"; }
        }

        public bool studentDetailsGridVisibility
        {
            get { return _studentDetailsGridVisibility; }
            set { _studentDetailsGridVisibility = value; NotifyPropertyChanged("studentDetailsGridVisibility"); }
        }
        public IList<Student> dgStudentsWithoutImagesData
        {
            get { return _dgStudentsWithoutImagesData; }
            set { _dgStudentsWithoutImagesData = value; NotifyPropertyChanged("dgStudentsWithoutImagesData"); }
        }
        public int selectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value; NotifyPropertyChanged("selectedIndex");
                lastImagePreviewVisibility = false;
                if (selectedIndex == 0 || selectedIndex == 1 || selectedIndex == 6)
                {
                    studentDetailsGridVisibility = false;
                }
                else
                {
                    dgStudentPhotosData = new ObservableCollection<StudentImage>();
                    studentImagePreview = null;
                    if (!studentDetailsGridVisibility) { studentDetailsGridVisibility = true; imgCaption = "Photo Preview"; }
                }
                if (dgPhotoShootData != null && dgPhotoShootData.Count > 0)
                {
                    if (selectedIndex == 2)
                    {
                        res = false;
                        (Application.Current as App).isClearStudentsVisible = true;
                        if (countImages())
                        {
                            if (arrPhotoShootId.Count() > 0)
                            {
                                //if (isFirstCall)
                                {
                                    ArrayList jobIDs = new ArrayList();
                                    jobIDs.AddRange(arrPhotoShootId);
                                    dgCountImagesData = clsDashBoard.CountImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), jobIDs);

                                    #region removing cleared students
                                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                                    Setting clearedStudentIds = (from stu in db.Settings where stu.settingName == "ClearingCorrectStudents" select stu).FirstOrDefault();

                                    if (clearedStudentIds != null && !string.IsNullOrEmpty(clearedStudentIds.settingValue))
                                    {
                                        ArrayList tempIds = new ArrayList();
                                        string[] ToatlIds = (clearedStudentIds.settingValue).Split(',');

                                        foreach (string id in ToatlIds)
                                        {
                                            if (!tempIds.Contains(Convert.ToInt32(id)))
                                                tempIds.Add(Convert.ToInt32(id));
                                        }

                                        List<CountImagesNStudents> tempImagesData = new List<CountImagesNStudents>();


                                        foreach (CountImagesNStudents imgd in dgCountImagesData)
                                        {
                                            if (!tempIds.Contains(imgd.StudentId))
                                                tempImagesData.Add(imgd);
                                            //foreach (string id in ToatlIds)
                                            //{
                                            //    if ((imgd.StudentId != (Convert.ToInt32(id))) && !tempImagesData.Contains(imgd) && !tempIds.Contains(imgd.StudentId))
                                            //    {
                                            //        tempImagesData.Add(imgd);
                                            //        tempIds.Add(imgd.StudentId);
                                            //    }
                                            //}
                                        }
                                        dgCountImagesData = new List<CountImagesNStudents>();
                                        dgCountImagesData = tempImagesData;
                                    }
                                    #endregion
                                }
                                //dgCountImagesData = new List<CountImagesNStudents>();
                            }
                        }
                    }
                    else if (dgStudentImportData != null && dgStudentImportData.Count > 0)
                    {
                        //5 - Star
                        if (selectedIndex == 3)
                        {
                            res = false;
                            if (bindGrid())
                            {
                                if (arrImportBatchId.Count() > 0 && arrPhotoShootId.Count() > 0)
                                {
                                    dg5StarData = clsDashBoard.GetStudentFor5Rating(arrPhotoShootId, arrImportBatchId);
                                    dgWithout5StarData = clsDashBoard.GetStudentForNot5Rating(arrPhotographyJobId, arrPhotoShootId, arrImportBatchId);
                                }
                            }
                        }
                        //Admin CD
                        else if (selectedIndex == 4)
                        {
                            res = false;
                            if (bindGrid())
                            {
                                if (arrImportBatchId.Count() > 0 && arrPhotoShootId.Count() > 0)
                                {
                                    dg5StarData = clsDashBoard.GetStudentForAdminCdYearbookcd(arrPhotoShootId, arrImportBatchId, "admincd");
                                    dgWithout5StarData = clsDashBoard.GetStudentForNotRatingAdminCdYearbookcd(arrPhotoShootId, arrImportBatchId, "admincd");//arrPhotographyJobId,
                                }
                            }
                        }
                        //Year Book
                        else if (selectedIndex == 5)
                        {
                            res = false;
                            if (bindGrid())
                            {
                                if (arrImportBatchId.Count() > 0 && arrPhotoShootId.Count() > 0)
                                {
                                    dg5StarData = clsDashBoard.GetStudentForAdminCdYearbookcd(arrPhotoShootId, arrImportBatchId, "yearbook");
                                    dgWithout5StarData = clsDashBoard.GetStudentForNotRatingAdminCdYearbookcd(arrPhotoShootId, arrImportBatchId, "yearbook");//arrPhotographyJobId,
                                }
                            }
                        }
                        //Students Without Images
                        else if (selectedIndex == 6)
                        {
                            res = false;
                            studentDetailsGridVisibility = false;
                            if (bindGrid())
                            {
                                if (arrImportBatchId.Count() > 0 && arrPhotoShootId.Count() > 0)
                                    dgStudentsWithoutImagesData = clsDashBoard.GetStudentsWithoutImgDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrPhotoShootId, arrImportBatchId, clsSchool.defaultSchoolId);
                            }
                        }
                    } // end of if(dgStudentImportData.Count > 0)
                }// end of if(dgPhotoShootData.Count > 0)
                else if (selectedIndex != 0 && selectedIndex != 1)
                {
                    selectedIndex = 0;
                }
            }
        }
        //public StudentImport selectedImportBatch
        //{
        //    get { return _selectedImportBatch; }
        //    set { _selectedImportBatch = value; NotifyPropertyChanged("selectedImportBatch"); }
        //}
        public PhotoShoot selectedPhotoShoot
        {
            get { return _selectedPhotoShoot; }
            set { _selectedPhotoShoot = value; NotifyPropertyChanged("selectedPhotoShoot"); }
        }
        public List<PhotoShoot> dgPhotoShootData
        {
            get { return _dgPhotoShootData; }
            set { _dgPhotoShootData = value; NotifyPropertyChanged("dgPhotoShootData"); }
        }
        public List<StudentImport> dgStudentImportData
        {
            get { return _dgStudentImportData; }
            set { _dgStudentImportData = value; NotifyPropertyChanged("dgStudentImportData"); }
        }
        public ObservableCollection<StudentImport> selectedImportBatches
        {
            get { return _selectedImportBatches; }
            set { _selectedImportBatches = value; NotifyPropertyChanged("selectedImportBatches"); }
        }
        public ObservableCollection<PhotoShoot> selectedPhotoShoots
        {
            get { return _selectedPhotoShoots; }
            set { _selectedPhotoShoots = value; NotifyPropertyChanged("selectedPhotoShoots"); }
        }
        public DataTable dgWithout5StarData
        {
            get { return _dgWithout5StarData; }
            set { _dgWithout5StarData = value; NotifyPropertyChanged("dgWithout5StarData"); }
        }
        public DataTable dg5StarData
        {
            get { return _dg5StarData; }
            set { _dg5StarData = value; NotifyPropertyChanged("dg5StarData"); }
        }
        #endregion

        # region Constructor
        public ValidatePhotoShootsViewModel()
        {
            selectedStudentsWithoutImages = new ObservableCollection<Student>();
            selectedStudentPhotos = new ObservableCollection<StudentImage>();
            selectedPhotoShoots = new ObservableCollection<PhotoShoot>(); selectedImportBatches = new ObservableCollection<StudentImport>();
            selectedCountImagesBatchs = new ObservableCollection<CountImagesNStudents>();
            lastImagePreviewVisibility = false;

            loadData();
        }
        # endregion

        #region Commands
        public RelayCommand CountImagesLeftClickCommand
        {
            get
            {
                return new RelayCommand(countImagesGridMouseup);
            }
        }
        public RelayCommand StarLeftClickCommand
        {
            get
            {
                return new RelayCommand(studentsGridMouseup);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand StudentPhotosTableKeyUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotosTableKeyUp);
            }
        }
        public RelayCommand StudentPhotosDoubleClickCommand
        {
            get
            {
                return new RelayCommand(studentPhotosDoubleClick);
            }
        }
        public RelayCommand StudentPhotoPreviewMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotoPreviewMouseUp);
            }
        }
        public RelayCommand StudentsTableKeyUpCommand
        {
            get
            {
                return new RelayCommand(StudentsTableKeyUp);
            }
        }
        public RelayCommand StudentsWithoutImagesLeftClickCommand
        {
            get
            {
                return new RelayCommand(studentsWithoutImagesGridMouseUp);
            }
        }
        public RelayCommand RowUpdateCommand
        {
            get
            {
                return new RelayCommand(inlineEdit);
            }
        }
        #endregion

        #region Methods
        private void inlineEdit()
        {
            if (selectedIndex == 0)
            {
                PhotoShoot _objPhotoshoot = new PhotoShoot();
                _objPhotoshoot = (from ps in db.PhotoShoots where ps.PhotoShotID == selectedPhotoShoot.PhotoShotID select ps).FirstOrDefault();

                _objPhotoshoot.IsValidated = selectedPhotoShoot.IsValidated;

                db.SubmitChanges();
            }
            else if (selectedIndex == 1)
            {
                StudentImport _objStudentImport = new StudentImport();
                _objStudentImport = (from si in db.StudentImports where si.ID == selectedImportBatch.ID select si).FirstOrDefault();

                _objStudentImport.IsValidated = selectedImportBatch.IsValidated;

                db.SubmitChanges();
            }

        }
        public void loadData()
        {
            try
            {
                selectedIndex = 0;
                #region Load Photoshoots data
                string allJobId = "";
                List<int> jobIds = clsDashBoard.getAllJobs(clsSchool.defaultSchoolId, new PhotoSorterDBModelDataContext(clsConnectionString.connectionString));
                if (jobIds.Count > 0)
                {
                    foreach (int jobid in jobIds)
                    {
                        allJobId += jobid + ",";
                    }
                    allJobId = allJobId.Substring(0, allJobId.Length - 1);
                    dgPhotoShootData = clsDashBoard.getPhotoShoot(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), allJobId);
                    if (dgPhotoShootData.Count > 0)
                    {
                        selectedPhotoShoot = dgPhotoShootData.FirstOrDefault();
                        //selectedPhotoShoots.Add(selectedPhotoShoot);
                    }
                }
                else
                {
                    dgPhotoShootData = new List<PhotoShoot>();
                }
                #endregion

                #region Load Import Batch
                dgStudentImportData = clsDashBoard.getStudentImportData(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId);
                if (dgStudentImportData.Count > 0)
                {
                    selectedImportBatch = dgStudentImportData.FirstOrDefault();
                    selectedImportBatches.Add(selectedImportBatch);
                }
                #endregion
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private bool bindGrid()
        {
            if (selectedPhotoShoot == null || selectedImportBatch == null)
            {
                MVVMMessageService.ShowMessage("Select at least one photoshoot and one import batch in order to determine results.");
                selectedIndex = 0;
                res = false;
            }
            else
            {
                arrPhotographyJobId = (from sps in selectedPhotoShoots select (int)sps.PhotographyjobID).Distinct().ToArray();
                arrPhotoShootId = (from sps in selectedPhotoShoots select sps.PhotoShotID).ToArray();
                arrImportBatchId = (from sib in selectedImportBatches select sib.ID).ToArray();
                res = true;
            }
            return res;
        }
        private bool countImages()
        {
            if (selectedPhotoShoot == null)
            {
                MVVMMessageService.ShowMessage("Select at least one photoshoot in order to determine results.");
                selectedIndex = 0;
                res = false;
            }
            else
            {
                arrPhotographyJobId = (from sps in selectedPhotoShoots select (int)sps.PhotographyjobID).Distinct().ToArray();
                arrPhotoShootId = (from sps in selectedPhotoShoots select sps.PhotoShotID).ToArray();
                res = true;
            }
            return res;
        }
        void studentsGridMouseup()
        {
            if (selectedStudent != null)
            {
                studentGridButtonsVisibility();
                dgStudentPhotosData = new ObservableCollection<StudentImage>(clsDashBoard.getStudentsImagesByStuId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrPhotoShootId, Convert.ToInt32(selectedStudent["studentid"])));
                if (dgStudentPhotosData.Count == 0)
                {
                    studentImagePreview = null;
                }
                else
                {
                    selectedStudentPhoto = dgStudentPhotosData.First();
                    selectImage();
                }
            }
        }
        void studentsWithoutImagesGridMouseUp()
        {
            studentGridButtonsVisibility();
        }
        void countImagesGridMouseup()
        {
            if (dgCountImagesData.Count != 0)
            {
                (Application.Current as App).isClearStudentsVisible = true;
                studentGridButtonsVisibility();
                lastImagePreviewVisibility = true; imgCaption = "First Image"; int i = 0;
                selectedStudentFromCountImages = selectedStudentFromCountImages == null ? dgCountImagesData.First() : selectedStudentFromCountImages;

                List<StudentImage> tempStudentPhotosData = clsDashBoard.getStudentsImagesByStuId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrPhotoShootId, Convert.ToInt32(selectedStudentFromCountImages.StudentId));
                if (tempStudentPhotosData.Count == 0)
                {
                    dgStudentPhotosData = null;
                    studentImagePreview = null;
                }
                else
                {
                    lastImagePreview = null;
                    List<StudentImage> tempData2 = (from tsp in tempStudentPhotosData orderby tsp.ImageNumber select tsp).ToList();
                    dgStudentPhotosData = new ObservableCollection<StudentImage>(tempData2);
                    for (i = 0; i < dgStudentPhotosData.Count; i++)
                    {
                        if (dgStudentPhotosData[i].Rating == "1")
                            continue;
                        else
                        {
                            selectedStudentPhoto = dgStudentPhotosData[i];
                            selectImage();
                            break;
                        }
                    }

                    showLastImage(dgStudentPhotosData.Last());
                }
            }
        }
        internal void editRecord()
        {
            if (selectedIndex == 6)
                editStudentRecord();
            else if (selectedStudentPhoto != null && curserFrom == "StudentPhotosGrid")
                studentPhotosDoubleClick();
        }
        private void studentPhotosDoubleClick()
        {
            try
            {
                if (selectedStudentPhoto != null)
                {
                    int studentImgID = selectedStudentPhoto.ID;
                    _objBulkRenameStudentImage = new BulkRenameStudentImage(studentImgID);
                    _objBulkRenameStudentImage.ShowDialog();
                    if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                    {
                        if (selectedIndex != 2)
                            refreshGrid();
                        if (selectedIndex == 2)
                        {
                            countImagesGridMouseup();

                            imgCaption = "Photo Preview";
                            lastImagePreviewVisibility = false; lastImagePreview = null;
                            studentPhotosGridButtonsVisibility();
                            curserFrom = "StudentPhotosGrid";
                            selectImage();


                            //db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                            //StudentImage tempImage = clsDashBoard.getImage(db, selectedStudentPhoto.ID);  //(from sti in db.StudentImages where sti.ID == selectedStudentPhoto.ID  select sti).FirstOrDefault();
                            //dgStudentPhotosData.Remove(selectedStudentPhoto);
                            //dgStudentPhotosData.Insert(dgStudentPhotosData.Count <= 1 ? 0 : dgStudentPhotosData.IndexOf(selectedStudentPhoto), tempImage);
                            //selectedStudentPhoto = tempImage;
                        }
                        else { studentsGridMouseup(); }
                    }
                }


            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void studentPhotosTableKeyUp()
        {
            imgCaption = "Photo Preview";
            lastImagePreviewVisibility = false; lastImagePreview = null;
            studentPhotosGridButtonsVisibility();
            curserFrom = "StudentPhotosGrid";
            selectImage();
        }
        private void StudentsTableKeyUp()
        {
            curserFrom = "StudentData";
            studentsGridMouseup();
        }
        private void showLastImage(StudentImage tempFirstImage)
        {
            string strFirstFileToShow = tempFirstImage.PhotoShoot.ImageFolder + "\\" + tempFirstImage.ImageName;
            string strFirstFilereduced = tempFirstImage.PhotoShoot.ImageFolder + "\\_reduced\\" + tempFirstImage.ImageName;

            if (!File.Exists(strFirstFilereduced))
            {
                strFirstFilereduced = strFileToShow;
                lastImagePreview = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
            }
            else
            {
                using (FileStream fs = new FileStream(strFirstFilereduced, FileMode.Open, FileAccess.Read))
                {
                    using (Image original = Image.FromStream(fs))
                    {
                        if (original.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        {
                            BitmapImage thumb = new BitmapImage();

                            fs.Seek(0, SeekOrigin.Begin);

                            thumb.BeginInit();
                            thumb.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
                            thumb.CacheOption = BitmapCacheOption.OnLoad;
                            //thumb.DecodePixelWidth = 200;
                            thumb.StreamSource = fs;
                            thumb.EndInit();
                            lastImagePreview = thumb;
                        }
                    }
                }
            }
        }
        private void selectImage()
        {
            try
            {
                if (selectedStudentPhoto != null)
                {
                    int ImageId = selectedStudentPhoto.ID;
                    PhotoSorterDBModelDataContext dbref = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    StudentImage objStudentImage = clsDashBoard.getStudentImageDetailsById(dbref, ImageId);
                    if (objStudentImage == null) { return; }


                    strFileToShow = objStudentImage.PhotoShoot.ImageFolder + "\\" + objStudentImage.ImageName;
                    string strFilereduced = objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + objStudentImage.ImageName;
                    if (!File.Exists(strFilereduced))
                        strFilereduced = strFileToShow;
                    if (!File.Exists(strFilereduced))
                    {
                        studentImagePreview = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(strFilereduced, FileMode.Open, FileAccess.Read))
                        {
                            using (Image original = Image.FromStream(fs))
                            {
                                if (original.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                                {
                                    decoderForJpeg(fs);
                                }
                            }
                        }
                    }
                }
                else
                {
                    studentImagePreview = null;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void decoderForJpeg(FileStream stream)
        {
            //using (var stream = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            BitmapImage thumb = new BitmapImage();

            stream.Seek(0, SeekOrigin.Begin);

            thumb.BeginInit();
            thumb.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
            thumb.CacheOption = BitmapCacheOption.OnLoad;
            //thumb.DecodePixelWidth = 200;
            thumb.StreamSource = stream;
            thumb.EndInit();
            studentImagePreview = thumb;
            //}
        }
        private void studentPhotoPreviewMouseUp()
        {
            try
            {
                string filePath = strFileToShow.ToString();
                if (File.Exists(filePath))
                {
                    Process.Start(filePath);
                }

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void bulkRename()
        {
            try
            {
                if (selectedIndex == 6)
                    bulkRenameStudents();
                else if (selectedStudentPhotos.Count != 0 && curserFrom == "StudentPhotosGrid")
                {
                    //Commented by Mohan on 16-10-2015
                    //int stuImgId;
                    //ArrayList arrStuId = new ArrayList();
                    //foreach (StudentImage stuImage in selectedStudentPhotos)
                    //{
                    //    try
                    //    {
                    //        stuImgId = Convert.ToInt32(stuImage.ID);
                    //        if (!arrStuId.Contains(stuImgId))
                    //        {
                    //            arrStuId.Add(stuImgId);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        clsStatic.WriteExceptionLogXML(ex);
                    //        MVVMMessageService.ShowMessage(ex.Message);
                    //    }
                    //}
                    ArrayList arrStuId = new ArrayList((from stuImage in selectedStudentPhotos select Convert.ToInt32(stuImage.ID)).ToList());
                    BulkRenameStudentImage _objBulkRenameStudentImage = new BulkRenameStudentImage(arrStuId);
                    _objBulkRenameStudentImage.ShowDialog();
                    if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                    {
                        refreshGrid();
                        if (selectedIndex == 2) { countImagesGridMouseup(); } else { studentsGridMouseup(); }
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void editStudentRecord()
        {
            try
            {
                if (selectedIndex == 6)
                {
                    if (selectedStudentsWithoutImages.Count != 0)
                    {
                        int studentID = selectedStudentsWithoutImages[0].ID;
                        AddEditStudent _objAddEditStudent = new AddEditStudent(studentID, 0, clsSchool.defaultSchoolId);
                        _objAddEditStudent.ShowDialog();
                        if (((AddEditStudentViewModel)(_objAddEditStudent.DataContext)).isSave)
                            refreshGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void bulkRenameStudents()
        {
            try
            {
                if (selectedIndex == 6)
                {
                    if (selectedStudentsWithoutImages.Count != 0)
                    {
                        int tempStudentId = 0;
                        ArrayList arrStudentsId = new ArrayList();
                        foreach (Student tempStudent in selectedStudentsWithoutImages)
                        {
                            try
                            {
                                tempStudentId = Convert.ToInt32(tempStudent.ID);
                                if (!arrStudentsId.Contains(tempStudentId))
                                {
                                    arrStudentsId.Add(tempStudentId);
                                }
                            }
                            catch (Exception ex)
                            {
                                MVVMMessageService.ShowMessage(ex.Message);
                                clsStatic.WriteExceptionLogXML(ex);
                            }
                        }

                        BulkRenameStudent _objBulkRenameStudent = new BulkRenameStudent(arrStudentsId);
                        _objBulkRenameStudent.ShowDialog();
                        if (((BulkRenameStudentViewModel)(_objBulkRenameStudent.DataContext)).isSave)
                        {
                            refreshGrid();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        public void refreshGrid()
        {
            try
            {
                if (selectedIndex == 0 || selectedIndex == 1 || selectedIndex == 6) { studentDetailsGridVisibility = false; }
                else
                {
                    dgStudentPhotosData = new ObservableCollection<StudentImage>();
                    studentImagePreview = null;
                    if (!studentDetailsGridVisibility) { studentDetailsGridVisibility = true; }
                }

                //Photoshoots data and Import batch
                if (selectedIndex == 0 || selectedIndex == 1)
                {
                    loadData();
                }
                //CountImages
                if (selectedIndex == 2)
                {
                    res = false;
                    if (countImages())
                    {
                        ArrayList jobIDs = new ArrayList();
                        jobIDs.AddRange(arrPhotoShootId);
                        //dgCountImagesData = clsDashBoard.CountImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), jobIDs);
                        int selectedIndexId = 0;
                        if (selectedStudentFromCountImages != null)
                            selectedIndexId = dgCountImagesData.IndexOf(selectedStudentFromCountImages);
                        dgCountImagesData = clsDashBoard.CountImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), jobIDs);

                        if (dgCountImagesData.Count > 0)
                            selectedStudentFromCountImages = dgCountImagesData[selectedIndexId];
                    }
                }
                //5 - Star
                if (selectedIndex == 3)
                {
                    res = false;
                    if (bindGrid())
                    {
                        dg5StarData = clsDashBoard.GetStudentFor5Rating(arrPhotoShootId, arrImportBatchId);
                        dgWithout5StarData = clsDashBoard.GetStudentForNot5Rating(arrPhotographyJobId, arrPhotoShootId, arrImportBatchId);
                    }
                }
                //Admin CD
                if (selectedIndex == 4)
                {
                    res = false;
                    if (bindGrid())
                    {
                        dg5StarData = clsDashBoard.GetStudentForAdminCdYearbookcd(arrPhotoShootId, arrImportBatchId, "admincd");
                        dgWithout5StarData = clsDashBoard.GetStudentForNotRatingAdminCdYearbookcd(arrPhotoShootId, arrImportBatchId, "admincd");//arrPhotographyJobId,
                    }
                }
                //Year Book
                if (selectedIndex == 5)
                {
                    res = false;
                    if (bindGrid())
                    {
                        dg5StarData = clsDashBoard.GetStudentForAdminCdYearbookcd(arrPhotoShootId, arrImportBatchId, "yearbook");
                        dgWithout5StarData = clsDashBoard.GetStudentForNotRatingAdminCdYearbookcd(arrPhotoShootId, arrImportBatchId, "yearbook");//arrPhotographyJobId,
                    }
                }
                //Students Without Images
                if (selectedIndex == 6)
                {
                    res = false;
                    studentDetailsGridVisibility = false;
                    if (bindGrid())
                    {
                        dgStudentsWithoutImagesData = clsDashBoard.GetStudentsWithoutImgDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrPhotoShootId, arrImportBatchId, clsSchool.defaultSchoolId);
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        internal void assignStudent()
        {
            if (selectedStudentPhotos.Count <= 0 || selectedIndex == 0 || selectedIndex == 1 || selectedIndex == 6)
            {
                MVVMMessageService.ShowMessage(errorMessages.SELECT_STUDENT_IMAGE);
                return;
            }

            ArrayList arrStudentImgId = new ArrayList((from stuImage in selectedStudentPhotos select Convert.ToInt32(stuImage.ID)).ToList());
            //Commented by Mohan on 16-10-2015
            //foreach (StudentImage stuImage in selectedStudentPhotos)
            //{
            //    try
            //    {
            //        studentImageId = stuImage.ID;
            //        if (!arrStudentImgId.Contains(studentImageId))
            //        {
            //            arrStudentImgId.Add(studentImageId);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        clsStatic.WriteExceptionLogXML(ex);
            //        MVVMMessageService.ShowMessage(ex.Message);
            //    }
            //}
            if (arrStudentImgId.Count != 0)
            {
                _objAssignStudent = new AssignStudent(arrStudentImgId);
                _objAssignStudent.ShowDialog();
                if (((AssignStudentViewModel)(_objAssignStudent.DataContext)).isSave)
                    refreshGrid();
            }
            else
            {
                MVVMMessageService.ShowMessage(errorMessages.SELECT_STUDENT_IMAGE);
            }

        }
        public void groupPanels()
        {
            try
            {
                if (selectedIndex == 0 || selectedIndex == 1 || selectedIndex == 6)
                    return;
                if (photosTableShowGroupPanel)
                    photosTableShowGroupPanel = false;
                else
                    photosTableShowGroupPanel = true;

            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        public void searchPanels()
        {
            try
            {
                if (selectedIndex == 0 || selectedIndex == 1 || selectedIndex == 6)
                    return;
                if (isSearchControlVisible == false || photosTableSearchControl == null)
                {
                    photosTableSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
                }
                else
                {
                    photosTableSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
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
            DialogResult = false;
        }
        /// <summary>
        /// this method used to delete student record(s)
        /// </summary>
        public void deleteRecords() //#Mohan ; #NUnitTest
        {
            try
            {
                if (selectedIndex == 6)
                {
                    clsStatic.clearTempXML();
                    bool hasError = false;
                    string message = "", studentFirstName = "", studentLastName = "", name = "";
                    ArrayList arrStudId = new ArrayList();
                    ArrayList arrStudentToDelete = new ArrayList();
                    ArrayList arrStudentToShow = new ArrayList();

                    foreach (Student tempStudent in selectedStudentsWithoutImages)
                    {
                        try
                        {
                            int studentId = Convert.ToInt32(tempStudent.ID);
                            studentFirstName = Convert.ToString(tempStudent.FirstName);
                            studentLastName = Convert.ToString(tempStudent.Lastname);
                            name = studentFirstName + " " + studentLastName;
                            if (!arrStudId.Contains(studentId))
                            {
                                arrStudId.Add(studentId);
                            }
                        }
                        catch (Exception ex)
                        {
                            clsStatic.WriteExceptionLogXML(ex);
                            MVVMMessageService.ShowMessage(ex.Message);
                        }
                    }
                    if (arrStudId.Count != 0)
                    {
                        if (arrStudId.Count == 1)
                        {
                            message = "Are you sure you want to delete student " + name + "?";
                        }
                        else
                        {
                            message = "Are you sure you want to delete multiple students?";
                        }
                        if (arrStudId.Count != 0)
                        {
                            string caption = "Confirmation";
                            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                            if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                            {
                                //Need to Check Students has images or not..
                                foreach (int studentId in arrStudId)
                                {
                                    int RetStudentImages = clsDashBoard.CountStudentImages(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), studentId);
                                    if (RetStudentImages <= 0)
                                        arrStudentToDelete.Add(studentId);
                                    else
                                    {
                                        hasError = true;
                                        arrStudentToShow.Add(studentId);
                                        clsErrorLog objError = new clsErrorLog();

                                        objError.Source = "Student";
                                        objError.MethodName = "Delete Student";
                                        objError.Message = "Student has images";
                                        IEnumerable<Student> _objStudentData = clsStudent.getStudentDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), studentId);
                                        foreach (var studentitem in _objStudentData)
                                        {
                                            objError.Parameters = studentitem.FirstName + " " + studentitem.Lastname + "," + studentitem.Grade;
                                        }
                                        clsStatic.WriteErrorLog(objError, objstatic.ExceptionLogXML);
                                        clsStatic.WriteErrorLog(objError, objstatic.ErrorLogXML);
                                    }
                                }
                                if (arrStudentToDelete.Count > 0)
                                {
                                    //int totalRecordsCount = dgStudentsWithoutImagesData.Count();
                                    //int deletedRecordsCount = arrStudentToDelete.Count;

                                    int delResult = clsStudent.deletestudents(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentToDelete);
                                    //bindStudentGrid();

                                    //foreach (int stuId in arrStudentToDelete)
                                    //{
                                    //    dgStudentsWithoutImagesData.Remove(dgStudentsWithoutImagesData.Where(i => i.ID == stuId).First());
                                    //}
                                    refreshGrid();

                                    //createDeletedRecordsLogFile("Validate-Students", totalRecordsCount, deletedRecordsCount);
                                }
                                if (hasError == true)
                                {
                                    string delMessage = errorMessages.AFTER_DELETION_STUDENTS_CONFIRMATION_ERRORS;
                                    string delCaption = "Confirmation";
                                    System.Windows.MessageBoxButton delButtons = System.Windows.MessageBoxButton.YesNo;
                                    System.Windows.MessageBoxImage delIcon = System.Windows.MessageBoxImage.Question;
                                    if (System.Windows.MessageBox.Show(delMessage, delCaption, delButtons, delIcon) == System.Windows.MessageBoxResult.Yes)
                                    {
                                        ShowErrors _objShowErrors = new ShowErrors();
                                        _objShowErrors.ShowDialog();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage("please select student(s)");
                    }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }


        internal void clearingCorrectStudents()
        {
            if (selectedCountImagesBatchs.Count > 0)
            {
                ArrayList ClearingStudentIds = new ArrayList();
                foreach (CountImagesNStudents stu in selectedCountImagesBatchs)
                {
                    if (!ClearingStudentIds.Contains(stu.StudentId))
                    {
                        ClearingStudentIds.Add(stu.StudentId);
                    }
                }

                int isClearingStudentIdsAvilable = (from clrstudents in db.Settings where clrstudents.settingName == "ClearingCorrectStudents" select clrstudents).Count();

                clsDashBoard.clearStudentBatch(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), ClearingStudentIds, isClearingStudentIdsAvilable > 0 ? true : false);

                List<CountImagesNStudents> tempImagesData = new List<CountImagesNStudents>();
                ArrayList tempIds = new ArrayList();
                foreach (CountImagesNStudents imgd in dgCountImagesData)
                {
                    if (!ClearingStudentIds.Contains(imgd.StudentId))
                        tempImagesData.Add(imgd);
                }
                dgCountImagesData = new List<CountImagesNStudents>();
                dgCountImagesData = tempImagesData;
            }
            else
                MVVMMessageService.ShowMessage("Please select student batch to clear.");
            isFirstCall = false;
        }

        #region Buttons visibility based on grid selection
        private void studentPhotosGridButtonsVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isEditVisible = true; (System.Windows.Application.Current as App).isBulkRenameVisible = true;
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isRefreshVisible = true; (Application.Current as App).isAssignStudentVisible = true;
        }
        void studentGridButtonsVisibility()
        {

            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isRefreshVisible = true;
            if (selectedIndex == 6)
            {
                (Application.Current as App).isBulkRenameVisible = true;
                (Application.Current as App).isEditVisible = true; (Application.Current as App).isDeleteVisible = true;
            }
            if (selectedIndex == 2)
                (Application.Current as App).isClearStudentsVisible = true;
            else
                (Application.Current as App).isClearStudentsVisible = false;
        }

        #endregion
        # endregion
    }
}
