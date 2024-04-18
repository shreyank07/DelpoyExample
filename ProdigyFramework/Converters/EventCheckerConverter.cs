using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    public class EventCheckerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == null || values[0] == DependencyProperty.UnsetValue
                || values[1] == null || values[1] == DependencyProperty.UnsetValue
                || parameter == null)
                return DependencyProperty.UnsetValue;
            bool comparerResult = false;
            int val1 = System.Convert.ToInt32(values[0]);
            int val2 = System.Convert.ToInt32(values[1]);
            switch (parameter.ToString().ToUpper())
            {
                case "AND":
                    comparerResult = (val1 & val2) == val2;
                    break;
                case "OR":
                    comparerResult = (val1 | val2) == val2;
                    break;
                default:
                    comparerResult = (val1 & val2) == val2;
                    break;
            }
            return comparerResult;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
