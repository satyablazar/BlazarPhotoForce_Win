using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhotoForce.WorkPlace
{
    public class GeneratePDFMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            bool totalRowsCount = (bool)values[0];
            int selectedRowsCount = (int)values[1];

            return totalRowsCount + " Rows (" + selectedRowsCount + " selected)";
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
