using Ookii.Dialogs.Wpf;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.OrdersManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoForce.StudentImageManagement
{
    public class RestoreRetouchImagesViewModel : ViewModelBase
    {
        #region Initialization
        string folderPath = "";
        //int imageIndex;
        //string pfImagePath;
        Dictionary<string, StudentImage> _dictImageDetails = new Dictionary<string, StudentImage>();
        #endregion

        #region Properties
        private string _restoreFolder;
        private string _warningText;
        private string _browseButtonText;

        public string browseButtonText
        {
            get { return _browseButtonText; }
            set { _browseButtonText = value; NotifyPropertyChanged("browseButtonText"); }
        }
        public string warningText
        {
            get { return _warningText; }
            set { _warningText = value; NotifyPropertyChanged("warningText"); }
        }
        public string restoreFolder
        {
            get { return _restoreFolder; }
            set { _restoreFolder = value; NotifyPropertyChanged("restoreFolder"); }
        }
        #endregion

        #region Constructors
        public RestoreRetouchImagesViewModel()
        {
            //warningText = "This function will allow you to browse to a folder where the retouch photos are stored. Using the serial number, the images will be matched to the correct student image in the photoforce .";
            warningText = "This function will allow you to browse for a folder where the retouched images are stored. Using the serial number, the location of the student images will determined. The retouched images will then be copied to this location, overwriting the originals.";
        }
        #endregion

        #region Commands
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
        private void restoreImages()
        {
            if (!string.IsNullOrEmpty(folderPath))
            {
                ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();
                string caption2 = "Confirmation";
                string message2 = "This operation is not reversible. Are you sure?";
                System.Windows.MessageBoxButton buttons2 = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon2 = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message2, caption2, buttons2, icon2) == System.Windows.MessageBoxResult.Yes)
                {
                    List<string> missingImages = new List<string>();
                    //loop through all the images in Group and replace if you find any counterpart in HighResFolder images
                    foreach (string touchSyncOriginalImagePath in _dictImageDetails.Keys)
                    {
                        try
                        {
                            string[] folderTypes = new string[] { "\\", "\\_reduced\\", "\\_dd\\" };

                            string sourceImageName = touchSyncOriginalImagePath.Substring(touchSyncOriginalImagePath.LastIndexOf("\\") + 1);
                            StudentImage tempStuImage = _dictImageDetails[touchSyncOriginalImagePath];

                            // Need to move photos from this location as per by lastname and firstname
                            if (tempStuImage != null)
                            {
                                foreach (string folderType in folderTypes)
                                {
                                    string destImagePath = tempStuImage.PhotoShoot.ImageFolder + folderType + tempStuImage.ImageName;
                                    string sourceImagePath = folderPath + folderType + sourceImageName;

                                    if (File.Exists(destImagePath))
                                    {
                                        if (File.Exists(sourceImagePath)) { File.Delete(destImagePath); }
                                        File.Copy(sourceImagePath, destImagePath, true);   //Changed by Mohan
                                    }
                                    else
                                    {
                                        if (folderType == "\\_dd\\")
                                        {
                                            if (Directory.Exists(tempStuImage.PhotoShoot.ImageFolder))
                                            {
                                                Directory.CreateDirectory(tempStuImage.PhotoShoot.ImageFolder + folderType);
                                                if (File.Exists(sourceImagePath)) { File.Delete(destImagePath); }
                                                File.Copy(sourceImagePath, destImagePath, true);
                                            }
                                            else
                                            {
                                                if (!missingImages.Contains(sourceImagePath)) { missingImages.Add(sourceImagePath); }
                                            }
                                        }
                                        else
                                        {
                                            if (!missingImages.Contains(sourceImagePath)) { missingImages.Add(sourceImagePath); }
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (!missingImages.Contains(touchSyncOriginalImagePath)) { missingImages.Add(touchSyncOriginalImagePath); }
                            }
                        }
                        catch (Exception ex)
                        {
                            MVVMMessageService.ShowMessage(ex.Message);
                            clsStatic.WriteExceptionLogXML(ex);
                        }
                    }
                    string tempMissingImages = string.Join("\n", missingImages.ToArray());
                    if (string.IsNullOrEmpty(tempMissingImages))
                    {
                        MVVMMessageService.ShowMessage(" Images copied succesfully.");
                    }
                    else
                    {
                        //MVVMMessageService.ShowMessage("Matching images were not found for the following images.\n " + missingImages);
                        MissingOrders _objMissingOrders = new MissingOrders(tempMissingImages,"");
                        _objMissingOrders.ShowDialog();
                    }
                    DialogResult = false;
                }
            }
        }

        private void browseImageFolder()
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            var res = dlg.ShowDialog();
            if (res != false)
                folderPath = dlg.SelectedPath;
            restoreFolder = folderPath;
            if (!string.IsNullOrEmpty(folderPath))
            {
                //mohan tangella
                var FilesPath = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".jpg"));
                if (FilesPath.Count() == 0) { MVVMMessageService.ShowMessage("There are no images in selected folder."); return; }

                foreach (string highResImage in FilesPath)
                {
                    string tempImageName = highResImage.Substring(highResImage.LastIndexOf("\\") + 1);

                    //getting image data based on the name.
                    StudentImage pfImage = clsDashBoard.getStudentImageDetailsByName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempImageName);
                    if (!_dictImageDetails.ContainsKey(highResImage)) { _dictImageDetails.Add(highResImage, pfImage); }
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
