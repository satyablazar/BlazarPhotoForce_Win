using PhotoForce.App_Code;
using PhotoForce.GroupManagement;
using PhotoForce.OrdersManagement;
using PhotoForce.PhotographyJobManagement;
using PhotoForce.Settings.UserControls;
using PhotoForce.WorkPlace.UserControls;
using PhotoForce.WorkPlace.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.MVVM
{

    public class ViewModelLocator
    {
        /// <summary>
        /// creating property for DashBoardViewModel , which is used in MainViewModel.
        /// </summary>
        static DashBoardViewModel _objDashBoardViewModel;
        public DashBoardViewModel DashBoardViewModelBase
        {
            get
            {
                if (_objDashBoardViewModel == null)
                    _objDashBoardViewModel = new DashBoardViewModel("ViewModelLocator");

                return _objDashBoardViewModel;
            }
        }

        /// <summary>
        /// creating property for StudentViewModel , which is used in MainViewModel.
        /// </summary>
        static StudentsViewModel _objStudentsViewModel;
        public StudentsViewModel StudentsViewModelBase
        {
            get
            {
                if (_objStudentsViewModel == null)
                    _objStudentsViewModel = new StudentsViewModel();

                return _objStudentsViewModel;
            }
        }

        /// <summary>
        /// creating property for ManageGroupsViewModel , which is used in MainViewModel.
        /// </summary>
        static ManageGroupsViewModel _objManageGroupsViewModel;
        public ManageGroupsViewModel ManageGroupsViewModelBase
        {
            get
            {
                if (_objManageGroupsViewModel == null)
                    _objManageGroupsViewModel = new ManageGroupsViewModel();

                return _objManageGroupsViewModel;
            }
        }

        /// <summary>
        /// creating property for UniversalStudentSearchVM , which is used in MainViewModel.
        /// </summary>
        static UniversalStudentSearchViewModel _objUniversalStudentSearchViewModel;
        public UniversalStudentSearchViewModel UniversalStudentSearchViewModelBase
        {
            get
            {
                if (_objUniversalStudentSearchViewModel == null)
                    _objUniversalStudentSearchViewModel = new UniversalStudentSearchViewModel();

                return _objUniversalStudentSearchViewModel;
            }
        }

        /// <summary>
        /// creating property for UniversalImageSearchVM , which is used in MainViewModel.
        /// </summary>
        static UniversalImageSearchViewModel _objUniversalImageSearchViewModel;
        public UniversalImageSearchViewModel UniversalImageSearchViewModelBase
        {
            get
            {
                if (_objUniversalImageSearchViewModel == null)
                    _objUniversalImageSearchViewModel = new UniversalImageSearchViewModel();

                return _objUniversalImageSearchViewModel;
            }
        }

        /// <summary>
        /// creating property for SchoolsViewModel , which is used in MainViewModel.
        /// </summary>
        static SchoolsViewModel _objSchoolsViewModel;
        public SchoolsViewModel SchoolsViewModelBase
        {
            get
            {
                if (_objSchoolsViewModel == null)
                    _objSchoolsViewModel = new SchoolsViewModel();

                return _objSchoolsViewModel;
            }
        }

        /// <summary>
        /// creating property for PhotographyJobViewModel , which is used in MainViewModel.
        /// </summary>
        static PhotographyJobViewModel _objPhotographyJobViewModel;
        public PhotographyJobViewModel PhotographyJobViewModelBase
        {
            get
            {
                if (_objPhotographyJobViewModel == null)
                    _objPhotographyJobViewModel = new PhotographyJobViewModel();

                return _objPhotographyJobViewModel;
            }
        }

        /// <summary>
        /// creating property for ViewPhotoShootViewModel , which is used in MainViewModel.
        /// </summary>
        static View_Management.UserControls.ViewPhotoShootViewModel _objViewPhotoShootViewModel;
        public View_Management.UserControls.ViewPhotoShootViewModel ViewPhotoShootViewModelBase
        {
            get
            {
                if (_objViewPhotoShootViewModel == null)
                    _objViewPhotoShootViewModel = new View_Management.UserControls.ViewPhotoShootViewModel();

                return _objViewPhotoShootViewModel;

            }
        }

        /// <summary>
        /// creating property for LockedPhotoShootsViewModel , which is used in MainViewModel.
        /// </summary>
        static LockedPhotosViewModel _objLockedPhotosViewModel;
        public LockedPhotosViewModel LockedPhotosViewModelBase
        {
            get
            {
                if (_objLockedPhotosViewModel == null)
                    _objLockedPhotosViewModel = new LockedPhotosViewModel();

                return _objLockedPhotosViewModel;
            }
        }

        /// <summary>
        /// creating property for ActivateStudentsViewModel , which is used in MainViewModel.
        /// </summary>
        static ActivateStudentsViewModel _objActivateStudentsViewModel;
        public ActivateStudentsViewModel ActivateStudentsViewModelBase
        {
            get
            {
                if (_objActivateStudentsViewModel == null)
                    _objActivateStudentsViewModel = new ActivateStudentsViewModel();

                return _objActivateStudentsViewModel;
            }
        }

        /// <summary>
        /// creating property for ImportBatchesViewModel ,which is used in MainViewModel.
        /// </summary>
        static ImportBatchesViewModel _objImportBatchesViewModel;
        public ImportBatchesViewModel ImportBatchesViewModelBase
        {
            get
            {
                if (_objImportBatchesViewModel == null)
                    _objImportBatchesViewModel = new ImportBatchesViewModel();

                return _objImportBatchesViewModel;
            }
        }

        /// <summary>
        /// creating property for FileLocationViewModel ,which is used in MainViewModel.
        /// </summary>
        //static FileLocationViewModel _objFileLocationViewModel;
        //public FileLocationViewModel FileLocationViewModelBase
        //{
        //    get
        //    {
        //        if (_objFileLocationViewModel == null)
        //            _objFileLocationViewModel = new FileLocationViewModel();

        //        return _objFileLocationViewModel;
        //    }

        //}

        /// <summary>
        /// creating property for MaskViewModel ,which is used in MainViewModel.
        /// </summary>
        static MaskViewModel _objMaskViewModel;
        public MaskViewModel MaskViewModelBase
        {
            get
            {
                if (_objMaskViewModel == null)
                    _objMaskViewModel = new MaskViewModel();

                return _objMaskViewModel;
            }
        }

        /// <summary>
        /// creating property for OrderformDefaultPricingViewModel ,which is used in MainViewModel.
        /// </summary>
        static OrderformDefaultPricingViewModel _objOrderformDefaultPricingViewModel;
        public OrderformDefaultPricingViewModel OrderformDefaultPricingViewModelBase
        {
            get
            {
                if (_objOrderformDefaultPricingViewModel == null)
                    _objOrderformDefaultPricingViewModel = new OrderformDefaultPricingViewModel();

                return _objOrderformDefaultPricingViewModel;
            }
        }

        /// <summary>
        /// creating property for ActivitiesViewModel ,which is used in MainViewModel.
        /// </summary>
        static ActivitiesViewModel _objActivitiesViewModel;
        public ActivitiesViewModel ActivitiesViewModelBase
        {
            get
            {
                if (_objActivitiesViewModel == null)
                    _objActivitiesViewModel = new ActivitiesViewModel();

                return _objActivitiesViewModel;
            }
        }
        /// <summary>
        /// creating property for OrdersViewModel ,which is used in MainViewModel.
        /// </summary>
        static OrdersViewModel _objOrdersViewModel;
        public OrdersViewModel OrdersViewModelBase
        {
            get
            {
                if (_objOrdersViewModel == null)
                {
                    _objOrdersViewModel = new OrdersViewModel();
                }

                return _objOrdersViewModel;
            }
        }
        /// <summary>
        /// creating property for OrderPackagesViewModel ,which is used in MainViewModel.
        /// </summary>
        static OrderPackagesViewModel _objOrderPackagesViewModel;
        public OrderPackagesViewModel OrderPackagesViewModelBase
        {
            get
            {
                if (_objOrderPackagesViewModel == null)
                {
                    _objOrderPackagesViewModel = new OrderPackagesViewModel();
                }

                return _objOrderPackagesViewModel;
            }
        }

        static ValidatePhotoShootsViewModel _objValidatePhotoShootsViewModel;
        public ValidatePhotoShootsViewModel ValidatePhotoShootsViewModelBase
        {
            get
            {
                if (_objValidatePhotoShootsViewModel == null)
                {
                    _objValidatePhotoShootsViewModel = new ValidatePhotoShootsViewModel();
                }

                return _objValidatePhotoShootsViewModel;
            }
        }

        static SearchOrdersViewModel _objSearchOrdersViewModel;
        public SearchOrdersViewModel SearchOrdersViewModelBase
        {
            get
            {
                if (_objSearchOrdersViewModel == null)
                {
                    _objSearchOrdersViewModel = new SearchOrdersViewModel();
                }

                return _objSearchOrdersViewModel;
            }
        }

        /// <summary>
        /// creating property for WorkflowItemsViewModel ,which is used in MainViewModel.
        /// </summary>
        static WorkflowItemsViewModel _objWorkflowItemsViewModel;
        public WorkflowItemsViewModel WorkflowItemsViewModelBase
        {
            get
            {
                if (_objWorkflowItemsViewModel == null)
                {
                    _objWorkflowItemsViewModel = new WorkflowItemsViewModel();
                }

                return _objWorkflowItemsViewModel;
            }
        }

        /// <summary>
        /// creating property for WorkflowCollectionsViewModel ,which is used in MainViewModel.
        /// </summary>
        static WorkflowCollectionsViewModel _objWorkflowCollectionsViewModel;
        public WorkflowCollectionsViewModel WorkflowCollectionsViewModelBase
        {
            get
            {
                if (_objWorkflowCollectionsViewModel == null)
                {
                    _objWorkflowCollectionsViewModel = new WorkflowCollectionsViewModel();
                }

                return _objWorkflowCollectionsViewModel;
            }
        }

        /// <summary>
        /// creating property for WorkflowCollectionsViewModel ,which is used in MainViewModel.
        /// </summary>
        static DashBoardWorkflowItemsViewModel _objDashBoardWorkflowItemsViewModel;
        public DashBoardWorkflowItemsViewModel DashBoardWorkflowItemsViewModelBase
        {
            get
            {
                if (_objDashBoardWorkflowItemsViewModel == null)
                {
                    _objDashBoardWorkflowItemsViewModel = new DashBoardWorkflowItemsViewModel();
                }

                return _objDashBoardWorkflowItemsViewModel;
            }
        }
        /// <summary>
        /// creating property for ImageQuixAccountsViewModel ,which is used in MainViewModel.
        /// </summary>
        static ImageQuixAccountsViewModel _objImageQuixAccountsViewModel;
        public ImageQuixAccountsViewModel ImageQuixAccountsViewModelBase
        {
            get
            {
                if (_objImageQuixAccountsViewModel == null)
                {
                    _objImageQuixAccountsViewModel = new ImageQuixAccountsViewModel();
                }

                return _objImageQuixAccountsViewModel;
            }
        }
        /// <summary>
        /// creating property for ManageUsersViewModel ,which is used in MainViewModel.
        /// </summary>
        static ManageUsersViewModel _objManageUsersViewModel;
        public ManageUsersViewModel ManageUsersViewModelBase
        {
            get
            {
                if (_objManageUsersViewModel == null)
                {
                    _objManageUsersViewModel = new ManageUsersViewModel();
                }

                return _objManageUsersViewModel;
            }
        }
        /// <summary>
        /// creating property for SPPriceSheetViewModel ,which is used in MainViewModel.
        /// </summary>
        static SPPriceSheetViewModel _objSPPriceSheetViewModel;
        public SPPriceSheetViewModel SPPriceSheetViewModelBase
        {
            get
            {
                if (_objSPPriceSheetViewModel == null)
                {
                    _objSPPriceSheetViewModel = new SPPriceSheetViewModel();
                }

                return _objSPPriceSheetViewModel;
            }
        }

        /// <summary>
        /// creating property for ActiveStudiosViewModel ,which is used in MainViewModel.
        /// </summary>
        static ActiveStudiosViewModel _objActiveStudiosViewModel;
        public ActiveStudiosViewModel ActiveStudiosViewModelBase
        {
            get
            {
                if (_objActiveStudiosViewModel == null)
                {
                    _objActiveStudiosViewModel = new ActiveStudiosViewModel();
                }

                return _objActiveStudiosViewModel;
            }
        }

        /// <summary>
        /// creating property for WorkflowItemsViewModel ,which is used in MainViewModel.
        /// </summary>
        static EquipmentItemsViewModel _objEquipmentItemsViewModel;
        public EquipmentItemsViewModel EquipmentItemsViewModelBase
        {
            get
            {
                if (_objEquipmentItemsViewModel == null)
                {
                    _objEquipmentItemsViewModel = new EquipmentItemsViewModel();
                }

                return _objEquipmentItemsViewModel;
            }
        }
        
        /// <summary>
        /// creating property for PhotoShootsWithoutWorkflowsViewModel ,which is used in MainViewModel.
        /// </summary>
        static PhotoShootsWithoutWorkflowsViewModel _objPhotoShootsWithoutWorkflowsViewModel;
        public PhotoShootsWithoutWorkflowsViewModel PhotoShootsWithoutWorkflowsViewModelBase
        {
            get
            {
                if (_objPhotoShootsWithoutWorkflowsViewModel == null)
                {
                    _objPhotoShootsWithoutWorkflowsViewModel = new PhotoShootsWithoutWorkflowsViewModel();
                }

                return _objPhotoShootsWithoutWorkflowsViewModel;
            }
        }

    }
}
