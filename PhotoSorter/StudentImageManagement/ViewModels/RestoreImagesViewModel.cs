using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Collections;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace PhotoForce.StudentImageManagement
{
    public class RestoreImagesViewModel : ViewModelBase
    {
        #region Initialization
        string folderPath = "";
        IEnumerable<GroupItem> studentGroupsColl;
        int groupId = 0;
        
        int imageIndex;
        string groupPath;
        ArrayList highResolutionImagesPath = new ArrayList();
        #endregion

        #region Properties
        private string _restoreFolder;
        private string _groupName;
        private ImageSource _imgCurrentSource;
        private ImageSource _imgReplaceWithSource;
        private string _currentImageName;
        private string _replaceImageName;
        private string _warningText;

        public string warningText
        {
            get { return _warningText; }
            set { _warningText = value; NotifyPropertyChanged("warningText"); }
        }
        public string replaceImageName
        {
            get { return _replaceImageName; }
            set { _replaceImageName = value; NotifyPropertyChanged("replaceImageName"); }
        }
        public string currentImageName
        {
            get { return _currentImageName; }
            set { _currentImageName = value; NotifyPropertyChanged("currentImageName"); }
        }
        public ImageSource imgReplaceWithSource
        {
            get { return _imgReplaceWithSource; }
            set { _imgReplaceWithSource = value; NotifyPropertyChanged("imgReplaceWithSource"); }
        }
        public ImageSource imgCurrentSource
        {
            get { return _imgCurrentSource; }
            set { _imgCurrentSource = value; NotifyPropertyChanged("imgCurrentSource"); }
        }
        public string groupName
        {
            get { return _groupName; }
            set { _groupName = value; NotifyPropertyChanged("groupName"); }
        }
        public string restoreFolder
        {
            get { return _restoreFolder; }
            set { _restoreFolder = value; }
        }
        #endregion

        #region Constructors
        public RestoreImagesViewModel(IEnumerable<GroupItem> tempStudentGroupsColl, int tempGroupId, string tempGroupName)
        {
            warningText = "This function will allow you to browse to a folder where the original high-res photos are stored. Using the serial number, the images will be matched to the correct student in the photoshoot . While copying the high-res photo back to the photoshoot path, it will be renamed in the format lastname_firstname_serialNo.jpg.";

            studentGroupsColl = tempStudentGroupsColl;
            groupId = tempGroupId;
            groupName = tempGroupName;
            applyDefaultImage();
        }
        #endregion

        #region Commands
        public RelayCommand OpenPhotoShootFolderCommand
        {
            get
            {
                return new RelayCommand(openPhotoShootFolder);
            }
        }
        public RelayCommand OpenHighResFolderCommand
        {
            get
            {
                return new RelayCommand(openHighResFolder);
            }
        }
        public RelayCommand StartCommand
        {
            get
            {
                return new RelayCommand(goToStart);
            }
        }
        public RelayCommand ShowNextCommand
        {
            get
            {
                return new RelayCommand(showNext);
            }
        }
        public RelayCommand BrowseImageFolderCommand
        {
            get
            {
                return new RelayCommand(browseImageFolder);
            }
        }
        public RelayCommand RestoreImagesCommand
        {
            get
            {
                return new RelayCommand(restoreImages);
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
        private void nextImage(GroupItem studentGroupData, string tempImage)
        {
            currentImageName = studentGroupData.StudentImage.ImageName;
            replaceImageName = tempImage.Substring(tempImage.LastIndexOf('\\') + 1);
            if (File.Exists(groupPath + "\\" + studentGroupData.StudentImage.ImageName))
            {
                DecoderForJpeg(groupPath + "\\" + studentGroupData.StudentImage.ImageName, "imgCurrent");
            }
            if (File.Exists(tempImage))
            {
                DecoderForJpeg(tempImage, "imgReplaceWith");
            }
        }

        private void showNext()
        {
            applyDefaultImage();
            currentImageName = "";
            replaceImageName = "";

            if (folderPath != "")
            {
                if (studentGroupsColl.Count() == imageIndex)
                {
                    string caption2 = "Confirmation";
                    string message2 = "Last image comparison reached. Start again from the beginning?";
                    System.Windows.MessageBoxButton buttons2 = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon2 = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message2, caption2, buttons2, icon2) == System.Windows.MessageBoxResult.Yes)
                    {
                        imageIndex = 0;
                    }
                    else
                    {
                        return;
                    }
                }
                //get group image data based on the index and find the respective image in High resolution folder.
                GroupItem studentGroupData = studentGroupsColl.ElementAt(imageIndex);
                string tempImage = (from string img in highResolutionImagesPath
                                    where studentGroupData.StudentImage.ImageNumber == img.Substring(img.LastIndexOf('_') + 1).Split('.')[0]
                                    select img).SingleOrDefault();
                if (studentGroupData != null)
                {
                    nextImage(studentGroupData, tempImage);
                }
                imageIndex++;
            }

        }

        private void restoreImages()
        {
            if (folderPath != "")
            {
                string caption2 = "Confirmation";
                string message2 = "This operation is not reversible. Are you sure?";
                System.Windows.MessageBoxButton buttons2 = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon2 = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message2, caption2, buttons2, icon2) == System.Windows.MessageBoxResult.Yes)
                {
                    int i = 0;
                    //loop through all the images in Group and replace if you find any counterpart in HighResFolder images
                    foreach (GroupItem grpItem in studentGroupsColl)
                    {
                        try
                        {
                            string highResImageName = "";
                            string highResImagePath = (from string img in highResolutionImagesPath
                                                       where grpItem.StudentImage.ImageNumber == img.Substring(img.LastIndexOf('_') + 1).Split('.')[0]
                                                       select img).SingleOrDefault();
                            if (!string.IsNullOrEmpty(highResImagePath))
                            {
                                i++;
                                highResImageName = grpItem.StudentImage.Student.Lastname + "_" + grpItem.StudentImage.Student.FirstName + "_" + grpItem.StudentImage.ImageNumber + grpItem.StudentImage.ImageName.Substring(grpItem.StudentImage.ImageName.LastIndexOf('.'));
                                clsDashBoard._UpdateStudentImgName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), grpItem.StudentImage.ID, highResImageName);
                                System.IO.File.Delete(groupPath + "\\" + grpItem.StudentImage.ImageName);
                                System.IO.File.Copy(highResImagePath, groupPath + "\\" + highResImageName,true);
                            }
                        }
                        catch (Exception ex)
                        {
                            MVVMMessageService.ShowMessage(ex.Message);
                            clsStatic.WriteExceptionLogXML(ex);
                        }
                    }
                    if (i != 0)
                    {
                        MVVMMessageService.ShowMessage("Images restored succesfully.");
                        DialogResult = false;
                    }
                    else
                    {
                        MVVMMessageService.ShowMessage("No matching images were found in the folder " + folderPath + ". Please select another folder and try again.");
                    }
                }
            }
        }

        private void browseImageFolder()
        {
            imageIndex = 0;
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            var res = dlg.ShowDialog();
            if (res != false)
                folderPath = dlg.SelectedPath;
            restoreFolder = folderPath;
            if (folderPath != "")
            {
                //mohan tangella

                string firstHighResImagePath = "";
                var FilesPath = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".jpg"));
                if (FilesPath.Count() == 0) { MVVMMessageService.ShowMessage("There are no images in selected folder."); return; }
                firstHighResImagePath = FilesPath.First();
                groupPath = (from p in studentGroupsColl select p.StudentImage.PhotoShoot.ImageFolder).FirstOrDefault();
                foreach (string highResImage in FilesPath)
                {
                    highResolutionImagesPath.Add(highResImage);
                }
                firstImage();
            }
        }

        private void openPhotoShootFolder()
        {
            if (!string.IsNullOrEmpty(groupPath))
            {
                System.Diagnostics.Process.Start(groupPath);
            }
        }

        private void openHighResFolder()
        {
            if (!string.IsNullOrEmpty(folderPath))
            {
                System.Diagnostics.Process.Start(folderPath);
            }
        }

        private void firstImage()
        {
            imageIndex = 0;
            GroupItem studentGroupData = studentGroupsColl.ElementAt(imageIndex);
            string tempImage = (from string img in highResolutionImagesPath
                                where studentGroupData.StudentImage.ImageNumber == img.Substring(img.LastIndexOf('_') + 1).Split('.')[0]
                                select img).SingleOrDefault();
            if (studentGroupData != null)
            {
                nextImage(studentGroupData, tempImage);
            }
            imageIndex++;
        }

        private void goToStart()
        {
            firstImage();
        }

        private void applyDefaultImage()
        {
            imgCurrentSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
            imgReplaceWithSource = new BitmapImage(new Uri("pack://application:,,,../../Images/imagenotfound.png"));
        }

        private void DecoderForJpeg(string strFile, string imgControlName)
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

                if (imgControlName == "imgCurrent")
                {
                    imgCurrentSource = thumb;
                }
                else
                {
                    imgReplaceWithSource = thumb;
                }
            }
            
        }

        private void windowClose()
        {
            DialogResult = false;
        }

        #endregion
    }
}
