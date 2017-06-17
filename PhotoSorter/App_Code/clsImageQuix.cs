using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.App_Code
{
    public static class clsImageQuix
    {
        public static List<IQPriceSheet> getAllIQPriceSheets(PhotoSorterDBModelDataContext db, int tempIQAccountId)
        {
            //return db.ExecuteQuery<IQPriceSheet>("select * from IQPriceSheet where IQAccountId = " + tempIQAccountId).ToList();

            return (from IQP in db.IQPriceSheets where IQP.IQAccountId == tempIQAccountId select IQP).ToList();
        }

        public static int deleteIQAccouts(PhotoSorterDBModelDataContext db, List<int> tempIQAccountIds)
        {
            string accoundIds = string.Join(",", tempIQAccountIds);
            db.ExecuteCommand("delete IQPriceSheet  where IQAccountId in (" + accoundIds + ")");

            db.ExecuteCommand("delete IQVandoSettings  where IQAccountId in (" + accoundIds + ")");

            return db.ExecuteCommand("delete IQAccounts where Id in (" + accoundIds + ")");
        }
        public static int deletePricesheets(PhotoSorterDBModelDataContext db, List<int> tempIQPricesheetIds, int tempIQAccountId )
        {
            string pricesheetIds = string.Join(",", tempIQPricesheetIds);
            return db.ExecuteCommand("delete IQPriceSheet  where Id in (" + pricesheetIds + ") and IQAccountId = " + tempIQAccountId);
        }
        public static int deleteVandoSettings(PhotoSorterDBModelDataContext db, List<int> tempIQVandoSettingIds, int tempIQAccountId)
        {
            string vandoSettingIds = string.Join(",", tempIQVandoSettingIds);
            return db.ExecuteCommand("delete IQVandoSettings where Id in (" + vandoSettingIds + ") and IQAccountId = " + tempIQAccountId);
        }
        public static int updateIQVandoSettings(PhotoSorterDBModelDataContext db, int tempIQVandoSettingId, int tempIQAccountId)
        {
            return db.ExecuteCommand("update IQVandoSettings set IsDefault = 0 where Id !=" + tempIQVandoSettingId + "and IQAccountId = " + tempIQAccountId);
        }
    }
}
