using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsActivities
    {
        public static List<ActivityType> getAllActivitiyTypes(PhotoSorterDBModelDataContext db)
        {
            return (from at in db.ActivityTypes select at).ToList();
        }
        public static List<Student> getAllStudents(PhotoSorterDBModelDataContext db)
        {
            return (from stu in db.Students where stu.StudentImport.SchoolID == clsSchool.defaultSchoolId select stu).ToList();
        }
        public static List<PhotoShoot> getAllPhotoshoots(PhotoSorterDBModelDataContext db)
        {
            return (from ps in db.PhotoShoots where ps.PhotographyJob.SchoolID == clsSchool.defaultSchoolId select ps).ToList();
        }

        //used in ActivitiesVM
        public static Activity getActivitiy(PhotoSorterDBModelDataContext db,int activityId)
        {
            return (from aa in db.Activities where aa.Id == activityId select aa).SingleOrDefault();
        }
        public static List<Activity> getAllActivities(PhotoSorterDBModelDataContext db)
        {
            return (from aa in db.Activities select aa).ToList();
        }
        public static List<Activity> getMyActivities(PhotoSorterDBModelDataContext db)
        {
            return (from aa in db.Activities where aa.User.UserName == clsStatic.userName select aa).ToList();
        }
        public static List<Activity> getPhotoShootActivities(PhotoSorterDBModelDataContext db, int photoShootId)
        {
            return (from aa in db.Activities where aa.PhotoShootId == photoShootId select aa).ToList();
        }
        public static List<Activity> getStudentActivities(PhotoSorterDBModelDataContext db, int studentId)
        {
            return (from aa in db.Activities where aa.StudentId == studentId select aa).ToList();
        }
        public static List<Activity> getSchoolActivities(PhotoSorterDBModelDataContext db, int schoolId)
        {
            return (from aa in db.Activities where aa.SchoolId == schoolId select aa).ToList();
        }
        //delete activities
        internal static int deleteMultipleActivities(PhotoSorterDBModelDataContext db, ArrayList arrActivityId)
        {
            string allActivityId = "";
            for (int i = 0; i < arrActivityId.Count; i++)
            {
                allActivityId += arrActivityId[i] + ",";
            }
            allActivityId = allActivityId.Substring(0, allActivityId.Length - 1);
            return db.ExecuteCommand(
                "delete from Activities where Id in (" + allActivityId + ")"
                );
        }
    }
}
