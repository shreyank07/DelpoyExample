using ProdigyFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    public class DoubleToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int precision = 0;
            if (value == DependencyProperty.UnsetValue || value == null)
                return "0";
            if (parameter == null || parameter == DependencyProperty.UnsetValue)
                precision = 0;

            else if (!int.TryParse(parameter.ToString(), out precision))
                precision = 0;

            double strvalue = System.Convert.ToDouble(value);
            return strvalue.FormatNumber(precision);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value.ToString().FormatStringToNumber();
            }
            catch
            {
                return value;
            }
        }
    }
}
