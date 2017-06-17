using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.WorkPlace;
using PhotoForce.MVVM;
using PhotoForce.WorkPlace.UserControls;
using PhotoForce.Mask_Management;
using PhotoForce.GroupManagement;
using PhotoForce.StudentImageManagement;
using PhotoForce.App_Code;
using RandomLicenseGenerator;
using PhotoForce.License_Management;
using System.IO;
using PhotoForce.Settings.UserControls;
using PhotoForce.View_Management.UserControls;
using System.Windows;
using System.Net;
using PhotoForce.Settings;
using PhotoForce.Student_Management;
using PhotoForce.Connection_Management;
using PhotoForce.OrdersManagement;
using PhotoForce.PhotographyJobManagement;
using PhotoForce.WorkflowManagement;

namespace PhotoForce
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Initialization
        SearchSchool _objSearchSchool;
        Boolean isErrorExist;
        PhotoSorterDBModelDataContext db;
        ViewModelLocator _viewModelocator = App.Current.Resources["ViewModelLocator"] as ViewModelLocator;

        string ftpUri = "ftp://ftp.freedphoto.com/web_users/pflogfile/";
        string ftpUserName = "simplephoto";
        string ftpPassword = "ociK3#37";
        #endregion

        #region Properties
        private ObservableCollection<object> _children;

        private string _userName;
        private string _statusInfo;
        private string _studentPanelCaption;
        private bool _isStudentWithoutImageEnabled;
        private bool _isImageWithoutStudentEnabled;
        int _viewIndex;
        int _dashBoardViewIndex;
        int _schoolIndex;
        int _orderIndex;
        private bool _isOrderPackagesVisible;
        int _settingsIndex;
        Visibility _txtTestConnectionVisibility;
        string _txtTestConnection;
        string _manageCollection;
        string _manageEquipmentItems;
        string _studioInfo;
        public string manageEquipmentItems
        {
            get { return _manageEquipmentItems; }
            set { _manageEquipmentItems = value; NotifyPropertyChanged();}
        }
        public string manageCollection
        {
            get { return _manageCollection; }
            set { _manageCollection = value; NotifyPropertyChanged(); }
        }

        public string txtTestConnection
        {
            get { return _txtTestConnection; }
            set { _txtTestConnection = value; NotifyPropertyChanged("txtTestConnection"); }
        }
        public Visibility txtTestConnectionVisibility
        {
            get { return _txtTestConnectionVisibility; }
            set { _txtTestConnectionVisibility = value; NotifyPropertyChanged("txtTestConnectionVisibility"); }
        }
        public int settingsIndex
        {
            get { return _settingsIndex; }
            set { _settingsIndex = value; NotifyPropertyChanged("settingsIndex"); }
        }

        public bool isOrderPackagesVisible
        {
            get { return _isOrderPackagesVisible; }
            set { _isOrderPackagesVisible = value; NotifyPropertyChanged("isOrderPackagesVisible"); }
        }
        public int activityIndex
        {
            get { return _schoolIndex; }
            set { _schoolIndex = value; NotifyPropertyChanged("activityIndex"); }
        }
        public int viewIndex
        {
            get { return _viewIndex; }
            set { _viewIndex = value; NotifyPropertyChanged("viewIndex"); }
        }
        public int dashBoardViewIndex
        {
            get { return _dashBoardViewIndex; }
            set { _dashBoardViewIndex = value; NotifyPropertyChanged("dashBoardViewIndex"); }
        }
        public int orderIndex
        {
            get { return _orderIndex; }
            set { _orderIndex = value; NotifyPropertyChanged("orderIndex"); }
        }

        #region Panel Captions
        private string _panelDashboardCaption;
        private string _panelStudentCaption;
        private string _panelSchoolCaption;
        private string _panelPhotoShootsCaption;
        private string _panelPhotoGraphyJobCaption;
        private string _panelGroupsCaption;
        private string _panelLockedPhotosCaption;
        private string _panelDeactivateStudentsCaption;
        private string _panelImportBatchesCaption;
        private string _panelMaskCaption;
        private string _panelFileLocCaption;
        private string _panelDefaultpackageCaption;

        public string panelDefaultpackageCaption
        {
            get { return _panelDefaultpackageCaption; }
            set { _panelDefaultpackageCaption = value; NotifyPropertyChanged("panelDefaultpackageCaption"); }
        }
        public string panelFileLocCaption
        {
            get { return _panelFileLocCaption; }
            set { _panelFileLocCaption = value; NotifyPropertyChanged("panelFileLocCaption"); }
        }
        public string panelMaskCaption
        {
            get { return _panelMaskCaption; }
            set { _panelMaskCaption = value; NotifyPropertyChanged("panelMaskCaption"); }
        }
        public string panelImportBatchesCaption
        {
            get { return _panelImportBatchesCaption; }
            set { _panelImportBatchesCaption = value; NotifyPropertyChanged("panelImportBatchesCaption"); }
        }
        public string panelDeactivateStudentsCaption
        {
            get { return _panelDeactivateStudentsCaption; }
            set { _panelDeactivateStudentsCaption = value; NotifyPropertyChanged("panelDeactivateStudentsCaption"); }
        }
        public string panelLockedPhotosCaption
        {
            get { return _panelLockedPhotosCaption; }
            set { _panelLockedPhotosCaption = value; NotifyPropertyChanged("panelLockedPhotosCaption"); }
        }
        public string panelGroupsCaption
        {
            get { return _panelGroupsCaption; }
            set { _panelGroupsCaption = value; NotifyPropertyChanged("panelGroupsCaption"); }
        }
        public string panelPhotoGraphyJobCaption
        {
            get { return _panelPhotoGraphyJobCaption; }
            set { _panelPhotoGraphyJobCaption = value; NotifyPropertyChanged("panelPhotoGraphyJobCaption"); }
        }
        public string panelPhotoShootsCaption
        {
            get { return _panelPhotoShootsCaption; }
            set { _panelPhotoShootsCaption = value; NotifyPropertyChanged("panelPhotoShootsCaption"); }
        }
        public string panelSchoolCaption
        {
            get { return _panelSchoolCaption; }
            set { _panelSchoolCaption = value; NotifyPropertyChanged("panelSchoolCaption"); }
        }
        public string panelStudentCaption
        {
            get { return _panelStudentCaption; }
            set { _panelStudentCaption = value; NotifyPropertyChanged("panelStudentCaption"); }
        }
        public string panelDashboardCaption
        {
            get { return _panelDashboardCaption; }
            set { _panelDashboardCaption = value; NotifyPropertyChanged("panelDashboardCaption"); }
        }
        #endregion

        public int selectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value; NotifyPropertyChanged("selectedIndex");
                if (selectedIndex == 0)
                {
                    menuDashBoard();
                }
                if (selectedIndex == 1)
                {
                    menuStudent();
                }
                if (selectedIndex == 2)
                {
                    menuGroups();
                }
            }
        }

        private bool _isDashboardVisible;
        private bool _isStudentVisible;
        private bool _isGroupVisible;
        private bool _isMaskSettingsVisible;
        private bool _isPhotographyJobVisible;
        private bool _isPhotoShootVisible;
        private bool _isLockedPhotosVisible;
        private bool _isSchoolsVisible;
        private bool _isActivateStudentVisible;
        private bool _isUniversalStudentVisible;
        private bool _isImportBatchVisible;
        private bool _isFileLocationVisible;
        private bool _isDefaultPriceGroupVisible;
        private int _selectedIndex;
        private bool _isDockViewsSettingsVisible;
        private bool _isActivitiesVisible;
        private bool _isUniversalImageVisible;
        private bool _isOrdersVisible;
        private bool _isValidatePhotoshootsVisible;
        private bool _isViewOrderByStudentVisible;
        private bool _isViewOrderByGalleryGroupVisible;
        private bool _isSearchOrdersVisible;
        private bool _isWorkflowItemsVisible;
        private bool _isWorkflowCollectionsVisible;
        private bool _isDashboardWorkflowsVisible;
        private bool _isImageQuixAccountsVisible;
        private bool _isManageUsersVisible;
        private bool _isSPPriceSheetVisible;
        private bool _isStudiosVisible;
        private bool _isEquipmentItemsVisible;
        private bool _isDashboardPhotoshootsWOWFVisible;

        public bool isDashboardPhotoshootsWOWFVisible
        {
            get { return _isDashboardPhotoshootsWOWFVisible; }
            set { _isDashboardPhotoshootsWOWFVisible = value; NotifyPropertyChanged(); }
        }
        public bool isEquipmentItemsVisible
        {
            get { return _isEquipmentItemsVisible; }
            set { _isEquipmentItemsVisible = value; NotifyPropertyChanged(); }
        }

        public bool isStudiosVisible
        {
            get { return _isStudiosVisible; }
            set { _isStudiosVisible = value; NotifyPropertyChanged(); }
        }
        public bool isSPPriceSheetVisible
        {
            get { return _isSPPriceSheetVisible; }
            set { _isSPPriceSheetVisible = value; NotifyPropertyChanged(); }
        }
        public bool isManageUsersVisible
        {
            get { return _isManageUsersVisible; }
            set { _isManageUsersVisible = value; NotifyPropertyChanged(); }
        }
        public bool isImageQuixAccountsVisible
        {
            get { return _isImageQuixAccountsVisible; }
            set { _isImageQuixAccountsVisible = value; NotifyPropertyChanged("isImageQuixAccountsVisible"); }
        }
        public bool isDashboardWorkflowsVisible
        {
            get { return _isDashboardWorkflowsVisible; }
            set { _isDashboardWorkflowsVisible = value; NotifyPropertyChanged("isDashboardWorkflowsVisible"); }
        }
        public bool isWorkflowCollectionsVisible
        {
            get { return _isWorkflowCollectionsVisible; }
            set { _isWorkflowCollectionsVisible = value; NotifyPropertyChanged("isWorkflowCollectionsVisible"); }
        }
        public bool isWorkflowItemsVisible
        {
            get { return _isWorkflowItemsVisible; }
            set { _isWorkflowItemsVisible = value; NotifyPropertyChanged("isWorkflowItemsVisible"); }
        }

        public bool isSearchOrdersVisible
        {
            get { return _isSearchOrdersVisible; }
            set { _isSearchOrdersVisible = value; NotifyPropertyChanged(); }
        }
        public bool isViewOrderByGalleryGroupVisible
        {
            get { return _isViewOrderByGalleryGroupVisible; }
            set { _isViewOrderByGalleryGroupVisible = value; NotifyPropertyChanged(); }
        }
        public bool isViewOrderByStudentVisible
        {
            get { return _isViewOrderByStudentVisible; }
            set { _isViewOrderByStudentVisible = value; NotifyPropertyChanged("isViewOrderByStudentVisible"); }
        }
        public bool isValidatePhotoshootsVisible
        {
            get { return _isValidatePhotoshootsVisible; }
            set { _isValidatePhotoshootsVisible = value; NotifyPropertyChanged("isValidatePhotoshootsVisible"); }
        }
        public bool isOrdersVisible
        {
            get { return _isOrdersVisible; }
            set { _isOrdersVisible = value; NotifyPropertyChanged("isOrdersVisible"); }
        }
        public bool isActivitiesVisible
        {
            get { return _isActivitiesVisible; }
            set { _isActivitiesVisible = value; NotifyPropertyChanged("isActivitiesVisible"); }
        }
        public bool isDockViewsSettingsVisible
        {
            get { return _isDockViewsSettingsVisible; }
            set { _isDockViewsSettingsVisible = value; NotifyPropertyChanged("isDockViewsSettingsVisible"); }
        }
        public bool isDefaultPriceGroupVisible
        {
            get { return _isDefaultPriceGroupVisible; }
            set { _isDefaultPriceGroupVisible = value; NotifyPropertyChanged("isDefaultPriceGroupVisible"); }
        }
        public bool isFileLocationVisible
        {
            get { return _isFileLocationVisible; }
            set { _isFileLocationVisible = value; NotifyPropertyChanged("isFileLocationVisible"); }
        }
        public bool isImportBatchVisible
        {
            get { return _isImportBatchVisible; }
            set { _isImportBatchVisible = value; NotifyPropertyChanged("isImportBatchVisible"); }
        }
        public bool isUniversalStudentVisible
        {
            get { return _isUniversalStudentVisible; }
            set { _isUniversalStudentVisible = value; NotifyPropertyChanged("isUniversalStudentVisible"); }
        }
        public bool isUniversalImageVisible
        {
            get { return _isUniversalImageVisible; }
            set { _isUniversalImageVisible = value; NotifyPropertyChanged("isUniversalImageVisible"); }
        }
        public bool isActivateStudentVisible
        {
            get { return _isActivateStudentVisible; }
            set { _isActivateStudentVisible = value; NotifyPropertyChanged("isActivateStudentVisible"); }
        }
        public bool isSchoolsVisible
        {
            get { return _isSchoolsVisible; }
            set { _isSchoolsVisible = value; NotifyPropertyChanged("isSchoolsVisible"); }
        }
        public bool isLockedPhotosVisible
        {
            get { return _isLockedPhotosVisible; }
            set { _isLockedPhotosVisible = value; NotifyPropertyChanged("isLockedPhotosVisible"); }
        }
        public bool isPhotoShootVisible
        {
            get { return _isPhotoShootVisible; }
            set { _isPhotoShootVisible = value; NotifyPropertyChanged("isPhotoShootVisible"); }
        }
        public bool isPhotographyJobVisible
        {
            get { return _isPhotographyJobVisible; }
            set { _isPhotographyJobVisible = value; NotifyPropertyChanged("isPhotographyJobVisible"); }
        }
        public bool isMaskSettingsVisible
        {
            get { return _isMaskSettingsVisible; }
            set { _isMaskSettingsVisible = value; NotifyPropertyChanged("isMaskSettingsVisible"); }
        }
        public bool isGroupVisible
        {
            get { return _isGroupVisible; }
            set { _isGroupVisible = value; NotifyPropertyChanged("isGroupVisible"); }
        }
        public bool isStudentVisible
        {
            get { return _isStudentVisible; }
            set { _isStudentVisible = value; NotifyPropertyChanged("isStudentVisible"); }
        }
        public bool isDashboardVisible
        {
            get { return _isDashboardVisible; }
            set { _isDashboardVisible = value; NotifyPropertyChanged("isDashboardVisible"); }
        }
        public bool isImageWithoutStudentEnabled
        {
            get { return _isImageWithoutStudentEnabled; }
            set { _isImageWithoutStudentEnabled = value; NotifyPropertyChanged("isImageWithoutStudentEnabled"); }
        }
        public bool isStudentWithoutImageEnabled
        {
            get { return _isStudentWithoutImageEnabled; }
            set { _isStudentWithoutImageEnabled = value; NotifyPropertyChanged("isStudentWithoutImageEnabled"); }
        }
        public string studentPanelCaption
        {
            get { return _studentPanelCaption; }
            set { _studentPanelCaption = value; NotifyPropertyChanged("studentPanelCaption"); }
        }
        public string statusInfo
        {
            get { return _statusInfo; }
            set { _statusInfo = value; NotifyPropertyChanged("statusInfo"); }
        }
        public string studioInfo
        {
            get { return _studioInfo; }
            set { _studioInfo = value; NotifyPropertyChanged("studioInfo"); }
        }
        public string userName
        {
            get { return _userName; }
            set { _userName = value; NotifyPropertyChanged("userName"); }
        }

        public ObservableCollection<object> childViewModels
        {
            get { return _children; }
            set { _children = value; }
        }
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                //////////license key //////////////

                bool checkForTable = clsLicensing.CheckForLicense(db);    //checks wether License table is existed or not.,if not creates the table
                if (checkForTable)
                {
                    //check for no. of credits 
                    List<Licensing> licensingList = clsLicensing.checkCreditsCount(db);

                    Licensing licensing = new Licensing();
                    try
                    {
                        foreach (Licensing i in licensingList)
                        {
                            licensing.Credits = i.Credits;
                            licensing.FirstName = i.FirstName;
                            licensing.LastName = i.LastName;
                        }
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.ToString());
                    }
                    userName = licensing.FirstName + " " + licensing.LastName;
                    credits = Convert.ToInt32(RSAEncryptDecrypt.Decrypt(licensing.Credits));
                }
                else
                {
                    CreateUser createUser = new CreateUser(clsLicensing.isCreditLogExists);
                    createUser.ShowDialog();
                    userName = ((CreateUserViewModel)(createUser.DataContext)).firstName + " " + ((CreateUserViewModel)(createUser.DataContext)).lastName;
                    credits = ((CreateUserViewModel)(createUser.DataContext)).credits;
                }

                isDockViewsSettingsVisible = false;
                bindPanelCaption(clsSchool.defaultSchoolName);

                if (isErrorExist)
                {
                    string message = errorMessages.DELETE_PHOTOS_ERROR;
                    MVVMMessageService.ShowMessage(message);
                }
                loadDashBoard();
                statusInfo = "Dashboard" + " - " + clsSchool.defaultSchoolName;

                School s = (from st in db.Schools where st.ID == clsSchool.defaultSchoolId select st).FirstOrDefault();
                int studioID = Convert.ToInt32(s.StudioId);
                Studio na = (from st in db.Studios where st.Id == studioID select st).FirstOrDefault();
                studioInfo = ", Studio" + " - " + na.StudioName;

                testForTempDB();
                manageCollection = "Manage Workflows"; // using same button for two purposes in workflows
                manageEquipmentItems = "Manage Equipments";
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

        }
        #endregion

        #region Commands

        #region NavBar Group Commands
        public RelayCommand<string> DashBoardVisibilityCommand
        {
            get
            {
                return new RelayCommand<string>(workPlaceVisibility);
            }
        }
        public RelayCommand MenuDashBoardCommand
        {
            get
            {
                return new RelayCommand(menuDashBoard);
            }
        }
        public RelayCommand MenuStudentCommand
        {
            get
            {
                return new RelayCommand(menuStudent);
            }
        }
        public RelayCommand MenuGroupsCommand
        {
            get
            {
                return new RelayCommand(menuGroups);
            }
        }
        public RelayCommand<string> UniversalStudentSearchCommand
        {
            get
            {
                return new RelayCommand<string>(universalStudentSearch);
            }
        }
        public RelayCommand MenuSchoolsCommand
        {
            get
            {
                return new RelayCommand(menuSchools);
            }
        }
        public RelayCommand<string> MenuPhotographyJobCommand
        {
            get
            {
                return new RelayCommand<string>(menuPhotographyJob);
            }
        }
        public RelayCommand MenuViewPhotoShootsCommand
        {
            get
            {
                return new RelayCommand(menuPhotoShoots);
            }
        }
        public RelayCommand MenuLockedPhotosCommand
        {
            get
            {
                return new RelayCommand(menuLockedPhotos);
            }
        }
        public RelayCommand MenuActivateStudentsCommand
        {
            get
            {
                return new RelayCommand(menuActivateStudents);
            }
        }
        public RelayCommand<string> MenuImportBatchesCommand
        {
            get
            {
                return new RelayCommand<string>(menuImportBatches);
            }
        }
        //public RelayCommand MenuFileLocationSettingsCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(menuFileLocationSettings);
        //    }
        //}

        public RelayCommand MenuOrderPackagesCommand
        {
            get
            {
                return new RelayCommand(menuOrderPackages);
            }
        }
        public RelayCommand MenuMaskSettingsCommand
        {
            get
            {
                return new RelayCommand(menuMaskSettings);
            }
        }
        public RelayCommand MenuOrderFormDefaultPricingGroupCommand
        {
            get
            {
                return new RelayCommand(menuOrderFormDefaultPricingGroup);
            }
        }
        public RelayCommand<string> MenuActivitiesCommand
        {
            get
            {
                return new RelayCommand<string>(menuActivities);
            }
        }
        public RelayCommand MenuOrdersCommand
        {
            get
            {
                return new RelayCommand(menuOrders);
            }
        }
        public RelayCommand MenuViewOrdersByStudentCommand
        {
            get
            {
                return new RelayCommand(menuViewOrdersByStudent);
            }
        }
        public RelayCommand MenuViewOrdersByGalleryGroupCommand
        {
            get
            {
                return new RelayCommand(menuViewOrdersByGalleryGroup);
            }
        }
        public RelayCommand MenuSearchOrdersCommand
        {
            get
            {
                return new RelayCommand(menuSearchOrders);
            }
        }
        public RelayCommand MenuWorkflowCommand
        {
            get
            {
                return new RelayCommand(WorkflowItems);
            }
        }
        public RelayCommand<string> WorkflowCollectionsCommand
        {
            get
            {
                return new RelayCommand<string>(menuWorkflowCollections);
            }
        }
        //public RelayCommand ValidatePhotoshootsCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(ValidateAdminCD);
        //    }
        //}
        //added by hema //on 16/03/2016
        public RelayCommand MenuImageQuixCommand
        {
            get
            {
                return new RelayCommand(menuImageQuix);
            }
        }
        public RelayCommand MenuManageUsersCommand
        {
            get
            {
                return new RelayCommand(menuManageUsers);
            }
        }
        public RelayCommand MenuSPPriceSheetCommand
        {
            get
            {
                return new RelayCommand(menuSPPriceSheet);
            }
        }
        public RelayCommand MenuStudioCommand
        {
            get
            {
                return new RelayCommand(menuStudios);
            }
        }
        public RelayCommand MenuEquipmentCommand
        {
            get
            {
                return new RelayCommand(EquipmentItems);
            }
        }
        public RelayCommand MenuStudiosCommand
        {
            get
            {
                return new RelayCommand(menuStudios);
            }
        }
        #endregion

        #region BarButton Item Commands
        public RelayCommand NewCommand
        {
            get
            {
                return new RelayCommand(newCommand);
            }
        }
        public RelayCommand EditCommand
        {
            get
            {
                return new RelayCommand(edit);
            }
        }
        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(delete);
            }
        }
        public RelayCommand DeleteAllCommand
        {
            get
            {
                return new RelayCommand(deleteAll);
            }
        }
        public RelayCommand RemoveFromGroupCommand
        {
            get
            {
                return new RelayCommand(removeFromGroup);
            }
        }

        public RelayCommand DragCommand
        {
            get
            {
                return new RelayCommand(drag);
            }
        }
        public RelayCommand SearchCommand
        {
            get
            {
                return new RelayCommand(search);
            }
        }
        public RelayCommand ImportFileCommand
        {
            get
            {
                return new RelayCommand(importFile);
            }
        }
        public RelayCommand UploadImagesCommand
        {
            get
            {
                return new RelayCommand(uploadImages);
            }
        }
        public RelayCommand ImportStudentsCommand
        {
            get
            {
                return new RelayCommand(importStudents);
            }
        }
        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(refresh);
            }
        }
        public RelayCommand MPOFCommand
        {
            get
            {
                return new RelayCommand(generatePDF);
            }
        }
        public RelayCommand ExportPhotosCommand
        {
            get
            {
                return new RelayCommand(exportPhotos);
            }
        }
        public RelayCommand BulkRenameCommand
        {
            get
            {
                return new RelayCommand(bulkRename);
            }
        }
        public RelayCommand CountImagesCommand
        {
            get
            {
                return new RelayCommand(countImages);
            }
        }
        public RelayCommand AddToGroupCommand
        {
            get
            {
                return new RelayCommand(addToGroup);
            }
        }
        public RelayCommand GeneratePasswordCommand
        {
            get
            {
                return new RelayCommand(generatePassword);
            }
        }
        public RelayCommand GroupPhotoCommand
        {
            get
            {
                return new RelayCommand(groupPhoto);
            }
        }
        public RelayCommand ExportStudentsCommand
        {
            get
            {
                return new RelayCommand(exportStudents);
            }
        }
        public RelayCommand RatingCommand
        {
            get
            {
                return new RelayCommand(rating);
            }
        }
        public RelayCommand YearBookCommand
        {
            get
            {
                return new RelayCommand(yearBook);
            }
        }
        public RelayCommand CreateTeacherGroupsCommand
        {
            get
            {
                return new RelayCommand(createTeacherGroups);
            }
        }
        public RelayCommand RenameSourceImagesCommand
        {
            get
            {
                return new RelayCommand(renameSourceImages);
            }
        }
        public RelayCommand GenerateReducedImagesCommand
        {
            get
            {
                return new RelayCommand(generateReducedImages);
            }
        }
        public RelayCommand GenerateReducedImagesAllCommand
        {
            get
            {
                return new RelayCommand(generateReducedImagesAll);
            }
        }
        //public RelayCommand StudentsWithoutImageCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(studentsWithoutImage);
        //    }
        //}
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return new RelayCommand(openFolder);
            }
        }
        public RelayCommand SyncDeletedImagesCommand
        {
            get
            {
                return new RelayCommand(syncDeletedImages);
            }
        }
        public RelayCommand ReSyncImageNamesCommand
        {
            get
            {
                return new RelayCommand(reSyncImageNames);
            }
        }
        public RelayCommand ReAssignPhotoshootToSchoolYearCommand
        {
            get
            {
                return new RelayCommand(reAssignPhotoShootToSchoolYear);
            }
        }
        public RelayCommand AboutUsCommand
        {
            get
            {
                return new RelayCommand(aboutUs);
            }
        }
        public RelayCommand GenerateQRCodeCommand
        {
            get
            {
                return new RelayCommand(generateQRCode);
            }
        }
        public RelayCommand AssignStudentCommand
        {
            get
            {
                return new RelayCommand(assignStudent);
            }
        }
        public RelayCommand DeactivateStudentCommand
        {
            get
            {
                return new RelayCommand(deActivateStudent);
            }
        }
        public RelayCommand ActivateStudentCommand
        {
            get
            {
                return new RelayCommand(activateStudent);
            }
        }
        public RelayCommand PrintDeviceQRCommand
        {
            get
            {
                return new RelayCommand(printDeviceQR);
            }
        }
        //Deleted after 4.50 as we are using single UI for all photshoot validations //Mohan
        //public RelayCommand CountAdminCDCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(countAdminCD);
        //    }
        //}
        //public RelayCommand CountYearbookCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(countYearBook);
        //    }
        //}
        //public RelayCommand ValidatePhotoshootsCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(validatePhotoshoots);
        //    }
        //}
        public RelayCommand DefaultSchoolCommand
        {
            get
            {
                return new RelayCommand(defaultSchool);
            }
        }
        public RelayCommand DefaultPackageCommand
        {
            get
            {
                return new RelayCommand(defalutPackage);
            }
        }
        public RelayCommand IPTCCommand
        {
            get
            {
                return new RelayCommand(IPTC);
            }
        }
        public RelayCommand AddCreditsCommand
        {
            get
            {
                return new RelayCommand(addCredits);
            }
        }
        public RelayCommand OpenDatabaseCommand
        {
            get
            {
                return new RelayCommand(openDatabase);
            }
        }
        public RelayCommand ReImportStudentsCommand
        {
            get
            {
                return new RelayCommand(reImportStudents);
            }
        }
        public RelayCommand ValidateDataFileCommand
        {
            get
            {
                return new RelayCommand(validateDataFile);
            }
        }
        public RelayCommand CheckForRenamePhotoshootsCommand
        {
            get
            {
                return new RelayCommand(checkForRenamePhotoshoots);
            }
        }
        public RelayCommand AutoAssignClassPhotosCommand
        {
            get
            {
                return new RelayCommand(autoAssignClassPhotos);
            }
        }
        public RelayCommand RestoreSourceImagesCommand
        {
            get
            {
                return new RelayCommand(restoreSourceImages);
            }
        }
        public RelayCommand AddToOrdersCommand
        {
            get
            {
                return new RelayCommand(addToOrders);
            }
        }
        public RelayCommand AutoCreateGroupPhotosCommand
        {
            get
            {
                return new RelayCommand(autoCreateGroupPhotos);
            }
        }
        public RelayCommand AddGroupPhotoCommand
        {
            get
            {
                return new RelayCommand(addClassPhotoToOrderItem);
            }
        }
        public RelayCommand ImportOrdersCommand
        {
            get
            {
                return new RelayCommand(importOrdersWithDownload);
            }
        }
        public RelayCommand ShiprushTrackingCommand
        {
            get
            {
                return new RelayCommand(shiprushTracking);
            }
        }
        public RelayCommand EditBillingCodesCommand
        {
            get
            {
                return new RelayCommand(editBillingCode);
            }
        }
        public RelayCommand SendLogFileCommand
        {
            get
            {
                return new RelayCommand(sendLogFile);
            }
        }
        public RelayCommand HasMissingImagesCommand
        {
            get
            {
                return new RelayCommand(hasMissingImages);
            }
        }
        //Added By Hema
        public RelayCommand FileLocationSettingsCommand
        {
            get
            {
                return new RelayCommand(fileLocationSettings);
            }
        }
        public RelayCommand EditOrdersCommand
        {
            get
            {
                return new RelayCommand(editOrders);
            }
        }
        public RelayCommand AssignOrderImagesCommand
        {
            get
            {
                return new RelayCommand(assignOrderImages);
            }
        }
        public RelayCommand CopyRetouchImagesCommand
        {
            get
            {
                return new RelayCommand(restoreRetouchImages);
            }
        }
        public RelayCommand ImagesWithoutOrdersCommand
        {
            get
            {
                return new RelayCommand(imagesWithoutOrders);
            }
        }
        public RelayCommand DataBaseBackUpCommand
        {
            get
            {
                return new RelayCommand(dataBaseBackUp);
            }
        }
        //public RelayCommand HasNotesCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(hasNotes);
        //    }
        //}
        public RelayCommand ImportYearbookChoiceCommand
        {
            get
            {
                return new RelayCommand(importYearbookChoice);
            }
        }
        public RelayCommand UpdateOrdersThroughExcelCommand
        {
            get
            {
                return new RelayCommand(updateOrdersThroughExcel);
            }
        }
        public RelayCommand MissingOrdersCommand
        {
            get
            {
                return new RelayCommand(missingOrders);
            }
        }
        public RelayCommand MergeSchoolsCommand
        {
            get
            {
                return new RelayCommand(mergeSchools);
            }
        }
        public RelayCommand AddCollectionsCommand
        {
            get
            {
                return new RelayCommand(addCollections);
            }
        }
        public RelayCommand SwitchStudentFLNamesCommand
        {
            get
            {
                return new RelayCommand(switchStudentFLNames);
            }
        }
        public RelayCommand DeleteManulStudentsCommand
        {
            get
            {
                return new RelayCommand(deleteManualStudents);
            }
        }
        public RelayCommand TrackStudentsacrossYearsCommand
        {
            get
            {
                return new RelayCommand(trackStudentacrossYears);
            }
        }
        public RelayCommand AddEquipmentItemsCommand
        {
            get
            {
                return new RelayCommand(manageEquipments);
            }
        }
        public RelayCommand ClearingCorrectStudentsCommand
        {
            get
            {
                return new RelayCommand(clearingCorrectStudents);
            }
        }
        public RelayCommand DuplicatePhotoShootsCommand
        {
            get
            {
                return new RelayCommand(createDuplicatePhotoShoots);
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Nav Bar GroupRelated Methods
        //Depend upon the user selection we will show different user controls .
        private void workPlaceVisibility(string tempIndex)
        {

            if (tempIndex == "0") { dashBoardViewIndex = 0; isDockViewsSettingsVisible = false; selectedIndex = Convert.ToInt32(tempIndex); }

            //to display validations usercontrol
            if (tempIndex == "1")
            {
                ValidatePhotoshoot();
            }
            if (tempIndex == "2")
            {
                dashBoardWorkflows();
            }
            if (tempIndex == "3")
            {
                dashBoardPhotoShootsWOWF();
            }
        }
        private void menuDashBoard()
        {
            dockPanleVisibility();
            isDashboardVisible = true;
            manageCollection = "Manage Workflows";
            manageEquipmentItems = "Manage Equipments";
            if (_viewModelocator.DashBoardViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.DashBoardViewModelBase);
            }
            else
                _viewModelocator.DashBoardViewModelBase.loadPhotoShoot(clsSchool.defaultSchoolId);

            statusInfo = "Dashboard" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuStudent()
        {
            dockPanleVisibility();
            isStudentVisible = true;

            if (_viewModelocator.StudentsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.StudentsViewModelBase);
            }
            else
                _viewModelocator.StudentsViewModelBase.getDefaultSchool();

            statusInfo = "Student" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuGroups()
        {
            dockPanleVisibility();
            isGroupVisible = true;

            if (_viewModelocator.ManageGroupsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.ManageGroupsViewModelBase);
            }
            else
                _viewModelocator.ManageGroupsViewModelBase.loadData();

            statusInfo = "Group" + " - " + clsSchool.defaultSchoolName;
        }
        private void universalStudentSearch(string tempUniversalText)
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;


            if (tempUniversalText.StartsWith("img:"))
            {
                isUniversalImageVisible = true;
                _viewModelocator.UniversalImageSearchViewModelBase.bindStudentImageGrid(tempUniversalText);
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.UniversalImageSearchViewModelBase);
                //_viewModelocator.UniversalStudentSearchViewModelBase.searchPanels();
                statusInfo = "Universal Image Search";
                studentPanelCaption = "Menu" + " - Universal Image Search";
            }
            else
            {
                isUniversalStudentVisible = true;
                isStudentWithoutImageEnabled = true;
                isImageWithoutStudentEnabled = false;

                _viewModelocator.UniversalStudentSearchViewModelBase.setVisibilityForUniversalStudent();
                _viewModelocator.UniversalStudentSearchViewModelBase.bindStudentGrid(tempUniversalText);
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.UniversalStudentSearchViewModelBase);
                _viewModelocator.UniversalStudentSearchViewModelBase.searchPanels();
                statusInfo = "Universal Student Search";
                studentPanelCaption = "Menu" + " - Universal Student Search";
            }
        }
        private void menuSchools()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isSchoolsVisible = true;

            childViewModels = new ObservableCollection<object>();
            childViewModels.Add(_viewModelocator.SchoolsViewModelBase);
            statusInfo = "School" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuPhotographyJob(string isFromNavPane)
        {
            if (Convert.ToInt32(isFromNavPane) == 1)
            {
                viewIndex = 0;
            }
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isPhotographyJobVisible = true;

            _viewModelocator.PhotographyJobViewModelBase.setButtonsVisibility();
            if (_viewModelocator.PhotographyJobViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.PhotographyJobViewModelBase);
            }
            else
                _viewModelocator.PhotographyJobViewModelBase.bindGrid();

            statusInfo = "School Years" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuPhotoShoots()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isPhotoShootVisible = true;

            _viewModelocator.ViewPhotoShootViewModelBase.setButtonVisibility();
            if (_viewModelocator.ViewPhotoShootViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.ViewPhotoShootViewModelBase);
            }
            else
                _viewModelocator.ViewPhotoShootViewModelBase.bindGrid();
            statusInfo = "PhotoShoot" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuLockedPhotos()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isLockedPhotosVisible = true;

            _viewModelocator.LockedPhotosViewModelBase.setButtonVisibility();
            if (_viewModelocator.LockedPhotosViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.LockedPhotosViewModelBase);
            }
            else
                _viewModelocator.LockedPhotosViewModelBase.bindGrid();
            statusInfo = "Locked Photos" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuActivateStudents()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isActivateStudentVisible = true;

            _viewModelocator.ActivateStudentsViewModelBase.setButtonsVisibility();
            if (_viewModelocator.ActivateStudentsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.ActivateStudentsViewModelBase);
            }
            else
                _viewModelocator.ActivateStudentsViewModelBase.bindToGrid();

            statusInfo = "Deactivate Students" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuImportBatches(string from)
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isImportBatchVisible = true;

            _viewModelocator.ImportBatchesViewModelBase.setButtonVisibility();
            if (_viewModelocator.ImportBatchesViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.ImportBatchesViewModelBase);
            }

            if (from == "students")
            {
                _viewModelocator.ImportBatchesViewModelBase.bindData(true);
                statusInfo = "Student Import Batches" + " - " + clsSchool.defaultSchoolName;
            }
            else
            {
                _viewModelocator.ImportBatchesViewModelBase.bindData(false);
                statusInfo = "Order Import Batches";
            }
        }
        //private void menuFileLocationSettings()
        //{
        //    dockPanleVisibility();
        //    isDockViewsSettingsVisible = true;
        //    isFileLocationVisible = true;

        //    _viewModelocator.FileLocationViewModelBase.setVisibilityForButtons();
        //    if (_viewModelocator.ImportBatchesViewModelBase == null)
        //    {
        //        childViewModels = new ObservableCollection<object>();
        //        childViewModels.Add(_viewModelocator.FileLocationViewModelBase);
        //    }
        //    else
        //        _viewModelocator.FileLocationViewModelBase.getSettingsData();


        //    statusInfo = "File Location" + " - " + clsSchool.defaultSchoolName;
        //}
        private void menuMaskSettings()
        {
            settingsIndex = 0;
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isMaskSettingsVisible = true;

            _viewModelocator.ActivitiesViewModelBase.setVisibilityForButtons();
            childViewModels = new ObservableCollection<object>();
            childViewModels.Add(_viewModelocator.MaskViewModelBase);

            statusInfo = "Mask" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuOrderFormDefaultPricingGroup()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isDefaultPriceGroupVisible = true;

            _viewModelocator.OrderformDefaultPricingViewModelBase.setVisibilityForButtons();
            childViewModels = new ObservableCollection<object>();
            childViewModels.Add(_viewModelocator.OrderformDefaultPricingViewModelBase);

            statusInfo = "Default Pricing" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuActivities(string type)
        {
            if (type == "All")
            {
                activityIndex = 0;
            }
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isActivitiesVisible = true;

            _viewModelocator.ActivitiesViewModelBase.setVisibilityForButtons();
            if (_viewModelocator.ActivitiesViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.ActivitiesViewModelBase);
            }
            else
            {
                _viewModelocator.ActivitiesViewModelBase.activitiesType = type;
                _viewModelocator.ActivitiesViewModelBase.loadData();
            }

            statusInfo = "Activities" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuOrders()
        {
            //if (Convert.ToInt32(isFromNavPane) == 1)
            //{
            //    orderIndex = 0;
            //}
            orderIndex = 0;
            isDockViewsSettingsVisible = true;

            dockPanleVisibility();
            isOrdersVisible = true;

            _viewModelocator.OrdersViewModelBase.buttonsVisibilityForOrderDetails();
            if (_viewModelocator.OrdersViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.OrdersViewModelBase);
            }
            else
                _viewModelocator.OrdersViewModelBase.loadData();

            statusInfo = "Orders" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuOrderPackages()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isOrderPackagesVisible = true;

            _viewModelocator.OrderPackagesViewModelBase.buttonsVisiblityForPackages();
            if (_viewModelocator.OrderPackagesViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.OrderPackagesViewModelBase);
            }
            else
                _viewModelocator.OrderPackagesViewModelBase.bindData();

            statusInfo = "Order Packages" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuViewOrdersByStudent()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isViewOrderByStudentVisible = true;

            _viewModelocator.OrdersViewModelBase.buttonsVisibilityForOrderDetails();
            if (_viewModelocator.OrdersViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.OrdersViewModelBase);
            }
            else
                _viewModelocator.OrdersViewModelBase.loadData(true);

            statusInfo = "View Orders By Student" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuViewOrdersByGalleryGroup()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isViewOrderByGalleryGroupVisible = true;

            _viewModelocator.OrdersViewModelBase.buttonsVisibilityForOrderDetails();
            if (_viewModelocator.OrdersViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.OrdersViewModelBase);
            }
            else
                _viewModelocator.OrdersViewModelBase.loadData(1);

            statusInfo = "View Orders By Gallery Group" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuSearchOrders()
        {
            dockPanleVisibility();
            (Application.Current as App).setAllButtonsVisibility();
            isDockViewsSettingsVisible = true;
            isSearchOrdersVisible = true;

            //_viewModelocator.OrdersViewModelBase.buttonsVisibilityForOrderDetails();
            if (_viewModelocator.SearchOrdersViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.SearchOrdersViewModelBase);
            }
            else
                _viewModelocator.SearchOrdersViewModelBase.loadData();

            statusInfo = "Search Orders" + " - " + clsSchool.defaultSchoolName;
        }
        private void ValidatePhotoshoot()
        {
            dashBoardViewIndex = 1;
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isValidatePhotoshootsVisible = true;

            if (_viewModelocator.ValidatePhotoShootsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.ValidatePhotoShootsViewModelBase);
            }
            else
                _viewModelocator.ValidatePhotoShootsViewModelBase.loadData();

            statusInfo = "Validate PhotoShoots" + " - " + clsSchool.defaultSchoolName;
        }
        private void WorkflowItems()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isWorkflowItemsVisible = true;

            if (_viewModelocator.WorkflowItemsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.WorkflowItemsViewModelBase);
            }
            else
                _viewModelocator.WorkflowItemsViewModelBase.loadData();

            statusInfo = "Workflow Items" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuWorkflowCollections(string from)
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isWorkflowCollectionsVisible = true;
            
            _viewModelocator.ActivitiesViewModelBase.setVisibilityForButtons();
            if (_viewModelocator.WorkflowCollectionsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.WorkflowCollectionsViewModelBase);
            }
            if (from == "workflows")
            {
                manageCollection = "Add/Remove Workflow Items";
                _viewModelocator.WorkflowCollectionsViewModelBase.bindData(true);
                statusInfo = "Workflow Collections" + " - " + clsSchool.defaultSchoolName;
            }
            else
            {
                manageEquipmentItems = "Add/Remove Equipment Items";
                _viewModelocator.WorkflowCollectionsViewModelBase.bindData(false);
                statusInfo = "Equipment Collections" + " - " + clsSchool.defaultSchoolName;
            }
        }
        private void dashBoardWorkflows()
        {
            dashBoardViewIndex = 2;
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isDashboardWorkflowsVisible = true;

            if (_viewModelocator.DashBoardWorkflowItemsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.DashBoardWorkflowItemsViewModelBase);
            }
            else
                _viewModelocator.DashBoardWorkflowItemsViewModelBase.loadData();

            statusInfo = "PhotoSoot Workflow Items" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuImageQuix()
        {
            try
            {
                dashBoardViewIndex = 2;
                dockPanleVisibility();
                isDockViewsSettingsVisible = true;
                isImageQuixAccountsVisible = true;

                if (_viewModelocator.ImageQuixAccountsViewModelBase == null)
                {
                    childViewModels = new ObservableCollection<object>();
                    childViewModels.Add(_viewModelocator.ImageQuixAccountsViewModelBase);
                }
                else
                    _viewModelocator.ImageQuixAccountsViewModelBase.bindData();

                statusInfo = "Image Quix" + " - " + clsSchool.defaultSchoolName;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void menuManageUsers()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isManageUsersVisible = true;

            _viewModelocator.ManageUsersViewModelBase.setButtonVisibility();
            if (_viewModelocator.ManageUsersViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.ManageUsersViewModelBase);
            }
            _viewModelocator.ManageUsersViewModelBase.bindData();
            statusInfo = "Manage Users" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuSPPriceSheet()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isSPPriceSheetVisible = true;

            _viewModelocator.SPPriceSheetViewModelBase.setButtonVisibility();
            if (_viewModelocator.SPPriceSheetViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.SPPriceSheetViewModelBase);
            }
            _viewModelocator.SPPriceSheetViewModelBase.bindData();
            statusInfo = "SP-Price Sheet" + " - " + clsSchool.defaultSchoolName;
        }
        private void menuStudios()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isStudiosVisible = true;

            _viewModelocator.ActiveStudiosViewModelBase.setButtonVisibility();
            if (_viewModelocator.ActiveStudiosViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.ActiveStudiosViewModelBase);
            }
            _viewModelocator.ActiveStudiosViewModelBase.bindData();
            statusInfo = "Studios" + " - " + clsSchool.defaultSchoolName;
        }
        private void EquipmentItems()
        {
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isEquipmentItemsVisible = true;

            if (_viewModelocator.EquipmentItemsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.EquipmentItemsViewModelBase);
            }
            else
                _viewModelocator.EquipmentItemsViewModelBase.loadData();

            statusInfo = "Equipment Items" + " - " + clsSchool.defaultSchoolName;
        }
        private void dashBoardPhotoShootsWOWF()
        {
            dashBoardViewIndex = 3;
            dockPanleVisibility();
            isDockViewsSettingsVisible = true;
            isDashboardPhotoshootsWOWFVisible = true;

            if (_viewModelocator.PhotoShootsWithoutWorkflowsViewModelBase == null)
            {
                childViewModels = new ObservableCollection<object>();
                childViewModels.Add(_viewModelocator.PhotoShootsWithoutWorkflowsViewModelBase);
            }
            else
                _viewModelocator.PhotoShootsWithoutWorkflowsViewModelBase.loadData();

            statusInfo = "PhotoSoots Without Workflows" + " - " + clsSchool.defaultSchoolName;
        }
        //Don't Delete #Mohan
        //private void menuOrderItems()
        //{
        //    if (fulfilledName == null)
        //    {
        //        fulfilledName = "UnProcessed";
        //        tempfull = "UnProcessed";
        //    }

        //    dockPanleVisibility();
        //    isOrdersVisible = true;
        //    isOrderViewVisible = false; isOrderItemsVisible = true;
        //    _viewModelocator.OrderItemsViewModelBase.buttonsVisibilityForOrderDetails();

        //    if (_viewModelocator.OrderItemsViewModelBase == null)
        //    {
        //        childViewModels = new ObservableCollection<object>();
        //        childViewModels.Add(_viewModelocator.OrderItemsViewModelBase);
        //    }
        //    else
        //        _viewModelocator.OrderItemsViewModelBase.loadData(tempfull);

        //    statusInfo = "Order Items";// +" - " + clsSchool.defaultSchoolName;
        //}
        #endregion

        #region BarButton Item Related Methods
        /// <summary>
        /// This method is used to create new record
        /// </summary>
        private void newCommand()
        {
            try
            {
                if (isDashboardVisible == true)
                    _viewModelocator.DashBoardViewModelBase.newRecord();
                if (isStudentVisible == true)
                    _viewModelocator.StudentsViewModelBase.newRecord();
                else if (isMaskSettingsVisible == true)
                    _viewModelocator.MaskViewModelBase.newRecord();
                else if (isGroupVisible == true)
                    _viewModelocator.ManageGroupsViewModelBase.newRecord();
                else if (isSchoolsVisible == true)
                    _viewModelocator.SchoolsViewModelBase.newRecord();
                else if (isPhotographyJobVisible == true)
                    _viewModelocator.PhotographyJobViewModelBase.newRecord();
                else if (isActivitiesVisible == true)
                    _viewModelocator.ActivitiesViewModelBase.newRecord();
                else if (isUniversalStudentVisible == true)
                    _viewModelocator.UniversalStudentSearchViewModelBase.newRecord();
                else if (isOrdersVisible == true)
                    _viewModelocator.OrdersViewModelBase.newRecord("");
                else if (isOrderPackagesVisible == true)
                    _viewModelocator.OrderPackagesViewModelBase.newRecord();
                else if (isViewOrderByStudentVisible)
                    _viewModelocator.OrdersViewModelBase.newRecord("true");
                else if (isViewOrderByGalleryGroupVisible)
                    _viewModelocator.OrdersViewModelBase.newRecord("1");
                else if (isWorkflowItemsVisible)
                    _viewModelocator.WorkflowItemsViewModelBase.newWorkflowItem();
                else if (isWorkflowCollectionsVisible)
                    _viewModelocator.WorkflowCollectionsViewModelBase.newWorkflowCollection();
                else if (isImageQuixAccountsVisible)
                    _viewModelocator.ImageQuixAccountsViewModelBase.newImageQuix();
                else if (isManageUsersVisible)
                    _viewModelocator.ManageUsersViewModelBase.addNewUser();
                else if (isSPPriceSheetVisible)
                    _viewModelocator.SPPriceSheetViewModelBase.newPriceSheet();
                else if (isStudiosVisible)
                    _viewModelocator.ActiveStudiosViewModelBase.newStudio();
                else if (isEquipmentItemsVisible)
                    _viewModelocator.EquipmentItemsViewModelBase.newWorkflowItem();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// This method is used to edit selected record (one at a time)
        /// </summary>
        private void edit()
        {
            try
            {
                if (isDashboardVisible == true)
                    _viewModelocator.DashBoardViewModelBase.editRecord();
                else if (isMaskSettingsVisible == true)
                    _viewModelocator.MaskViewModelBase.editRecord();
                else if (isSchoolsVisible == true)
                    _viewModelocator.SchoolsViewModelBase.editRecord();
                else if (isStudentVisible == true)
                    _viewModelocator.StudentsViewModelBase.editRecord();
                else if (isGroupVisible == true)
                    _viewModelocator.ManageGroupsViewModelBase.editRecord();
                else if (isPhotographyJobVisible == true)
                    _viewModelocator.PhotographyJobViewModelBase.editRecord();
                else if (isPhotoShootVisible == true)
                    _viewModelocator.ViewPhotoShootViewModelBase.editRecord();
                else if (isUniversalStudentVisible == true)
                    _viewModelocator.UniversalStudentSearchViewModelBase.editRecord();
                else if (isActivitiesVisible == true)
                    _viewModelocator.ActivitiesViewModelBase.editRecord();
                else if (isUniversalImageVisible == true)
                    _viewModelocator.UniversalImageSearchViewModelBase.editRecord();
                else if (isOrdersVisible == true || isViewOrderByGalleryGroupVisible == true)
                    _viewModelocator.OrdersViewModelBase.editRecord();
                else if (isViewOrderByStudentVisible == true)
                    _viewModelocator.OrdersViewModelBase.editRecord();
                else if (isOrderPackagesVisible == true)
                    _viewModelocator.OrderPackagesViewModelBase.editPackage();
                else if (isValidatePhotoshootsVisible)
                    _viewModelocator.ValidatePhotoShootsViewModelBase.editRecord();
                else if (isImportBatchVisible)
                    _viewModelocator.ImportBatchesViewModelBase.editImportBatches();
                else if (isWorkflowItemsVisible)
                    _viewModelocator.WorkflowItemsViewModelBase.editWorkflowItem();
                else if (isWorkflowCollectionsVisible)
                    _viewModelocator.WorkflowCollectionsViewModelBase.editWorkflowCollection();
                else if (isDashboardWorkflowsVisible)
                    _viewModelocator.DashBoardWorkflowItemsViewModelBase.editRecord();
                else if (isImageQuixAccountsVisible)
                    _viewModelocator.ImageQuixAccountsViewModelBase.editImageQuix();
                else if (isManageUsersVisible)
                    _viewModelocator.ManageUsersViewModelBase.editUser();
                else if (isSPPriceSheetVisible)
                    _viewModelocator.SPPriceSheetViewModelBase.editPriceSheet();
                else if (isStudiosVisible)
                    _viewModelocator.ActiveStudiosViewModelBase.editStudio();
                else if (isEquipmentItemsVisible)
                    _viewModelocator.EquipmentItemsViewModelBase.editWorkflowItem();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to delete selected record(s)
        /// </summary>
        private void delete()
        {
            try
            {
                //Users selectedUser = (from usr in db.Users where usr.UserName == clsStatic.userName select usr).FirstOrDefault();
                //if (selectedUser.Role == "Admin")
                {
                    if (isMaskSettingsVisible == true)
                        _viewModelocator.MaskViewModelBase.deleteMask();
                    else if (isDashboardVisible == true)
                        _viewModelocator.DashBoardViewModelBase.delete();
                    else if (isStudentVisible == true)
                        _viewModelocator.StudentsViewModelBase.deleteRecords();
                    else if (isSchoolsVisible == true)
                        _viewModelocator.SchoolsViewModelBase.deleteRecords();
                    else if (isGroupVisible == true)
                        _viewModelocator.ManageGroupsViewModelBase.deleteGroup();
                    else if (isPhotographyJobVisible == true)
                        _viewModelocator.PhotographyJobViewModelBase.deleteJob();
                    else if (isPhotoShootVisible == true)
                        _viewModelocator.ViewPhotoShootViewModelBase.deleteShoot();
                    else if (isLockedPhotosVisible == true)
                        _viewModelocator.LockedPhotosViewModelBase.deleteImages();
                    else if (isActivitiesVisible == true)
                        _viewModelocator.ActivitiesViewModelBase.deleteRecord();
                    else if (isOrdersVisible == true || isViewOrderByGalleryGroupVisible == true)
                        _viewModelocator.OrdersViewModelBase.deleteOrder();
                    else if (isViewOrderByStudentVisible == true)
                        _viewModelocator.OrdersViewModelBase.deleteOrder();
                    else if (isWorkflowItemsVisible)
                        _viewModelocator.WorkflowItemsViewModelBase.deleteWorkflow();
                    else if (isWorkflowCollectionsVisible)
                        _viewModelocator.WorkflowCollectionsViewModelBase.delete();
                    else if (isDashboardWorkflowsVisible)
                        _viewModelocator.DashBoardWorkflowItemsViewModelBase.deleteRecords();
                    else if (isImageQuixAccountsVisible)
                        _viewModelocator.ImageQuixAccountsViewModelBase.delete();
                    else if (isManageUsersVisible)
                        _viewModelocator.ManageUsersViewModelBase.deleteUser();
                    else if (isSPPriceSheetVisible)
                        _viewModelocator.SPPriceSheetViewModelBase.deletePriceSheet();
                    else if (isValidatePhotoshootsVisible)
                        _viewModelocator.ValidatePhotoShootsViewModelBase.deleteRecords();
                    else if (isEquipmentItemsVisible)
                        _viewModelocator.EquipmentItemsViewModelBase.deleteWorkflow();
                }
                //else
                //    MVVMMessageService.ShowMessage("You have no permitions to perform this action.");
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// This method is used to delete all locked Images
        /// </summary>
        private void deleteAll()
        {
            //if (isManageUsersVisible)
            //{
            //    _viewModelocator.ManageUsersViewModelBase.deleteAllUsers();
            //}
            //else
            //{
            string caption = "Confirmation";
            string message = errorMessages.DELETE_ALL_PHOTOS;
            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
            if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
            {
                _viewModelocator.LockedPhotosViewModelBase.deleteAllImages();
            }
            //}
        }
        /// <summary>
        /// This method is used to remove selected records
        /// </summary>
        private void removeFromGroup()
        {
            if (isGroupVisible == true)
                _viewModelocator.ManageGroupsViewModelBase.removeGroupItem();
            else if (isOrdersVisible == true || isViewOrderByGalleryGroupVisible == true)
                _viewModelocator.OrdersViewModelBase.removeOrderItem();
            else if (isViewOrderByStudentVisible == true)
                _viewModelocator.OrdersViewModelBase.removeOrderItem();
        }
        /// <summary>
        /// This method is used to group panels based on selected column
        /// </summary>
        private void drag()
        {
            try
            {
                if (isMaskSettingsVisible == true)
                    _viewModelocator.MaskViewModelBase.groupPanels();
                else if (isDashboardVisible == true)
                    _viewModelocator.DashBoardViewModelBase.groupPanels();
                else if (isStudentVisible == true)
                    _viewModelocator.StudentsViewModelBase.groupPanels();
                else if (isGroupVisible == true)
                    _viewModelocator.ManageGroupsViewModelBase.groupPanels();
                else if (isSchoolsVisible == true)
                    _viewModelocator.SchoolsViewModelBase.groupPanels();
                else if (isPhotoShootVisible == true)
                    _viewModelocator.ViewPhotoShootViewModelBase.groupPanels();
                else if (isLockedPhotosVisible == true)
                    _viewModelocator.LockedPhotosViewModelBase.groupPanels();
                else if (isActivateStudentVisible == true)
                    _viewModelocator.ActivateStudentsViewModelBase.groupPanels();
                else if (isPhotographyJobVisible == true)
                    _viewModelocator.PhotographyJobViewModelBase.groupPanels();
                else if (isOrdersVisible == true || isViewOrderByGalleryGroupVisible == true)
                    _viewModelocator.OrdersViewModelBase.groupPanels();
                else if (isViewOrderByStudentVisible == true)
                    _viewModelocator.OrdersViewModelBase.groupPanels();
                else if (isActivitiesVisible == true)
                    _viewModelocator.ActivitiesViewModelBase.groupPanels();
                else if (isValidatePhotoshootsVisible)
                    _viewModelocator.ValidatePhotoShootsViewModelBase.groupPanels();
                else if (isWorkflowItemsVisible)
                    _viewModelocator.WorkflowItemsViewModelBase.groupPanels();
                else if (isWorkflowCollectionsVisible == true)
                    _viewModelocator.WorkflowCollectionsViewModelBase.groupPanels();
                else if (isDashboardWorkflowsVisible == true)
                    _viewModelocator.DashBoardWorkflowItemsViewModelBase.groupPanels();
                else if (isImageQuixAccountsVisible)
                    _viewModelocator.ImageQuixAccountsViewModelBase.groupPanels();
                else if (isManageUsersVisible)
                    _viewModelocator.ManageUsersViewModelBase.groupPanels();
                else if (isSPPriceSheetVisible)
                    _viewModelocator.SPPriceSheetViewModelBase.groupPanels();
                else if (isStudiosVisible)
                    _viewModelocator.ActiveStudiosViewModelBase.groupPanels();
                else if (isEquipmentItemsVisible)
                    _viewModelocator.EquipmentItemsViewModelBase.groupPanels();
                else if (isDashboardPhotoshootsWOWFVisible)
                    _viewModelocator.PhotoShootsWithoutWorkflowsViewModelBase.groupPanels();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        private void search()
        {
            try
            {
                if (isMaskSettingsVisible == true)
                    _viewModelocator.MaskViewModelBase.searchPanels();
                else if (isDashboardVisible == true)
                    _viewModelocator.DashBoardViewModelBase.searchPanels();
                else if (isStudentVisible == true)
                    _viewModelocator.StudentsViewModelBase.searchPanels();
                else if (isGroupVisible == true)
                    _viewModelocator.ManageGroupsViewModelBase.searchPanels();
                else if (isSchoolsVisible == true)
                    _viewModelocator.SchoolsViewModelBase.searchPanels();
                else if (isLockedPhotosVisible == true)
                    _viewModelocator.LockedPhotosViewModelBase.searchPanels();
                else if (isPhotoShootVisible == true)
                    _viewModelocator.ViewPhotoShootViewModelBase.searchPanels();
                else if (isPhotographyJobVisible == true)
                    _viewModelocator.PhotographyJobViewModelBase.searchPanels();
                else if (isActivateStudentVisible == true)
                    _viewModelocator.ActivateStudentsViewModelBase.searchPanels();
                else if (isOrdersVisible == true || isViewOrderByGalleryGroupVisible == true)
                    _viewModelocator.OrdersViewModelBase.searchPanels();
                else if (isViewOrderByStudentVisible == true)
                    _viewModelocator.OrdersViewModelBase.searchPanels();
                //else if (isOrderItemsVisible == true)
                //    _viewModelocator.OrderItemsViewModelBase.searchPanels();
                else if (isActivitiesVisible == true)
                    _viewModelocator.ActivitiesViewModelBase.searchPanels();
                else if (isValidatePhotoshootsVisible)
                    _viewModelocator.ValidatePhotoShootsViewModelBase.searchPanels();
                else if (isWorkflowItemsVisible)
                    _viewModelocator.WorkflowItemsViewModelBase.searchPanels();
                else if (isWorkflowCollectionsVisible == true)
                    _viewModelocator.WorkflowCollectionsViewModelBase.searchPanels();
                else if (isDashboardWorkflowsVisible == true)
                    _viewModelocator.DashBoardWorkflowItemsViewModelBase.searchPanels();
                else if (isImageQuixAccountsVisible)
                    _viewModelocator.ImageQuixAccountsViewModelBase.searchPanels();
                else if (isManageUsersVisible)
                    _viewModelocator.ManageUsersViewModelBase.searchPanels();
                else if (isSPPriceSheetVisible)
                    _viewModelocator.SPPriceSheetViewModelBase.searchPanels();
                else if (isStudiosVisible)
                    _viewModelocator.ActiveStudiosViewModelBase.searchPanels();
                else if (isEquipmentItemsVisible)
                    _viewModelocator.EquipmentItemsViewModelBase.searchPanels();
                else if (isDashboardPhotoshootsWOWFVisible)
                    _viewModelocator.PhotoShootsWithoutWorkflowsViewModelBase.searchPanels();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void importFile()
        {
            try
            {
                if (isDashboardVisible == true)
                    _viewModelocator.DashBoardViewModelBase.importQRDataFile();
                else if (isPhotographyJobVisible == true)
                    _viewModelocator.PhotographyJobViewModelBase.importDataFile();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// this method used to upload images to the new photoshoot
        /// </summary>
        private void uploadImages()
        {
            _viewModelocator.DashBoardViewModelBase.importQRDataFile();
        }
        /// <summary>
        /// this method used to import student from excel file
        /// </summary>
        private void importStudents()
        {
            try
            {
                if (isStudentVisible == true)
                    _viewModelocator.StudentsViewModelBase.studentImportFromStudent();
                else if (isDashboardVisible == true)
                    _viewModelocator.DashBoardViewModelBase.studentImportFromDashboard();
                else if (isSchoolsVisible == true)
                    _viewModelocator.SchoolsViewModelBase.StudentImportFromSchool();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// This method is used to refresh the grid's after some change in data
        /// </summary>
        private void refresh()
        {
            try
            {
                if (isDashboardVisible == true)
                    _viewModelocator.DashBoardViewModelBase.refreshGrids();
                else if (isStudentVisible == true)
                    _viewModelocator.StudentsViewModelBase.refreshGrid();
                else if (isSchoolsVisible == true)
                    _viewModelocator.SchoolsViewModelBase.RefreshGrid();
                else if (isMaskSettingsVisible == true)
                    _viewModelocator.MaskViewModelBase.refreshGrid();
                else if (isGroupVisible == true)
                    _viewModelocator.ManageGroupsViewModelBase.refreshGrid();
                else if (isPhotoShootVisible == true)
                    _viewModelocator.ViewPhotoShootViewModelBase.refreshGrid();
                else if (isPhotographyJobVisible == true)
                    _viewModelocator.PhotographyJobViewModelBase.refreshGrid();
                else if (isOrdersVisible == true)
                    _viewModelocator.OrdersViewModelBase.loadData();
                else if (isValidatePhotoshootsVisible)
                    _viewModelocator.ValidatePhotoShootsViewModelBase.refreshGrid();
                else if (isViewOrderByStudentVisible == true)
                    _viewModelocator.OrdersViewModelBase.loadData(true);
                else if (isManageUsersVisible)
                    _viewModelocator.ManageUsersViewModelBase.bindData();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// this method used to print MPOF
        /// </summary>
        private void generatePDF()
        {
            try
            {
                _viewModelocator.ManageGroupsViewModelBase.generatePDF();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// this method used to export photos
        /// </summary>
        private void exportPhotos()
        {
            try
            {
                if (isGroupVisible == true)
                    _viewModelocator.ManageGroupsViewModelBase.exportPhoto();
                else if (isOrdersVisible == true || isViewOrderByGalleryGroupVisible == true)
                    _viewModelocator.OrdersViewModelBase.exportOrders();
                else if (isSearchOrdersVisible == true)
                    _viewModelocator.SearchOrdersViewModelBase.exportOrders();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// this method used to edit more than one record at a time
        /// </summary>
        private void bulkRename()
        {
            try
            {
                if (isMaskSettingsVisible == true)
                    _viewModelocator.MaskViewModelBase.BulkRename();
                else if (isDashboardVisible == true)
                    _viewModelocator.DashBoardViewModelBase.bulkRename();
                else if (isSchoolsVisible == true)
                    _viewModelocator.SchoolsViewModelBase.BulkRename();
                else if (isStudentVisible == true)
                    _viewModelocator.StudentsViewModelBase.bulkRename();
                else if (isGroupVisible == true)
                    _viewModelocator.ManageGroupsViewModelBase.bulkRename();
                else if (isPhotographyJobVisible == true)
                    _viewModelocator.PhotographyJobViewModelBase.bulkRename();
                else if (isPhotoShootVisible == true)
                    _viewModelocator.ViewPhotoShootViewModelBase.bulkRename();
                else if (isOrdersVisible == true)
                    _viewModelocator.OrdersViewModelBase.bulkRename();
                else if (isViewOrderByStudentVisible == true)
                    _viewModelocator.OrdersViewModelBase.bulkRename();
                else if (isValidatePhotoshootsVisible)
                    _viewModelocator.ValidatePhotoShootsViewModelBase.bulkRename();
                else if (isDashboardWorkflowsVisible)
                    _viewModelocator.DashBoardWorkflowItemsViewModelBase.bulkRename();
                else if (isEquipmentItemsVisible)
                    _viewModelocator.EquipmentItemsViewModelBase.bulkRename();
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        /// <summary>
        /// this method used to count total images for each student in selected Photoshoot
        /// </summary>
        private void countImages()
        {
            //if (isDashboardVisible == true)
            //    _viewModelocator.DashBoardViewModelBase.CountImages();
            if (isStudentVisible == true)
                _viewModelocator.StudentsViewModelBase.countImages();
        }
        private void addToGroup()
        {
            if (isDashboardVisible == true)
                _viewModelocator.DashBoardViewModelBase.addImagesToGroup();
            else if (isStudentVisible == true)
                _viewModelocator.StudentsViewModelBase.addImagesToGroup();
            else if (isGroupVisible == true)
                _viewModelocator.ManageGroupsViewModelBase.addImagesToGroup();
        }
        /// <summary>
        /// this method used to generate password to the selected Students/Students without password
        /// </summary>
        private void generatePassword()
        {
            _viewModelocator.StudentsViewModelBase.generatePassword();
        }
        private void groupPhoto()
        {
            _viewModelocator.ManageGroupsViewModelBase.AddGroupClassPhoto();
        }
        /// <summary>
        /// this method used to export student
        /// </summary>
        private void exportStudents()
        {
            _viewModelocator.StudentsViewModelBase.exportStudents();
        }
        private void rating()
        {
            _viewModelocator.DashBoardViewModelBase.getRating();
        }
        private void yearBook()
        {
            _viewModelocator.DashBoardViewModelBase.updateYearbook();
        }
        /// <summary>
        /// This method will auto creates teacher(s) groups
        /// </summary>
        private void createTeacherGroups()
        {
            _viewModelocator.DashBoardViewModelBase.autoCreateTeacherGroups();
        }
        /// <summary>
        /// This method is used to rename source images int format lastname_firstname_serialno.jpg
        /// </summary>
        private void renameSourceImages()
        {
            if (isDashboardVisible == true)
                _viewModelocator.DashBoardViewModelBase.renameSourceImages();
            else if (isStudentVisible == true)
                _viewModelocator.StudentsViewModelBase.renameSourceImages();
        }
        /// <summary>
        /// this method used to generate reduced images 
        /// </summary>
        private void generateReducedImages()
        {
            _viewModelocator.DashBoardViewModelBase.GenerateReducedImages();
        }
        private void generateReducedImagesAll()
        {
            //_viewModelocator.objDashBoardViewModel.GenerateAllReducedImages();
        }
        //private void studentsWithoutImage()
        //{
        //    _viewModelocator.DashBoardViewModelBase.studentWithoutImage();
        //}
        /// <summary>
        /// this method used to open source folder
        /// </summary>
        private void openFolder()
        {
            if (isDashboardVisible == true)
                _viewModelocator.DashBoardViewModelBase.openFolder();
            if (isGroupVisible == true)
                _viewModelocator.ManageGroupsViewModelBase.OpenFolder();
            if (isStudentVisible == true)
                _viewModelocator.StudentsViewModelBase.OpenFolder();
            else if (isLockedPhotosVisible == true)
                _viewModelocator.LockedPhotosViewModelBase.openFolder();
            else if (isOrdersVisible == true)
                _viewModelocator.OrdersViewModelBase.openFolder();
            else if (isViewOrderByStudentVisible == true)
                _viewModelocator.OrdersViewModelBase.openFolder();
        }
        private void syncDeletedImages()
        {
            _viewModelocator.DashBoardViewModelBase.SyncDeleteImages();
        }
        private void reSyncImageNames()
        {
            string caption = "Confirmation";
            string message = errorMessages.RESYNC_IMAGE_NAMES;
            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
            if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
            {
                message = "This process is not reversible. Are you sure?";
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    _viewModelocator.DashBoardViewModelBase.ReSyncImageNames();
                }
            }
        }
        /// <summary>
        /// this method used to add images (OrderItems) to the selected order
        /// </summary>
        private void addToOrders()
        {
            if (isOrdersVisible == true || isViewOrderByGalleryGroupVisible == true)
                _viewModelocator.OrdersViewModelBase.addImagesToOrders();
        }
        private void reAssignPhotoShootToSchoolYear()
        {
            //we are not using this method , as we can directly edit school year by simply double click the photo shoot or by editing the photo shoot .
            //_viewModelocator.objDashBoardViewModel.ReAssignPhotoshootToSchoolYear();
        }
        private void aboutUs()
        {
            AboutUs _objAboutUs = new AboutUs(userName, credits);
            _objAboutUs.ShowDialog();

            //PhotoForce.OrdersManagement.NewManualOrders _objNewManualOrders = new PhotoForce.OrdersManagement.NewManualOrders();
            //_objNewManualOrders.ShowDialog();
            //PhotoForce.OrdersManagement.TestWindow _objTestWindow = new PhotoForce.OrdersManagement.TestWindow();
            //_objTestWindow.ShowDialog();
        }
        /// <summary>
        /// this method used to generate QRCode for the selected/filtered/all students
        /// </summary>
        private void generateQRCode()
        {
            _viewModelocator.StudentsViewModelBase.generateQRCode();
        }
        /// <summary>
        /// this method used to assign student to the selected images
        /// </summary>
        private void assignStudent()
        {
            if (isDashboardVisible == true)
                _viewModelocator.DashBoardViewModelBase.assignStudent();
            else if (isStudentVisible == true)
                _viewModelocator.StudentsViewModelBase.assignStudent();
            else if (isGroupVisible == true)
                _viewModelocator.ManageGroupsViewModelBase.assignStudent();
            else if (isValidatePhotoshootsVisible)
                _viewModelocator.ValidatePhotoShootsViewModelBase.assignStudent();
        }
        /// <summary>
        /// this method used to deactivate selected active students
        /// </summary>
        private void deActivateStudent()
        {
            _viewModelocator.StudentsViewModelBase.deactivateStudents();
        }
        /// <summary>
        /// this method used to activate selected deactivated students
        /// </summary>
        private void activateStudent()
        {
            _viewModelocator.ActivateStudentsViewModelBase.ActivateStudentsView();

        }
        private void printDeviceQR()
        {
            _viewModelocator.DashBoardViewModelBase.printDeviceData(true);

        }
        //Deleted after 4.50 as we are using single UI for all photshoot validations //Mohan
        //private void countAdminCD()
        //{
        //    _viewModelocator.DashBoardViewModelBase.CountAdminCD();

        //}
        //private void countYearBook()
        //{
        //    _viewModelocator.DashBoardViewModelBase.CountYearbookD();
        //}
        //private void validatePhotoshoots()
        //{
        //    _viewModelocator.DashBoardViewModelBase.validatePhotoshoots();
        //}
        /// <summary>
        /// this method used to set/chage default school
        /// </summary>
        private void defaultSchool()
        {
            clsSchool.previousSchoolId = clsSchool.getPreviousSchoolId();

            _objSearchSchool = new SearchSchool("Set Default School");
            _objSearchSchool.ShowDialog();

            if (clsSchool.previousSchoolId != clsSchool.defaultSchoolId)
            {
                assignDataAfterSchoolChange();
            }
        }
        /// <summary>
        /// this method used to refresh the data after default school was changed
        /// </summary>
        private void assignDataAfterSchoolChange()
        {
            bindPanelCaption(clsSchool.defaultSchoolName);
            if (isDashboardVisible == true)
            {
                _viewModelocator.DashBoardViewModelBase.loadPhotoShoot(clsSchool.defaultSchoolId);
                statusInfo = "Dashboard" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isValidatePhotoshootsVisible)
            {
                _viewModelocator.ValidatePhotoShootsViewModelBase.loadData();
                statusInfo = "Validate Photshoots" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isDashboardWorkflowsVisible)
            {
                _viewModelocator.DashBoardWorkflowItemsViewModelBase.loadData();
                statusInfo = "Workflow Items" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isStudentVisible == true)
            {
                _viewModelocator.StudentsViewModelBase.getDefaultSchool();
                statusInfo = "Student" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isGroupVisible == true)
            {
                _viewModelocator.ManageGroupsViewModelBase.loadData();
                statusInfo = "Group" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isSchoolsVisible)
            {
                _viewModelocator.SchoolsViewModelBase.bindGrid();
                statusInfo = "School" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isPhotoShootVisible == true)
            {
                _viewModelocator.ViewPhotoShootViewModelBase.bindGrid();
                statusInfo = "PhotoShoot" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isPhotographyJobVisible == true)
            {
                _viewModelocator.PhotographyJobViewModelBase.bindGrid();
                statusInfo = "School Years" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isLockedPhotosVisible == true)
            {
                _viewModelocator.LockedPhotosViewModelBase.bindGrid();
                statusInfo = "Locked Photos" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isActivateStudentVisible == true)
            {
                _viewModelocator.ActivateStudentsViewModelBase.bindToGrid();
                statusInfo = "Deactivate Students" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isImportBatchVisible == true)
            {
                _viewModelocator.ImportBatchesViewModelBase.bindData(true);
                statusInfo = "Import Batches" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isMaskSettingsVisible == true)
            {
                _viewModelocator.MaskViewModelBase.GetDefaultSchool();
                statusInfo = "Mask" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isDefaultPriceGroupVisible == true)
            {
                statusInfo = "Default Pricing" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isOrderPackagesVisible == true)
            {
                //no need to call this method becuase order packages data same for all schools.
                //_viewModelocator.OrderPackagesViewModelBase.getDefaultSchool();
                statusInfo = " Orders Packages " + " - " + clsSchool.defaultSchoolName;
            }
            else if (isOrdersVisible)
            {
                _viewModelocator.OrdersViewModelBase.loadData();
                statusInfo = "Orders" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isViewOrderByStudentVisible)
            {
                _viewModelocator.OrdersViewModelBase.loadData(true);
                statusInfo = "View Orders By Student" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isViewOrderByGalleryGroupVisible)
            {
                _viewModelocator.OrdersViewModelBase.loadData(1);
                statusInfo = "View Orders By Gallery Group" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isSearchOrdersVisible == true)
            {
                _viewModelocator.SearchOrdersViewModelBase.loadData();
                statusInfo = "Search Orders" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isWorkflowItemsVisible)
            {
                _viewModelocator.WorkflowItemsViewModelBase.loadData();
                statusInfo = "Workflow Items" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isWorkflowCollectionsVisible)
            {
                _viewModelocator.WorkflowCollectionsViewModelBase.bindData(true);
                statusInfo = "Workflow Collections" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isImageQuixAccountsVisible)
            {
                _viewModelocator.ImageQuixAccountsViewModelBase.bindData();
                statusInfo = "Image Quix" + " - " + clsSchool.defaultSchoolName;
            }
            else if (isActivitiesVisible)
            {
                _viewModelocator.ActivitiesViewModelBase.loadData();
                statusInfo = "Activities" + " - " + clsSchool.defaultSchoolName;
            }
            School s = (from st in db.Schools where st.ID == clsSchool.defaultSchoolId select st).FirstOrDefault();
            int studioID = Convert.ToInt32(s.StudioId);
            Studio na = (from st in db.Studios where st.Id == studioID select st).FirstOrDefault();
            if(na != null)
                studioInfo = ", Studio" + " - " + na.StudioName;
        }
        private void defalutPackage()
        {
            _viewModelocator.SchoolsViewModelBase.SetDefaultPackages();
        }
        private void IPTC()
        {
            if (isDashboardVisible == true)
                _viewModelocator.DashBoardViewModelBase.generateJIF();
            else
                _viewModelocator.ManageGroupsViewModelBase.GenerateJIF();
        }
        private void addCredits()
        {
            AddCredits _objAddCredits = new AddCredits("B");
            _objAddCredits.ShowDialog();
            if (((AddCreditsViewModel)(_objAddCredits.DataContext)).isSave == true)
            {
                credits = ((AddCreditsViewModel)(_objAddCredits.DataContext)).credits;
            }

        }
        private void fileLocationSettings()
        {
            //FileLocation _objFileLocation = new FileLocation();
            //_objFileLocation.ShowDialog();
            //if (((FileLocationViewModel)(_objFileLocation.DataContext)).isSave == true)
            //{
            //    credits = ((AddCreditsViewModel)(_objFileLocation.DataContext)).credits;
            //}
        }
        /// <summary>
        /// this method used to chage/edit database
        /// </summary>
        private void openDatabase()
        {
            ManageDBConnections _objManageDBConnections = new ManageDBConnections();
            _objManageDBConnections.ShowDialog();
            //do similar to, what we do after default school change..
            if (((ManageDBConnectionsViewModel)(_objManageDBConnections.DataContext)).isConnectionStringSelected)
            {
                assignDataAfterSchoolChange();
                MVVMMessageService.ShowMessage("DB is updated.");
                testForTempDB();
            }
        }
        private void reImportStudents()
        {
            ReImportStudents _objReImportStudents = new ReImportStudents();
            _objReImportStudents.ShowDialog();

            string caption = "Confirmation";
            string message = "";
            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
            System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
            if (((ReImportStudentsViewModel)(_objReImportStudents.DataContext)).isSave)
            {
                bool updateThreeFields = ((ReImportStudentsViewModel)(_objReImportStudents.DataContext)).isUpdateSplFields;

                message = "This process is not reversible. Are you sure?";
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    _viewModelocator.StudentsViewModelBase.reImportStudents(updateThreeFields);
                }
            }

        }
        /// <summary>
        /// this method used to validate datafile
        /// </summary>
        private void validateDataFile()
        {
            ValidateDataFile _objValidateDataFile = new ValidateDataFile();
            _objValidateDataFile.ShowDialog();
        }
        /// <summary>
        /// this method used to rename photoshoots
        /// </summary>
        private void checkForRenamePhotoshoots()
        {
            string checkForRenameSourceImages = _viewModelocator.DashBoardViewModelBase.checkForRenameSourceImages();
            if (!string.IsNullOrEmpty(checkForRenameSourceImages))
            {
                string message = "Images have not been renamed for the following photoshoots: \n\n" + checkForRenameSourceImages;
                string caption = "Information";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.OK;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Information;
                MVVMMessageService.ShowMessage(message, caption, buttons, icon);

                //if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                //{
                //    return;
                //}
            }
        }
        /// <summary>
        /// this method used to auto assign class photo
        /// </summary>
        private void autoAssignClassPhotos()
        {
            _viewModelocator.DashBoardViewModelBase.addClassPhotosToGroup();
        }
        private void autoCreateGroupPhotos()
        {
            if (isOrdersVisible == true)
                _viewModelocator.OrdersViewModelBase.autoCreateGroupPhotos();
        }
        /// <summary>
        /// this method used to add class photo to selected orderitem
        /// </summary>
        private void addClassPhotoToOrderItem()
        {
            if (_isOrdersVisible)
                _viewModelocator.OrdersViewModelBase.addClassPhotoToOrderItem();
        }
        private void restoreSourceImages()
        {
            _viewModelocator.ManageGroupsViewModelBase.restoreHighResImages();
        }
        //private void importOrders()
        //{
        //    _viewModelocator.OrdersViewModelBase.importOrdersProgressBar(false);
        //}
        /// <summary>
        /// This method used to select path from which orders to be import
        /// It contactains 3 types 1)-XML files from FTP site(only for neal) 2)-XML files from local disk 3)-excel file from local disk
        /// </summary>
        private void shiprushTracking()
        {
            _viewModelocator.OrdersViewModelBase.shiprushTracking();
        }
        private void importOrdersWithDownload()
        {
            _viewModelocator.OrdersViewModelBase.importOrdersPathSelector();
        }
        /// <summary>
        /// used to edit OrderItemBilliingCodes for a selected order
        /// </summary>
        private void editBillingCode()
        {
            if (isOrdersVisible)
                _viewModelocator.OrdersViewModelBase.editBillingCode("");
            else if (isViewOrderByGalleryGroupVisible == true)
                _viewModelocator.OrdersViewModelBase.editBillingCode("1");
        }
        /// <summary>
        /// used to send log file // log file will send to ftp://ftp.freedphoto.com/web_users/pflogfile/
        /// </summary>
        private void sendLogFile()
        {
            //send log file
            if (File.Exists(clsStatic.exceptionLogXML))
            {
                uploadFtpFile(clsStatic.exceptionLogXML);
            }
        }
        /// <summary>
        /// This method tries to find the images which are present in Photo Force but not found in hard disk
        /// </summary>
        private void hasMissingImages()
        {
            _viewModelocator.OrdersViewModelBase.checkMissingImages();
        }
        /// <summary>
        /// To edit more than one order at a time // appending vendor number to privious vendor number without deleting previous vendor number
        /// </summary>
        private void editOrders()
        {
            if (isOrdersVisible || isViewOrderByGalleryGroupVisible == true)
                _viewModelocator.OrdersViewModelBase.bulkEditOrders();
        }
        /// <summary>
        /// this method tries to assign StudentImage to the orderItem if orderItem doesn't contain image details
        /// </summary>
        private void assignOrderImages()
        {
            if (isOrdersVisible || isViewOrderByGalleryGroupVisible == true)
                _viewModelocator.OrdersViewModelBase.assignOrderImages();
        }
        private void restoreRetouchImages()
        {
            RestoreRetouchImages _objRestoreRetouchImages = new RestoreRetouchImages();
            _objRestoreRetouchImages.ShowDialog();
        }
        /// <summary>
        /// this method finds the all images that doesn't have orders from a group and importbatch
        /// </summary>
        private void imagesWithoutOrders()
        {
            _viewModelocator.OrdersViewModelBase.imagesWithoutOrders();
        }
        /// <summary>
        /// this method used to take database backup and copying to FTP server 
        /// </summary>
        private void dataBaseBackUp()
        {
            DataBaseBackUp _objDataBaseBackUp = new DataBaseBackUp();
            _objDataBaseBackUp.ShowDialog();
        }
        //void hasNotes()
        //{
        //    _viewModelocator.OrdersViewModelBase.hasNotes();
        //}
        /// <summary>
        /// This function will add self-selected yearbook images into the group . The images will be given a rating of 6, and all duplicates will be removed, leaving only one image per student.
        /// </summary>
        private void importYearbookChoice()
        {
            _viewModelocator.ManageGroupsViewModelBase.importYearbookSelection();
        }
        /// <summary>
        /// This method will update the Vendor order number in photo force based on the OrderId value in the excel
        /// </summary>
        private void updateOrdersThroughExcel()
        {
            if (isOrdersVisible == true)
                _viewModelocator.OrdersViewModelBase.updateOrdersThroughExcel();
        }
        /// <summary>
        /// this method used to find missing orders from orders comparing with excel file
        /// </summary>
        private void missingOrders()
        {
            MissingOrders _objMissingOrders = new MissingOrders();
            _objMissingOrders.ShowDialog();
        }
        /// <summary>
        /// used to merge two schools
        /// </summary>
        /// <parm>first seleced school merged with second selected school(survivor)</parm>
        private void mergeSchools()
        {
            if (isSchoolsVisible == true)
                _viewModelocator.SchoolsViewModelBase.mergeSchools();
        }
        private void addCollections()
        {            
            if (isDashboardVisible)
                _viewModelocator.DashBoardViewModelBase.addCollectionItems("Dashboard");
            else if (isWorkflowCollectionsVisible)
                _viewModelocator.WorkflowCollectionsViewModelBase.addCollectionItems("Dashboard");
        }
        private void manageEquipments()
        {
            if (isDashboardVisible)
                _viewModelocator.DashBoardViewModelBase.addCollectionItems("Equipment");
            else if (isWorkflowCollectionsVisible)
                _viewModelocator.WorkflowCollectionsViewModelBase.addCollectionItems("Equipment");
        }
        private void switchStudentFLNames()
        {
            if (isStudentVisible)
                _viewModelocator.StudentsViewModelBase.switchStudentFLNames();
        }
        private void deleteManualStudents()
        {
            if (isStudentVisible)
                _viewModelocator.StudentsViewModelBase.deleteManualStudents();
        }
        private void trackStudentacrossYears()
        {
            if (isStudentVisible)
            {
                TrackStudentsAcrossYears _objTrackStudentsAcrossYears = new TrackStudentsAcrossYears();
                _objTrackStudentsAcrossYears.ShowDialog();
            }
        }
        private void clearingCorrectStudents()
        {
            if (isValidatePhotoshootsVisible)
                _viewModelocator.ValidatePhotoShootsViewModelBase.clearingCorrectStudents();
        }
        private void createDuplicatePhotoShoots()
        {
            if (isDashboardVisible)
                _viewModelocator.DashBoardViewModelBase.createDuplicatePhotoShoots();
        }
        #endregion

        public void uploadFtpFile(string fileName)
        {
            try
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri + clsStatic.userName + " " + getFolderName());
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // credentials.
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

                // Copy the contents of the file to the request stream.
                StreamReader sourceStream = new StreamReader(fileName);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                if (fileContents.Length >= 10000000)
                {
                    clsStatic.updateRegistryLogName(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + clsStatic.APP_DATA_FOLDER_NAME + "\\" + getFolderName());
                }

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                MVVMMessageService.ShowMessage("Exception log file sent successfully.");
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage("Error while sending log file.\n" + ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }

        }
        /// <summary>
        /// this method used to display db connected to: Orginal/TestDatabase
        /// </summary>
        void testForTempDB()
        {
            string tempConnectionString = clsConnectionString.connectionString;

            string[] splitserver = tempConnectionString.Split(';');
            string[] servername = splitserver[0].Split('=');
            string[] splitdatabase = tempConnectionString.Split(';');
            string[] databasename = splitdatabase[1].Split('=');
            string server = servername[1].ToString();
            string dataBaseName = databasename[1].ToString();

            if ((server.ToLower() == "fpsql\\sqlexpress,1522") && dataBaseName.ToLower() == "photoforce4") //server.ToLower() == "frvm02\\sqlexpress,1522" || 
            {
                txtTestConnectionVisibility = Visibility.Collapsed;
            }
            else
            {
                txtTestConnectionVisibility = Visibility.Visible;
                txtTestConnection = "Connected To Test Database.";
            }
        }

        string getFolderName()
        {
            DateTime todayDate = DateTime.Now;
            return string.Concat(todayDate.ToShortDateString().Split('/')) + string.Concat(todayDate.ToShortTimeString().Split(':')) + ".xml";
        }

        internal void deleteImages()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            IList<StudentImage> _objStudentImagesList = clsDashBoard.getDeletedStudentimg(db);

            foreach (StudentImage stuImage in _objStudentImagesList)
            {
                try
                {
                    string imgId = "";
                    string imgName = stuImage.ImageName;
                    string path = stuImage.PhotoShoot.ImageFolder;
                    imgId = imgId + stuImage.ID + ",";
                    if (File.Exists(path + "\\" + imgName))
                    {
                        File.Delete(path + "\\" + imgName);
                    }
                    imgId = imgId.Substring(0, imgId.Length - 1);
                    int delStuID = clsDashBoard.deletestudentimage(db, stuImage.ID);
                }
                catch (Exception ex)
                {
                    isErrorExist = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
            }

        }

        internal void bindPanelCaption(string defaultSchoolName)
        {
            panelDashboardCaption = "Dashboard" + " - " + defaultSchoolName;
            panelStudentCaption = "Menu" + " - " + defaultSchoolName;
            panelSchoolCaption = "Menu" + " - " + defaultSchoolName;
            panelPhotoShootsCaption = "Menu" + " - " + defaultSchoolName;
            panelPhotoGraphyJobCaption = "Menu" + " - " + defaultSchoolName;
            panelGroupsCaption = "Menu" + " - " + defaultSchoolName;
            panelLockedPhotosCaption = "Menu" + " - " + defaultSchoolName;
            panelDeactivateStudentsCaption = "Menu" + " - " + defaultSchoolName;
            panelImportBatchesCaption = "Menu" + " - " + defaultSchoolName;
            panelMaskCaption = "Settings" + " - " + defaultSchoolName;
            panelFileLocCaption = "Settings" + " - " + defaultSchoolName;
            panelDefaultpackageCaption = "Settings" + " - " + defaultSchoolName;
        }

        private void loadDashBoard()
        {
            isDashboardVisible = true;
            childViewModels = new ObservableCollection<object>();
            childViewModels.Add(_viewModelocator.DashBoardViewModelBase);
        }

        /// <summary>
        /// dockPanleVisibility method set all the panles visibility to false.
        /// </summary>
        private void dockPanleVisibility()
        {
            isDashboardVisible = false; isGroupVisible = false; isStudentVisible = false; isActivitiesVisible = false;
            isSchoolsVisible = false; isPhotographyJobVisible = false; isPhotoShootVisible = false; isLockedPhotosVisible = false; isActivateStudentVisible = false; isImportBatchVisible = false;
            isFileLocationVisible = false; isMaskSettingsVisible = false; isDefaultPriceGroupVisible = false;
            isUniversalStudentVisible = false; isUniversalImageVisible = false; isManageUsersVisible = false; isStudiosVisible = false;
            isOrderPackagesVisible = false; isOrdersVisible = false; isValidatePhotoshootsVisible = false; isViewOrderByStudentVisible = false; isViewOrderByGalleryGroupVisible = false; isWorkflowItemsVisible = false;
            isSearchOrdersVisible = false; isWorkflowCollectionsVisible = false; isDashboardWorkflowsVisible = false; isImageQuixAccountsVisible = false; isSPPriceSheetVisible = false; isEquipmentItemsVisible = false;
            isDashboardPhotoshootsWOWFVisible = false;
        }
        #endregion
    }
}
