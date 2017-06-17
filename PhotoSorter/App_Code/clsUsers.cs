using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsUsers
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

        public static User checkForUserName(PhotoSorterDBModelDataContext db, string tempUserName)
        {
            try
            {
                return db.ExecuteQuery<User>("select * from Users where UserName = '" + tempUserName + "'").FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        public static List<User> getAllUsers(PhotoSorterDBModelDataContext db)
        {
            try
            {
                return (from alu in db.Users select alu).ToList();
            }
            catch (Exception)
            {
                return null;
            }

        }
        //public static int deleteStudio(PhotoSorterDBModelDataContext db, ArrayList selectedIds)
        //{
        //    string allUserIds = convertArrayListToString(selectedIds);
        //    return db.ExecuteCommand("delete Studio where Id in (" + allUserIds + ")");
        //}
    }
}
