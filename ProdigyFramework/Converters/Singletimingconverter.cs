using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ProdigyFramework.Helpers;

namespace ProdigyFramework.Converters
{
   public class Singletimingconverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int precision = 0;
            if (value == null || value == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;
            if (parameter == null || parameter == DependencyProperty.UnsetValue)
                precision = 0;

            else if (!int.TryParse(parameter.ToString(), out precision))
                precision = 0;

            double strvalue = System.Convert.ToDouble(value) / 1e9;
            return CUtilities.ToEngineeringNotation(strvalue) +"s";
            //if (!(strvalue > 10))
            //    return string.Format("{0:#,###.####}s", strvalue.FormatNumber(precision));
            //else
            //    return string.Format("{0:#,###.####}s", strvalue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
