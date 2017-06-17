using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.WorkPlace.UserControls
{
    public class ActivateStudentsViewModel : ViewModelBase
    {

        #region Properties
        private IEnumerable<Student> _dgActivateStudentsData;
        private List<Student> _selectedStudentsList;
        private bool _activateStudentsShowGroupPanel;
        bool _isSearchControlVisible;
        SearchControl _activateStudentsSearchControl;
        ShowSearchPanelMode _activateStudentsSearchPanelMode;

        public SearchControl activateStudentsSearchControl
        {
            get { return _activateStudentsSearchControl; }
            set { _activateStudentsSearchControl = value; NotifyPropertyChanged("activateStudentsSearchControl"); }
        }
        public ShowSearchPanelMode activateStudentsSearchPanelMode
        {
            get { return _activateStudentsSearchPanelMode; }
            set { _activateStudentsSearchPanelMode = value; NotifyPropertyChanged("activateStudentsSearchPanelMode"); }
        }
        public bool isSearchControlVisible
        {
            get { return _isSearchControlVisible; }
            set { _isSearchControlVisible = value; NotifyPropertyChanged("isSearchControlVisible"); }
        }
        public bool activateStudentsShowGroupPanel
        {
            get { return _activateStudentsShowGroupPanel; }
            set { _activateStudentsShowGroupPanel = value; NotifyPropertyChanged("activateStudentsShowGroupPanel"); }
        }
        public List<Student> selectedStudentsList
        {
            get { return _selectedStudentsList; }
            set { _selectedStudentsList = value; NotifyPropertyChanged("selectedStudentsList"); }
        }
        public IEnumerable<Student> dgActivateStudentsData
        {
            get { return _dgActivateStudentsData; }
            set { _dgActivateStudentsData = value; NotifyPropertyChanged("dgActivateStudentsData"); }
        }
        #endregion

        #region Constructors
        public ActivateStudentsViewModel()
        {
            selectedStudentsList = new List<Student>();
            ActivateStudentsView();
        }
        #endregion

        #region Methods
        internal void searchPanels()
        {
            if (activateStudentsSearchControl == null || !isSearchControlVisible) //!PhotoJobTableView.SearchControl.IsVisible
            {
                activateStudentsSearchPanelMode = ShowSearchPanelMode.Always; isSearchControlVisible = true;
            }
            else
            {
                activateStudentsSearchPanelMode = ShowSearchPanelMode.Never; isSearchControlVisible = false;
            }
        }
        internal void groupPanels()
        {
            if (activateStudentsShowGroupPanel)
                activateStudentsShowGroupPanel = false;
            else
                activateStudentsShowGroupPanel = true;
        }

        internal void bindToGrid()
        {
            try
            {
                dgActivateStudentsData = clsDashBoard.getDeactivateStudents(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        internal void ActivateStudentsView()
        {
            try
            {
                ArrayList arrStudentId = new ArrayList();
                foreach (Student tempStudent in selectedStudentsList)
                {
                    try
                    {
                        int studentId = Convert.ToInt32(tempStudent.ID);
                        if (!arrStudentId.Contains(studentId))
                        {
                            arrStudentId.Add(studentId);
                        }
                    }
                    catch (Exception ex)
                    {
                        MVVMMessageService.ShowMessage(ex.Message);
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                }
                if (arrStudentId.Count > 0)
                {
                    int UpdateStudent = clsStudent.UpdateStudentStatus(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudentId);
                    bindToGrid();
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        internal void setButtonsVisibility()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
            (System.Windows.Application.Current as App).isDragVisible = true; (System.Windows.Application.Current as App).isSearchVisible = true;
            (System.Windows.Application.Current as App).isRefreshVisible = true;
        }
        #endregion
    }
}
