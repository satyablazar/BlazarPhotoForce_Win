using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.IO;

namespace PhotoForce.App_Code
{
    public static class clsVersion
    {
        public static void VersionUpdate(PhotoSorterDBModelDataContext db, string dbVersion, string assemblyVersion, int opcode)
        {
            int dbMajorVersion = Convert.ToInt32(dbVersion.ToString().Split('.')[0]);
            int dbMinorVersion = Convert.ToInt32(dbVersion.ToString().Split('.')[1]);

            #region Initialization
            bool isError = false;
            string deleteViewOrder = "IF OBJECT_ID ('dbo.View_Order', 'V') IS NOT NULL DROP VIEW dbo.View_Order";

            string orderView = "CREATE VIEW View_Order " +
                               "AS SELECT " +
                    "OrderView.StudentImageId, OrderView.OrderId, OrderView.ImageName, OrderView.SchoolName, OrderView.PhotoShootID, OrderView.Ticketcode, s.Teacher, s.Grade, " +
                         "OrderView.ImageNumber, OrderView.Packages, OrderView.Package, s.StudentID, s.Custom1, OrderView.StudentPhotoOrderId, OrderView.sp_SimplePhotoBillingCode, " +
                         "OrderView.LabCost, OrderView.VenueName, OrderView.sp_JobNumber, OrderView.GroupImageId, OrderView.Fulfilled, OrderView.SimplePhotoOrderId, " +
                         "OrderView.Cust_FirstName, OrderView.Cust_LastName, OrderView.VendorDate, OrderView.sp_ItemDescription, OrderView.sp_GroupName, OrderView.sp_Name, " +
                         "OrderView.sp_Password, OrderView.sp_PriceListName, OrderView.Comments, OrderView.sp_ProductType,OrderView.crop_dimensions, OrderView.crop_orientation, " +
                         "OrderView.crop_coordinates, OrderView.VendorOrderNo, OrderView.Ship_FirstName, " +
                         "OrderView.Ship_LastName, OrderView.Ship_Address, OrderView.Ship_City, OrderView.Ship_State, OrderView.Ship_PostalCode, OrderView.Ship_Country, " +
                         "OrderView.Ship_Phone, OrderView.Cust_Email " +
                        "FROM dbo.Student AS s RIGHT OUTER JOIN " +
                             "(SELECT scl.SchoolName, si.ImageName, si.ImageNumber, si.Packages, si.Ticketcode, si.PhotoShootID, si.StudentIDPK, spo.OrderId, spo.GroupImageId, " +
                                                         "spo.sp_SimplePhotoBillingCode, spo.Id AS StudentPhotoOrderId, spo.StudentImageId, spo.sp_ItemDescription, spo.sp_GroupName, spo.sp_Name, " +
                                                         "spo.Comments, spo.sp_Password, spo.sp_PriceListName, spo.VenueName, spo.LabCost, spo.sp_JobNumber, spo.sp_ProductType, Ord.Fulfilled, " +
                                                         "spo.crop_dimensions, spo.crop_orientation, spo.crop_coordinates,Ord.VendorDate, Ord.VendorOrderNo, Ord.Cust_FirstName, Ord.Cust_LastName, Ord.Ship_FirstName, Ord.Ship_LastName, Ord.SimplePhotoOrderId," +
                                                         "Ord.Ship_Address, Ord.Ship_City, Ord.Ship_State, Ord.Ship_PostalCode, Ord.Ship_Country, Ord.Ship_Phone, Ord.Cust_Email, " +
                                                         "{ fn CONCAT({ fn CONCAT(op.Item, '-') }, ISNULL(CAST(spo.Quantity AS Varchar), '')) } AS Package " +
                               "FROM dbo.StudentImage AS si RIGHT OUTER JOIN " +
                                                         "dbo.StudentPhotoOrder AS spo ON si.ID = spo.StudentImageId OR spo.OrderId <> NULL LEFT OUTER JOIN " +
                                                         "dbo.School AS scl ON scl.ID = si.SchoolID INNER JOIN " +
                                                         "dbo.Orders AS Ord ON Ord.Id = spo.OrderId LEFT OUTER JOIN " +
                                                         "dbo.OrderPackages AS op ON op.SimplePhotoItemId = spo.sp_SimplePhotoBillingCode) AS OrderView ON s.ID = OrderView.StudentIDPK";

            //            string orderItemsView = "CREATE VIEW View_OrderItems "
            //+ "AS SELECT "
            //+ "OrderItemsView.StudentImageId, OrderItemsView.ImageName, OrderItemsView.SchoolName, OrderItemsView.FirstName, s.Lastname, s.Teacher, s.Grade, s.DOB," +
            //                         "s.Emailaddress, s.Password, OrderItemsView.ImageNumber, OrderItemsView.Admincd, OrderItemsView.yearbook, OrderItemsView.Packages, " +
            //                         "OrderItemsView.StudentPhotoOrderId, OrderItemsView.sp_SimplePhotoBillingCode, OrderItemsView.OrderId, OrderItemsView.Rating, s.Custom1, s.Custom2, " +
            //                         "s.Custom3, s.Custom4, s.Custom5, OrderItemsView.GroupImageId, OrderItemsView.ExportDate, OrderItemsView.Folder, OrderItemsView.PS4User"
            //                         + " FROM dbo.Student AS s RIGHT OUTER JOIN "
            //                             + "(SELECT scl.SchoolName, si.ImageName, si.ImageNumber, si.Packages, si.yearbook, si.Admincd, si.FirstName, si.PhotoShootID, si.Rating, si.StudentIDPK," +
            //                                                         "spo.OrderId, spo.GroupImageId, spo.sp_SimplePhotoBillingCode, oeb.ExportDate, oeb.Folder, oeb.PS4User, spo.StudentImageId, " +
            //                                                         "spo.Id AS StudentPhotoOrderId " +
            //                               "FROM dbo.StudentImage AS si RIGHT OUTER JOIN " +
            //                                                         "dbo.StudentPhotoOrder AS spo ON si.ID = spo.StudentImageId OR spo.OrderId <> NULL LEFT OUTER JOIN " +
            //                                                         "dbo.School AS scl ON scl.ID = si.SchoolID LEFT OUTER JOIN " +
            //                                                         "dbo.OrderExportItem AS oei ON oei.OrderItemId = spo.Id LEFT OUTER JOIN " +
            //                                                         "dbo.OrderExportBatch AS oeb ON oeb.Id = oei.ExportBatchId) AS OrderItemsView ON s.ID = OrderItemsView.StudentIDPK";

            string subOrderView = "IF EXISTS(select * FROM sys.views where name = 'View_Order') select top 1 count(ID) from Student ";

            string deleteSeasonView = "IF OBJECT_ID ('dbo.View_StudentSeasonImage', 'V') IS NOT NULL DROP VIEW dbo.View_StudentSeasonImage";
            string deleteStudentSchoolView = "IF OBJECT_ID ('dbo.View_StudentSchool', 'V') IS NOT NULL DROP VIEW dbo.View_StudentSchool";

            string seasonImageView = "CREATE VIEW View_StudentSeasonImage AS SELECT dbo.School.SchoolName, dbo.PhotographyJob.JobName AS Season, dbo.PhotoShoot.PhotoShotName AS PhotoshootName, dbo.StudentImage.ImageName, " +
                         "dbo.Student.FirstName, dbo.Student.Lastname, dbo.Student.ID AS StudentID, dbo.PhotographyJob.ID AS SeasonID, dbo.School.ID AS SchoolID, " +
                         "dbo.PhotoShoot.PhotoShotID AS PhotoshootID, dbo.PhotoShoot.PhotoShotDate AS PhotoShootDate, dbo.StudentImage.ID AS StudentImageID," +
                         "dbo.StudentImage.yearbook, dbo.StudentImage.Rating, dbo.StudentImage.Admincd, dbo.Student.StudentImportID, dbo.Student.Teacher, " +
                         "dbo.Student.StudentID AS SchoolStudentIdentifier, dbo.Student.Password, dbo.Student.Grade, dbo.Student.RecordStatus " +
                            " FROM dbo.PhotographyJob INNER JOIN" +
                            " dbo.PhotoShoot ON dbo.PhotographyJob.ID = dbo.PhotoShoot.PhotographyjobID INNER JOIN" +
                            " dbo.StudentImage ON dbo.PhotoShoot.PhotoShotID = dbo.StudentImage.PhotoShootID INNER JOIN" +
                            " dbo.Student ON dbo.StudentImage.StudentIDPK = dbo.Student.ID INNER JOIN" +
                            " dbo.School ON dbo.PhotographyJob.SchoolID = dbo.School.ID";

            string studentSchoolView = "CREATE VIEW View_StudentSchool AS SELECT dbo.StudentImport.SchoolID, dbo.School.SchoolName, dbo.Student.ID, dbo.Student.StudentImportID, dbo.Student.FirstName, dbo.Student.Lastname, " +
                         "dbo.Student.StudentID, dbo.Student.Password, dbo.Student.Teacher, dbo.Student.Grade, dbo.Student.Custom1, dbo.Student.Custom2, dbo.Student.Custom3, " +
                         "dbo.Student.Custom4, dbo.Student.Custom5, dbo.Student.CreatedOn, dbo.Student.RecordStatus, dbo.Student.DOB, dbo.Student.Address, " +
                         "dbo.Student.City, dbo.Student.State, dbo.Student.Zip, dbo.Student.Phone, dbo.Student.Emailaddress " +
                         "FROM dbo.Student INNER JOIN " +
                                "dbo.StudentImport ON dbo.Student.StudentImportID = dbo.StudentImport.ID INNER JOIN " +
                                "dbo.School ON dbo.StudentImport.SchoolID = dbo.School.ID";

            bool isItFromPrevious = false;
            #endregion

            //if (dbVersion.Split('.')[1] != "00")    //somthing like 4.00
            //{
            if (dbMajorVersion == 3)
            {
                # region Version 3.45
                if (dbVersion == "3.45")
                {
                    try
                    {
                        string script1 = "Update PhotographyJob set JobYear=[JobYear] + '-' + cast(cast([JobYear] as int)+1 as varchar) where JobYear not like '%-%';";
                        script1 += "alter table studentimage add OriginalImageName varchar(50) null, failedRename bit null;";
                        string script2 = " create procedure [sp_insertorupdatetblversion] (@opcode int, @version decimal(20,2)) AS " +
                             " BEGIN if(@opcode=0) insert into tblVersion (Version) values (@version) else update tblVersion set Version=@version END;";
                        string script3 = "update StudentImage set OriginalImageName=ImageName,failedRename=0 where failedRename IS NULL;";

                        db.ExecuteCommand(script1);
                        db.ExecuteCommand(script2);
                        db.ExecuteCommand(script3);

                        db.SubmitChanges();

                        //string updateVersion = "update tblVersion set PSVersion='" + assemblyVersion + "'";
                        //db.ExecuteCommand(updateVersion);
                    }
                    catch (Exception ex)
                    {
                        isError = true;
                        clsStatic.WriteExceptionLogXML(ex);
                    }
                }

                # endregion

                #region Version 3.56

                try
                {
                    if (Convert.ToDecimal(dbVersion) <= Convert.ToDecimal("3.56") || dbMinorVersion == 0)
                    {
                        string alterStudentScript = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '[dbo].[student]' AND COLUMN_NAME = 'PhotographyJobID') BEGIN alter table student ADD PhotographyJobID INTEGER,CONSTRAINT FK_PhotographyJob_ID FOREIGN KEY (PhotographyJobID) REFERENCES photographyjob (ID) END;";
                        string EndYearScript = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '[dbo].[PhotographyJob]' AND COLUMN_NAME = 'EndYear') BEGIN ALTER TABLE [dbo].[PhotographyJob] Add EndYear Varchar(10) END;";
                        string StartYearScript = " IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '[dbo].[PhotographyJob]' AND COLUMN_NAME = 'StartYear') BEGIN ALTER TABLE [dbo].[PhotographyJob] ADD StartYear Varchar(10) END;";
                        string JobYearScript = "IF exists( select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='PhotographyJob' and COLUMN_NAME='JobYear') ALTER TABLE PhotographyJob drop COLUMN JobYear";

                        db.ExecuteCommand(alterStudentScript);
                        db.ExecuteCommand(StartYearScript);
                        db.ExecuteCommand(EndYearScript);
                        db.ExecuteCommand(JobYearScript);
                        db.SubmitChanges();

                        //string updateVersion = "update tblVersion set PSVersion='" + assemblyVersion + "'";
                        //db.ExecuteCommand(updateVersion);
                    }

                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }

                #endregion

                #region Version 3.61
                try
                {
                    if (dbMinorVersion <= 61 || dbMinorVersion == 0)
                    {
                        string updatePhotographyJob = "ALTER TABLE PhotographyJob ALTER COLUMN JobDate DATETIME";

                        db.ExecuteCommand(updatePhotographyJob);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 3.65
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 65 || dbMinorVersion == 0)
                    {
                        string schoolAdminCDRequired = "IF COL_LENGTH ('[dbo].[school]','YearBookRequired') IS NULL BEGIN ALTER TABLE [dbo].[school] Add YearBookRequired bit null END;";
                        string schoolIDRequired = "IF COL_LENGTH('[dbo].[school]', 'IDRequired') IS NULL BEGIN ALTER TABLE [dbo].[school] Add IDRequired bit null END;";


                        db.ExecuteCommand(schoolAdminCDRequired);
                        db.ExecuteCommand(schoolIDRequired);

                        //string updateVersion = "update tblVersion set PSVersion='" + assemblyVersion + "'";
                        //db.ExecuteCommand(updateVersion);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion
            }
            else if (dbMajorVersion == 4)
            {
                #region Versions Between 4.17
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 17 || dbMinorVersion == 0)
                    {
                        string activityTypeTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ActivityType') " + "CREATE TABLE ActivityType(Id INTEGER IDENTITY(1,1) PRIMARY KEY, Type VarChar(50) null);";
                        string activitiesTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Activities') " + "CREATE TABLE Activities" +
    "(Id INTEGER NOT NULL IDENTITY(1,1) Constraint PK_Activities PRIMARY KEY ,Subject nvarchar(50) NULL,ShortDescription nvarchar(50) NULL,Notes nvarchar(max) null,Status nchar(10) null,"
    + "StudentId int null,PhotoShootId int null,SchoolId int not null,PhotographyJobId int not null,ActivityDate datetime null,Type int not null,UserName int not null,CONSTRAINT FK_Activities_Users FOREIGN KEY (UserName)"
    + "REFERENCES Users(Id),CONSTRAINT FK_Activities_School FOREIGN KEY (SchoolId)REFERENCES School(ID),CONSTRAINT FK_Activities_PhotographyJob FOREIGN KEY (PhotographyJobId)REFERENCES PhotographyJob(ID),CONSTRAINT FK_Activities_ActivityType FOREIGN KEY (Type)"
    + "REFERENCES ActivityType(Id))";

                        db.ExecuteCommand(activityTypeTable);
                        db.ExecuteCommand(activitiesTable);

                        isItFromPrevious = true;
                        //string updateVersion = "update tblVersion set PSVersion='" + assemblyVersion + "'";
                        //db.ExecuteCommand(updateVersion);
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.19
                try
                {
                    int res = 0;
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 21 || dbMinorVersion == 0)
                    {
                        string updateOrdersTable1 = "IF COL_LENGTH ('[dbo].[Orders]','VendorOrderNo') IS NULL BEGIN ALTER TABLE [dbo].[Orders] Add VendorOrderNo varchar(10) null END";
                        string updateOrdersTable2 = "IF COL_LENGTH ('[dbo].[Orders]','VendorDate') IS NULL BEGIN ALTER TABLE [dbo].[Orders] Add VendorDate datetime null END";
                        string updateOrdersTable3 = "IF COL_LENGTH ('[dbo].[Orders]','CreatedOn') IS NULL BEGIN EXEC sp_rename 'dbo.Orders.OrderDate', 'CreatedOn', 'COLUMN' END";
                        string updateOrdersTable4 = "IF COL_LENGTH ('[dbo].[Orders]','Fulfilled') IS NULL BEGIN EXEC sp_rename 'dbo.Orders.FullFilled', 'Fulfilled', 'COLUMN' END";

                        string ordersTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders') " + "CREATE TABLE Orders(Id INTEGER IDENTITY(1,1) PRIMARY KEY," +
    "VendorOrderNo varchar(10) null," +
    "CreatedOn datetime not null," +
    "VendorDate datetime null,Fulfilled bit null)";

                        string studentPhotosOrderTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'StudentPhotoOrder') " + "CREATE TABLE StudentPhotoOrder" +
    "(Id INTEGER NOT NULL IDENTITY(1,1) Constraint PK_StudentPhotoOrder PRIMARY KEY ,"
    + "StudentImageId int NULL,"
    + "OrderId int null,"
    + "SchoolId int null,"
    + "CONSTRAINT FK_StudentPhotoOrder_StudentImageId FOREIGN KEY (StudentImageId)"
    + "REFERENCES StudentImage(Id),"
    + "CONSTRAINT FK_StudentPhotoOrder_Order FOREIGN KEY (OrderId)"
    + "REFERENCES Orders(Id),"
    + "CONSTRAINT FK_StudentPhotoOrder_School FOREIGN KEY (SchoolId)"
    + "REFERENCES School(ID))";

                        if (dbMinorVersion == 19) { db.ExecuteCommand(ordersTable); }

                        db.ExecuteCommand(updateOrdersTable4);
                        db.ExecuteCommand(updateOrdersTable1); db.ExecuteCommand(updateOrdersTable2);
                        db.ExecuteCommand(updateOrdersTable3);
                        db.ExecuteCommand(studentPhotosOrderTable);

                        if (dbMinorVersion == 21)
                        {
                            res = 1;
                        }
                        else
                        {
                            res = db.ExecuteQuery<int>(subOrderView).FirstOrDefault();
                        }

                        if (res == 0)
                        {
                            db.ExecuteCommand(orderView);
                        }
                        else
                        {
                            db.ExecuteCommand(deleteViewOrder);
                            db.ExecuteCommand(orderView);
                        }

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.24
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 25 || dbMinorVersion == 0)
                    {
                        string updateStudentsTable1 = "IF COL_LENGTH ('[dbo].[Student]','IsStudent') IS NULL BEGIN ALTER TABLE [dbo].[Student] Add IsStudent varchar(10) null END";
                        string updateStudentsTable2 = "IF COL_LENGTH ('[dbo].[Student]','Title') IS NULL BEGIN ALTER TABLE [dbo].[Student] Add Title varchar(10) null END";

                        db.ExecuteCommand(updateStudentsTable1);
                        db.ExecuteCommand(updateStudentsTable2);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.25
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 25 || dbMinorVersion == 0)
                    {
                        string updateStudentPhotoOrdersTable1 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','GroupImageId') IS NULL BEGIN ALTER TABLE [dbo].[StudentPhotoOrder] Add GroupImageId int null END";

                        db.ExecuteCommand(updateStudentPhotoOrdersTable1);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.30
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 31 || dbMinorVersion == 0)
                    {
                        string updateStudentTable = "ALTER TABLE student ALTER COLUMN Title varchar(10)";
                        string res = db.ExecuteQuery<string>("SELECT data_type FROM information_schema.columns WHERE table_name = 'student' and column_name = 'Title'").FirstOrDefault();

                        if (res == "bit")
                        {
                            db.ExecuteCommand(updateStudentTable);
                        }

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.32
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 32 || dbMinorVersion == 0)
                    {
                        string createOrderPackagesTable1 = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderPackages') " + "CREATE TABLE OrderPackages(Id INTEGER IDENTITY(1,1) PRIMARY KEY,"
    + "Item varchar(30) null,"
    + "Package varchar(150) null,"
    + "SimplePhotoItemId varchar(30) null)";

                        string updateOrdersTable5 = "IF COL_LENGTH ('[dbo].[Orders]','SimplePhotoOrderId') IS NULL BEGIN ALTER TABLE dbo.Orders ADD SimplePhotoOrderId int NULL, OrderTotal decimal NULL,"
    + "ShippingCost decimal NULL, Cust_FirstName varchar(50) null,Cust_LastName varchar(50) null,"
    + "Cust_Address varchar(150),Cust_City varchar(10),Cust_State varchar(5),Cust_PostalCode varchar(10),"
    + "Cust_Country varchar(5),Cust_Email varchar(50),Cust_Phone varchar(15),"
    + "Ship_FirstName varchar(50) null,Ship_LastName varchar(50) null,XMLOrder VarChar(MAX) NULL, OrdersLog VarChar(MAX) NULL,"
    + "Ship_Address varchar(150),Ship_City varchar(10),Ship_State varchar(5),Ship_PostalCode varchar(10),"
    + "Ship_Country varchar(5),Ship_Phone varchar(20),Ship_UseBillToShip varchar(5) null END";


                        string updateStudentPhotoOrderTable = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','sp_SimplePhotoBillingCode') IS NULL BEGIN ALTER TABLE dbo.StudentPhotoOrder ADD sp_SimplePhotoBillingCode varchar(10) null,sp_ItemDescription nvarchar(250) null,sp_GroupName varchar(50) null,"
    + "sp_Name varchar(50) null,sp_Password varchar(20) null,sp_PriceListName varchar(50) null END";

                        db.ExecuteCommand(createOrderPackagesTable1);
                        db.ExecuteCommand(updateOrdersTable5);
                        db.ExecuteCommand(updateStudentPhotoOrderTable);

                        //db.ExecuteCommand(deleteViewOrder);
                        //db.ExecuteCommand(orderView);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.34
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 33 || dbMinorVersion == 0)
                    {
                        string updateOrdersTable6 = "IF COL_LENGTH ('[dbo].[Orders]','OrderedFromVendor') IS NULL BEGIN ALTER TABLE dbo.Orders ADD OrderedFromVendor DateTime null END";
                        string updateOrdersTable7 = "IF COL_LENGTH ('[dbo].[Orders]','IsExported') IS NULL BEGIN ALTER TABLE dbo.Orders ADD " +
                        "ExportDate DateTime null,ExportFolder varchar(150) null,IsExported bit null END";
                        string updateOrdersTable8 = "IF COL_LENGTH ('[dbo].[Orders]','isSimplePhotoBillingCodeFilled') IS NULL BEGIN ALTER TABLE dbo.Orders ADD " +
                        "PS4User varchar(20) null,isSimplePhotoBillingCodeFilled bit null END";

                        db.ExecuteCommand(updateOrdersTable6);
                        db.ExecuteCommand(updateOrdersTable7);
                        db.ExecuteCommand(updateOrdersTable8);

                        int res = db.ExecuteQuery<int>(subOrderView).FirstOrDefault();

                        //db.ExecuteCommand(deleteViewOrder);
                        //db.ExecuteCommand(orderView);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.36
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 35 || dbMinorVersion == 0)
                    {
                        string updateOrdersTable8 = "IF COL_LENGTH ('[dbo].[Orders]','Platform') IS NULL BEGIN ALTER TABLE dbo.Orders ADD " +
                        "Platform varchar(50) null END";

                        string updateOrdersTable9 = "ALTER TABLE Orders ALTER COLUMN Ship_City VARCHAR(25)";
                        string updateOrdersTable13 = "ALTER TABLE Orders ALTER COLUMN Ship_State VARCHAR(20)";
                        string updateOrdersTable14 = "ALTER TABLE Orders ALTER COLUMN Ship_Country VARCHAR(50)";

                        string updateOrdersTable10 = "ALTER TABLE Orders ALTER COLUMN Cust_City VARCHAR(25)";
                        string updateOrdersTable11 = "ALTER TABLE Orders ALTER COLUMN Cust_State VARCHAR(20)";
                        string updateOrdersTable12 = "ALTER TABLE Orders ALTER COLUMN Cust_Country VARCHAR(50)";

                        db.ExecuteCommand(updateOrdersTable8); db.ExecuteCommand(updateOrdersTable9);
                        db.ExecuteCommand(updateOrdersTable10); db.ExecuteCommand(updateOrdersTable11);
                        db.ExecuteCommand(updateOrdersTable12); db.ExecuteCommand(updateOrdersTable13);
                        db.ExecuteCommand(updateOrdersTable14);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.37
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 36 || dbMinorVersion == 0)
                    {
                        string updateOrderTable15 = "IF COL_LENGTH ('[dbo].[Orders]','OrderType') IS NULL BEGIN ALTER TABLE dbo.Orders ADD OrderType varchar(50) NULL, Title varchar(150) null,Description varchar(150) null,ProductCode varchar(50) NULL END";
                        string updateOrderDetailsTable2 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','VenueName') IS NULL BEGIN ALTER TABLE dbo.StudentPhotoOrder ADD VenueName varchar(50) NULL, LabCost decimal NULL END";
                        string updateOrderDetailsTable3 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','sp_JobNumber') IS NULL BEGIN ALTER TABLE dbo.StudentPhotoOrder ADD sp_ProductType varchar(50) NULL,sp_JobNumber varchar(50) NULL END";
                        db.ExecuteCommand(updateOrderTable15); db.ExecuteCommand(updateOrderDetailsTable2);
                        db.ExecuteCommand(updateOrderDetailsTable3);

                        //db.ExecuteCommand(deleteViewOrder);
                        //db.ExecuteCommand(orderView);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.38
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 37 || dbMinorVersion == 0)
                    {
                        string updateOrderDetailsTable3 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','sp_JobNumber') IS NULL BEGIN ALTER TABLE dbo.StudentPhotoOrder ADD sp_ProductType varchar(50) NULL,sp_JobNumber varchar(50) NULL END";
                        db.ExecuteCommand(updateOrderDetailsTable3);

                        //db.ExecuteCommand(deleteViewOrder);
                        //db.ExecuteCommand(orderView);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }

                #endregion

                #region Version 4.39
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 38 || dbMinorVersion == 0)
                    {
                        string updateOrderDetailsTable4 = "ALTER TABLE StudentPhotoOrder ALTER COLUMN sp_ItemDescription NVARCHAR(250)";

                        db.ExecuteCommand(updateOrderDetailsTable4);

                        //db.ExecuteCommand(deleteViewOrder);
                        //db.ExecuteCommand(orderView);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.41
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 42 || dbMinorVersion == 0)
                    {
                        string updateOrderDetailsTable7 = "ALTER TABLE PhotographyJob ALTER COLUMN SchoolId int null";
                        string updateOrderDetailsTable5 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','Comments') IS NULL BEGIN ALTER TABLE StudentPhotoOrder ADD Comments NVARCHAR(MAX) NULL END";
                        string updateOrderDetailsTable6 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','Quantity') IS NULL BEGIN ALTER TABLE StudentPhotoOrder ADD Quantity INT NULL END";
                        string updateOrderTable16 = "IF COL_LENGTH ('[dbo].[Orders]','HasMissingImages') IS NULL BEGIN ALTER TABLE Orders ADD HasMissingImages bit NULL END";
                        string updateOrderTable8 = "ALTER TABLE Orders ALTER COLUMN VendorOrderNo varchar(50) NULL";

                        db.ExecuteCommand(updateOrderDetailsTable7);
                        db.ExecuteCommand(updateOrderTable16); db.ExecuteCommand(updateOrderDetailsTable5);
                        db.ExecuteCommand(updateOrderDetailsTable6);
                        db.ExecuteCommand(updateOrderTable8);

                        db.ExecuteCommand(deleteViewOrder);
                        db.ExecuteCommand(orderView);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.49
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 49 || dbMinorVersion == 0)
                    {
                        string updateOrderDetailsTable10 = "ALTER TABLE StudentPhotoOrder ALTER COLUMN SchoolId int null";
                        string updateOrderTable17 = "IF COL_LENGTH ('[dbo].[Orders]','Retouch') IS NULL BEGIN ALTER TABLE Orders ADD Retouch bit NULL END";
                        string updateOrderTable18 = "IF COL_LENGTH ('[dbo].[Orders]','ImportedBy') IS NULL BEGIN ALTER TABLE Orders ADD ImportedBy varchar(20) NULL END";
                        string updateOrderTable19 = "IF COL_LENGTH ('[dbo].[Orders]','ExportedBy') IS NULL BEGIN EXEC sp_rename 'dbo.Orders.PS4User', 'ExportedBy', 'COLUMN' END";

                        db.ExecuteCommand(updateOrderDetailsTable10);
                        db.ExecuteCommand(updateOrderTable17); db.ExecuteCommand(updateOrderTable18);
                        db.ExecuteCommand(updateOrderTable19);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.50
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 50 || dbMinorVersion == 0)
                    {
                        string createOrdersImport = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrdersImport') CREATE TABLE OrdersImport (" +
                                                    "Id INTEGER NOT NULL IDENTITY(1,1) Constraint PK_OrdersImport PRIMARY KEY ," +
                                                    "Description varchar(100) NULL," +
                                                    "Notes text null," +
                                                    "CreatedOn datetime null)";
                        string updateOrderTable20 = "IF COL_LENGTH ('[dbo].[Orders]','OrdersImportId') IS NULL BEGIN ALTER TABLE Orders ADD OrdersImportId int NULL ,CONSTRAINT FK_Orders_OrdersImport FOREIGN KEY (OrdersImportId)" +
                                                    "REFERENCES OrdersImport(Id) END";

                        db.ExecuteCommand(createOrdersImport);
                        db.ExecuteCommand(updateOrderTable20);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.52
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 52 || dbMinorVersion == 0)
                    {
                        string updateOrderDetailsTable11 = "ALTER TABLE StudentPhotoOrder ALTER COLUMN [sp_password] varchar(70) null";
                        string updateOrderDetailsTable12 = "Alter table studentphotoorder alter column sp_GroupName varchar(100) null";
                        string updateOrderDetailsTable13 = "Alter table studentphotoorder alter column sp_Name varchar(80) null";
                        string updateOrderDetailsTable14 = "Alter table studentphotoorder alter column sp_PriceListName varchar(100) null";
                        string updateOrderDetailsTable15 = "Alter table studentphotoorder alter column VenueName varchar(100) null";


                        db.ExecuteCommand(updateOrderDetailsTable11); db.ExecuteCommand(updateOrderDetailsTable12); db.ExecuteCommand(updateOrderDetailsTable13);
                        db.ExecuteCommand(updateOrderDetailsTable14); db.ExecuteCommand(updateOrderDetailsTable15);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.55
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion == 55 || dbMinorVersion == 0)
                    {
                        string updateStudentTable1 = "IF COL_LENGTH ('[dbo].[Student]','SchoolYear') IS NOT NULL ALTER TABLE Student DROP COLUMN SchoolYear";

                        db.ExecuteCommand(deleteSeasonView);
                        db.ExecuteCommand(seasonImageView);

                        db.ExecuteCommand(deleteStudentSchoolView);
                        db.ExecuteCommand(studentSchoolView);


                        db.ExecuteCommand(updateStudentTable1);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.56

                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion == 56 || dbMinorVersion == 0)
                    {
                        string updateStudentImage1 = "ALTER TABLE StudentImage ALTER COLUMN ImageName VARCHAR(150)";
                        string updateStudentImage2 = "ALTER TABLE StudentImage ALTER COLUMN OriginalImageName VARCHAR(150)";

                        db.ExecuteCommand(updateStudentImage1); db.ExecuteCommand(updateStudentImage2);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.65
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion == 65 || dbMinorVersion == 0)
                    {
                        string updateOrderPackages = "IF COL_LENGTH ('[dbo].[OrderPackages]','DefaultPrice') IS NULL ALTER TABLE dbo.OrderPackages ADD DefaultPrice decimal(5,2) NULL,Seniors bit NULL,SortOrder int null;";
                        string insertValuesToSortOrder = "UPDATE OrderPackages SET SortOrder = Id;";
                        string updateSeniorsToFalse = "UPDATE OrderPackages SET Seniors = 0;";

                        db.ExecuteCommand(updateOrderPackages);
                        db.ExecuteCommand(insertValuesToSortOrder);
                        db.ExecuteCommand(updateSeniorsToFalse);


                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.66<>4.70
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 70 || dbMinorVersion == 0)
                    {
                        string updateOrderPackages = "IF COL_LENGTH ('[dbo].[Orders]','PaymentMethod') IS NULL ALTER TABLE dbo.Orders ADD PaymentMethod varchar(50) NULL,HasNotes bit NULL;";

                        string updateOrdersImport = "IF COL_LENGTH ('[dbo].[OrdersImport]','OrderType') IS NULL ALTER TABLE dbo.OrdersImport ADD OrderType varchar(25) NULL,SchoolID int NULL;";

                        db.ExecuteCommand(updateOrderPackages);
                        db.ExecuteCommand(updateOrdersImport);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.76
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 76 || dbMinorVersion == 0)
                    {
                        string createdNextOrder = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NextOrder') Create table NextOrder (Id INTEGER IDENTITY(1,1) PRIMARY KEY ,NextOrderId int null);";
                        int orderId = db.ExecuteQuery<int>("select max(id) from orders").FirstOrDefault();
                        string insertIntoNextOrder = "insert into NextOrder(NextOrderId) values(" + orderId + ")";

                        db.ExecuteCommand(createdNextOrder); db.ExecuteCommand(insertIntoNextOrder);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.77
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 79 || dbMinorVersion == 0)
                    {
                        string updateOrders23 = "IF COL_LENGTH ('[dbo].[Orders]','Variance') IS NULL ALTER TABLE dbo.Orders ADD Variance decimal NULL";

                        db.ExecuteCommand(updateOrders23);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.81
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 82 || dbMinorVersion == 0)
                    {
                        string updateStudent2 = "update student set IsStudent = 'Student' where IsStudent = 'Yes'";
                        string updateStudent3 = "update student set IsStudent = 'Staff' where IsStudent = 'No'";

                        db.ExecuteCommand(updateStudent2);
                        db.ExecuteCommand(updateStudent3);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.82
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 83 || dbMinorVersion == 0)
                    {
                        string updateOrders24 = "UPDATE Orders SET Cust_FirstName = S.FirstName, Cust_LastName = S.Lastname " +
                                       "FROM StudentPhotoOrder spo JOIN Orders O ON O.Id=spo.OrderId JOIN StudentImage si ON si.ID=spo.StudentImageId " +
                                       "Join Student S ON S.ID = si.StudentIDPK " +
                                       "WHERE O.OrderType = 'Manual Orders' and O.Cust_FirstName is null and O.Cust_LastName is null";

                        db.ExecuteCommand(updateOrders24);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.84
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 85 || dbMinorVersion == 0)
                    {
                        string updateSchoolTable1 = "IF COL_LENGTH ('[dbo].[School]','Students') IS NULL alter table School add Students int null,Rating varchar(10) null,IsActive bit null,SchoolYear varchar(100) null ";
                        string updateOrderTable26 = "Alter table orders alter column Description varchar(500) null";
                        db.ExecuteCommand(updateSchoolTable1); db.ExecuteCommand(updateOrderTable26);

                        db.ExecuteCommand(deleteViewOrder);
                        db.ExecuteCommand(orderView);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }

                #endregion

                #region Version 4.85
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 86 || dbMinorVersion == 0)
                    {
                        string updateOrdersTable27 = "Alter table orders alter column [Cust_PostalCode] varchar(30) null";
                        string updateOrdersTable28 = "Alter table orders alter column [Ship_PostalCode] varchar(30) null";
                        string updateOrdersTable29 = "update Orders set OrderType = 'Simple Photo Order' where OrderType ='Shipping'";

                        db.ExecuteCommand(updateOrdersTable27); db.ExecuteCommand(updateOrdersTable28);
                        db.ExecuteCommand(updateOrdersTable29);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }

                #endregion

                #region Version 4.89
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 90 || dbMinorVersion == 0)
                    {
                        string updateSchoolTable3 = "IF COL_LENGTH ('[dbo].[School]','SchoolCampus') IS NOT NULL ALTER TABLE School DROP COLUMN SchoolCampus";
                        string updateSchoolTable4 = "update School set IsActive = 'true' where IsActive is null";
                        db.ExecuteCommand(updateSchoolTable3);
                        db.ExecuteCommand(updateSchoolTable4);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }

                #endregion

                #region Version 4.98
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 98 || dbMinorVersion == 0)
                    {

                        string updateStudentTable4 = "IF COL_LENGTH ('[dbo].[Student]','SchoolCampus') IS NULL ALTER TABLE Student Add SchoolCampus varchar(50) null";


                        string createWorkflowItem = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WorkflowItem') CREATE TABLE WorkflowItem (" +
                                                    "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                    "SortOrder integer not null, " +
                                                    "Description nvarchar(250) NULL, " +
                                                    "Type nvarchar(15) null, " +
                                                    "Assignedto nvarchar(25) null, " +
                                                    "Status nvarchar(50) null)";

                        string createWorkflowCollection = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WorkflowCollection') CREATE TABLE WorkflowCollection (" +
                                                    "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                    "Name varchar(100) null, " +
                                                    "CreatedOn datetime, " +
                                                    "CreatedBy nvarchar(100) null)";

                        string createWorkflowCollectionItems = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WorkflowCollectionItems') CREATE TABLE WorkflowCollectionItems (" +
                                                    "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                    "WorkflowItemId int FOREIGN KEY REFERENCES WorkflowItem(Id), " +
                                                    "WorkflowCollectionId int FOREIGN KEY REFERENCES WorkflowCollection(Id))";

                        string createPhotoshootWorkflowItem = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PhotoshootWorkflowItem') CREATE TABLE PhotoshootWorkflowItem (" +
                                                    "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                    "WorkflowItemId int FOREIGN KEY REFERENCES WorkflowItem(Id), " +
                                                    "PhotoShootID int FOREIGN KEY REFERENCES PhotoShoot(PhotoShotID), " +
                                                    "Notes nvarchar(255) null, " +
                                                    "DueDate datetime, " +
                                                    "CreatedOn datetime, " +
                                                    "CreatedBy nvarchar(100) null, " +
                                                    "Assignedto nvarchar(25) null, " +
                                                    "Status nvarchar(50) null)";

                        db.ExecuteCommand(updateStudentTable4);
                        db.ExecuteCommand(createWorkflowItem);
                        db.ExecuteCommand(createWorkflowCollection);
                        db.ExecuteCommand(createWorkflowCollectionItems);
                        db.ExecuteCommand(createPhotoshootWorkflowItem);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }

                #endregion

                #region Version 4.99
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 99 || dbMinorVersion == 0)
                    {
                        string updateWorkflowCollectionItems1 = "IF COL_LENGTH ('[dbo].[WorkflowCollectionItems]','SortOrder') IS NULL Alter table WorkflowCollectionItems Add SortOrder int null";
                        string updatePhotoshootWorkflowItem1 = "IF COL_LENGTH ('[dbo].[PhotoshootWorkflowItem]','SortOrder') IS NULL Alter table PhotoshootWorkflowItem add SortOrder int null";
                        string updateWorkflowItem1 = "IF COL_LENGTH ('[dbo].[WorkflowItem]','Offset') IS NULL ALTER TABLE [dbo].[WorkflowItem] Add Offset int null , BeforeAfter bit null";

                        db.ExecuteCommand(updateWorkflowCollectionItems1);
                        db.ExecuteCommand(updatePhotoshootWorkflowItem1);
                        db.ExecuteCommand(updateWorkflowItem1);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.105
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 104 || dbMinorVersion == 0)
                    {
                        string updatePhotoshootWorkflowItem2 = "Alter table PhotoshootWorkflowItem Alter Column Notes varchar(max) null";

                        db.ExecuteCommand(updatePhotoshootWorkflowItem2);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.111
                //try
                //{
                //    if (isItFromPrevious == true ? true : dbMinorVersion <= 111 || dbMinorVersion == 0)
                //    {
                //        db.ExecuteCommand(deleteViewOrder);
                //        db.ExecuteCommand(orderView);
                //        isItFromPrevious = true;
                //    }

                //}
                //catch (Exception ex)
                //{
                //    isError = true;
                //    clsStatic.WriteExceptionLogXML(ex);
                //}
                #endregion

                #region Version 4.112
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 112 || dbMinorVersion == 0)
                    {
                        string updateStudentPhotoOrder1 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','crop_cordinates') IS NULL Alter table StudentPhotoOrder Add crop_coordinates varchar(20) null,crop_dimensions varchar(20) null,crop_orientation varchar(10) null ";
                        //string updateStudentPhotoOrder2 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','crop_dimensions') IS NULL Alter table StudentPhotoOrder Add crop_dimensions varchar(20) null";
                        //string updateStudentPhotoOrder3 = "IF COL_LENGTH ('[dbo].[StudentPhotoOrder]','crop_orientation') IS NULL Alter table StudentPhotoOrder Add crop_orientation varchar(10) null";

                        db.ExecuteCommand(updateStudentPhotoOrder1);
                        //db.ExecuteCommand(updateStudentPhotoOrder2);
                        //db.ExecuteCommand(updateStudentPhotoOrder3);

                        db.ExecuteCommand(deleteViewOrder);
                        db.ExecuteCommand(orderView);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.113
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 113 || dbMinorVersion == 0)
                    {
                        string createIQAccounts = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IQAccounts') CREATE TABLE IQAccounts (" +
                                                     "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                     "IQAccountCode varchar(50) null, " +
                                                     "Description nvarchar(250) NULL)";

                        string createIQPriceSheet = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IQPriceSheet') CREATE TABLE IQPriceSheet (" +
                                                    "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                    "IQAccountId int FOREIGN KEY REFERENCES IQAccounts(Id),IQPriceSheetId int null, " +
                                                    "Description nvarchar(250) NULL)";

                        db.ExecuteCommand(createIQAccounts);
                        db.ExecuteCommand(createIQPriceSheet);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.116
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 116 || dbMinorVersion == 0)
                    {
                        string updatePhotoShoot = "IF COL_LENGTH ('[dbo].[PhotoShoot]','IsValidated') IS NULL Alter table PhotoShoot Add IsValidated bit null ";
                        string updateStudentImport = "IF COL_LENGTH ('[dbo].[StudentImport]','IsValidated') IS NULL Alter table StudentImport Add IsValidated bit null ";

                        db.ExecuteCommand(updatePhotoShoot);
                        db.ExecuteCommand(updateStudentImport);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.120
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 119 || dbMinorVersion == 0)
                    {
                        string createIQVandoSettings = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IQVandoSettings') CREATE TABLE IQVandoSettings (" +
                                                       "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                       "IQAccountId int FOREIGN KEY REFERENCES IQAccounts(Id), IQVandoId int null, " +
                                                       "Description nvarchar(250) NULL, IsDefault bit null)";

                        string updatePhotoshootWorkflowItem = "IF COL_LENGTH ('[dbo].[PhotoshootWorkflowItem]','HasNotes') IS NULL Alter table PhotoshootWorkflowItem Add HasNotes bit null ";

                        string updatePhotoshootWorkflowItem1 = "update PhotoshootWorkflowItem set HasNotes = 1 where Notes is not null ";
                        string updatePhotoshootWorkflowItem2 = "update PhotoshootWorkflowItem set HasNotes = 0 where Notes is null or Notes = '' ";

                        db.ExecuteCommand(createIQVandoSettings);

                        db.ExecuteCommand(updatePhotoshootWorkflowItem);
                        db.ExecuteCommand(updatePhotoshootWorkflowItem1);
                        db.ExecuteCommand(updatePhotoshootWorkflowItem2);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.121
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 120 || dbMinorVersion == 0)
                    {
                        string updatePhotoshoot1 = "IF COL_LENGTH ('[dbo].[Photoshoot]','OnCalendar') IS NULL Alter Table Photoshoot add Job# varchar(12) null,OnCalendar bit null,Scheduled bit null ";
                        db.ExecuteCommand(updatePhotoshoot1);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.122
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 122 || dbMinorVersion == 0)
                    {
                        System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME);

                        foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.128
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 127 || dbMinorVersion == 0)
                    {
                        //string updateUsers = "IF COL_LENGTH ('[dbo].[Users]','StudioName') IS NULL Alter Table Users add StudioName varchar(50) null, Address varchar(50) null, City varchar(20) null, Phone varchar(15) null, Zip varchar(10) null, SimplePhotoPhotographerID varchar(15) null ";

                        string createSimplePhotoPriceSheet = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SimplePhotoPriceSheet') CREATE TABLE SimplePhotoPriceSheet ( " +
                                                   "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                   "SPPriceSheetId int null, " +
                                                   "Description nvarchar(250) NULL)";

                        string updateOrderPackages = "IF COL_LENGTH ('[dbo].[OrderPackages]','Weight') IS NULL Alter Table OrderPackages add Weight decimal(5,2) null ";
                        string updateOrders = "IF COL_LENGTH ('[dbo].[Orders]','TrackingNumber') IS NULL Alter Table Orders add TrackingNumber varchar(20) null, isShipped bit null ";

                        //db.ExecuteCommand(updateUsers);
                        db.ExecuteCommand(createSimplePhotoPriceSheet);
                        db.ExecuteCommand(updateOrderPackages);
                        db.ExecuteCommand(updateOrders);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.129
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 128 || dbMinorVersion == 0)
                    {
                        string createStudiosTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Studio') CREATE TABLE Studio ( " +
                                                   "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                   "PFStudioId varchar(20) null, StudioName varchar(50) NULL, PrimaryContact varchar(50) null, " +
                                                   "Email varchar(100) NULL, Address varchar(100) null, City varchar(30) NULL, State varchar(20) null, " +
                                                   "Phone varchar(15) NULL, Zip varchar(10) NULL )";

                        string alterSchools = "IF COL_LENGTH ('[dbo].[School]','StudioId') IS NULL Alter table School Add StudioId int FOREIGN KEY REFERENCES Studio(Id)  ";
                        string insertFreedStudio = "insert into studio(PFStudioId,StudioName,PrimaryContact,Email,Address,City,State,Phone,Zip) values ('1','FreedPhoto','Neal Freed','neal@freedphoto.com','4931 Cordell Avenue','Bethesda','MD','3016525452','20814')";
                        string updateSchools = "update School set StudioId = 1";
                        string removeColsFromUsers = "IF COL_LENGTH ('[dbo].[Users]','StudioName') IS NOT NULL ALTER TABLE Users DROP COLUMN StudioName, Address,City,Phone,Zip,SimplePhotoPhotographerID";

                        db.ExecuteCommand(createStudiosTable);
                        db.ExecuteCommand(alterSchools);
                        db.ExecuteCommand(insertFreedStudio);
                        db.ExecuteCommand(updateSchools);
                        db.ExecuteCommand(removeColsFromUsers);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.130
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 129 || dbMinorVersion == 0)
                    {
                        string createSimplePhotoExportBatchTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SimplePhotoExportBatch') CREATE TABLE SimplePhotoExportBatch ( " +
                                                                   "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, ExportType varchar(10) null, ExportPath text NULL, CreatedOn datetime not null, " +
                                                                   "CreatedBy varchar(20) null,GroupsExported varchar(20) null, NoOfImages int null )";

                        db.ExecuteCommand(createSimplePhotoExportBatchTable);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.131
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 131 || dbMinorVersion == 0)
                    {
                        string alterOrdersTable = "alter table orders alter column trackingnumber varchar(50) ";
                        db.ExecuteCommand(alterOrdersTable);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.135
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 134 || dbMinorVersion == 0)
                    {
                        string createPhotoshootTypeTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PhotoshootTypeTable') CREATE TABLE PhotoshootTypeTable ( " +
                                                    "Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                                                    "PhotoshootType varchar(50) null)";
                        string updatePhotoshootTypeTable1 = "IF COL_LENGTH ('[dbo].[PhotoShoot]','PhotoshootType') IS NULL Alter table PhotoShoot Add PhotoshootType int FOREIGN KEY REFERENCES PhotoshootTypeTable(Id), Season varchar(10) null ";
                        //string updatePhotoShootTable12 = "IF COL_LENGTH ('[dbo].[PhotoShoot]','Season') IS NULL Alter Table PhotoShoot add Season varchar(10) null";
                        string updatePhotoshootWorkflowItems = "IF COL_LENGTH ('[dbo].[PhotoshootWorkflowItem]','CompletedBy') IS NULL Alter Table PhotoshootWorkflowItem add CompletedBy datetime null, CompletedOn datetime null";

                        string updateSchool = "IF COL_LENGTH ('[dbo].[School]','IDCards') IS NULL Alter table School Add IDCards int null,Calendars int null,  Pencils int null" ;
                        string updatePhotoShootWorkflowItem22 = "update PhotoshootWorkflowItem set Status = 'Completed' where Status = 'Done'";



                        db.ExecuteCommand(createPhotoshootTypeTable);
                        db.ExecuteCommand(updatePhotoshootTypeTable1);
                       // db.ExecuteCommand(updatePhotoShootTable12);
                        db.ExecuteCommand(updatePhotoshootWorkflowItems);
                        db.ExecuteCommand(updateSchool);
                        db.ExecuteCommand(updatePhotoShootWorkflowItem22);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.136
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 135 || dbMinorVersion == 0)
                    {

                        string updateSchool22 = "IF COL_LENGTH ('[dbo].[School]','ContractExpiration') IS NULL Alter table School Add ContractExpiration datetime null";

                        db.ExecuteCommand(updateSchool22);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.140
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 139 || dbMinorVersion == 0)
                    {

                        string updatePhotoshootWorkflowItem = "IF COL_LENGTH ('[dbo].[PhotoshootWorkflowItem]','CompletedBy') IS NOT NULL ALTER TABLE PhotoshootWorkflowItem ALTER COLUMN CompletedBy nvarchar(25)";

                        db.ExecuteCommand(updatePhotoshootWorkflowItem);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.145
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 144 || dbMinorVersion == 0)
                    {
                        string createItemClassTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ItemClassType') CREATE TABLE ItemClassType ( Id INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY, ClassType varchar(15) null)";

                        string updateItemClassType1 = "Insert into ItemClassType(ClassType) values ('Workflow')";
                        string updateItemClassType2 = "Insert into ItemClassType(ClassType) values ('Equipment')";

                        string updateWorkflowItem12 = "IF COL_LENGTH ('[dbo].[WorkflowItem]','ItemClassTypeId') IS NULL Alter table WorkflowItem Add ItemClassTypeId int FOREIGN KEY REFERENCES ItemClassType(Id), Notes varchar(max) null";
                        string updateWorkflowCollection12 = "IF COL_LENGTH ('[dbo].[WorkflowCollection]','ItemClassTypeId') IS NULL Alter table WorkflowCollection Add ItemClassTypeId int FOREIGN KEY REFERENCES ItemClassType(Id)";

                        string updateWorkflowItem13 = "UPDATE WorkflowItem SET ItemClassTypeId = 1 WHERE Id in (select Id from WorkflowItem)";
                        string updateWorkflowCollection13 = "UPDATE WorkflowCollection SET ItemClassTypeId = 1 WHERE Id in (select Id from WorkflowCollection)";

                        
                        
                        db.ExecuteCommand(createItemClassTable);
                        db.ExecuteCommand(updateItemClassType1);
                        db.ExecuteCommand(updateItemClassType2);  

                        db.ExecuteCommand(updateWorkflowItem12);
                        db.ExecuteCommand(updateWorkflowCollection12);
                        db.ExecuteCommand(updateWorkflowItem13);
                        db.ExecuteCommand(updateWorkflowCollection13);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.146
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 145 || dbMinorVersion == 0)
                    {
                        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;
                        if (File.Exists(appDataPath + "\\" + "DockDashBoardPhotoShootViewLayout.xml"))
                        {
                            File.Delete(appDataPath + "\\" + "DockDashBoardPhotoShootViewLayout.xml");
                        }
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.152
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 151 || dbMinorVersion == 0)
                    {

                        string alterSchools10 = "IF COL_LENGTH ('[dbo].[School]','Contact1Name') IS NULL ALTER TABLE School Add Contact1Name nvarchar(15) null, " +
                                                "Contact1Type nvarchar(12) null, Contact1Email nvarchar(30) null, Contact1Notes varchar(max) null, " +
                                                "Contact2Name nvarchar(15) null, Contact2Type nvarchar(12) null, Contact2Email nvarchar(30) null, Contact2Notes varchar(max) null, " +
                                                "Contact3Name nvarchar(15) null, Contact3Type nvarchar(12) null, Contact3Email nvarchar(30) null, Contact3Notes varchar(max) null";

                        db.ExecuteCommand(alterSchools10);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.156
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 155 || dbMinorVersion == 0)
                    {
                        string updateWorkflowItem10 = "IF COL_LENGTH ('[dbo].[WorkflowItem]','Quantity') IS NULL ALTER TABLE WorkflowItem Add Quantity int null";
                        string updateWorkflowItem11 = "UPDATE WorkflowItem SET Quantity=SortOrder WHERE ItemClassTypeId = 2";
                        string updateWorkflowItem12 = "UPDATE WorkflowItem SET SortOrder=0 WHERE ItemClassTypeId = 2";

                        string updateWorkflowCollectionItems10 = "IF COL_LENGTH ('[dbo].[WorkflowCollectionItems]','Quantity') IS NULL ALTER TABLE WorkflowCollectionItems Add Quantity int null";
                        string updateWorkflowCollectionItems11 = "UPDATE WorkflowCollectionItems SET Quantity=SortOrder WHERE WorkflowItemId in (select Id from WorkflowItem where ItemClassTypeId = 2)";
                        string updateWorkflowCollectionItems12 = "UPDATE WorkflowCollectionItems SET SortOrder=0 WHERE WorkflowItemId in (select Id from WorkflowItem where ItemClassTypeId = 2)";

                        string updatePhotoShootWorkflowItems10 = "IF COL_LENGTH ('[dbo].[PhotoshootWorkflowItem]','Quantity') IS NULL ALTER TABLE PhotoshootWorkflowItem Add Quantity int null";
                        string updatePhotoShootWorkflowItems11 = "UPDATE PhotoshootWorkflowItem SET Quantity=SortOrder WHERE WorkflowItemId in (select Id from WorkflowItem where ItemClassTypeId = 2)";
                        string updatePhotoShootWorkflowItems12 = "UPDATE PhotoshootWorkflowItem SET SortOrder=0 WHERE WorkflowItemId in (select Id from WorkflowItem where ItemClassTypeId = 2)";

                        db.ExecuteCommand(updateWorkflowItem10);
                        db.ExecuteCommand(updateWorkflowItem11);
                        db.ExecuteCommand(updateWorkflowItem12);
                        db.ExecuteCommand(updateWorkflowCollectionItems10);
                        db.ExecuteCommand(updateWorkflowCollectionItems11);
                        db.ExecuteCommand(updateWorkflowCollectionItems12);
                        db.ExecuteCommand(updatePhotoShootWorkflowItems10);
                        db.ExecuteCommand(updatePhotoShootWorkflowItems11);
                        db.ExecuteCommand(updatePhotoShootWorkflowItems12);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.159
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 158 || dbMinorVersion == 0)
                    {
                        string updateOrders12 = "IF COL_LENGTH ('[dbo].[Orders]','Studio') IS NULL ALTER TABLE Orders Add Studio Varchar(20) null";

                        db.ExecuteCommand(updateOrders12);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.163
                //Commented by hema 
                //BCZ Not getting correct data 
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 162 || dbMinorVersion == 0)
                    {
                        //string updateOrdersImport11 = "IF COL_LENGTH ('[dbo].[OrdersImport]','[School_Id]') IS NULL Alter table OrdersImport Add [School_Id] int FOREIGN KEY REFERENCES School(Id) ";
                        //string updateOrdersImport12 = "update OrdersImport set SchoolID = 0 where SchoolID is null ";
                        //string updateOrdersImport13 = "update OrdersImport set School_Id = SchoolID where id in (select Id from OrdersImport where SchoolID != 0 ) and SchoolID != 0 ";
                        //string updateOrdersImport14 = "IF COL_LENGTH ('[dbo].[OrdersImport]','SchoolID') IS NOT NULL ALTER TABLE OrdersImport DROP COLUMN SchoolID";
                        //string updateOrdersImport15 = "exec  sp_rename 'OrdersImport.School_Id', 'SchoolID', 'Column' ";

                        //db.ExecuteCommand(updateOrdersImport11);
                        //db.ExecuteCommand(updateOrdersImport12);
                        //db.ExecuteCommand(updateOrdersImport13);
                        //db.ExecuteCommand(updateOrdersImport14);
                        //db.ExecuteCommand(updateOrdersImport15);

                        string updateStudents12 = "IF COL_LENGTH ('[dbo].[Student]','OfficialFirstName') IS NULL ALTER TABLE Student Add OfficialFirstName Varchar(50) null, OfficialLastName Varchar(50) null";

                        db.ExecuteCommand(updateStudents12);

                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.167
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 166 || dbMinorVersion == 0)
                    {
                        string updateSchool25 = "IF COL_LENGTH ('[dbo].[School]','Visit') IS NULL ALTER TABLE School Add Visit Varchar(50) null";
                        string updateSchool26 = "Update School set Visit = 'www.freedpics.com' where ID in " +
                                                "(select distinct(sc.ID) from School sc join Studio sti on sti.Id = sc.StudioId where StudioName = 'FreedPhoto')";

                        db.ExecuteCommand(updateSchool25);
                        db.ExecuteCommand(updateSchool26);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.168
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 167 || dbMinorVersion == 0)
                    {
                        string updateOrders25 = "IF COL_LENGTH ('[dbo].[Orders]','IsStandardRetouch') IS NULL ALTER TABLE Orders Add IsStandardRetouch bit null";
                        string updateOrders26 = "Update Orders set IsStandardRetouch = 1 where Id in (select Distinct OrderId from StudentPhotoOrder where (sp_SimplePhotoBillingCode =  'M101' or sp_SimplePhotoBillingCode =  'M102' or sp_SimplePhotoBillingCode =  'M127' or sp_SimplePhotoBillingCode =  'M128' " +
                                                    "or sp_SimplePhotoBillingCode =  'S138' or sp_SimplePhotoBillingCode =  'S139') and OrderId in ( " +
                                                    "select Distinct(OrderId) from StudentPhotoOrder where sp_SimplePhotoBillingCode Like 'M134' and  OrderId is not null ))";

                        db.ExecuteCommand(updateOrders25);
                        db.ExecuteCommand(updateOrders26);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.169
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 168 || dbMinorVersion == 0)
                    {
                        string updateSchools26 = "IF COL_LENGTH ('[dbo].[School]','Contact1Name') IS NOT NULL ALTER TABLE School ALTER COLUMN Contact1Name nvarchar(30)";
                        string updateSchools27 = "IF COL_LENGTH ('[dbo].[School]','Contact1Email') IS NOT NULL ALTER TABLE School ALTER COLUMN Contact1Email nvarchar(50)";

                        string updateSchools28 = "IF COL_LENGTH ('[dbo].[School]','Contact2Name') IS NOT NULL ALTER TABLE School ALTER COLUMN Contact2Name nvarchar(30)";
                        string updateSchools29 = "IF COL_LENGTH ('[dbo].[School]','Contact2Email') IS NOT NULL ALTER TABLE School ALTER COLUMN Contact2Email nvarchar(50)";

                        string updateSchools30 = "IF COL_LENGTH ('[dbo].[School]','Contact3Name') IS NOT NULL ALTER TABLE School ALTER COLUMN Contact3Name nvarchar(30)";
                        string updateSchools31 = "IF COL_LENGTH ('[dbo].[School]','Contact3Email') IS NOT NULL ALTER TABLE School ALTER COLUMN Contact3Email nvarchar(50)";

                        db.ExecuteCommand(updateSchools26); db.ExecuteCommand(updateSchools27);
                        db.ExecuteCommand(updateSchools28); db.ExecuteCommand(updateSchools29);
                        db.ExecuteCommand(updateSchools30); db.ExecuteCommand(updateSchools31);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.172
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 171 || dbMinorVersion == 0)
                    {
                        string updateSchools32 = "If COL_LENGTH('School','Affiliation') IS NULL ALTER TABLE School ADD Affiliation varchar(100) null";
                        string updateOrders33 = "If COL_LENGTH('Orders','GotPhotoOrderKey') IS NULL ALTER TABLE Orders ADD GotPhotoOrderKey varchar(50) null";

                        db.ExecuteCommand(updateSchools32); 
                        db.ExecuteCommand(updateOrders33);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion

                #region Version 4.176
                try
                {
                    if (isItFromPrevious == true ? true : dbMinorVersion <= 175 || dbMinorVersion == 0)
                    {
                        string updateStudentPhotoOrder32 = "If COL_LENGTH('StudentPhotoOrder','gp_FileName') IS NULL ALTER TABLE StudentPhotoOrder ADD gp_FileName varchar(100) null";
                        string updateStudentPhotoOrder33 = "If COL_LENGTH('StudentPhotoOrder','gp_FileName2') IS NULL ALTER TABLE StudentPhotoOrder ADD gp_FileName2 varchar(100) null";

                        db.ExecuteCommand(updateStudentPhotoOrder32);
                        db.ExecuteCommand(updateStudentPhotoOrder33);
                        isItFromPrevious = true;
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    clsStatic.WriteExceptionLogXML(ex);
                }
                #endregion
            }
            #region update if we don't have any scripts to run
            try
            {
                string updateVersion = "update tblVersion set PSVersion='" + assemblyVersion + "'";
                db.ExecuteCommand(updateVersion);

                if (isError)
                {
                    System.Windows.MessageBox.Show("While applying scripts ,errors has occured . Please check log file for more details.");
                }
                else
                {
                    System.Windows.MessageBox.Show("DB is updated.");
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
            #endregion
            
            //}
            //else
            //{
            //    #region Version update
            //    try
            //    {
            //        //add code here
            //        string updateVersion = "update tblVersion set PSVersion='" + assemblyVersion + "'";
            //        db.ExecuteCommand(updateVersion);
            //    }
            //    catch (Exception ex)
            //    {
            //        clsStatic.WriteExceptionLogXML(ex);
            //    }
            //    #endregion
            //}
        }

        public static void neededUpdate(PhotoSorterDBModelDataContext db, string tempConnectionString)
        {
            string connectionString = tempConnectionString;

            #region Must Needed Updated for > 4 versions

            #region Before 4.00
            try
            {
                string hasClassPhotoInGroups = "IF COL_LENGTH('[dbo].[group]', 'hasClassPhoto') IS NULL BEGIN ALTER TABLE [dbo].[group] Add hasClassPhoto bit null END;";
                string RenameSourceImagesInPhotoShoot = "IF COL_LENGTH('[dbo].[PhotoShoot]', 'RenameSourceImages') IS NULL BEGIN ALTER TABLE [dbo].[PhotoShoot] Add RenameSourceImages bit null END;";
                db.ExecuteCommand(hasClassPhotoInGroups);
                db.ExecuteCommand(RenameSourceImagesInPhotoShoot);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }

            #endregion

            #region 4.5
            try
            {
                string versionColumnUpdate = "IF exists( select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='tblVersion' and COLUMN_NAME='PSVersion' and DATA_TYPE='decimal') ALTER TABLE tblVersion ALTER COLUMN PSVersion Varchar(10)";
                db.ExecuteCommand(versionColumnUpdate);
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }

            #endregion

            #region 4.26
            //Clear Rating
            db.ExecuteCommand(" update StudentImage set Rating = null where Rating = 'Clear' ");
            #endregion

            #region 4.12
            try
            {
                db.ExecuteCommand("select * from Users where 1 = 0");
            }
            catch (Exception)
            {
                string userTable = "IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users') " + "CREATE TABLE Users(Id INTEGER IDENTITY(1,1) PRIMARY KEY, UserName VarChar(20) null);";
                db.ExecuteCommand(userTable);

                callLoginWindow(connectionString);
            }

            callLoginWindow(connectionString);

            #endregion

            #endregion
        }

        public static void callLoginWindow(string connectionString)
        {
            //Login Window
            PhotoForce.WorkPlace.Views.Login _objLogin = new WorkPlace.Views.Login(connectionString);
            _objLogin.ShowDialog();

            if (((PhotoForce.WorkPlace.LoginViewModel)(_objLogin.DataContext)).isLogin == false)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }
        }
    }
}
