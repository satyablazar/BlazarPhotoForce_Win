using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhotoForce.WorkPlace
{
    public class IntToEnableConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool tempEnable;
            if ((int)value != 1)
            {
                tempEnable = false;
            }
            else
            {
                tempEnable = true;
            }
            return tempEnable;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    
    public class IntToEnableConveter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool tempEnable;
            if ((int)value != 1)
            {
                tempEnable = true;
            }
            else
            {
                tempEnable = false;
            }
            return tempEnable;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    //public class StringToBoolConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        bool tempEnable;
    //        if ((string)value != null)
    //        {
    //            tempEnable = true;
    //        }
    //        else
    //        {
    //            tempEnable = false;
    //        }
    //        return tempEnable;
    //    }
    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        return null;
    //    }
    //}
    
}
