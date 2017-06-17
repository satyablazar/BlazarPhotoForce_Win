using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace PhotoForce.WorkPlace
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility tempVisibility;
            if ((bool)value)
            {
                tempVisibility = Visibility.Visible;
            }
            else
            {
                tempVisibility = Visibility.Collapsed;
            }
            return tempVisibility;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    //used to hide photo preview in Dashboard
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility tempVisibility;
            if ((int)value == 0)
            {
                tempVisibility = Visibility.Visible;
            }
            else
            {
                tempVisibility = Visibility.Collapsed;
            }
            return tempVisibility;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


    //used to collapse when dockpanel layout visible
    //used in MainWindow.Xaml
    public class TabControlBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility tempVisibility;
            if ((bool)value)
            {
                tempVisibility = Visibility.Collapsed;
            }
            else
            {
                tempVisibility = Visibility.Visible;
            }
            return tempVisibility;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    //used to hide buttons(new,seach,drag,refresh,bulkrename.....) when universal student is visible.
    public class BoolToIsVisibileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool tempVisibility;
            if ((bool)value)
            {
                tempVisibility = false;
            }
            else
            {
                tempVisibility = true;
            }
            return tempVisibility;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    //to show groups related icons only when photoshoots and groups are visible.
    //public class BoolToIsVisibileConverter2 : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType,
    //        object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        bool photoshootVisibility = (bool)values[0];
    //        string groupVisibility = values[1].ToString().ToLower();

    //        if ((string)values[2] == "IsVisible")
    //        {
    //            if (photoshootVisibility == true || groupVisibility == "true")
    //            {
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        }
    //        else
    //        {
    //            //dashBoardVisibility == true || groupVisibility == "true" ||
    //            if (photoshootVisibility == true || groupVisibility == "true" || values[3].ToString().ToLower() == "true" || values[4].ToString().ToLower() == "true")
    //            {
    //                return false;
    //            }
    //            else
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes,
    //        object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        return null;
    //    }
    //}

    //to show groups related icons only when photoshoots and groups are visible.
    public class BoolToIsVisibileConverter3 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            bool isUnset = CheckForUnset.checkForUnsetValue(values);
            if (isUnset) { return false; }

            bool dashBoardVisibility = (bool)values[0];
            bool groupVisibility = (bool)values[1];
            bool studentVisibility = (bool)values[2];
            bool ordersVisibility = (bool)values[3];

            if (dashBoardVisibility == true || groupVisibility == true || studentVisibility == true || ordersVisibility == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    //used in MainWindow to show/hide group photos and add to orders buttons
    public class BoolToVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            bool isUnset = CheckForUnset.checkForUnsetValue(values);
            if(isUnset){return false;}

            bool res = false;
            bool groupsVisibility = (bool)values[0];
            bool ordersVisibility = (bool)values[1];
            bool viewOrderByGalleryGroupVisible = (bool)values[2];
            bool searchOrdersVisiblity = (bool)values[3];
            string buttonName = (string)values[4];

            if (buttonName == "AddToOrders")
            {
                //CreateManulOrdersViewModel _obj = new CreateManulOrdersViewModel();
                res = (ordersVisibility == true || viewOrderByGalleryGroupVisible == true) ? true : false;
            }
            else if (buttonName == "ExportPhotos")
            {
                res = (ordersVisibility == true || groupsVisibility == true || viewOrderByGalleryGroupVisible == true || searchOrdersVisiblity == true) ? true : false;
            }
            return res;
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public static class CheckForUnset
    {
        public static bool checkForUnsetValue(object[] values)
        {
            foreach (object item in values)
            {
                if (item == DependencyProperty.UnsetValue)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
