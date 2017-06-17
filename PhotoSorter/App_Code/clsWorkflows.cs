using PhotoForce.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsWorkflows
    {
        public static int deleteWorkflow(PhotoSorterDBModelDataContext db, List<int> workflowIds)
        {
            string allWorkflowIds = string.Join(",", workflowIds);
            db.ExecuteCommand("Delete PhotoshootWorkflowItem where WorkflowItemId in (" + allWorkflowIds + ")");
            db.ExecuteCommand("Delete WorkflowCollectionItems where WorkflowItemId in (" + allWorkflowIds + ")");

            return db.ExecuteCommand("Delete WorkflowItem where Id in (" + allWorkflowIds + ")");
        }
        public static int deleteWorkflowCollection(PhotoSorterDBModelDataContext db, List<int> workflowCollectionIds)
        {
            string allWorkflowCollectionIds = string.Join(",", workflowCollectionIds);
            if (!string.IsNullOrEmpty(allWorkflowCollectionIds))
            {
                db.ExecuteCommand("Delete from WorkflowCollectionItems where WorkflowCollectionId in (" + allWorkflowCollectionIds + ")");

                return db.ExecuteCommand("Delete from WorkflowCollection where Id in (" + allWorkflowCollectionIds + ")");
            }
            else
                return 0;
        }
        public static int deleteWorkflowCollectionItems(PhotoSorterDBModelDataContext db, List<int> workflowCollectionItemIds)
        {
            string allWorkflowCollectionItemIds = string.Join(",", workflowCollectionItemIds);
            if (!string.IsNullOrEmpty(allWorkflowCollectionItemIds))
            {
                return db.ExecuteCommand("Delete from WorkflowCollectionItems where Id in (" + allWorkflowCollectionItemIds + ")");
            }
            else
                return 0;
        }
        public static int deleteCollectionItemsById(PhotoSorterDBModelDataContext db, List<int> workflowCollectionItemIds, int collectionId)
        {
            string allWorkflowCollectionItemIds = string.Join(",", workflowCollectionItemIds);
            if (!string.IsNullOrEmpty(allWorkflowCollectionItemIds))
            {
                return db.ExecuteCommand("Delete from WorkflowCollectionItems where WorkflowItemId in (" + allWorkflowCollectionItemIds + ") and WorkflowCollectionId = " + collectionId);
            }
            else
                return 0;
        }
        public static PhotoshootWorkflowItem getPhotoWorkflowItem(PhotoSorterDBModelDataContext db, int photoshootWorkflowItemId)
        {
            return (from pfi in db.PhotoshootWorkflowItems where pfi.Id == photoshootWorkflowItemId select pfi).FirstOrDefault();
        }
        public static List<WorkflowCollectionItem> getAllCollectionItems(PhotoSorterDBModelDataContext db, int collectionId)
        {
            return db.ExecuteQuery<WorkflowCollectionItem>("select * from WorkflowCollectionItems where WorkflowCollectionId = " + collectionId).ToList();
        }
        public static List<WorkflowItem> getWorkflowItemsWithPhotoshoot(PhotoSorterDBModelDataContext db, int photoshootId, string isFromTab)
        {
            int tempId = db.ExecuteQuery<int>("select Id from ItemClassType where ClassType = '" + isFromTab + "'").FirstOrDefault();
            return db.ExecuteQuery<WorkflowItem>("select * from WorkflowItem where ItemClassTypeId = " + tempId + " and id in (select WorkflowItemid from PhotoshootWorkflowItem where PhotoShootID =" + photoshootId + ")").ToList();
        }
        //Got the error here
        public static int removePhotoshootWorkflows(PhotoSorterDBModelDataContext db, List<int> workflowItemIds, int PhotoShootID)
        {
            string allWorkflowItemIds = string.Join(",", workflowItemIds);
            if (!string.IsNullOrEmpty(allWorkflowItemIds))
            {
                string query = "delete from PhotoshootWorkflowItem where WorkflowItemId in (" + allWorkflowItemIds + ") and PhotoShootID = " + PhotoShootID;
                return db.ExecuteCommand(query);
            }
            else
                return 0;
        }
        public static List<WorkflowItem> getAllWorkflowItems(PhotoSorterDBModelDataContext db, int collectionId)
        {
            List<WorkflowItem> res = new List<WorkflowItem>();
            List<int> workflowIds = new List<int>();
            workflowIds = db.ExecuteQuery<int>("select WorkflowItemId from WorkflowCollectionItems where WorkflowCollectionId = " + collectionId).ToList();
            string allworkflowIds = string.Join(",", workflowIds);

            if (!string.IsNullOrEmpty(allworkflowIds))
            {
                string query = "select * from WorkflowItem where Id in (" + allworkflowIds + ")";
                res = db.ExecuteQuery<WorkflowItem>(query).ToList();
            }
            return res;
        }
        public static List<WorkflowItem> getAllWorkflows(PhotoSorterDBModelDataContext db, int photoShootId)
        {
            List<WorkflowItem> totalResult = new List<WorkflowItem>();
            List<int> workflowItemIds = db.ExecuteQuery<int>("select WorkflowItemId from PhotoshootWorkflowItem where PhotoShootID = " + photoShootId).ToList();

            foreach (int wfIdArray in workflowItemIds)
            {
                List<WorkflowItem> result = (from wf in db.WorkflowItems
                                             where wfIdArray == wf.Id
                                             select wf).ToList();
                totalResult.AddRange(result);
            }

            return totalResult;
        }
        public static List<PhotoshootWorkflowItem> getAllPhotoshootWorkflowItem(PhotoSorterDBModelDataContext db, int photoShootId, string classType)
        {
            //var res = (from pwi in db.PhotoshootWorkflowItems join wi in db.WorkflowItems on pwi.WorkflowItemId equals wi.Id where wi.ItemClassTypeId == wi.ItemClassType.Id && pwi.PhotoShootID == photoShootId && wi.ItemClassType.ClassType == classType select pwi);
            string query = "select pw.* from PhotoshootWorkflowItem pw join WorkflowItem as wi on pw.WorkflowItemId = wi.Id join ItemClassType ic on ic.Id = wi.ItemClassTypeId where PhotoShootID = " + photoShootId + " and ic.ClassType = '" + classType + "'";
            return db.ExecuteQuery<PhotoshootWorkflowItem>(query).ToList();
        }
        public static int getSortOrder(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<int>("select SortOrder from WorkflowItem order by SortOrder desc").FirstOrDefault();
        }
        public static List<PhotoShoot> getWorkflowPhotoShoots(PhotoSorterDBModelDataContext db, int schoolId)
        {
            List<PhotoShoot> photographyJobs = db.ExecuteQuery<PhotoShoot>("select * from PhotoShoot ps join PhotographyJob pj on ps.PhotographyjobID = pj.ID where ps.ImageFolder is null and SchoolID = " + schoolId).ToList();
            return photographyJobs;
        }
        public static List<PhotoshootWorkflowItem> getPSWorkflowItems(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<PhotoshootWorkflowItem>("select pwi.* from PhotoshootWorkflowItem pwi join WorkflowItem wi on pwi.WorkflowItemId = wi.Id join ItemClassType ic on wi.ItemClassTypeId = ic.Id and ic.ClassType = 'Workflow' Order by PhotoShootID desc").ToList();
        }
        public static List<ComboBoxItem> getNotesForWorkflowItem(PhotoSorterDBModelDataContext db, int workflowItemId, int photoshootId)
        {
            return (from wfi in db.PhotoshootWorkflowItems where wfi.WorkflowItemId == workflowItemId && wfi.PhotoShootID == photoshootId && wfi.Notes != "" && wfi.Notes != null select new ComboBoxItem { Name = wfi.Notes }).ToList();
        }
        public static int updatePhotoShootWorkflowItems(PhotoSorterDBModelDataContext db, PhotoshootWorkflowItem PSWorkflow)
        {
            return db.ExecuteCommand("update PhotoshootWorkflowItem set SortOrder = " + PSWorkflow.SortOrder + " where Id = " + PSWorkflow.Id);
        }
        public static int updateWorkflowItem(PhotoSorterDBModelDataContext db, WorkflowItem oldCollectionItem)
        {
            return db.ExecuteCommand("update WorkflowItem set SortOrder = " + oldCollectionItem.SortOrder + " where Id = " + oldCollectionItem.Id);
        }
        public static int upadteWorkflowCollectionItem(PhotoSorterDBModelDataContext db, WorkflowCollectionItem oldCollectionItem)
        {
            return db.ExecuteCommand("update WorkflowCollectionItems set SortOrder = " + oldCollectionItem.SortOrder + " where Id = " + oldCollectionItem.Id);
        }
        public static int updateAllPhotoShootWorkflowItemsAssignedTo(PhotoSorterDBModelDataContext db, ArrayList PSWorkflowIds, string assignedTo)
        {
            string tempPSWorkflowIds = clsDashBoard.convertArrayListToString(PSWorkflowIds);
            return db.ExecuteCommand("update PhotoshootWorkflowItem set Assignedto = '" + assignedTo + "' where Id in (" + tempPSWorkflowIds + ")");
        }
        public static int updateAllPhotoShootWorkflowItemsStatus(PhotoSorterDBModelDataContext db, ArrayList PSWorkflowIds, string status)
        {
            string tempPSWorkflowIds = clsDashBoard.convertArrayListToString(PSWorkflowIds);
            return db.ExecuteCommand("update PhotoshootWorkflowItem set Status = '" + status + "' where Id in (" + tempPSWorkflowIds + ")");
        }
        public static int updateAllPhotoShootWorkflowItemsdueDate(PhotoSorterDBModelDataContext db, ArrayList PSWorkflowIds, string dueDate)
        {
            string tempPSWorkflowIds = clsDashBoard.convertArrayListToString(PSWorkflowIds);
            return db.ExecuteCommand("update PhotoshootWorkflowItem set DueDate = '" + dueDate + "' where Id in (" + tempPSWorkflowIds + ")");
        }
        public static int updateAllPhotoShootWorkflowItemsCompletedOn(PhotoSorterDBModelDataContext db, ArrayList PSWorkflowIds, string completedOn)
        {
            string tempPSWorkflowIds = clsDashBoard.convertArrayListToString(PSWorkflowIds);
            return db.ExecuteCommand("update PhotoshootWorkflowItem set CompletedOn = '" + completedOn + "' where Id in (" + tempPSWorkflowIds + ")");
        }
        public static int updateAllPhotoShootWorkflowItemsCompletedBy(PhotoSorterDBModelDataContext db, ArrayList PSWorkflowIds, string completedBy)
        {
            string tempPSWorkflowIds = clsDashBoard.convertArrayListToString(PSWorkflowIds);
            return db.ExecuteCommand("update PhotoshootWorkflowItem set CompletedBy = '" + completedBy + "' where Id in (" + tempPSWorkflowIds + ")");
        }
        //public static int updateAllPhotoShootWorkflowItemsStatus(PhotoSorterDBModelDataContext db, ArrayList PSWorkflowIds, string status)
        //{
        //    string tempPSWorkflowIds = clsDashBoard.convertArrayListToString(PSWorkflowIds);
        //    return db.ExecuteCommand("update PhotoshootWorkflowItem set Assignedto = '" + assignedTo + "', Status = '" + status + "', DueDate = '" + dueDate + "', CompletedOn = '" + completedOn + "', CompletedBy = '" + completedBy + "' where Id in (" + tempPSWorkflowIds + ")");
        //}
        public static int deletePhotoshootWorkflowItems(PhotoSorterDBModelDataContext db, ArrayList PSWorkflowIds)
        {
            string tempPSWorkflowIds = clsDashBoard.convertArrayListToString(PSWorkflowIds);
            return db.ExecuteCommand("delete from PhotoshootWorkflowItem where Id in (" + tempPSWorkflowIds + ")");
        }
        public static int updatePhotoShootWorkflowItemshasNotes(PhotoSorterDBModelDataContext db, PhotoshootWorkflowItem PSWorkflow, string isFrom)
        {
            if (isFrom == "from Delete")
                return db.ExecuteCommand("update PhotoshootWorkflowItem set HasNotes = 0 where Id = " + PSWorkflow.Id);
            else
                return db.ExecuteCommand("update PhotoshootWorkflowItem set HasNotes = 1 where Id = " + PSWorkflow.Id);
        }
        public static List<WorkflowItem> getAllEquipmentItems(PhotoSorterDBModelDataContext db, int collectionId)
        {
            List<WorkflowItem> res = new List<WorkflowItem>();
            List<int> workflowIds = new List<int>();
            workflowIds = db.ExecuteQuery<int>("select WorkflowItemId from WorkflowCollectionItems where WorkflowCollectionId = " + collectionId).ToList();
            string allworkflowIds = string.Join(",", workflowIds);

            if (!string.IsNullOrEmpty(allworkflowIds))
            {
                int tempId = db.ExecuteQuery<int>("select Id from ItemClassType where ClassType = 'Equipment'").FirstOrDefault();

                string query = "select * from WorkflowItem where Id in (" + allworkflowIds + ") and ItemClassTypeId = " + tempId;
                res = db.ExecuteQuery<WorkflowItem>(query).ToList();
            }
            return res;
        }
        public static int SaveOrInsertWorkflowFilterString(PhotoSorterDBModelDataContext db, string filterString, bool isUpdate)
        {
            Regex re = new Regex("'");
            filterString = re.Replace(filterString, "@");

            string query = (isUpdate == true) ? ("update Settings set settingValue = '" + filterString + "' where settingName = 'WorkflowFilterString'") : "Insert into Settings (settingName,settingValue) values ('WorkflowFilterString','" + filterString + "')";
            return db.ExecuteCommand(query);
        }
        public static List<WorkflowCollection> getCollectionItems(PhotoSorterDBModelDataContext db, bool isWorkflowCollections)
        {
            string query = "";
            if(isWorkflowCollections)
                query = "select * from WorkflowCollection where ItemClassTypeId = 1";
            else
                query = "select * from WorkflowCollection where ItemClassTypeId = 2";
            return db.ExecuteQuery<WorkflowCollection>(query).ToList();

        }
        public static int updateCollectionItemSortOrder(PhotoSorterDBModelDataContext db, int workflowItemId, int quantity, int sortOrder, string isFrom)
        {
            if (isFrom == "Equipment")
                return db.ExecuteCommand("update WorkflowCollectionItems set Quantity = " + quantity + ",SortOrder = " + sortOrder + " where WorkflowItemId =" + workflowItemId);
            else
                return db.ExecuteCommand("update WorkflowCollectionItems set SortOrder = " + sortOrder + " where WorkflowItemId =" + workflowItemId);
        }        

    }
}
