using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Microsoft.Win32;

namespace PhotoForce.App_Code
{
    public class clsStatic
    {
        #region Initialization
        private string settingsXmlFile = "settings.xml";
        private string templateCoordinatesXmlFile = "TemplatesCoordinates\\Coordinates.xml";
        private string templatePDFFolder = "Templates";
        public static string APP_DATA_FOLDER_NAME = "PhotoSorter3";
        public static string exceptionLogXML = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + APP_DATA_FOLDER_NAME + "\\exceptionLog.xml";
        private static string errorLogXML = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + APP_DATA_FOLDER_NAME + "\\errorLog.xml";
        public static string userName = "";
        public static int userId ;

        #endregion

        #region Properties
        public string SettingsXMLFile
        {
            get { return settingsXmlFile; }
            set { settingsXmlFile = value; }
        }

        public string TemplateCoordinatesXmlFile
        {
            get { return templateCoordinatesXmlFile; }
            set { templateCoordinatesXmlFile = value; }
        }

        public string TemplatePDFFolder
        {
            get { return templatePDFFolder; }
            set { templatePDFFolder = value; }
        }
        public string ExceptionLogXML
        {
            get { return exceptionLogXML; }
            set { exceptionLogXML = value; }
        }
        public string ErrorLogXML
        {
            get { return errorLogXML; }
            set { errorLogXML = value; }
        }
        #endregion


        #region Constructors
        public clsStatic()
        {
            getLogNameFromRegistry(exceptionLogXML);
        }
        #endregion

        #region Methods
        public static void clearTempXML()
        {
            if (File.Exists(errorLogXML))
            {
                //string fileName = errorLogXML;
                XDocument xmlSetting = XDocument.Load(errorLogXML);
                //xmlSetting.RemoveNodes();
                var pdfAuthPath = xmlSetting.Descendants("error");
                pdfAuthPath.Remove();
                xmlSetting.Save(errorLogXML);
            }
        }

        public static void WriteExceptionLogXML(Exception ex)
        {
            try
            {
                string filename = exceptionLogXML;

                if (!File.Exists(filename))
                {
                    XmlDocument XD = new XmlDocument();
                    XmlNode Root = XD.AppendChild(XD.CreateElement("Data"));
                    XD.Save(exceptionLogXML);
                }
                //create new instance of XmlDocument
                XmlDocument doc = new XmlDocument();


                //load from file
                doc.Load(filename);

                //create node and add value
                XmlNode node = doc.CreateNode(XmlNodeType.Element, "exception", null);

                //create message node
                XmlNode nodeMessage = doc.CreateElement("message");
                //add value for it
                nodeMessage.InnerText = ex.Message;

                //create stackTrace node
                XmlNode nodeStackTrace = doc.CreateElement("stackTrace");
                nodeStackTrace.InnerText = ex.StackTrace;

                //create dateTime node
                XmlNode nodeDateTime = doc.CreateElement("dateTime");
                nodeDateTime.InnerText = DateTime.Now.ToString(); ;

                //add to parent node
                node.AppendChild(nodeMessage);
                node.AppendChild(nodeStackTrace);
                node.AppendChild(nodeDateTime);

                //add to elements collection
                doc.DocumentElement.AppendChild(node);

                //save back
                doc.Save(filename);
            }
            catch (Exception ex1)
            {
                
            }
            

        }

        public static void WriteErrorLog(clsErrorLog objClsErrorLog, string filename)
        {
            filename = errorLogXML;
            if (!File.Exists(filename))
            {
                XmlDocument XD = new XmlDocument();
                XmlNode Root = XD.AppendChild(XD.CreateElement("Data"));
                XD.Save(errorLogXML);
            }

            //create new instance of XmlDocument
            XmlDocument docTemp = new XmlDocument();


            //load from file
            docTemp.Load(filename);

            //create node and add value
            XmlNode node = docTemp.CreateNode(XmlNodeType.Element, "error", null);

            //XmlNode nodePermanentOwner = docPermanent.CreateNode(XmlNodeType.Element, "error", null);

            //create Source node
            XmlNode nodeSource = docTemp.CreateElement("Source");
            //add value for it
            nodeSource.InnerText = objClsErrorLog.Source;

            //create MethodName node
            XmlNode nodeMethodName = docTemp.CreateElement("MethodName");
            nodeMethodName.InnerText = objClsErrorLog.MethodName;

            //create Parameters node
            XmlNode nodeParameters = docTemp.CreateElement("Parameters");
            nodeParameters.InnerText = objClsErrorLog.Parameters;

            //create Message node
            XmlNode nodeMessage = docTemp.CreateElement("Message");
            nodeMessage.InnerText = objClsErrorLog.Message;

            //create UserComments node
            XmlNode nodeUserComments = docTemp.CreateElement("UserComments");
            nodeUserComments.InnerText = objClsErrorLog.UserComments;


            //create dateTime node
            XmlNode nodeDateTime = docTemp.CreateElement("dateTime");
            nodeDateTime.InnerText = DateTime.Now.ToString(); ;

            //add to parent node
            node.AppendChild(nodeSource);
            node.AppendChild(nodeMethodName);
            node.AppendChild(nodeParameters);
            node.AppendChild(nodeMessage);
            node.AppendChild(nodeUserComments);
            node.AppendChild(nodeDateTime);

            //add to elements collection
            docTemp.DocumentElement.AppendChild(node);

            //XmlNode nodePermanent = docPermanent.DocumentElement.OwnerDocument.ImportNode(node, true);


            //save back
            docTemp.Save(filename);
        }


        public static void WriteErrorLogRenameImages(clsErrorLog objClsErrorLog, string filename)
        {
            if (!File.Exists(errorLogXML))
            {
                XmlDocument XD = new XmlDocument();
                XmlNode Root = XD.AppendChild(XD.CreateElement("Data"));
                XD.Save(errorLogXML);
            }

            //create new instance of XmlDocument
            XmlDocument docTemp = new XmlDocument();


            //load from file
            docTemp.Load(errorLogXML);

            //create node and add value
            XmlNode node = docTemp.CreateNode(XmlNodeType.Element, "error", null);

            //XmlNode nodePermanentOwner = docPermanent.CreateNode(XmlNodeType.Element, "error", null);

            //create Source node
            XmlNode nodeSource = docTemp.CreateElement("Source");
            //add value for it
            nodeSource.InnerText = objClsErrorLog.Source;

            //create MethodName node
            XmlNode nodeMethodName = docTemp.CreateElement("MethodName");
            nodeMethodName.InnerText = objClsErrorLog.MethodName;

            //create Parameters node
            XmlNode nodeParameters = docTemp.CreateElement("Parameters");
            nodeParameters.InnerText = objClsErrorLog.Parameters;

            //create Message node
            XmlNode nodeMessage = docTemp.CreateElement("Message");
            nodeMessage.InnerText = objClsErrorLog.Message;

            //create UserComments node
            XmlNode nodeUserComments = docTemp.CreateElement("UserComments");
            nodeUserComments.InnerText = objClsErrorLog.UserComments;

            //create ImageName node
            XmlNode nodeImageName = docTemp.CreateElement("ImageName");
            nodeImageName.InnerText = objClsErrorLog.ImageName;

            //create ImagePath node
            XmlNode nodeImagePath = docTemp.CreateElement("ImagePath");
            nodeImagePath.InnerText = objClsErrorLog.ImagePath;

            //create dateTime node
            XmlNode nodeDateTime = docTemp.CreateElement("dateTime");
            nodeDateTime.InnerText = DateTime.Now.ToString();



            //add to parent node
            node.AppendChild(nodeSource);
            node.AppendChild(nodeMethodName);
            node.AppendChild(nodeParameters);
            node.AppendChild(nodeMessage);
            node.AppendChild(nodeUserComments);
            node.AppendChild(nodeDateTime);
            node.AppendChild(nodeImageName);
            node.AppendChild(nodeImagePath);

            //add to elements collection
            docTemp.DocumentElement.AppendChild(node);

            //XmlNode nodePermanent = docPermanent.DocumentElement.OwnerDocument.ImportNode(node, true);


            //save back
            docTemp.Save(errorLogXML);
        }

        public static void WriteErrorLogImportImages(clsErrorLog objClsErrorLog, string filename)
        {

            if (!File.Exists(errorLogXML))
            {
                XmlDocument XD = new XmlDocument();
                XmlNode Root = XD.AppendChild(XD.CreateElement("Data"));
                XD.Save(errorLogXML);
            }

            //create new instance of XmlDocument
            XmlDocument docTemp = new XmlDocument();


            //load from file
            docTemp.Load(errorLogXML);

            //create node and add value
            XmlNode node = docTemp.CreateNode(XmlNodeType.Element, "error", null);

            //XmlNode nodePermanentOwner = docPermanent.CreateNode(XmlNodeType.Element, "error", null);

            //create Source node
            XmlNode nodeSource = docTemp.CreateElement("Source");
            //add value for it
            nodeSource.InnerText = objClsErrorLog.Source;

            //create MethodName node
            XmlNode nodeMethodName = docTemp.CreateElement("MethodName");
            nodeMethodName.InnerText = objClsErrorLog.MethodName;

            ////create Parameters node
            //XmlNode nodeParameters = docTemp.CreateElement("Parameters");
            //nodeParameters.InnerText = objClsErrorLog.Parameters;

            //create Message node
            XmlNode nodeMessage = docTemp.CreateElement("Message");
            nodeMessage.InnerText = objClsErrorLog.Message;

            //create UserComments node
            XmlNode nodeUserComments = docTemp.CreateElement("UserComments");
            nodeUserComments.InnerText = objClsErrorLog.UserComments;


            //create dateTime node
            XmlNode nodeDateTime = docTemp.CreateElement("dateTime");
            nodeDateTime.InnerText = DateTime.Now.ToString(); ;

            //add to parent node
            node.AppendChild(nodeSource);
            node.AppendChild(nodeMethodName);
            //node.AppendChild(nodeParameters);
            node.AppendChild(nodeMessage);
            node.AppendChild(nodeUserComments);
            node.AppendChild(nodeDateTime);

            //add to elements collection
            docTemp.DocumentElement.AppendChild(node);

            //XmlNode nodePermanent = docPermanent.DocumentElement.OwnerDocument.ImportNode(node, true);


            //save back
            docTemp.Save(errorLogXML);
        }

        public static void updateRegistryLogName(string logFileName)
        {
            try
            {
                exceptionLogXML = logFileName;
                RegistryKey PhotoForce = Registry.CurrentUser.CreateSubKey(@"Software\Photo Sorter");
                using (RegistryKey LogDetails = PhotoForce.CreateSubKey("LogFile"))
                {
                    // Create data for the LogName subkey.
                    LogDetails.SetValue("LogFileName", logFileName, RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
        }

        public static void getLogNameFromRegistry(string LogFileName)
        {
            RegistryKey PhotoForce = Registry.CurrentUser.OpenSubKey(@"Software\Photo Sorter\LogFile");
            if (PhotoForce != null)
            {
                exceptionLogXML = PhotoForce.GetValue("LogFileName").ToString();
            }
            else
            {
                updateRegistryLogName(LogFileName);
            }
        }

        #endregion
    }

    public class clsErrorLog
    {
        public string Source { get; set; }

        public string MethodName { get; set; }

        public string Parameters { get; set; }

        public string Message { get; set; }

        public string UserComments { get; set; }

        public string ImageName { get; set; }

        public string ImagePath { get; set; }

        public DateTime DateTime { get; set; }

        #region  code added Abhilasha
        public static bool displaymsg = true;
        #endregion

    }

    public static class errorMessages
    {
        public static string NO_DB_PRESENT = "Error in Database Connection";
        public static string RE_IMPORT_STUDENTS = "This process will attempt to update the student records with the values of excel sheet based on ID.\n\n Continue ?";
        public static string RESYNC_IMAGE_NAMES = "Using the serial number of the image file in the Student Photos grid, this process will attempt to find the corresponding image file in the Photo Shoot folder. It will then update the image name, first name and last name according to file name on the hard disk.\n\n Continue ?";
        public static string RESYNC_IMAGE_NAMES_SUCCESS = "Resynched Successfully.";
        public static string FOLDER_NOT_EMPTY = "Folder is not empty. Please select some other folder.";
        public static string SCHOOL_PATH_DOES_NOT_EXIST = "School path does not exist.";
        public static string ALTERNATIVE_PATH_DOES_NOT_EXIST = "Path does not exist.";
        public static string EXCEPTION_MESSAGE = "Error While Saving the Data. Please check the Exception Log for more Details.";
        public static string COMMON_EXCEPTION_MESSAGE = "Error. Please check the Exception Log for more Details.";
        public static string FILL_ALL_VALUES = "Please fill name, school year and date.";
        public static string SCHOOL_NAME_ALREADY_EXISTS_DB = "There is already a school with this name. Please select Some other name.";
        public static string SCHOOL_NAME_ALREADY_EXISTS_FOLDER = "There is already a school folder with this name. \nAre you sure you want to use this folder?";
        public static string PHOTO_JOB_NAME_ALREADY_EXISTS_DB = "There is already a Season with this name. Please select Some other name.";
        public static string PHOTO_JOB_NAME_ALREADY_EXISTS_FOLDER = "There is already a job folder with this name. \nYou sure want to use this folder?";
        public static string PHOTO_SHOOT_NAME_ALREADY_EXISTS_DB = "There is already a Photography Shoot with this name. Please select Some other name.";
        public static string PHOTO_SHOOT_NAME_ALREADY_EXISTS_FOLDER = "There is already a job Shoot with this name. \nYou sure want to use this folder?";
        public static string GROUP_NAME_ALREADY_EXISTS_DB = "There is already a Group with this name. Please select some other name..";
        public static string SELECT_MASK = "Select a mask to continue..";
        public static string AFTER_COPY_OPEN_LOCATION_CONFIRMATION = "Images copied successfully. Do you want to open the image folder?";
        public static string AFTER_GENERATE_OPEN_LOCATION_CONFIRMATION_ERRORS = "PDFs generated with errors. Do you want to open the Exception Log?";
        public static string AFTER_GENERATE_OPEN_LOCATION_CONFIRMATION_ERRORS_NEW = "PDFs generated with errors. Click Yes to open PDF Folder and No to go back.";
        public static string AFTER_GENERATE_OPEN_LOCATION_CONFIRMATION_SUCCESSFULLY = "PDFs generated successfully. Do you want to open the PDF folder?";
        public static string BEFORE_GENERATEPDF_CONFIRMATION1 = "You are about to generate ";
        public static string BEFORE_GENERATEPDF_CONFIRMATION2 = " PDFs. Do you want to continue? ";
        public static string REMOVE_IMAGES_FROM_GROUP1 = "Are you sure you want to remove ";
        public static string REMOVE_IMAGES_FROM_GROUP2 = " photos from ";
        public static string REMOVE_IMAGES_FROM_ORDER = " order items from ";
        public static string FILL_SCHOOL_PATH = "Please choose a path for school to continue.";
        public static string BEFORE_IMPORT_STUDENT_IMAGES_CONFIRMATION1 = "You are about to import student images into ";
        public static string BEFORE_IMPORT_STUDENT_IMAGES_CONFIRMATION2 = ". Do you want to continue?";
        public static string FILE_ALREADY_USED_IN_IMPORT_FILE = "The selected file has already been imported. Please choose a file in a different location.";
        public static string GET_RATING_PHOTOJOBANDSHOOT_SELECTED1 = "You are going to get the rating of ";
        public static string GET_RATING_PHOTOJOBANDSHOOT_SELECTED2 = " images. Are you sure you want to continue?";
        public static string GET_RATING_PHOTOJOBANDSHOOT_SELECTED1_NEW = "Ratings of ";
        public static string GET_RATING_PHOTOJOBANDSHOOT_SELECTED2_NEW = " images will be read and stored in PhotoForce. This task may take some time and will be performed in the background so that you can continue working.";
        public static string GET_EXPORT_DATA1 = "Data of ";
        public static string GET_EXPORT_DATA2 = " Students has been exported successfully. Do you want to open the output folder? ";
        public static string CREATED_DATA_FILE = " Data file has been created successfully. Do you want to open the folder? ";
        public static string FIELDS_CANNOT_BE_EMPTY = "Photoshoot name and file path cannnot be empty.";
        public static string FILE_IMPORTED_SUCCESSFULLY = "Data has been imported successfully under ";
        public static string QR_IMAGES_IMPORTED_SUCCESSFULLY = "QR Images has been imported successfully under ";
        public static string FILE_IMPORTED_ERRORS1 = "Data has been imported with some errors under ";
        public static string FILE_IMPORTED_ERRORS2 = ". Do you want to open the exception log now?";
        public static string IMPORT_STUDENT_CANNOT_EMPTY = "Description and file path cannot be empty.";
        public static string BEFORE_IMPORT_STUDENT_DATA_CONFIRMATION1 = "You are about to import the student data list from ";
        public static string BEFORE_IMPORT_STUDENT_DATA_CONFIRMATION2 = " into the ";
        public static string BEFORE_IMPORT_STUDENT_DATA_CONFIRMATION3 = ". Do you want to continue?";
        public static string SCHOOL_PATH_DOES_NOT_EXISTS = "School path does not exists.";
        public static string AUTO_CREATE_GROUP_WITHOUT_TEACHER = " Rows found without a teacher. If you continue, all records without a teacher will be grouped together. Continue?";
        public static string STUDENT_DATA_FILE_EXIST1 = "StudentDatafile.txt already exists in  ";
        public static string STUDENT_DATA_FILE_EXIST2 = ". Do you want to overwrite?";
        public static string DELETION_WITH_ERRORS = "deletion completed with errors. Please check exception log for more details";
        public static string BEFORE_AUTO_GENERATE_GROUP1 = "You are about to create groups";
        public static string BEFORE_AUTO_GENERATE_GROUP2 = ". Do you want to continue?";
        public static string AFTER_CREATING_GROUP_CONFIRMATION1 = " groups created for selected photoshoots";
        public static string AFTER_CREATING_GROUP_CONFIRMATION2 = ". To find them, click the 'Groups' tab and look for the group name prefixed with the teachers name ";        //". Open the Group section, and look for records where the Group names are prefixed with teacher name ";
        public static string AFTER_CREATING_STUDENT_GROUP_CONFIRMATION2 = ". Open the Groups section, and look for groups where the group name starts with Validation. ";
        public static string AFTER_CREATING_GROUP_CONFIRMATION3 = " and followed by the season ";
        public static string AFTER_CREATING_GROUP_JOB_CONFIRMATION1 = " groups created for season ";
        public static string AFTER_CREATING_GROUP_JOB_CONFIRMATION2 = ". To find them, click the 'Groups' tab and look for the group name prefixed with the teachers name ";        //". Open the Group section, and look for records where the Group names are prefixed with teacher name ";
        public static string AFTER_CREATING_GROUP_JOB_CONFIRMATION3 = ", followed by the season ";
        public static string AFTER_CREATING_GROUP_ONE_CONFIRMATION1 = " groups were created for the selected photoshoot";
        public static string BEFORE_RENAME_SOURCE_IMAGES1 = "Images in photoshoot ";
        public static string BEFORE_RENAME_SOURCE_IMAGES2 = " will be renamed in the format lastname_firstname_serialno.jpg. Do you want to continue?";
        public static string BEFORE_RENAME_SOURCE_IMAGES1_SELECTED = "Selected student images ";
        public static string BEFORE_RENAME_SOURCE_IMAGES2_SELECTED = " will be renamed in the format lastname_firstname_serialno.jpg. Do you want to continue?";
        public static string AFTER_RENAME_SOURCE_IMAGES = "All images renamed successfully.";
        public static string AFTER_RENAME_SOURCE_IMAGES_SELECTED = "All selected images renamed successfully.";
        public static string AFTER_RENAME_SOURCE_IMAGES_ERROR = "Renaming of images Completed with errors. Do you want to see the error details?";
        public static string PATH_DOES_NOT_EXISTS = "Path does not exist.";
        public static string EXPORT_WITH_ERRORS = "Export completed with errors. Please check exception log for more details";
        public static string DELETE_PHOTOS_ERROR = "Some of the selected images could not be deleted. They will be deleted when the program restarts.";
        public static string DELETE_PHOTOS_SUCCESSFUL_MSG = "Images deleted successfully";
        public static string DELETE_ALL_PHOTOS = "Are you sure you want to delete all images?";
        public static string GENERATE_REDUCED_IMAGES = "Are you sure you want to generate reduced size images for the selected photoshoot?\nThis process will run in the background, so you can continue with your work while the program is generating the images.";
        public static string GENERATE_REDUCED_IMAGES_ALL = "Are you sure you want to generate all reduced size images? The process might take several minutes to complete.\nIt will run in the background, so you can continue with your work while the program is generating the images.";
        public static string CREATE_ITPC_DATA = "Are you sure you want to create itpc data for all the images in selected photoshoot?\nThis process will run in the background, so you can continue with your work while the program is creating itpc data for the images.";
        public static string CREATE_ITPC_DATA_GROUP_IMAGE = "Are you sure you want to write all the student image passwords to selected group image?";
        public static string VALIDATE_ADMIN_CD_SAME_SCHOOL = "Please select Seasons of same school.";
        public static string AFTER_DELETION_STUDENTS_CONFIRMATION_ERRORS = "Some students could not be deleted. Do you want to open the error Log?";
        public static string TWO_SCHOOL_SELECT_ERRORS = "Please select students from the same school.";
        public static string BEFORE_DELETION_STUDENT_IMAGE_CONFIRMATION = "Are you sure you want to delete selected student image?";
        public static string BEFORE_DELETION_STUDENT_IMAGES_CONFIRMATION = "Are you sure you want to delete selected student images?";
        public static string BEFORE_DEACTIVATE_STUDENT_CONFIRMATION1 = "You are about to deactivate ";
        public static string BEFORE_DEACTIVATE_STUDENT_CONFIRMATION2 = " student.\nContinue?";
        public static string BEFORE_DEACTIVATE_STUDENTS_CONFIRMATION1 = "You are about to deactivate ";
        public static string BEFORE_DEACTIVATE_STUDENTS_CONFIRMATION2 = " students.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_STUDENTS_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_STUDENTS_CONFIRMATION2 = " students.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_STUDENT_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_STUDENT_CONFIRMATION2 = " student.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_SCHOOLS_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_SCHOOLS_CONFIRMATION2 = " schools.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_SCHOOL_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_SCHOOL_CONFIRMATION2 = " school.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_PHOTOSHOOTS_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_PHOTOSHOOTS_CONFIRMATION2 = " photoshoots.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_PHOTOSHOOT_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_PHOTOSHOOT_CONFIRMATION2 = " photoshoot.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_GROUPS_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_GROUPS_CONFIRMATION2 = " groups.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_GROUP_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_GROUP_CONFIRMATION2 = " group.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_JOBS_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_JOBS_CONFIRMATION2 = " job Name.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_JOB_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_JOB_CONFIRMATION2 = " job Name.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_MASKS_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_MASKS_CONFIRMATION2 = " masks.\nContinue?";
        public static string BEFORE_RENAMING_SELECTED_MASK_CONFIRMATION1 = "You are about to rename the selected ";
        public static string BEFORE_RENAMING_SELECTED_MASK_CONFIRMATION2 = " mask.\nContinue?";
        public static string SELECT_PHOTOSHOOTS_FROM_ONE_SEASON = "Please select photoshoots from one school year only.";
        public static string UNABLE_TO_CONVERT_DATE1 = "Unable to convert date: ";
        public static string UNABLE_TO_CONVERT_DATE2 = ". It is not a valid date. It should be in MM/dd/yyyy format.";
        public static string UNABLE_TO_GET_PHOTO_RATING = "Unable to get the photo rating since reduced size images have not been generated yet.\n\nClose this dialog, generate reduced images, and try again.";
        public static string MESSAGE_FOR_IMPORT_FILE = "Selected record does not have any data file to export.";
        public static string MESSAGE_FOR_JOB_YEAR = "Unable to determine the school year from the season. Please select a school year.";
        public static string MESSAGE_FOR_JOB_YEAR_QR_IMPORT = "Unable to determine the school year from the season."; //removed school year combobox from Import QR
        public static string SELECT_SCHOOL_YEAR = "Please select school year.";
        public static string SCHOOL_ENDYEAR_LESSTHAN_STARTYEAR = "School end year should not less than start year";
        public static string SKIP_SCHOOL_YEAR = "Should not allow to skip school year(s), e.g. 2014 | 2016.";
        public static string SELECT_RECORDS_SAME_SEASON = "Please select records from the same season.";
        public static string RASSIGN_PHOTOSHOOT_SEASON = "Please select only one photoshoot.";
        public static string CHANGE_CONFIG_FILE = "Cannot connect to database,please check the connection string.";
        public static string EXECUTE_VERSION_LOWER_THAN_DB = "The version of the database is later than the installed version of the software. You should consider upgrading to avoid potential problems.\n\nContinue anyway?";
        public static string SELECT_STUDENT = "please select student(s)";
        public static string SELECT_BULK_RENAME_GROUP = "Please select group(s) to rename.";
        
        //ADD NEW CONNECTION
        public static string ALL_FILEDS_MANDATORY = "All fields are mandatory.";
        public static string TEST_CONNECTION_SUCCESFUL = "Test connection succesful.";
        public static string TEST_CONNECTION_FAIL = "Unable to connect to the database with supplied connection settings. Please review and try again.";
        //end

        //DASHBOARD
        public static string PATH_NO_IMAGES = "Path doesn't contain any images.";
        public static string GENERATED_IPTC_ALL_IMAGES = "Generated  IPTC data for all images.";
        public static string REDUCE_IMAGES_SUCESSFULL = "Reduced images created successfully.";
        //END

        //EXPORT
        public static string SOURCE_FOLDER_NO_IMAGES = "Source folder doesn't contain images.";
        public static string SOURCE_FOLDER_DOESNOT_EXISTS = "Source folder doesn't exists.";
        //END

        //MANAGE DB CONNECTIONS
        public static string FAILED_TO_CONNECT = "Failed to connect database.,please select another connection string.";
        //END

        //MANAGE GROUPS
        public static string SELECT_GROUP_IMAGE = "Please select group image.";
        //END

        //For new user
        public static string SELECT_DEFAULT_SCHOOL = "Please select default school first to continue.";

        #region  code added Abhilasha
        public static string RENAME_WITHOUT_REDUCED_IMAGES_ERROR = "Unable to rename source images because no reduced images were found. Please generate reduced images first.";
        public static string DATABASE_UPDATE_CONFIRMATION = "The database is about to be upgraded. Do you want to proceed?";
        public static string SELECT_STUDENT_IMAGE = "Please select student image(s).";
        public static string SELECT_SEASON_NAME = "Please enter season name.";
        #endregion

    }
}
