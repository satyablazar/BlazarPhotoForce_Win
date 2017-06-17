using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.WorkPlace.UserControls
{
    public class LockedPhotosViewModel : ViewModelBase
    {
        //I don't think we are using this class any more.
        //#Mohan
        #region Properties
        ObservableCollection<StudentImage> _dgLockedPhotoData;
        StudentImage _selectedImage;
        List<StudentImage> _selectedImages;
        bool _isSearchControlVisible;
        SearchControl _lockedPhotosSearchControl;
        ShowSearchPanelMode _lockedPhotosSearchPanelMode;
        bool _lockedPhotosShowGroupPanel;

        public ShowSearchPanelMode lockedPhotosSearchPanelMode
        {
            get { return _lockedPhotosSearchPanelMode; }
            set { _lockedPhotosSearchPanelMode = value; NotifyPropertyChanged("lockedPhotosSearchPanelMode"); }
        }
        public SearchControl lockedPhotosSearchControl
        {
            get { return _lockedPhotosSearchControl; }
            set { _lockedPhotosSearchControl = value; NotifyPropertyChanged("lockedPhotosSearchControl"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public bool lockedPhotosShowGroupPanel
        {
            get { return _lockedPhotosShowGroupPanel; }
            set { _lockedPhotosShowGroupPanel = value; NotifyPropertyChanged("lockedPhotosShowGroupPanel"); }
        }
        public List<StudentImage> selectedImages
        {
            get { return _selectedImages; }
            set { _selectedImages = value; NotifyPropertyChanged("selectedImages"); }
        }

        public StudentImage selectedImage
        {
            get { return _selectedImage; }
            set { _selectedImage = value; NotifyPropertyChanged("selectedImage"); }
        }

        public ObservableCollection<StudentImage> dgLockedPhotoData
        {
            get { return _dgLockedPhotoData; }
            set { _dgLockedPhotoData = value; NotifyPropertyChanged("dgLockedPhotoData"); }
        }
        #endregion

        #region Constructors
        public LockedPhotosViewModel()
        {
            dgLockedPhotoData = new ObservableCollection<StudentImage>(); selectedImages = new List<StudentImage>();
            lockedPhotosShowGroupPanel = false;

            bindGrid();
        }
        #endregion

        #region Methods
        internal void GetDefaultSchool()
        {
            bindGrid();
        }

        internal void bindGrid()
        {
            try
            {
                dgLockedPhotoData = new ObservableCollection<StudentImage>(clsDashBoard.getLockedPhotos(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId));
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        internal void searchPanels()
        {
            if (lockedPhotosSearchControl == null || !isSearchControlVisible) //!PhotoJobTableView.SearchControl.IsVisible
            {
                lockedPhotosSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
            else
            {
                lockedPhotosSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
            }
        }
        public void groupPanels()
        {
            try
            {
                if (lockedPhotosShowGroupPanel)
                    lockedPhotosShowGroupPanel = false;
                else
                    lockedPhotosShowGroupPanel = true;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        internal void openFolder()
        {
            if (selectedImage != null)
            {
                string path = selectedImage.PhotoShoot.ImageFolder;
                Process.Start(path);
            }
        }

        public void deleteImages()  //#Mohan ; #NUnitTest
        {
            try
            {
                string message = "", imageName = "";
                string path = "";
                PhotoSorterDBModelDataContext db1 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                if (selectedImage != null)
                {
                    int delPhotoId = 0;
                    ArrayList arrShootId = new ArrayList();
                    bool isError = false;
                    foreach (StudentImage stuImage in selectedImages)
                    {
                        try
                        {
                            int id = Convert.ToInt32(stuImage.ID);
                            imageName = Convert.ToString(stuImage.ImageName);
                            path = Convert.ToString(stuImage.PhotoShoot.ImageFolder);
                            if (!arrShootId.Contains(id))
                            {
                                arrShootId.Add(id);
                            }
                        }
                        catch (Exception ex)
                        {
                            isError = true;
                            clsStatic.WriteExceptionLogXML(ex);
                            MVVMMessageService.ShowMessage(ex.Message);
                        }
                    }
                    if (arrShootId.Count == 1)
                        message = "Are you sure you want to delete photoshoot " + imageName + "?";
                    else
                        message = "Are you sure you want to delete multiple photoshoots?";
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        //int totalRecordsCount = dgLockedPhotoData.Count();
                        //int deletedRecordsCount = arrShootId.Count;

                        delPhotoId = clsDashBoard.deleteStudentImages(db1, arrShootId);
                        if (File.Exists(path + "\\" + imageName))
                        {
                            File.Delete(path + "\\" + imageName);
                        }
                        foreach (int imgId in arrShootId)
                        {
                            dgLockedPhotoData.Remove(dgLockedPhotoData.Where(i => i.ID == imgId).First());
                        }
                        //bindGrid();
                        //createDeletedRecordsLogFile("Locked PhotoShoots", totalRecordsCount, deletedRecordsCount);
                    }
                    if (isError)
                    {
                        MVVMMessageService.ShowMessage(errorMessages.DELETION_WITH_ERRORS);
                    }
                }

            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        internal void deleteAllImages()
        {
            PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            IList<StudentImage> _lstDeletedStudentImages = clsDashBoard.getDeletedStudentImage(db);

            foreach (StudentImage image in _lstDeletedStudentImages)
            {
                try
                {
                    string imgid = "";
                    string imgname = image.ImageName;
                    string path = image.PhotoShoot.ImageFolder;
                    imgid = imgid + image.ID + ",";
                    if (File.Exists(path + "\\" + imgname))
                    {

                        File.Delete(path + "\\" + imgname);

                    }
                    imgid = imgid.Substring(0, imgid.Length - 1);
                    int delStuID = clsDashBoard.deleteStudentImage(db, image.ID);
                    bindGrid();
                }
                catch (Exception ex)
                {
                    MVVMMessageService.ShowMessage(ex.Message);
                    clsStatic.WriteExceptionLogXML(ex);
                }

            }
        }
        internal void setButtonVisibility()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
            (System.Windows.Application.Current as App).isDeleteVisible = true;
            (System.Windows.Application.Current as App).isDragVisible = true; (System.Windows.Application.Current as App).isSearchVisible = true;
            (System.Windows.Application.Current as App).isRefreshVisible = true; (System.Windows.Application.Current as App).isDeleteAllVisible = true;
        }
        #endregion
    }
}
