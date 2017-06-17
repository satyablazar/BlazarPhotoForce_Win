using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using PhotoForce.MVVM;
using PhotoForce.Extensions;
using PhotoForce.Helpers;

namespace PhotoForce.App_Code
{
    public static class clsStudent
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

        public static List<Student> getStudentsDetailsClass(PhotoSorterDBModelDataContext db, int schoolId)
        {
            //return db.ExecuteQuery<Student>("select distinct s.*  from student s where s.ID not in (select distinct s.id as studentid from StudentImage si inner join " +
            //"student s on s.id = si.studentidpk where  s.StudentImportID in (" + studentImportIds + ") and si.PhotoShootID in (" + photoShootIds + ")) and s.StudentImportID in (" + studentImportIds + ") order by s.Id  ").ToList();
            List<Student> lstStudentDetails = new List<Student>();
            lstStudentDetails = (from p in db.Students where p.RecordStatus == true && p.StudentImport.SchoolID == schoolId orderby p.StudentImport.ID, p.Lastname select p).ToList();
            return lstStudentDetails;
        }
        public static Student updateStudent(PhotoSorterDBModelDataContext db, int stuId)
        {
            return (from p in db.Students where p.ID == stuId && p.RecordStatus == true select p).SingleOrDefault();
        }
        public static StudentImage getStudentSeasonImageByName(PhotoSorterDBModelDataContext db, string imageName)
        {
            //return (from p in db.View_StudentSeasonImages where p.ImageName == imageName select p).FirstOrDefault();
            return (db.ExecuteQuery<StudentImage>("select * from StudentImage where ImageName = '" + imageName + "'").FirstOrDefault());
        }
        static string removeApostrophe(string oldString)
        {
            return oldString.Replace("'", "''"); //apostrophe causing unhandled exception
        }


        internal static int UpdateSingleSchoolYear(PhotoSorterDBModelDataContext db, int? photographyJobId, int StuId)
        {
            return db.ExecuteCommand(
                "update Student set PhotographyJobID='" + photographyJobId + "' where ID in (" + StuId + ")"
                );
        }
        public static IEnumerable<Student> getStudentDetails(PhotoSorterDBModelDataContext db, int studentId)
        {
            return (db.ExecuteQuery<Student>("select * from Student where ID = " + studentId + " and RecordStatus='true'").ToList());
        }

        public static int deletestudents(PhotoSorterDBModelDataContext db, ArrayList arrStuId)
        {
            string allStuId = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(allStuId)) { return 0; }

            return db.ExecuteCommand(
                "delete from student where ID in (" + allStuId + ")"
                );
        }

        public static DataTable listToDataTable(this List<Student> items)
        {
            string[] unWantedColumns = new string[] { "PhotographyJobID", "StudentImages", "StudentImport", "PhotographyJob" };
            var tb = new DataTable(typeof(Student).Name);
            PropertyInfo[] props = typeof(Student).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            try
            {
                foreach (var prop in props)
                {
                    if (unWantedColumns.Contains(((System.Reflection.MemberInfo)(prop)).Name)) { continue; }
                    // Check for nullable as well..
                    tb.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                if (!tb.Columns.Contains("SchoolYear")) { tb.Columns.Add("SchoolYear"); }
                foreach (var item in items)
                {
                    DataRow dr = tb.NewRow();
                    if (((Student)item).DOB != null) { dr["DOB"] = ((Student)item).DOB; }
                    if (((Student)item).PhotographyJob != null) { dr["SchoolYear"] = ((Student)item).PhotographyJob.JobName; }
                    if (((Student)item).StudentImport != null) { dr["StudentImportID"] = ((Student)item).StudentImport.ID; }
                    if (((Student)item).CreatedOn != null) { dr["CreatedOn"] = ((Student)item).CreatedOn; }
                    dr["ID"] = ((Student)item).ID; dr["FirstName"] = ((Student)item).FirstName; dr["Lastname"] = ((Student)item).Lastname;
                    dr["RecordStatus"] = ((Student)item).RecordStatus;
                    dr["Address"] = ((Student)item).Address; dr["City"] = ((Student)item).City; dr["State"] = ((Student)item).State;
                    dr["Zip"] = ((Student)item).Zip; dr["Phone"] = ((Student)item).Phone; dr["Emailaddress"] = ((Student)item).Emailaddress;
                    dr["StudentID"] = ((Student)item).StudentID; dr["Password"] = ((Student)item).Password; dr["Teacher"] = ((Student)item).Teacher;
                    dr["Grade"] = ((Student)item).Grade; dr["Custom1"] = ((Student)item).Custom1;
                    dr["Custom2"] = ((Student)item).Custom2; dr["Custom3"] = ((Student)item).Custom3; dr["Custom4"] = ((Student)item).Custom4; dr["Custom5"] = ((Student)item).Custom5;
                    dr["SchoolCampus"] = ((Student)item).SchoolCampus;
                    tb.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
            return tb;
        }
        internal static DataTable GetAllStudentsForExport(PhotoSorterDBModelDataContext db, ArrayList arrStudentId)
        {
            string AllStuID = "";
            AllStuID = convertArrayListToString(arrStudentId);
            if (string.IsNullOrEmpty(AllStuID)) { return null; }
            //Added by Mohan
            //check ToDataTable Method , Filtered Unwanted Columns ,so that we can easily export data to EXCEL
            //have to find better way than this.
            return listToDataTable(db.ExecuteQuery<Student>(
                "select * from Student where ID in (" + AllStuID + ") and RecordStatus='true'"
                ).ToList());
        }

        //used in import stduent view model
        //Commented by Mohan
        //Not using in code 
        //public static int insertImportedStudentData(DataRow dr, int ImportID, int opcode, int? photographyJobID, int rwcount)
        //{
        //    SqlParameter[] param;
        //    if (rwcount <= 12)
        //    {
        //        param = new SqlParameter[21];
        //        param[0] = new SqlParameter("@opcode", opcode);
        //        param[1] = new SqlParameter("@StudentImportID", ImportID);
        //        param[2] = new SqlParameter("@FirstName", Convert.ToString(dr["First Name"]));
        //        param[3] = new SqlParameter("@LastName", Convert.ToString(dr["Last name"]));
        //        param[4] = new SqlParameter("@StudentID", Convert.ToString(dr["Student ID"]));
        //        ////param[5] = new SqlParameter("@Password", Convert.ToString(dr["Password"]));
        //        //if(dr["Password"] == null)
        //        //    param[5] = new SqlParameter("@Password", DBNull.Value);
        //        //else
        //        param[5] = new SqlParameter("@Password", Convert.ToString(dr["Password"]));
        //        param[6] = new SqlParameter("@Teacher", Convert.ToString(dr["Teacher"]));
        //        param[7] = new SqlParameter("@Grade", Convert.ToString(dr["Grade"]));
        //        param[8] = new SqlParameter("@Custom1", Convert.ToString(dr["Custom1"]));
        //        param[9] = new SqlParameter("@Custom2", Convert.ToString(dr["Custom2"]));
        //        param[10] = new SqlParameter("@Custom3", Convert.ToString(dr["Custom3"]));
        //        param[11] = new SqlParameter("@Custom4", Convert.ToString(dr["Custom4"]));
        //        param[12] = new SqlParameter("@Custom5", Convert.ToString(dr["Custom5"]));
        //        //param[13] = new SqlParameter("@SchoolYear", SchoolYear);
        //        param[13] = new SqlParameter("@PhotographyJobID", photographyJobID);
        //        param[14] = new SqlParameter("@Address", "");
        //        param[15] = new SqlParameter("@DOB", DBNull.Value);
        //        param[16] = new SqlParameter("@City", "");
        //        param[17] = new SqlParameter("@State", "");
        //        param[18] = new SqlParameter("@Zip", "");
        //        param[19] = new SqlParameter("@Phone", "");
        //        param[20] = new SqlParameter("@Emailaddress", "");
        //    }
        //    else
        //    {
        //        param = new SqlParameter[21];
        //        param[0] = new SqlParameter("@opcode", opcode);
        //        param[1] = new SqlParameter("@StudentImportID", ImportID);
        //        param[2] = new SqlParameter("@FirstName", Convert.ToString(dr["First Name"]));
        //        param[3] = new SqlParameter("@LastName", Convert.ToString(dr["Last name"]));
        //        param[4] = new SqlParameter("@StudentID", Convert.ToString(dr["Student ID"]));
        //        //param[5] = new SqlParameter("@Password", Convert.ToString(dr["Password"]));
        //        //if (dr["Password"] == null)
        //        //    param[5] = new SqlParameter("@Password", DBNull.Value);
        //        //else
        //        param[5] = new SqlParameter("@Password", Convert.ToString(dr["Password"]));
        //        param[6] = new SqlParameter("@Teacher", Convert.ToString(dr["Teacher"]));
        //        param[7] = new SqlParameter("@Grade", Convert.ToString(dr["Grade"]));
        //        param[8] = new SqlParameter("@Custom1", Convert.ToString(dr["Custom1"]));
        //        param[9] = new SqlParameter("@Custom2", Convert.ToString(dr["Custom2"]));
        //        param[10] = new SqlParameter("@Custom3", Convert.ToString(dr["Custom3"]));
        //        param[11] = new SqlParameter("@Custom4", Convert.ToString(dr["Custom4"]));
        //        param[12] = new SqlParameter("@Custom5", Convert.ToString(dr["Custom5"]));
        //        //param[13] = new SqlParameter("@SchoolYear", SchoolYear);
        //        param[13] = new SqlParameter("@PhotographyJobID", photographyJobID);
        //        param[14] = new SqlParameter("@DOB", Convert.ToString(dr["DOB"]) == "" ? (object)DBNull.Value : Convert.ToDateTime(dr["DOB"]));
        //        param[15] = new SqlParameter("@Address", Convert.ToString(dr["Address"]));
        //        param[16] = new SqlParameter("@City", Convert.ToString(dr["City"]));
        //        param[17] = new SqlParameter("@State", Convert.ToString(dr["State"]));
        //        param[18] = new SqlParameter("@Zip", Convert.ToString(dr["Zip"]));
        //        param[19] = new SqlParameter("@Phone", Convert.ToString(dr["Phone"]));
        //        param[20] = new SqlParameter("@Emailaddress", Convert.ToString(dr["Emailaddress"]));
        //    }

        //    return WCFSQLHelper.executeNonQuery_SP("sp_InsertImportedStudentData", param);
        //}

        //used in count images
        internal static List<CountImagesNStudents> CountImages(PhotoSorterDBModelDataContext db, ArrayList arrPhotographyJobID) //Changed from IEnumerable to List  //Mohan
        {
            String jobId = arrPhotographyJobID[0].ToString();
            return db.ExecuteQuery<CountImagesNStudents>("Select (s.FirstName+' '+s.Lastname) as Name, COUNT(firstname+lastname) as 'Total' from Student s where  s.photographyjobid = " + jobId + " and s.recordstatus=1 group by  s.FirstName,s.Lastname having count(firstname+lastname) > 1 order by total desc").ToList();
        }

        #region Bulk Rename Student
        internal static int UpdateMultipleStudentID(PhotoSorterDBModelDataContext db, string StudentID, ArrayList arrStuId)
        {

            string AllStudentID = convertArrayListToString(arrStuId);

            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set StudentID='" + StudentID + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleStudentTitle(PhotoSorterDBModelDataContext db, string Title, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Title='" + Title + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleStudentIdentity(PhotoSorterDBModelDataContext db, string isStudent, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set IsStudent='" + isStudent + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleSchoolCampus(PhotoSorterDBModelDataContext db, string schoolCampus, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set SchoolCampus ='" + schoolCampus + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleStudentFirstName(PhotoSorterDBModelDataContext db, string Firstname, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set FirstName='" + Firstname + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleStudentLastName(PhotoSorterDBModelDataContext db, string LastName, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set LastName='" + LastName + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleStudentTeacher(PhotoSorterDBModelDataContext db, string Teacher, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Teacher='" + Teacher + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleStudentGrade(PhotoSorterDBModelDataContext db, string Grade, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Grade='" + Grade + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleCustom1(PhotoSorterDBModelDataContext db, string Custom1, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Custom1='" + Custom1 + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleCustom2(PhotoSorterDBModelDataContext db, string Custom2, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Custom2='" + Custom2 + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleCustom3(PhotoSorterDBModelDataContext db, string Custom3, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Custom3='" + Custom3 + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleCustom4(PhotoSorterDBModelDataContext db, string Custom4, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Custom4='" + Custom4 + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleCustom5(PhotoSorterDBModelDataContext db, string Custom5, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Custom5='" + Custom5 + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleAddress(PhotoSorterDBModelDataContext db, string address, ArrayList arrStuimgId)
        {
            string AllStudentID = convertArrayListToString(arrStuimgId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Address='" + address + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleCitystudent(PhotoSorterDBModelDataContext db, string City, ArrayList arrStuimgId)
        {
            string AllStudentID = convertArrayListToString(arrStuimgId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set City='" + City + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleStatestudent(PhotoSorterDBModelDataContext db, string State, ArrayList arrStuimgId)
        {
            string AllStudentID = convertArrayListToString(arrStuimgId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set State='" + State + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleZipstudent(PhotoSorterDBModelDataContext db, string zip, ArrayList arrStuimgId)
        {
            string AllStudentID = convertArrayListToString(arrStuimgId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Zip='" + zip + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultiplePhone(PhotoSorterDBModelDataContext db, string phone, ArrayList arrStuimgId)
        {
            string AllStudentID = convertArrayListToString(arrStuimgId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Phone='" + phone + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleEmailAddress(PhotoSorterDBModelDataContext db, string emailaddress, ArrayList arrStuimgId)
        {
            string AllStudentID = convertArrayListToString(arrStuimgId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set Emailaddress='" + emailaddress + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleDOB(PhotoSorterDBModelDataContext db, string dob, ArrayList arrStuimgId)
        {
            string AllStudentID = convertArrayListToString(arrStuimgId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set DOB='" + dob + "' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultiplePasswords(PhotoSorterDBModelDataContext db, string txtPassword, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            if (txtPassword != null)
                return db.ExecuteCommand(
                    "update Student set [Password]='" + txtPassword + "' where ID in (" + AllStudentID + ")"
                    );
            else
                return db.ExecuteCommand(
                "update Student set [Password]=null where ID in (" + AllStudentID + ")"
                );
        }
        internal static int UpdateMultipleSchoolYear(PhotoSorterDBModelDataContext db, int? photographyJobID, ArrayList arrStuId)
        {
            string AllStudentID = convertArrayListToString(arrStuId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set PhotographyJobID='" + photographyJobID + "' where ID in (" + AllStudentID + ")" // ,SchoolYear ='" + JobName + "' 
                );
        }
        #endregion

        //Generate PWD
        internal static DataTable getAllStudents(PhotoSorterDBModelDataContext db, int schoolId)
        {
            return Conversions.ToDataTable(getStudentsDetailsClass(db, schoolId));
        }

        internal static DataTable getSelectedStudents(PhotoSorterDBModelDataContext db, ArrayList arrStudentId)
        {
            string AllStudentID = convertArrayListToString(arrStudentId);
            if (string.IsNullOrEmpty(AllStudentID)) { return null; }

            return Conversions.ToDataTable(db.ExecuteQuery<Student>(
                "select * from Student where ID in (" + AllStudentID + ") and RecordStatus='true'"
                ).ToList());
        }

        //Activate Students VM
        internal static int UpdateStudentStatus(PhotoSorterDBModelDataContext db, ArrayList arrStudentId)
        {
            string AllStudentID = convertArrayListToString(arrStudentId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set RecordStatus='true' where ID in (" + AllStudentID + ")"
                );
        }

        //universal student seach VM
        public static IEnumerable<Student> getStudentsByUniversalSearch(PhotoSorterDBModelDataContext db, string searchString)
        {
            return db.ExecuteQuery<Student>("SELECT * From Student WHERE CONCAT(FirstName,' ',Lastname) like '" + "%" + searchString + "%" + "' and RecordStatus = 1").ToList();
        }

        //universal Image seach VM
        public static IEnumerable<StudentImage> getStudentImagesByUniversalSearch(PhotoSorterDBModelDataContext db, string searchString)
        {
            string query = "SELECT * From StudentImage WHERE ImageName like '%" + searchString + "%' and RecordStatus = " + "'Active'";
            return db.ExecuteQuery<StudentImage>(query).ToList();
        }

        //upload student images
        internal static string getStudentFirstName(PhotoSorterDBModelDataContext db, string studentname)
        {
            return (from p in db.Students orderby (p.CreatedOn) descending where p.FirstName.Contains(studentname) && p.RecordStatus == true select p.FirstName).FirstOrDefault();
        }
        internal static int getMaxStudentId(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<int>("select max(id) ID from student").SingleOrDefault();
        }
        internal static int updateStudentId(PhotoSorterDBModelDataContext db, int id)
        {
            return db.ExecuteCommand(
                "update Student set StudentID=" + id + " where ID = " + id + "");
        }
        //student VM
        internal static int DeactivateStudent(PhotoSorterDBModelDataContext db, ArrayList arrSelectedStudId)
        {
            string AllStudentID = convertArrayListToString(arrSelectedStudId);
            if (string.IsNullOrEmpty(AllStudentID)) { return 0; }

            return db.ExecuteCommand(
                "update Student set RecordStatus='false' where ID in (" + AllStudentID + ")"
                );
        }
        internal static int swapStudentFirstNLastNames(PhotoSorterDBModelDataContext db, string FirstName, string LastName, int StudentID)
        {
            return db.ExecuteCommand(
                "update Student set FirstName = '"+ LastName + "', Lastname = '"+ FirstName +"' where ID = " + StudentID );
        }
        internal static int deleteManualStudents(PhotoSorterDBModelDataContext db, int SchoolID)
        {
            return db.ExecuteCommand(
                "Delete Student where ID in (select Distinct s.ID from Student s join StudentImport stui on stui.ID = s.StudentImportID where stui.SchoolID = " + SchoolID + " and s.FirstName like 'Manual Import%' and s.ID not in (select Distinct StudentIDPK from StudentImage si join student s on s.ID = si.StudentIDPK where si.SchoolID = " + SchoolID + "))");
        }
        internal static DataTable GetAllGotPhotoStudentsForExport(PhotoSorterDBModelDataContext db, ArrayList arrStudentId)
        {
            string AllStuID = "";
            AllStuID = convertArrayListToString(arrStudentId);
            if (string.IsNullOrEmpty(AllStuID)) { return null; }
            //Added by Hema
            //check ToDataTable Method , Filtered Unwanted Columns ,so that we can easily export data to EXCEL
            //have to find better way than this.
            //string query = "select ID, Password,FirstName,Lastname,Teacher,Zip,City,State from Student where ID in (" + AllStuID + ") and RecordStatus='true'";

            string query = "select Password,FirstName,Lastname,Teacher,s.Zip,s.City,s.State,sc.SchoolName  as SchoolName, s.Grade,s.Emailaddress from Student s " +
                                "join StudentImport si on si.ID = s.StudentImportID " +
                                "join School sc on sc.ID = si.SchoolID " +
                                "where s.ID in (" + AllStuID + ") and RecordStatus='true'";

            return listToGotPhotoDataTable(WCFSQLHelper.getDataTable(query));

        }

        public static DataTable listToGotPhotoDataTable(this DataTable items)
        {
            //string[] unWantedColumns = new string[] { "ID", "StudentImportID", "FirstName", "Lastname", "StudentID", "Password", "Teacher", "Grade", "Custom1", "Custom2", "Custom3", "Custom4", "Custom5",
            //                                          "CreatedOn", "RecordStatus", "DOB", "Address", "City", "State", "Zip", "Phone", "Emailaddress", "PhotographyJobID", "IsStudent", "Title", "SchoolCampus", "OfficialFirstName",
            //                                          "OfficialLastName",  "StudentImages", "PhotographyJob", "SchoolName" };

            var tb = new DataTable(typeof(DataTable).Name);
            //PropertyInfo[] props = typeof(DataTable).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            try
            {
                //foreach (var prop in props)
                //{
                //    if (unWantedColumns.Contains(((System.Reflection.MemberInfo)(prop)).Name)) { continue; }  // this functionality for filtering unwanted columns and 
                //    // Check for nullable as well..
                //    tb.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                //}
                #region columns
                                
                if (!tb.Columns.Contains("Firstname")) { tb.Columns.Add("Firstname"); }
                if (!tb.Columns.Contains("Lastname")) { tb.Columns.Add("Lastname"); }
                if (!tb.Columns.Contains("Gender")) { tb.Columns.Add("Gender"); }
                if (!tb.Columns.Contains("Birthdate")) { tb.Columns.Add("Birthdate"); }
                if (!tb.Columns.Contains("Institution")) { tb.Columns.Add("Institution"); }
                if (!tb.Columns.Contains("Identifier")) { tb.Columns.Add("Identifier"); }
                if (!tb.Columns.Contains("Group")) { tb.Columns.Add("Group"); }
                if (!tb.Columns.Contains("Teacher")) { tb.Columns.Add("Teacher"); }
                if (!tb.Columns.Contains("Expires")) { tb.Columns.Add("Expires"); }
                if (!tb.Columns.Contains("Street")) { tb.Columns.Add("Street"); }
                if (!tb.Columns.Contains("Zip")) { tb.Columns.Add("Zip"); }
                if (!tb.Columns.Contains("City")) { tb.Columns.Add("City"); }
                if (!tb.Columns.Contains("State")) { tb.Columns.Add("State"); }
                if (!tb.Columns.Contains("Buyer Firstname")) { tb.Columns.Add("Buyer Firstname"); }
                if (!tb.Columns.Contains("Buyer Lastname")) { tb.Columns.Add("Buyer Lastname"); }
                if (!tb.Columns.Contains("Buyer Email")) { tb.Columns.Add("Buyer Email"); }
                if (!tb.Columns.Contains("Buyer Phone")) { tb.Columns.Add("Buyer Phone"); }
                if (!tb.Columns.Contains("Buyer2 Firstname")) { tb.Columns.Add("Buyer2 Firstname"); }
                if (!tb.Columns.Contains("Buyer2 Lastname")) { tb.Columns.Add("Buyer2 Lastname"); }
                if (!tb.Columns.Contains("Buyer2 Email")) { tb.Columns.Add("Buyer2 Email"); }
                if (!tb.Columns.Contains("Buyer2 Phone")) { tb.Columns.Add("Buyer2 Phone"); }
                if (!tb.Columns.Contains("Access Code")) { tb.Columns.Add("Access Code"); }
                #endregion
                foreach (DataRow item in items.Rows)
                {
                    DataRow dr = tb.NewRow();
                    dr["Firstname"] = ((DataRow)item)["FirstName"]; dr["Lastname"] = ((DataRow)item)["Lastname"];
                    dr["Gender"] = ""; dr["Birthdate"] = "";
                    dr["Institution"] = ((DataRow)item)["SchoolName"] != null ? ((DataRow)item)["SchoolName"] : "";
                    dr["Identifier"] = ""; dr["Group"] = ((DataRow)item)["Grade"];
                    dr["Teacher"] = ((DataRow)item)["Teacher"]; dr["Expires"] = "";
                    dr["Street"] = ""; dr["Zip"] = ((DataRow)item)["Zip"];
                    dr["City"] = ((DataRow)item)["City"]; dr["State"] = ((DataRow)item)["State"];
                    dr["Buyer Firstname"] = "";    dr["Buyer Lastname"] = "";
                    dr["Buyer Email"] = ((DataRow)item)["Emailaddress"]; dr["Buyer Phone"] = "";
                    dr["Buyer2 Firstname"] = ""; dr["Buyer2 Lastname"] = "";
                    dr["Buyer2 Email"] = ((DataRow)item)["Emailaddress"]; dr["Buyer2 Phone"] = "";
                    dr["Access Code"] = ((DataRow)item)["Password"];

                    tb.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
            return tb;
        }
    }
}
