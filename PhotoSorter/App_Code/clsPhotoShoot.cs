using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsPhotoShoot
    {
        public static int checkForSchoolYear(PhotoSorterDBModelDataContext db, int presentSchoolYear, int tempSchoolID)
        {
            return db.ExecuteQuery<int>("select count(*) from photographyjob where StartYear like '%" + presentSchoolYear + "%' and schoolID= " + tempSchoolID).FirstOrDefault();
        }
        internal static void updateSchoolYear(PhotoSorterDBModelDataContext db, int Jobid, string Startyear, string EndYear, string JobName)
        {
            var cust = (from c in db.PhotographyJobs
                        where c.ID == Jobid
                        select c).First();
            cust.StartYear = Startyear;
            cust.EndYear = EndYear;
            cust.JobName = JobName;
            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        public static PhotographyJob getPhotographyJob(PhotoSorterDBModelDataContext db, int jobId)
        {
            return (from pj in db.PhotographyJobs where pj.ID == jobId select pj).FirstOrDefault();
        }
        public static int isSchoolYearExists(PhotoSorterDBModelDataContext db, string jobName, int schoolId)
        {
            return db.ExecuteQuery<int>("select ID from photographyjob where JobName like '%" + jobName + "%' and schoolID = " + schoolId).FirstOrDefault();
        }
        //addeditphotshoot VM
        internal static int updatePhotographyJobID(PhotoSorterDBModelDataContext db, int? photographyJobId, int photoShootId)
        {
            return db.ExecuteCommand(
                "update PhotoShoot set PhotographyjobID='" + photographyJobId + "' where PhotoShotID in (" + photoShootId + ")"
                );
        }
        public static List<int> isJobNameNotExists(PhotoSorterDBModelDataContext db, string jobName)
        {
            return db.ExecuteQuery<int>("select SchoolID from photographyjob where JobName like '" + jobName + "'").ToList();
        }
    }
}
