using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhotoForce.WorkPlace
{
    public class GridFooterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            string totalRowsCount = values[0].ToString();
            string selectedRowsCount = values[1].ToString();

            return totalRowsCount + " Rows (" + selectedRowsCount + " selected)";
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
