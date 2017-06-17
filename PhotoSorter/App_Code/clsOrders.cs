using PhotoForce.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsOrders
    {
        public static int upadteDefaultPricing(PhotoSorterDBModelDataContext db, string package, int packageID, float price)
        {
            return db.ExecuteCommand("update DefaultPackages set package = '" + package + "' ,price = " + price + " where Id = " + packageID);
        }
        public static Order getLastInsertedOrder(PhotoSorterDBModelDataContext db)
        {
            return (from lio in db.Orders orderby lio.CreatedOn descending select lio).FirstOrDefault();
        }
        public static NextOrder getNextOrder(PhotoSorterDBModelDataContext db)
        {
            string query = "select top 1 * from NextOrder";
            NextOrder temp = db.ExecuteQuery<NextOrder>(query).FirstOrDefault();
            if (temp != null)
            {
                temp.NextOrderId = temp.NextOrderId + 1;
                db.SubmitChanges();
            }
            else
            {
                db.ExecuteCommand("Insert NextOrder (NextOrderId) values (0)");
                temp = db.ExecuteQuery<NextOrder>(query).FirstOrDefault();

                temp.NextOrderId = temp.NextOrderId + 1;
                db.SubmitChanges();
            }

            return (from lio in db.NextOrders select lio).FirstOrDefault();
        }
        public static DataTable getAllManualImports(PhotoSorterDBModelDataContext db)
        {
            return WCFSQLHelper.getDataTable("select ISNULL(ss.SchoolName,'') as SchoolName ,oi.SchoolID ,oi.CreatedOn,oi.Id,oi.Description,oi.Notes,ISNULL(oi.OrderType,'') as OrderType from OrdersImport oi left join School ss on ss.ID=oi.SchoolID where oi.OrderType like '%Manual%'");
        }
        public static List<OrderPackage> getAllOrderPackages(PhotoSorterDBModelDataContext db)
        {
            return (from op in db.OrderPackages select op).ToList();
        }
        // Used in View Orders By Gallery Group
        public static List<string> getGalleryGroupNames(PhotoSorterDBModelDataContext db)
        {
            return db.ExecuteQuery<string>("select distinct sp_GroupName from StudentPhotoOrder where sp_GroupName is not null order by sp_GroupName").ToList();
        }
        public static List<Order> getOrdersByGalleryName(PhotoSorterDBModelDataContext db, string galleryGroup)
        {
            string query = "select * from Orders where id in (select distinct o.Id from Orders O " +
                                            "left outer join StudentPhotoOrder spo on spo.Orderid = O.Id " +
                                            "where sp_groupname like '%" + galleryGroup + "%')";
            return db.ExecuteQuery<Order>(query).ToList();
        }
        public static List<View_Order> getItemsByGalleryNameAndId(PhotoSorterDBModelDataContext db, string galleryGroup, int OrderId)
        {
            return db.ExecuteQuery<View_Order>("select * from View_Order where OrderId = " + OrderId + " and sp_GroupName like '%" + galleryGroup + "%'").ToList();
        }
        //End

        public static View_Order getViewOrderByOrderDetailId(PhotoSorterDBModelDataContext db, int studentPhotoOrderId)
        {
            return (from o in db.View_Orders where o.StudentPhotoOrderId == studentPhotoOrderId select o).FirstOrDefault();
        }
        public static List<Order> getAllOrders(PhotoSorterDBModelDataContext db)
        {
            return (from o in db.Orders select o).ToList();
        }
        public static List<OrderPackage> getAllOrderBullingCodes(PhotoSorterDBModelDataContext db)
        {
            return (from op in db.OrderPackages select op).ToList();
        }
        public static string getOrderPackage(PhotoSorterDBModelDataContext db, string BCode)
        {
            return (from op in db.OrderPackages where op.SimplePhotoItemId == BCode select op.Item).FirstOrDefault();
        }
        public static OrderPackage getOrderPackagebyBillingCode(PhotoSorterDBModelDataContext db, string BCode)
        {
            return (from op in db.OrderPackages where op.SimplePhotoItemId == BCode select op).FirstOrDefault();
        }
        public static List<View_Order> getOrderImagesFromOrderId(PhotoSorterDBModelDataContext db, ArrayList orderIds)
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            List<View_Order> totalOrderItems = new List<View_Order>();

            foreach (int Id in orderIds)
            {
                int[] orderDetailIds = getImagesIdsByOrder(db, Id).Select(a => (int)a.Id).ToArray();
                List<View_Order> tempOrders = getImagesOrdersFromIds(db, orderDetailIds, Id).ToList();
                //tempOrders.Add(new View_Order
                //{
                //    ImageName = "Order.jpg",
                //    Cust_FirstName = tempOrders.First().Cust_FirstName,
                //    Cust_LastName = tempOrders.First().Cust_LastName,
                //    StudentID = tempOrders.First().OrderId.ToString(),
                //    Packages = "AY-1",
                //    VendorOrderNo = tempOrders.First().VendorOrderNo,
                //    Ship_FirstName = tempOrders.First().Ship_FirstName,
                //    Ship_LastName = tempOrders.First().Ship_LastName,
                //    Ship_Address = tempOrders.First().Ship_Address,
                //    Ship_City = tempOrders.First().Ship_City,
                //    Ship_State = tempOrders.First().Ship_State,
                //    Ship_PostalCode = tempOrders.First().Ship_PostalCode,
                //    Ship_Country = tempOrders.First().Ship_Country,
                //    Cust_Email = tempOrders.First().Cust_Email
                //});
                totalOrderItems.AddRange(tempOrders);
            }
            return totalOrderItems;
        }

        public static List<View_Order> getImagesOrdersFromIds(PhotoSorterDBModelDataContext db, int[] orderDetailIds, int orderId)
        {
            List<View_Order> totalResult = new List<View_Order>();
            List<int[]> lstOrderDetailIds = new List<int[]>();

            if (orderDetailIds.Count() > 1000)
            {
                lstOrderDetailIds = orderDetailIds
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
                lstOrderDetailIds.Add(orderDetailIds);
            }
            foreach (int[] imgIdArray in lstOrderDetailIds)
            {
                List<View_Order> result = (from vo in db.View_Orders
                                           where imgIdArray.Contains((int)vo.StudentPhotoOrderId) && vo.OrderId == orderId
                                           select vo).ToList();
                totalResult.AddRange(result);
            }

            return totalResult;
            // return (from vo in db.View_Orders where imageIds.Contains((int)vo.StudentImageId) && vo.OrderId == orderId select vo).ToList();
        }
        public static List<View_Order> getOrderItemsById(PhotoSorterDBModelDataContext db, int orderId)
        {
            return (from vo in db.View_Orders where vo.OrderId == orderId select vo).ToList();
        }
        public static List<StudentPhotoOrder> getImagesIdsByOrder(PhotoSorterDBModelDataContext db, int orderId)
        {
            return (from spo in db.StudentPhotoOrders where spo.OrderId == orderId select spo).Where(x => x != null).ToList();
        }
        public static List<StudentPhotoOrder> getImageByIds(PhotoSorterDBModelDataContext db, List<int?> imgIds)
        {
            return (from spo in db.StudentPhotoOrders where imgIds.Contains((int)spo.StudentImageId) select spo).Where(x => x != null).ToList();
        }
        public static StudentPhotoOrder getStudentPhotoOrderById(PhotoSorterDBModelDataContext db, int SPOId)
        {
            return (from spo in db.StudentPhotoOrders where spo.Id == SPOId select spo).FirstOrDefault();
        }
        public static List<StudentPhotoOrder> getStudentPhotoOrdersByItemIds(PhotoSorterDBModelDataContext db, List<int> SPOIds)
        {
            return (from spo in db.StudentPhotoOrders where SPOIds.Contains(spo.Id) select spo).ToList();
        }
        public static Order getOrderById(PhotoSorterDBModelDataContext db, int OrderId)
        {
            return (from o in db.Orders where o.Id == OrderId select o).FirstOrDefault();
        }
        public static int UpdateOrderLog(PhotoSorterDBModelDataContext db, string tempLog, int OrderId)
        {
            return db.ExecuteCommand("update Orders set OrdersLog='" + tempLog + "' where Id = " + OrderId);
        }
        public static List<StudentImage> getImagesForOrders(PhotoSorterDBModelDataContext db, int OrderId, int schoolId, int schoolYearId)
        {
            string allStuImageId = getPhotoOrdersImagesByOrderId(db, OrderId);
            //if (allStuImageId == "") { return null; } //Mohan

            //          string query = "select si.Id,si.ImageName,st.FirstName,st.Lastname,st.Teacher,st.Grade from StudentImage si join PhotoShoot p"
            //+" on p.PhotographyjobID=" +schoolYearId  
            //+" join Student st on si.StudentIDPK = st.ID"
            //+ " WHERE p.PhotoShotID = si.PhotoShootID and si.SchoolID =" + schoolId + " and  si.Id not in (" + allStuImageId + ")";

            //         return db.ExecuteQuery<StudentImage>(query).ToList();
            return db.ExecuteQuery<StudentImage>("select * from StudentImage si left join PhotoShoot p on p.PhotographyjobID= " + schoolYearId + "WHERE p.PhotoShotID = si.PhotoShootID and SchoolID = " + schoolId + " and id not in (" + allStuImageId + ")").ToList();
        }
        public static int removeImagesFromOrder(PhotoSorterDBModelDataContext db, ArrayList orderItemIds)
        {
            string allOrderItemId = "";
            for (int i = 0; i < orderItemIds.Count; i++)
            {
                allOrderItemId += orderItemIds[i] + ",";
            }
            allOrderItemId = allOrderItemId.Substring(0, allOrderItemId.Length - 1);
            return db.ExecuteCommand(
                "delete from StudentPhotoOrder where Id in (" + allOrderItemId + ")"
                );
        }
        public static int? getGroupPhotoByImageId(PhotoSorterDBModelDataContext db, int? ImageId)
        {
            string query = "select studentImageId from GroupClassPhoto where GroupId in (select GroupId from GroupItem where StudentPhotoID = " + ImageId + ")";
            return db.ExecuteQuery<int>(query).FirstOrDefault();
        }
        public static StudentImage getGroupImageIdForOrders(PhotoSorterDBModelDataContext db, int OrderDetailId, int OrderId)
        {
            string query = ("select GroupImageId from StudentPhotoOrder where Id = " + OrderDetailId + "and OrderId = " + OrderId);
            int? GroupImageId = db.ExecuteQuery<int?>(query).FirstOrDefault();

            if (GroupImageId == null) { return null; }
            return db.ExecuteQuery<StudentImage>("select * from StudentImage where ID = " + GroupImageId).FirstOrDefault();
        }

        public static List<StudentImage> getStudentImageIdsByOrderId(PhotoSorterDBModelDataContext db, int OrderId)
        {
            string allStuImageId = getPhotoOrdersImagesByOrderId(db, OrderId);
            //if (allStuImageId == "") { return null; }
            return db.ExecuteQuery<StudentImage>("select * from StudentImage where id in (" + allStuImageId + ")").ToList();
        }

        public static List<StudentImage> getImagesBySeachString(PhotoSorterDBModelDataContext db, int OrderId, string searchString)
        {
            string allStuImageId = getPhotoOrdersImagesByOrderId(db, OrderId);

            //            return db.ExecuteQuery<StudentImage>("select si.id,si.ImageName,st.FirstName,st.Lastname,s.SchoolName as SchoolName,st.Teacher as Teacher,st.Grade from StudentImage si"
            //+ " join school s on si.SchoolID = s.ID "
            //+ " join Student st on si.StudentIDPK = st.ID"
            // + " WHERE ImageName like '%" + searchString + "%' and si.id not in (" + allStuImageId + ")").ToList();
            return db.ExecuteQuery<StudentImage>("select * from StudentImage WHERE ImageName like '%" + searchString + "%' and id not in (" + allStuImageId + ")").ToList();
        }
        public static string getPhotoOrdersImagesByOrderId(PhotoSorterDBModelDataContext db, int OrderId)
        {
            string allStuId = "";
            List<int?> arrStuImageId = new List<int?>();
            arrStuImageId = (from spo in db.StudentPhotoOrders where spo.OrderId == OrderId select spo.StudentImageId).Where(x => x != null).ToList();

            if (arrStuImageId.Count == 0) { return "''"; }

            for (int i = 0; i < arrStuImageId.Count; i++)
            {
                string temp = arrStuImageId[i].ToString();
                allStuId += temp + ",";
            }
            return allStuId.Substring(0, allStuId.Length - 1);
        }
        public static List<StudentPhotoOrder> getNullBillingCodeOrderItems(PhotoSorterDBModelDataContext db)
        {
            return (from spo in db.StudentPhotoOrders where (spo.sp_SimplePhotoBillingCode == null || spo.sp_SimplePhotoBillingCode.Equals(string.Empty)) && spo.StudentImageId != null select spo).ToList();
        }
        public static List<int> getNullBillingCodeByOrderId(PhotoSorterDBModelDataContext db, int OrderId)
        {
            return (from spo in db.StudentPhotoOrders where spo.OrderId == OrderId && (spo.sp_SimplePhotoBillingCode == null || spo.sp_SimplePhotoBillingCode.Equals(string.Empty)) && spo.StudentImageId != null select spo.Id).ToList();
        }
        public static string getPackageItemById(PhotoSorterDBModelDataContext db, string billingCode)
        {
            string res = "";
            res = (from op in db.OrderPackages where op.SimplePhotoItemId == billingCode select op.Item).FirstOrDefault();
            if (res == null)
            {
                return "";
            }
            else
            {
                return res;
            }
        }
        public static int deleteOrdersById(PhotoSorterDBModelDataContext db, List<int> OrderIds)
        {
            //first we have to delete all the images in studentphotoorder where OrderId = OrderId
            //then delete from order table where Id = OrderId
            string allOrdersId = "";
            try
            {
                for (int i = 0; i < OrderIds.Count; i++)
                {
                    allOrdersId += OrderIds[i] + ",";
                }
                allOrdersId = allOrdersId.Substring(0, allOrdersId.Length - 1);

                db.ExecuteCommand("delete from StudentPhotoOrder where OrderId in (" + allOrdersId + ")");
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                return 0;
            }

            return db.ExecuteCommand("delete from Orders where Id in (" + allOrdersId + ")");
        }
        public static int updateOrderItemBillingCode(PhotoSorterDBModelDataContext db, int orderItemId, string billingCode)
        {
            return db.ExecuteCommand(
                "update StudentPhotoOrder set sp_SimplePhotoBillingCode='" + billingCode + "' where Id in (" + orderItemId + ")"
                );
        }

        public static OrderPackage getOrderPackageById(PhotoSorterDBModelDataContext db, int packageID)
        {
            return (from op in db.OrderPackages where op.Id == packageID select op).FirstOrDefault();
        }

        public static int UpdateOrder(PhotoSorterDBModelDataContext db, int OrderID, bool flag)
        {
            return db.ExecuteCommand("Update Orders set isSimplePhotoBillingCodeFilled = '" + flag + "'" + "where Id = " + OrderID);
        }
        public static View_Order getAllOrderItemsBillingCodes(PhotoSorterDBModelDataContext db, int? OrderID)
        {
            //string allOrderIds = string.Concat(",", OrderItemIDs.ToArray(typeof(int)) as int[]);

            return db.ExecuteQuery<View_Order>("select * from View_Order where (sp_SimplePhotoBillingCode is null or sp_SimplePhotoBillingCode = '') and OrderId = " + OrderID).FirstOrDefault();
        }
        //Bharat
        public static string getImageFolder(PhotoSorterDBModelDataContext db, int? photoShootId)
        {
            return (from ps in db.PhotoShoots where ps.PhotoShotID == photoShootId select ps.ImageFolder).FirstOrDefault();
        }
        public static void updateHasMissingImages(PhotoSorterDBModelDataContext db, bool value, int orderid)
        {
            db.ExecuteCommand("update Orders set HasMissingImages='" + value + "'  where Id = " + orderid
               );
        }
        public static void updateOrderRetouch(PhotoSorterDBModelDataContext db, bool value, int orderid)
        {
            db.ExecuteCommand("update Orders set Retouch='" + value + "'  where Id = " + orderid
               );
        }
        public static Order checkForOrderByDate(PhotoSorterDBModelDataContext db)
        {
            string query = "SELECT * FROM Orders where (DATEPART(yy, CreatedOn) = " + DateTime.Today.Year + " AND DATEPART(mm, CreatedOn) = " + DateTime.Today.Month + " AND DATEPART(dd, CreatedOn) = " + DateTime.Today.Day + ")"
                + "and OrderType = 'Manual Orders' order by CreatedOn desc";
            return db.ExecuteQuery<Order>(query).FirstOrDefault();
        }
        public static int updateOrderPackagePrice(PhotoSorterDBModelDataContext db, int orderPackageId, decimal packageCost)
        {
            return db.ExecuteCommand("Update OrderPackages set DefaultPrice = " + packageCost + " where Id = " + orderPackageId);
        }
        public static bool isRetouchTrue(PhotoSorterDBModelDataContext db, int OrderId, int stuImageId)
        {
            View_Order Retouch = db.ExecuteQuery<View_Order>("select * from View_Order where OrderId = " + OrderId + " and sp_SimplePhotoBillingCode = 'F135' and StudentImageId = " + stuImageId + " ").FirstOrDefault();

            if (Retouch != null)
            {
                return true;
            }
            return false;
        }
        public static List<int> isRichmondTrue(PhotoSorterDBModelDataContext db)
        {
            List<int> OrderIDs = db.ExecuteQuery<int>("select distinct OrderId from View_Order where sp_SimplePhotoBillingCode like '%R%'").ToList();

            return OrderIDs;
        }
        public static bool isManualOrder(PhotoSorterDBModelDataContext db, int OrderId)
        {
            View_Order isManual = db.ExecuteQuery<View_Order>("select * from Orders where Id = " + OrderId + " and OrderType = 'Manual Orders'").FirstOrDefault();

            if (isManual != null)
            {
                return true;
            }
            return false;
        }
        public static bool isStandardTrue(PhotoSorterDBModelDataContext db, int OrderId, int stuImageId)
        {
            View_Order Retouch = db.ExecuteQuery<View_Order>("select * from View_Order where OrderId = " + OrderId + " and sp_SimplePhotoBillingCode = 'M134'  and StudentImageId = " + stuImageId + " ").FirstOrDefault();

            if (Retouch != null)
            {
                return true;
            }
            return false;
        }
        public static bool getOrderDetails(PhotoSorterDBModelDataContext db, int OrderId)
        {
            Order tempOrder = db.ExecuteQuery<Order>("select * from Orders where SimplePhotoOrderId = " + OrderId).FirstOrDefault();

            if (tempOrder != null)
            {
                return true;
            }
            return false;
        }
        public static Order getOrderDetailsByOrderId(PhotoSorterDBModelDataContext db, int OrderId)
        {
            Order tempOrder = db.ExecuteQuery<Order>("select * from Orders where Id = " + OrderId).FirstOrDefault();

            if (tempOrder != null)
            {
                return tempOrder;
            }
            return null;
        }
        public static bool getOrderItemDetailsById(PhotoSorterDBModelDataContext db, int OrderId)
        {
            View_Order tempOrder = db.ExecuteQuery<View_Order>("select * from View_Order where OrderId = " + OrderId).FirstOrDefault();

            if (tempOrder != null)
            {
                return true;
            }
            return false;
        }
        public static List<OrdersImport> getOrderImportBatchs(PhotoSorterDBModelDataContext db)
        {
            //return (from Oi in db.OrdersImports orderby Oi.CreatedOn descending select Oi).ToList();
            return (from Oi in db.OrdersImports where Oi.OrderType == "Manual" orderby Oi.CreatedOn descending select Oi).ToList();
        }

        public static List<Student> studentsInOrders(PhotoSorterDBModelDataContext db)
        {
            string query = "select * from Student where id in( select distinct Studentidpk from StudentImage si join StudentPhotoOrder spo on si.ID = spo.StudentImageId)";
            return db.ExecuteQuery<Student>(query).ToList();
        }

        public static List<Order> ordersForStudent(PhotoSorterDBModelDataContext db, int studentId)
        {
            string query = "select * from Orders where id in(select distinct OrderId from StudentImage si join StudentPhotoOrder spo on si.ID = spo.StudentImageId where si.StudentIDPK=" + studentId + ")";
            return db.ExecuteQuery<Order>(query).ToList();
        }

        public static bool updateOrderHasNotes(PhotoSorterDBModelDataContext db, int orderid)
        {
            View_Order HasNotes = db.ExecuteQuery<View_Order>("select * from View_Order where (Comments != null or Comments != '') and OrderId = " + orderid).FirstOrDefault();
            if (HasNotes != null)
            {
                string q = "update Orders set HasNotes= 1  where Id = " + orderid;
                db.ExecuteCommand(q);
                return true;
            }

            return false;
        }
        public static List<Order> getOrdersByStudentImageId(PhotoSorterDBModelDataContext db, int studentImageId)
        {
            string query = "select * from orders where id in ( select distinct OrderId from StudentPhotoOrder where StudentImageId = " + studentImageId + ")";
            return db.ExecuteQuery<Order>(query).ToList();
        }
        public static List<StudentImage> getStudedentImagesNotinStudentPhotoOrder(PhotoSorterDBModelDataContext db, List<int> PhotoShootIds, int StudentId)
        {
            string tempIds = string.Join(",", PhotoShootIds);

            string query = "select distinct si.* from StudentImage si where  si.PhotoShootID in (" + tempIds + ") and si.StudentIDPK in (" + StudentId + ") and si.ID not in " +
                     "(select sii.id from StudentImage sii join StudentPhotoOrder spo on spo.StudentImageId=sii.id) ";

            return db.ExecuteQuery<StudentImage>(query).ToList();

        }
        public static List<Order> getStudentPhotoOrdersByGrpNameImportBatchBillingCode(PhotoSorterDBModelDataContext db, string billingCodes, int importBatchId, string groupName, string firstName, string lastName)
        {
            //string query = "select spo.* from StudentPhotoOrder spo left outer join Orders O on " +
            //                                            "O.ID = spo.OrderId join StudentImage si on si.ID = spo.StudentImageId " +
            //                                            "join Student s on s.Id = si.StudentIDPK " +
            //                                            "where spo.sp_SimplePhotoBillingCode in(" + billingCodes + ") and o.OrdersImportId in (" + importBatchId + ") and spo.sp_GroupName in ('" + groupName + "')";
            string query = "select * from Orders where Id in (select spo.OrderId from StudentPhotoOrder spo left outer join Orders O on " +
                                                        "O.ID = spo.OrderId join StudentImage si on si.ID = spo.StudentImageId " +
                                                        "join Student s on s.Id = si.StudentIDPK " +
                                                        "where spo.sp_SimplePhotoBillingCode in(" + billingCodes + ") and o.OrdersImportId in (" + importBatchId + ") and spo.sp_GroupName in ('" + groupName + "') and s.FirstName like '" + firstName + "' and s.Lastname like '" + lastName + "')";
            return db.ExecuteQuery<Order>(query).ToList();
        }

        //public static DataTable getGetAllOrderItems(PhotoSorterDBModelDataContext db, ArrayList selectedOrderIds)
        //{
        //    string allOrderIds = "";
        //    for (int i = 0; i < selectedOrderIds.Count; i++)
        //    {
        //        allOrderIds += selectedOrderIds[i] + ",";
        //    }
        //    allOrderIds = allOrderIds.Substring(0, allOrderIds.Length - 1);

        //    //return (db.ExecuteQuery<View_Order>("select * from View_Order where OrderId in (" + allOrderIds + ")").ToList());
        //    return WCFSQLHelper.getDataTable("select o.Id as OrderID, so.sp_Name as Contact, o.Ship_Address as Address1, o.Cust_Address as Address2, o.Ship_Phone as Phone, o.Ship_City as City, o.Ship_State as State, o.Ship_PostalCode as PostalCode, so.StudentImageId as ItemID, so.sp_ItemDescription as ItemDescription, so.Quantity as ItemQuantity, so.sp_SimplePhotoBillingCode as totalWeight, o.TrackingNumber as TrackingNumber, o.isShipped as isShipped from Orders o left join StudentPhotoOrder so on so.OrderId = o.Id where o.Id in (" + allOrderIds + ")");
        //    //return WCFSQLHelper.getDataTable("select o.Id as OrderID, o.Title as Company, so.sp_Name as Contact, o.Ship_Address as Address1, o.Cust_Address as Address2, o.Ship_Phone as Phone, o.Ship_City as City, o.Ship_State as State, o.Ship_PostalCode as PostalCode, so.StudentImageId as ItemID, so.sp_ItemDescription as ItemDescription, so.Quantity as ItemQuantity, so.sp_PriceListName as ItemPrice, so.sp_SimplePhotoBillingCode as totalWeight, o.OrderTotal as TrackingNumber, o.IsExported as isShipped from Orders o left join StudentPhotoOrder so on so.OrderId = o.Id where o.Id in (" + allOrderIds + ")");
        //}
        public static DataTable getOrderDetailsbyOrderID(PhotoSorterDBModelDataContext db, ArrayList selectedOrderIds)
        {
            string allOrderIds = "";
            for (int i = 0; i < selectedOrderIds.Count; i++)
            {
                allOrderIds += selectedOrderIds[i] + ",";
            }
            allOrderIds = allOrderIds.Substring(0, allOrderIds.Length - 1);

            //return WCFSQLHelper.getDataTable("select o.Id as OrderID,o.SimplePhotoOrderID as SimplePhotoOrderID, o.Ship_FirstName, o.Ship_LastName, o.Ship_Address as Address1, o.Ship_Phone as Phone, o.Ship_City as City, o.Ship_State as State, o.Ship_PostalCode as PostalCode, o.TrackingNumber as TrackingNumber, o.isShipped as isShipped from Orders o where o.Id in (" + allOrderIds + ")");
            return WCFSQLHelper.getDataTable("select o.Id as OrderID,o.SimplePhotoOrderID as SimplePhotoOrderID, o.Ship_FirstName, o.Ship_LastName, o.Ship_Address as Address1, o.Ship_Phone as Phone, o.Ship_City as City, o.Ship_State as State, o.Ship_PostalCode as PostalCode, o.TrackingNumber as TrackingNumber, o.isShipped as isShipped, o.Cust_Email as EmailAddress from Orders o where o.Id in (" + allOrderIds + ")");
        }

        public static List<Student> getStudentsHaventOrdered(PhotoSorterDBModelDataContext db, int schoolId)
        {
            //For specified school
            //return db.ExecuteQuery<Student>("select * from Student s join StudentImport si on s.StudentImportID = si.ID where si.SchoolID = "+ schoolId +"and s.ID not in " +
            //     "(select distinct StudentIDPK from StudentImage where ID in (select distinct StudentImageId from StudentPhotoOrder where SchoolId = " + schoolId + ")) order by s.ID").ToList();


            return db.ExecuteQuery<Student>("select * from Student where ID not in (select distinct StudentIDPK from StudentImage where ID in (select distinct StudentImageId from studentPhotoOrder))").ToList(); 
        }
        public static void updateOrderStandardRetouch(PhotoSorterDBModelDataContext db, bool value, int orderid)
        {
            db.ExecuteCommand("update Orders set IsStandardRetouch='" + value + "'  where Id = " + orderid
               );
        }
    }
}
