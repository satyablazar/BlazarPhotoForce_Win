using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ookii.Dialogs.Wpf;
using PhotoForce.App_Code;
using PhotoForce.Error_Management;
using PhotoForce.MVVM;
using PhotoForce.GroupManagement;
using System.ComponentModel;
using System.Windows;

namespace PhotoForce.WorkPlace
{
    public class GeneratePDFViewModel : ViewModelBase
    {
        #region Initialization
        Boolean fileExist;
        int photographyJobId, schoolId, groupId, flagValue;
        string strSchoolName = "";
        ArrayList arrStudents;
        ArrayList arrGroupId;
        string ImageNameGrp = "";
        static int totalPackage = 0;
        DataTable getPackages;
        ReviewPricing _objReviewPricing;
        School _objSchool;
        string schoolPath = "";
        Group _objGroup;
        string groupName = "";
        PhotographyJob _objPhotographyJob;
        string jobName = "";
        string imageFolder = "";
        string pathNameToOpen = "";
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        string generatedFileURL = "";
        string currentDateTime = "";
        GetTemplateCoordinates _objGetTemplateCoordinates;
        ArrayList arrStd = new ArrayList();
        BackgroundWorker worker = new BackgroundWorker();
        bool isFiltred = false;
        WaitCursorViewModel _objWaitCursorViewModel;
        #endregion

        #region Properties
        private IEnumerable<Template> _templateData;
        private DateTime _deadLine;
        private string _fullPath;
        private string _alternateFullPath;
        private int _grpIdCount;
        private string _subFolderText;
        //private string _processingContent;
        private string _templateSelectValue;
        private string _lblStopContent;
        private bool _isSeperateFolderChecked;
        private bool _isExportChecked;
        private bool _isAllStudentsChecked;
        private Visibility _lblShowErrorLogVisibility;
        private Visibility _btnErrorLogVisibility;
        private bool _isAlternateFolder;
        private int _minValue;
        private int _maxValue;
        private int _currentProgress;
        private Visibility _isProgressBarVisible;
        bool _generateEnable;

        public bool generateEnable
        {
            get { return _generateEnable; }
            set { _generateEnable = value; NotifyPropertyChanged("generateEnable"); }
        }
        public int maxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; NotifyPropertyChanged("maxValue"); }
        }
        public int minValue
        {
            get { return _minValue; }
            set { _minValue = value; NotifyPropertyChanged("minValue"); }
        }
        public int currentProgress
        {
            get { return _currentProgress; }
            set { _currentProgress = value; NotifyPropertyChanged("currentProgress"); }
        }
        public Visibility isProgressBarVisible
        {
            get { return _isProgressBarVisible; }
            set { _isProgressBarVisible = value; NotifyPropertyChanged("isProgressBarVisible"); }
        }
        public bool isAlternateFolder
        {
            get { return _isAlternateFolder; }
            set { _isAlternateFolder = value; NotifyPropertyChanged("isAlternateFolder"); }
        }
        public System.Windows.Visibility btnErrorLogVisibility
        {
            get { return _btnErrorLogVisibility; }
            set { _btnErrorLogVisibility = value; NotifyPropertyChanged("btnErrorLogVisibility"); }
        }
        public System.Windows.Visibility lblShowErrorLogVisibility
        {
            get { return _lblShowErrorLogVisibility; }
            set { _lblShowErrorLogVisibility = value; NotifyPropertyChanged("lblShowErrorLogVisibility"); }
        }
        public bool isAllStudentsChecked
        {
            get { return _isAllStudentsChecked; }
            set { _isAllStudentsChecked = value; NotifyPropertyChanged("isAllStudentsChecked"); }
        }
        public bool isExportChecked
        {
            get { return _isExportChecked; }
            set { _isExportChecked = value; NotifyPropertyChanged("isExportChecked"); }
        }
        public bool isSeperateFolderChecked
        {
            get { return _isSeperateFolderChecked; }
            set { _isSeperateFolderChecked = value; NotifyPropertyChanged("isSeperateFolderChecked"); }
        }
        public string lblStopContent
        {
            get { return _lblStopContent; }
            set { _lblStopContent = value; NotifyPropertyChanged("lblStopContent"); }
        }
        public string templateSelectValue
        {
            get { return _templateSelectValue; }
            set { _templateSelectValue = value; NotifyPropertyChanged("templateSelectValue"); }
        }
        //public string processingContent
        //{
        //    get { return _processingContent; }
        //    set { _processingContent = value; NotifyPropertyChanged("processingContent"); }
        //}
        public string subFolderText
        {
            get { return _subFolderText; }
            set { _subFolderText = value; NotifyPropertyChanged("subFolderText"); }
        }
        public int grpIdCount
        {
            get { return _grpIdCount; }
            set { _grpIdCount = value; NotifyPropertyChanged("grpIdCount"); }
        }
        public string alternateFullPath
        {
            get { return _alternateFullPath; }
            set { _alternateFullPath = value; NotifyPropertyChanged("alternateFullPath"); }
        }
        public string fullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; NotifyPropertyChanged("fullPath"); }
        }
        public DateTime deadLine
        {
            get { return _deadLine; }
            set { _deadLine = value; NotifyPropertyChanged("deadLine"); }
        }
        public IEnumerable<Template> templateData
        {
            get { return _templateData; }
            set { _templateData = value; NotifyPropertyChanged("templateData"); }
        }
        #endregion

        #region Constructors
        public GeneratePDFViewModel(int schlId, int jobId, ArrayList tempStd, ArrayList grpId, bool tempFiltred)
        {
            isFiltred = tempFiltred;
            generateEnable = true;
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            _objSchool = clsDashBoard.getSchoolName(db, schlId);
            _objPhotographyJob = clsDashBoard.getJobName(db, jobId);

            photographyJobId = jobId;
            arrStudents = tempStd;
            arrStd = tempStd;
            schoolId = schlId;
            arrGroupId = grpId;
            grpIdCount = arrGroupId.Count;
            schoolPath = _objSchool.folderPath;
            jobName = _objPhotographyJob.JobName;
            isAllStudentsChecked = true;
            lblShowErrorLogVisibility = System.Windows.Visibility.Collapsed; btnErrorLogVisibility = System.Windows.Visibility.Collapsed;

            minValue = 0; maxValue = 10; currentProgress = 0; isProgressBarVisible = Visibility.Collapsed;
            loadData();

            if (grpIdCount == 1)
            {
                isExportChecked = true;
                _objGroup = clsGroup.getGroupname(db, Convert.ToInt32(grpId[0]));
                groupName = _objGroup.GroupName;
                generatedFileURL = (clsDashBoard.getSettingByName(db, "PdfOutputFolder").settingValue).ToString();
                fullPath = schoolPath + "\\" + groupName + "\\" + "__multipose" + "\\";
                subFolderText = DateTime.Now.ToString();
            }
            else
            {
                isAlternateFolder = true;
            }
        }
        #endregion

        #region Commands
        public RelayCommand ReviewPricingCommand
        {
            get
            {
                return new RelayCommand(reviewPricing);
            }
        }
        public RelayCommand OpenSchoolFolderCommand
        {
            get
            {
                return new RelayCommand(openSchoolFolder);
            }
        }
        public RelayCommand AlternateFolderCommand
        {
            get
            {
                return new RelayCommand(alternateFolder);
            }
        }
        public RelayCommand GeneratePDFCommand
        {
            get
            {
                return new RelayCommand(generatePDF);
            }
        }
        public RelayCommand ErrorLogCommand
        {
            get
            {
                return new RelayCommand(errorLog);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(WindowClose);
            }
        }
        #endregion

        #region Methods
        private void loadData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            templateData = clsDashBoard.getTemplates(db);
            deadLine = DateTime.Now;
            strSchoolName = clsDashBoard.getSchoolNameByID(db, schoolId);

            _objSchool = clsDashBoard.getSchoolName(db, schoolId);
            _objPhotographyJob = clsDashBoard.getJobName(db, photographyJobId);
        }
        private void CreatePDF(string studentId, string fName, string lName, string grade, string tempDeadline, string schoolName, string password, string teacherName, string folder, string strTemplate, ArrayList StudentImages, ref bool isError)
        {
            try
            {
                _objGetTemplateCoordinates = new GetTemplateCoordinates();
                currentDateTime = DateTime.Now.ToString("yyyyMMdd-HHmm");
                groupName = groupName.Replace("\\", "");
                groupName = clsDashBoard.SanitizeFileName(groupName);

                subFolderText = subFolderText.Replace("\\", "");
                subFolderText = clsDashBoard.SanitizeFileName(subFolderText);

                if (isExportChecked == true)
                    _objGetTemplateCoordinates.GeneratedFileURL = schoolPath + "\\" + groupName + "\\" + "_multipose" + "\\" + subFolderText + "\\";
                else
                    _objGetTemplateCoordinates.GeneratedFileURL = alternateFullPath + "\\";

                if (isExportChecked == true)
                {
                    if (!Directory.Exists(schoolPath + "\\" + groupName + "\\" + "_multipose" + "\\" + subFolderText + "\\"))
                    {
                        Directory.CreateDirectory(schoolPath + "\\" + groupName + "\\" + "_multipose" + "\\" + subFolderText + "\\").ToString();
                    }
                    pathNameToOpen = schoolPath + "\\" + groupName + "\\" + "_multipose" + "\\" + subFolderText;
                }
                else
                {
                    if (!Directory.Exists(alternateFullPath + "\\"))
                    {
                        Directory.CreateDirectory(alternateFullPath + "\\").ToString();
                    }
                    pathNameToOpen = alternateFullPath;
                }
                _objGetTemplateCoordinates.TemplateToSelect = strTemplate;

                _objGetTemplateCoordinates.StudentFirstName = fName;

                _objGetTemplateCoordinates.StudentId = studentId;

                _objGetTemplateCoordinates.StudentLastName = lName;

                _objGetTemplateCoordinates.StudentGrade = grade;

                _objGetTemplateCoordinates.StudentTeacher = teacherName;

                _objGetTemplateCoordinates.DeadlineDate = tempDeadline;

                _objGetTemplateCoordinates.DeadlineDate2 = tempDeadline;

                _objGetTemplateCoordinates.SchoolName = schoolName;

                _objGetTemplateCoordinates.Password = password;

                _objGetTemplateCoordinates.Password2 = password;


                string stdImageFolder = imageFolder + "\\_reduced";
                if (Directory.Exists(stdImageFolder))
                {
                    int ImageCount = 0;
                    if (StudentImages.Count > 0)
                    {

                        for (int i = 0; i < StudentImages.Count; i++, ImageCount++)
                        {
                            if (ImageCount == StudentImages.Count)
                                ImageCount = 0;
                            if (i == 0)
                            {
                                _objGetTemplateCoordinates.StudentImage1_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum0 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum0 = imagesequentialnum0.Substring(imagesequentialnum0.LastIndexOf('_') + 1);
                                string[] imgext0 = imagesequentialnum0.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName1 = imgext0[0];
                            }
                            else if (i == 1)
                            {
                                _objGetTemplateCoordinates.StudentImage2_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum1 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum1 = imagesequentialnum1.Substring(imagesequentialnum1.LastIndexOf('_') + 1);
                                string[] imgext1 = imagesequentialnum1.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName2 = imgext1[0];
                            }
                            else if (i == 2)
                            {
                                _objGetTemplateCoordinates.StudentImage3_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum2 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum2 = imagesequentialnum2.Substring(imagesequentialnum2.LastIndexOf('_') + 1);
                                string[] imgext2 = imagesequentialnum2.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName3 = imgext2[0];
                            }
                            else if (i == 3)
                            {
                                _objGetTemplateCoordinates.StudentImage4_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum3 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum3 = imagesequentialnum3.Substring(imagesequentialnum3.LastIndexOf('_') + 1);
                                string[] imgext3 = imagesequentialnum3.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName4 = imgext3[0];
                            }
                            else if (i == 4)
                            {
                                _objGetTemplateCoordinates.StudentImage5_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum4 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum4 = imagesequentialnum4.Substring(imagesequentialnum4.LastIndexOf('_') + 1);
                                string[] imgext4 = imagesequentialnum4.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName5 = imgext4[0];
                            }
                            else if (i == 5)
                            {
                                _objGetTemplateCoordinates.StudentImage6_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum5 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum5 = imagesequentialnum5.Substring(imagesequentialnum5.LastIndexOf('_') + 1);
                                string[] imgext5 = imagesequentialnum5.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName6 = imgext5[0];
                            }
                            else if (i == 6)
                            {
                                _objGetTemplateCoordinates.StudentImage7_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum6 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum6 = imagesequentialnum6.Substring(imagesequentialnum6.LastIndexOf('_') + 1);
                                string[] imgext6 = imagesequentialnum6.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName7 = imgext6[0];
                            }
                            else if (i == 7)
                            {
                                _objGetTemplateCoordinates.StudentImage8_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum7 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum7 = imagesequentialnum7.Substring(imagesequentialnum7.LastIndexOf('_') + 1);
                                string[] imgext7 = imagesequentialnum7.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName8 = imgext7[0];
                            }
                            else if (i == 8)
                            {
                                _objGetTemplateCoordinates.StudentImage9_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum8 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum8 = imagesequentialnum8.Substring(imagesequentialnum8.LastIndexOf('_') + 1);
                                string[] imgext8 = imagesequentialnum8.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName9 = imgext8[0];
                            }
                            else if (i == 9)
                            {
                                _objGetTemplateCoordinates.StudentImage10_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum9 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum9 = imagesequentialnum9.Substring(imagesequentialnum9.LastIndexOf('_') + 1);
                                string[] imgext9 = imagesequentialnum9.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName10 = imgext9[0];
                            }
                            else if (i == 10)
                            {
                                _objGetTemplateCoordinates.StudentImage11_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum10 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum10 = imagesequentialnum10.Substring(imagesequentialnum10.LastIndexOf('_') + 1);
                                string[] imgext10 = imagesequentialnum10.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName11 = imgext10[0];
                            }
                            else if (i == 11)
                            {
                                _objGetTemplateCoordinates.StudentImage12_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum11 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum11 = imagesequentialnum11.Substring(imagesequentialnum11.LastIndexOf('_') + 1);
                                string[] imgext11 = imagesequentialnum11.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName12 = imgext11[0];
                            }
                            else if (i == 12)
                            {
                                _objGetTemplateCoordinates.StudentImage13_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum12 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum12 = imagesequentialnum12.Substring(imagesequentialnum12.LastIndexOf('_') + 1);
                                string[] imgext12 = imagesequentialnum12.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName13 = imgext12[0];
                            }
                            else if (i == 13)
                            {
                                _objGetTemplateCoordinates.StudentImage14_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum13 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum13 = imagesequentialnum13.Substring(imagesequentialnum13.LastIndexOf('_') + 1);
                                string[] imgext13 = imagesequentialnum13.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName14 = imgext13[0];
                            }
                            else if (i == 14)
                            {
                                _objGetTemplateCoordinates.StudentImage15_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum14 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum14 = imagesequentialnum14.Substring(imagesequentialnum14.LastIndexOf('_') + 1);
                                string[] imgext14 = imagesequentialnum14.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName15 = imgext14[0];
                            }
                            else if (i == 15)
                            {
                                _objGetTemplateCoordinates.StudentImage16_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum15 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum15 = imagesequentialnum15.Substring(imagesequentialnum15.LastIndexOf('_') + 1);
                                string[] imgext15 = imagesequentialnum15.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName16 = imgext15[0];
                            }
                            else if (i == 16)
                            {
                                _objGetTemplateCoordinates.StudentImage17_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum16 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum16 = imagesequentialnum16.Substring(imagesequentialnum16.LastIndexOf('_') + 1);
                                string[] imgext16 = imagesequentialnum16.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName17 = imgext16[0];
                            }
                            else if (i == 17)
                            {
                                _objGetTemplateCoordinates.StudentImage18_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum17 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum17 = imagesequentialnum17.Substring(imagesequentialnum17.LastIndexOf('_') + 1);
                                string[] imgext17 = imagesequentialnum17.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName18 = imgext17[0];
                            }
                            else if (i == 18)
                            {
                                _objGetTemplateCoordinates.StudentImage19_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum18 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum18 = imagesequentialnum18.Substring(imagesequentialnum18.LastIndexOf('_') + 1);
                                string[] imgext18 = imagesequentialnum18.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName19 = imgext18[0];
                            }
                            else if (i == 19)
                            {
                                _objGetTemplateCoordinates.StudentImage20_URL = StudentImages[ImageCount].ToString();
                                string imagesequentialnum19 = StudentImages[ImageCount].ToString().Substring(StudentImages[ImageCount].ToString().LastIndexOf('\\') + 1);
                                imagesequentialnum19 = imagesequentialnum19.Substring(imagesequentialnum19.LastIndexOf('_') + 1);
                                string[] imgext19 = imagesequentialnum19.Split(new string[] { "." }, StringSplitOptions.None);
                                _objGetTemplateCoordinates.StudentImageName20 = imgext19[0];
                            }

                        }
                    }
                    else
                    {
                        isError = true;
                        clsErrorLog objError = new clsErrorLog();

                        objError.Source = "Student";
                        objError.MethodName = "CreatePDF";
                        objError.Message = "No Picture for the Student";
                        objError.Parameters = _objGetTemplateCoordinates.StudentFirstName + " " + _objGetTemplateCoordinates.StudentLastName + "," + _objGetTemplateCoordinates.StudentGrade;
                        clsStatic.WriteErrorLog(objError, "ErrorLog");

                    }
                }
                else
                {
                    isError = true;

                    clsErrorLog objError = new clsErrorLog();
                    objError.Source = "Student";
                    objError.MethodName = "CreatePDF";
                    objError.Message = "Student Images does not exist";
                    objError.Parameters = _objGetTemplateCoordinates.StudentFirstName + " " + _objGetTemplateCoordinates.StudentLastName + "," + _objGetTemplateCoordinates.StudentGrade;
                    clsStatic.WriteErrorLog(objError, "ErrorLog");
                }

                getPackages = new DataTable();
                PhotoSorterDBModelDataContext dbb = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                getPackages = clsDashBoard.getPackagesBySchoolIdDataTable(dbb, schoolId);
                totalPackage = getPackages.Rows.Count;
                string packageName = "";

                for (int i = 0; i < totalPackage; i++)
                {

                    packageName = Convert.ToString(getPackages.Rows[i]["item"]);

                    if (packageName.ToUpper() == "A")
                        _objGetTemplateCoordinates.Package_A_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "B")
                        _objGetTemplateCoordinates.Package_B_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "C")
                        _objGetTemplateCoordinates.Package_C_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "D")
                        _objGetTemplateCoordinates.Package_D_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "E")
                        _objGetTemplateCoordinates.Package_E_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "F")
                        _objGetTemplateCoordinates.Package_F_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "G")
                        _objGetTemplateCoordinates.Package_G_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "H")
                        _objGetTemplateCoordinates.Package_H_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "I")
                        _objGetTemplateCoordinates.Package_I_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "J")
                        _objGetTemplateCoordinates.Package_J_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "K")
                        _objGetTemplateCoordinates.Package_K_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "L")
                        _objGetTemplateCoordinates.Package_L_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "P")
                        _objGetTemplateCoordinates.Package_P_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "Q")
                        _objGetTemplateCoordinates.Package_Q_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "R")
                        _objGetTemplateCoordinates.Package_R_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "S")
                        _objGetTemplateCoordinates.Package_S_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "T")
                        _objGetTemplateCoordinates.Package_T_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "U")
                        _objGetTemplateCoordinates.Package_U_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "V")
                        _objGetTemplateCoordinates.Package_V_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "W")
                        _objGetTemplateCoordinates.Package_W_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "X")
                        _objGetTemplateCoordinates.Package_X_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "Y")
                        _objGetTemplateCoordinates.Package_Y_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "Z")
                        _objGetTemplateCoordinates.Package_Z_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "DD")
                        _objGetTemplateCoordinates.Package_DD_Price = "$" + getPackages.Rows[i]["price"].ToString();
                    else if (packageName.ToUpper() == "DDR")
                        _objGetTemplateCoordinates.Package_DDR_Price = "$" + getPackages.Rows[i]["price"].ToString();
                }
                // Getpackages.Clear();


                string fileName = clsDashBoard.getTemplateLocationById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(templateSelectValue));
                string filename2 = clsDashBoard.getTemplateLocationByName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), "Template A1");
                if (File.Exists(fileName))
                {
                    if (isSeperateFolderChecked == true)
                    {
                        _objGetTemplateCoordinates.handelAllActivities(fileName, "1", flagValue, true);
                        if (File.Exists(filename2))
                            _objGetTemplateCoordinates.handelAllActivities(filename2, "2", flagValue, true);
                    }
                    else
                    {
                        _objGetTemplateCoordinates.handelAllActivities(fileName, "1", flagValue, false);
                        if (File.Exists(filename2))
                            _objGetTemplateCoordinates.handelAllActivities(filename2, "2", flagValue, false);
                    }
                }
                else
                {
                    //processingContent = "Template does not exists";
                    isError = true;
                }
            }
            catch (Exception ex)
            {
                _objGetTemplateCoordinates.ErrorExist = true;
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        private void reviewPricing()
        {
            _objReviewPricing = new ReviewPricing(schoolId);
            _objReviewPricing.ShowDialog();
        }
        private void openSchoolFolder()
        {
            var regexItem = new System.Text.RegularExpressions.Regex(@"(?<=\ |^)[a-zA-Z0-9_-]+\.?(?=\ |$)");
            if (!regexItem.IsMatch(groupName))
            {
                groupName = System.Text.RegularExpressions.Regex.Replace(groupName, @"(?<=\ |^)[a-zA-Z0-9_-]+\.?(?=\ |$)", " ");
            }
            if (Directory.Exists(schoolPath))
            {
                if (fileExist == true)
                {
                    pathNameToOpen = schoolPath + "\\" + groupName + "\\" + "_multipose" + "\\" + subFolderText;
                    Process.Start(pathNameToOpen);
                }
                else
                {
                    pathNameToOpen = schoolPath + "\\" + groupName + "\\" + "_multipose";
                    Process.Start(pathNameToOpen);
                }
            }
            else
                MVVMMessageService.ShowMessage(errorMessages.SCHOOL_PATH_DOES_NOT_EXIST);
        }
        private void alternateFolder()
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            var res = dlg.ShowDialog();
            if (res != false)
                alternateFullPath = dlg.SelectedPath;
        }
        async void generatePDF()
        {
            try
            {
                Boolean FlagValueCheck = true;
                if (isExportChecked == true)
                {
                    if (!Directory.Exists(schoolPath))
                    {
                        FlagValueCheck = false;
                        MVVMMessageService.ShowMessage(errorMessages.SCHOOL_PATH_DOES_NOT_EXIST);
                    }
                }
                else
                {
                    if (!Directory.Exists(alternateFullPath))
                    {
                        FlagValueCheck = false;
                        MVVMMessageService.ShowMessage(errorMessages.ALTERNATIVE_PATH_DOES_NOT_EXIST);
                    }
                }

                string fileName = clsDashBoard.getTemplateLocationById(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(templateSelectValue));
                if (!File.Exists(fileName))
                {
                    FlagValueCheck = false;
                    MVVMMessageService.ShowMessage("Template does not exists., Please check the template settings.");
                    return;
                }

                if (FlagValueCheck == true)
                {
                    int pfdCount = 0;
                    if (isAllStudentsChecked && isFiltred == false)
                    {
                        arrStudents = clsGroup.getStudentIdStringByGroupId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrGroupId);
                        pfdCount = clsGroup.getStudentPdfCountByGroups(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), arrGroupId);
                    }
                    else
                    {
                        arrStudents = arrStd;
                        pfdCount = arrStudents.Count;
                    }

                    string message = errorMessages.BEFORE_GENERATEPDF_CONFIRMATION1 + pfdCount + errorMessages.BEFORE_GENERATEPDF_CONFIRMATION2;
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;

                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                    {
                        flagValue = arrStudents.Count;
                        Boolean ContainsFile = false;
                        if (string.IsNullOrEmpty(subFolderText))
                            subFolderText = DateTime.Now.ToString("yyyyMMdd-HHmm");
                        //If exist check for file contains..
                        if (isExportChecked == true)
                            ContainsFile = isDirectoryContainFiles(schoolPath + "\\" + groupName + "\\" + "_multipose" + "\\" + subFolderText);
                        else
                            ContainsFile = isDirectoryContainFiles(alternateFullPath);
                        if ((ContainsFile == false))
                        {
                            try
                            {
                                //processingContent = "please wait.........";
                                clsStatic.clearTempXML();
                                // The await operator suspends generatePDF.
                                //  - AccessTheWebAsync can't continue until EndProcessBar is complete.
                                //  - Meanwhile, control returns to the caller of uploadImages.
                                //  - Control resumes here when EndProcessBar is complete. 
                                //  - The await operator then retrieves the result from EndProcessBar if method has any return type.
                                await EndProcessBar();
                            }
                            catch (Exception ex)
                            {
                                clsStatic.WriteExceptionLogXML(ex);
                                MVVMMessageService.ShowMessage(ex.Message);
                            }
                        }
                        else
                        {
                            //MVVMMessageService.ShowMessage(errorMessages.FOLDER_NOT_EMPTY);
                            //call new window ,similar to message box and check wether user selected 'delete folders' option or not.
                            DeleteFoldersMessage _objDeleteFoldersMessage = new DeleteFoldersMessage();
                            _objDeleteFoldersMessage.ShowDialog();
                            if (((DeleteFoldersMessageViewModel)(_objDeleteFoldersMessage.DataContext)).isCancelled == false)
                            {
                                if (((DeleteFoldersMessageViewModel)(_objDeleteFoldersMessage.DataContext)).isDeleteFoldersSelected)
                                {
                                    System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(alternateFullPath);

                                    foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                                    {
                                        file.Delete();
                                    }
                                    foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                                    {
                                        dir.Delete(true);
                                    }

                                    clsStatic.clearTempXML();
                                    // The await operator suspends generatePDF.
                                    //  - AccessTheWebAsync can't continue until EndProcessBar is complete.
                                    //  - Meanwhile, control returns to the caller of uploadImages.
                                    //  - Control resumes here when EndProcessBar is complete. 
                                    //  - The await operator then retrieves the result from EndProcessBar if method has any return type.
                                    await EndProcessBar();
                                }
                                else
                                {
                                    clsStatic.clearTempXML();
                                    // The await operator suspends generatePDF.
                                    //  - AccessTheWebAsync can't continue until EndProcessBar is complete.
                                    //  - Meanwhile, control returns to the caller of uploadImages.
                                    //  - Control resumes here when EndProcessBar is complete. 
                                    //  - The await operator then retrieves the result from EndProcessBar if method has any return type.
                                    await EndProcessBar();
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }

        private void errorLog()
        {
            ShowErrors _objShowErrors = new ShowErrors();
            _objShowErrors.ShowDialog();

            DialogResult = false;
        }
        private void WindowClose()
        {
            DialogResult = false;
            if (_objWaitCursorViewModel != null)
                _objWaitCursorViewModel.Dispose();
        }
        public bool isDirectoryContainFiles(string path)
        {
            if (!Directory.Exists(path)) return false;
            return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Any();
        }
        /// <summary>
        /// The method has an async modifier. 
        /// The return type is Task or Task T. 
        /// Here, it is Task T because the return statement returns an integer.
        /// The method name ends in "Async."
        /// 
        /// You can avoid performance bottlenecks and enhance the overall responsiveness of your application by using asynchronous programming.
        /// However, traditional techniques for writing asynchronous applications can be complicated, making them difficult to write, debug, and maintain.
        /// This async method will execute ,if you want to return anything to called method
        /// declare a return type 
        ///
        /// This method will generate MPOF(PDF)
        /// </summary>
        /// <returns></returns>
        async Task EndProcessBar()
        {
            string message = "";
            try
            {
                //worker.DoWork += delegate(object s, DoWorkEventArgs args)
                //{
                _objWaitCursorViewModel = new WaitCursorViewModel();
                    db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                    int iRecCount = arrStudents.Count;
                    lblStopContent = "Esc to stop...";
                    string strTemplate = templateSelectValue;
                    bool isError = false;
                    List<StudentImage> objStudentImageGroup;
                    isProgressBarVisible = Visibility.Visible;
                    maxValue = iRecCount;
                    currentProgress++;
                    //if you want to return ,follow below code
                    //int processCount = await Task.Run<int>
                    await Task.Run(() =>
                        {
                            for (int i = 0; i < iRecCount; i++)
                            {
                                for (int k = 0; k < arrGroupId.Count; k++)
                                {
                                    groupId = Convert.ToInt32(arrGroupId[k]);
                                    _objGroup = clsGroup.getGroupname(db, groupId);
                                    groupName = _objGroup.GroupName;

                                    objStudentImageGroup = clsGroup.getSelectedStudentsByGroup(db, groupId, arrStudents[i].ToString());

                                    if (objStudentImageGroup.Count > 0)
                                    {
                                        string fname = objStudentImageGroup[0].Student.FirstName;

                                        //processingContent = "Processing... " + fname;
                                        string lname = objStudentImageGroup[0].Student.Lastname;
                                        string folder = "";
                                        string schoolName = strSchoolName;
                                        string deadline = deadLine.ToString();
                                        string grade = objStudentImageGroup[0].Student.Grade;
                                        string teacherName = objStudentImageGroup[0].Student.Teacher;
                                        string password = objStudentImageGroup[0].Student.Password;
                                        string deadline2 = deadLine.ToString();
                                        string imagename = objStudentImageGroup[0].ImageName;
                                        string studentId = objStudentImageGroup[0].ID.ToString();


                                        ArrayList StudentImages = new ArrayList();

                                        List<GroupClassPhoto> objGroupPhoto;
                                        objGroupPhoto = (List<GroupClassPhoto>)clsGroup.getGroupPhotoByGroupId(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), Convert.ToInt32(groupId));
                                        int grpcount = objGroupPhoto.Count;
                                        string GrpImageFolder = "";
                                        foreach (GroupClassPhoto objClsPht in objGroupPhoto)
                                        {
                                            ImageNameGrp = objClsPht.StudentImage.ImageName;
                                            GrpImageFolder = objClsPht.StudentImage.PhotoShoot.ImageFolder;
                                            StudentImages.Add(GrpImageFolder + "\\" + ImageNameGrp);
                                        }


                                        for (int j = 0; j < objStudentImageGroup.Count; j++)
                                        {
                                            imageFolder = Convert.ToString(objStudentImageGroup[j].PhotoShoot.ImageFolder);
                                            if (File.Exists(imageFolder + "\\_reduced\\" + objStudentImageGroup[j].ImageName))
                                                StudentImages.Add(imageFolder + "\\_reduced\\" + objStudentImageGroup[j].ImageName);
                                            if (StudentImages.Count == 20)
                                                break;
                                        }

                                        //here we have to generate the pdf for this particular record
                                        CreatePDF(studentId, fname, lname, grade, deadline, schoolName, password, teacherName, folder, strTemplate, StudentImages, ref isError);
                                        currentProgress++;
                                    }
                                }
                            }
                            return currentProgress;
                        });
                    if (isError)
                    {
                        //processingContent = "";
                        generateEnable = false;
                        lblShowErrorLogVisibility = Visibility.Visible;
                        btnErrorLogVisibility = Visibility.Visible;
                        isProgressBarVisible = Visibility.Collapsed;

                        message = errorMessages.AFTER_GENERATE_OPEN_LOCATION_CONFIRMATION_ERRORS_NEW;
                    }
                    else
                    {
                        lblShowErrorLogVisibility = System.Windows.Visibility.Collapsed;
                        btnErrorLogVisibility = System.Windows.Visibility.Collapsed;
                    }


                    if (_objGetTemplateCoordinates.ErrorExist == true)
                    {
                        message = errorMessages.AFTER_GENERATE_OPEN_LOCATION_CONFIRMATION_ERRORS;
                        clsStatic stat = new clsStatic();
                        pathNameToOpen = stat.ExceptionLogXML;

                    }
                    else if ((!_objGetTemplateCoordinates.ErrorExist) && !isError)
                    {
                        message = errorMessages.AFTER_GENERATE_OPEN_LOCATION_CONFIRMATION_SUCCESSFULLY;
                        DialogResult = false;
                    }

                    fileExist = true;
                    string caption = "Confirmation";
                    System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                    System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                    if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                        Process.Start(pathNameToOpen);
                    else
                    {
                        if (isError)
                            return;
                    }
                ///};
                //background worker support cancellation
                ///worker.WorkerSupportsCancellation = true;

                // Configure the function to run when completed
               /// worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                // Launch the worker
                ///worker.RunWorkerAsync();
                    generatePDFCompleted();
            }
            catch (Exception ex)
            {
                if (_objGetTemplateCoordinates.ErrorExist == true)
                {

                }
                clsStatic.WriteExceptionLogXML(ex);
                //processingContent = "";
                if (ex.Message == "ShowDialog can be called only on hidden windows.")
                {
                    DialogResult = false;
                }
                else
                {
                    MVVMMessageService.ShowMessage("Error Occured while Genarating PDF");
                }

            }
        }
        //void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    //isBottomButtonsEnabled = true;
        //}
        void generatePDFCompleted()
        {
            //isBottomButtonsEnabled = true;
            _objWaitCursorViewModel.Dispose();
        }
        #endregion
    }
}
