using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.Data;
using System.IO;
using PhotoForce.Helpers;

namespace PhotoForce.PhotographyJobManagement
{
    public class ValidateDataFileViewModel : ViewModelBase
    {
        #region Initialization
        DataTable dtDatafileData = new DataTable();
        string selectedPath = "";
        #endregion

        #region Properties
        DataTable _dgImagesInDataFileData;
        DataTable _dgFilesInFolderData;
        bool _isValidateGroupFile;
        bool _isValidateOrdersFile;
        string _txtPath;

        public string txtPath
        {
            get { return _txtPath; }
            set { _txtPath = value; NotifyPropertyChanged(); }
        }
        public bool isValidateOrdersFile
        {
            get { return _isValidateOrdersFile; }
            set
            {
                _isValidateOrdersFile = value; NotifyPropertyChanged();
                if (!string.IsNullOrEmpty(txtPath))
                {
                    txtPath = ""; dgImagesInDataFileData = null; dgFilesInFolderData = null;
                }
            }
        }
        public bool isValidateGroupFile
        {
            get { return _isValidateGroupFile; }
            set
            {
                _isValidateGroupFile = value; NotifyPropertyChanged();
                if (!string.IsNullOrEmpty(txtPath))
                {
                    txtPath = ""; dgImagesInDataFileData = null; dgFilesInFolderData = null;
                }
            }
        }

        public DataTable dgFilesInFolderData
        {
            get { return _dgFilesInFolderData; }
            set { _dgFilesInFolderData = value; NotifyPropertyChanged("dgFilesInFolderData"); }
        }

        public DataTable dgImagesInDataFileData
        {
            get { return _dgImagesInDataFileData; }
            set { _dgImagesInDataFileData = value; NotifyPropertyChanged("dgImagesInDataFileData"); }
        }
        #endregion

        #region Constructor
        public ValidateDataFileViewModel()//DataTable tempImageNamesInDataFile, DataTable tempImageNamesInFolder, string tempSelectedPath)
        {
            //selectedPath = tempSelectedPath;
            //BindGrid(tempImageNamesInDataFile, tempImageNamesInFolder);
        }
        #endregion

        # region Bind Grid
        private void BindGrid(DataTable tempImageNamesInDataFile, DataTable tempImageNamesInFolder)
        {
            dgImagesInDataFileData = tempImageNamesInDataFile;
            dgFilesInFolderData = tempImageNamesInFolder;
        }
        # endregion

        # region Commands
        public RelayCommand ValidateDataFileCommand
        {
            get
            {
                return new RelayCommand(validateDataFile);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return new RelayCommand(openFolder);
            }
        }
        # endregion

        #region Methods
        private void openFolder()
        {
            if (selectedPath != "")
            {
                System.Diagnostics.Process.Start(selectedPath);
            }
        }
        public void EditorialResponse(string word, string replacement, string fileName)
        {
            string saveFileName = fileName.ToLower().Replace(".txt", "_PF1.txt");
            StreamReader reader = new StreamReader(fileName);
            string input = reader.ReadToEnd();
            reader.Close();

            using (StreamWriter writer = new StreamWriter(saveFileName, true))
            {
                {
                    string output = input.Replace(word, replacement);
                    writer.Write(output);
                }
                writer.Close();
            }
            System.IO.File.Delete(fileName);
            System.IO.File.Move(saveFileName, fileName);
        }

        /// <summary>
        /// validate the datafile for required columns
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string checkForRequiredColumns(string filePath)
        {
            string[] reqColumns = new string[] { };
            string colName = ""; bool isHavingImageColumn = false;
            if (isValidateGroupFile)
            {
                reqColumns = new string[] { "Image Name", "First Name", "Last Name", "Student ID", "Teacher", "Grade",
                "Password", "School", "Location", "Packages", "Phone", "Custom1", "Custom2", "Custom3", "Custom4", "Custom5", "Custom6", "Custom7", "Custom8"};
            }
            else if (isValidateOrdersFile)
            {
                reqColumns = new string[] { "Image Name", "First Name", "Last Name", "Student id", "Teacher", "Grade",
                "activity", "packages", "retouching","Custom1", "Ticket Code",  "Group Image Name",
                "Group Image Path", "Order Number", "Ship To First Name", "Ship To Last Name", "Address", "City",
                "State","Zip","Country","Email","Vendor Order Id","Vendor Date"};
            }
            //checking wether any column is missed.
            DataRow dr = dtDatafileData.Rows[0];

            foreach (DataColumn col in dtDatafileData.Columns)
            {
                if (col.ColumnName == "Image")
                {
                    EditorialResponse("Image\t", "Image Name\t", filePath);
                    isHavingImageColumn = true;
                    break;
                }
            }

            if (isHavingImageColumn) { dtDatafileData = DataLoader.ReadTextFile(filePath); dr = dtDatafileData.Rows[0]; } //image details from selected datafile .}

            try
            {
                foreach (string col in reqColumns)
                {
                    colName = col;
                    dr[colName].ToString();
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return "Cannot find required column " + colName + Environment.NewLine + "Please check the data file.";
            }
            return "";
        }
        private void validateDataFile()
        {
            if (isValidateGroupFile == false && isValidateOrdersFile == false) { MVVMMessageService.ShowMessage("Please select any option to proceed."); return; }
            //browse for a file
            //you will get path
            //compare image names of data file with images in folder
            //print the un-matched one's
            string selectedDataFilePath = "";
            Ookii.Dialogs.Wpf.VistaOpenFileDialog dlg = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dlg.Filter = "txt files (*.txt)|*.txt";
            var res = dlg.ShowDialog();
            if (res == false)
            {
                return;
            }
            if (File.Exists(dlg.FileName))
            {
                txtPath = dlg.FileName;
            }
            else
            {
                MVVMMessageService.ShowMessage("Path does not exist.");
                return;
            }

            try
            {
                DataTable dtFolderImageNames = new DataTable();
                dtFolderImageNames.Columns.Add("Image Name", typeof(string));
                DataTable dtDataFileImageNames = new DataTable();
                dtDataFileImageNames.Columns.Add("Image Name", typeof(string));

                selectedDataFilePath = txtPath;
                selectedPath = System.IO.Path.GetDirectoryName(selectedDataFilePath);
                string[] fodlerImageNamesWithPath = Directory.GetFiles(selectedPath, "*.jpg", SearchOption.TopDirectoryOnly); //all the image names in selected folder ends with .JPG
                dtDatafileData = DataLoader.ReadTextFile(selectedDataFilePath); //image details from selected datafile .
                if (dtDatafileData == null || dtDatafileData.Rows.Count == 0) { MVVMMessageService.ShowMessage("Datafile is empty."); return; }
                if (dtDatafileData != null)
                {
                    //validate here
                    string tempMessage = checkForRequiredColumns(selectedDataFilePath);
                    if (!string.IsNullOrEmpty(tempMessage)) { MVVMMessageService.ShowMessage(tempMessage); return; }
                }

                string[] dataFileImageNames = new string[dtDatafileData.Rows.Count];
                string[] folderImageNames = new string[fodlerImageNamesWithPath.Count()];
                string[] tempFolderImageNames = new string[fodlerImageNamesWithPath.Count()];   //to avoid case-sensitive while looping.

                int k = 0;
                foreach (string imageName in fodlerImageNamesWithPath)
                {
                    string tempFolderImageName = imageName.Substring(imageName.LastIndexOf('\\') + 1);
                    folderImageNames[k] = tempFolderImageName.ToLower();
                    tempFolderImageNames[k] = tempFolderImageName;
                    k++;
                }

                for (int i = 0; i < dtDatafileData.Rows.Count; i++)
                {
                    string tempImageName = dtDatafileData.Rows[i].Field<string>("Image Name");
                    dataFileImageNames[i] = tempImageName.ToLower();
                    if (!folderImageNames.Contains(tempImageName.ToLower()))
                    {
                        dtDataFileImageNames.Rows.Add(tempImageName);
                    }
                }

                foreach (string imageName in tempFolderImageNames)
                {
                    if (!dataFileImageNames.Contains(imageName.ToLower()))
                    {
                        dtFolderImageNames.Rows.Add(imageName);
                    }
                }
                DataView dt = new DataView(dtDataFileImageNames);
                DataTable distinctTable = dt.ToTable(true, "Image Name");

                dt = new DataView(dtFolderImageNames);
                DataTable distinctImages = dt.ToTable(true, "Image Name");

                BindGrid(distinctTable, distinctImages);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void windowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
