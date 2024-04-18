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
    public class BytesToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string charSeparator = string.Empty;
            if (value == DependencyProperty.UnsetValue || value == null)
                return DependencyProperty.UnsetValue;
            if (parameter != null || parameter != DependencyProperty.UnsetValue)
                charSeparator = parameter.ToString();

            return ProdigyFramework.Extension.FrameworkExtensions.GetHexString((byte[])value, charSeparator);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string charSeparator = string.Empty;
            if (value == DependencyProperty.UnsetValue || value == null)
                return DependencyProperty.UnsetValue;
            if (parameter != null || parameter != DependencyProperty.UnsetValue)
                charSeparator = parameter.ToString();
            return ProdigyFramework.Extension.FrameworkExtensions.GetBytes(value.ToString(), charSeparator);
        }
    }
}
