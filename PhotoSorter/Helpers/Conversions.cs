using PhotoForce.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.Helpers
{
    public static class Conversions
    {
        // By rahul ...
        // Convert ienumerable to datatable
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                // Check for nullable as well..
                tb.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                tb.Rows.Add(values);
            }
            return tb;
        }

        //used in CreateExcel 
        public static DataTable listToDataTable(this List<StudentImage> items)   //to remove unwanted columns       added by mohan
        {
            ArrayList arrStudID = new ArrayList();
            string[] unWantedColumns = new string[] { "DOB", "SchoolID", "PhotoShootID", "QuixieID", "Teacher", "Grade", "HomeRoom", "Address", "Emailaddress",
                "City", "FailedRename","State", "Zip", "Phone", "Custom4", "Custom5", "Custom6", "Custom7", "Custom8", "Packages","OriginalImageName", "yearbook", "Multipose", "Ticketcode", "Rating", "RecordStatus", "StudentIDPK",
                "School", "Student", "PhotoShoot","ImageName", "ImageNumber", "Admincd", "GroupClassPhotos", "GroupItems","StudentImportID","CreatedOn","SchoolYear","PhotographyJobID","StudentImages","StudentImport","PhotographyJob" };
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
                foreach (var item in items)
                {
                    if (!arrStudID.Contains(((StudentImage)item).Student.ID))        //instead of filtering duplicate students here,check the possibility through query...
                    {
                        DataRow dr = tb.NewRow();
                        dr["ID"] = ((StudentImage)item).ID; dr["FirstName"] = ((StudentImage)item).Student.FirstName; dr["Lastname"] = ((StudentImage)item).Student.Lastname;
                        dr["StudentID"] = ((StudentImage)item).StudentID; dr["Password"] = ((StudentImage)item).Password;
                        dr["Custom1"] = ((StudentImage)item).Custom1;
                        dr["Custom2"] = ((StudentImage)item).Custom2; dr["Custom3"] = ((StudentImage)item).Custom3;
                        tb.Rows.Add(dr);
                        arrStudID.Add(((StudentImage)item).Student.ID);
                    }
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
