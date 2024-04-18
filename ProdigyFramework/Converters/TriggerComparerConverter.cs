using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    public class TriggerComparerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isSet = false;
            if (values[0] == null || values[0] == DependencyProperty.UnsetValue 
                || values[1] == null || values[1] == DependencyProperty.UnsetValue
                || values[2] == null || values[2] == DependencyProperty.UnsetValue)
                return isSet;

            if (values[0].ToString().ToUpper().Trim() != "AUTO")
            {
                if (Math.Abs(System.Convert.ToDouble(values[1]) - System.Convert.ToDouble(values[2])) <= 2E-6)
                    isSet = true;
            }
            return isSet;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
