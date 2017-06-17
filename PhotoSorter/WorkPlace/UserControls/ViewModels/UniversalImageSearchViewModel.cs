using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.StudentImageManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoForce.WorkPlace.UserControls
{
    public class UniversalImageSearchViewModel : ViewModelBase
    {
        #region Initialization
        BulkRenameStudentImage _objBulkRenameStudentImage;
        PhotoSorterDBModelDataContext db;
        string univerasalSearchString;
        string strFile = "";
        string strFileToShow = "";
        string strFilereduced = "";
        #endregion

        #region Properties
        IEnumerable<StudentImage> _dgStudentPhotosData;
        ObservableCollection<StudentImage> _selectedStudentPhotos;
        StudentImage _selectedStudentPhoto;
        ImageSource _studentPhotoPreview;

        public IEnumerable<StudentImage> dgStudentPhotosData
        {
            get { return _dgStudentPhotosData; }
            set { _dgStudentPhotosData = value; NotifyPropertyChanged("dgStudentPhotosData"); }
        }
        public ObservableCollection<StudentImage> selectedStudentPhotos
        {
            get { return _selectedStudentPhotos; }
            set { _selectedStudentPhotos = value; NotifyPropertyChanged("selectedStudentPhotos"); }
        }
        public StudentImage selectedStudentPhoto
        {
            get { return _selectedStudentPhoto; }
            set { _selectedStudentPhoto = value; NotifyPropertyChanged("selectedStudentPhoto"); }
        }
        public ImageSource studentPhotoPreview
        {
            get { return _studentPhotoPreview; }
            set { _studentPhotoPreview = value; NotifyPropertyChanged("studentPhotoPreview"); }
        }
        #endregion

        #region Constructors
        public UniversalImageSearchViewModel()
        {
            selectedStudentPhotos = new ObservableCollection<StudentImage>();
        }
        #endregion

        #region Commands
        public RelayCommand StudentPhotosMouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(editRecord);
            }
        }
        public RelayCommand StudentPhotosTableMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotosTableMouseUp);
            }
        }
        public RelayCommand StudentPhotoPreviewMouseUpCommand
        {
            get
            {
                return new RelayCommand(studentPhotoPreviewMouseUp);
            }
        }
        public RelayCommand StudentPhotosGroupFocusCommand
        {
            get 
            {
                return new RelayCommand(studentPhotosGroupFocus);
            }
        }
        #endregion

        #region Methods
        internal void editRecord()
        {
            if (selectedStudentPhoto != null)
            {
                int studentImgID = selectedStudentPhoto.ID;
                _objBulkRenameStudentImage = new BulkRenameStudentImage(studentImgID);
                _objBulkRenameStudentImage.ShowDialog();
                if (((BulkRenameStudentImageViewModel)(_objBulkRenameStudentImage.DataContext)).isSave)
                    bindStudentImageGrid(univerasalSearchString);
            }
        }
        private void studentPhotosTableMouseUp()
        {
            if (selectedStudentPhoto != null)
            {
                setVisibilityForPhotosGrid();
                selectImage();
            }
        }
        public void bindStudentImageGrid(string searchString)
        {
            univerasalSearchString = searchString.Replace("img:","");
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            try
            {
                dgStudentPhotosData = clsStudent.getStudentImagesByUniversalSearch(db, univerasalSearchString);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void selectImage()
        {
            try
            {
                if (selectedStudentPhoto != null)
                {
                    int imageId = selectedStudentPhoto.ID;
                    PhotoSorterDBModelDataContext db1 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    StudentImage _objStudentImage = clsDashBoard.getStudentImageDetailsById(db1, imageId);
                    if (_objStudentImage == null) { return; }

                    strFile = _objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + _objStudentImage.ImageName;
                    strFileToShow = _objStudentImage.PhotoShoot.ImageFolder + "\\" + _objStudentImage.ImageName;
                    strFilereduced = _objStudentImage.PhotoShoot.ImageFolder + "\\_reduced\\" + _objStudentImage.ImageName;
                    if (!File.Exists(strFilereduced))
                        strFilereduced = strFile;
                    if (!File.Exists(strFilereduced))
                    {
                        studentPhotoPreview = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
                    }
                    else
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(strFilereduced);
                        if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        {
                            decoderForJpeg(strFilereduced);
                        }

                    }
                }
                else
                    studentPhotoPreview = null;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void decoderForJpeg(string strFile)
        {
            using (var stream = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapImage thumb = new BitmapImage();

                stream.Seek(0, SeekOrigin.Begin);

                thumb.BeginInit();
                thumb.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
                thumb.CacheOption = BitmapCacheOption.OnLoad;
                thumb.StreamSource = stream;
                thumb.EndInit();

                studentPhotoPreview = thumb;
            }
        }
        private void studentPhotoPreviewMouseUp()
        {
            try
            {
                string filePath = strFileToShow.ToString();
                filePath = "file:///" + filePath;
                if (filePath.StartsWith("file"))
                {
                    filePath = filePath.Substring(8, filePath.Length - 8);
                    if (File.Exists(filePath))
                    {
                        Process.Start(filePath);
                    }
                    else
                    {
                        filePath = strFilereduced.ToString();
                        filePath = "file:///" + filePath;
                        try
                        {
                            Process.Start(filePath);
                        }
                        catch (Exception ex)
                        {
                            MVVMMessageService.ShowMessage(ex.Message);
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
        private void studentPhotosGroupFocus()
        {
            setVisibilityForPhotosGrid();
        }
        internal void setVisibilityForPhotosGrid()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isEditVisible = true;
        }
        #endregion
    }
}
