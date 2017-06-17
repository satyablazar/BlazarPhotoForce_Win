using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.Extensions
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PaymentMethods
    {
        Check ,
        Dollar,
        [Description("VMC/Am/Ex")]
        VMCAmEx
    }

    public enum OrderTypeInOrdersImport
    {
        Excel,
        Auto,
        Manual
    }

    //public enum OrderTypeInOrders
    //{
    //    [Description("Manual Orders")]
    //    ManualOrders,
    //    [Description("Simple Photo Order")]
    //    SimplePhotoOrder 
    //}
    public enum ConnectionState
    {
        OpenDB,
        UpgradeDB,
        Cancel
    }
    public enum AddOrReplaceCollectionItems
    {
        AddItems,
        RemoveItems
    }
    public enum StudentType
    {
        Student,
        Staff,
        StaffGroup,
        Class,
        Family,
        Siblings,
        Club,
        Team,
        Player,
        Misc,
    }
    public enum Season
    {
        Fall,
        Spring,
    }
}
