using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class EnumToBooleanReturnDefaultConverter : IValueConverter
    {
        public object DefaultValueWhenFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            // You could also directly pass an enum value using {x:Static},
            // then there is no need to parse
            string parameterString = parameter.ToString();
            if (parameterString == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return DependencyProperty.UnsetValue;
            }

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (System.Convert.ToBoolean(value) == false)
                return DefaultValueWhenFalse;

            string parameterString = parameter.ToString();

            if (Enum.IsDefined(targetType, parameterString) == false)
            {
                return DependencyProperty.UnsetValue;
            }

            return Enum.Parse(targetType, parameterString);
        }
    }
}
