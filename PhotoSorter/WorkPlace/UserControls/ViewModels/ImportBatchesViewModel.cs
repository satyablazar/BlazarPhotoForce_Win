using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Collections.ObjectModel;
using PhotoForce.Student_Management;

namespace PhotoForce.WorkPlace.UserControls
{
    public class ImportBatchesViewModel : ViewModelBase
    {
        #region Initilizations
        public bool isFromStudent;
        int rowIndex;
        #endregion

        #region Properties
        private DataTable _dgStudentImportData;
        private DataRowView _studentImportSelectedItem;

        public DataRowView studentImportSelectedItem
        {
            get { return _studentImportSelectedItem; }
            set
            {
                _studentImportSelectedItem = value; NotifyPropertyChanged("studentImportSelectedItem");
                if (studentImportSelectedItem != null) { rowIndex = dgStudentImportData.Rows.IndexOf(studentImportSelectedItem.Row); }
            }
        }
        public DataTable dgStudentImportData
        {
            get { return _dgStudentImportData; }
            set { _dgStudentImportData = value; NotifyPropertyChanged("dgStudentImportData"); }
        }
        #endregion

        #region Constructors
        public ImportBatchesViewModel()
        {

        }
        public ImportBatchesViewModel(bool isfromStudentsImport)
        {
            bindData(isfromStudentsImport);
        }
        #endregion

        #region Commands
        public RelayCommand ImportBatchesDoubleClickCommand
        {
            get
            {
                return new RelayCommand(importBatchesDoubleClick);
            }
        }
        #endregion

        #region Methods
        internal void bindData(bool isfromStudentsImport)
        {
            try
            {
                isFromStudent = isfromStudentsImport;
                dgStudentImportData = clsDashBoard.getStudentImportData(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId, isfromStudentsImport);

                if (dgStudentImportData == null) { return; }
                if (dgStudentImportData.Rows.Count != 0 && rowIndex <= dgStudentImportData.Rows.Count) { studentImportSelectedItem = dgStudentImportData.DefaultView[rowIndex]; }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        //internal void getFileData()
        //{
        //    try
        //    {
        //        if (studentImportFocusedRow != null)
        //        {
        //            if (studentImportFocusedRow.DataFile != null)
        //            {
        //                importDataFile = studentImportFocusedRow.DataFile.ToArray();
        //                importFileDesc = studentImportFocusedRow.Description;

        //                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
        //                var res = dlg.ShowDialog();

        //                if (res != false)
        //                {
        //                    String[] importFile = { Encoding.UTF8.GetString(importDataFile) };
        //                    File.WriteAllLines(dlg.SelectedPath + "\\" + importFileDesc + "" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".txt", importFile);
        //                    Process.Start(dlg.SelectedPath + "\\" + importFileDesc + "" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".txt");
        //                }
        //            }
        //            else
        //                MVVMMessageService.ShowMessage(errorMessages.MESSAGE_FOR_IMPORT_FILE);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        clsStatic.WriteExceptionLogXML(ex);
        //        MVVMMessageService.ShowMessage(ex.Message);
        //    }
        //}

        void importBatchesDoubleClick()
        {
            if (studentImportSelectedItem != null && isFromStudent)
            {
                editImportBatches();
            }
        }

        internal void editImportBatches()
        {
            if (!isFromStudent) { return; }
            if (studentImportSelectedItem != null)
            {
                EditImportBatches _objEditImportBatches = new EditImportBatches(studentImportSelectedItem);
                _objEditImportBatches.ShowDialog();


                if (((EditImportBatchesViewModel)(_objEditImportBatches.DataContext)).isSave)
                {
                    bindData(true);
                }
            }
        }

        public void groupPanels()
        {
            try
            {
                //if (photographyJobShowGroupPanel)
                //    photographyJobShowGroupPanel = false;
                //else
                //    photographyJobShowGroupPanel = true;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        internal void setButtonVisibility()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
            (System.Windows.Application.Current as App).isDeleteVisible = false; (System.Windows.Application.Current as App).isEditVisible = true;
            (System.Windows.Application.Current as App).isDragVisible = false; (System.Windows.Application.Current as App).isSearchVisible = false;
            (System.Windows.Application.Current as App).isRefreshVisible = false; (System.Windows.Application.Current as App).isDeleteAllVisible = false;
        }
        #endregion
    }
}
