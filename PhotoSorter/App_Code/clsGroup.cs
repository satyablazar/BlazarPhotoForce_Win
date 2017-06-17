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
using PhotoForce.Extensions;
using System.Collections.ObjectModel;

namespace PhotoForce.App_Code
{
    public class clsGroup
    {
        internal static int GetCountGroupUnderSchool(PhotoSorterDBModelDataContext db, string groupName, int schoolid)
        {
            return (from p in db.Groups where p.GroupName == groupName && p.SchoolID == schoolid select p).Count();
        }
        internal static int GetCheckGroupUpdate(PhotoSorterDBModelDataContext db, string groupName, int schoolId, int groupId)
        {
            return (from p in db.Groups where p.GroupName == groupName && p.SchoolID == schoolId && p.ID != groupId select p).Count();
        }
        public static Group getGroupById(PhotoSorterDBModelDataContext db, int groupId)
        {
            return (from p in db.Groups where p.ID == groupId select p).FirstOrDefault();
        }
        public static IEnumerable<Group> getGroupDetails(PhotoSorterDBModelDataContext db, int GrpID)
        {
            return (db.ExecuteQuery<Group>("select * from [Group] where ID = " + GrpID).ToList());
        }

        //used in export
        internal static int[] GetStudentImgIdOnMultiGroupid(PhotoSorterDBModelDataContext db, ArrayList arrGroupId)
        {
            string allGroupId = "";
            for (int i = 0; i < arrGroupId.Count; i++)
            {
                allGroupId += arrGroupId[i] + ",";
            }
            allGroupId = allGroupId.Substring(0, allGroupId.Length - 1);
            int[] result = db.ExecuteQuery<int>("SELECT StudentPhotoID FROM GroupItem gi inner join StudentImage si on gi.StudentPhotoID = si.ID inner join student s on s.id = si.StudentIDPK where s.recordstatus='true' and gi.GroupID in (" + allGroupId + ")").ToArray();
            //int[] retval = obj.ToArray();//new int[obj.Count];
            //for (int i = 0; i < obj.Count; i++)
            //{
            //    retval.Add(obj[i]);
            //}
            return result;
        }

        //used in addnewgroupVM
        public static Group updateGroup(PhotoSorterDBModelDataContext db, int groupId)
        {
            return (from p in db.Groups where p.ID == groupId select p).FirstOrDefault();
        }

        //used in add stu to group VM
        //internal static void insertGroupItems(PhotoSorterDBModelDataContext db, int groupID, ArrayList arrImageIds)
        internal static void insertGroupItems(PhotoSorterDBModelDataContext db, int groupID, int arrImageIds)
        {
            //foreach (int imagesId in arrImageIds)
            //{
            //    var groupItemId = (from p in db.GroupItems where p.GroupID == groupID && p.StudentPhotoID == imagesId select p.ID).FirstOrDefault();
            //    if (groupItemId == 0)
            //    {
            //        GroupItem gi = new GroupItem();
            //        gi.GroupID = groupID;
            //        gi.StudentPhotoID = imagesId;
            //        db.GroupItems.InsertOnSubmit(gi);
            //        db.SubmitChanges();
            //    }
            //}
            int imagesId = arrImageIds;
            var groupItemId = (from p in db.GroupItems where p.GroupID == groupID && p.StudentPhotoID == imagesId select p.ID).FirstOrDefault();
            if (groupItemId == 0)
            {
                GroupItem gi = new GroupItem();
                gi.GroupID = groupID;
                gi.StudentPhotoID = imagesId;
                db.GroupItems.InsertOnSubmit(gi);
                db.SubmitChanges();
            }
        }
        internal static IEnumerable<GroupExtraColumn> getGroupsBySchool(PhotoSorterDBModelDataContext db, int SchoolId)
        {
            return db.ExecuteQuery<GroupExtraColumn>("select g.ID,GroupName,Notes,isnull(StudentCount,0) as StudentCount from [Group] g left join (select GroupID,COUNT(*) as StudentCount from GroupItem group by GroupID) gi on g.ID=gi.GroupID where SchoolID=" + SchoolId + " order by createdon desc").ToList();
        }

        #region Bulk Rename Group
        internal static int UpdateMultipleGroupName(PhotoSorterDBModelDataContext db, string Name, ArrayList arrGroupId)
        {
            string allGroupId = "";
            for (int i = 0; i < arrGroupId.Count; i++)
            {
                allGroupId += arrGroupId[i] + ",";
            }
            allGroupId = allGroupId.Substring(0, allGroupId.Length - 1);
            return db.ExecuteCommand(
                "update [Group] set GroupName='" + Name + "' where ID in (" + allGroupId + ")"
                );
        }
        internal static int UpdateMultipleGroupNotes(PhotoSorterDBModelDataContext db, string Note, ArrayList arrGroupId)
        {
            string allGroupId = "";
            for (int i = 0; i < arrGroupId.Count; i++)
            {
                allGroupId += arrGroupId[i] + ",";
            }
            allGroupId = allGroupId.Substring(0, allGroupId.Length - 1);
            return db.ExecuteCommand(
                "update [Group] set Notes='" + Note + "' where ID in (" + allGroupId + ")"
                );
        }
        #endregion

        //manage groups
        public static List<Group> getAllGroups(PhotoSorterDBModelDataContext db, int schoolId) //changed to List from IEnumerable   //mohan
        {
            return (from p in db.Groups where p.SchoolID == schoolId orderby p.createdOn descending select p).ToList();
        }
        //manage groups
        public static int getFirstGroupForSchool(PhotoSorterDBModelDataContext db, int schoolId) //changed to List from IEnumerable   //mohan
        {
            return (from p in db.Groups where p.SchoolID == schoolId orderby p.createdOn descending select p.ID).FirstOrDefault();
        }
        public static List<String> getAllGroupNames(PhotoSorterDBModelDataContext db, int schoolId) //changed to List from IEnumerable   //mohan
        {
            return (from p in db.Groups where p.SchoolID == schoolId orderby p.createdOn descending select p.GroupName).ToList();
        }
        public static List<GroupItem> getstudentImagesByGroup(PhotoSorterDBModelDataContext db, int groupId)  //changed to List from IEnumerable   //mohan
        {
            return (from gi in db.GroupItems 
                    join si in db.StudentImages on gi.StudentPhotoID equals si.ID
                    join s in db.Students
                        on si.StudentIDPK equals s.ID
                    where gi.GroupID == groupId && s.RecordStatus == true
                    select gi).ToList();
        }
        //Added By Mohan On 4rth Nov 2015
        public static List<GroupItem> getstudentImagesWithoutOrdersByGroupAndBatchIds(PhotoSorterDBModelDataContext db, int groupId, int batchId)  
        {
            return db.ExecuteQuery<GroupItem>("select * from GroupItem where GroupID = " + groupId + " and StudentPhotoID not in (select StudentImageId from StudentPhotoOrder where OrderId in (select id from Orders where OrdersImportId = " + batchId + "))").ToList();
        }
        public static List<GroupItem> getstudentImagesWithOrdersByGroupAndBatchIds(PhotoSorterDBModelDataContext db, int groupId, int batchId)  
        {
            return db.ExecuteQuery<GroupItem>("select * from GroupItem where GroupID = " + groupId + " and StudentPhotoID in (select StudentImageId from StudentPhotoOrder where OrderId in (select id from Orders where OrdersImportId = " + batchId + "))").ToList();
        }
        public static List<GroupClassPhoto> getClassPhotoByGroup(PhotoSorterDBModelDataContext db, int groupId)  //changed to List from IEnumerable   //mohan
        {
            return (from p in db.GroupClassPhotos where p.GroupId == groupId select p).ToList();
        }
        //# by hema for pspa
        #region
        public static List<GroupClassPhoto> getClassPhotoByGroupPSPA(PhotoSorterDBModelDataContext db, ArrayList GroupId)
        {
            List<GroupClassPhoto> tempGroupClassPhoto = new List<GroupClassPhoto>();
            foreach (int groupId in GroupId)
            {
                tempGroupClassPhoto.AddRange(from p in db.GroupClassPhotos where p.GroupId == groupId select p);
            }
            return tempGroupClassPhoto.ToList();
        }
        public static List<GroupItem> getstudentImagesByGroupPSPA(PhotoSorterDBModelDataContext db, ArrayList GroupId)
        {
            List<GroupItem> tempGroupItem = new List<GroupItem>();
            foreach (int groupId in GroupId)
            {
                tempGroupItem.AddRange(from gi in db.GroupItems
                                       join si in db.StudentImages on gi.StudentPhotoID equals si.ID
                                       join s in db.Students
                                           on si.StudentIDPK equals s.ID
                                       where gi.GroupID == groupId && s.RecordStatus == true
                                       select gi);
            }
            return tempGroupItem.ToList();
        }
        public static List<GroupItem> getstudentSelectyedImagesByGroupSP(PhotoSorterDBModelDataContext db, int groupId, int[] selectedStudentImageIds)
        {
            List<GroupItem> tempGroupItem = new List<GroupItem>();

            //string StudentIDs = string.Join(",", selectedStudentImageIds);
            
            //tempGroupItem = new ObservableCollection<GroupItem>(db.ExecuteQuery<GroupItem>("select * from GroupItem gi join StudentImage si on gi.StudentPhotoID = si.ID join Student s on si.StudentIDPK = s.ID where gi.GroupID = " + groupId + " and s.RecordStatus = 1 and si.ID in (" + StudentIDs + ")").ToList());
            
            //return tempGroupItem.ToList();


            foreach (int selectedStuID in selectedStudentImageIds)
            {
                tempGroupItem.AddRange(from gi in db.GroupItems
                                       join si in db.StudentImages on gi.StudentPhotoID equals si.ID
                                       join s in db.Students
                                           on si.StudentIDPK equals s.ID
                                       where gi.GroupID == groupId && s.RecordStatus == true && si.ID == selectedStuID
                                       select gi);
            }

            return tempGroupItem.ToList();
        }
        #endregion
        internal static int deleteGroup(PhotoSorterDBModelDataContext db, ArrayList arrGroupId)
        {
            string allGroupId = "";
            for (int i = 0; i < arrGroupId.Count; i++)
            {
                allGroupId += arrGroupId[i] + ",";
            }
            allGroupId = allGroupId.Substring(0, allGroupId.Length - 1);
            return db.ExecuteCommand(
                "delete from [Group] where ID in (" + allGroupId + ")"
                );
        }
        internal static int deleteGroupItem(PhotoSorterDBModelDataContext db, int groupId, int studentImgId)
        {
            return db.ExecuteCommand(
                "Delete From GroupItem where GroupID=" + groupId + " and StudentPhotoID=" + studentImgId
                );
        }
        internal static int updateGroupItem(PhotoSorterDBModelDataContext db, int groupItemId, int studentImgId)
        {
            return db.ExecuteCommand(
                "update GroupItem set StudentPhotoID=" + studentImgId+" where ID=" + groupItemId 
                );
        }
        internal static int deleteGroupItems(PhotoSorterDBModelDataContext db, List<int> lstGrpId)
        {
            string AllGroupItemID = string.Join(",",lstGrpId);
            //for (int i = 0; i < lstGrpId.Count; i++)
            //{
            //    AllGroupItemID += lstGrpId[i] + ",";
            //}
            //AllGroupItemID = AllGroupItemID.Substring(0, AllGroupItemID.Length - 1);
            return db.ExecuteCommand(
                "delete from GroupItem where ID in (" + AllGroupItemID + ")"
                );
        }
        internal static string getGroupClassImageName(PhotoSorterDBModelDataContext db, int grpId)
        {
            return db.ExecuteQuery<string>(
                "select ImageName from StudentImage where id = (select StudentImageid from GroupClassPhoto where id  = (select MAX(id) from GroupClassPhoto where GroupId = " + grpId + "))"
                ).FirstOrDefault();
        }
        internal static int deleteFromGroupPhoto(PhotoSorterDBModelDataContext db, int grpId, int StudentImgId)
        {
            return db.ExecuteCommand(
                "Delete From GroupClassPhoto where GroupID=" + grpId + " and StudentImageID=" + StudentImgId
                );
        }
        public static IEnumerable<GroupItem> getImagesFromGroupItemForSelected(PhotoSorterDBModelDataContext db, Int32 grpId, int[] arrStudId)
        {
            String imageId = "";
            // Need to get lastname and firstname of student image
            for (int i = 0; i < arrStudId.Count(); i++)
            {
                imageId += arrStudId[i].ToString() + ",";
            }
            imageId = imageId.Substring(0, imageId.Length - 1);
            return db.ExecuteQuery<GroupItem>(
                "select distinct * from groupitem where GroupID = " + grpId + " and StudentPhotoID in (" + imageId + ")"
                ).ToList();
        }
        public static IEnumerable<GroupItem> getImagesFromGroupItem(PhotoSorterDBModelDataContext db, Int32 grpId)
        {
            return db.ExecuteQuery<GroupItem>(
                "select distinct * from GroupItem g inner join StudentImage si on g.StudentPhotoID = si.ID inner join student s on s.id = si.StudentIDPK where GroupID = " + grpId + " and s.recordstatus='true'"
                ).ToList();
        }
        public static IEnumerable<GroupClassPhoto> getGroupPhotoByGrpId(PhotoSorterDBModelDataContext db, int grpId)
        {
            return db.ExecuteQuery<GroupClassPhoto>(
                "select * from GroupClassPhoto where Groupid =" + grpId + ""
                ).ToList();
        }

        //Generate PDF VM
        internal static ArrayList getStudentIdStringByGroupId(PhotoSorterDBModelDataContext db, ArrayList arrGroupId)
        {
            string allGroupId = "";
            for (int i = 0; i < arrGroupId.Count; i++)
            {
                allGroupId += arrGroupId[i] + ",";
            }
            allGroupId = allGroupId.Substring(0, allGroupId.Length - 1);

            List<string> obj = db.ExecuteQuery<string>("SELECT distinct  CAST(s.ID as nvarchar(20)) as ID,s.FirstName FROM StudentImage si inner join GroupItem gi on  si.ID = gi.StudentPhotoID  inner join Student s on si.StudentIDPK=s.ID where gi.GroupID in(" + allGroupId + ") order by s.Firstname").ToList();
            ArrayList result = new ArrayList();
            for (int i = 0; i < obj.Count; i++)
            {
                result.Add(obj[i]);
            }
            return result;
        }
        internal static int getStudentPdfCountByGroups(PhotoSorterDBModelDataContext db, ArrayList arrGroupId)
        {
            int result = 0;
            foreach (int grpId in arrGroupId)
            {
                string query = "SELECT distinct CAST(s.ID as nvarchar(20)) as ID,s.FirstName FROM StudentImage si inner join GroupItem gi on  si.ID = gi.StudentPhotoID  inner join Student s on si.StudentIDPK=s.ID where gi.GroupID=" + grpId + " order by s.Firstname";
                List<string> obj = db.ExecuteQuery<string>(query).ToList();

                if (obj.Count != 0) { result += obj.Count; }

            }
            return result;
        }
        internal static Group getGroupname(PhotoSorterDBModelDataContext db, int groupId)
        {
            return (db.ExecuteQuery<Group>("select * from [Group] where ID=" + groupId + "").FirstOrDefault());
        }
        public static List<StudentImage> getSelectedStudentsByGroup(PhotoSorterDBModelDataContext db, int groupId, string studentId)
        {
            return (from p in db.GroupItems join p2 in db.StudentImages on p.StudentPhotoID equals p2.ID join s in db.Students on p2.StudentIDPK equals s.ID where p.GroupID == groupId && s.ID == Convert.ToInt32(studentId) select p2).ToList();
        }
        public static IEnumerable<GroupClassPhoto> getGroupPhotoByGroupId(PhotoSorterDBModelDataContext db, int grpId)
        {
            return db.ExecuteQuery<GroupClassPhoto>(
                "select * from GroupClassPhoto where Groupid =" + grpId + ""
                ).ToList();
        }
        //auto create groups
        public static int getGroupId(PhotoSorterDBModelDataContext db, string groupName, int schoolId)
        {
            Group obj = db.ExecuteQuery<Group>("select ID from [group] where groupname = '" + groupName + "' and schoolid=" + schoolId).FirstOrDefault();
            if (obj != null)
                return obj.ID;
            else
                return 0;
        }
        public static int checkGroupItem(PhotoSorterDBModelDataContext db, int groupid, int studentImageId)
        {
            GroupItem objitem = db.ExecuteQuery<GroupItem>("select ID from groupitem where groupid = " + groupid + " and studentphotoid=" + studentImageId).FirstOrDefault();
            if (objitem != null)
                return objitem.ID;
            else
                return 0;
        }
        //dashboard
        public static int updateGroupHasClassPhoto(PhotoSorterDBModelDataContext db, bool tempHasClassPhoto, int grpId)
        {
            return db.ExecuteCommand(
                "update [group] set hasclassPhoto='" + tempHasClassPhoto + "' where ID in (" + grpId + ")"
                );
        }
        public static int deleteStudentImage(PhotoSorterDBModelDataContext db, ArrayList GroupItemID)  //, ArrayList arrStuImgId
        {
            string AllstuimgID = "";
            for (int i = 0; i < GroupItemID.Count; i++)
            {
                AllstuimgID += GroupItemID[i] + ",";
            }
            AllstuimgID = AllstuimgID.Substring(0, AllstuimgID.Length - 1);
            db.ExecuteCommand("delete StudentPhotoOrder where StudentImageId in (" + AllstuimgID + ")");
            return db.ExecuteCommand(
                "delete from studentimage where ID in (" + AllstuimgID + ")"
                );
        }

        #region add by hema for imageQuix
        public static List<GroupItem> getstudentImagesByGroupID(PhotoSorterDBModelDataContext db, int groupId)  //changed to List from IEnumerable   //mohan
        {
            return db.ExecuteQuery<GroupItem>("select g.GroupName, si.*  from StudentImage as si join GroupItem gi on gi.StudentPhotoID = si.ID join [Group] g on g.ID = gi.GroupID where g.ID in (" + groupId + ") Order by g.GroupName").ToList();
        }        
        #endregion
        //used in exportViewModel
        //internal static DataSet getImagesByMaskDetails(PhotoSorterDBModelDataContext db, string query)
        //{
        //    DataSet ds = new DataSet();
        //    ds.Tables.Add(clsDashBoard.ToDataTable(db.ExecuteQuery<StudentImage>(query).ToList()));
        //    return ds;
        //}
    }
}
