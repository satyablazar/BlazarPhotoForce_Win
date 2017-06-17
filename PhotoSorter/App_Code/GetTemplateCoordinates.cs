using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;
using System.Drawing.Imaging;

namespace PhotoForce.App_Code
{
    public class GetTemplateCoordinates
    {
        #region Global Variables

        PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        private static String appPath = System.AppDomain.CurrentDomain.BaseDirectory;
        //Declaring Variables required for PDF Creation

        private Boolean isCheckedVal;
        private PdfReader reader;
        private Stream fs;
        private Stream baseFilepdfStream;

        private PdfContentByte pdfContentByte;
        private PdfStamper stamper;
        private static string generatedFileNameToCombine = "";
        private static int iFlag = 0;

        //Declaring Variables setting final values to be printed in PDF

        private string studentFullName = null;
        private string studentFullNameForFile = null;
        //private iTextSharp.text.Image studentGroupImage_Source;
        private iTextSharp.text.Image studentImage1_Source;
        private iTextSharp.text.Image studentImage2_Source;
        private iTextSharp.text.Image studentImage3_Source;
        private iTextSharp.text.Image studentImage4_Source;
        private iTextSharp.text.Image studentImage5_Source;
        private iTextSharp.text.Image studentImage6_Source;
        private iTextSharp.text.Image studentImage7_Source;
        private iTextSharp.text.Image studentImage8_Source;
        private iTextSharp.text.Image studentImage9_Source;
        private iTextSharp.text.Image studentImage10_Source;
        private iTextSharp.text.Image studentImage11_Source;
        private iTextSharp.text.Image studentImage12_Source;
        private iTextSharp.text.Image studentImage13_Source;
        private iTextSharp.text.Image studentImage14_Source;
        private iTextSharp.text.Image studentImage15_Source;
        private iTextSharp.text.Image studentImage16_Source;
        private iTextSharp.text.Image studentImage17_Source;
        private iTextSharp.text.Image studentImage18_Source;
        private iTextSharp.text.Image studentImage19_Source;
        private iTextSharp.text.Image studentImage20_Source;

        // Declaring Variables for Getting the Master PDF's and Setting the Final generated PDF
        //private string baseFileURL = null;

        private static Int32 newFileSuffix = 1;
        private static Int32 mergePdfFileSuffix = 001;

        private string extensionOfFile = ".pdf";

        private string baseFile;
        private string generatedFile;
        private string mergedGeneratedFile;


        #endregion

        #region Properties

        //Values of Decisions To Be Made at Run Time

        public string GeneratedFileURL { get; set; }
        public string TemplateToSelect { get; set; }

        //Values Of Data To Be Printed
        public Boolean ErrorExist { get; set; }

        public string StudentFirstName { get; set; }

        public string StudentId { get; set; } //Added By Mohan

        public string StudentLastName { get; set; }

        public string Password { get; set; }

        public string Password2 { get; set; }

        public string StudentGrade { get; set; }

        public string StudentTeacher { get; set; }

        public string DeadlineDate { get; set; }

        public string DeadlineDate2 { get; set; }

        public string SchoolName { get; set; }

        public string OrderFormCaption { get; set; }


        public string StudentImage1_URL { get; set; }
        public string StudentImage2_URL { get; set; }
        public string StudentImage3_URL { get; set; }
        public string StudentImage4_URL { get; set; }
        public string StudentImage5_URL { get; set; }
        public string StudentImage6_URL { get; set; }
        public string StudentImage7_URL { get; set; }
        public string StudentImage8_URL { get; set; }
        public string StudentImage9_URL { get; set; }
        public string StudentImage10_URL { get; set; }
        public string StudentImage11_URL { get; set; }
        public string StudentImage12_URL { get; set; }
        public string StudentImage13_URL { get; set; }
        public string StudentImage14_URL { get; set; }
        public string StudentImage15_URL { get; set; }
        public string StudentImage16_URL { get; set; }
        public string StudentImage17_URL { get; set; }
        public string StudentImage18_URL { get; set; }
        public string StudentImage19_URL { get; set; }
        public string StudentImage20_URL { get; set; }


        public string StudentImageName1 { get; set; }
        public string StudentImageName2 { get; set; }
        public string StudentImageName3 { get; set; }
        public string StudentImageName4 { get; set; }
        public string StudentImageName5 { get; set; }
        public string StudentImageName6 { get; set; }
        public string StudentImageName7 { get; set; }
        public string StudentImageName8 { get; set; }
        public string StudentImageName9 { get; set; }
        public string StudentImageName10 { get; set; }
        public string StudentImageName11 { get; set; }
        public string StudentImageName12 { get; set; }
        public string StudentImageName13 { get; set; }
        public string StudentImageName14 { get; set; }
        public string StudentImageName15 { get; set; }
        public string StudentImageName16 { get; set; }
        public string StudentImageName17 { get; set; }
        public string StudentImageName18 { get; set; }
        public string StudentImageName19 { get; set; }
        public string StudentImageName20 { get; set; }

        public string Package_A_Price { get; set; }
        public string Package_B_Price { get; set; }
        public string Package_C_Price { get; set; }
        public string Package_D_Price { get; set; }
        public string Package_E_Price { get; set; }
        public string Package_F_Price { get; set; }
        public string Package_G_Price { get; set; }
        public string Package_H_Price { get; set; }
        public string Package_I_Price { get; set; }
        public string Package_J_Price { get; set; }
        public string Package_K_Price { get; set; }
        public string Package_L_Price { get; set; }

        public string Package_P_Price { get; set; }
        public string Package_Q_Price { get; set; }
        public string Package_R_Price { get; set; }
        public string Package_S_Price { get; set; }
        public string Package_T_Price { get; set; }
        public string Package_U_Price { get; set; }
        public string Package_V_Price { get; set; }
        public string Package_W_Price { get; set; }
        public string Package_X_Price { get; set; }
        public string Package_Y_Price { get; set; }
        public string Package_Z_Price { get; set; }
        public string Package_DD_Price { get; set; }
        public string Package_DDR_Price { get; set; }

        // Coordinates of Data to be Printed

        Int32 frstnameCreditCard_x;
        Int32 frstnameCreditCard_y;
        Int32 lastnameCreditCard_x;
        Int32 lastnameCreditCard_y;

        Int32 studentpassword_x;
        Int32 studentpassword_y;

        Int32 studentpassword2_x;
        Int32 studentpassword2_y;

        Int32 frstname_x;
        Int32 frstname_y;

        Int32 studentId_x;  //Added By Mohan
        Int32 studentId_y;  //Added By Mohan

        Int32 lastname_x;
        Int32 lastname_y;

        Int32 gradex;
        Int32 gradey;

        Int32 schoolnameCreditCard_x;
        Int32 schoolnameCreditCard_y;

        Int32 teachername_x;
        Int32 teachername_y;


        Int32 deadline_x;
        Int32 deadline_y;
        DateTime deadline;
        String DeadlinetoPrint;

        Int32 deadline2_x;
        Int32 deadline2_y;
        DateTime deadline2;
        String DeadlinetoPrint2;

        Int32 image1_x;
        Int32 image1_y;
        Int32 image1_height;
        Int32 image1_width;
        Int32 image1_x_name;
        Int32 image1_y_name;

        Int32 image2_x;
        Int32 image2_y;
        Int32 image2_height;
        Int32 image2_width;
        Int32 image2_x_name;
        Int32 image2_y_name;

        Int32 image3_x;
        Int32 image3_y;
        Int32 image3_height;
        Int32 image3_width;
        //Int32 image3_name;
        Int32 image3_x_name;
        Int32 image3_y_name;

        Int32 image4_x;
        Int32 image4_y;
        Int32 image4_height;
        Int32 image4_width;
        Int32 image4_x_name;
        Int32 image4_y_name;

        Int32 image5_x;
        Int32 image5_y;
        Int32 image5_height;
        Int32 image5_width;
        Int32 image5_x_name;
        Int32 image5_y_name;

        Int32 image6_x;
        Int32 image6_y;
        Int32 image6_height;
        Int32 image6_width;
        Int32 image6_x_name;
        Int32 image6_y_name;

        Int32 image7_x;
        Int32 image7_y;
        Int32 image7_height;
        Int32 image7_width;
        Int32 image7_x_name;
        Int32 image7_y_name;

        Int32 image8_x;
        Int32 image8_y;
        Int32 image8_height;
        Int32 image8_width;
        Int32 image8_x_name;
        Int32 image8_y_name;

        Int32 image9_x;
        Int32 image9_y;
        Int32 image9_height;
        Int32 image9_width;
        Int32 image9_x_name;
        Int32 image9_y_name;

        Int32 image10_x;
        Int32 image10_y;
        Int32 image10_height;
        Int32 image10_width;
        Int32 image10_x_name;
        Int32 image10_y_name;

        Int32 image11_x;
        Int32 image11_y;
        Int32 image11_height;
        Int32 image11_width;
        Int32 image11_x_name;
        Int32 image11_y_name;

        Int32 image12_x;
        Int32 image12_y;
        Int32 image12_height;
        Int32 image12_width;
        Int32 image12_x_name;
        Int32 image12_y_name;

        Int32 image13_x;
        Int32 image13_y;
        Int32 image13_height;
        Int32 image13_width;
        Int32 image13_x_name;
        Int32 image13_y_name;

        Int32 image14_x;
        Int32 image14_y;
        Int32 image14_height;
        Int32 image14_width;
        Int32 image14_x_name;
        Int32 image14_y_name;

        Int32 image15_x;
        Int32 image15_y;
        Int32 image15_height;
        Int32 image15_width;
        Int32 image15_x_name;
        Int32 image15_y_name;

        Int32 image16_x;
        Int32 image16_y;
        Int32 image16_height;
        Int32 image16_width;
        Int32 image16_x_name;
        Int32 image16_y_name;

        Int32 image17_x;
        Int32 image17_y;
        Int32 image17_height;
        Int32 image17_width;
        Int32 image17_x_name;
        Int32 image17_y_name;

        Int32 image18_x;
        Int32 image18_y;
        Int32 image18_height;
        Int32 image18_width;
        Int32 image18_x_name;
        Int32 image18_y_name;

        Int32 image19_x;
        Int32 image19_y;
        Int32 image19_height;
        Int32 image19_width;
        Int32 image19_x_name;
        Int32 image19_y_name;

        Int32 image20_x;
        Int32 image20_y;
        Int32 image20_height;
        Int32 image20_width;
        Int32 image20_x_name;
        Int32 image20_y_name;

        Int32 packageA_x;
        Int32 packageA_y;
        String packageA;

        Int32 packageB_x;
        Int32 packageB_y;
        String packageB;

        Int32 packageC_x;
        Int32 packageC_y;
        String packageC;

        Int32 packageD_x;
        Int32 packageD_y;
        String packageD;

        Int32 packageE_x;
        Int32 packageE_y;
        String packageE;

        Int32 packageF_x;
        Int32 packageF_y;
        String packageF;

        Int32 packageG_x;
        Int32 packageG_y;
        String packageG;

        Int32 packageH_x;
        Int32 packageH_y;
        String packageH;

        Int32 packageI_x;
        Int32 packageI_y;
        String packageI;

        Int32 packageJ_x;
        Int32 packageJ_y;
        String packageJ;

        Int32 packageK_x;
        Int32 packageK_y;
        String packageK;

        Int32 packageL_x;
        Int32 packageL_y;
        String packageL;

        Int32 packageP_x;
        Int32 packageP_y;
        String packageP;

        Int32 packageQ_x;
        Int32 packageQ_y;
        String packageQ;

        Int32 packageR_x;
        Int32 packageR_y;
        String packageR;

        Int32 packageS_x;
        Int32 packageS_y;
        String packageS;

        Int32 packageT_x;
        Int32 packageT_y;
        String packageT;

        Int32 packageU_x;
        Int32 packageU_y;
        String packageU;

        Int32 packageV_x;
        Int32 packageV_y;
        String packageV;

        Int32 packageW_x;
        Int32 packageW_y;
        String packageW;

        Int32 packageX_x;
        Int32 packageX_y;
        String packageX;

        Int32 packageY_x;
        Int32 packageY_y;
        String packageY;

        Int32 packageZ_x;
        Int32 packageZ_y;
        String packageZ;

        Int32 packageDD_x;
        Int32 packageDD_y;
        String packageDD;

        Int32 packageDDR_x;
        Int32 packageDDR_y;
        String packageDDR;

        #endregion

        #region Functions

        internal void handelAllActivities(string fileName, string Templatevalue, int FlagValue, bool Ischecked)
        {
            isCheckedVal = Ischecked;
            ++iFlag;
            getvaluesFromXML(Templatevalue);
            InitializeVariables();
            pdfInitialize(fileName, Templatevalue);
            pdfWrite(Templatevalue, FlagValue);
            pdfClose();
        }


        private void InitializeVariables()
        {
            StudentLastName = StudentLastName.Replace("\\", "");
            StudentLastName = clsDashBoard.SanitizeFileName(StudentLastName);

            StudentFirstName = StudentFirstName.Replace("\\", "");
            StudentFirstName = clsDashBoard.SanitizeFileName(StudentFirstName);

            studentFullNameForFile = StudentLastName + "_" + StudentFirstName + "_";
            studentFullName = StudentFirstName + " " + StudentLastName;

            if ((StudentImage1_URL != null) && (StudentImage1_URL.Trim() != ""))
                studentImage1_Source = iTextSharp.text.Image.GetInstance(StudentImage1_URL);
            if ((StudentImage2_URL != null) && (StudentImage2_URL.Trim() != ""))
                studentImage2_Source = iTextSharp.text.Image.GetInstance(StudentImage2_URL);
            if ((StudentImage3_URL != null) && (StudentImage3_URL.Trim() != ""))
                studentImage3_Source = iTextSharp.text.Image.GetInstance(StudentImage3_URL);
            if ((StudentImage4_URL != null) && (StudentImage4_URL.Trim() != ""))
                studentImage4_Source = iTextSharp.text.Image.GetInstance(StudentImage4_URL);
            if ((StudentImage5_URL != null) && (StudentImage5_URL.Trim() != ""))
                studentImage5_Source = iTextSharp.text.Image.GetInstance(StudentImage5_URL);
            if ((StudentImage6_URL != null) && (StudentImage6_URL.Trim() != ""))
                studentImage6_Source = iTextSharp.text.Image.GetInstance(StudentImage6_URL);
            if ((StudentImage7_URL != null) && (StudentImage7_URL.Trim() != ""))
                studentImage7_Source = iTextSharp.text.Image.GetInstance(StudentImage7_URL);
            if ((StudentImage8_URL != null) && (StudentImage8_URL.Trim() != ""))
                studentImage8_Source = iTextSharp.text.Image.GetInstance(StudentImage8_URL);
            if ((StudentImage9_URL != null) && (StudentImage9_URL.Trim() != ""))
                studentImage9_Source = iTextSharp.text.Image.GetInstance(StudentImage9_URL);
            if ((StudentImage10_URL != null) && (StudentImage10_URL.Trim() != ""))
                studentImage10_Source = iTextSharp.text.Image.GetInstance(StudentImage10_URL);
            if ((StudentImage11_URL != null) && (StudentImage11_URL.Trim() != ""))
                studentImage11_Source = iTextSharp.text.Image.GetInstance(StudentImage11_URL);
            if ((StudentImage12_URL != null) && (StudentImage12_URL.Trim() != ""))
                studentImage12_Source = iTextSharp.text.Image.GetInstance(StudentImage12_URL);
            if ((StudentImage13_URL != null) && (StudentImage13_URL.Trim() != ""))
                studentImage13_Source = iTextSharp.text.Image.GetInstance(StudentImage13_URL);
            if ((StudentImage14_URL != null) && (StudentImage14_URL.Trim() != ""))
                studentImage14_Source = iTextSharp.text.Image.GetInstance(StudentImage14_URL);
            if ((StudentImage15_URL != null) && (StudentImage15_URL.Trim() != ""))
                studentImage15_Source = iTextSharp.text.Image.GetInstance(StudentImage15_URL);
            if ((StudentImage16_URL != null) && (StudentImage16_URL.Trim() != ""))
                studentImage16_Source = iTextSharp.text.Image.GetInstance(StudentImage16_URL);
            if ((StudentImage17_URL != null) && (StudentImage17_URL.Trim() != ""))
                studentImage17_Source = iTextSharp.text.Image.GetInstance(StudentImage17_URL);
            if ((StudentImage18_URL != null) && (StudentImage18_URL.Trim() != ""))
                studentImage18_Source = iTextSharp.text.Image.GetInstance(StudentImage18_URL);
            if ((StudentImage19_URL != null) && (StudentImage19_URL.Trim() != ""))
                studentImage19_Source = iTextSharp.text.Image.GetInstance(StudentImage19_URL);
            if ((StudentImage20_URL != null) && (StudentImage20_URL.Trim() != ""))
                studentImage20_Source = iTextSharp.text.Image.GetInstance(StudentImage20_URL);

        }

        private void getvaluesFromXML(string Templatevalue)
        {
            if (Templatevalue == "1")
            {
                string XMLFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"TemplatesCoordinates\Coordinates.xml");
                DataSet ds = new DataSet();
                ds.ReadXml(XMLFilePath);
                int a = Convert.ToInt32(TemplateToSelect);
                DataTable dt = ds.Tables[a];

                frstnameCreditCard_x = Convert.ToInt32(dt.Rows[0]["StudentFirstName_CreditCard_X"]);
                frstnameCreditCard_y = Convert.ToInt32(dt.Rows[0]["StudentFirstName_CreditCard_Y"]);
                lastnameCreditCard_x = Convert.ToInt32(dt.Rows[0]["StudentLastName_CreditCard_X"]);
                lastnameCreditCard_y = Convert.ToInt32(dt.Rows[0]["StudentLastName_CreditCard_Y"]);

                //Password
                studentpassword_x = Convert.ToInt32(dt.Rows[0]["StudentPassword_X"]);
                studentpassword_y = Convert.ToInt32(dt.Rows[0]["StudentPassword_Y"]);

                frstname_x = Convert.ToInt32(dt.Rows[0]["StudentFirstName_X"]);
                frstname_y = Convert.ToInt32(dt.Rows[0]["StudentFirstName_Y"]);

                //Added By Mohan Tangella
                studentId_x = Convert.ToInt32(dt.Rows[0]["StudentId_X"]);
                studentId_y = Convert.ToInt32(dt.Rows[0]["StudentId_Y"]);

                lastname_x = Convert.ToInt32(dt.Rows[0]["StudentLastName_X"]);
                lastname_y = Convert.ToInt32(dt.Rows[0]["StudentLastName_Y"]);

                schoolnameCreditCard_x = Convert.ToInt32(dt.Rows[0]["SchoolName_CreditCard_X"]);
                schoolnameCreditCard_y = Convert.ToInt32(dt.Rows[0]["SchoolName_CreditCard_Y"]);

                gradex = Convert.ToInt32(dt.Rows[0]["StudentGrade_X"]);
                gradey = Convert.ToInt32(dt.Rows[0]["StudentGrade_Y"]);

                teachername_x = Convert.ToInt32(dt.Rows[0]["StudentTeacher_X"]);
                teachername_y = Convert.ToInt32(dt.Rows[0]["StudentTeacehr_Y"]);

                deadline_x = Convert.ToInt32(dt.Rows[0]["DeadlineDate_X"]);
                deadline_y = Convert.ToInt32(dt.Rows[0]["DeadlineDate_Y"]);
                deadline = Convert.ToDateTime(DeadlineDate);

                deadline2_x = Convert.ToInt32(dt.Rows[0]["DeadlineDateTwo_X"]);
                deadline2_y = Convert.ToInt32(dt.Rows[0]["DeadlineDateTwo_Y"]);
                deadline2 = Convert.ToDateTime(DeadlineDate2);

                String getmonthfromdate = deadline.ToString("MMMM", CultureInfo.InvariantCulture); //October
                String getdatefromdate = Convert.ToString(deadline.Day);//28
                String getdayfromdate = Convert.ToString(deadline.DayOfWeek);//sunday

                String getmonthfromdate2 = deadline2.ToString("MMMM", CultureInfo.InvariantCulture); //October
                String getdatefromdate2 = Convert.ToString(deadline2.Day);//28
                String getdayfromdate2 = Convert.ToString(deadline2.DayOfWeek);//sunday

                DeadlinetoPrint = SchoolName + " by " + getdayfromdate + ", " + getmonthfromdate + " " + getdatefromdate;

                DeadlinetoPrint2 = getdayfromdate2 + "," + getmonthfromdate2 + " " + getdatefromdate2;

                packageA_x = Convert.ToInt32(dt.Rows[0]["PackageA_X"]);
                packageA_y = Convert.ToInt32(dt.Rows[0]["PackageA_Y"]);
                packageA = String.Format("{0:0.00}", Package_A_Price);

                packageB_x = Convert.ToInt32(dt.Rows[0]["PackageB_X"]);
                packageB_y = Convert.ToInt32(dt.Rows[0]["PackageB_Y"]);
                packageB = String.Format("{0:0.00}", Package_B_Price);

                packageC_x = Convert.ToInt32(dt.Rows[0]["PackageC_X"]);
                packageC_y = Convert.ToInt32(dt.Rows[0]["PackageC_Y"]);
                packageC = String.Format("{0:0.00}", Package_C_Price);

                packageD_x = Convert.ToInt32(dt.Rows[0]["PackageD_X"]);
                packageD_y = Convert.ToInt32(dt.Rows[0]["PackageD_Y"]);
                packageD = String.Format("{0:0.00}", Package_D_Price);

                packageE_x = Convert.ToInt32(dt.Rows[0]["PackageE_X"]);
                packageE_y = Convert.ToInt32(dt.Rows[0]["PackageE_Y"]);
                packageE = String.Format("{0:0.00}", Package_E_Price);

                packageF_x = Convert.ToInt32(dt.Rows[0]["PackageF_X"]);
                packageF_y = Convert.ToInt32(dt.Rows[0]["PackageF_Y"]);
                packageF = String.Format("{0:0.00}", Package_F_Price);

                packageG_x = Convert.ToInt32(dt.Rows[0]["PackageG_X"]);
                packageG_y = Convert.ToInt32(dt.Rows[0]["PackageG_Y"]);
                packageG = String.Format("{0:0.00}", Package_G_Price);

                packageH_x = Convert.ToInt32(dt.Rows[0]["PackageH_X"]);
                packageH_y = Convert.ToInt32(dt.Rows[0]["PackageH_Y"]);
                packageH = String.Format("{0:0.00}", Package_H_Price);

                packageI_x = Convert.ToInt32(dt.Rows[0]["PackageI_X"]);
                packageI_y = Convert.ToInt32(dt.Rows[0]["PackageI_Y"]);
                packageI = String.Format("{0:0.00}", Package_I_Price);

                packageJ_x = Convert.ToInt32(dt.Rows[0]["PackageJ_X"]);
                packageJ_y = Convert.ToInt32(dt.Rows[0]["PackageJ_Y"]);
                packageJ = String.Format("{0:0.00}", Package_J_Price);

                packageK_x = Convert.ToInt32(dt.Rows[0]["PackageK_X"]);
                packageK_y = Convert.ToInt32(dt.Rows[0]["PackageK_Y"]);
                packageK = String.Format("{0:0.00}", Package_K_Price);

                packageL_x = Convert.ToInt32(dt.Rows[0]["PackageL_X"]);
                packageL_y = Convert.ToInt32(dt.Rows[0]["PackageL_Y"]);
                packageL = String.Format("{0:0.00}", Package_L_Price);

                packageP_x = Convert.ToInt32(dt.Rows[0]["PackageP_X"]);
                packageP_y = Convert.ToInt32(dt.Rows[0]["PackageP_Y"]);
                packageP = String.Format("{0:0.00}", Package_P_Price);

                packageQ_x = Convert.ToInt32(dt.Rows[0]["PackageQ_X"]);
                packageQ_y = Convert.ToInt32(dt.Rows[0]["PackageQ_Y"]);
                packageQ = String.Format("{0:0.00}", Package_Q_Price);

                packageR_x = Convert.ToInt32(dt.Rows[0]["PackageR_X"]);
                packageR_y = Convert.ToInt32(dt.Rows[0]["PackageR_Y"]);
                packageR = String.Format("{0:0.00}", Package_R_Price);

                packageS_x = Convert.ToInt32(dt.Rows[0]["PackageS_X"]);
                packageS_y = Convert.ToInt32(dt.Rows[0]["PackageS_Y"]);
                packageS = String.Format("{0:0.00}", Package_S_Price);

                packageT_x = Convert.ToInt32(dt.Rows[0]["PackageT_X"]);
                packageT_y = Convert.ToInt32(dt.Rows[0]["PackageT_Y"]);
                packageT = String.Format("{0:0.00}", Package_T_Price);

                packageU_x = Convert.ToInt32(dt.Rows[0]["PackageU_X"]);
                packageU_y = Convert.ToInt32(dt.Rows[0]["PackageU_Y"]);
                packageU = String.Format("{0:0.00}", Package_U_Price);

                packageV_x = Convert.ToInt32(dt.Rows[0]["PackageV_X"]);
                packageV_y = Convert.ToInt32(dt.Rows[0]["PackageV_Y"]);
                packageV = String.Format("{0:0.00}", Package_V_Price);

                packageW_x = Convert.ToInt32(dt.Rows[0]["PackageW_X"]);
                packageW_y = Convert.ToInt32(dt.Rows[0]["PackageW_Y"]);
                packageW = String.Format("{0:0.00}", Package_W_Price);

                packageX_x = Convert.ToInt32(dt.Rows[0]["PackageX_X"]);
                packageX_y = Convert.ToInt32(dt.Rows[0]["PackageX_Y"]);
                packageX = String.Format("{0:0.00}", Package_X_Price);

                packageY_x = Convert.ToInt32(dt.Rows[0]["PackageY_X"]);
                packageY_y = Convert.ToInt32(dt.Rows[0]["PackageY_Y"]);
                packageY = String.Format("{0:0.00}", Package_Y_Price);

                packageZ_x = Convert.ToInt32(dt.Rows[0]["PackageZ_X"]);
                packageZ_y = Convert.ToInt32(dt.Rows[0]["PackageZ_Y"]);
                packageZ = String.Format("{0:0.00}", Package_Z_Price);

                packageDD_x = Convert.ToInt32(dt.Rows[0]["PackageDD_X"]);
                packageDD_y = Convert.ToInt32(dt.Rows[0]["PackageDD_Y"]);
                packageDD = String.Format("{0:0.00}", Package_DD_Price);

                packageDDR_x = Convert.ToInt32(dt.Rows[0]["PackageDDR_X"]);
                packageDDR_y = Convert.ToInt32(dt.Rows[0]["PackageDDR_Y"]);
                packageDDR = String.Format("{0:0.00}", Package_DDR_Price);
            }
            else
            {
                string XMLFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"TemplatesCoordinates\\Coordinates.xml");
                DataSet ds = new DataSet();
                ds.ReadXml(XMLFilePath);
                int a = Convert.ToInt32(TemplateToSelect);
                DataTable dt = ds.Tables[a];

                studentpassword2_x = Convert.ToInt32(dt.Rows[0]["StudentPasswordTwo_X"]);
                studentpassword2_y = Convert.ToInt32(dt.Rows[0]["StudentPasswordTwo_Y"]);

                image1_x = Convert.ToInt32(dt.Rows[0]["StudentImage1_X"]);
                image1_y = Convert.ToInt32(dt.Rows[0]["StudentImage1_Y"]);
                image1_height = Convert.ToInt32(dt.Rows[0]["StudentImage1_Height"]);
                image1_width = Convert.ToInt32(dt.Rows[0]["StudentImage1_Width"]);
                image1_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName1_X"]);
                image1_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName1_Y"]);

                image2_x = Convert.ToInt32(dt.Rows[0]["StudentImage2_X"]);
                image2_y = Convert.ToInt32(dt.Rows[0]["StudentImage2_Y"]);
                image2_height = Convert.ToInt32(dt.Rows[0]["StudentImage2_Height"]);
                image2_width = Convert.ToInt32(dt.Rows[0]["StudentImage2_Width"]);
                image2_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName2_X"]);
                image2_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName2_Y"]);

                image3_x = Convert.ToInt32(dt.Rows[0]["StudentImage3_X"]);
                image3_y = Convert.ToInt32(dt.Rows[0]["StudentImage3_Y"]);
                image3_height = Convert.ToInt32(dt.Rows[0]["StudentImage3_Height"]);
                image3_width = Convert.ToInt32(dt.Rows[0]["StudentImage3_Width"]);
                image3_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName3_X"]);
                image3_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName3_Y"]);

                image4_x = Convert.ToInt32(dt.Rows[0]["StudentImage4_X"]);
                image4_y = Convert.ToInt32(dt.Rows[0]["StudentImage4_Y"]);
                image4_height = Convert.ToInt32(dt.Rows[0]["StudentImage4_Height"]);
                image4_width = Convert.ToInt32(dt.Rows[0]["StudentImage4_Width"]);
                image4_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName4_X"]);
                image4_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName4_Y"]);

                image5_x = Convert.ToInt32(dt.Rows[0]["StudentImage5_X"]);
                image5_y = Convert.ToInt32(dt.Rows[0]["StudentImage5_Y"]);
                image5_height = Convert.ToInt32(dt.Rows[0]["StudentImage5_Height"]);
                image5_width = Convert.ToInt32(dt.Rows[0]["StudentImage5_Width"]);
                image5_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName5_X"]);
                image5_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName5_Y"]);

                image6_x = Convert.ToInt32(dt.Rows[0]["StudentImage6_X"]);
                image6_y = Convert.ToInt32(dt.Rows[0]["StudentImage6_Y"]);
                image6_height = Convert.ToInt32(dt.Rows[0]["StudentImage6_Height"]);
                image6_width = Convert.ToInt32(dt.Rows[0]["StudentImage6_Width"]);
                image6_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName6_X"]);
                image6_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName6_Y"]);

                image7_x = Convert.ToInt32(dt.Rows[0]["StudentImage7_X"]);
                image7_y = Convert.ToInt32(dt.Rows[0]["StudentImage7_Y"]);
                image7_height = Convert.ToInt32(dt.Rows[0]["StudentImage7_Height"]);
                image7_width = Convert.ToInt32(dt.Rows[0]["StudentImage7_Width"]);
                image7_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName7_X"]);
                image7_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName7_Y"]);

                image8_x = Convert.ToInt32(dt.Rows[0]["StudentImage8_X"]);
                image8_y = Convert.ToInt32(dt.Rows[0]["StudentImage8_Y"]);
                image8_height = Convert.ToInt32(dt.Rows[0]["StudentImage8_Height"]);
                image8_width = Convert.ToInt32(dt.Rows[0]["StudentImage8_Width"]);
                image8_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName8_X"]);
                image8_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName8_Y"]);

                image9_x = Convert.ToInt32(dt.Rows[0]["StudentImage9_X"]);
                image9_y = Convert.ToInt32(dt.Rows[0]["StudentImage9_Y"]);
                image9_height = Convert.ToInt32(dt.Rows[0]["StudentImage9_Height"]);
                image9_width = Convert.ToInt32(dt.Rows[0]["StudentImage9_Width"]);
                image9_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName9_X"]);
                image9_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName9_Y"]);

                image10_x = Convert.ToInt32(dt.Rows[0]["StudentImage10_X"]);
                image10_y = Convert.ToInt32(dt.Rows[0]["StudentImage10_Y"]);
                image10_height = Convert.ToInt32(dt.Rows[0]["StudentImage10_Height"]);
                image10_width = Convert.ToInt32(dt.Rows[0]["StudentImage10_Width"]);
                image10_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName10_X"]);
                image10_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName10_Y"]);

                image11_x = Convert.ToInt32(dt.Rows[0]["StudentImage11_X"]);
                image11_y = Convert.ToInt32(dt.Rows[0]["StudentImage11_Y"]);
                image11_height = Convert.ToInt32(dt.Rows[0]["StudentImage11_Height"]);
                image11_width = Convert.ToInt32(dt.Rows[0]["StudentImage11_Width"]);
                image11_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName11_X"]);
                image11_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName11_Y"]);

                image12_x = Convert.ToInt32(dt.Rows[0]["StudentImage12_X"]);
                image12_y = Convert.ToInt32(dt.Rows[0]["StudentImage12_Y"]);
                image12_height = Convert.ToInt32(dt.Rows[0]["StudentImage12_Height"]);
                image12_width = Convert.ToInt32(dt.Rows[0]["StudentImage12_Width"]);
                image12_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName12_X"]);
                image12_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName12_Y"]);

                image13_x = Convert.ToInt32(dt.Rows[0]["StudentImage13_X"]);
                image13_y = Convert.ToInt32(dt.Rows[0]["StudentImage13_Y"]);
                image13_height = Convert.ToInt32(dt.Rows[0]["StudentImage13_Height"]);
                image13_width = Convert.ToInt32(dt.Rows[0]["StudentImage13_Width"]);
                image13_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName13_X"]);
                image13_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName13_Y"]);

                image14_x = Convert.ToInt32(dt.Rows[0]["StudentImage14_X"]);
                image14_y = Convert.ToInt32(dt.Rows[0]["StudentImage14_Y"]);
                image14_height = Convert.ToInt32(dt.Rows[0]["StudentImage14_Height"]);
                image14_width = Convert.ToInt32(dt.Rows[0]["StudentImage14_Width"]);
                image14_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName14_X"]);
                image14_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName14_Y"]);

                image15_x = Convert.ToInt32(dt.Rows[0]["StudentImage15_X"]);
                image15_y = Convert.ToInt32(dt.Rows[0]["StudentImage15_Y"]);
                image15_height = Convert.ToInt32(dt.Rows[0]["StudentImage15_Height"]);
                image15_width = Convert.ToInt32(dt.Rows[0]["StudentImage15_Width"]);
                image15_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName15_X"]);
                image15_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName15_Y"]);

                image16_x = Convert.ToInt32(dt.Rows[0]["StudentImage16_X"]);
                image16_y = Convert.ToInt32(dt.Rows[0]["StudentImage16_Y"]);
                image16_height = Convert.ToInt32(dt.Rows[0]["StudentImage16_Height"]);
                image16_width = Convert.ToInt32(dt.Rows[0]["StudentImage16_Width"]);
                image16_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName16_X"]);
                image16_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName16_Y"]);

                image17_x = Convert.ToInt32(dt.Rows[0]["StudentImage17_X"]);
                image17_y = Convert.ToInt32(dt.Rows[0]["StudentImage17_Y"]);
                image17_height = Convert.ToInt32(dt.Rows[0]["StudentImage17_Height"]);
                image17_width = Convert.ToInt32(dt.Rows[0]["StudentImage17_Width"]);
                image17_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName17_X"]);
                image17_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName17_Y"]);

                image18_x = Convert.ToInt32(dt.Rows[0]["StudentImage18_X"]);
                image18_y = Convert.ToInt32(dt.Rows[0]["StudentImage18_Y"]);
                image18_height = Convert.ToInt32(dt.Rows[0]["StudentImage18_Height"]);
                image18_width = Convert.ToInt32(dt.Rows[0]["StudentImage18_Width"]);
                image18_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName18_X"]);
                image18_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName18_Y"]);

                image19_x = Convert.ToInt32(dt.Rows[0]["StudentImage19_X"]);
                image19_y = Convert.ToInt32(dt.Rows[0]["StudentImage19_Y"]);
                image19_height = Convert.ToInt32(dt.Rows[0]["StudentImage19_Height"]);
                image19_width = Convert.ToInt32(dt.Rows[0]["StudentImage19_Width"]);
                image19_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName19_X"]);
                image19_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName19_Y"]);

                image20_x = Convert.ToInt32(dt.Rows[0]["StudentImage20_X"]);
                image20_y = Convert.ToInt32(dt.Rows[0]["StudentImage20_Y"]);
                image20_height = Convert.ToInt32(dt.Rows[0]["StudentImage20_Height"]);
                image20_width = Convert.ToInt32(dt.Rows[0]["StudentImage20_Width"]);
                image20_x_name = Convert.ToInt32(dt.Rows[0]["StudentImageName20_X"]);
                image20_y_name = Convert.ToInt32(dt.Rows[0]["StudentImageName20_Y"]);


            }
        }

        private void pdfInitialize(string fileName, string templatevalue)
        {

            mergedGeneratedFile = GeneratedFileURL;
            if (templatevalue == "1")
            {

                baseFile = fileName;

                baseFilepdfStream = new FileStream(baseFile, FileMode.Open);
                reader = new PdfReader(baseFilepdfStream);
                do
                {
                    if (isCheckedVal == true)
                    {
                        generatedFile = GeneratedFileURL + studentFullNameForFile + newFileSuffix + extensionOfFile;
                        newFileSuffix = newFileSuffix + 1;
                    }
                    else
                    {
                        generatedFile = GeneratedFileURL + studentFullNameForFile + newFileSuffix + extensionOfFile;
                        newFileSuffix = newFileSuffix + 1;
                    }

                }

                while (File.Exists(generatedFile));
                newFileSuffix = 1;
                fs = new FileStream(generatedFile, FileMode.Create, FileAccess.Write);
                stamper = new PdfStamper(reader, fs);
                generatedFileNameToCombine = generatedFile + ",";
                baseFilepdfStream.Close();
            }
            else
            {
                baseFile = fileName;
                generatedFile = GeneratedFileURL + studentFullNameForFile + newFileSuffix + extensionOfFile;

                baseFilepdfStream = new FileStream(baseFile, FileMode.Open);

                reader = new PdfReader(baseFilepdfStream);

                do
                {
                    if (isCheckedVal == true)
                    {
                        generatedFile = GeneratedFileURL + studentFullNameForFile + newFileSuffix + extensionOfFile;
                        newFileSuffix = newFileSuffix + 1;
                    }
                    else
                    {
                        generatedFile = GeneratedFileURL + studentFullNameForFile + newFileSuffix + extensionOfFile;
                        newFileSuffix = newFileSuffix + 1;
                    }

                }

                while (File.Exists(generatedFile));
                newFileSuffix = 1;
                fs = new FileStream(generatedFile, FileMode.Create, FileAccess.Write);
                stamper = new PdfStamper(reader, fs);
                generatedFileNameToCombine += generatedFile;
                baseFilepdfStream.Close();
            }


        }

        private void pdfWrite(string Templatevalue, int FlagVal)
        {
            if (Templatevalue == "1")
            {

                pdfContentByte = stamper.GetOverContent(1);
                pdfSetFontAttributes(pdfContentByte);

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, studentFullName, frstnameCreditCard_x, frstnameCreditCard_y, 0);
                pdfContentByte.EndText();


                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, SchoolName, schoolnameCreditCard_x, schoolnameCreditCard_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentFirstName, frstname_x, frstname_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentLastName, lastname_x, lastname_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentGrade, gradex, gradey, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentTeacher, teachername_x, teachername_y, 0);
                pdfContentByte.EndText();

                //Added By Mohan Tangella
                pdfContentByte.BeginText();

                iTextSharp.text.pdf.PdfContentByte cb = pdfContentByte;
                iTextSharp.text.pdf.Barcode128 bc = new Barcode128();
                bc.TextAlignment = Element.ALIGN_LEFT;
                bc.Code = StudentId;
                bc.StartStopText = false;
                bc.CodeType = iTextSharp.text.pdf.Barcode128.EAN13;
                bc.Extended = false;

                iTextSharp.text.Image img = bc.CreateImageWithBarcode(cb,
                iTextSharp.text.BaseColor.BLACK, iTextSharp.text.BaseColor.BLACK);

                pdfContentByte.AddImage(img, 160, 0, 0, 30, studentId_x, studentId_y);
                GC.Collect();

                //pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentId, studentId_x, studentId_y, 0);
                pdfContentByte.EndText();

                pdfSetPackageFontAttributes(pdfContentByte);
                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageA == null ? "" : packageA, packageA_x, packageA_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageB == null ? "" : packageB, packageB_x, packageB_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageC == null ? "" : packageC, packageC_x, packageC_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageD == null ? "" : packageD, packageD_x, packageD_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageE == null ? "" : packageE, packageE_x, packageE_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageF == null ? "" : packageF, packageF_x, packageF_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageG == null ? "" : packageG, packageG_x, packageG_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageH == null ? "" : packageH, packageH_x, packageH_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageI == null ? "" : packageI, packageI_x, packageI_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageJ == null ? "" : packageJ, packageJ_x, packageJ_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageK == null ? "" : packageK, packageK_x, packageK_y, 0);
                pdfContentByte.EndText();

                int templateno = Convert.ToInt32(TemplateToSelect);
                if (templateno == 1)
                {
                    pdfContentByte.BeginText();
                    pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageL == null ? "" : packageL, packageL_x, packageL_y, 0);
                    pdfContentByte.EndText();
                }
                //Package L is not found on template 1 that's why this line is commented by rahul on 04-Oct-2013 


                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageP == null ? "" : packageP, packageP_x, packageP_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageQ == null ? "" : packageQ, packageQ_x, packageQ_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageR == null ? "" : packageR, packageR_x, packageR_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageS == null ? "" : packageS, packageS_x, packageS_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageT == null ? "" : packageT, packageT_x, packageT_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageU == null ? "" : packageU, packageU_x, packageU_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageV == null ? "" : packageV, packageV_x, packageV_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageW == null ? "" : packageW, packageW_x, packageW_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageX == null ? "" : packageX, packageX_x, packageX_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageY == null ? "" : packageY, packageY_x, packageY_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageDD == null ? "" : packageDD, packageDD_x, packageDD_y, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, packageDDR == null ? "" : packageDDR, packageDDR_x, packageDDR_y, 0);
                pdfContentByte.EndText();


                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, DeadlinetoPrint, deadline_x, deadline_y, 0);
                pdfContentByte.EndText();


                pdfSetBoldPassword(pdfContentByte);
                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Password, studentpassword_x, studentpassword_y, 0);
                pdfContentByte.EndText();

                pdfSetCurrentDateFontAttributes(pdfContentByte);
                pdfSetBoldPassword(pdfContentByte);
                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, DeadlinetoPrint2, deadline2_x, deadline2_y, 0);
                pdfContentByte.EndText();


            }
            else
            {
                int newHeight = 0;
                int newWidth = 0;
                pdfContentByte = stamper.GetOverContent(1);
                pdfSetFontAttributes(pdfContentByte);

                if (studentImage1_Source != null)
                {
                    getImageatCenter(80, studentImage1_Source.Url.OriginalString.ToString(), 1, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage1_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage1_Source, newWidth, 0, 0, newHeight, image1_x, image1_y);
                    img.Dispose();
                    GC.Collect();
                }
                if (studentImage2_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage2_Source.Url.OriginalString.ToString(), 2, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage2_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage2_Source, newWidth, 0, 0, newHeight, image2_x, image2_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage3_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage3_Source.Url.OriginalString.ToString(), 3, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage3_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage3_Source, newWidth, 0, 0, newHeight, image3_x, image3_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage4_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage4_Source.Url.OriginalString.ToString(), 4, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage4_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage4_Source, newWidth, 0, 0, newHeight, image4_x, image4_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage5_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage5_Source.Url.OriginalString.ToString(), 5, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage5_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage5_Source, newWidth, 0, 0, newHeight, image5_x, image5_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage6_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage6_Source.Url.OriginalString.ToString(), 6, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage6_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage6_Source, newWidth, 0, 0, newHeight, image6_x, image6_y);

                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage7_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage7_Source.Url.OriginalString.ToString(), 7, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage7_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage7_Source, newWidth, 0, 0, newHeight, image7_x, image7_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage8_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage8_Source.Url.OriginalString.ToString(), 8, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage8_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage8_Source, newWidth, 0, 0, newHeight, image8_x, image8_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage9_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage9_Source.Url.OriginalString.ToString(), 9, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage9_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage9_Source, newWidth, 0, 0, newHeight, image9_x, image9_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage10_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage10_Source.Url.OriginalString.ToString(), 10, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage10_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage10_Source, newWidth, 0, 0, newHeight, image10_x, image10_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage11_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage11_Source.Url.OriginalString.ToString(), 11, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage11_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage11_Source, newWidth, 0, 0, newHeight, image11_x, image11_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage12_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage12_Source.Url.OriginalString.ToString(), 12, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage12_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage12_Source, newWidth, 0, 0, newHeight, image12_x, image12_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage13_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage13_Source.Url.OriginalString.ToString(), 13, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage13_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage13_Source, newWidth, 0, 0, newHeight, image13_x, image13_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage14_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage14_Source.Url.OriginalString.ToString(), 14, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage14_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage14_Source, newWidth, 0, 0, newHeight, image14_x, image14_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage15_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage15_Source.Url.OriginalString.ToString(), 15, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage15_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage15_Source, newWidth, 0, 0, newHeight, image15_x, image15_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage16_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage16_Source.Url.OriginalString.ToString(), 16, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage16_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage16_Source, newWidth, 0, 0, newHeight, image16_x, image16_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage17_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage17_Source.Url.OriginalString.ToString(), 17, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage17_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage17_Source, newWidth, 0, 0, newHeight, image17_x, image17_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage18_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage18_Source.Url.OriginalString.ToString(), 18, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage18_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage18_Source, newWidth, 0, 0, newHeight, image18_x, image18_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage19_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage19_Source.Url.OriginalString.ToString(), 19, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage19_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage19_Source, newWidth, 0, 0, newHeight, image19_x, image19_y);
                    img.Dispose();
                    GC.Collect();
                }

                if (studentImage20_Source != null)
                {
                    newHeight = 0;
                    newWidth = 0;
                    getImageatCenter(80, studentImage20_Source.Url.OriginalString.ToString(), 20, out newHeight, out newWidth);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(studentImage20_Source.Url.OriginalString.ToString());
                    pdfContentByte.AddImage(studentImage20_Source, newWidth, 0, 0, newHeight, image20_x, image20_y);
                    img.Dispose();
                    GC.Collect();
                }



                pdfSetBoldPassword(pdfContentByte);
                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Password2, studentpassword2_x, studentpassword2_y, 0);
                pdfContentByte.EndText();

                pdfSetBoldImageName(pdfContentByte);
                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName1, image1_x_name, image1_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName2, image2_x_name, image2_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName3, image3_x_name, image3_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName4, image4_x_name, image4_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName5, image5_x_name, image5_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName6, image6_x_name, image6_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName7, image7_x_name, image7_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName8, image8_x_name, image8_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName9, image9_x_name, image9_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName10, image10_x_name, image10_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName11, image11_x_name, image11_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName12, image12_x_name, image12_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName13, image13_x_name, image13_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName14, image14_x_name, image14_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName15, image15_x_name, image15_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName16, image16_x_name, image16_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName17, image17_x_name, image17_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName18, image18_x_name, image18_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName19, image19_x_name, image19_y_name, 0);
                pdfContentByte.EndText();

                pdfContentByte.BeginText();
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, StudentImageName20, image20_x_name, image20_y_name, 0);
                pdfContentByte.EndText();

                pdfClose();
                //Merge Two Pdf into one...
                pdfMerge(generatedFileNameToCombine, FlagVal);
                generatedFileNameToCombine = "";
            }



        }

        private void pdfSetFontAttributes(PdfContentByte cb)
        {
            BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SetColorFill(BaseColor.BLACK);
            cb.SetFontAndSize(baseFont, 8);
        }

        private void pdfSetPackageFontAttributes(PdfContentByte cb)
        {
            BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SetColorFill(BaseColor.BLACK);
            cb.SetFontAndSize(baseFont, 7);
        }

        private void pdfSetCurrentDateFontAttributes(PdfContentByte cb)
        {
            cb.SetColorFill(BaseColor.WHITE);
        }

        private void pdfSetBoldImageName(PdfContentByte cb)
        {
            cb.SetFontAndSize(FontFactory.GetFont(FontFactory.HELVETICA_BOLD).BaseFont, 8);
            cb.SetColorFill(BaseColor.BLACK);
        }

        private void pdfSetBoldPassword(PdfContentByte cbb)
        {
            cbb.SetFontAndSize(FontFactory.GetFont(FontFactory.HELVETICA_BOLD).BaseFont, 10);
        }

        private void pdfSetBoldSchoolTime(PdfContentByte cb)
        {
            cb.SetFontAndSize(FontFactory.GetFont(FontFactory.HELVETICA_BOLD).BaseFont, 8);
        }

        private void pdfClose()
        {
            stamper.Close();
        }

        private void getImageatCenter(int intImageSize, string ImgPath, int imageNumber, out int newHeight, out int newWidth)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(ImgPath);

            newHeight = img.Height;
            newWidth = img.Width;
            float CenterWidth = 0;
            float CenterHeight = 0;

            if (img.Height > intImageSize && img.Width > intImageSize)
            {
                if (img.Height > img.Width)
                {
                    newHeight = intImageSize;
                    newWidth = (int)((intImageSize * img.Width) / img.Height);
                    //
                }
                else if (img.Width > img.Height)
                {
                    newWidth = intImageSize;
                    newHeight = (int)((intImageSize * img.Height) / img.Width);
                }

                else
                {
                    newWidth = newHeight = intImageSize;
                }

            }
            else if (img.Height > intImageSize || img.Width > intImageSize)
            {
                if (img.Height > intImageSize)
                {
                    newHeight = intImageSize;
                    newWidth = (int)((intImageSize * img.Width) / img.Height);

                }
                else if (img.Width > intImageSize)
                {
                    newWidth = intImageSize;
                    newHeight = (int)((intImageSize * img.Height) / img.Width);
                }
            }
            // Get Center Height and Weight
            CenterWidth = newWidth / 2;
            CenterHeight = newHeight / 2;
            if (imageNumber == 1)
            {
                if (newWidth > newHeight)
                {
                    image1_x = image1_x - Convert.ToInt32(CenterWidth);
                    image1_y = image1_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image1_x = image1_x - Convert.ToInt32(CenterWidth);
                    image1_y = image1_y + (Convert.ToInt32(CenterHeight * 1.29375) - 32);
                }
            }
            if (imageNumber == 2)
            {
                if (newWidth > newHeight)
                {
                    image2_x = image2_x - Convert.ToInt32(CenterWidth);
                    image2_y = image2_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image2_x = image2_x - Convert.ToInt32(CenterWidth);
                    image2_y = image2_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            if (imageNumber == 3)
            {
                if (newWidth > newHeight)
                {
                    image3_x = image3_x - Convert.ToInt32(CenterWidth);
                    image3_y = image3_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image3_x = image3_x - Convert.ToInt32(CenterWidth);
                    image3_y = image3_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            if (imageNumber == 4)
            {
                if (newWidth > newHeight)
                {
                    image4_x = image4_x - Convert.ToInt32(CenterWidth);
                    image4_y = image4_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image4_x = image4_x - Convert.ToInt32(CenterWidth);
                    image4_y = image4_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 5)
            {
                if (newWidth > newHeight)
                {
                    image5_x = image5_x - Convert.ToInt32(CenterWidth);
                    image5_y = image5_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image5_x = image5_x - Convert.ToInt32(CenterWidth);
                    image5_y = image5_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 6)
            {
                if (newWidth > newHeight)
                {
                    image6_x = image6_x - Convert.ToInt32(CenterWidth);
                    image6_y = image6_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image6_x = image6_x - Convert.ToInt32(CenterWidth);
                    image6_y = image6_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 7)
            {
                if (newWidth > newHeight)
                {
                    image7_x = image7_x - Convert.ToInt32(CenterWidth);
                    image7_y = image7_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image7_x = image7_x - Convert.ToInt32(CenterWidth);
                    image7_y = image7_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 8)
            {
                if (newWidth > newHeight)
                {
                    image8_x = image8_x - Convert.ToInt32(CenterWidth);
                    image8_y = image8_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image8_x = image8_x - Convert.ToInt32(CenterWidth);
                    image8_y = image8_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 9)
            {
                if (newWidth > newHeight)
                {
                    image9_x = image9_x - Convert.ToInt32(CenterWidth);
                    image9_y = image9_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image9_x = image9_x - Convert.ToInt32(CenterWidth);
                    image9_y = image9_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 10)
            {
                if (newWidth > newHeight)
                {
                    image10_x = image10_x - Convert.ToInt32(CenterWidth);
                    image10_y = image10_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image10_x = image10_x - Convert.ToInt32(CenterWidth);
                    image10_y = image10_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 11)
            {
                if (newWidth > newHeight)
                {
                    image11_x = image11_x - Convert.ToInt32(CenterWidth);
                    image11_y = image11_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image11_x = image11_x - Convert.ToInt32(CenterWidth);
                    image11_y = image11_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 12)
            {
                if (newWidth > newHeight)
                {
                    image12_x = image12_x - Convert.ToInt32(CenterWidth);
                    image12_y = image12_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image12_x = image12_x - Convert.ToInt32(CenterWidth);
                    image12_y = image12_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 13)
            {
                if (newWidth > newHeight)
                {
                    image13_x = image13_x - Convert.ToInt32(CenterWidth);
                    image13_y = image13_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image13_x = image13_x - Convert.ToInt32(CenterWidth);
                    image13_y = image13_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 14)
            {
                if (newWidth > newHeight)
                {
                    image14_x = image14_x - Convert.ToInt32(CenterWidth);
                    image14_y = image14_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image14_x = image14_x - Convert.ToInt32(CenterWidth);
                    image14_y = image14_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 15)
            {
                if (newWidth > newHeight)
                {
                    image15_x = image15_x - Convert.ToInt32(CenterWidth);
                    image15_y = image15_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image15_x = image15_x - Convert.ToInt32(CenterWidth);
                    image15_y = image15_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 16)
            {
                if (newWidth > newHeight)
                {
                    image16_x = image16_x - Convert.ToInt32(CenterWidth);
                    image16_y = image16_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image16_x = image16_x - Convert.ToInt32(CenterWidth);
                    image16_y = image16_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 17)
            {
                if (newWidth > newHeight)
                {
                    image17_x = image17_x - Convert.ToInt32(CenterWidth);
                    image17_y = image17_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image17_x = image17_x - Convert.ToInt32(CenterWidth);
                    image17_y = image17_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 18)
            {
                if (newWidth > newHeight)
                {
                    image18_x = image18_x - Convert.ToInt32(CenterWidth);
                    image18_y = image18_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image18_x = image18_x - Convert.ToInt32(CenterWidth);
                    image18_y = image18_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 19)
            {
                if (newWidth > newHeight)
                {
                    image19_x = image19_x - Convert.ToInt32(CenterWidth);
                    image19_y = image19_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image19_x = image19_x - Convert.ToInt32(CenterWidth);
                    image19_y = image19_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
            else if (imageNumber == 20)
            {
                if (newWidth > newHeight)
                {
                    image20_x = image20_x - Convert.ToInt32(CenterWidth - 3);
                    image20_y = image20_y + Convert.ToInt32(CenterHeight * 1.29375);
                }
                else
                {
                    image20_x = image20_x - Convert.ToInt32(CenterWidth);
                    image20_y = image20_y + (Convert.ToInt32(CenterHeight * 1.29375) - 25);
                }
            }
        }

        private void pdfMerge(string PathToSave, int FlagVall)
        {
            string NewPathWithTeacher = "";
            string PathForPdf1 = "", PathForPdf2 = "";
            // Extract path of existing pdf
            string[] combinepath = PathToSave.Split(',');
            PathForPdf1 = combinepath[0];
            PathForPdf2 = combinepath[1];
            List<String> pdf = new List<string>();
            pdf.Add(PathForPdf1);
            pdf.Add(PathForPdf2);
            byte[] mergedPdf = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    using (PdfCopy copy = new PdfCopy(document, ms))
                    {
                        document.Open();

                        for (int i = 0; i < pdf.Count; ++i)
                        {
                            PdfReader reader = new PdfReader(pdf[i]);
                            // loop over the pages in that document
                            int n = reader.NumberOfPages;
                            for (int page = 0; page < n; )
                            {
                                copy.AddPage(copy.GetImportedPage(reader, ++page));
                            }
                        }
                    }
                }
                mergedPdf = ms.ToArray();
                NewPathWithTeacher = mergedGeneratedFile;
                string pathh = "";
                string Teachername = "No Teacher";
                if (isCheckedVal == true)
                {
                    if (StudentTeacher != null && StudentTeacher != "")
                        Teachername = StudentTeacher;
                    Teachername = Teachername.Replace("\\", "");
                    Teachername = clsDashBoard.SanitizeFileName(Teachername);
                    if (!Directory.Exists(NewPathWithTeacher + "//" + Teachername))
                    {
                        Directory.CreateDirectory(NewPathWithTeacher + "//" + Teachername);
                    }
                    mergedGeneratedFile = mergedGeneratedFile + Teachername + "//" + studentFullNameForFile + String.Format("{0:000}", mergePdfFileSuffix) + extensionOfFile;
                    pathh = mergedGeneratedFile;
                }
                else
                {
                    mergedGeneratedFile = mergedGeneratedFile + studentFullNameForFile + String.Format("{0:000}", mergePdfFileSuffix) + extensionOfFile;
                    pathh = mergedGeneratedFile;
                }

                mergePdfFileSuffix = mergePdfFileSuffix + 001;

                System.IO.File.WriteAllBytes(pathh, mergedPdf);
            }
            // Need to delete the two pdf
            for (int i = 0; i < combinepath.Length; i++)
            {
                if (File.Exists(combinepath[i]))
                {
                    File.Delete(combinepath[i]);
                }
            }

            if ((FlagVall * 2) == iFlag)
            {
                mergePdfFileSuffix = 001;
                iFlag = 0;
            }
        }
        #endregion
    }
}
