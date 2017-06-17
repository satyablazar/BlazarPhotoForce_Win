using PhotoForce.Extensions;
using PhotoForce.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.App_Code
{
    public static class clsDashBoard
    {
        public static string convertArrayListToString(ArrayList Ids)
        {
            string res = "";
            if (Ids.Count != 0)
            {
                res = string.Join(",", Ids.Cast<int>().ToArray());
            }
            return res;
        }

        public static List<int> getAllJobs(int schoolID, PhotoSorterDBModelDataContext db)
        {
            List<int> obj = db.ExecuteQuery<int>("select ID from PhotographyJob where SchoolID = " + schoolID + "").ToList();
            return obj;
        }
        public static List<PhotoShoot> getPhotoShoot(PhotoSorterDBModelDataContext db, string jobid)        //changed to list<T> from ienumerable<T>    //mohan
        {
            return db.ExecuteQuery<PhotoShoot>(
                "select * from PhotoShoot where photographyjobid in (" + jobid + ")"
                ).ToList();
        }
        internal static List<StudentImage> getStudentImageDetailsBySchool(PhotoSorterDBModelDataContext db, int studentId, int schoolId)
        {
            //return (db.ExecuteQuery<StudentImage>("select si.ID,SchoolID,PhotoShootID,ImageName,ImageNumber,s.FirstName,s.Lastname,s.StudentID ,s.Teacher,s.Grade,HomeRoom,s.DOB,s.[Address],s.City,s.[State],s.Zip,s.Phone,si.Custom1,si.Custom2,si.Custom3,si.Custom4,si.Custom5,si.Custom6,si.Custom7,si.Custom8,Packages,admincd,yearbook,Multipose,Ticketcode,s.Emailaddress,s.Password,Rating from StudentImage si join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and si.StudentIDPK = " + studentId + " and si.schoolid=" + schoolId).ToList());

            return (from si in db.StudentImages where si.StudentIDPK == studentId && si.SchoolID == schoolId && si.RecordStatus == "Active" select si).ToList();

        }
        internal static StudentImage getStudentImageDetailsById(PhotoSorterDBModelDataContext db, int ImageId)
        {
            return (from p in db.StudentImages where p.ID == ImageId select p).FirstOrDefault();
        }
        internal static StudentImage getStudentImageDetailsByName(PhotoSorterDBModelDataContext db, string imageName)
        {
            return (from p in db.StudentImages where p.ImageName == imageName orderby p.PhotoShootID descending select p).FirstOrDefault();
        }
        public static int getStudentIdByImageId(PhotoSorterDBModelDataContext db, int ImageId)
        {
            return db.ExecuteQuery<int>(
                "select studentidpk from studentimage where id = " + ImageId
                ).FirstOrDefault();
        }

        internal static int UpdateMultipleJobName(PhotoSorterDBModelDataContext db, string jobname, ArrayList arrJobId)
        {
            string allJobId = convertArrayListToString(arrJobId);

            return db.ExecuteCommand(
                "update PhotographyJob set JobName='" + jobname + "' where ID in (" + allJobId + ")"
                );
        }
        internal static int UpdateMultipleJobDate(PhotoSorterDBModelDataContext db, string datee, ArrayList arrJobId)
        {
            string allJobId = convertArrayListToString(arrJobId);

            return db.ExecuteCommand(
                "update PhotographyJob set JobDate='" + datee + "' where ID in (" + allJobId + ")"
                );
        }

        internal static int getMaxImportIdForSchool(PhotoSorterDBModelDataContext db, int schoolid)
        {
            try
            {
                StudentImport _objStudentImport = db.ExecuteQuery<StudentImport>("select max(ID) ID from StudentImport where schoolid=" + schoolid).SingleOrDefault();

                return _objStudentImport.ID;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // To get job Details
        public static Setting getSettingByName(PhotoSorterDBModelDataContext db, string settingName)
        {
            return (db.ExecuteQuery<Setting>("select id,settingName,settingValue from Settings where settingName='" + settingName + "'").First());
        }
        internal static string getSchoolFolderPath(PhotoSorterDBModelDataContext db, int schoolId)
        {
            School obj = db.ExecuteQuery<School>("select * from school where id=" + schoolId).First();
            return obj.folderPath;
        }
        public static IEnumerable<MaskDetail> getAllMaskDetails(PhotoSorterDBModelDataContext db, int maskId)
        {
            return db.ExecuteQuery<MaskDetail>(
                "select * from MaskDetails where maskid=" + maskId + " order by sortorder"
                ).ToList();
        }
        public static DataTable getAllMaskDetailsDatatable(PhotoSorterDBModelDataContext db, int maskId)
        {
            return Conversions.ToDataTable(db.ExecuteQuery<MaskDetail>(
                "select MaskDetID,MaskID,MaskDetail,Type,SortOrder from MaskDetails where maskid=" + maskId + " order by sortorder"
                ).ToList());
        }
        internal static string getmaxmaskid(PhotoSorterDBModelDataContext db)
        {
            tblmask obj = db.ExecuteQuery<tblmask>("select max(maskid) maskid from tblmask").SingleOrDefault();
            return Convert.ToString(obj.MaskID);
        }
        public static tblmask _UpdateMask(PhotoSorterDBModelDataContext db, int maskId)
        {
            return (from p in db.tblmasks where p.MaskID == maskId select p).SingleOrDefault();
        }
        public static IEnumerable<School> getSchools(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<School>(
                "select * from School order by schoolName"
                ).ToList();
        }
        public static IEnumerable<School> getSchools(PhotoSorterDBModelDataContext db, bool isActive)
        {
            string query = isActive == true ? "select * from School where IsActive ='" + isActive + "' or IsActive is null order by schoolName" : "select * from School order by schoolName";
            return db.ExecuteQuery<School>(query).ToList();
        }
        internal static int getmaxImportId(PhotoSorterDBModelDataContext db)
        {
            StudentImport obj = db.ExecuteQuery<StudentImport>("select max(ID) ID from StudentImport").SingleOrDefault();
            return obj.ID;
        }
        //internal static int getCountForFile(PhotoSorterDBModelDataContext db, string studentId, string schoolYear, int schoolId)
        //{
        //    return db.ExecuteQuery<Student>("select * from Student s join studentimport si on s.studentimportid = si.id where s.StudentID='" + studentId + "' and s.SchoolYear='" + schoolYear + "' and s.RecordStatus='true' and si.schoolid='" + schoolId + "'").Count(); //No School Year Student MohanSept3rd
        //}
        internal static DataTable getAllStudents(PhotoSorterDBModelDataContext db, Int32 StudentId)
        {
            //return WCFSQLHelper.getDataTable("select s.ID,s.FirstName,s.LastName,s.Teacher,s.Grade,s.StudentID,s.Custom5,ss.SchoolName from Student s inner join studentimport si on s.StudentImportID = si.id " +
            //                    " join School ss on si.SchoolID = ss.ID where s.ID in (" + StudentId + ") and s.RecordStatus='true'");
            return WCFSQLHelper.getDataTable("select s.ID,s.FirstName,s.LastName,s.Teacher,s.Grade,s.StudentID,s.Custom5,s.Password,ss.SchoolName,pj.JobName as SchoolYear, ss.Visit from Student s inner join studentimport si on s.StudentImportID = si.id " +
                               " join School ss on si.SchoolID = ss.ID join PhotographyJob pj on pj.ID = s.PhotographyJobID where s.ID in (" + StudentId + ") and s.RecordStatus='true'");
        }
        public static IEnumerable<StudentImage> getImgWithoutStudents(PhotoSorterDBModelDataContext db, ArrayList arrPhotoShootIds)
        {
            string photoShootIds = convertArrayListToString(arrPhotoShootIds);

            return db.ExecuteQuery<StudentImage>("select * from StudentImage where PhotoShootID in (" + photoShootIds + ") and studentidpk not in (SELECT distinct [StudentIDPK] FROM [Student] where RecordStatus='true' )").ToList();
        }
        public static int deleteStudentImage(PhotoSorterDBModelDataContext db, ArrayList arrStuImgId, string photoShootsId)
        {
            string allStuimgId = convertArrayListToString(arrStuImgId);

            db.ExecuteCommand(
                "DELETE w FROM studentphotoorder w INNER JOIN studentimage e ON w.StudentImageId in (" + allStuimgId + ") Where e.PhotoShootID in (" + photoShootsId + ")"
               );
            return db.ExecuteCommand(
                "delete from studentimage where ID in (" + allStuimgId + ")"
                );
        }
        internal static List<StudentImage> getStudentImageDetails(PhotoSorterDBModelDataContext db, int studentImgId)
        {
            return (db.ExecuteQuery<StudentImage>("select si.ID,SchoolID,PhotoShootID,ImageName,ImageNumber,s.FirstName,s.Lastname,s.StudentID ,s.Teacher,s.Grade,HomeRoom,s.DOB,s.[Address],s.City,s.[State],s.Zip,s.Phone,s.Custom1,s.Custom2,s.Custom3,s.Custom4,s.Custom5,si.Custom6,si.Custom7,si.Custom8,Packages,yearbook,Multipose,Ticketcode,s.Emailaddress,s.Password,Rating from StudentImage si join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and si.StudentIDPK = " + studentImgId).ToList());
        }
        //internal static DataTable bindStudentSchoolYear(PhotoSorterDBModelDataContext db, int schoolid)
        //{
        //    DataTable retdt = new DataTable();
        //    return WCFSQLHelper.getDataSetText("select distinct SchoolYear from View_StudentSchool where SchoolID = " + schoolid + " and SchoolYear is not null").Tables[0]; //No School Year in Student MohanSept3rd
        //}
        internal static int CountStudentImages(PhotoSorterDBModelDataContext db, int item)
        {
            return (from p in db.StudentImages where p.StudentIDPK == item select p).Count();
        }
        public static IList<Student> GetStudentsWithoutImgDetails(PhotoSorterDBModelDataContext db, int[] arrPhotoShootId, int[] arrStudentImportId, int schoolid)//, string schoolyear)
        {
            String photoShootIds = string.Join(",", arrPhotoShootId);
            String studentImportIds = string.Join(",", arrStudentImportId);

            //return db.ExecuteQuery<Student>("SELECT S.* FROM student S join Studentimport SI  on SI.ID = S.StudentImportID WHERE Si.SchoolID = " + schoolid + " and S.schoolyear in('" + schoolyear + "') and S.ID not in ( SELECT StudentIDpk FROM studentimage WHERE photoshootid in(" + photoShootIds + "))").ToList();
            //3.59 //return db.ExecuteQuery<Student>("SELECT S.* FROM student S join Studentimport SI  on SI.ID = S.StudentImportID WHERE StudentImportID in(" + studentImportIds + ") and Si.SchoolID = " + schoolid + " and S.ID not in ( SELECT StudentIDpk FROM studentimage WHERE photoshootid in(" + photoShootIds + "))").ToList();

            //Changed by Mohan Tangella  (commented by hemachandra  ---> Its wrong query)
            //return db.ExecuteQuery<Student>("SELECT S.* FROM student S join Studentimport SI  on SI.ID = S.StudentImportID WHERE StudentImportID in(" + studentImportIds + ") and Si.SchoolID = " + schoolid + " and S.ID not in ( select distinct StudentIDPK from StudentImage si join Student ss on si.StudentIDPK=ss.ID where StudentImportID in(" + studentImportIds + "))").ToList();

            //Changed by hemachandra on 4/12/2015
            return db.ExecuteQuery<Student>("select distinct s.*  from student s where s.ID not in (select distinct s.id as studentid from StudentImage si inner join " +
                                                 "student s on s.id = si.studentidpk where  s.StudentImportID in (" + studentImportIds + ") and si.PhotoShootID in (" + photoShootIds + ")) and s.StudentImportID in (" + studentImportIds + ") order by s.Id  ").ToList();
        }

        //count images
        internal static List<CountImagesNStudents> CountImages(PhotoSorterDBModelDataContext db, ArrayList arrPhotographyJobId) //Changed from IEnumerable to List  //Mohan
        {
            String jobIds = convertArrayListToString(arrPhotographyJobId);

            return db.ExecuteQuery<CountImagesNStudents>("Select s.id as StudentId,(s.FirstName+' '+s.Lastname) as Name, COUNT(s.id) as 'Total' from Student s inner join studentimage as si on s.ID = si.StudentIDPK join PhotoShoot as ps on si.PhotoShootID = ps.PhotoShotID where  photoshootid in(" + jobIds + ") and s.recordstatus=1 group by  s.id,s.FirstName,s.Lastname order by total desc").ToList();
        }

        //assign student
        internal static int updateAssignStudent(PhotoSorterDBModelDataContext db, int studentId, ArrayList arrStuImageId)
        {
            string allStudentID = "";
            for (int i = 0; i < arrStuImageId.Count; i++)
            {
                allStudentID += arrStuImageId[i] + ",";
            }
            allStudentID = allStudentID.Substring(0, allStudentID.Length - 1);
            if (!string.IsNullOrEmpty(allStudentID))
            {
                return db.ExecuteCommand(
                    "update StudentImage set StudentIdPK=" + studentId + " where ID in (" + allStudentID + ")"
                    );
            }
            else
                return 0;
        }
        internal static DataTable getStudentsDetails(PhotoSorterDBModelDataContext db, int schoolId)
        {
            //commented by mohan 
            //while importing photoshoots program doesn't allow to import different schools .
            //so every time we have only one school

            //query changed by Mohan
            //previousely school year column taking from student now changed to photography job year sept 3rd 2015
            //return ToDataTable( db.ExecuteQuery<Student>("select s.ID,s.FirstName,s.LastName,s.Teacher,s.Grade,s.StudentImportID,p.JobName from Student s inner join StudentImport sim on sim.id= s.StudentImportID inner join PhotographyJob p on p.ID= s.PhotographyJobID where RecordStatus='true' and sim.SchoolID = " + schoolId).ToList());

            return WCFSQLHelper.getDataTable("select s.ID,s.FirstName,s.Lastname,s.Teacher,s.Grade,s.StudentImportID,s.studentid,p.JobName from Student s inner join StudentImport sim on sim.id= s.StudentImportID inner join PhotographyJob p on p.ID= s.PhotographyJobID where RecordStatus='true' and sim.SchoolID = " + schoolId);
        }

        #region Update Student Image
        internal static int UpdateMultiplePackages(PhotoSorterDBModelDataContext db, string packages, ArrayList arrStuImgId)
        {
            string allStudentId = convertArrayListToString(arrStuImgId);

            return db.ExecuteCommand(
                "update StudentImage set Packages='" + packages + "' where ID in (" + allStudentId + ")"
                );
        }
        internal static int UpdateMultipleHomeRoom(PhotoSorterDBModelDataContext db, string homeRoom, ArrayList arrStuImgId)
        {
            string allStudentId = convertArrayListToString(arrStuImgId);

            return db.ExecuteCommand(
                "update StudentImage set HomeRoom='" + homeRoom + "' where ID in (" + allStudentId + ")"
                );
        }
        internal static int UpdateMultipleTicketCode(PhotoSorterDBModelDataContext db, string ticketCode, ArrayList arrStuImgId)
        {
            string allStudentId = convertArrayListToString(arrStuImgId);

            return db.ExecuteCommand(
                "update StudentImage set Ticketcode='" + ticketCode + "' where ID in (" + allStudentId + ")"
                );
        }
        internal static int UpdateRating(PhotoSorterDBModelDataContext db, string rating, ArrayList arrStuimgId)
        {
            int result = 0;
            string allStudentId = convertArrayListToString(arrStuimgId);

            if (rating == "Clear")
                result = db.ExecuteCommand(
                "update StudentImage set rating=null where ID in (" + allStudentId + ")"
                );
            else
                result = db.ExecuteCommand(
                         "update StudentImage set rating='" + rating + "' where ID in (" + allStudentId + ")"
                         );

            return result;

        }
        internal static int UpdateMultipleYearbook(PhotoSorterDBModelDataContext db, string yearBook, ArrayList arrStuImgId)
        {
            string allStudentId = convertArrayListToString(arrStuImgId);

            return db.ExecuteCommand(
                "update StudentImage set yearbook='" + yearBook + "' where ID in (" + allStudentId + ")"
                );
        }
        internal static int UpdateMultipleAdminCD(PhotoSorterDBModelDataContext db, string adminCD, ArrayList arrStuImgId)
        {
            string allStudentId = convertArrayListToString(arrStuImgId);

            return db.ExecuteCommand(
                "update StudentImage set admincd='" + adminCD + "' where ID in (" + allStudentId + ")"
                );
        }
        internal static int updateCustomValue(PhotoSorterDBModelDataContext db, string customValue, ArrayList arrStuImgId, string Custom)
        {
            string allStudentId = convertArrayListToString(arrStuImgId);

            return db.ExecuteCommand(
                "update StudentImage set " + Custom + "='" + customValue + "' where ID in (" + allStudentId + ")"
                );
        }
        #endregion



        //export
        public static School getSchoolById(PhotoSorterDBModelDataContext db, int schoolid)
        {
            return (db.ExecuteQuery<School>("select * from school where ID=" + schoolid + "").SingleOrDefault());
        }
        public static DataTable GetMaskDetails(PhotoSorterDBModelDataContext db, Int32 maskId)
        {
            return Conversions.ToDataTable(db.ExecuteQuery<MaskDetail>(
                "select * from maskdetails where maskid = " + maskId + " order by sortorder"
                ).ToList());
        }
        public static IEnumerable<tblmask> getMasks(PhotoSorterDBModelDataContext db, int schoolId)
        {
            return db.ExecuteQuery<tblmask>(
                "select * from tblmask order by maskname"
                ).ToList();
        }

        private static readonly Regex InvalidFileRegex = new Regex(string.Format("[{0}]", Regex.Escape(@"<>:""/\|?*")));
        public static string SanitizeFileName(string fileName)
        {
            return InvalidFileRegex.Replace(fileName, string.Empty);
        }
        public static DataTable getImageNameLastNameFirstName(PhotoSorterDBModelDataContext db, int[] arrImageId)
        {
            string imageId = arrImageId.Count() > 0 ? string.Join(",", arrImageId) : "";

            if (!string.IsNullOrEmpty(imageId))
            {
                return Conversions.ToDataTable(db.ExecuteQuery<StudentImage>(
                    "select si.ID,si.ImageNumber,ImageName,si.schoolid,si.photoshootid, si.StudentID, si.studentIdPk,s.FirstName,s.Lastname,s.StudentID ,s.Teacher,s.Grade,s.Password from StudentImage si join student s on s.id = si.studentidpk where s.Recordstatus = 'true' and si.Recordstatus = 'Active' and si.ID in (" + imageId + ")"
                    ).ToList());
            }
            else
                return null;
        }
        internal static PhotoShoot getImageFolder(PhotoSorterDBModelDataContext db, int PhotoID)
        {
            return (db.ExecuteQuery<PhotoShoot>("select * from PhotoShoot where PhotoShotID=" + PhotoID + "").FirstOrDefault());
        }

        //ValidateAdminCD
        public static List<StudentImage> getStudentsImagesByStuId(PhotoSorterDBModelDataContext db, int[] photoShootIds, int studentId)
        {
            List<StudentImage> tempRes = new List<StudentImage>();
            string allPhotoshootIds = photoShootIds.Count() > 0 ? string.Join(",", photoShootIds) : "";
            int[] stuImageIds = db.ExecuteQuery<int>("select * from StudentImage where PhotoShootID in( " + allPhotoshootIds + ") and StudentIDPK = " + studentId).ToArray();
            if (stuImageIds.Count() > 0)
            {
                string imageIds = string.Join(",", stuImageIds);
                if (!string.IsNullOrEmpty(imageIds))
                {
                    tempRes = db.ExecuteQuery<StudentImage>(
                    "select si.ID,si.ImageNumber,ImageName,si.schoolid,si.photoshootid,si.studentIdPk,si.yearbook,si.Admincd,si.Rating,s.FirstName,s.Lastname,s.StudentID ,s.Teacher,s.Grade,s.Password from StudentImage si join student s on s.id = si.studentidpk where s.Recordstatus = 'true' and si.Recordstatus = 'Active' and si.ID in (" + imageIds + ")"
                    ).ToList();
                }
            }
            return tempRes;
        }
        //show error log
        public static IEnumerable<Renameerrorlog> getErrorLogRenameImages(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<Renameerrorlog>(
                "select * from Renameerrorlog"
                ).ToList();
        }

        //review pricing
        internal static IEnumerable<Package> getPackagesBySchoolId(PhotoSorterDBModelDataContext db, int schoolId)
        {
            return (db.ExecuteQuery<Package>("select ID,item,package, price from Package where schoolId = " + schoolId).ToList());
        }

        //Generate PDF VM
        internal static string getTemplateLocationById(PhotoSorterDBModelDataContext db, int templateId)
        {
            Template _objTemplate = db.ExecuteQuery<Template>("select id,templatePath from Templates where id=" + templateId).First();
            return _objTemplate.templatePath;
        }
        internal static string getTemplateLocationByName(PhotoSorterDBModelDataContext db, string templateName)
        {
            Template obj = db.ExecuteQuery<Template>("select * from Templates where templatecode='" + templateName + "'").First();
            return obj.templatePath;
        }
        internal static DataTable getPackagesBySchoolIdDataTable(PhotoSorterDBModelDataContext db, int schoolId)
        {
            return Conversions.ToDataTable(db.ExecuteQuery<Package>("select ID,item,package, price from Package where schoolId = " + schoolId).ToList());
        }
        public static School getSchoolName(PhotoSorterDBModelDataContext db, int schoolId)
        {
            return (db.ExecuteQuery<School>("select * from school where ID=" + schoolId + "").SingleOrDefault());
        }
        public static PhotographyJob getJobName(PhotoSorterDBModelDataContext db, int jobId)
        {
            return (db.ExecuteQuery<PhotographyJob>("select * from PhotographyJob where ID=" + jobId + "").SingleOrDefault());
        }
        internal static string getSchoolNameByID(PhotoSorterDBModelDataContext db, int schoolId)
        {
            School obj = db.ExecuteQuery<School>("select id,schoolName from school where ID=" + schoolId).First();
            return obj.SchoolName;
        }
        internal static IEnumerable<Template> getTemplates(PhotoSorterDBModelDataContext db)
        {
            return (db.ExecuteQuery<Template>("select Id,templateCode,templatePath from Templates where templateCode='template A' or templatecode='template B' ").ToList());
        }
        public static IEnumerable<Student> getDeactivateStudents(PhotoSorterDBModelDataContext db, int schoolId)
        {
            return (db.ExecuteQuery<Student>("select s.* from student s inner join StudentImport si on s.StudentImportID=si.ID where s.RecordStatus='false' and si.SchoolID=" + schoolId + "").ToList());
        }
        //import batches VM
        // changed to public from internal by hema - NUnit
        public static DataTable getStudentImportData(PhotoSorterDBModelDataContext db, int schoolID, bool isfromStudentsImport)
        {
            //return  (db.ExecuteQuery<StudentImport>("select * from StudentImport where SchoolID = " + schoolID).ToList()); //now we are using common UserControl to display Order Import batches and stud import batches

            DataTable dt = new DataTable();
            if (isfromStudentsImport)
            {
                dt = WCFSQLHelper.getDataTable("select ID,Description,Notes,CreatedOn from StudentImport where SchoolID = " + schoolID);
            }
            else
            {
                dt = WCFSQLHelper.getDataTable("select ID,Description,Notes,CreatedOn from OrdersImport");
            }
            return dt;
        }
        internal static List<StudentImport> getStudentImportData(PhotoSorterDBModelDataContext db, int schoolID)
        {
            return (db.ExecuteQuery<StudentImport>("select * from StudentImport where SchoolID = " + schoolID).ToList());
        }
        internal static StudentImport getSelectedStudentImportData(PhotoSorterDBModelDataContext db, int schoolID, int importId)
        {
            return (db.ExecuteQuery<StudentImport>("select * from StudentImport where SchoolID = " + schoolID + "and ID = " + importId).FirstOrDefault());

        }
        //locked photos VM
        public static int deleteStudentImage(PhotoSorterDBModelDataContext db, int stuimgid)
        {
            return db.ExecuteCommand(
                "delete from studentimage where ID = " + stuimgid
                );
        }
        public static IList<StudentImage> getDeletedStudentImage(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<StudentImage>(
                "select * from StudentImage where recordstatus = 'Delete'"
                ).ToList();
        }
        public static int deleteStudentImages(PhotoSorterDBModelDataContext db, ArrayList stuImgId)
        {
            string allstuImageId = convertArrayListToString(stuImgId);

            return db.ExecuteCommand(
                "delete from studentimage where ID in (" + allstuImageId + ")"
                );
        }
        public static List<StudentImage> getLockedPhotos(PhotoSorterDBModelDataContext db, int schoolId)    //changed from IEnumerable to List
        {
            return db.ExecuteQuery<StudentImage>("select * from StudentImage  where schoolid=" + schoolId + " and  recordstatus='delete'").ToList();
        }

        //order default pricing
        internal static IEnumerable<DefaultPackage> getDefaultPricing(PhotoSorterDBModelDataContext db)
        {
            return (db.ExecuteQuery<DefaultPackage>("select ID,item,package, price from DefaultPackages").ToList());
        }

        //PhotographyJob VM
        public static IEnumerable<PhotographyJob> getJobs(PhotoSorterDBModelDataContext db, int schoolID)
        {
            return db.ExecuteQuery<PhotographyJob>(
                "select ID,JobName,JobDate,ImageFolder,SchoolID,StartYear,EndYear from PhotographyJob where SchoolID = " + schoolID + " order by jobDate desc"
                ).ToList();     //season changes
        }
        public static int deletePhotographyJob(PhotoSorterDBModelDataContext db, ArrayList arrPhotoJobId)
        {
            string allPhotoId = convertArrayListToString(arrPhotoJobId);

            return db.ExecuteCommand(
                "delete from PhotographyJob where ID in (" + allPhotoId + ")"
                );
        }

        //AddEditSchool VM
        public static School getSchoolDetails(PhotoSorterDBModelDataContext db, int schoolId)
        {
            //return (db.ExecuteQuery<School>("select * from School where ID = " + schoolId).ToList());
            return db.Schools.Where(sch => sch.ID == schoolId).FirstOrDefault(); //changed by Mohan
        }
        public static int getSchoolByPath(PhotoSorterDBModelDataContext db, string tempSchoolPath, int tempSclId)
        {
            //storedproc's will open a connection to database while executing it
            //which is not a better practice so instead we directly executing the quries
            //Changed by Mohan
            //#Mohan
            string x = tempSchoolPath.Replace("'", "''");
            int retValue = 0;
            if (tempSclId == 0)
            {
                retValue = db.ExecuteCommand("select count(*) countimg from School where folderpath like '" + x + "'");
                retValue = retValue < 0 ? 0 : retValue;
            }
            else
            {
                retValue = db.ExecuteCommand("select count(*) countimg from School where folderpath like '%" + x + "%' and ID !=" + tempSclId);
                retValue = retValue < 0 ? 0 : retValue;
            }
            return retValue;
            //SqlParameter[] param = new SqlParameter[2];
            //param[0] = new SqlParameter("@SchoolPath", tempSchoolPath);
            //param[1] = new SqlParameter("@SchoolID", tempSclId);
            //return WCFSQLHelper.getDataTable_SP("sp_GetSchoolFolderCount", param);
        }
        public static School updateSchool(PhotoSorterDBModelDataContext db, int schoolID)
        {
            return (from p in db.Schools where p.ID == schoolID select p).SingleOrDefault();
        }
        internal static string getMaxSchoolId(PhotoSorterDBModelDataContext db)
        {
            School obj = db.ExecuteQuery<School>("select max(ID) ID from school").SingleOrDefault();
            return Convert.ToString(obj.ID);
        }

        //Bulk Rename VM
        internal static int UpdateMultipleSchoolName(PhotoSorterDBModelDataContext db, string Schoolname, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set SchoolName='" + Schoolname + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleAddress1(PhotoSorterDBModelDataContext db, string address1, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Address1='" + address1 + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleAddress2(PhotoSorterDBModelDataContext db, string address2, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Address2='" + address2 + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleAddress3(PhotoSorterDBModelDataContext db, string address3, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Address3='" + address3 + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleCity(PhotoSorterDBModelDataContext db, string city, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set City='" + city + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleState(PhotoSorterDBModelDataContext db, string state, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set State='" + state + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleZip(PhotoSorterDBModelDataContext db, string zip, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Zip='" + zip + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleNotes(PhotoSorterDBModelDataContext db, string notes, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Notes='" + notes + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleIsActiveValueForSchool(PhotoSorterDBModelDataContext db, bool isActive, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set isActive='" + isActive + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleYearbookForSchool(PhotoSorterDBModelDataContext db, bool yearbook, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set yearbookrequired='" + yearbook.ToString() + "' where ID in (" + allSchoolId + ")"
                );
        }
        internal static int UpdateMultipleIDRequiredForSchool(PhotoSorterDBModelDataContext db, bool IDRequired, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set idrequired='" + IDRequired.ToString() + "' where ID in (" + allSchoolId + ")"
                );
        }

        //schools VM
        public static int deleteSchools(PhotoSorterDBModelDataContext db, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "delete from school where ID in (" + allSchoolId + ")"
                );
        }

        #region Validation AdmindCD
        internal static DataTable GetStudentForAdminCdYearbookcd(int[] arrPhotoShootId, int[] arrImportBatchId, string windowName)
        {
            string sql = "";
            string allPhotoshootId = string.Join(",", arrPhotoShootId);
            string allImportBatchId = string.Join(",", arrImportBatchId);

            if (!string.IsNullOrEmpty(allPhotoshootId) && !string.IsNullOrEmpty(allImportBatchId))
            {
                //if (windowName == "Admincd")
                sql = "select   FirstName,  LastName,  studentid,  count(studentid) CountStudentID from   View_StudentSeasonImage " +
                    "where   PhotoshootID in(" + allPhotoshootId + ") and StudentImportID in(" + allImportBatchId + ") and " + windowName + " = 1  group by   studentid,   firstname,   lastname having   count(studentid)> 1 ";

                //else
                //    sql = "select   FirstName,  LastName,  studentid,  count(studentid) CountStudentID from   View_StudentSeasonImage " +
                //        "where   PhotoshootID in(" + allJobId + ") and StudentImportID in(" + allImportBatchId + ") and yearbook = 1  group by   studentid,   firstname,   lastname having   count(studentid)> 1  ";

                return WCFSQLHelper.getDataSetText(sql).Tables[0];
            }
            else
                return null;
        }
        internal static System.Data.DataTable GetStudentForNotRatingAdminCdYearbookcd(int[] arrPhotoShootId, int[] arrImportBatchId, string windowname)//int[] arrAllJobId, 
        {
            //string allJobId = string.Join(",", arrAllJobId);
            string photoShootIds = string.Join(",", arrPhotoShootId);
            string allImportBatchId = string.Join(",", arrImportBatchId);
            string sql = "";

            // sql = "select distinct   FirstName,  LastName,  id as studentid from student where  PhotographyJobID in(" + allJobId + ")  and ID not in " +
            //"(Select distinct StudentID from View_StudentSeasonImage  where PhotographyJobID in(" + allJobId + ") and StudentImportID in(" + allImportBatchId + ") and isnull(admincd, 0) = 1) order by   Lastname, FirstName ";

            //Changed by Mohan    //#commented by hema on 2-12-2015 (Its wrong)
            //sql = "select distinct s.FirstName,  s.LastName,  s.id as studentid from student s " +
            //        "join StudentImage si on s.ID = si.StudentIDPK " +
            //        "where  si.PhotoShootID in(" + photoShootIds + ") and s.ID not in (Select distinct StudentID from View_StudentSeasonImage " +
            //        "where PhotoshootID in(" + photoShootIds + ") and StudentImportID in(" + allImportBatchId + ") and isnull(" + windowname + ", 0) = 1) order by   Lastname, FirstName ";

            // changed by hema(include Students without images aslo)
            //sql = "select distinct s.id as studentid,s.FirstName,s.LastName from StudentImage si inner join " +
            //             "student s on s.id = si.studentidpk where  s.StudentImportID in (" + allImportBatchId + ") and si.PhotoShootID in (" + photoShootIds + ") and s.ID not in " +
            //            "(select distinct StudentID from View_StudentSeasonImage  where PhotoshootID in (" + photoShootIds + ") " +
            //            "and StudentImportID in (" + allImportBatchId + ") and isnull(" + windowname + ", 0) = 1) " + "union " + "select distinct s.ID as studentid,s.firstName,s.Lastname from student s where s.ID not in (select distinct s.id as studentid from StudentImage si inner join " +
            //            "student s on s.id = si.studentidpk where  s.StudentImportID in (" + allImportBatchId + ") and si.PhotoShootID in (" + photoShootIds + ")) and s.StudentImportID in (" + allImportBatchId + ") ";

            //changed by hema 4-12-2015(exclude students without images)
            if (!string.IsNullOrEmpty(photoShootIds) && !string.IsNullOrEmpty(allImportBatchId))
            {
                sql = "select distinct s.id as studentid,s.FirstName,s.LastName from StudentImage si inner join " +
                            "student s on s.id = si.studentidpk where  s.StudentImportID in (" + allImportBatchId + ") and si.PhotoShootID in (" + photoShootIds + ") and s.ID not in " +
                           "(select distinct StudentID from View_StudentSeasonImage  where PhotoshootID in (" + photoShootIds + ") " +
                           "and StudentImportID in (" + allImportBatchId + ") and isnull(" + windowname + ", 0) = 1) order by   Lastname, FirstName ";

                return WCFSQLHelper.getDataSetText(sql).Tables[0];
            }
            else
                return null;
        }
        #endregion

        #region  code added Abhilasha
        internal static List<StudentImage> GetStudentImgfromPhotoShoot(PhotoSorterDBModelDataContext db, int shootId)   //changed to list<T> from ienumerable<T>
        {
            //return db.ExecuteQuery<StudentImage>("select si.ID,si.ImageNumber,si.SchoolID,si.PhotoShootID,si.ImageName,si.QuixieID,s.FirstName,s.Lastname,s.StudentID,s.Teacher,s.Grade,si.HomeRoom,s.DOB,s.[Address],s.City,s.[State],s.Zip,s.Phone,si.Custom1,si.Custom2,si.Custom3,si.Custom4,si.Custom5,si.Custom6,si.Custom7,si.Custom8,si.OriginalImageName,si.FailedRename,si.Packages,si.yearbook,si.admincd,si.Multipose,si.Ticketcode,s.Emailaddress,s.Password,si.Rating from StudentImage si join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and PhotoShootID = " + shootId + "  and s.RecordStatus='true'").ToList();                 // added two more column originalimagename and failedrename to rename source images
            return db.ExecuteQuery<StudentImage>("select si.* from StudentImage si join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and PhotoShootID = " + shootId + "  and s.RecordStatus='true'").ToList();                 // added two more column originalimagename and failedrename to rename source images
        }
        internal static bool checkTableVersion(PhotoSorterDBModelDataContext db)
        {
            bool exists;
            try
            {
                exists = true;
                string versionReName = "IF exists( select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='tblVersion' and COLUMN_NAME='Version') begin exec sp_RENAME 'tblVersion.Version', 'PSVersion' , 'COLUMN' end";

                db.ExecuteCommand("select * from tblVersion where 1 = 0");
                db.ExecuteCommand(versionReName);
            }
            catch
            {
                exists = false;
                db.ExecuteCommand("Create table tblVersion(PSVersion  varchar(10),Notes varchar(200))");
            }
            return exists;
        }

        public static string GetPhotoShootPath(PhotoSorterDBModelDataContext db, int selectedPhotoShootID)
        {
            PhotoShoot obj = db.ExecuteQuery<PhotoShoot>("select photoshotid,photoshotname,ImageFolder from PhotoShoot where photoshotid=" + selectedPhotoShootID).First();
            return obj.ImageFolder;
        }

        #endregion

        //AddEditPhotoShoot VM
        internal static IEnumerable<PhotoShoot> getPhotoShootDetails(PhotoSorterDBModelDataContext db, int PhotoShootId)
        {
            return (db.ExecuteQuery<PhotoShoot>("select * from PhotoShoot where PhotoShotID = " + PhotoShootId).ToList());
        }
        public static PhotoShoot updatePhotoShoot(PhotoSorterDBModelDataContext db, int photoShootId)
        {
            return (from p in db.PhotoShoots where p.PhotoShotID == photoShootId select p).SingleOrDefault();
        }
        //bulk rename photoshoot 
        internal static int updateMultiplePhotoShootName(PhotoSorterDBModelDataContext db, string shootName, ArrayList arrShootId)
        {
            string allShootId = convertArrayListToString(arrShootId);

            return db.ExecuteCommand(
                "update PhotoShoot set PhotoShotName='" + shootName + "' where PhotoShotID in (" + allShootId + ")"
                );
        }

        internal static int updateMultiplePhotographyJobId(PhotoSorterDBModelDataContext db, int? photographyJobId, ArrayList arrShootId)
        {
            string allShootId = convertArrayListToString(arrShootId);

            return db.ExecuteCommand(
                "update PhotoShoot set PhotographyjobID='" + photographyJobId + "' where PhotoShotID in (" + allShootId + ")"
                );
        }

        internal static int updateMultiplePhotoShootDate(PhotoSorterDBModelDataContext db, string shootdate, ArrayList arrShootId)
        {
            string allShootId = convertArrayListToString(arrShootId);

            return db.ExecuteCommand(
                "update PhotoShoot set PhotoShotDate='" + shootdate + "' where PhotoShotID in (" + allShootId + ")"
                );
        }

        //yearbook selection
        public static int UpdateYearbook(PhotoSorterDBModelDataContext db, int jobId, Boolean type)
        {
            //storedproc's will open a connection to database while executing it
            //which is not a better practice so instead we directly executing the quries
            //Changed by Mohan
            //#Mohan
            //SqlParameter[] param = new SqlParameter[2];
            //param[0] = new SqlParameter("@JobID", jobid);
            //param[1] = new SqlParameter("@Yearbok", type);
            //return WCFSQLHelper.executeNonQuery_SP("sp_UpdateYearBookJobID", param);

            string query = "update si set si.yearbook ='" + type + "' from StudentImage si inner join PhotoShoot ps on si.photoshootid = ps.photoshotid where ps.photographyjobid= " + jobId + "";
            return db.ExecuteCommand(query);
        }
        internal static int UpdateYearBookSelected(PhotoSorterDBModelDataContext db, ArrayList tempArrImageId, Boolean type)
        {
            string allImageId = convertArrayListToString(tempArrImageId);

            return db.ExecuteCommand(
                "update StudentImage set yearbook='" + type + "' where ID in (" + allImageId + ")"
                );
        }
        internal static int UpdateYearBookPhotoShoot(PhotoSorterDBModelDataContext db, int shootId, Boolean type)
        {
            return db.ExecuteCommand(
                "update StudentImage set yearbook='" + type + "' where PhotoShootID =" + shootId + ""
                );
        }
        //sync delete messages
        public static IList<StudentImage> getDeletedStudentimg(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<StudentImage>(
                "select * from StudentImage where recordstatus = 'Delete'"
                ).ToList();
        }
        public static int deletestudentimage(PhotoSorterDBModelDataContext db, int stuImIId)
        {
            return db.ExecuteCommand(
                "delete from studentimage where ID = " + stuImIId
                );
        }
        //get rating VM
        internal static List<PhotoShoot> getPhotoShootDetailsById(PhotoSorterDBModelDataContext db, int jobId, int photoShootId)
        {
            if (photoShootId == 0)
                return (db.ExecuteQuery<PhotoShoot>("select PhotoShotID,ImageFolder,PhotoShotName from PhotoShoot where PhotographyjobID = " + jobId).ToList());
            else
                return (db.ExecuteQuery<PhotoShoot>("select PhotoShotID,ImageFolder,PhotoShotName from PhotoShoot where PhotoShotID = " + photoShootId).ToList());
        }
        #region  code added Abhilasha
        internal static List<StudentImage> getStudentImgDetailsByShootId(PhotoSorterDBModelDataContext db, int shootId)
        {
            List<StudentImage> si = new List<StudentImage>();

            si = db.ExecuteQuery<StudentImage>("select si.ID,si.ImageNumber,SchoolID,PhotoShootID,ImageName,QuixieID,s.FirstName,s.Lastname,s.StudentID,s.Teacher,s.Grade,HomeRoom,s.DOB,s.[Address],s.City,s.[State],s.Zip,s.Phone,s.Custom1,s.Custom2,s.Custom3,s.Custom4,s.Custom5,si.Custom6,si.Custom7,si.Custom8,si.OriginalImageName,si.FailedRename,Packages,yearbook,admincd,Multipose,Ticketcode,s.Emailaddress,s.Password,Rating from StudentImage si join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and PhotoShootID = " + shootId + "  and s.RecordStatus='true'").ToList();                 // added two more column originalimagename and failedrename to rename source images
            return si;

        }
        #endregion
        public static StudentImage updateStudentImage(PhotoSorterDBModelDataContext db, int imgId)
        {
            return (from p in db.StudentImages where p.ID == imgId select p).SingleOrDefault();
        }
        internal static int updateRating(PhotoSorterDBModelDataContext db, string rating, Int32 StuimgId)
        {
            return db.ExecuteCommand(
                "update StudentImage set rating='" + rating + "' where ID in (" + StuimgId + ")"
                );
        }
        //progressbar rename VM
        internal static IList<StudentImage> GetStudentImgfromImages(PhotoSorterDBModelDataContext db, ArrayList photoShootId)
        {
            string allImgId = convertArrayListToString(photoShootId);

            return (db.ExecuteQuery<StudentImage>("select si.ID,SchoolID,PhotoShootID,ImageName,QuixieID,s.FirstName,s.Lastname,s.StudentID ,s.Teacher,s.Grade,HomeRoom,s.DOB,s.[Address],s.City,s.[State],s.Zip,s.Phone,s.Custom1,s.Custom2,s.Custom3,s.Custom4,s.Custom5,si.Custom6,si.Custom7,si.Custom8,Packages,yearbook,Multipose,Ticketcode,s.Emailaddress,s.Password,Rating from StudentImage si join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and s.RecordStatus='true' and si.ID in (" + allImgId + ")").ToList());
        }
        internal static IList<StudentImage> GetStudentImgfromPhotoShootFMulti(PhotoSorterDBModelDataContext db, int Shootid)
        {
            return (db.ExecuteQuery<StudentImage>("select si.ID,SchoolID,PhotoShootID,ImageName,QuixieID,s.FirstName,s.Lastname,s.StudentID ,s.Teacher,s.Grade,HomeRoom,s.DOB,s.[Address],s.City,s.[State],s.Zip,s.Phone,s.Custom1,s.Custom2,s.Custom3,s.Custom4,s.Custom5,si.Custom6,si.Custom7,si.Custom8,Packages,yearbook,Multipose,Ticketcode,s.Emailaddress,s.Password,Rating from StudentImage si join PhotoShoot ps on si.PhotoShootId = ps.PhotoShotID join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and s.RecordStatus='true' and si.photoshootid =" + Shootid + "").ToList());
        }
        #region  code added Abhilasha
        internal static int updateStudentImgName(PhotoSorterDBModelDataContext db, int imgId, string imageName)
        {
            return db.ExecuteCommand(
                "update StudentImage set ImageName='" + imageName.Replace("'", "''") + "' where ID in (" + imgId + ")"
                );
        }

        internal static int updateOriginalStudentImgName(PhotoSorterDBModelDataContext db, int imgId, int status)
        {
            if (status == 0)
                return db.ExecuteCommand(
                    "update StudentImage set OriginalImageName=imagename,failedrename=" + status + " where ID in (" + imgId + ")"
                    );
            else
                return db.ExecuteCommand(
                               "update StudentImage set failedrename=" + status + " where ID in (" + imgId + ")"
                               );
        }
        #endregion

        //Auto Create Groups VM
        internal static IEnumerable<PhotoShoot> getPhotoShootDetailsbyJobid(PhotoSorterDBModelDataContext db, int PhotojobId)
        {
            return (db.ExecuteQuery<PhotoShoot>("select * from PhotoShoot where photographyjobid = " + PhotojobId).ToList());
        }
        internal static int getStudentImagesWithoutTeacherCountJobID(PhotoSorterDBModelDataContext db, ArrayList arrSelectedShootId)
        {
            //changed the below query on 7-may-2014. by gagan benipal. Because it was taking teacher from studentImage instead of student
            string allShootId = convertArrayListToString(arrSelectedShootId);

            return db.ExecuteQuery<int>("select si.id from StudentImage si join student s on si.studentidpk=s.id  where  photoshootid in (" + allShootId + ")   and (s.Teacher='' or s.Teacher is null) and s.RecordStatus='true' and si.RecordStatus='Active'").Count();
        }
        internal static int getStudentImagesWithoutTeacherCountSelectedImages(PhotoSorterDBModelDataContext db, ArrayList arrSelectedShootId, ArrayList selectedImageIds)
        {
            //changed the below query on 7-may-2014. by gagan benipal. Because it was taking teacher from studentImage instead of student
            string allShootId = convertArrayListToString(arrSelectedShootId);
            string allImageIds = convertArrayListToString(selectedImageIds);
            return db.ExecuteQuery<int>("select si.id from StudentImage si join student s on si.studentidpk=s.id  where  photoshootid in (" + allShootId + ")   and (s.Teacher='' or s.Teacher is null) and s.RecordStatus='true' and si.RecordStatus='Active' and si.Id in (" + allImageIds + ")").Count();
        }
        internal static List<string> getTeachersByPhotoJob(PhotoSorterDBModelDataContext db, ArrayList arrShootId)
        {
            //changed the below query on 7-may-2014. by gagan benipal. Because it was taking teacher from studentImage instead of student
            //return db.ExecuteQuery<string>("SELECT s.Teacher FROM StudentImage si join student s on si.studentidpk=s.id join photoshoot ps on si.photoshootid = ps.photoshotid Where ps.PhotographyjobID = " + selectedJobID + " group by s.Teacher").ToList();
            string AllShootID = convertArrayListToString(arrShootId);

            return db.ExecuteQuery<string>("select teacher from(SELECT Teacher=case when s.Teacher is null then 'No Teacher' when s.Teacher ='' then 'No Teacher' else s.Teacher end  FROM StudentImage si join student s on si.studentidpk=s.id  Where photoshootid in (" + AllShootID + ") and s.RecordStatus='true' and si.RecordStatus='Active')abc group by Teacher").ToList();
        }
        internal static List<int> getStudentImageIdsByTeacher(PhotoSorterDBModelDataContext db1, ArrayList arrShootId, string teacher)
        {
            //changed the below query on 7-may-2014. by gagan benipal. Because it was taking teacher from studentImage instead of student
            string allShootId = convertArrayListToString(arrShootId);

            List<int> retval;
            if (teacher == "No Teacher")
                retval = db1.ExecuteQuery<int>("select si.id from StudentImage si join student s on si.studentidpk=s.id Where photoshootid in (" + allShootId + ") and (s.Teacher is null or s.teacher='') and s.RecordStatus='true' and si.RecordStatus='Active'").ToList();
            else
                retval = db1.ExecuteQuery<int>("select si.id from StudentImage si join student s on si.studentidpk=s.id Where photoshootid in (" + allShootId + ") and s.Teacher='" + teacher + "' and s.RecordStatus='true' and si.RecordStatus='Active'").ToList();
            return retval;
            //return db1.ExecuteQuery<int>("SELECT ID FROM StudentImage Where photoshootid = " + selectedPhotoShootID + " and Teacher='" + teacher + "'").ToList();
        }

        internal static List<int> getStudentImageIdsByStudentId(PhotoSorterDBModelDataContext db, int studentId, ArrayList arrShootId)
        {
            string allShootId = convertArrayListToString(arrShootId);

            return db.ExecuteQuery<int>("select id from StudentImage where studentidpk= " + studentId + " and photoshootid in (" + allShootId + ")").ToList();
        }

        //export to text VM
        public static IEnumerable<StudentImage> getStudents(PhotoSorterDBModelDataContext db, int jobId)
        {
            return db.ExecuteQuery<StudentImage>(
                "select si.ID,si.ImageNumber,SchoolID,PhotoShootID,ImageName,QuixieID,s.FirstName,s.Lastname,s.StudentID ,s.Teacher,s.Grade,HomeRoom,s.DOB,s.[Address],s.City,s.[State],s.Zip,s.Phone,s.Custom1,s.Custom2,s.Custom3,s.Custom4,s.Custom5,si.Custom6,si.Custom7,si.Custom8,Packages,yearbook,Multipose,Ticketcode,s.Emailaddress,s.Password,Rating from StudentImage si join PhotoShoot ps on si.PhotoShootId = ps.PhotoShotID join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and ps.PhotographyJobID = " + jobId + "  and s.RecordStatus='true'"
                ).ToList();
        }
        #region  code added Abhilasha
        public static List<StudentImage> getStudentsByPhotoShootId(PhotoSorterDBModelDataContext db, int photoShootId)  //changed to list<T> from ienumerable<T>
        {
            return db.ExecuteQuery<StudentImage>(
             "select si.ID,si.ImageNumber,SchoolID,PhotoShootID,ImageName,QuixieID,s.FirstName,s.Lastname,s.StudentID ,s.Teacher,s.Grade,HomeRoom,s.DOB,s.[Address],s.City,s.[State],s.Zip,s.Phone,s.Custom1,s.Custom2,s.Custom3,s.Custom4,s.Custom5,si.Custom6,si.Custom7,si.Custom8,si.OriginalImageName,si.failedrename,Packages,yearbook,admincd,Multipose,Ticketcode,s.Emailaddress,s.Password,Rating from StudentImage si join student s on s.id = si.studentidpk where si.Recordstatus = 'Active' and PhotoShootID = " + photoShootId + " and s.RecordStatus='true'"                          // featch two new columns originalfilename and failedrename from studentimage table
             ).ToList();
        }
        #endregion

        //public static DataTable getOrderDetailsByOrderIds(PhotoSorterDBModelDataContext db, ArrayList orderIds)
        //{
        //    List<StudentImage> totalOrders = new List<StudentImage>();
        //    foreach (int Id in orderIds)
        //    {
        //        int[] orderImageIds = clsOrders.getImagesIdsByOrder(db, Id).Select(a => (int)a.StudentImageId).ToArray();
        //        totalOrders.AddRange(GetStudentsByImageIdArray(db, orderImageIds));
        //        totalOrders.Add(new StudentImage { ImageName = "Order.jpg" });
        //    }
        //    return ToDataTable(totalOrders);
        //}

        public static List<StudentImage> GetStudentsByImageIdArray(PhotoSorterDBModelDataContext db, int[] tempArrImgId) //changed from IEnumerable to List //Mohan
        {
            List<StudentImage> totalResult = new List<StudentImage>();
            List<int[]> stuImageIds = new List<int[]>();

            if (tempArrImgId.Count() > 1000)
            {
                stuImageIds = tempArrImgId
                    .Select((e, i) => new { e, p = i / 1000 })
                    .GroupBy(e => e.p)
                    .Select(g => g
                        .Select(e => e.e)
                        .ToArray()
                    )
                    .ToList();
            }
            else
            {
                stuImageIds.Add(tempArrImgId);
            }

            foreach (int[] imgIdArray in stuImageIds)
            {
                List<StudentImage> result = (from si in db.StudentImages
                                             where imgIdArray.Contains(si.ID)
                                             select si).ToList();
                totalResult.AddRange(result);
            }

            return totalResult;
        }


        //select school year VM
        public static int UpdatePhotoShootSeason(PhotoSorterDBModelDataContext db, int jobId, int photoShootId)
        {
            return db.ExecuteCommand("update PhotoShoot set PhotographyjobID=" + jobId + " where PhotoShotID=" + photoShootId);
        }

        # region For Validation Admincd, Yearbookcd and Validate 5-star

        //private static string getStringFromIds(int[] Ids)
        //{
        //    string retValue = "";
        //    if (Ids.Count() > 0)
        //    {
        //        for (int i = 0; i < Ids.Count(); i++)
        //        {
        //            retValue += Ids[i] + ",";
        //        }
        //        retValue = retValue.Substring(0, retValue.Length - 1);
        //    }
        //    return retValue;
        //}

        internal static DataTable GetStudentFor5Rating(int[] photoShootIds, int[] importBatchIds)
        {
            string allPhotoshootId = string.Join(",", photoShootIds); string allImportBatchId = string.Join(",", importBatchIds);

            if (!string.IsNullOrEmpty(allPhotoshootId))
            {
                String sql = "select   FirstName,  LastName,  studentid,  count(studentid) CountStudentID from   View_StudentSeasonImage " +
                    "where   PhotoshootID in(" + allPhotoshootId + ") and StudentImportID in(" + allImportBatchId + ") and rating=5 and Recordstatus=1 " +
                    "group by studentid,   firstname,   lastname having   count(studentid)> 1 " +
                    "order by   count(studentid) desc,   lastname,   firstname";
                return WCFSQLHelper.getDataSetText(sql).Tables[0];
            }
            else
                return null;
        }

        internal static System.Data.DataTable GetStudentForNot5Rating(int[] JobIds, int[] photoShootIds, int[] importBatchIds)
        {
            string allPhotoshootId = string.Join(",", photoShootIds); string allImportBatchId = string.Join(",", importBatchIds);
            string allJobID = string.Join(",", JobIds);

            //string sql = "select distinct FirstName,  LastName,  id from  student where  photographyjobID in(" + AllJobID + ")   and ID not in " +
            //"(select distinct StudentID from View_StudentSeasonImage  where  isnull(rating, 0) = 5) order by   Lastname, FirstName ";

            //Query Changed by Hemachandram on 2-12-2015 (include Students without images also)
            //string sql = "select distinct s.id as studentid,s.FirstName,s.LastName from StudentImage si inner join " +
            //             "student s on s.id = si.studentidpk where  s.StudentImportID in (" + allImportBatchId + ") and si.PhotoShootID in (" + allPhotoshootId + ") and s.ID not in " +
            //            "(select distinct StudentID from View_StudentSeasonImage  where PhotoshootID in (" + allPhotoshootId + ") " +
            //            "and StudentImportID in (" + allImportBatchId + ") and isnull(rating, 0) = 5) " + "union " + "select distinct s.ID as studentid,s.firstName,s.Lastname from student s where s.ID not in (select distinct s.id as studentid from StudentImage si inner join " +
            //            "student s on s.id = si.studentidpk where  s.StudentImportID in (" + allImportBatchId + ") and si.PhotoShootID in (" + allPhotoshootId + ")) and s.StudentImportID in (" + allImportBatchId + ") ";
            if (!string.IsNullOrEmpty(allJobID))
            {
                string sql = "select distinct s.id as studentid,s.FirstName,s.LastName from StudentImage si inner join " +
                             "student s on s.id = si.studentidpk where  s.StudentImportID in (" + allImportBatchId + ") and si.PhotoShootID in (" + allPhotoshootId + ") and s.ID not in " +
                            "(select distinct StudentID from View_StudentSeasonImage  where PhotoshootID in (" + allPhotoshootId + ") " +
                            "and StudentImportID in (" + allImportBatchId + ") and isnull(rating, 0) = 5) " +
                            "order by studentid desc,   lastname,   firstname";

                return WCFSQLHelper.getDataSetText(sql).Tables[0];
            }
            else
                return null;
        }

        # endregion

        //bulk rename masks
        internal static int UpdateMultipleMaskName(PhotoSorterDBModelDataContext db, string mskname, ArrayList arrMaskId)
        {
            string allMaskId = convertArrayListToString(arrMaskId);

            return db.ExecuteCommand(
                "update tblmask set MaskName='" + mskname + "' where MaskID in (" + allMaskId + ")"
                );
        }
        //upload images
        public static School getSchoolByName(PhotoSorterDBModelDataContext db, string schoolName)
        {
            return (from p in db.Schools where p.SchoolName == schoolName select p).SingleOrDefault();
        }
        public static string getJobYear(PhotoSorterDBModelDataContext db, int jobId)
        {
            IList<PhotographyJob> obj = db.ExecuteQuery<PhotographyJob>("select * from PhotographyJob where ID = " + jobId + "").ToList();
            return Convert.ToString(obj.First().StartYear + "-" + obj.First().EndYear);   //season changes
        }
        internal static int GetCountForPath(PhotoSorterDBModelDataContext db, string photoShootName)
        {
            return (from p in db.PhotoShoots where p.ImageFolder.Contains(photoShootName) select p).Count();
        }
        internal static int getMaxPhotoShootId(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<int>("select max(photoshotid) photoshotid from PhotoShoot").SingleOrDefault();
        }

        //Masks ViewModel
        internal static int deleteMultipleMasks(PhotoSorterDBModelDataContext db, ArrayList arrMasksId)
        {
            string allMasksId = convertArrayListToString(arrMasksId);

            return db.ExecuteCommand(
                "delete from tblmask where MaskID in (" + allMasksId + ")"
                );
        }
        public static List<MaskDetailsItem> BindMasks(PhotoSorterDBModelDataContext db)     //changed to List from IQuerable by Mohan
        {
            return db.ExecuteQuery<MaskDetailsItem>("SELECT m.maskid ,m.maskname,m.schoolid,maskdetail = STUFF((SELECT ', ' + maskdetail FROM maskdetails md WHERE m.maskid = md.maskid FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')FROM tblmask m   ORDER BY m.maskname").ToList();
            //return WCFSQLHelper.getDataSet_SP("sp_GetMaskDetails").Tables[0];     //instead of sp ,now we are using query ....changed by mohan
        }

        //File Location VM
        public static Template getTemplateByCode(PhotoSorterDBModelDataContext db, string TempCode)
        {
            return (db.ExecuteQuery<Template>("select Id,templateCode,templatePath from Templates where templateCode='" + TempCode + "'").First());
        }
        //********************** new function created on 22 oct *********************
        internal static int updateSettings(string imagePathLoc, string templateApath, string templateBackApath, string templateBpath)
        {
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@ImagePathLoc", imagePathLoc);
            param[1] = new SqlParameter("@templateApath", templateApath);
            param[2] = new SqlParameter("@templateBackApath", templateBackApath);
            param[3] = new SqlParameter("@templateBpath", templateBpath);
            return WCFSQLHelper.executeNonQuery_SP("sp_UpdateSettingsAndTemplates", param);
        }
        //*******************************

        //View PhotoShoot VM
        public static IEnumerable<PhotoShoot> getPhotoShoot(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<PhotoShoot>(
                "select * from PhotoShoot"
                ).ToList();
        }


        //Dashboard VM
        #region  code added Abhilasha
        internal static DataTable getFailedRenameStudents(PhotoSorterDBModelDataContext db)
        {
            return Conversions.ToDataTable(db.ExecuteQuery<StudentImage>("Select * from studentimage where failedrename= 1").ToList());
        }
        #endregion

        public static int deleteBulkPhotoShoot(PhotoSorterDBModelDataContext db, ArrayList arrShootId)
        {
            string allShootId = convertArrayListToString(arrShootId);

            // first have to delete all workflows related to it
            //List<int> workflowIds = new List<int>();

            try
            {
                //workflowIds = (db.ExecuteQuery<int>("select WorkflowItemId from PhotoshootWorkflowItem where PhotoShootID in (" + allShootId + ")").ToList());
                //string allworkflowIds = string.Join(",", workflowIds);

                db.ExecuteCommand("Delete PhotoshootWorkflowItem where PhotoShootID in (" + allShootId + ")");
                //if (allworkflowIds.Length > 0)
                //{
                //db.ExecuteCommand("Delete WorkflowCollectionItems where WorkflowItemId in (" + allworkflowIds + ")");
                //db.ExecuteCommand("Delete WorkflowItem where Id in (" + allworkflowIds + ")");
                //}

                //first we have to delete all the images in studentphotoorder where StudentImageId matches id of student image
                db.ExecuteCommand(
                   "DELETE w FROM studentphotoorder w INNER JOIN studentimage e ON w.StudentImageId=e.Id Where  e.PhotoShootID in (" + allShootId + ")"
                  );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return db.ExecuteCommand(
                    "delete from PhotoShoot where PhotoShotID in (" + allShootId + ")"
                    );
        }
        internal static IList<StudentImage> getStudimageAndPhotoShootFromId(PhotoSorterDBModelDataContext db, int imgId)
        {
            return (db.ExecuteQuery<StudentImage>("select * from StudentImage where ID =" + imgId + "").ToList());
        }
        internal static int updateRecordStatus(PhotoSorterDBModelDataContext db, ArrayList arrStuId, string status)
        {
            string allStuImgId = convertArrayListToString(arrStuId);

            return db.ExecuteCommand(
                "update StudentImage set Recordstatus='" + status + "' where ID in (" + allStuImgId + ")"
                );
        }
        internal static int updatePackages(PhotoSorterDBModelDataContext db, string package, int studentImageId)
        {
            return db.ExecuteCommand(
                           "update StudentImage set Packages='" + package + "' where ID in (" + studentImageId + ")"
                           );
        }
        internal static int updateOrderPackages(PhotoSorterDBModelDataContext db, int Quantity, int studentImageId, int studentPhotoOrderId, string BillingCode, int? groupImageId)
        {
            if (groupImageId == null)
                return db.ExecuteCommand("update StudentPhotoOrder set Quantity = " + Quantity + " , sp_SimplePhotoBillingCode='" + BillingCode + "', GroupImageId = NULL  where Id = " + studentPhotoOrderId);
            else
                return db.ExecuteCommand("update StudentPhotoOrder set Quantity = " + Quantity + " , sp_SimplePhotoBillingCode='" + BillingCode + "', GroupImageId = " + groupImageId + "  where Id = " + studentPhotoOrderId);
        }
        internal static int deleteErrorLog(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteCommand(
                "Delete From Renameerrorlog"
                );
        }
        public static void updateRenameSourceImagesPhotShoot(PhotoSorterDBModelDataContext db, int photoShootId)
        {
            db.ExecuteCommand("update PhotoShoot set RenameSourceImages = '1' where PhotoShotID = " + photoShootId + " ");
        }
        public static StudentImage updateStudentImgName(PhotoSorterDBModelDataContext db, int ID)
        {
            return (from p in db.StudentImages where p.ID == ID select p).SingleOrDefault();
        }
        internal static int updatePhotoshoot(PhotoSorterDBModelDataContext db, Boolean type, int photoshootid)
        {
            return db.ExecuteCommand(
                "update Photoshoot set isReduced ='" + type + "' where PhotoShotID ='" + photoshootid + "'"
                );
        }
        public static IEnumerable<PhotoShoot> getPhotoShoot(PhotoSorterDBModelDataContext db, int jobId)
        {
            return db.ExecuteQuery<PhotoShoot>(
                "select * from PhotoShoot where photographyjobid=" + jobId
                ).ToList();
        }
        public static IEnumerable<PhotoShoot> getPhotoShootNames(PhotoSorterDBModelDataContext db, string jobid)   //used in dashboard ,checkForRenameSourceImages Method   //mohan
        {
            return db.ExecuteQuery<PhotoShoot>(
                "select PhotoShotName,photoshotid from PhotoShoot where photographyjobid in (" + jobid + ") and (RenameSourceImages is null or RenameSourceImages=0)"
                ).ToList();
        }
        public static List<StudentImage> getStudentImagesbyStudentId(PhotoSorterDBModelDataContext db, string jobid)
        {
            return db.ExecuteQuery<StudentImage>(
                "select * from StudentImage where StudentIDPK=" + Convert.ToInt32(jobid)
                ).ToList();
        }
        //Added by hema for ImageQuix
        public static DataTable getGroupNameAndGroupImages(PhotoSorterDBModelDataContext db, ArrayList arrGroupIds, bool isExportAllImagesChecked, int[] arrStudentImageIds)
        {
            string groupIds = convertArrayListToString(arrGroupIds);
            if (isExportAllImagesChecked == true)
            {
                if (!string.IsNullOrEmpty(groupIds))
                {
                    return WCFSQLHelper.getDataTable(
                        "select g.GroupName,gi.GroupId as GroupId, ps.ImageFolder as ImageFolder, si.*  from StudentImage as si join GroupItem gi on gi.StudentPhotoID = si.ID join [Group] g on g.ID = gi.GroupID join PhotoShoot ps on ps.PhotoShotID = si.PhotoShootID  where g.ID in (" + groupIds + ") Order by g.GroupName"
                        );
                }
                else
                    return null;
            }
            else
            {
                string imageIds = string.Join(",", arrStudentImageIds);
                return WCFSQLHelper.getDataTable(
                    "select g.GroupName,gi.GroupId as GroupId, ps.ImageFolder as ImageFolder, si.*  from StudentImage as si join GroupItem gi on gi.StudentPhotoID = si.ID join [Group] g on g.ID = gi.GroupID join PhotoShoot ps on ps.PhotoShotID = si.PhotoShootID  where g.ID in (" + groupIds + ") and gi.StudentPhotoID in (" + imageIds + ") Order by g.GroupName");
            }
        }
        public static List<string> getstudentImageNamesByGroupId(PhotoSorterDBModelDataContext db, int groupId)
        {
            return db.ExecuteQuery<string>("select si.ImageName from StudentImage si join GroupItem gi on gi.StudentPhotoID = si.ID where gi.GroupID = " + groupId).ToList();
        }
        public static List<string> getImageNamesIdArray(PhotoSorterDBModelDataContext db, int[] imagesIds)
        {
            string imageIdArray = string.Join(",", imagesIds);
            return db.ExecuteQuery<string>("select si.ImageName from StudentImage si where si.Id in (" + imageIdArray + ")").ToList();
        }
        public static DataTable getUsersData(PhotoSorterDBModelDataContext db)
        {
            //return  (db.ExecuteQuery<StudentImport>("select * from StudentImport where SchoolID = " + schoolID).ToList()); //now we are using common UserControl to display Order Import batches and stud import batches

            DataTable dt = WCFSQLHelper.getDataTable("select Id,UserName from Users order by Id asc");
            
            return dt;
        }
        public static int updateUser(PhotoSorterDBModelDataContext db, int selectedUserID, string userName)        
        {
            return db.ExecuteCommand("update Users set UserName = '" + userName + "' where Id = " + selectedUserID);
        }
        public static int deleteUser(PhotoSorterDBModelDataContext db, ArrayList selectedUserIDs)
        {
            string allUserIds = convertArrayListToString(selectedUserIDs);
            return db.ExecuteCommand("delete Users where Id in (" + allUserIds + ")");
            //else
            //    return db.ExecuteCommand("delete Users");
        }
        public static int createNewUser(PhotoSorterDBModelDataContext db, string userName)
        {
            return db.ExecuteCommand("Insert into Users (UserName) values ('" + userName + "')");
        }

        public static int deleteSPPriceSheets(PhotoSorterDBModelDataContext db, ArrayList selectedUserIDs)
        {
            string allUserIds = convertArrayListToString(selectedUserIDs);
            return db.ExecuteCommand("delete SimplePhotoPriceSheet where Id in (" + allUserIds + ")");
            //else
            //    return db.ExecuteCommand("delete Users");
        }
        public static List<StudentImage> getStudentImagesAcrossYearsbyStudentId(PhotoSorterDBModelDataContext db, string StudentName)
        {
            //return db.ExecuteQuery<StudentImage>("Select * from StudentImage where studentIDPK in (SELECT ID FROM [PhotoForce4].[dbo].[View_StudentSchool] " +
            //                                        " where Firstname + Lastname = '" + StudentName + "' and cast(StudentID as varchar(100)) = '" + StudentID + "')").ToList();

            return db.ExecuteQuery<StudentImage>("select * from StudentImage where ImageName like '" + StudentName + "' ").ToList();
        }
        #region  code added Abhilasha
        internal static int _UpdateStudentImgName(PhotoSorterDBModelDataContext db, int ImgID, string imagename)
        {
            return db.ExecuteCommand(
                "update StudentImage set ImageName='" + imagename.Replace("'", "''") + "' where ID in (" + ImgID + ")"
                );
        }

        internal static int _UpdateOriginalStudentImgName(PhotoSorterDBModelDataContext db, int ImgID, int status)
        {
            if (status == 0)
                return db.ExecuteCommand(
                    "update StudentImage set OriginalImageName=imagename,failedrename=" + status + " where ID in (" + ImgID + ")"
                    );
            else
                return db.ExecuteCommand(
                               "update StudentImage set failedrename=" + status + " where ID in (" + ImgID + ")"
                               );
        }
        #endregion

        public static int UpdateMultipleConatctsName(PhotoSorterDBModelDataContext db, int number, string Contactname, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Contact" + number + "Name='" + Contactname + "' where ID in (" + allSchoolId + ")"
                );
        }
        public static int UpdateMultipleConatctsType(PhotoSorterDBModelDataContext db, int number, string ContactType, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Contact" + number + "Type='" + ContactType + "' where ID in (" + allSchoolId + ")"
                );
        }
        public static int UpdateMultipleConatctsEmail(PhotoSorterDBModelDataContext db, int number, string ContactEmail, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Contact" + number + "Email='" + ContactEmail + "' where ID in (" + allSchoolId + ")"
                );
        }
        public static int UpdateMultipleConatctsNotes(PhotoSorterDBModelDataContext db, int number, string Contactnotes, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Contact" + number + "Notes='" + Contactnotes + "' where ID in (" + allSchoolId + ")"
                );
        }
        public static int UpdateMultipleAffiliation(PhotoSorterDBModelDataContext db, string Affiliation, ArrayList arrSchoolId)
        {
            string allSchoolId = convertArrayListToString(arrSchoolId);

            return db.ExecuteCommand(
                "update school set Affiliation = '" + Affiliation + "' where ID in (" + allSchoolId + ")"                
                );
        }
        //public static StudentImage getImage(PhotoSorterDBModelDataContext db, int StudentID)
        //{
        //    StudentImage si = new StudentImage();

        //    int[] stuImageIds = db.ExecuteQuery<int>("select * from StudentImage where ID = " + StudentID ).ToArray();
        //    string imageIds = string.Join(",", stuImageIds);
        //    si = db.ExecuteQuery<StudentImage>(
        //            "select si.ImageName, si.ImageNumber,sc.SchoolName,s.FirstName,s.Lastname,s.Teacher,s.Grade,si.Rating, si.yearbook,si.Admincd from StudentImage si join Student s on s.ID = si.StudentIDPK join School sc on sc.ID = si.SchoolID where s.Recordstatus = 'true' and si.Recordstatus = 'Active' and si.ID in (" + imageIds + ")").FirstOrDefault();

        //    return si;
        //}
        public static int clearStudentBatch(PhotoSorterDBModelDataContext db, ArrayList StudentIDs, bool isUpdate)
        {
            if (isUpdate)
            {
                Setting ClearingRecord = db.ExecuteQuery<Setting>("select * from settings where settingName = 'ClearingCorrectStudents'").FirstOrDefault();
                if (ClearingRecord != null && !string.IsNullOrEmpty(ClearingRecord.settingValue))
                {
                    string tempIds = ClearingRecord.settingValue;

                    string[] oldIds = tempIds.Split(',');

                    foreach (string id in oldIds)
                    {
                        if (!StudentIDs.Contains(Convert.ToInt32(id)))
                            StudentIDs.Add(Convert.ToInt32(id));
                    }
                }
            }
            string updateString = convertArrayListToString(StudentIDs);
            string query = (isUpdate == true) ? ("update Settings set settingValue = '" + updateString + "' where settingName = 'ClearingCorrectStudents'") : "Insert into Settings (settingName,settingValue) values ('ClearingCorrectStudents','" + updateString + "')";
            return db.ExecuteCommand(query);
        }
        public static List<string> getPhotographyJobData(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<string>("select Distinct(JobName)from PhotographyJob").ToList();
        }
        public static List<PhotoShoot> getPhotoShootsWithoutWorkflows(PhotoSorterDBModelDataContext db, string studio, string schoolYear)
        {
            if (string.IsNullOrEmpty(studio) && string.IsNullOrEmpty(schoolYear))
                return db.ExecuteQuery<PhotoShoot>("select * from PhotoShoot ps left outer join " +
                                                        "PhotographyJob pj on pj.Id = ps.PhotographyjobID left outer join " +
                                                        "School sc on sc.Id = pj.SchoolID left outer join " +
                                                        "Studio stu on stu.Id = sc.StudioId where ps.PhotoShotID not in (select distinct(PhotoShootID) from PhotoShootWorkflowItem)").ToList();
            else
                return db.ExecuteQuery<PhotoShoot>("select * from PhotoShoot ps left outer join " +
                                                        "PhotographyJob pj on pj.Id = ps.PhotographyjobID left outer join " +
                                                        "School sc on sc.Id = pj.SchoolID left outer join " +
                                                        "Studio stu on stu.Id = sc.StudioId where ps.PhotoShotID not in (select distinct(PhotoShootID) from PhotoShootWorkflowItem) " +
                                                        "and pj.JobName = '" + schoolYear + "' and stu.StudioName = '" + studio + "'  order by ps.PhotoShotID").ToList();

        }
    }
}
