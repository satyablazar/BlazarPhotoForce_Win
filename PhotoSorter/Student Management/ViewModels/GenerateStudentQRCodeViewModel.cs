using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using PhotoForce.App_Code;
using PhotoForce.MVVM;

namespace PhotoForce.Student_Management
{
    public class GenerateStudentQRCodeViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        ArrayList arrSelectedStudents;
        ArrayList arrFilterStudents;
        ArrayList arrImportBatchID;
        //PrintStudentsQRCode _objPrintStudentsQRCode;
        StudentQRCodeOneStudentGroupByTeacher _objStudentQRCodeOneStudentGroupByTeacher;
        StudentQRCodeOneStudentWithOutTeacher _objStudentQRCodeOneStudentWithOutTeacher;
        StudentQRCodeEightStudentsGroupByTeacher _objStudentQRCodeEightStudentsGroupByTeacher;
        StudentQRCodeEightStudentsWithOutGroupByTeacher _objStudentQRCodeEightStudentsWithOutGroupByTeacher;
        //string[] studentGridSortedColumn;
        #endregion

        #region Properties
        private bool _isSelectedStudents;
        private bool _isPrintWithTeacher;
        private bool _isFilteredStudents;
        private bool _printOneIsChecked;
        private bool _printEightIsChecked;

        public bool printEightIsChecked
        {
            get { return _printEightIsChecked; }
            set { _printEightIsChecked = value; NotifyPropertyChanged(); }
        }
        public bool printOneIsChecked
        {
            get { return _printOneIsChecked; }
            set { _printOneIsChecked = value; NotifyPropertyChanged(); }
        }
        public bool isFilteredStudents
        {
            get { return _isFilteredStudents; }
            set { _isFilteredStudents = value; NotifyPropertyChanged("isFilteredStudents"); }
        }
        public bool isPrintWithTeacher
        {
            get { return _isPrintWithTeacher; }
            set { _isPrintWithTeacher = value; NotifyPropertyChanged("isPrintWithTeacher"); }
        }
        public bool isSelectedStudents
        {
            get { return _isSelectedStudents; }
            set { _isSelectedStudents = value; NotifyPropertyChanged("isSelectedStudents"); }
        }
        #endregion

        #region Constructor
        public GenerateStudentQRCodeViewModel(ArrayList selectedStudent, ArrayList filteredStudent, ArrayList importBatchId)//, string[] sortedColumn
        {
            //studentGridSortedColumn = null;
            arrSelectedStudents = selectedStudent;
            arrFilterStudents = filteredStudent;
            arrImportBatchID = importBatchId;
            //studentGridSortedColumn = sortedColumn;
            isFilteredStudents = true; isPrintWithTeacher = true;
            printOneIsChecked = true;
            if (selectedStudent.Count == 0)
                isSelectedStudents = false;
            else
                isSelectedStudents = true;
        }
        #endregion

        #region Commands
        public RelayCommand GenerateQRCodesCommand
        {
            get
            {
                return new RelayCommand(generateQRCodes);
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
        private void generateQRCodes()
        {
            if (printOneIsChecked)
            {
                if (isPrintWithTeacher == true)
                {
                    if (isFilteredStudents == true)
                    {
                        _objStudentQRCodeOneStudentGroupByTeacher = new StudentQRCodeOneStudentGroupByTeacher(arrFilterStudents);//, studentGridSortedColumn
                    }
                    else
                    {
                        _objStudentQRCodeOneStudentGroupByTeacher = new StudentQRCodeOneStudentGroupByTeacher(arrSelectedStudents);//, studentGridSortedColumn
                    }
                    _objStudentQRCodeOneStudentGroupByTeacher.ShowDialog();
                }
                else
                {
                    if (isFilteredStudents == true)
                    {
                        _objStudentQRCodeOneStudentWithOutTeacher = new StudentQRCodeOneStudentWithOutTeacher(arrFilterStudents); //, studentGridSortedColumn //By default data comes in sort order so no need to do sorting again
                    }
                    else
                    {
                        _objStudentQRCodeOneStudentWithOutTeacher = new StudentQRCodeOneStudentWithOutTeacher(arrSelectedStudents);//, studentGridSortedColumn
                    }
                    _objStudentQRCodeOneStudentWithOutTeacher.ShowDialog();
                }
            }
            else if (printEightIsChecked)
            {
                if (isPrintWithTeacher)
                {
                    if (isFilteredStudents)
                    {
                        //objPrintBarcode = new PrintStudentsBarcode(arrFilterStudents);
                        _objStudentQRCodeEightStudentsGroupByTeacher = new StudentQRCodeEightStudentsGroupByTeacher(arrFilterStudents);
                    }
                    else
                    {
                        //objPrintBarcode = new PrintStudentsBarcode(arrSelectedStudents);
                        _objStudentQRCodeEightStudentsGroupByTeacher = new StudentQRCodeEightStudentsGroupByTeacher(arrSelectedStudents);
                    }
                    //objPrintBarcode.ShowDialog();
                    _objStudentQRCodeEightStudentsGroupByTeacher.ShowDialog();
                }
                else
                {
                    if (isFilteredStudents)
                    {
                        //objPrintBarcode = new PrintStudentsBarcode(arrFilterStudents);
                        _objStudentQRCodeEightStudentsWithOutGroupByTeacher = new StudentQRCodeEightStudentsWithOutGroupByTeacher(arrFilterStudents);
                    }
                    else
                    {
                        //objPrintBarcode = new PrintStudentsBarcode(arrSelectedStudents);
                        _objStudentQRCodeEightStudentsWithOutGroupByTeacher = new StudentQRCodeEightStudentsWithOutGroupByTeacher(arrSelectedStudents);
                    }
                    //objPrintBarcode.ShowDialog();
                    _objStudentQRCodeEightStudentsWithOutGroupByTeacher.ShowDialog();
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
