using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace PhotoForce.WorkPlace
{
    //public class ColorValueConverter : MarkupExtension, IValueConverter
    //{
    //    DateTime? tempDate = null;
    //    string tempStatus = "";

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        //return value is string && (string)value == "+" ? Brushes.Green : Brushes.Red;

    //        if (value is string)
    //            tempStatus = (string)value;
    //        else
    //            tempDate = (DateTime)value;

    //        if (!string.IsNullOrEmpty(tempStatus) && tempDate != null)
    //        {
    //            if (tempStatus == "Completed" && DateTime.Today.Date <= tempDate)
    //            {
    //                tempDate = null; tempStatus = "";
    //                return Brushes.Green;
    //            }
    //            else if ((tempStatus == "Started" || tempStatus == "Not Started") && DateTime.Today.Date <= tempDate)
    //            {
    //                tempDate = null; tempStatus = "";
    //                return Brushes.White;
    //            }
    //            else if ((tempStatus == "Started" || tempStatus == "Not Started") && DateTime.Today.Date > tempDate)
    //            {
    //                tempDate = null; tempStatus = "";
    //                return Brushes.Red;
    //            }
    //            else
    //                return Brushes.Transparent;
    //        }
    //        else
    //        {
    //            if (!string.IsNullOrEmpty(tempStatus))
    //            {
    //                if (tempStatus == "Completed")
    //                    return Brushes.Green;
    //                else if (tempStatus == "Started")
    //                    return Brushes.White;
    //                else if (tempStatus == "Not Started")
    //                    return Brushes.Red;
    //            }
    //             return Brushes.Transparent;
    //        }
            
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        return this;
    //    }
    //}


    public class ColorValueConverter1 : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values[0] == DependencyProperty.UnsetValue) { return Brushes.Transparent; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            DateTime? tempDate = (values[0] is DateTime ) ? values[0] as DateTime? : values[1] as DateTime?;
            string status = (values[0] is string) ? values[0] as string : values[1] as string;

            if (status == "Completed" && DateTime.Today.Date <= tempDate)
                return Brushes.Green;
            else if ((status == "Started" || status == "Not Started") && DateTime.Today.Date <= tempDate)
                return Brushes.White;
            else if ((status == "Started" || status == "Not Started") && DateTime.Today.Date > tempDate)
                return Brushes.Red;
            else
                return Brushes.Transparent;


            //int? first = values[0] as int?;
            //int? second = values[1] as int?;
            //if (first == null || second == null) return Brushes.Black;
            //return first > second ? Brushes.Red : Brushes.Green;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
