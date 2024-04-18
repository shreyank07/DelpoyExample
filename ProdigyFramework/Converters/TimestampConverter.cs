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
    public class TimestampConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int precision = 4;
            if (values[0] == DependencyProperty.UnsetValue || values[0] == null
                || values[1] == DependencyProperty.UnsetValue || values[1] == null)
                return DependencyProperty.UnsetValue;
            if (parameter == null || parameter == DependencyProperty.UnsetValue)
                precision = 4;
            else if (!int.TryParse(parameter.ToString(), out precision))
                precision = 4;

            double time = System.Convert.ToDouble(values[0]);
            double triggerTime = System.Convert.ToDouble(values[1]);

            if (triggerTime < 0)
                triggerTime = 0;

            double strvalue = time - triggerTime;
            return CUtilities.ToEngineeringNotation(strvalue , precision) + "s";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
