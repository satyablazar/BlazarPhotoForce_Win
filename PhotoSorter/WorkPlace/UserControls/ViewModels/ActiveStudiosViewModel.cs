using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using PhotoForce.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.WorkPlace.UserControls
{
    public class ActiveStudiosViewModel : ViewModelBase
    {

        #region Initialization
        PhotoSorterDBModelDataContext db;
        #endregion

        #region Properties
        ObservableCollection<Studio> _dgStudiosData;
        ObservableCollection<Studio> _selectedStudios;
        Studio _selectedStudio;

        public ObservableCollection<Studio> dgStudiosData
        {
            get { return _dgStudiosData; }
            set { _dgStudiosData = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<Studio> selectedStudios
        {
            get { return _selectedStudios; }
            set { _selectedStudios = value; NotifyPropertyChanged(); }
        }
        public Studio selectedStudio
        {
            get { return _selectedStudio; }
            set { _selectedStudio = value; NotifyPropertyChanged(); }
        }
        #region Group & Search panels
        bool _studiosShowGroupPanel;

        public bool studiosShowGroupPanel
        {
            get { return _studiosShowGroupPanel; }
            set { _studiosShowGroupPanel = value; NotifyPropertyChanged(); }
        }

        ShowSearchPanelMode _studiosSearchPanelMode;
        SearchControl _studiosSearchControl;
        bool _isSearchControlVisible;

        public ShowSearchPanelMode studiosSearchPanelMode
        {
            get { return _studiosSearchPanelMode; }
            set { _studiosSearchPanelMode = value; NotifyPropertyChanged(); }
        }
        public SearchControl studiosSearchControl
        {
            get { return _studiosSearchControl; }
            set { _studiosSearchControl = value; NotifyPropertyChanged(); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged(); }
        }
        #endregion
        #endregion

        public ActiveStudiosViewModel()
        {
            dgStudiosData = new ObservableCollection<Studio>();
            selectedStudios = new ObservableCollection<Studio>();
            bindData();
        }

        #region Commands
        public RelayCommand StudiosMouseUpCommand
        {
            get
            {
                return new RelayCommand(StudiosTableKeyUp);
            }
        }
        public RelayCommand RowUpdateCommand
        {
            get
            {
                return new RelayCommand(inlineGridEdit);
            }
        }
        #endregion

        #region Methods
        internal void bindData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            dgStudiosData = new ObservableCollection<Studio>((from Stu in db.Studios orderby Stu.Id select Stu).ToList());
            if (dgStudiosData.Count > 0)
                selectedStudio = dgStudiosData.First();
        }
        internal void newStudio()
        {
            AddEditStudio _objAddEditStudio = new AddEditStudio();
            _objAddEditStudio.ShowDialog();

            bindData();
            if (dgStudiosData.Count > 0)
                selectedStudio = dgStudiosData[dgStudiosData.Count - 1];
        }
        internal void editStudio()
        {
            if (selectedStudio != null)
            {
                AddEditStudio _objAddEditStudio = new AddEditStudio(selectedStudio);
                _objAddEditStudio.ShowDialog();

                if (((AddEditStudioViewModel)(_objAddEditStudio.DataContext)).isSave)
                {
                    // selectedUser["UserName"] = (((AddEditUsersViewModel)(_objAddEditUsers.DataContext)).userName).ToString();
                    try
                    {
                        dgStudiosData.Insert(dgStudiosData.Count <= 1 ? 0 : dgStudiosData.IndexOf(selectedStudio), ((AddEditStudioViewModel)(_objAddEditStudio.DataContext))._objStudio);
                        dgStudiosData.Remove(selectedStudio);
                        selectedStudio = ((AddEditStudioViewModel)(_objAddEditStudio.DataContext))._objStudio;
                        selectedStudios.Add(selectedStudio);
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.Message);
                    }
                }
            }
        }
        internal void deletePriceSheet()
        {
            if (selectedStudios.Count > 0)
            {
                try
                {
                    //string message = "";
                    //if (selectedStudios.Count == 1)
                    //    message = "Are you sure you want to delete studio '" + selectedStudio.StudioName.ToString() + "' ?";
                    //else
                    //    message = "Are you sure you want to delete selected studios ?";
                    //string caption = "Confirmation";
                    //System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    //System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    //if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    //{
                    //    ArrayList selectedIds = new ArrayList();
                    //    foreach (Studio tempstudio in selectedStudios)
                    //        selectedIds.Add(tempstudio.Id);

                    //    //clsUsers.deleteStudio(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), selectedIds);
                    //    bindData();
                    //    if (dgStudiosData.Count > 0)
                    //        selectedStudio = dgStudiosData[dgStudiosData.Count - 1];
                    //}
                }
                catch (Exception ex)
                {
                    //MVVMMessageService.ShowMessage(ex.Message);
                }
            }
        }

        #region group and search panels
        # region Search panel
        /// <summary>
        /// This method is used to search panels
        /// </summary>
        internal void searchPanels()
        {
            if (studiosSearchControl == null || isSearchControlVisible == false)
            {
                studiosSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
            else
            {
                studiosSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
            }
        }
        # endregion

        # region Group panels
        /// <summary>
        /// This method is used to group panels based on selected column
        /// </summary>
        internal void groupPanels()
        {
            if (studiosShowGroupPanel)
                studiosShowGroupPanel = false;
            else
                studiosShowGroupPanel = true;
        }
        # endregion
        #endregion

        private void inlineGridEdit()
        {
            try
            {
                db.SubmitChanges();
                //clsDashBoard.updateUser(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(selectedUser.Id), selectedUser.UserName.ToString());
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void StudiosTableKeyUp()
        {
            setButtonVisibility();
        }

        #region Buttons visibility        
        /// <summary>
        /// this method used for buttons visibility
        /// </summary>        
        public void setButtonVisibility()
        {
            (Application.Current as App).setAllButtonsVisibility();
            (Application.Current as App).isDragVisible = true; (Application.Current as App).isSearchVisible = true;
            (Application.Current as App).isNewVisible = true; (Application.Current as App).isEditVisible = true;
            //(Application.Current as App).isDeleteVisible = true;
        }

        #endregion

        #endregion
    
    }
}
