using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsSchool
    {
        #region Default School
        //defaultschool
        public static int defaultSchoolId;
        public static string defaultSchoolName;
        public static int previousSchoolId;

        public static School getFirstSchool(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<School>("select top 1 * from school").FirstOrDefault();
        }
        public static bool updateDefaultSchoolRegistry(string schoolName, int schoolId)
        {
            try
            {
                defaultSchoolId = schoolId;
                defaultSchoolName = schoolName;
                RegistryKey PhotoForce = Registry.CurrentUser.CreateSubKey(@"Software\Photo Sorter");
                using (RegistryKey DefaultSchool = PhotoForce.CreateSubKey("DefaultSchool"))
                {
                    // Create data for the DefaultSchool subkey.
                    DefaultSchool.SetValue("SchoolName", schoolName, RegistryValueKind.String);
                    DefaultSchool.SetValue("SchoolID", schoolId, RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return false;
            }
            return true;
        }

        public static void getDefaultSchoolFromRegistry(string schoolName, int schoolId)
        {
            RegistryKey PhotoForce = Registry.CurrentUser.OpenSubKey(@"Software\Photo Sorter\DefaultSchool");
            if (PhotoForce != null)
            {
                defaultSchoolId = Convert.ToInt32(PhotoForce.GetValue("SchoolID").ToString());
                defaultSchoolName = PhotoForce.GetValue("SchoolName").ToString();
            }
            else
            {
                bool status = updateDefaultSchoolRegistry(schoolName, schoolId);
            }
        }

        public static int getPreviousSchoolId()
        {
            int prevSchoolId = 0;
            RegistryKey PhotoForce = Registry.CurrentUser.OpenSubKey(@"Software\Photo Sorter\DefaultSchool");
            if (PhotoForce != null)
            {
                prevSchoolId = Convert.ToInt32(PhotoForce.GetValue("SchoolID"));
            }
            return prevSchoolId;
        }

        public static List<School> getAllSchools(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<School>("select * from school order by schoolname asc").ToList();
        }

        public static int insertSchoolPackages(PhotoSorterDBModelDataContext db, int schoolId)
        {
            return db.ExecuteQuery<int>("insert Into package(SchoolID,item,package,price,defaultval) select " + schoolId + ",item,package,price,0 from DefaultPackages").FirstOrDefault();
        }
        public static int getSchoolId(PhotoSorterDBModelDataContext db, string SchoolName)
        {
            return (from s in db.Schools where s.SchoolName == SchoolName select s.ID).FirstOrDefault();
        }
        public static bool mergeSchools(PhotoSorterDBModelDataContext db, List<int> selectedSchoolIds, string mergedSchoolName, string survivingSchool)
        {
            try
            {
                //selectedSchoolIds[0] is Merged school
                //selectedSchoolIds[1] is Survival school
                db.ExecuteCommand("update Student set SchoolCampus = '" + mergedSchoolName + "' where StudentImportID in (select ID from StudentImport where SchoolID = " + selectedSchoolIds[0] + ")");
                db.ExecuteCommand("update StudentImport set SchoolID =" + selectedSchoolIds[1] + "where SchoolID = " + selectedSchoolIds[0]);

                db.ExecuteCommand("update SchoolImageNameFormat set SchoolID =" + selectedSchoolIds[1] + "where SchoolID = " + selectedSchoolIds[0]);
                db.ExecuteCommand("update tblmask set SchoolID =" + selectedSchoolIds[1] + "where schoolid = " + selectedSchoolIds[0]);
                db.ExecuteCommand("update PhotographyJob set SchoolID =" + selectedSchoolIds[1] + "where SchoolID = " + selectedSchoolIds[0]);
                db.ExecuteCommand("update Package set SchoolID =" + selectedSchoolIds[1] + "where schoolId = " + selectedSchoolIds[0]);
                db.ExecuteCommand("update StudentImage set SchoolID =" + selectedSchoolIds[1] + "where SchoolID = " + selectedSchoolIds[0]);
                db.ExecuteCommand("update Activities set SchoolID =" + selectedSchoolIds[1] + "where SchoolId = " + selectedSchoolIds[0]);
                db.ExecuteCommand("update StudentPhotoOrder set SchoolID =" + selectedSchoolIds[1] + "where SchoolId = " + selectedSchoolIds[0]);
                db.ExecuteCommand("update OrdersImport set SchoolID =" + selectedSchoolIds[1] + "where SchoolID = " + selectedSchoolIds[0]);
                db.ExecuteCommand("update [Group] set SchoolID =" + selectedSchoolIds[1] + "where SchoolID = " + selectedSchoolIds[0]);
                //updating school campus name for student if column is null
                db.ExecuteCommand("update Student set SchoolCampus = '" + survivingSchool + "' where StudentImportID in (select ID from StudentImport where SchoolID = " + selectedSchoolIds[1] + ") and SchoolCampus is null");
                db.ExecuteCommand("update School set IsActive = 'false' where ID = " + selectedSchoolIds[0]);
                //db.ExecuteCommand("update DefaultSchool set SchoolID =" + selectedSchoolIds[1] + "where schoolid = " + selectedSchoolIds[0]); //no need to update this
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return false;
            }
            return true;
        }
        public static List<int> getSelectedSchools(PhotoSorterDBModelDataContext db, List<int> selectedIds)
        {
            if (selectedIds.Count > 0)
            {
                string allSchoolIds = "";

                for (int i = 0; i < selectedIds.Count; i++)
                {
                    allSchoolIds += selectedIds[i] + ",";
                }
                allSchoolIds = allSchoolIds.Substring(0, allSchoolIds.Length - 1);

                return db.ExecuteQuery<int>("select Id from School where Id not in (" + allSchoolIds + ")").ToList();
            }
            else
                return db.ExecuteQuery<int>("select Id from School").ToList();
        }
        #endregion
    }
}
