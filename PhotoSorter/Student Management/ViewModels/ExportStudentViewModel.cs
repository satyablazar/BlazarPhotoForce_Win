    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.Extensions;
using System.Collections;
using System.Data;
using System.IO;
using System.Diagnostics;
using Ookii.Dialogs.Wpf;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using Excel;
using PhotoForce.Helpers;
using PhotoForce.Mask_Management;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;

namespace PhotoForce.Student_Management
{
    public class ExportStudentViewModel : ViewModelBase
    {
        #region Initialization
        SearchSchool _objSearchSchool;
        ArrayList arrStudImgId = new ArrayList();
        ArrayList arrAllFilteredList = new ArrayList();
        public string pathToSave = "";
        string fileName = "StudentDatafile.txt";

        #endregion

        #region Properties
        string _schoolName;
        string _alternateFolderPath;
        bool _isAlternateFolder;
        bool _isOutputFolder;
        bool _isSelectedStudnets;
        bool _isFilterStudents;
        bool _isGotPhotoExportChecked;

        public bool isGotPhotoExportChecked
        {
            get { return _isGotPhotoExportChecked; }
            set { _isGotPhotoExportChecked = value; NotifyPropertyChanged("isGotPhotoExportChecked"); }
        }
        public bool isFilterStudents
        {
            get { return _isFilterStudents; }
            set { _isFilterStudents = value; NotifyPropertyChanged("isFilterStudents"); }
        }
        public bool isSelectedStudnets
        {
            get { return _isSelectedStudnets; }
            set { _isSelectedStudnets = value; NotifyPropertyChanged("isSelectedStudnets"); }
        }
        public bool isOutputFolder
        {
            get { return _isOutputFolder; }
            set { _isOutputFolder = value; NotifyPropertyChanged("isOutputFolder"); }
        }
        public bool isAlternateFolder
        {
            get { return _isAlternateFolder; }
            set { _isAlternateFolder = value; NotifyPropertyChanged("isAlternateFolder"); }
        }
        public string alternateFolderPath
        {
            get { return _alternateFolderPath; }
            set { _alternateFolderPath = value; NotifyPropertyChanged("alternateFolderPath"); }
        }
        public string schoolName
        {
            get { return _schoolName; }
            set { _schoolName = value; NotifyPropertyChanged("SchoolName"); }
        }
        #endregion

        #region Constructors
        public ExportStudentViewModel(int schoolId, string tempSchoolName, ArrayList arrStuId)
        {
            isGotPhotoExportChecked = false;
            isFilterStudents = true; isOutputFolder = true;
            arrStudImgId = arrStuId;
            //imageFolderPath = ((Setting)clsDashBoard.getSettingByName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), "ImageFolderLocation")).settingValue;   //why this code??   //commented by mohan
            getDefaultSchool();
        }
        public ExportStudentViewModel(int schoolId, string tempSchoolName, ArrayList arrstuId, ArrayList arrAllFilteredLst)
        {
            isFilterStudents = true; isOutputFolder = true;
            arrStudImgId = arrstuId;
            arrAllFilteredList = arrAllFilteredLst;
            //imageFolderPath = ((Setting)clsDashBoard.getSettingByName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), "ImageFolderLocation")).settingValue;       //why this code??   //mohan
            getDefaultSchool();
        }
        public ExportStudentViewModel(int schoolId, string tempSchoolName)
        {
            isFilterStudents = true; isOutputFolder = true;
            getDefaultSchool();
        }
        #endregion

        #region Commands

        public RelayCommand ExportCommand
        {
            get
            {
                return new RelayCommand(export);
            }
        }
        public RelayCommand BrowseCommand
        {
            get
            {
                return new RelayCommand(browseAlternateFolder);
            }
        }
        public RelayCommand AlternateFolderCommand
        {
            get
            {
                return new RelayCommand(alternateFolder);
            }
        }
        public RelayCommand OutputFolderCommand
        {
            get
            {
                return new RelayCommand(outputFolder);
            }
        }
        public RelayCommand SearchCommand
        {
            get
            {
                return new RelayCommand(search);
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
        private void export()
        {
            try
            {
                int i = 0;
                DataTable objStudents = new DataTable();
                if (isOutputFolder == true)
                {
                    if (clsSchool.defaultSchoolId > 0)
                        pathToSave = clsDashBoard.getSchoolFolderPath(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), clsSchool.defaultSchoolId);
                }
                else
                    pathToSave = alternateFolderPath;

                if (!Directory.Exists(pathToSave))
                {
                    MVVMMessageService.ShowMessage(errorMessages.PATH_DOES_NOT_EXISTS);
                    return;
                }
                if (isSelectedStudnets == true)
                {
                    if (arrStudImgId.Count > 0)
                    {
                        if (!isGotPhotoExportChecked)
                            objStudents = clsStudent.GetAllStudentsForExport(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudImgId);
                        else
                            objStudents = clsStudent.GetAllGotPhotoStudentsForExport(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrStudImgId);
                    }
                }
                else if (arrAllFilteredList.Count > 0)
                {
                    if (!isGotPhotoExportChecked)
                        objStudents = clsStudent.GetAllStudentsForExport(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrAllFilteredList);
                    else
                        objStudents = clsStudent.GetAllGotPhotoStudentsForExport(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrAllFilteredList);
                }

                if (objStudents.Rows.Count <= 0)
                {
                    MVVMMessageService.ShowMessage("No data found");
                    return;
                }
                #region
                //displays warning message if any student password is null
                //else
                //{
                //    foreach (DataRow stu in objStudents.Rows)
                //    {
                //        //string s = stu["Password"].ToString();
                //        if (string.IsNullOrEmpty(stu["Password"].ToString()))
                //        {
                //            string message = "One or more students with no password(s) attached.\n Do you want to continue with the export?";
                //            string caption = "Confirmation";
                //            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                //            System.Windows.MessageBoxImage iconn = System.Windows.MessageBoxImage.Question;
                //            if (MVVMMessageService.ShowMessage(message, caption, buttons, iconn) == System.Windows.MessageBoxResult.No)
                //                return;
                //            else
                //                break;
                //        }
                //    }
                //}
                #endregion

                string filePath = pathToSave + "\\" + fileName;
                string backupFilePath = pathToSave + "\\backup of " + fileName.Replace("txt", "xlsx");
                if (File.Exists(filePath))
                {
                    string message = errorMessages.STUDENT_DATA_FILE_EXIST1 + pathToSave + errorMessages.STUDENT_DATA_FILE_EXIST2;
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage iconn = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, iconn) == System.Windows.MessageBoxResult.No)
                        return;
                }
                if (isGotPhotoExportChecked)
                {
                    string gotphotopath = filePath.Replace("txt", "csv");
                    using (StreamWriter writer = new StreamWriter(gotphotopath))
                    {
                        Rfc4180Writer.WriteDataTable(objStudents, writer, true);
                    }
                }
                else
                    CreateExcelFile.CreateExcelDocument(objStudents, filePath.Replace("txt", "xlsx"));

                bool isError = false;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
                {
                    for (i = 0; i < objStudents.Rows.Count; i++)
                    {
                        try
                        {
                            if (i == 0)
                            {
                                if (!isGotPhotoExportChecked)
                                    file.WriteLine("First Name\tLast Name\tOfficial FN\tOfficial LN\tStudent ID\tPassword\tTeacher\tGrade\tSchool Year\tCustom1\tCustom2\tCustom3\tCustom4\tCustom5\tSchoolCampus");
                                else
                                {
                                     //file.WriteLine("password\tfirstname\tlastname\tinstitution\tidentifier\tgroup\tteacher\texpires\tstreet\tzip\tcity\tstate\tparent_firstname\tparent_lastname\tparent_email\tparent_phone");

                                     file.WriteLine("Firstname\tLastname\tGender\tBirthdate\tInstitution\tIdentifier\tGroup\tTeacher\tExpires\tStreet\tZip\tCity\tState\tBuyer Firstname\tBuyer Lastname\tBuyer Email\tBuyer Phone\tBuyer2 Firstname\tBuyer2 Lastname\tBuyer2 Email\tBuyer2 Phone\tAccess Code");
                                }
                            }
                            if (!isGotPhotoExportChecked)
                            {
                                file.WriteLine(
                                    objStudents.Rows[i]["FirstName"] + "\t" + objStudents.Rows[i]["Lastname"] + "\t" + objStudents.Rows[i]["OfficialFirstName"] + "\t" + objStudents.Rows[i]["OfficialLastName"] + "\t" + objStudents.Rows[i]["StudentID"] + "\t" + objStudents.Rows[i]["Password"] + "\t" +
                                    objStudents.Rows[i]["Teacher"] + "\t" + objStudents.Rows[i]["Grade"] + "\t" + objStudents.Rows[i]["SchoolYear"] + "\t" + objStudents.Rows[i]["ID"] + "\t" + objStudents.Rows[i]["Custom2"] + "\t" + objStudents.Rows[i]["Custom3"] + "\t" + objStudents.Rows[i]["Custom4"] + "\t" + objStudents.Rows[i]["Custom5"] + "\t" + objStudents.Rows[i]["SchoolCampus"]
                                    );
                            }
                            else
                            {
                                //file.WriteLine(
                                //    objStudents.Rows[i]["password"] + "\t" + objStudents.Rows[i]["firstname"] + "\t" + objStudents.Rows[i]["lastname"] + "\t" + objStudents.Rows[i]["institution"] + "\t" + objStudents.Rows[i]["identifier"] + "\t" + objStudents.Rows[i]["group"] + "\t" +
                                //    objStudents.Rows[i]["teacher"] + "\t" + objStudents.Rows[i]["expires"] + "\t" + objStudents.Rows[i]["street"] + "\t" + objStudents.Rows[i]["zip"] + "\t" + objStudents.Rows[i]["city"] + "\t" + objStudents.Rows[i]["state"] + "\t" +
                                //    objStudents.Rows[i]["parent_firstname"] + "\t" + objStudents.Rows[i]["parent_lastname"] + "\t" + objStudents.Rows[i]["parent_email"] + "\t" + objStudents.Rows[i]["parent_phone"]
                                //    );

                                file.WriteLine(
                                   objStudents.Rows[i]["Firstname"] + "\t" + objStudents.Rows[i]["Lastname"] + "\t" + objStudents.Rows[i]["Gender"] + "\t" + objStudents.Rows[i]["Birthdate"] + "\t" +
                                   objStudents.Rows[i]["Institution"] + "\t" + objStudents.Rows[i]["Identifier"] + "\t" + objStudents.Rows[i]["Group"] + "\t" +
                                   objStudents.Rows[i]["Teacher"] + "\t" + objStudents.Rows[i]["Expires"] + "\t" + objStudents.Rows[i]["Street"] + "\t" + objStudents.Rows[i]["Zip"] + "\t" + objStudents.Rows[i]["City"] + "\t" + objStudents.Rows[i]["State"] + "\t" +
                                   objStudents.Rows[i]["Buyer Firstname"] + "\t" + objStudents.Rows[i]["Buyer Lastname"] + "\t" + objStudents.Rows[i]["Buyer Email"] + "\t" + objStudents.Rows[i]["Buyer Phone"] + "\t" +
                                   objStudents.Rows[i]["Buyer2 Firstname"] + "\t" + objStudents.Rows[i]["Buyer2 Lastname"]+ "\t" + objStudents.Rows[i]["Buyer2 Email"] + "\t" + objStudents.Rows[i]["Buyer2 Phone"] + "\t" + objStudents.Rows[i]["Access Code"]
                                   );
                            }

                        }
                        catch (Exception ex)
                        {
                            isError = true;
                            clsStatic.WriteExceptionLogXML(ex);
                        }
                    }
                }                
                if (!isError)
                {
                    if(!isGotPhotoExportChecked)
                        File.Copy(filePath.Replace("txt", "xlsx"), backupFilePath, true); //create one more file as back up so neal can work with original file                    
                    string message = errorMessages.GET_EXPORT_DATA1 + objStudents.Rows.Count + errorMessages.GET_EXPORT_DATA2;
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage iconn = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, iconn) == System.Windows.MessageBoxResult.Yes)
                    {
                        Process.Start(pathToSave);
                    }
                    DialogResult = false;
                }
                else
                {
                    MVVMMessageService.ShowMessage(errorMessages.EXPORT_WITH_ERRORS);
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        

        //public string DataTableToCSV(DataTable datatable, char seperator)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < datatable.Columns.Count; i++)
        //    {
        //        sb.Append(datatable.Columns[i]);
        //        if (i < datatable.Columns.Count - 1)
        //            sb.Append(seperator);
        //    }
        //    sb.AppendLine();
        //    foreach (DataRow dr in datatable.Rows)
        //    {
        //        for (int i = 0; i < datatable.Columns.Count; i++)
        //        {
        //            sb.Append(dr[i].ToString());

        //            if (i < datatable.Columns.Count - 1)
        //                sb.Append(seperator);
        //        }
        //        sb.AppendLine();
        //    }
        //    return sb.ToString();
        //}

        private void browseAlternateFolder()
        {
            try
            {
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                var res = dlg.ShowDialog();
                if (res != false)
                    alternateFolderPath = dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void alternateFolder()
        {
            isOutputFolder = false; isAlternateFolder = true;
            schoolName = "";
        }
        private void outputFolder()
        {
            isAlternateFolder = false; isOutputFolder = true;
            schoolName = clsSchool.defaultSchoolName;
            alternateFolderPath = "";
        }
        private void search()
        {
            try
            {
                _objSearchSchool = new SearchSchool(arrStudImgId, "ExportStudents");
                _objSearchSchool.ShowDialog();
                schoolName = clsSchool.defaultSchoolName;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        internal void getDefaultSchool()
        {
            schoolName = clsSchool.defaultSchoolName;
        }
        #endregion
    }

    public class Rfc4180Writer
    {
        public static void WriteDataTable(DataTable sourceTable, TextWriter writer, bool includeHeaders) 
        {
        if (includeHeaders) 
        {
            IEnumerable<String> headerValues = sourceTable.Columns
                .OfType<DataColumn>()
                .Select(column => QuoteValue(column.ColumnName));
                
            writer.WriteLine(String.Join(",", headerValues));
        }

        IEnumerable<String> items = null;

        foreach (DataRow row in sourceTable.Rows) 
        {
            items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
            writer.WriteLine(String.Join(",", items));
        }

            writer.Flush();
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }
    }
}
